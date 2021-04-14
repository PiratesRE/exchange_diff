using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MrsPublicFolderAccessor : MrsAccessorBase
	{
		public MrsPublicFolderAccessor(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider, batchName)
		{
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			return this.InternalCreateSubscription(jobItem, false);
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			return this.InternalCreateSubscription(jobItem, true);
		}

		public SubscriptionSnapshot TestCreateSubscription(PublicFolderEndpoint endpoint, MailboxData mailboxData, Stream csvStream)
		{
			return this.InternalCreateSubscription(MigrationBatchDirection.Onboarding, endpoint, null, mailboxData, csvStream, true);
		}

		public override SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId)
		{
			SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(subscriptionId);
			return subscriptionSnapshot.Status;
		}

		public override SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, false);
			GetPublicFolderMigrationRequestStatisticsCommand getPublicFolderMigrationRequestStatisticsCommand = new GetPublicFolderMigrationRequestStatisticsCommand(mrsidentity);
			if (base.IncludeReport)
			{
				getPublicFolderMigrationRequestStatisticsCommand.IncludeReport = true;
			}
			MailboxData localMailbox = (MailboxData)mrsidentity.MailboxData;
			return this.RetrieveSubscriptionSnapshot(getPublicFolderMigrationRequestStatisticsCommand, localMailbox) ?? SubscriptionSnapshot.CreateRemoved();
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			MigrationJob migrationJob = jobItem.MigrationJob;
			return this.UpdateSubscription(subscriptionId, migrationJob.JobDirection, (PublicFolderEndpoint)endpoint, (PublicFolderJobSubscriptionSettings)migrationJob.SubscriptionSettings);
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			ResumePublicFolderMigrationRequestCommand command = new ResumePublicFolderMigrationRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true),
				SuspendWhenReadyToComplete = !finalize
			};
			Type left;
			base.Run<RequestBase>(command, out left);
			return left == null;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, true);
			SuspendPublicFolderMigrationRequestCommand command = new SuspendPublicFolderMigrationRequestCommand(mrsidentity);
			Type left;
			base.Run<RequestBase>(command, out left);
			return left == null;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, true);
			RemovePublicFolderMigrationRequestCommand command = new RemovePublicFolderMigrationRequestCommand(mrsidentity);
			Type left;
			base.Run<RequestBase>(command, out left);
			return left == null;
		}

		private static SnapshotStatus GetSubscriptionStatus(RequestStatus requestStatus, bool suspendFlag)
		{
			switch (requestStatus)
			{
			case RequestStatus.None:
			case RequestStatus.CompletionInProgress:
				MigrationLogger.Log(MigrationEventType.Error, "unsupported status for public folder mailbox migration request: {0}", new object[]
				{
					requestStatus
				});
				return SnapshotStatus.Corrupted;
			case RequestStatus.Queued:
			case RequestStatus.InProgress:
				return SnapshotStatus.InProgress;
			case RequestStatus.AutoSuspended:
				if (suspendFlag)
				{
					return SnapshotStatus.AutoSuspended;
				}
				return SnapshotStatus.InProgress;
			case RequestStatus.Synced:
				return SnapshotStatus.Synced;
			case (RequestStatus)6:
			case (RequestStatus)7:
			case (RequestStatus)8:
			case (RequestStatus)9:
				break;
			case RequestStatus.Completed:
				return SnapshotStatus.Finalized;
			case RequestStatus.CompletedWithWarning:
				return SnapshotStatus.CompletedWithWarning;
			default:
				switch (requestStatus)
				{
				case RequestStatus.Suspended:
					if (suspendFlag)
					{
						return SnapshotStatus.Suspended;
					}
					return SnapshotStatus.InProgress;
				case RequestStatus.Failed:
					if (suspendFlag)
					{
						return SnapshotStatus.Failed;
					}
					return SnapshotStatus.InProgress;
				}
				break;
			}
			MigrationLogger.Log(MigrationEventType.Error, "unknown status for public folder mailbox migration request: {0}", new object[]
			{
				requestStatus
			});
			return SnapshotStatus.Corrupted;
		}

		private static void ApplyNewOrSetCommonParameters(NewPublicFolderMigrationRequestCommandBase command, MigrationBatchDirection jobDirection, PublicFolderEndpoint endpoint, PublicFolderJobSubscriptionSettings jobSubscriptionSettings)
		{
			switch (jobDirection)
			{
			case MigrationBatchDirection.Local:
				command.SourceDatabase = DatabaseIdParameter.Parse(jobSubscriptionSettings.SourcePublicFolderDatabase);
				break;
			case MigrationBatchDirection.Onboarding:
				command.AuthenticationMethod = endpoint.AuthenticationMethod;
				command.OutlookAnywhereHostName = endpoint.RpcProxyServer.ToString();
				command.RemoteCredential = endpoint.Credentials;
				command.RemoteMailboxLegacyDN = endpoint.SourceMailboxLegacyDN;
				command.RemoteMailboxServerLegacyDN = endpoint.PublicFolderDatabaseServerLegacyDN;
				break;
			default:
				throw new NotSupportedException(string.Format("Batch direction '{0}' is not supported.", jobDirection));
			}
			if (jobSubscriptionSettings != null && jobSubscriptionSettings.BadItemLimit != null)
			{
				command.BadItemLimit = jobSubscriptionSettings.BadItemLimit.Value;
			}
			if (jobSubscriptionSettings != null && jobSubscriptionSettings.LargeItemLimit != null)
			{
				command.LargeItemLimit = jobSubscriptionSettings.LargeItemLimit.Value;
			}
		}

		private SubscriptionSnapshot InternalCreateSubscription(MigrationJobItem jobItem, bool whatIf)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationJob migrationJob = jobItem.MigrationJob;
			SubscriptionSnapshot result;
			using (IMigrationMessageItem migrationMessageItem = migrationJob.FindMessageItem(base.DataProvider, migrationJob.InitializationPropertyDefinitions))
			{
				using (IMigrationAttachment attachment = migrationMessageItem.GetAttachment("Request.csv", PropertyOpenMode.ReadOnly))
				{
					result = this.InternalCreateSubscription(migrationJob.JobDirection, (PublicFolderEndpoint)migrationJob.SourceEndpoint, (PublicFolderJobSubscriptionSettings)migrationJob.SubscriptionSettings, (MailboxData)jobItem.LocalMailbox, attachment.Stream, whatIf);
				}
			}
			return result;
		}

		private SubscriptionSnapshot InternalCreateSubscription(MigrationBatchDirection jobDirection, PublicFolderEndpoint endpoint, PublicFolderJobSubscriptionSettings jobSubscriptionSettings, MailboxData targetMailbox, Stream csvStream, bool whatIf)
		{
			NewPublicFolderMigrationRequestCommand newPublicFolderMigrationRequestCommand = new NewPublicFolderMigrationRequestCommand(whatIf)
			{
				BatchName = base.BatchName,
				TargetMailbox = targetMailbox.GetIdParameter<MailboxIdParameter>(),
				CSVStream = csvStream
			};
			if (base.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				newPublicFolderMigrationRequestCommand.Organization = new OrganizationIdParameter(base.OrganizationId);
			}
			MrsPublicFolderAccessor.ApplyNewOrSetCommonParameters(newPublicFolderMigrationRequestCommand, jobDirection, endpoint, jobSubscriptionSettings);
			Type type;
			RequestBase requestBase = base.Run<RequestBase>(newPublicFolderMigrationRequestCommand, out type);
			if (whatIf)
			{
				return null;
			}
			if (requestBase != null)
			{
				MRSSubscriptionId id = new MRSSubscriptionId(requestBase.RequestGuid, MigrationType.PublicFolder, targetMailbox);
				return SubscriptionSnapshot.CreateId(id);
			}
			SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(targetMailbox);
			MigrationUtil.AssertOrThrow(subscriptionSnapshot != null && subscriptionSnapshot.Id != null, "job endpoint '{0}' and user '{1}'", new object[]
			{
				endpoint,
				targetMailbox.MailboxIdentifier
			});
			MigrationLogger.Log(MigrationEventType.Verbose, "The previous subscription was {0}, updating it.", new object[]
			{
				subscriptionSnapshot.Status
			});
			this.UpdateSubscription((ISubscriptionId)subscriptionSnapshot.Id, jobDirection, endpoint, jobSubscriptionSettings);
			MigrationLogger.Log(MigrationEventType.Verbose, "now resuming subscription.", new object[]
			{
				subscriptionSnapshot.Status
			});
			this.ResumeSubscription((ISubscriptionId)subscriptionSnapshot.Id, false);
			return subscriptionSnapshot;
		}

		private bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationBatchDirection jobDirection, PublicFolderEndpoint endpoint, PublicFolderJobSubscriptionSettings jobSubscriptionSettings)
		{
			SetPublicFolderMigrationRequestCommand command = new SetPublicFolderMigrationRequestCommand(subscriptionId)
			{
				BatchName = base.BatchName
			};
			MrsPublicFolderAccessor.ApplyNewOrSetCommonParameters(command, jobDirection, endpoint, jobSubscriptionSettings);
			Type left;
			base.Run<RequestBase>(command, out left);
			return left == null;
		}

		private SubscriptionSnapshot RetrieveSubscriptionSnapshot(MailboxData targetMailbox)
		{
			MigrationUtil.ThrowOnNullArgument(targetMailbox, "targetMailbox");
			string identity;
			if (base.DataProvider.OrganizationId == OrganizationId.ForestWideOrgId)
			{
				identity = "\\PublicFolderMailboxMigration" + targetMailbox.UserMailboxId;
			}
			else
			{
				identity = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
				{
					base.DataProvider.OrganizationId.OrganizationalUnit.Name,
					"PublicFolderMailboxMigration" + targetMailbox.UserMailboxId
				});
			}
			GetPublicFolderMigrationRequestStatisticsCommand getPublicFolderMigrationRequestStatisticsCommand = new GetPublicFolderMigrationRequestStatisticsCommand(identity);
			if (base.IncludeReport)
			{
				getPublicFolderMigrationRequestStatisticsCommand.IncludeReport = true;
			}
			return this.RetrieveSubscriptionSnapshot(getPublicFolderMigrationRequestStatisticsCommand, targetMailbox) ?? SubscriptionSnapshot.CreateRemoved();
		}

		private SubscriptionSnapshot RetrieveSubscriptionSnapshot(GetPublicFolderMigrationRequestStatisticsCommand getStatisticsCommand, MailboxData localMailbox)
		{
			PublicFolderMailboxMigrationRequestStatistics publicFolderMailboxMigrationRequestStatistics = base.Run<PublicFolderMailboxMigrationRequestStatistics>(getStatisticsCommand);
			if (publicFolderMailboxMigrationRequestStatistics == null)
			{
				return null;
			}
			MRSSubscriptionId id = new MRSSubscriptionId(publicFolderMailboxMigrationRequestStatistics.RequestGuid, MigrationType.PublicFolder, localMailbox);
			if (publicFolderMailboxMigrationRequestStatistics.Status == RequestStatus.None)
			{
				return null;
			}
			ExDateTime createTime;
			if (publicFolderMailboxMigrationRequestStatistics.QueuedTimestamp != null)
			{
				createTime = (ExDateTime)publicFolderMailboxMigrationRequestStatistics.QueuedTimestamp.Value;
			}
			else
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "subscription {0} hasn't been queued yet", new object[]
				{
					publicFolderMailboxMigrationRequestStatistics
				});
				createTime = ExDateTime.UtcNow;
			}
			LocalizedString? errorMessage = null;
			if (!publicFolderMailboxMigrationRequestStatistics.IsValid)
			{
				errorMessage = new LocalizedString?(publicFolderMailboxMigrationRequestStatistics.ValidationMessage);
			}
			else if (publicFolderMailboxMigrationRequestStatistics.Status == RequestStatus.Failed)
			{
				errorMessage = new LocalizedString?(publicFolderMailboxMigrationRequestStatistics.Message);
			}
			ExDateTime? lastUpdateTime = (ExDateTime?)publicFolderMailboxMigrationRequestStatistics.LastUpdateTimestamp;
			ExDateTime? lastSyncTime = null;
			if (publicFolderMailboxMigrationRequestStatistics.Status == RequestStatus.AutoSuspended)
			{
				lastSyncTime = (ExDateTime?)publicFolderMailboxMigrationRequestStatistics.SuspendedTimestamp;
			}
			SubscriptionSnapshot subscriptionSnapshot = new SubscriptionSnapshot(id, MrsPublicFolderAccessor.GetSubscriptionStatus(publicFolderMailboxMigrationRequestStatistics.Status, publicFolderMailboxMigrationRequestStatistics.Suspend), publicFolderMailboxMigrationRequestStatistics.InitialSeedingCompletedTimestamp != null, createTime, lastUpdateTime, lastSyncTime, errorMessage, publicFolderMailboxMigrationRequestStatistics.BatchName)
			{
				TotalQueuedDuration = publicFolderMailboxMigrationRequestStatistics.TotalQueuedDuration,
				TotalInProgressDuration = publicFolderMailboxMigrationRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
				{
					RequestState.InitializingMove,
					RequestState.InitialSeeding,
					RequestState.Completion
				}),
				TotalSyncedDuration = publicFolderMailboxMigrationRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
				{
					RequestState.AutoSuspended
				}),
				EstimatedTotalTransferSize = new ByteQuantifiedSize?(publicFolderMailboxMigrationRequestStatistics.EstimatedTransferSize),
				EstimatedTotalTransferCount = new ulong?(publicFolderMailboxMigrationRequestStatistics.EstimatedTransferItemCount),
				BytesTransferred = publicFolderMailboxMigrationRequestStatistics.BytesTransferred,
				CurrentBytesTransferredPerMinute = publicFolderMailboxMigrationRequestStatistics.BytesTransferredPerMinute,
				Report = publicFolderMailboxMigrationRequestStatistics.Report,
				PercentageComplete = new int?(publicFolderMailboxMigrationRequestStatistics.PercentComplete)
			};
			subscriptionSnapshot.TotalStalledDuration = SubscriptionSnapshot.Subtract(publicFolderMailboxMigrationRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.Stalled,
				RequestState.TransientFailure,
				RequestState.Suspended,
				RequestState.Failed
			}), subscriptionSnapshot.TotalSyncedDuration);
			long numberItemsSynced = 0L;
			long numberItemsSkipped = 0L;
			if (publicFolderMailboxMigrationRequestStatistics.ItemsTransferred != null)
			{
				numberItemsSynced = MrsAccessorBase.HandleLongOverflow(publicFolderMailboxMigrationRequestStatistics.ItemsTransferred.Value, publicFolderMailboxMigrationRequestStatistics);
				numberItemsSkipped = (long)(publicFolderMailboxMigrationRequestStatistics.BadItemsEncountered + publicFolderMailboxMigrationRequestStatistics.LargeItemsEncountered);
			}
			long value = MrsAccessorBase.HandleLongOverflow(publicFolderMailboxMigrationRequestStatistics.TotalMailboxItemCount, publicFolderMailboxMigrationRequestStatistics);
			subscriptionSnapshot.SetStatistics(numberItemsSynced, numberItemsSkipped, new long?(value));
			if (publicFolderMailboxMigrationRequestStatistics.BytesTransferred != null && publicFolderMailboxMigrationRequestStatistics.TotalInProgressDuration != null && publicFolderMailboxMigrationRequestStatistics.TotalInProgressDuration.Value.Ticks > 0L)
			{
				subscriptionSnapshot.AverageBytesTransferredPerHour = new ByteQuantifiedSize?(publicFolderMailboxMigrationRequestStatistics.BytesTransferred.Value / (ulong)publicFolderMailboxMigrationRequestStatistics.TotalInProgressDuration.Value.Ticks * 36000000000UL);
			}
			else
			{
				subscriptionSnapshot.AverageBytesTransferredPerHour = null;
			}
			return subscriptionSnapshot;
		}

		private const string MrsRequestNamePrefix = "PublicFolderMailboxMigration";
	}
}
