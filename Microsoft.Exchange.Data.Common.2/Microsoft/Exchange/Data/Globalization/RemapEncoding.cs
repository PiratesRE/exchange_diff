using System;
using System.Text;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	internal class RemapEncoding : Encoding
	{
		public RemapEncoding(int codePage) : base(codePage)
		{
			if (codePage == 28591)
			{
				this.encodingEncoding = Encoding.GetEncoding(28591);
				this.decodingEncoding = Encoding.GetEncoding(1252);
				return;
			}
			if (codePage == 28599)
			{
				this.encodingEncoding = Encoding.GetEncoding(28599);
				this.decodingEncoding = Encoding.GetEncoding(1254);
				return;
			}
			throw new ArgumentException();
		}

		public override int CodePage
		{
			get
			{
				return this.encodingEncoding.CodePage;
			}
		}

		public override string BodyName
		{
			get
			{
				return this.encodingEncoding.BodyName;
			}
		}

		public override string EncodingName
		{
			get
			{
				return this.encodingEncoding.EncodingName;
			}
		}

		public override string HeaderName
		{
			get
			{
				return this.encodingEncoding.HeaderName;
			}
		}

		public override string WebName
		{
			get
			{
				return this.encodingEncoding.WebName;
			}
		}

		public override int WindowsCodePage
		{
			get
			{
				return this.encodingEncoding.WindowsCodePage;
			}
		}

		public override bool IsBrowserDisplay
		{
			get
			{
				return this.encodingEncoding.IsBrowserDisplay;
			}
		}

		public override bool IsBrowserSave
		{
			get
			{
				return this.encodingEncoding.IsBrowserSave;
			}
		}

		public override bool IsMailNewsDisplay
		{
			get
			{
				return this.encodingEncoding.IsMailNewsDisplay;
			}
		}

		public override bool IsMailNewsSave
		{
			get
			{
				return this.encodingEncoding.IsMailNewsSave;
			}
		}

		public override bool IsSingleByte
		{
			get
			{
				return this.encodingEncoding.IsSingleByte;
			}
		}

		public override byte[] GetPreamble()
		{
			return this.encodingEncoding.GetPreamble();
		}

		public override int GetMaxByteCount(int charCount)
		{
			return this.encodingEncoding.GetMaxByteCount(charCount);
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return this.decodingEncoding.GetMaxCharCount(byteCount);
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return this.encodingEncoding.GetByteCount(chars, index, count);
		}

		public override int GetByteCount(string s)
		{
			return this.encodingEncoding.GetByteCount(s);
		}

		public unsafe override int GetByteCount(char* chars, int count)
		{
			return this.encodingEncoding.GetByteCount(chars, count);
		}

		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return this.encodingEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return this.encodingEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return this.encodingEncoding.GetBytes(chars, charCount, bytes, byteCount);
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return this.decodingEncoding.GetCharCount(bytes, index, count);
		}

		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return this.decodingEncoding.GetCharCount(bytes, count);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return this.decodingEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
		}

		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			return this.decodingEncoding.GetChars(bytes, byteCount, chars, charCount);
		}

		public override string GetString(byte[] bytes, int index, int count)
		{
			return this.decodingEncoding.GetString(bytes, index, count);
		}

		public override Decoder GetDecoder()
		{
			return this.decodingEncoding.GetDecoder();
		}

		public override Encoder GetEncoder()
		{
			return this.encodingEncoding.GetEncoder();
		}

		public override object Clone()
		{
			return (Encoding)base.MemberwiseClone();
		}

		private Encoding encodingEncoding;

		private Encoding decodingEncoding;
	}
}
