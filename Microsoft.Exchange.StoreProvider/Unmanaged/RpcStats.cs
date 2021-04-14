using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct RpcStats
	{
		internal uint rpcCount;

		internal uint emptyRpcCount;

		internal uint releaseOnlyRpcCount;

		internal uint messagesCreated;
	}
}
