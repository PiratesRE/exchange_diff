using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	public enum InboundClientProxyStates : byte
	{
		None,
		XProxyReceived,
		XProxyReceivedAndAuthenticated
	}
}
