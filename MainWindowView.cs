// (c) Kyle Sabo 2011

using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace mangle_port
{
    public partial class MainWindowView : MetroContentControl
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Link;
            }

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> filesToAdd = new List<string>();
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    IEnumerable<string> jpgs = Directory.EnumerateFiles(file, "*.jpg", SearchOption.AllDirectories);
                    IEnumerable<string> pngs = Directory.EnumerateFiles(file, "*.png", SearchOption.AllDirectories);

                    filesToAdd.AddRange(jpgs.Union(pngs));
                }
                else if (File.Exists(file))
                {
                    filesToAdd.Add(file);
                }
            }

            if (filesToAdd.Any())
            {
                IOrderedEnumerable<string> toAdd = filesToAdd.OrderBy(x => x);

                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    foreach (string file in toAdd)
                    {
                        ((MainWindowViewModel)this.DataContext).AddFileToList(file);
                    }
                }));

                e.Effects = DragDropEffects.Link;
            }

            e.Handled = true;
        }
    }
}
