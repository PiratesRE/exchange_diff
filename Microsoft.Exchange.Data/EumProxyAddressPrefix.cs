using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class EumProxyAddressPrefix : ProxyAddressPrefix
	{
		internal EumProxyAddressPrefix() : base("EUM")
		{
		}

		public override ProxyAddress GetProxyAddress(string address, bool isPrimaryAddress)
		{
			if (address.IndexOf("phone-context=") == -1)
			{
				return new InvalidProxyAddress(null, ProxyAddressPrefix.UM, address, isPrimaryAddress, new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidEumAddress(address), null));
			}
			return new EumProxyAddress(address, isPrimaryAddress);
		}

		public override ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress)
		{
			return new EumProxyAddressTemplate(addressTemplate, isPrimaryAddress);
		}
	}
}
