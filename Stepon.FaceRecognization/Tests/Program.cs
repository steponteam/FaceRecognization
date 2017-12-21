using System;
using System.Drawing;
using System.Drawing.Imaging;
using Stepon.FaceRecognization;
using Stepon.FaceRecognization.Age;
using Stepon.FaceRecognization.Common;
using Stepon.FaceRecognization.Extensions;
using Stepon.FaceRecognization.Gender;

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

        private static void TestAgeAndGender()
        {
            using (var detection = LocatorFactory.GetDetectionLocator("appid", "key"))
            {
                var image1 = Image.FromFile("test2.jpg");
                using (var estimate = new FaceAge("appid", "key"))
                {
                    var result1 = estimate.StaticEstimation(detection, new Bitmap(image1));
                    foreach (var result1Age in result1.Ages)
                    {
                        Console.WriteLine(result1Age);
                    }
                }

                using (var estimate = new FaceGender("appid", "key"))
                {
                    var result1 = estimate.StaticEstimation(detection, new Bitmap(image1));
                    foreach (var result1Gender in result1.Genders)
                    {
                        Console.WriteLine(result1Gender);
                    }
                }
            }

            //another
            var age = new FaceAge("appid", "key");
            var gender = new FaceGender("appid", "key");
            using (var detection = LocatorFactory.GetDetectionLocator("appid", "key", age, gender))
            {
                var image1 = Image.FromFile("test2.jpg");
                var result = detection.Detect(new Bitmap(image1), out var location,
                    LocateOperation.IncludeAge | LocateOperation.IncludeGender); //default is None, no age and gender estimation
                for (var i = 0; i < location.FaceCount; i++)
                {
                    Console.WriteLine(location.Ages[i]);
                    Console.WriteLine(location.Genders[i]);
                }
            }
            age.Dispose();
            gender.Dispose();

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