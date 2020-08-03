﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace AMDownloader
{
    // Represents contracts that can be used by the QueueProcessor
    public interface IQueueable
    {
        public Task StartAsync(int numStreams); // numStreams = max num of parallel streams per IQueueable
        public void Pause();
        public bool IsQueued { get; }
    }

    class QueueProcessor
    {
        #region Fields
        private const int DEFAULT_MAX_PARALLEL_DOWNLOADS = 2;
        private readonly SemaphoreSlim _semaphore;
        private readonly List<IQueueable> _itemsProcessing;
        private List<IQueueable> _itemsUnprocessed;
        private BlockingCollection<IQueueable> _queueList;
        private CancellationTokenSource _ctsCancel;
        private CancellationToken _ctCancel;
        #endregion // Fields

        #region Properties
        public bool IsBusy { get { return (_ctsCancel != null); } }
        #endregion // Properties

        #region Constructors

        public QueueProcessor(int maxParallelDownloads = DEFAULT_MAX_PARALLEL_DOWNLOADS)
        {
            _queueList = new BlockingCollection<IQueueable>();
            _itemsProcessing = new List<IQueueable>();
            _semaphore = new SemaphoreSlim(maxParallelDownloads);
        }
        #endregion // Constructors

        #region Private methods
        private async Task ProcessQueueAsync(int numStreams)
        {
            var tasks = new List<Task>();

            _ctsCancel = new CancellationTokenSource();
            _ctCancel = _ctsCancel.Token;
            _itemsProcessing.Clear();

            while (_queueList.Count() > 0 && !_ctCancel.IsCancellationRequested)
            {
                IQueueable item;
                if (!_queueList.TryTake(out item)) break;

                if (!item.IsQueued) continue;
                
                _itemsUnprocessed.Add(item);
                Task t = Task.Run(async () =>
                {
                    _itemsProcessing.Add(item);
                    _semaphore.Wait();

                    if (!_ctCancel.IsCancellationRequested && item.IsQueued)
                    {
                        await item.StartAsync(numStreams);
                        _itemsUnprocessed.Remove(item);
                    }

                    _semaphore.Release();
                    _itemsProcessing.Remove(item);
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());

            _ctsCancel = null;
            _ctCancel = default;
        }

        private void RecreateQueue(params IQueueable[] itemsOnTop)
        {
            _ctsCancel?.Cancel();

            var tempList = new BlockingCollection<IQueueable>();

            foreach (var item in itemsOnTop)
                tempList.Add(item);

            while (_queueList.Count > 0)
            {
                IQueueable item;

                if (_queueList.TryTake(out item))
                {
                    if (itemsOnTop.Contains(item))
                        continue;
                    else
                        tempList.Add(item);
                }
                else
                {
                    break;
                }
            }

            var disposeList = _queueList;
            _queueList = tempList;
            disposeList.Dispose();
        }
        #endregion // Private methods

        #region Public methods
        // Producer
        public void Add(IQueueable item)
        {
            if (_queueList.Contains(item)) return;

            try
            {
                _queueList.TryAdd(item);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        // Consumer
        public async Task StartAsync(int numStreams = 5, params IQueueable[] firstItems)
        {
            if (_ctsCancel != null) return;
            if (firstItems != null) RecreateQueue(firstItems);

            _itemsUnprocessed = new List<IQueueable>();
            await ProcessQueueAsync(numStreams);

            if (_ctCancel.IsCancellationRequested && _itemsProcessing.Count > 0)
            {
                // items that are being downloaded were taken out of queue; add them back to the top          
                foreach(var item in _itemsProcessing)
                {
                    item.Pause();
                }
            }
            foreach(var item in _itemsUnprocessed)
            {
                this.Add(item);
            }
        }

        public void Stop()
        {
            _ctsCancel?.Cancel();
        }
        #endregion // Public methods

        #region Public functions
        public bool Contains(IQueueable value)
        {
            return (_queueList.Contains(value));
        }
        public int Count()
        {
            return _queueList.Count;
        }
        #endregion // Public functions
    }
}