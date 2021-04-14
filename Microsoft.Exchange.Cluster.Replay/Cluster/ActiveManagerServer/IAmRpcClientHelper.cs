using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal interface IAmRpcClientHelper
	{
		int RpcchGetAutomountConsensusState(string serverName);

		bool IsReplayRunning(AmServerName serverName);

		bool IsReplayRunning(string serverFqdn);

		AmRole GetActiveManagerRole(string serverToRpc, out string errorMessage);

		void MountDatabaseDirectEx(string serverToRpc, Guid dbGuid, AmMountArg mountArg);

		void DismountDatabaseDirect(string serverToRpc, Guid dbGuid, AmDismountArg dismountArg);

		void AttemptCopyLastLogsDirect(string serverToRpc, Guid dbGuid, DatabaseMountDialOverride mountDialOverride, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, string sourceServer, int actionCode, int skipValidationChecks, bool mountPending, string uniqueOperationId, int subactionAttemptNumber, ref AmAcllReturnStatus acllStatus);
	}
}
