using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ProxyAddressTemplate : ProxyAddressBase, IComparable<ProxyAddressTemplate>
	{
		protected ProxyAddressTemplate(ProxyAddressPrefix prefix, string addressTemplateString, bool isPrimaryAddress) : base(prefix, addressTemplateString, isPrimaryAddress)
		{
			if (prefix.DisplayName.Length == 0)
			{
				throw new ArgumentException(DataStrings.ProxyAddressTemplateEmptyPrefixOrValue(this.ProxyAddressTemplateString));
			}
			if (addressTemplateString.Length == 0)
			{
				throw new ArgumentException(DataStrings.ProxyAddressTemplateEmptyPrefixOrValue(this.ProxyAddressTemplateString));
			}
		}

		public string AddressTemplateString
		{
			get
			{
				return base.ValueString;
			}
		}

		public string ProxyAddressTemplateString
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
			return ProxyAddressTemplate.Parse(base.Prefix.PrimaryPrefix, this.AddressTemplateString);
		}

		public override ProxyAddressBase ToSecondary()
		{
			if (base.IsPrimaryAddress)
			{
				return ProxyAddressTemplate.Parse(base.Prefix.SecondaryPrefix, this.AddressTemplateString);
			}
			return this;
		}

		public static ProxyAddressTemplate Parse(string proxyAddressTemplateString)
		{
			if (proxyAddressTemplateString == null)
			{
				throw new ArgumentNullException("proxyAddressTemplateString");
			}
			if (proxyAddressTemplateString.Length == 0)
			{
				throw new ArgumentException(DataStrings.ProxyAddressTemplateEmptyPrefixOrValue(proxyAddressTemplateString));
			}
			int num = proxyAddressTemplateString.IndexOf(':');
			string prefixString = proxyAddressTemplateString.Substring(0, Math.Max(num, 0));
			string addressTemplateString = proxyAddressTemplateString.Substring(num + 1);
			return ProxyAddressTemplate.Parse(proxyAddressTemplateString, prefixString, addressTemplateString);
		}

		public static ProxyAddressTemplate Parse(string prefixString, string addressTemplateString)
		{
			return ProxyAddressTemplate.Parse(null, prefixString, addressTemplateString);
		}

		private static ProxyAddressTemplate Parse(string proxyAddressTemplateString, string prefixString, string addressTemplateString)
		{
			if (prefixString == null)
			{
				throw new ArgumentNullException("prefixString");
			}
			if (addressTemplateString == null)
			{
				throw new ArgumentNullException("addressTemplateString");
			}
			ProxyAddressPrefix proxyAddressPrefix;
			if (prefixString.Length == 0 && SmtpProxyAddressTemplate.IsValidSmtpAddressTemplate(addressTemplateString))
			{
				proxyAddressPrefix = ProxyAddressPrefix.Smtp;
			}
			else
			{
				proxyAddressPrefix = ProxyAddressPrefix.GetPrefix(prefixString);
			}
			bool flag = StringComparer.Ordinal.Equals(proxyAddressPrefix.PrimaryPrefix, prefixString);
			ProxyAddressTemplate result;
			try
			{
				ProxyAddressTemplate proxyAddressTemplate = proxyAddressPrefix.GetProxyAddressTemplate(addressTemplateString, flag);
				proxyAddressTemplate.RawProxyAddressBaseString = proxyAddressTemplateString;
				result = proxyAddressTemplate;
			}
			catch (ArgumentOutOfRangeException parseException)
			{
				result = new InvalidProxyAddressTemplate(proxyAddressTemplateString, proxyAddressPrefix, addressTemplateString, flag, parseException);
			}
			return result;
		}

		public int CompareTo(ProxyAddressTemplate other)
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

		public static bool operator ==(ProxyAddressTemplate a, ProxyAddressTemplate b)
		{
			return a == b;
		}

		public static bool operator !=(ProxyAddressTemplate a, ProxyAddressTemplate b)
		{
			return !(a == b);
		}
	}
}
