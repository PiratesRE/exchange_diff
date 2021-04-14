using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class UrlCompareSink : ITextSink
	{
		public void Initialize(string url)
		{
			this.url = url;
			this.urlPosition = 0;
		}

		public void Reset()
		{
			this.urlPosition = -1;
		}

		public bool IsActive
		{
			get
			{
				return this.urlPosition >= 0;
			}
		}

		public bool IsMatch
		{
			get
			{
				return this.urlPosition == this.url.Length;
			}
		}

		public bool IsEnough
		{
			get
			{
				return this.urlPosition < 0;
			}
		}

		public void Write(char[] buffer, int offset, int count)
		{
			if (this.IsActive)
			{
				int num = offset + count;
				while (offset < num)
				{
					if (this.urlPosition == 0)
					{
						if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
						{
							offset++;
							continue;
						}
					}
					else if (this.urlPosition == this.url.Length)
					{
						if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
						{
							offset++;
							continue;
						}
						this.urlPosition = -1;
						return;
					}
					if (buffer[offset] != this.url[this.urlPosition])
					{
						this.urlPosition = -1;
						return;
					}
					offset++;
					this.urlPosition++;
				}
			}
		}

		public void Write(int ucs32Char)
		{
			if (Token.LiteralLength(ucs32Char) != 1)
			{
				this.urlPosition = -1;
				return;
			}
			if (this.urlPosition == 0)
			{
				if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
				{
					return;
				}
			}
			else if (this.urlPosition == this.url.Length)
			{
				if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
				{
					return;
				}
				this.urlPosition = -1;
				return;
			}
			if ((char)ucs32Char != this.url[this.urlPosition])
			{
				this.urlPosition = -1;
				return;
			}
			this.urlPosition++;
		}

		private string url;

		private int urlPosition;
	}
}
