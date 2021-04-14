using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ProxyAddress : ProxyAddressBase, IComparable<ProxyAddress>
	{
		protected ProxyAddress(ProxyAddressPrefix prefix, string addressString, bool isPrimaryAddress) : this(prefix, addressString, isPrimaryAddress, false)
		{
		}

		protected ProxyAddress(ProxyAddressPrefix prefix, string addressString, bool isPrimaryAddress, bool suppressAddressValidation) : base(prefix, addressString, isPrimaryAddress, suppressAddressValidation)
		{
		}

		public string AddressString
		{
			get
			{
				return base.ValueString;
			}
		}

		public string ProxyAddressString
		{
			get
			{
				return base.ProxyAddressBaseString;
			}
		}

		public override ProxyAddressBase ToPrimary()
		{
			if (base.IsPrimaryAddress)
			{
				return this;
			}
			return ProxyAddress.Parse(base.Prefix.PrimaryPrefix, this.AddressString);
		}

		public override ProxyAddressBase ToSecondary()
		{
			if (base.IsPrimaryAddress)
			{
				return ProxyAddress.Parse(base.Prefix.SecondaryPrefix, this.AddressString);
			}
			return this;
		}

		private static void BreakPrefixAndAddress(string proxyAddressString, out string prefixString, out string addressString)
		{
			if (proxyAddressString == null)
			{
				throw new ArgumentNullException("proxyAddressString");
			}
			if (proxyAddressString.Length == 0)
			{
				throw new ArgumentException(DataStrings.ExceptionEmptyProxyAddress);
			}
			int num = proxyAddressString.IndexOf(':');
			prefixString = proxyAddressString.Substring(0, Math.Max(num, 0));
			addressString = proxyAddressString.Substring(num + 1);
		}

		internal static ProxyAddress ParseFromAD(string proxyAddressString)
		{
			string prefixString = null;
			string text = null;
			ProxyAddress.BreakPrefixAndAddress(proxyAddressString, out prefixString, out text);
			ProxyAddress proxyAddress = ProxyAddress.Parse(proxyAddressString, prefixString, text);
			if (typeof(X400ProxyAddress) == proxyAddress.GetType() && !((X400ProxyAddress)proxyAddress).EndingWithSemicolon)
			{
				proxyAddress = new InvalidProxyAddress(proxyAddressString, proxyAddress.Prefix, text, proxyAddress.IsPrimaryAddress, new ArgumentOutOfRangeException(DataStrings.InvalidX400AddressSpace(proxyAddressString)));
			}
			return proxyAddress;
		}

		public static ProxyAddress Parse(string proxyAddressString)
		{
			string prefixString = null;
			string addressString = null;
			ProxyAddress.BreakPrefixAndAddress(proxyAddressString, out prefixString, out addressString);
			return ProxyAddress.Parse(proxyAddressString, prefixString, addressString);
		}

		public static bool TryParse(string proxyAddressString, out ProxyAddress result)
		{
			result = null;
			try
			{
				result = ProxyAddress.Parse(proxyAddressString);
			}
			catch (ArgumentException)
			{
			}
			if (result == null || result is InvalidProxyAddress)
			{
				result = null;
				return false;
			}
			return true;
		}

		public static ProxyAddress Parse(string prefixString, string addressString)
		{
			return ProxyAddress.Parse(null, prefixString, addressString);
		}

		private static ProxyAddress Parse(string proxyAddressString, string prefixString, string addressString)
		{
			if (prefixString == null)
			{
				throw new ArgumentNullException("prefixString");
			}
			if (addressString == null)
			{
				throw new ArgumentNullException("addressString");
			}
			ProxyAddressPrefix proxyAddressPrefix;
			if (prefixString.Length == 0 && SmtpAddress.IsValidSmtpAddress(addressString))
			{
				proxyAddressPrefix = ProxyAddressPrefix.Smtp;
			}
			else
			{
				try
				{
					proxyAddressPrefix = ProxyAddressPrefix.GetPrefix(prefixString);
				}
				catch (ArgumentOutOfRangeException parseException)
				{
					return new InvalidProxyAddress(proxyAddressString, new CustomProxyAddressPrefix("ERROR"), proxyAddressString ?? (prefixString + ':' + addressString), true, parseException);
				}
				catch (ArgumentException ex)
				{
					return new InvalidProxyAddress(proxyAddressString, new CustomProxyAddressPrefix("ERROR"), proxyAddressString ?? (prefixString + ':' + addressString), true, new ArgumentOutOfRangeException(ex.Message, ex));
				}
			}
			bool isPrimaryAddress = StringComparer.Ordinal.Equals(proxyAddressPrefix.PrimaryPrefix, prefixString);
			ProxyAddress result;
			try
			{
				ProxyAddress proxyAddress = proxyAddressPrefix.GetProxyAddress(addressString, isPrimaryAddress);
				proxyAddress.RawProxyAddressBaseString = proxyAddressString;
				result = proxyAddress;
			}
			catch (ArgumentOutOfRangeException parseException2)
			{
				result = new InvalidProxyAddress(proxyAddressString, proxyAddressPrefix, addressString, isPrimaryAddress, parseException2);
			}
			return result;
		}

		public int CompareTo(ProxyAddress other)
		{
			return base.CompareTo(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public static bool operator ==(ProxyAddress a, ProxyAddress b)
		{
			return a == b;
		}

		public static bool operator !=(ProxyAddress a, ProxyAddress b)
		{
			return !(a == b);
		}
	}
}
