using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmRpcClientWrapper : IAmRpcClientHelper
	{
		public int RpcchGetAutomountConsensusState(string serverName)
		{
			return AmRpcClientHelper.RpcchGetAutomountConsensusState(serverName);
		}

		public bool IsReplayRunning(AmServerName serverName)
		{
			bool result = false;
			try
			{
				result = AmRpcClientHelper.IsRunning(serverName.Fqdn);
			}
			catch (AmServerException ex)
			{
				AmTrace.Error("IsReplayRunning() failed with {0}", new object[]
				{
					ex
				});
			}
			catch (AmServerTransientException ex2)
			{
				AmTrace.Error("IsReplayRunning() failed with {0}", new object[]
				{
					ex2
				});
			}
			return result;
		}

		public bool IsReplayRunning(string serverFqdn)
		{
			return this.IsReplayRunning(new AmServerName(serverFqdn));
		}

		public AmRole GetActiveManagerRole(string serverToRpc, out string errorMessage)
		{
			return AmRpcClientHelper.GetActiveManagerRole(serverToRpc, out errorMessage);
		}

		public void MountDatabaseDirectEx(string serverToRpc, Guid dbGuid, AmMountArg mountArg)
		{
			AmRpcClientHelper.MountDatabaseDirectEx(serverToRpc, dbGuid, mountArg);
		}

		public void DismountDatabase(IADDatabase database, int flags)
		{
			AmRpcClientHelper.DismountDatabase(database, flags);
		}

		public void DismountDatabaseDirect(string serverToRpc, Guid dbGuid, AmDismountArg dismountArg)
		{
			AmRpcClientHelper.DismountDatabaseDirect(serverToRpc, dbGuid, dismountArg);
		}

		public void AttemptCopyLastLogsDirect(string serverToRpc, Guid dbGuid, DatabaseMountDialOverride mountDialOverride, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, string sourceServer, int actionCode, int skipValidationChecks, bool mountPending, string uniqueOperationId, int subactionAttemptNumber, ref AmAcllReturnStatus acllStatus)
		{
			AmRpcClientHelper.AttemptCopyLastLogsDirect(serverToRpc, dbGuid, mountDialOverride, numRetries, e00timeoutMs, networkIOtimeoutMs, networkConnecttimeoutMs, sourceServer, actionCode, skipValidationChecks, mountPending, uniqueOperationId, subactionAttemptNumber, ref acllStatus);
		}
	}
}
