using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Css;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct ScratchBuffer
	{
		public char[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public int Offset
		{
			get
			{
				return 0;
			}
		}

		public int Length
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public int Capacity
		{
			get
			{
				if (this.buffer != null)
				{
					return this.buffer.Length;
				}
				return 64;
			}
		}

		public BufferString BufferString
		{
			get
			{
				return new BufferString(this.buffer, 0, this.count);
			}
		}

		public char this[int offset]
		{
			get
			{
				return this.buffer[offset];
			}
			set
			{
				this.buffer[offset] = value;
			}
		}

		public BufferString SubString(int offset, int count)
		{
			return new BufferString(this.buffer, offset, count);
		}

		public void Reset()
		{
			this.count = 0;
		}

		public void Reset(int space)
		{
			this.count = 0;
			if (this.buffer == null || this.buffer.Length < space)
			{
				this.buffer = new char[space];
			}
		}

		public bool AppendRtfTokenText(RtfToken token, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = this.GetSpace(maxSize)) != 0 && (num2 = token.Text.Read(this.buffer, this.count, num2)) != 0)
			{
				this.count += num2;
				num += num2;
			}
			return num != 0;
		}

		public bool AppendTokenText(Token token, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = this.GetSpace(maxSize)) != 0 && (num2 = token.Text.Read(this.buffer, this.count, num2)) != 0)
			{
				this.count += num2;
				num += num2;
			}
			return num != 0;
		}

		public bool AppendHtmlAttributeValue(HtmlAttribute attr, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = this.GetSpace(maxSize)) != 0 && (num2 = attr.Value.Read(this.buffer, this.count, num2)) != 0)
			{
				this.count += num2;
				num += num2;
			}
			return num != 0;
		}

		public bool AppendCssPropertyValue(CssProperty prop, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = this.GetSpace(maxSize)) != 0 && (num2 = prop.Value.Read(this.buffer, this.count, num2)) != 0)
			{
				this.count += num2;
				num += num2;
			}
			return num != 0;
		}

		public int AppendInt(int value)
		{
			int num = 1;
			bool flag = false;
			if (value < 0)
			{
				flag = true;
				value = -value;
				num++;
				if (value < 0)
				{
					value = int.MaxValue;
				}
			}
			int i = value;
			while (i >= 10)
			{
				i /= 10;
				num++;
			}
			this.EnsureSpace(num);
			int num2 = this.count + num;
			while (value >= 10)
			{
				this.buffer[--num2] = (char)(value % 10 + 48);
				value /= 10;
			}
			this.buffer[--num2] = (char)(value + 48);
			if (flag)
			{
				this.buffer[num2 - 1] = '-';
			}
			this.count += num;
			return num;
		}

		public int AppendFractional(int value, int decimalPoint)
		{
			int num = this.AppendInt(value / decimalPoint);
			if (value % decimalPoint != 0)
			{
				if (value < 0)
				{
					value = -value;
				}
				int num2 = (int)(((long)value * 100L + (long)(decimalPoint / 2)) / (long)decimalPoint) % 100;
				if (num2 != 0)
				{
					num += this.Append('.');
					if (num2 % 10 == 0)
					{
						num2 /= 10;
					}
					num += this.AppendInt(num2);
				}
			}
			return num;
		}

		public int AppendHex2(uint value)
		{
			this.EnsureSpace(2);
			uint num = value >> 4 & 15U;
			if (num < 10U)
			{
				this.buffer[this.count++] = (char)(num + 48U);
			}
			else
			{
				this.buffer[this.count++] = (char)(num - 10U + 65U);
			}
			num = (value & 15U);
			if (num < 10U)
			{
				this.buffer[this.count++] = (char)(num + 48U);
			}
			else
			{
				this.buffer[this.count++] = (char)(num - 10U + 65U);
			}
			return 2;
		}

		public int Append(char ch)
		{
			return this.Append(ch, int.MaxValue);
		}

		public int Append(char ch, int maxSize)
		{
			if (this.GetSpace(maxSize) == 0)
			{
				return 0;
			}
			this.buffer[this.count++] = ch;
			return 1;
		}

		public int Append(string str)
		{
			return this.Append(str, int.MaxValue);
		}

		public int Append(string str, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = Math.Min(this.GetSpace(maxSize), str.Length - num)) != 0)
			{
				str.CopyTo(num, this.buffer, this.count, num2);
				this.count += num2;
				num += num2;
			}
			return num;
		}

		public int Append(char[] buffer, int offset, int length)
		{
			return this.Append(buffer, offset, length, int.MaxValue);
		}

		public int Append(char[] buffer, int offset, int length, int maxSize)
		{
			int num = 0;
			int num2;
			while ((num2 = Math.Min(this.GetSpace(maxSize), length)) != 0)
			{
				System.Buffer.BlockCopy(buffer, offset * 2, this.buffer, this.count * 2, num2 * 2);
				this.count += num2;
				offset += num2;
				length -= num2;
				num += num2;
			}
			return num;
		}

		public string ToString(int offset, int count)
		{
			return new string(this.buffer, offset, count);
		}

		public void DisposeBuffer()
		{
			this.buffer = null;
			this.count = 0;
		}

		private int GetSpace(int maxSize)
		{
			if (this.count >= maxSize)
			{
				return 0;
			}
			if (this.buffer == null)
			{
				this.buffer = new char[64];
			}
			else if (this.buffer.Length == this.count)
			{
				char[] dst = new char[this.buffer.Length * 2];
				System.Buffer.BlockCopy(this.buffer, 0, dst, 0, this.count * 2);
				this.buffer = dst;
			}
			return this.buffer.Length - this.count;
		}

		private void EnsureSpace(int space)
		{
			if (this.buffer == null)
			{
				this.buffer = new char[Math.Max(space, 64)];
				return;
			}
			if (this.buffer.Length - this.count < space)
			{
				char[] dst = new char[Math.Max(this.buffer.Length * 2, this.count + space)];
				System.Buffer.BlockCopy(this.buffer, 0, dst, 0, this.count * 2);
				this.buffer = dst;
			}
		}

		private char[] buffer;

		private int count;
	}
}
