using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Rpc.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmRpcServer : AmRpcServerBase
	{
		public static object Locker
		{
			get
			{
				return AmRpcServer.m_locker;
			}
		}

		public static bool Started
		{
			get
			{
				return AmRpcServer.m_fRpcServerStarted;
			}
		}

		public static bool TryStart(ActiveManagerCore amInstance)
		{
			bool fRpcServerStarted;
			lock (AmRpcServer.m_locker)
			{
				if (!AmRpcServer.m_fRpcServerStarted)
				{
					AmTrace.Debug("Starting Active Manager Rpc", new object[0]);
					AmRpcServer.m_amInstance = amInstance;
					try
					{
						FileSecurity activeManagerRpcSecurity = Microsoft.Exchange.Cluster.Replay.ObjectSecurity.ActiveManagerRpcSecurity;
						if (activeManagerRpcSecurity != null)
						{
							AmRpcServer.m_rpcServer = (AmRpcServer)RpcServerBase.RegisterServer(typeof(AmRpcServer), activeManagerRpcSecurity, 1, false, (uint)RegistryParameters.MaximumRpcThreadCount);
							if (AmRpcServer.m_rpcServer != null)
							{
								AmTrace.Debug("Active Manager Rpc server successfully started.", new object[0]);
								AmRpcServer.m_fRpcServerStarted = true;
							}
						}
						if (AmRpcServer.m_rpcServer == null)
						{
							AmTrace.Error("Active Manager Rpc server failed to start!", new object[0]);
							ReplayEventLogConstants.Tuple_AmRpcServerFailedToStart.LogEvent(null, new object[0]);
						}
						goto IL_1BF;
					}
					catch (RpcException ex)
					{
						AmTrace.Error("RPC Error occurred while trying to register the Active Manager RPC Server. Exception: {0}", new object[]
						{
							ex.ToString()
						});
						ReplayEventLogConstants.Tuple_AmRpcServerFailedToRegister.LogEvent(null, new object[]
						{
							ex.Message
						});
						goto IL_1BF;
					}
					catch (ADTransientException ex2)
					{
						AmTrace.Error("Transient exception occurred while retrieving the ActiveManagerRpcSecurity object. Exception: {0}", new object[]
						{
							ex2.ToString()
						});
						ReplayEventLogConstants.Tuple_AmRpcServerFailedToFindExchangeServersUsg.LogEvent(null, new object[]
						{
							ex2.Message
						});
						goto IL_1BF;
					}
					catch (ADExternalException ex3)
					{
						AmTrace.Error("Start(): Permanent exception occurred while retrieving the ActiveManagerRpcSecurity object. Exception: {0}", new object[]
						{
							ex3.ToString()
						});
						ReplayEventLogConstants.Tuple_AmRpcServerFailedToFindExchangeServersUsg.LogEvent(null, new object[]
						{
							ex3.Message
						});
						goto IL_1BF;
					}
					catch (ADOperationException ex4)
					{
						AmTrace.Error("Start(): Permanent exception occurred while retrieving the ActiveManagerRpcSecurity object. Exception: {0}", new object[]
						{
							ex4.ToString()
						});
						ReplayEventLogConstants.Tuple_AmRpcServerFailedToFindExchangeServersUsg.LogEvent(null, new object[]
						{
							ex4.Message
						});
						goto IL_1BF;
					}
				}
				AmTrace.Debug("Active Manager RPC server already started.", new object[0]);
				IL_1BF:
				fRpcServerStarted = AmRpcServer.m_fRpcServerStarted;
			}
			return fRpcServerStarted;
		}

		public static void Stop()
		{
			lock (AmRpcServer.m_locker)
			{
				AmTrace.Debug("Stopping Active Manager Rpc", new object[0]);
				if (AmRpcServer.m_rpcServer != null)
				{
					RpcServerBase.StopServer(AmRpcServerBase.RpcIntfHandle);
					AmRpcServer.m_rpcServer = null;
					AmRpcServer.m_fRpcServerStarted = false;
				}
			}
		}

		public override RpcErrorExceptionInfo RpcsGetServerForDatabase(Guid guid, ref AmDbStatusInfo2 dbInfo)
		{
			AmDbStatusInfo2 tmpDbInfo = null;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tmpDbInfo = AmRpcServer.m_amInstance.AmServerGetServerForDatabase(guid);
			});
			dbInfo = tmpDbInfo;
			return result;
		}

		public override RpcErrorExceptionInfo MountDatabase(Guid guid, int storeFlags, int amFlags, int mountDialoverride)
		{
			AmTrace.Debug("Mounting database: {0} storeFlags: 0x{1:X} amFlags: 0x{2:X} mountdialoverride: {3}", new object[]
			{
				guid,
				storeFlags,
				amFlags,
				mountDialoverride
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Mount);
				AmRpcServer.m_amInstance.MountDatabase(guid, (MountFlags)storeFlags, (AmMountFlags)amFlags, (DatabaseMountDialOverride)mountDialoverride, actionCode);
			});
		}

		public override RpcErrorExceptionInfo DismountDatabase(Guid guid, int flags)
		{
			AmTrace.Debug("Dismounting database: {0} flags: {1}", new object[]
			{
				guid,
				flags
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Dismount);
				AmRpcServer.m_amInstance.DismountDatabase(guid, (UnmountFlags)flags, actionCode);
			});
		}

		public override RpcErrorExceptionInfo MoveDatabaseEx(Guid guid, int mountFlags, int dismountFlags, int mountDialOverride, string fromServer, string targetServerFqdn, bool tryOtherHealthyServers, int skipValidationChecks, int iActionCode, string moveComment, ref AmDatabaseMoveResult databaseMoveResult)
		{
			AmTrace.Debug("Moving database: {0} flags: {1} mountdialoverride: {2} targetserver: {3}", new object[]
			{
				guid,
				mountFlags,
				mountDialOverride,
				targetServerFqdn
			});
			if (ActiveManagerUtil.IsNullEncoded(fromServer))
			{
				fromServer = null;
			}
			if (ActiveManagerUtil.IsNullEncoded(targetServerFqdn))
			{
				targetServerFqdn = null;
			}
			AmDatabaseMoveResult tempMoveResult = null;
			databaseMoveResult = null;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode;
				if (iActionCode == 4)
				{
					actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Move);
				}
				else if (iActionCode == 7)
				{
					actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
				}
				else
				{
					actionCode = new AmDbActionCode(iActionCode);
				}
				AmRpcServer.m_amInstance.MoveDatabase(guid, (MountFlags)mountFlags, (UnmountFlags)dismountFlags, (DatabaseMountDialOverride)mountDialOverride, new AmServerName(fromServer), new AmServerName(targetServerFqdn), tryOtherHealthyServers, (AmBcsSkipFlags)skipValidationChecks, actionCode, moveComment, ref tempMoveResult);
			});
			databaseMoveResult = tempMoveResult;
			return result;
		}

		public override RpcErrorExceptionInfo GetPrimaryActiveManager(ref AmPamInfo pamInfo)
		{
			AmPamInfo tmpPamInfo = null;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tmpPamInfo = AmRpcServer.m_amInstance.GetPrimaryActiveManager();
			});
			pamInfo = tmpPamInfo;
			return result;
		}

		public override RpcErrorExceptionInfo GetActiveManagerRole(ref AmRole amRole, ref string errorMessage)
		{
			AmRole tmpAmRole = AmRole.Unknown;
			string tmpMsg = string.Empty;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tmpAmRole = AmRpcServer.m_amInstance.GetActiveManagerRole(out tmpMsg);
			});
			amRole = tmpAmRole;
			errorMessage = tmpMsg;
			return result;
		}

		public override RpcErrorExceptionInfo CheckThirdPartyListener(ref bool healthy, ref string errorMessage)
		{
			healthy = false;
			bool tempHealthy = false;
			string tmpMsg = string.Empty;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tempHealthy = ThirdPartyManager.Instance.CheckHealth(out tmpMsg);
			});
			errorMessage = tmpMsg;
			healthy = tempHealthy;
			return result;
		}

		public override RpcErrorExceptionInfo AttemptCopyLastLogsDirect(Guid guid, int mountDialOverride, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, string sourceServer, int iActionCode, int skipValidationChecks, bool mountPending, string uniqueOperationId, int subactionAttemptNumber, ref AmAcllReturnStatus acllStatus)
		{
			AmTrace.Debug("Running ACLL for database (on behalf of PAM): {0}", new object[]
			{
				guid
			});
			AmAcllReturnStatus tempAcllStatus = null;
			acllStatus = null;
			AmAcllArgs acllArgs = new AmAcllArgs();
			acllArgs.NumRetries = numRetries;
			acllArgs.E00TimeoutMs = e00timeoutMs;
			acllArgs.NetworkIOTimeoutMs = networkIOtimeoutMs;
			acllArgs.NetworkConnectTimeoutMs = networkConnecttimeoutMs;
			acllArgs.SourceServer = new AmServerName(sourceServer);
			acllArgs.ActionCode = new AmDbActionCode(iActionCode);
			acllArgs.SkipValidationChecks = (AmBcsSkipFlags)skipValidationChecks;
			acllArgs.MountPending = mountPending;
			acllArgs.MountDialOverride = (DatabaseMountDialOverride)mountDialOverride;
			acllArgs.UniqueOperationId = uniqueOperationId;
			acllArgs.SubactionAttemptNumber = subactionAttemptNumber;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.AttemptCopyLastLogsDirect(guid, acllArgs, ref tempAcllStatus);
			});
			acllStatus = tempAcllStatus;
			return result;
		}

		public override RpcErrorExceptionInfo MountDatabaseDirect(Guid guid, AmMountArg mountArg)
		{
			AmTrace.Debug("Mounting database (on behalf of PAM): {0}", new object[]
			{
				guid
			});
			RpcErrorExceptionInfo rpcErrorExceptionInfo = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode = new AmDbActionCode(mountArg.Reason);
				AmRpcServer.m_amInstance.MountDatabaseDirect(guid, (MountFlags)mountArg.StoreFlags, (AmMountFlags)mountArg.AmMountFlags, actionCode);
			});
			if (rpcErrorExceptionInfo.IsFailed())
			{
				ReplayCrimsonEvents.MountDirectFailed.Log<Guid, string>(guid, rpcErrorExceptionInfo.ErrorMessage);
			}
			return rpcErrorExceptionInfo;
		}

		public override RpcErrorExceptionInfo DismountDatabaseDirect(Guid guid, AmDismountArg dismountArg)
		{
			AmTrace.Debug("Dismounting database: {0}", new object[]
			{
				guid
			});
			RpcErrorExceptionInfo rpcErrorExceptionInfo = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode = new AmDbActionCode(dismountArg.Reason);
				AmRpcServer.m_amInstance.DismountDatabaseDirect(guid, (UnmountFlags)dismountArg.Flags, actionCode);
			});
			if (rpcErrorExceptionInfo.IsFailed())
			{
				ReplayCrimsonEvents.DismountDirectFailed.Log<Guid, string>(guid, rpcErrorExceptionInfo.ErrorMessage);
			}
			return rpcErrorExceptionInfo;
		}

		public override RpcErrorExceptionInfo ServerSwitchOver(string sourceServer)
		{
			AmTrace.Debug("Request ServerSwitchOver from {0}", new object[]
			{
				sourceServer
			});
			ReplayCrimsonEvents.SwitchoverInitiated.Log<string>(sourceServer);
			RpcErrorExceptionInfo rpcErrorExceptionInfo = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.ServerSwitchOver(new AmServerName(sourceServer));
			});
			if (rpcErrorExceptionInfo.IsFailed())
			{
				ReplayCrimsonEvents.SwitchoverFailed.Log<string, string>(sourceServer, rpcErrorExceptionInfo.ErrorMessage);
			}
			else
			{
				ReplayCrimsonEvents.SwitchoverSuccess.Log<string>(sourceServer);
			}
			return rpcErrorExceptionInfo;
		}

		public override RpcErrorExceptionInfo ServerMoveAllDatabases(string sourceServer, string targetServer, int mountFlags, int dismountFlags, int mountDialOverride, int tryOtherHealthyServers, int skipValidationChecks, int actionCode, string moveComment, string componentName, ref List<AmDatabaseMoveResult> databaseMoveResults)
		{
			AmTrace.Debug("Request ServerMoveAllDatabases from {0}", new object[]
			{
				sourceServer
			});
			List<AmDatabaseMoveResult> tempMoveResults = null;
			AmServerName sourceServerName = new AmServerName(sourceServer);
			AmServerName targetServerName = new AmServerName(targetServer);
			if (string.IsNullOrEmpty(moveComment))
			{
				moveComment = ReplayStrings.AmBcsNoneSpecified;
			}
			RpcErrorExceptionInfo rpcErrorExceptionInfo = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tempMoveResults = AmRpcServer.m_amInstance.ServerMoveAllDatabases(sourceServerName, targetServerName, (MountFlags)mountFlags, (UnmountFlags)dismountFlags, (DatabaseMountDialOverride)mountDialOverride, Convert.ToBoolean(tryOtherHealthyServers), (AmBcsSkipFlags)skipValidationChecks, new AmDbActionCode(actionCode), moveComment, componentName);
			});
			databaseMoveResults = tempMoveResults;
			if (rpcErrorExceptionInfo.IsFailed())
			{
				ReplayCrimsonEvents.ServerMoveAllDatabasesFailed.Log<string, string, string>(sourceServer, rpcErrorExceptionInfo.ErrorMessage, moveComment);
			}
			else
			{
				IEnumerable<AmDatabaseMoveResult> source = from moveResult in databaseMoveResults
				where moveResult.MoveStatus != AmDbMoveStatus.Succeeded
				select moveResult;
				int num = source.Count<AmDatabaseMoveResult>();
				AmTrace.Debug("ServerMoveAllDatabases from {0} had {1} failed database moves.", new object[]
				{
					sourceServer,
					num
				});
				if (num > 0)
				{
					ReplayCrimsonEvents.ServerMoveAllDatabasesFailed.Log<string, LocalizedString, string>(sourceServer, ReplayStrings.ServerMoveAllDatabasesFailed(num), moveComment);
				}
			}
			return rpcErrorExceptionInfo;
		}

		public override RpcErrorExceptionInfo IsRunning()
		{
			return new RpcErrorExceptionInfo();
		}

		public override RpcErrorExceptionInfo ReportSystemEvent(int eventCode, string reportingServer)
		{
			AmTrace.Debug("ReportSystemEvent request received (code={0}, reportingServer={1})", new object[]
			{
				eventCode,
				reportingServer
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.ReportSystemEvent((AmSystemEventCode)eventCode, reportingServer);
			});
		}

		public override RpcErrorExceptionInfo AmRefreshConfiguration(int refreshFlags, int maxSecondsToWait)
		{
			AmTrace.Debug("AmRefreshConfiguration request received (flags = {0}, timeout={1})", new object[]
			{
				refreshFlags,
				maxSecondsToWait
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.AmRefreshConfiguration((AmRefreshConfigurationFlags)refreshFlags, maxSecondsToWait);
			});
		}

		public override RpcErrorExceptionInfo RemountDatabase(Guid guid, int mountFlags, int mountDialOverride, string fromServer)
		{
			AmTrace.Debug("Remounting database: {0} flags: {1} mountdialoverride: {2} fromServer: {3}", new object[]
			{
				guid,
				mountFlags,
				mountDialOverride,
				fromServer
			});
			if (ActiveManagerUtil.IsNullEncoded(fromServer))
			{
				fromServer = null;
			}
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.FailureItem, AmDbActionCategory.Remount);
				AmRpcServer.m_amInstance.RemountDatabase(guid, (MountFlags)mountFlags, (DatabaseMountDialOverride)mountDialOverride, new AmServerName(fromServer), actionCode);
			});
		}

		public override RpcErrorExceptionInfo RpcsGetAutomountConsensusState(ref int automountConsensusState)
		{
			MommyMayIAutomount dummy = MommyMayIAutomount.InsufficientInformationDoNotMount;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				if (AmSystemManager.Instance.ConfigManager.MommyMayIMount)
				{
					dummy = MommyMayIAutomount.SomebodyGrantsPermission;
					return;
				}
				dummy = MommyMayIAutomount.InsufficientInformationDoNotMount;
			});
			AmTrace.Debug("Retrieved the DAC consensus state as {0}.", new object[]
			{
				dummy
			});
			automountConsensusState = (int)dummy;
			return result;
		}

		public override RpcErrorExceptionInfo RpcsSetAutomountConsensusState(int automountConsensusState)
		{
			AmTrace.Debug("Setting the DAC consensus state to {0}.", new object[]
			{
				automountConsensusState
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				if (automountConsensusState == 0)
				{
					AmSystemManager.Instance.ConfigManager.SetCurfue();
					AmSystemManager.Instance.ConfigManager.ProhibitAutoMount("Called from RPC");
					return;
				}
				AmSystemManager.Instance.ConfigManager.AllowAutoMount("Called from RPC");
			});
		}

		public override RpcErrorExceptionInfo ReportServiceKill(string serviceName, string serverName, string timeStampStrInUtc)
		{
			AmTrace.Debug("ReportServiceKill called. (serviceName = {0}, serverName = {1}, timeStampStrInUtc = {2})", new object[]
			{
				serviceName,
				serverName,
				timeStampStrInUtc
			});
			return AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.ReportServiceKill(serviceName, new AmServerName(serverName), ExDateTime.Parse(timeStampStrInUtc));
			});
		}

		public override RpcErrorExceptionInfo GetDeferredRecoveryEntries(ref List<AmDeferredRecoveryEntry> entries)
		{
			AmTrace.Debug("Entering GetDeferredRecoveryEntries", new object[0]);
			List<AmDeferredRecoveryEntry> tempEntries = null;
			RpcErrorExceptionInfo rpcErrorExceptionInfo = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				tempEntries = AmRpcServer.m_amInstance.GetDeferredRecoveryEntries();
			});
			entries = tempEntries;
			AmTrace.Debug("Exiting GetDeferredRecoveryEntries (EntriesFound={0} Error={1})", new object[]
			{
				(entries != null) ? entries.Count.ToString() : "<null>",
				rpcErrorExceptionInfo.IsFailed() ? rpcErrorExceptionInfo.ErrorMessage : "<none>"
			});
			return rpcErrorExceptionInfo;
		}

		public override RpcErrorExceptionInfo GenericRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGenericReplyInfo tmpReplyInfo = null;
			RpcErrorExceptionInfo result = AmRpcExceptionWrapper.Instance.RunRpcServerOperation(delegate()
			{
				AmRpcServer.m_amInstance.GenericRpcDispatch(requestInfo, ref tmpReplyInfo);
			});
			if (tmpReplyInfo != null)
			{
				replyInfo = tmpReplyInfo;
			}
			return result;
		}

		private static AmRpcServer m_rpcServer = null;

		private static ActiveManagerCore m_amInstance = null;

		private static bool m_fRpcServerStarted = false;

		private static object m_locker = new object();

		private delegate void AmOperation();
	}
}
