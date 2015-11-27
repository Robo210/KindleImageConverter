// (c) Kyle Sabo 2011

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Ionic.Zip;
using Kindle.Profiles;

namespace mangle_port
{
    public class MainWindowViewModel : DependencyObject
    {
        private ObservableCollection<FileConversionInfo> fileList;

        private readonly CommandBindingCollection _CommandBindings;

        public CommandBindingCollection CommandBindings
        {
            get
            {
                return _CommandBindings;
            }
        }

        // Profiles property
        public static readonly DependencyProperty ProfilesProperty = DependencyProperty.Register(
            "Profiles",
            typeof(IEnumerable<KindleProfile>),
            typeof(MainWindowViewModel));

        public IEnumerable<KindleProfile> Profiles
        {
            get
            {
                return (IEnumerable<KindleProfile>)GetValue(ProfilesProperty);
            }
            private set
            {
                SetValue(ProfilesProperty, value);
            }
        }

        // SelectedProfile property
        public static readonly DependencyProperty SelectedProfileProperty = DependencyProperty.Register(
            "SelectedProfile",
            typeof(KindleProfile),
            typeof(MainWindowViewModel));

        public KindleProfile SelectedProfile
        {
            get
            {
                return (KindleProfile)GetValue(SelectedProfileProperty);
            }
            private set
            {
                SetValue(SelectedProfileProperty, value);
            }
        }

        // ImageFileList property
        public static readonly DependencyProperty ImageFileListProperty = DependencyProperty.Register(
            "ImageFileList",
            typeof(IEnumerable<FileConversionInfo>),
            typeof(MainWindowViewModel));

        public IEnumerable<FileConversionInfo> ImageFileList
        {
            get
            {
                return (IEnumerable<FileConversionInfo>)GetValue(ImageFileListProperty);
            }
            private set
            {
                SetValue(ImageFileListProperty, value);
            }
        }

        // SelectedImage property
        public static readonly DependencyProperty SelectedImageProperty = DependencyProperty.Register(
            "SelectedImage",
            typeof(FileConversionInfo),
            typeof(MainWindowViewModel),
            new PropertyMetadata((s, e) => ((MainWindowViewModel)s).OnSelectedImageChanged(e)));

