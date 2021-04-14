using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.ThirdPartyReplication;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbPamAction : AmDbAction
	{
		internal AmDbPamAction(AmConfig cfg, IADDatabase db, AmDbActionCode actionCode, string uniqueOperationId) : base(cfg, db, actionCode, uniqueOperationId)
		{
		}

		public void ChangeActiveServerForThirdParty(string newActiveServerName, TimeSpan lockTimeout)
		{
			using (AmDatabaseOperationLock.Lock(base.DatabaseGuid, AmDbLockReason.Move, new TimeSpan?(lockTimeout)))
			{
				if (!base.State.IsAdminDismounted)
				{
					throw new ChangeActiveServerException(base.DatabaseGuid, newActiveServerName, ReplayStrings.TPRChangeFailedBecauseNotDismounted);
				}
				AmServerName activeServer = base.State.ActiveServer;
				AmServerName amServerName = new AmServerName(newActiveServerName);
				if (activeServer.Equals(amServerName))
				{
					throw new ChangeActiveServerException(base.DatabaseGuid, newActiveServerName, ReplayStrings.TPRChangeFailedBecauseAlreadyActive(activeServer.ToString()));
				}
				IAmBcsErrorLogger errorLogger = new AmBcsSingleCopyFailureLogger();
				AmBcsServerChecks checks = AmBcsServerChecks.DebugOptionDisabled | AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed;
				LocalizedString empty = LocalizedString.Empty;
				AmBcsServerValidation amBcsServerValidation = new AmBcsServerValidation(amServerName, activeServer, base.Database, base.Config, errorLogger, null);
				if (!amBcsServerValidation.RunChecks(checks, ref empty))
				{
					AmTrace.Error("ChangeActiveServerForThirdParty: DB {0}: ValidateServer() returned error: {1}", new object[]
					{
						base.DatabaseName,
						empty
					});
					throw new ChangeActiveServerException(base.DatabaseGuid, newActiveServerName, ReplayStrings.TPRChangeFailedServerValidation(base.DatabaseName, newActiveServerName, empty));
				}
				base.WriteStateMountSkipped(amServerName);
				this.UpdateAdProperties(true, activeServer);
				ReplayCrimsonEvents.TPRChangeActiveServerSucceeded.Log<string, Guid, AmServerName, AmServerName>(base.DatabaseName, base.DatabaseGuid, activeServer, amServerName);
			}
		}

		protected override void DismountInternal(UnmountFlags flags)
		{
			base.DismountCommon(flags);
		}

		protected override void MountInternal(MountFlags storeMountFlags, AmMountFlags amMountFlags, DatabaseMountDialOverride mountDialOverride, ref AmDbOperationDetailedStatus mountStatus)
		{
			Exception ex = null;
			int natSkippedServersCount = 0;
			int num = 0;
			bool flag = true;
			if (ThirdPartyManager.IsThirdPartyReplicationEnabled)
			{
				flag = false;
			}
			if (base.ActionCode.IsAdminOperation)
			{
				if (base.State.IsActiveServerValid)
				{
					flag = false;
				}
				else
				{
					AmTrace.Debug("Mount issued for the first time on this database. We will try all the available servers. (db={0})", new object[]
					{
						base.DatabaseName
					});
				}
			}
			AmServerName amServerName = base.State.ActiveServer;
			if (AmServerName.IsNullOrEmpty(amServerName))
			{
				amServerName = new AmServerName(base.Database.Server.Name);
			}
			AmBcsSkipFlags skipValidationChecks = AmBcsSkipFlags.SkipAll;
			if (flag)
			{
				skipValidationChecks = AmBcsSkipFlags.None;
			}
			IBestCopySelector bestCopySelector = this.ConstructBestCopySelector(flag, skipValidationChecks, null, amServerName, amServerName, mountDialOverride, null);
			AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
			if (base.ActionCode.IsAdminOperation)
			{
				dbNodeAttemptTable.ClearFailedTime(base.DatabaseGuid);
			}
			AmServerName sourceServer = amServerName;
			AmAcllReturnStatus amAcllReturnStatus = null;
			AmServerName amServerName2 = bestCopySelector.FindNextBestCopy();
			while (amServerName2 != null)
			{
				num++;
				base.DbTrace.Debug("Attempting mount on server {0}", new object[]
				{
					amServerName2
				});
				this.AttemptMountOnServer(amServerName2, sourceServer, storeMountFlags, amMountFlags, UnmountFlags.SkipCacheFlush, mountDialOverride, AmBcsSkipFlags.None, flag, ref natSkippedServersCount, ref amAcllReturnStatus, out ex);
				base.DbTrace.Debug("AttemptMountOnServer returned AcllStatus: {0}", new object[]
				{
					amAcllReturnStatus
				});
				sourceServer = base.State.ActiveServer;
				if (ex == null)
				{
					break;
				}
				bestCopySelector.ErrorLogger.ReportServerFailure(amServerName2, "CopyHasBeenTriedCheck", ex.Message);
				if (ex is AmRoleChangedWhileOperationIsInProgressException || ex is AmMountTimeoutException)
				{
					break;
				}
				amServerName2 = bestCopySelector.FindNextBestCopy();
				base.CurrentAttemptNumber++;
			}
			this.CheckActionResultsAndUpdateAdProperties(amServerName, null, bestCopySelector, ex, num, natSkippedServersCount);
		}

		protected override void RemountInternal(MountFlags mountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer)
		{
			Exception exception = null;
			base.AttemptDismount(base.State.ActiveServer, UnmountFlags.SkipCacheFlush, true, out exception);
			int num = 0;
			AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
			AmAcllReturnStatus amAcllReturnStatus = null;
			this.AttemptMountOnServer(base.State.ActiveServer, base.State.ActiveServer, mountFlags, AmMountFlags.None, UnmountFlags.SkipCacheFlush, mountDialoverride, AmBcsSkipFlags.None, false, ref num, ref amAcllReturnStatus, out exception);
			base.DbTrace.Debug("AttemptMountOnServer returned AcllStatus: {0}", new object[]
			{
				amAcllReturnStatus
			});
			AmHelper.ThrowDbActionWrapperExceptionIfNecessary(exception);
		}

		protected override void MoveInternal(MountFlags storeMountFlags, UnmountFlags dismountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer, AmServerName targetServer, bool tryOtherHealthyServers, AmBcsSkipFlags skipValidationChecks, string componentName, ref AmDbOperationDetailedStatus moveStatus)
		{
			AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
			IBestCopySelector bestCopySelector = null;
			Exception ex = null;
			bool flag = false;
			int num = 0;
			int natSkippedServersCount = 0;
			if (!base.State.IsEntryExist)
			{
				base.DbTrace.Error("Database was never mounted. Move is applicable only if it was mounted at least once", new object[0]);
				throw new AmDatabaseNeverMountedException();
			}
			if (base.State.IsAdminDismounted)
			{
				AmTrace.Diagnostic("Moving a dismounted database {0}. The database will be moved, but won't be mounted", new object[]
				{
					base.DatabaseName
				});
			}
			moveStatus.InitialDbState = base.State.Copy();
			AmServerName activeServer = base.State.ActiveServer;
			AmAcllReturnStatus amAcllReturnStatus = null;
			bool flag2 = true;
			try
			{
				this.CheckIfMoveApplicableForDatabase(base.State.ActiveServer, fromServer, base.ActionCode);
				bestCopySelector = this.ConstructBestCopySelector(tryOtherHealthyServers, skipValidationChecks, null, activeServer, targetServer, mountDialoverride, componentName);
				if (base.ActionCode.IsAutomaticShutdownSwitchover)
				{
					base.AttemptDismount(base.State.ActiveServer, dismountFlags, true, out ex);
					flag = true;
				}
				AmServerName amServerName = bestCopySelector.FindNextBestCopy();
				while (amServerName != null)
				{
					num++;
					if (!flag)
					{
						base.AttemptDismount(activeServer, dismountFlags, true, out ex);
						flag = true;
					}
					AmMountFlags amMountFlags = AmMountFlags.None;
					if (BitMasker.IsOn((int)skipValidationChecks, 4))
					{
						amMountFlags = AmMountFlags.MoveWithSkipHealth;
					}
					this.AttemptMountOnServer(amServerName, base.State.ActiveServer, storeMountFlags, amMountFlags, dismountFlags, mountDialoverride, skipValidationChecks, tryOtherHealthyServers, ref natSkippedServersCount, ref amAcllReturnStatus, out ex);
					base.DbTrace.Debug("AttemptMountOnServer returned AcllStatus: {0}", new object[]
					{
						amAcllReturnStatus
					});
					moveStatus.AddSubstatus(new AmDbOperationSubStatus(amServerName, amAcllReturnStatus, ex));
					if (ex == null)
					{
						flag2 = false;
						break;
					}
					bestCopySelector.ErrorLogger.ReportServerFailure(amServerName, "CopyHasBeenTriedCheck", ex.Message);
					if (ex is AmRoleChangedWhileOperationIsInProgressException)
					{
						flag2 = false;
						break;
					}
					if (ex is AmMountTimeoutException)
					{
						flag2 = false;
						break;
					}
					amServerName = bestCopySelector.FindNextBestCopy();
					base.CurrentAttemptNumber++;
				}
			}
			finally
			{
				moveStatus.FinalDbState = base.State.Copy();
			}
			if (flag2)
			{
				MountStatus storeDatabaseMountStatus = AmStoreHelper.GetStoreDatabaseMountStatus(base.State.ActiveServer, base.Database.Guid);
				if (storeDatabaseMountStatus != base.State.MountStatus)
				{
					ReplayCrimsonEvents.MismatchErrorAfterMove.Log<string, Guid, AmServerName, MountStatus, MountStatus>(base.Database.Name, base.Database.Guid, base.State.ActiveServer, base.State.MountStatus, storeDatabaseMountStatus);
					if (storeDatabaseMountStatus == MountStatus.Dismounted)
					{
						base.State.MountStatus = MountStatus.Dismounted;
						base.WriteState();
					}
				}
			}
			this.CheckActionResultsAndUpdateAdProperties(activeServer, targetServer, bestCopySelector, ex, num, natSkippedServersCount);
		}

		private void UpdateAdProperties(bool isOperationSuccess, AmServerName initialActiveServer)
		{
			if (!AmServerName.IsNullOrEmpty(base.State.ActiveServer))
			{
				bool isForceUpdate = !AmServerName.IsEqual(initialActiveServer, base.State.ActiveServer);
				if (isOperationSuccess || isForceUpdate)
				{
					base.DbTrace.Debug("Issuing async SetDatabaseLegacyDnAndOwningServer", new object[0]);
					ThreadPool.QueueUserWorkItem(delegate(object param0)
					{
						Exception ex = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
						{
							SharedDependencies.WritableADHelper.SetDatabaseLegacyDnAndOwningServer(this.DatabaseGuid, initialActiveServer, this.State.ActiveServer, isForceUpdate);
						});
						if (ex != null)
						{
							this.DbTrace.Error("SetDatabaseLegacyDnAndOwningServer failed: {0}", new object[]
							{
								ex
							});
						}
					});
					return;
				}
			}
			else
			{
				AmTrace.Error("Skipped updating ad properties for database {0} since active server is empty", new object[]
				{
					base.DatabaseName
				});
			}
		}

		private void CheckActionResultsAndUpdateAdProperties(AmServerName initialSourceServer, AmServerName targetServer, IBestCopySelector bcs, Exception lastException, int countServersTried, int natSkippedServersCount)
		{
			Exception lastException2 = bcs.LastException;
			if (countServersTried == 0)
			{
				lastException = lastException2;
			}
			else if (lastException == null && natSkippedServersCount == countServersTried)
			{
				AmTrace.Diagnostic("{0} for database {1} was not attempted in any of the servers in the DAG since all the servers in the DAG recently unsuccessful in performing the failover action.", new object[]
				{
					base.ActionCode,
					base.DatabaseName
				});
				lastException = new AmDbOperationAttempedTooSoonException(base.DatabaseName);
			}
			else if (bcs.BestCopySelectionType == AmBcsType.BestCopySelection && lastException != null && lastException2 != null)
			{
				lastException = lastException2;
			}
			this.UpdateAdProperties(lastException == null, initialSourceServer);
			AmHelper.ThrowDbActionWrapperExceptionIfNecessary(lastException);
		}

		private bool IsAcllRequired(AmServerName serverToMount, AmServerName sourceServer)
		{
			if (base.Config.DagConfig.IsThirdPartyReplEnabled)
			{
				return false;
			}
			bool result = true;
			if (base.Database.ReplicationType != ReplicationType.Remote)
			{
				AmTrace.Debug("Skipping ACLL for database '{0}' on server '{1}' since it is not replicated.", new object[]
				{
					base.DatabaseName,
					serverToMount
				});
				result = false;
			}
			else if (!base.State.IsMountSucceededAtleastOnce)
			{
				AmTrace.Debug("Skipping ACLL for database '{0}' on server '{1}' since it has never been successfully mounted.", new object[]
				{
					base.DatabaseName,
					serverToMount
				});
				result = false;
			}
			else if (AmServerName.IsEqual(serverToMount, sourceServer))
			{
				AmTrace.Debug("Skipping ACLL for database '{0}' on server '{1}' since it is already the source server.", new object[]
				{
					base.DatabaseName,
					serverToMount
				});
				result = false;
			}
			return result;
		}

		private bool AttemptMountOnServer(AmServerName serverToMount, AmServerName sourceServer, MountFlags storeMountFlags, AmMountFlags amMountFlags, UnmountFlags dismountFlags, DatabaseMountDialOverride mountDialoverride, AmBcsSkipFlags skipValidationChecks, bool tryOtherHealthyServers, ref int natSkippedServersCount, ref AmAcllReturnStatus acllStatus, out Exception lastException)
		{
			bool flag = true;
			bool fLossyMountEnabled = false;
			bool isSuccess = false;
			bool isAcllSuccess = false;
			bool isMasterServerChanged = false;
			lastException = null;
			bool isSuccess2;
			try
			{
				acllStatus = new AmAcllReturnStatus();
				AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
				if (!base.ActionCode.IsAdminOperation && base.Config.IsIgnoreServerDebugOptionEnabled(serverToMount))
				{
					ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(serverToMount.Fqdn, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), "Mount database");
					throw new AmDbOperationException("Mount not applicable for a server when debug options are enabled");
				}
				bool flag2 = this.IsAcllRequired(serverToMount, sourceServer);
				if (dbNodeAttemptTable.IsOkayForAction(base.Database, serverToMount, base.ActionCode))
				{
					if (!flag2)
					{
						acllStatus.NoLoss = true;
						acllStatus.MountAllowed = true;
					}
					else
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						try
						{
							ReplayCrimsonEvents.AcllInitiated.LogGeneric(base.PrepareSubactionArgs(new object[]
							{
								serverToMount,
								sourceServer
							}));
							AmAcllReturnStatus tempAcllStatus = null;
							lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
							{
								this.RunAttemptCopyLastLogsOnServer(serverToMount, sourceServer, mountDialoverride, skipValidationChecks, 1, 15000, 15000, 15000, out fLossyMountEnabled, ref tempAcllStatus);
								isAcllSuccess = true;
							});
							acllStatus = tempAcllStatus;
						}
						finally
						{
							stopwatch.Stop();
							if (isAcllSuccess)
							{
								ReplayCrimsonEvents.AcllSuccess2.LogGeneric(base.PrepareSubactionArgs(new object[]
								{
									serverToMount,
									fLossyMountEnabled,
									stopwatch.Elapsed
								}));
							}
							else
							{
								ReplayCrimsonEvents.AcllFailed.LogGeneric(base.PrepareSubactionArgs(new object[]
								{
									serverToMount,
									stopwatch.Elapsed,
									(lastException != null) ? lastException.Message : ReplayStrings.UnknownError
								}));
							}
						}
						if (lastException == null)
						{
							if (base.State.IsAdminDismounted && !base.ActionCode.IsAdminMountOperation)
							{
								AmTrace.Debug("Skipping mount for database '{0}' on server '{1}' since it was admin dismounted.", new object[]
								{
									base.DatabaseName,
									serverToMount
								});
								if (this.UpdateMaster(serverToMount, true))
								{
									this.SendReplicaNotifications();
									isMasterServerChanged = true;
								}
								isSuccess = true;
								flag = false;
							}
						}
						else
						{
							flag = false;
						}
					}
					if (!flag)
					{
						goto IL_45C;
					}
					isSuccess = false;
					lastException = null;
					Stopwatch stopwatch2 = new Stopwatch();
					stopwatch2.Start();
					try
					{
						ReplayCrimsonEvents.DirectMountInitiated.LogGeneric(base.PrepareSubactionArgs(new object[]
						{
							serverToMount,
							storeMountFlags,
							fLossyMountEnabled,
							amMountFlags
						}));
						lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
						{
							TimeSpan mountTimeout = this.DetermineMountTimeout(skipValidationChecks);
							this.RunMountDatabaseDirect(serverToMount, storeMountFlags, amMountFlags, fLossyMountEnabled, mountTimeout, ref isMasterServerChanged);
							isSuccess = true;
						});
						goto IL_45C;
					}
					finally
					{
						stopwatch2.Stop();
						if (isSuccess)
						{
							ReplayCrimsonEvents.DirectMountSuccess.LogGeneric(base.PrepareSubactionArgs(new object[]
							{
								serverToMount,
								stopwatch2.Elapsed
							}));
						}
						else
						{
							string text = (lastException != null) ? lastException.Message : ReplayStrings.UnknownError;
							ReplayCrimsonEvents.DirectMountFailed.LogGeneric(base.PrepareSubactionArgs(new object[]
							{
								serverToMount,
								stopwatch2.Elapsed,
								text
							}));
						}
					}
				}
				natSkippedServersCount++;
				AmTrace.Debug("Mount for database '{0}' skipped server '{1}' since a recent mount operation on this node failed.", new object[]
				{
					base.DatabaseName,
					serverToMount
				});
				lastException = new AmDbOperationAttempedTooSoonException(base.DatabaseName);
				ReplayCrimsonEvents.MountServerSkipped.LogGeneric(base.PrepareSubactionArgs(new object[]
				{
					serverToMount,
					lastException.Message
				}));
				isSuccess = false;
				IL_45C:
				isSuccess2 = isSuccess;
			}
			finally
			{
				if (!isMasterServerChanged)
				{
					AmTrace.Error("The active server for database '{0}' has not been changed, so we need to rollback the database state tracker information.", new object[]
					{
						base.DatabaseName
					});
					AmDatabaseStateTracker databaseStateTracker = AmSystemManager.Instance.DatabaseStateTracker;
					if (databaseStateTracker != null)
					{
						databaseStateTracker.UpdateActive(base.DatabaseGuid, sourceServer);
					}
				}
			}
			return isSuccess2;
		}

		private TimeSpan DetermineMountTimeout(AmBcsSkipFlags skipFlags)
		{
			int mountTimeoutInSec = RegistryParameters.MountTimeoutInSec;
			if (mountTimeoutInSec == 0 || BitMasker.IsOn((int)skipFlags, 8))
			{
				return InvokeWithTimeout.InfiniteTimeSpan;
			}
			return TimeSpan.FromSeconds((double)mountTimeoutInSec);
		}

		private void RunMountDatabaseDirect(AmServerName serverToMount, MountFlags storeMountFlags, AmMountFlags amMountFlags, bool fLossyMountEnabled, TimeSpan mountTimeout, ref bool isMasterChanged)
		{
			bool flag = false;
			bool flag2 = false;
			isMasterChanged = false;
			AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
			AmServerName activeServer = base.State.ActiveServer;
			try
			{
				isMasterChanged = this.UpdateMaster(serverToMount, false);
				base.ReportStatus(AmDbActionStatus.StoreMountInitiated);
				if (fLossyMountEnabled)
				{
					storeMountFlags |= MountFlags.AllowDatabasePatch;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2229677373U);
				try
				{
					InvokeWithTimeout.Invoke(delegate()
					{
						AmDbAction.MountDatabaseDirect(serverToMount, this.State.LastMountedServer, this.DatabaseGuid, storeMountFlags, amMountFlags, this.ActionCode);
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2441489725U);
					}, mountTimeout);
				}
				catch (TimeoutException ex)
				{
					base.DbTrace.Error("Mount timeout on {0}: {1}", new object[]
					{
						base.DatabaseName,
						ex
					});
					AmMountTimeoutException ex2 = new AmMountTimeoutException(base.DatabaseName, serverToMount.NetbiosName, (int)mountTimeout.TotalSeconds, ex);
					this.HandleMountTimeout(serverToMount, ex2);
					throw ex2;
				}
				flag = true;
				dbNodeAttemptTable.ClearFailedTime(base.DatabaseGuid);
				try
				{
					base.WriteStateMountSuccess();
				}
				catch (AmRoleChangedWhileOperationIsInProgressException ex3)
				{
					base.DbTrace.Warning("Mount AmRoleChanged exception (error={0})", new object[]
					{
						ex3
					});
				}
				base.ReportStatus(AmDbActionStatus.StoreMountSuccessful);
			}
			catch (TransientException ex4)
			{
				base.DbTrace.Error("Mount transient exception (error={0})", new object[]
				{
					ex4
				});
				flag2 = true;
				throw;
			}
			catch (AmReplayServiceDownException ex5)
			{
				base.DbTrace.Error("Mount transient RPC exception (error={0})", new object[]
				{
					ex5
				});
				flag2 = true;
				throw;
			}
			finally
			{
				if (flag)
				{
					SharedDependencies.WritableADHelper.ResetAllowFileRestoreDsFlag(base.DatabaseGuid, activeServer, serverToMount);
				}
				else
				{
					base.WriteStateMountFailed(true);
					if (base.ActionCode.IsAutomaticFailureItem && !flag2)
					{
						dbNodeAttemptTable.MarkFailedTime(base.DatabaseGuid, serverToMount, base.ActionCode);
					}
					base.ReportStatus(AmDbActionStatus.StoreMountFailed);
				}
				if (isMasterChanged && AmSystemManager.Instance.Config.IsPAM)
				{
					this.SendReplicaNotifications();
				}
			}
		}

		private void HandleMountTimeout(AmServerName serverThatFailedToMount, AmMountTimeoutException timeoutException)
		{
			ReplayCrimsonEvents.MountTimeout.Log<string, Guid, string, string>(base.DatabaseName, base.DatabaseGuid, serverThatFailedToMount.Fqdn, timeoutException.Message);
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.TimeoutFailure, AmDbActionCategory.Move);
			AmDbMoveOperation amDbMoveOperation = new AmDbMoveOperation(base.Database, actionCode);
			AmDbMoveArguments arguments = amDbMoveOperation.Arguments;
			arguments.MountDialOverride = DatabaseMountDialOverride.None;
			arguments.MoveComment = timeoutException.Message;
			arguments.SourceServer = serverThatFailedToMount;
			amDbMoveOperation.Arguments = arguments;
			amDbMoveOperation.Enqueue();
		}

		private void RunAttemptCopyLastLogsOnServer(AmServerName serverToMount, AmServerName sourceServer, DatabaseMountDialOverride mountDialOverride, AmBcsSkipFlags skipValidationChecks, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, out bool fLossyMountEnabled, ref AmAcllReturnStatus acllStatus)
		{
			acllStatus = null;
			bool flag = false;
			fLossyMountEnabled = false;
			try
			{
				base.ReportStatus(AmDbActionStatus.AcllInitiated);
				AmAcllArgs amAcllArgs = new AmAcllArgs();
				amAcllArgs.NumRetries = numRetries;
				amAcllArgs.E00TimeoutMs = e00timeoutMs;
				amAcllArgs.NetworkIOTimeoutMs = networkIOtimeoutMs;
				amAcllArgs.NetworkConnectTimeoutMs = networkConnecttimeoutMs;
				amAcllArgs.SourceServer = sourceServer;
				amAcllArgs.ActionCode = base.ActionCode;
				amAcllArgs.MountDialOverride = mountDialOverride;
				amAcllArgs.SkipValidationChecks = skipValidationChecks;
				amAcllArgs.UniqueOperationId = base.UniqueOperationId;
				amAcllArgs.SubactionAttemptNumber = base.CurrentAttemptNumber;
				if (base.State.IsAdminDismounted && !base.ActionCode.IsAdminMountOperation)
				{
					amAcllArgs.MountPending = false;
				}
				else
				{
					amAcllArgs.MountPending = true;
				}
				AmDbAction.AttemptCopyLastLogsDirect(serverToMount, base.DatabaseGuid, amAcllArgs, ref acllStatus);
				fLossyMountEnabled = (!acllStatus.NoLoss && acllStatus.MountAllowed);
				flag = true;
				base.ReportStatus(AmDbActionStatus.AcllSuccessful);
			}
			finally
			{
				if (!flag)
				{
					base.ReportStatus(AmDbActionStatus.AcllFailed);
				}
			}
		}

		private bool UpdateMaster(AmServerName newMasterServer, bool isMountSkipped)
		{
			bool flag = false;
			AmServerName activeServer = base.State.ActiveServer;
			if (!AmServerName.IsEqual(activeServer, newMasterServer))
			{
				AmTrace.Debug("Active server changed. (previous={0}, current={1})", new object[]
				{
					activeServer,
					newMasterServer
				});
				flag = true;
			}
			base.ReportStatus(AmDbActionStatus.UpdateMasterServerInitiated);
			bool flag2 = isMountSkipped ? base.WriteStateMountSkipped(newMasterServer) : base.WriteStateMountStart(newMasterServer);
			if (flag2)
			{
				AmDatabaseStateTracker databaseStateTracker = AmSystemManager.Instance.DatabaseStateTracker;
				if (databaseStateTracker != null)
				{
					databaseStateTracker.UpdateActive(base.DatabaseGuid, newMasterServer);
				}
			}
			if (flag)
			{
				if (!activeServer.IsEmpty)
				{
					ReplayCrimsonEvents.ActiveServerChanged.LogGeneric(base.PrepareSubactionArgs(new object[]
					{
						activeServer,
						newMasterServer
					}));
				}
				else
				{
					ReplayCrimsonEvents.ActiveServerChanged.LogGeneric(base.PrepareSubactionArgs(new object[]
					{
						"<none>",
						newMasterServer
					}));
				}
			}
			base.ReportStatus(AmDbActionStatus.UpdateMasterServerFinished);
			return flag;
		}

		private void SendReplicaNotifications()
		{
			AmMultiNodeReplicaNotifier amMultiNodeReplicaNotifier = new AmMultiNodeReplicaNotifier(base.Database, base.ActionCode, true);
			amMultiNodeReplicaNotifier.SendAllNotifications();
		}

		private IBestCopySelector ConstructBestCopySelector(bool tryOtherServers, AmBcsSkipFlags skipValidationChecks, AmMultiNodeCopyStatusFetcher statusFetcher, AmServerName initialFromServer, AmServerName targetServer, DatabaseMountDialOverride mountDialOverride, string componentName)
		{
			if (tryOtherServers)
			{
				return new AmBestCopySelection(base.DatabaseGuid, base.Database, base.ActionCode, statusFetcher, initialFromServer, base.Config, mountDialOverride, skipValidationChecks, new AmDbAction.PrepareSubactionArgsDelegate(base.PrepareSubactionArgs), componentName);
			}
			return new AmSingleCopySelection(base.DatabaseGuid, base.Database, base.ActionCode, initialFromServer, targetServer, base.Config, skipValidationChecks, new AmDbAction.PrepareSubactionArgsDelegate(base.PrepareSubactionArgs));
		}

		private void CheckIfMoveApplicableForDatabase(AmServerName activeServer, AmServerName moveFromServer, AmDbActionCode actionCode)
		{
			if (base.Database.ReplicationType != ReplicationType.Remote)
			{
				AmTrace.Debug("Move ignored for database {0} since it is not replicated.", new object[]
				{
					base.DatabaseName
				});
				throw new AmDbMoveOperationNotSupportedException(base.DatabaseName);
			}
			if (!AmServerName.IsNullOrEmpty(moveFromServer))
			{
				if (!AmServerName.IsEqual(activeServer, moveFromServer))
				{
					AmTrace.Diagnostic("Move ignored for database {0} since the master server is different from the server that originally initiated the failover. (master={1}, movedInitiatedFrom={2})", new object[]
					{
						base.DatabaseName,
						activeServer,
						moveFromServer
					});
					throw new AmDbMoveOperationNoLongerApplicableException(base.DatabaseName, moveFromServer.NetbiosName, activeServer.NetbiosName);
				}
				if (actionCode.Reason == AmDbActionReason.TimeoutFailure)
				{
					bool flag = AmStoreHelper.IsMounted(moveFromServer, base.DatabaseGuid);
					if (flag)
					{
						throw new AmDbMoveOperationOnTimeoutFailureCancelled(base.DatabaseName, moveFromServer.NetbiosName);
					}
				}
			}
			if (base.Config.DagConfig.MemberServers.Length < 2)
			{
				AmTrace.Diagnostic("Move ignored for database {0} since according to active manager there is just one server in the DAG)", new object[]
				{
					base.DatabaseName
				});
				throw new AmDbMoveOperationNotSupportedException(base.DatabaseName);
			}
		}
	}
}
