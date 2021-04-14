using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetSeeding : IGetStatus
	{
		void CheckReseedBlocked();

		bool TryBeginDbSeed(RpcSeederArgs rpcSeederArgs);

		void EndDbSeed();

		void FailedDbSeed(ExEventLog.EventTuple errorEventTuple, LocalizedString errorMessage, ExtendedErrorInfo errorInfo);
	}
}
