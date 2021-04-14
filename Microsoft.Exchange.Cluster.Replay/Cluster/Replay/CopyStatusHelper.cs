using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class CopyStatusHelper
	{
		internal static CopyStatusClientCachedEntry[] GetCopyStatus(AmServerName amServer, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, int timeoutMs, ActiveManager activeManager, out Exception exception)
		{
			RpcHealthStateInfo[] array = null;
			return CopyStatusHelper.GetCopyStatus(amServer, collectionFlags2, dbGuids, timeoutMs, activeManager, false, out array, out exception);
		}

		internal static CopyStatusClientCachedEntry[] GetCopyStatus(AmServerName amServer, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, int timeoutMs, ActiveManager activeManager, bool isGetHealthStates, out RpcHealthStateInfo[] healthStates, out Exception exception)
		{
			CopyStatusClientCachedEntry[] result = null;
			RpcDatabaseCopyStatus2[] statuses = null;
			TimeSpan rpcDuration = TimeSpan.Zero;
			Exception tempEx = null;
			exception = null;
			healthStates = null;
			RpcHealthStateInfo[] tmpHealthStates = null;
			tempEx = CopyStatusHelper.TimeCopyStatusRpc(delegate
			{
				if (!isGetHealthStates)
				{
					statuses = Dependencies.ReplayRpcClientWrapper.GetCopyStatus(amServer.Fqdn, collectionFlags2, dbGuids, timeoutMs);
					return;
				}
				RpcCopyStatusContainer copyStatusWithHealthState = Dependencies.ReplayRpcClientWrapper.GetCopyStatusWithHealthState(amServer.Fqdn, collectionFlags2, dbGuids, timeoutMs);
				statuses = copyStatusWithHealthState.CopyStatuses;
				tmpHealthStates = copyStatusWithHealthState.HealthStates;
			}, out rpcDuration);
			healthStates = tmpHealthStates;
			exception = tempEx;
			if (exception != null)
			{
				result = (from guid in dbGuids
				select CopyStatusHelper.ConstructCopyStatusCachedEntry(guid, amServer, null, tempEx, rpcDuration, activeManager)).ToArray<CopyStatusClientCachedEntry>();
			}
			else if (statuses != null)
			{
				Dictionary<Guid, RpcDatabaseCopyStatus2> tempStatusTable = statuses.ToDictionary((RpcDatabaseCopyStatus2 status) => status.DBGuid);
				tempEx = new ReplayServiceRpcUnknownInstanceException();
				result = (from guid in dbGuids
				let statusFound = tempStatusTable.ContainsKey(guid)
				select CopyStatusHelper.ConstructCopyStatusCachedEntry(guid, amServer, statusFound ? tempStatusTable[guid] : null, statusFound ? null : tempEx, rpcDuration, activeManager)).ToArray<CopyStatusClientCachedEntry>();
			}
			else
			{
				DiagCore.RetailAssert(false, "If no exception was thrown by GetCopyStatus RPC, then we should have some status results!", new object[0]);
			}
			return result;
		}

		internal static CopyStatusClientCachedEntry[] GetAllCopyStatuses(AmServerName amServer, RpcGetDatabaseCopyStatusFlags2 collectionFlags2, IEnumerable<IADDatabase> expectedDatabases, int timeoutMs, ActiveManager activeManager, bool isGetHealthStates, out RpcHealthStateInfo[] healthStates, out Exception exception)
		{
			CopyStatusClientCachedEntry[] result = null;
			RpcDatabaseCopyStatus2[] statuses = null;
			TimeSpan rpcDuration = TimeSpan.Zero;
			new ReplayStopwatch();
			Exception tempEx = null;
			exception = null;
			healthStates = null;
			RpcHealthStateInfo[] tmpHealthStates = null;
			tempEx = CopyStatusHelper.TimeCopyStatusRpc(delegate
			{
				if (!isGetHealthStates)
				{
					statuses = Dependencies.ReplayRpcClientWrapper.GetCopyStatus(amServer.Fqdn, collectionFlags2, null, timeoutMs);
					return;
				}
				RpcCopyStatusContainer copyStatusWithHealthState = Dependencies.ReplayRpcClientWrapper.GetCopyStatusWithHealthState(amServer.Fqdn, collectionFlags2, null, timeoutMs);
				statuses = copyStatusWithHealthState.CopyStatuses;
				tmpHealthStates = copyStatusWithHealthState.HealthStates;
			}, out rpcDuration);
			healthStates = tmpHealthStates;
			exception = tempEx;
			if (exception != null)
			{
				result = (from db in expectedDatabases
				select CopyStatusHelper.ConstructCopyStatusCachedEntry(db.Guid, amServer, null, tempEx, rpcDuration, activeManager)).ToArray<CopyStatusClientCachedEntry>();
			}
			else if (statuses != null)
			{
				Dictionary<Guid, RpcDatabaseCopyStatus2> tempStatusTable = statuses.ToDictionary((RpcDatabaseCopyStatus2 status) => status.DBGuid);
				tempEx = new ReplayServiceRpcUnknownInstanceException();
				result = (from db in expectedDatabases
				let guid = db.Guid
				let statusFound = tempStatusTable.ContainsKey(guid)
				select CopyStatusHelper.ConstructCopyStatusCachedEntry(guid, amServer, statusFound ? tempStatusTable[guid] : null, statusFound ? null : tempEx, rpcDuration, activeManager)).ToArray<CopyStatusClientCachedEntry>();
			}
			else
			{
				DiagCore.RetailAssert(false, "If no exception was thrown by GetCopyStatus RPC, then we should have some status results!", new object[0]);
			}
			return result;
		}

		internal static bool CheckCopyStatusNewer(ICopyStatusCachedEntry newStatus, ICopyStatusCachedEntry cachedStatus)
		{
			bool result = false;
			if (cachedStatus == null || cachedStatus.CopyStatus == null || newStatus.CopyStatus == null)
			{
				result = true;
			}
			else if (newStatus.CopyStatus.StatusRetrievedTime > cachedStatus.CopyStatus.StatusRetrievedTime)
			{
				result = true;
				if (newStatus.CopyStatus.LastLogGeneratedTime > DateTime.MinValue && newStatus.CopyStatus.LastLogGeneratedTime < cachedStatus.CopyStatus.LastLogGeneratedTime)
				{
					newStatus.CopyStatus.LastLogGenerated = cachedStatus.CopyStatus.LastLogGenerated;
					newStatus.CopyStatus.LastLogGeneratedTime = cachedStatus.CopyStatus.LastLogGeneratedTime;
				}
			}
			return result;
		}

		internal static Exception RunADOperation(Action operation)
		{
			Exception result = null;
			try
			{
				operation();
			}
			catch (ADExternalException ex)
			{
				result = ex;
			}
			catch (ADOperationException ex2)
			{
				result = ex2;
			}
			catch (ADTransientException ex3)
			{
				result = ex3;
			}
			return result;
		}

		internal static CopyStatusRpcResult ConvertExceptionToCopyStatusRpcResultEnum(Exception exception)
		{
			if (exception == null)
			{
				return CopyStatusRpcResult.Success;
			}
			Exception ex;
			if (exception.TryGetExceptionOrInnerOfType(out ex))
			{
				return CopyStatusRpcResult.RpcError;
			}
			return CopyStatusRpcResult.ServerError;
		}

		internal static string GetCopyStatusMonitoringDisplayString(RpcDatabaseCopyStatus2 status)
		{
			if (status == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(1323);
			stringBuilder.AppendFormat("WorkerProcessId            : {0}{1}", status.WorkerProcessId, Environment.NewLine);
			stringBuilder.AppendFormat("ActivationPreference       : {0}{1}", status.ActivationPreference, Environment.NewLine);
			stringBuilder.AppendFormat("CopyStatus                 : {0}{1}", status.CopyStatus, Environment.NewLine);
			stringBuilder.AppendFormat("Viable                     : {0}{1}", status.Viable, Environment.NewLine);
			stringBuilder.AppendFormat("ActivationSuspended        : {0}{1}", status.ActivationSuspended, Environment.NewLine);
			stringBuilder.AppendFormat("ErrorEventId               : {0}{1}", status.ErrorEventId, Environment.NewLine);
			stringBuilder.AppendFormat("LastStatusTransitionTime   : {0}{1}", status.LastStatusTransitionTime, Environment.NewLine);
			stringBuilder.AppendFormat("StatusRetrievedTime        : {0}{1}", status.StatusRetrievedTime, Environment.NewLine);
			stringBuilder.AppendFormat("InstanceStartTime          : {0}{1}", status.InstanceStartTime, Environment.NewLine);
			stringBuilder.AppendFormat("LowestLogPresent           : {0}{1}", status.LowestLogPresent, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogInspected           : {0}{1}", status.LastLogInspected, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogReplayed            : {0}{1}", status.LastLogReplayed, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogCopied              : {0}{1}", status.LastLogCopied, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogCopyNotified        : {0}{1}", status.LastLogCopyNotified, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogGenerated           : {0}{1}", status.LastLogGenerated, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogGeneratedTime       : {0}{1}", status.LastLogGeneratedTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastCopyNotifiedLogTime    : {0}{1}", status.LastCopyNotifiedLogTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastInspectedLogTime       : {0}{1}", status.LastInspectedLogTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastReplayedLogTime        : {0}{1}", status.LastReplayedLogTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastCopiedLogTime          : {0}{1}", status.LastCopiedLogTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogInfoFromClusterGen  : {0}{1}", status.LastLogInfoFromClusterGen, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogInfoFromClusterTime : {0}{1}", status.LastLogInfoFromClusterTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogInfoFromCopierTime  : {0}{1}", status.LastLogInfoFromCopierTime, Environment.NewLine);
			stringBuilder.AppendFormat("LastLogInfoIsStale         : {0}{1}", status.LastLogInfoIsStale, Environment.NewLine);
			stringBuilder.AppendFormat("ReplayLagEnabled           : {0}{1}", status.ReplayLagEnabled, Environment.NewLine);
			stringBuilder.AppendFormat("ReplayLagPlayDownReason    : {0}{1}", status.ReplayLagPlayDownReason, Environment.NewLine);
			stringBuilder.AppendFormat("ReplayLagPercentage        : {0}", status.ReplayLagPercentage);
			return stringBuilder.ToString();
		}

		private static Exception TimeCopyStatusRpc(Action rpcOperation, out TimeSpan rpcDuration)
		{
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			Exception result = null;
			try
			{
				replayStopwatch.Start();
				rpcOperation();
			}
			catch (TaskServerTransientException ex)
			{
				result = ex;
			}
			catch (TaskServerException ex2)
			{
				result = ex2;
			}
			finally
			{
				replayStopwatch.Stop();
				rpcDuration = replayStopwatch.Elapsed;
			}
			return result;
		}

		private static CopyStatusClientCachedEntry ConstructCopyStatusCachedEntry(Guid dbGuid, AmServerName server, RpcDatabaseCopyStatus2 status, Exception exception, TimeSpan rpcDuration, ActiveManager activeManager)
		{
			CopyStatusClientCachedEntry copyStatusClientCachedEntry = new CopyStatusClientCachedEntry(dbGuid, server);
			copyStatusClientCachedEntry.CopyStatus = status;
			copyStatusClientCachedEntry.RpcDuration = rpcDuration;
			copyStatusClientCachedEntry.LastException = exception;
			copyStatusClientCachedEntry.Result = CopyStatusHelper.ConvertExceptionToCopyStatusRpcResultEnum(exception);
			if (status != null)
			{
				copyStatusClientCachedEntry.ActiveServer = new AmServerName(status.ActiveDatabaseCopy, false);
			}
			else if (activeManager != null)
			{
				copyStatusClientCachedEntry.ActiveServer = CopyStatusHelper.GetActiveServerForDatabase(dbGuid, activeManager);
			}
			DiagCore.RetailAssert(copyStatusClientCachedEntry.Result != CopyStatusRpcResult.Success || copyStatusClientCachedEntry.CopyStatus != null, "If the GetCopyStatus RPC result was 'Success', we have to have a CopyStatus value!", new object[0]);
			DiagCore.RetailAssert(copyStatusClientCachedEntry.CopyStatus != null || copyStatusClientCachedEntry.Result != CopyStatusRpcResult.Success, "If the CopyStatus value was null, we have to return a Result that is *not* 'Success'!", new object[0]);
			return copyStatusClientCachedEntry;
		}

		private static AmServerName GetActiveServerForDatabase(Guid dbGuid, ActiveManager activeManager)
		{
			AmServerName result = null;
			Exception ex = null;
			try
			{
				DatabaseLocationInfo serverForDatabase = activeManager.GetServerForDatabase(dbGuid, GetServerForDatabaseFlags.BasicQuery);
				result = new AmServerName(serverForDatabase.ServerFqdn, false);
			}
			catch (DatabaseNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ObjectNotFoundException ex3)
			{
				ex = ex3;
			}
			catch (StoragePermanentException ex4)
			{
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				ex = ex5;
			}
			catch (AmCommonTransientException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				ExTraceGlobals.MonitoringTracer.TraceError<Guid, Exception>(0L, "CopyStatusHelper.GetActiveServerForDatabase(): Returning NULL. Got exception for database {0}: {1}", dbGuid, ex);
			}
			return result;
		}
	}
}
