using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class HttpTokenReader
	{
		public HttpTokenReader(string content)
		{
			this._value = content;
			if (!string.IsNullOrEmpty(this._value))
			{
				this._index = 0;
				this._c = this._value[0];
			}
		}

		public void SkipLinearWhiteSpace(bool optional)
		{
			if (!this.EndOfContent)
			{
				if (this._c == '\r')
				{
					this.InternalMoveNext();
				}
				if (this._c == '\n')
				{
					this.InternalMoveNext();
				}
			}
			if (!optional)
			{
				this.ValidatePosition();
				if (this._c != ' ' && this._c != '\t')
				{
					throw new FormatException("Missing required whitespace.");
				}
				this.InternalMoveNext();
			}
			while ((!this.EndOfContent && this._c == ' ') || this._c == '\t')
			{
				this.InternalMoveNext();
			}
		}

		public void SkipChar(char c)
		{
			this.ValidatePosition();
			if (c != this._c)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "The current character does not match the required value '{0}'.", new object[]
				{
					c
				}));
			}
			this.InternalMoveNext();
		}

		public char PeekChar()
		{
			this.ValidatePosition();
			return this._c;
		}

		public string ReadTokenOrQuotedString(bool optional)
		{
			if (optional)
			{
				if (this.EndOfContent || (this._c != '"' && this.IsSeparatorChar))
				{
					return string.Empty;
				}
			}
			else
			{
				this.ValidatePosition();
			}
			if (this._c == '"')
			{
				return this.ReadQuotedString();
			}
			return this.ReadToken();
		}

		public string ReadToken()
		{
			this.ValidatePosition();
			if (!this.IsTokenChar)
			{
				throw new FormatException("Missing required token.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._c);
			this.InternalMoveNext();
			while (!this.EndOfContent && this.IsTokenChar)
			{
				stringBuilder.Append(this._c);
				this.InternalMoveNext();
			}
			return stringBuilder.ToString();
		}

		public string ReadQuotedString()
		{
			this.SkipChar('"');
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			while (!this.EndOfContent)
			{
				if (flag && this.IsChar)
				{
					stringBuilder.Append(this._c);
					flag = false;
				}
				else if (this._c == '\\')
				{
					flag = true;
				}
				else
				{
					if (!this.IsTextChar || this._c == '"')
					{
						break;
					}
					stringBuilder.Append(this._c);
				}
				this.InternalMoveNext();
			}
			this.SkipChar('"');
			return stringBuilder.ToString();
		}

		public bool EndOfContent
		{
			get
			{
				return this._value == null || this._index >= this._value.Length;
			}
		}

		private bool IsSeparatorChar
		{
			get
			{
				return !this.EndOfContent && (this._c == '(' || this._c == ')' || this._c == '<' || this._c == '>' || this._c == '@' || this._c == ',' || this._c == ';' || this._c == ':' || this._c == '\\' || this._c == '"' || this._c == '/' || this._c == '[' || this._c == ']' || this._c == '?' || this._c == '=' || this._c == '{' || this._c == '}' || this._c == ' ' || this._c == '\t');
			}
		}

		private bool IsTokenChar
		{
			get
			{
				return !this.EndOfContent && this._c > '\u001f' && this._c < '\u007f' && !this.IsSeparatorChar;
			}
		}

		private bool IsTextChar
		{
			get
			{
				return !this.EndOfContent && this._c > '\u001f' && this._c < 'Ā' && this._c != '\u007f';
			}
		}

		private bool IsChar
		{
			get
			{
				return !this.EndOfContent && this._c >= '\0' && this._c <= '\u007f';
			}
		}

		private void ValidatePosition()
		{
			if (this.EndOfContent)
			{
				throw new FormatException("Unexpected end of content.");
			}
		}

		private void InternalMoveNext()
		{
			if (this.EndOfContent)
			{
				throw new InvalidOperationException("Internal parser error.");
			}
			this._index++;
			if (!this.EndOfContent)
			{
				this._c = this._value[this._index];
				return;
			}
			this._c = '\0';
		}

		private readonly string _value;

		private char _c;

		private int _index;
	}
}
