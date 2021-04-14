using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceManager : IReplicaInstanceManager
	{
		public ReplicaInstanceManager(DumpsterRedeliveryManager dumpsterManager, SkippedLogsDeleter logDeleter)
		{
			this.DumpsterManagerInstance = dumpsterManager;
			this.SkippedLogsDeleter = logDeleter;
			this.m_replicaInstances = new Dictionary<Guid, ReplicaInstanceContainer>(48);
			this.ActiveManagerInstance = ActiveManager.CreateCustomActiveManager(false, this.AdLookup.DagLookup, this.AdLookup.ServerLookup, this.AdLookup.MiniServerLookup, null, null, this.AdLookup.DatabaseLookup, null, true);
			ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplicaInstanceManager is instantiated.");
		}

		public SeedManager SeedManagerInstance { get; set; }

		public IReplayAdObjectLookup AdLookup
		{
			get
			{
				return Dependencies.ReplayAdObjectLookup;
			}
		}

		public IADConfig ADConfig
		{
			get
			{
				return Dependencies.ADConfig;
			}
		}

		private bool IsStopping { get; set; }

		private bool IsFinalShutdownSequence { get; set; }

		private DumpsterRedeliveryManager DumpsterManagerInstance { get; set; }

		private SkippedLogsDeleter SkippedLogsDeleter { get; set; }

		private ActiveManager ActiveManagerInstance { get; set; }

		public void PrepareToStop()
		{
			lock (this.m_locker)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplicaInstanceManager: PrepareToStop called.");
				this.IsStopping = true;
				this.DisableQueues(this.m_replicaInstances.Keys);
			}
		}

		public void Stop()
		{
			this.DisableQueuesAndRemoveInstances(this.m_replicaInstances.Keys, true);
			this.m_firstFullScanCompleted.Close();
			if (this.ActiveManagerInstance != null)
			{
				this.ActiveManagerInstance.Dispose();
				this.ActiveManagerInstance = null;
			}
		}

		public bool TryWaitForFirstFullConfigUpdater()
		{
			TimeSpan timeout = TimeSpan.FromMilliseconds((double)RegistryParameters.GetMailboxDatabaseCopyStatusRPCTimeoutInMSec);
			return this.m_firstFullScanCompleted.WaitOne(timeout) == ManualOneShotEvent.Result.Success;
		}

		public void ConfigurationUpdater(Guid? databaseGuid, bool waitForRestarts, bool isHighPriority, bool forceRestart, ReplayConfigChangeHints changeHint)
		{
			ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigurationUpdater( {0} ) call is entering. changeHint={1}, isHighPriority={2}, waitForRestarts={3}", new object[]
			{
				(databaseGuid != null) ? databaseGuid.ToString() : "<null>",
				changeHint,
				isHighPriority,
				waitForRestarts
			});
			lock (this.m_locker)
			{
				if (this.IsStopping)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigurationUpdater( {0} ) call is exiting due to service stop.", (databaseGuid != null) ? databaseGuid.ToString() : "<null>");
					throw new ReplayServiceShuttingDownException();
				}
				this.m_fConfigUpdaterRunning = true;
			}
			Exception ex = null;
			try
			{
				this.ConfigurationUpdaterInternal(databaseGuid, waitForRestarts, isHighPriority, forceRestart, changeHint);
			}
			catch (OperationAbortedException innerException)
			{
				throw new ReplayServiceShuttingDownException(innerException);
			}
			catch (ADPossibleOperationException ex2)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<ADPossibleOperationException>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater exiting with ADPossibleOperationException:{0}", ex2);
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater exiting with ADTransientException:{0}", ex3);
				ex = ex3;
			}
			catch (TransientException ex4)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<TransientException>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater exiting with TransientException:{0}", ex4);
				ex = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater exiting with DataSourceOperationException:{0}", ex5);
				ex = ex5;
			}
			catch (DataValidationException ex6)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater exiting with DataValidationException:{0}", ex6);
				ex = ex6;
			}
			finally
			{
				if (changeHint == ReplayConfigChangeHints.PeriodicFullScan || changeHint == ReplayConfigChangeHints.RunConfigUpdaterRpc)
				{
					this.m_firstFullScanCompleted.Set();
				}
				lock (this.m_locker)
				{
					this.m_fConfigUpdaterRunning = false;
				}
			}
			ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigUpdater done.");
			if (ex != null)
			{
				ReplayConfigurationHelper.ThrowDbOperationWrapperExceptionIfNecessary(ex);
			}
		}

		public void RestartInstance(RestartInstanceWrapper instanceToRestart)
		{
			try
			{
				this.RestartInstanceInternal(instanceToRestart);
			}
			catch (OperationAbortedException innerException)
			{
				throw new ReplayServiceShuttingDownException(innerException);
			}
		}

		public bool TryGetReplicaInstance(Guid guid, out ReplicaInstance instance)
		{
			instance = null;
			bool result;
			lock (this.m_locker)
			{
				ReplicaInstanceContainer replicaInstanceContainer;
				if (this.m_replicaInstances.TryGetValue(guid, out replicaInstanceContainer))
				{
					instance = replicaInstanceContainer.ReplicaInstance;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool TryGetReplicaInstanceContainer(Guid guid, out ReplicaInstanceContainer instance)
		{
			instance = null;
			bool result;
			lock (this.m_locker)
			{
				result = this.m_replicaInstances.TryGetValue(guid, out instance);
			}
			return result;
		}

		public List<ReplicaInstance> GetAllReplicaInstances()
		{
			List<ReplicaInstance> list = null;
			lock (this.m_locker)
			{
				list = new List<ReplicaInstance>(this.m_replicaInstances.Count);
				foreach (ReplicaInstanceContainer replicaInstanceContainer in this.m_replicaInstances.Values)
				{
					list.Add(replicaInstanceContainer.ReplicaInstance);
				}
			}
			return list;
		}

		public int GetRICount()
		{
			return this.m_replicaInstances.Count;
		}

		public List<ReplicaInstanceContainer> GetAllReplicaInstanceContainers()
		{
			List<ReplicaInstanceContainer> list = null;
			lock (this.m_locker)
			{
				list = new List<ReplicaInstanceContainer>(this.m_replicaInstances.Count);
				foreach (ReplicaInstanceContainer item in this.m_replicaInstances.Values)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public ISetVolumeInfo GetVolumeInfoCallback(Guid instanceGuid, bool activePassiveAgnostic = false)
		{
			ReplicaInstanceContext replicaInstanceContext;
			if (activePassiveAgnostic)
			{
				replicaInstanceContext = this.GetCurrentTargetOrSourceReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.AutoReseed, true);
			}
			else
			{
				replicaInstanceContext = this.GetCurrentReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.AutoReseed, true);
			}
			if (replicaInstanceContext != null)
			{
				return replicaInstanceContext;
			}
			return null;
		}

		public ISetSeeding GetSeederStatusCallback(Guid instanceGuid)
		{
			ReplicaInstanceContext currentReplicaInstanceContext = this.GetCurrentReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.DbSeeder, true);
			if (currentReplicaInstanceContext != null)
			{
				return currentReplicaInstanceContext;
			}
			return null;
		}

		public ISetPassiveSeeding GetPassiveSeederStatusCallback(Guid instanceGuid)
		{
			ReplicaInstanceContext currentReplicaInstanceContext = this.GetCurrentReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.DbSeeder, false);
			if (currentReplicaInstanceContext != null)
			{
				return currentReplicaInstanceContext;
			}
			return null;
		}

		public ISetActiveSeeding GetActiveSeederStatusCallback(Guid instanceGuid)
		{
			ReplicaInstanceContext currentSourceReplicaInstanceContext = this.GetCurrentSourceReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.DbSeeder, false);
			if (currentSourceReplicaInstanceContext != null)
			{
				return currentSourceReplicaInstanceContext;
			}
			return null;
		}

		public ISetGeneration GetSetGenerationCallback(Guid instanceGuid)
		{
			ReplicaInstanceContext currentReplicaInstanceContext = this.GetCurrentReplicaInstanceContext(instanceGuid, ReplayConfigChangeHints.DbSeeder, true);
			if (currentReplicaInstanceContext != null)
			{
				return currentReplicaInstanceContext;
			}
			return null;
		}

		public IGetStatus GetStatusRetrieverCallback(Guid instanceGuid)
		{
			ReplicaInstance replicaInstance = null;
			IGetStatus result = null;
			if (this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				if (replicaInstance.Configuration.Type != ReplayConfigType.RemoteCopyTarget)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, ReplayConfigType>((long)this.GetHashCode(), "ReplicaInstanceManager: GetStatusRetrieverCallback ({0}) could not find a running target RI. An RI of type '{1}' is currently running. Returning <null>.", instanceGuid, replicaInstance.Configuration.Type);
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, ReplayConfigType>((long)this.GetHashCode(), "ReplicaInstanceManager: GetStatusRetrieverCallback ({0}) returned an RI which has config type of '{1}'.", instanceGuid, replicaInstance.Configuration.Type);
					result = replicaInstance.CurrentContext;
				}
			}
			else
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "ReplicaInstanceManager: GetStatusRetrieverCallback ({0}) could not find a running RI. Returning <null>.", instanceGuid);
			}
			return result;
		}

		public bool GetRunningInstanceSignatureAndCheckpoint(Guid instanceGuid, out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp)
		{
			logfileSignature = null;
			lowestGenerationRequired = 0L;
			highestGenerationRequired = 0L;
			lastGenerationBackedUp = 0L;
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				JET_SIGNATURE? jet_SIGNATURE;
				bool signatureAndCheckpoint = replicaInstance.GetSignatureAndCheckpoint(out jet_SIGNATURE, out lowestGenerationRequired, out highestGenerationRequired, out lastGenerationBackedUp);
				if (signatureAndCheckpoint)
				{
					if (jet_SIGNATURE.Value.ulRandom == 0U && !jet_SIGNATURE.Value.logtimeCreate.HasValue)
					{
						return false;
					}
					logfileSignature = new JET_SIGNATURE?(new JET_SIGNATURE((int)jet_SIGNATURE.Value.ulRandom, jet_SIGNATURE.Value.logtimeCreate.ToDateTime(), null));
					return true;
				}
			}
			return false;
		}

		public CopyStatusEnum GetCopyStatus(Guid instanceGuid)
		{
			CopyStatusEnum result = CopyStatusEnum.Unknown;
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				result = replicaInstance.GetCopyStatus(RpcGetDatabaseCopyStatusFlags2.ReadThrough).CopyStatus;
			}
			return result;
		}

		public ReplayState GetRunningInstanceState(Guid instanceGuid)
		{
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				return replicaInstance.Configuration.ReplayState;
			}
			return null;
		}

		public bool CreateTempLogFileForRunningInstance(Guid instanceGuid)
		{
			bool result = false;
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				Exception ex;
				result = ReplicaInstance.CreateTempLogFile(replicaInstance.Configuration, out ex);
			}
			return result;
		}

		public AmAcllReturnStatus AmAttemptCopyLastLogsCallback(Guid mdbGuid, AmAcllArgs acllArgs)
		{
			if (mdbGuid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("mdbGuid");
			}
			AmAcllReturnStatus amAcllReturnStatus = new AmAcllReturnStatus();
			AmDbActionCode actionCode = acllArgs.ActionCode;
			bool mountPending = acllArgs.MountPending;
			AmServerName sourceServer = acllArgs.SourceServer;
			DatabaseMountDialOverride mountDialOverride = acllArgs.MountDialOverride;
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, AmDbActionCode>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback: called for DB '{0}' and ActionCode '{1}'.", mdbGuid, actionCode);
			if (AmServerName.IsNullOrEmpty(sourceServer))
			{
				amAcllReturnStatus.NoLoss = true;
				amAcllReturnStatus.MountAllowed = true;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, AmAcllReturnStatus>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback: DB '{0}' has not been mounted before. Returning status: {1}", mdbGuid, amAcllReturnStatus);
				return amAcllReturnStatus;
			}
			if (AmServerName.IsEqual(sourceServer, AmServerName.LocalComputerName))
			{
				amAcllReturnStatus.NoLoss = true;
				amAcllReturnStatus.MountAllowed = true;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, AmAcllReturnStatus>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback: DB '{0}' is SourceServer = self. Returning status: {1}", mdbGuid, amAcllReturnStatus);
				return amAcllReturnStatus;
			}
			Exception innerException;
			TargetReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<TargetReplicaInstance>(mdbGuid, ReplayConfigChangeHints.AmAttemptCopyLastLogs, sourceServer, out innerException, true, -1);
			if (runningReplicaInstance != null)
			{
				ReplayConfiguration configuration = runningReplicaInstance.Configuration;
				AcllPerformanceTracker acllPerformanceTracker = new AcllPerformanceTracker(runningReplicaInstance.Configuration, acllArgs.UniqueOperationId, acllArgs.SubactionAttemptNumber);
				ReplicaInstanceAttemptCopyLastLogs operation = new ReplicaInstanceAttemptCopyLastLogs(acllArgs, acllPerformanceTracker, runningReplicaInstance);
				ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
				this.EnqueueReplicaInstanceOperation(operation, true, out replicaInstanceQueuedItem);
				ReplicaInstanceAttemptCopyLastLogs replicaInstanceAttemptCopyLastLogs = (ReplicaInstanceAttemptCopyLastLogs)replicaInstanceQueuedItem;
				try
				{
					replicaInstanceQueuedItem.Wait();
				}
				finally
				{
					acllPerformanceTracker.RecordDuration(AcllTimedOperation.AcllQueuedOpStart, replicaInstanceAttemptCopyLastLogs.StartTimeDuration);
					acllPerformanceTracker.RecordDuration(AcllTimedOperation.AcllQueuedOpExecution, replicaInstanceAttemptCopyLastLogs.ExecutionDuration);
					acllPerformanceTracker.LogEvent();
				}
				amAcllReturnStatus = replicaInstanceAttemptCopyLastLogs.AcllStatus;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, AmAcllReturnStatus>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback(): DB '{0}': Returning: {1}", mdbGuid, amAcllReturnStatus);
				if (!amAcllReturnStatus.MountAllowed)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback(): DB '{0}': Blocking database mount.", mdbGuid);
				}
				return amAcllReturnStatus;
			}
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, AmServerName>((long)this.GetHashCode(), "AmAttemptCopyLastLogsCallback(): Could not find a running target instance for DB {0} with source server '{1}'.", mdbGuid, sourceServer);
			throw new AmDbAcllErrorNoReplicaInstance(mdbGuid.ToString(), AmServerName.LocalComputerName.NetbiosName, innerException);
		}

		public void AmPreMountCallback(Guid mdbGuid, ref int storeMountFlags, AmMountFlags amMountFlags, MountDirectPerformanceTracker mountPerf, out LogStreamResetOnMount logReset, out ReplicaInstanceContext replicaInstanceContext)
		{
			logReset = null;
			replicaInstanceContext = null;
			if (mdbGuid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("mdbGuid");
			}
			ReplicaInstance instance = null;
			Exception exception = null;
			mountPerf.RunTimedOperation(MountDatabaseDirectOperation.GetRunningReplicaInstance, delegate
			{
				instance = this.GetRunningReplicaInstance<ReplicaInstance>(mdbGuid, ReplayConfigChangeHints.AmPreMountCallback, null, out exception, true, RegistryParameters.LogShipACLLTimeoutInMsec);
			});
			if (instance != null)
			{
				replicaInstanceContext = instance.CurrentContext;
				ReplicaInstancePreMountCallback replicaInstancePreMountCallback = new ReplicaInstancePreMountCallback(storeMountFlags, amMountFlags, mountPerf, instance);
				ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
				this.EnqueueReplicaInstanceOperation(replicaInstancePreMountCallback, true, out replicaInstanceQueuedItem);
				ReplicaInstancePreMountCallback replicaInstancePreMountCallback2 = (ReplicaInstancePreMountCallback)replicaInstanceQueuedItem;
				try
				{
					replicaInstanceQueuedItem.Wait();
				}
				finally
				{
					mountPerf.RecordDuration(MountDatabaseDirectOperation.PreMountQueuedOpStart, replicaInstancePreMountCallback2.StartTimeDuration);
					mountPerf.RecordDuration(MountDatabaseDirectOperation.PreMountQueuedOpExecution, replicaInstancePreMountCallback2.ExecutionDuration);
				}
				storeMountFlags = replicaInstancePreMountCallback.StoreMountFlags;
				logReset = replicaInstancePreMountCallback2.LogReset;
				ReplayQueuedItemBase restartOperation = replicaInstancePreMountCallback2.RestartOperation;
				return;
			}
			ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "AmPreMountCallback ({0}): RI not found even after ConfigUpdater was run again.", mdbGuid);
			if (exception != null)
			{
				throw new AmPreMountCallbackFailedNoReplicaInstanceErrorException(mdbGuid.ToString(), Environment.MachineName, exception.Message, exception);
			}
			throw new AmPreMountCallbackFailedNoReplicaInstanceException(mdbGuid.ToString(), Environment.MachineName);
		}

		public void DisableReplayLag(Guid guid, ActionInitiatorType actionInitiator, string reason)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.DisableReplayLag, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "DisableReplayLag: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.DisableReplayLagOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(new ReplicaInstanceDisableReplayLag(runningReplicaInstance)
			{
				Reason = reason,
				ActionInitiator = actionInitiator
			}, out replicaInstanceQueuedItem);
			replicaInstanceQueuedItem.Wait();
		}

		public void EnableReplayLag(Guid guid, ActionInitiatorType actionInitiator)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.EnableReplayLag, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "EnableReplayLag: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.EnableReplayLagOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(new ReplicaInstanceEnableReplayLag(runningReplicaInstance)
			{
				ActionInitiator = actionInitiator
			}, out replicaInstanceQueuedItem);
			replicaInstanceQueuedItem.Wait();
		}

		public void RequestSuspend(Guid guid, string suspendComment, DatabaseCopyActionFlags flags, ActionInitiatorType initiator)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.DbSuspendBefore, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "RequestSuspend: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.SuspendOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(new ReplicaInstanceRequestSuspend(runningReplicaInstance)
			{
				SuspendComment = suspendComment,
				Flags = flags,
				ActionInitiator = initiator
			}, out replicaInstanceQueuedItem);
			try
			{
				replicaInstanceQueuedItem.Wait();
			}
			finally
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4150668605U);
				this.TryNotifyChangedReplayConfiguration(guid, false, ReplayConfigChangeHints.DbSuspendAfter);
			}
		}

		public void RequestSuspendAndFail(Guid guid, uint errorEventId, string errorMessage, string suspendComment, bool preserveExistingError, bool blockResume = false, bool blockReseed = false, bool blockInPlaceReseed = false)
		{
			this.RequestSuspendAndFailInternal(guid, true, errorEventId, errorMessage, suspendComment, preserveExistingError, blockResume, blockReseed, blockInPlaceReseed);
		}

		public void RequestSuspendAndFail_SupportApi(Guid guid, bool fSuspendCopy, uint errorEventId, string errorMessage, string suspendComment, bool preserveExistingError)
		{
			this.RequestSuspendAndFailInternal(guid, fSuspendCopy, errorEventId, errorMessage, suspendComment, preserveExistingError, false, false, false);
		}

		public void RequestResume(Guid guid, DatabaseCopyActionFlags flags)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.DbResumeBefore, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "RequestResume: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.ResumeOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(new ReplicaInstanceRequestResume(runningReplicaInstance)
			{
				Flags = flags
			}, out replicaInstanceQueuedItem);
			try
			{
				replicaInstanceQueuedItem.Wait();
			}
			finally
			{
				this.TryNotifyChangedReplayConfiguration(guid, true, ReplayConfigChangeHints.DbResumeAfter);
			}
		}

		public void SyncSuspendResumeState(Guid guid)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.DbSyncSuspendResumeStateBefore, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "SyncSuspendResumeState: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.SyncSuspendResumeOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceSyncSuspendResumeState operation = new ReplicaInstanceSyncSuspendResumeState(runningReplicaInstance);
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(operation, out replicaInstanceQueuedItem);
			replicaInstanceQueuedItem.Wait();
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3089509693U);
		}

		internal void ClearLossyMountRecord(Guid dbGuid)
		{
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(dbGuid, out replicaInstance))
			{
				replicaInstance.Configuration.ReplayState.LastAcllLossAmount = 0L;
			}
		}

		internal void SetWorkerProcessId(Guid dbGuid, int processId)
		{
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(dbGuid, out replicaInstance))
			{
				replicaInstance.Configuration.ReplayState.WorkerProcessId = processId;
			}
		}

		internal void ClearLastAcllRunWithSkipHealthChecksRecord(Guid dbGuid)
		{
			ReplicaInstance replicaInstance;
			if (this.TryGetReplicaInstance(dbGuid, out replicaInstance))
			{
				replicaInstance.Configuration.ReplayState.LastAcllRunWithSkipHealthChecks = false;
			}
		}

		private static Dictionary<string, ReplayConfiguration> ConvertDictionary(Dictionary<Guid, ReplayConfiguration> configs)
		{
			return configs.ToDictionary((KeyValuePair<Guid, ReplayConfiguration> kvp) => SafeInstanceTable<ReplicaInstance>.GetIdentityFromGuid(kvp.Key), (KeyValuePair<Guid, ReplayConfiguration> kvp) => kvp.Value);
		}

		private static List<string> ConvertConfigList(List<Guid> configs)
		{
			List<string> list = new List<string>(configs.Count);
			foreach (Guid guid in configs)
			{
				list.Add(SafeInstanceTable<MonitoredDatabase>.GetIdentityFromGuid(guid));
			}
			return list;
		}

		private void ConfigurationUpdaterInternal(Guid? databaseGuid, bool waitForRestarts, bool isHighPriority, bool forceRestart, ReplayConfigChangeHints changeHint)
		{
			Dictionary<Guid, ReplayConfiguration> dictionary;
			List<Guid> list;
			List<KeyValuePair<IADDatabase, Exception>> failedConfigs;
			this.FindExistingAndDeletedConfigurations(databaseGuid, changeHint, out dictionary, out list, out failedConfigs);
			this.CheckStopPending();
			this.LogEventsForFailedConfigs(failedConfigs);
			if (databaseGuid == null)
			{
				int replicaInstanceManagerNumThreadsPerDbCopy = RegistryParameters.ReplicaInstanceManagerNumThreadsPerDbCopy;
				Dependencies.ThreadPoolThreadCountManager.SetMinThreads(replicaInstanceManagerNumThreadsPerDbCopy * dictionary.Count, null, false);
				IOBufferPool.ConfigureCopyCount(dictionary.Count);
			}
			List<ReplayConfiguration> configurations;
			List<RestartInstanceWrapper> list2;
			this.GetNewConfigurationsAndChangedInstances(forceRestart, dictionary.Values, out configurations, out list2);
			this.CheckStopPending();
			this.DisableQueuesAndRemoveInstances(list, false);
			this.CheckStopPending();
			Dictionary<string, ReplayConfiguration> configurationsFound = ReplicaInstanceManager.ConvertDictionary(dictionary);
			List<string> databasesToStopMonitoring = ReplicaInstanceManager.ConvertConfigList(list);
			List<string> list3 = new List<string>(list2.Count);
			foreach (RestartInstanceWrapper restartInstanceWrapper in list2)
			{
				list3.Add(SafeInstanceTable<MonitoredDatabase>.GetIdentityFromGuid(restartInstanceWrapper.IdentityGuid));
			}
			RemoteDataProvider.MonitorDatabases(databasesToStopMonitoring, list3, configurationsFound);
			this.CheckStopPending();
			this.RestartInstances(list2, waitForRestarts, isHighPriority);
			this.CheckStopPending();
			this.StartInstances(configurations);
			this.NotifyDumpsterManagerOfRunningConfigs(dictionary, databaseGuid == null);
			this.NotifyAmPerfCounterUpdaterOfRunningConfigs();
		}

		private void RequestSuspendAndFailInternal(Guid guid, bool fSuspendCopy, uint errorEventId, string errorMessage, string suspendComment, bool preserveExistingError, bool blockResume = false, bool blockReseed = false, bool blockInPlaceReseed = false)
		{
			if (guid == Guid.Empty)
			{
				throw new ReplayServiceRpcArgumentException("guid");
			}
			Exception innerException;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(guid, ReplayConfigChangeHints.DbSuspendBefore, out innerException, true, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid>((long)this.GetHashCode(), "RequestSuspend: ReplicaInstance for database '{0}' could not be found in the hashtable.", guid);
				throw new ReplayServiceUnknownReplicaInstanceException(ReplayStrings.SuspendOperationName, guid.ToString(), innerException);
			}
			ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
			this.EnqueueReplicaInstanceOperation(new ReplicaInstanceRequestSuspendAndFail(runningReplicaInstance)
			{
				ErrorEventId = errorEventId,
				ErrorMessage = errorMessage,
				SuspendComment = suspendComment,
				PreserveExistingError = preserveExistingError,
				SuspendCopy = fSuspendCopy,
				BlockResume = blockResume,
				BlockReseed = blockReseed,
				BlockInPlaceReseed = blockInPlaceReseed
			}, out replicaInstanceQueuedItem);
			replicaInstanceQueuedItem.Wait();
		}

		private void LogEventsForFailedConfigs(List<KeyValuePair<IADDatabase, Exception>> failedConfigs)
		{
			foreach (KeyValuePair<IADDatabase, Exception> keyValuePair in failedConfigs)
			{
				IADDatabase key = keyValuePair.Key;
				Exception value = keyValuePair.Value;
				ReplayEventLogConstants.Tuple_ConfigUpdaterFailedToFindConfig.LogEvent(key.Guid.ToString(), new object[]
				{
					key.Name,
					Environment.MachineName,
					value.Message
				});
			}
		}

		private void NotifyDumpsterManagerOfRunningConfigs(Dictionary<Guid, ReplayConfiguration> configs, bool allConfigs)
		{
			List<ReplicaInstance> allReplicaInstances = this.GetAllReplicaInstances();
			List<ReplayConfiguration> allRunningConfigs = new List<ReplayConfiguration>(allReplicaInstances.Count);
			allReplicaInstances.ForEach(delegate(ReplicaInstance ri)
			{
				allRunningConfigs.Add(ri.Configuration);
			});
			this.SkippedLogsDeleter.UpdateDiscoveredConfigurations(allRunningConfigs);
			if (allConfigs)
			{
				List<ReplayConfiguration> allConfigurations = new List<ReplayConfiguration>(configs.Values);
				this.DumpsterManagerInstance.UpdateDiscoveredConfigurations(allConfigurations);
				return;
			}
			foreach (ReplicaInstance replicaInstance in allReplicaInstances)
			{
				if (!configs.ContainsKey(replicaInstance.Configuration.IdentityGuid))
				{
					configs.Add(replicaInstance.Configuration.IdentityGuid, replicaInstance.Configuration);
				}
			}
			List<ReplayConfiguration> allConfigurations2 = new List<ReplayConfiguration>(configs.Values);
			this.DumpsterManagerInstance.UpdateDiscoveredConfigurations(allConfigurations2);
		}

		private void FindExistingAndDeletedConfigurations(Guid? databaseGuid, ReplayConfigChangeHints changeHint, out Dictionary<Guid, ReplayConfiguration> allConfigs, out List<Guid> instancesToDelete, out List<KeyValuePair<IADDatabase, Exception>> failedConfigs)
		{
			ExTraceGlobals.ReplayManagerTracer.TraceFunction<string>((long)this.GetHashCode(), "Entering ReplicaInstanceManager.FindExistingAndDeletedConfigurations(). databaseGuid = {0}", (databaseGuid == null) ? "null" : databaseGuid.ToString());
			bool flag = databaseGuid != null;
			allConfigs = null;
			instancesToDelete = null;
			if (flag)
			{
				allConfigs = new Dictionary<Guid, ReplayConfiguration>(1);
				instancesToDelete = new List<Guid>(1);
				failedConfigs = new List<KeyValuePair<IADDatabase, Exception>>(1);
				Exception ex;
				ReplayConfiguration localReplayConfiguration = ReplayConfigurationHelper.GetLocalReplayConfiguration(databaseGuid.Value, this.ADConfig, this.ActiveManagerInstance, out ex);
				if (localReplayConfiguration != null)
				{
					allConfigs.Add(databaseGuid.Value, localReplayConfiguration);
				}
				else if (changeHint == ReplayConfigChangeHints.DbCopyRemoved)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, ReplayConfigChangeHints>((long)this.GetHashCode(), "ReplicaInstanceManager: ConfigurationUpdater adding database {0} to deleted instances list due to explicit action of '{1}'", databaseGuid.Value, changeHint);
					instancesToDelete.Add(databaseGuid.Value);
				}
				else
				{
					if (ex != null)
					{
						ReplayEventLogConstants.Tuple_ConfigUpdaterFailedToFindConfig.LogEvent(databaseGuid.Value.ToString(), new object[]
						{
							databaseGuid.Value.ToString(),
							Environment.MachineName,
							ex.Message
						});
						ReplayConfigurationHelper.ThrowDbOperationWrapperExceptionIfNecessary(ex);
						return;
					}
					ReplayEventLogConstants.Tuple_ConfigUpdaterFailedToFindConfig.LogEvent(databaseGuid.Value.ToString(), new object[]
					{
						databaseGuid.Value.ToString(),
						Environment.MachineName,
						string.Empty
					});
					throw new ReplayServiceCouldNotFindReplayConfigException(databaseGuid.Value.ToString(), Environment.MachineName);
				}
			}
			else
			{
				allConfigs = ReplayConfigurationHelper.GetAllLocalConfigurations(this.ADConfig, this.ActiveManagerInstance, out failedConfigs);
				instancesToDelete = this.GetDeletedInstances(allConfigs);
			}
			ExTraceGlobals.ReplayManagerTracer.TraceFunction((long)this.GetHashCode(), "Leaving ReplicaInstanceManager.FindExistingAndDeletedConfigurations()");
		}

		private void RestartInstances(List<RestartInstanceWrapper> instancesToRestart, bool waitForRestarts, bool isHighPriority)
		{
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstances called with {0} instances to restart.", instancesToRestart.Count);
			ExTraceGlobals.PFDTracer.TracePfd<int, int>((long)this.GetHashCode(), "PFD CRS {0} RestartInstances called with {1} instances to restart", 29981, instancesToRestart.Count);
			List<ReplicaInstanceQueuedItem> list = null;
			if (waitForRestarts)
			{
				list = new List<ReplicaInstanceQueuedItem>(instancesToRestart.Count);
			}
			foreach (RestartInstanceWrapper restartInstanceWrapper in instancesToRestart)
			{
				lock (this.m_locker)
				{
					this.CheckStopPending();
					ReplicaInstance replicaInstance = restartInstanceWrapper.OldReplicaInstance.ReplicaInstance;
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstances enqueueing Restart on instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
					ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} RestartInstances enqueueing Restart on instance {1}({2})", 21405, replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
					ReplicaInstanceRestartOperation operation = new ReplicaInstanceRestartOperation(restartInstanceWrapper, this);
					ReplicaInstanceQueuedItem item;
					this.EnqueueReplicaInstanceOperation(operation, isHighPriority, out item);
					if (waitForRestarts)
					{
						list.Add(item);
					}
				}
			}
			if (waitForRestarts)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstances is now waiting for RI restarts to complete.");
				foreach (ReplicaInstanceQueuedItem replicaInstanceQueuedItem in list)
				{
					replicaInstanceQueuedItem.Wait();
				}
			}
		}

		private void RestartInstanceInternal(RestartInstanceWrapper instanceToRestart)
		{
			ReplicaInstanceContainer replicaInstanceContainer = null;
			ReplicaInstance replicaInstance = null;
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance called for {0}({1}).", instanceToRestart.DisplayName, instanceToRestart.Identity);
			lock (this.m_locker)
			{
				this.CheckStopPending();
				if (!this.m_replicaInstances.TryGetValue(instanceToRestart.IdentityGuid, out replicaInstanceContainer))
				{
					DiagCore.RetailAssert(false, "RestartInstance called for {0}({1}) but no RI currently exists in the table!", new object[]
					{
						instanceToRestart.DisplayName,
						instanceToRestart.Identity
					});
				}
				replicaInstance = replicaInstanceContainer.ReplicaInstance;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance calling PrepareToStop on instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
				ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} RestartInstance calling PrepareToStop on instance {1}({2})", 21405, replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
				SourceSeedTable.Instance.CancelSeedingIfAppropriate(SourceSeedTable.CancelReason.ConfigChanged, instanceToRestart.IdentityGuid);
				replicaInstance.PrepareToStop();
				replicaInstance.MarkRestartSoonFlag();
			}
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance is constructing a new ReplayConfiguration for instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
			ReplayConfiguration configuration = replicaInstance.Configuration;
			Exception ex;
			ReplayConfiguration localReplayConfiguration = ReplayConfigurationHelper.GetLocalReplayConfiguration(instanceToRestart.IdentityGuid, this.ADConfig, this.ActiveManagerInstance, out ex);
			if (localReplayConfiguration != null)
			{
				if (!replicaInstance.Stopped)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance calling Stop on instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
					replicaInstance.Stop();
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance is skipping Stop on instance {0}({1}) since it has already been stopped.", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
				}
				ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} RestartInstance stopped instance {1}({2})", 17693, replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
				try
				{
					ReplicaInstance replicaInstance2 = this.ConstructReplicaInstance(localReplayConfiguration, replicaInstance, replicaInstanceContainer.PerformanceCounters);
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string, ReplayConfigType>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance created new instance for {0}({1}) of type {2}", replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity, replicaInstance2.Configuration.Type);
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstanceManager: DebugInfo for instance: {0}", replicaInstance2.Configuration.ToString());
					replicaInstanceContainer.UpdateReplicaInstance(replicaInstance2);
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: Starting instance {0}({1})", replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
					ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Started instance {1}({2})", 21789, replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
					replicaInstance2.Start();
				}
				catch (DatabaseVolumeInfoException ex2)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceError<DatabaseVolumeInfoException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance: RI for '{1}' failed to construct with DatabaseVolumeInfoException: {0}", ex2, replicaInstance.Configuration.DisplayName);
					ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(replicaInstance.Configuration.Identity, new object[]
					{
						replicaInstance.Configuration.ServerName,
						replicaInstance.Configuration.Type,
						replicaInstance.Configuration.DisplayName,
						ex2.Message
					});
				}
				catch (TransientException ex3)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceError<TransientException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: RestartInstance: RI for '{1}' failed to construct with TransientException: {0}", ex3, replicaInstance.Configuration.DisplayName);
					ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(replicaInstance.Configuration.Identity, new object[]
					{
						replicaInstance.Configuration.ServerName,
						replicaInstance.Configuration.Type,
						replicaInstance.Configuration.DisplayName,
						ex3.Message
					});
				}
				catch (DataSourceOperationException ex4)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceError<DataSourceOperationException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StartInstances: RI for '{1}' failed to construct with DataSourceOperationException: {0}", ex4, replicaInstance.Configuration.DisplayName);
					ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(replicaInstance.Configuration.Identity, new object[]
					{
						replicaInstance.Configuration.ServerName,
						replicaInstance.Configuration.Type,
						replicaInstance.Configuration.DisplayName,
						ex4.Message
					});
				}
				return;
			}
			if (ex != null)
			{
				ReplayEventLogConstants.Tuple_ConfigUpdaterFailedToFindConfig.LogEvent(configuration.IdentityGuid.ToString(), new object[]
				{
					configuration.IdentityGuid.ToString(),
					Environment.MachineName,
					ex.Message
				});
				ReplayConfigurationHelper.ThrowDbOperationWrapperExceptionIfNecessary(ex);
				return;
			}
			ReplayEventLogConstants.Tuple_ConfigUpdaterFailedToFindConfig.LogEvent(configuration.IdentityGuid.ToString(), new object[]
			{
				configuration.IdentityGuid.ToString(),
				Environment.MachineName,
				string.Empty
			});
			throw new ReplayServiceCouldNotFindReplayConfigException(configuration.IdentityGuid.ToString(), Environment.MachineName);
		}

		private void StartInstances(List<ReplayConfiguration> configurations)
		{
			List<ReplicaInstanceContainer> replicaInstances = new List<ReplicaInstanceContainer>(configurations.Count);
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplicaInstanceManager: StartInstances called with {0} configs to start.", configurations.Count);
			ExTraceGlobals.PFDTracer.TracePfd<int, int>((long)this.GetHashCode(), "PFD CRS {0} StartInstances called with {1} configs to start.", 29981, configurations.Count);
			bool isThirdPartyReplicationEnabled = ThirdPartyManager.IsThirdPartyReplicationEnabled;
			int riCount = 0;
			using (List<ReplayConfiguration>.Enumerator enumerator = configurations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ReplayConfiguration configuration = enumerator.Current;
					try
					{
						IPerfmonCounters perfCounters = null;
						if (isThirdPartyReplicationEnabled)
						{
							perfCounters = new NullPerfmonCounters();
						}
						else
						{
							perfCounters = new PerfmonCounters(ReplayServicePerfmon.GetInstance(configuration.Name));
						}
						ReplicaInstance replicaInstance = null;
						ReplicaInstance.DisposeIfActionUnsuccessful(delegate
						{
							replicaInstance = this.ConstructReplicaInstance(configuration, null, perfCounters);
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string, ReplayConfigType>((long)this.GetHashCode(), "ReplicaInstanceManager: Created new instance {0}({1}) of type {2}", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity, configuration.Type);
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstanceManager: DebugInfo for instance: {0}", replicaInstance.Configuration.ToString());
							ReplicaInstanceActionQueue queue = new ReplicaInstanceActionQueue();
							ReplicaInstanceContainer replicaInstanceContainer2 = new ReplicaInstanceContainer(replicaInstance, queue, perfCounters);
							lock (this.m_locker)
							{
								this.CheckStopPending();
								this.m_replicaInstances.Add(configuration.IdentityGuid, replicaInstanceContainer2);
								riCount = this.m_replicaInstances.Count;
								replicaInstances.Add(replicaInstanceContainer2);
							}
						}, replicaInstance);
					}
					catch (DatabaseVolumeInfoException ex)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceError<DatabaseVolumeInfoException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StartInstances: RI for '{1}' failed to construct with DatabaseVolumeInfoException: {0}", ex, configuration.DisplayName);
						ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(configuration.Identity, new object[]
						{
							configuration.ServerName,
							configuration.Type,
							configuration.DisplayName,
							ex.Message
						});
					}
					catch (TransientException ex2)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceError<TransientException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StartInstances: RI for '{1}' failed to construct with TransientException: {0}", ex2, configuration.DisplayName);
						ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(configuration.Identity, new object[]
						{
							configuration.ServerName,
							configuration.Type,
							configuration.DisplayName,
							ex2.Message
						});
					}
					catch (DataSourceOperationException ex3)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceError<DataSourceOperationException, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StartInstances: RI for '{1}' failed to construct with DataSourceOperationException: {0}", ex3, configuration.DisplayName);
						ReplayEventLogConstants.Tuple_InstanceFailedToStart.LogEvent(configuration.Identity, new object[]
						{
							configuration.ServerName,
							configuration.Type,
							configuration.DisplayName,
							ex3.Message
						});
					}
				}
			}
			IOBufferPool.ConfigureCopyCount(riCount);
			foreach (ReplicaInstanceContainer replicaInstanceContainer in replicaInstances)
			{
				ReplicaInstance replicaInstance2 = replicaInstanceContainer.ReplicaInstance;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: Starting instance {0}({1})", replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
				ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Started instance {1}({2})", 21789, replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
				replicaInstance2.Start();
			}
		}

		private void StopInstances(IEnumerable<Guid> instancesToStop)
		{
			int num = instancesToStop.Count<Guid>();
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances called with {0} instances to stop.", num);
			ExTraceGlobals.PFDTracer.TracePfd<int, int>((long)this.GetHashCode(), "PFD CRS {0} StopInstances called with {1} instances to stop", 25885, num);
			foreach (Guid key in instancesToStop)
			{
				lock (this.m_locker)
				{
					this.CheckStopPending();
					ReplicaInstanceContainer replicaInstanceContainer = this.m_replicaInstances[key];
					ReplicaInstance replicaInstance = replicaInstanceContainer.ReplicaInstance;
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances calling PrepareToStop on instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
					ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} StopInstances calling PrepareToStop on instance {1}({2})", 21405, replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
					replicaInstance.PrepareToStop();
					if (!this.IsStopping && replicaInstance is TargetReplicaInstance)
					{
						try
						{
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances calling SeedManager.CancelDbSeed() on instance {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
							this.SeedManagerInstance.CancelDbSeed(replicaInstance.Configuration.IdentityGuid, false);
							ReplayEventLogConstants.Tuple_SeedInstanceCleanupConfigChanged.LogEvent(null, new object[]
							{
								replicaInstance.Configuration.Name
							});
						}
						catch (SeederInstanceNotFoundException)
						{
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances: no seeder instance is currently running for {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Configuration.Identity);
						}
					}
				}
			}
			foreach (Guid key2 in instancesToStop)
			{
				ReplicaInstanceContainer replicaInstanceContainer2 = null;
				ReplicaInstance replicaInstance2 = null;
				lock (this.m_locker)
				{
					this.CheckStopPending();
					replicaInstanceContainer2 = this.m_replicaInstances[key2];
					replicaInstance2 = replicaInstanceContainer2.ReplicaInstance;
				}
				if (!replicaInstance2.Stopped)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances calling Stop on instance {0}({1})", replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
					replicaInstance2.Stop();
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: StopInstances is skipping Stop on instance {0}({1}) since it has already been stopped.", replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
				}
				lock (this.m_locker)
				{
					this.CheckStopPending();
					this.m_replicaInstances.Remove(replicaInstance2.Configuration.IdentityGuid);
					SuspendLockTable.RemoveInstance(replicaInstance2.Configuration.ReplayState.SuspendLock);
					ReplayServicePerfmon.RemoveInstance(replicaInstance2.Configuration.DatabaseName);
					replicaInstanceContainer2.Dispose();
				}
				ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} StopInstances stopped instance {1}({2})", 17693, replicaInstance2.Configuration.DisplayName, replicaInstance2.Configuration.Identity);
			}
		}

		private List<Guid> GetDeletedInstances(Dictionary<Guid, ReplayConfiguration> currentConfigurations)
		{
			List<Guid> list = new List<Guid>();
			lock (this.m_locker)
			{
				foreach (KeyValuePair<Guid, ReplicaInstanceContainer> keyValuePair in this.m_replicaInstances)
				{
					this.CheckStopPending();
					if (!currentConfigurations.ContainsKey(keyValuePair.Key))
					{
						ReplicaInstance replicaInstance = keyValuePair.Value.ReplicaInstance;
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: Found deleted instance: {0}({1})", replicaInstance.Configuration.DisplayName, replicaInstance.Identity);
						list.Add(keyValuePair.Key);
					}
				}
			}
			return list;
		}

		private void GetNewConfigurationsAndChangedInstances(bool forceRestart, IEnumerable<ReplayConfiguration> currentConfigurations, out List<ReplayConfiguration> newConfigurations, out List<RestartInstanceWrapper> instancesToRestart)
		{
			ExTraceGlobals.ReplayManagerTracer.TraceFunction<bool>((long)this.GetHashCode(), "Entering ReplicaInstanceManager.GetNewConfigurationsAndChangedInstances(). forceRestart = {0}", forceRestart);
			newConfigurations = new List<ReplayConfiguration>();
			instancesToRestart = new List<RestartInstanceWrapper>();
			foreach (ReplayConfiguration replayConfiguration in currentConfigurations)
			{
				ReplicaInstanceContainer replicaInstanceContainer = null;
				lock (this.m_locker)
				{
					this.CheckStopPending();
					if (!this.m_replicaInstances.ContainsKey(replayConfiguration.IdentityGuid))
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: Found new config: {0}({1})", replayConfiguration.DisplayName, replayConfiguration.Identity);
						ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Found new config: {1}({2})", 18717, replayConfiguration.DisplayName, replayConfiguration.Identity);
						newConfigurations.Add(replayConfiguration);
					}
					else
					{
						replicaInstanceContainer = this.m_replicaInstances[replayConfiguration.IdentityGuid];
					}
				}
				if (replicaInstanceContainer != null && replicaInstanceContainer.ReplicaInstance.ShouldBeRestarted(replayConfiguration, forceRestart))
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstanceManager: Found changed or broken/suspended instance, or requesting to restart soon, restarting: {0}({1})", replayConfiguration.DisplayName, replayConfiguration.Identity);
					ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Found changed or broken/suspended instance, or requesting to restart soon, restarting: {1}({2})", 29981, replayConfiguration.DisplayName, replayConfiguration.Identity);
					instancesToRestart.Add(new RestartInstanceWrapper(replicaInstanceContainer));
				}
			}
			ExTraceGlobals.ReplayManagerTracer.TraceFunction((long)this.GetHashCode(), "Leaving ReplicaInstanceManager.GetNewConfigurationsAndChangedInstances()");
		}

		private void DisableQueues(IEnumerable<Guid> instancesToDisable)
		{
			lock (this.m_locker)
			{
				foreach (Guid key in instancesToDisable)
				{
					ReplicaInstanceActionQueue queue = this.m_replicaInstances[key].Queue;
					queue.PrepareToStop();
				}
			}
		}

		private void DisableQueuesAndRemoveInstances(IEnumerable<Guid> instancesToStop, bool fStopping)
		{
			IEnumerable<Guid> enumerable = null;
			lock (this.m_locker)
			{
				this.IsFinalShutdownSequence = fStopping;
				enumerable = (from dbGuid in this.m_replicaInstances.Keys
				where instancesToStop.Contains(dbGuid)
				select dbGuid).ToList<Guid>();
				if (!fStopping)
				{
					this.DisableQueues(enumerable);
				}
			}
			foreach (Guid key in enumerable)
			{
				ReplicaInstanceActionQueue queue = this.m_replicaInstances[key].Queue;
				queue.Stop();
			}
			this.StopInstances(enumerable);
		}

		private ReplicaInstance ConstructReplicaInstance(ReplayConfiguration configuration, ReplicaInstance previousReplicaInstance, IPerfmonCounters perfCounters)
		{
			ReplicaInstance result;
			switch (configuration.Type)
			{
			case ReplayConfigType.RemoteCopyTarget:
				perfCounters.Reset();
				result = new TargetReplicaInstance(configuration, previousReplicaInstance, perfCounters);
				break;
			case ReplayConfigType.RemoteCopySource:
				perfCounters.Reset();
				result = new SourceReplicaInstance(configuration, previousReplicaInstance, perfCounters);
				break;
			case ReplayConfigType.SingleCopySource:
				perfCounters.Reset();
				result = new SingleCopyReplicaInstance(configuration, perfCounters);
				break;
			default:
				ExDiagnostics.FailFast(string.Format(CultureInfo.CurrentCulture, "Unexpected type {0} in ReplayConfigType", new object[]
				{
					configuration.Type
				}), true);
				result = null;
				break;
			}
			return result;
		}

		private ReplicaInstanceContext GetCurrentReplicaInstanceContext(Guid instanceGuid, ReplayConfigChangeHints changeHints, bool waitForRIRestartIfNeeded)
		{
			Exception ex;
			TargetReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<TargetReplicaInstance>(instanceGuid, changeHints, out ex, waitForRIRestartIfNeeded, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "ReplicaInstanceManager: GetCurrentReplicaInstanceContext ({0}) could not find a running Target RI. Returning <null>.", instanceGuid);
				return null;
			}
			return runningReplicaInstance.CurrentContext;
		}

		private ReplicaInstanceContext GetCurrentSourceReplicaInstanceContext(Guid instanceGuid, ReplayConfigChangeHints changeHints, bool waitForRIRestartIfNeeded)
		{
			Exception ex;
			SourceReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<SourceReplicaInstance>(instanceGuid, changeHints, out ex, waitForRIRestartIfNeeded, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "ReplicaInstanceManager: GetCurrentSourceReplicaInstanceContext ({0}) could not find a running Source RI. Returning <null>.", instanceGuid);
				return null;
			}
			return runningReplicaInstance.CurrentContext;
		}

		private ReplicaInstanceContext GetCurrentTargetOrSourceReplicaInstanceContext(Guid instanceGuid, ReplayConfigChangeHints changeHints, bool waitForRIRestartIfNeeded)
		{
			Exception ex;
			ReplicaInstance runningReplicaInstance = this.GetRunningReplicaInstance<ReplicaInstance>(instanceGuid, changeHints, out ex, waitForRIRestartIfNeeded, -1);
			if (runningReplicaInstance == null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "ReplicaInstanceManager: GetCurrentTargetOrSourceReplicaInstanceContext ({0}) could not find a running Target RI. Returning <null>.", instanceGuid);
				return null;
			}
			return runningReplicaInstance.CurrentContext;
		}

		private TReplicaInstance GetRunningReplicaInstance<TReplicaInstance>(Guid instanceGuid, ReplayConfigChangeHints restartChangeHint, out Exception exception, bool waitForRIRestartIfNeeded = true, int waitTimeoutMs = -1) where TReplicaInstance : ReplicaInstance
		{
			return this.GetRunningReplicaInstance<TReplicaInstance>(instanceGuid, restartChangeHint, null, out exception, waitForRIRestartIfNeeded, waitTimeoutMs);
		}

		private TReplicaInstance GetRunningReplicaInstance<TReplicaInstance>(Guid instanceGuid, ReplayConfigChangeHints restartChangeHint, AmServerName sourceServer, out Exception exception, bool waitForRIRestartIfNeeded = true, int waitTimeoutMs = -1) where TReplicaInstance : ReplicaInstance
		{
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid, AmServerName>((long)this.GetHashCode(), "GetRunningReplicaInstance<{0}> called: InstanceGuid: {1}, SourceServer: {2}", typeof(TReplicaInstance).Name, instanceGuid, sourceServer);
			exception = null;
			ReplicaInstance replicaInstance = null;
			TReplicaInstance treplicaInstance = default(TReplicaInstance);
			if (!this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetRunningReplicaInstance did not find a running RI for instance {0}", instanceGuid);
			}
			treplicaInstance = this.ConvertToReplicaInstance<TReplicaInstance>(replicaInstance, sourceServer);
			if (treplicaInstance == null)
			{
				try
				{
					Dependencies.ConfigurationUpdater.NotifyChangedReplayConfiguration(instanceGuid, waitForRIRestartIfNeeded, restartChangeHint, waitTimeoutMs);
					if (!waitForRIRestartIfNeeded)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetRunningReplicaInstance did not find a valid running RI for instance {0}, and has enqueued an RI restart with the ConfigUpdater. Returning null.", instanceGuid);
						return default(TReplicaInstance);
					}
					if (!this.TryGetReplicaInstance(instanceGuid, out replicaInstance))
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetRunningReplicaInstance did not find a running RI for instance {0} even after re-running ConfigUpdater. The configuration may have gone away.", instanceGuid);
					}
					else
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetRunningReplicaInstance found a running RI for instance {0} after re-running ConfigUpdater.", instanceGuid);
						treplicaInstance = this.ConvertToReplicaInstance<TReplicaInstance>(replicaInstance, sourceServer);
						if (treplicaInstance == null)
						{
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "GetRunningReplicaInstance ({0}): Instance '{1}' is either not of the requested type, or is pointing to wrong SourceMachine.", instanceGuid, replicaInstance.Configuration.DisplayName);
						}
					}
				}
				catch (TaskServerTransientException ex)
				{
					exception = ex;
					ExTraceGlobals.ReplayManagerTracer.TraceError<Guid, TaskServerTransientException>((long)this.GetHashCode(), "GetRunningReplicaInstance ({0}): NotifyChangedReplayConfiguration() threw exception: {1}", instanceGuid, ex);
				}
				catch (TaskServerException ex2)
				{
					exception = ex2;
					ExTraceGlobals.ReplayManagerTracer.TraceError<Guid, TaskServerException>((long)this.GetHashCode(), "GetRunningReplicaInstance ({0}): NotifyChangedReplayConfiguration() threw exception: {1}", instanceGuid, ex2);
				}
				return treplicaInstance;
			}
			return treplicaInstance;
		}

		private void TryNotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, ReplayConfigChangeHints restartChangeHint)
		{
			this.TryNotifyChangedReplayConfiguration(dbGuid, waitForCompletion, false, restartChangeHint);
		}

		private void TryNotifyChangedReplayConfiguration(Guid dbGuid, bool waitForCompletion, bool isHighPriority, ReplayConfigChangeHints restartChangeHint)
		{
			try
			{
				Dependencies.ConfigurationUpdater.NotifyChangedReplayConfiguration(dbGuid, waitForCompletion, !waitForCompletion, isHighPriority, restartChangeHint, -1);
			}
			catch (TaskServerTransientException arg)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<TaskServerTransientException>((long)this.GetHashCode(), "TryNotifyChangedReplayConfiguration: NotifyChangedReplayConfiguration() threw exception: {0}", arg);
			}
			catch (TaskServerException arg2)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<TaskServerException>((long)this.GetHashCode(), "TryNotifyChangedReplayConfiguration: NotifyChangedReplayConfiguration() threw exception: {0}", arg2);
			}
		}

		private TReplicaInstance ConvertToReplicaInstance<TReplicaInstance>(ReplicaInstance instance, AmServerName sourceServer) where TReplicaInstance : ReplicaInstance
		{
			TReplicaInstance result = default(TReplicaInstance);
			bool flag = AmServerName.IsNullOrEmpty(sourceServer);
			if (instance != null && instance is TReplicaInstance)
			{
				if (flag || instance.Configuration.IsSourceMachineEqual(sourceServer))
				{
					result = (instance as TReplicaInstance);
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ConvertToReplicaInstance ({0}): RI '{1}' is pointing to a different source server. Expected: {2}; Actual: {3}", new object[]
					{
						instance.Configuration.IdentityGuid,
						instance.Configuration.DisplayName,
						sourceServer,
						instance.Configuration.SourceMachine
					});
				}
			}
			return result;
		}

		private bool TryGetReplicaInstanceActionQueue(Guid guid, out ReplicaInstanceActionQueue queue)
		{
			queue = null;
			bool result;
			lock (this.m_locker)
			{
				ReplicaInstanceContainer replicaInstanceContainer;
				if (this.m_replicaInstances.TryGetValue(guid, out replicaInstanceContainer))
				{
					queue = replicaInstanceContainer.Queue;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool EnqueueReplicaInstanceOperation(ReplicaInstanceQueuedItem operation, out ReplicaInstanceQueuedItem enqueuedOperation)
		{
			return this.EnqueueReplicaInstanceOperation(operation, false, out enqueuedOperation);
		}

		private bool EnqueueReplicaInstanceOperation(ReplicaInstanceQueuedItem operation, bool isHighPriority, out ReplicaInstanceQueuedItem enqueuedOperation)
		{
			bool result = false;
			enqueuedOperation = null;
			ReplicaInstanceActionQueue actionQueue;
			if (this.TryGetReplicaInstanceActionQueue(operation.DbGuid, out actionQueue))
			{
				this.EnqueueReplicaInstanceOperation(operation, actionQueue, isHighPriority, out enqueuedOperation);
				return result;
			}
			throw new ReplayServiceUnknownReplicaInstanceException(operation.Name, operation.DbCopyName);
		}

		private bool EnqueueReplicaInstanceOperation(ReplicaInstanceQueuedItem operation, ReplicaInstanceActionQueue actionQueue, bool isHighPriority, out ReplicaInstanceQueuedItem enqueuedOperation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			bool flag = false;
			enqueuedOperation = operation;
			if (isHighPriority)
			{
				if (actionQueue.EnqueueHighPriority(operation, null))
				{
					flag = true;
				}
			}
			else if (operation.IsDuplicateAllowed)
			{
				if (actionQueue.Enqueue(operation, null))
				{
					flag = true;
				}
			}
			else
			{
				ReplicaInstanceQueuedItem replicaInstanceQueuedItem;
				EventWaitHandle eventWaitHandle;
				flag = actionQueue.EnqueueUniqueItem(operation, null, false, out replicaInstanceQueuedItem, out eventWaitHandle);
				if (!flag && replicaInstanceQueuedItem != null)
				{
					enqueuedOperation = replicaInstanceQueuedItem;
				}
			}
			return flag;
		}

		private void NotifyAmPerfCounterUpdaterOfRunningConfigs()
		{
			List<ReplicaInstance> allReplicaInstances = this.GetAllReplicaInstances();
			List<Guid> allDatabases = new List<Guid>(allReplicaInstances.Count);
			allReplicaInstances.ForEach(delegate(ReplicaInstance ri)
			{
				allDatabases.Add(ri.Configuration.IdentityGuid);
			});
			AmSystemManager.Instance.UpdateAmPerfCounterUpdaterDatabasesList(allDatabases);
		}

		private void CheckStopPending()
		{
			if (this.IsStopping && !this.IsFinalShutdownSequence)
			{
				throw new OperationAbortedException();
			}
		}

		private const string FirstConfigUpdaterFullScanCompletedEventName = "FirstConfigUpdaterFullScanCompletedEvent";

		private ManualOneShotEvent m_firstFullScanCompleted = new ManualOneShotEvent("FirstConfigUpdaterFullScanCompletedEvent");

		private object m_locker = new object();

		private Dictionary<Guid, ReplicaInstanceContainer> m_replicaInstances;

		private bool m_fConfigUpdaterRunning;
	}
}
