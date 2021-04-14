using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	[Serializable]
	internal class ImageInfo
	{
		public ImageInfo(ImageSource imageSource)
		{
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			this.imageSource = imageSource;
			this.Width = this.imageSource.Width;
			this.Height = this.imageSource.Height;
		}

		protected ImageInfo()
		{
		}

		public ImageSource ImageSource
		{
			get
			{
				return this.imageSource;
			}
		}

		public float Width { get; private set; }

		public float Height { get; private set; }

		public SalientObjectAnalysis SalientObjectAnalysis { get; set; }

		public bool IsLocked
		{
			get
			{
				return null == this.imageSource;
			}
		}

		public static ImageInfo FromBase64(string base64)
		{
			if (base64 == null)
			{
				throw new ArgumentNullException("base64");
			}
			ImageInfo result;
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64)))
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					result = (ImageInfo)binaryFormatter.Deserialize(gzipStream);
				}
			}
			return result;
		}

		public void Lock()
		{
			this.imageSource = null;
		}

		public string ToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.Serialize(gzipStream, this);
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		internal SalientObjectAnalysis PerformSalientObjectAnalysis()
		{
			this.CheckNotLocked();
			return new SalientObjectAnalysis(this.imageSource);
		}

		private void CheckNotLocked()
		{
			if (this.IsLocked)
			{
				throw new InvalidOperationException("ImageInfo is locked and no further analysis is possible.");
			}
		}

		[NonSerialized]
		private ImageSource imageSource;
	}
}
