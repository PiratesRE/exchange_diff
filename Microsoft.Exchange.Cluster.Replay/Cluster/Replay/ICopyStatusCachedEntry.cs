using System;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface ICopyStatusCachedEntry
	{
		RpcDatabaseCopyStatus2 CopyStatus { get; }
	}
}
