using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.Mime
{
	public struct DecodingOptions
	{
		public DecodingOptions(DecodingFlags decodingFlags)
		{
			this = new DecodingOptions(decodingFlags, null);
		}

		public DecodingOptions(DecodingFlags decodingFlags, Encoding encoding)
		{
			this.decodingFlags = decodingFlags;
			this.charset = ((encoding == null) ? null : Charset.GetCharset(encoding));
		}

		public DecodingOptions(DecodingFlags decodingFlags, string charsetName)
		{
			this.decodingFlags = decodingFlags;
			this.charset = ((charsetName == null) ? null : Charset.GetCharset(charsetName));
		}

		internal DecodingOptions(string charsetName)
		{
			this = new DecodingOptions(DecodingOptions.Default.DecodingFlags, charsetName);
		}

		internal static Charset DefaultCharset
		{
			get
			{
				if (DecodingOptions.defaultCharset == null)
				{
					Charset charset = Charset.DefaultMimeCharset;
					if (!charset.IsAvailable || charset.AsciiSupport < CodePageAsciiSupport.Fine)
					{
						charset = Charset.UTF8;
					}
					DecodingOptions.defaultCharset = charset;
				}
				return DecodingOptions.defaultCharset;
			}
		}

		public DecodingFlags DecodingFlags
		{
			get
			{
				return this.decodingFlags;
			}
			internal set
			{
				this.decodingFlags = value;
			}
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

		public Encoding CharsetEncoding
		{
			get
			{
				if (this.charset != null && this.charset.IsAvailable)
				{
					return this.charset.GetEncoding();
				}
				return null;
			}
		}

		public bool AllowUTF8
		{
			get
			{
				return (this.decodingFlags & DecodingFlags.Utf8) == DecodingFlags.Utf8;
			}
		}

		internal Charset Charset
		{
			get
			{
				return this.charset;
			}
			set
			{
				this.charset = value;
			}
		}

		private static Charset defaultCharset;

		public static readonly DecodingOptions None = default(DecodingOptions);

		public static readonly DecodingOptions Default = new DecodingOptions(DecodingFlags.AllEncodings);

		private DecodingFlags decodingFlags;

		private Charset charset;
	}
}
