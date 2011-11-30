// (c) Kyle Sabo 2011

using System;
using System.IO;
using System.Windows;

namespace mangle_port
{
    public class FileConversionInfo
        : DependencyObject, IDisposable
    {
        private string inputFilePath;
        private string outputPath;
        private Stream inputStream;
        private bool disposed;

        // DisplayName property
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            "DisplayName",
            typeof(string),
            typeof(FileConversionInfo),
            new PropertyMetadata(string.Empty));

        public string DisplayName
        {
            get
            {
                return (string)GetValue(DisplayNameProperty);
            }
            set
            {
                SetValue(DisplayNameProperty, value);
            }
        }

        // OutputName property
        public static readonly DependencyProperty OutputNameProperty = DependencyProperty.Register(
            "OutputName",
            typeof(string),
            typeof(FileConversionInfo),
            new PropertyMetadata(string.Empty));

        public string OutputName
        {
            get
            {
                return (string)GetValue(OutputNameProperty);
            }
            set
            {
                SetValue(OutputNameProperty, value);
            }
        }

        public string InputFilePath
        {
            get
            {
                return this.inputFilePath;
            }
            set
            {
                this.inputFilePath = value;
            }
        }

        public string OutputPath
        {
            get
            {
                return this.outputPath;
            }
            set
            {
                this.outputPath = value;
            }
        }

        public Stream FileStream
        {
            get
            {
                return this.inputStream;
            }
            set
            {
                this.inputStream = value;
            }
        }

        public FileConversionInfo()
        {
            this.disposed = false;
            this.inputStream = null;
            this.inputFilePath = string.Empty;
            this.outputPath = string.Empty;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~FileConversionInfo()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing && this.inputStream != null)
                {
                    // Dispose managed resources.
                    this.inputStream.Dispose();
                    this.inputStream = null;
                }

                // Dispose unmanaged resources.
                // <none>

                this.disposed = true;
            }
        }
    }
}
