using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal enum DisconnectReason
	{
		ClientDisconnect,
		ServerDropped,
		NetworkRundown
	}
}
