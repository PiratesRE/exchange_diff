using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Win32;
using www.outlook.com.highavailability.ServerLocator.v1;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AmRpcClientHelper
	{
		internal static TimeSpan WcfTimeoutShort
		{
			get
			{
				if (AmRpcClientHelper.m_wcfTimeout == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_wcfTimeout = new TimeSpan?(AmRpcClientHelper.wcfTimeoutDefault);
						}
						else
						{
							int num = (int)((double)registryKey.GetValue("WCFTimeoutInMsec", AmRpcClientHelper.wcfTimeoutDefault.TotalMilliseconds));
							AmRpcClientHelper.m_wcfTimeout = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
						}
					}
				}
				return AmRpcClientHelper.m_wcfTimeout.Value;
			}
		}

		internal static TimeSpan WcfTimeoutLong
		{
			get
			{
				if (AmRpcClientHelper.m_wcfTimeoutLong == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_wcfTimeoutLong = new TimeSpan?(AmRpcClientHelper.wcfTimeoutLongDefault);
						}
						else
						{
							int num = (int)((double)registryKey.GetValue("WCFLongTimeoutInMsec", AmRpcClientHelper.wcfTimeoutLongDefault.TotalMilliseconds));
							AmRpcClientHelper.m_wcfTimeoutLong = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
						}
					}
				}
				return AmRpcClientHelper.m_wcfTimeoutLong.Value;
			}
		}

		internal static TimeSpan WcfTimeoutReceive
		{
			get
			{
				if (AmRpcClientHelper.m_wcfTimeoutReceive == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_wcfTimeoutReceive = new TimeSpan?(AmRpcClientHelper.wcfTimeoutReceiveDefault);
						}
						else
						{
							int num = (int)((double)registryKey.GetValue("WCFReceiveTimeoutInMsec", AmRpcClientHelper.wcfTimeoutReceiveDefault.TotalMilliseconds));
							AmRpcClientHelper.m_wcfTimeoutReceive = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
						}
					}
				}
				return AmRpcClientHelper.m_wcfTimeoutReceive.Value;
			}
		}

		internal static int RpcTimeoutShort
		{
			get
			{
				if (AmRpcClientHelper.m_rpcTimeout == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_rpcTimeout = new int?(5000);
						}
						else
						{
							int value = (int)registryKey.GetValue("RPCTimeoutInMsec", 5000);
							AmRpcClientHelper.m_rpcTimeout = new int?(value);
						}
					}
				}
				return AmRpcClientHelper.m_rpcTimeout.Value;
			}
		}

		internal static int RpcTimeoutAcllShort
		{
			get
			{
				if (AmRpcClientHelper.m_rpcTimeoutAcllShort == null)
				{
					int num = 120000;
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_rpcTimeoutAcllShort = new int?(num);
						}
						else
						{
							int value = (int)registryKey.GetValue("RPCTimeoutAcllShortInMsec", num);
							AmRpcClientHelper.m_rpcTimeoutAcllShort = new int?(value);
						}
					}
				}
				return AmRpcClientHelper.m_rpcTimeoutAcllShort.Value;
			}
		}

		internal static bool LogDiagnosticEvents
		{
			get
			{
				if (AmRpcClientHelper.m_logDiagnosticEvents == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_logDiagnosticEvents = new bool?(false);
						}
						else
						{
							object value = registryKey.GetValue("LogActiveManagerClientDiagnosticEvents", null);
							if (value != null)
							{
								AmRpcClientHelper.m_logDiagnosticEvents = new bool?(Convert.ToBoolean(value));
							}
							else
							{
								AmRpcClientHelper.m_logDiagnosticEvents = new bool?(false);
							}
						}
					}
				}
				return AmRpcClientHelper.m_logDiagnosticEvents.Value;
			}
		}

		internal static TimeSpan WcfCleanupTime
		{
			get
			{
				if (AmRpcClientHelper.m_wcfCleanupTime == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey == null)
						{
							AmRpcClientHelper.m_wcfCleanupTime = new TimeSpan?(AmRpcClientHelper.wcfCleanupTimeDefault);
						}
						else
						{
							int num = (int)((double)registryKey.GetValue("WCFCleanupTimeInMsec", AmRpcClientHelper.wcfCleanupTimeDefault.TotalMilliseconds));
							AmRpcClientHelper.m_wcfCleanupTime = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
						}
					}
				}
				return AmRpcClientHelper.m_wcfCleanupTime.Value;
			}
		}

		private static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		public static void MountDatabase(IADDatabase database, int storeFlags, int amFlags, int mountDialOverride)
		{
			AmRpcClientHelper.RunDatabaseRpcWithReferral(AmRpcOperationHint.Mount, database, null, delegate(AmRpcClient rpcClient, string rpcServerName)
			{
				if (AmRpcClientHelper.IsMountWithAmFlagsRpcSupported(rpcServerName))
				{
					return rpcClient.MountDatabase3(database.Guid, storeFlags, amFlags, mountDialOverride);
				}
				return rpcClient.MountDatabase(database.Guid, storeFlags, mountDialOverride);
			});
		}

		public static Exception AdminMountDatabaseWrapper(IADDatabase database)
		{
			Exception result = null;
			try
			{
				AmRpcClientHelper.MountDatabase(database, 0, 0, 0);
			}
			catch (AmServerException ex)
			{
				result = ex;
			}
			catch (AmServerTransientException ex2)
			{
				result = ex2;
			}
			return result;
		}

		public static void RemountDatabase(IADDatabase database, int flags, int mountDialOverride, string fromServer)
		{
			AmRpcClientHelper.RunDatabaseRpcWithReferral(AmRpcOperationHint.Remount, database, null, (AmRpcClient rpcClient, string rpcServerName) => rpcClient.RemountDatabase(database.Guid, flags, mountDialOverride, fromServer));
		}

		public static void DismountDatabase(IADDatabase database, int flags)
		{
			AmRpcClientHelper.RunDatabaseRpcWithReferral(AmRpcOperationHint.Dismount, database, null, (AmRpcClient rpcClient, string rpcServerName) => rpcClient.DismountDatabase(database.Guid, flags));
		}

		public static void MoveDatabaseEx(IADDatabase database, int flags, int dismountFlags, int mountDialOverride, string fromServer, string targetServer, bool tryOtherHealthyServers, int skipValidationChecks, AmDbActionCode actionCode, string moveComment, out string lastServerContacted, ref AmDatabaseMoveResult moveResult)
		{
			AmDatabaseMoveResult tempMoveResult = null;
			string tempLastServerContacted = null;
			lastServerContacted = null;
			moveResult = null;
			if (moveComment == null)
			{
				moveComment = string.Empty;
			}
			try
			{
				AmRpcClientHelper.RunDatabaseRpcWithReferral(AmRpcOperationHint.MoveEx, database, targetServer, delegate(AmRpcClient rpcClient, string serverName)
				{
					fromServer = ActiveManagerUtil.NullEncode(fromServer);
					targetServer = ActiveManagerUtil.NullEncode(targetServer);
					tempLastServerContacted = serverName;
					if (AmRpcVersionControl.IsMoveWithCatalogFailureReasonCodeSupported(AmRpcClientHelper.GetServerVersion(serverName)))
					{
						return rpcClient.MoveDatabaseEx3(database.Guid, flags, dismountFlags, mountDialOverride, fromServer, targetServer, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, (int)actionCode, moveComment, ref tempMoveResult);
					}
					if (AmRpcClientHelper.IsMoveDatabaseEx3RpcSupported(serverName))
					{
						if (AmDbActionReason.CatalogFailureItem == actionCode.Reason)
						{
							actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
						}
						return rpcClient.MoveDatabaseEx3(database.Guid, flags, dismountFlags, mountDialOverride, fromServer, targetServer, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, (int)actionCode, moveComment, ref tempMoveResult);
					}
					return rpcClient.MoveDatabaseEx2(database.Guid, flags, dismountFlags, mountDialOverride, fromServer, targetServer, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, (int)actionCode, ref tempMoveResult);
				});
			}
			finally
			{
				moveResult = tempMoveResult;
				lastServerContacted = tempLastServerContacted;
			}
		}

		public static int RpcchGetAutomountConsensusState(string serverName)
		{
			int automountConsensusState = 0;
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.GetAutomountConsensusState, serverName, null, null, (AmRpcClient rpcClient, string rpcServerName) => rpcClient.RpccGetAutomountConsensusState(ref automountConsensusState));
			return automountConsensusState;
		}

		public static void RpcchSetAutomountConsensusState(string serverName, int automountConsensusState)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.SetAutomountConsensusState, serverName, null, null, (AmRpcClient rpcClient, string rpcServerName) => rpcClient.RpccSetAutomountConsensusState(automountConsensusState));
		}

		public static void RpcchSetAutomountConsensusStateBestEffort(string serverName, int automountConsensusState, TimeSpan timeout)
		{
			try
			{
				AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.SetAutomountConsensusState, serverName, null, new int?((int)timeout.TotalMilliseconds), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.RpccSetAutomountConsensusState(automountConsensusState));
			}
			catch (AmServerException arg)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<AmServerException>(0L, "RpcchGetAutomountConsensusState got an exception: {0}", arg);
			}
			catch (AmServerTransientException arg2)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<AmServerTransientException>(0L, "RpcchGetAutomountConsensusState got a transient exception: {0}", arg2);
			}
		}

		public static AmPamInfo GetPrimaryActiveManager(IADDatabaseAvailabilityGroup dag)
		{
			IADToplogyConfigurationSession adsession = AmRpcClientHelper.GetADSession();
			return AmRpcClientHelper.GetPrimaryActiveManager(dag, adsession);
		}

		public static AmPamInfo GetPrimaryActiveManager(IADDatabaseAvailabilityGroup dag, IADToplogyConfigurationSession adSession)
		{
			bool flag = false;
			Exception ex = null;
			AmPamInfo amPamInfo = null;
			MultiValuedProperty<ADObjectId> servers = dag.Servers;
			if (servers == null || servers.Count == 0)
			{
				throw new AmFailedToDeterminePAM(dag.Name);
			}
			foreach (ADObjectId entryId in servers)
			{
				try
				{
					IADServer iadserver = adSession.ReadMiniServer(entryId);
					string fqdn = iadserver.Fqdn;
					AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.GetPAM, fqdn, null, null, (AmRpcClient rpcClient, string rpcServerName) => rpcClient.GetPrimaryActiveManager(ref amPamInfo));
					flag = true;
					break;
				}
				catch (AmServerException ex2)
				{
					ex = ex2;
				}
				catch (AmServerTransientException ex3)
				{
					ex = ex3;
				}
				catch (ADTransientException ex4)
				{
					ex = ex4;
				}
			}
			if (!flag)
			{
				throw ex;
			}
			return amPamInfo;
		}

		public static void AttemptCopyLastLogsDirect(string serverToRpc, Guid dbGuid, DatabaseMountDialOverride mountDialOverride, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, string sourceServer, int actionCode, int skipValidationChecks, bool mountPending, string uniqueOperationId, int subactionAttemptNumber, ref AmAcllReturnStatus acllStatus)
		{
			acllStatus = null;
			AmAcllReturnStatus tempAcllStatus = null;
			bool tempNoLoss = false;
			bool tempMountAllowed = false;
			if (uniqueOperationId == null)
			{
				throw new ArgumentNullException("uniqueOperationId");
			}
			try
			{
				int value = AmRpcClientHelper.RpcTimeoutAcllShort;
				if (mountDialOverride == DatabaseMountDialOverride.BestEffort)
				{
					value = 0;
				}
				AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.AcllDirect, serverToRpc, null, new int?(value), delegate(AmRpcClient rpcClient, string rpcServerName)
				{
					if (AmRpcClientHelper.IsAttemptCopyLastLogsDirect3Supported(serverToRpc))
					{
						return rpcClient.AttemptCopyLastLogsDirect3(dbGuid, (int)mountDialOverride, numRetries, e00timeoutMs, networkIOtimeoutMs, networkConnecttimeoutMs, sourceServer, actionCode, skipValidationChecks, mountPending, uniqueOperationId, subactionAttemptNumber, ref tempAcllStatus);
					}
					if (AmRpcClientHelper.IsAttemptCopyLastLogsDirect2Supported(serverToRpc))
					{
						return rpcClient.AttemptCopyLastLogsDirect2(dbGuid, (int)mountDialOverride, numRetries, e00timeoutMs, networkIOtimeoutMs, networkConnecttimeoutMs, sourceServer, actionCode, mountPending, uniqueOperationId, subactionAttemptNumber, ref tempAcllStatus);
					}
					RpcErrorExceptionInfo result = rpcClient.AttemptCopyLastLogsDirect(dbGuid, (int)mountDialOverride, numRetries, e00timeoutMs, networkIOtimeoutMs, networkConnecttimeoutMs, sourceServer, actionCode, ref tempNoLoss, ref tempMountAllowed);
					tempAcllStatus = new AmAcllReturnStatus();
					tempAcllStatus.NoLoss = tempNoLoss;
					tempAcllStatus.MountAllowed = tempMountAllowed;
					return result;
				});
			}
			finally
			{
				acllStatus = tempAcllStatus;
			}
		}

		public static void MountDatabaseDirectEx(string serverToRpc, Guid dbGuid, AmMountArg mountArg)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.MountDirectEx, serverToRpc, null, new int?(0), delegate(AmRpcClient rpcClient, string rpcServerName)
			{
				if (AmRpcClientHelper.IsMountWithAmFlagsRpcSupported(rpcServerName))
				{
					return rpcClient.MountDatabaseDirect3(dbGuid, mountArg);
				}
				return rpcClient.MountDatabaseDirectEx(dbGuid, mountArg);
			});
		}

		public static void DismountDatabaseDirect(string serverToRpc, Guid dbGuid, AmDismountArg dismountArg)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.DismountDirect, serverToRpc, null, new int?(0), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.DismountDatabaseDirect(dbGuid, dismountArg));
		}

		public static void ServerSwitchOver(string serverToRpc, string sourceServerFqdn)
		{
			if (string.IsNullOrEmpty(serverToRpc))
			{
				IADToplogyConfigurationSession adsession = AmRpcClientHelper.GetADSession();
				serverToRpc = AmRpcClientHelper.GetPrimaryActiveManager(sourceServerFqdn, adsession);
			}
			AmRpcClientHelper.RunServerRpcWithReferral(AmRpcOperationHint.SwitchOver, serverToRpc, new int?(0), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.ServerSwitchOver(sourceServerFqdn));
		}

		public static List<AmDatabaseMoveResult> ServerMoveAllDatabases(string sourceServerFqdn, string targetServerFqdn, int mountFlags, int dismountFlags, int mountDialOverride, bool tryOtherHealthyServers, int skipValidationChecks, int actionCode, string moveComment, out string lastServerContacted)
		{
			return AmRpcClientHelper.ServerMoveAllDatabases(sourceServerFqdn, targetServerFqdn, mountFlags, dismountFlags, mountDialOverride, tryOtherHealthyServers, skipValidationChecks, actionCode, moveComment, null, out lastServerContacted);
		}

		public static List<AmDatabaseMoveResult> ServerMoveAllDatabases(string sourceServerFqdn, string targetServerFqdn, int mountFlags, int dismountFlags, int mountDialOverride, bool tryOtherHealthyServers, int skipValidationChecks, int actionCode, string moveComment, string componentName, out string lastServerContacted)
		{
			List<AmDatabaseMoveResult> moveResults = null;
			string tempLastServerContacted = null;
			lastServerContacted = null;
			string dagMemberServerFqdn;
			if (string.IsNullOrEmpty(sourceServerFqdn))
			{
				sourceServerFqdn = string.Empty;
				if (string.IsNullOrEmpty(targetServerFqdn))
				{
					throw new ArgumentException("targetServerFqdn must be specified when source is not");
				}
				dagMemberServerFqdn = targetServerFqdn;
			}
			else
			{
				dagMemberServerFqdn = sourceServerFqdn;
			}
			if (targetServerFqdn == null)
			{
				targetServerFqdn = string.Empty;
			}
			if (moveComment == null)
			{
				moveComment = string.Empty;
			}
			if (componentName == null)
			{
				componentName = string.Empty;
			}
			IADToplogyConfigurationSession adsession = AmRpcClientHelper.GetADSession();
			string primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(dagMemberServerFqdn, adsession);
			AmRpcClientHelper.RunServerRpcWithReferral(AmRpcOperationHint.MoveAllDatabases, primaryActiveManager, new int?(0), delegate(AmRpcClient rpcClient, string serverName)
			{
				tempLastServerContacted = serverName;
				if (string.IsNullOrEmpty(sourceServerFqdn) && !AmRpcClientHelper.IsServerMoveBackDatabasesRpcSupported(serverName))
				{
					throw new AmRpcOperationNotImplemented("Move -ActivatePreferredOnServer", serverName);
				}
				if (AmRpcClientHelper.IsServerMoveAllDatabases3RpcSupported(serverName))
				{
					return rpcClient.ServerMoveAllDatabases3(sourceServerFqdn, targetServerFqdn, mountFlags, dismountFlags, mountDialOverride, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, actionCode, moveComment, componentName, ref moveResults);
				}
				if (AmRpcClientHelper.IsMoveDatabaseEx3RpcSupported(serverName))
				{
					return rpcClient.ServerMoveAllDatabases2(sourceServerFqdn, targetServerFqdn, mountFlags, dismountFlags, mountDialOverride, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, actionCode, moveComment, ref moveResults);
				}
				return rpcClient.ServerMoveAllDatabases(sourceServerFqdn, targetServerFqdn, mountFlags, dismountFlags, mountDialOverride, Convert.ToInt32(tryOtherHealthyServers), skipValidationChecks, actionCode, ref moveResults);
			});
			lastServerContacted = tempLastServerContacted;
			return moveResults;
		}

		public static bool IsRunning(string serverName)
		{
			RpcErrorExceptionInfo errorInfo = null;
			AmRpcExceptionWrapper.Instance.ClientRetryableOperation(serverName, delegate
			{
				using (AmRpcClient amRpcClient = AmRpcClientHelper.GetAmRpcClient(AmRpcOperationHint.IsRunning, serverName, new int?(AmRpcClientHelper.RpcTimeoutShort)))
				{
					errorInfo = amRpcClient.IsRunning();
				}
			});
			return !errorInfo.IsFailed();
		}

		public static AmRole GetActiveManagerRole(string serverToRpc, out string errorMessage)
		{
			AmRole amRole = AmRole.Unknown;
			string tmpMsg = string.Empty;
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.GetAmRole, serverToRpc, null, new int?(AmRpcClientHelper.RpcTimeoutShort), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.GetActiveManagerRole(ref amRole, ref tmpMsg));
			errorMessage = tmpMsg;
			return amRole;
		}

		public static bool CheckThirdPartyListener(string serverToRpc, out string errorMessage)
		{
			bool healthy = false;
			string tmpMsg = string.Empty;
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.CheckThirdPartyListener, serverToRpc, null, new int?(AmRpcClientHelper.RpcTimeoutShort), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.CheckThirdPartyListener(ref healthy, ref tmpMsg));
			errorMessage = tmpMsg;
			return healthy;
		}

		public static void ReportSystemEvent(string serverToRpc, int eventCode, string reportingServer)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.ReportSystemEvent, serverToRpc, null, new int?(AmRpcClientHelper.RpcTimeoutShort), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.ReportSystemEvent(eventCode, reportingServer));
		}

		public static void AmRefreshConfiguration(string serverToRpc, AmRefreshConfigurationFlags refreshFlags, int maxSecondsToWait)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.AmRefreshConfiguration, serverToRpc, null, new int?(AmRpcClientHelper.RpcTimeoutShort + maxSecondsToWait * 1000), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.AmRefreshConfiguration((int)refreshFlags, maxSecondsToWait));
		}

		public static bool IsReportSystemEventSupportEventCode(AmSystemEventCode eventCode, string serverName)
		{
			return AmRpcVersionControl.IsReportSystemEventRpcSupported(AmRpcClientHelper.GetServerVersion(serverName), eventCode);
		}

		public static bool IsAttemptCopyLastLogsDirect2Supported(string serverName)
		{
			return AmRpcVersionControl.IsAttemptCopyLastLogsDirect2RpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsAttemptCopyLastLogsDirect3Supported(string serverName)
		{
			return AmRpcVersionControl.IsAttemptCopyLastLogsDirect3RpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsServerMoveAllDatabasesRpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsServerMoveAllDatabasesRpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsMoveDatabaseEx2RpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsMoveDatabaseEx2RpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsMoveDatabaseEx3RpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsMoveDatabaseEx3RpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsMountWithAmFlagsRpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsMountWithAmFlagsRpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsReportServiceKillRpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsReportServiceKillRpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsGetDeferredRecoveryEntriesRpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsGetDeferredRecoveryEntriesRpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsServerMoveAllDatabases3RpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsServerMoveAllDatabases3RpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		public static bool IsServerMoveBackDatabasesRpcSupported(string serverName)
		{
			return AmRpcVersionControl.IsServerMoveBackDatabasesRpcSupported(AmRpcClientHelper.GetServerVersion(serverName));
		}

		internal static AmDbStatusInfo2 RpcchGetServerForDatabase(IADDatabase database, NetworkCredential networkCredential, IFindAdObject<IADDatabaseAvailabilityGroup> dagLookup, IFindMiniServer findMiniServer, ActiveManagerClientPerfmonInstance perfCounters, bool isService, out ServerVersion serverVersionNumberOrNull)
		{
			bool flag = false;
			Exception ex = null;
			AmDbStatusInfo2 result = null;
			serverVersionNumberOrNull = null;
			Interlocked.Increment(ref AmRpcClientHelper.s_getServerForDatabaseCalls);
			if (isService)
			{
				result = AmRpcClientHelper.RpcchGetServerForDatabaseRPC(database, networkCredential, MachineName.Local, out flag, out ex);
			}
			else
			{
				List<ADObjectId> orderedServerIdsForDatabase = ActiveManagerUtil.GetOrderedServerIdsForDatabase(dagLookup, database, out ex);
				if (ex != null)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Exception>(0L, "RpcchGetServerForDatabase(): An AD error occurred when trying to find copies of DB '{0}': {1}", database.Name, ex);
					throw new AmDatabaseADException(database.Name, ex.Message, ex);
				}
				int num = 0;
				if (!string.Equals(orderedServerIdsForDatabase[0].Name, MachineName.Local, StringComparison.OrdinalIgnoreCase))
				{
					num = 0;
					for (int i = 0; i < orderedServerIdsForDatabase.Count; i++)
					{
						if (string.Equals(database.Server.Name, orderedServerIdsForDatabase[i].Name, StringComparison.OrdinalIgnoreCase))
						{
							num = i;
							break;
						}
					}
				}
				int j = 0;
				while (j < orderedServerIdsForDatabase.Count * 2)
				{
					IADServer iadserver = findMiniServer.ReadMiniServerByObjectId(orderedServerIdsForDatabase[num]);
					if (iadserver != null)
					{
						if (iadserver.VersionNumber < 1937801646)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "RpcchGetServerForDatabase({0}) is NOT attempting to talk to server {1}, as it is running version {2:x}, which does not support the correct IDL (req'd version=3.0, build {3:x}).", new object[]
							{
								database.Name,
								iadserver.Fqdn,
								iadserver.VersionNumber,
								1937801646
							});
						}
						else if (j < orderedServerIdsForDatabase.Count && AmRpcClientHelper.ServerIsPotentiallyInMaintenanceMode(iadserver))
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "RpcchGetServerForDatabase({0}) is NOT attempting to talk to server {1}, as it is possibly in maintenance mode or switched over. DatabaseCopyAutoActivationPolicy is '{2}', DatabaseCopyActivationDisabledAndMoveNow is {3}, ComponentStates is {4}.", new object[]
							{
								database.Name,
								iadserver.Fqdn,
								iadserver.DatabaseCopyAutoActivationPolicy,
								iadserver.DatabaseCopyActivationDisabledAndMoveNow,
								(iadserver.ComponentStates != null) ? string.Join(",", iadserver.ComponentStates.ToArray()) : string.Empty
							});
						}
						else
						{
							string fqdn = iadserver.Fqdn;
							if (ReplayRpcVersionControl.IsServerLocatorServiceSupported(iadserver.AdminDisplayVersion))
							{
								AmWCFCallType callType = AmWCFCallType.Unknown;
								Exception ex2 = null;
								try
								{
									if (string.Equals(fqdn, NativeHelpers.GetLocalComputerFqdn(false), StringComparison.OrdinalIgnoreCase))
									{
										callType = AmWCFCallType.LocalServer;
									}
									else if (string.Equals(fqdn.Substring(fqdn.IndexOf(".") + 1), NativeHelpers.GetDomainName(), StringComparison.OrdinalIgnoreCase))
									{
										if (iadserver.ServerSite != null && string.Equals(iadserver.ServerSite.Name, NativeHelpers.GetSiteName(false), StringComparison.OrdinalIgnoreCase))
										{
											callType = AmWCFCallType.RemoteServerSameDomainSameSite;
										}
										else
										{
											callType = AmWCFCallType.RemoteServerSameDomainDifferentSite;
										}
									}
									else
									{
										callType = AmWCFCallType.RemoteServerDifferentDomain;
									}
								}
								catch (ADTransientException ex3)
								{
									ex2 = ex3;
								}
								catch (CannotGetDomainInfoException ex4)
								{
									ex2 = ex4;
								}
								catch (CannotGetSiteInfoException ex5)
								{
									ex2 = ex5;
								}
								catch (CannotGetComputerNameException ex6)
								{
									ex2 = ex6;
								}
								if (ex2 != null)
								{
									ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string>(0L, "RpcchGetServerForDatabase({0}) failed to determine callType. Error: {1}", database.Name, ex2.Message);
								}
								result = AmRpcClientHelper.GetServerForDatabaseWCF(database, networkCredential, fqdn, callType, perfCounters, out flag, out ex, out serverVersionNumberOrNull);
								if (flag)
								{
									break;
								}
							}
							else
							{
								result = AmRpcClientHelper.RpcchGetServerForDatabaseRPC(database, networkCredential, fqdn, out flag, out ex);
								if (flag)
								{
									break;
								}
							}
							Interlocked.Increment(ref AmRpcClientHelper.s_getServerForDatabaseErrs);
						}
					}
					j++;
					num = (num + 1) % orderedServerIdsForDatabase.Count;
				}
			}
			if (!flag)
			{
				if (ex != null)
				{
					throw ex;
				}
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "RpcchGetServerForDatabase({0}) did not get any real information. Creating a fake AmDbStatusInfo2 structure with mountedTime=UtcNow.", database.Name);
				result = new AmDbStatusInfo2(null, (database.ReplicationType == ReplicationType.Remote) ? 1 : 0, null, DateTime.UtcNow);
			}
			return result;
		}

		public static bool ServerIsPotentiallyInMaintenanceMode(IADServer miniServer)
		{
			return miniServer.DatabaseCopyAutoActivationPolicy == DatabaseCopyAutoActivationPolicyType.Blocked || miniServer.DatabaseCopyActivationDisabledAndMoveNow || !ServerComponentStates.IsServerOnline(miniServer.ComponentStates);
		}

		public static void AmReportServiceKill(string serverToRpc, int timeOutInMSec, string serviceName, string serverReporting, string timeStampStrInUtc)
		{
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.ReportServiceKill, serverToRpc, null, new int?(timeOutInMSec), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.ReportServiceKill(serviceName, serverReporting, timeStampStrInUtc));
		}

		public static List<AmDeferredRecoveryEntry> GetDeferredRecoveryEntries(string serverToRpc)
		{
			List<AmDeferredRecoveryEntry> entries = null;
			AmRpcClientHelper.RunServerRpcWithReferral(AmRpcOperationHint.GetDeferredRecoveryEntries, serverToRpc, new int?(0), (AmRpcClient rpcClient, string serverName) => rpcClient.GetDeferredRecoveryEntries(ref entries));
			return entries;
		}

		internal static AmDbStatusInfo2 GetServerForDatabaseWCF(IADDatabase database, NetworkCredential networkCredential, string serverName, AmWCFCallType callType, ActiveManagerClientPerfmonInstance perfCounters, out bool isSuccess, out Exception lastException, out ServerVersion serverVersionNumberOrNull)
		{
			isSuccess = false;
			lastException = null;
			serverVersionNumberOrNull = null;
			bool internalIsSuccess = false;
			Exception internalLastException = null;
			AmDbStatusInfo2 internalAmDbInfo = null;
			ServerVersion internalServerVersionNumber = null;
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					ServerLocatorServiceClient serverLocatorServiceClient = null;
					try
					{
						perfCounters.GetServerForDatabaseWCFCalls.Increment();
						perfCounters.GetServerForDatabaseWCFCallsPerSec.Increment();
						switch (callType)
						{
						case AmWCFCallType.LocalServer:
							perfCounters.GetServerForDatabaseWCFLocalCalls.Increment();
							perfCounters.GetServerForDatabaseWCFLocalCallsPerSec.Increment();
							break;
						case AmWCFCallType.RemoteServerSameDomainSameSite:
							perfCounters.GetServerForDatabaseWCFLocalDomainLocalSiteCalls.Increment();
							perfCounters.GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec.Increment();
							break;
						case AmWCFCallType.RemoteServerSameDomainDifferentSite:
							perfCounters.GetServerForDatabaseWCFLocalDomainRemoteSiteCalls.Increment();
							perfCounters.GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec.Increment();
							break;
						case AmWCFCallType.RemoteServerDifferentDomain:
							perfCounters.GetServerForDatabaseWCFRemoteDomainCalls.Increment();
							perfCounters.GetServerForDatabaseWCFRemoteDomainCallsPerSec.Increment();
							break;
						}
						Stopwatch stopwatch = Stopwatch.StartNew();
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<Guid, string, string>(0L, "GetServerForDatabaseWCF({0}({1})) sending WCF call to {2}", database.Guid, database.Name, serverName);
						serverLocatorServiceClient = ActiveManager.s_wcfClientProxyPool.TakeClientFromThePool(serverName);
						while (serverLocatorServiceClient != null && !serverLocatorServiceClient.IsUsable)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "GetServerForDatabaseWCF client found for server '{0}' is not usable. Searching for another one.", serverName);
							serverLocatorServiceClient.Dispose(true);
							serverLocatorServiceClient = ActiveManager.s_wcfClientProxyPool.TakeClientFromThePool(serverName);
						}
						if (serverLocatorServiceClient == null)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "GetServerForDatabaseWCF creates new client for server '{0}'.", serverName);
							serverLocatorServiceClient = ServerLocatorServiceClient.Create(serverName, AmRpcClientHelper.WcfTimeoutShort, AmRpcClientHelper.WcfTimeoutShort, AmRpcClientHelper.WcfTimeoutReceive, AmRpcClientHelper.WcfTimeoutShort);
						}
						DatabaseServerInformation serverForDatabase = serverLocatorServiceClient.GetServerForDatabase(database.Guid);
						internalAmDbInfo = new AmDbStatusInfo2(serverForDatabase.ServerFqdn, (database.ReplicationType == ReplicationType.Remote) ? 1 : 0, serverForDatabase.LastMountedServerFqdn, serverForDatabase.MountedTimeUtc);
						internalServerVersionNumber = new ServerVersion(serverForDatabase.ServerVersion);
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "GetServerForDatabaseWCF({0}({1}) successfully talked to server {2}. Result: IsHighlyAvailable = {3}, LastMountedServerFqdn = {4}, MasterServerFqdn = {5}, MountedTime = {6}.", new object[]
						{
							database.Guid,
							database.Name,
							serverName,
							internalAmDbInfo.IsHighlyAvailable,
							internalAmDbInfo.LastMountedServerFqdn,
							internalAmDbInfo.MasterServerFqdn,
							internalAmDbInfo.MountedTime
						});
						internalIsSuccess = true;
						long elapsedTicks = stopwatch.ElapsedTicks;
						perfCounters.GetServerForDatabaseWCFLatency.IncrementBy(elapsedTicks);
						perfCounters.GetServerForDatabaseWCFLatencyTimeBase.Increment();
						switch (callType)
						{
						case AmWCFCallType.LocalServer:
							perfCounters.GetServerForDatabaseWCFLocalLatency.IncrementBy(elapsedTicks);
							perfCounters.GetServerForDatabaseWCFLocalLatencyTimeBase.Increment();
							break;
						case AmWCFCallType.RemoteServerSameDomainSameSite:
							perfCounters.GetServerForDatabaseWCFLocalDomainLocalSiteLatency.IncrementBy(elapsedTicks);
							perfCounters.GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase.Increment();
							break;
						case AmWCFCallType.RemoteServerSameDomainDifferentSite:
							perfCounters.GetServerForDatabaseWCFLocalDomainRemoteSiteLatency.IncrementBy(elapsedTicks);
							perfCounters.GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase.Increment();
							break;
						case AmWCFCallType.RemoteServerDifferentDomain:
							perfCounters.GetServerForDatabaseWCFRemoteDomainLatency.IncrementBy(elapsedTicks);
							perfCounters.GetServerForDatabaseWCFRemoteDomainLatencyTimeBase.Increment();
							break;
						}
					}
					catch (ServerLocatorClientException ex2)
					{
						perfCounters.GetServerForDatabaseWCFErrors.Increment();
						perfCounters.GetServerForDatabaseWCFErrorsPerSec.Increment();
						ExTraceGlobals.ActiveManagerClientTracer.TraceError<ServerLocatorClientException>(0L, "GetServerForDatabaseWCF caught the following exception: {0}", ex2);
						internalLastException = new AmServerException(ex2.Message, ex2);
					}
					catch (ServerLocatorClientTransientException ex3)
					{
						if (ex3.InnerException is TimeoutException)
						{
							perfCounters.GetServerForDatabaseWCFTimeouts.Increment();
							perfCounters.GetServerForDatabaseWCFTimeoutsPerSec.Increment();
						}
						else
						{
							perfCounters.GetServerForDatabaseWCFErrors.Increment();
							perfCounters.GetServerForDatabaseWCFErrorsPerSec.Increment();
						}
						ExTraceGlobals.ActiveManagerClientTracer.TraceError<ServerLocatorClientTransientException>(0L, "GetServerForDatabaseWCF caught the following exception: {0}", ex3);
						internalLastException = new AmServerTransientException(ex3.Message, ex3);
					}
					finally
					{
						if (serverLocatorServiceClient != null)
						{
							if (serverLocatorServiceClient.IsUsable)
							{
								if (!ActiveManager.s_wcfClientProxyPool.ReturnClientIntoThePool(serverName, serverLocatorServiceClient))
								{
									serverLocatorServiceClient.Dispose(true);
								}
							}
							else
							{
								serverLocatorServiceClient.Dispose(true);
							}
						}
					}
				}, AmRpcClientHelper.WcfTimeoutLong);
			}
			catch (TimeoutException ex)
			{
				perfCounters.GetServerForDatabaseWCFTimeouts.Increment();
				perfCounters.GetServerForDatabaseWCFTimeoutsPerSec.Increment();
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<TimeoutException>(0L, "GetServerForDatabaseWCF caught the following exception: {0}", ex);
				isSuccess = false;
				lastException = new AmServerTransientException(ex.Message, ex);
				return null;
			}
			isSuccess = internalIsSuccess;
			lastException = internalLastException;
			serverVersionNumberOrNull = internalServerVersionNumber;
			return internalAmDbInfo;
		}

		internal static AmDbStatusInfo2 RpcchGetServerForDatabaseRPC(IADDatabase database, NetworkCredential networkCredential, string serverName, out bool isSuccess, out Exception lastException)
		{
			isSuccess = false;
			lastException = null;
			AmDbStatusInfo2 amDbInfo = null;
			try
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<Guid, string, string>(0L, "RpcchGetServerForDatabaseRPC({0}({1}) sending RPC to {2}", database.Guid, database.Name, serverName);
				AmRpcClientHelper.RunRpcOperationWithAuth(AmRpcOperationHint.Gsfd, serverName, database.Name, networkCredential, new int?(AmRpcClientHelper.RpcTimeoutShort), (AmRpcClient rpcClient, string rpcServerName) => rpcClient.RpccGetServerForDatabase(database.Guid, ref amDbInfo));
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "RpcchGetServerForDatabaseRPC({0}({1}) successfully talked to server {2}. Result: IsHighlyAvailable = {3}, LastMountedServerFqdn = {4}, MasterServerFqdn = {5}, MountedTime = {6}.", new object[]
				{
					database.Guid,
					database.Name,
					serverName,
					amDbInfo.IsHighlyAvailable,
					amDbInfo.LastMountedServerFqdn,
					amDbInfo.MasterServerFqdn,
					amDbInfo.MountedTime
				});
				isSuccess = true;
			}
			catch (AmServerException ex)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<AmServerException>(0L, "RpcchGetServerForDatabaseRPC caught the following exception: {0}", ex);
				lastException = ex;
			}
			catch (AmServerTransientException ex2)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<AmServerTransientException>(0L, "RpcchGetServerForDatabaseRPC caught the following exception: {0}", ex2);
				lastException = ex2;
			}
			return amDbInfo;
		}

		internal static string GetPrimaryActiveManager(string dagMemberServerFqdn, IADToplogyConfigurationSession adSession)
		{
			IADServer miniServer = AmRpcClientHelper.GetMiniServer(dagMemberServerFqdn, adSession);
			if (miniServer == null)
			{
				throw new AmServerNotFoundException(dagMemberServerFqdn);
			}
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = null;
			try
			{
				iaddatabaseAvailabilityGroup = adSession.ReadADObject<IADDatabaseAvailabilityGroup>(miniServer.DatabaseAvailabilityGroup);
			}
			catch (ADTransientException arg)
			{
				AmRpcClientHelper.Tracer.TraceDebug<ADTransientException>(0L, "GetPrimaryActiveManager() ReadMiniServerByObjectId experienced an ADTransientException: {0}", arg);
			}
			if (iaddatabaseAvailabilityGroup == null)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "DAG for server '{0}' could not be found.", dagMemberServerFqdn);
				throw new AmServerDagNotFound(dagMemberServerFqdn);
			}
			AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(iaddatabaseAvailabilityGroup, adSession);
			return primaryActiveManager.ServerName;
		}

		private static string GetServerToPerformDatabaseOperation(IADDatabase database, string targetServer)
		{
			string result = null;
			IADToplogyConfigurationSession adsession = AmRpcClientHelper.GetADSession();
			IADServer iadserver = adsession.ReadMiniServer(database.MasterServerOrAvailabilityGroup);
			if (iadserver == null)
			{
				IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = null;
				try
				{
					iaddatabaseAvailabilityGroup = adsession.ReadADObject<IADDatabaseAvailabilityGroup>(database.MasterServerOrAvailabilityGroup);
				}
				catch (ADTransientException arg)
				{
					AmRpcClientHelper.Tracer.TraceDebug<ADTransientException>(0L, "GetServerToPerformDatabaseOperation experienced an ADTransientException: {0}", arg);
				}
				if (iaddatabaseAvailabilityGroup == null)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "Database {0} is on a standalone server, but master server is null.", database.Name);
					throw new AmDatabaseMasterIsInvalid(database.Name);
				}
				AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(iaddatabaseAvailabilityGroup, adsession);
				if (primaryActiveManager != null)
				{
					result = primaryActiveManager.ServerName;
				}
			}
			else if (string.IsNullOrEmpty(targetServer))
			{
				ADObjectId databaseAvailabilityGroup = iadserver.DatabaseAvailabilityGroup;
				if (databaseAvailabilityGroup != null)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string, ADObjectId>(0L, "Database {0} master is pointing to a standalone server, but the server pointed is a member of the DAG {1}.", database.Name, databaseAvailabilityGroup);
					throw new AmDatabaseMasterIsInvalid(database.Name);
				}
				result = iadserver.Fqdn;
			}
			else
			{
				result = targetServer;
			}
			return result;
		}

		internal static AmRpcClient GetAmRpcClient(AmRpcOperationHint rpcOperationHint, string serverToRpc)
		{
			return AmRpcClientHelper.GetAmRpcClient(rpcOperationHint, serverToRpc, null, null);
		}

		internal static AmRpcClient GetAmRpcClient(AmRpcOperationHint rpcOperationHint, string serverName, int? timeOutMs)
		{
			return AmRpcClientHelper.GetAmRpcClient(rpcOperationHint, serverName, null, timeOutMs);
		}

		internal static AmRpcClient GetAmRpcClient(AmRpcOperationHint rpcOperationHint, string serverName, NetworkCredential networkCredential, int? timeOutMs)
		{
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<AmRpcOperationHint, string, int?>(0L, "GetAmRpcClient called. (operation={0}, server={1}, timeout={2})", rpcOperationHint, serverName, timeOutMs);
			if (!AmRpcClientHelper.IsRpcOperationImplemented(rpcOperationHint, serverName))
			{
				throw new AmRpcOperationNotImplemented(rpcOperationHint.ToString(), serverName);
			}
			AmRpcClient amRpcClient = new AmRpcClient(serverName, networkCredential);
			if (timeOutMs != null)
			{
				amRpcClient.SetTimeOut(timeOutMs.Value);
			}
			return amRpcClient;
		}

		internal static bool IsRpcOperationImplemented(AmRpcOperationHint rpcOperationHint, string serverName)
		{
			bool result = true;
			if (rpcOperationHint == AmRpcOperationHint.Remount)
			{
				if (!AmRpcVersionControl.IsRemountRpcSupported(AmRpcClientHelper.GetServerVersion(serverName)))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.AcllDirect2)
			{
				if (!AmRpcClientHelper.IsAttemptCopyLastLogsDirect2Supported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.AcllDirect3)
			{
				if (!AmRpcClientHelper.IsAttemptCopyLastLogsDirect3Supported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.MoveAllDatabases)
			{
				if (!AmRpcClientHelper.IsServerMoveAllDatabasesRpcSupported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.MoveEx2)
			{
				if (!AmRpcClientHelper.IsMoveDatabaseEx2RpcSupported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.MoveEx3 || rpcOperationHint == AmRpcOperationHint.MoveAllDatabases2)
			{
				if (!AmRpcClientHelper.IsMoveDatabaseEx3RpcSupported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.MountWithAmFlags)
			{
				if (!AmRpcClientHelper.IsMountWithAmFlagsRpcSupported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.ReportServiceKill)
			{
				if (!AmRpcClientHelper.IsReportServiceKillRpcSupported(serverName))
				{
					result = false;
				}
			}
			else if (rpcOperationHint == AmRpcOperationHint.GetDeferredRecoveryEntries && !AmRpcClientHelper.IsGetDeferredRecoveryEntriesRpcSupported(serverName))
			{
				result = false;
			}
			return result;
		}

		internal static void RunRpcOperation(AmRpcOperationHint rpcOperationHint, string serverName, int? timeoutMs, AmRpcClientHelper.InternalRpcOperation rpcOperation)
		{
			AmRpcClientHelper.RunRpcOperation(rpcOperationHint, serverName, null, timeoutMs, rpcOperation);
		}

		private static void RunServerRpcWithReferral(AmRpcOperationHint rpcOperationHint, string serverToRpc, int? timeoutMs, AmRpcClientHelper.InternalRpcOperation rpcOperation)
		{
			if (serverToRpc == null)
			{
				throw new AmFailedToFindSuitableServer();
			}
			for (int i = 0; i < 2; i++)
			{
				RpcErrorExceptionInfo errorInfo = null;
				AmRpcExceptionWrapper.Instance.ClientRetryableOperation(serverToRpc, delegate
				{
					using (AmRpcClient amRpcClient = AmRpcClientHelper.GetAmRpcClient(rpcOperationHint, serverToRpc, timeoutMs))
					{
						errorInfo = rpcOperation(amRpcClient, serverToRpc);
					}
				});
				try
				{
					AmRpcExceptionWrapper.Instance.ClientRethrowIfFailed(serverToRpc, errorInfo);
					break;
				}
				catch (AmReferralException ex)
				{
					if (i != 0)
					{
						throw;
					}
					serverToRpc = ex.ReferredServer;
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<AmRpcOperationHint, string>(0L, "Got referral server while trying {0}: {1}", rpcOperationHint, serverToRpc);
				}
			}
		}

		private static void RunDatabaseRpcWithReferral(AmRpcOperationHint rpcOperationHint, IADDatabase database, string targetServer, AmRpcClientHelper.InternalRpcOperation rpcOperation)
		{
			string serverToRpc = AmRpcClientHelper.GetServerToPerformDatabaseOperation(database, targetServer);
			if (serverToRpc == null)
			{
				throw new AmFailedToFindSuitableServer();
			}
			for (int i = 0; i < 2; i++)
			{
				RpcErrorExceptionInfo errorInfo = null;
				AmRpcExceptionWrapper.Instance.ClientRetryableOperation(serverToRpc, delegate
				{
					using (AmRpcClient amRpcClient = AmRpcClientHelper.GetAmRpcClient(rpcOperationHint, serverToRpc, new int?(0)))
					{
						errorInfo = rpcOperation(amRpcClient, serverToRpc);
					}
				});
				try
				{
					AmRpcExceptionWrapper.Instance.ClientRethrowIfFailed(database.Name, serverToRpc, errorInfo);
					break;
				}
				catch (AmReferralException ex)
				{
					if (i != 0)
					{
						throw;
					}
					serverToRpc = ex.ReferredServer;
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<AmRpcOperationHint, string>(0L, "Got referral server while trying {0}: {1}", rpcOperationHint, serverToRpc);
				}
			}
		}

		private static void RunRpcOperation(AmRpcOperationHint rpcOperationHint, string serverName, string databaseName, int? timeoutMs, AmRpcClientHelper.InternalRpcOperation rpcOperation)
		{
			AmRpcClientHelper.RunRpcOperationWithAuth(rpcOperationHint, serverName, databaseName, null, timeoutMs, rpcOperation);
		}

		private static void RunRpcOperationWithAuth(AmRpcOperationHint rpcOperationHint, string serverName, string databaseName, NetworkCredential networkCredential, int? timeoutMs, AmRpcClientHelper.InternalRpcOperation rpcOperation)
		{
			RpcErrorExceptionInfo errorInfo = null;
			AmRpcExceptionWrapper.Instance.ClientRetryableOperation(serverName, delegate
			{
				using (AmRpcClient amRpcClient = AmRpcClientHelper.GetAmRpcClient(rpcOperationHint, serverName, networkCredential, timeoutMs))
				{
					errorInfo = rpcOperation(amRpcClient, serverName);
				}
			});
			AmRpcExceptionWrapper.Instance.ClientRethrowIfFailed(databaseName, serverName, errorInfo);
		}

		private static IADToplogyConfigurationSession GetADSession()
		{
			return ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
		}

		private static IADServer GetMiniServer(string serverFqdn, IADToplogyConfigurationSession adSession)
		{
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(serverFqdn);
			return adSession.FindMiniServerByName(nodeNameFromFqdn);
		}

		private static ServerVersion GetServerVersion(string serverFqdn)
		{
			IADToplogyConfigurationSession adsession = AmRpcClientHelper.GetADSession();
			IADServer miniServer = AmRpcClientHelper.GetMiniServer(serverFqdn, adsession);
			if (miniServer == null)
			{
				throw new AmServerNotFoundToVerifyRpcVersion(serverFqdn);
			}
			return miniServer.AdminDisplayVersion;
		}

		private const string RegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Client";

		private const string WcfTimeoutRegKey = "WCFTimeoutInMsec";

		private const string WcfTimeoutLongRegKey = "WCFLongTimeoutInMsec";

		private const string WcfTimeoutReceiveRegKey = "WCFReceiveTimeoutInMsec";

		private const string RpcTimeoutRegKey = "RPCTimeoutInMsec";

		private const string RpcTimeoutAcllShortRegKey = "RPCTimeoutAcllShortInMsec";

		private const string LogDiagnosticEventsRegKey = "LogActiveManagerClientDiagnosticEvents";

		private const string WcfCleanupTimeRegKey = "WCFCleanupTimeInMsec";

		internal const long InvalidLogGeneration = -1L;

		internal const int RpcTimeoutDefault = 5000;

		internal const int RpcTimeoutInfinite = 0;

		private const int ActiveManagerIdlVersion3Build = 1937801646;

		private const int MaxRetryCount = 2;

		private static int? m_rpcTimeout = null;

		private static int? m_rpcTimeoutAcllShort = null;

		private static TimeSpan? m_wcfTimeout = null;

		private static TimeSpan wcfTimeoutDefault = TimeSpan.FromSeconds(3.0);

		private static TimeSpan? m_wcfTimeoutLong = null;

		private static TimeSpan wcfTimeoutLongDefault = TimeSpan.FromSeconds(5.0);

		private static TimeSpan? m_wcfTimeoutReceive = null;

		private static TimeSpan wcfTimeoutReceiveDefault = TimeSpan.FromMinutes(5.0);

		private static bool? m_logDiagnosticEvents = null;

		private static TimeSpan? m_wcfCleanupTime = null;

		private static TimeSpan wcfCleanupTimeDefault = TimeSpan.FromMinutes(30.0);

		private static long s_getServerForDatabaseCalls;

		private static long s_getServerForDatabaseErrs;

		internal delegate RpcErrorExceptionInfo InternalRpcOperation(AmRpcClient rpcClient, string serverName);
	}
}
