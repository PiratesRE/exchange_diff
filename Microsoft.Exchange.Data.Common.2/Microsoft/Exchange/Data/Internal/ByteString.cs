using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Internal
{
	internal static class ByteString
	{
		public unsafe static int IndexOf(byte[] buffer, byte val, int offset, int count)
		{
			fixed (byte* ptr = buffer)
			{
				byte* ptr2 = ptr + offset;
				while ((ptr2 & 3) != 0)
				{
					int result;
					if (count == 0)
					{
						result = -1;
					}
					else
					{
						if (*ptr2 != val)
						{
							count--;
							ptr2++;
							continue;
						}
						result = (int)((long)(ptr2 - ptr));
					}
					return result;
				}
				uint num = (uint)((int)val + ((int)val << 8));
				num += num << 16;
				while (count >= 32)
				{
					offset = 0;
					uint num2 = *(uint*)ptr2 ^ num;
					if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
					{
						offset += 4;
						num2 = (*(uint*)(ptr2 + 4) ^ num);
						if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
						{
							offset += 4;
							num2 = (*(uint*)(ptr2 + 8) ^ num);
							if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
							{
								offset += 4;
								num2 = (*(uint*)(ptr2 + 12) ^ num);
								if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
								{
									offset += 4;
									num2 = (*(uint*)(ptr2 + 16) ^ num);
									if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
									{
										offset += 4;
										num2 = (*(uint*)(ptr2 + 20) ^ num);
										if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
										{
											offset += 4;
											num2 = (*(uint*)(ptr2 + 24) ^ num);
											if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
											{
												offset += 4;
												num2 = (*(uint*)(ptr2 + 28) ^ num);
												if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
												{
													ptr2 += 32;
													count -= 32;
													continue;
												}
											}
										}
									}
								}
							}
						}
					}
					ptr2 += offset;
					int num3 = (int)((long)(ptr2 - ptr));
					if (*ptr2 == val)
					{
						return num3;
					}
					if (ptr2[1] == val)
					{
						return num3 + 1;
					}
					if (ptr2[2] == val)
					{
						return num3 + 2;
					}
					if (ptr2[3] == val)
					{
						return num3 + 3;
					}
					ptr2 += 4;
					count -= offset + 4;
				}
				while (count != 0)
				{
					if (*ptr2 == val)
					{
						return (int)((long)(ptr2 - ptr));
					}
					count--;
					ptr2++;
				}
				return -1;
			}
		}

		public unsafe static int IndexOf(byte[] buffer, byte val, int offset, int count, out bool containsBinary)
		{
			containsBinary = false;
			fixed (byte* ptr = buffer)
			{
				byte* ptr2 = ptr + offset;
				while ((ptr2 & 3) != 0)
				{
					int result;
					if (count == 0)
					{
						result = -1;
					}
					else
					{
						if ((*ptr2 & 128) != 0)
						{
							containsBinary = true;
						}
						if (*ptr2 != val)
						{
							count--;
							ptr2++;
							continue;
						}
						result = (int)((long)(ptr2 - ptr));
					}
					return result;
				}
				uint num = (uint)((int)val + ((int)val << 8));
				num += num << 16;
				bool flag = false;
				while (count >= 32)
				{
					containsBinary = (containsBinary || flag);
					offset = 0;
					uint num2 = *(uint*)ptr2 ^ num;
					uint num3 = *(uint*)ptr2;
					flag = ((num3 & 2155905152U) != 0U);
					if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
					{
						containsBinary = (containsBinary || flag);
						offset += 4;
						num2 = (*(uint*)(ptr2 + 4) ^ num);
						num3 = *(uint*)(ptr2 + 4);
						flag = ((num3 & 2155905152U) != 0U);
						if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
						{
							containsBinary = (containsBinary || flag);
							offset += 4;
							num2 = (*(uint*)(ptr2 + 8) ^ num);
							num3 = *(uint*)(ptr2 + 8);
							flag = ((num3 & 2155905152U) != 0U);
							if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
							{
								containsBinary = (containsBinary || flag);
								offset += 4;
								num2 = (*(uint*)(ptr2 + 12) ^ num);
								num3 = *(uint*)(ptr2 + 12);
								flag = ((num3 & 2155905152U) != 0U);
								if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
								{
									containsBinary = (containsBinary || flag);
									offset += 4;
									num2 = (*(uint*)(ptr2 + 16) ^ num);
									num3 = *(uint*)(ptr2 + 16);
									flag = ((num3 & 2155905152U) != 0U);
									if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
									{
										containsBinary = (containsBinary || flag);
										offset += 4;
										num2 = (*(uint*)(ptr2 + 20) ^ num);
										num3 = *(uint*)(ptr2 + 20);
										flag = ((num3 & 2155905152U) != 0U);
										if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
										{
											containsBinary = (containsBinary || flag);
											offset += 4;
											num2 = (*(uint*)(ptr2 + 24) ^ num);
											num3 = *(uint*)(ptr2 + 24);
											flag = ((num3 & 2155905152U) != 0U);
											if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
											{
												containsBinary = (containsBinary || flag);
												offset += 4;
												num2 = (*(uint*)(ptr2 + 28) ^ num);
												num3 = *(uint*)(ptr2 + 28);
												flag = ((num3 & 2155905152U) != 0U);
												if (((num2 ^ 4294967295U ^ 2130640639U + num2) & 2164326656U) == 0U)
												{
													containsBinary = (containsBinary || flag);
													flag = ((num3 & 2155905152U) != 0U);
													ptr2 += 32;
													count -= 32;
													continue;
												}
											}
										}
									}
								}
							}
						}
					}
					ptr2 += offset;
					int num4 = (int)((long)(ptr2 - ptr));
					if (*ptr2 == val)
					{
						containsBinary = (containsBinary || (*ptr2 & 128) != 0);
						return num4;
					}
					if (ptr2[1] == val)
					{
						containsBinary = (containsBinary || (*ptr2 & 128) != 0 || (ptr2[1] & 128) != 0);
						return num4 + 1;
					}
					if (ptr2[2] == val)
					{
						containsBinary = (containsBinary || (*ptr2 & 128) != 0 || (ptr2[1] & 128) != 0 || (ptr2[2] & 128) != 0);
						return num4 + 2;
					}
					if (ptr2[3] == val)
					{
						containsBinary = (containsBinary || (*ptr2 & 128) != 0 || (ptr2[1] & 128) != 0 || (ptr2[2] & 128) != 0 || (ptr2[3] & 128) != 0);
						return num4 + 3;
					}
					ptr2 += 4;
					count -= offset + 4;
					containsBinary = (containsBinary || flag);
				}
				while (count != 0)
				{
					if ((*ptr2 & 128) != 0)
					{
						containsBinary = true;
					}
					if (*ptr2 == val)
					{
						return (int)((long)(ptr2 - ptr));
					}
					count--;
					ptr2++;
				}
				return -1;
			}
		}

		public static void ValidateStringArgument(string value, bool allowUTF8)
		{
			if (ByteString.IsStringArgumentValid(value, allowUTF8))
			{
				return;
			}
			if (allowUTF8)
			{
				throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8);
			}
			throw new ArgumentException(SharedStrings.StringArgumentMustBeAscii);
		}

		public static bool IsStringArgumentValid(string value, bool allowUTF8)
		{
			if (allowUTF8)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] >= '\u0080')
				{
					return false;
				}
			}
			return true;
		}

		public static int StringToBytesCount(string value, bool allowUTF8)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			if (allowUTF8)
			{
				try
				{
					return CTSGlobals.Utf8Encoding.GetByteCount(value);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8, innerException);
				}
			}
			return value.Length;
		}

		public static byte[] StringToBytesAndAppendCRLF(string value, bool allowUTF8)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ByteString.Empty;
			}
			return ByteString.StringToBytes(value + Environment.NewLine, allowUTF8);
		}

		public static byte[] StringToBytes(string value, bool allowUTF8 = true)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ByteString.Empty;
			}
			if (allowUTF8)
			{
				try
				{
					return CTSGlobals.Utf8Encoding.GetBytes(value);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8, innerException);
				}
			}
			byte[] array = new byte[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = ((value[i] < '\u0080') ? ((byte)value[i]) : 63);
			}
			return array;
		}

		public static int StringToBytes(string value, byte[] bytes, int bytesOffset, bool allowUTF8)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			return ByteString.StringToBytes(value, 0, value.Length, bytes, bytesOffset, allowUTF8);
		}

		public static int StringToBytes(string value, int valueOffset, int valueCount, byte[] bytes, int bytesOffset, bool allowUTF8)
		{
			if (allowUTF8)
			{
				try
				{
					return CTSGlobals.Utf8Encoding.GetBytes(value, valueOffset, valueCount, bytes, bytesOffset);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8, innerException);
				}
			}
			for (int i = 0; i < valueCount; i++)
			{
				bytes[bytesOffset + i] = ((value[valueOffset + i] < '\u0080') ? ((byte)value[valueOffset + i]) : 63);
			}
			return valueCount;
		}

		public static string BytesToString(byte[] bytes, bool allowUTF8)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return string.Empty;
			}
			return ByteString.BytesToString(bytes, 0, bytes.Length, allowUTF8);
		}

		public static string BytesToString(byte[] bytes, int offset, int count, bool allowUTF8)
		{
			if (allowUTF8)
			{
				try
				{
					return CTSGlobals.Utf8Encoding.GetString(bytes, offset, count);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8, innerException);
				}
			}
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = (char)((bytes[offset + i] < 128) ? bytes[offset + i] : 63);
			}
			return new string(array);
		}

		public static int BytesToCharCount(byte[] bytes, bool allowUTF8)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return 0;
			}
			if (allowUTF8)
			{
				try
				{
					return CTSGlobals.Utf8Encoding.GetCharCount(bytes);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(SharedStrings.StringArgumentMustBeUTF8, innerException);
				}
			}
			return bytes.Length;
		}

		public static char BytesToChar(byte[] bytes, int index, out int bytesUsed, out bool replacementChar, bool allowUTF8)
		{
			if (allowUTF8)
			{
				if (bytes[index] < 128)
				{
					bytesUsed = 1;
					replacementChar = false;
					return (char)bytes[index];
				}
				int num = 0;
				byte ch = bytes[index];
				byte ch2 = (index + 1 < bytes.Length) ? bytes[index + 1] : 0;
				byte ch3 = (index + 2 < bytes.Length) ? bytes[index + 2] : 0;
				byte ch4 = (index + 3 < bytes.Length) ? bytes[index + 3] : 0;
				if (ByteString.IsUTF8NonASCII(ch, ch2, ch3, ch4, out num))
				{
					try
					{
						char[] chars = CTSGlobals.Utf8Encoding.GetChars(bytes, index, num);
						bytesUsed = num;
						replacementChar = false;
						return chars[0];
					}
					catch (Exception)
					{
					}
				}
				bytesUsed = 1;
				replacementChar = true;
				return '�';
			}
			else
			{
				bytesUsed = 1;
				if (bytes[index] < 128)
				{
					replacementChar = false;
					return (char)bytes[index];
				}
				replacementChar = true;
				return '?';
			}
		}

		internal static int CompareI(byte[] str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
		{
			if (str1 == null || str2 == null)
			{
				if (str1 != null)
				{
					return 1;
				}
				if (str2 != null)
				{
					return -1;
				}
				return 0;
			}
			else if (allowUTF8)
			{
				int num = 0;
				int num2 = 0;
				while (num < str1Length && num2 < str2Length)
				{
					int num3 = str1Offset + num;
					int num4 = str2Offset + num2;
					if (str1[num3] < 128 && str2[num4] < 128)
					{
						byte b = ByteString.LowerC[(int)str1[num3]];
						byte b2 = ByteString.LowerC[(int)str2[num4]];
						if (b != b2)
						{
							if (b >= b2)
							{
								return 1;
							}
							return -1;
						}
						else
						{
							num++;
							num2++;
						}
					}
					else
					{
						int num5 = 0;
						bool flag = false;
						byte b3 = str1[num3];
						char c = ByteString.BytesToChar(str1, num3, out num5, out flag, allowUTF8);
						int num6 = 0;
						bool flag2 = false;
						byte b4 = str2[num4];
						char c2 = ByteString.BytesToChar(str2, num4, out num6, out flag2, allowUTF8);
						if (num + num5 > str1Length || num2 + num6 > str2Length)
						{
							if (b3 != b4)
							{
								if (b3 >= b4)
								{
									return 1;
								}
								return -1;
							}
							else
							{
								num++;
								num2++;
							}
						}
						else
						{
							if (flag || flag2)
							{
								if (flag != flag2)
								{
									if (c >= c2)
									{
										return 1;
									}
									return -1;
								}
								else if (b3 != b4)
								{
									if (b3 >= b4)
									{
										return 1;
									}
									return -1;
								}
							}
							else
							{
								c = char.ToLower(c);
								c2 = char.ToLower(c2);
								if (c != c2)
								{
									if (c >= c2)
									{
										return 1;
									}
									return -1;
								}
							}
							num += num5;
							num2 += num6;
						}
					}
				}
				if (num != str1Length)
				{
					return 1;
				}
				if (num2 >= str2Length)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				int num7 = 0;
				while (num7 < str1Length && num7 < str2Length)
				{
					int num8 = str1Offset + num7;
					int num9 = str2Offset + num7;
					byte b5 = (str1[num8] < 128) ? ByteString.LowerC[(int)str1[num8]] : str1[num8];
					byte b6 = (str2[num9] < 128) ? ByteString.LowerC[(int)str2[num9]] : str2[num9];
					if (b5 != b6)
					{
						if (b5 >= b6)
						{
							return 1;
						}
						return -1;
					}
					else
					{
						num7++;
					}
				}
				if (num7 != str1Length)
				{
					return 1;
				}
				if (num7 >= str2Length)
				{
					return 0;
				}
				return -1;
			}
		}

		internal static int CompareI(string str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
		{
			if (str1 == null || str2 == null)
			{
				if (str1 != null)
				{
					return 1;
				}
				if (str2 != null)
				{
					return -1;
				}
				return 0;
			}
			else if (allowUTF8)
			{
				int num = 0;
				int num2 = 0;
				while (num < str1Length && num2 < str2Length)
				{
					int index = str1Offset + num;
					int num3 = str2Offset + num2;
					if (str1[index] < '\u0080' && str2[num3] < 128)
					{
						byte b = ByteString.LowerC[(int)str1[index]];
						byte b2 = ByteString.LowerC[(int)str2[num3]];
						if (b != b2)
						{
							if (b >= b2)
							{
								return 1;
							}
							return -1;
						}
						else
						{
							num++;
							num2++;
						}
					}
					else
					{
						byte b3 = (byte)str1[index];
						char c = str1[index];
						int num4 = 0;
						bool flag = false;
						byte b4 = str2[num3];
						char c2 = ByteString.BytesToChar(str2, num3, out num4, out flag, allowUTF8);
						if (num2 + num4 > str2Length)
						{
							if (b3 != b4)
							{
								if (b3 >= b4)
								{
									return 1;
								}
								return -1;
							}
							else
							{
								num++;
								num2++;
							}
						}
						else
						{
							if (flag)
							{
								if (b3 != b4)
								{
									if (b3 >= b4)
									{
										return 1;
									}
									return -1;
								}
							}
							else
							{
								c = char.ToLower(c);
								c2 = char.ToLower(c2);
								if (c != c2)
								{
									if (c >= c2)
									{
										return 1;
									}
									return -1;
								}
							}
							num++;
							num2 += num4;
						}
					}
				}
				if (num != str1Length)
				{
					return 1;
				}
				if (num2 >= str2Length)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				int num5 = 0;
				while (num5 < str1Length && num5 < str2Length)
				{
					int index2 = str1Offset + num5;
					int num6 = str2Offset + num5;
					byte b5 = (str1[index2] < '\u0080') ? ByteString.LowerC[(int)((byte)str1[index2])] : ((byte)str1[index2]);
					byte b6 = (str2[num6] < 128) ? ByteString.LowerC[(int)str2[num6]] : str2[num6];
					if (b5 != b6)
					{
						if (b5 >= b6)
						{
							return 1;
						}
						return -1;
					}
					else
					{
						num5++;
					}
				}
				if (num5 != str1Length)
				{
					return 1;
				}
				if (num5 >= str2Length)
				{
					return 0;
				}
				return -1;
			}
		}

		internal static int CompareI(byte[] str1, byte[] str2, bool allowUTF8)
		{
			if (str1 != null && str2 != null)
			{
				return ByteString.CompareI(str1, 0, str1.Length, str2, 0, str2.Length, allowUTF8);
			}
			if (str1 != null)
			{
				return 1;
			}
			if (str2 != null)
			{
				return -1;
			}
			return 0;
		}

		internal static bool EqualsI(byte[] str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
		{
			if (str1 == null || str2 == null)
			{
				return str1 == null && str2 == null;
			}
			return (allowUTF8 || str1Length == str2Length) && ByteString.CompareI(str1, str1Offset, str1Length, str2, str2Offset, str2Length, allowUTF8) == 0;
		}

		internal static bool EqualsI(string str1, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
		{
			if (str1 == null || str2 == null)
			{
				return str1 == null && str2 == null;
			}
			return (allowUTF8 || str1.Length == str2Length) && ByteString.CompareI(str1, 0, str1.Length, str2, str2Offset, str2Length, allowUTF8) == 0;
		}

		internal static uint ComputeCrcI(byte[] bytes, int offset, int length)
		{
			uint num = 0U;
			for (int i = 0; i < length; i++)
			{
				int num2 = offset + i;
				byte ch = (bytes[num2] < 128) ? ByteString.LowerC[(int)bytes[num2]] : bytes[num2];
				num = ByteString.ComputeCrc(num, ch);
			}
			return num;
		}

		internal static uint ComputeCrc(byte[] bytes, int offset, int length)
		{
			uint num = 0U;
			for (int i = 0; i < length; i++)
			{
				num = ByteString.ComputeCrc(num, bytes[offset + i]);
			}
			return num;
		}

		private static uint ComputeCrc(uint seed, byte ch)
		{
			return ByteString.CrcTable[(int)((UIntPtr)((seed ^ (uint)ch) & 255U))] ^ seed >> 8;
		}

		internal static bool IsUTF8_2(byte ch1, byte ch2)
		{
			return ch1 >= 194 && ch1 <= 223 && ch2 >= 128 && ch2 <= 191;
		}

		internal static bool IsUTF8_3(byte ch1, byte ch2, byte ch3)
		{
			return (ch1 == 224 && ch2 >= 160 && ch2 <= 191 && ch3 >= 128 && ch3 <= 191) || (ch1 >= 225 && ch1 <= 236 && ch2 >= 128 && ch2 <= 191 && ch3 >= 128 && ch3 <= 191) || (ch1 == 237 && ch2 >= 128 && ch2 <= 159 && ch3 >= 128 && ch3 <= 191) || (ch1 >= 238 && ch1 <= 239 && ch2 >= 128 && ch2 <= 191 && ch3 >= 128 && ch3 <= 191);
		}

		internal static bool IsUTF8_4(byte ch1, byte ch2, byte ch3, byte ch4)
		{
			return (ch1 == 240 && ch2 >= 144 && ch2 <= 191 && ch3 >= 128 && ch3 <= 191 && ch4 >= 128 && ch4 <= 191) || (ch1 >= 241 && ch1 <= 243 && ch2 >= 128 && ch2 <= 191 && ch3 >= 128 && ch3 <= 191 && ch4 >= 128 && ch4 <= 191) || (ch1 == 244 && ch2 >= 128 && ch2 <= 143 && ch3 >= 128 && ch3 <= 191 && ch4 >= 128 && ch4 <= 191);
		}

		internal static bool IsUTF8NonASCII(byte ch1, byte ch2, byte ch3, byte ch4, out int bytesUsed)
		{
			if (ByteString.IsUTF8_2(ch1, ch2))
			{
				bytesUsed = 2;
				return true;
			}
			if (ByteString.IsUTF8_3(ch1, ch2, ch3))
			{
				bytesUsed = 3;
				return true;
			}
			if (ByteString.IsUTF8_4(ch1, ch2, ch3, ch4))
			{
				bytesUsed = 4;
				return true;
			}
			bytesUsed = 0;
			return false;
		}

		internal static readonly byte[] Empty = new byte[0];

		internal static readonly byte[] LowerC = new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			123,
			124,
			125,
			126,
			127,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		private static readonly uint[] CrcTable = new uint[]
		{
			0U,
			1996959894U,
			3993919788U,
			2567524794U,
			124634137U,
			1886057615U,
			3915621685U,
			2657392035U,
			249268274U,
			2044508324U,
			3772115230U,
			2547177864U,
			162941995U,
			2125561021U,
			3887607047U,
			2428444049U,
			498536548U,
			1789927666U,
			4089016648U,
			2227061214U,
			450548861U,
			1843258603U,
			4107580753U,
			2211677639U,
			325883990U,
			1684777152U,
			4251122042U,
			2321926636U,
			335633487U,
			1661365465U,
			4195302755U,
			2366115317U,
			997073096U,
			1281953886U,
			3579855332U,
			2724688242U,
			1006888145U,
			1258607687U,
			3524101629U,
			2768942443U,
			901097722U,
			1119000684U,
			3686517206U,
			2898065728U,
			853044451U,
			1172266101U,
			3705015759U,
			2882616665U,
			651767980U,
			1373503546U,
			3369554304U,
			3218104598U,
			565507253U,
			1454621731U,
			3485111705U,
			3099436303U,
			671266974U,
			1594198024U,
			3322730930U,
			2970347812U,
			795835527U,
			1483230225U,
			3244367275U,
			3060149565U,
			1994146192U,
			31158534U,
			2563907772U,
			4023717930U,
			1907459465U,
			112637215U,
			2680153253U,
			3904427059U,
			2013776290U,
			251722036U,
			2517215374U,
			3775830040U,
			2137656763U,
			141376813U,
			2439277719U,
			3865271297U,
			1802195444U,
			476864866U,
			2238001368U,
			4066508878U,
			1812370925U,
			453092731U,
			2181625025U,
			4111451223U,
			1706088902U,
			314042704U,
			2344532202U,
			4240017532U,
			1658658271U,
			366619977U,
			2362670323U,
			4224994405U,
			1303535960U,
			984961486U,
			2747007092U,
			3569037538U,
			1256170817U,
			1037604311U,
			2765210733U,
			3554079995U,
			1131014506U,
			879679996U,
			2909243462U,
			3663771856U,
			1141124467U,
			855842277U,
			2852801631U,
			3708648649U,
			1342533948U,
			654459306U,
			3188396048U,
			3373015174U,
			1466479909U,
			544179635U,
			3110523913U,
			3462522015U,
			1591671054U,
			702138776U,
			2966460450U,
			3352799412U,
			1504918807U,
			783551873U,
			3082640443U,
			3233442989U,
			3988292384U,
			2596254646U,
			62317068U,
			1957810842U,
			3939845945U,
			2647816111U,
			81470997U,
			1943803523U,
			3814918930U,
			2489596804U,
			225274430U,
			2053790376U,
			3826175755U,
			2466906013U,
			167816743U,
			2097651377U,
			4027552580U,
			2265490386U,
			503444072U,
			1762050814U,
			4150417245U,
			2154129355U,
			426522225U,
			1852507879U,
			4275313526U,
			2312317920U,
			282753626U,
			1742555852U,
			4189708143U,
			2394877945U,
			397917763U,
			1622183637U,
			3604390888U,
			2714866558U,
			953729732U,
			1340076626U,
			3518719985U,
			2797360999U,
			1068828381U,
			1219638859U,
			3624741850U,
			2936675148U,
			906185462U,
			1090812512U,
			3747672003U,
			2825379669U,
			829329135U,
			1181335161U,
			3412177804U,
			3160834842U,
			628085408U,
			1382605366U,
			3423369109U,
			3138078467U,
			570562233U,
			1426400815U,
			3317316542U,
			2998733608U,
			733239954U,
			1555261956U,
			3268935591U,
			3050360625U,
			752459403U,
			1541320221U,
			2607071920U,
			3965973030U,
			1969922972U,
			40735498U,
			2617837225U,
			3943577151U,
			1913087877U,
			83908371U,
			2512341634U,
			3803740692U,
			2075208622U,
			213261112U,
			2463272603U,
			3855990285U,
			2094854071U,
			198958881U,
			2262029012U,
			4057260610U,
			1759359992U,
			534414190U,
			2176718541U,
			4139329115U,
			1873836001U,
			414664567U,
			2282248934U,
			4279200368U,
			1711684554U,
			285281116U,
			2405801727U,
			4167216745U,
			1634467795U,
			376229701U,
			2685067896U,
			3608007406U,
			1308918612U,
			956543938U,
			2808555105U,
			3495958263U,
			1231636301U,
			1047427035U,
			2932959818U,
			3654703836U,
			1088359270U,
			936918000U,
			2847714899U,
			3736837829U,
			1202900863U,
			817233897U,
			3183342108U,
			3401237130U,
			1404277552U,
			615818150U,
			3134207493U,
			3453421203U,
			1423857449U,
			601450431U,
			3009837614U,
			3294710456U,
			1567103746U,
			711928724U,
			3020668471U,
			3272380065U,
			1510334235U,
			755167117U
		};
	}
}
