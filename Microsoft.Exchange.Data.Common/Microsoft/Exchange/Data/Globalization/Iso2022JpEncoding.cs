using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization.Iso2022Jp;

namespace Microsoft.Exchange.Data.Globalization
{
	internal class Iso2022JpEncoding : Encoding
	{
		internal Iso2022DecodingMode KillSwitch
		{
			get
			{
				return Iso2022JpEncoding.InternalReadKillSwitch();
			}
		}

		public Iso2022JpEncoding(int codePage) : base(codePage)
		{
			if (codePage == 50220)
			{
				this.defaultEncoding = Encoding.GetEncoding(50220);
				return;
			}
			if (codePage == 50221)
			{
				this.defaultEncoding = Encoding.GetEncoding(50221);
				return;
			}
			if (codePage == 50222)
			{
				this.defaultEncoding = Encoding.GetEncoding(50222);
				return;
			}
			string paramName = string.Format("Iso2022JpEncoding does not support codepage {0}", codePage);
			throw new ArgumentException("codePage", paramName);
		}

		internal static Iso2022DecodingMode InternalReadKillSwitch()
		{
			switch (RegistryConfigManager.Iso2022JpEncodingOverride)
			{
			default:
				return Iso2022DecodingMode.Default;
			case 1:
				return Iso2022DecodingMode.Override;
			case 2:
				return Iso2022DecodingMode.Throw;
			}
		}

		public override int CodePage
		{
			get
			{
				return this.defaultEncoding.CodePage;
			}
		}

		public override string BodyName
		{
			get
			{
				return this.defaultEncoding.BodyName;
			}
		}

		public override string EncodingName
		{
			get
			{
				return this.defaultEncoding.EncodingName;
			}
		}

		public override string HeaderName
		{
			get
			{
				return this.defaultEncoding.HeaderName;
			}
		}

		public override string WebName
		{
			get
			{
				return this.defaultEncoding.WebName;
			}
		}

		public override int WindowsCodePage
		{
			get
			{
				return this.defaultEncoding.WindowsCodePage;
			}
		}

		public override bool IsBrowserDisplay
		{
			get
			{
				return this.defaultEncoding.IsBrowserDisplay;
			}
		}

		public override bool IsBrowserSave
		{
			get
			{
				return this.defaultEncoding.IsBrowserSave;
			}
		}

		public override bool IsMailNewsDisplay
		{
			get
			{
				return this.defaultEncoding.IsMailNewsDisplay;
			}
		}

		public override bool IsMailNewsSave
		{
			get
			{
				return this.defaultEncoding.IsMailNewsSave;
			}
		}

		public override bool IsSingleByte
		{
			get
			{
				return this.defaultEncoding.IsSingleByte;
			}
		}

		public override byte[] GetPreamble()
		{
			return this.defaultEncoding.GetPreamble();
		}

		public override int GetMaxByteCount(int charCount)
		{
			return this.defaultEncoding.GetMaxByteCount(charCount);
		}

		public override int GetMaxCharCount(int byteCount)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetMaxCharCount(byteCount);
			case Iso2022DecodingMode.Override:
				return this.defaultEncoding.GetMaxCharCount(byteCount);
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return this.defaultEncoding.GetByteCount(chars, index, count);
		}

		public override int GetByteCount(string s)
		{
			return this.defaultEncoding.GetByteCount(s);
		}

		public unsafe override int GetByteCount(char* chars, int count)
		{
			return this.defaultEncoding.GetByteCount(chars, count);
		}

		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return this.defaultEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return this.defaultEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return this.defaultEncoding.GetBytes(chars, charCount, bytes, byteCount);
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetCharCount(bytes, index, count);
			case Iso2022DecodingMode.Override:
			{
				Decoder decoder = this.GetDecoder();
				return decoder.GetCharCount(bytes, index, count);
			}
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetCharCount(bytes, count);
			case Iso2022DecodingMode.Override:
				throw new NotImplementedException();
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
			case Iso2022DecodingMode.Override:
			{
				Decoder decoder = this.GetDecoder();
				return decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
			}
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetChars(bytes, byteCount, chars, charCount);
			case Iso2022DecodingMode.Override:
				throw new NotImplementedException();
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public override string GetString(byte[] bytes, int index, int count)
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
				return this.defaultEncoding.GetString(bytes, index, count);
			case Iso2022DecodingMode.Override:
			{
				Decoder decoder = this.GetDecoder();
				char[] array = new char[this.GetMaxCharCount(count)];
				int chars = decoder.GetChars(bytes, index, count, array, 0);
				return new string(array, 0, chars);
			}
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public override Decoder GetDecoder()
		{
			switch (this.KillSwitch)
			{
			case Iso2022DecodingMode.Default:
			case Iso2022DecodingMode.Override:
				return new Iso2022JpDecoder(this);
			case Iso2022DecodingMode.Throw:
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException();
			}
		}

		public override Encoder GetEncoder()
		{
			return this.defaultEncoding.GetEncoder();
		}

		public override object Clone()
		{
			return (Iso2022JpEncoding)base.MemberwiseClone();
		}

		internal Encoding DefaultEncoding
		{
			get
			{
				return this.defaultEncoding;
			}
		}

		private Encoding defaultEncoding;
	}
}
