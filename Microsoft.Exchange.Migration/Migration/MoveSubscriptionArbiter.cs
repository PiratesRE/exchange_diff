using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MoveSubscriptionArbiter : SubscriptionArbiterBase
	{
		private MoveSubscriptionArbiter()
		{
		}

		public static MoveSubscriptionArbiter Instance
		{
			get
			{
				return MoveSubscriptionArbiter.soleInstance;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			base.SetResolveStatus(MigrationUserStatus.Syncing, SnapshotStatus.Finalized, MigrationUserStatus.Completed);
			base.SetResolveStatus(MigrationUserStatus.Syncing, SnapshotStatus.CompletedWithWarning, MigrationUserStatus.CompletedWithWarnings);
			base.SetResolveStatus(MigrationUserStatus.Synced, SnapshotStatus.Finalized, MigrationUserStatus.Completed);
			base.SetResolveStatus(MigrationUserStatus.Synced, SnapshotStatus.CompletedWithWarning, MigrationUserStatus.CompletedWithWarnings);
			base.SetResolveStatus(MigrationUserStatus.IncrementalSyncing, SnapshotStatus.Finalized, MigrationUserStatus.Completed);
			base.SetResolveStatus(MigrationUserStatus.IncrementalSyncing, SnapshotStatus.CompletedWithWarning, MigrationUserStatus.CompletedWithWarnings);
			base.SetDefaultResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.InProgress);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.AutoSuspended, MigrationUserStatus.Synced);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Synced, MigrationUserStatus.Synced);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Suspended, MigrationUserStatus.CompletionFailed);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Failed, MigrationUserStatus.CompletionFailed);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Corrupted, MigrationUserStatus.CompletionFailed);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Removed, MigrationUserStatus.Corrupted);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.Finalized, MigrationUserStatus.Completed);
			base.SetResolveStatus(MigrationUserStatus.Completing, SnapshotStatus.CompletedWithWarning, MigrationUserStatus.CompletedWithWarnings);
			base.SetResolveOperation(MigrationUserStatus.Completing, new SubscriptionArbiterBase.ResolveOperation(MoveSubscriptionArbiter.ResolveMoveItemCompleting));
			base.SetResolveOperation(MigrationUserStatus.CompletionFailed, new SubscriptionArbiterBase.ResolveOperation(MoveSubscriptionArbiter.ResolveJobItemCompletionFailed));
			base.SetResolveOperation(MigrationUserStatus.CompletedWithWarnings, new SubscriptionArbiterBase.ResolveOperation(MoveSubscriptionArbiter.ResolveJobItemCompletedWithWarnings));
		}

		private static SubscriptionArbiterBase.StatusAndError ResolveMoveItemCompleting(MigrationJob job, MigrationJobItem jobItem, SubscriptionSnapshot subscription)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (job.FinalizeTime == null)
			{
				throw new MigrationDataCorruptionException("the job should have been finalized" + job);
			}
			TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationInitialSyncTimeOutForFailingSubscriptions");
			if (job.SupportsSyncTimeouts && subscription.IsTimedOut(config))
			{
				return new SubscriptionArbiterBase.StatusAndError(MigrationUserStatus.CompletionFailed, new SyncTimeoutException(ItemStateTransitionHelper.LocalizeTimeSpan(config)));
			}
			return null;
		}

		private static SubscriptionArbiterBase.StatusAndError ResolveJobItemCompletionFailed(MigrationJob job, MigrationJobItem jobItem, SubscriptionSnapshot subscription)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (subscription.Status == SnapshotStatus.Suspended)
			{
				return new SubscriptionArbiterBase.StatusAndError(MigrationUserStatus.CompletionFailed, new ExternallySuspendedException());
			}
			return new SubscriptionArbiterBase.StatusAndError(MigrationUserStatus.CompletionFailed, SubscriptionArbiterBase.ResolveLocalizedError(jobItem, subscription, MigrationUserStatus.CompletionFailed));
		}

		private static SubscriptionArbiterBase.StatusAndError ResolveJobItemCompletedWithWarnings(MigrationJob job, MigrationJobItem jobItem, SubscriptionSnapshot subscription)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			return new SubscriptionArbiterBase.StatusAndError(MigrationUserStatus.CompletedWithWarnings, new MigrationPermanentException(subscription.ErrorMessage ?? Strings.UnknownMigrationError));
		}

		private static readonly MoveSubscriptionArbiter soleInstance = new MoveSubscriptionArbiter();
	}
}