        public void OnSelectedImageChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedImage != null)
            {
                this.RemoveFilesVisibility = Visibility.Visible;
            }
            else
            {
                this.RemoveFilesVisibility = Visibility.Collapsed;
            }
        }

        public FileConversionInfo SelectedImage
        {
            get
            {
                return (FileConversionInfo)GetValue(SelectedImageProperty);
            }
            private set
            {
                SetValue(SelectedImageProperty, value);
            }
        }

        // TODO: Rename to RemoveFilesIsVisible
        // RemoveFilesVisibilityProperty
        public static readonly DependencyProperty RemoveFilesVisibilityProperty = DependencyProperty.Register(
            "RemoveFilesVisibility",
            typeof(Visibility),
            typeof(MainWindowViewModel),
            new PropertyMetadata(Visibility.Collapsed));

        public Visibility RemoveFilesVisibility
        {
            get
            {
                return (Visibility)GetValue(RemoveFilesVisibilityProperty);
            }
            private set
            {
                SetValue(RemoveFilesVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty ExportIsEnabledProperty = DependencyProperty.Register(
            "ExportIsEnabled",
            typeof(bool),
            typeof(MainWindowViewModel));

        public bool ExportIsEnabled
        {
            get
            {
                return (bool)GetValue(ExportIsEnabledProperty);
            }
            set
            {
                SetValue(ExportIsEnabledProperty, value);
            }
        }

        public MainWindowViewModel()
        {
            this.fileList = new ObservableCollection<FileConversionInfo>();
            this.fileList.CollectionChanged += fileList_CollectionChanged;
            ImageFileList = this.fileList;

            this.Profiles = new ReadOnlyObservableCollection<KindleProfile>(new ObservableCollection<KindleProfile>(new KindleProfile[] { new Kindle3(), new KindlePaperWhite12(), new KindlePaperWhite3Voyage() }));
            this.SelectedProfile = this.Profiles.First();

            this._CommandBindings = new CommandBindingCollection();
            CommandBinding newBinding = new CommandBinding(ApplicationCommands.New, NewCmdExecuted, (s, e) => e.CanExecute = true);
            CommandBinding addFilesBinding = new CommandBinding(MainWindowCommands.AddFiles, AddFilesCmdExecuted, (s, e) => e.CanExecute = true);
            CommandBinding removeFilesBinding = new CommandBinding(MainWindowCommands.RemoveFiles, RemoveFilesCmdExecuted, RemoveFilesCanExecute);
            CommandBinding exportFilesBinding = new CommandBinding(MainWindowCommands.ExportFiles, ExportFilesCmdExecuted, ExportFilesCanExecute);
            CommandBinding moveUpBinding = new CommandBinding(ComponentCommands.MoveUp, MoveUpCmdExecuted, MoveCanExecute);
            CommandBinding moveDownBinding = new CommandBinding(ComponentCommands.MoveDown, MoveDownCmdExecuted, MoveCanExecute);
            this._CommandBindings.Add(newBinding);
            this._CommandBindings.Add(addFilesBinding);
            this._CommandBindings.Add(removeFilesBinding);
            this._CommandBindings.Add(exportFilesBinding);
            this._CommandBindings.Add(moveUpBinding);
            this._CommandBindings.Add(moveDownBinding);
        }

        private void fileList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.fileList.Count > 0)
            {
                this.ExportIsEnabled = true;
            }
            else
            {
                this.ExportIsEnabled = false;
            }
        }

        private string GetOutputName(int i)
        {
            return string.Format("{0:D3}.png", i);
        }

        public void AddFileToList(string filename)
        {
            string file = Path.GetFileName(filename);
            string extension = Path.GetExtension(filename);

            if (extension == ".zip")
            {
                using (ZipFile zip = ZipFile.Read(filename))
                {
                    var selection = from e in zip
                                    where (Path.GetExtension(e.FileName) == ".jpg" || Path.GetExtension(e.FileName) == ".png")
                                    select e;

                    foreach (var imageFile in selection)
                    {
                        file = Path.GetFileName(imageFile.FileName);
                        Stream stream = new MemoryStream();
                        imageFile.Extract(stream);
                        fileList.Add(new FileConversionInfo()
                            {
                                InputFilePath = imageFile.FileName,
                                DisplayName = file,
                                FileStream = stream,
                                OutputName = GetOutputName(fileList.Count)
                            });
                    }
                }
            }
            else if (extension == ".jpg" || extension == ".png")
            {
                fileList.Add(new FileConversionInfo()
                    {
                        InputFilePath = filename,
                        DisplayName = file,
                        OutputName = GetOutputName(fileList.Count)
                    });
            }
        }

        public void RemoveFileFromList(FileConversionInfo item)
        {
            item.Dispose();

            this.fileList.Remove(item);
        }

        void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            foreach (FileConversionInfo item in this.fileList)
            {
                item.Dispose();
            }

            this.fileList.Clear();
            GC.Collect(); // temp hack
        }

        void AddFilesCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All Types|*.jpg;*.png;*.zip|Images|*.jpg;*.png|Zip Files|*.zip"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                AddFileToList(filename);
            }
        }

        void RemoveFilesCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            RemoveFileFromList(this.SelectedImage);
        }

        void RemoveFilesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedImage != null)
            {
                e.CanExecute = true;
            }
        }

        void ExportFilesCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var dlg1 = new Ionic.Utils.FolderBrowserDialogEx
            {
                Description = "Select a folder for the extracted files:",
                ShowNewFolderButton = true,
                ShowEditBox = true,
                NewStyle = true,
                RootFolder = System.Environment.SpecialFolder.MyComputer,
                SelectedPath = folder,
                ShowFullPathInEditBox = false,
            };

            var result = dlg1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folder = dlg1.SelectedPath;
            }
            else
            {
                return;
            }

            ProgressBarDialog progressBar = new ProgressBarDialog()
            {
                TotalThreads = fileList.Count
            };

            string folderName = Path.GetFileName(folder);
            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = Path.GetRandomFileName();
            }

            using (FileStream file = File.Create(Path.Combine(folder, Path.ChangeExtension(folderName, ".manga"))))
            {
                file.WriteByte(0);
            }

            using (FileStream file = File.Create(Path.Combine(folder, Path.ChangeExtension(folderName, ".manga_save"))))
            {
                string output = string.Format("LAST=/mnt/us/pictures/{0}/{1}", folderName, fileList.First().OutputName);
                byte[] info = new UTF8Encoding(true).GetBytes(output);
                file.Write(info, 0, info.Length);
            }

            progressBar.CancellationTokenSource = new CancellationTokenSource();

            foreach (FileConversionInfo fileInfo in fileList)
            {
                fileInfo.OutputPath = Path.Combine(folder, fileInfo.OutputName);
                fileInfo.KindleProfile = this.SelectedProfile;
                ConversionThread thread = new ConversionThread(progressBar);
                ThreadPool.QueueUserWorkItem(thread.ThreadCallback, fileInfo);
            }

            progressBar.ShowDialog();
        }

        void ExportFilesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.fileList.Count > 0)
            {
                e.CanExecute = true;
            }
        }

        void MoveUpCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            FileConversionInfo selected = this.SelectedImage;
            if (selected != null)
            {
                int index = this.fileList.IndexOf(selected);

                if (index > 0)
                {
                    this.fileList.Move(index, index - 1);
                }
            }
        }

        void MoveDownCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            FileConversionInfo selected = this.SelectedImage;
            if (selected != null)
            {
                int index = this.fileList.IndexOf(selected);

                if (index < this.fileList.Count - 1)
                {
                    this.fileList.Move(index, index + 1);
                }
            }
        }

        void MoveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedImage != null)
            {
                e.CanExecute = true;
            }
        }
    }
}
