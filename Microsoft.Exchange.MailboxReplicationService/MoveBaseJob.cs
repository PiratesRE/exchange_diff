using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MoveBaseJob : BaseJob
	{
		public MoveBaseJob()
		{
		}

		public Guid MailboxGuid { get; private set; }

		public List<MailboxMover> MbxContexts { get; private set; }

		public bool CleanupMailboxSD { get; private set; }

		public virtual bool IsOnlineMove { get; protected set; }

		protected virtual bool ReachedThePointOfNoReturn
		{
			get
			{
				return base.TimeTracker.CurrentState == RequestState.Completed || base.TimeTracker.CurrentState == RequestState.CompletedWithWarnings || base.SyncStage >= SyncStage.Cleanup;
			}
		}

		protected bool IsIntegNeeded
		{
			get
			{
				return CommonUtils.GetBitMask(this.isIntegData, 1) == 1;
			}
			set
			{
				CommonUtils.SetBitMask(ref this.isIntegData, 1, value);
			}
		}

		protected bool IsIntegDoneOnce
		{
			get
			{
				return CommonUtils.GetBitMask(this.isIntegData, 2) == 2;
			}
			set
			{
				CommonUtils.SetBitMask(ref this.isIntegData, 2, value);
			}
		}

		protected override bool IsInFinalization
		{
			get
			{
				return base.TimeTracker.CurrentState == RequestState.Completed || base.TimeTracker.CurrentState == RequestState.CompletedWithWarnings || base.SyncStage >= SyncStage.FinalIncrementalSync;
			}
		}

		public bool IsPushingToTitanium
		{
			get
			{
				return base.CachedRequestJob.Direction == RequestDirection.Push && base.CachedRequestJob.TargetVersion < Server.E2007MinVersion && base.CachedRequestJob.RequestStyle == RequestStyle.CrossOrg;
			}
		}

		protected override int CopyStartPercentage
		{
			get
			{
				return 25;
			}
		}

		protected override int CopyEndPercentage
		{
			get
			{
				return 95;
			}
		}

		protected override bool RelinquishAfterOfflineMoveFailure
		{
			get
			{
				return !this.IsOnlineMove && this.CanBeCanceledOrSuspended() && !this.IsPushingToTitanium;
			}
		}

		public override void Initialize(TransactionalRequestJob moveRequest)
		{
			this.MailboxGuid = moveRequest.ExchangeGuid;
			if (moveRequest.RequestType == MRSRequestType.Move)
			{
				base.RequestKeyGuid = moveRequest.ExchangeGuid;
			}
			base.Initialize(moveRequest);
			MrsTracer.Service.Function("MoveBaseJob.Initialize: mailboxGuid=\"{0}\", destMdbGuid={1}, sourceDcName={2}, destDcName={3}", new object[]
			{
				moveRequest.ExchangeGuid,
				(moveRequest.TargetDatabase != null) ? moveRequest.TargetDatabase.ObjectGuid : Guid.Empty,
				moveRequest.SourceDCName,
				moveRequest.TargetDCName
			});
			this.preserveMailboxSignature = moveRequest.PreserveMailboxSignature;
			this.restartingAfterSignatureChange = moveRequest.RestartingAfterSignatureChange;
			this.isIntegData = (moveRequest.IsIntegData ?? 0);
			this.IsOnlineMove = !moveRequest.IsOffline;
			this.skipPreFinalSyncDataProcessing = moveRequest.SkipPreFinalSyncDataProcessing;
			base.RequestJobIdentity = string.Empty;
			if (moveRequest.UserId != null)
			{
				base.RequestJobIdentity = moveRequest.UserId.ToString();
			}
			this.MbxContexts = new List<MailboxMover>();
			if (moveRequest.RequestType == MRSRequestType.PublicFolderMigration || moveRequest.RequestType == MRSRequestType.PublicFolderMailboxMigration || moveRequest.RequestType == MRSRequestType.PublicFolderMove || moveRequest.RequestType == MRSRequestType.FolderMove)
			{
				return;
			}
			MailboxCopierFlags mailboxCopierFlags = MailboxCopierFlags.None;
			if (moveRequest.RequestStyle == RequestStyle.CrossOrg)
			{
				mailboxCopierFlags |= MailboxCopierFlags.CrossOrg;
			}
			string orgID = (moveRequest.OrganizationId != null && moveRequest.OrganizationId.OrganizationalUnit != null) ? (moveRequest.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			if (moveRequest.PrimaryIsMoving)
			{
				TenantPartitionHint partitionHint = moveRequest.PartitionHint;
				MailboxMover item = new MailboxMover(moveRequest, this, mailboxCopierFlags | MailboxCopierFlags.Root, MrsStrings.PrimaryMailboxTracingId(orgID, moveRequest.ExchangeGuid), moveRequest.ExchangeGuid, partitionHint);
				this.MbxContexts.Add(item);
				if (moveRequest.RequestType == MRSRequestType.Move && moveRequest.TargetContainerGuid != null)
				{
					IEnumerable<ContainerMailboxInformation> mailboxContainerMailboxes = RemoteSourceMailbox.GetMailboxContainerMailboxes(moveRequest.SourceServer, moveRequest.SourceMDBGuid, moveRequest.ExchangeGuid);
					foreach (ContainerMailboxInformation containerMailboxInformation in from m in mailboxContainerMailboxes
					where m.MailboxGuid != moveRequest.ExchangeGuid
					select m)
					{
						TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(containerMailboxInformation.PersistableTenantPartitionHint);
						MailboxCopierFlags mailboxCopierFlags2 = mailboxCopierFlags;
						mailboxCopierFlags2 |= ((partitionHint.GetExternalDirectoryOrganizationId() == tenantPartitionHint.GetExternalDirectoryOrganizationId()) ? MailboxCopierFlags.ContainerAggregated : MailboxCopierFlags.ContainerOrg);
						item = new MailboxMover(moveRequest, this, mailboxCopierFlags2, MrsStrings.ContainerMailboxTracingId(moveRequest.TargetContainerGuid.Value, containerMailboxInformation.MailboxGuid), containerMailboxInformation.MailboxGuid, tenantPartitionHint);
						this.MbxContexts.Add(item);
					}
				}
			}
			if (moveRequest.ArchiveIsMoving)
			{
				MailboxCopierFlags mailboxCopierFlags3 = MailboxCopierFlags.SourceIsArchive | MailboxCopierFlags.TargetIsArchive | mailboxCopierFlags;
				if (!moveRequest.PrimaryIsMoving)
				{
					mailboxCopierFlags3 |= MailboxCopierFlags.Root;
				}
				MailboxMover item2 = new MailboxMover(moveRequest, this, mailboxCopierFlags3, MrsStrings.ArchiveMailboxTracingId(orgID, moveRequest.ArchiveGuid.Value), moveRequest.ArchiveGuid.Value);
				this.MbxContexts.Add(item2);
			}
			this.CleanupMailboxSD = (moveRequest.SourceVersion < Server.E14MinVersion && moveRequest.TargetVersion >= Server.E14MinVersion);
		}

		protected MailboxCopierBase GetRootMailboxContext()
		{
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (mailboxCopierBase.IsRoot)
				{
					return mailboxCopierBase;
				}
			}
			return null;
		}

		public override List<MailboxCopierBase> GetAllCopiers()
		{
			List<MailboxCopierBase> list = new List<MailboxCopierBase>();
			foreach (MailboxMover item in this.MbxContexts)
			{
				list.Add(item);
			}
			return list;
		}

		public override void ValidateAndPopulateRequestJob(List<ReportEntry> entries)
		{
			if (base.CachedRequestJob is MailboxRelocationRequestStatistics)
			{
				return;
			}
			throw new NotImplementedException();
		}

		protected override bool CanBeCanceledOrSuspended()
		{
			return !this.ReachedThePointOfNoReturn;
		}

		protected override void CleanupCanceledJob()
		{
			this.DeleteReplica();
		}

		protected override bool ResetAfterFailure(out bool relinquishJobNow)
		{
			Exception ex = base.LastFailure;
			if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
			{
				WellKnownException.MapiImportFailure
			}))
			{
				ex = ex.InnerException;
				base.LastFailure = ex;
			}
			if (this.preserveMailboxSignature && CommonUtils.ExceptionIs(ex, new WellKnownException[]
			{
				WellKnownException.MapiCannotRegisterMapping
			}))
			{
				RestartMoveSignatureChangeTransientException failure = new RestartMoveSignatureChangeTransientException(ex);
				base.Report.Append(MrsStrings.ReportMoveRestartedDueToSignatureChange, failure, ReportEntryFlags.Source);
				FailureLog.Write(base.RequestJobGuid, failure, false, base.TimeTracker.CurrentState, base.SyncStage, null, null);
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveRestartedDueToSignatureChange, new object[]
				{
					base.RequestJobIdentity,
					this.MailboxGuid.ToString(),
					CommonUtils.FullExceptionMessage(ex).ToString()
				});
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
					{
						mbxCtx.ClearSyncState(SyncStateClearReason.MailboxSignatureChange);
					});
				}, delegate(Exception saveFailure)
				{
					LocalizedString localizedString = CommonUtils.FullExceptionMessage(saveFailure);
					base.Report.Append(MrsStrings.ReportSyncStateSaveFailed2(CommonUtils.GetFailureType(saveFailure)), saveFailure, ReportEntryFlags.Cleanup);
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SyncStateSaveFailed, new object[]
					{
						base.RequestJobIdentity,
						this.MailboxGuid.ToString(),
						base.RequestJobStoringMDB.ToString(),
						localizedString
					});
				});
				this.restartingAfterSignatureChange = true;
				ex = new MailboxSignatureChangedTransientException(ex);
				base.LastFailure = ex;
			}
			if (ex is TooManyMissingItemsPermanentException && !this.IsIntegDoneOnce && !ConfigBase<MRSConfigSchema>.GetConfig<bool>("DisableAutomaticRepair") && base.IsMailboxCapabilitySupportedBy(MailboxCapabilities.RunIsInteg, true))
			{
				this.IsIntegNeeded = true;
				RestartMoveSourceCorruptionTransientException failure2 = new RestartMoveSourceCorruptionTransientException(ex);
				base.Report.Append(MrsStrings.ReportMoveRestartedDueToSourceCorruption, failure2, ReportEntryFlags.Source);
				FailureLog.Write(base.RequestJobGuid, failure2, false, base.TimeTracker.CurrentState, base.SyncStage, null, null);
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveRestartedDueToMailboxCorruption, new object[]
				{
					base.RequestJobIdentity,
					this.MailboxGuid.ToString(),
					CommonUtils.FullExceptionMessage(ex).ToString()
				});
				CommonUtils.CatchKnownExceptions(delegate
				{
					base.SaveRequest(false, delegate(TransactionalRequestJob rj)
					{
						rj.RestartFromScratch = true;
						rj.IsIntegData = new int?(this.isIntegData);
					});
				}, delegate(Exception saveFailure)
				{
					LocalizedString localizedString = CommonUtils.FullExceptionMessage(saveFailure);
					base.Report.Append(MrsStrings.ReportSyncStateSaveFailed2(CommonUtils.GetFailureType(saveFailure)), saveFailure, ReportEntryFlags.Cleanup);
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SyncStateSaveFailed, new object[]
					{
						base.RequestJobIdentity,
						base.GetRequestKeyGuid().ToString(),
						base.RequestJobStoringMDB.ToString(),
						localizedString
					});
				});
				ex = new MailboxCorruptionTransientException(ex);
				base.LastFailure = ex;
			}
			if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
			{
				WellKnownException.GlobalCounterRangeExceeded
			}))
			{
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_IncrementalMoveRestartDueToGlobalCounterRangeDepletion, new object[]
				{
					base.RequestJobIdentity,
					this.MailboxGuid.ToString(),
					CommonUtils.FullExceptionMessage(ex).ToString()
				});
			}
			if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
			{
				WellKnownException.MRSMailboxIsLocked
			}))
			{
				ex = this.ProcessMailboxLockedFailure(ex);
				base.LastFailure = ex;
			}
			return base.ResetAfterFailure(out relinquishJobNow);
		}

		protected override void UpdateRequestAfterFailure(RequestJobBase rj, Exception failure)
		{
			base.UpdateRequestAfterFailure(rj, failure);
			rj.IsOffline = !this.IsOnlineMove;
			rj.PreserveMailboxSignature = this.preserveMailboxSignature;
			rj.RestartingAfterSignatureChange = this.restartingAfterSignatureChange;
			rj.IsIntegData = new int?(this.isIntegData);
			if (CommonUtils.ExceptionIs(failure, new WellKnownException[]
			{
				WellKnownException.MRSUpdateMovedMailbox,
				WellKnownException.MRSTransient
			}))
			{
				rj.DomainControllerToUpdate = null;
				rj.RemoteDomainControllerToUpdate = null;
			}
			if (CommonUtils.ExceptionIs(failure, new WellKnownException[]
			{
				WellKnownException.GlobalCounterRangeExceeded
			}))
			{
				rj.IncrementallyUpdateGlobalCounterRanges = true;
			}
			rj.TimeTracker.SetTimestamp(RequestJobTimestamp.LastProgressCheckpoint, new DateTime?(base.GetTimeAtWhichLastProgressWasMade()));
		}

		protected override void CleanupAfterFailure()
		{
			if (!this.IsOnlineMove && this.CanBeCanceledOrSuspended())
			{
				MrsTracer.Service.Debug("Cleaning up destination mailbox due to offline move failure", new object[0]);
				using (List<MailboxMover>.Enumerator enumerator = this.MbxContexts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxMover mbxCtx = enumerator.Current;
						CommonUtils.CatchKnownExceptions(delegate
						{
							if (mbxCtx.IsDestinationConnected && mbxCtx.DestMailbox.MailboxExists())
							{
								mbxCtx.DestMailbox.DeleteMailbox(7);
								RestartMoveOfflineTransientException failure = new RestartMoveOfflineTransientException(mbxCtx.TargetTracingID);
								this.Report.Append(MrsStrings.ReportRemovingTargetMailboxDueToOfflineMoveFailure(mbxCtx.TargetTracingID), failure, ReportEntryFlags.None);
								FailureLog.Write(this.RequestJobGuid, failure, false, this.TimeTracker.CurrentState, this.SyncStage, null, null);
							}
						}, null);
					}
				}
			}
		}

		protected virtual void CleanupSourceMailbox(MailboxMover mbxCtx, bool moveIsSuccessful)
		{
		}

		protected virtual void CleanupDestinationMailbox(MailboxCopierBase mbxCtx, bool moveIsSuccessful)
		{
		}

		protected virtual MailboxChangesManifest EnumerateSourceHierarchyChanges(MailboxCopierBase mbxCtx, bool catchup, SyncContext syncContext)
		{
			if (catchup)
			{
				return mbxCtx.SourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags.Catchup, 0);
			}
			return mbxCtx.EnumerateHierarchyChanges(syncContext);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.MbxContexts != null)
			{
				foreach (MailboxMover mailboxMover in this.MbxContexts)
				{
					mailboxMover.UnconfigureProviders();
				}
				this.MbxContexts.Clear();
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MoveBaseJob>(this);
		}

		protected override void MakeConnections()
		{
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
			}
			bool flag = true;
			bool flag2 = false;
			Exception sourceConnectFailure = null;
			Exception targetConnectFailure = null;
			int sourcePrimaryVersion = 0;
			int sourceArchiveVersion = 0;
			int targetPrimaryVersion = 0;
			int targetArchiveVersion = 0;
			string sourcePrimaryServerName = null;
			string sourceArchiveServerName = null;
			string targetPrimaryServerName = null;
			string targetArchiveServerName = null;
			LocalizedString reasonForMoveFinished = LocalizedString.Empty;
			using (List<MailboxCopierBase>.Enumerator enumerator2 = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator2.Current;
					CommonUtils.CatchKnownExceptions(delegate
					{
						MrsTracer.Service.Debug("Attempting to connect to the destination mailbox {0}.", new object[]
						{
							mbxCtx.TargetTracingID
						});
						mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
					}, delegate(Exception failure)
					{
						MrsTracer.Service.Warning("Failed to connect to destination mailbox: {0}", new object[]
						{
							CommonUtils.FullExceptionMessage(failure)
						});
						if (targetConnectFailure == null)
						{
							targetConnectFailure = failure;
						}
					});
					if (targetConnectFailure == null)
					{
						MailboxInformation mailboxInformation = mbxCtx.DestMailbox.GetMailboxInformation();
						MailboxServerInformation mailboxServerInformation;
						bool flag3;
						mbxCtx.DestMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation, out flag3);
						mbxCtx.TargetServerInfo = mailboxServerInformation;
						if (mailboxServerInformation != null)
						{
							if (mbxCtx.ShouldReportEntry(ReportEntryKind.TargetConnection))
							{
								base.Report.Append(MrsStrings.ReportDestinationMailboxConnection(mbxCtx.TargetTracingID, mailboxServerInformation.ServerInfoString, (mailboxInformation != null) ? mailboxInformation.MdbName : "(null)"), new ConnectivityRec(mbxCtx.IsPrimary ? ServerKind.Target : ServerKind.TargetArchive, mailboxInformation, mailboxServerInformation));
							}
							if (flag3)
							{
								base.Report.Append(MrsStrings.ReportDatabaseFailedOver);
								throw new RelinquishJobDatabaseFailoverTransientException();
							}
							if (mbxCtx.IsPrimary)
							{
								targetPrimaryVersion = mailboxServerInformation.MailboxServerVersion;
								targetPrimaryServerName = mailboxServerInformation.MailboxServerName;
							}
							else
							{
								targetArchiveVersion = mailboxServerInformation.MailboxServerVersion;
								targetArchiveServerName = mailboxServerInformation.MailboxServerName;
							}
						}
						if (!mbxCtx.DestMailbox.MailboxExists())
						{
							MrsTracer.Service.Debug("Destination mailbox does not exist.", new object[0]);
							flag = false;
						}
						else
						{
							MrsTracer.Service.Debug("Destination mailbox {0} exists.", new object[]
							{
								mbxCtx.TargetTracingID
							});
							if (this.restartingAfterSignatureChange)
							{
								flag = false;
							}
							else
							{
								SyncStateError syncStateError = mbxCtx.LoadSyncState(base.Report);
								if (syncStateError != SyncStateError.None)
								{
									flag = false;
									RestartMoveCorruptSyncStateTransientException failure3 = new RestartMoveCorruptSyncStateTransientException(mbxCtx.TargetTracingID);
									FailureLog.Write(base.RequestJobGuid, failure3, false, base.TimeTracker.CurrentState, base.SyncStage, null, null);
									if (base.CachedRequestJob.FailOnCorruptSyncState)
									{
										throw new CorruptSyncStatePermanentException(mbxCtx.TargetTracingID);
									}
									base.Report.Append(MrsStrings.ReportRestartingMoveBecauseSyncStateDoesNotExist(mbxCtx.TargetTracingID), failure3, ReportEntryFlags.Target);
								}
							}
							if (base.SyncStage >= SyncStage.Cleanup)
							{
								flag2 = true;
							}
							else if (!this.ValidateHomeMDBValue(mbxCtx, mailboxInformation))
							{
								flag2 = true;
								reasonForMoveFinished = MrsStrings.ReportHomeMdbPointsToTarget;
							}
							else if (!this.ValidateTargetMailbox(mailboxInformation, out reasonForMoveFinished))
							{
								flag2 = true;
							}
							else
							{
								uint num = (mbxCtx.DestMailboxWrapper.LastConnectedServerInfo != null) ? mbxCtx.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion : 0U;
								if (this.preserveMailboxSignature && mbxCtx.SyncState != null && mbxCtx.SyncState.MailboxSignatureVersion != num)
								{
									flag = false;
									RestartMoveSignatureVersionMismatchTransientException failure2 = new RestartMoveSignatureVersionMismatchTransientException(mbxCtx.TargetTracingID, mbxCtx.SyncState.MailboxSignatureVersion, num);
									base.Report.Append(MrsStrings.ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent(mbxCtx.TargetTracingID, mbxCtx.SyncState.MailboxSignatureVersion, num), failure2, ReportEntryFlags.Target);
									FailureLog.Write(base.RequestJobGuid, failure2, false, base.TimeTracker.CurrentState, base.SyncStage, null, null);
								}
							}
						}
					}
				}
			}
			if (flag2)
			{
				base.Report.Append(MrsStrings.ReportMoveAlreadyFinished2(reasonForMoveFinished));
				if (base.SyncStage < SyncStage.Cleanup)
				{
					this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
					{
						if (mbxCtx.SyncState == null)
						{
							mbxCtx.SyncState = new PersistedSyncData(base.RequestJobGuid);
						}
						mbxCtx.SyncState.CompletedCleanupTasks |= (PostMoveCleanupStatusFlags.SourceMailboxCleanup | PostMoveCleanupStatusFlags.UpdateSourceMailbox);
					});
				}
				base.ScheduleWorkItem<int>(new Action<int>(this.PostMoveCleanup), 0, WorkloadType.Unknown);
				return;
			}
			using (List<MailboxCopierBase>.Enumerator enumerator3 = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator3.Current;
					CommonUtils.CatchKnownExceptions(delegate
					{
						MrsTracer.Service.Debug("Connecting to the source mailbox {0}.", new object[]
						{
							mbxCtx.SourceTracingID
						});
						mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.None);
					}, delegate(Exception failure)
					{
						MrsTracer.Service.Warning("Failed to connect to source mailbox: {0}", new object[]
						{
							CommonUtils.FullExceptionMessage(failure)
						});
						if (sourceConnectFailure == null)
						{
							sourceConnectFailure = failure;
						}
					});
					if (sourceConnectFailure == null)
					{
						MailboxInformation mailboxInformation2 = mbxCtx.SourceMailbox.GetMailboxInformation();
						MailboxServerInformation mailboxServerInformation2;
						bool flag4;
						mbxCtx.SourceMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation2, out flag4);
						mbxCtx.SourceServerInfo = mailboxServerInformation2;
						if (mailboxServerInformation2 != null)
						{
							if (mbxCtx.ShouldReportEntry(ReportEntryKind.SourceConnection))
							{
								base.Report.Append(MrsStrings.ReportSourceMailboxConnection(mbxCtx.SourceTracingID, mailboxServerInformation2.ServerInfoString, (mailboxInformation2 != null) ? mailboxInformation2.MdbName : "(null)"), new ConnectivityRec(mbxCtx.IsPrimary ? ServerKind.Source : ServerKind.SourceArchive, mailboxInformation2, mailboxServerInformation2));
							}
							if (flag4)
							{
								base.Report.Append(MrsStrings.ReportDatabaseFailedOver);
								throw new RelinquishJobDatabaseFailoverTransientException();
							}
							if (mbxCtx.IsPrimary)
							{
								sourcePrimaryVersion = mailboxServerInformation2.MailboxServerVersion;
								sourcePrimaryServerName = mailboxServerInformation2.MailboxServerName;
							}
							else
							{
								sourceArchiveVersion = mailboxServerInformation2.MailboxServerVersion;
								sourceArchiveServerName = mailboxServerInformation2.MailboxServerName;
							}
						}
						MailboxMover mailboxMover = mbxCtx as MailboxMover;
						if (mailboxMover != null && mbxCtx.IsDestinationConnected && mbxCtx.DestMailbox.MailboxExists() && !mailboxMover.CompareRootFolders())
						{
							MrsTracer.Service.Warning("Root folder EntryIDs don't match", new object[0]);
							flag = false;
						}
					}
				}
			}
			if (!flag)
			{
				base.SyncStage = SyncStage.None;
				base.OverallProgress = 0;
				this.restartingAfterSignatureChange = false;
			}
			if (sourceConnectFailure == null)
			{
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(DateTime.UtcNow));
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.SourceConnectionFailure, null);
			}
			if (targetConnectFailure == null)
			{
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulTargetConnection, new DateTime?(DateTime.UtcNow));
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.TargetConnectionFailure, null);
			}
			if (sourceConnectFailure != null || targetConnectFailure != null)
			{
				base.CheckRequestIsValid();
				if (sourceConnectFailure != null)
				{
					throw sourceConnectFailure;
				}
				if (targetConnectFailure != null)
				{
					throw targetConnectFailure;
				}
			}
			base.SaveRequest(true, delegate(TransactionalRequestJob rj)
			{
				if (rj.RestartFromScratch)
				{
					MrsTracer.Service.Debug("Move must be restarted from scratch.", new object[0]);
					this.SyncStage = SyncStage.None;
					this.OverallProgress = 0;
				}
				this.TimeTracker.SetTimestamp(RequestJobTimestamp.Failure, null);
				this.TimeTracker.SetTimestamp(RequestJobTimestamp.Suspended, null);
				rj.FailureCode = null;
				rj.FailureType = null;
				rj.FailureSide = null;
				rj.Message = LocalizedString.Empty;
				rj.Status = RequestStatus.InProgress;
				rj.SourceServer = sourcePrimaryServerName;
				rj.SourceVersion = sourcePrimaryVersion;
				rj.SourceArchiveServer = sourceArchiveServerName;
				rj.SourceArchiveVersion = sourceArchiveVersion;
				rj.TargetServer = targetPrimaryServerName;
				rj.TargetVersion = targetPrimaryVersion;
				rj.TargetArchiveServer = targetArchiveServerName;
				rj.TargetArchiveVersion = targetArchiveVersion;
				rj.RestartingAfterSignatureChange = this.restartingAfterSignatureChange;
				if (this.SyncStage >= SyncStage.FinalIncrementalSync)
				{
					this.TimeTracker.CurrentState = RequestState.Completion;
					return;
				}
				this.TimeTracker.CurrentState = RequestState.InitialSeeding;
			});
			foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
			{
				mailboxCopierBase2.ExchangeSourceAndTargetVersions();
			}
			this.StartMoveOrStoreIsIntegTask();
		}

		protected void RemoveExchangeServersDenyACE(RawSecurityDescriptor mailboxSD, MailboxMover mbxCtx)
		{
			if (mailboxSD == null || mailboxSD.DiscretionaryAcl == null)
			{
				return;
			}
			MappedPrincipal mappedPrincipal = new MappedPrincipal();
			mappedPrincipal.MailboxGuid = WellKnownPrincipalMapper.ExchangeServers;
			MappedPrincipal[] array = mbxCtx.SourceMailbox.ResolvePrincipals(new MappedPrincipal[]
			{
				mappedPrincipal
			});
			SecurityIdentifier securityIdentifier = (array != null && array.Length > 0 && array[0] != null) ? array[0].ObjectSid : null;
			if (securityIdentifier == null)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			foreach (GenericAce genericAce in mailboxSD.DiscretionaryAcl)
			{
				if (genericAce.AceType == AceType.AccessDenied)
				{
					CommonAce commonAce = genericAce as CommonAce;
					if ((commonAce.AccessMask & 1) != 0 && securityIdentifier.Equals(commonAce.SecurityIdentifier))
					{
						num = num2;
						break;
					}
				}
				num2++;
			}
			if (num != -1)
			{
				MrsTracer.Service.Debug("Removing Deny FullAccess ExchangeServers ACE from '{0}' security descriptor", new object[]
				{
					mbxCtx.SourceTracingID
				});
				mailboxSD.DiscretionaryAcl.RemoveAce(num);
			}
		}

		protected abstract void MigrateSecurityDescriptors();

		protected abstract void UpdateMovedMailbox();

		protected virtual void UpdateSourceMailbox()
		{
		}

		protected override void UpdateRequestOnSave(TransactionalRequestJob rj, UpdateRequestOnSaveType updateType)
		{
			rj.IsOffline = !this.IsOnlineMove;
			rj.PreserveMailboxSignature = this.preserveMailboxSignature;
			rj.RestartingAfterSignatureChange = this.restartingAfterSignatureChange;
			rj.IsIntegData = new int?(this.isIntegData);
			if (updateType == UpdateRequestOnSaveType.InMemoryUpdatesOnly)
			{
				return;
			}
			if (!this.IsInFinalization)
			{
				MailboxCopierBase archiveCtx = null;
				foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
				{
					if (!mailboxCopierBase.IsRoot)
					{
						archiveCtx = mailboxCopierBase;
					}
				}
				CommonUtils.CatchKnownExceptions(delegate
				{
					MailboxInformation mailboxInformation = this.GetRootMailboxContext().SourceMailbox.GetMailboxInformation();
					MailboxInformation mailboxInformation2 = (archiveCtx != null) ? archiveCtx.SourceMailbox.GetMailboxInformation() : null;
					rj.TotalMailboxItemCount = mailboxInformation.MailboxItemCount;
					rj.TotalMailboxSize = mailboxInformation.MailboxSize;
					if (mailboxInformation2 != null)
					{
						rj.TotalArchiveItemCount = new ulong?(mailboxInformation2.MailboxItemCount);
						rj.TotalArchiveSize = new ulong?(mailboxInformation2.MailboxSize);
					}
				}, null);
			}
		}

		protected override void SaveSyncState()
		{
			if (!this.IsOnlineMove && base.SyncStage < SyncStage.Cleanup)
			{
				MrsTracer.Service.Debug("Not saving sync state for an offline move", new object[0]);
				return;
			}
			base.SaveSyncState();
		}

		protected void ForeachMailboxContext(Action<MailboxMover> del)
		{
			foreach (MailboxMover obj in this.MbxContexts)
			{
				del(obj);
			}
		}

		private void StartMoveOrStoreIsIntegTask()
		{
			if (this.IsIntegNeeded && !this.IsIntegDoneOnce)
			{
				DateTime? timestamp = base.TimeTracker.GetTimestamp(RequestJobTimestamp.IsIntegStarted);
				bool flag = false;
				foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
				{
					if (!mailboxCopierBase.IsIntegDone)
					{
						mailboxCopierBase.StartIsInteg();
						flag = true;
						if (timestamp == null)
						{
							base.TimeTracker.SetTimestamp(RequestJobTimestamp.IsIntegStarted, new DateTime?(DateTime.UtcNow));
						}
						else if (DateTime.UtcNow - timestamp > ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("AutomaticRepairAbandonInterval"))
						{
							throw new MailboxCorruptionRepairAbandonedPermanentException(timestamp.Value);
						}
					}
				}
				if (flag)
				{
					base.ScheduleWorkItem(TimeSpan.FromSeconds(30.0), new Action(this.QueryStoreIsInteg), WorkloadType.Unknown);
					return;
				}
				this.IsIntegDoneOnce = true;
			}
			base.ScheduleWorkItem(new Action(this.StartMove), WorkloadType.Unknown);
		}

		private void QueryStoreIsInteg()
		{
			base.RefreshRequestIfNeeded();
			bool flag = true;
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (!mailboxCopierBase.IsIntegDone && !mailboxCopierBase.QueryIsInteg())
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				base.ScheduleWorkItem(TimeSpan.FromSeconds(30.0), new Action(this.QueryStoreIsInteg), WorkloadType.Unknown);
				return;
			}
			this.IsIntegDoneOnce = true;
			base.ScheduleWorkItem(new Action(this.StartMove), WorkloadType.Unknown);
		}

		private void StartMove()
		{
			MrsTracer.Service.Debug("WorkItem: StartMove", new object[0]);
			this.IsOnlineMove = !base.CachedRequestJob.ForceOfflineMove;
			base.CheckServersHealth();
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (mailboxCopierBase.SupportsRuleAPIs)
				{
					mailboxCopierBase.SourceMailbox.ConfigMailboxOptions(MailboxOptions.IgnoreExtendedRuleFAIs);
				}
				if (this.IsOnlineMove)
				{
					MrsTracer.Service.Debug("Switching source mailbox {0} to SyncSource mode.", new object[]
					{
						mailboxCopierBase.SourceTracingID
					});
					bool flag;
					this.SetSourceInTransitStatus(mailboxCopierBase, InTransitStatus.SyncSource, out flag);
					if (!flag)
					{
						MrsTracer.Service.Debug("Source does not support online move, using offline move mode.", new object[0]);
						this.IsOnlineMove = false;
					}
				}
				else
				{
					MrsTracer.Service.Debug("Switching source mailbox {0} to MoveSource mode.", new object[]
					{
						mailboxCopierBase.SourceTracingID
					});
					bool flag;
					this.SetSourceInTransitStatus(mailboxCopierBase, InTransitStatus.MoveSource, out flag);
				}
			}
			if (this.IsOnlineMove && base.SyncStage != SyncStage.None)
			{
				foreach (MailboxCopierBase destinationInTransitStatus in this.GetAllCopiers())
				{
					this.SetDestinationInTransitStatus(destinationInTransitStatus);
				}
			}
			base.TimeTracker.SetTimestamp(RequestJobTimestamp.MailboxLocked, null);
			if (this.IsOnlineMove && base.SyncStage != SyncStage.None)
			{
				MrsTracer.Service.Debug("Recovering an interrupted move.", new object[0]);
				foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
				{
					mailboxCopierBase2.SourceMailbox.SetMailboxSyncState(mailboxCopierBase2.ICSSyncState.ProviderState);
				}
				this.ForeachMailboxContext(delegate(MailboxMover mover)
				{
					mover.VerifyAndUpdateDestinationMailboxSignature(this.preserveMailboxSignature);
				});
				if (base.CachedRequestJob.IncrementallyUpdateGlobalCounterRanges)
				{
					this.ForeachMailboxContext(delegate(MailboxMover mover)
					{
						mover.FinalizeStoreSignaturePreservingMailboxMove(this.preserveMailboxSignature);
					});
					base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob request)
					{
						request.IncrementallyUpdateGlobalCounterRanges = false;
					});
				}
				this.ForeachMailboxContext(delegate(MailboxMover mover)
				{
					mover.UpdateMappingMetadata(this.preserveMailboxSignature);
				});
				this.ScheduleNextWorkItem();
				base.Report.Append(MrsStrings.ReportRequestContinued(base.SyncStage.ToString()));
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_RequestContinued, new object[]
				{
					base.RequestJobIdentity,
					this.MailboxGuid.ToString(),
					base.SyncStage.ToString()
				});
				return;
			}
			if (!this.IsOnlineMove)
			{
				if (base.CachedRequestJob.PreventCompletion)
				{
					throw new CannotPreventCompletionForOfflineMovePermanentException();
				}
				MrsTracer.Service.Debug("Doing offline move. Switching all source mailboxes to offline state", new object[0]);
				foreach (MailboxCopierBase mbxCtx2 in this.GetAllCopiers())
				{
					bool flag2;
					this.SetSourceInTransitStatus(mbxCtx2, InTransitStatus.MoveSource, out flag2);
				}
			}
			base.Report.Append(MrsStrings.ReportRequestStarted);
			this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				mbxCtx.ReportSourceMailboxSize();
			});
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveStarted, new object[]
			{
				base.RequestJobIdentity,
				this.MailboxGuid.ToString(),
				base.CachedRequestJob.SourceMDBName,
				base.CachedRequestJob.TargetMDBName,
				base.CachedRequestJob.Flags.ToString()
			});
			base.ScheduleWorkItem(new Action(this.CleanupOrphanedDestinationMailbox), WorkloadType.Unknown);
		}

		protected virtual void CleanupOrphanedDestinationMailbox()
		{
			MrsTracer.Service.Debug("WorkItem: CleanupOrphanedDestinationMailbox", new object[0]);
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			bool flag = false;
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				mailboxCopierBase.ClearSyncState(SyncStateClearReason.CleanupOrphanedMailbox);
				if (mailboxCopierBase.DestMailbox.MailboxExists())
				{
					MrsTracer.Service.Debug("Deleting destination mailbox {0}.", new object[]
					{
						mailboxCopierBase.TargetTracingID
					});
					mailboxCopierBase.DestMailbox.DeleteMailbox(2);
					flag = true;
				}
			}
			int num = 0;
			if (flag)
			{
				base.Report.Append(MrsStrings.ReportMailboxRemovedRetrying(30));
				base.FlushReport(null);
				num = 30;
			}
			base.ScheduleWorkItem<int>(TimeSpan.FromSeconds((double)num), new Action<int>(this.CreateDestinationMailbox), 0, WorkloadType.Unknown);
		}

		private void CreateDestinationMailbox(int iCreationAttempts)
		{
			MrsTracer.Service.Debug("WorkItem: CreateDestinationMailbox", new object[0]);
			iCreationAttempts++;
			base.RefreshRequestIfNeeded();
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			bool retryCreation = false;
			LocalizedString retryMailboxID = LocalizedString.Empty;
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator.Current;
					ExecutionContext.Create(new DataContext[]
					{
						new SimpleValueDataContext("Mailbox", mbxCtx.TargetTracingID)
					}).Execute(delegate
					{
						if (mbxCtx.DestMailbox.MailboxExists())
						{
							return;
						}
						MailboxSignatureFlags mailboxSignatureFlags = MailboxSignatureFlags.None;
						if (mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo.MailboxShapeVersion >= 3U && mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo.MailboxShapeVersion >= 3U && this.CachedRequestJob.RequestStyle == RequestStyle.IntraOrg)
						{
							mailboxSignatureFlags |= MailboxSignatureFlags.GetMailboxShape;
						}
						if (mbxCtx.Flags.HasFlag(MailboxCopierFlags.ContainerAggregated) || mbxCtx.Flags.HasFlag(MailboxCopierFlags.ContainerOrg))
						{
							mailboxSignatureFlags |= MailboxSignatureFlags.GetLegacy;
						}
						else if (this.preserveMailboxSignature && mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion >= 3U && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion >= 4U && mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion == mbxCtx.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion)
						{
							mailboxSignatureFlags |= MailboxSignatureFlags.GetMailboxSignature;
						}
						else
						{
							mailboxSignatureFlags |= MailboxSignatureFlags.GetLegacy;
							this.preserveMailboxSignature = false;
							if (iCreationAttempts == 1 && mbxCtx.ShouldReportEntry(ReportEntryKind.SignaturePreservation))
							{
								this.Report.Append(MrsStrings.ReportMailboxSignatureIsNotPreserved(mbxCtx.SourceTracingID));
							}
						}
						CreateMailboxResult createMailboxResult = CreateMailboxResult.Success;
						try
						{
							IL_1C4:
							MrsTracer.Service.Debug("Reading basic mailbox info from the source {0}, flags {1}.", new object[]
							{
								mbxCtx.SourceTracingID,
								mailboxSignatureFlags
							});
							byte[] mailboxBasicInfo = mbxCtx.SourceMailbox.GetMailboxBasicInfo(mailboxSignatureFlags);
							MrsTracer.Service.Debug("Creating destination mailbox.", new object[0]);
							createMailboxResult = mbxCtx.DestMailbox.CreateMailbox(mailboxBasicInfo, mailboxSignatureFlags);
						}
						catch (Exception ex)
						{
							if (CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
							{
								WellKnownException.MapiCannotPreserveSignature,
								WellKnownException.MapiCannotRegisterMapping
							}))
							{
								MrsTracer.Service.Warning("Unable to preserve mailbox signature, error {0}", new object[]
								{
									CommonUtils.FullExceptionMessage(ex)
								});
								if (this.preserveMailboxSignature)
								{
									mailboxSignatureFlags &= ~MailboxSignatureFlags.GetMailboxSignature;
									mailboxSignatureFlags |= MailboxSignatureFlags.GetLegacy;
									this.preserveMailboxSignature = false;
									this.Report.Append(MrsStrings.ReportUnableToPreserveMailboxSignature(mbxCtx.SourceTracingID));
									MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_UnableToPreserveMailboxSignature, new object[]
									{
										this.RequestJobIdentity,
										this.MailboxGuid.ToString(),
										CommonUtils.FullExceptionMessage(ex).ToString()
									});
									goto IL_1C4;
								}
							}
							throw;
						}
						switch (createMailboxResult)
						{
						case CreateMailboxResult.CleanupNotComplete:
							MrsTracer.Service.Debug("Creation failed: destination mailbox tombstone already exists. It should be cleaned up automatically by the Store shortly.", new object[0]);
							if (iCreationAttempts <= this.TestIntegration.MaxTombstoneRetries)
							{
								retryMailboxID = mbxCtx.TargetTracingID;
								retryCreation = true;
								return;
							}
							MrsTracer.Service.Error("Giving up mailbox creation attempts.", new object[0]);
							throw new DestinationMailboxNotCleanedUpPermanentException(this.MailboxGuid);
						case CreateMailboxResult.ObjectNotFound:
							MrsTracer.Service.Error("Giving up mailbox creation attempts.", new object[0]);
							throw new DestinationADNotUpToDatePermanentException(this.MailboxGuid);
						default:
							this.SetDestinationInTransitStatus(mbxCtx);
							mbxCtx.PostCreateDestinationMailbox();
							return;
						}
					});
				}
			}
			if (retryCreation)
			{
				base.Report.Append(MrsStrings.ReportRetryingMailboxCreation(retryMailboxID, (int)MoveBaseJob.TombstoneCleanupRetryDelay.TotalSeconds, iCreationAttempts, base.TestIntegration.MaxTombstoneRetries));
				base.FlushReport(null);
				MrsTracer.Service.Debug("Will retry mailbox creation in {0}", new object[]
				{
					MoveBaseJob.TombstoneCleanupRetryDelay
				});
				base.ScheduleWorkItem<int>(MoveBaseJob.TombstoneCleanupRetryDelay, new Action<int>(this.CreateDestinationMailbox), iCreationAttempts, WorkloadType.Unknown);
				return;
			}
			if (!this.IsOnlineMove)
			{
				this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
				{
					MrsTracer.Service.Debug("Switching source mailbox {0} to MoveSource (offline) mode.", new object[]
					{
						mbxCtx.SourceTracingID
					});
					bool flag;
					this.SetSourceInTransitStatus(mbxCtx, InTransitStatus.MoveSource, out flag);
					this.SetDestinationInTransitStatus(mbxCtx);
				});
			}
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				mailboxCopierBase.SyncState = new PersistedSyncData(base.RequestJobGuid);
				mailboxCopierBase.ICSSyncState = new MailboxMapiSyncState();
				uint num = (mailboxCopierBase.DestMailboxWrapper.LastConnectedServerInfo != null) ? mailboxCopierBase.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion : 0U;
				MrsTracer.Service.Debug("Destination mailbox {0} created successfully with MbxSignatureVersion {1}", new object[]
				{
					mailboxCopierBase.TargetTracingID,
					num
				});
				mailboxCopierBase.SyncState.MailboxSignatureVersion = num;
			}
			base.SyncStage = SyncStage.MailboxCreated;
			base.OverallProgress = 5;
			base.TimeTracker.CurrentState = RequestState.CreatingMailbox;
			if (base.TimeTracker.GetTimestamp(RequestJobTimestamp.Start) == null)
			{
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.Start, new DateTime?(DateTime.UtcNow));
			}
			base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
		}

		protected virtual void CatchupFolderHierarchy()
		{
			if (this.IsOnlineMove)
			{
				MrsTracer.Service.Debug("WorkItem: catchup folder hierarchy.", new object[0]);
				foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
				{
					this.EnumerateSourceHierarchyChanges(mailboxCopierBase, true, null);
					mailboxCopierBase.ICSSyncState.ProviderState = mailboxCopierBase.SourceMailbox.GetMailboxSyncState();
					mailboxCopierBase.SaveICSSyncState(true);
				}
			}
			base.SyncStage = SyncStage.CreatingFolderHierarchy;
			base.OverallProgress = 10;
			base.TimeTracker.CurrentState = RequestState.CreatingFolderHierarchy;
			base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob moveRequest)
			{
				moveRequest.RestartFromScratch = false;
			});
			base.ScheduleWorkItem(new Action(this.CreateFolderHierarchy), WorkloadType.Unknown);
		}

		private void CreateFolderHierarchy()
		{
			MrsTracer.Service.Debug("WorkItem: create folder hierarchy.", new object[0]);
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MoveBaseJob.<>c__DisplayClassa3 CS$<>8__locals1 = new MoveBaseJob.<>c__DisplayClassa3();
					CS$<>8__locals1.mbxCtx = enumerator.Current;
					base.RefreshRequestIfNeeded();
					if (this.IsOnlineMove)
					{
						base.CheckServersHealth();
					}
					MailboxInformation mailboxInformation = CS$<>8__locals1.mbxCtx.SourceMailbox.GetMailboxInformation();
					if (base.CachedRequestJob.IgnoreRuleLimitErrors && mailboxInformation.RulesSize > 32768)
					{
						MailboxInformation mailboxInformation2 = CS$<>8__locals1.mbxCtx.DestMailbox.GetMailboxInformation();
						if (mailboxInformation2.ServerVersion < Server.E2007MinVersion)
						{
							base.Report.Append(MrsStrings.ReportRulesWillNotBeCopied);
						}
					}
					FolderRecDataFlags dataToCopy = FolderRecDataFlags.SearchCriteria;
					if (!base.CachedRequestJob.SkipFolderPromotedProperties)
					{
						dataToCopy |= FolderRecDataFlags.PromotedProperties;
					}
					if (!base.CachedRequestJob.SkipFolderViews)
					{
						dataToCopy |= FolderRecDataFlags.Views;
					}
					if (!base.CachedRequestJob.SkipFolderRestrictions)
					{
						dataToCopy |= FolderRecDataFlags.Restrictions;
					}
					FolderMap sourceFolderMap = CS$<>8__locals1.mbxCtx.GetSourceFolderMap(GetFolderMapFlags.ForceRefresh);
					CS$<>8__locals1.mbxCtx.MailboxSizeTracker.ResetFoldersProcessed(sourceFolderMap.Count);
					if (CS$<>8__locals1.mbxCtx.ShouldReportEntry(ReportEntryKind.StartingFolderHierarchyCreation))
					{
						base.Report.Append(MrsStrings.ReportInitializingFolderHierarchy(CS$<>8__locals1.mbxCtx.SourceTracingID, sourceFolderMap.Count));
					}
					CS$<>8__locals1.mbxCtx.PreProcessHierarchy();
					base.RefreshRequestIfNeeded();
					if (this.IsOnlineMove)
					{
						base.CheckServersHealth();
					}
					ExDateTime utcNextICSSave = ExDateTime.UtcNow + BaseJob.FlushInterval;
					sourceFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.AllFolders, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext context)
					{
						this.RefreshRequestIfNeeded();
						if (this.IsOnlineMove)
						{
							this.CheckServersHealth();
						}
						this.SaveState(SaveStateFlags.Lazy | SaveStateFlags.DontSaveRequestJob | SaveStateFlags.DontReportSyncStage, CS$<>8__locals1.mbxCtx, null);
						ExDateTime utcNow = ExDateTime.UtcNow;
						if (utcNow > utcNextICSSave)
						{
							CS$<>8__locals1.mbxCtx.SaveICSSyncState(true);
							utcNextICSSave = utcNow + BaseJob.FlushInterval;
						}
						ExecutionContext.Create(new DataContext[]
						{
							new FolderRecWrapperDataContext(folderRec)
						}).Execute(delegate
						{
							if (!CS$<>8__locals1.mbxCtx.ShouldCreateFolder(context, folderRec))
							{
								MrsTracer.Service.Debug("ShouldCreateFolder returned false for folder {0} in mailbox {1}. Skipping it.", new object[]
								{
									folderRec.FullFolderName,
									CS$<>8__locals1.mbxCtx.TargetTracingID
								});
								return;
							}
							byte[] entryId = folderRec.EntryId;
							FolderStateSnapshot folderStateSnapshot = CS$<>8__locals1.mbxCtx.ICSSyncState[entryId];
							if (folderStateSnapshot.State.HasFlag(FolderState.Created))
							{
								MrsTracer.Service.Debug("FolderState {0} indicates that the folder has already been created and it's properties copied. Skipping it.", new object[]
								{
									folderStateSnapshot.State
								});
								CS$<>8__locals1.mbxCtx.MailboxSizeTracker.IncrementFoldersProcessed();
								return;
							}
							using (ISourceFolder folder = CS$<>8__locals1.mbxCtx.SourceMailbox.GetFolder(entryId))
							{
								if (folder == null)
								{
									MrsTracer.Service.Warning("Folder '{0}' {1} disappeared, skipping", new object[]
									{
										folderRec.FolderName,
										folderRec.EntryId
									});
								}
								else
								{
									folderRec.EnsureDataLoaded(folder, dataToCopy, CS$<>8__locals1.mbxCtx);
									CS$<>8__locals1.mbxCtx.TranslateFolderData(folderRec);
									CreateFolderFlags createFolderFlags = CreateFolderFlags.None;
									if (CS$<>8__locals1.mbxCtx.IsPublicFolderMigration && CS$<>8__locals1.mbxCtx.IsRoot && CS$<>8__locals1.mbxCtx.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.CanStoreCreatePFDumpster))
									{
										createFolderFlags = CreateFolderFlags.CreatePublicFolderDumpster;
									}
									byte[] entryId2;
									CS$<>8__locals1.mbxCtx.CreateFolder(context, folderRec, createFolderFlags, out entryId2);
									if (folderRec.FolderType != FolderType.Search)
									{
										using (IDestinationFolder folder2 = CS$<>8__locals1.mbxCtx.DestMailbox.GetFolder(entryId2))
										{
											if (folder2 == null)
											{
												MrsTracer.Service.Error("Something deleted destination folder from under us", new object[0]);
												throw new UnexpectedErrorPermanentException(-2147221238);
											}
											bool flag;
											CS$<>8__locals1.mbxCtx.CopyFolderProperties(folderRec, folder, folder2, dataToCopy, out flag);
											ExAssert.RetailAssert(!flag, "at this stage, we should have had no problems copying properties");
										}
										CS$<>8__locals1.mbxCtx.MailboxSizeTracker.IncrementFoldersProcessed();
									}
									folderStateSnapshot.State |= FolderState.Created;
								}
							}
						});
					});
					sourceFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.SearchFolders, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext context)
					{
						this.RefreshRequestIfNeeded();
						if (this.IsOnlineMove)
						{
							this.CheckServersHealth();
						}
						this.SaveState(SaveStateFlags.Lazy | SaveStateFlags.DontSaveRequestJob | SaveStateFlags.DontReportSyncStage, CS$<>8__locals1.mbxCtx, null);
						ExDateTime utcNow = ExDateTime.UtcNow;
						if (utcNow > utcNextICSSave)
						{
							CS$<>8__locals1.mbxCtx.SaveICSSyncState(true);
							utcNextICSSave = utcNow + BaseJob.FlushInterval;
						}
						ExecutionContext.Create(new DataContext[]
						{
							new FolderRecWrapperDataContext(folderRec)
						}).Execute(delegate
						{
							byte[] entryId = folderRec.EntryId;
							FolderStateSnapshot folderStateSnapshot = CS$<>8__locals1.mbxCtx.ICSSyncState[entryId];
							if (folderStateSnapshot.State.HasFlag(FolderState.SearchFolderCopied))
							{
								MrsTracer.Service.Debug("FolderState {0} indicates that the search folder has already been created and it's properties copied. Skipping it.", new object[]
								{
									folderStateSnapshot.State
								});
								return;
							}
							using (ISourceFolder folder = CS$<>8__locals1.mbxCtx.SourceMailbox.GetFolder(entryId))
							{
								if (folder == null)
								{
									MrsTracer.Service.Debug("Source folder got deleted recently. Skipping, will sync this folder deletion later.", new object[0]);
									return;
								}
								using (IDestinationFolder folder2 = CS$<>8__locals1.mbxCtx.DestMailbox.GetFolder(folderRec.EntryId))
								{
									if (folder2 == null)
									{
										MrsTracer.Service.Error("Something deleted destination folder from under us", new object[0]);
										throw new UnexpectedErrorPermanentException(-2147221238);
									}
									bool flag;
									CS$<>8__locals1.mbxCtx.CopyFolderProperties(folderRec, folder, folder2, dataToCopy, out flag);
									ExAssert.RetailAssert(!flag, "at this stage, we should have had no problems copying properties");
								}
							}
							folderStateSnapshot.State |= FolderState.SearchFolderCopied;
							folderStateSnapshot.UpdateFolderDataCopied(folderRec.FolderRec);
							CS$<>8__locals1.mbxCtx.MailboxSizeTracker.IncrementFoldersProcessed();
						});
					});
					CS$<>8__locals1.mbxCtx.SaveICSSyncState(true);
					base.Report.Append(MrsStrings.ReportFolderHierarchyInitialized(CS$<>8__locals1.mbxCtx.TargetTracingID, CS$<>8__locals1.mbxCtx.MailboxSizeTracker.FoldersProcessed));
				}
			}
			base.SyncStage = SyncStage.CreatingInitialSyncCheckpoint;
			base.OverallProgress = 15;
			base.TimeTracker.CurrentState = RequestState.CreatingInitialSyncCheckpoint;
			base.SaveState(SaveStateFlags.Regular, null);
			base.ScheduleWorkItem(new Action(this.CatchupFolders), WorkloadType.Unknown);
		}

		protected void CatchupFolders()
		{
			if (this.IsOnlineMove)
			{
				MrsTracer.Service.Debug("WorkItem: catchup folder contents.", new object[0]);
				base.FoldersProcessed = 0;
				using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MoveBaseJob.<>c__DisplayClassad CS$<>8__locals1 = new MoveBaseJob.<>c__DisplayClassad();
						CS$<>8__locals1.mbxCtx = enumerator.Current;
						base.RefreshRequestIfNeeded();
						if (this.IsOnlineMove)
						{
							base.CheckServersHealth();
						}
						CS$<>8__locals1.mbxCtx.SourceMailbox.SetMailboxSyncState(CS$<>8__locals1.mbxCtx.ICSSyncState.ProviderState);
						FolderMap destinationFolderMap = CS$<>8__locals1.mbxCtx.GetDestinationFolderMap(GetFolderMapFlags.ForceRefresh);
						FolderMap srcFolderMap = CS$<>8__locals1.mbxCtx.GetSourceFolderMap(GetFolderMapFlags.None);
						base.TotalFolders = srcFolderMap.Count;
						ExDateTime utcNextICSSave = ExDateTime.UtcNow + BaseJob.FlushInterval;
						destinationFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper destFolderRec, FolderMap.EnumFolderContext ctx)
						{
							this.SaveState(SaveStateFlags.Lazy | SaveStateFlags.DontSaveRequestJob | SaveStateFlags.DontReportSyncStage, CS$<>8__locals1.mbxCtx, null);
							ExDateTime utcNow = ExDateTime.UtcNow;
							if (utcNow > utcNextICSSave)
							{
								CS$<>8__locals1.mbxCtx.ICSSyncState.ProviderState = CS$<>8__locals1.mbxCtx.SourceMailbox.GetMailboxSyncState();
								CS$<>8__locals1.mbxCtx.SaveICSSyncState(true);
								utcNextICSSave = utcNow + BaseJob.FlushInterval;
							}
							ExecutionContext.Create(new DataContext[]
							{
								new FolderRecWrapperDataContext(destFolderRec)
							}).Execute(delegate
							{
								if (!CS$<>8__locals1.mbxCtx.IsContentAvailableInTargetMailbox(destFolderRec))
								{
									MrsTracer.Service.Debug("Ignoring folder '{0}' since it's contents do not reside in this mailbox", new object[]
									{
										destFolderRec.FullFolderName
									});
									return;
								}
								byte[] sourceFolderEntryId = CS$<>8__locals1.mbxCtx.GetSourceFolderEntryId(destFolderRec);
								FolderStateSnapshot folderStateSnapshot = CS$<>8__locals1.mbxCtx.ICSSyncState[sourceFolderEntryId];
								if (folderStateSnapshot.State.HasFlag(FolderState.CatchupFolderComplete))
								{
									MrsTracer.Service.Debug("FolderState {0} indicates that catchup has already been executed for the folder. Skipping it.", new object[]
									{
										folderStateSnapshot.State
									});
									this.FoldersProcessed++;
									return;
								}
								FolderRecWrapper folderRecWrapper = srcFolderMap[sourceFolderEntryId];
								if (folderRecWrapper == null)
								{
									return;
								}
								using (ISourceFolder folder = CS$<>8__locals1.mbxCtx.SourceMailbox.GetFolder(sourceFolderEntryId))
								{
									if (folder == null)
									{
										MrsTracer.Service.Debug("Source folder '{0}' has disappeared, skipping", new object[]
										{
											folderRecWrapper.FullFolderName
										});
										return;
									}
									CS$<>8__locals1.mbxCtx.CatchupFolder(folderRecWrapper.FolderRec, folder);
								}
								folderStateSnapshot.State |= FolderState.CatchupFolderComplete;
								this.FoldersProcessed++;
							});
						});
						CS$<>8__locals1.mbxCtx.ICSSyncState.ProviderState = CS$<>8__locals1.mbxCtx.SourceMailbox.GetMailboxSyncState();
						CS$<>8__locals1.mbxCtx.SaveICSSyncState(true);
					}
				}
			}
			base.Report.Append(MrsStrings.ReportInitialSyncCheckpointCompleted(base.FoldersProcessed));
			base.SyncStage = SyncStage.LoadingMessages;
			base.OverallProgress = 20;
			base.TimeTracker.CurrentState = RequestState.LoadingMessages;
			base.SaveState(SaveStateFlags.Regular, null);
			base.ScheduleWorkItem(new Action(this.InitializeCopyMessageStatistics), WorkloadType.Unknown);
		}

		private void InitializeCopyMessageStatistics()
		{
			MrsTracer.Service.Debug("WorkItem: initializing job statistics.", new object[0]);
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MoveBaseJob.<>c__DisplayClassb4 CS$<>8__locals1 = new MoveBaseJob.<>c__DisplayClassb4();
					CS$<>8__locals1.mbxCtx = enumerator.Current;
					base.RefreshRequestIfNeeded();
					if (this.IsOnlineMove)
					{
						base.CheckServersHealth();
					}
					FolderMap srcFolderMap = CS$<>8__locals1.mbxCtx.GetSourceFolderMap(GetFolderMapFlags.None);
					base.TotalFolders = srcFolderMap.Count;
					base.FoldersProcessed = 0;
					FolderMap destinationFolderMap = CS$<>8__locals1.mbxCtx.GetDestinationFolderMap(GetFolderMapFlags.None);
					bool foldersPending = false;
					destinationFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper destFolderRec, FolderMap.EnumFolderContext ctx)
					{
						if (!CS$<>8__locals1.mbxCtx.IsContentAvailableInTargetMailbox(destFolderRec))
						{
							MrsTracer.Service.Debug("Ignoring folder '{0}' since it's contents do not reside in this mailbox", new object[]
							{
								destFolderRec.FullFolderName
							});
							return;
						}
						byte[] sourceFolderEntryId = CS$<>8__locals1.mbxCtx.GetSourceFolderEntryId(destFolderRec);
						FolderStateSnapshot folderStateSnapshot = CS$<>8__locals1.mbxCtx.ICSSyncState[sourceFolderEntryId];
						if (folderStateSnapshot.State.HasFlag(FolderState.CopyMessagesComplete))
						{
							MrsTracer.Service.Debug("FolderState {0} indicates that CopyMessages has already been executed for the folder. Using persisted statistics.", new object[]
							{
								folderStateSnapshot.State
							});
							CS$<>8__locals1.mbxCtx.MailboxSizeTracker.TrackFolder(folderStateSnapshot);
							this.FoldersProcessed++;
							return;
						}
						foldersPending = true;
						FolderRecWrapper folderRecWrapper = srcFolderMap[sourceFolderEntryId];
						if (folderRecWrapper != null)
						{
							MrsTracer.Service.Debug("FolderState {0} indicates that CopyMessages has not yet been executed for the folder. Using estimated statistics.", new object[]
							{
								folderStateSnapshot.State
							});
							CS$<>8__locals1.mbxCtx.MailboxSizeTracker.TrackFolder(folderRecWrapper.FolderRec);
						}
					});
					destinationFolderMap.ResetFolderHierarchyEnumerator();
					CS$<>8__locals1.mbxCtx.MailboxSizeTracker.IsFinishedEstimating = true;
					CS$<>8__locals1.mbxCtx.CopyMessagesCompleted = !foldersPending;
				}
			}
			base.ScheduleWorkItem(new Action(this.EnumerateFolderMessages), WorkloadType.Unknown);
		}

		private void EnumerateFolderMessages()
		{
			MrsTracer.Service.Debug("WorkItem: enumerating folders to create a list of messages.", new object[0]);
			base.TestIntegration.Barrier("PostponeEnumerateFolderMessages", new Action(base.RefreshRequestIfNeeded));
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator.Current;
					if (!mbxCtx.CopyMessagesCompleted)
					{
						base.RefreshRequestIfNeeded();
						if (this.IsOnlineMove)
						{
							base.CheckServersHealth();
						}
						FolderMap sourceFolderMap = mbxCtx.GetSourceFolderMap(GetFolderMapFlags.None);
						FolderMap destinationFolderMap = mbxCtx.GetDestinationFolderMap(GetFolderMapFlags.None);
						EntryIdMap<FolderStateSnapshot> foldersToCopy = new EntryIdMap<FolderStateSnapshot>();
						Dictionary<FolderStateSnapshot, FolderRecWrapper> destinationFolderRecs = new Dictionary<FolderStateSnapshot, FolderRecWrapper>(base.TestIntegration.FolderBatchSize);
						List<MessageRec> messagesToCopy = new List<MessageRec>();
						int numFoldersEnumeratedInCurrentBatch = 0;
						IEnumerator<FolderRecWrapper> folderHierarchyEnumerator = destinationFolderMap.GetFolderHierarchyEnumerator(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder);
						while (folderHierarchyEnumerator.MoveNext())
						{
							FolderRecWrapper destFolderRec = folderHierarchyEnumerator.Current;
							byte[] sourceFolderEntryId = mbxCtx.GetSourceFolderEntryId(destFolderRec);
							FolderRecWrapper srcFolderRec = sourceFolderMap[sourceFolderEntryId];
							if (srcFolderRec != null)
							{
								ExecutionContext.Create(new DataContext[]
								{
									new SimpleValueDataContext("Mailbox", mbxCtx.SourceTracingID),
									new FolderRecWrapperDataContext(srcFolderRec)
								}).Execute(delegate
								{
									if (!mbxCtx.IsContentAvailableInTargetMailbox(destFolderRec))
									{
										MrsTracer.Service.Debug("Ignoring folder '{0}' since it's contents do not reside in this mailbox", new object[]
										{
											destFolderRec.FullFolderName
										});
										return;
									}
									FolderStateSnapshot folderStateSnapshot = mbxCtx.ICSSyncState[sourceFolderEntryId];
									if (folderStateSnapshot.State.HasFlag(FolderState.CopyMessagesComplete))
									{
										MrsTracer.Service.Debug("FolderState {0} indicates that CopyMessages has already been executed for the folder \"{1}\" in mailbox {2}. Skipping it.", new object[]
										{
											folderStateSnapshot.State,
											srcFolderRec.FullFolderName,
											mbxCtx.TargetTracingID
										});
										return;
									}
									using (ISourceFolder folder = mbxCtx.SourceMailbox.GetFolder(srcFolderRec.EntryId))
									{
										if (folder == null)
										{
											MrsTracer.Service.Debug("Source folder \"{0}\" got deleted. Skipping it.", new object[]
											{
												srcFolderRec.FullFolderName
											});
										}
										else
										{
											EnumerateMessagesFlags enumerateMessagesFlags = EnumerateMessagesFlags.RegularMessages | EnumerateMessagesFlags.DeletedMessages | EnumerateMessagesFlags.IncludeExtendedData;
											if (mbxCtx.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.IcsMidSetCheck))
											{
												enumerateMessagesFlags |= EnumerateMessagesFlags.SkipICSMidSetMissing;
											}
											MrsTracer.Service.Debug("Enumerating messages in folder \"{0}\" with flags {1}.", new object[]
											{
												srcFolderRec.FullFolderName,
												enumerateMessagesFlags
											});
											List<MessageRec> list = folder.EnumerateMessages(enumerateMessagesFlags, null);
											folderStateSnapshot.TotalMessages = 0;
											folderStateSnapshot.TotalMessageByteSize = 0UL;
											folderStateSnapshot.MessagesWritten = 0;
											folderStateSnapshot.MessageByteSizeWritten = 0UL;
											folderStateSnapshot.SoftDeletedMessageCount = 0;
											folderStateSnapshot.TotalSoftDeletedMessageSize = 0UL;
											List<MessageRec> list2 = new List<MessageRec>();
											if (list != null && list.Count > 0)
											{
												MrsTracer.Service.Debug("{0} message(s) found in folder \"{1}\" for mailbox {2}.", new object[]
												{
													list.Count,
													srcFolderRec.FullFolderName,
													mbxCtx.SourceTracingID
												});
												foreach (MessageRec messageRec in list)
												{
													if (messageRec.IsDeleted)
													{
														folderStateSnapshot.SoftDeletedMessageCount++;
														folderStateSnapshot.TotalSoftDeletedMessageSize += (ulong)((long)messageRec.MessageSize);
													}
													else
													{
														list2.Add(messageRec);
														folderStateSnapshot.TotalMessageByteSize += (ulong)((long)messageRec.MessageSize);
													}
												}
											}
											bool flag = false;
											if (list2.Count == 0)
											{
												MrsTracer.Service.Debug("No messages to be copied for the folder \"{0}\" in mailbox {1}. Seeding next folder.", new object[]
												{
													srcFolderRec.FullFolderName,
													mbxCtx.SourceTracingID
												});
												mbxCtx.MailboxSizeTracker.TrackFolder(folderStateSnapshot);
											}
											else
											{
												MrsTracer.Service.Debug("Enumerated {0} normal messages for folder \"{1}\" in mailbox {2}", new object[]
												{
													list2.Count,
													srcFolderRec.FullFolderName,
													mbxCtx.SourceTracingID
												});
												int messagesWritten;
												ulong messageByteSizeWritten;
												ulong totalMessageByteSize;
												List<MessageRec> messagesToCopy = folderStateSnapshot.GetMessagesToCopy(list2, MessageRecSortBy.AscendingTimeStamp, out messagesWritten, out messageByteSizeWritten, out totalMessageByteSize);
												folderStateSnapshot.TotalMessages = list2.Count;
												folderStateSnapshot.TotalMessageByteSize = totalMessageByteSize;
												folderStateSnapshot.MessagesWritten = messagesWritten;
												folderStateSnapshot.MessageByteSizeWritten = messageByteSizeWritten;
												mbxCtx.MailboxSizeTracker.TrackFolder(folderStateSnapshot);
												if (messagesToCopy.Count == 0)
												{
													MrsTracer.Service.Debug("Messages already copied for folder \"{0}\" in mailbox {1}. Seeding next folder.", new object[]
													{
														srcFolderRec.FullFolderName,
														mbxCtx.SourceTracingID
													});
												}
												else
												{
													MrsTracer.Service.Debug("Collecting {0} messages to copy for folder \"{1}\" in mailbox {2}.", new object[]
													{
														messagesToCopy.Count,
														srcFolderRec.FullFolderName,
														mbxCtx.SourceTracingID
													});
													foldersToCopy[messagesToCopy[0].FolderId] = folderStateSnapshot;
													destinationFolderRecs[folderStateSnapshot] = destFolderRec;
													messagesToCopy.AddRange(messagesToCopy);
													numFoldersEnumeratedInCurrentBatch++;
													flag = true;
												}
											}
											if (!flag)
											{
												if (folder.GetFolderRec(null, GetFolderRecFlags.NoProperties).IsGhosted)
												{
													folderStateSnapshot.State |= FolderState.IsGhosted;
												}
												folderStateSnapshot.State |= FolderState.CopyMessagesComplete;
												this.FoldersProcessed++;
											}
										}
									}
								});
								mbxCtx.SaveICSSyncState(false);
								if (numFoldersEnumeratedInCurrentBatch >= base.TestIntegration.FolderBatchSize)
								{
									break;
								}
							}
						}
						base.TestIntegration.Barrier("PostponeWriteMessages", new Action(base.RefreshRequestIfNeeded));
						if (base.TestIntegration.InjectMissingItems > 0)
						{
							Random random = new Random();
							for (int i = 0; i < base.TestIntegration.InjectMissingItems; i++)
							{
								if (messagesToCopy.Count > 0)
								{
									MrsTracer.Service.Warning("MISSING MESSAGE INJECTION: removing message from the list", new object[0]);
									messagesToCopy.RemoveAt(random.Next(messagesToCopy.Count));
								}
							}
						}
						if (messagesToCopy.Count > 0)
						{
							this.RefreshMailboxSizeStatistics();
							MrsTracer.Service.Debug("Sorting {0} messages for mailbox '{1}'", new object[]
							{
								messagesToCopy.Count,
								mbxCtx.SourceTracingID
							});
							MessageRecSorter messageRecSorter = new MessageRecSorter();
							Queue<List<MessageRec>> arg = messageRecSorter.Sort(messagesToCopy, MessageRecSortBy.AscendingTimeStamp);
							if (base.SyncStage == SyncStage.LoadingMessages)
							{
								base.Report.Append(MrsStrings.ReportInitialSeedingStarted(base.TotalMessages, new ByteQuantifiedSize(base.TotalMessageByteSize).ToString()));
								MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveSeedingStarted, new object[]
								{
									base.RequestJobIdentity,
									this.MailboxGuid.ToString(),
									base.TotalMessages,
									new ByteQuantifiedSize(base.TotalMessageByteSize).ToString()
								});
							}
							base.SyncStage = SyncStage.CopyingMessages;
							base.TimeTracker.CurrentState = RequestState.CopyingMessages;
							SaveStateFlags saveStateFlags = SaveStateFlags.Lazy;
							if (this.IsOnlineMove)
							{
								saveStateFlags |= SaveStateFlags.RelinquishLongRunningJob;
							}
							base.SaveState(saveStateFlags, null);
							base.ScheduleWorkItem<MailboxCopierBase, EntryIdMap<FolderStateSnapshot>, Dictionary<FolderStateSnapshot, FolderRecWrapper>, Queue<List<MessageRec>>>(new Action<MailboxCopierBase, EntryIdMap<FolderStateSnapshot>, Dictionary<FolderStateSnapshot, FolderRecWrapper>, Queue<List<MessageRec>>>(this.WriteFolderMessages), mbxCtx, foldersToCopy, destinationFolderRecs, arg, WorkloadType.Unknown);
							return;
						}
						mbxCtx.SaveICSSyncState(true);
						mbxCtx.CopyMessagesCompleted = true;
					}
				}
			}
			this.RefreshMailboxSizeStatistics();
			base.ReportSessionStatistics(MrsStrings.ReportMessagesCopied);
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				mailboxCopierBase.GetDestinationFolderMap(GetFolderMapFlags.None).ResetFolderHierarchyEnumerator();
				mailboxCopierBase.CopyLocalDirectoryEntryId();
				mailboxCopierBase.CopyChangedFoldersData();
			}
			base.ReportSessionStatistics(MrsStrings.ReportInitialSeedingCompleted(base.MessagesWritten, new ByteQuantifiedSize(base.MessageSizeWritten).ToString()));
			base.CheckBadItemCount(true);
			base.SyncStage = SyncStage.IncrementalSync;
			base.OverallProgress = this.CopyEndPercentage;
			base.TimeTracker.SetTimestamp(RequestJobTimestamp.InitialSeedingCompleted, new DateTime?(DateTime.UtcNow));
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveSeedingCompleted, new object[]
			{
				base.RequestJobIdentity,
				this.MailboxGuid.ToString(),
				base.MessagesWritten.ToString(),
				new ByteQuantifiedSize(base.MessageSizeWritten).ToString()
			});
			MailboxCopierBase rootMailboxContext = this.GetRootMailboxContext();
			if (rootMailboxContext != null && rootMailboxContext.ShouldReportEntry(ReportEntryKind.AggregatedSoftDeletedMessages))
			{
				int num = 0;
				ulong num2 = 0UL;
				foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
				{
					num += mailboxCopierBase2.MailboxSizeTracker.DeletedMessageCount;
					num2 += mailboxCopierBase2.MailboxSizeTracker.TotalDeletedMessageSize;
				}
				if (num > 0)
				{
					base.Report.Append(MrsStrings.ReportSoftDeletedItemsNotMigrated(num, new ByteQuantifiedSize(num2).ToString()));
				}
			}
			else
			{
				foreach (MailboxCopierBase mailboxCopierBase3 in this.GetAllCopiers())
				{
					if (mailboxCopierBase3.MailboxSizeTracker.DeletedMessageCount > 0)
					{
						base.Report.Append(MrsStrings.ReportSoftDeletedItemsWillNotBeMigrated(mailboxCopierBase3.SourceTracingID, mailboxCopierBase3.MailboxSizeTracker.DeletedMessageCount, new ByteQuantifiedSize(mailboxCopierBase3.MailboxSizeTracker.TotalDeletedMessageSize).ToString()));
					}
				}
			}
			foreach (MailboxCopierBase mailboxCopierBase4 in this.GetAllCopiers())
			{
				mailboxCopierBase4.GetDestinationFolderMap(GetFolderMapFlags.None).ResetFolderHierarchyEnumerator();
			}
			SaveStateFlags saveStateFlags2 = SaveStateFlags.Regular;
			if (this.IsOnlineMove)
			{
				saveStateFlags2 |= SaveStateFlags.RelinquishLongRunningJob;
			}
			base.SaveState(saveStateFlags2, null);
			RequestJobLog.Write(base.CachedRequestJob, RequestState.InitialSeedingComplete);
			if (this.IsOnlineMove)
			{
				base.ScheduleWorkItem<int, ulong>(new Action<int, ulong>(this.IncrementalSync), 0, 0UL, WorkloadType.Unknown);
				return;
			}
			base.ScheduleWorkItem(new Action(this.FinalSync), WorkloadType.Unknown);
		}

		private void WriteFolderMessages(MailboxCopierBase mailboxToWriteMessages, EntryIdMap<FolderStateSnapshot> foldersToCopy, Dictionary<FolderStateSnapshot, FolderRecWrapper> destinationFolderRecs, Queue<List<MessageRec>> writeQueue)
		{
			base.RefreshRequestIfNeeded();
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			List<MessageRec> list = writeQueue.Dequeue();
			ulong num = 0UL;
			foreach (MessageRec messageRec in list)
			{
				num += (ulong)((long)messageRec.MessageSize);
			}
			MrsTracer.Service.Debug("Copying {0} messages from {1}, {2}", new object[]
			{
				list.Count,
				mailboxToWriteMessages.SourceTracingID,
				new ByteQuantifiedSize(num).ToString()
			});
			mailboxToWriteMessages.CopyMessages(list);
			foreach (MessageRec messageRec2 in list)
			{
				FolderStateSnapshot folderStateSnapshot = foldersToCopy[messageRec2.FolderId];
				folderStateSnapshot.UpdateMessageCopyWatermark(messageRec2);
				folderStateSnapshot.MessagesWritten++;
				folderStateSnapshot.MessageByteSizeWritten += (ulong)((long)messageRec2.MessageSize);
			}
			foreach (FolderStateSnapshot folderStateSnaphot in foldersToCopy.Values)
			{
				mailboxToWriteMessages.MailboxSizeTracker.TrackFolder(folderStateSnaphot);
			}
			this.RefreshMailboxSizeStatistics();
			if (writeQueue.Count == 0)
			{
				FolderMap sourceFolderMap = mailboxToWriteMessages.GetSourceFolderMap(GetFolderMapFlags.None);
				foreach (FolderStateSnapshot folderStateSnapshot2 in foldersToCopy.Values)
				{
					FolderRecWrapper folderRecWrapper = sourceFolderMap[folderStateSnapshot2.FolderId];
					using (ISourceFolder folder = mailboxToWriteMessages.SourceMailbox.GetFolder(folderRecWrapper.EntryId))
					{
						if (folder == null)
						{
							MrsTracer.Service.Debug("Source folder \"{0}\" got deleted. Skipping it.", new object[]
							{
								folderRecWrapper.FullFolderName
							});
							return;
						}
						if (folder.GetFolderRec(null, GetFolderRecFlags.NoProperties).IsGhosted)
						{
							folderStateSnapshot2.State |= FolderState.IsGhosted;
						}
						folderStateSnapshot2.State |= FolderState.CopyMessagesComplete;
						base.FoldersProcessed++;
					}
				}
				mailboxToWriteMessages.SaveICSSyncState(false);
				base.ScheduleWorkItem(new Action(this.EnumerateFolderMessages), WorkloadType.Unknown);
				return;
			}
			base.TimeTracker.CurrentState = RequestState.CopyingMessages;
			mailboxToWriteMessages.SaveICSSyncState(false);
			SaveStateFlags saveStateFlags = SaveStateFlags.Lazy;
			if (this.IsOnlineMove)
			{
				saveStateFlags |= SaveStateFlags.RelinquishLongRunningJob;
			}
			base.SaveState(saveStateFlags, null);
			base.ScheduleWorkItem<MailboxCopierBase, EntryIdMap<FolderStateSnapshot>, Dictionary<FolderStateSnapshot, FolderRecWrapper>, Queue<List<MessageRec>>>(new Action<MailboxCopierBase, EntryIdMap<FolderStateSnapshot>, Dictionary<FolderStateSnapshot, FolderRecWrapper>, Queue<List<MessageRec>>>(this.WriteFolderMessages), mailboxToWriteMessages, foldersToCopy, destinationFolderRecs, writeQueue, WorkloadType.Unknown);
		}

		private void IncrementalSync(int iterationsCount, ulong totalChurn)
		{
			base.TestIntegration.Barrier("PostponeSync", new Action(base.RefreshRequestIfNeeded));
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			base.SyncStage = SyncStage.IncrementalSync;
			base.TimeTracker.CurrentState = RequestState.IncrementalSync;
			MrsTracer.Service.Debug("WorkItem: incremental synchronization.", new object[0]);
			int num;
			this.ApplyIncrementalChanges(out num);
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveIncrementalSyncCompleted, new object[]
			{
				base.RequestJobIdentity,
				this.MailboxGuid.ToString(),
				num
			});
			iterationsCount++;
			totalChurn += (ulong)((long)num);
			if (num > 100 && iterationsCount <= 10 && totalChurn <= (ulong)((long)base.TotalMessages * 100L / 100L))
			{
				MrsTracer.Service.Debug("Too many updates during incremental sync, will repeat the sync.", new object[0]);
				base.ScheduleWorkItem<int, ulong>(new Action<int, ulong>(this.IncrementalSync), iterationsCount, totalChurn, WorkloadType.Unknown);
				return;
			}
			base.CheckBadItemCount(true);
			DateTime? timestamp = base.CachedRequestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
			DateTime? timestamp2 = base.CachedRequestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter);
			TimeSpan incrementalSyncInterval = base.CachedRequestJob.IncrementalSyncInterval;
			DateTime utcNow = DateTime.UtcNow;
			if (base.CachedRequestJob.SuspendWhenReadyToComplete || base.CachedRequestJob.PreventCompletion)
			{
				MrsTracer.Service.Debug("Suspending move job after IncrementalSync.", new object[0]);
				base.TimeTracker.CurrentState = RequestState.AutoSuspended;
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.Suspended, new DateTime?(utcNow));
				base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob moveRequest)
				{
					moveRequest.Status = RequestStatus.AutoSuspended;
					moveRequest.Suspend = true;
					LocalizedString message = base.CachedRequestJob.SuspendWhenReadyToComplete ? MrsStrings.MoveHasBeenAutoSuspended(this.MailboxGuid) : MrsStrings.MoveIsPreventedFromFinalization;
					moveRequest.Message = MrsStrings.MoveRequestMessageInformational(message);
					moveRequest.SuspendWhenReadyToComplete = false;
					base.ReportSessionStatistics(MrsStrings.ReportAutoSuspendingJob);
				});
				return;
			}
			if (base.CachedRequestJob.JobType >= MRSJobType.RequestJobE15_AutoResume && base.CachedRequestJob.RequestType != MRSRequestType.PublicFolderMigration && base.CachedRequestJob.RequestType != MRSRequestType.PublicFolderMailboxMigration)
			{
				DateTime? nextScheduleTime = BaseJob.GetNextScheduledTime(timestamp, timestamp2, incrementalSyncInterval);
				if (nextScheduleTime != null)
				{
					MrsTracer.Service.Debug("Relinquishing in Synced state.", new object[0]);
					base.TimeTracker.CurrentState = RequestState.AutoSuspended;
					base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob moveRequest)
					{
						moveRequest.Status = RequestStatus.Synced;
						this.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(nextScheduleTime.Value));
						LocalizedString message = MrsStrings.MoveHasBeenSynced(this.MailboxGuid, nextScheduleTime.Value);
						moveRequest.Message = MrsStrings.MoveRequestMessageInformational(message);
						this.ReportSessionStatistics(MrsStrings.ReportSyncedJob(nextScheduleTime.Value.ToLocalTime()));
					});
					return;
				}
			}
			DateTime? timestamp3 = base.TimeTracker.GetTimestamp(RequestJobTimestamp.FailedDataGuarantee);
			if (timestamp3 != null)
			{
				MrsTracer.Service.Debug("The move has previously failed to get DataGuarantee during finalization. Will check DG again before entering final sync.", new object[0]);
				LocalizedString failureReason;
				if (!base.CheckDataGuarantee(timestamp3.Value, out failureReason))
				{
					MrsTracer.Service.Warning("DataGaurantee is still not satisfied for {0}", new object[]
					{
						timestamp3.Value
					});
					Exception ex = new MailboxDataReplicationFailedPermanentException(failureReason);
					TimeSpan t = DateTime.UtcNow - timestamp3.Value;
					if (t >= base.GetConfig<TimeSpan>("DataGuaranteeMaxWait"))
					{
						base.TimeTracker.SetTimestamp(RequestJobTimestamp.FailedDataGuarantee, null);
						throw ex;
					}
					DateTime pickupTime = DateTime.UtcNow + base.GetConfig<TimeSpan>("DataGuaranteeRetryInterval");
					throw new RelinquishJobDGTimeoutTransientException(pickupTime, ex);
				}
				else
				{
					MrsTracer.Service.Debug("DataGuarantee now is satisfied for {0}", new object[]
					{
						timestamp3.Value
					});
					base.TimeTracker.SetTimestamp(RequestJobTimestamp.FailedDataGuarantee, null);
				}
			}
			MrsTracer.Service.Debug("Scheduling Final Sync.", new object[0]);
			base.TimeTracker.CurrentState = RequestState.Completion;
			base.SaveState(SaveStateFlags.Regular, null);
			base.ScheduleWorkItem(new Action(this.FinalSync), WorkloadType.Unknown);
		}

		protected virtual void FinalSync()
		{
			base.TestIntegration.Barrier("PostponeFinalSync", new Action(base.RefreshRequestIfNeeded));
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			this.MigrateSecurityDescriptors();
			base.OverallProgress = 95;
			base.SyncStage = SyncStage.FinalIncrementalSync;
			base.TimeTracker.CurrentState = RequestState.Finalization;
			base.TimeTracker.SetTimestamp(RequestJobTimestamp.FinalSync, new DateTime?(DateTime.UtcNow));
			base.Report.Append(MrsStrings.ReportFinalSyncStarted);
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveFinalizationStarted, new object[]
			{
				base.RequestJobIdentity,
				this.MailboxGuid.ToString()
			});
			TimeSpan preFinalSyncDataProcessingDuration = TimeSpan.Zero;
			TimeSpan archivePreFinalSyncDataProcessingDuration = TimeSpan.Zero;
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator.Current;
					if (this.IsOnlineMove && (base.TestIntegration.ForcePreFinalSyncDataProcessing || (!this.skipPreFinalSyncDataProcessing && mbxCtx.SourceMailboxWrapper.MailboxVersion != null && mbxCtx.DestMailboxWrapper.MailboxVersion != null && mbxCtx.DestMailboxWrapper.MailboxVersion.Value > mbxCtx.SourceMailboxWrapper.MailboxVersion.Value)))
					{
						MrsTracer.Service.Debug("Calling PreFinalSyncDataProcessing(0)", new object[]
						{
							mbxCtx.SourceMailboxWrapper.MailboxVersion
						});
						ExDateTime now = ExDateTime.Now;
						try
						{
							mbxCtx.DestMailbox.PreFinalSyncDataProcessing(mbxCtx.SourceMailboxWrapper.MailboxVersion);
						}
						finally
						{
							TimeSpan preFinalSyncDuration = ExDateTime.Now.Subtract(now);
							CommonUtils.CatchKnownExceptions(delegate
							{
								if (mbxCtx.IsDestinationConnected && mbxCtx.DestMailbox != null)
								{
									if (mbxCtx.Flags.HasFlag(MailboxCopierFlags.TargetIsArchive))
									{
										archivePreFinalSyncDataProcessingDuration = preFinalSyncDuration;
										return;
									}
									preFinalSyncDataProcessingDuration = preFinalSyncDuration;
								}
							}, null);
							MrsTracer.Service.Debug("PreFinalSyncDataProcessing finished. Duration: {0}", new object[]
							{
								preFinalSyncDuration
							});
						}
					}
					MailboxOptions mailboxOptions = MailboxOptions.Finalize;
					if (mbxCtx.SupportsRuleAPIs)
					{
						mailboxOptions |= MailboxOptions.IgnoreExtendedRuleFAIs;
					}
					mbxCtx.SourceMailbox.ConfigMailboxOptions(mailboxOptions);
					mbxCtx.DestMailbox.ConfigMailboxOptions(mailboxOptions);
					MrsTracer.Service.Debug("Switching source mailbox into MoveSource mode.", new object[0]);
					bool flag;
					this.SetSourceInTransitStatus(mbxCtx, InTransitStatus.MoveSource, out flag);
				}
			}
			if (this.IsOnlineMove)
			{
				int num;
				this.ApplyIncrementalChanges(out num);
			}
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				mailboxCopierBase.CopyChangedFoldersData();
				mailboxCopierBase.ReportSourceMailboxSize();
			}
			base.CheckBadItemCount(true);
			base.SaveState(SaveStateFlags.Regular, null);
			this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				mbxCtx.FinalSyncCopyMailboxData();
			});
			try
			{
				this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
				{
					mbxCtx.CopyHighWatermark(this.preserveMailboxSignature);
				});
			}
			catch (MailboxSignatureChangedTransientException)
			{
				base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob rj)
				{
					rj.RestartFromScratch = true;
				});
				throw;
			}
			base.TestIntegration.Barrier("PostponeResumeAccessToMailbox", new Action(base.RefreshRequestIfNeeded));
			base.ReportSessionStatistics(MrsStrings.ReportSessionStatisticsUpdated, preFinalSyncDataProcessingDuration, archivePreFinalSyncDataProcessingDuration);
			MrsTracer.Service.Debug("Mailbox data moved successfully.", new object[0]);
			base.StartDataGuaranteeWait();
			if (!base.SkipContentVerification)
			{
				base.Report.Append(MrsStrings.ReportVerifyingMailboxContents);
				List<FolderSizeRec> list = new List<FolderSizeRec>();
				this.ScheduleContentVerification(list);
				base.ScheduleWorkItem<List<FolderSizeRec>>(new Action<List<FolderSizeRec>>(this.OutputVerificationResults), list, WorkloadType.Unknown);
			}
			if (base.TestIntegration.GetIntValueAndDecrement("IntroduceFailureAfterCopyingHighWatermarkNTimes", 0, 0, 2147483647) > 0)
			{
				throw new MailboxReplicationTransientException(new LocalizedString("Injecting transient failure after copying high watermark."));
			}
			base.ScheduleWorkItem(new Action(this.WaitForMailboxChangesToReplicate), WorkloadType.Unknown);
		}

		private void ApplyIncrementalChanges(out int numberOfUpdates)
		{
			if (base.CachedRequestJob.RequestType == MRSRequestType.Move && base.CachedRequestJob.TargetContainerGuid != null)
			{
				MailboxCopierBase mailboxCopierBase = this.MbxContexts.SingleOrDefault((MailboxMover c) => c.IsRoot && c.IsPrimary);
				if (mailboxCopierBase != null)
				{
					MailboxInformation mailboxInformation = mailboxCopierBase.SourceMailbox.GetMailboxInformation();
					List<Guid> expectedMailboxes = (from c in this.MbxContexts
					where c.IsPrimary
					select c.SourceMailboxGuid).ToList<Guid>();
					if (mailboxInformation.ContainerMailboxGuids.Any((Guid currentMailbox) => !expectedMailboxes.Remove(currentMailbox)))
					{
						base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob rj)
						{
							rj.RestartFromScratch = true;
						});
						throw new ContainerMailboxesChangedTransientException();
					}
					using (List<Guid>.Enumerator enumerator = expectedMailboxes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Guid expectedMailbox = enumerator.Current;
							MailboxMover mailboxMover = this.MbxContexts.Single((MailboxMover m) => m.SourceMailboxGuid == expectedMailbox);
							if (mailboxMover.DestMailbox.MailboxExists())
							{
								mailboxMover.DestMailbox.DeleteMailbox(2);
								mailboxMover.Disconnect();
							}
							this.MbxContexts.Remove(mailboxMover);
						}
					}
				}
			}
			numberOfUpdates = 0;
			foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
			{
				base.RefreshRequestIfNeeded();
				SyncContext syncContext = mailboxCopierBase2.CreateSyncContext();
				mailboxCopierBase2.SourceMailbox.SetMailboxSyncState(mailboxCopierBase2.ICSSyncState.ProviderState);
				MailboxChangesManifest mailboxChangesManifest = this.EnumerateSourceHierarchyChanges(mailboxCopierBase2, false, syncContext);
				if (mailboxCopierBase2.ShouldReportEntry(ReportEntryKind.HierarchyChanges))
				{
					base.Report.Append(MrsStrings.ReportIncrementalSyncHierarchyChanges(mailboxCopierBase2.SourceTracingID, mailboxChangesManifest.ChangedFolders.Count, mailboxChangesManifest.DeletedFolders.Count));
				}
				MailboxChanges mailboxChanges = new MailboxChanges(mailboxChangesManifest);
				base.EnumerateAndApplyIncrementalChanges(mailboxCopierBase2, syncContext, mailboxChanges.HierarchyChanges);
				numberOfUpdates += syncContext.NumberOfHierarchyUpdates + syncContext.CopyMessagesCount.TotalContentCopied;
			}
		}

		protected virtual void ScheduleContentVerification(List<FolderSizeRec> verificationResults)
		{
			this.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				FolderHierarchy sourceFolderHierarchy = mbxCtx.GetSourceFolderHierarchy();
				sourceFolderHierarchy.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper folderRecWrapper, FolderMap.EnumFolderContext ctx)
				{
					this.ScheduleWorkItem<MailboxMover, FolderRecWrapper, List<FolderSizeRec>>(new Action<MailboxMover, FolderRecWrapper, List<FolderSizeRec>>(this.VerifyFolderContents), mbxCtx, folderRecWrapper, verificationResults, WorkloadType.Unknown);
				});
			});
		}

		protected virtual void VerifyFolderContents(MailboxCopierBase mbxCtx, FolderRecWrapper folderRecWrapper, List<FolderSizeRec> verificationResults)
		{
			base.RefreshRequestIfNeeded();
			FolderSizeRec folderSizeRec = mbxCtx.VerifyFolderContents(folderRecWrapper, ((FolderMapping)folderRecWrapper).WKFType, true);
			verificationResults.Add(folderSizeRec);
			mbxCtx.ReportBadItems(folderSizeRec.MissingItems);
		}

		private void OutputVerificationResults(List<FolderSizeRec> verificationResults)
		{
			int num = 0;
			ulong num2 = 0UL;
			bool flag = false;
			foreach (FolderSizeRec folderSizeRec in verificationResults)
			{
				num += folderSizeRec.Source.Count + folderSizeRec.SourceFAI.Count;
				num2 += folderSizeRec.Source.Size + folderSizeRec.SourceFAI.Size;
				if (folderSizeRec.Missing.Count > 0)
				{
					flag = true;
				}
			}
			base.Report.Append(MrsStrings.ReportMailboxContentsVerificationComplete(verificationResults.Count, num, new ByteQuantifiedSize(num2).ToString()), verificationResults);
			if (flag)
			{
				base.GetSkippedItemCounts().RecordMissingItems(verificationResults);
			}
			base.CheckBadItemCount(true);
		}

		protected void WaitForMailboxChangesToReplicate()
		{
			base.RefreshRequestIfNeeded();
			if (base.IsDataGuaranteeSatisfied(CommonUtils.IsMultiTenantEnabled()))
			{
				base.ScheduleWorkItem(new Action(this.UpdateAD), WorkloadType.Unknown);
				return;
			}
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				mailboxCopierBase.SourceMailboxWrapper.Ping();
				SessionStatistics sessionStatistics = mailboxCopierBase.SourceMailbox.GetSessionStatistics(SessionStatisticsFlags.MapiDiagnosticGetProp);
				if (sessionStatistics != null && !string.IsNullOrEmpty(sessionStatistics.MapiDiagnosticGetProp))
				{
					base.Report.AppendDebug(string.Format("Diagnostic LIDs (Source Ping): '{0}'", sessionStatistics.MapiDiagnosticGetProp));
				}
			}
			base.ScheduleWorkItem(TimeSpan.FromSeconds(5.0), new Action(this.WaitForMailboxChangesToReplicate), WorkloadType.Unknown);
		}

		private void UpdateAD()
		{
			if (base.CachedRequestJob.BlockFinalization)
			{
				throw new FinalizationIsBlockedPermanentException();
			}
			base.TimeTracker.CurrentState = RequestState.Finalization;
			base.TestIntegration.Barrier("BreakpointBeforeUMM", new Action(base.RefreshRequestIfNeeded));
			this.UpdateMovedMailbox();
			base.TestIntegration.Barrier("BreakpointAfterUMM", new Action(base.RefreshRequestIfNeeded));
			base.OverallProgress = 99;
			base.SyncStage = SyncStage.Cleanup;
			base.TimeTracker.CurrentState = RequestState.Cleanup;
			CommonUtils.CatchKnownExceptions(delegate
			{
				base.SaveState(SaveStateFlags.Regular, null);
			}, delegate(Exception saveFailure)
			{
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(saveFailure);
				base.Report.Append(MrsStrings.ReportRequestSaveFailed2(CommonUtils.GetFailureType(saveFailure)), saveFailure, ReportEntryFlags.Cleanup);
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_RequestSaveFailed, new object[]
				{
					base.GetRequestKeyGuid().ToString(),
					base.RequestJobGuid.ToString(),
					base.RequestJobStoringMDB.ToString(),
					localizedString
				});
			});
			base.ScheduleWorkItem<int>(new Action<int>(this.PostMoveCleanup), 0, WorkloadType.Unknown);
		}

		private void PostMoveCleanupTargetMailbox(MailboxCopierBase mbxCtx)
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				if (!mbxCtx.IsDestinationConnected)
				{
					mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
					mbxCtx.DestMailbox.ConfigMailboxOptions(MailboxOptions.Finalize);
				}
				MailboxInformation mailboxInformation = null;
				bool flag = true;
				if (mbxCtx.SyncState.CleanupRetryAttempts > 0 && mbxCtx.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.MailboxTableInfoFlags))
				{
					mailboxInformation = mbxCtx.DestMailbox.GetMailboxInformation();
				}
				if (mailboxInformation != null)
				{
					MailboxMiscFlags mailboxTableFlags = (MailboxMiscFlags)mailboxInformation.MailboxTableFlags;
					if (!mailboxTableFlags.HasFlag(MailboxMiscFlags.CreatedByMove))
					{
						flag = false;
					}
				}
				if (mbxCtx.Flags.HasFlag(MailboxCopierFlags.ContainerOrg))
				{
					ReportEntry[] entries = null;
					ADUser aduser = mbxCtx.DestMailbox.GetADUser();
					try
					{
						mbxCtx.DestMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation.UpdateMailbox, null, this.CachedRequestJob.DestDomainControllerToUpdate, out entries, this.CachedRequestJob.TargetMDBGuid, (aduser.ArchiveDatabase != null) ? aduser.ArchiveDatabase.ObjectGuid : Guid.Empty, (aduser.ArchiveDomain != null) ? aduser.ArchiveDomain.ToString() : null, aduser.ArchiveStatus, UpdateMovedMailboxFlags.SkipMailboxReleaseCheck | UpdateMovedMailboxFlags.SkipProvisioningCheck, this.CachedRequestJob.TargetContainerGuid, this.CachedRequestJob.TargetUnifiedMailboxId);
					}
					finally
					{
						this.AppendReportEntries(entries);
					}
				}
				mbxCtx.DestMailbox.SeedMBICache();
				if (!this.TestIntegration.DoNotUnlockTargetMailbox)
				{
					if (flag)
					{
						this.SetDestinationInTransitStatus(mbxCtx);
						this.TimeTracker.SetTimestamp(RequestJobTimestamp.MailboxLocked, null);
						bool flag2;
						mbxCtx.DestMailbox.SetInTransitStatus(InTransitStatus.NotInTransit, out flag2);
					}
					if (mbxCtx.IsUpgradeToE15OrHigher)
					{
						mbxCtx.DestMailbox.SetProps(MoveBaseJob.SetLocalizedToTrue);
					}
					if (mbxCtx.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.NonMrsLogon))
					{
						mbxCtx.DestMailbox.Disconnect();
						try
						{
							mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.NonMrsLogon);
						}
						catch (LocalizedException innerException)
						{
							throw new UnableToVerifyMailboxConnectivityTransientException(mbxCtx.TargetTracingID, innerException);
						}
						mbxCtx.DestMailbox.Disconnect();
						mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
						mbxCtx.DestMailbox.ConfigMailboxOptions(MailboxOptions.Finalize);
					}
				}
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.TargetMailboxCleanup;
				mbxCtx.ReportTargetMailboxSize();
				this.Report.Append(MrsStrings.ReportDestinationMailboxResetSucceeded(mbxCtx.TargetTracingID));
			}, delegate(Exception failure)
			{
				mbxCtx.DestMailbox.Disconnect();
				bool flag = mbxCtx.SyncState.CleanupRetryAttempts <= this.GetConfig<int>("MaxCleanupRetries");
				if (CommonUtils.ExceptionIs(failure, new WellKnownException[]
				{
					WellKnownException.MRSMailboxIsLocked
				}))
				{
					failure = this.ProcessMailboxLockedFailure(failure);
					if (failure is RelinquishJobTransientException)
					{
						failure.PreserveExceptionStack();
						throw failure;
					}
					if (!CommonUtils.IsTransientException(failure))
					{
						flag = false;
					}
				}
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
				if (!flag)
				{
					this.Warnings.Add(MrsStrings.DestinationMailboxResetFailed(localizedString));
					FailureLog.Write(this.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupResetTargetMailbox, null, null);
					mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.TargetMailboxCleanup;
					MailboxReplicationService.LogEvent(this.CachedRequestJob.TargetIsLocal ? MRSEventLogConstants.Tuple_LocalDestinationMailboxResetFailed : MRSEventLogConstants.Tuple_RemoteDestinationMailboxResetFailed, new object[]
					{
						this.RequestJobIdentity,
						mbxCtx.TargetTracingID,
						this.CachedRequestJob.Flags.ToString(),
						this.CachedRequestJob.TargetMDBName,
						localizedString
					});
				}
				this.Report.Append(MrsStrings.ReportDestinationMailboxResetFailed3(CommonUtils.GetFailureType(failure), mbxCtx.SyncState.CleanupRetryAttempts, this.GetConfig<int>("MaxCleanupRetries") + 1), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
				MrsTracer.Service.Error("Failure trying to reset InTransit status on destination mailbox after the move.", new object[0]);
			});
		}

		protected virtual void PostMoveUpdateSourceMailbox(MailboxCopierBase mbxCtx)
		{
			if (mbxCtx != this.GetRootMailboxContext())
			{
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.UpdateSourceMailbox;
				return;
			}
			if (base.CachedRequestJob.SkipConvertingSourceToMeu)
			{
				base.Report.Append(MrsStrings.ReportSkippingUpdateSourceMailbox);
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.UpdateSourceMailbox;
				return;
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				if (this.TestIntegration.GetIntValueAndDecrement("InjectNFaultsPostMoveUpdateSourceMailbox", 0, 0, 2147483647) > 0)
				{
					throw new ADOperationException(TestIntegration.FaultInjectionExceptionMessage);
				}
				if (!mbxCtx.IsSourceConnected)
				{
					mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.DoNotOpenMapiSession);
					mbxCtx.SourceMailbox.ConfigMailboxOptions(MailboxOptions.Finalize);
					MailboxServerInformation mailboxServerInformation;
					bool flag;
					mbxCtx.SourceMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation, out flag);
				}
				this.UpdateSourceMailbox();
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.UpdateSourceMailbox;
			}, delegate(Exception failure)
			{
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
				MrsTracer.Service.Error("Failure {0} morphing source mailbox to MEU after the move.", new object[]
				{
					localizedString
				});
				mbxCtx.SourceMailbox.Disconnect();
				if (mbxCtx.SyncState.CleanupRetryAttempts > this.GetConfig<int>("MaxCleanupRetries"))
				{
					mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.UpdateSourceMailbox;
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveUnableToUpdateSourceMailbox, new object[]
					{
						this.RequestJobIdentity,
						this.MailboxGuid.ToString(),
						localizedString
					});
					this.Report.Append(MrsStrings.ReportUnableToUpdateSourceMailbox2(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Source);
					this.Warnings.Add(MrsStrings.UnableToUpdateSourceMailbox(localizedString));
					if (CommonUtils.IsMultiTenantEnabled() && this.CachedRequestJob.Direction == RequestDirection.Push && this.CachedRequestJob.RequestStyle == RequestStyle.CrossOrg)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(string.Format("MailboxIdentity: {0}", this.RequestJobIdentity));
						stringBuilder.AppendLine(string.Format("MailboxGuid: {0}", mbxCtx.SourceTracingID));
						stringBuilder.AppendLine(string.Format("RequestFlags: {0}", this.CachedRequestJob.Flags));
						stringBuilder.AppendLine(string.Format("Database: {0}", this.CachedRequestJob.SourceMDBName));
						stringBuilder.AppendLine(string.Format("Exception: {0}", localizedString));
						new EventNotificationItem(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, "SourceMailboxNotMorphedToMeu", ResultSeverityLevel.Error)
						{
							Message = stringBuilder.ToString()
						}.Publish(false);
					}
					FailureLog.Write(this.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupUnableToUpdateSourceMailbox, null, null);
					return;
				}
				this.UpdateRequestAfterFailure(this.CachedRequestJob, failure);
				this.Report.Append(MrsStrings.ReportSourceMailboxUpdateFailed(mbxCtx.SourceTracingID, CommonUtils.GetFailureType(failure), mbxCtx.SyncState.CleanupRetryAttempts, this.GetConfig<int>("MaxCleanupRetries") + 1), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Source);
			});
		}

		protected virtual void PostMoveCleanupSourceMailbox(MailboxCopierBase mailboxMover)
		{
			MailboxMover mbxCtx = (MailboxMover)mailboxMover;
			if (base.CachedRequestJob.SkipConvertingSourceToMeu)
			{
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SourceMailboxCleanup;
				base.Report.Append(MrsStrings.ReportSourceMailboxCleanupSkipped(mbxCtx.SourceTracingID));
				return;
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				if (!mbxCtx.IsSourceConnected)
				{
					mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.DoNotOpenMapiSession);
					mbxCtx.SourceMailbox.ConfigMailboxOptions(MailboxOptions.Finalize);
					MailboxServerInformation mailboxServerInformation;
					bool flag;
					mbxCtx.SourceMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation, out flag);
				}
				this.CleanupSourceMailbox(mbxCtx, true);
				DeleteMailboxFlags deleteMailboxFlags = DeleteMailboxFlags.MailboxMoved;
				if (mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.SourceMailboxWrapper.LastConnectedServerInfo.DeleteMailboxVersion != 0U)
				{
					deleteMailboxFlags |= DeleteMailboxFlags.SoftDelete;
					if (mbxCtx.IsRoot && mbxCtx.SourceMailboxWrapper.HasMapiSession)
					{
						this.FlushReport(null);
						CommonUtils.CatchKnownExceptions(delegate
						{
							using (MapiStore systemMailbox = MapiUtils.GetSystemMailbox(this.RequestJobStoringMDB.ObjectGuid))
							{
								ReportData reportData = new ReportData(this.GetRequestKeyGuid(), this.CachedRequestJob.ReportVersion);
								reportData.Load(systemMailbox);
								MoveHistoryEntryInternal mhei = new MoveHistoryEntryInternal(this.CachedRequestJob, reportData);
								mbxCtx.SourceMailbox.AddMoveHistoryEntry(mhei, this.GetConfig<int>("MaxMoveHistoryLength"));
							}
						}, delegate(Exception failure)
						{
							MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SourceMailboxMoveHistoryEntryFailed, new object[]
							{
								this.RequestJobIdentity,
								mbxCtx.TargetTracingID,
								this.CachedRequestJob.SourceMDBName,
								CommonUtils.FullExceptionMessage(failure)
							});
						});
					}
				}
				try
				{
					mbxCtx.SourceMailbox.DeleteMailbox((int)deleteMailboxFlags);
				}
				catch (LocalizedException ex)
				{
					if (mbxCtx.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.R5DC) || !CommonUtils.ExceptionIs(ex, new WellKnownException[]
					{
						WellKnownException.MapiNotFound
					}))
					{
						throw;
					}
					MrsTracer.Service.Debug("Ignoring MapiExceptionNotFound thrown by DeleteMailbox on a downlevel MRSProxy", new object[0]);
				}
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SourceMailboxCleanup;
				this.Report.Append(MrsStrings.ReportSourceMailboxCleanupSucceeded(mbxCtx.SourceTracingID));
			}, delegate(Exception failure)
			{
				mbxCtx.SourceMailbox.Disconnect();
				bool flag = mbxCtx.SyncState.CleanupRetryAttempts <= this.GetConfig<int>("MaxCleanupRetries");
				if (failure is UnsupportedRemoteServerVersionWithOperationPermanentException)
				{
					flag = false;
				}
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
				if (!flag)
				{
					this.Warnings.Add(MrsStrings.SourceMailboxCleanupFailed(localizedString));
					FailureLog.Write(this.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupDeleteSourceMailbox, null, null);
					mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SourceMailboxCleanup;
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SourceMailboxCleanupFailed, new object[]
					{
						this.RequestJobIdentity,
						mbxCtx.SourceTracingID,
						this.CachedRequestJob.SourceMDBName,
						localizedString
					});
				}
				this.Report.Append(MrsStrings.ReportSourceMailboxCleanupFailed3(mbxCtx.SourceTracingID, CommonUtils.GetFailureType(failure), mbxCtx.SyncState.CleanupRetryAttempts, this.GetConfig<int>("MaxCleanupRetries") + 1), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Source);
				MrsTracer.Service.Error("Failure trying to cleanup source mailbox after the move.", new object[0]);
			});
		}

		protected virtual void PostMoveMarkRehomeOnRelatedRequests(MailboxCopierBase mbxCtx)
		{
			Guid mdbGuid = mbxCtx.SourceMdbGuid;
			Guid mbxGuid = mbxCtx.SourceMailboxGuid;
			CommonUtils.CatchKnownExceptions(delegate
			{
				if (!this.TestIntegration.DoNotAutomaticallyMarkRehome && this.CachedRequestJob.RequestStyle != RequestStyle.CrossOrg)
				{
					using (MapiStore systemMailbox = MapiUtils.GetSystemMailbox(mdbGuid))
					{
						using (MapiFolder requestJobsFolder = RequestJobXML.GetRequestJobsFolder(systemMailbox))
						{
							using (MapiTable contentsTable = requestJobsFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
							{
								if (contentsTable.GetRowCount() != 0)
								{
									RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(systemMailbox);
									contentsTable.SetColumns(requestJobNamedPropertySet.PropTags);
									this.PostMoveMarkRehomeOnRelatedRequestsHelper(true, contentsTable, systemMailbox, requestJobNamedPropertySet, mdbGuid, mbxGuid);
									this.PostMoveMarkRehomeOnRelatedRequestsHelper(false, contentsTable, systemMailbox, requestJobNamedPropertySet, mdbGuid, mbxGuid);
								}
							}
						}
					}
				}
				mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SetRelatedRequestsRehome;
			}, delegate(Exception failure)
			{
				if (ConfigBase<MRSConfigSchema>.GetConfig<bool>("CrossResourceForestEnabled") && failure is SystemMailboxNotFoundPermanentException)
				{
					mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SetRelatedRequestsRehome;
				}
				MrsTracer.Service.Error("Failure trying to mark related requests for rehoming after the move.", new object[0]);
			});
		}

		protected virtual void OnMoveCompleted(MailboxCopierBase mbxCtx)
		{
		}

		private void PostMoveMarkRehomeOnRelatedRequestsHelper(bool isSource, MapiTable contentsTable, MapiStore systemMbx, RequestJobNamedPropertySet nps, Guid mdbGuid, Guid mbxGuid)
		{
			int num = isSource ? 18 : 19;
			Restriction restriction = Restriction.And(new Restriction[]
			{
				Restriction.EQ(nps.PropTags[num], mbxGuid),
				Restriction.EQ(nps.PropTags[20], false)
			});
			SortOrder sortOrder = new SortOrder(nps.PropTags[num], SortFlags.Ascend);
			sortOrder.Add(nps.PropTags[20], SortFlags.Descend);
			sortOrder.Add(nps.PropTags[2], SortFlags.Ascend);
			contentsTable.SortTable(sortOrder, SortTableFlags.None);
			if (contentsTable.FindRow(restriction, BookMark.Beginning, FindRowFlag.None))
			{
				List<RequestJobObjectId> list = new List<RequestJobObjectId>();
				for (;;)
				{
					PropValue[][] array = contentsTable.QueryRows(1, QueryRowsFlags.None);
					if (array == null || array.Length == 0)
					{
						break;
					}
					PropValue[] array2 = array[0];
					Guid value = MapiUtils.GetValue<Guid>(array2[num], Guid.Empty);
					bool value2 = MapiUtils.GetValue<bool>(array2[20], false);
					if (value != mbxGuid || value2)
					{
						goto IL_155;
					}
					Guid value3 = MapiUtils.GetValue<Guid>(array2[26], Guid.Empty);
					byte[] value4 = MapiUtils.GetValue<byte[]>(array2[27], null);
					MrsTracer.Service.Debug("Found relevant {0} job with mailbox guid {1} to rehome on database {2}. RequestGuid: {3}.", new object[]
					{
						isSource ? "Source" : "Target",
						mbxGuid,
						mdbGuid,
						value3
					});
					list.Add(new RequestJobObjectId(value3, mdbGuid, value4));
				}
				MrsTracer.Service.Debug("Store returned no more rows when searching for {0} jobs with mailbox guid {1} to rehome on database {2}.", new object[]
				{
					isSource ? "Source" : "Target",
					mbxGuid,
					mdbGuid
				});
				goto IL_23E;
				IL_155:
				MrsTracer.Service.Debug("No more relevant {0} jobs with mailbox guid {1} to rehome on database {2}.", new object[]
				{
					isSource ? "Source" : "Target",
					mbxGuid,
					mdbGuid
				});
				IL_23E:
				lock (this.syncRoot)
				{
					using (List<RequestJobObjectId>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RequestJobObjectId jobId = enumerator.Current;
							CommonUtils.CatchKnownExceptions(delegate
							{
								MapiUtils.RetryOnObjectChanged(delegate
								{
									using (RequestJobProvider requestJobProvider = new RequestJobProvider(mdbGuid, systemMbx))
									{
										using (TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)requestJobProvider.Read<TransactionalRequestJob>(jobId))
										{
											if (transactionalRequestJob != null)
											{
												transactionalRequestJob.RehomeRequest = true;
												requestJobProvider.Save(transactionalRequestJob);
											}
										}
									}
								});
							}, delegate(Exception failure)
							{
								MrsTracer.Service.Error("Unexpected failure occurred trying to mark rehome on job '{0}', skipping it. {1}", new object[]
								{
									jobId.RequestGuid,
									CommonUtils.FullExceptionMessage(failure)
								});
							});
						}
					}
				}
			}
		}

		private void PostMoveCleanup(int iPostMoveCleanupRetries)
		{
			MrsTracer.Service.Debug("WorkItem: PostMoveCleanup", new object[0]);
			base.TestIntegration.Barrier("PostponeCleanup", new Action(base.RefreshRequestIfNeeded));
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (mailboxCopierBase.SyncState != null && mailboxCopierBase.SyncState.CleanupRetryAttempts > iPostMoveCleanupRetries)
				{
					iPostMoveCleanupRetries = mailboxCopierBase.SyncState.CleanupRetryAttempts;
				}
			}
			if (iPostMoveCleanupRetries > base.GetConfig<int>("MaxCleanupRetries"))
			{
				if (base.Warnings.Count == 0)
				{
					Exception failure = new TooManyCleanupRetriesPermanentException();
					foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
					{
						if ((mailboxCopierBase2.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.TargetMailboxCleanup) != PostMoveCleanupStatusFlags.TargetMailboxCleanup)
						{
							base.Warnings.Add(MrsStrings.DestinationMailboxResetFailed(MrsStrings.ErrorTooManyCleanupRetries));
							FailureLog.Write(base.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupResetTargetMailbox, null, null);
						}
						if ((mailboxCopierBase2.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.UpdateSourceMailbox) != PostMoveCleanupStatusFlags.UpdateSourceMailbox)
						{
							base.Warnings.Add(MrsStrings.SourceMailboxUpdateFailed(MrsStrings.ErrorTooManyCleanupRetries));
							FailureLog.Write(base.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupUnableToUpdateSourceMailbox, null, null);
						}
						if ((mailboxCopierBase2.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.SourceMailboxCleanup) != PostMoveCleanupStatusFlags.SourceMailboxCleanup)
						{
							base.Warnings.Add(MrsStrings.SourceMailboxCleanupFailed(MrsStrings.ErrorTooManyCleanupRetries));
							FailureLog.Write(base.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupDeleteSourceMailbox, null, null);
						}
						if ((mailboxCopierBase2.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.SetRelatedRequestsRehome) != PostMoveCleanupStatusFlags.SetRelatedRequestsRehome)
						{
							base.Warnings.Add(MrsStrings.SettingRehomeOnRelatedRequestsFailed(MrsStrings.ErrorTooManyCleanupRetries));
							FailureLog.Write(base.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupUnableToRehomeRelatedRequests, null, null);
						}
					}
				}
				base.ScheduleWorkItem<int>(new Action<int>(this.CompleteMove), 0, WorkloadType.Unknown);
				return;
			}
			if (iPostMoveCleanupRetries == 0)
			{
				base.Report.Append(MrsStrings.ReportPostMoveCleanupStarted);
			}
			iPostMoveCleanupRetries++;
			MrsTracer.Service.Debug("Performing post-move cleanup for mailbox move, attempt {0}.", new object[]
			{
				iPostMoveCleanupRetries
			});
			foreach (MailboxCopierBase mailboxCopierBase3 in this.GetAllCopiers())
			{
				mailboxCopierBase3.SyncState.CleanupRetryAttempts = iPostMoveCleanupRetries;
				if ((mailboxCopierBase3.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.UpdateSourceMailbox) != PostMoveCleanupStatusFlags.UpdateSourceMailbox)
				{
					this.PostMoveUpdateSourceMailbox(mailboxCopierBase3);
				}
				if ((mailboxCopierBase3.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.SourceMailboxCleanup) != PostMoveCleanupStatusFlags.SourceMailboxCleanup)
				{
					this.PostMoveCleanupSourceMailbox(mailboxCopierBase3);
				}
				if ((mailboxCopierBase3.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.TargetMailboxCleanup) != PostMoveCleanupStatusFlags.TargetMailboxCleanup)
				{
					this.PostMoveCleanupTargetMailbox(mailboxCopierBase3);
				}
				if ((mailboxCopierBase3.SyncState.CompletedCleanupTasks & PostMoveCleanupStatusFlags.SetRelatedRequestsRehome) == PostMoveCleanupStatusFlags.None)
				{
					this.PostMoveMarkRehomeOnRelatedRequests(mailboxCopierBase3);
				}
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				base.SaveState(SaveStateFlags.DontSaveRequestJob, null);
			}, null);
			if (!this.CleanupIsComplete())
			{
				base.Report.Append(MrsStrings.ReportRetryingPostMoveCleanup(30, iPostMoveCleanupRetries, base.GetConfig<int>("MaxCleanupRetries") + 1));
				base.FlushReport(null);
				base.ScheduleWorkItem<int>(TimeSpan.FromSeconds(30.0), new Action<int>(this.PostMoveCleanup), iPostMoveCleanupRetries, WorkloadType.Unknown);
				return;
			}
			base.StartDataGuaranteeWait();
			base.ScheduleWorkItem(new Action(this.FinalDataGuaranteeWait), WorkloadType.Unknown);
		}

		private void FinalDataGuaranteeWait()
		{
			bool dataGuaranteed = false;
			Exception dataGuaranteeFailure = null;
			bool flag = true;
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (!mailboxCopierBase.IsDestinationConnected)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				base.ResetDataGuaranteeWait();
				dataGuaranteeFailure = new TargetMailboxConnectionWasLostPermanentException();
			}
			else
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					dataGuaranteed = this.IsDataGuaranteeSatisfied(false);
				}, delegate(Exception failure)
				{
					dataGuaranteeFailure = failure;
				});
				if (dataGuaranteeFailure == null && !dataGuaranteed)
				{
					base.ScheduleWorkItem(TimeSpan.FromSeconds(5.0), new Action(this.FinalDataGuaranteeWait), WorkloadType.Unknown);
					return;
				}
			}
			if (dataGuaranteeFailure != null)
			{
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(dataGuaranteeFailure);
				base.Warnings.Add(MrsStrings.DestinationMailboxResetNotGuaranteed(localizedString));
				FailureLog.Write(base.RequestJobGuid, dataGuaranteeFailure, false, RequestState.Cleanup, SyncStage.CleanupUnableToGuaranteeUnlock, null, null);
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_DestinationMailboxResetNotGuaranteed, new object[]
				{
					base.RequestJobIdentity,
					this.GetRootMailboxContext().TargetTracingID,
					base.CachedRequestJob.TargetMDBName,
					localizedString
				});
				base.Report.Append(MrsStrings.ReportDestinationMailboxResetNotGuaranteed(CommonUtils.GetFailureType(dataGuaranteeFailure)), dataGuaranteeFailure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
				MrsTracer.Service.Error("Unable to guarantee InTransit status reset on destination mailbox after the move.", new object[0]);
			}
			base.ScheduleWorkItem<int>(new Action<int>(this.CompleteMove), 0, WorkloadType.Unknown);
		}

		private void CompleteMove(int iCompleteMoveRetries)
		{
			MrsTracer.Service.Debug("WorkItem: CompleteMove", new object[0]);
			base.TimeTracker.CurrentState = RequestState.Cleanup;
			if (iCompleteMoveRetries > base.GetConfig<int>("MaxCleanupRetries"))
			{
				base.Disconnect();
				return;
			}
			iCompleteMoveRetries++;
			MoveHistoryEntryInternal mhei = null;
			if (!this.doneUpdatingMoveComplete)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.TestIntegration.Barrier("PostponeComplete", new Action(this.RefreshRequestIfNeeded));
					this.TimeTracker.CurrentState = RequestState.Completed;
					this.Report.Append(MrsStrings.ReportRequestCompleted);
					this.CompleteRequest(true, out mhei);
					this.doneUpdatingMoveComplete = true;
				}, delegate(Exception failure)
				{
					LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
					if (iCompleteMoveRetries > this.GetConfig<int>("MaxCleanupRetries"))
					{
						MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_FailedToUpdateCompletedRequest, new object[]
						{
							this.RequestJobIdentity,
							this.GetRequestKeyGuid().ToString(),
							this.RequestJobStoringMDB.ToString(),
							localizedString
						});
					}
					MrsTracer.Service.Debug("Failed to save completed move request, attempt {0}/{1}", new object[]
					{
						iCompleteMoveRetries,
						this.GetConfig<int>("MaxCleanupRetries") + 1
					});
				});
			}
			if (!this.doneUpdatingMoveComplete && iCompleteMoveRetries <= base.GetConfig<int>("MaxCleanupRetries"))
			{
				base.ScheduleWorkItem<int>(TimeSpan.FromSeconds(30.0), new Action<int>(this.CompleteMove), iCompleteMoveRetries, WorkloadType.Unknown);
				return;
			}
			if (this.doneUpdatingMoveComplete && mhei != null)
			{
				MailboxCopierBase rootCtx = this.GetRootMailboxContext();
				CommonUtils.CatchKnownExceptions(delegate
				{
					if (!rootCtx.IsDestinationConnected)
					{
						rootCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
					}
					rootCtx.DestMailbox.AddMoveHistoryEntry(mhei, this.GetConfig<int>("MaxMoveHistoryLength"));
				}, delegate(Exception failure)
				{
					rootCtx.DestMailbox.Disconnect();
					LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_DestinationMailboxMoveHistoryEntryFailed, new object[]
					{
						this.RequestJobIdentity,
						this.GetRootMailboxContext().TargetTracingID,
						this.CachedRequestJob.TargetMDBName,
						localizedString
					});
				});
			}
			if (this.doneUpdatingMoveComplete)
			{
				using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxCopierBase mbxCtx = enumerator.Current;
						CommonUtils.CatchKnownExceptions(delegate
						{
							if (!mbxCtx.IsDestinationConnected)
							{
								mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
							}
							mbxCtx.ClearSyncState(SyncStateClearReason.MoveComplete);
							if (mbxCtx.IsRoot)
							{
								this.CleanupDestinationMailbox(mbxCtx, true);
							}
							this.OnMoveCompleted(mbxCtx);
							this.Report.Append(MrsStrings.ReportDestinationMailboxClearSyncStateSucceeded(mbxCtx.TargetTracingID));
						}, delegate(Exception failure)
						{
							LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
							MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_DestinationMailboxSyncStateDeletionFailed, new object[]
							{
								this.RequestJobIdentity,
								mbxCtx.TargetTracingID,
								this.CachedRequestJob.TargetMDBName,
								localizedString
							});
							this.Report.Append(MrsStrings.ReportDestinationMailboxClearSyncStateFailed2(CommonUtils.GetFailureType(failure), 1, 1), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
							this.FlushReport(null);
						});
					}
				}
			}
			base.Disconnect();
		}

		private void ScheduleNextWorkItem()
		{
			SyncStage syncStage = base.SyncStage;
			switch (syncStage)
			{
			case SyncStage.MailboxCreated:
				base.OverallProgress = 5;
				base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
				return;
			case SyncStage.CreatingFolderHierarchy:
				base.OverallProgress = 10;
				base.ScheduleWorkItem(new Action(this.CreateFolderHierarchy), WorkloadType.Unknown);
				return;
			case SyncStage.CreatingInitialSyncCheckpoint:
				base.OverallProgress = 15;
				base.ScheduleWorkItem(new Action(this.CatchupFolders), WorkloadType.Unknown);
				return;
			case SyncStage.LoadingMessages:
			case SyncStage.CopyingMessages:
				base.SyncStage = SyncStage.LoadingMessages;
				base.OverallProgress = 20;
				base.ScheduleWorkItem(new Action(this.InitializeCopyMessageStatistics), WorkloadType.Unknown);
				return;
			case (SyncStage)7:
			case (SyncStage)8:
			case (SyncStage)9:
				goto IL_10F;
			case SyncStage.IncrementalSync:
			case SyncStage.FinalIncrementalSync:
				base.SyncStage = SyncStage.IncrementalSync;
				base.OverallProgress = this.CopyEndPercentage;
				base.ScheduleWorkItem<int, ulong>(new Action<int, ulong>(this.IncrementalSync), 0, 0UL, WorkloadType.Unknown);
				return;
			case SyncStage.Cleanup:
				break;
			default:
				if (syncStage != SyncStage.SyncFinished)
				{
					goto IL_10F;
				}
				break;
			}
			base.SyncStage = SyncStage.Cleanup;
			base.OverallProgress = 99;
			base.ScheduleWorkItem<int>(new Action<int>(this.PostMoveCleanup), 0, WorkloadType.Unknown);
			return;
			IL_10F:
			throw new UnexpectedErrorPermanentException(-2147024809);
		}

		private bool CleanupIsComplete()
		{
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				if (mailboxCopierBase.SyncState.CompletedCleanupTasks != PostMoveCleanupStatusFlags.All)
				{
					return false;
				}
			}
			return true;
		}

		private void DeleteReplica()
		{
			base.CheckDisposed();
			MrsTracer.Service.Debug("Deleting mailbox replica.", new object[0]);
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator.Current;
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (!mbxCtx.IsDestinationConnected)
						{
							mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
						}
						mbxCtx.ClearSyncState(SyncStateClearReason.DeleteReplica);
						this.CleanupDestinationMailbox(mbxCtx, false);
						if (mbxCtx.DestMailbox.MailboxExists())
						{
							MailboxInformation mailboxInformation = mbxCtx.DestMailbox.GetMailboxInformation();
							LocalizedString localizedString;
							if (this.ValidateHomeMDBValue(mbxCtx, mailboxInformation) && this.ValidateTargetMailbox(mailboxInformation, out localizedString) && ((MailboxMiscFlags)mailboxInformation.MailboxTableFlags).HasFlag(MailboxMiscFlags.CreatedByMove))
							{
								MrsTracer.Service.Debug("Deleting destination mailbox {0}", new object[]
								{
									mbxCtx.TargetTracingID
								});
								mbxCtx.DestMailbox.DeleteMailbox(7);
							}
						}
					}, delegate(Exception failure)
					{
						LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
						this.Report.Append(MrsStrings.ReportDestinationMailboxCleanupFailed2(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
						MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_DestinationMailboxCleanupFailed, new object[]
						{
							this.RequestJobIdentity,
							mbxCtx.TargetTracingID,
							this.CachedRequestJob.TargetMDBName,
							localizedString
						});
					});
				}
			}
			MoveHistoryEntryInternal mhei;
			base.RemoveRequest(true, out mhei);
			using (List<MailboxMover>.Enumerator enumerator2 = this.MbxContexts.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MoveBaseJob.<>c__DisplayClass128 CS$<>8__locals3 = new MoveBaseJob.<>c__DisplayClass128();
					CS$<>8__locals3.mbxCtx = enumerator2.Current;
					bool resetInTransitSuccess = false;
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (!CS$<>8__locals3.mbxCtx.IsSourceConnected)
						{
							CS$<>8__locals3.mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.None);
						}
						bool flag;
						this.SetSourceInTransitStatus(CS$<>8__locals3.mbxCtx, InTransitStatus.NotInTransit, out flag);
						resetInTransitSuccess = true;
						this.CleanupSourceMailbox(CS$<>8__locals3.mbxCtx, false);
						if (CS$<>8__locals3.mbxCtx.IsRoot && mhei != null)
						{
							CS$<>8__locals3.mbxCtx.SourceMailbox.AddMoveHistoryEntry(mhei, this.GetConfig<int>("MaxMoveHistoryLength"));
						}
					}, delegate(Exception failure)
					{
						if (!resetInTransitSuccess)
						{
							LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
							MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SourceMailboxResetFailed, new object[]
							{
								this.RequestJobIdentity,
								CS$<>8__locals3.mbxCtx.SourceTracingID,
								this.CachedRequestJob.SourceMDBName,
								localizedString
							});
						}
					});
				}
			}
			base.Disconnect();
		}

		private bool ValidateHomeMDBValue(MailboxCopierBase mbxCtx, MailboxInformation mailboxInfo)
		{
			MailboxMover mailboxMover = mbxCtx as MailboxMover;
			return mailboxMover == null || this.IsPushingToTitanium || mailboxInfo.MailboxHomeMdbGuid != mailboxMover.DestMdbGuid;
		}

		protected virtual bool ValidateTargetMailbox(MailboxInformation mailboxInfo, out LocalizedString moveFinishedReason)
		{
			moveFinishedReason = MrsStrings.ReportTargetUserIsNotMailEnabledUser;
			foreach (MailboxMover mailboxMover in this.MbxContexts)
			{
				if (mailboxMover.IsPrimary)
				{
					return base.CachedRequestJob.RequestStyle != RequestStyle.CrossOrg || mailboxInfo.ServerVersion < Server.E2007MinVersion || mailboxInfo.RecipientType == 3;
				}
			}
			return true;
		}

		private Exception ProcessMailboxLockedFailure(Exception failure)
		{
			MrsTracer.Service.Debug("Processing MailboxIsLocked failure", new object[0]);
			TimeSpan config = base.GetConfig<TimeSpan>("MailboxLockoutTimeout");
			if (config == TimeSpan.Zero)
			{
				return failure;
			}
			DateTime? timestamp = base.TimeTracker.GetTimestamp(RequestJobTimestamp.MailboxLocked);
			DateTime utcNow = DateTime.UtcNow;
			if (timestamp == null)
			{
				timestamp = new DateTime?(utcNow);
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.MailboxLocked, timestamp);
			}
			TimeSpan timeSpan = utcNow - timestamp.Value;
			MrsTracer.Service.Debug("Current mailbox lockout duration: {0}", new object[]
			{
				timeSpan
			});
			if (timeSpan <= config)
			{
				DateTime pickupTime = DateTime.UtcNow + base.GetConfig<TimeSpan>("MailboxLockoutRetryInterval");
				return new RelinquishJobMailboxLockoutTransientException(pickupTime, failure);
			}
			if (CommonUtils.GetExceptionSide(failure) == ExceptionSide.Source)
			{
				return new SourceMailboxAlreadyBeingMovedPermanentException(failure.InnerException);
			}
			return new DestMailboxAlreadyBeingMovedPermanentException(failure.InnerException);
		}

		protected virtual void SetSourceInTransitStatus(MailboxCopierBase mbxCtx, InTransitStatus status, out bool sourceSupportsOnlineMove)
		{
			mbxCtx.SourceMailbox.SetInTransitStatus(status, out sourceSupportsOnlineMove);
		}

		protected virtual void SetDestinationInTransitStatus(MailboxCopierBase mbxCtx)
		{
			InTransitStatus inTransitStatus;
			if (!this.destSupportsOnlineMove)
			{
				inTransitStatus = InTransitStatus.MoveDestination;
			}
			else
			{
				inTransitStatus = InTransitStatus.SyncDestination;
			}
			if (base.CachedRequestJob.AllowLargeItems && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo != null && mbxCtx.DestMailboxWrapper.LastConnectedServerInfo.InTransitStatusVersion > 0U)
			{
				inTransitStatus |= InTransitStatus.AllowLargeItems;
			}
			MrsTracer.Service.Debug("Switching destination mailbox to {0} mode.", new object[]
			{
				inTransitStatus
			});
			mbxCtx.DestMailbox.SetInTransitStatus(inTransitStatus, out this.destSupportsOnlineMove);
			if (this.IsOnlineMove && !this.destSupportsOnlineMove)
			{
				MrsTracer.Service.Debug("Destination does not support online move, switching to offline move mode.", new object[0]);
				this.IsOnlineMove = false;
			}
		}

		private void RefreshMailboxSizeStatistics()
		{
			base.TotalMessages = 0;
			base.TotalMessageByteSize = 0UL;
			base.MessagesWritten = 0;
			base.MessageSizeWritten = 0UL;
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				base.TotalMessages += mailboxCopierBase.MailboxSizeTracker.MessageCount;
				base.TotalMessageByteSize += mailboxCopierBase.MailboxSizeTracker.TotalMessageSize;
				base.MessagesWritten += mailboxCopierBase.MailboxSizeTracker.AlreadyCopiedCount;
				base.MessageSizeWritten += mailboxCopierBase.MailboxSizeTracker.AlreadyCopiedSize;
			}
		}

		private static readonly TimeSpan TombstoneCleanupRetryDelay = TimeSpan.FromSeconds(10.0);

		private static readonly PropValueData[] SetLocalizedToTrue = new PropValueData[]
		{
			new PropValueData(PropTag.Localized, true)
		};

		private bool destSupportsOnlineMove = true;

		private bool doneUpdatingMoveComplete;

		private bool skipPreFinalSyncDataProcessing;

		private bool preserveMailboxSignature;

		private bool restartingAfterSignatureChange;

		private int isIntegData;
	}
}
