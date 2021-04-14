using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct Boolean : IComparable, IConvertible, IComparable<bool>, IEquatable<bool>
	{
		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			if (!this)
			{
				return 0;
			}
			return 1;
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			if (!this)
			{
				return "False";
			}
			return "True";
		}

		public string ToString(IFormatProvider provider)
		{
			if (!this)
			{
				return "False";
			}
			return "True";
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is bool && this == (bool)obj;
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public bool Equals(bool obj)
		{
			return this == obj;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is bool))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeBoolean"));
			}
			if (this == (bool)obj)
			{
				return 0;
			}
			if (!this)
			{
				return -1;
			}
			return 1;
		}

		[__DynamicallyInvokable]
		public int CompareTo(bool value)
		{
			if (this == value)
			{
				return 0;
			}
			if (!this)
			{
				return -1;
			}
			return 1;
		}

		[__DynamicallyInvokable]
		public static bool Parse(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			bool result = false;
			if (!bool.TryParse(value, out result))
			{
				throw new FormatException(Environment.GetResourceString("Format_BadBoolean"));
			}
			return result;
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string value, out bool result)
		{
			result = false;
			if (value == null)
			{
				return false;
			}
			if ("True".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				result = true;
				return true;
			}
			if ("False".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				result = false;
				return true;
			}
			value = bool.TrimWhiteSpaceAndNull(value);
			if ("True".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				result = true;
				return true;
			}
			if ("False".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				result = false;
				return true;
			}
			return false;
		}

		private static string TrimWhiteSpaceAndNull(string value)
		{
			int i = 0;
			int num = value.Length - 1;
			char c = '\0';
			while (i < value.Length)
			{
				if (!char.IsWhiteSpace(value[i]) && value[i] != c)
				{
					IL_52:
					while (num >= i && (char.IsWhiteSpace(value[num]) || value[num] == c))
					{
						num--;
					}
					return value.Substring(i, num - i + 1);
				}
				i++;
			}
			goto IL_52;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Boolean;
		}

		[__DynamicallyInvokable]
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return this;
		}

		[__DynamicallyInvokable]
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"Boolean",
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
			return Convert.ToDecimal(this);
		}

		[__DynamicallyInvokable]
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"Boolean",
				"DateTime"
			}));
		}

		[__DynamicallyInvokable]
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		private bool m_value;

		internal const int True = 1;

		internal const int False = 0;

		internal const string TrueLiteral = "True";

		internal const string FalseLiteral = "False";

		[__DynamicallyInvokable]
		public static readonly string TrueString = "True";

		[__DynamicallyInvokable]
		public static readonly string FalseString = "False";
	}
}
