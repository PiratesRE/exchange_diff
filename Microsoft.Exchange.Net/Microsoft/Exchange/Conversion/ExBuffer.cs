using System;
using System.Security;

namespace Microsoft.Exchange.Conversion
{
	internal static class ExBuffer
	{
		public unsafe static int IndexOf(byte[] buffer, byte val, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int result;
			try
			{
				fixed (byte* ptr = buffer)
				{
					byte* ptr2 = ptr + offset;
					byte* ptr3 = ptr2 + count;
					if ((long)(ptr3 - ptr2) >= 36L)
					{
						uint num = (uint)((int)val | (int)val << 8);
						num |= num << 16;
						uint num2 = *(uint*)ptr2 ^ num;
						if ((num2 & 255U) != 0U)
						{
							if ((num2 & 65280U) != 0U)
							{
								if ((num2 & 16711680U) != 0U)
								{
									if ((num2 & 4278190080U) != 0U)
									{
										ptr2 += 4U - (ptr2 & 3U);
										ptr3 -= 32;
										do
										{
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
											num2 = (*(uint*)ptr2 ^ num);
											if ((num2 & 255U) == 0U)
											{
												goto IL_2B8;
											}
											if ((num2 & 65280U) == 0U)
											{
												goto IL_2C4;
											}
											if ((num2 & 16711680U) == 0U)
											{
												goto IL_2D3;
											}
											if ((num2 & 4278190080U) == 0U)
											{
												goto IL_2E2;
											}
											ptr2 += 4;
										}
										while (ptr2 < ptr3);
										ptr3 += 32;
										goto IL_29A;
									}
									IL_2E2:
									return (int)((long)(ptr2 - ptr) + 3L);
								}
								IL_2D3:
								return (int)((long)(ptr2 - ptr) + 2L);
							}
							IL_2C4:
							return (int)((long)(ptr2 - ptr) + 1L);
						}
						goto IL_2B8;
					}
					IL_29A:
					if (ptr2 < ptr3)
					{
						ptr2--;
						while (++ptr2 != ptr3)
						{
							if (*ptr2 == val)
							{
								goto IL_2B8;
							}
						}
					}
					return -1;
					IL_2B8:
					result = (int)((long)(ptr2 - ptr));
				}
			}
			finally
			{
				byte* ptr = null;
			}
			return result;
		}

		public unsafe static SecureString ToSecureString(char[] chars)
		{
			if (chars == null || chars.Length == 0)
			{
				return new SecureString();
			}
			fixed (char* ptr = chars)
			{
				return new SecureString(ptr, chars.Length);
			}
		}
	}
}
