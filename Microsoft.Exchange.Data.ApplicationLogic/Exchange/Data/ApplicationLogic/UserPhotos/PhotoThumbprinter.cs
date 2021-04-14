using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoThumbprinter
	{
		private PhotoThumbprinter()
		{
		}

		public int Compute(Stream photo)
		{
			if (photo == null)
			{
				throw new ArgumentNullException("photo");
			}
			if (photo.Position != 0L)
			{
				throw new ArgumentException("Position within stream MUST be at the beginning.", "photo");
			}
			int result;
			using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
			{
				result = BitConverter.ToInt32(messageDigestForNonCryptographicPurposes.ComputeHash(photo), 0);
			}
			return result;
		}

		public string FormatAsETag(int? thumbprint)
		{
			if (thumbprint == null)
			{
				return string.Empty;
			}
			return "\"" + thumbprint.Value.ToString("X8") + "\"";
		}

		public bool ThumbprintMatchesETag(int thumbprint, string etag)
		{
			return this.FormatAsETag(new int?(thumbprint)).Equals(etag, StringComparison.Ordinal);
		}

		public int GenerateThumbprintForNegativeCache()
		{
			return Guid.NewGuid().GetHashCode();
		}

		internal static readonly PhotoThumbprinter Default = new PhotoThumbprinter();
	}
}
