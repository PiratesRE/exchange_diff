using System;
using System.Globalization;
using System.IO;
using Microsoft.Office.Story.V1.GraphicsInterop;
using Microsoft.Office.Story.V1.GraphicsInterop.Wic;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	internal class ImageSource
	{
		public ImageSource(Stream stream) : this(ImageSource.ReadToEnd(stream))
		{
		}

		public ImageSource(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
			{
				throw new ArgumentException("Image buffer does not contain a valid image to load.", "buffer");
			}
			this.buffer = buffer;
			IWICImagingFactory iwicimagingFactory = WicUtility.CreateFactory();
			IWICBitmapFrameDecode iwicbitmapFrameDecode = iwicimagingFactory.Load(this.ImageStream);
			int num;
			int num2;
			iwicbitmapFrameDecode.GetSize(out num, out num2);
			this.Width = (float)num;
			this.Height = (float)num2;
			IWICMetadataQueryReader iwicmetadataQueryReader = null;
			try
			{
				iwicbitmapFrameDecode.GetMetadataQueryReader(out iwicmetadataQueryReader);
				string s;
				DateTime value;
				if (iwicmetadataQueryReader.TryGetMetadataProperty("/app1/ifd/exif/subifd:{uint=36867}", out s) && DateTime.TryParseExact(s, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out value))
				{
					this.DateTaken = new DateTime?(value);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				ushort value2;
				if (iwicmetadataQueryReader.TryGetMetadataProperty("/app1/ifd/{ushort=274}", out value2))
				{
					this.Orientation = ImageSource.TransformOptionsFromUshort(value2);
				}
				else
				{
					this.Orientation = WICBitmapTransformOptions.WICBitmapTransformRotate0;
				}
			}
			catch (Exception)
			{
			}
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicmetadataQueryReader);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapFrameDecode);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicimagingFactory);
		}

		public Stream ImageStream
		{
			get
			{
				return new MemoryStream(this.buffer, false);
			}
		}

		public float Height { get; private set; }

		public float Width { get; private set; }

		public Uri Source { get; set; }

		public DateTime? DateTaken { get; set; }

		public WICBitmapTransformOptions Orientation { get; set; }

		public byte[] GetBuffer()
		{
			return this.buffer;
		}

		private static byte[] ReadToEnd(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static WICBitmapTransformOptions TransformOptionsFromUshort(ushort value)
		{
			WICBitmapTransformOptions result = WICBitmapTransformOptions.WICBitmapTransformRotate0;
			switch (value)
			{
			case 2:
				result = WICBitmapTransformOptions.WICBitmapTransformFlipHorizontal;
				break;
			case 3:
				result = WICBitmapTransformOptions.WICBitmapTransformRotate180;
				break;
			case 4:
				result = WICBitmapTransformOptions.WICBitmapTransformFlipVertical;
				break;
			case 5:
				result = (WICBitmapTransformOptions.WICBitmapTransformRotate90 | WICBitmapTransformOptions.WICBitmapTransformRotate180 | WICBitmapTransformOptions.WICBitmapTransformFlipHorizontal);
				break;
			case 6:
				result = WICBitmapTransformOptions.WICBitmapTransformRotate90;
				break;
			case 7:
				result = (WICBitmapTransformOptions.WICBitmapTransformRotate90 | WICBitmapTransformOptions.WICBitmapTransformFlipHorizontal);
				break;
			case 8:
				result = WICBitmapTransformOptions.WICBitmapTransformRotate270;
				break;
			}
			return result;
		}

		private const string DateTakenMetadataPath = "/app1/ifd/exif/subifd:{uint=36867}";

		private const string DateFormat = "yyyy:MM:dd HH:mm:ss";

		private const string OrientationMetadataPath = "/app1/ifd/{ushort=274}";

		private readonly byte[] buffer;
	}
}
