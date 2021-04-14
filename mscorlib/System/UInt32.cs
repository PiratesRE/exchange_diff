﻿using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct UInt32 : IComparable, IFormattable, IConvertible, IComparable<uint>, IEquatable<uint>
	{
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is uint))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeUInt32"));
			}
			uint num = (uint)value;
			if (this < num)
			{
				return -1;
			}
			if (this > num)
			{
				return 1;
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public int CompareTo(uint value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is uint && this == (uint)obj;
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public bool Equals(uint obj)
		{
			return this == obj;
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return (int)this;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override string ToString()
		{
			return Number.FormatUInt32(this, null, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(IFormatProvider provider)
		{
			return Number.FormatUInt32(this, null, NumberFormatInfo.GetInstance(provider));
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(string format)
		{
			return Number.FormatUInt32(this, format, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(string format, IFormatProvider provider)
		{
			return Number.FormatUInt32(this, format, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static uint Parse(string s)
		{
			return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static uint Parse(string s, NumberStyles style)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.ParseUInt32(s, style, NumberFormatInfo.CurrentInfo);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static uint Parse(string s, IFormatProvider provider)
		{
			return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static uint Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.ParseUInt32(s, style, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static bool TryParse(string s, out uint result)
		{
			return Number.TryParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out uint result)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.TryParseUInt32(s, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.UInt32;
		}

		[__DynamicallyInvokable]
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		[__DynamicallyInvokable]
		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
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
			return this;
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
			return Convert.ToDecimal(this);
		}

		[__DynamicallyInvokable]
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"UInt32",
				"DateTime"
			}));
		}

		[__DynamicallyInvokable]
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		private uint m_value;

		[__DynamicallyInvokable]
		public const uint MaxValue = 4294967295U;

		[__DynamicallyInvokable]
		public const uint MinValue = 0U;
	}
}
