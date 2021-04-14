using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct TokenRun
	{
		internal TokenRun(Token token)
		{
			this.token = token;
		}

		public RunType Type
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Type;
			}
		}

		public bool IsTextRun
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Type >= (RunType)2147483648U;
			}
		}

		public bool IsSpecial
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Type == RunType.Special;
			}
		}

		public bool IsNormal
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Type == (RunType)2147483648U;
			}
		}

		public bool IsLiteral
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Type == (RunType)3221225472U;
			}
		}

		public RunTextType TextType
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].TextType;
			}
		}

		public char[] RawBuffer
		{
			get
			{
				return this.token.Buffer;
			}
		}

		public int RawOffset
		{
			get
			{
				return this.token.WholePosition.RunOffset;
			}
		}

		public int RawLength
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Length;
			}
		}

		public uint Kind
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Kind;
			}
		}

		public int Literal
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Value;
			}
		}

		public int Length
		{
			get
			{
				if (this.IsNormal)
				{
					return this.RawLength;
				}
				if (!this.IsLiteral)
				{
					return 0;
				}
				return Token.LiteralLength(this.Literal);
			}
		}

		public int Value
		{
			get
			{
				return this.token.RunList[this.token.WholePosition.Run].Value;
			}
		}

		public char FirstChar
		{
			get
			{
				if (!this.IsLiteral)
				{
					return this.RawBuffer[this.RawOffset];
				}
				return Token.LiteralFirstChar(this.Literal);
			}
		}

		public char LastChar
		{
			get
			{
				if (!this.IsLiteral)
				{
					return this.RawBuffer[this.RawOffset + this.RawLength - 1];
				}
				return Token.LiteralLastChar(this.Literal);
			}
		}

		public bool IsAnyWhitespace
		{
			get
			{
				return this.TextType <= RunTextType.UnusualWhitespace;
			}
		}

		public int ReadLiteral(char[] buffer)
		{
			int value = this.token.RunList[this.token.WholePosition.Run].Value;
			int num = Token.LiteralLength(value);
			if (num == 1)
			{
				buffer[0] = (char)value;
				return 1;
			}
			buffer[0] = Token.LiteralFirstChar(value);
			buffer[1] = Token.LiteralLastChar(value);
			return 2;
		}

		public string GetString(int maxSize)
		{
			int run = this.token.WholePosition.Run;
			Token.RunEntry[] runList = this.token.RunList;
			RunType type = runList[run].Type;
			if (type == (RunType)2147483648U)
			{
				return new string(this.token.Buffer, this.token.WholePosition.RunOffset, Math.Min(maxSize, runList[run].Length));
			}
			if (type != (RunType)3221225472U)
			{
				return string.Empty;
			}
			if (this.Length == 1)
			{
				return this.FirstChar.ToString();
			}
			Token.Fragment fragment = default(Token.Fragment);
			fragment.Initialize(run, this.token.WholePosition.RunOffset);
			fragment.Tail = fragment.Head + 1;
			return this.token.GetString(ref fragment, maxSize);
		}

		[Conditional("DEBUG")]
		private void AssertCurrent()
		{
		}

		private Token token;
	}
}
