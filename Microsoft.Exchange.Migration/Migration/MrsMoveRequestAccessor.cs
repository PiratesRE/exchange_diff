using System;
using System.Collections.Generic;
using System.Linq;
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
	internal class MrsMoveRequestAccessor : MrsAccessorBase
	{
		public MrsMoveRequestAccessor(IMigrationDataProvider dataProvider, string batchName, bool legacyManualSyncs = false) : base(dataProvider, batchName)
		{
			this.LegacyManualSyncs = legacyManualSyncs;
		}

		private protected bool LegacyManualSyncs { protected get; private set; }

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
			MoveSubscriptionSnapshot moveSubscriptionSnapshot = subscriptionSnapshot as MoveSubscriptionSnapshot;
			MoveJobItemSubscriptionSettings moveJobItemSubscriptionSettings = migrationJobItem.SubscriptionSettings as MoveJobItemSubscriptionSettings;
			MoveJobSubscriptionSettings moveJobSubscriptionSettings = migrationJobItem.MigrationJob.SubscriptionSettings as MoveJobSubscriptionSettings;
			if (moveSubscriptionSnapshot == null)
			{
				return false;
			}
			if (moveSubscriptionSnapshot.Direction != migrationJobItem.MigrationJob.JobDirection)
			{
				return false;
			}
			if (moveJobItemSubscriptionSettings != null && moveJobSubscriptionSettings != null)
			{
				if (!string.IsNullOrEmpty(moveJobItemSubscriptionSettings.TargetDatabase))
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(moveJobItemSubscriptionSettings.TargetDatabase, moveSubscriptionSnapshot.TargetDatabase))
					{
						return false;
					}
				}
				else if (moveJobSubscriptionSettings.TargetDatabases != null && moveJobSubscriptionSettings.TargetDatabases.Length >= 1 && !moveJobSubscriptionSettings.TargetDatabases.Contains(moveSubscriptionSnapshot.TargetDatabase, StringComparer.OrdinalIgnoreCase))
				{
					return false;
				}
				if (!string.IsNullOrEmpty(moveJobItemSubscriptionSettings.TargetArchiveDatabase))
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(moveJobItemSubscriptionSettings.TargetArchiveDatabase, moveSubscriptionSnapshot.TargetArchiveDatabase))
					{
						return false;
					}
				}
				else if (moveJobSubscriptionSettings.TargetArchiveDatabases != null && moveJobSubscriptionSettings.TargetArchiveDatabases.Length >= 1 && !moveJobSubscriptionSettings.TargetArchiveDatabases.Contains(moveSubscriptionSnapshot.TargetArchiveDatabase, StringComparer.OrdinalIgnoreCase))
				{
					return false;
				}
				if (moveJobItemSubscriptionSettings.PrimaryOnly != null || moveJobItemSubscriptionSettings.ArchiveOnly != null)
				{
					if (moveJobItemSubscriptionSettings.PrimaryOnly.Value != moveSubscriptionSnapshot.PrimaryOnly || moveJobItemSubscriptionSettings.ArchiveOnly.Value != moveSubscriptionSnapshot.ArchiveOnly)
					{
						return false;
					}
				}
				else if ((moveJobSubscriptionSettings.PrimaryOnly != null || moveJobSubscriptionSettings.ArchiveOnly != null) && (moveJobSubscriptionSettings.PrimaryOnly.Value != moveSubscriptionSnapshot.PrimaryOnly || moveJobSubscriptionSettings.ArchiveOnly.Value != moveSubscriptionSnapshot.ArchiveOnly))
				{
					return false;
				}
			}
			return true;
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateMoveSubscription(jobItem, false);
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			return this.CreateMoveSubscription(jobItem, true);
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
			GetMoveRequestStatisticsCommand getMoveRequestStatisticsCommand = new GetMoveRequestStatisticsCommand();
			getMoveRequestStatisticsCommand.Identity = mrssubscriptionId;
			if (base.IncludeReport)
			{
				getMoveRequestStatisticsCommand.IncludeReport = true;
			}
			MoveRequestStatistics moveRequestStatistics = this.Run(getMoveRequestStatisticsCommand);
			if (moveRequestStatistics == null)
			{
				return null;
			}
			return this.SubscriptionSnapshotFromMove(moveRequestStatistics, mrssubscriptionId.MailboxData);
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			MigrationUtil.AssertOrThrow(mrssubscriptionId != null, "SubscriptionId needs to be a MRSSubscriptionID", new object[0]);
			ExchangeRemoteMoveEndpoint exchangeRemoteMoveEndpoint = endpoint as ExchangeRemoteMoveEndpoint;
			MigrationUtil.AssertOrThrow(endpoint == null || exchangeRemoteMoveEndpoint != null, "endpoint if passed in, needs to be an Exchange endpoint", new object[0]);
			MigrationUtil.AssertOrThrow(jobItem == null || jobItem.MigrationType == MigrationType.ExchangeLocalMove || jobItem.MigrationType == MigrationType.ExchangeRemoteMove, "job-item if passed in, needs to be an Exchange jobItem", new object[0]);
			SetMoveRequestCommand setMoveRequestCommand = new SetMoveRequestCommand(new Type[]
			{
				typeof(CannotSetCompletedPermanentException)
			});
			setMoveRequestCommand.Identity = subscriptionId;
			setMoveRequestCommand.BatchName = base.BatchName;
			if (this.LegacyManualSyncs && adoptingSubscription)
			{
				setMoveRequestCommand.SuspendWhenReadyToComplete = true;
			}
			this.ApplySubscriptionSettings(setMoveRequestCommand, jobItem.LocalMailbox, exchangeRemoteMoveEndpoint, jobItem.MigrationJob.SubscriptionSettings, jobItem.SubscriptionSettings, jobItem.MigrationJob.JobDirection, !this.LegacyManualSyncs);
			Type left;
			this.Run(setMoveRequestCommand, out left);
			return left == null;
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			ResumeMoveRequestCommand resumeMoveRequestCommand = new ResumeMoveRequestCommand();
			resumeMoveRequestCommand.Identity = base.GetMRSIdentity(subscriptionId, true);
			if (!finalize && this.LegacyManualSyncs)
			{
				resumeMoveRequestCommand.SuspendWhenReadyToComplete = true;
			}
			Type left;
			this.Run(resumeMoveRequestCommand, out left);
			return left == null;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new SuspendMoveRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			Type left;
			this.Run(new RemoveMoveRequestCommand
			{
				Identity = base.GetMRSIdentity(subscriptionId, true)
			}, out left);
			return left == null;
		}

		public MoveRequestStatistics Run(MrsAccessorCommand command)
		{
			Type type;
			return this.Run(command, out type);
		}

		public MoveRequestStatistics Run(MrsAccessorCommand command, out Type ignoredErrorType)
		{
			return base.Run<MoveRequestStatistics>(command, out ignoredErrorType);
		}

		internal static void RetrieveDuplicatedSettings(MoveJobSubscriptionSettings moveJobSettings, MoveJobItemSubscriptionSettings moveJobItemSettings, bool isNewMoveRequest, out Unlimited<int>? badItemLimit, out Unlimited<int>? largeItemLimit, out string targetDatabase, out string targetArchiveDatabase, out bool primaryOnly, out bool archiveOnly)
		{
			primaryOnly = false;
			archiveOnly = false;
			targetDatabase = null;
			targetArchiveDatabase = null;
			badItemLimit = null;
			largeItemLimit = null;
			if (moveJobItemSettings.BadItemLimit != null || moveJobSettings.BadItemLimit != null)
			{
				Unlimited<int>? badItemLimit2 = moveJobItemSettings.BadItemLimit;
				badItemLimit = new Unlimited<int>?(((badItemLimit2 != null) ? new Unlimited<int>?(badItemLimit2.GetValueOrDefault()) : moveJobSettings.BadItemLimit).Value);
			}
			if (moveJobItemSettings.LargeItemLimit != null || moveJobSettings.LargeItemLimit != null)
			{
				Unlimited<int>? largeItemLimit2 = moveJobItemSettings.LargeItemLimit;
				largeItemLimit = new Unlimited<int>?(((largeItemLimit2 != null) ? new Unlimited<int>?(largeItemLimit2.GetValueOrDefault()) : moveJobSettings.LargeItemLimit).Value);
			}
			if (isNewMoveRequest)
			{
				if (moveJobItemSettings.PrimaryOnly != null || moveJobItemSettings.ArchiveOnly != null)
				{
					primaryOnly = moveJobItemSettings.PrimaryOnly.Value;
					archiveOnly = moveJobItemSettings.ArchiveOnly.Value;
				}
				else if (moveJobSettings.PrimaryOnly != null || moveJobSettings.ArchiveOnly != null)
				{
					primaryOnly = moveJobSettings.PrimaryOnly.Value;
					archiveOnly = moveJobSettings.ArchiveOnly.Value;
				}
				if (!archiveOnly)
				{
					if (!string.IsNullOrEmpty(moveJobItemSettings.TargetDatabase))
					{
						targetDatabase = moveJobItemSettings.TargetDatabase;
					}
					else if (moveJobSettings.TargetDatabases != null && moveJobSettings.TargetDatabases.Length > 0)
					{
						targetDatabase = MrsMoveRequestAccessor.PickRandom(moveJobSettings.TargetDatabases);
					}
				}
				if (!primaryOnly)
				{
					if (!string.IsNullOrEmpty(moveJobItemSettings.TargetArchiveDatabase))
					{
						targetArchiveDatabase = moveJobItemSettings.TargetArchiveDatabase;
						return;
					}
					if (moveJobSettings.TargetArchiveDatabases != null && moveJobSettings.TargetArchiveDatabases.Length > 0)
					{
						targetArchiveDatabase = MrsMoveRequestAccessor.PickRandom(moveJobSettings.TargetArchiveDatabases);
					}
				}
			}
		}

		protected virtual SubscriptionSnapshot CreateMoveSubscription(MigrationJobItem jobItem, bool whatIf)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.AssertOrThrow(jobItem.LocalMailbox != null, "LocalMailbox must have been found by now!", new object[0]);
			NewMoveRequestCommand newMoveRequestCommand = new NewMoveRequestCommand(whatIf);
			newMoveRequestCommand.Identity = jobItem.LocalMailbox.GetIdParameter<MailboxOrMailUserIdParameter>();
			newMoveRequestCommand.BatchName = base.BatchName;
			newMoveRequestCommand.AutoCleanup = false;
			ExchangeRemoteMoveEndpoint exchangeRemoteMoveEndpoint = null;
			switch (jobItem.MigrationJob.JobDirection)
			{
			case MigrationBatchDirection.Local:
				goto IL_F9;
			case MigrationBatchDirection.Onboarding:
				exchangeRemoteMoveEndpoint = (jobItem.MigrationJob.SourceEndpoint as ExchangeRemoteMoveEndpoint);
				MigrationUtil.AssertOrThrow(exchangeRemoteMoveEndpoint != null, "SourceEndpoint must be a non-null ExchangeRemoteMoveEndpoint.", new object[0]);
				goto IL_F9;
			case MigrationBatchDirection.Offboarding:
				exchangeRemoteMoveEndpoint = (jobItem.MigrationJob.TargetEndpoint as ExchangeRemoteMoveEndpoint);
				MigrationUtil.AssertOrThrow(exchangeRemoteMoveEndpoint != null, "TargetEndpoint must be a non-null ExchangeRemoteMoveEndpoint.", new object[0]);
				goto IL_F9;
			}
			ExAssert.RetailAssert(false, "Batch direction '{0}' is not yet supported.", new object[]
			{
				jobItem.MigrationJob.JobDirection
			});
			IL_F9:
			if (this.LegacyManualSyncs)
			{
				newMoveRequestCommand.SuspendWhenReadyToComplete = true;
			}
			else
			{
				newMoveRequestCommand.IncrementalSyncInterval = jobItem.MigrationJob.IncrementalSyncInterval.Value;
			}
			this.ApplySubscriptionSettings(newMoveRequestCommand, jobItem.LocalMailbox, exchangeRemoteMoveEndpoint, jobItem.MigrationJob.SubscriptionSettings, jobItem.SubscriptionSettings, jobItem.MigrationJob.JobDirection, !this.LegacyManualSyncs);
			SubscriptionSnapshot result;
			try
			{
				MoveRequestStatistics move = this.Run(newMoveRequestCommand);
				if (whatIf)
				{
					result = null;
				}
				else
				{
					result = this.SubscriptionSnapshotFromMove(move, jobItem.LocalMailbox);
				}
			}
			catch (MigrationPermanentException ex)
			{
				if (!(ex.InnerException is ManagementObjectAlreadyExistsException))
				{
					throw;
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "Found an already existing subscription for the job item, checking if it can be a failed job-item retry case.", new object[0]);
				ISubscriptionId subscriptionId = jobItem.SubscriptionId;
				if (subscriptionId == null)
				{
					subscriptionId = new MRSSubscriptionId(jobItem.MigrationType, jobItem.LocalMailbox);
				}
				SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(subscriptionId);
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
						this.UpdateSubscription(subscriptionId, exchangeRemoteMoveEndpoint, jobItem, true);
						MigrationLogger.Log(MigrationEventType.Verbose, "now resuming subscription.", new object[]
						{
							subscriptionSnapshot.Status
						});
						this.ResumeSubscription(subscriptionId, false);
					}
					result = subscriptionSnapshot;
				}
			}
			return result;
		}

		protected SubscriptionSnapshot SubscriptionSnapshotFromMove(MoveRequestStatistics move, IMailboxData localMailbox)
		{
			RequestStatus status = move.Status;
			SnapshotStatus status2 = MrsMoveRequestAccessor.SubscriptionStatusFromRequestStatus(status, move.Suspend);
			LocalizedString? errorMessage = null;
			if (!move.IsValid)
			{
				errorMessage = new LocalizedString?(move.ValidationMessage);
			}
			else if (move.Status == RequestStatus.Failed || move.Status == RequestStatus.CompletedWithWarning)
			{
				errorMessage = new LocalizedString?(move.Message);
			}
			bool flag = move.StartAfter != null && move.Status == RequestStatus.Queued && move.LastUpdateTimestamp.Value <= move.StartAfter;
			bool flag2 = move.CompleteAfter != null && move.Status == RequestStatus.Synced && move.LastUpdateTimestamp.Value <= move.CompleteAfter;
			ExDateTime value;
			if (flag || flag2)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				value = ((utcNow < (ExDateTime)move.LastUpdateTimestamp.Value) ? utcNow : ((ExDateTime)move.LastUpdateTimestamp.Value));
			}
			else
			{
				value = (ExDateTime)move.LastUpdateTimestamp.Value;
			}
			MRSSubscriptionId id = new MRSSubscriptionId(move.RequestGuid, string.IsNullOrEmpty(move.RemoteHostName) ? MigrationType.ExchangeLocalMove : MigrationType.ExchangeRemoteMove, localMailbox);
			MigrationBatchDirection migrationBatchDirection = MigrationBatchDirection.Local;
			if (move.Flags.HasFlag(RequestFlags.CrossOrg))
			{
				switch (move.Direction)
				{
				case RequestDirection.Pull:
					migrationBatchDirection = MigrationBatchDirection.Onboarding;
					break;
				case RequestDirection.Push:
					migrationBatchDirection = MigrationBatchDirection.Offboarding;
					break;
				}
			}
			string targetDatabase = null;
			string targetArchiveDatabase = null;
			if (migrationBatchDirection == MigrationBatchDirection.Local || migrationBatchDirection == MigrationBatchDirection.Onboarding)
			{
				if (move.TargetDatabase != null)
				{
					targetDatabase = move.TargetDatabase.Name;
				}
				if (move.TargetArchiveDatabase != null)
				{
					targetArchiveDatabase = move.TargetArchiveDatabase.Name;
				}
			}
			else if (migrationBatchDirection == MigrationBatchDirection.Offboarding)
			{
				targetDatabase = move.RemoteDatabaseName;
				targetArchiveDatabase = move.RemoteArchiveDatabaseName;
			}
			SubscriptionSnapshot subscriptionSnapshot = new MoveSubscriptionSnapshot(id, status2, move.InitialSeedingCompletedTimestamp != null, (ExDateTime)move.QueuedTimestamp.Value, new ExDateTime?(value), new ExDateTime?((ExDateTime)move.LastUpdateTimestamp.Value), errorMessage, move.BatchName, migrationBatchDirection, move.PrimaryOnly, move.ArchiveOnly, targetDatabase, targetArchiveDatabase);
			long numberItemsSynced = 0L;
			if (move.ItemsTransferred != null)
			{
				numberItemsSynced = MrsAccessorBase.HandleLongOverflow(move.ItemsTransferred.Value, move);
			}
			long value2 = MrsAccessorBase.HandleLongOverflow(move.TotalMailboxItemCount, move);
			long numberItemsSkipped = (long)(move.BadItemsEncountered + move.LargeItemsEncountered);
			subscriptionSnapshot.SetStatistics(numberItemsSynced, numberItemsSkipped, new long?(value2));
			subscriptionSnapshot.TotalQueuedDuration = move.TotalQueuedDuration;
			subscriptionSnapshot.TotalInProgressDuration = move.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.InitializingMove,
				RequestState.InitialSeeding,
				RequestState.Completion
			});
			subscriptionSnapshot.TotalSyncedDuration = move.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.AutoSuspended
			});
			subscriptionSnapshot.TotalStalledDuration = SubscriptionSnapshot.Subtract(move.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.Stalled,
				RequestState.Relinquished,
				RequestState.TransientFailure,
				RequestState.Suspended,
				RequestState.Failed
			}), subscriptionSnapshot.TotalSyncedDuration);
			subscriptionSnapshot.EstimatedTotalTransferSize = new ByteQuantifiedSize?(move.TotalMailboxSize + (move.TotalArchiveSize ?? ByteQuantifiedSize.Zero));
			subscriptionSnapshot.EstimatedTotalTransferCount = new ulong?(move.TotalMailboxItemCount + (move.TotalArchiveItemCount ?? 0UL));
			subscriptionSnapshot.BytesTransferred = move.BytesTransferred;
			subscriptionSnapshot.CurrentBytesTransferredPerMinute = move.BytesTransferredPerMinute;
			subscriptionSnapshot.AverageBytesTransferredPerHour = ((move.BytesTransferred != null && move.TotalInProgressDuration != null && move.TotalInProgressDuration.Value.Ticks > 0L) ? new ByteQuantifiedSize?(move.BytesTransferred.Value / (ulong)move.TotalInProgressDuration.Value.Ticks * 36000000000UL) : null);
			subscriptionSnapshot.Report = move.Report;
			subscriptionSnapshot.PercentageComplete = new int?(move.PercentComplete);
			return subscriptionSnapshot;
		}

		private static SnapshotStatus SubscriptionStatusFromRequestStatus(RequestStatus requestStatus, bool suspendFlag)
		{
			switch (requestStatus)
			{
			case RequestStatus.None:
			case RequestStatus.CompletionInProgress:
				MigrationLogger.Log(MigrationEventType.Error, "unsupported status for move request: {0}", new object[]
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
			MigrationLogger.Log(MigrationEventType.Error, "Unknown status for move request: {0}", new object[]
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

		private void ApplySubscriptionSettings(NewMoveRequestCommandBase command, IMailboxData localMailbox, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings, MigrationBatchDirection direction, bool skipSWRTC)
		{
			ExchangeRemoteMoveEndpoint exchangeRemoteMoveEndpoint = endpointSettings as ExchangeRemoteMoveEndpoint;
			MoveJobSubscriptionSettings moveJobSubscriptionSettings = jobSettings as MoveJobSubscriptionSettings;
			MoveJobItemSubscriptionSettings moveJobItemSettings = jobItemSettings as MoveJobItemSubscriptionSettings;
			bool flag = command is NewMoveRequestCommand;
			Unlimited<int>? unlimited;
			Unlimited<int>? unlimited2;
			string text;
			string text2;
			bool flag2;
			bool flag3;
			MrsMoveRequestAccessor.RetrieveDuplicatedSettings(moveJobSubscriptionSettings, moveJobItemSettings, flag, out unlimited, out unlimited2, out text, out text2, out flag2, out flag3);
			if (exchangeRemoteMoveEndpoint != null)
			{
				command.RemoteHostName = exchangeRemoteMoveEndpoint.RemoteServer;
				command.RemoteCredential = exchangeRemoteMoveEndpoint.Credentials;
			}
			if (unlimited != null)
			{
				command.BadItemLimit = unlimited.Value;
			}
			if (unlimited2 != null)
			{
				command.LargeItemLimit = unlimited2.Value;
			}
			DateTime? startAfter = (DateTime?)moveJobSubscriptionSettings.StartAfter;
			if (startAfter != null && startAfter.Value == DateTime.MinValue)
			{
				startAfter = null;
			}
			if (startAfter != null || (!flag && skipSWRTC))
			{
				command.StartAfter = startAfter;
			}
			DateTime? completeAfter = (DateTime?)moveJobSubscriptionSettings.CompleteAfter;
			if (completeAfter != null && completeAfter.Value == DateTime.MinValue)
			{
				completeAfter = null;
			}
			if (completeAfter != null || (!flag && skipSWRTC))
			{
				command.CompleteAfter = completeAfter;
			}
			if (flag)
			{
				NewMoveRequestCommand newMoveRequestCommand = (NewMoveRequestCommand)command;
				switch (direction)
				{
				case MigrationBatchDirection.Local:
					goto IL_1D0;
				case MigrationBatchDirection.Onboarding:
					MigrationUtil.AssertOrThrow(localMailbox.RecipientType == MigrationUserRecipientType.Mailuser, "Onboarding target recipients need to be MEUs", new object[0]);
					newMoveRequestCommand.IgnoreTenantMigrationPolicies = true;
					newMoveRequestCommand.Remote = true;
					goto IL_1D0;
				case MigrationBatchDirection.Offboarding:
					if (flag3)
					{
						MigrationUtil.AssertOrThrow(localMailbox.RecipientType == MigrationUserRecipientType.Mailuser, "Offboarding source recipients need to be MEUs for archive-only.", new object[0]);
					}
					else
					{
						MigrationUtil.AssertOrThrow(localMailbox.RecipientType == MigrationUserRecipientType.Mailbox, "Offboarding source recipients need to be MBXs when moving primary mailbox.", new object[0]);
					}
					newMoveRequestCommand.IgnoreTenantMigrationPolicies = true;
					newMoveRequestCommand.Outbound = true;
					goto IL_1D0;
				}
				ExAssert.RetailAssert(false, "Batch direction '{0}' is not yet supported.", new object[]
				{
					direction
				});
				IL_1D0:
				if (flag2)
				{
					newMoveRequestCommand.PrimaryOnly = true;
				}
				if (flag3)
				{
					newMoveRequestCommand.ArchiveOnly = true;
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (direction == MigrationBatchDirection.Offboarding)
					{
						newMoveRequestCommand.RemoteTargetDatabase = text;
					}
					else
					{
						newMoveRequestCommand.TargetDatabase = text;
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					if (direction == MigrationBatchDirection.Offboarding)
					{
						newMoveRequestCommand.RemoteArchiveTargetDatabase = text2;
					}
					else
					{
						newMoveRequestCommand.ArchiveTargetDatabase = text2;
					}
				}
				if (!string.IsNullOrEmpty(moveJobSubscriptionSettings.TargetDeliveryDomain))
				{
					newMoveRequestCommand.TargetDeliveryDomain = moveJobSubscriptionSettings.TargetDeliveryDomain;
				}
			}
		}
	}
}
