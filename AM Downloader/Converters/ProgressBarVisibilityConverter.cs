﻿// Copyright (C) 2020-2021 Antik Mozib.

using AMDownloader.ObjectModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AMDownloader
{
    internal class ProgressBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DownloadStatus status = (DownloadStatus)value;
            switch (status)
            {
                case DownloadStatus.Queued:
                case DownloadStatus.Ready:
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}