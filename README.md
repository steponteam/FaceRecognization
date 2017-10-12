## Face Recognization
This project is a C# wrapper of arcsoft's library.

Stepon.FaceRecognizationCore is for .net core 2.0. It uses [CoreCompat/System.Drawing](https://github.com/CoreCompat/System.Drawing), you should install libgdiplus.

## How to use
Get app id and sdk key from [arcsoft's website](http://www.arcsoft.com.cn/ai/arcface.html) (it's free). Remember to update the libraries if arcsoft has newer version. The appid and key is associated with sdk.

There are three main classes to use for recognization:
- FaceDetection for image to detect face location
- FaceTracking for video to detect face location
- FaceRecognize for face's feature extracting and matching

``FaceProcessor`` is a high level wrapper for above three classes. You can just use this class to complete the face verify task.

## Example
Face detecting:
```
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
```

Face traking (recommended for video face traking, this just an example for image, check FaceDemo for complete example):
```
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
```
Face matching:
```
using (var proccesor = new FaceProcessor("appid",
                "locatorKey", "recognizeKey", true))
{
    var image1 = Image.FromFile("test2.jpg");
    var image2 = Image.FromFile("test.jpg");

    var result1 = proccesor.LocateExtract(new Bitmap(image1));
    var result2 = proccesor.LocateExtract(new Bitmap(image2));
    
    //you can persist result1[0].FeatureData to further use

    if ((result1 != null) & (result2 != null))
        Console.WriteLine(proccesor.Match(result1[0].FeatureData, result2[0].FeatureData, true));
}
```

## FaceDemo
This is a complete example to show the using of this library. It use ffmepg(``NReco.VideoConverter``) to capture IP camera via RTSP. You can also use ``Emgu.CV``(or other library) to capture web camera.