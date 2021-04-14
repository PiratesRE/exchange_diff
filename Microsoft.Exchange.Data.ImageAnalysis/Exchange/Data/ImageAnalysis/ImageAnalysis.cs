using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	internal class ImageAnalysis : IImageAnalysis
	{
		public KeyValuePair<byte[], ImageAnalysisResult> GenerateThumbnail(Stream imageStream, int minImageWidth, int minImageHeight, int maxThumbnailWidth, int maxThumbnailHeight, out int width, out int height)
		{
			ImageAnalysisResult value = ImageAnalysisResult.UnknownFailure;
			byte[] key = null;
			width = 0;
			height = 0;
			using (Image image = Image.FromStream(imageStream, true, false))
			{
				if (image.Width < minImageWidth || image.Height < minImageHeight)
				{
					value = ImageAnalysisResult.ImageTooSmallForAnalysis;
				}
				else
				{
					this.RotateImageIfNeeded(image);
					key = this.PreviewImage(image, maxThumbnailWidth, maxThumbnailHeight, out width, out height);
					value = ImageAnalysisResult.ThumbnailSuccess;
				}
			}
			return new KeyValuePair<byte[], ImageAnalysisResult>(key, value);
		}

		public ISalientObjectAnalysis GetSalientObjectanalysis(byte[] imageData, int imageWidth, int imageHeight)
		{
			return new SalientObjectAnalysisWrapper(imageData, imageWidth, imageHeight);
		}

		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.CompositingQuality = CompositingQuality.Default;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
			}
			return bitmap;
		}

		private static void CalcDesiredDimension(float curWidth, float curHeight, ref int desiredWidth, ref int desiredHeight)
		{
			float num = (float)desiredWidth / (float)desiredHeight;
			float num2 = curWidth / curHeight;
			if (num2 > num)
			{
				desiredHeight = (int)((float)desiredWidth / num2);
				return;
			}
			desiredWidth = (int)((float)desiredHeight * num2);
		}

		private byte[] PreviewImage(Image image, int maxImageWidth, int maxImageHeight, out int width, out int height)
		{
			int width2 = maxImageWidth;
			int height2 = maxImageHeight;
			ImageAnalysis.CalcDesiredDimension((float)image.Width, (float)image.Height, ref width2, ref height2);
			byte[] result;
			using (Bitmap bitmap = ImageAnalysis.ResizeImage(image, width2, height2))
			{
				width = bitmap.Width;
				height = bitmap.Height;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					bitmap.Save(memoryStream, ImageFormat.Png);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private void RotateImageIfNeeded(Image image)
		{
			PropertyItem[] propertyItems = image.PropertyItems;
			int i = 0;
			while (i < propertyItems.Length)
			{
				PropertyItem propertyItem = propertyItems[i];
				if (propertyItem.Id == ImageAnalysis.propertyTagOrientation && (int)propertyItem.Type == ImageAnalysis.propertyTagTypeShort)
				{
					ushort num = (ushort)propertyItem.Value[0];
					if (num == ImageAnalysis.orientationBottomRightSide)
					{
						image.RotateFlip(RotateFlipType.Rotate180FlipNone);
						return;
					}
					if (num == ImageAnalysis.orientationRightsideTop)
					{
						image.RotateFlip(RotateFlipType.Rotate90FlipNone);
						return;
					}
					if (num == ImageAnalysis.orientationLeftsideBottom)
					{
						image.RotateFlip(RotateFlipType.Rotate270FlipNone);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		private static int propertyTagOrientation = 274;

		private static int propertyTagTypeShort = 3;

		private static ushort orientationBottomRightSide = 3;

		private static ushort orientationRightsideTop = 6;

		private static ushort orientationLeftsideBottom = 8;
	}
}
