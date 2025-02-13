﻿// Copyright (C) Antik Mozib. All rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AMDownloader
{
    internal class StatusBarItemVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Trim().Length > 0) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}