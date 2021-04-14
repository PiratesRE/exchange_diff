using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ReplayRpcClientHelper
	{
		public static void RequestSuspend3(string servername, Guid guid, string suspendcomment, uint flags, uint initiator)
		{
			ReplayRpcClientHelper.defaultInstance.RequestSuspend3(servername, guid, suspendcomment, flags, initiator);
		}

		internal static void RequestResume2(string servername, Guid guid, uint flags)
		{
			ReplayRpcClientHelper.defaultInstance.RequestResume2(servername, guid, flags);
		}

		internal static void RpccDisableReplayLag(string serverName, Guid dbGuid, string disableReason, ActionInitiatorType actionInitiator)
		{
			ReplayRpcClientHelper.defaultInstance.RpccDisableReplayLag(serverName, dbGuid, disableReason, actionInitiator);
		}

		internal static void RpccEnableReplayLag(string serverName, Guid dbGuid, ActionInitiatorType actionInitiator)
		{
			ReplayRpcClientHelper.defaultInstance.RpccEnableReplayLag(serverName, dbGuid, actionInitiator);
		}

		internal static RpcDatabaseCopyStatus2[] GetCopyStatus(string serverName, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids)
		{
			return ReplayRpcClientHelper.defaultInstance.GetCopyStatus(serverName, collectionFlags2, dbGuids);
		}

		internal static void NotifyChangedReplayConfiguration(string serverName, Guid dbGuid, ServerVersion serverVersion, bool waitForCompletion, bool isHighPriority, ReplayConfigChangeHints changeHint)
		{
			ReplayRpcClientHelper.defaultInstance.NotifyChangedReplayConfiguration(serverName, dbGuid, serverVersion, waitForCompletion, isHighPriority, changeHint);
		}

		internal static void NotifyChangedReplayConfigurationAsync(string serverName, Guid dbGuid, ReplayConfigChangeHints changeHint)
		{
			ReplayRpcClientHelper.defaultInstance.NotifyChangedReplayConfigurationAsync(serverName, dbGuid, changeHint);
		}

		internal static Dictionary<Guid, RpcDatabaseCopyStatus2> ParseStatusResults(RpcDatabaseCopyStatus2[] statusResults)
		{
			Dictionary<Guid, RpcDatabaseCopyStatus2> dictionary = new Dictionary<Guid, RpcDatabaseCopyStatus2>();
			if (statusResults != null && statusResults.Length > 0)
			{
				foreach (RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus in statusResults)
				{
					if (!dictionary.ContainsKey(rpcDatabaseCopyStatus.DBGuid))
					{
						dictionary.Add(rpcDatabaseCopyStatus.DBGuid, rpcDatabaseCopyStatus);
					}
				}
			}
			return dictionary;
		}

		private static ReplayRpcClientWrapper defaultInstance = new ReplayRpcClientWrapper();
	}
}
