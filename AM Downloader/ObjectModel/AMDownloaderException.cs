﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AMDownloader.ObjectModel
{
    class AMDownloaderException : Exception
    {
        public AMDownloaderException()
        {
        }

        public AMDownloaderException(string message)
            : base(message)
        {
        }

        public AMDownloaderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
