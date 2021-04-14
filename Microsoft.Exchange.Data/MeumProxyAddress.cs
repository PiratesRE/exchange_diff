using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class MeumProxyAddress : ProxyAddress
	{
		public MeumProxyAddress(string address, bool primaryAddress) : base(ProxyAddressPrefix.Meum, address, primaryAddress)
		{
		}
	}
}
