NReco.VideoConverter (FFMpeg wrapper)
-------------------------------------
Website (release notes, examples etc): https://www.nrecosite.com/video_converter_net.aspx
API documentation: https://www.nrecosite.com/doc/NReco.VideoConverter/
Nuget package: https://www.nuget.org/packages/NReco.VideoConverter/

License
-------
VideoConverter can be used for FREE in single-deployment projects (websites, intranet/extranet) or applications for company's internal business purposes (redistributed only internally inside the company). 
Commercial license (included into enterprise source code pack) is required for:
1) Applications for external redistribution (ISV)
2) SaaS deployments

How to use
----------
var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
ffMpeg.ConvertMedia("input.mov", "output.mp4", Format.mp4);