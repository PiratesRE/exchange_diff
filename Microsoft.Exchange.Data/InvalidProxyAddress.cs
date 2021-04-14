using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class InvalidProxyAddress : ProxyAddress, IInvalidProxy
	{
		public InvalidProxyAddress(string proxyAddressString, ProxyAddressPrefix prefix, string address, bool isPrimaryAddress, ArgumentOutOfRangeException parseException) : base(prefix, address, isPrimaryAddress, true)
		{
			if (parseException == null)
			{
				throw new ArgumentNullException("parseException", "An invalid proxy address must contain the exception that makes it invalid.");
			}
			base.RawProxyAddressBaseString = proxyAddressString;
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
			return new InvalidProxyAddress(null, base.Prefix, base.AddressString, true, this.ParseException);
		}

		public override ProxyAddressBase ToSecondary()
		{
			if (base.IsPrimaryAddress)
			{
				return new InvalidProxyAddress(null, base.Prefix, base.AddressString, false, this.ParseException);
			}
			return this;
		}

		private ArgumentOutOfRangeException parseException;
	}
}
