using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct ExRpcPerfData
	{
		internal int ulTotalLength;

		internal int ulPid;

		internal long i64ProcessStartTime;

		internal Guid gPn10;

		internal Guid gPn11;

		internal Guid gPn12;

		internal Guid gPn13;

		internal Guid gPn14;

		internal Guid gPn15;

		internal Guid gPn16;

		internal Guid gPn17;

		internal Guid gPn20;

		internal Guid gPn21;

		internal Guid gPn22;

		internal Guid gPn23;

		internal Guid gPn24;

		internal Guid gPn25;

		internal Guid gPn26;

		internal Guid gPn27;

		internal int ulHdrEnd;

		internal int ulNumConnectionCaches;

		internal int ulCacheTotalCapacity;

		internal int ulCacheConnectionsActive;

		internal int ulCacheConnectionsIdle;

		internal int ulOutOfLimitCreations;

		internal int ulExRpcConnectionCreations;

		internal int ulExRpcConnectionDisposals;

		internal int ulExRpcConnectionOutstanding;

		internal int ulUnkObjectsTotal;

		internal int ulLogonObjects;

		internal int ulFolderObjects;

		internal int ulMessageObjects;

		internal int ulRpcReqsSent;

		internal int ulRpcReqsSucceeded;

		internal int ulRpcReqsFailed;

		internal int ulRpcReqsFailedWithException;

		internal int ulRpcReqsFailedTotal;

		internal int ulRpcReqsOutstanding;

		internal long i64RpcLatencyTotal;

		internal long i64RpcLatencyAverage;

		internal int ulRpcSlowReqsTotal;

		internal long i64RpcSlowReqsLatencyTotal;

		internal long i64RpcSlowReqsLatencyAverage;

		internal int ulRpcReqsBytesSent;

		internal int ulRpcReqsBytesSentAverage;

		internal int ulRpcReqsBytesReceived;

		internal int ulRpcReqsBytesReceivedAverage;

		internal int ulRopReqsSent;

		internal int ulRopReqsComplete;

		internal int ulRopReqsOutstanding;

		internal int ulRpcPoolPools;

		internal int ulRpcPoolContextHandles;

		internal int ulRpcPoolSessions;

		internal int ulRpcPoolThreadsActive;

		internal int ulRpcPoolThreadsTotal;

		internal long ulRpcPoolLatencyAverage;

		internal int ulRpcPoolSessionNotifReceived;

		internal int ulRpcPoolAsyncNotifReceived;

		internal int ulRpcPoolAsyncNotifParked;

		internal int ulAverageBase;

		internal long ulLargeAverageBase;
	}
}
