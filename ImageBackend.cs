// (c) Kyle Sabo 2011

using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using Kindle.Profiles;
using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace mangle_port
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
                Image image = Image.FromFile(inputFilePath);
                Bitmap bitmap = new Bitmap(image);

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
                Image image = Image.FromStream(stream);

                Bitmap bitmap32 = new Bitmap(image);

                return ConvertImage_Internal(profile, bitmap32, outputFilePath, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ConvertImage_Internal(KindleProfile profile, Bitmap image, string outputFilePath, CancellationToken cancellationToken)
        {
            Bitmap bitmap = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format24bppRgb);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap rotated = ImageBackend.RotateImage(profile, bitmap);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap resized = ImageBackend.ResizeImage(profile, rotated);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap quantized = ImageBackend.QuantizeImage(profile, resized);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap output = quantized.Clone(new Rectangle(0, 0, quantized.Width, quantized.Height), PixelFormat.Format8bppIndexed);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            output.Save(outputFilePath);

            return true;
        }

        private static Bitmap QuantizeImage(KindleProfile profile, Bitmap image)
        {
            Bitmap output;
            FloydSteinbergColorDithering dithering = new FloydSteinbergColorDithering();
            dithering.ColorTable = profile.Palette;

            output = dithering.Apply(image);

            return output;
        }

        private static Bitmap ResizeImage(KindleProfile profile, Bitmap image)
        {
            Bitmap output;
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

        private static Bitmap RotateImage(KindleProfile profile, Bitmap image)
        {
            if (image.Width > image.Height)
            {
                Bitmap output;
                RotateBicubic filter = new RotateBicubic(90);

                output = filter.Apply(image);

                return output;
            }

            return image;
        }
    }
}
