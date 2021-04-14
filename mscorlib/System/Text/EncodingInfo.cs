using System;

namespace System.Text
{
	[Serializable]
	public sealed class EncodingInfo
	{
		internal EncodingInfo(int codePage, string name, string displayName)
		{
			this.iCodePage = codePage;
			this.strEncodingName = name;
			this.strDisplayName = displayName;
		}

		public int CodePage
		{
			get
			{
				return this.iCodePage;
			}
		}

		public string Name
		{
			get
			{
				return this.strEncodingName;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.strDisplayName;
			}
		}

		public Encoding GetEncoding()
		{
			return Encoding.GetEncoding(this.iCodePage);
		}

		public override bool Equals(object value)
		{
			EncodingInfo encodingInfo = value as EncodingInfo;
			return encodingInfo != null && this.CodePage == encodingInfo.CodePage;
		}

		public override int GetHashCode()
		{
			return this.CodePage;
		}

		private int iCodePage;

		private string strEncodingName;

		private string strDisplayName;
	}
}
