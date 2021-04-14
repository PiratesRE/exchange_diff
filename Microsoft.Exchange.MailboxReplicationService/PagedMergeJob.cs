using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PagedMergeJob : MergeJob
	{
		private TimeSpan CrawlAndCopyFolderTimeout
		{
			get
			{
				return base.GetConfig<TimeSpan>("CrawlAndCopyFolderTimeout");
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
			}
			base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
			base.StartMerge();
		}

		protected override void CopyMessages(IReadOnlyCollection<FolderMapping> foldersToCopy)
		{
			base.RefreshRequestIfNeeded();
			base.SyncStage = SyncStage.CopyingMessages;
			base.TimeTracker.CurrentState = RequestState.CopyingMessages;
			base.CheckServersHealth();
			foreach (FolderMapping folderMapping in foldersToCopy)
			{
				if (folderMapping.FolderType != FolderType.Search)
				{
					base.MailboxMerger.MailboxSizeTracker.TrackFolder(folderMapping.FolderRec);
					base.Report.Append(MrsStrings.ReportMergingFolder(folderMapping.FullFolderName, folderMapping.TargetFolder.FullFolderName));
				}
			}
			this.ScheduleIncrementalSync();
			this.crawler = new MailboxContentsCrawler(base.MailboxMerger, foldersToCopy);
			this.ScheduleCrawlAndCopyFolder();
			base.MailboxMerger.MailboxSizeTracker.IsFinishedEstimating = true;
		}

		protected override void CalculateCopyingProgress()
		{
			if (base.MessagesWritten < base.TotalMessages)
			{
				base.OverallProgress = this.CopyStartPercentage + (this.CopyEndPercentage - this.CopyStartPercentage) * base.MessagesWritten / base.TotalMessages;
				return;
			}
			base.OverallProgress = this.CopyEndPercentage;
		}

		protected override void UnconfigureProviders()
		{
			if (this.crawler != null)
			{
				this.crawler.Dispose();
				this.crawler = null;
			}
			base.UnconfigureProviders();
		}

		protected virtual void PerformFolderRecoverySync(MailboxChanges changes, MailboxContentsCrawler crawler)
		{
		}

		protected virtual void ScheduleIncrementalSync()
		{
			if (this.IsInteractive)
			{
				base.ScheduleWorkItem(new PeriodicWorkItem(ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("ActivatedJobIncrementalSyncInterval"), new Action(this.IncrementalSync)));
				return;
			}
			base.ScheduleWorkItem(new Action(this.IncrementalSync), WorkloadType.Unknown);
		}

		private void ScheduleCrawlAndCopyFolder()
		{
			base.ScheduleWorkItem(new Action(this.CrawlAndCopyFolder), this.IsInteractive ? WorkloadType.MailboxReplicationServiceInternalMaintenance : WorkloadType.MailboxReplicationService);
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

		private void CopyFolderPropertiesAndContents(FolderMapping folder, FolderContentsCrawler sourceFolderCrawler, bool shouldCopyProperties, TimeSpan maxOperationDuration)
		{
			PagedMergeJob.<>c__DisplayClass1 CS$<>8__locals1 = new PagedMergeJob.<>c__DisplayClass1();
			CS$<>8__locals1.folder = folder;
			CS$<>8__locals1.sourceFolderCrawler = sourceFolderCrawler;
			CS$<>8__locals1.shouldCopyProperties = shouldCopyProperties;
			CS$<>8__locals1.maxOperationDuration = maxOperationDuration;
			CS$<>8__locals1.<>4__this = this;
			using (IDestinationFolder destFolder = base.MailboxMerger.DestMailbox.GetFolder(CS$<>8__locals1.folder.TargetFolder.EntryId))
			{
				if (destFolder == null)
				{
					base.Report.Append(MrsStrings.ReportTargetFolderDeleted(CS$<>8__locals1.folder.TargetFolder.FullFolderName, TraceUtils.DumpEntryId(CS$<>8__locals1.folder.TargetFolder.EntryId), CS$<>8__locals1.folder.FullFolderName));
					return;
				}
				ExecutionContext.Create(new DataContext[]
				{
					new FolderRecWrapperDataContext(CS$<>8__locals1.folder)
				}).Execute(delegate
				{
					CS$<>8__locals1.<>4__this.MailboxMerger.CopyFolderPropertiesAndContents(CS$<>8__locals1.folder, CS$<>8__locals1.sourceFolderCrawler, destFolder, CS$<>8__locals1.shouldCopyProperties, CS$<>8__locals1.maxOperationDuration);
					destFolder.Flush();
				});
			}
			base.SaveState(SaveStateFlags.Lazy, null);
		}

		private void CrawlAndCopyFolder()
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			MrsTracer.Service.Debug("WorkItem: crawling and copying messages.", new object[0]);
			FolderContentsCrawler sourceFolderCrawler;
			bool shouldCopyProperties;
			FolderMapping nextFolderToCopy = this.crawler.GetNextFolderToCopy(out sourceFolderCrawler, out shouldCopyProperties);
			if (nextFolderToCopy != null)
			{
				this.CopyFolderPropertiesAndContents(nextFolderToCopy, sourceFolderCrawler, shouldCopyProperties, this.CrawlAndCopyFolderTimeout);
				this.ScheduleCrawlAndCopyFolder();
				return;
			}
			base.ScheduleWorkItem(new Action(base.CopyMailboxProperties), WorkloadType.Unknown);
			base.ScheduleWorkItem(new Action(this.FinalizeMerge), WorkloadType.Unknown);
		}

		private void IncrementalSync()
		{
			base.RefreshRequestIfNeeded();
			base.CheckServersHealth();
			base.TestIntegration.Barrier("PostponeSync", new Action(base.RefreshRequestIfNeeded));
			MrsTracer.Service.Debug("WorkItem: incremental synchronization.", new object[0]);
			SyncContext syncContext = base.MailboxMerger.CreateSyncContext();
			MailboxChanges mailboxChanges = new MailboxChanges(base.MailboxMerger.EnumerateHierarchyChanges(syncContext));
			base.Report.Append(MrsStrings.ReportIncrementalSyncHierarchyChanges(base.MailboxMerger.SourceTracingID, mailboxChanges.HierarchyChanges.ChangedFolders.Count, mailboxChanges.HierarchyChanges.DeletedFolders.Count));
			base.EnumerateAndApplyIncrementalChanges(base.MailboxMerger, syncContext, mailboxChanges.HierarchyChanges);
			if (mailboxChanges.HasFolderRecoverySync)
			{
				this.PerformFolderRecoverySync(mailboxChanges, this.crawler);
			}
		}

		private MailboxContentsCrawler crawler;
	}
}
