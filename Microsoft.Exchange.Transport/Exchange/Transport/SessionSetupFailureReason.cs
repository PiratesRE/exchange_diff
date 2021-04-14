using System;

namespace Microsoft.Exchange.Transport
{
	internal enum SessionSetupFailureReason
	{
		None,
		UserLookupFailure,
		DnsLookupFailure,
		ConnectionFailure,
		ProtocolError,
		SocketError,
		Shutdown,
		BackEndLocatorFailure
	}
}
