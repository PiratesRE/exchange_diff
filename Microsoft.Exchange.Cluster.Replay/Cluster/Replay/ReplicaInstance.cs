using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.RpcEndpoint;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ReplicaInstance : DisposeTrackableBase, IIdentityGuid
	{
		protected ReplicaInstance(ReplayConfiguration configuration, bool fTargetInstance, ReplicaInstance previousReplicaInstance, IPerfmonCounters perfCounters)
		{
			bool flag = false;
			try
			{
				this.Configuration = configuration;
				this.m_fTargetInstance = fTargetInstance;
				this.m_tprModeEnabled = ThirdPartyManager.IsThirdPartyReplicationEnabled;
				this.configName = configuration.Name;
				this.m_components = new List<IStartStop>(10);
				this.PerformanceCounters = perfCounters;
				this.Configuration.ReplayState.PerfmonCounters = perfCounters;
				this.m_instanceCreateTime = DateTime.UtcNow;
				if (this.IsThirdPartyReplicationEnabled && this.IsTarget)
				{
					this.PrepareToStopCalledEvent = new ManualOneShotEvent("PrepareToStopCalledEvent");
					this.m_currentContext = new ReplicaInstanceContext();
					this.m_currentContext.InitializeContext(this);
					this.m_currentContext.InitializeVolumeInfo();
				}
				else
				{
					this.m_currentContext = new ReplicaInstanceContext();
					this.Configuration.ReplayState.UseSetBrokenForIOFailures(this.m_currentContext);
					this.PrepareToStopCalledEvent = new ManualOneShotEvent("PrepareToStopCalledEvent");
					this.m_configurationCheckIdleEvent = null;
					this.FileChecker = ReplicaInstance.ConstructFileChecker(this.Configuration);
					this.InitializeContexts(previousReplicaInstance);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.Configuration.DatabaseName;
			}
		}

		internal static void DisposeIfActionUnsuccessful(Action operation, IDisposable toDispose)
		{
			bool flag = false;
			try
			{
				operation();
				flag = true;
			}
			finally
			{
				if (!flag && toDispose != null)
				{
					toDispose.Dispose();
				}
			}
		}

		internal static FileChecker ConstructFileChecker(IReplayConfiguration configuration)
		{
			string databaseFullFilePath = configuration.DestinationEdbPath;
			string logfileDirectory = configuration.DestinationLogPath;
			string systemDirectory = configuration.DestinationSystemPath;
			if (configuration.Type == ReplayConfigType.RemoteCopySource)
			{
				logfileDirectory = configuration.SourceLogPath;
				systemDirectory = configuration.SourceSystemPath;
				databaseFullFilePath = configuration.SourceEdbPath;
			}
			else
			{
				logfileDirectory = configuration.DestinationLogPath;
				systemDirectory = configuration.DestinationLogPath;
				databaseFullFilePath = configuration.DestinationEdbPath;
			}
			return new FileChecker(configuration.Name, logfileDirectory, systemDirectory, configuration.LogFilePrefix, "." + configuration.LogExtension, databaseFullFilePath, configuration.IdentityGuid);
		}

		protected void InitializeSuspendState()
		{
			ReplayState replayState = this.Configuration.ReplayState;
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool, bool>((long)replayState.SuspendLock.GetHashCode(), "InitializeSuspendState({0}): SuspendWanted is being set to {1}. Previous= {2}", this.Configuration.Name, replayState.SuspendLockRemote.SuspendWanted, replayState.SuspendLock.SuspendWanted);
			replayState.SuspendLock.SuspendWanted = replayState.SuspendLockRemote.SuspendWanted;
			replayState.SuspendLock.Arbitrate(null);
		}

		internal static FailureTag TargetFileCheckerExceptionToFailureTag(FileCheckException ex)
		{
			if (ex is FileCheckAccessDeniedException || ex is FileCheckAccessDeniedDismountFailedException)
			{
				return FailureTag.NoOp;
			}
			if (ex is FileCheckEDBMissingException || ex is FileCheckLogfileMissingException || ex is FileCheckJustCreatedEDBException || ex is FileCheckRequiredLogfileGapException)
			{
				return FailureTag.Reseed;
			}
			if (ex is FileCheckIOErrorException)
			{
				return ReplicaInstance.IOExceptionToFailureTag(ex.InnerException as IOException);
			}
			if (ex is FileCheckIsamErrorException)
			{
				return ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex.InnerException as EsentErrorException);
			}
			if (ex is FileCheckCorruptFileException)
			{
				return FailureTag.IoHard;
			}
			return FailureTag.AlertOnly;
		}

		internal static FailureTag IOExceptionToFailureTag(IOException ioException)
		{
			if (ioException == null)
			{
				return FailureTag.AlertOnly;
			}
			if (FileOperations.IsVolumeLockedException(ioException))
			{
				return FailureTag.LockedVolume;
			}
			if (ioException is DirectoryNotFoundException || ioException is FileNotFoundException)
			{
				return FailureTag.NoOp;
			}
			if (ioException is EndOfStreamException)
			{
				return FailureTag.NoOp;
			}
			if (ioException is DriveNotFoundException)
			{
				return FailureTag.IoHard;
			}
			if (ioException is PathTooLongException)
			{
				return FailureTag.Unrecoverable;
			}
			if (ioException is FileLoadException)
			{
				return FailureTag.Unrecoverable;
			}
			if (FileOperations.IsDiskFullException(ioException))
			{
				return FailureTag.Space;
			}
			if (FileOperations.IsRetryableIOException(ioException))
			{
				return FailureTag.NoOp;
			}
			return FailureTag.IoHard;
		}

		internal static FailureTag SourceIsamErrorExceptionToFailureTag(EsentErrorException ex)
		{
			if (ex == null)
			{
				return FailureTag.NoOp;
			}
			if (ex is EsentFragmentationException)
			{
				return FailureTag.Unrecoverable;
			}
			if (ex is EsentInconsistentException)
			{
				return FailureTag.Unrecoverable;
			}
			if (ex is EsentDataException)
			{
				return FailureTag.Corruption;
			}
			if (ex is EsentMemoryException)
			{
				return FailureTag.Memory;
			}
			if (ex is EsentDiskException)
			{
				return FailureTag.Space;
			}
			if (ex is EsentUsageException)
			{
				return FailureTag.Unrecoverable;
			}
			return FailureTag.NoOp;
		}

		internal static FailureTag TargetIsamErrorExceptionToFailureTag(EsentErrorException ex)
		{
			if (ex == null)
			{
				return FailureTag.AlertOnly;
			}
			if (ex is EsentLogFileCorruptException || ex is EsentLogCorruptedException)
			{
				return FailureTag.RecoveryRedoLogCorruption;
			}
			if (ex is EsentFragmentationException)
			{
				return FailureTag.Unrecoverable;
			}
			if (ex is EsentInconsistentException)
			{
				return FailureTag.Reseed;
			}
			if (ex is EsentDataException)
			{
				return FailureTag.Corruption;
			}
			if (ex is EsentMemoryException)
			{
				return FailureTag.Memory;
			}
			if (ex is EsentDiskException)
			{
				return FailureTag.Space;
			}
			if (ex is EsentStateException)
			{
				return FailureTag.Reseed;
			}
			if (ex is EsentUsageException)
			{
				return FailureTag.Unrecoverable;
			}
			if (ex is EsentFatalException)
			{
				return FailureTag.AlertOnly;
			}
			if (ex is EsentIOException)
			{
				if (ex is EsentDiskIOException)
				{
					return FailureTag.Reseed;
				}
				return FailureTag.NoOp;
			}
			else
			{
				if (ex is EsentResourceException)
				{
					return FailureTag.NoOp;
				}
				return FailureTag.AlertOnly;
			}
		}

		private static void SuspendActivation(ReplayState state, string comment)
		{
			state.ActivationSuspended = true;
			ReplicaInstance.WriteSuspendComment(state, comment);
		}

		private static void ResumeActivation(ReplayState state)
		{
			state.ActivationSuspended = false;
			if (!state.Suspended)
			{
				state.SuspendMessage = null;
			}
		}

		private static bool UpdateDatabaseMountStatus(RpcDatabaseCopyStatus2 copyStatus, Guid dbGuid, ReplicaInstance instance, string activeServer, MountStatus? mountStatus)
		{
			bool flag = false;
			Exception ex = null;
			try
			{
				bool flag2 = mountStatus == null || string.IsNullOrEmpty(activeServer);
				MountStatus mountStatus2 = mountStatus ?? MountStatus.Unknown;
				if (flag2)
				{
					mountStatus2 = ActiveManagerCore.GetDatabaseMountStatus(dbGuid, out activeServer);
				}
				if (!string.IsNullOrEmpty(activeServer) && !Cluster.StringIEquals(activeServer, Dependencies.ManagementClassHelper.LocalMachineName))
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<Guid>((long)instance.GetHashCode(), "GetCopyStatus.UpdateDatabaseMountStatus: DB copy {0} is not Active even though a Source RI is running.", dbGuid);
					return false;
				}
				switch (mountStatus2)
				{
				case MountStatus.Unknown:
					flag = true;
					copyStatus.CopyStatus = CopyStatusEnum.Dismounted;
					copyStatus.ErrorMessage = ReplayStrings.CouldNotGetMountStatus;
					break;
				case MountStatus.Mounted:
					copyStatus.CopyStatus = CopyStatusEnum.Mounted;
					break;
				case MountStatus.Dismounted:
					copyStatus.CopyStatus = CopyStatusEnum.Dismounted;
					break;
				case MountStatus.Mounting:
					copyStatus.CopyStatus = CopyStatusEnum.Mounting;
					break;
				case MountStatus.Dismounting:
					copyStatus.CopyStatus = CopyStatusEnum.Dismounting;
					break;
				}
			}
			catch (ClusterException ex2)
			{
				ex = ex2;
			}
			catch (AmServerException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ex = new AmFailedToReadClusdbException(ex.Message);
				flag = true;
				copyStatus.CopyStatus = CopyStatusEnum.Failed;
				copyStatus.ErrorMessage = ReplayStrings.CouldNotGetMountStatusError(ex.Message);
				copyStatus.ExtendedErrorInfo = new ExtendedErrorInfo(ex);
			}
			if (flag)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<Guid>((long)instance.GetHashCode(), "Querying store to get the database mount status for DB {0}.", dbGuid);
				MountStatus storeDatabaseMountStatus = AmStoreHelper.GetStoreDatabaseMountStatus(AmServerName.LocalComputerName, dbGuid);
				if (storeDatabaseMountStatus == MountStatus.Mounted)
				{
					copyStatus.CopyStatus = CopyStatusEnum.Mounted;
					copyStatus.ErrorMessage = null;
				}
				else
				{
					copyStatus.CopyStatus = CopyStatusEnum.Dismounted;
				}
			}
			return true;
		}

		private static Exception RunCiOperation(Action op)
		{
			if (RegistryParameters.IsManagedStore())
			{
				ExTraceGlobals.ReplayServiceRpcTracer.TraceError(0L, "RunCiOperation ignoring action: CI not supported with Managed Store");
				return null;
			}
			try
			{
				op();
			}
			catch (CiSeederSearchCatalogException ex)
			{
				ExTraceGlobals.ReplayServiceRpcTracer.TraceError<CiSeederSearchCatalogException>(0L, "RunCiOperation caught and returns: {0}", ex);
				return ex;
			}
			return null;
		}

		private static void WriteSuspendComment(ReplayState state, string comment)
		{
			if (!string.IsNullOrEmpty(comment))
			{
				state.SuspendMessage = comment;
				if (!Cluster.StringIEquals(state.SuspendMessage, comment))
				{
					throw new SuspendMessageWriteFailedException();
				}
			}
		}

		private void InitializeContexts(ReplicaInstance previousReplicaInstance)
		{
			if (previousReplicaInstance != null && previousReplicaInstance.Configuration.Type == ReplayConfigType.RemoteCopyTarget)
			{
				this.m_previousContext = new ReplicaInstanceContextMinimal(previousReplicaInstance.CurrentContext);
			}
			this.m_currentContext.InitializeContext(this);
			this.m_currentContext.CarryOverPreviousStatus(this.m_previousContext);
			this.m_currentContext.InitializeVolumeInfo();
		}

		protected virtual bool ShouldAcquireSuspendLockInConfigChecker
		{
			get
			{
				return true;
			}
		}

		private StateLock SuspendLockConfigChecker { get; set; }

		public void Start()
		{
			lock (this)
			{
				if (!this.m_fStarted)
				{
					this.m_fStarted = true;
					if (this.IsThirdPartyReplicationEnabled && this.IsTarget)
					{
						this.TraceDebug("TRI.Start in TPR: nothing to do");
					}
					else
					{
						this.BeginConfigurationChecker();
					}
				}
				else
				{
					this.TraceDebug("the replica instance is already started, we only need to start once before stop, bail this one");
				}
			}
		}

		private void BeginConfigurationChecker()
		{
			this.CreateIdleEvent();
			bool flag = false;
			try
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConfigurationChecker), this);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.SetAndClearIdleEvent();
				}
			}
		}

		private void ConfigurationChecker(object stateIgnored)
		{
			try
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					try
					{
						this.AcquireSuspendLockForConfigChecker();
						this.CheckInstanceAbortRequested();
						if (!this.HasDestinationEdbPaths())
						{
							this.CurrentContext.SetBroken(FailureTag.Configuration, ReplayEventLogConstants.Tuple_NoDatabasesInReplica, new string[0]);
						}
						else
						{
							this.ConfigurationCheckerInternal();
						}
					}
					catch (SetBrokenControlTransferException)
					{
					}
					catch (LogRepairUnexpectedVerifyException ex)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_FileCheckError, ex, new string[]
						{
							ex.Message
						});
					}
					catch (LogRepairFailedToCopyFromActiveException ex2)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_FileCheckError, ex2, new string[]
						{
							ex2.Message
						});
					}
					catch (LogRepairFailedToVerifyFromActiveException ex3)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_FileCheckError, ex3, new string[]
						{
							ex3.Message
						});
					}
					catch (LogRepairDivergenceCheckFailedException ex4)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_FileCheckError, ex4, new string[]
						{
							ex4.Message
						});
					}
					catch (LogRepairNotPossibleException ex5)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogRepairNotPossible, ex5, new string[]
						{
							ex5.Message
						});
					}
					catch (LogRepairRetryCountExceededException ex6)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogRepairFailedDueToRetryLimit, new string[]
						{
							ex6.RetryCount.ToString()
						});
					}
					catch (LogRepairTransientException ex7)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_FileCheckError, ex7, new string[]
						{
							ex7.Message
						});
					}
					catch (LastLogReplacementException ex8)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_IncrementalReseedFailedError, ex8, new string[]
						{
							ex8.Message
						});
					}
					catch (ReseedCheckMissingLogfileException ex9)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_IncrementalReseedFailedError, ex9, new string[]
						{
							ex9.Message
						});
					}
					catch (IncSeedDivergenceCheckFailedException ex10)
					{
						FailureTag failureTag = FailureTag.AlertOnly;
						if (ex10.InnerException != null)
						{
							IOException ex11 = ex10.InnerException as IOException;
							if (ex11 != null)
							{
								int num = 0;
								if (FileOperations.IsFatalIOException(ex11, out num))
								{
									failureTag = FailureTag.Reseed;
								}
							}
							else
							{
								EsentErrorException ex12 = ex10.InnerException as EsentErrorException;
								if (ex12 != null)
								{
									if (ex12 is EsentDiskReadVerificationFailureException || ex12 is EsentLogFileCorruptException)
									{
										failureTag = FailureTag.RecoveryRedoLogCorruption;
									}
									else
									{
										failureTag = ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex12);
									}
								}
								else if (ex10.InnerException is LastLogReplacementException)
								{
									failureTag = FailureTag.Reseed;
								}
							}
						}
						this.CurrentContext.SetBroken(failureTag, ReplayEventLogConstants.Tuple_IncrementalReseedFailedError, ex10, new string[]
						{
							ex10.Message
						});
					}
					catch (IncSeedConfigNotSupportedException ex13)
					{
						this.CurrentContext.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_IncrementalReseedInitException, ex13, new string[]
						{
							ex13.Message
						});
					}
					catch (IncrementalReseedFileStateChangedException)
					{
						this.TraceDebug("IncrementalReseed re-dirtied the database. Restarting the ReplicaInstance to recompute file state and re-run IR.");
						this.CurrentContext.RestartInstanceNow(ReplayConfigChangeHints.IncReseedRedirtiedDatabase);
					}
					catch (IncrementalReseedFailedException ex14)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_IncrementalReseedFailedError, ex14, new string[]
						{
							ex14.Message
						});
					}
					catch (IncrementalReseedPrereqException ex15)
					{
						this.CurrentContext.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_IncrementalReseedPrereqError, ex15, new string[]
						{
							ex15.Message
						});
					}
					catch (IncrementalReseedRetryableException ex16)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_IncrementalReseedRetryableError, ex16, new string[]
						{
							ex16.Message
						});
					}
					catch (LogFileCheckException ex17)
					{
						this.CurrentContext.SetBroken(FailureTag.RecoveryRedoLogCorruption, ReplayEventLogConstants.Tuple_FileCheckError, ex17, new string[]
						{
							ex17.Message
						});
					}
					catch (FileCheckException ex18)
					{
						this.CurrentContext.SetBroken(ReplicaInstance.TargetFileCheckerExceptionToFailureTag(ex18), ReplayEventLogConstants.Tuple_FileCheckError, ex18, new string[]
						{
							ex18.Message
						});
					}
					catch (ADExternalException ex19)
					{
						this.CurrentContext.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedADError, ex19, new string[]
						{
							ex19.Message
						});
					}
					catch (EsentErrorException ex20)
					{
						this.CurrentContext.SetBroken(ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex20), ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedGeneric, ex20, new string[]
						{
							ex20.Message
						});
					}
					catch (IOException ex21)
					{
						this.CurrentContext.SetBroken(ReplicaInstance.IOExceptionToFailureTag(ex21), ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedGeneric, ex21, new string[]
						{
							ex21.Message
						});
					}
					catch (UnauthorizedAccessException ex22)
					{
						this.CurrentContext.SetBroken(FailureTag.Configuration, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedGeneric, ex22, new string[]
						{
							ex22.Message
						});
					}
					catch (TransientException ex23)
					{
						this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedTransient, ex23, new string[]
						{
							ex23.Message
						});
					}
					catch (ReplicaInstance.OperationAbortedDueToAdminSuspendException)
					{
						this.TraceError("Leaving ConfigurationChecker due to Admin Suspend requested.");
					}
					catch (OperationAbortedException)
					{
						this.TraceError("Leaving ConfigurationChecker due to PrepareToStopCalled, higher priority lock, or AcquireLock failure.");
						this.CurrentContext.RestartInstanceSoon(true);
					}
					finally
					{
						if (this.IsTarget)
						{
							ThreadPool.QueueUserWorkItem(delegate(object obj)
							{
								TargetReplicaInstance.SyncTargetSuspendResumeState(obj as ReplayConfiguration);
							}, this.Configuration);
						}
						this.ReleaseSuspendLockForConfigChecker();
					}
				});
			}
			finally
			{
				this.EndConfigurationChecker();
			}
		}

		protected abstract bool ConfigurationCheckerInternal();

		private void EndConfigurationChecker()
		{
			try
			{
				lock (this)
				{
					this.SetAndClearIdleEvent();
					if (!this.IsBroken && !this.CurrentContext.Suspended && !this.m_fPrepareToStopCalled)
					{
						try
						{
							this.AcquireSuspendLockForConfigChecker();
							this.StartComponents();
						}
						catch (EsentErrorException ex)
						{
							this.CurrentContext.SetBroken(ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex), ReplayEventLogConstants.Tuple_IsamException, ex, new string[]
							{
								ex.Message,
								string.Empty
							});
						}
						catch (LogInspectorFailedGeneralException ex2)
						{
							this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogInspectorFailed, ex2, new string[]
							{
								ex2.FileName,
								ex2.SpecificError
							});
						}
						catch (ReplicaInstance.OperationAbortedDueToAdminSuspendException)
						{
							this.TraceError("Leaving EndConfigurationChecker due to Admin Suspend requested.");
						}
						catch (OperationAbortedException)
						{
							this.TraceError("Leaving EndConfigurationChecker due to PrepareToStopCalled, higher priority lock, or AcquireLock failure.");
							this.CurrentContext.RestartInstanceSoon(true);
						}
						finally
						{
							this.ReleaseSuspendLockForConfigChecker();
						}
						if (this.IsBroken)
						{
							this.PrepareToStop();
						}
					}
				}
			}
			catch (MonitoredDatabaseInitException ex3)
			{
				this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedTransient, ex3, new string[]
				{
					ex3.Message
				});
			}
			catch (TransientException ex4)
			{
				this.CurrentContext.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedTransient, ex4, new string[]
				{
					ex4.ToString()
				});
			}
			catch (ADExternalException ex5)
			{
				this.CurrentContext.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_ConfigurationCheckerFailedADError, ex5, new string[]
				{
					ex5.ToString()
				});
			}
		}

		public void MarkRestartSoonFlag()
		{
			this.m_currentContext.RestartSoon = true;
		}

		protected abstract void StartComponents();

		public void PrepareToStop()
		{
			IStartStop[] array = null;
			lock (this)
			{
				if (!this.m_fPrepareToStopCalled)
				{
					this.PrepareToStopTime = DateTime.UtcNow;
					this.m_fPrepareToStopCalled = true;
					ReplayCrimsonEvents.CopyStopRequested.Log<Guid, string, string, string>(this.Configuration.DatabaseGuid, this.Configuration.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, Environment.StackTrace);
					this.PrepareToStopInternal();
					array = this.m_components.ToArray();
				}
			}
			if (this.IsThirdPartyReplicationEnabled && this.IsTarget)
			{
				return;
			}
			this.FileChecker.PrepareToStop();
			if (array != null)
			{
				foreach (IStartStop startStop in array)
				{
					startStop.PrepareToStop();
				}
			}
			this.PrepareToStopCalledEvent.Set();
		}

		internal void CheckHealth()
		{
			if (this.PrepareToStopCalled && !this.CurrentContext.InAttemptCopyLastLogs)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.HungCopyLimitInSec);
				TimeSpan timeSpan2 = DateTime.UtcNow - this.PrepareToStopTime;
				if (timeSpan2 >= timeSpan)
				{
					string arg = string.Format("PrepareToStop set but copy still not restarted. Hung for {0} and limit is {1}", timeSpan2, timeSpan);
					ReplicaInstance.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Copy({0}) is hung: {1}", this.DatabaseName, arg);
					if (this.IsDisconnected)
					{
						ReplicaInstance.Tracer.TraceError((long)this.GetHashCode(), "Disconnected too long. Marking for restart");
						this.m_currentContext.RestartInstanceSoonAdminVisible();
					}
				}
			}
		}

		protected abstract void PrepareToStopInternal();

		public TComponent GetComponent<TComponent>() where TComponent : IStartStop
		{
			foreach (IStartStop startStop in this.m_components)
			{
				if (startStop.GetType() == typeof(TComponent))
				{
					return (TComponent)((object)startStop);
				}
			}
			return default(TComponent);
		}

		public virtual void Stop()
		{
			try
			{
				this.WaitForConfigCheckerToStopIfNecessary();
				IStartStop[] array = null;
				lock (this)
				{
					DiagCore.RetailAssert(null == this.m_configurationCheckIdleEvent, "m_configurationCheckIdleEvent was set. SetAndClearIdleEvent wasn't called?", new object[0]);
				}
				this.StopInternal();
				if (!this.IsThirdPartyReplicationEnabled || !this.IsTarget)
				{
					lock (this)
					{
						array = this.m_components.ToArray();
					}
					foreach (IStartStop startStop in array)
					{
						startStop.Stop();
					}
					if (this.AcllAutoLockReleaseInstance != null)
					{
						this.AcllAutoLockReleaseInstance.Stop();
						this.AcllAutoLockReleaseInstance = null;
					}
					lock (this)
					{
						this.m_fStarted = false;
						this.m_fStopped = true;
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstance.Stop(): {0}", this.Configuration.Name);
					}
				}
			}
			finally
			{
				this.Dispose();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing && this.PrepareToStopCalledEvent != null)
					{
						this.PrepareToStopCalledEvent.Close();
					}
					this.m_fDisposed = true;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReplicaInstance>(this);
		}

		protected abstract void StopInternal();

		public ReplayConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
			protected set
			{
				this.m_configuration = value;
			}
		}

		public IPerfmonCounters PerfmonCounters
		{
			get
			{
				return this.PerformanceCounters;
			}
		}

		protected FileChecker FileChecker
		{
			get
			{
				return this.m_fileChecker;
			}
			set
			{
				this.m_fileChecker = value;
			}
		}

		internal AcllAutoLockRelease AcllAutoLockReleaseInstance { get; set; }

		public string Identity
		{
			get
			{
				return this.Configuration.Identity;
			}
		}

		internal bool Started
		{
			get
			{
				return this.m_fStarted;
			}
		}

		protected bool StartedComponents
		{
			get
			{
				return this.m_components.Count > 0;
			}
		}

		public bool PrepareToStopCalled
		{
			get
			{
				return this.m_fPrepareToStopCalled;
			}
		}

		public bool Stopped
		{
			get
			{
				return this.m_fStopped;
			}
		}

		public bool IsBroken
		{
			get
			{
				return this.m_currentContext.IsBroken;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				return this.m_currentContext.IsDisconnected;
			}
		}

		public ReplicaInstanceContext CurrentContext
		{
			get
			{
				return this.m_currentContext;
			}
		}

		public ReplicaInstanceContextMinimal PreviousContext
		{
			get
			{
				return this.m_previousContext;
			}
		}

		protected DateTime ZeroFileTime
		{
			get
			{
				return ReplayState.ZeroFileTime;
			}
		}

		public DateTime PrepareToStopTime { get; private set; }

		internal virtual void RequestSuspendAndFail(ReplicaInstanceRequestSuspendAndFail op)
		{
			Exception ex = null;
			try
			{
				if (op.SuspendCopy && (!op.PreserveExistingError || !this.CurrentContext.Suspended))
				{
					this.Configuration.ReplayState.ActionInitiator = ActionInitiatorType.Service;
					this.RequestReplicationSuspend(op.SuspendComment);
				}
				if (op.BlockReseed)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Setting ReseedBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
					this.Configuration.ReplayState.ReseedBlocked = true;
					if (!this.Configuration.ReplayState.ReseedBlocked)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Error occurred setting ReseedBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
						throw new ReplayServiceSuspendReseedBlockedException();
					}
				}
				if (op.BlockResume)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Setting ResumeBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
					this.Configuration.ReplayState.ResumeBlocked = true;
					if (!this.Configuration.ReplayState.ResumeBlocked)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Error occurred setting ResumeBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
						throw new ReplayServiceSuspendResumeBlockedException();
					}
				}
				if (op.BlockInPlaceReseed)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Setting InPlaceReseedBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
					this.Configuration.ReplayState.InPlaceReseedBlocked = true;
					if (!this.Configuration.ReplayState.InPlaceReseedBlocked)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "RequestSuspendAndFail(): {0} ({1}): Error occurred setting InPlaceReseedBlocked to 'true'.", this.m_configuration.DisplayName, this.m_configuration.Identity);
						throw new ReplayServiceSuspendInPlaceReseedBlockedException();
					}
				}
			}
			catch (SuspendWantedWriteFailedException innerException)
			{
				ex = new ReplayServiceSuspendRpcFailedException(innerException);
			}
			catch (SuspendMessageWriteFailedException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "RequestSuspendAndFail(): Ignoring exception: {0}", ex2.ToString());
			}
			catch (TaskServerException ex3)
			{
				ex = ex3;
			}
			catch (TaskServerTransientException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "RequestSuspendAndFail(): failed to suspend: {0}", ex.ToString());
				throw ex;
			}
			if (!op.PreserveExistingError || !this.CurrentContext.IsBroken)
			{
				lock (this)
				{
					if (!op.SuspendCopy)
					{
						this.CurrentContext.DoNotRestart = true;
					}
					this.CurrentContext.PersistFailure(op.ErrorEventId, new LocalizedString(op.ErrorMessage));
				}
			}
			SourceSeedTable.Instance.CancelSeedingIfAppropriate(SourceSeedTable.CancelReason.CopySuspended, this.Configuration.IdentityGuid);
			if (op.SuspendCopy)
			{
				this.SuspendContentIndexing();
			}
			this.PrepareToStop();
		}

		internal virtual void RequestSuspend(string suspendComment, DatabaseCopyActionFlags flags, ActionInitiatorType initiator)
		{
			Exception ex = null;
			try
			{
				this.Configuration.ReplayState.ActionInitiator = initiator;
				if ((flags & DatabaseCopyActionFlags.Activation) == DatabaseCopyActionFlags.Activation)
				{
					ReplicaInstance.SuspendActivation(this.Configuration.ReplayState, suspendComment);
				}
				if ((flags & DatabaseCopyActionFlags.Replication) == DatabaseCopyActionFlags.Replication)
				{
					string activeServerForDatabase = ReplicaInstance.GetActiveServerForDatabase(this.Configuration);
					bool flag = Cluster.StringIEquals(activeServerForDatabase, Dependencies.ManagementClassHelper.LocalMachineName);
					if (flag)
					{
						throw new ReplayServiceSuspendRpcInvalidForActiveCopyException(this.Configuration.DisplayName);
					}
					if (this.CurrentContext.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.None)
					{
						throw new ReplayServiceSuspendRpcInvalidSeedingSourceException(this.Configuration.DisplayName);
					}
					this.RequestReplicationSuspend(suspendComment);
					this.StopMonitoredDatabase();
					ex = this.SuspendContentIndexing();
				}
			}
			catch (SuspendWantedWriteFailedException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "RequestSuspend(): Caught exception: {0}", ex2.ToString());
				throw new ReplayServiceSuspendRpcFailedException(ex2);
			}
			catch (SuspendMessageWriteFailedException ex3)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "RequestSuspend(): Caught exception: {0}", ex3.ToString());
				throw new ReplayServiceSuspendCommentException(ex3);
			}
			if (ex != null)
			{
				string errMsg = string.Format("{0}:{1}", ex.GetType(), ex.Message);
				ReplayCrimsonEvents.RpcExSearchFailed.Log<Guid, string, string, string, string>(this.Configuration.DatabaseGuid, this.Configuration.DatabaseName, "RequestSuspend", Dependencies.ManagementClassHelper.LocalMachineName, ex.ToString());
				throw new ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(errMsg);
			}
		}

		internal virtual void RequestResume(DatabaseCopyActionFlags flags)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string, DatabaseCopyActionFlags>((long)this.GetHashCode(), "RequestResume() called for DB '{0}' ({1}) with flags: {2}", this.Configuration.DisplayName, this.Configuration.Identity, flags);
			bool flag = (flags & DatabaseCopyActionFlags.ActiveCopy) == DatabaseCopyActionFlags.ActiveCopy;
			bool flag2 = (flags & DatabaseCopyActionFlags.SyncSuspendResume) == DatabaseCopyActionFlags.SyncSuspendResume;
			bool skipSettingResumeAutoReseedState = (flags & DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState) == DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState;
			if ((flags & DatabaseCopyActionFlags.Activation) == DatabaseCopyActionFlags.Activation)
			{
				ReplicaInstance.ResumeActivation(this.Configuration.ReplayState);
			}
			if ((flags & DatabaseCopyActionFlags.Replication) == DatabaseCopyActionFlags.Replication)
			{
				if (!this.RequestReplicationResume(flag, flag2, skipSettingResumeAutoReseedState))
				{
					throw new ReplayServiceResumeRpcFailedException();
				}
				if (flag)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "Skipped mounting the content index catalog as part of RequestResume() because the copy is the active.");
					return;
				}
				if (this.CurrentContext.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.None)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "Skipped mounting the content index catalog because the copy is currently a seeding source. It will be resumed after seeding is finished.");
					return;
				}
				if (flag2)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "Skipped mounting the content index catalog because RequestResume is called with 'syncOnly == true'.");
					return;
				}
				Exception ex = this.ResumeContentIndexing();
				if (ex != null)
				{
					ReplayCrimsonEvents.RpcExSearchFailed.Log<Guid, string, string, string, string>(this.Configuration.DatabaseGuid, this.Configuration.DatabaseName, "RequestResume", Dependencies.ManagementClassHelper.LocalMachineName, ex.ToString());
					throw new ReplayServiceResumeRpcPartialSuccessCatalogFailedException(ex.Message);
				}
			}
		}

		internal void SyncSuspendResumeState()
		{
			DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.SyncSuspendResume | DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState;
			ActionInitiatorType actionInitiator = this.Configuration.ReplayState.ActionInitiator;
			ActionInitiatorType initiator = (actionInitiator == ActionInitiatorType.Unknown) ? ActionInitiatorType.Service : actionInitiator;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncSuspendResumeState() called for database copy '{0}'", this.Configuration.DisplayName);
			if (this.CurrentContext.Suspended)
			{
				this.RequestSuspend(null, flags, initiator);
			}
			else
			{
				this.RequestResume(flags);
			}
			flags = (DatabaseCopyActionFlags.Activation | DatabaseCopyActionFlags.SyncSuspendResume);
			if (this.Configuration.ReplayState.ActivationSuspended)
			{
				this.RequestSuspend(null, flags, initiator);
				return;
			}
			this.RequestResume(flags);
		}

		public void DisableReplayLag(string reason, ActionInitiatorType actionInitiator)
		{
			ReplayState replayState = this.Configuration.ReplayState;
			if (this.m_configuration.ReplayLagTime == EnhancedTimeSpan.Zero)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "DisableReplayLag( {0} ): Skipping because this copy has no replay lag configured!", this.m_configuration.DisplayName);
				return;
			}
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "DisableReplayLag( {0} ): Disabling replay lag due to reason: {1}", this.m_configuration.DisplayName, reason);
			replayState.ReplayLagDisabled = true;
			if (!replayState.ReplayLagDisabled)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "DisableReplayLag( {0} ): Disabling replay lag failed to set registry value.", this.m_configuration.DisplayName);
				throw new DisableReplayLagWriteFailedException(this.m_configuration.DisplayName);
			}
			replayState.ReplayLagDisabledReason = reason;
			replayState.ReplayLagActionInitiator = actionInitiator;
			LogReplayer component = this.GetComponent<LogReplayer>();
			if (component != null)
			{
				component.DisableReplayLag();
			}
		}

		public void EnableReplayLag(ActionInitiatorType actionInitiator)
		{
			ReplayState replayState = this.Configuration.ReplayState;
			if (this.m_configuration.ReplayLagTime == EnhancedTimeSpan.Zero)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "EnableReplayLag( {0} ): Skipping because this copy has no replay lag configured!", this.m_configuration.DisplayName);
				return;
			}
			if (replayState.ReplayLagActionInitiator == ActionInitiatorType.Administrator && actionInitiator != ActionInitiatorType.Administrator)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "EnableReplayLag( {0} ): Replay lag cannot be enabled because the lag has been disabled by an administrative action!", this.m_configuration.DisplayName);
				throw new EnableReplayLagAlreadyDisabledFailedException(this.m_configuration.DisplayName);
			}
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "EnableReplayLag( {0} ): Re-instating replay lag!", this.m_configuration.DisplayName);
			replayState.ReplayLagDisabled = false;
			replayState.ReplayLagDisabledReason = null;
			replayState.ReplayLagActionInitiator = actionInitiator;
			LogReplayer component = this.GetComponent<LogReplayer>();
			if (component != null)
			{
				component.EnableReplayLag();
			}
		}

		private void RequestReplicationSuspend(string comment)
		{
			ReplayState replayState = this.Configuration.ReplayState;
			StateLock suspendLock = replayState.SuspendLock;
			StateLockRemote suspendLockRemote = replayState.SuspendLockRemote;
			bool flag = false;
			lock (this)
			{
				if (suspendLock.CurrentOwner == LockOwner.Suspend)
				{
					flag = true;
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "RequestSuspend: Instance is already suspended. Leaving RPC.");
					this.CurrentContext.SetSuspended();
					ReplicaInstance.WriteSuspendComment(replayState, comment);
				}
			}
			if (flag)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3223727421U);
			}
			else
			{
				suspendLockRemote.EnterSuspend();
				if (this.IsTarget)
				{
					this.m_currentContext.RestartInstanceSoon(true);
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3311807805U);
				LockOwner lockOwner;
				if (!suspendLock.TryEnterSuspend(true, out lockOwner))
				{
					suspendLockRemote.TryLeaveSuspend();
					if (lockOwner == LockOwner.AttemptCopyLastLogs)
					{
						throw new ReplayServiceSuspendBlockedAcllException();
					}
					if (lockOwner == LockOwner.Backup)
					{
						throw new ReplayServiceSuspendBlockedBackupInProgressException();
					}
					if (lockOwner == LockOwner.Component)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<ReplicaInstanceStage, CopyStatusEnum>((long)this.GetHashCode(), "RequestSuspend: Instance could not be suspended because 'Component' owns the lock. RI stage is: '{0}'. CopyStatus is: '{1}'", this.CurrentContext.ProgressStage, this.CurrentContext.GetStatus());
						throw new ReplayServiceSuspendBlockedResynchronizingException();
					}
					throw new ReplayServiceSuspendRpcFailedException();
				}
				else
				{
					lock (this)
					{
						if (suspendLock.CurrentOwner != LockOwner.Suspend)
						{
							throw new ReplayServiceSuspendRpcFailedException();
						}
						ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "RequestSuspend: Instance has been successfully suspended.");
						this.CurrentContext.SetSuspended();
						ReplicaInstance.WriteSuspendComment(replayState, comment);
					}
				}
			}
			if (this.IsTarget)
			{
				TargetReplicaInstance.EnsureTargetDismounted(this.Configuration);
			}
		}

		private bool RequestReplicationResume(bool isActiveCopy, bool syncOnly, bool skipSettingResumeAutoReseedState)
		{
			bool result = false;
			bool flag = false;
			ReplayState replayState = this.Configuration.ReplayState;
			StateLock suspendLock = replayState.SuspendLock;
			StateLockRemote suspendLockRemote = replayState.SuspendLockRemote;
			lock (this)
			{
				if (replayState.ResumeBlocked)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<string>((long)this.GetHashCode(), "RequestResume {0}: Resume failed because ResumeBlocked = true", this.configName);
					throw new ReplayServiceResumeBlockedException(AmExceptionHelper.GetMessageOrNoneString(replayState.ConfigBrokenMessage));
				}
				if (!isActiveCopy && replayState.ReseedBlocked)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<string>((long)this.GetHashCode(), "RequestResume {0}: Resume failed because ResumeBlocked = true", this.configName);
					throw new ReplayServiceResumeBlockedException(AmExceptionHelper.GetMessageOrNoneString(replayState.ConfigBrokenMessage));
				}
				replayState.ReseedBlocked = false;
				if (suspendLock.CurrentOwner != LockOwner.Suspend)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "RequestResume: Instance is already resumed. Leaving RPC.");
					this.CurrentContext.ClearSuspended(isActiveCopy, false, syncOnly);
					replayState.SuspendMessage = null;
					result = true;
					if (!syncOnly)
					{
						flag = true;
					}
				}
				else
				{
					if (this.CurrentContext.Seeding)
					{
						ExTraceGlobals.ReplayServiceRpcTracer.TraceError<string>((long)this.GetHashCode(), "RequestResume {0}: Resume failed due to a seed in progress.", this.configName);
						throw new ReplayServiceResumeRpcFailedSeedingException();
					}
					if (!skipSettingResumeAutoReseedState && this.CurrentContext.GetStatus() == CopyStatusEnum.FailedAndSuspended)
					{
						Exception ex = AutoReseedWorkflowState.WriteManualWorkflowExecutionState(this.Configuration.IdentityGuid, AutoReseedWorkflowType.ManualResume);
						if (ex != null)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceError<string, Exception>((long)this.GetHashCode(), "RequestResume {0}: RegistryParameterException while writing AutoReseed registry state to mark that the resume action is starting: {1}", this.configName, ex);
						}
					}
					if (suspendLockRemote.TryLeaveSuspend())
					{
						suspendLock.LeaveSuspend();
						ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "RequestResume: Instance has been successfully resumed.");
						this.CurrentContext.ClearSuspended(isActiveCopy, true, syncOnly);
						if (!replayState.ActivationSuspended)
						{
							replayState.SuspendMessage = null;
						}
						replayState.LogRepairRetryCount = 0L;
						flag = true;
						result = true;
					}
					else
					{
						ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "RequestResume: Instance was not successfully resumed. suspendLockRemote.TryLeaveSuspend() failed!");
					}
				}
			}
			if (flag)
			{
				this.m_currentContext.RestartInstanceSoonAdminVisible();
			}
			return result;
		}

		private Exception SuspendContentIndexing()
		{
			Exception ex = null;
			Guid identityGuid = this.Configuration.IdentityGuid;
			ReplicaInstance.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "RI.SuspendContentIndexing() called for DB {0}({1}).", this.Configuration.DisplayName, identityGuid);
			TimeSpan invokeTimeout = TimeSpan.FromSeconds((double)RegistryParameters.CISuspendResumeTimeoutInSec);
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					ex = ReplicaInstance.UpdateReplicationContentIndexing();
				}, invokeTimeout);
			}
			catch (TimeoutException ex)
			{
				TimeoutException ex2;
				ex = ex2;
			}
			if (ex != null)
			{
				ReplicaInstance.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "RI.SuspendContentIndexing() failed for {0} : {1}", this.Configuration.DisplayName, ex);
			}
			return ex;
		}

		private Exception ResumeContentIndexing()
		{
			Exception ex = null;
			Guid identityGuid = this.Configuration.IdentityGuid;
			ReplicaInstance.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "RI.ResumeContentIndexing() called for DB {0}({1}).", this.Configuration.DisplayName, identityGuid);
			TimeSpan invokeTimeout = TimeSpan.FromSeconds((double)RegistryParameters.CISuspendResumeTimeoutInSec);
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					ex = ReplicaInstance.UpdateReplicationContentIndexing();
				}, invokeTimeout);
			}
			catch (TimeoutException ex)
			{
				TimeoutException ex2;
				ex = ex2;
			}
			if (ex != null)
			{
				ReplicaInstance.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "RI.ResumeContentIndexing() failed for {0} : {1}", this.Configuration.DisplayName, ex);
			}
			return ex;
		}

		internal static Exception UpdateReplicationContentIndexing()
		{
			Exception result = null;
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug(0L, "RI.UpdateReplicationContentIndexing() is called");
			SearchServiceRpcClient searchServiceRpcClient = null;
			bool discard = false;
			try
			{
				searchServiceRpcClient = RpcConnectionPool.GetSearchRpcClient();
				searchServiceRpcClient.UpdateIndexSystems();
			}
			catch (RpcException ex)
			{
				discard = true;
				result = ex;
			}
			finally
			{
				if (searchServiceRpcClient != null)
				{
					RpcConnectionPool.ReturnSearchRpcClientToCache(ref searchServiceRpcClient, discard);
				}
			}
			return result;
		}

		protected void StopMonitoredDatabase()
		{
			RemoteDataProvider.StopMonitoredDatabase(this.Configuration.Identity);
		}

		internal RpcDatabaseCopyStatus2 GetCopyStatus(RpcGetDatabaseCopyStatusFlags2 collectionFlags)
		{
			this.PerfmonCounters.RecordOneGetCopyStatusCall();
			MountStatus? mountStatus = null;
			string activeServerForDatabase = ReplicaInstance.GetActiveServerForDatabase(this.Configuration, out mountStatus);
			bool flag = Cluster.StringIEquals(activeServerForDatabase, Dependencies.ManagementClassHelper.LocalMachineName);
			RpcDatabaseCopyStatus2 result;
			if (this.Configuration.Type == ReplayConfigType.SingleCopySource)
			{
				result = this.SingleCopyGetCopyStatus(activeServerForDatabase, mountStatus);
			}
			else if (flag)
			{
				result = this.SourceGetCopyStatus(activeServerForDatabase, collectionFlags, mountStatus);
			}
			else if (this.Configuration.Type == ReplayConfigType.RemoteCopySource)
			{
				result = this.SourceGetCopyStatus(activeServerForDatabase, collectionFlags, mountStatus);
			}
			else
			{
				result = this.TargetGetCopyStatus(activeServerForDatabase, collectionFlags);
			}
			return result;
		}

		internal RpcDatabaseCopyStatus2 SingleCopyGetCopyStatus(string activeServer, MountStatus? mountStatus)
		{
			RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus = new RpcDatabaseCopyStatus2();
			this.UpdateCopyStatusCommon(rpcDatabaseCopyStatus, activeServer);
			if (!ReplicaInstance.UpdateDatabaseMountStatus(rpcDatabaseCopyStatus, this.Configuration.IdentityGuid, this, activeServer, mountStatus))
			{
				rpcDatabaseCopyStatus.CopyStatus = CopyStatusEnum.Dismounted;
				return rpcDatabaseCopyStatus;
			}
			if (rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Mounted)
			{
				try
				{
					using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null))
					{
						int num;
						int minimumSupportedDatabaseSchemaVersion;
						int maximumSupportedDatabaseSchemaVersion;
						int requestedDatabaseSchemaVersion;
						newStoreControllerInstance.GetDatabaseProcessInfo(this.Configuration.IdentityGuid, out num, out minimumSupportedDatabaseSchemaVersion, out maximumSupportedDatabaseSchemaVersion, out requestedDatabaseSchemaVersion);
						rpcDatabaseCopyStatus.MinimumSupportedDatabaseSchemaVersion = minimumSupportedDatabaseSchemaVersion;
						rpcDatabaseCopyStatus.MaximumSupportedDatabaseSchemaVersion = maximumSupportedDatabaseSchemaVersion;
						rpcDatabaseCopyStatus.RequestedDatabaseSchemaVersion = requestedDatabaseSchemaVersion;
					}
				}
				catch (MapiPermanentException arg)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError<MapiPermanentException>(0L, "SingleCopyGetCopyStatus MapiPermanentException:{0}", arg);
				}
				catch (MapiRetryableException arg2)
				{
					ExTraceGlobals.LogReplayerTracer.TraceError<MapiRetryableException>(0L, "SingleCopyGetCopyStatus MapiRetryableException:{0}", arg2);
				}
			}
			return rpcDatabaseCopyStatus;
		}

		internal RpcDatabaseCopyStatus2 SourceGetCopyStatus(string activeServer, RpcGetDatabaseCopyStatusFlags2 collectionFlags, MountStatus? mountStatus)
		{
			RpcDatabaseCopyStatus2 copyStatus = new RpcDatabaseCopyStatus2();
			this.UpdateCopyStatusCommon(copyStatus, activeServer);
			this.UpdateDatabaseSchemaInfo(copyStatus);
			if (!ReplicaInstance.UpdateDatabaseMountStatus(copyStatus, this.Configuration.IdentityGuid, this, activeServer, mountStatus))
			{
				if (this.IsThirdPartyReplicationEnabled)
				{
					copyStatus.CopyStatus = CopyStatusEnum.NonExchangeReplication;
				}
				else
				{
					copyStatus.CopyStatus = CopyStatusEnum.Initializing;
				}
				return copyStatus;
			}
			ReplayState state = this.Configuration.ReplayState;
			copyStatus.ErrorEventId = (uint)state.ConfigBrokenEventId;
			copyStatus.ErrorMessage = state.ConfigBrokenMessage;
			MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(this.Configuration.ServerName, this.Configuration.DatabaseGuid);
			if (monitoredDatabase != null)
			{
				EndOfLog currentEndOfLog = monitoredDatabase.CurrentEndOfLog;
				copyStatus.LastLogInfoFromCopierTime = DateTime.UtcNow;
				copyStatus.LastLogCopyNotified = currentEndOfLog.Generation;
				copyStatus.LastCopyNotifiedLogTime = currentEndOfLog.Utc;
				copyStatus.LowestLogPresent = monitoredDatabase.StartOfLogGeneration;
				Exception ex = AmHelper.RunAmClusterOperation(delegate
				{
					copyStatus.LastLogInfoFromClusterGen = state.GetLastLogCommittedGenerationNumberFromCluster();
					bool flag = false;
					copyStatus.LastLogInfoFromClusterTime = (DateTime)state.GetLatestLogGenerationTimeStampFromCluster(out flag);
				});
				if (ex != null)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<Exception>((long)this.GetHashCode(), "ReplicaInstance.SourceGetCopyStatus hit cluster ex: {0}", ex);
				}
				copyStatus.LastLogGenerated = Math.Max(copyStatus.LastLogCopyNotified, copyStatus.LastLogInfoFromClusterGen);
			}
			if (this.IsThirdPartyReplicationEnabled)
			{
				return copyStatus;
			}
			if (monitoredDatabase != null)
			{
				monitoredDatabase.CollectConnectionStatus(copyStatus);
			}
			copyStatus.SeedingSource = this.CurrentContext.ActiveSeedingSource;
			copyStatus.SeedingSourceForDB = this.CurrentContext.IsSeedingSourceForDB;
			copyStatus.SeedingSourceForCI = this.CurrentContext.IsSeedingSourceForCI;
			copyStatus.ReplicationIsInBlockMode = true;
			return copyStatus;
		}

		private IADServer GetCachedLocalADServer()
		{
			try
			{
				IMonitoringADConfig config = Dependencies.MonitoringADConfigProvider.GetConfig(true);
				if (config != null)
				{
					return config.LookupMiniServerByName(AmServerName.LocalComputerName);
				}
			}
			catch (MonitoringADConfigException arg)
			{
				ReplicaInstance.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "GetCachedLocalADServer failed to get AD config: {0}", arg);
			}
			return this.Configuration.GetAdServerObject();
		}

		internal RpcDatabaseCopyStatus2 TargetGetCopyStatus(string activeServer, RpcGetDatabaseCopyStatusFlags2 collectionFlags)
		{
			RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus = new RpcDatabaseCopyStatus2();
			this.UpdateCopyStatusCommon(rpcDatabaseCopyStatus, activeServer);
			this.UpdateDatabaseSchemaInfo(rpcDatabaseCopyStatus);
			if (this.IsThirdPartyReplicationEnabled && this.IsTarget)
			{
				rpcDatabaseCopyStatus.CopyStatus = CopyStatusEnum.NonExchangeReplication;
				return rpcDatabaseCopyStatus;
			}
			ReplayState replayState = this.Configuration.ReplayState;
			ReplicaInstanceContext currentContext = this.CurrentContext;
			ReplicaInstanceContextMinimal previousContext = this.PreviousContext;
			lock (this)
			{
				rpcDatabaseCopyStatus.CopyStatus = currentContext.GetStatus();
				rpcDatabaseCopyStatus.LastStatusTransitionTime = this.ToNonNullableDateTime(new DateTime?(replayState.LastStatusTransitionTime));
				if (rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Failed || rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended || rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndHealthy || rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndResynchronizing)
				{
					uint errorEventId = 0U;
					string errorMessage = null;
					ExtendedErrorInfo extendedErrorInfo = null;
					if (currentContext.IsBroken || currentContext.IsDisconnected)
					{
						errorEventId = currentContext.ErrorEventId;
						errorMessage = currentContext.ErrorMessage;
						extendedErrorInfo = currentContext.ExtendedErrorInfo;
					}
					else if (previousContext != null && (previousContext.FailureInfo.IsFailed || previousContext.FailureInfo.IsDisconnected))
					{
						errorEventId = previousContext.FailureInfo.ErrorEventId;
						errorMessage = previousContext.FailureInfo.ErrorMessage;
						extendedErrorInfo = previousContext.FailureInfo.ExtendedErrorInfo;
					}
					rpcDatabaseCopyStatus.ErrorEventId = errorEventId;
					rpcDatabaseCopyStatus.ErrorMessage = errorMessage;
					rpcDatabaseCopyStatus.ExtendedErrorInfo = extendedErrorInfo;
				}
				if (rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Suspended || rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended || rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Seeding)
				{
					rpcDatabaseCopyStatus.SuspendComment = replayState.SuspendMessage;
				}
				rpcDatabaseCopyStatus.Viable = currentContext.Viable;
				rpcDatabaseCopyStatus.LatestAvailableLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestCopyNotificationTime));
				rpcDatabaseCopyStatus.LastCopyNotifiedLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestCopyNotificationTime));
				rpcDatabaseCopyStatus.LastCopiedLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestCopyTime));
				rpcDatabaseCopyStatus.LastLogCopyNotified = replayState.CopyNotificationGenerationNumber;
				rpcDatabaseCopyStatus.LastLogCopied = replayState.CopyGenerationNumber;
				rpcDatabaseCopyStatus.LastInspectedLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestInspectorTime));
				rpcDatabaseCopyStatus.LastLogInspected = replayState.InspectorGenerationNumber;
				rpcDatabaseCopyStatus.LastReplayedLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestReplayTime));
				rpcDatabaseCopyStatus.CurrentReplayLogTime = this.ToNonNullableDateTime(new DateTime?(replayState.CurrentReplayTime));
				rpcDatabaseCopyStatus.ReplaySuspended = replayState.ReplaySuspended;
				rpcDatabaseCopyStatus.LastLogReplayed = replayState.ReplayGenerationNumber;
				rpcDatabaseCopyStatus.LowestLogPresent = this.FileChecker.FileState.LowestGenerationPresent;
				rpcDatabaseCopyStatus.DataProtectionTime = rpcDatabaseCopyStatus.LastInspectedLogTime;
				rpcDatabaseCopyStatus.DataAvailabilityTime = rpcDatabaseCopyStatus.LastReplayedLogTime;
				if (currentContext.PassiveSeedingSourceContext == PassiveSeedingSourceContextEnum.Database)
				{
					rpcDatabaseCopyStatus.SeedingSource = true;
				}
				rpcDatabaseCopyStatus.SeedingSourceForDB = currentContext.IsSeedingSourceForDB;
				rpcDatabaseCopyStatus.SeedingSourceForCI = currentContext.IsSeedingSourceForCI;
				this.UpdateReplayActualLagStatus(rpcDatabaseCopyStatus);
				rpcDatabaseCopyStatus.StatusRetrievedTime = DateTime.UtcNow;
			}
			lock (this.m_lastloggeneratedlocker)
			{
				this.UpdateLastLogGenerated(rpcDatabaseCopyStatus, replayState);
				rpcDatabaseCopyStatus.CopyQueueNotKeepingUp = (rpcDatabaseCopyStatus.GetCopyQueueLength() > 0L && this.PerfmonCounters.CopyQueueNotKeepingUp > 0L);
				rpcDatabaseCopyStatus.ReplayQueueNotKeepingUp = (rpcDatabaseCopyStatus.GetReplayQueueLength() > 0L && this.PerfmonCounters.ReplayQueueNotKeepingUp > 0L);
			}
			if (rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Seeding)
			{
				ReplicaSeederPerfmonInstance instance = ReplicaSeederPerfmon.GetInstance(this.Configuration.Name);
				rpcDatabaseCopyStatus.DbSeedingPercent = (int)instance.DbSeedingProgress.RawValue;
				rpcDatabaseCopyStatus.DbSeedingKBytesRead = instance.DbSeedingBytesRead.RawValue;
				rpcDatabaseCopyStatus.DbSeedingKBytesWritten = instance.DbSeedingBytesWritten.RawValue;
				rpcDatabaseCopyStatus.DbSeedingKBytesReadPerSec = instance.DbSeedingBytesReadPerSecond.NextValue();
				rpcDatabaseCopyStatus.DbSeedingKBytesWrittenPerSec = instance.DbSeedingBytesWrittenPerSecond.NextValue();
			}
			rpcDatabaseCopyStatus.LatestFullBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestFullBackupTime));
			rpcDatabaseCopyStatus.LatestIncrementalBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestIncrementalBackupTime));
			rpcDatabaseCopyStatus.LatestDifferentialBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestDifferentialBackupTime));
			rpcDatabaseCopyStatus.LatestCopyBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestCopyBackupTime));
			if (rpcDatabaseCopyStatus.LatestFullBackupTime > this.DefaultTime)
			{
				rpcDatabaseCopyStatus.SnapshotLatestFullBackup = replayState.SnapshotLatestFullBackup;
			}
			if (rpcDatabaseCopyStatus.LatestIncrementalBackupTime > this.DefaultTime)
			{
				rpcDatabaseCopyStatus.SnapshotLatestIncrementalBackup = replayState.SnapshotLatestIncrementalBackup;
			}
			if (rpcDatabaseCopyStatus.LatestDifferentialBackupTime > this.DefaultTime)
			{
				rpcDatabaseCopyStatus.SnapshotLatestDifferentialBackup = replayState.SnapshotLatestDifferentialBackup;
			}
			if (rpcDatabaseCopyStatus.LatestCopyBackupTime > this.DefaultTime)
			{
				rpcDatabaseCopyStatus.SnapshotLatestCopyBackup = replayState.SnapshotLatestCopyBackup;
			}
			LogCopier component = this.GetComponent<LogCopier>();
			if (component != null)
			{
				component.CollectConnectionStatus(rpcDatabaseCopyStatus);
				if (component.PassiveBlockMode.IsBlockModeActive)
				{
					rpcDatabaseCopyStatus.ReplicationIsInBlockMode = true;
				}
			}
			LogReplayer component2 = this.GetComponent<LogReplayer>();
			if (component2 != null)
			{
				rpcDatabaseCopyStatus.MaxLogToReplay = component2.MaxLogToReplay;
			}
			rpcDatabaseCopyStatus.LogsReplayedSinceInstanceStart = currentContext.LogsReplayedSinceInstanceStart;
			rpcDatabaseCopyStatus.LogsCopiedSinceInstanceStart = currentContext.LogsCopiedSinceInstanceStart;
			return rpcDatabaseCopyStatus;
		}

		private void UpdateCopyStatusCommon(RpcDatabaseCopyStatus2 copyStatus, string activeServer)
		{
			ReplayState replayState = this.Configuration.ReplayState;
			copyStatus.StatusRetrievedTime = DateTime.UtcNow;
			copyStatus.InstanceStartTime = this.m_instanceCreateTime;
			copyStatus.DBGuid = this.Configuration.IdentityGuid;
			copyStatus.DBName = this.Configuration.DatabaseName;
			copyStatus.MailboxServer = Dependencies.ManagementClassHelper.LocalMachineName;
			copyStatus.ServerVersion = this.Configuration.ServerVersion;
			copyStatus.ActiveDatabaseCopy = activeServer.ToUpperInvariant();
			copyStatus.WorkerProcessId = replayState.WorkerProcessId;
			copyStatus.ActivationPreference = this.Configuration.ActivationPreference;
			copyStatus.NodeStatus = (ActiveManagerCore.IsLocalNodePubliclyUp() ? NodeUpStatusEnum.Up : NodeUpStatusEnum.Down);
			copyStatus.IsPrimaryActiveManager = ActiveManagerCore.IsLocalNodePAM();
			this.UpdateDiskSpaceStatus(copyStatus);
			this.UpdateVolumeInfo(copyStatus);
			if (this.IsThirdPartyReplicationEnabled && this.IsTarget)
			{
				copyStatus.ContentIndexStatus = ContentIndexStatusType.Unknown;
				return;
			}
			copyStatus.IsLastCopyAvailabilityChecksPassed = replayState.IsLastCopyAvailabilityChecksPassed;
			copyStatus.IsLastCopyRedundancyChecksPassed = replayState.IsLastCopyRedundancyChecksPassed;
			copyStatus.LastCopyAvailabilityChecksPassedTime = this.ToNonNullableDateTime(new DateTime?(replayState.LastCopyAvailabilityChecksPassedTime));
			copyStatus.LastCopyRedundancyChecksPassedTime = this.ToNonNullableDateTime(new DateTime?(replayState.LastCopyRedundancyChecksPassedTime));
			copyStatus.LatestFullBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestFullBackupTime));
			copyStatus.LatestIncrementalBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestIncrementalBackupTime));
			copyStatus.LatestDifferentialBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestDifferentialBackupTime));
			copyStatus.LatestCopyBackupTime = this.ToNonNullableDateTime(new DateTime?(replayState.LatestCopyBackupTime));
			if (copyStatus.LatestFullBackupTime > this.DefaultTime)
			{
				copyStatus.SnapshotLatestFullBackup = replayState.SnapshotLatestFullBackup;
			}
			if (copyStatus.LatestIncrementalBackupTime > this.DefaultTime)
			{
				copyStatus.SnapshotLatestIncrementalBackup = replayState.SnapshotLatestIncrementalBackup;
			}
			if (copyStatus.LatestDifferentialBackupTime > this.DefaultTime)
			{
				copyStatus.SnapshotLatestDifferentialBackup = replayState.SnapshotLatestDifferentialBackup;
			}
			if (copyStatus.LatestCopyBackupTime > this.DefaultTime)
			{
				copyStatus.SnapshotLatestCopyBackup = replayState.SnapshotLatestCopyBackup;
			}
			if (replayState.DumpsterRedeliveryRequired)
			{
				copyStatus.DumpsterRequired = true;
				copyStatus.DumpsterServers = replayState.DumpsterRedeliveryServers;
				copyStatus.DumpsterStartTime = replayState.DumpsterRedeliveryStartTime;
				copyStatus.DumpsterEndTime = replayState.DumpsterRedeliveryEndTime;
			}
			else
			{
				copyStatus.DumpsterRequired = false;
				copyStatus.DumpsterServers = null;
				copyStatus.DumpsterStartTime = DateTime.MinValue;
				copyStatus.DumpsterEndTime = DateTime.MinValue;
			}
			copyStatus.SinglePageRestore = replayState.SinglePageRestore;
			copyStatus.SinglePageRestoreNumber = replayState.SinglePageRestoreNumber;
			copyStatus.ActivationSuspended = replayState.ActivationSuspended;
			if (copyStatus.ActivationSuspended)
			{
				copyStatus.SuspendComment = replayState.SuspendMessage;
			}
			copyStatus.ResumeBlocked = replayState.ResumeBlocked;
			copyStatus.ReseedBlocked = replayState.ReseedBlocked;
			copyStatus.InPlaceReseedBlocked = replayState.InPlaceReseedBlocked;
			copyStatus.ActionInitiator = replayState.ActionInitiator;
			copyStatus.LostWrite = replayState.LostWrite;
			this.UpdateReplayLagStatus(copyStatus);
			this.UpdateContentIndexStatus(copyStatus);
			IADServer cachedLocalADServer = this.GetCachedLocalADServer();
			copyStatus.ActivationDisabledAndMoveNow = cachedLocalADServer.DatabaseCopyActivationDisabledAndMoveNow;
			copyStatus.AutoActivationPolicy = (int)cachedLocalADServer.DatabaseCopyAutoActivationPolicy;
			copyStatus.HAComponentOffline = (ServerComponentStates.ReadEffectiveComponentState(null, cachedLocalADServer.ComponentStates, ServerComponentStateSources.All, ServerComponentStates.GetComponentId(ServerComponentEnum.HighAvailability), ServiceState.Active) == ServiceState.Inactive);
		}

		private void UpdateDatabaseSchemaInfo(RpcDatabaseCopyStatus2 copyStatus)
		{
			Guid databaseGuid = this.Configuration.DatabaseGuid;
			Guid serverGuid = this.Configuration.GetAdServerObject().Guid;
			int minVersion = -1;
			int maxVersion = -1;
			int requestedVersion = -1;
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				ClusterDBHelpers.ReadServerDatabaseSchemaVersionRange(serverGuid, 0, 0, out minVersion, out maxVersion);
				ClusterDBHelpers.ReadRequestedDatabaseSchemaVersion(databaseGuid, 0, out requestedVersion);
			});
			if (ex != null)
			{
				ExTraceGlobals.StateTracer.TraceError<Guid, Exception>((long)this.GetHashCode(), "UpdateDatabaseSchemaInfo for db '{0}' failed to read from cluster: {1}", databaseGuid, ex);
			}
			copyStatus.MinimumSupportedDatabaseSchemaVersion = minVersion;
			copyStatus.MaximumSupportedDatabaseSchemaVersion = maxVersion;
			copyStatus.RequestedDatabaseSchemaVersion = requestedVersion;
		}

		private void UpdateVolumeInfo(RpcDatabaseCopyStatus2 copyStatus)
		{
			lock (this)
			{
				DatabaseVolumeInfo databaseVolumeInfo = this.CurrentContext.DatabaseVolumeInfo;
				ReplayState replayState = this.Configuration.ReplayState;
				if (databaseVolumeInfo.IsValid)
				{
					if (databaseVolumeInfo.IsExchangeVolumeMountPointValid)
					{
						copyStatus.ExchangeVolumeMountPoint = databaseVolumeInfo.ExchangeVolumeMountPoint.Path;
					}
					else if (replayState.VolumeInfoIsValid)
					{
						copyStatus.ExchangeVolumeMountPoint = replayState.ExchangeVolumeMountPoint;
					}
					copyStatus.DatabaseVolumeMountPoint = databaseVolumeInfo.DatabaseVolumeMountPoint.Path;
					copyStatus.DatabaseVolumeName = databaseVolumeInfo.DatabaseVolumeName.Path;
					copyStatus.DatabasePathIsOnMountedFolder = databaseVolumeInfo.IsDatabasePathOnMountedFolder;
					copyStatus.LogVolumeMountPoint = databaseVolumeInfo.LogVolumeMountPoint.Path;
					copyStatus.LogVolumeName = databaseVolumeInfo.LogVolumeName.Path;
					copyStatus.LogPathIsOnMountedFolder = databaseVolumeInfo.IsLogPathOnMountedFolder;
					copyStatus.VolumeInfoLastError = string.Empty;
				}
				else
				{
					copyStatus.VolumeInfoLastError = databaseVolumeInfo.LastException.Message;
					if (replayState.VolumeInfoIsValid)
					{
						copyStatus.ExchangeVolumeMountPoint = replayState.ExchangeVolumeMountPoint;
						copyStatus.DatabaseVolumeMountPoint = replayState.DatabaseVolumeMountPoint;
						copyStatus.DatabaseVolumeName = replayState.DatabaseVolumeName;
						copyStatus.DatabasePathIsOnMountedFolder = replayState.IsDatabasePathOnMountedFolder;
						copyStatus.LogVolumeMountPoint = replayState.LogVolumeMountPoint;
						copyStatus.LogVolumeName = replayState.LogVolumeName;
						copyStatus.LogPathIsOnMountedFolder = replayState.IsLogPathOnMountedFolder;
					}
				}
				copyStatus.LastDatabaseVolumeName = replayState.LastDatabaseVolumeName;
				copyStatus.LastDatabaseVolumeNameTransitionTime = this.ToNonNullableDateTime(new DateTime?(replayState.LastDatabaseVolumeNameTransitionTime));
			}
		}

		private void UpdateDiskSpaceStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			ulong num;
			ulong num2;
			if (DiskHelper.GetFreeSpace(this.Configuration.DestinationLogPath, out num, out num2) == null)
			{
				int freeSpacePercentage = DiskHelper.GetFreeSpacePercentage(num2, num);
				copyStatus.DiskTotalSpaceBytes = num;
				copyStatus.DiskFreeSpaceBytes = num2;
				copyStatus.DiskFreeSpacePercent = freeSpacePercentage;
			}
		}

		private void UpdateReplayLagStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			ReplayState replayState = this.Configuration.ReplayState;
			if (this.Configuration.Type != ReplayConfigType.SingleCopySource)
			{
				if (this.Configuration.ReplayLagTime == EnhancedTimeSpan.Zero)
				{
					copyStatus.ReplayLagEnabled = ReplayLagEnabledEnum.Disabled;
					return;
				}
				copyStatus.ConfiguredReplayLagTime = this.Configuration.ReplayLagTime;
				copyStatus.ReplayLagEnabled = (replayState.ReplayLagDisabled ? ReplayLagEnabledEnum.Disabled : ReplayLagEnabledEnum.Enabled);
				if (copyStatus.ReplayLagEnabled == ReplayLagEnabledEnum.Disabled)
				{
					copyStatus.ReplayLagDisabledReason = replayState.ReplayLagDisabledReason;
					if (replayState.ReplayLagActionInitiator == ActionInitiatorType.Administrator)
					{
						copyStatus.ReplayLagEnabled = ReplayLagEnabledEnum.CmdletDisabled;
					}
				}
			}
		}

		private void UpdateReplayActualLagStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan actualReplayLagTime = TimeSpan.Zero;
			ReplayState replayState = this.Configuration.ReplayState;
			if (copyStatus.LastReplayedLogTime > this.DefaultTime && utcNow > copyStatus.LastReplayedLogTime)
			{
				actualReplayLagTime = utcNow.Subtract(copyStatus.LastReplayedLogTime);
			}
			copyStatus.ActualReplayLagTime = actualReplayLagTime;
			copyStatus.ReplayLagPercentage = LogReplayer.GetActualReplayLagPercentage(this.Configuration);
			copyStatus.ReplayLagPlayDownReason = this.ConvertReplayLagPlayDownReason(this.Configuration.ReplayState.ReplayLagPlayDownReason);
			if (copyStatus.ReplayLagPlayDownReason != ReplayLagPlayDownReasonEnum.None && copyStatus.ReplayLagEnabled == ReplayLagEnabledEnum.Enabled)
			{
				copyStatus.ReplayLagEnabled = ReplayLagEnabledEnum.Disabled;
			}
			if (replayState.ReplayLagDisabled && copyStatus.ReplayLagPlayDownReason == ReplayLagPlayDownReasonEnum.None)
			{
				copyStatus.ReplayLagPlayDownReason = ReplayLagPlayDownReasonEnum.LagDisabled;
			}
		}

		private ReplayLagPlayDownReasonEnum ConvertReplayLagPlayDownReason(LogReplayPlayDownReason reason)
		{
			switch (reason)
			{
			case LogReplayPlayDownReason.None:
			case LogReplayPlayDownReason.NormalLogReplay:
				return ReplayLagPlayDownReasonEnum.None;
			case LogReplayPlayDownReason.LagDisabled:
				return ReplayLagPlayDownReasonEnum.LagDisabled;
			case LogReplayPlayDownReason.NotEnoughFreeSpace:
				return ReplayLagPlayDownReasonEnum.NotEnoughFreeSpace;
			case LogReplayPlayDownReason.InRequiredRange:
				return ReplayLagPlayDownReasonEnum.InRequiredRange;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for enum {0}", new object[]
				{
					reason.ToString()
				});
				return ReplayLagPlayDownReasonEnum.None;
			}
		}

		private void UpdateLastLogGenerated(RpcDatabaseCopyStatus2 copyStatus, ReplayState replayState)
		{
			LastLogInfo lastLogInfo = replayState.GetLastLogInfo();
			copyStatus.LastLogGenerated = lastLogInfo.LastLogGenToReport;
			copyStatus.LastLogGeneratedTime = lastLogInfo.CollectionTime;
			copyStatus.LastLogInfoIsStale = lastLogInfo.IsStale;
			copyStatus.LastLogInfoFromClusterTime = lastLogInfo.ClusterLastLogTime;
			copyStatus.LastLogInfoFromClusterGen = lastLogInfo.ClusterLastLogGen;
			copyStatus.LastLogInfoFromCopierTime = lastLogInfo.ReplLastLogTime;
			if (lastLogInfo.IsStale && string.IsNullOrEmpty(copyStatus.ErrorMessage))
			{
				copyStatus.ErrorMessage = ReplayStrings.LastLogGenerationTimeStampStale(lastLogInfo.StaleCheckTime.ToString());
			}
		}

		internal static string GetActiveServerForDatabase(Guid mdbGuid, string mbdName, string activeServerFallback, out MountStatus? mountStatus)
		{
			string text = null;
			mountStatus = null;
			try
			{
				mountStatus = new MountStatus?(ActiveManagerCore.GetDatabaseMountStatus(mdbGuid, out text));
			}
			catch (ClusterException arg)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string, ClusterException>(0L, "TargetReplicaInstance {0}\\{1} : UpdateActiveDatabaseCopy() caught exception: {2}", activeServerFallback, mbdName, arg);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = MachineName.GetNodeNameFromFqdn(activeServerFallback);
			}
			return text;
		}

		internal static string GetActiveServerForDatabase(ReplayConfiguration config)
		{
			MountStatus? mountStatus;
			return ReplicaInstance.GetActiveServerForDatabase(config, out mountStatus);
		}

		internal static string GetActiveServerForDatabase(ReplayConfiguration config, out MountStatus? mountStatus)
		{
			return ReplicaInstance.GetActiveServerForDatabase(config.IdentityGuid, config.DisplayName, config.SourceMachine, out mountStatus);
		}

		private void UpdateContentIndexStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			if (RegistryParameters.IgnoreCatalogHealthSetByCI)
			{
				copyStatus.ContentIndexStatus = ContentIndexStatusType.Healthy;
				copyStatus.ContentIndexErrorMessage = LocalizedString.Empty;
				copyStatus.ContentIndexErrorCode = null;
				copyStatus.CICurrentness = ContentIndexCurrentness.Current;
				copyStatus.ContentIndexVersion = new int?(0);
				copyStatus.ContentIndexBacklog = new int?(0);
				copyStatus.ContentIndexRetryQueueSize = new int?(0);
				copyStatus.ContentIndexMailboxesToCrawl = null;
				copyStatus.ContentIndexSeedingSource = string.Empty;
				copyStatus.ContentIndexSeedingPercent = null;
				return;
			}
			LocalizedString value;
			int? contentIndexErrorCode;
			ContentIndexCurrentness cicurrentness;
			int? contentIndexVersion;
			int? contentIndexBacklog;
			int? contentIndexRetryQueueSize;
			int? contentIndexMailboxesToCrawl;
			string contentIndexSeedingSource;
			int? contentIndexSeedingPercent;
			ContentIndexStatusType contentIndexStatus = this.GetContentIndexStatus(out value, out contentIndexErrorCode, out cicurrentness, out contentIndexVersion, out contentIndexBacklog, out contentIndexRetryQueueSize, out contentIndexMailboxesToCrawl, out contentIndexSeedingSource, out contentIndexSeedingPercent);
			copyStatus.ContentIndexStatus = contentIndexStatus;
			copyStatus.ContentIndexErrorMessage = value;
			copyStatus.ContentIndexErrorCode = contentIndexErrorCode;
			copyStatus.CICurrentness = cicurrentness;
			copyStatus.ContentIndexVersion = contentIndexVersion;
			copyStatus.ContentIndexBacklog = contentIndexBacklog;
			copyStatus.ContentIndexRetryQueueSize = contentIndexRetryQueueSize;
			copyStatus.ContentIndexMailboxesToCrawl = contentIndexMailboxesToCrawl;
			copyStatus.ContentIndexSeedingSource = contentIndexSeedingSource;
			copyStatus.ContentIndexSeedingPercent = contentIndexSeedingPercent;
		}

		private ContentIndexStatusType GetContentIndexStatus(out LocalizedString errorMessage, out int? errorCode, out ContentIndexCurrentness currentness, out int? version, out int? backlog, out int? retryQueueSize, out int? mailboxToCrawl, out string seedingSource, out int? seedingPercent)
		{
			errorMessage = LocalizedString.Empty;
			errorCode = null;
			currentness = ContentIndexCurrentness.Unknown;
			version = null;
			backlog = null;
			retryQueueSize = null;
			mailboxToCrawl = null;
			seedingSource = string.Empty;
			seedingPercent = null;
			if (this.Configuration.Database.IsPublicFolderDatabase)
			{
				return ContentIndexStatusType.Healthy;
			}
			Guid identityGuid = this.Configuration.IdentityGuid;
			AmSearchServiceMonitor searchServiceMonitor = Dependencies.ReplayCoreManager.SearchServiceMonitor;
			if (!searchServiceMonitor.IsServiceRunning)
			{
				errorMessage = IndexStatus.GetExcludeReasonFromErrorCode(IndexStatusErrorCode.ServiceNotRunning);
				return ContentIndexStatusType.Failed;
			}
			IndexStatus indexStatus = null;
			try
			{
				indexStatus = IndexStatusStore.Instance.GetIndexStatus(identityGuid);
			}
			catch (Exception ex)
			{
				if (ex is SecurityException || ex is UnauthorizedAccessException || ex is IOException || ex is IndexStatusException)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<Exception>((long)this.GetHashCode(), "GetIndexStatus occurred an error. Exception = {0}.", ex);
					errorMessage = new LocalizedString(ex.Message);
					return ContentIndexStatusType.Unknown;
				}
				throw;
			}
			errorMessage = IndexStatus.GetExcludeReasonFromErrorCode(indexStatus.ErrorCode);
			errorCode = new int?((int)indexStatus.ErrorCode);
			ContentIndexStatusType indexingState = indexStatus.IndexingState;
			switch (indexingState)
			{
			case ContentIndexStatusType.Healthy:
				break;
			case ContentIndexStatusType.Crawling:
				version = new int?(indexStatus.Version.QueryVersion);
				mailboxToCrawl = new int?(indexStatus.MailboxesToCrawl);
				goto IL_222;
			case ContentIndexStatusType.Failed:
				goto IL_222;
			case ContentIndexStatusType.Seeding:
			{
				ReplicaSeederPerfmonInstance instance = ReplicaSeederPerfmon.GetInstance(this.Configuration.Name);
				seedingPercent = new int?((int)instance.CiSeedingPercent.RawValue);
				seedingSource = indexStatus.SeedingSource;
				goto IL_222;
			}
			default:
				if (indexingState != ContentIndexStatusType.HealthyAndUpgrading)
				{
					goto IL_222;
				}
				mailboxToCrawl = new int?(indexStatus.MailboxesToCrawl);
				break;
			}
			version = new int?(indexStatus.Version.QueryVersion);
			backlog = new int?((int)indexStatus.AgeOfLastNotificationProcessed);
			retryQueueSize = new int?((int)indexStatus.RetriableItemsCount);
			currentness = ((backlog < RegistryParameters.CICurrentnessThresholdInSeconds) ? ContentIndexCurrentness.Current : ContentIndexCurrentness.NotCurrent);
			IL_222:
			return indexStatus.IndexingState;
		}

		internal ReplayQueuedItemBase AmPreMountCallback(Guid mdbGuid, ref int storeMountFlags, AmMountFlags amMountFlags, MountDirectPerformanceTracker mountPerf, out LogStreamResetOnMount logReset)
		{
			ReplayQueuedItemBase result = null;
			bool flag = false;
			ReplayConfiguration config = this.Configuration;
			logReset = new LogStreamResetOnMount(config);
			try
			{
				long lastAcllLossAmount = config.ReplayState.LastAcllLossAmount;
				if (lastAcllLossAmount > 0L)
				{
					storeMountFlags |= 2;
				}
				mountPerf.LastAcllLossAmount = lastAcllLossAmount;
				bool lastAcllRunWithSkipHealthChecks = config.ReplayState.LastAcllRunWithSkipHealthChecks;
				if (lastAcllRunWithSkipHealthChecks)
				{
					amMountFlags |= AmMountFlags.MoveWithSkipHealth;
				}
				mountPerf.LastAcllRunWithSkipHealthChecks = lastAcllRunWithSkipHealthChecks;
				if (config.ReplayState.Suspended)
				{
					if (BitMasker.IsOn((int)amMountFlags, 1))
					{
						ReplayCrimsonEvents.ForcedMountAttempted.Log<string, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName);
					}
					else
					{
						if (!BitMasker.IsOn((int)amMountFlags, 2))
						{
							string text = config.ReplayState.ConfigBrokenMessage;
							if (string.IsNullOrEmpty(text))
							{
								text = config.ReplayState.SuspendMessage;
							}
							if (string.IsNullOrEmpty(text))
							{
								text = ReplayStrings.UnknownError;
							}
							throw new AmPreMountCallbackFailedMountInhibitException(config.Name, config.ServerName, text);
						}
						ReplayCrimsonEvents.ForcedMoveAttempted.Log<string, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName);
					}
				}
				int tempFlags = storeMountFlags;
				mountPerf.RunTimedOperation(MountDatabaseDirectOperation.PreventMountIfNecessary, delegate
				{
					this.PreventMountIfNecessary(tempFlags);
				});
				if (config.Type == ReplayConfigType.RemoteCopyTarget)
				{
					flag = true;
				}
				else if (config.Type == ReplayConfigType.RemoteCopySource && !config.IsSourceMachineEqual(AmServerName.LocalComputerName))
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Source RI found but SourceMachine [{1}] is different from local. Setting fTriggerConfigUpdater to 'true'.", mdbGuid, config.SourceMachine);
					flag = true;
				}
				if (config.Type != ReplayConfigType.SingleCopySource)
				{
					mountPerf.RunTimedOperation(MountDatabaseDirectOperation.ResumeActiveCopy, delegate
					{
						this.ResumeActiveCopy(mdbGuid, config);
					});
				}
				tempFlags = storeMountFlags;
				LogStreamResetOnMount tempLogRest = logReset;
				mountPerf.RunTimedOperation(MountDatabaseDirectOperation.UpdateLastLogGenOnMount, delegate
				{
					ReplayConfiguration.TryUpdateLastLogGenerationNumberOnMount(config, tempLogRest, mountPerf, tempFlags, this.IsThirdPartyReplicationEnabled ? 0L : this.FileChecker.FileState.HighestGenerationPresent);
				});
				IncrementalReseeder.CleanupFiles(this.Configuration, false, true);
			}
			catch (ReplayServiceShuttingDownException ex)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, ReplayServiceShuttingDownException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): ReplicaInstance could not be restarted due to ReplayServiceShuttingDownException: {1}", mdbGuid, ex);
				throw new AmPreMountCallbackFailedException(config.Name, ex.Message, ex);
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, IOException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Exception occurred: {1}", mdbGuid, ex2);
				throw new AmPreMountCallbackFailedException(config.Name, ex2.Message, ex2);
			}
			catch (SecurityException ex3)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, SecurityException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Exception occurred: {1}", mdbGuid, ex3);
				throw new AmPreMountCallbackFailedException(config.Name, ex3.Message, ex3);
			}
			catch (IncrementalReseedRetryableException ex4)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, IncrementalReseedRetryableException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Exception occurred: {1}", mdbGuid, ex4);
				throw new AmPreMountCallbackFailedException(config.Name, ex4.Message, ex4);
			}
			catch (IncrementalReseedFailedException ex5)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid, IncrementalReseedFailedException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Exception occurred: {1}", mdbGuid, ex5);
				throw new AmPreMountCallbackFailedException(config.Name, ex5.Message, ex5);
			}
			finally
			{
				lock (this)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "AmPreMountCallback ({0}): Leaving ACLL lock if held.", mdbGuid);
					config.ReplayState.SuspendLock.LeaveAttemptCopyLastLogs();
					this.CurrentContext.ClearAttemptCopyLastLogsEndTime();
				}
				if (flag)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "AmPreMountCallback ({0}): fTriggerConfigUpdater='true', so calling RI.RestartInstanceSoon().", mdbGuid);
					this.CurrentContext.RestartInstanceSoon(true);
					result = Dependencies.ConfigurationUpdater.NotifyChangedReplayConfiguration(mdbGuid, false, true, true, ReplayConfigChangeHints.AmPreMountCallbackRI, -1);
				}
			}
			return result;
		}

		private void ResumeActiveCopy(Guid mdbGuid, ReplayConfiguration config)
		{
			try
			{
				this.RequestResume(DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation | DatabaseCopyActionFlags.ActiveCopy);
			}
			catch (TaskServerException ex)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid, TaskServerException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): RequestResume failed with exception: {1}", mdbGuid, ex);
				ReplayCrimsonEvents.RpcResumeFailed.Log<Guid, string>(mdbGuid, ex.Message);
			}
			catch (TaskServerTransientException ex2)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<Guid, TaskServerTransientException>((long)this.GetHashCode(), "AmPreMountCallback ({0}): RequestResume failed with exception: {1}", mdbGuid, ex2);
				ReplayCrimsonEvents.RpcResumeFailed.Log<Guid, string>(config.IdentityGuid, ex2.Message);
			}
		}

		private void PreventMountIfNecessary(int mapiMountFlags)
		{
			ReplayConfiguration configuration = this.Configuration;
			string text = configuration.DestinationEdbPath;
			string destinationLogPath = configuration.DestinationLogPath;
			bool flag = (mapiMountFlags & 1) != 0;
			Exception ex = null;
			if (RegistryParameters.DatabaseType == 1 && text.TrimEnd(new char[0]).EndsWith(".edb", StringComparison.OrdinalIgnoreCase))
			{
				text = Path.ChangeExtension(text, ".mdf");
			}
			if (!flag)
			{
				bool flag2 = configuration.DatabaseCreated;
				if (!flag2)
				{
					AmDbStateInfo databaseStateInfo = ActiveManagerCore.GetDatabaseStateInfo(configuration.IdentityGuid);
					flag2 = (databaseStateInfo.IsEntryExist && databaseStateInfo.IsMountSucceededAtleastOnce);
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): DB '{1}' skipping cluster DB state check since DatabaseCreated AD attribute is 'true'", configuration.Identity, configuration.DatabaseName);
				}
				if (flag2 && !File.Exists(text))
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): DB '{1}' has been mounted before but no longer has an EDB file. Blocking mount! EdbFilePath={2}", configuration.Identity, configuration.DatabaseName, text);
					throw new AmMountBlockedDbMountedBeforeWithMissingEdbException(configuration.Name, text);
				}
			}
			else
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): DB '{1}' skipping EDB file existence check because ForceDatabaseCreation flag supplied! MountFlags={2}; EdbFilePath={3}", new object[]
				{
					configuration.Identity,
					configuration.DatabaseName,
					(MountFlags)mapiMountFlags,
					text
				});
			}
			if (configuration.Type == ReplayConfigType.SingleCopySource && !File.Exists(text))
			{
				if (Directory.Exists(destinationLogPath))
				{
					long num = ShipControl.HighestGenerationInDirectory(new DirectoryInfo(destinationLogPath), configuration.LogFilePrefix, "." + configuration.LogExtension);
					if (num >= 1L)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, long, string>((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): Non-replicated DB has no EDB file but has some logfiles. Blocking mount! HighestLog={1}, EdbFilePath={2}", configuration.Identity, num, text);
						throw new AmMountBlockedOnStandaloneDbWithMissingEdbException(configuration.Name, num, text);
					}
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): Non-replicated DB has no EDB file and no logfiles. Allowing mount to continue. EdbFilePath={1}, LogFilePath={2}", configuration.Identity, text, destinationLogPath);
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "PreventMountIfNecessary ({0}): Non-replicated DB has no EDB file and no log directory. Allowing mount to continue. EdbFilePath={1}, LogFilePath={2}", configuration.Identity, text, destinationLogPath);
				}
			}
			if (!this.IsEdbLogDirectoryUnderMountPoint(out ex))
			{
				throw new AmMountCallbackFailedWithDBFolderNotUnderMountPointException(configuration.Name, AmExceptionHelper.GetExceptionMessageOrNoneString(ex));
			}
		}

		protected void CleanUpTempIncReseedFiles()
		{
			if (!IncrementalReseeder.CheckForInterruptedPatch(this.Configuration, null))
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: CleanUpTempIncReseedFiles(): Proceeding with cleanup of IncReseed files as necessary.", this.Configuration.DisplayName);
				IncrementalReseeder.CleanupFiles(this.Configuration, false, true);
			}
		}

		internal abstract AmAcllReturnStatus AttemptCopyLastLogsRcr(AmAcllArgs acllArgs, AcllPerformanceTracker acllPerf);

		public bool ShouldBeRestarted(ReplayConfiguration newConfig, bool forceRestart)
		{
			bool result = false;
			if (!forceRestart && this.CurrentContext.DoNotRestart)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstance {0} ({1}) will not be restarting because DoNotRestart has been set.", this.Configuration.Identity, this.Configuration.Name);
				return false;
			}
			if (this.CurrentContext.ShouldNotRestartInstanceDueToAcll())
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstance {0} ({1}) will not be restarting because of ACLL (pending failover).", this.Configuration.Identity, this.Configuration.Name);
				return false;
			}
			ReplayConfigChangedFlags replayConfigChangedFlags;
			bool flag = this.Configuration.ConfigEquals(newConfig, out replayConfigChangedFlags);
			if (this.CurrentContext.PassiveSeedingSourceContext != PassiveSeedingSourceContextEnum.None || this.CurrentContext.ActiveSeedingSource)
			{
				if (this.IsBroken)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because it is broken while being a seeding source.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
					return true;
				}
				if ((replayConfigChangedFlags & ReplayConfigChangedFlags.ActiveServer) != ReplayConfigChangedFlags.None)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because the active server has changed, and it is a seeding source.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
					return true;
				}
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstance {0} ({1}) will *NOT* be restarting because it is a seeding source.", this.Configuration.Identity, this.Configuration.Name);
				return false;
			}
			else
			{
				if (!flag)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Found changed config: {0}({1})", this.Configuration.Identity, this.Configuration.Name);
					ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Found changed config: {1}({2})", 27933, this.Configuration.Identity, this.Configuration.Name);
					result = true;
				}
				if (this.CurrentContext.RestartSoon)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because restart soon was requested.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
					return true;
				}
				if (forceRestart)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because 'forceRestart' was specified.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
					return true;
				}
				if (this.IsTarget)
				{
					if (this.IsBroken && !this.CurrentContext.Suspended)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because it is broken.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
						result = true;
					}
					else if (this.PreviousContext != null && this.PreviousContext.Suspended && !this.Configuration.ReplayState.SuspendLockRemote.SuspendWanted && this.Configuration.ReplayState.Suspended)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because it is no longer admin suspended.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
						result = true;
					}
					else if (this.Configuration.ReplayState.SuspendLockRemote.SuspendWanted && !this.Configuration.ReplayState.Suspended)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0} ({1}): ReplicaInstance should be restarted because admin suspend has been requested.", this.Configuration.DisplayName, this.Configuration.IdentityGuid);
						result = true;
					}
				}
				return result;
			}
		}

		protected void CreateDirectories(string[] directoriesToCreate)
		{
			foreach (string text in directoriesToCreate)
			{
				Exception ex = DirectoryOperations.TryCreateDirectory(text);
				if (ex != null)
				{
					FailureTag failureTag = FailureTag.Configuration;
					IOException ex2 = ex as IOException;
					if (ex2 != null && DirectoryOperations.IsPathOnLockedVolume(text))
					{
						failureTag = FailureTag.LockedVolume;
					}
					this.CurrentContext.SetBrokenAndThrow(failureTag, ReplayEventLogConstants.Tuple_FailedToCreateDirectory, ex, new string[]
					{
						text,
						ex.ToString()
					});
				}
			}
		}

		protected void CheckDirectories(IEnumerable<string> directoriesToCheck)
		{
			foreach (string text in directoriesToCheck)
			{
				if (text != null && !Directory.Exists(text))
				{
					try
					{
						DirectoryOperations.ProbeDirectory(text);
					}
					catch (IOException ex)
					{
						if (DirectoryOperations.IsPathOnLockedVolume(text))
						{
							this.CurrentContext.SetBrokenAndThrow(FailureTag.LockedVolume, ReplayEventLogConstants.Tuple_FailedToCreateDirectory, ex, new string[]
							{
								text,
								ex.ToString()
							});
						}
						throw;
					}
					this.CurrentContext.SetBroken(FailureTag.IoHard, ReplayEventLogConstants.Tuple_NoDirectory, new string[]
					{
						text
					});
				}
			}
		}

		protected void CheckEdbLogDirectoriesIfNeeded()
		{
			if (RegistryParameters.DisableEdbLogDirectoryCreation)
			{
				string[] array = new string[]
				{
					Path.GetDirectoryName(this.Configuration.DestinationEdbPath),
					this.Configuration.DestinationLogPath
				};
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string[]>((long)this.GetHashCode(), "ReplicaInstance.CheckEdbLogSystemDirectoriesIfNeeded() for database {0} checking directories {1}", this.Configuration.Name, array);
				this.CheckDirectories(array);
			}
		}

		protected bool IsEdbLogDirectoryUnderMountPoint(out Exception exception)
		{
			exception = null;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstance.IsEdbLogDirectoryUnderMountPoint() for database {0}.", this.Configuration.Name);
			if (!RegistryParameters.EnforceDbFolderUnderMountPoint)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstance.IsEdbLogDirectoryUnderMountPoint() for database {0}. Regkey EnforceDbFolderUnderMountPoint is not set, skipping checks.", this.Configuration.Name);
				return true;
			}
			DatabaseVolumeInfo instance = DatabaseVolumeInfo.GetInstance(this.Configuration);
			if (!instance.IsValid || instance.LastException != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "ReplicaInstance.IsEdbLogDirectoryUnderMountPoint(): Couldn't get valid volume info for DB '{0}'. Error: '{1}'.", this.Configuration.Name, AmExceptionHelper.GetExceptionMessageOrNoneString(instance.LastException));
				exception = instance.LastException;
				return false;
			}
			bool result;
			if (instance.IsDatabasePathOnMountedFolder && instance.IsLogPathOnMountedFolder)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstance.IsEdbLogDirectoryUnderMountPoint(): Path '{0}' is under a mountpoint.", this.Configuration.Name);
				result = true;
			}
			else
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "ReplicaInstance.CheckEdbLogDirectoryUnderMountPoint(): Database folders for DB '{0}' is not under a mountpoint and regkey EnforceDbFolderUnderMountPoint is set to True. Either disable the regkey or move the database folder under a mountpoint.", this.Configuration.Name);
				result = false;
			}
			return result;
		}

		protected void CheckEdbLogDirectoryUnderMountPoint()
		{
			Exception ex = null;
			if (!this.IsEdbLogDirectoryUnderMountPoint(out ex))
			{
				this.CurrentContext.SetBroken(FailureTag.IoHard, ReplayEventLogConstants.Tuple_DatabaseDirectoryNotUnderMountPoint, ex, new string[]
				{
					this.Configuration.DestinationEdbPath,
					AmExceptionHelper.GetExceptionMessageOrNoneString(ex)
				});
			}
		}

		protected bool HasDestinationEdbPaths()
		{
			return this.Configuration.DestinationEdbPath != null;
		}

		protected bool HasSourceEdbPaths()
		{
			return this.Configuration.SourceEdbPath != null;
		}

		protected void DeleteAllLogFiles(string directory, ISetBroken setBroken)
		{
			if (!Directory.Exists(directory))
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ReplicaInstance {0} ({1}): Directory '{2}' not found, so skipping deletion in DeleteAllLogFiles().", this.Configuration.Identity, this.Configuration.Name, directory);
				setBroken.SetBroken(FailureTag.IoHard, ReplayEventLogConstants.Tuple_NoDirectory, new string[]
				{
					directory
				});
				return;
			}
			string text = string.Empty;
			try
			{
				foreach (string text2 in Directory.GetFiles(directory, "*." + this.Configuration.LogExtension))
				{
					text = text2;
					File.Delete(text2);
				}
				foreach (string text3 in Directory.GetFiles(directory, "*.jsl"))
				{
					text = text3;
					File.Delete(text3);
				}
				Directory.GetFiles(directory, this.Configuration.LogFilePrefix + "*." + this.Configuration.LogExtension);
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)this.GetHashCode(), "Delete files successfully from '{0}'", directory);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, IOException>((long)this.GetHashCode(), "Deleting file {0} failed with exception {1}", text, ex);
				setBroken.SetBroken(ReplicaInstance.IOExceptionToFailureTag(ex), ReplayEventLogConstants.Tuple_CouldNotDeleteLogFile, ex, new string[]
				{
					text,
					ex.ToString()
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, UnauthorizedAccessException>((long)this.GetHashCode(), "Deleting file {0} failed with exception {1}", text, ex2);
				setBroken.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_CouldNotDeleteLogFile, ex2, new string[]
				{
					text,
					ex2.ToString()
				});
			}
		}

		public static long CopyGeneration(ReplayConfiguration config, FileState fileState, ISetGeneration setGeneration, ManualOneShotEvent shuttingDownEvent)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<FileState>((long)fileState.GetHashCode(), "Calculating copy generation from FileState: {0}", fileState);
			long num = 0L;
			if (0L != fileState.HighestGenerationPresent)
			{
				num = fileState.HighestGenerationPresent + 1L;
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<long>((long)fileState.GetHashCode(), "Log generation {0} is present", fileState.HighestGenerationPresent);
			}
			else
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)fileState.GetHashCode(), "No logfiles present");
				ReplayCrimsonEvents.CopyStartingWithNoLogs.Log<string, string, Guid>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, config.DatabaseGuid);
				Exception ex = null;
				try
				{
					LogTruncater.RequestGlobalTruncationCoordination(1L, config.SourceMachine, config.TargetMachine, config.IdentityGuid, config.LogFilePrefix, config.DestinationLogPath, config.CircularLoggingEnabled, shuttingDownEvent);
					long num2 = LogTruncater.CalculateLowestGenerationRequired(config, fileState);
					num = LogTruncater.RequestGlobalTruncationCoordination(num2, config.SourceMachine, config.TargetMachine, config.IdentityGuid, config.LogFilePrefix, config.DestinationLogPath, config.CircularLoggingEnabled, shuttingDownEvent);
					num = Math.Max(num, num2 - (long)RegistryParameters.MaxLogFilesToSeed);
					num = Math.Min(num, num2);
					ReplayCrimsonEvents.CopyStartingReceivedGlobalStartGen.Log<string, string, Guid, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, config.DatabaseGuid, LogCopier.FormatLogGeneration(num));
					if (num == 1L)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>((long)fileState.GetHashCode(), "TargetReplicaInstance {0} : CopyGeneration 1 may indicate truncation is disabled. LogCopier will copy all logs.", config.Name);
						num = 0L;
					}
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)fileState.GetHashCode(), "TargetReplicaInstance {0} : CopyGeneration determined that global truncation generation is {1}", config.Name, num);
				}
				catch (EsentErrorException ex2)
				{
					ex = ex2;
				}
				catch (LogTruncationException ex3)
				{
					ex = ex3;
				}
				finally
				{
					if (ex != null)
					{
						num = 0L;
					}
				}
				if (setGeneration != null && setGeneration.IsLogStreamStartGenerationResetPending && fileState.HighestGenerationPresent == 0L && config.ReplayState.LogStreamStartGeneration <= fileState.LowestGenerationRequired)
				{
					long logStreamStartGeneration = config.ReplayState.LogStreamStartGeneration;
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)fileState.GetHashCode(), "TargetReplicaInstance {0} : Log copying has been asked to reset the log stream start generation to {1}", config.Name, logStreamStartGeneration);
					ReplayCrimsonEvents.CopyStartingFoundLogResetPending.Log<string, string, Guid, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, config.DatabaseGuid, LogCopier.FormatLogGeneration(logStreamStartGeneration));
					num = Math.Max(logStreamStartGeneration, num);
				}
				if (ex != null)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception, long>((long)fileState.GetHashCode(), "TargetReplicaInstance {0} : CopyGeneration calculation failed. LogCopier will copy either all logs on active, or logs since previous LogStreamStartGeneration. CopyGenerationNumber = {2}. Error: {1}", config.Name, ex, num);
					ReplayCrimsonEvents.CopyStartingButFailedToGetGlobalStartGen.Log<string, string, Guid, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, config.DatabaseGuid, ex.ToString());
				}
			}
			ReplayCrimsonEvents.CopyStartingWithLogGen.Log<string, string, Guid, string>(config.DatabaseName, Dependencies.ManagementClassHelper.LocalMachineName, config.DatabaseGuid, LogCopier.FormatLogGeneration(num));
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)fileState.GetHashCode(), "TargetReplicaInstance {0} : Log copying will start from generation {1}", config.Name, num);
			return num;
		}

		public static void MoveE00Log(string sourcePath, LocalizedString reason, string destinationDirectory, ReplayConfiguration replayConfiguration)
		{
			string currentDateString = FileOperations.GetCurrentDateString();
			string fileName = Path.GetFileName(sourcePath);
			string path = currentDateString + fileName;
			string text = Path.Combine(destinationDirectory, path);
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)replayConfiguration.GetHashCode(), "ReplicaInstance {0}: Moving current log from '{1}' to '{2}'. Reason: {3}", new object[]
			{
				replayConfiguration.DisplayName,
				sourcePath,
				text,
				reason
			});
			try
			{
				Directory.CreateDirectory(destinationDirectory, ObjectSecurity.ExchangeFolderSecurity);
				File.Move(sourcePath, text);
				ReplayEventLogConstants.Tuple_E00LogMoved.LogEvent(null, new object[]
				{
					replayConfiguration.DatabaseName,
					sourcePath,
					text,
					reason
				});
			}
			catch (IOException innerException)
			{
				throw new CouldNotMoveLogFileException(sourcePath, text, innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new CouldNotMoveLogFileException(sourcePath, text, innerException2);
			}
		}

		internal static string[] GetArgumentsWithDb(string[] argumentsWithoutDb, string database)
		{
			string[] array = new string[argumentsWithoutDb.Length + 1];
			array[0] = database;
			if (argumentsWithoutDb.Length > 0)
			{
				argumentsWithoutDb.CopyTo(array, 1);
			}
			return array;
		}

		protected void StartComponent(IStartStop component)
		{
			if (!this.PrepareToStopCalled)
			{
				component.Start();
				this.m_components.Add(component);
			}
		}

		protected DateTime WriteTimeFromGeneration(long generation)
		{
			if (0L == generation)
			{
				return this.ZeroFileTime;
			}
			string logfileSuffix = "." + this.Configuration.LogExtension;
			string fileName = Path.Combine(this.Configuration.DestinationLogPath, EseHelper.MakeLogfileName(this.Configuration.LogFilePrefix, logfileSuffix, generation));
			DateTime result = this.ZeroFileTime;
			Exception ex = null;
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				result = fileInfo.LastWriteTimeUtc;
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError((long)this.GetHashCode(), "{0}({1}): WriteTimeFromGeneration() for gen {2} threw exception. Returning ZeroFileTime instead: {3}", new object[]
				{
					this.Configuration.DisplayName,
					this.Configuration.Identity,
					generation,
					ex
				});
			}
			return result;
		}

		protected void SetWriteTimesFromFiles()
		{
			ReplayState replayState = this.Configuration.ReplayState;
			replayState.LatestCopyTime = this.WriteTimeFromGeneration(replayState.CopyGenerationNumber);
			replayState.LatestInspectorTime = this.WriteTimeFromGeneration(replayState.InspectorGenerationNumber);
			replayState.LatestReplayTime = this.WriteTimeFromGeneration(replayState.ReplayGenerationNumber);
		}

		protected void SetReplayState(long copyGeneration, long inspectorGeneration, long replayGeneration)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "{0}({1}): SetReplayState(): CopyGeneration={2}, InspectorGeneration={3}, ReplayGeneration={4}", new object[]
			{
				this.configName,
				this.Configuration.Identity,
				copyGeneration,
				inspectorGeneration,
				replayGeneration
			});
			ReplayState replayState = this.Configuration.ReplayState;
			replayState.CopyGenerationNumber = copyGeneration;
			replayState.InspectorGenerationNumber = inspectorGeneration;
			replayState.ReplayGenerationNumber = replayGeneration;
			this.SetWriteTimesFromFiles();
		}

		protected void SetReplayState(FileState fileState)
		{
			long highestGenerationPresent = fileState.HighestGenerationPresent;
			long highestGenerationPresent2 = fileState.HighestGenerationPresent;
			long lowestGenerationRequired = fileState.LowestGenerationRequired;
			this.SetReplayState(highestGenerationPresent, highestGenerationPresent2, lowestGenerationRequired);
		}

		protected void ClearReplayState()
		{
			ReplayState replayState = this.Configuration.ReplayState;
			replayState.CopyGenerationNumber = replayState.InspectorGenerationNumber;
			replayState.CurrentReplayTime = this.ZeroFileTime;
			this.SetWriteTimesFromFiles();
		}

		protected void AcquireSuspendLockForConfigChecker()
		{
			StateLock stateLock = null;
			if (this.ShouldAcquireSuspendLockInConfigChecker)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplicaInstance.AcquireSuspendLockForConfigChecker() '{0}': Attempting to acquire Component lock.", this.configName);
				stateLock = this.Configuration.ReplayState.SuspendLock;
				LockOwner lockOwner;
				if (!stateLock.TryEnter(LockOwner.Component, false, out lockOwner))
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "ReplicaInstance.AcquireSuspendLockForConfigChecker() '{0}': Component lock could not be acquired. CurrentOwner='{1}'", this.configName, lockOwner);
					if (lockOwner == LockOwner.Suspend)
					{
						throw new ReplicaInstance.OperationAbortedDueToAdminSuspendException();
					}
					throw new OperationAbortedException();
				}
			}
			this.SuspendLockConfigChecker = stateLock;
		}

		protected void ReleaseSuspendLockForConfigChecker()
		{
			if (this.SuspendLockConfigChecker != null)
			{
				this.SuspendLockConfigChecker.Leave(LockOwner.Component);
				this.SuspendLockConfigChecker = null;
			}
		}

		protected void CheckInstanceAbortRequested()
		{
			if (this.PrepareToStopCalled || this.CurrentContext.RestartSoon)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "CheckInstanceAbortRequested() '{0}': ConfigChecker exiting due to PrepareToStop or RestartInstanceSoon.", this.configName);
				throw new OperationAbortedException();
			}
			LockOwner lockOwner;
			if (this.SuspendLockConfigChecker == null || !this.SuspendLockConfigChecker.ShouldGiveUpLock(out lockOwner))
			{
				return;
			}
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "CheckInstanceAbortRequested() '{0}': ConfigChecker should give up the lock. HighestPending = '{1}'", this.configName, lockOwner);
			if (lockOwner == LockOwner.Suspend)
			{
				throw new ReplicaInstance.OperationAbortedDueToAdminSuspendException();
			}
			throw new OperationAbortedException();
		}

		[Conditional("DEBUG")]
		private void AssertClearedReplayState()
		{
			ReplayState replayState = this.Configuration.ReplayState;
		}

		private void CreateIdleEvent()
		{
			lock (this)
			{
				this.m_configurationCheckIdleEvent = new ManualResetEvent(false);
			}
		}

		private void SetAndClearIdleEvent()
		{
			lock (this)
			{
				ManualResetEvent configurationCheckIdleEvent = this.m_configurationCheckIdleEvent;
				this.m_configurationCheckIdleEvent = null;
				configurationCheckIdleEvent.Set();
			}
		}

		internal abstract bool GetSignatureAndCheckpoint(out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp);

		public bool CheckFirstBackupLogfile(string destinationLogPath, string logFilePrefix, string logExtension)
		{
			long generation = this.FileChecker.FileState.LastGenerationBackedUp + 1L;
			string path = EseHelper.MakeLogfileName(logFilePrefix, logExtension, generation);
			string path2 = Path.Combine(destinationLogPath, path);
			return File.Exists(path2);
		}

		public static bool CreateTempLogFile(ReplayConfiguration config, out Exception exception)
		{
			return EseHelper.CreateTempLog(config.LogFilePrefix, config.DestinationLogPath, out exception);
		}

		protected void WaitForConfigCheckerToStopIfNecessary()
		{
			ManualResetEvent manualResetEvent = null;
			lock (this)
			{
				manualResetEvent = this.m_configurationCheckIdleEvent;
			}
			if (manualResetEvent != null)
			{
				manualResetEvent.WaitOne();
				DiagCore.RetailAssert(null == this.m_configurationCheckIdleEvent, "m_configurationCheckIdleEvent wasn't cleared.", new object[0]);
			}
		}

		protected DateTime ToNonNullableDateTime(DateTime? dateTime)
		{
			if (dateTime != null)
			{
				return dateTime.Value;
			}
			return this.DefaultTime;
		}

		protected virtual void TraceDebug(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ReplicaInstance {0}: {1}", this.Configuration.Name, message);
		}

		protected virtual void TraceError(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "ReplicaInstance {0}: {1}", this.Configuration.Name, message);
		}

		public bool IsTarget
		{
			get
			{
				return this.m_fTargetInstance;
			}
		}

		public bool IsThirdPartyReplicationEnabled
		{
			get
			{
				return this.m_tprModeEnabled;
			}
		}

		private const string PrepareToStopCalledEventName = "PrepareToStopCalledEvent";

		public static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.ReplicaInstanceTracer;

		protected readonly ManualOneShotEvent PrepareToStopCalledEvent;

		protected readonly IPerfmonCounters PerformanceCounters;

		private ReplayConfiguration m_configuration;

		private ReplicaInstanceContext m_currentContext;

		private ReplicaInstanceContextMinimal m_previousContext;

		private bool m_fStarted;

		private bool m_fPrepareToStopCalled;

		private bool m_fStopped;

		private bool m_fDisposed;

		private volatile ManualResetEvent m_configurationCheckIdleEvent;

		private readonly bool m_fTargetInstance;

		private bool m_tprModeEnabled;

		private List<IStartStop> m_components;

		private FileChecker m_fileChecker;

		private object m_lastloggeneratedlocker = new object();

		protected string configName;

		protected readonly DateTime m_instanceCreateTime;

		protected readonly DateTime DefaultTime = ReplayState.ZeroFileTime;

		protected delegate LocalizedString DirectoryCreationErrorStringDelegate(string directory, Exception ex);

		protected delegate LocalizedString DirectoryNotFoundErrorStringDelegate(string directory);

		protected delegate LocalizedString RemoveLogFileErrorStringDelegate(string logFile, Exception ex);

		private class OperationAbortedDueToAdminSuspendException : Exception
		{
		}
	}
}
