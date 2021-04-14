using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.GraphicsInterop.Wic;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class ArgbImage : ImageBase<ArgbPixel, byte>
	{
		public ArgbImage(int width, int height) : base(width, height)
		{
		}

		public ArgbImage(int width, int height, ArgbPixel[] image) : base(width, height, image)
		{
		}

		public ArgbImage(Stream stream) : base(stream, ArgbImage.WicImageFormat)
		{
		}

		public ArgbImage(IWICImagingFactory factory, IWICBitmapSource bitmapSource) : base(factory, bitmapSource, ArgbImage.WicImageFormat)
		{
		}

		public static Guid WicImageFormat
		{
			get
			{
				return WicGuids.GUID_WICPixelFormat32bppBGRA;
			}
		}

		internal const float MaximumContrastRatio = 21f;

		internal const float MinimumContrastRatio = 1f;
	}
}
