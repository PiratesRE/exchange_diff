using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class SmtpProxyAddressPrefix : ProxyAddressPrefix
	{
		internal SmtpProxyAddressPrefix() : base("SMTP")
		{
		}

		public override ProxyAddress GetProxyAddress(string address, bool isPrimaryAddress)
		{
			if (address.IndexOf('@') == -1)
			{
				return new InvalidProxyAddress(null, ProxyAddressPrefix.Smtp, address, isPrimaryAddress, new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidSmtpAddress(address), null));
			}
			return new SmtpProxyAddress(address, isPrimaryAddress);
		}

		public override ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress)
		{
			return new SmtpProxyAddressTemplate(addressTemplate, isPrimaryAddress);
		}
	}
}
