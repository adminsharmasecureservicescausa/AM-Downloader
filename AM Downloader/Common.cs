﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Data;

namespace AMDownloader
{
    static class Common
    {
        public const long KILOBYTE = 1024;
        public const long MEGABYTE = KILOBYTE * KILOBYTE;
        public const long GIGABYTE = MEGABYTE * KILOBYTE;
        public const long TERABYTE = GIGABYTE * KILOBYTE;

        public static class ApplicationPaths
        {
            public static string DownloadsHistory { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name, "history"); } }
            public static string SavedLocationsHistory { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name, "downloadpaths"); } }
            public static string DownloadsFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); } }
        }

        public static string PrettifySize(long? num)
        {
            if (num == null)
            {
                return string.Empty;
            }

            double result = 0;
            double.TryParse(num.ToString(), out result);

            if (result > GIGABYTE)
            {
                result = Math.Round(result / GIGABYTE, 3);
                return result.ToString("#0.000") + " GB";
            }
            else if (result > MEGABYTE)
            {
                result = Math.Round(result / MEGABYTE, 2);
                return result.ToString("#0.00") + " MB";
            }
            else if (result > KILOBYTE)
            {
                result = Math.Round(result / KILOBYTE, 0);
                return result.ToString() + " KB";
            }
            else
            {
                return result.ToString() + " B";
            }
        }

        public static string GetValidFilename(string defaultFilename)
        {
            string path = Path.GetDirectoryName(defaultFilename);
            string filename = Path.GetFileName(defaultFilename);
            string result = path + Path.DirectorySeparatorChar + filename;
            int i = 0;

            while (File.Exists(result))
            {
                result = path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + " (" + ++i + ")" + Path.GetExtension(filename);
            };

            return result;
        }

        public static string PrettifySpeed(long? speed)
        {
            if (speed > 1024)
            {
                return ((double)speed / 1024).ToString("#0.00") + " MB/s";
            }
            else
            {
                if (speed == null)
                {
                    return string.Empty;
                }
                else
                {
                    return speed.ToString() + " KB/s";
                }
            }
        }
    }
}
