using System;

namespace Microsoft.Exchange.Transport.Logging
{
	internal enum ProtocolEvent
	{
		Connect,
		Disconnect,
		Send,
		Receive,
		Information,
		Count
	}
}
