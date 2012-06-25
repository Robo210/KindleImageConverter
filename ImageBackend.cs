// (c) Kyle Sabo 2011

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using AForge.Imaging.Formats;
using Kindle.Profiles;

namespace mangle_port
{
    public class ImageBackend
    {
        public static bool ConvertImage(KindleProfile profile, string inputFilePath, string outputFilePath)
        {
            Bitmap bitmap = ImageDecoder.DecodeFromFile(inputFilePath);

            Bitmap rotated = RotateImage(profile, bitmap);
            Bitmap resized = ResizeImage(profile, rotated);
            Bitmap quantized = QuantizeImage(profile, resized);

            quantized.Save(outputFilePath);

            return true;
        }

        public static bool ConvertImage(KindleProfile profile, string inputFilePath, Stream stream, string outputFilePath, CancellationToken cancellationToken)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            Bitmap bitmap32 = new Bitmap(image);
            Bitmap bitmap = bitmap32.Clone(new Rectangle(0, 0, bitmap32.Width, bitmap32.Height), PixelFormat.Format24bppRgb);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap rotated = RotateImage(profile, bitmap);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap resized = ResizeImage(profile, rotated);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            Bitmap quantized = QuantizeImage(profile, resized);

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

        public static Bitmap QuantizeImage(KindleProfile profile, Bitmap image)
        {
            Bitmap output;
            FloydSteinbergColorDithering dithering = new FloydSteinbergColorDithering();
            dithering.ColorTable = profile.Palette;

            output = dithering.Apply(image);

            return output;
        }

        public static Bitmap ResizeImage(KindleProfile profile, Bitmap image)
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

        public static Bitmap RotateImage(KindleProfile profile, Bitmap image)
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
