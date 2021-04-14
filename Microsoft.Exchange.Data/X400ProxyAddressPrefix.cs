using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class X400ProxyAddressPrefix : ProxyAddressPrefix
	{
		public X400ProxyAddressPrefix() : base("X400")
		{
		}

		public override ProxyAddress GetProxyAddress(string address, bool isPrimaryAddress)
		{
			return new X400ProxyAddress(address, isPrimaryAddress);
		}

		public override ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress)
		{
			return new X400ProxyAddressTemplate(addressTemplate, isPrimaryAddress);
		}
	}
}
