using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct RpcStatistics
	{
		public static RpcStatistics CreateFromNative(RpcStats nativeStats)
		{
			return new RpcStatistics
			{
				rpcCount = nativeStats.rpcCount,
				emptyRpcCount = nativeStats.emptyRpcCount,
				releaseOnlyRpcCount = nativeStats.releaseOnlyRpcCount,
				messagesCreated = nativeStats.messagesCreated
			};
		}

		public uint rpcCount;

		public uint emptyRpcCount;

		public uint releaseOnlyRpcCount;

		public uint messagesCreated;
	}
}
