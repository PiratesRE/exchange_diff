using System;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SyncJob : PagedMergeJob
	{
		protected override int CopyEndPercentage
		{
			get
			{
				return 95;
			}
		}

		private int GetActionsPageSize
		{
			get
			{
				return base.GetConfig<int>("GetActionsPageSize");
			}
		}

		public override void Initialize(TransactionalRequestJob syncRequest)
		{
			base.Initialize(syncRequest);
			base.RequestJobIdentity = syncRequest.Identity.ToString();
			base.IncrementalSyncInterval = syncRequest.IncrementalSyncInterval;
			Guid targetExchangeGuid = syncRequest.TargetExchangeGuid;
			MailboxCopierFlags mailboxCopierFlags = MailboxCopierFlags.None;
			LocalizedString sourceTracingID = LocalizedString.Empty;
			switch (syncRequest.SyncProtocol)
			{
			case SyncProtocol.Imap:
				mailboxCopierFlags |= MailboxCopierFlags.Imap;
				sourceTracingID = MrsStrings.ImapTracingId(syncRequest.EmailAddress.ToString());
				break;
			case SyncProtocol.Eas:
				mailboxCopierFlags |= MailboxCopierFlags.Eas;
				sourceTracingID = MrsStrings.EasTracingId(syncRequest.EmailAddress.ToString());
				break;
			case SyncProtocol.Pop:
				mailboxCopierFlags |= MailboxCopierFlags.Pop;
				sourceTracingID = MrsStrings.PopTracingId(syncRequest.EmailAddress.ToString());
				break;
			}
			string orgID = (syncRequest.OrganizationId != null && syncRequest.OrganizationId.OrganizationalUnit != null) ? (syncRequest.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			base.MailboxMerger = new MailboxMerger(Guid.Empty, targetExchangeGuid, syncRequest, this, mailboxCopierFlags, sourceTracingID, MrsStrings.PrimaryMailboxTracingId(orgID, targetExchangeGuid));
		}

		protected override void PerformFolderRecoverySync(MailboxChanges changes, MailboxContentsCrawler crawler)
		{
			foreach (FolderChangesManifest folderChangesManifest in from folderChanges in changes.FolderChanges.Values
			where folderChanges.FolderRecoverySync
			select folderChanges)
			{
				FolderMapping folder = base.MailboxMerger.SourceHierarchy[folderChangesManifest.FolderId] as FolderMapping;
				crawler.ResetFolder(folder);
			}
		}

		protected override void FinalizeMerge()
		{
			base.CheckBadItemCount(true);
			base.SyncStage = SyncStage.IncrementalSync;
			base.OverallProgress = this.CopyEndPercentage;
			base.ScheduleWorkItem(new Action(this.AutoSuspendJob), WorkloadType.Unknown);
		}

		protected override void ScheduleIncrementalSync()
		{
			if (base.MailboxMerger.CanReplay && this.GetActionsPageSize > 0)
			{
				if (this.IsInteractive)
				{
					base.ScheduleWorkItem(new PeriodicWorkItem(ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("ActivatedJobIncrementalSyncInterval"), new Action(this.ReplayActions)));
				}
				else
				{
					base.ScheduleWorkItem(new Action(this.ReplayActions), WorkloadType.Unknown);
				}
			}
			base.ScheduleIncrementalSync();
		}

		private void AutoSuspendJob()
		{
			DateTime? timestamp = base.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter);
			DateTime? timestamp2 = base.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
			TimeSpan incrementalSyncInterval = base.CachedRequestJob.IncrementalSyncInterval;
			DateTime? nextSchedule = BaseJob.GetNextScheduledTime(timestamp2, timestamp, incrementalSyncInterval);
			if (nextSchedule != null)
			{
				base.Report.Append(MrsStrings.ReportSyncedJob(nextSchedule.Value.ToLocalTime()));
				base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob mergeRequest)
				{
					mergeRequest.Status = RequestStatus.Synced;
					this.TimeTracker.CurrentState = RequestState.AutoSuspended;
					this.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(nextSchedule.Value));
					mergeRequest.Message = MrsStrings.MoveRequestMessageInformational(MrsStrings.JobHasBeenSynced);
				});
				return;
			}
			base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob mergeRequest)
			{
				mergeRequest.Status = RequestStatus.Completed;
				base.TimeTracker.CurrentState = RequestState.Completed;
				mergeRequest.Message = MrsStrings.MoveRequestMessageInformational(MrsStrings.ReportRequestCompleted);
			});
		}

		private void ReplayActions()
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			MrsTracer.Service.Debug("WorkItem: replay actions.", new object[0]);
			MergeSyncContext syncContext = (MergeSyncContext)base.MailboxMerger.CreateSyncContext();
			this.EnumerateAndReplayActions(base.MailboxMerger, syncContext);
		}

		private void EnumerateAndReplayActions(MailboxMerger mbxContext, MergeSyncContext syncContext)
		{
			if (mbxContext.ReplaySyncState == null)
			{
				mbxContext.ReplaySyncState = new ReplaySyncState();
			}
			string text = mbxContext.ReplaySyncState.ProviderState;
			bool flag = true;
			int num = 0;
			while (flag)
			{
				MergeSyncContext mergeSyncContext = (MergeSyncContext)base.MailboxMerger.CreateSyncContext();
				string text2;
				ReplayActionsQueue andTranslateActions = mbxContext.GetAndTranslateActions(text, this.GetActionsPageSize, mergeSyncContext, out text2, out flag);
				base.Report.Append(MrsStrings.ReportReplayActionsEnumerated(mbxContext.TargetTracingID, andTranslateActions.Count, num));
				try
				{
					mbxContext.ReplayActions(andTranslateActions, mergeSyncContext);
					text = text2;
					syncContext.NumberOfActionsReplayed += mergeSyncContext.NumberOfActionsReplayed;
					syncContext.NumberOfActionsIgnored += mergeSyncContext.NumberOfActionsIgnored;
				}
				catch (MailboxReplicationTransientException)
				{
					if (mergeSyncContext.LastActionProcessed != null)
					{
						text = mergeSyncContext.LastActionProcessed.Watermark;
					}
				}
				finally
				{
					base.Report.Append(MrsStrings.ReportReplayActionsSynced(mbxContext.TargetTracingID, mergeSyncContext.NumberOfActionsReplayed, mergeSyncContext.NumberOfActionsIgnored));
					mbxContext.ReplaySyncState.ProviderState = text;
					mbxContext.SaveReplaySyncState();
				}
				num++;
			}
			base.Report.Append(MrsStrings.ReportReplayActionsCompleted(mbxContext.TargetTracingID, syncContext.NumberOfActionsReplayed, syncContext.NumberOfActionsIgnored));
		}
	}
}
