using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[NonVersionable]
	[__DynamicallyInvokable]
	[Serializable]
	public struct Decimal : IFormattable, IComparable, IConvertible, IDeserializationCallback, IComparable<decimal>, IEquatable<decimal>
	{
		[__DynamicallyInvokable]
		public Decimal(int value)
		{
			int num = value;
			if (num >= 0)
			{
				this.flags = 0;
			}
			else
			{
				this.flags = int.MinValue;
				num = -num;
			}
			this.lo = num;
			this.mid = 0;
			this.hi = 0;
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public Decimal(uint value)
		{
			this.flags = 0;
			this.lo = (int)value;
			this.mid = 0;
			this.hi = 0;
		}

		[__DynamicallyInvokable]
		public Decimal(long value)
		{
			long num = value;
			if (num >= 0L)
			{
				this.flags = 0;
			}
			else
			{
				this.flags = int.MinValue;
				num = -num;
			}
			this.lo = (int)num;
			this.mid = (int)(num >> 32);
			this.hi = 0;
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public Decimal(ulong value)
		{
			this.flags = 0;
			this.lo = (int)value;
			this.mid = (int)(value >> 32);
			this.hi = 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Decimal(float value);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Decimal(double value);

		internal Decimal(Currency value)
		{
			decimal num = Currency.ToDecimal(value);
			this.lo = num.lo;
			this.mid = num.mid;
			this.hi = num.hi;
			this.flags = num.flags;
		}

		[__DynamicallyInvokable]
		public static long ToOACurrency(decimal value)
		{
			return new Currency(value).ToOACurrency();
		}

		[__DynamicallyInvokable]
		public static decimal FromOACurrency(long cy)
		{
			return Currency.ToDecimal(Currency.FromOACurrency(cy));
		}

		[__DynamicallyInvokable]
		public Decimal(int[] bits)
		{
			this.lo = 0;
			this.mid = 0;
			this.hi = 0;
			this.flags = 0;
			this.SetBits(bits);
		}

		private void SetBits(int[] bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException("bits");
			}
			if (bits.Length == 4)
			{
				int num = bits[3];
				if ((num & 2130771967) == 0 && (num & 16711680) <= 1835008)
				{
					this.lo = bits[0];
					this.mid = bits[1];
					this.hi = bits[2];
					this.flags = num;
					return;
				}
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_DecBitCtor"));
		}

		[__DynamicallyInvokable]
		public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
		{
			if (scale > 28)
			{
				throw new ArgumentOutOfRangeException("scale", Environment.GetResourceString("ArgumentOutOfRange_DecimalScale"));
			}
			this.lo = lo;
			this.mid = mid;
			this.hi = hi;
			this.flags = (int)scale << 16;
			if (isNegative)
			{
				this.flags |= int.MinValue;
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			try
			{
				this.SetBits(decimal.GetBits(this));
			}
			catch (ArgumentException innerException)
			{
				throw new SerializationException(Environment.GetResourceString("Overflow_Decimal"), innerException);
			}
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				this.SetBits(decimal.GetBits(this));
			}
			catch (ArgumentException innerException)
			{
				throw new SerializationException(Environment.GetResourceString("Overflow_Decimal"), innerException);
			}
		}

		private Decimal(int lo, int mid, int hi, int flags)
		{
			if ((flags & 2130771967) == 0 && (flags & 16711680) <= 1835008)
			{
				this.lo = lo;
				this.mid = mid;
				this.hi = hi;
				this.flags = flags;
				return;
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_DecBitCtor"));
		}

		internal static decimal Abs(decimal d)
		{
			return new decimal(d.lo, d.mid, d.hi, d.flags & int.MaxValue);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Add(decimal d1, decimal d2)
		{
			decimal.FCallAddSub(ref d1, ref d2, 0);
			return d1;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallAddSub(ref decimal d1, ref decimal d2, byte bSign);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallAddSubOverflowed(ref decimal d1, ref decimal d2, byte bSign, ref bool overflowed);

		[__DynamicallyInvokable]
		public static decimal Ceiling(decimal d)
		{
			return -decimal.Floor(-d);
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[__DynamicallyInvokable]
		public static int Compare(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FCallCompare(ref decimal d1, ref decimal d2);

		[SecuritySafeCritical]
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is decimal))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDecimal"));
			}
			decimal num = (decimal)value;
			return decimal.FCallCompare(ref this, ref num);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public int CompareTo(decimal value)
		{
			return decimal.FCallCompare(ref this, ref value);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Divide(decimal d1, decimal d2)
		{
			decimal.FCallDivide(ref d1, ref d2);
			return d1;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallDivide(ref decimal d1, ref decimal d2);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallDivideOverflowed(ref decimal d1, ref decimal d2, ref bool overflowed);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override bool Equals(object value)
		{
			if (value is decimal)
			{
				decimal num = (decimal)value;
				return decimal.FCallCompare(ref this, ref num) == 0;
			}
			return false;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public bool Equals(decimal value)
		{
			return decimal.FCallCompare(ref this, ref value) == 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern int GetHashCode();

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool Equals(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) == 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Floor(decimal d)
		{
			decimal.FCallFloor(ref d);
			return d;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallFloor(ref decimal d);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override string ToString()
		{
			return Number.FormatDecimal(this, null, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(string format)
		{
			return Number.FormatDecimal(this, format, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(IFormatProvider provider)
		{
			return Number.FormatDecimal(this, null, NumberFormatInfo.GetInstance(provider));
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(string format, IFormatProvider provider)
		{
			return Number.FormatDecimal(this, format, NumberFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public static decimal Parse(string s)
		{
			return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo);
		}

		[__DynamicallyInvokable]
		public static decimal Parse(string s, NumberStyles style)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			return Number.ParseDecimal(s, style, NumberFormatInfo.CurrentInfo);
		}

		[__DynamicallyInvokable]
		public static decimal Parse(string s, IFormatProvider provider)
		{
			return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public static decimal Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			return Number.ParseDecimal(s, style, NumberFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string s, out decimal result)
		{
			return Number.TryParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out result);
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out decimal result)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			return Number.TryParseDecimal(s, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		[__DynamicallyInvokable]
		public static int[] GetBits(decimal d)
		{
			return new int[]
			{
				d.lo,
				d.mid,
				d.hi,
				d.flags
			};
		}

		internal static void GetBytes(decimal d, byte[] buffer)
		{
			buffer[0] = (byte)d.lo;
			buffer[1] = (byte)(d.lo >> 8);
			buffer[2] = (byte)(d.lo >> 16);
			buffer[3] = (byte)(d.lo >> 24);
			buffer[4] = (byte)d.mid;
			buffer[5] = (byte)(d.mid >> 8);
			buffer[6] = (byte)(d.mid >> 16);
			buffer[7] = (byte)(d.mid >> 24);
			buffer[8] = (byte)d.hi;
			buffer[9] = (byte)(d.hi >> 8);
			buffer[10] = (byte)(d.hi >> 16);
			buffer[11] = (byte)(d.hi >> 24);
			buffer[12] = (byte)d.flags;
			buffer[13] = (byte)(d.flags >> 8);
			buffer[14] = (byte)(d.flags >> 16);
			buffer[15] = (byte)(d.flags >> 24);
		}

		internal static decimal ToDecimal(byte[] buffer)
		{
			int num = (int)buffer[0] | (int)buffer[1] << 8 | (int)buffer[2] << 16 | (int)buffer[3] << 24;
			int num2 = (int)buffer[4] | (int)buffer[5] << 8 | (int)buffer[6] << 16 | (int)buffer[7] << 24;
			int num3 = (int)buffer[8] | (int)buffer[9] << 8 | (int)buffer[10] << 16 | (int)buffer[11] << 24;
			int num4 = (int)buffer[12] | (int)buffer[13] << 8 | (int)buffer[14] << 16 | (int)buffer[15] << 24;
			return new decimal(num, num2, num3, num4);
		}

		private static void InternalAddUInt32RawUnchecked(ref decimal value, uint i)
		{
			uint num = (uint)value.lo;
			uint num2 = num + i;
			value.lo = (int)num2;
			if (num2 < num || num2 < i)
			{
				num = (uint)value.mid;
				num2 = num + 1U;
				value.mid = (int)num2;
				if (num2 < num || num2 < 1U)
				{
					value.hi++;
				}
			}
		}

		private static uint InternalDivRemUInt32(ref decimal value, uint divisor)
		{
			uint num = 0U;
			if (value.hi != 0)
			{
				ulong num2 = (ulong)value.hi;
				value.hi = (int)((uint)(num2 / (ulong)divisor));
				num = (uint)(num2 % (ulong)divisor);
			}
			if (value.mid != 0 || num != 0U)
			{
				ulong num2 = (ulong)num << 32 | (ulong)value.mid;
				value.mid = (int)((uint)(num2 / (ulong)divisor));
				num = (uint)(num2 % (ulong)divisor);
			}
			if (value.lo != 0 || num != 0U)
			{
				ulong num2 = (ulong)num << 32 | (ulong)value.lo;
				value.lo = (int)((uint)(num2 / (ulong)divisor));
				num = (uint)(num2 % (ulong)divisor);
			}
			return num;
		}

		private static void InternalRoundFromZero(ref decimal d, int decimalCount)
		{
			int num = (d.flags & 16711680) >> 16;
			int num2 = num - decimalCount;
			if (num2 <= 0)
			{
				return;
			}
			uint num4;
			uint num5;
			do
			{
				int num3 = (num2 > 9) ? 9 : num2;
				num4 = decimal.Powers10[num3];
				num5 = decimal.InternalDivRemUInt32(ref d, num4);
				num2 -= num3;
			}
			while (num2 > 0);
			if (num5 >= num4 >> 1)
			{
				decimal.InternalAddUInt32RawUnchecked(ref d, 1U);
			}
			d.flags = ((decimalCount << 16 & 16711680) | (d.flags & int.MinValue));
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static decimal Max(decimal d1, decimal d2)
		{
			if (decimal.FCallCompare(ref d1, ref d2) < 0)
			{
				return d2;
			}
			return d1;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static decimal Min(decimal d1, decimal d2)
		{
			if (decimal.FCallCompare(ref d1, ref d2) >= 0)
			{
				return d2;
			}
			return d1;
		}

		[__DynamicallyInvokable]
		public static decimal Remainder(decimal d1, decimal d2)
		{
			d2.flags = ((d2.flags & int.MaxValue) | (d1.flags & int.MinValue));
			if (decimal.Abs(d1) < decimal.Abs(d2))
			{
				return d1;
			}
			d1 -= d2;
			if (d1 == 0m)
			{
				d1.flags = ((d1.flags & int.MaxValue) | (d2.flags & int.MinValue));
			}
			decimal d3 = decimal.Truncate(d1 / d2);
			decimal d4 = d3 * d2;
			decimal num = d1 - d4;
			if ((d1.flags & -2147483648) != (num.flags & -2147483648))
			{
				if (-0.000000000000000000000000001m <= num && num <= 0.000000000000000000000000001m)
				{
					num.flags = ((num.flags & int.MaxValue) | (d1.flags & int.MinValue));
				}
				else
				{
					num += d2;
				}
			}
			return num;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Multiply(decimal d1, decimal d2)
		{
			decimal.FCallMultiply(ref d1, ref d2);
			return d1;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallMultiply(ref decimal d1, ref decimal d2);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallMultiplyOverflowed(ref decimal d1, ref decimal d2, ref bool overflowed);

		[__DynamicallyInvokable]
		public static decimal Negate(decimal d)
		{
			return new decimal(d.lo, d.mid, d.hi, d.flags ^ int.MinValue);
		}

		public static decimal Round(decimal d)
		{
			return decimal.Round(d, 0);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Round(decimal d, int decimals)
		{
			decimal.FCallRound(ref d, decimals);
			return d;
		}

		public static decimal Round(decimal d, MidpointRounding mode)
		{
			return decimal.Round(d, 0, mode);
		}

		[SecuritySafeCritical]
		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			if (decimals < 0 || decimals > 28)
			{
				throw new ArgumentOutOfRangeException("decimals", Environment.GetResourceString("ArgumentOutOfRange_DecimalRound"));
			}
			if (mode < MidpointRounding.ToEven || mode > MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidEnumValue", new object[]
				{
					mode,
					"MidpointRounding"
				}), "mode");
			}
			if (mode == MidpointRounding.ToEven)
			{
				decimal.FCallRound(ref d, decimals);
			}
			else
			{
				decimal.InternalRoundFromZero(ref d, decimals);
			}
			return d;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallRound(ref decimal d, int decimals);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Subtract(decimal d1, decimal d2)
		{
			decimal.FCallAddSub(ref d1, ref d2, 128);
			return d1;
		}

		[__DynamicallyInvokable]
		public static byte ToByte(decimal value)
		{
			uint num;
			try
			{
				num = decimal.ToUInt32(value);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Byte"), innerException);
			}
			if (num < 0U || num > 255U)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Byte"));
			}
			return (byte)num;
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static sbyte ToSByte(decimal value)
		{
			int num;
			try
			{
				num = decimal.ToInt32(value);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_SByte"), innerException);
			}
			if (num < -128 || num > 127)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_SByte"));
			}
			return (sbyte)num;
		}

		[__DynamicallyInvokable]
		public static short ToInt16(decimal value)
		{
			int num;
			try
			{
				num = decimal.ToInt32(value);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Int16"), innerException);
			}
			if (num < -32768 || num > 32767)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Int16"));
			}
			return (short)num;
		}

		[SecuritySafeCritical]
		internal static Currency ToCurrency(decimal d)
		{
			Currency result = default(Currency);
			decimal.FCallToCurrency(ref result, d);
			return result;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallToCurrency(ref Currency result, decimal d);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double ToDouble(decimal d);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int FCallToInt32(decimal d);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static int ToInt32(decimal d)
		{
			if ((d.flags & 16711680) != 0)
			{
				decimal.FCallTruncate(ref d);
			}
			if (d.hi == 0 && d.mid == 0)
			{
				int num = d.lo;
				if (d.flags >= 0)
				{
					if (num >= 0)
					{
						return num;
					}
				}
				else
				{
					num = -num;
					if (num <= 0)
					{
						return num;
					}
				}
			}
			throw new OverflowException(Environment.GetResourceString("Overflow_Int32"));
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static long ToInt64(decimal d)
		{
			if ((d.flags & 16711680) != 0)
			{
				decimal.FCallTruncate(ref d);
			}
			if (d.hi == 0)
			{
				long num = ((long)d.lo & (long)((ulong)-1)) | (long)d.mid << 32;
				if (d.flags >= 0)
				{
					if (num >= 0L)
					{
						return num;
					}
				}
				else
				{
					num = -num;
					if (num <= 0L)
					{
						return num;
					}
				}
			}
			throw new OverflowException(Environment.GetResourceString("Overflow_Int64"));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static ushort ToUInt16(decimal value)
		{
			uint num;
			try
			{
				num = decimal.ToUInt32(value);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"), innerException);
			}
			if (num < 0U || num > 65535U)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"));
			}
			return (ushort)num;
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static uint ToUInt32(decimal d)
		{
			if ((d.flags & 16711680) != 0)
			{
				decimal.FCallTruncate(ref d);
			}
			if (d.hi == 0 && d.mid == 0)
			{
				uint num = (uint)d.lo;
				if (d.flags >= 0 || num == 0U)
				{
					return num;
				}
			}
			throw new OverflowException(Environment.GetResourceString("Overflow_UInt32"));
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static ulong ToUInt64(decimal d)
		{
			if ((d.flags & 16711680) != 0)
			{
				decimal.FCallTruncate(ref d);
			}
			if (d.hi == 0)
			{
				ulong num = (ulong)d.lo | (ulong)d.mid << 32;
				if (d.flags >= 0 || num == 0UL)
				{
					return num;
				}
			}
			throw new OverflowException(Environment.GetResourceString("Overflow_UInt64"));
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float ToSingle(decimal d);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal Truncate(decimal d)
		{
			decimal.FCallTruncate(ref d);
			return d;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallTruncate(ref decimal d);

		[__DynamicallyInvokable]
		public static implicit operator decimal(byte value)
		{
			return new decimal((int)value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static implicit operator decimal(sbyte value)
		{
			return new decimal((int)value);
		}

		[__DynamicallyInvokable]
		public static implicit operator decimal(short value)
		{
			return new decimal((int)value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static implicit operator decimal(ushort value)
		{
			return new decimal((int)value);
		}

		[__DynamicallyInvokable]
		public static implicit operator decimal(char value)
		{
			return new decimal((int)value);
		}

		[__DynamicallyInvokable]
		public static implicit operator decimal(int value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static implicit operator decimal(uint value)
		{
			return new decimal(value);
		}

		[__DynamicallyInvokable]
		public static implicit operator decimal(long value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static implicit operator decimal(ulong value)
		{
			return new decimal(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator decimal(float value)
		{
			return new decimal(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator decimal(double value)
		{
			return new decimal(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator byte(decimal value)
		{
			return decimal.ToByte(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static explicit operator sbyte(decimal value)
		{
			return decimal.ToSByte(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator char(decimal value)
		{
			ushort result;
			try
			{
				result = decimal.ToUInt16(value);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(Environment.GetResourceString("Overflow_Char"), innerException);
			}
			return (char)result;
		}

		[__DynamicallyInvokable]
		public static explicit operator short(decimal value)
		{
			return decimal.ToInt16(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static explicit operator ushort(decimal value)
		{
			return decimal.ToUInt16(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator int(decimal value)
		{
			return decimal.ToInt32(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static explicit operator uint(decimal value)
		{
			return decimal.ToUInt32(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator long(decimal value)
		{
			return decimal.ToInt64(value);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static explicit operator ulong(decimal value)
		{
			return decimal.ToUInt64(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator float(decimal value)
		{
			return decimal.ToSingle(value);
		}

		[__DynamicallyInvokable]
		public static explicit operator double(decimal value)
		{
			return decimal.ToDouble(value);
		}

		[__DynamicallyInvokable]
		public static decimal operator +(decimal d)
		{
			return d;
		}

		[__DynamicallyInvokable]
		public static decimal operator -(decimal d)
		{
			return decimal.Negate(d);
		}

		[__DynamicallyInvokable]
		public static decimal operator ++(decimal d)
		{
			return decimal.Add(d, 1m);
		}

		[__DynamicallyInvokable]
		public static decimal operator --(decimal d)
		{
			return decimal.Subtract(d, 1m);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal operator +(decimal d1, decimal d2)
		{
			decimal.FCallAddSub(ref d1, ref d2, 0);
			return d1;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal operator -(decimal d1, decimal d2)
		{
			decimal.FCallAddSub(ref d1, ref d2, 128);
			return d1;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal operator *(decimal d1, decimal d2)
		{
			decimal.FCallMultiply(ref d1, ref d2);
			return d1;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static decimal operator /(decimal d1, decimal d2)
		{
			decimal.FCallDivide(ref d1, ref d2);
			return d1;
		}

		[__DynamicallyInvokable]
		public static decimal operator %(decimal d1, decimal d2)
		{
			return decimal.Remainder(d1, d2);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator ==(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) == 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator !=(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) != 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator <(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) < 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator <=(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) <= 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator >(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) > 0;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static bool operator >=(decimal d1, decimal d2)
		{
			return decimal.FCallCompare(ref d1, ref d2) >= 0;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Decimal;
		}

		[__DynamicallyInvokable]
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		[__DynamicallyInvokable]
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"Decimal",
				"Char"
			}));
		}

		[__DynamicallyInvokable]
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		[__DynamicallyInvokable]
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		[__DynamicallyInvokable]
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		[__DynamicallyInvokable]
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		[__DynamicallyInvokable]
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		[__DynamicallyInvokable]
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		[__DynamicallyInvokable]
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		[__DynamicallyInvokable]
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		[__DynamicallyInvokable]
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}

		[__DynamicallyInvokable]
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		[__DynamicallyInvokable]
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		[__DynamicallyInvokable]
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"Decimal",
				"DateTime"
			}));
		}

		[__DynamicallyInvokable]
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		private const int SignMask = -2147483648;

		private const byte DECIMAL_NEG = 128;

		private const byte DECIMAL_ADD = 0;

		private const int ScaleMask = 16711680;

		private const int ScaleShift = 16;

		private const int MaxInt32Scale = 9;

		private static uint[] Powers10 = new uint[]
		{
			1U,
			10U,
			100U,
			1000U,
			10000U,
			100000U,
			1000000U,
			10000000U,
			100000000U,
			1000000000U
		};

		[__DynamicallyInvokable]
		public const decimal Zero = 0m;

		[__DynamicallyInvokable]
		public const decimal One = 1m;

		[__DynamicallyInvokable]
		public const decimal MinusOne = -1m;

		[__DynamicallyInvokable]
		public const decimal MaxValue = 79228162514264337593543950335m;

		[__DynamicallyInvokable]
		public const decimal MinValue = -79228162514264337593543950335m;

		private const decimal NearNegativeZero = -0.000000000000000000000000001m;

		private const decimal NearPositiveZero = 0.000000000000000000000000001m;

		private int flags;

		private int hi;

		private int lo;

		private int mid;
	}
}
