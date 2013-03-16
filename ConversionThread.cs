// (c) Kyle Sabo 2011

using Kindle.Profiles;
using System;
using System.IO;

namespace mangle_port
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
                    if (fileInfo.FileStream != null)
                    {
                        ImageBackend.ConvertImage(KindleProfiles.Kindle3, fileInfo.FileStream, fileInfo.OutputPath, progressViewModel.CancellationTokenSource.Token);
                    }
                    else
                    {
                        ImageBackend.ConvertImage(KindleProfiles.Kindle3, fileInfo.InputFilePath, fileInfo.OutputPath, progressViewModel.CancellationTokenSource.Token);
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
