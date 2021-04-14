using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class IncrementalMergeJob : MergeJob
	{
		public override void ValidateAndPopulateRequestJob(List<ReportEntry> entries)
		{
			base.ValidateAndPopulateRequestJob(entries);
		}

		protected override int CopyStartPercentage
		{
			get
			{
				return 10;
			}
		}

		protected override int CopyEndPercentage
		{
			get
			{
				return 95;
			}
		}

		protected override void StartMerge()
		{
			MrsTracer.Service.Debug("WorkItem: StartMerge", new object[0]);
			if (base.MailboxMerger.SyncState != null && base.MailboxMerger.ICSSyncState != null)
			{
				MrsTracer.Service.Debug("Recovering an interrupted merge.", new object[0]);
				base.CheckServersHealth();
				base.MailboxMerger.SourceMailbox.SetMailboxSyncState(base.MailboxMerger.ICSSyncState.ProviderState);
				if (base.SyncStage == SyncStage.IncrementalSync || base.SyncStage == SyncStage.FinalIncrementalSync)
				{
					base.OverallProgress = this.CopyEndPercentage;
					base.ScheduleWorkItem(new Action(this.InitializeIncrementalSync), WorkloadType.Unknown);
					base.Report.Append(MrsStrings.ReportRequestContinued(base.SyncStage.ToString()));
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_RequestContinued, new object[]
					{
						base.RequestJobIdentity,
						base.RequestJobGuid.ToString(),
						base.SyncStage.ToString()
					});
					return;
				}
			}
			base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
			base.StartMerge();
			base.MailboxMerger.ICSSyncState = new MailboxMapiSyncState();
		}

		protected override void BeforeDataCopy()
		{
			base.MailboxMerger.SourceHierarchy.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext context)
			{
				FolderMapping folderMapping = (FolderMapping)folderRec;
				if (!folderMapping.IsIncluded)
				{
					return;
				}
				using (ISourceFolder folder = base.MailboxMerger.SourceMailbox.GetFolder(folderMapping.EntryId))
				{
					if (folder != null)
					{
						FolderRec folderRec2 = folder.GetFolderRec(base.MailboxMerger.GetAdditionalFolderPtags(), GetFolderRecFlags.None);
						base.MailboxMerger.CatchupFolder(folderRec2, folder);
					}
				}
			});
			base.MailboxMerger.ICSSyncState.ProviderState = base.MailboxMerger.SourceMailbox.GetMailboxSyncState();
			base.MailboxMerger.SaveICSSyncState(true);
		}

		protected override void FinalizeMerge()
		{
			base.CheckBadItemCount(true);
			base.ReportProgress(true);
			base.SyncStage = SyncStage.IncrementalSync;
			base.OverallProgress = this.CopyEndPercentage;
			base.TimeTracker.SetTimestamp(RequestJobTimestamp.InitialSeedingCompleted, new DateTime?(DateTime.UtcNow));
			RequestJobLog.Write(base.CachedRequestJob, RequestState.InitialSeedingComplete);
			base.ScheduleWorkItem<bool>(new Action<bool>(this.IncrementalSync), true, WorkloadType.Unknown);
		}

		protected virtual void AutoSuspendJob()
		{
			if (base.CachedRequestJob.SuspendWhenReadyToComplete || base.CachedRequestJob.PreventCompletion)
			{
				base.Report.Append(MrsStrings.ReportAutoSuspendingJob);
				base.TimeTracker.CurrentState = RequestState.AutoSuspended;
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.Suspended, new DateTime?(DateTime.UtcNow));
				base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob mergeRequest)
				{
					mergeRequest.Status = RequestStatus.AutoSuspended;
					mergeRequest.Suspend = true;
					mergeRequest.Message = MrsStrings.MoveRequestMessageInformational(MrsStrings.JobHasBeenAutoSuspended);
				});
				return;
			}
			DateTime? timestamp = base.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter);
			DateTime? timestamp2 = base.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
			TimeSpan incrementalSyncInterval = base.CachedRequestJob.IncrementalSyncInterval;
			DateTime utcNow = DateTime.UtcNow;
			DateTime? nextSchedule = BaseJob.GetNextScheduledTime(timestamp2, timestamp, incrementalSyncInterval);
			base.Report.Append(MrsStrings.ReportSyncedJob(nextSchedule.Value.ToLocalTime()));
			base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob mergeRequest)
			{
				mergeRequest.Status = RequestStatus.Synced;
				this.TimeTracker.CurrentState = RequestState.AutoSuspended;
				this.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(nextSchedule.Value));
				mergeRequest.Message = MrsStrings.MoveRequestMessageInformational(MrsStrings.JobHasBeenSynced);
			});
		}

		private void CatchupFolderHierarchy()
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			if (string.IsNullOrEmpty(base.MailboxMerger.ICSSyncState.ProviderState))
			{
				MrsTracer.Service.Debug("WorkItem: catchup folder hierarchy.", new object[0]);
				base.MailboxMerger.SourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags.Catchup, 0);
				return;
			}
			MrsTracer.Service.Debug("Folder hierarchy sync state already exists, will not run hierarchy catchup", new object[0]);
		}

		protected virtual void IncrementalSync(bool firstIncrementalSync)
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			base.TestIntegration.Barrier("PostponeSync", new Action(base.RefreshRequestIfNeeded));
			base.MailboxSnapshotTimestamp = DateTime.UtcNow;
			base.SyncStage = SyncStage.IncrementalSync;
			base.OverallProgress = this.CopyEndPercentage;
			base.TimeTracker.CurrentState = RequestState.IncrementalSync;
			MrsTracer.Service.Debug("WorkItem: incremental synchronization.", new object[0]);
			this.EnumerateAndApplyIncrementalChanges();
			base.CopyMailboxProperties();
			base.CheckBadItemCount(true);
			if (firstIncrementalSync && !base.SkipContentVerification)
			{
				base.ScheduleContentVerification();
			}
			base.ScheduleWorkItem(new Action(this.AutoSuspendJob), WorkloadType.Unknown);
		}

		protected SyncContext EnumerateAndApplyIncrementalChanges()
		{
			SyncContext syncContext = base.MailboxMerger.CreateSyncContext();
			MailboxChanges mailboxChanges = new MailboxChanges(base.MailboxMerger.EnumerateHierarchyChanges(syncContext));
			base.Report.Append(MrsStrings.ReportIncrementalSyncHierarchyChanges(base.MailboxMerger.SourceTracingID, mailboxChanges.HierarchyChanges.ChangedFolders.Count, mailboxChanges.HierarchyChanges.DeletedFolders.Count));
			base.EnumerateAndApplyIncrementalChanges(base.MailboxMerger, syncContext, mailboxChanges.HierarchyChanges);
			return syncContext;
		}

		private void InitializeIncrementalSync()
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			base.ComputeFolderMapping(false);
			base.ScheduleWorkItem<bool>(new Action<bool>(this.IncrementalSync), false, WorkloadType.Unknown);
		}
	}
}
