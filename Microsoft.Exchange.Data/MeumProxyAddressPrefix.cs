using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class MeumProxyAddressPrefix : ProxyAddressPrefix
	{
		internal MeumProxyAddressPrefix() : base("MEUM")
		{
		}

		public override ProxyAddress GetProxyAddress(string address, bool primaryAddress)
		{
			return MeumProxyAddressFactory.CreateFromAddressString(address, primaryAddress);
		}

		public override ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool primaryAddress)
		{
			throw new NotSupportedException("Mserve UM proxy address templates are not supported");
		}
	}
}
