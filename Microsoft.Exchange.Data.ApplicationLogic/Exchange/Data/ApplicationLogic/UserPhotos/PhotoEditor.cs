using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoEditor : IPhotoEditor
	{
		private PhotoEditor()
		{
		}

		public IDictionary<UserPhotoSize, byte[]> CropAndScale(Stream photo)
		{
			if (photo == null)
			{
				throw new ArgumentNullException("photo");
			}
			if (photo.Position != 0L)
			{
				throw new ArgumentException("Position within stream MUST be at the beginning.", "photo");
			}
			Dictionary<UserPhotoSize, Image> allScaleCroppedImages = UserPhotoUtilities.GetAllScaleCroppedImages(photo);
			Dictionary<UserPhotoSize, byte[]> dictionary = new Dictionary<UserPhotoSize, byte[]>();
			IDictionary<UserPhotoSize, byte[]> result;
			try
			{
				foreach (KeyValuePair<UserPhotoSize, Image> keyValuePair in allScaleCroppedImages)
				{
					if (keyValuePair.Value != null)
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							keyValuePair.Value.Save(memoryStream, ImageFormat.Jpeg);
							dictionary[keyValuePair.Key] = memoryStream.ToArray();
						}
					}
				}
				result = dictionary;
			}
			finally
			{
				foreach (Image image in allScaleCroppedImages.Values)
				{
					if (image != null)
					{
						image.Dispose();
					}
				}
			}
			return result;
		}

		internal static readonly PhotoEditor Default = new PhotoEditor();
	}
}
