using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct BufferString
	{
		public BufferString(char[] buffer, int offset, int count)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.count = count;
		}

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
				return this.offset;
			}
		}

		public int Length
		{
			get
			{
				return this.count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.count == 0;
			}
		}

		public char this[int index]
		{
			get
			{
				return this.buffer[this.offset + index];
			}
		}

		public static int CompareLowerCaseStringToBufferStringIgnoreCase(string left, BufferString right)
		{
			int num = Math.Min(left.Length, right.Length);
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)(left[i] - ParseSupport.ToLowerCase(right[i]));
				if (num2 != 0)
				{
					return num2;
				}
			}
			return left.Length - right.Length;
		}

		public void Set(char[] buffer, int offset, int count)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.count = count;
		}

		public BufferString SubString(int offset, int count)
		{
			return new BufferString(this.buffer, this.offset + offset, count);
		}

		public void Trim(int offset, int count)
		{
			this.offset += offset;
			this.count = count;
		}

		public void TrimWhitespace()
		{
			while (this.count != 0 && ParseSupport.WhitespaceCharacter(this.buffer[this.offset]))
			{
				this.offset++;
				this.count--;
			}
			if (this.count != 0)
			{
				int num = this.offset + this.count - 1;
				while (ParseSupport.WhitespaceCharacter(this.buffer[num--]))
				{
					this.count--;
				}
			}
		}

		public bool EqualsToString(string rightPart)
		{
			if (this.count != rightPart.Length)
			{
				return false;
			}
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (this.buffer[this.offset + i] != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
		{
			if (this.count != rightPart.Length)
			{
				return false;
			}
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (ParseSupport.ToLowerCase(this.buffer[this.offset + i]) != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool StartsWithLowerCaseStringIgnoreCase(string rightPart)
		{
			if (this.count < rightPart.Length)
			{
				return false;
			}
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (ParseSupport.ToLowerCase(this.buffer[this.offset + i]) != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool StartsWithString(string rightPart)
		{
			if (this.count < rightPart.Length)
			{
				return false;
			}
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (this.buffer[this.offset + i] != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool EndsWithLowerCaseStringIgnoreCase(string rightPart)
		{
			if (this.count < rightPart.Length)
			{
				return false;
			}
			int num = this.offset + this.count - rightPart.Length;
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (ParseSupport.ToLowerCase(this.buffer[num + i]) != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool EndsWithString(string rightPart)
		{
			if (this.count < rightPart.Length)
			{
				return false;
			}
			int num = this.offset + this.count - rightPart.Length;
			for (int i = 0; i < rightPart.Length; i++)
			{
				if (this.buffer[num + i] != rightPart[i])
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			if (this.buffer == null)
			{
				return null;
			}
			if (this.count != 0)
			{
				return new string(this.buffer, this.offset, this.count);
			}
			return string.Empty;
		}

		[Conditional("DEBUG")]
		private static void AssertStringIsLowerCase(string rightPart)
		{
		}

		public static readonly BufferString Null = default(BufferString);

		private char[] buffer;

		private int offset;

		private int count;
	}
}
