using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class DocumentLibraryObjectId : ObjectId
	{
		protected internal DocumentLibraryObjectId(UriFlags uriFlags)
		{
			this.uriFlags = uriFlags;
		}

		public static DocumentLibraryObjectId Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentException("str");
			}
			Uri uri = null;
			try
			{
				uri = new Uri(str);
			}
			catch (UriFormatException innerException)
			{
				throw new CorruptDataException(str, Strings.ExCorruptData, innerException);
			}
			if (uri.IsUnc)
			{
				int num = str.IndexOf(UncObjectId.QueryPart);
				if (num < 0 || str[num - 1] != '?')
				{
					throw new CorruptDataException(str, Strings.ExCorruptData);
				}
				UriFlags uriFlags;
				try
				{
					uriFlags = (UriFlags)Enum.Parse(typeof(UriFlags), str.Substring(num + UncObjectId.QueryPart.Length));
				}
				catch (ArgumentException innerException2)
				{
					throw new CorruptDataException(str, Strings.ExCorruptData, innerException2);
				}
				Uri uri2 = new Uri(str.Substring(0, num - 1));
				if (!Utils.IsValidUncUri(uri2))
				{
					throw new CorruptDataException(str, Strings.ExCorruptData);
				}
				return new UncObjectId(uri2, uriFlags);
			}
			else
			{
				if (uri.IsFile)
				{
					throw new ArgumentException("str");
				}
				return SharepointSiteId.Parse(str);
			}
		}

		public static DocumentLibraryObjectId Deserialize(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			if (byteArray.Length == 0)
			{
				throw new ArgumentException("byteArray");
			}
			UTF8Encoding utf8Encoding = new UTF8Encoding();
			return DocumentLibraryObjectId.Parse(utf8Encoding.GetString(byteArray));
		}

		public static DocumentLibraryObjectId Deserialize(string base64Id)
		{
			if (base64Id == null)
			{
				throw new ArgumentNullException("base64Id");
			}
			byte[] byteArray = Convert.FromBase64String(base64Id);
			return DocumentLibraryObjectId.Deserialize(byteArray);
		}

		public UriFlags UriFlags
		{
			get
			{
				return this.uriFlags;
			}
		}

		public string ToBase64String()
		{
			return Convert.ToBase64String(this.GetBytes());
		}

		public override byte[] GetBytes()
		{
			string s = this.ToString();
			UTF8Encoding utf8Encoding = new UTF8Encoding();
			return utf8Encoding.GetBytes(s);
		}

		private UriFlags uriFlags;
	}
}
