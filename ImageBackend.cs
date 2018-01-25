// (c) Kyle Sabo 2011

using Accord.Imaging;
using Accord.Imaging.ColorReduction;
using Accord.Imaging.Filters;
using Kindle.Profiles;
using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace KindleImageConverter
{
    public class ImageBackend
    {
        public static bool ConvertImage(KindleProfile profile, string inputFilePath, string outputFilePath, CancellationToken cancellationToken)
        {
            Contract.Requires(profile != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(inputFilePath));
            Contract.Requires(!string.IsNullOrWhiteSpace(outputFilePath));
            Contract.Requires(cancellationToken != null);

            try
            {
                Bitmap bitmap = new Bitmap(inputFilePath);

                return ConvertImage_Internal(profile, bitmap, outputFilePath, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ConvertImage(KindleProfile profile, Stream stream, string outputFilePath, CancellationToken cancellationToken)
        {
            Contract.Requires(profile != null);
            Contract.Requires(stream != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(outputFilePath));
            Contract.Requires(cancellationToken != null);

            try
            {
                Bitmap bitmap = new Bitmap(stream);

                return ConvertImage_Internal(profile, bitmap, outputFilePath, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ConvertImage_Internal(KindleProfile profile, Bitmap bitmap, string outputFilePath, CancellationToken cancellationToken)
        {
            Bitmap bitmap24 = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);

            using (UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(bitmap24))
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                using (UnmanagedImage rotated = ImageBackend.RotateImage(profile, unmanagedImage))
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    using (UnmanagedImage resized = ImageBackend.ResizeImage(profile, rotated))
                    {

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return false;
                        }

                        Bitmap quantized = ImageBackend.QuantizeImage(profile, resized);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return false;
                        }

                        Bitmap output = (quantized.PixelFormat == PixelFormat.Format8bppIndexed || quantized.PixelFormat == PixelFormat.Format4bppIndexed)
                            ? quantized
                            : quantized.Clone(new Rectangle(0, 0, quantized.Width, quantized.Height), PixelFormat.Format8bppIndexed);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return false;
                        }

                        output.Save(outputFilePath);

                        return true;
                    }
                }
            }
        }

        private static Bitmap QuantizeImage(KindleProfile profile, UnmanagedImage image)
        {
            Bitmap output;
            FloydSteinbergColorDithering dithering = new FloydSteinbergColorDithering
            {
                ColorTable = profile.Palette
            };

            output = dithering.Apply(image);

            return output;
        }

        private static UnmanagedImage ResizeImage(KindleProfile profile, UnmanagedImage image)
        {
            UnmanagedImage output;
            ResizeBicubic filter;

            if (image.Width > image.Height)
            {
                double scalingFactor = (double)image.Width / profile.Width;
                int newHeight = (int)Math.Floor((double)image.Height / scalingFactor);

                filter = new ResizeBicubic(profile.Width, newHeight);
            }
            else
            {
                double scalingFactor = (double)image.Height / profile.Height;
                int newWidth = (int)Math.Floor((double)image.Width / scalingFactor);

                filter = new ResizeBicubic(newWidth, profile.Height);
            }

            output = filter.Apply(image);

            return output;
        }

        private static UnmanagedImage RotateImage(KindleProfile profile, UnmanagedImage image)
        {
            if (image.Width > image.Height)
            {
                UnmanagedImage output;
                RotateBicubic filter = new RotateBicubic(90);

                output = filter.Apply(image);

                return output;
            }

            return image;
        }
    }
}
