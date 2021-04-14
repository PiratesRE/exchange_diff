using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct HashCode
	{
		public HashCode(bool ignore)
		{
			this.offset = 0;
			this.hash1 = (this.hash2 = 5381);
		}

		public static int CalculateEmptyHash()
		{
			return 371857150;
		}

		public unsafe static int Calculate(string obj)
		{
			int num = 5381;
			int num2 = num;
			fixed (char* ptr = obj)
			{
				char* ptr2 = ptr;
				for (int i = obj.Length; i > 0; i -= 2)
				{
					num = ((num << 5) + num ^ (int)(*ptr2));
					if (i < 2)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ptr2[1]);
					ptr2 += 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public unsafe static int Calculate(BufferString obj)
		{
			int num = 5381;
			int num2 = num;
			fixed (char* buffer = obj.Buffer)
			{
				char* ptr = buffer + obj.Offset;
				for (int i = obj.Length; i > 0; i -= 2)
				{
					num = ((num << 5) + num ^ (int)(*ptr));
					if (i == 1)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ptr[1]);
					ptr += 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public unsafe static int CalculateLowerCase(string obj)
		{
			int num = 5381;
			int num2 = num;
			fixed (char* ptr = obj)
			{
				char* ptr2 = ptr;
				for (int i = obj.Length; i > 0; i -= 2)
				{
					num = ((num << 5) + num ^ (int)ParseSupport.ToLowerCase(*ptr2));
					if (i == 1)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ParseSupport.ToLowerCase(ptr2[1]));
					ptr2 += 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public unsafe static int CalculateLowerCase(BufferString obj)
		{
			int num = 5381;
			int num2 = num;
			fixed (char* buffer = obj.Buffer)
			{
				char* ptr = buffer + obj.Offset;
				for (int i = obj.Length; i > 0; i -= 2)
				{
					num = ((num << 5) + num ^ (int)ParseSupport.ToLowerCase(*ptr));
					if (i == 1)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ParseSupport.ToLowerCase(ptr[1]));
					ptr += 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public unsafe static int Calculate(char[] buffer, int offset, int length)
		{
			int num = 5381;
			int num2 = num;
			HashCode.CheckArgs(buffer, offset, length);
			fixed (char* ptr = buffer)
			{
				char* ptr2 = ptr + offset;
				while (length > 0)
				{
					num = ((num << 5) + num ^ (int)(*ptr2));
					if (length == 1)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ptr2[1]);
					ptr2 += 2;
					length -= 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public unsafe static int CalculateLowerCase(char[] buffer, int offset, int length)
		{
			int num = 5381;
			int num2 = num;
			HashCode.CheckArgs(buffer, offset, length);
			fixed (char* ptr = buffer)
			{
				char* ptr2 = ptr + offset;
				while (length > 0)
				{
					num = ((num << 5) + num ^ (int)ParseSupport.ToLowerCase(*ptr2));
					if (length == 1)
					{
						break;
					}
					num2 = ((num2 << 5) + num2 ^ (int)ParseSupport.ToLowerCase(ptr2[1]));
					ptr2 += 2;
					length -= 2;
				}
			}
			return num + num2 * 1566083941;
		}

		public void Initialize()
		{
			this.offset = 0;
			this.hash1 = (this.hash2 = 5381);
		}

		public unsafe void Advance(char* s, int len)
		{
			if ((this.offset & 1) != 0)
			{
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)(*s));
				s++;
				len--;
				this.offset++;
			}
			this.offset += len;
			while (len > 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)(*s));
				if (len == 1)
				{
					return;
				}
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)s[1]);
				s += 2;
				len -= 2;
			}
		}

		public unsafe void AdvanceLowerCase(char* s, int len)
		{
			if ((this.offset & 1) != 0)
			{
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)ParseSupport.ToLowerCase(*s));
				s++;
				len--;
				this.offset++;
			}
			this.offset += len;
			while (len > 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)ParseSupport.ToLowerCase(*s));
				if (len == 1)
				{
					return;
				}
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)ParseSupport.ToLowerCase(s[1]));
				s += 2;
				len -= 2;
			}
		}

		public void Advance(int ucs32)
		{
			if (ucs32 >= 65536)
			{
				char c = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
				char c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
				if (((this.offset += 2) & 1) == 0)
				{
					this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c);
					this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c2);
					return;
				}
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c);
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c2);
				return;
			}
			else
			{
				if ((this.offset++ & 1) == 0)
				{
					this.hash1 = ((this.hash1 << 5) + this.hash1 ^ ucs32);
					return;
				}
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ ucs32);
				return;
			}
		}

		public void AdvanceLowerCase(int ucs32)
		{
			if (ucs32 < 65536)
			{
				this.AdvanceLowerCase((char)ucs32);
				return;
			}
			char c = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
			char c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
			if (((this.offset += 2) & 1) == 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c);
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c2);
				return;
			}
			this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c);
			this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c2);
		}

		public void Advance(char c)
		{
			if ((this.offset++ & 1) == 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c);
				return;
			}
			this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c);
		}

		public int AdvanceAndFinalizeHash(char c)
		{
			if ((this.offset++ & 1) == 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)c);
			}
			else
			{
				this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)c);
			}
			return this.hash1 + this.hash2 * 1566083941;
		}

		public void AdvanceLowerCase(char c)
		{
			if ((this.offset++ & 1) == 0)
			{
				this.hash1 = ((this.hash1 << 5) + this.hash1 ^ (int)ParseSupport.ToLowerCase(c));
				return;
			}
			this.hash2 = ((this.hash2 << 5) + this.hash2 ^ (int)ParseSupport.ToLowerCase(c));
		}

		public unsafe void Advance(BufferString obj)
		{
			fixed (char* buffer = obj.Buffer)
			{
				this.Advance(buffer + obj.Offset, obj.Length);
			}
		}

		public unsafe void AdvanceLowerCase(BufferString obj)
		{
			fixed (char* buffer = obj.Buffer)
			{
				this.AdvanceLowerCase(buffer + obj.Offset, obj.Length);
			}
		}

		public unsafe void Advance(char[] buffer, int offset, int length)
		{
			HashCode.CheckArgs(buffer, offset, length);
			fixed (char* ptr = buffer)
			{
				this.Advance(ptr + offset, length);
			}
		}

		public unsafe void AdvanceLowerCase(char[] buffer, int offset, int length)
		{
			HashCode.CheckArgs(buffer, offset, length);
			fixed (char* ptr = buffer)
			{
				this.AdvanceLowerCase(ptr + offset, length);
			}
		}

		private static void CheckArgs(char[] buffer, int offset, int length)
		{
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (offset + length < offset || offset + length > num)
			{
				throw new ArgumentOutOfRangeException("offset + length");
			}
		}

		public int FinalizeHash()
		{
			return this.hash1 + this.hash2 * 1566083941;
		}

		private int hash1;

		private int hash2;

		private int offset;
	}
}
