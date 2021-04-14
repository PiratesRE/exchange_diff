using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MRSSyncRequestAccessorBase : MrsAccessorBase
	{
		public MRSSyncRequestAccessorBase(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider, batchName)
		{
		}

		public override bool IsSnapshotCompatible(SubscriptionSnapshot subscriptionSnapshot, MigrationJobItem migrationJobItem)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionSnapshot, "subscriptionSnapshot");
			MigrationUtil.ThrowOnNullArgument(migrationJobItem, "migrationJobItem");
			if (migrationJobItem.MigrationJob.OriginalJobId == null)
			{
				if (!StringComparer.InvariantCultureIgnoreCase.Equals(base.BatchName, subscriptionSnapshot.BatchName))
				{
					return false;
				}
				if (subscriptionSnapshot.InjectionCompletedTime != null && subscriptionSnapshot.InjectionCompletedTime.Value < migrationJobItem.CreationTime)
				{
					return false;
				}
			}
			return true;
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateSyncSubscription(jobItem, false);
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateSyncSubscription(jobItem, true);
		}

		public override SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			if (mrssubscriptionId == null)
			{
				return SnapshotStatus.Corrupted;
			}
			SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(mrssubscriptionId);
			if (subscriptionSnapshot != null)
			{
				return subscriptionSnapshot.Status;
			}
			return SnapshotStatus.Removed;
		}

		public override SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			if (mrssubscriptionId == null)
			{
				return null;
			}
			GetSyncRequestStatisticsCommand getSyncRequestStatisticsCommand = new GetSyncRequestStatisticsCommand();
			getSyncRequestStatisticsCommand.Identity = mrssubscriptionId;
			if (base.IncludeReport)
			{
				getSyncRequestStatisticsCommand.IncludeReport = true;
			}
			SyncRequestStatistics syncRequestStatistics = base.Run<SyncRequestStatistics>(getSyncRequestStatisticsCommand);
			if (syncRequestStatistics == null)
			{
				return null;
			}
			return this.RetrieveSubscriptionSnapshot(syncRequestStatistics, mrssubscriptionId.MailboxData);
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			MigrationUtil.AssertOrThrow(mrssubscriptionId != null, "SubscriptionId needs to be a MRSSubscriptionID", new object[0]);
			SetSyncRequestCommand setSyncRequestCommand = new SetSyncRequestCommand();
			setSyncRequestCommand.Identity = subscriptionId;
			setSyncRequestCommand.BatchName = base.BatchName;
			this.ApplySubscriptionSettings(setSyncRequestCommand, jobItem.Identifier, jobItem.LocalMailbox, endpoint, jobItem.MigrationJob.SubscriptionSettings, jobItem.SubscriptionSettings);
			Type left;
			this.Run(setSyncRequestCommand, out left);
			return left == null;
		}

		public RequestBase Run(MrsAccessorCommand command)
		{
			Type type;
			return this.Run(command, out type);
		}

		public RequestBase Run(MrsAccessorCommand command, out Type ignoredErrorType)
		{
			return base.Run<RequestBase>(command, out ignoredErrorType);
		}

		protected SubscriptionSnapshot CreateSyncSubscription(MigrationJobItem jobItem, bool whatIf)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			return this.InternalCreateSyncSubscription(jobItem, jobItem.MigrationJob.SourceEndpoint, jobItem.MigrationJob.SubscriptionSettings, jobItem.SubscriptionSettings, jobItem.LocalMailbox, jobItem.Identifier, whatIf);
		}

		protected SubscriptionSnapshot RetrieveSubscriptionSnapshot(SyncRequestStatistics request, IMailboxData localMailbox)
		{
			ExDateTime createTime;
			if (request.QueuedTimestamp != null)
			{
				createTime = (ExDateTime)request.QueuedTimestamp.Value;
			}
			else
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "subscription {0} hasn't been queued yet", new object[]
				{
					request
				});
				createTime = ExDateTime.UtcNow;
			}
			LocalizedString? errorMessage = null;
			if (!request.IsValid)
			{
				errorMessage = new LocalizedString?(request.ValidationMessage);
			}
			else if (request.Status == RequestStatus.Failed || request.Status == RequestStatus.CompletedWithWarning)
			{
				errorMessage = new LocalizedString?(request.Message);
			}
			new ExDateTime?((ExDateTime)request.LastUpdateTimestamp.Value);
			if (request.Status == RequestStatus.AutoSuspended)
			{
				(ExDateTime?)request.SuspendedTimestamp;
			}
			MRSSubscriptionId id = new MRSSubscriptionId(request.RequestGuid, MigrationType.IMAP, localMailbox);
			SyncSubscriptionSnapshot syncSubscriptionSnapshot = new SyncSubscriptionSnapshot(id, MRSSyncRequestAccessorBase.SubscriptionStatusFromRequestStatus(request.Status, request.Suspend), request.SyncStage == SyncStage.IncrementalSync, createTime, new ExDateTime?((ExDateTime)request.LastUpdateTimestamp.Value), new ExDateTime?((ExDateTime)request.LastUpdateTimestamp.Value), errorMessage, request.BatchName, request.SyncProtocol, request.RemoteCredentialUsername, request.EmailAddress);
			long numberItemsSynced = 0L;
			if (request.ItemsTransferred != null)
			{
				numberItemsSynced = MrsAccessorBase.HandleLongOverflow(request.ItemsTransferred.Value, request);
			}
			long value = MrsAccessorBase.HandleLongOverflow(request.TotalMailboxItemCount, request);
			long numberItemsSkipped = (long)(request.BadItemsEncountered + request.LargeItemsEncountered);
			syncSubscriptionSnapshot.SetStatistics(numberItemsSynced, numberItemsSkipped, new long?(value));
			syncSubscriptionSnapshot.TotalQueuedDuration = request.TotalQueuedDuration;
			syncSubscriptionSnapshot.TotalInProgressDuration = request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.InitializingMove,
				RequestState.InitialSeeding,
				RequestState.Completion
			});
			syncSubscriptionSnapshot.TotalSyncedDuration = request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.AutoSuspended
			});
			syncSubscriptionSnapshot.TotalStalledDuration = SubscriptionSnapshot.Subtract(request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.Stalled,
				RequestState.Relinquished,
				RequestState.TransientFailure,
				RequestState.Suspended,
				RequestState.Failed
			}), syncSubscriptionSnapshot.TotalSyncedDuration);
			syncSubscriptionSnapshot.EstimatedTotalTransferSize = new ByteQuantifiedSize?(new ByteQuantifiedSize(request.TotalMailboxSize) + ((request.TotalArchiveSize == null) ? ByteQuantifiedSize.Zero : new ByteQuantifiedSize(request.TotalArchiveSize.Value)));
			syncSubscriptionSnapshot.EstimatedTotalTransferCount = new ulong?(request.TotalMailboxItemCount + (request.TotalArchiveItemCount ?? 0UL));
			syncSubscriptionSnapshot.BytesTransferred = request.BytesTransferred;
			syncSubscriptionSnapshot.CurrentBytesTransferredPerMinute = request.BytesTransferredPerMinute;
			syncSubscriptionSnapshot.AverageBytesTransferredPerHour = ((request.BytesTransferred != null && request.TotalInProgressDuration != null && request.TotalInProgressDuration.Value.Ticks > 0L) ? new ByteQuantifiedSize?(request.BytesTransferred.Value / (ulong)request.TotalInProgressDuration.Value.Ticks * 36000000000UL) : null);
			syncSubscriptionSnapshot.Report = request.Report;
			syncSubscriptionSnapshot.PercentageComplete = new int?(request.PercentComplete);
			return syncSubscriptionSnapshot;
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			Type left;
			this.Run(new ResumeSyncRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new SuspendSyncRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new RemoveSyncRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		private static SnapshotStatus SubscriptionStatusFromRequestStatus(RequestStatus requestStatus, bool suspendFlag)
		{
			switch (requestStatus)
			{
			case RequestStatus.None:
			case RequestStatus.CompletionInProgress:
				MigrationLogger.Log(MigrationEventType.Error, "unsupported status for sync request: {0}", new object[]
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
			MigrationLogger.Log(MigrationEventType.Error, "Unknown status for sync request: {0}", new object[]
			{
				requestStatus
			});
			return SnapshotStatus.Corrupted;
		}

		protected abstract void ApplySubscriptionSettings(NewSyncRequestCommandBase command, string identifier, IMailboxData localMailbox, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings);

		private SubscriptionSnapshot InternalCreateSyncSubscription(MigrationJobItem jobItem, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings, IMailboxData localMailbox, string identifier, bool whatIf)
		{
			MigrationUtil.AssertOrThrow(localMailbox != null, "LocalMailbox must not be null", new object[0]);
			NewSyncRequestCommand newSyncRequestCommand = new NewSyncRequestCommand(whatIf);
			newSyncRequestCommand.BatchName = base.BatchName;
			this.ApplySubscriptionSettings(newSyncRequestCommand, identifier, localMailbox, endpointSettings, jobSettings, jobItemSettings);
			SubscriptionSnapshot result;
			try
			{
				RequestBase requestBase = this.Run(newSyncRequestCommand);
				if (whatIf)
				{
					result = null;
				}
				else
				{
					ISubscriptionId subscriptionId = new MRSSubscriptionId(requestBase.RequestGuid, jobItem.MigrationType, localMailbox);
					result = this.RetrieveSubscriptionSnapshot(subscriptionId);
				}
			}
			catch (MigrationPermanentException ex)
			{
				if (jobItem == null)
				{
					throw;
				}
				if (!(ex.InnerException is ManagementObjectAlreadyExistsException))
				{
					throw;
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "Found an already existing subscription for the job item, checking if it can be a failed job-item retry case.", new object[0]);
				ISubscriptionId subscriptionId2 = jobItem.SubscriptionId;
				if (subscriptionId2 == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "The job item didn't have a subscriptionId, so a sync request was not expected to exist.", new object[0]);
					throw;
				}
				SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(subscriptionId2);
				if (subscriptionSnapshot == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "The job item didn't have a snapshot corresponding to the subscriptionId, so something is wrong (race condition, etc.).", new object[0]);
					throw;
				}
				if (!this.IsSnapshotCompatible(subscriptionSnapshot, jobItem))
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "The subscription wasn't compatible with the accessor, failing.", new object[0]);
					throw new UserAlreadyBeingMigratedException(jobItem.Identifier, ex.InnerException);
				}
				if (whatIf)
				{
					result = null;
				}
				else
				{
					if (subscriptionSnapshot.Status == SnapshotStatus.Failed || subscriptionSnapshot.Status == SnapshotStatus.AutoSuspended || subscriptionSnapshot.Status == SnapshotStatus.Suspended)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "The previous subscription was {0}, updating it.", new object[]
						{
							subscriptionSnapshot.Status
						});
						this.UpdateSubscription(jobItem.SubscriptionId, endpointSettings as MigrationEndpointBase, jobItem, true);
						MigrationLogger.Log(MigrationEventType.Verbose, "now resuming subscription.", new object[]
						{
							subscriptionSnapshot.Status
						});
						this.ResumeSubscription(jobItem.SubscriptionId, false);
					}
					result = subscriptionSnapshot;
				}
			}
			return result;
		}
	}
}
