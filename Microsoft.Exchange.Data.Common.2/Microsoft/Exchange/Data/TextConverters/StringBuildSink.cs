using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class StringBuildSink : ITextSinkEx, ITextSink
	{
		public StringBuildSink()
		{
			this.sb = new StringBuilder();
		}

		public bool IsEnough
		{
			get
			{
				return this.sb.Length >= this.maxLength;
			}
		}

		public void Reset(int maxLength)
		{
			this.maxLength = maxLength;
			this.sb.Length = 0;
		}

		public void Write(char[] buffer, int offset, int count)
		{
			count = Math.Min(count, this.maxLength - this.sb.Length);
			this.sb.Append(buffer, offset, count);
		}

		public void Write(int ucs32Char)
		{
			if (Token.LiteralLength(ucs32Char) == 1)
			{
				this.sb.Append((char)ucs32Char);
				return;
			}
			this.sb.Append(Token.LiteralFirstChar(ucs32Char));
			if (!this.IsEnough)
			{
				this.sb.Append(Token.LiteralLastChar(ucs32Char));
			}
		}

		public void Write(string value)
		{
			this.sb.Append(value);
		}

		public void WriteNewLine()
		{
			this.sb.Append('\r');
			if (!this.IsEnough)
			{
				this.sb.Append('\n');
			}
		}

		public override string ToString()
		{
			return this.sb.ToString();
		}

		private StringBuilder sb;

		private int maxLength;
	}
}
