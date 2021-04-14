using System;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.Mime
{
	public class EncodingOptions
	{
		public EncodingOptions(string charsetName, string cultureName, EncodingFlags encodingFlags)
		{
			this.cultureName = cultureName;
			this.encodingFlags = encodingFlags;
			this.charset = ((charsetName == null) ? null : Charset.GetCharset(charsetName));
			if (this.charset != null)
			{
				this.charset.GetEncoding();
			}
		}

		internal EncodingOptions(Charset charset)
		{
			this.charset = charset;
			this.cultureName = null;
			this.encodingFlags = EncodingFlags.None;
		}

		public string CharsetName
		{
			get
			{
				if (this.charset != null)
				{
					return this.charset.Name;
				}
				return null;
			}
		}

		public string CultureName
		{
			get
			{
				return this.cultureName;
			}
		}

		public EncodingFlags EncodingFlags
		{
			get
			{
				return this.encodingFlags;
			}
		}

		public bool AllowUTF8
		{
			get
			{
				return (byte)(this.encodingFlags & EncodingFlags.AllowUTF8) == 8;
			}
		}

		internal Charset GetEncodingCharset()
		{
			Charset defaultCharset = this.charset;
			if (defaultCharset == null)
			{
				defaultCharset = EncodingOptions.DefaultCharset;
			}
			return defaultCharset;
		}

		internal static readonly Charset DefaultCharset = DecodingOptions.DefaultCharset;

		private EncodingFlags encodingFlags;

		private Charset charset;

		private string cultureName;
	}
}
