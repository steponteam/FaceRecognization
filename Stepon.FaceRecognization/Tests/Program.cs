using System;
using System.Drawing;
using System.Drawing.Imaging;
using Stepon.FaceRecognization;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Extensions;

namespace Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestMatching();
            Console.WriteLine("process complete");
            Console.ReadKey();
        }

        private static void TestMatching()
        {
            using (var proccesor = new FaceProcessor("appid",
                "locatorKey", "recognizeKey", true))
            {
                var image1 = Image.FromFile("test2.jpg");
                var image2 = Image.FromFile("test.jpg");

                var result1 = proccesor.LocateExtract(new Bitmap(image1));
                var result2 = proccesor.LocateExtract(new Bitmap(image2));

                if ((result1 != null) & (result2 != null))
                    Console.WriteLine(proccesor.Match(result1[0].FeatureData, result2[0].FeatureData, true));
            }
        }

        private static void Test()
        {
            using (var detection = LocatorFactory.GetTrackingLocator("appid",
                "sdkKey"))
            {
                var image1 = new Bitmap(Image.FromFile("test.jpg"));
                var image2 = new Bitmap(Image.FromFile("test3.jpg"));
                var image3 = new Bitmap(Image.FromFile("test3.jpg"));
                var image4 = new Bitmap(Image.FromFile("test4.jpg"));


                var r1 = detection.Detect(new Bitmap(image1), out var l1);

                var r2 = detection.Detect(new Bitmap(image2), out var l2);

                var r3 = detection.Detect(new Bitmap(image3), out var l3);

                var r4 = detection.Detect(new Bitmap(image4), out var l4);

                using (var g = Graphics.FromImage(image2))
                {
                    foreach (var s in l2.Faces)
                    {
                        var face = s.ToRectangle();
                        g.DrawRectangle(new Pen(Color.Chartreuse), face.X, face.Y, face.Width, face.Height);
                    }
                }

                image2.Save("output.jpg", ImageFormat.Jpeg);

                l1.Dispose();
                l2.Dispose();
                l3.Dispose();
                l4.Dispose();
            }
        }

        private static void TestTracking()
        {
            using (var detection = LocatorFactory.GetTrackingLocator("appId", "sdkKey"))
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
            using (var detection = LocatorFactory.GetDetectionLocator("appId", "sdkKey"))
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