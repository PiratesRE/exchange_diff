using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class InvalidProxyAddressTemplate : ProxyAddressTemplate, IInvalidProxy
	{
		public InvalidProxyAddressTemplate(string proxyAddressTemplateString, ProxyAddressPrefix prefix, string addressTemplate, bool isPrimaryAddress, ArgumentOutOfRangeException parseException) : base(prefix, addressTemplate, isPrimaryAddress)
		{
			if (parseException == null)
			{
				throw new ArgumentNullException("parseException", "An invalid proxy address template must contain the exception that makes it invalid.");
			}
			base.RawProxyAddressBaseString = proxyAddressTemplateString;
			this.parseException = parseException;
		}

		public ArgumentOutOfRangeException ParseException
		{
			get
			{
				return this.parseException;
			}
		}

		public override string ToString()
		{
			return base.RawProxyAddressBaseString;
		}

		public override ProxyAddressBase ToPrimary()
		{
			if (base.IsPrimaryAddress)
			{
				return this;
			}
			return new InvalidProxyAddressTemplate(null, base.Prefix, base.AddressTemplateString, true, this.ParseException);
		}

		public override ProxyAddressBase ToSecondary()
		{
			if (base.IsPrimaryAddress)
			{
				return new InvalidProxyAddressTemplate(null, base.Prefix, base.AddressTemplateString, false, this.ParseException);
			}
			return this;
		}

		private ArgumentOutOfRangeException parseException;
	}
}
