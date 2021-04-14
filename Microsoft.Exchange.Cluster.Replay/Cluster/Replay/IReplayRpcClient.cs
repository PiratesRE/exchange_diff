using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IReplayRpcClient
	{
		void RequestSuspend3(string serverName, Guid guid, string suspendComment, uint flags, uint initiator);

		void RequestResume2(string serverName, Guid guid, uint flags);

		void RpccDisableReplayLag(string serverName, Guid dbGuid, string disableReason, ActionInitiatorType actionInitiator);

		void RpccEnableReplayLag(string serverName, Guid dbGuid, ActionInitiatorType actionInitiator);

		RpcDatabaseCopyStatus2[] GetCopyStatus(string serverName, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids);

		RpcDatabaseCopyStatus2[] GetCopyStatus(string serverName, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, int timeoutMs);

		RpcCopyStatusContainer GetCopyStatusWithHealthState(string serverName, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, int timeoutMs);

		void NotifyChangedReplayConfiguration(string serverName, Guid dbGuid, ServerVersion serverVersion, bool waitForCompletion, bool isHighPriority, ReplayConfigChangeHints changeHint);
	}
}
