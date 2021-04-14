using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UserPhotoUtilities
	{
		public static Dictionary<UserPhotoSize, Image> GetAllScaleCroppedImages(Stream originalImageStream)
		{
			Util.ThrowOnNullArgument(originalImageStream, "originalImageStream");
			Dictionary<UserPhotoSize, Image> result;
			using (Image imageFromStream = UserPhotoUtilities.GetImageFromStream(originalImageStream, true))
			{
				Dictionary<UserPhotoSize, Image> dictionary = new Dictionary<UserPhotoSize, Image>();
				foreach (UserPhotoSize userPhotoSize in (UserPhotoSize[])Enum.GetValues(typeof(UserPhotoSize)))
				{
					dictionary.Add(userPhotoSize, UserPhotoUtilities.GetImageOfSize(imageFromStream, userPhotoSize));
				}
				result = dictionary;
			}
			return result;
		}

		private static Image GetImageOfSize(Image originalImage, UserPhotoSize size)
		{
			Util.ThrowOnNullArgument(originalImage, "originalImage");
			EnumValidator.ThrowIfInvalid<UserPhotoSize>(size, "size");
			return UserPhotoUtilities.GetScaledAndCroppedImage(originalImage, UserPhotoDimensions.GetImageSize(size));
		}

		private static Image GetImageFromStream(Stream imageStream, bool checkSizes)
		{
			Util.ThrowOnNullArgument(imageStream, "imageStream");
			if (checkSizes)
			{
				if (imageStream.Length <= 0L)
				{
					UserPhotoUtilities.Tracer.TraceError<long, int>(0L, "Stream of length {0} does not meet minimum file size requirement of {1}.", imageStream.Length, 0);
					throw new UserPhotoFileTooSmallException();
				}
				if (imageStream.Length > 20971520L)
				{
					UserPhotoUtilities.Tracer.TraceError<long, int>(0L, "Stream of length {0} exceeds maximum file size requirement of {1}.", imageStream.Length, 20);
					throw new UserPhotoFileTooLargeException();
				}
			}
			Image result;
			try
			{
				Image image = Image.FromStream(imageStream);
				if (checkSizes && Math.Min(image.Height, image.Width) < UserPhotoUtilities.MinImageSize)
				{
					UserPhotoUtilities.Tracer.TraceError<int, int, int>(0L, "Image of {0} x {1} does not meet the minimum size requirement of {2}.", image.Width, image.Height, UserPhotoUtilities.MinImageSize);
					throw new UserPhotoImageTooSmallException();
				}
				result = image;
			}
			catch (ArgumentException)
			{
				UserPhotoUtilities.Tracer.TraceError(0L, "Stream provided did not represent an Image.");
				throw new UserPhotoNotAnImageException();
			}
			return result;
		}

		private static Image GetScaledAndCroppedImage(Image image, Size maxImageSize)
		{
			if (maxImageSize.Width > image.Size.Width || maxImageSize.Height > image.Size.Height)
			{
				return null;
			}
			Size imageScaledSize = UserPhotoUtilities.GetImageScaledSize(image.Size, maxImageSize);
			Image result;
			using (Bitmap bitmap = new Bitmap(imageScaledSize.Width, imageScaledSize.Height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
				}
				RectangleF imageCenterCrop = UserPhotoUtilities.GetImageCenterCrop(imageScaledSize, maxImageSize);
				result = bitmap.Clone(imageCenterCrop, bitmap.PixelFormat);
			}
			return result;
		}

		private static RectangleF GetImageCenterCrop(Size imageSize, Size maxImageSize)
		{
			PointF imageCenterPosition = UserPhotoUtilities.GetImageCenterPosition(imageSize, maxImageSize);
			float width = (float)Math.Min(imageSize.Width, maxImageSize.Width);
			float height = (float)Math.Min(imageSize.Height, maxImageSize.Height);
			return new RectangleF(imageCenterPosition, new SizeF(width, height));
		}

		private static PointF GetImageCenterPosition(Size imageSize, Size maxImageSize)
		{
			PointF result = new PointF(0f, 0f);
			if (UserPhotoUtilities.NeedsHorizontalCrop(imageSize, maxImageSize))
			{
				result.X = (float)(imageSize.Width - maxImageSize.Width) * 0.5f;
			}
			if (UserPhotoUtilities.NeedsVerticalCrop(imageSize, maxImageSize))
			{
				result.Y = (float)(imageSize.Height - maxImageSize.Height) * 0.35f;
			}
			return result;
		}

		private static float GetVerticalScaleFactor(Size imageSize, Size maxImageSize)
		{
			float result = 1f;
			if (UserPhotoUtilities.NeedsVerticalCrop(imageSize, maxImageSize))
			{
				result = (float)maxImageSize.Height / (float)imageSize.Height;
			}
			return result;
		}

		private static float GetHorizontalScaleFactor(Size imageSize, Size maxImageSize)
		{
			float result = 1f;
			if (UserPhotoUtilities.NeedsHorizontalCrop(imageSize, maxImageSize))
			{
				result = (float)maxImageSize.Width / (float)imageSize.Width;
			}
			return result;
		}

		private static bool NeedsVerticalCrop(Size imageSize, Size maxImageSize)
		{
			return imageSize.Height > maxImageSize.Height;
		}

		private static bool NeedsHorizontalCrop(Size imageSize, Size maxImageSize)
		{
			return imageSize.Width > maxImageSize.Width;
		}

		private static Size GetImageScaledSize(Size imageSize, Size maxImageSize)
		{
			float scale = Math.Max(UserPhotoUtilities.GetVerticalScaleFactor(imageSize, maxImageSize), UserPhotoUtilities.GetHorizontalScaleFactor(imageSize, maxImageSize));
			return UserPhotoUtilities.GetScaledSize(imageSize, scale);
		}

		private static Size GetScaledSize(Size size, float scale)
		{
			int width = (int)Math.Ceiling((double)((float)size.Width * scale));
			int height = (int)Math.Ceiling((double)((float)size.Height * scale));
			return new Size(width, height);
		}

		internal const int MaxFileSize = 20;

		internal const int MinFileSize = 0;

		internal const float LandscapeCropRatio = 0.5f;

		internal const float PortraitCropRatio = 0.35f;

		private const long Hash = 0L;

		internal static readonly int MinImageSize = UserPhotoDimensions.HR48x48ImageSize.Height;

		private static readonly Trace Tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
