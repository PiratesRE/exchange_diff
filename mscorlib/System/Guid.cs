using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Microsoft.Win32;

namespace System
{
	[ComVisible(true)]
	[NonVersionable]
	[__DynamicallyInvokable]
	[Serializable]
	public struct Guid : IFormattable, IComparable, IComparable<Guid>, IEquatable<Guid>
	{
		[__DynamicallyInvokable]
		public Guid(byte[] b)
		{
			if (b == null)
			{
				throw new ArgumentNullException("b");
			}
			if (b.Length != 16)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_GuidArrayCtor", new object[]
				{
					"16"
				}));
			}
			this._a = ((int)b[3] << 24 | (int)b[2] << 16 | (int)b[1] << 8 | (int)b[0]);
			this._b = (short)((int)b[5] << 8 | (int)b[4]);
			this._c = (short)((int)b[7] << 8 | (int)b[6]);
			this._d = b[8];
			this._e = b[9];
			this._f = b[10];
			this._g = b[11];
			this._h = b[12];
			this._i = b[13];
			this._j = b[14];
			this._k = b[15];
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this._a = (int)a;
			this._b = (short)b;
			this._c = (short)c;
			this._d = d;
			this._e = e;
			this._f = f;
			this._g = g;
			this._h = h;
			this._i = i;
			this._j = j;
			this._k = k;
		}

		[__DynamicallyInvokable]
		public Guid(int a, short b, short c, byte[] d)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			if (d.Length != 8)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_GuidArrayCtor", new object[]
				{
					"8"
				}));
			}
			this._a = a;
			this._b = b;
			this._c = c;
			this._d = d[0];
			this._e = d[1];
			this._f = d[2];
			this._g = d[3];
			this._h = d[4];
			this._i = d[5];
			this._j = d[6];
			this._k = d[7];
		}

		[__DynamicallyInvokable]
		public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this._a = a;
			this._b = b;
			this._c = c;
			this._d = d;
			this._e = e;
			this._f = f;
			this._g = g;
			this._h = h;
			this._i = i;
			this._j = j;
			this._k = k;
		}

		[__DynamicallyInvokable]
		public Guid(string g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			this = Guid.Empty;
			Guid.GuidResult guidResult = default(Guid.GuidResult);
			guidResult.Init(Guid.GuidParseThrowStyle.All);
			if (Guid.TryParseGuid(g, Guid.GuidStyles.Any, ref guidResult))
			{
				this = guidResult.parsedGuid;
				return;
			}
			throw guidResult.GetGuidParseException();
		}

		[__DynamicallyInvokable]
		public static Guid Parse(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			Guid.GuidResult guidResult = default(Guid.GuidResult);
			guidResult.Init(Guid.GuidParseThrowStyle.AllButOverflow);
			if (Guid.TryParseGuid(input, Guid.GuidStyles.Any, ref guidResult))
			{
				return guidResult.parsedGuid;
			}
			throw guidResult.GetGuidParseException();
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string input, out Guid result)
		{
			Guid.GuidResult guidResult = default(Guid.GuidResult);
			guidResult.Init(Guid.GuidParseThrowStyle.None);
			if (Guid.TryParseGuid(input, Guid.GuidStyles.Any, ref guidResult))
			{
				result = guidResult.parsedGuid;
				return true;
			}
			result = Guid.Empty;
			return false;
		}

		[__DynamicallyInvokable]
		public static Guid ParseExact(string input, string format)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (format.Length != 1)
			{
				throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
			}
			char c = format[0];
			Guid.GuidStyles flags;
			if (c == 'D' || c == 'd')
			{
				flags = Guid.GuidStyles.RequireDashes;
			}
			else if (c == 'N' || c == 'n')
			{
				flags = Guid.GuidStyles.None;
			}
			else if (c == 'B' || c == 'b')
			{
				flags = Guid.GuidStyles.BraceFormat;
			}
			else if (c == 'P' || c == 'p')
			{
				flags = Guid.GuidStyles.ParenthesisFormat;
			}
			else
			{
				if (c != 'X' && c != 'x')
				{
					throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
				}
				flags = Guid.GuidStyles.HexFormat;
			}
			Guid.GuidResult guidResult = default(Guid.GuidResult);
			guidResult.Init(Guid.GuidParseThrowStyle.AllButOverflow);
			if (Guid.TryParseGuid(input, flags, ref guidResult))
			{
				return guidResult.parsedGuid;
			}
			throw guidResult.GetGuidParseException();
		}

		[__DynamicallyInvokable]
		public static bool TryParseExact(string input, string format, out Guid result)
		{
			if (format == null || format.Length != 1)
			{
				result = Guid.Empty;
				return false;
			}
			char c = format[0];
			Guid.GuidStyles flags;
			if (c == 'D' || c == 'd')
			{
				flags = Guid.GuidStyles.RequireDashes;
			}
			else if (c == 'N' || c == 'n')
			{
				flags = Guid.GuidStyles.None;
			}
			else if (c == 'B' || c == 'b')
			{
				flags = Guid.GuidStyles.BraceFormat;
			}
			else if (c == 'P' || c == 'p')
			{
				flags = Guid.GuidStyles.ParenthesisFormat;
			}
			else
			{
				if (c != 'X' && c != 'x')
				{
					result = Guid.Empty;
					return false;
				}
				flags = Guid.GuidStyles.HexFormat;
			}
			Guid.GuidResult guidResult = default(Guid.GuidResult);
			guidResult.Init(Guid.GuidParseThrowStyle.None);
			if (Guid.TryParseGuid(input, flags, ref guidResult))
			{
				result = guidResult.parsedGuid;
				return true;
			}
			result = Guid.Empty;
			return false;
		}

		private static bool TryParseGuid(string g, Guid.GuidStyles flags, ref Guid.GuidResult result)
		{
			if (g == null)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
				return false;
			}
			string text = g.Trim();
			if (text.Length == 0)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
				return false;
			}
			bool flag = text.IndexOf('-', 0) >= 0;
			if (flag)
			{
				if ((flags & (Guid.GuidStyles.AllowDashes | Guid.GuidStyles.RequireDashes)) == Guid.GuidStyles.None)
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
					return false;
				}
			}
			else if ((flags & Guid.GuidStyles.RequireDashes) != Guid.GuidStyles.None)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
				return false;
			}
			bool flag2 = text.IndexOf('{', 0) >= 0;
			if (flag2)
			{
				if ((flags & (Guid.GuidStyles.AllowBraces | Guid.GuidStyles.RequireBraces)) == Guid.GuidStyles.None)
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
					return false;
				}
			}
			else if ((flags & Guid.GuidStyles.RequireBraces) != Guid.GuidStyles.None)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
				return false;
			}
			bool flag3 = text.IndexOf('(', 0) >= 0;
			if (flag3)
			{
				if ((flags & (Guid.GuidStyles.AllowParenthesis | Guid.GuidStyles.RequireParenthesis)) == Guid.GuidStyles.None)
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
					return false;
				}
			}
			else if ((flags & Guid.GuidStyles.RequireParenthesis) != Guid.GuidStyles.None)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidUnrecognized");
				return false;
			}
			bool result2;
			try
			{
				if (flag)
				{
					result2 = Guid.TryParseGuidWithDashes(text, ref result);
				}
				else if (flag2)
				{
					result2 = Guid.TryParseGuidWithHexPrefix(text, ref result);
				}
				else
				{
					result2 = Guid.TryParseGuidWithNoStyle(text, ref result);
				}
			}
			catch (IndexOutOfRangeException innerException)
			{
				result.SetFailure(Guid.ParseFailureKind.FormatWithInnerException, "Format_GuidUnrecognized", null, null, innerException);
				result2 = false;
			}
			catch (ArgumentException innerException2)
			{
				result.SetFailure(Guid.ParseFailureKind.FormatWithInnerException, "Format_GuidUnrecognized", null, null, innerException2);
				result2 = false;
			}
			return result2;
		}

		private static bool TryParseGuidWithHexPrefix(string guidString, ref Guid.GuidResult result)
		{
			guidString = Guid.EatAllWhitespace(guidString);
			if (string.IsNullOrEmpty(guidString) || guidString[0] != '{')
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidBrace");
				return false;
			}
			if (!Guid.IsHexPrefix(guidString, 1))
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidHexPrefix", "{0xdddddddd, etc}");
				return false;
			}
			int num = 3;
			int num2 = guidString.IndexOf(',', num) - num;
			if (num2 <= 0)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidComma");
				return false;
			}
			if (!Guid.StringToInt(guidString.Substring(num, num2), -1, 4096, out result.parsedGuid._a, ref result))
			{
				return false;
			}
			if (!Guid.IsHexPrefix(guidString, num + num2 + 1))
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidHexPrefix", "{0xdddddddd, 0xdddd, etc}");
				return false;
			}
			num = num + num2 + 3;
			num2 = guidString.IndexOf(',', num) - num;
			if (num2 <= 0)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidComma");
				return false;
			}
			if (!Guid.StringToShort(guidString.Substring(num, num2), -1, 4096, out result.parsedGuid._b, ref result))
			{
				return false;
			}
			if (!Guid.IsHexPrefix(guidString, num + num2 + 1))
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidHexPrefix", "{0xdddddddd, 0xdddd, 0xdddd, etc}");
				return false;
			}
			num = num + num2 + 3;
			num2 = guidString.IndexOf(',', num) - num;
			if (num2 <= 0)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidComma");
				return false;
			}
			if (!Guid.StringToShort(guidString.Substring(num, num2), -1, 4096, out result.parsedGuid._c, ref result))
			{
				return false;
			}
			if (guidString.Length <= num + num2 + 1 || guidString[num + num2 + 1] != '{')
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidBrace");
				return false;
			}
			num2++;
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				if (!Guid.IsHexPrefix(guidString, num + num2 + 1))
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidHexPrefix", "{... { ... 0xdd, ...}}");
					return false;
				}
				num = num + num2 + 3;
				if (i < 7)
				{
					num2 = guidString.IndexOf(',', num) - num;
					if (num2 <= 0)
					{
						result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidComma");
						return false;
					}
				}
				else
				{
					num2 = guidString.IndexOf('}', num) - num;
					if (num2 <= 0)
					{
						result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidBraceAfterLastNumber");
						return false;
					}
				}
				uint num3 = (uint)Convert.ToInt32(guidString.Substring(num, num2), 16);
				if (num3 > 255U)
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Overflow_Byte");
					return false;
				}
				array[i] = (byte)num3;
			}
			result.parsedGuid._d = array[0];
			result.parsedGuid._e = array[1];
			result.parsedGuid._f = array[2];
			result.parsedGuid._g = array[3];
			result.parsedGuid._h = array[4];
			result.parsedGuid._i = array[5];
			result.parsedGuid._j = array[6];
			result.parsedGuid._k = array[7];
			if (num + num2 + 1 >= guidString.Length || guidString[num + num2 + 1] != '}')
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidEndBrace");
				return false;
			}
			if (num + num2 + 1 != guidString.Length - 1)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_ExtraJunkAtEnd");
				return false;
			}
			return true;
		}

		private static bool TryParseGuidWithNoStyle(string guidString, ref Guid.GuidResult result)
		{
			int num = 0;
			int num2 = 0;
			if (guidString.Length != 32)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
				return false;
			}
			foreach (char c in guidString)
			{
				if (c < '0' || c > '9')
				{
					char c2 = char.ToUpper(c, CultureInfo.InvariantCulture);
					if (c2 < 'A' || c2 > 'F')
					{
						result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvalidChar");
						return false;
					}
				}
			}
			if (!Guid.StringToInt(guidString.Substring(num, 8), -1, 4096, out result.parsedGuid._a, ref result))
			{
				return false;
			}
			num += 8;
			if (!Guid.StringToShort(guidString.Substring(num, 4), -1, 4096, out result.parsedGuid._b, ref result))
			{
				return false;
			}
			num += 4;
			if (!Guid.StringToShort(guidString.Substring(num, 4), -1, 4096, out result.parsedGuid._c, ref result))
			{
				return false;
			}
			num += 4;
			int num3;
			if (!Guid.StringToInt(guidString.Substring(num, 4), -1, 4096, out num3, ref result))
			{
				return false;
			}
			num += 4;
			num2 = num;
			long num4;
			if (!Guid.StringToLong(guidString, ref num2, num, out num4, ref result))
			{
				return false;
			}
			if (num2 - num != 12)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
				return false;
			}
			result.parsedGuid._d = (byte)(num3 >> 8);
			result.parsedGuid._e = (byte)num3;
			num3 = (int)(num4 >> 32);
			result.parsedGuid._f = (byte)(num3 >> 8);
			result.parsedGuid._g = (byte)num3;
			num3 = (int)num4;
			result.parsedGuid._h = (byte)(num3 >> 24);
			result.parsedGuid._i = (byte)(num3 >> 16);
			result.parsedGuid._j = (byte)(num3 >> 8);
			result.parsedGuid._k = (byte)num3;
			return true;
		}

		private static bool TryParseGuidWithDashes(string guidString, ref Guid.GuidResult result)
		{
			int num = 0;
			int num2 = 0;
			if (guidString[0] == '{')
			{
				if (guidString.Length != 38 || guidString[37] != '}')
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
					return false;
				}
				num = 1;
			}
			else if (guidString[0] == '(')
			{
				if (guidString.Length != 38 || guidString[37] != ')')
				{
					result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
					return false;
				}
				num = 1;
			}
			else if (guidString.Length != 36)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
				return false;
			}
			if (guidString[8 + num] != '-' || guidString[13 + num] != '-' || guidString[18 + num] != '-' || guidString[23 + num] != '-')
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidDashes");
				return false;
			}
			num2 = num;
			int num3;
			if (!Guid.StringToInt(guidString, ref num2, 8, 8192, out num3, ref result))
			{
				return false;
			}
			result.parsedGuid._a = num3;
			num2++;
			if (!Guid.StringToInt(guidString, ref num2, 4, 8192, out num3, ref result))
			{
				return false;
			}
			result.parsedGuid._b = (short)num3;
			num2++;
			if (!Guid.StringToInt(guidString, ref num2, 4, 8192, out num3, ref result))
			{
				return false;
			}
			result.parsedGuid._c = (short)num3;
			num2++;
			if (!Guid.StringToInt(guidString, ref num2, 4, 8192, out num3, ref result))
			{
				return false;
			}
			num2++;
			num = num2;
			long num4;
			if (!Guid.StringToLong(guidString, ref num2, 8192, out num4, ref result))
			{
				return false;
			}
			if (num2 - num != 12)
			{
				result.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvLen");
				return false;
			}
			result.parsedGuid._d = (byte)(num3 >> 8);
			result.parsedGuid._e = (byte)num3;
			num3 = (int)(num4 >> 32);
			result.parsedGuid._f = (byte)(num3 >> 8);
			result.parsedGuid._g = (byte)num3;
			num3 = (int)num4;
			result.parsedGuid._h = (byte)(num3 >> 24);
			result.parsedGuid._i = (byte)(num3 >> 16);
			result.parsedGuid._j = (byte)(num3 >> 8);
			result.parsedGuid._k = (byte)num3;
			return true;
		}

		[SecuritySafeCritical]
		private static bool StringToShort(string str, int requiredLength, int flags, out short result, ref Guid.GuidResult parseResult)
		{
			return Guid.StringToShort(str, null, requiredLength, flags, out result, ref parseResult);
		}

		[SecuritySafeCritical]
		private unsafe static bool StringToShort(string str, ref int parsePos, int requiredLength, int flags, out short result, ref Guid.GuidResult parseResult)
		{
			fixed (int* ptr = &parsePos)
			{
				return Guid.StringToShort(str, ptr, requiredLength, flags, out result, ref parseResult);
			}
		}

		[SecurityCritical]
		private unsafe static bool StringToShort(string str, int* parsePos, int requiredLength, int flags, out short result, ref Guid.GuidResult parseResult)
		{
			result = 0;
			int num;
			bool result2 = Guid.StringToInt(str, parsePos, requiredLength, flags, out num, ref parseResult);
			result = (short)num;
			return result2;
		}

		[SecuritySafeCritical]
		private static bool StringToInt(string str, int requiredLength, int flags, out int result, ref Guid.GuidResult parseResult)
		{
			return Guid.StringToInt(str, null, requiredLength, flags, out result, ref parseResult);
		}

		[SecuritySafeCritical]
		private unsafe static bool StringToInt(string str, ref int parsePos, int requiredLength, int flags, out int result, ref Guid.GuidResult parseResult)
		{
			fixed (int* ptr = &parsePos)
			{
				return Guid.StringToInt(str, ptr, requiredLength, flags, out result, ref parseResult);
			}
		}

		[SecurityCritical]
		private unsafe static bool StringToInt(string str, int* parsePos, int requiredLength, int flags, out int result, ref Guid.GuidResult parseResult)
		{
			result = 0;
			int num = (parsePos == null) ? 0 : (*parsePos);
			try
			{
				result = ParseNumbers.StringToInt(str, 16, flags, parsePos);
			}
			catch (OverflowException ex)
			{
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.All)
				{
					throw;
				}
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.AllButOverflow)
				{
					throw new FormatException(Environment.GetResourceString("Format_GuidUnrecognized"), ex);
				}
				parseResult.SetFailure(ex);
				return false;
			}
			catch (Exception failure)
			{
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.None)
				{
					parseResult.SetFailure(failure);
					return false;
				}
				throw;
			}
			if (requiredLength != -1 && parsePos != null && *parsePos - num != requiredLength)
			{
				parseResult.SetFailure(Guid.ParseFailureKind.Format, "Format_GuidInvalidChar");
				return false;
			}
			return true;
		}

		[SecuritySafeCritical]
		private static bool StringToLong(string str, int flags, out long result, ref Guid.GuidResult parseResult)
		{
			return Guid.StringToLong(str, null, flags, out result, ref parseResult);
		}

		[SecuritySafeCritical]
		private unsafe static bool StringToLong(string str, ref int parsePos, int flags, out long result, ref Guid.GuidResult parseResult)
		{
			fixed (int* ptr = &parsePos)
			{
				return Guid.StringToLong(str, ptr, flags, out result, ref parseResult);
			}
		}

		[SecuritySafeCritical]
		private unsafe static bool StringToLong(string str, int* parsePos, int flags, out long result, ref Guid.GuidResult parseResult)
		{
			result = 0L;
			try
			{
				result = ParseNumbers.StringToLong(str, 16, flags, parsePos);
			}
			catch (OverflowException ex)
			{
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.All)
				{
					throw;
				}
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.AllButOverflow)
				{
					throw new FormatException(Environment.GetResourceString("Format_GuidUnrecognized"), ex);
				}
				parseResult.SetFailure(ex);
				return false;
			}
			catch (Exception failure)
			{
				if (parseResult.throwStyle == Guid.GuidParseThrowStyle.None)
				{
					parseResult.SetFailure(failure);
					return false;
				}
				throw;
			}
			return true;
		}

		private static string EatAllWhitespace(string str)
		{
			int length = 0;
			char[] array = new char[str.Length];
			foreach (char c in str)
			{
				if (!char.IsWhiteSpace(c))
				{
					array[length++] = c;
				}
			}
			return new string(array, 0, length);
		}

		private static bool IsHexPrefix(string str, int i)
		{
			return str.Length > i + 1 && str[i] == '0' && char.ToLower(str[i + 1], CultureInfo.InvariantCulture) == 'x';
		}

		[__DynamicallyInvokable]
		public byte[] ToByteArray()
		{
			return new byte[]
			{
				(byte)this._a,
				(byte)(this._a >> 8),
				(byte)(this._a >> 16),
				(byte)(this._a >> 24),
				(byte)this._b,
				(byte)(this._b >> 8),
				(byte)this._c,
				(byte)(this._c >> 8),
				this._d,
				this._e,
				this._f,
				this._g,
				this._h,
				this._i,
				this._j,
				this._k
			};
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.ToString("D", null);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this._a ^ ((int)this._b << 16 | (int)((ushort)this._c)) ^ ((int)this._f << 24 | (int)this._k);
		}

		[__DynamicallyInvokable]
		public override bool Equals(object o)
		{
			if (o == null || !(o is Guid))
			{
				return false;
			}
			Guid guid = (Guid)o;
			return guid._a == this._a && guid._b == this._b && guid._c == this._c && guid._d == this._d && guid._e == this._e && guid._f == this._f && guid._g == this._g && guid._h == this._h && guid._i == this._i && guid._j == this._j && guid._k == this._k;
		}

		[__DynamicallyInvokable]
		public bool Equals(Guid g)
		{
			return g._a == this._a && g._b == this._b && g._c == this._c && g._d == this._d && g._e == this._e && g._f == this._f && g._g == this._g && g._h == this._h && g._i == this._i && g._j == this._j && g._k == this._k;
		}

		private int GetResult(uint me, uint them)
		{
			if (me < them)
			{
				return -1;
			}
			return 1;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Guid))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeGuid"));
			}
			Guid guid = (Guid)value;
			if (guid._a != this._a)
			{
				return this.GetResult((uint)this._a, (uint)guid._a);
			}
			if (guid._b != this._b)
			{
				return this.GetResult((uint)this._b, (uint)guid._b);
			}
			if (guid._c != this._c)
			{
				return this.GetResult((uint)this._c, (uint)guid._c);
			}
			if (guid._d != this._d)
			{
				return this.GetResult((uint)this._d, (uint)guid._d);
			}
			if (guid._e != this._e)
			{
				return this.GetResult((uint)this._e, (uint)guid._e);
			}
			if (guid._f != this._f)
			{
				return this.GetResult((uint)this._f, (uint)guid._f);
			}
			if (guid._g != this._g)
			{
				return this.GetResult((uint)this._g, (uint)guid._g);
			}
			if (guid._h != this._h)
			{
				return this.GetResult((uint)this._h, (uint)guid._h);
			}
			if (guid._i != this._i)
			{
				return this.GetResult((uint)this._i, (uint)guid._i);
			}
			if (guid._j != this._j)
			{
				return this.GetResult((uint)this._j, (uint)guid._j);
			}
			if (guid._k != this._k)
			{
				return this.GetResult((uint)this._k, (uint)guid._k);
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public int CompareTo(Guid value)
		{
			if (value._a != this._a)
			{
				return this.GetResult((uint)this._a, (uint)value._a);
			}
			if (value._b != this._b)
			{
				return this.GetResult((uint)this._b, (uint)value._b);
			}
			if (value._c != this._c)
			{
				return this.GetResult((uint)this._c, (uint)value._c);
			}
			if (value._d != this._d)
			{
				return this.GetResult((uint)this._d, (uint)value._d);
			}
			if (value._e != this._e)
			{
				return this.GetResult((uint)this._e, (uint)value._e);
			}
			if (value._f != this._f)
			{
				return this.GetResult((uint)this._f, (uint)value._f);
			}
			if (value._g != this._g)
			{
				return this.GetResult((uint)this._g, (uint)value._g);
			}
			if (value._h != this._h)
			{
				return this.GetResult((uint)this._h, (uint)value._h);
			}
			if (value._i != this._i)
			{
				return this.GetResult((uint)this._i, (uint)value._i);
			}
			if (value._j != this._j)
			{
				return this.GetResult((uint)this._j, (uint)value._j);
			}
			if (value._k != this._k)
			{
				return this.GetResult((uint)this._k, (uint)value._k);
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(Guid a, Guid b)
		{
			return a._a == b._a && a._b == b._b && a._c == b._c && a._d == b._d && a._e == b._e && a._f == b._f && a._g == b._g && a._h == b._h && a._i == b._i && a._j == b._j && a._k == b._k;
		}

		[__DynamicallyInvokable]
		public static bool operator !=(Guid a, Guid b)
		{
			return !(a == b);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static Guid NewGuid()
		{
			Guid result;
			Marshal.ThrowExceptionForHR(Win32Native.CoCreateGuid(out result), new IntPtr(-1));
			return result;
		}

		[__DynamicallyInvokable]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		private static char HexToChar(int a)
		{
			a &= 15;
			return (char)((a > 9) ? (a - 10 + 97) : (a + 48));
		}

		[SecurityCritical]
		private unsafe static int HexsToChars(char* guidChars, int offset, int a, int b)
		{
			return Guid.HexsToChars(guidChars, offset, a, b, false);
		}

		[SecurityCritical]
		private unsafe static int HexsToChars(char* guidChars, int offset, int a, int b, bool hex)
		{
			if (hex)
			{
				guidChars[offset++] = '0';
				guidChars[offset++] = 'x';
			}
			guidChars[offset++] = Guid.HexToChar(a >> 4);
			guidChars[offset++] = Guid.HexToChar(a);
			if (hex)
			{
				guidChars[offset++] = ',';
				guidChars[offset++] = '0';
				guidChars[offset++] = 'x';
			}
			guidChars[offset++] = Guid.HexToChar(b >> 4);
			guidChars[offset++] = Guid.HexToChar(b);
			return offset;
		}

		[SecuritySafeCritical]
		public unsafe string ToString(string format, IFormatProvider provider)
		{
			if (format == null || format.Length == 0)
			{
				format = "D";
			}
			int offset = 0;
			bool flag = true;
			bool flag2 = false;
			if (format.Length != 1)
			{
				throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
			}
			char c = format[0];
			string text;
			if (c == 'D' || c == 'd')
			{
				text = string.FastAllocateString(36);
			}
			else if (c == 'N' || c == 'n')
			{
				text = string.FastAllocateString(32);
				flag = false;
			}
			else if (c == 'B' || c == 'b')
			{
				text = string.FastAllocateString(38);
				fixed (string text2 = text)
				{
					char* ptr = text2;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr[(IntPtr)(offset++) * 2] = '{';
					ptr[37] = '}';
				}
			}
			else if (c == 'P' || c == 'p')
			{
				text = string.FastAllocateString(38);
				fixed (string text3 = text)
				{
					char* ptr2 = text3;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr2[(IntPtr)(offset++) * 2] = '(';
					ptr2[37] = ')';
				}
			}
			else
			{
				if (c != 'X' && c != 'x')
				{
					throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
				}
				text = string.FastAllocateString(68);
				fixed (string text4 = text)
				{
					char* ptr3 = text4;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr3[(IntPtr)(offset++) * 2] = '{';
					ptr3[67] = '}';
				}
				flag = false;
				flag2 = true;
			}
			fixed (string text5 = text)
			{
				char* ptr4 = text5;
				if (ptr4 != null)
				{
					ptr4 += RuntimeHelpers.OffsetToStringData / 2;
				}
				if (flag2)
				{
					ptr4[(IntPtr)(offset++) * 2] = '0';
					ptr4[(IntPtr)(offset++) * 2] = 'x';
					offset = Guid.HexsToChars(ptr4, offset, this._a >> 24, this._a >> 16);
					offset = Guid.HexsToChars(ptr4, offset, this._a >> 8, this._a);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					ptr4[(IntPtr)(offset++) * 2] = '0';
					ptr4[(IntPtr)(offset++) * 2] = 'x';
					offset = Guid.HexsToChars(ptr4, offset, this._b >> 8, (int)this._b);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					ptr4[(IntPtr)(offset++) * 2] = '0';
					ptr4[(IntPtr)(offset++) * 2] = 'x';
					offset = Guid.HexsToChars(ptr4, offset, this._c >> 8, (int)this._c);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					ptr4[(IntPtr)(offset++) * 2] = '{';
					offset = Guid.HexsToChars(ptr4, offset, (int)this._d, (int)this._e, true);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					offset = Guid.HexsToChars(ptr4, offset, (int)this._f, (int)this._g, true);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					offset = Guid.HexsToChars(ptr4, offset, (int)this._h, (int)this._i, true);
					ptr4[(IntPtr)(offset++) * 2] = ',';
					offset = Guid.HexsToChars(ptr4, offset, (int)this._j, (int)this._k, true);
					ptr4[(IntPtr)(offset++) * 2] = '}';
				}
				else
				{
					offset = Guid.HexsToChars(ptr4, offset, this._a >> 24, this._a >> 16);
					offset = Guid.HexsToChars(ptr4, offset, this._a >> 8, this._a);
					if (flag)
					{
						ptr4[(IntPtr)(offset++) * 2] = '-';
					}
					offset = Guid.HexsToChars(ptr4, offset, this._b >> 8, (int)this._b);
					if (flag)
					{
						ptr4[(IntPtr)(offset++) * 2] = '-';
					}
					offset = Guid.HexsToChars(ptr4, offset, this._c >> 8, (int)this._c);
					if (flag)
					{
						ptr4[(IntPtr)(offset++) * 2] = '-';
					}
					offset = Guid.HexsToChars(ptr4, offset, (int)this._d, (int)this._e);
					if (flag)
					{
						ptr4[(IntPtr)(offset++) * 2] = '-';
					}
					offset = Guid.HexsToChars(ptr4, offset, (int)this._f, (int)this._g);
					offset = Guid.HexsToChars(ptr4, offset, (int)this._h, (int)this._i);
					offset = Guid.HexsToChars(ptr4, offset, (int)this._j, (int)this._k);
				}
			}
			return text;
		}

		[__DynamicallyInvokable]
		public static readonly Guid Empty;

		private int _a;

		private short _b;

		private short _c;

		private byte _d;

		private byte _e;

		private byte _f;

		private byte _g;

		private byte _h;

		private byte _i;

		private byte _j;

		private byte _k;

		[Flags]
		private enum GuidStyles
		{
			None = 0,
			AllowParenthesis = 1,
			AllowBraces = 2,
			AllowDashes = 4,
			AllowHexPrefix = 8,
			RequireParenthesis = 16,
			RequireBraces = 32,
			RequireDashes = 64,
			RequireHexPrefix = 128,
			HexFormat = 160,
			NumberFormat = 0,
			DigitFormat = 64,
			BraceFormat = 96,
			ParenthesisFormat = 80,
			Any = 15
		}

		private enum GuidParseThrowStyle
		{
			None,
			All,
			AllButOverflow
		}

		private enum ParseFailureKind
		{
			None,
			ArgumentNull,
			Format,
			FormatWithParameter,
			NativeException,
			FormatWithInnerException
		}

		private struct GuidResult
		{
			internal void Init(Guid.GuidParseThrowStyle canThrow)
			{
				this.parsedGuid = Guid.Empty;
				this.throwStyle = canThrow;
			}

			internal void SetFailure(Exception nativeException)
			{
				this.m_failure = Guid.ParseFailureKind.NativeException;
				this.m_innerException = nativeException;
			}

			internal void SetFailure(Guid.ParseFailureKind failure, string failureMessageID)
			{
				this.SetFailure(failure, failureMessageID, null, null, null);
			}

			internal void SetFailure(Guid.ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument)
			{
				this.SetFailure(failure, failureMessageID, failureMessageFormatArgument, null, null);
			}

			internal void SetFailure(Guid.ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument, string failureArgumentName, Exception innerException)
			{
				this.m_failure = failure;
				this.m_failureMessageID = failureMessageID;
				this.m_failureMessageFormatArgument = failureMessageFormatArgument;
				this.m_failureArgumentName = failureArgumentName;
				this.m_innerException = innerException;
				if (this.throwStyle != Guid.GuidParseThrowStyle.None)
				{
					throw this.GetGuidParseException();
				}
			}

			internal Exception GetGuidParseException()
			{
				switch (this.m_failure)
				{
				case Guid.ParseFailureKind.ArgumentNull:
					return new ArgumentNullException(this.m_failureArgumentName, Environment.GetResourceString(this.m_failureMessageID));
				case Guid.ParseFailureKind.Format:
					return new FormatException(Environment.GetResourceString(this.m_failureMessageID));
				case Guid.ParseFailureKind.FormatWithParameter:
					return new FormatException(Environment.GetResourceString(this.m_failureMessageID, new object[]
					{
						this.m_failureMessageFormatArgument
					}));
				case Guid.ParseFailureKind.NativeException:
					return this.m_innerException;
				case Guid.ParseFailureKind.FormatWithInnerException:
					return new FormatException(Environment.GetResourceString(this.m_failureMessageID), this.m_innerException);
				default:
					return new FormatException(Environment.GetResourceString("Format_GuidUnrecognized"));
				}
			}

			internal Guid parsedGuid;

			internal Guid.GuidParseThrowStyle throwStyle;

			internal Guid.ParseFailureKind m_failure;

			internal string m_failureMessageID;

			internal object m_failureMessageFormatArgument;

			internal string m_failureArgumentName;

			internal Exception m_innerException;
		}
	}
}
