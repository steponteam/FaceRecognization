## Face Recognization
This project is a C# wrapper of arcsoft's library.

Stepon.FaceRecognizationCore is for .net core 2.0. It uses [CoreCompat/System.Drawing](https://github.com/CoreCompat/System.Drawing), you should install libgdiplus.

## How to use

### Nuget Package
For .net framework 4.5.1 (V1.1.0):
```powershell
Install-Package Stepon.FaceRecognization
```
For .net standard 2.0 (V1.1.0):
```powershell
Install-Package Stepon.FaceRecognizationCore
```

Get app id and sdk key from [arcsoft's website](http://www.arcsoft.com.cn/ai/arcface.html) (it's free). Remember to update the libraries if arcsoft has newer version. The appid and key is associated with sdk.

There are three main classes to use for recognization:
- FaceDetection for image to detect face location
- FaceTracking for video to detect face location
- FaceRecognize for face's feature extracting and matching
- FaceAge for age estimation
- FaceGender for gender estimation

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
Face age and gender estimate:
```
var age = new FaceAge("appid", "key");
var gender = new FaceGender("appid", "key");
using (var detection = LocatorFactory.GetDetectionLocator("appid", "key", age, gender))
{
    var image1 = Image.FromFile("test.jpg");
    var bitmap = new Bitmap(image1);
    var result = detection.Detect(bitmap, out var location,
        LocateOperation.IncludeAge | LocateOperation.IncludeGender);//default is None, no age and gender estimate

    using (var g = Graphics.FromImage(bitmap))
    {
        for (var i = 0; i < location.FaceCount; i++)
        {
            var face = location.Faces[i].ToRectangle();
            g.DrawRectangle(new Pen(Color.Chartreuse), face.X, face.Y, face.Width, face.Height);
            g.DrawString($"age:{location.Ages[i]},gender:{location.Genders[i]}", new Font("Yanhei", 12), new SolidBrush(Color.Aqua), face.X, face.Y);
        } 
    }

    bitmap.Save("Ok.jpg", ImageFormat.Jpeg);

    location.Dispose();
}
age.Dispose();
gender.Dispose();
```
It also can be used standalone:
```
using (var detection = LocatorFactory.GetDetectionLocator("appid", "key")) // should pay attension of static or preview mode, you should use detection for static estimation and tracking for prview estimation
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
```


## FaceDemo
This is a complete example to show the using of this library. It use ffmepg(``NReco.VideoConverter``) to capture IP camera via RTSP. You can also use ``Emgu.CV``(or other library) to capture web camera.