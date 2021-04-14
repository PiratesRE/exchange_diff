using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyIMAPSubscriptionHandler : LegacySubscriptionHandlerBase, ILegacySubscriptionHandler, IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
		internal LegacyIMAPSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob migrationJob) : base(dataProvider, migrationJob)
		{
			this.resourceAccessor = new SyncResourceAccessor(dataProvider);
		}

		public MigrationType SupportedMigrationType
		{
			get
			{
				return MigrationType.IMAP;
			}
		}

		public bool SupportsDupeDetection
		{
			get
			{
				return false;
			}
		}

		public override bool SupportsActiveIncrementalSync
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsAdvancedValidation
		{
			get
			{
				return false;
			}
		}

		public bool CreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.AssertOrThrow(jobItem.LocalMailbox != null, "job item should have a local mailbox before creating subscriptions", new object[0]);
			MigrationUtil.AssertOrThrow(jobItem.SubscriptionSettings != null, "job item should have subscription settings before creating subscriptions", new object[0]);
			MigrationUtil.AssertOrThrow(base.Job.SubscriptionSettings != null, "job should have subscription settings before creating subscriptions", new object[0]);
			ImapEndpoint imapEndpoint = (ImapEndpoint)base.Job.SourceEndpoint;
			IMAPJobItemSubscriptionSettings imapjobItemSubscriptionSettings = (IMAPJobItemSubscriptionSettings)jobItem.SubscriptionSettings;
			IMAPJobSubscriptionSettings imapjobSubscriptionSettings = (IMAPJobSubscriptionSettings)base.Job.SubscriptionSettings;
			CreateIMAPSyncSubscriptionArgs subscriptionCreationArgs = new CreateIMAPSyncSubscriptionArgs(base.DataProvider.OrganizationId.OrganizationalUnit, ((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN, jobItem.Identifier, jobItem.Identifier, SmtpAddress.Parse(jobItem.Identifier), imapjobItemSubscriptionSettings.Username, imapjobItemSubscriptionSettings.EncryptedPassword, imapEndpoint.RemoteServer, imapEndpoint.Port, imapjobSubscriptionSettings.ExcludedFolders, imapEndpoint.Security, imapEndpoint.Authentication, imapjobItemSubscriptionSettings.UserRootFolder, false);
			this.resourceAccessor.CreateSubscription(jobItem, subscriptionCreationArgs);
			return true;
		}

		public bool TestCreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			throw new NotImplementedException("TestCreateUnderlyingSubscriptions is not available for IMAP");
		}

		public MigrationProcessorResult SyncToUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			return ItemStateTransitionHelper.ProcessDelayedSubscription(base.DataProvider, base.Job, jobItem);
		}

		public void DeleteUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "disabling IMAP subscription for item {0} because handler DOES NOT support dupe detection", new object[]
			{
				jobItem
			});
			base.StopUnderlyingSubscriptions(jobItem);
		}

		public void ResumeUnderlyingSubscriptions(MigrationUserStatus startedStatus, MigrationJobItem jobItem)
		{
			this.resourceAccessor.FinalizeSubscription(jobItem, MigrationUserStatus.Completing, MigrationUserStatus.Completed, MigrationUserStatus.IncrementalFailed);
		}

		public override void DisableSubscriptions(MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			try
			{
				this.resourceAccessor.UpdateSubscription(jobItem, UpdateSyncSubscriptionAction.Disable);
			}
			catch (MigrationPermanentException innerException)
			{
				throw new UnableToDisableSubscriptionTransientException(innerException)
				{
					InternalError = "Unable to disable subscription for " + jobItem
				};
			}
		}

		public override void SyncSubscriptionSettings(MigrationJobItem jobItem)
		{
		}

		public override IEnumerable<MigrationJobItem> GetJobItemsForSubscriptionCheck(ExDateTime? cutoffTime, MigrationUserStatus status, int maxItemsToCheck)
		{
			return MigrationJobItem.GetBySubscriptionLastChecked(base.DataProvider, base.Job, cutoffTime, status, maxItemsToCheck);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LegacyIMAPSubscriptionHandler>(this);
		}

		private readonly SyncResourceAccessor resourceAccessor;
	}
}
