using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal struct RtfTextElement
	{
		internal RtfTextElement(RtfToken token)
		{
			this.token = token;
		}

		public RunTextType TextType
		{
			get
			{
				return this.token.ElementTextType;
			}
		}

		public TextMapping TextMapping
		{
			get
			{
				return this.token.TextMapping;
			}
		}

		public int Length
		{
			get
			{
				return this.RawLength;
			}
		}

		public bool IsAnyWhitespace
		{
			get
			{
				return this.TextType <= RunTextType.UnusualWhitespace;
			}
		}

		public bool IsLiteral
		{
			get
			{
				return false;
			}
		}

		public char[] RawBuffer
		{
			get
			{
				return this.token.CharBuffer;
			}
		}

		public int RawOffset
		{
			get
			{
				return this.token.ElementOffset;
			}
		}

		public int RawLength
		{
			get
			{
				return this.token.ElementLength;
			}
		}

		public int Literal
		{
			get
			{
				return 0;
			}
		}

		public int Value
		{
			get
			{
				return 0;
			}
		}

		public bool Eof
		{
			get
			{
				return true;
			}
		}

		public int Read(char[] buffer, int offset, int count)
		{
			return 0;
		}

		public void WriteTo(ITextSink sink)
		{
			sink.Write(this.token.CharBuffer, this.token.ElementOffset, this.token.ElementLength);
		}

		public string GetString(int maxLength)
		{
			return new string(this.token.CharBuffer, this.token.ElementOffset, this.token.ElementLength);
		}

		[Conditional("DEBUG")]
		private void AssertCurrent()
		{
		}

		private RtfToken token;
	}
}
