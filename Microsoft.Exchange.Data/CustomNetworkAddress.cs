using System;

namespace Microsoft.Exchange.Data
{
	public sealed class CustomNetworkAddress : NetworkAddress
	{
		public CustomNetworkAddress(NetworkProtocol protocol, string address) : base(protocol, address)
		{
		}
	}
}
