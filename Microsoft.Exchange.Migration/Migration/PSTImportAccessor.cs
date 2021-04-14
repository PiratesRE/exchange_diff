using System;
using System.Collections.Generic;
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
	internal class PSTImportAccessor : MrsAccessorBase
	{
		public PSTImportAccessor(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider, batchName)
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
			PSTImportSubscriptionSnapshot pstimportSubscriptionSnapshot = subscriptionSnapshot as PSTImportSubscriptionSnapshot;
			PSTJobItemSubscriptionSettings pstjobItemSubscriptionSettings = migrationJobItem.SubscriptionSettings as PSTJobItemSubscriptionSettings;
			if (pstimportSubscriptionSnapshot == null)
			{
				return false;
			}
			if (pstjobItemSubscriptionSettings != null)
			{
				if (!StringComparer.OrdinalIgnoreCase.Equals(pstjobItemSubscriptionSettings.PstFilePath, pstimportSubscriptionSnapshot.PstFilePath))
				{
					return false;
				}
				if ((pstjobItemSubscriptionSettings.PrimaryOnly != null || pstjobItemSubscriptionSettings.ArchiveOnly != null) && (pstjobItemSubscriptionSettings.PrimaryOnly.Value != pstimportSubscriptionSnapshot.PrimaryOnly || pstjobItemSubscriptionSettings.ArchiveOnly.Value != pstimportSubscriptionSnapshot.ArchiveOnly))
				{
					return false;
				}
			}
			return true;
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateMailboxImportSubscription(jobItem, false);
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateMailboxImportSubscription(jobItem, true);
		}

		public SubscriptionSnapshot TestCreateSubscription(PSTImportEndpoint endpoint, PSTJobSubscriptionSettings jobSettings, PSTJobItemSubscriptionSettings jobItemSettings, IMailboxData localMailbox, string identifier)
		{
			return this.InternalCreateMailboxImportSubscription(endpoint, null, jobSettings, jobItemSettings, localMailbox, identifier, true);
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
			GetMailboxImportRequestStatisticsCommand getMailboxImportRequestStatisticsCommand = new GetMailboxImportRequestStatisticsCommand();
			getMailboxImportRequestStatisticsCommand.Identity = mrssubscriptionId;
			if (base.IncludeReport)
			{
				getMailboxImportRequestStatisticsCommand.IncludeReport = true;
			}
			MailboxImportRequestStatistics mailboxImportRequestStatistics = base.Run<MailboxImportRequestStatistics>(getMailboxImportRequestStatisticsCommand);
			if (mailboxImportRequestStatistics == null)
			{
				return null;
			}
			return this.RetrieveSubscriptionSnapshot(mailboxImportRequestStatistics, subscriptionId.MailboxData);
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			MigrationUtil.AssertOrThrow(mrssubscriptionId != null, "SubscriptionId needs to be a MRSSubscriptionID", new object[0]);
			PSTImportEndpoint pstimportEndpoint = endpoint as PSTImportEndpoint;
			MigrationUtil.AssertOrThrow(endpoint == null || pstimportEndpoint != null, "endpoint if passed in, needs to be a PST Import endpoint", new object[0]);
			MigrationUtil.AssertOrThrow(jobItem == null || jobItem.MigrationType == MigrationType.PSTImport, "job-item if passed in, needs to be a PST Import jobItem", new object[0]);
			SetMailboxImportRequestCommand setMailboxImportRequestCommand = new SetMailboxImportRequestCommand();
			setMailboxImportRequestCommand.Identity = subscriptionId;
			setMailboxImportRequestCommand.BatchName = base.BatchName;
			this.ApplySubscriptionSettings(setMailboxImportRequestCommand, jobItem.LocalMailbox, pstimportEndpoint, jobItem.MigrationJob.SubscriptionSettings, jobItem.SubscriptionSettings);
			Type left;
			this.Run(setMailboxImportRequestCommand, out left);
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

		internal static void RetrieveDuplicatedSettings(PSTJobSubscriptionSettings jobSettings, PSTJobItemSubscriptionSettings jobItemSettings, bool isNewMailboxImportRequest, out Unlimited<int>? badItemLimit, out Unlimited<int>? largeItemLimit, out bool primaryOnly, out bool archiveOnly)
		{
			primaryOnly = false;
			archiveOnly = false;
			badItemLimit = null;
			largeItemLimit = null;
			if (jobSettings != null && jobSettings.BadItemLimit != null)
			{
				badItemLimit = new Unlimited<int>?(jobSettings.BadItemLimit.Value);
			}
			if (jobSettings != null && jobSettings.LargeItemLimit != null)
			{
				largeItemLimit = new Unlimited<int>?(jobSettings.LargeItemLimit.Value);
			}
			if (isNewMailboxImportRequest && jobItemSettings != null && (jobItemSettings.PrimaryOnly != null || jobItemSettings.ArchiveOnly != null))
			{
				primaryOnly = jobItemSettings.PrimaryOnly.Value;
				archiveOnly = jobItemSettings.ArchiveOnly.Value;
			}
		}

		protected virtual SubscriptionSnapshot CreateMailboxImportSubscription(MigrationJobItem jobItem, bool whatIf)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			PSTImportEndpoint endpoint = jobItem.MigrationJob.SourceEndpoint as PSTImportEndpoint;
			return this.InternalCreateMailboxImportSubscription(endpoint, jobItem, jobItem.MigrationJob.SubscriptionSettings as PSTJobSubscriptionSettings, jobItem.SubscriptionSettings as PSTJobItemSubscriptionSettings, jobItem.LocalMailbox, jobItem.Identifier, whatIf);
		}

		protected SubscriptionSnapshot RetrieveSubscriptionSnapshot(MailboxImportRequestStatistics request, IMailboxData localMailbox)
		{
			if (request.QueuedTimestamp != null)
			{
				(ExDateTime)request.QueuedTimestamp.Value;
			}
			else
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "subscription {0} hasn't been queued yet", new object[]
				{
					request
				});
				ExDateTime utcNow = ExDateTime.UtcNow;
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
			MRSSubscriptionId id = new MRSSubscriptionId(request.RequestGuid, MigrationType.PSTImport, localMailbox);
			PSTImportSubscriptionSnapshot pstimportSubscriptionSnapshot = new PSTImportSubscriptionSnapshot(id, PSTImportAccessor.SubscriptionStatusFromRequestStatus(request.Status, request.Suspend), false, (ExDateTime)request.QueuedTimestamp.Value, new ExDateTime?((ExDateTime)request.LastUpdateTimestamp.Value), new ExDateTime?((ExDateTime)request.LastUpdateTimestamp.Value), errorMessage, request.BatchName, request.PrimaryOnly, request.ArchiveOnly, request.FilePath);
			long numberItemsSynced = 0L;
			if (request.ItemsTransferred != null)
			{
				numberItemsSynced = MrsAccessorBase.HandleLongOverflow(request.ItemsTransferred.Value, request);
			}
			long value = MrsAccessorBase.HandleLongOverflow(request.TotalMailboxItemCount, request);
			long numberItemsSkipped = (long)(request.BadItemsEncountered + request.LargeItemsEncountered);
			pstimportSubscriptionSnapshot.SetStatistics(numberItemsSynced, numberItemsSkipped, new long?(value));
			pstimportSubscriptionSnapshot.TotalQueuedDuration = request.TotalQueuedDuration;
			pstimportSubscriptionSnapshot.TotalInProgressDuration = request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.InitializingMove,
				RequestState.InitialSeeding,
				RequestState.Completion
			});
			pstimportSubscriptionSnapshot.TotalSyncedDuration = request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.AutoSuspended
			});
			pstimportSubscriptionSnapshot.TotalStalledDuration = SubscriptionSnapshot.Subtract(request.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.Stalled,
				RequestState.Relinquished,
				RequestState.TransientFailure,
				RequestState.Suspended,
				RequestState.Failed
			}), pstimportSubscriptionSnapshot.TotalSyncedDuration);
			pstimportSubscriptionSnapshot.EstimatedTotalTransferSize = new ByteQuantifiedSize?(new ByteQuantifiedSize(request.TotalMailboxSize) + ((request.TotalArchiveSize == null) ? ByteQuantifiedSize.Zero : new ByteQuantifiedSize(request.TotalArchiveSize.Value)));
			pstimportSubscriptionSnapshot.EstimatedTotalTransferCount = new ulong?(request.TotalMailboxItemCount + (request.TotalArchiveItemCount ?? 0UL));
			pstimportSubscriptionSnapshot.BytesTransferred = request.BytesTransferred;
			pstimportSubscriptionSnapshot.CurrentBytesTransferredPerMinute = request.BytesTransferredPerMinute;
			pstimportSubscriptionSnapshot.AverageBytesTransferredPerHour = ((request.BytesTransferred != null && request.TotalInProgressDuration != null && request.TotalInProgressDuration.Value.Ticks > 0L) ? new ByteQuantifiedSize?(request.BytesTransferred.Value / (ulong)request.TotalInProgressDuration.Value.Ticks * 36000000000UL) : null);
			pstimportSubscriptionSnapshot.Report = request.Report;
			pstimportSubscriptionSnapshot.PercentageComplete = new int?(request.PercentComplete);
			return pstimportSubscriptionSnapshot;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new RemoveMailboxImportRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			Type left;
			this.Run(new ResumeMailboxImportRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new SuspendMailboxImportRequestCommand
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
				MigrationLogger.Log(MigrationEventType.Error, "unsupported status for mailbox import request: {0}", new object[]
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
			MigrationLogger.Log(MigrationEventType.Error, "Unknown status for mailbox import request: {0}", new object[]
			{
				requestStatus
			});
			return SnapshotStatus.Corrupted;
		}

		private static string PickRandom(IList<string> databases)
		{
			MigrationUtil.ThrowOnNullArgument(databases, "databases");
			int num = new Random().Next(0, databases.Count);
			if (num >= 0)
			{
				return databases[num];
			}
			return null;
		}

		private void ApplySubscriptionSettings(NewMailboxImportRequestCommandBase command, IMailboxData localMailbox, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings)
		{
			PSTImportEndpoint pstimportEndpoint = endpointSettings as PSTImportEndpoint;
			PSTJobSubscriptionSettings jobSettings2 = jobSettings as PSTJobSubscriptionSettings;
			PSTJobItemSubscriptionSettings pstjobItemSubscriptionSettings = jobItemSettings as PSTJobItemSubscriptionSettings;
			bool flag = command is NewMailboxImportRequestCommand;
			Unlimited<int>? unlimited;
			Unlimited<int>? unlimited2;
			bool flag2;
			bool flag3;
			PSTImportAccessor.RetrieveDuplicatedSettings(jobSettings2, pstjobItemSubscriptionSettings, flag, out unlimited, out unlimited2, out flag2, out flag3);
			if (pstimportEndpoint != null)
			{
				command.RemoteHostName = pstimportEndpoint.RemoteServer;
				command.RemoteCredential = pstimportEndpoint.Credentials;
			}
			if (unlimited != null)
			{
				command.BadItemLimit = unlimited.Value;
			}
			if (unlimited2 != null)
			{
				command.LargeItemLimit = unlimited2.Value;
			}
			if (flag)
			{
				NewMailboxImportRequestCommand newMailboxImportRequestCommand = (NewMailboxImportRequestCommand)command;
				newMailboxImportRequestCommand.PstFilePath = pstjobItemSubscriptionSettings.PstFilePath;
				if (!string.IsNullOrEmpty(pstjobItemSubscriptionSettings.SourceRootFolder))
				{
					newMailboxImportRequestCommand.SourceRootFolder = pstjobItemSubscriptionSettings.SourceRootFolder;
				}
				if (!string.IsNullOrEmpty(pstjobItemSubscriptionSettings.TargetRootFolder))
				{
					newMailboxImportRequestCommand.TargetRootFolder = pstjobItemSubscriptionSettings.TargetRootFolder;
				}
				if (flag2)
				{
					newMailboxImportRequestCommand.IsArchive = false;
				}
				if (flag3)
				{
					newMailboxImportRequestCommand.IsArchive = true;
				}
			}
		}

		private SubscriptionSnapshot InternalCreateMailboxImportSubscription(PSTImportEndpoint endpoint, MigrationJobItem jobItem, PSTJobSubscriptionSettings jobSettings, PSTJobItemSubscriptionSettings jobItemSettings, IMailboxData localMailbox, string identifier, bool whatIf)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.AssertOrThrow(localMailbox != null, "LocalMailbox must not be null", new object[0]);
			NewMailboxImportRequestCommand newMailboxImportRequestCommand = new NewMailboxImportRequestCommand(whatIf);
			newMailboxImportRequestCommand.Mailbox = localMailbox.GetIdParameter<MailboxOrMailUserIdParameter>();
			newMailboxImportRequestCommand.BatchName = base.BatchName;
			newMailboxImportRequestCommand.Name = base.BatchName + ":" + identifier;
			newMailboxImportRequestCommand.AutoCleanup = false;
			this.ApplySubscriptionSettings(newMailboxImportRequestCommand, localMailbox, endpoint, jobSettings, jobItemSettings);
			SubscriptionSnapshot result;
			try
			{
				RequestBase requestBase = this.Run(newMailboxImportRequestCommand);
				if (whatIf)
				{
					result = null;
				}
				else
				{
					ISubscriptionId subscriptionId = new MRSSubscriptionId(requestBase.RequestGuid, MigrationType.PSTImport, localMailbox);
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
				if (jobItem.SubscriptionId == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "The job item didn't have a subscriptionId, so a mailbox import was not expected to exist.", new object[0]);
					throw;
				}
				SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(jobItem.SubscriptionId);
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
						this.UpdateSubscription(jobItem.SubscriptionId, endpoint, jobItem, true);
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
