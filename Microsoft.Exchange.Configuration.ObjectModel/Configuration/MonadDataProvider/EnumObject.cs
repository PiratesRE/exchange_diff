using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[ImmutableObject(true)]
	[Serializable]
	internal class EnumObject : IComparable, IComparable<EnumObject>, IFormattable, IConvertible
	{
		public EnumObject(Enum enumValue)
		{
			this.value = enumValue;
		}

		public Enum Value
		{
			get
			{
				return this.value;
			}
		}

		private string LocalizedDescription
		{
			get
			{
				if (this.localizedDescription == null)
				{
					this.localizedDescription = ((this.Value == null) ? string.Empty : LocalizedDescriptionAttribute.FromEnum(this.Value.GetType(), this.Value));
				}
				return this.localizedDescription;
			}
		}

		public static explicit operator EnumObject(Enum enumValue)
		{
			return new EnumObject(enumValue);
		}

		public static implicit operator Enum(EnumObject enumObject)
		{
			if (enumObject != null)
			{
				return enumObject.Value;
			}
			return null;
		}

		public override bool Equals(object right)
		{
			if (right == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, right))
			{
				return true;
			}
			if (base.GetType() != right.GetType())
			{
				return false;
			}
			EnumObject enumObject = (EnumObject)right;
			return object.Equals(this.Value, enumObject.Value);
		}

		public override int GetHashCode()
		{
			if (this.Value == null)
			{
				return 0;
			}
			return this.Value.GetType().GetHashCode() ^ this.Value.GetHashCode();
		}

		public override string ToString()
		{
			if (this.Value != null)
			{
				return this.Value.ToString();
			}
			return string.Empty;
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.LocalizedDescription;
		}

		public int CompareTo(EnumObject right)
		{
			string strA = this.LocalizedDescription;
			string strB = (right == null) ? string.Empty : right.LocalizedDescription;
			return string.Compare(strA, strB);
		}

		int IComparable.CompareTo(object right)
		{
			EnumObject enumObject = right as EnumObject;
			if (enumObject == null)
			{
				throw new ArgumentException("Argument must be of type EnumObject", "right");
			}
			return this.CompareTo(enumObject);
		}

		public TypeCode GetTypeCode()
		{
			return this.Value.GetTypeCode();
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this.Value, CultureInfo.CurrentCulture);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this.Value, CultureInfo.CurrentCulture);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this.Value, CultureInfo.CurrentCulture);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this.Value, CultureInfo.CurrentCulture);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this.Value, CultureInfo.CurrentCulture);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this.Value, CultureInfo.CurrentCulture);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this.Value, CultureInfo.CurrentCulture);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this.Value, CultureInfo.CurrentCulture);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this.Value, CultureInfo.CurrentCulture);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this.Value, CultureInfo.CurrentCulture);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this.Value, CultureInfo.CurrentCulture);
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return this.ToString(null, provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (base.GetType() == conversionType)
			{
				return this;
			}
			return ((IConvertible)this.Value).ToType(conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this.Value, CultureInfo.CurrentCulture);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this.Value, CultureInfo.CurrentCulture);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this.Value, CultureInfo.CurrentCulture);
		}

		private Enum value;

		private string localizedDescription;
	}
}
