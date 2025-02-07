﻿// Copyright (C) Antik Mozib. All rights reserved.

using System.Threading.Tasks;

namespace AMDownloader.QueueProcessing
{
    internal interface IQueueable
    {
        public Task StartAsync();

        public void Pause();

        public bool IsQueued { get; }
        public bool IsCompleted { get; }
    }
}