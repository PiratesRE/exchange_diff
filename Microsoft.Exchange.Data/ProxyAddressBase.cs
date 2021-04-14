using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	public abstract class ProxyAddressBase : IComparable, IComparable<ProxyAddressBase>
	{
		protected ProxyAddressBase(ProxyAddressPrefix prefix, string valueString, bool isPrimaryAddress) : this(prefix, valueString, isPrimaryAddress, false)
		{
		}

		protected ProxyAddressBase(ProxyAddressPrefix prefix, string valueString, bool isPrimaryAddress, bool suppressAddressValidation)
		{
			if (null == prefix)
			{
				throw new ArgumentNullException("prefix");
			}
			if (valueString == null)
			{
				throw new ArgumentNullException("valueString");
			}
			if (!suppressAddressValidation)
			{
				ProxyAddressBase.ValidateAddressString(valueString);
			}
			this.prefix = prefix;
			this.valueString = valueString;
			this.isPrimaryAddress = isPrimaryAddress;
		}

		public ProxyAddressPrefix Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public bool IsPrimaryAddress
		{
			get
			{
				return this.isPrimaryAddress;
			}
		}

		public string PrefixString
		{
			get
			{
				if (this.IsPrimaryAddress)
				{
					return this.Prefix.PrimaryPrefix;
				}
				return this.Prefix.SecondaryPrefix;
			}
		}

		internal string ValueString
		{
			get
			{
				return this.valueString;
			}
		}

		internal string ProxyAddressBaseString
		{
			get
			{
				return this.PrefixString + ':' + this.ValueString;
			}
		}

		internal string RawProxyAddressBaseString
		{
			get
			{
				string result;
				if ((result = this.rawProxyAddressBaseString) == null)
				{
					result = (this.RawProxyAddressBaseString = this.ProxyAddressBaseString);
				}
				return result;
			}
			set
			{
				this.rawProxyAddressBaseString = value;
			}
		}

		public override string ToString()
		{
			return this.ProxyAddressBaseString;
		}

		public abstract ProxyAddressBase ToPrimary();

		public abstract ProxyAddressBase ToSecondary();

		public ProxyAddressBase GetSimilarProxy(string address)
		{
			if (this is ProxyAddress)
			{
				return this.Prefix.GetProxyAddress(address, this.IsPrimaryAddress);
			}
			if (this is ProxyAddressTemplate)
			{
				return this.Prefix.GetProxyAddressTemplate(address, this.IsPrimaryAddress);
			}
			return null;
		}

		public static void ValidateAddressString(string valueString)
		{
			if (!ProxyAddressBase.IsAddressStringValid(valueString))
			{
				throw new ArgumentOutOfRangeException(DataStrings.ProxyAddressShouldNotBeAllSpace, null);
			}
		}

		public static bool IsAddressStringValid(string valueString)
		{
			return !string.IsNullOrEmpty(valueString) && !string.IsNullOrEmpty(valueString.Trim());
		}

		public override int GetHashCode()
		{
			return (StringComparer.OrdinalIgnoreCase.GetHashCode(this.PrefixString) << 5) + StringComparer.OrdinalIgnoreCase.GetHashCode(this.ValueString);
		}

		public override bool Equals(object obj)
		{
			return this == obj as ProxyAddressBase;
		}

		public static bool Equals(ProxyAddressBase a, ProxyAddressBase b, StringComparison comparisonType)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			bool flag = string.Equals(a.PrefixString, b.PrefixString, comparisonType) && string.Equals(a.ValueString, b.ValueString, comparisonType);
			if (!flag && (a is InvalidProxyAddress || a is InvalidProxyAddressTemplate || b is InvalidProxyAddress || b is InvalidProxyAddressTemplate))
			{
				flag = string.Equals(a.RawProxyAddressBaseString, b.RawProxyAddressBaseString, comparisonType);
			}
			return flag;
		}

		public static bool operator ==(ProxyAddressBase a, ProxyAddressBase b)
		{
			return ProxyAddressBase.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(ProxyAddressBase a, ProxyAddressBase b)
		{
			return !(a == b);
		}

		public int CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is ProxyAddressBase))
			{
				throw new ArgumentException(DataStrings.InvalidTypeArgumentException("other", other.GetType(), typeof(ProxyAddressBase)), "other");
			}
			return this.CompareTo((ProxyAddressBase)other);
		}

		public int CompareTo(ProxyAddressBase other)
		{
			if (null == other)
			{
				return 1;
			}
			int num = StringComparer.OrdinalIgnoreCase.Compare(this.PrefixString, other.PrefixString);
			if (num != 0)
			{
				return num;
			}
			return StringComparer.OrdinalIgnoreCase.Compare(this.ValueString, other.ValueString);
		}

		public bool Equals(ProxyAddressBase other)
		{
			return this == other;
		}

		internal const char PrefixValueSeparatorCharacter = ':';

		public static readonly int MaxLength = 1123;

		private readonly ProxyAddressPrefix prefix;

		private readonly bool isPrimaryAddress;

		private readonly string valueString;

		private string rawProxyAddressBaseString;
	}
}
