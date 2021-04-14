using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace System
{
	[FriendAccessAllowed]
	internal class Number
	{
		private Number()
		{
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatDecimal(decimal value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatDouble(double value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatInt32(int value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatUInt32(uint value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatInt64(long value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatUInt64(ulong value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatSingle(float value, string format, NumberFormatInfo info);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern bool NumberBufferToDecimal(byte* number, ref decimal value);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool NumberBufferToDouble(byte* number, ref double value);

		[FriendAccessAllowed]
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern string FormatNumberBuffer(byte* number, string format, NumberFormatInfo info, char* allDigits);

		private static bool HexNumberToInt32(ref Number.NumberBuffer number, ref int value)
		{
			uint num = 0U;
			bool result = Number.HexNumberToUInt32(ref number, ref num);
			value = (int)num;
			return result;
		}

		private static bool HexNumberToInt64(ref Number.NumberBuffer number, ref long value)
		{
			ulong num = 0UL;
			bool result = Number.HexNumberToUInt64(ref number, ref num);
			value = (long)num;
			return result;
		}

		[SecuritySafeCritical]
		private unsafe static bool HexNumberToUInt32(ref Number.NumberBuffer number, ref uint value)
		{
			int num = number.scale;
			if (num > 10 || num < number.precision)
			{
				return false;
			}
			char* ptr = number.digits;
			uint num2 = 0U;
			while (--num >= 0)
			{
				if (num2 > 268435455U)
				{
					return false;
				}
				num2 *= 16U;
				if (*ptr != '\0')
				{
					uint num3 = num2;
					if (*ptr != '\0')
					{
						if (*ptr >= '0' && *ptr <= '9')
						{
							num3 += (uint)(*ptr - '0');
						}
						else if (*ptr >= 'A' && *ptr <= 'F')
						{
							num3 += (uint)(*ptr - 'A' + '\n');
						}
						else
						{
							num3 += (uint)(*ptr - 'a' + '\n');
						}
						ptr++;
					}
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		[SecuritySafeCritical]
		private unsafe static bool HexNumberToUInt64(ref Number.NumberBuffer number, ref ulong value)
		{
			int num = number.scale;
			if (num > 20 || num < number.precision)
			{
				return false;
			}
			char* ptr = number.digits;
			ulong num2 = 0UL;
			while (--num >= 0)
			{
				if (num2 > 1152921504606846975UL)
				{
					return false;
				}
				num2 *= 16UL;
				if (*ptr != '\0')
				{
					ulong num3 = num2;
					if (*ptr != '\0')
					{
						if (*ptr >= '0' && *ptr <= '9')
						{
							num3 += (ulong)((long)(*ptr - '0'));
						}
						else if (*ptr >= 'A' && *ptr <= 'F')
						{
							num3 += (ulong)((long)(*ptr - 'A' + '\n'));
						}
						else
						{
							num3 += (ulong)((long)(*ptr - 'a' + '\n'));
						}
						ptr++;
					}
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		private static bool IsWhite(char ch)
		{
			return ch == ' ' || (ch >= '\t' && ch <= '\r');
		}

		[SecuritySafeCritical]
		private unsafe static bool NumberToInt32(ref Number.NumberBuffer number, ref int value)
		{
			int num = number.scale;
			if (num > 10 || num < number.precision)
			{
				return false;
			}
			char* digits = number.digits;
			int num2 = 0;
			while (--num >= 0)
			{
				if (num2 > 214748364)
				{
					return false;
				}
				num2 *= 10;
				if (*digits != '\0')
				{
					num2 += (int)(*(digits++) - '0');
				}
			}
			if (number.sign)
			{
				num2 = -num2;
				if (num2 > 0)
				{
					return false;
				}
			}
			else if (num2 < 0)
			{
				return false;
			}
			value = num2;
			return true;
		}

		[SecuritySafeCritical]
		private unsafe static bool NumberToInt64(ref Number.NumberBuffer number, ref long value)
		{
			int num = number.scale;
			if (num > 19 || num < number.precision)
			{
				return false;
			}
			char* digits = number.digits;
			long num2 = 0L;
			while (--num >= 0)
			{
				if (num2 > 922337203685477580L)
				{
					return false;
				}
				num2 *= 10L;
				if (*digits != '\0')
				{
					num2 += (long)(*(digits++) - '0');
				}
			}
			if (number.sign)
			{
				num2 = -num2;
				if (num2 > 0L)
				{
					return false;
				}
			}
			else if (num2 < 0L)
			{
				return false;
			}
			value = num2;
			return true;
		}

		[SecuritySafeCritical]
		private unsafe static bool NumberToUInt32(ref Number.NumberBuffer number, ref uint value)
		{
			int num = number.scale;
			if (num > 10 || num < number.precision || number.sign)
			{
				return false;
			}
			char* digits = number.digits;
			uint num2 = 0U;
			while (--num >= 0)
			{
				if (num2 > 429496729U)
				{
					return false;
				}
				num2 *= 10U;
				if (*digits != '\0')
				{
					uint num3 = num2 + (uint)(*(digits++) - '0');
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		[SecuritySafeCritical]
		private unsafe static bool NumberToUInt64(ref Number.NumberBuffer number, ref ulong value)
		{
			int num = number.scale;
			if (num > 20 || num < number.precision || number.sign)
			{
				return false;
			}
			char* digits = number.digits;
			ulong num2 = 0UL;
			while (--num >= 0)
			{
				if (num2 > 1844674407370955161UL)
				{
					return false;
				}
				num2 *= 10UL;
				if (*digits != '\0')
				{
					ulong num3 = num2 + (ulong)((long)(*(digits++) - '0'));
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		[SecurityCritical]
		private unsafe static char* MatchChars(char* p, string str)
		{
			char* ptr = str;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Number.MatchChars(p, ptr);
		}

		[SecurityCritical]
		private unsafe static char* MatchChars(char* p, char* str)
		{
			if (*str == '\0')
			{
				return null;
			}
			while (*str != '\0')
			{
				if (*p != *str && (*str != '\u00a0' || *p != ' '))
				{
					return null;
				}
				p++;
				str++;
			}
			return p;
		}

		[SecuritySafeCritical]
		internal unsafe static decimal ParseDecimal(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			decimal result = 0m;
			Number.StringToNumber(value, options, ref numberBuffer, numfmt, true);
			if (!Number.NumberBufferToDecimal(numberBuffer.PackForNative(), ref result))
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Decimal"));
			}
			return result;
		}

		[SecuritySafeCritical]
		internal unsafe static double ParseDouble(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			double result = 0.0;
			if (!Number.TryStringToNumber(value, options, ref numberBuffer, numfmt, false))
			{
				string text = value.Trim();
				if (text.Equals(numfmt.PositiveInfinitySymbol))
				{
					return double.PositiveInfinity;
				}
				if (text.Equals(numfmt.NegativeInfinitySymbol))
				{
					return double.NegativeInfinity;
				}
				if (text.Equals(numfmt.NaNSymbol))
				{
					return double.NaN;
				}
				throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
			}
			else
			{
				if (!Number.NumberBufferToDouble(numberBuffer.PackForNative(), ref result))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_Double"));
				}
				return result;
			}
		}

		[SecuritySafeCritical]
		internal unsafe static int ParseInt32(string s, NumberStyles style, NumberFormatInfo info)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			int result = 0;
			Number.StringToNumber(s, style, ref numberBuffer, info, false);
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToInt32(ref numberBuffer, ref result))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_Int32"));
				}
			}
			else if (!Number.NumberToInt32(ref numberBuffer, ref result))
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Int32"));
			}
			return result;
		}

		[SecuritySafeCritical]
		internal unsafe static long ParseInt64(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			long result = 0L;
			Number.StringToNumber(value, options, ref numberBuffer, numfmt, false);
			if ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToInt64(ref numberBuffer, ref result))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_Int64"));
				}
			}
			else if (!Number.NumberToInt64(ref numberBuffer, ref result))
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Int64"));
			}
			return result;
		}

		[SecurityCritical]
		private unsafe static bool ParseNumber(ref char* str, NumberStyles options, ref Number.NumberBuffer number, StringBuilder sb, NumberFormatInfo numfmt, bool parseDecimal)
		{
			number.scale = 0;
			number.sign = false;
			string text = null;
			string text2 = null;
			string str2 = null;
			string str3 = null;
			bool flag = false;
			string str4;
			string str5;
			if ((options & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				text = numfmt.CurrencySymbol;
				if (numfmt.ansiCurrencySymbol != null)
				{
					text2 = numfmt.ansiCurrencySymbol;
				}
				str2 = numfmt.NumberDecimalSeparator;
				str3 = numfmt.NumberGroupSeparator;
				str4 = numfmt.CurrencyDecimalSeparator;
				str5 = numfmt.CurrencyGroupSeparator;
				flag = true;
			}
			else
			{
				str4 = numfmt.NumberDecimalSeparator;
				str5 = numfmt.NumberGroupSeparator;
			}
			int num = 0;
			bool flag2 = sb != null;
			bool flag3 = flag2 && (options & NumberStyles.AllowHexSpecifier) > NumberStyles.None;
			int num2 = flag2 ? int.MaxValue : 50;
			char* ptr = str;
			char c = *ptr;
			for (;;)
			{
				if (!Number.IsWhite(c) || (options & NumberStyles.AllowLeadingWhite) == NumberStyles.None || ((num & 1) != 0 && ((num & 1) == 0 || ((num & 32) == 0 && numfmt.numberNegativePattern != 2))))
				{
					bool flag4;
					char* ptr2;
					if ((flag4 = ((options & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num & 1) == 0)) && (ptr2 = Number.MatchChars(ptr, numfmt.positiveSign)) != null)
					{
						num |= 1;
						ptr = ptr2 - 1;
					}
					else if (flag4 && (ptr2 = Number.MatchChars(ptr, numfmt.negativeSign)) != null)
					{
						num |= 1;
						number.sign = true;
						ptr = ptr2 - 1;
					}
					else if (c == '(' && (options & NumberStyles.AllowParentheses) != NumberStyles.None && (num & 1) == 0)
					{
						num |= 3;
						number.sign = true;
					}
					else
					{
						if ((text == null || (ptr2 = Number.MatchChars(ptr, text)) == null) && (text2 == null || (ptr2 = Number.MatchChars(ptr, text2)) == null))
						{
							break;
						}
						num |= 32;
						text = null;
						text2 = null;
						ptr = ptr2 - 1;
					}
				}
				c = *(++ptr);
			}
			int num3 = 0;
			int num4 = 0;
			for (;;)
			{
				char* ptr2;
				if ((c >= '0' && c <= '9') || ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None && ((c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))))
				{
					num |= 4;
					if (c != '0' || (num & 8) != 0 || flag3)
					{
						if (num3 < num2)
						{
							if (flag2)
							{
								sb.Append(c);
							}
							else
							{
								number.digits[(IntPtr)(num3++) * 2] = c;
							}
							if (c != '0' || parseDecimal)
							{
								num4 = num3;
							}
						}
						if ((num & 16) == 0)
						{
							number.scale++;
						}
						num |= 8;
					}
					else if ((num & 16) != 0)
					{
						number.scale--;
					}
				}
				else if ((options & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num & 16) == 0 && ((ptr2 = Number.MatchChars(ptr, str4)) != null || (flag && (num & 32) == 0 && (ptr2 = Number.MatchChars(ptr, str2)) != null)))
				{
					num |= 16;
					ptr = ptr2 - 1;
				}
				else
				{
					if ((options & NumberStyles.AllowThousands) == NumberStyles.None || (num & 4) == 0 || (num & 16) != 0 || ((ptr2 = Number.MatchChars(ptr, str5)) == null && (!flag || (num & 32) != 0 || (ptr2 = Number.MatchChars(ptr, str3)) == null)))
					{
						break;
					}
					ptr = ptr2 - 1;
				}
				c = *(++ptr);
			}
			bool flag5 = false;
			number.precision = num4;
			if (flag2)
			{
				sb.Append('\0');
			}
			else
			{
				number.digits[num4] = '\0';
			}
			if ((num & 4) != 0)
			{
				if ((c == 'E' || c == 'e') && (options & NumberStyles.AllowExponent) != NumberStyles.None)
				{
					char* ptr3 = ptr;
					c = *(++ptr);
					char* ptr2;
					if ((ptr2 = Number.MatchChars(ptr, numfmt.positiveSign)) != null)
					{
						c = *(ptr = ptr2);
					}
					else if ((ptr2 = Number.MatchChars(ptr, numfmt.negativeSign)) != null)
					{
						c = *(ptr = ptr2);
						flag5 = true;
					}
					if (c >= '0' && c <= '9')
					{
						int num5 = 0;
						do
						{
							num5 = num5 * 10 + (int)(c - '0');
							c = *(++ptr);
							if (num5 > 1000)
							{
								num5 = 9999;
								while (c >= '0' && c <= '9')
								{
									c = *(++ptr);
								}
							}
						}
						while (c >= '0' && c <= '9');
						if (flag5)
						{
							num5 = -num5;
						}
						number.scale += num5;
					}
					else
					{
						ptr = ptr3;
						c = *ptr;
					}
				}
				for (;;)
				{
					if (!Number.IsWhite(c) || (options & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
					{
						bool flag4;
						char* ptr2;
						if ((flag4 = ((options & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num & 1) == 0)) && (ptr2 = Number.MatchChars(ptr, numfmt.positiveSign)) != null)
						{
							num |= 1;
							ptr = ptr2 - 1;
						}
						else if (flag4 && (ptr2 = Number.MatchChars(ptr, numfmt.negativeSign)) != null)
						{
							num |= 1;
							number.sign = true;
							ptr = ptr2 - 1;
						}
						else if (c == ')' && (num & 2) != 0)
						{
							num &= -3;
						}
						else
						{
							if ((text == null || (ptr2 = Number.MatchChars(ptr, text)) == null) && (text2 == null || (ptr2 = Number.MatchChars(ptr, text2)) == null))
							{
								break;
							}
							text = null;
							text2 = null;
							ptr = ptr2 - 1;
						}
					}
					c = *(++ptr);
				}
				if ((num & 2) == 0)
				{
					if ((num & 8) == 0)
					{
						if (!parseDecimal)
						{
							number.scale = 0;
						}
						if ((num & 16) == 0)
						{
							number.sign = false;
						}
					}
					str = ptr;
					return true;
				}
			}
			str = ptr;
			return false;
		}

		[SecuritySafeCritical]
		internal unsafe static float ParseSingle(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			double num = 0.0;
			if (!Number.TryStringToNumber(value, options, ref numberBuffer, numfmt, false))
			{
				string text = value.Trim();
				if (text.Equals(numfmt.PositiveInfinitySymbol))
				{
					return float.PositiveInfinity;
				}
				if (text.Equals(numfmt.NegativeInfinitySymbol))
				{
					return float.NegativeInfinity;
				}
				if (text.Equals(numfmt.NaNSymbol))
				{
					return float.NaN;
				}
				throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
			}
			else
			{
				if (!Number.NumberBufferToDouble(numberBuffer.PackForNative(), ref num))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_Single"));
				}
				float num2 = (float)num;
				if (float.IsInfinity(num2))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_Single"));
				}
				return num2;
			}
		}

		[SecuritySafeCritical]
		internal unsafe static uint ParseUInt32(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			uint result = 0U;
			Number.StringToNumber(value, options, ref numberBuffer, numfmt, false);
			if ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToUInt32(ref numberBuffer, ref result))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_UInt32"));
				}
			}
			else if (!Number.NumberToUInt32(ref numberBuffer, ref result))
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_UInt32"));
			}
			return result;
		}

		[SecuritySafeCritical]
		internal unsafe static ulong ParseUInt64(string value, NumberStyles options, NumberFormatInfo numfmt)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			ulong result = 0UL;
			Number.StringToNumber(value, options, ref numberBuffer, numfmt, false);
			if ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToUInt64(ref numberBuffer, ref result))
				{
					throw new OverflowException(Environment.GetResourceString("Overflow_UInt64"));
				}
			}
			else if (!Number.NumberToUInt64(ref numberBuffer, ref result))
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_UInt64"));
			}
			return result;
		}

		[SecuritySafeCritical]
		private unsafe static void StringToNumber(string str, NumberStyles options, ref Number.NumberBuffer number, NumberFormatInfo info, bool parseDecimal)
		{
			if (str == null)
			{
				throw new ArgumentNullException("String");
			}
			fixed (string text = str)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				if (!Number.ParseNumber(ref ptr2, options, ref number, null, info, parseDecimal) || ((long)(ptr2 - ptr) < (long)str.Length && !Number.TrailingZeros(str, (int)((long)(ptr2 - ptr)))))
				{
					throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
				}
			}
		}

		private static bool TrailingZeros(string s, int index)
		{
			for (int i = index; i < s.Length; i++)
			{
				if (s[i] != '\0')
				{
					return false;
				}
			}
			return true;
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseDecimal(string value, NumberStyles options, NumberFormatInfo numfmt, out decimal result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0m;
			return Number.TryStringToNumber(value, options, ref numberBuffer, numfmt, true) && Number.NumberBufferToDecimal(numberBuffer.PackForNative(), ref result);
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseDouble(string value, NumberStyles options, NumberFormatInfo numfmt, out double result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0.0;
			return Number.TryStringToNumber(value, options, ref numberBuffer, numfmt, false) && Number.NumberBufferToDouble(numberBuffer.PackForNative(), ref result);
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseInt32(string s, NumberStyles style, NumberFormatInfo info, out int result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0;
			if (!Number.TryStringToNumber(s, style, ref numberBuffer, info, false))
			{
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToInt32(ref numberBuffer, ref result))
				{
					return false;
				}
			}
			else if (!Number.NumberToInt32(ref numberBuffer, ref result))
			{
				return false;
			}
			return true;
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseInt64(string s, NumberStyles style, NumberFormatInfo info, out long result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0L;
			if (!Number.TryStringToNumber(s, style, ref numberBuffer, info, false))
			{
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToInt64(ref numberBuffer, ref result))
				{
					return false;
				}
			}
			else if (!Number.NumberToInt64(ref numberBuffer, ref result))
			{
				return false;
			}
			return true;
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseSingle(string value, NumberStyles options, NumberFormatInfo numfmt, out float result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0f;
			double num = 0.0;
			if (!Number.TryStringToNumber(value, options, ref numberBuffer, numfmt, false))
			{
				return false;
			}
			if (!Number.NumberBufferToDouble(numberBuffer.PackForNative(), ref num))
			{
				return false;
			}
			float num2 = (float)num;
			if (float.IsInfinity(num2))
			{
				return false;
			}
			result = num2;
			return true;
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseUInt32(string s, NumberStyles style, NumberFormatInfo info, out uint result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0U;
			if (!Number.TryStringToNumber(s, style, ref numberBuffer, info, false))
			{
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToUInt32(ref numberBuffer, ref result))
				{
					return false;
				}
			}
			else if (!Number.NumberToUInt32(ref numberBuffer, ref result))
			{
				return false;
			}
			return true;
		}

		[SecuritySafeCritical]
		internal unsafe static bool TryParseUInt64(string s, NumberStyles style, NumberFormatInfo info, out ulong result)
		{
			byte* stackBuffer = stackalloc byte[checked(unchecked((UIntPtr)Number.NumberBuffer.NumberBufferBytes) * 1)];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(stackBuffer);
			result = 0UL;
			if (!Number.TryStringToNumber(s, style, ref numberBuffer, info, false))
			{
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!Number.HexNumberToUInt64(ref numberBuffer, ref result))
				{
					return false;
				}
			}
			else if (!Number.NumberToUInt64(ref numberBuffer, ref result))
			{
				return false;
			}
			return true;
		}

		internal static bool TryStringToNumber(string str, NumberStyles options, ref Number.NumberBuffer number, NumberFormatInfo numfmt, bool parseDecimal)
		{
			return Number.TryStringToNumber(str, options, ref number, null, numfmt, parseDecimal);
		}

		[SecuritySafeCritical]
		[FriendAccessAllowed]
		internal unsafe static bool TryStringToNumber(string str, NumberStyles options, ref Number.NumberBuffer number, StringBuilder sb, NumberFormatInfo numfmt, bool parseDecimal)
		{
			if (str == null)
			{
				return false;
			}
			fixed (string text = str)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				if (!Number.ParseNumber(ref ptr2, options, ref number, sb, numfmt, parseDecimal) || ((long)(ptr2 - ptr) < (long)str.Length && !Number.TrailingZeros(str, (int)((long)(ptr2 - ptr)))))
				{
					return false;
				}
			}
			return true;
		}

		private const int NumberMaxDigits = 50;

		private const int Int32Precision = 10;

		private const int UInt32Precision = 10;

		private const int Int64Precision = 19;

		private const int UInt64Precision = 20;

		[FriendAccessAllowed]
		internal struct NumberBuffer
		{
			[SecurityCritical]
			public unsafe NumberBuffer(byte* stackBuffer)
			{
				this.baseAddress = stackBuffer;
				this.digits = (char*)(stackBuffer + (IntPtr)6 * 2);
				this.precision = 0;
				this.scale = 0;
				this.sign = false;
			}

			[SecurityCritical]
			public unsafe byte* PackForNative()
			{
				int* ptr = (int*)this.baseAddress;
				*ptr = this.precision;
				ptr[1] = this.scale;
				ptr[2] = (this.sign ? 1 : 0);
				return this.baseAddress;
			}

			public static readonly int NumberBufferBytes = 114 + IntPtr.Size;

			[SecurityCritical]
			private unsafe byte* baseAddress;

			[SecurityCritical]
			public unsafe char* digits;

			public int precision;

			public int scale;

			public bool sign;
		}
	}
}
