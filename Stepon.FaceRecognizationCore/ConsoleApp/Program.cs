using System;
using System.Drawing;
using System.Drawing.Imaging;
using Stepon.FaceRecognizationCore;
using Stepon.FaceRecognizationCore.Common;
using Stepon.FaceRecognizationCore.Extensions;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestDetection();
            Console.WriteLine("Complete");
        }

        private static void TestMatching()
        {
            using (var proccesor = new FaceProcessor("appId",
                "ftKey", "frKey", true))
            {
                var image1 = Image.FromFile("test2.jpg");
                var image2 = Image.FromFile("test.jpg");

                var result1 = proccesor.LocateExtract(new Bitmap(image1));
                var result2 = proccesor.LocateExtract(new Bitmap(image2));

                if ((result1 != null) & (result2 != null))
                    Console.WriteLine(proccesor.Match(result1[0].FeatureData, result2[0].FeatureData, true));
            }
        }

        private static void TestTracking()
        {
            using (var detection = LocatorFactory.GetTrackingLocator("appId",
                "fdKey"))
            {
                var image = Image.FromFile("test.jpg");
                var bitmap = new Bitmap(image);

                var result = detection.Detect(bitmap, out var locateResult);
                using (locateResult)
                {
                    if (result == ErrorCode.Ok && locateResult.FaceCount > 0)
                    {
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            var face = locateResult.Faces[0].ToRectangle();
                            g.DrawRectangle(new Pen(Color.Chartreuse), face.X, face.Y, face.Width, face.Height);
                        }

                        bitmap.Save("output.jpg", ImageFormat.Jpeg);
                    }
                }
            }
        }

        private static void TestDetection()
        {
            using (var detection = LocatorFactory.GetDetectionLocator("appId",
                "ftKey"))
            {
                var image = Image.FromFile("test.jpg");
                var bitmap = new Bitmap(image);

                var result = detection.Detect(bitmap, out var locateResult);
                using (locateResult)
                {
                    if (result == ErrorCode.Ok && locateResult.FaceCount > 0)
                    {
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            var face = locateResult.Faces[0].ToRectangle();
                            g.DrawRectangle(new Pen(Color.Chartreuse), face.X, face.Y, face.Width, face.Height);
                        }

                        bitmap.Save("output.jpg", ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}