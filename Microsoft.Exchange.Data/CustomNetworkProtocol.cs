using System;

namespace Microsoft.Exchange.Data
{
	public sealed class CustomNetworkProtocol : NetworkProtocol
	{
		public CustomNetworkProtocol(string protocolName, string displayName) : base(protocolName, displayName)
		{
		}

		public CustomNetworkProtocol(string protocolName) : base(protocolName, protocolName)
		{
		}

		public override NetworkAddress GetNetworkAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentNullException("address");
			}
			return new CustomNetworkAddress(this, address);
		}
	}
}
