// (c) Kyle Sabo 2011

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;
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

        public static bool ConvertImage(KindleProfile profile, string inputFilePath, Stream stream, string outputFilePath)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            Bitmap bitmap32 = new Bitmap(image);
            Bitmap bitmap = bitmap32.Clone(new Rectangle(0, 0, bitmap32.Width, bitmap32.Height), PixelFormat.Format24bppRgb);


            Bitmap rotated = RotateImage(profile, bitmap);
            Bitmap resized = ResizeImage(profile, rotated);
            Bitmap quantized = QuantizeImage(profile, resized);
            Bitmap output = quantized.Clone(new Rectangle(0, 0, quantized.Width, quantized.Height), PixelFormat.Format8bppIndexed);

            output.Save(outputFilePath);

            return true;
        }

        public static ITargetBlock<FileConversionInfo> CreateImageProcessingNetwork(KindleProfile profile, CancellationToken cancel)
        {
            //
            // Create the dataflow blocks that form the network.
            //

            var fileInfoBlock = new BroadcastBlock<FileConversionInfo>(fileInfo => fileInfo, new DataflowBlockOptions() { CancellationToken = cancel});

            // Create a dataflow block that takes a folder path as input
            // and returns a Bitmap object.
            var loadBitmap = new TransformBlock<FileConversionInfo, Bitmap>(fileInfo =>
            {
                try
                {
                    return ImageDecoder.DecodeFromFile(fileInfo.InputFilePath);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage
                    // of the network.
                    return null;
                }
            }, new ExecutionDataflowBlockOptions() { CancellationToken = cancel, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            // Create a dataflow block that takes a Bitmap object
            // and rotates it.
            var createRotatedBitmap = new TransformBlock<Bitmap, Bitmap>(bitmap =>
            {
                try
                {
                    return RotateImage(profile, bitmap);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage
                    // of the network.
                    return null;
                }
            }, new ExecutionDataflowBlockOptions() { CancellationToken = cancel, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            // Create a dataflow block that takes a Bitmap object
            // and resizes it.
            var createResizedBitmap = new TransformBlock<Bitmap, Bitmap>(bitmap =>
            {
                try
                {
                    return ResizeImage(profile, bitmap);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage
                    // of the network.
                    return null;
                }
            }, new ExecutionDataflowBlockOptions() { CancellationToken = cancel, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            // Create a dataflow block that takes a collection of Bitmap objects
            // and resizes them.
            var createQuantizedBitmap = new TransformBlock<Bitmap, Bitmap>(bitmap =>
            {
                try
                {
                    return QuantizeImage(profile, bitmap);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage
                    // of the network.
                    return null;
                }
            }, new ExecutionDataflowBlockOptions() { CancellationToken = cancel, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            var joinBlock = new JoinBlock<FileConversionInfo, Bitmap>();

            // Create a dataflow block that displays the provided bitmap on the form.
            var saveBitmap = new ActionBlock<Tuple<FileConversionInfo, Bitmap>>(tuple =>
            {
                var bitmap = tuple.Item2;
                bitmap.Save(tuple.Item1.OutputPath);
            }, new ExecutionDataflowBlockOptions() { CancellationToken = cancel });

            // Create a dataflow block that responds to a cancellation request
            var operationCancelled = new ActionBlock<object>(delegate
            {
                // Display the error image to indicate that the operation
                // was cancelled.

                // TODO
            });

            //
            // Connect the network.
            //

            fileInfoBlock.LinkTo(loadBitmap);
            fileInfoBlock.LinkTo(joinBlock.Target1);

            // Link loadBitmaps to createRotatedBitmap.
            // The provided predicate ensures that createRotatedBitmap accepts the
            // bitmap only if that bitmap is not null.
            loadBitmap.LinkTo(createRotatedBitmap, bitmap => bitmap != null);

            // Also link loadBitmaps to operationCancelled.
            // When createRotatedBitmap rejects the message, loadBitmaps
            // offers the message to operationCancelled.
            // operationCancelled accepts all messages because we do not provide a
            // predicate.
            loadBitmap.LinkTo(operationCancelled);

            createRotatedBitmap.LinkTo(createResizedBitmap, bitmap => bitmap != null);

            createRotatedBitmap.LinkTo(operationCancelled);

            createResizedBitmap.LinkTo(createQuantizedBitmap, bitmap => bitmap != null);

            createResizedBitmap.LinkTo(operationCancelled);

            createQuantizedBitmap.LinkTo(joinBlock.Target2, bitmap => bitmap != null);

            createQuantizedBitmap.LinkTo(operationCancelled);

            joinBlock.LinkTo(saveBitmap);

            // Return the head of the network.
            return fileInfoBlock;
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
