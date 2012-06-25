// (c) Kyle Sabo 2011

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace mangle_port
{
    /// <summary>
    /// Interaction logic for ProgressBarDialog.xaml
    /// </summary>
    public partial class ProgressBarDialog : Window
    {
        public ProgressBarDialog()
        {
            InitializeComponent();
        }

        public delegate void ProgressDelegate();

        private int finishedThreads;

        public int TotalThreads
        {
            get;
            set;
        }

        public CancellationTokenSource CancellationTokenSource
        {
            get;
            set;
        }

        private int progress;

        public int Progress
        {
            get
            {
                return progress;
            }
            set{
                progress = Interlocked.Add(ref finishedThreads, 1);
                progress = (int)Math.Round(((double)finishedThreads / (double)TotalThreads) * 100);
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ProgressDelegate(() => progressBar.Value = progress));

                if (finishedThreads >= TotalThreads)
                {
                    Debug.Assert(finishedThreads == TotalThreads);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ProgressDelegate(() => this.Close()));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.CancellationTokenSource.Cancel();
            this.Close();
        }
    }
}
