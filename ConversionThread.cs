// (c) Kyle Sabo 2011

using Kindle.Profiles;
using System;
using System.Diagnostics;
using System.IO;

namespace KindleImageConverter
{
    class ConversionThread
    {
        public void ThreadCallback(object context)
        {
            try
            {
                FileConversionInfo fileInfo = context as FileConversionInfo;

                if (fileInfo != null)
                {
                    Debug.Assert(fileInfo.KindleProfile != null);
                    Debug.Assert(fileInfo.FileStream != null || fileInfo.InputFilePath != null);

                    if (fileInfo.FileStream != null)
                    {
                        ImageBackend.ConvertImage(fileInfo.KindleProfile, fileInfo.FileStream, fileInfo.OutputPath, progressViewModel.CancellationTokenSource.Token);
                    }
                    else
                    {
                        ImageBackend.ConvertImage(fileInfo.KindleProfile, fileInfo.InputFilePath, fileInfo.OutputPath, progressViewModel.CancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                progressViewModel.Progress += 1;
            }
        }

        public ConversionThread(ProgressBarDialog dialog)
        {
            this.progressViewModel = dialog;
        }

        private ProgressBarDialog progressViewModel;
    }
}
