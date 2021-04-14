using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.CommonMath;
using Microsoft.Office.Story.V1.GraphicsInterop;
using Microsoft.Office.Story.V1.GraphicsInterop.Wic;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal abstract class ImageBase<TPixel, TValue> where TPixel : struct, IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
	{
		protected ImageBase(int width, int height) : this()
		{
			this.EnsureBuffer(width, height);
		}

		protected ImageBase(int width, int height, TPixel[] image) : this()
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (image.Length != width * height)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Array is of wrong size {0}. Must be {1} * {2} = {3} in size.", new object[]
				{
					image.Length,
					width,
					height,
					width * height
				}), "image");
			}
			this.Width = width;
			this.Height = height;
			this.Image = image;
		}

		protected ImageBase(Stream stream, Guid wicImageFormat) : this()
		{
			IWICImagingFactory iwicimagingFactory = WicUtility.CreateFactory();
			IWICBitmapFrameDecode iwicbitmapFrameDecode = iwicimagingFactory.Load(stream);
			this.LoadFromWic(iwicimagingFactory, iwicbitmapFrameDecode, wicImageFormat);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicimagingFactory);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapFrameDecode);
		}

		protected ImageBase(IWICImagingFactory factory, IWICBitmapSource bitmapSource, Guid wicImageFormat) : this()
		{
			this.LoadFromWic(factory, bitmapSource, wicImageFormat);
		}

		private ImageBase()
		{
		}

		[DataMember]
		public Vector2 Offset { get; set; }

		[DataMember]
		public int Width { get; private set; }

		[DataMember]
		public int Height { get; private set; }

		[DataMember]
		public TPixel[] Image { get; private set; }

		public int Bands
		{
			get
			{
				TPixel tpixel = default(TPixel);
				return tpixel.Bands;
			}
		}

		public TPixel this[int x, int y]
		{
			get
			{
				return this.Image[x + y * this.Width];
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} by {1}, {2} band of type {3}", new object[]
			{
				this.Width,
				this.Height,
				this.Bands,
				typeof(TValue).Name
			});
		}

		private void EnsureBuffer(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			if (width != this.Width || height != this.Height || this.Image.Length != this.Width * this.Height)
			{
				this.Width = width;
				this.Height = height;
				this.Image = new TPixel[width * height];
			}
		}

		private void LoadFromWic(IWICImagingFactory factory, IWICBitmapSource bitmapSource, Guid wicImageFormat)
		{
			int width;
			int height;
			bitmapSource.GetSize(out width, out height);
			this.EnsureBuffer(width, height);
			factory.FillBlobFromBitmapSource(bitmapSource, this.Image, wicImageFormat);
		}
	}
}
