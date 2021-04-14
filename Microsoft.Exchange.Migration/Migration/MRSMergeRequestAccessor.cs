using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSMergeRequestAccessor : MrsAccessorBase
	{
		public MRSMergeRequestAccessor(IMigrationDataProvider dataProvider, string batchName, bool legacyManualSyncs = false) : base(dataProvider, batchName)
		{
			this.LegacyManualSyncs = legacyManualSyncs;
		}

		private protected bool LegacyManualSyncs { protected get; private set; }

		protected string JobName
		{
			get
			{
				return "Simple Migration Merge Request";
			}
		}

		protected MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.Merge;
			}
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			return this.InternalCreateSubscription((ExchangeOutlookAnywhereEndpoint)jobItem.MigrationJob.SourceEndpoint, (ExchangeJobSubscriptionSettings)jobItem.MigrationJob.SubscriptionSettings, MRSMergeRequestAccessor.GetSettings(jobItem), jobItem.LocalMailbox, jobItem.MigrationJob.IncrementalSyncInterval.Value, false);
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			return this.TestCreateSubscription((ExchangeOutlookAnywhereEndpoint)jobItem.MigrationJob.SourceEndpoint, (ExchangeJobSubscriptionSettings)jobItem.MigrationJob.SubscriptionSettings, MRSMergeRequestAccessor.GetSettings(jobItem), jobItem.LocalMailbox, jobItem.MigrationJob.IncrementalSyncInterval, true);
		}

		public SubscriptionSnapshot TestCreateSubscription(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobSubscriptionSettings jobSettings, ExchangeJobItemSubscriptionSettings subscriptionSettings, IMailboxData localMailbox, TimeSpan? incrementalSyncInterval, bool forceNew)
		{
			return this.InternalCreateSubscription(endpoint, jobSettings, subscriptionSettings, localMailbox, incrementalSyncInterval ?? TimeSpan.FromDays(1.0), true);
		}

		public override SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSAccessorIdCommand command = new MRSAccessorIdCommand("Get-MergeRequest", null, null, base.GetMRSIdentity(subscriptionId, false));
			RequestBase requestBase = this.Run(command);
			return MRSMergeRequestAccessor.GetSubscriptionStatus(requestBase.Status, requestBase.Suspend, null);
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			return this.UpdateSubscription(subscriptionId, endpoint, (ExchangeJobSubscriptionSettings)jobItem.MigrationJob.SubscriptionSettings, MRSMergeRequestAccessor.GetSettings(jobItem));
		}

		public bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, ExchangeJobSubscriptionSettings jobSubscriptionSettings, ExchangeJobItemSubscriptionSettings subscriptionSettings)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.ThrowOnNullArgument(subscriptionSettings, "subscriptionSettings");
			MRSMergeRequestAccessor.UpdateMRSAccessorCommand updateMRSAccessorCommand = new MRSMergeRequestAccessor.UpdateMRSAccessorCommand("Set-MergeRequest", base.GetMRSIdentity(subscriptionId, false), (ExchangeOutlookAnywhereEndpoint)endpoint, jobSubscriptionSettings, subscriptionSettings);
			if (!string.IsNullOrWhiteSpace(base.BatchName))
			{
				updateMRSAccessorCommand.Command.AddParameter("BatchName", base.BatchName);
			}
			Type left;
			this.Run(updateMRSAccessorCommand, out left);
			return left == null;
		}

		public override SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, false);
			MRSAccessorIdCommand mrsaccessorIdCommand = new MRSAccessorIdCommand("Get-MergeRequestStatistics", null, null, mrsidentity);
			if (base.IncludeReport)
			{
				mrsaccessorIdCommand.IncludeReport = true;
			}
			SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(mrsaccessorIdCommand, mrsidentity.MailboxData);
			if (subscriptionSnapshot == null)
			{
				subscriptionSnapshot = SubscriptionSnapshot.CreateRemoved();
			}
			return subscriptionSnapshot;
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, true);
			MrsAccessorCommand command = new MRSMergeRequestAccessor.ResumeMRSAccessorCommand("Resume-MergeRequest", mrsidentity, this.LegacyManualSyncs);
			Type left;
			this.Run(command, out left);
			return left == null;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, true);
			MrsAccessorCommand command = new MRSAccessorIdCommand("Suspend-MergeRequest", new Type[]
			{
				typeof(CannotSetCompletedPermanentException)
			}, new Type[]
			{
				typeof(CannotSetCompletingPermanentException)
			}, mrsidentity);
			Type left;
			this.Run(command, out left);
			return left == null;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId mrsidentity = base.GetMRSIdentity(subscriptionId, true);
			MrsAccessorCommand command = new MRSMergeRequestAccessor.MRSAccessorIdCommandIgnoreMissing("Remove-MergeRequest", new Type[]
			{
				typeof(CannotSetCompletingPermanentException)
			}, mrsidentity);
			Type left;
			this.Run(command, out left);
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

		internal static ExchangeJobItemSubscriptionSettings GetSettings(MigrationJobItem jobItem)
		{
			if (jobItem.IsPAW || (((ExchangeOutlookAnywhereEndpoint)jobItem.MigrationJob.SourceEndpoint).UseAutoDiscover && jobItem.SubscriptionSettings != null))
			{
				return (ExchangeJobItemSubscriptionSettings)jobItem.SubscriptionSettings;
			}
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
			string propertyValue = exchangeProvisioningDataStorage.ExchangeRecipient.GetPropertyValue<string>(PropTag.EmailAddress);
			string text = null;
			ExchangeMigrationRecipientWithHomeServer exchangeMigrationRecipientWithHomeServer = exchangeProvisioningDataStorage.ExchangeRecipient as ExchangeMigrationRecipientWithHomeServer;
			if (exchangeMigrationRecipientWithHomeServer != null)
			{
				text = exchangeMigrationRecipientWithHomeServer.MsExchHomeServerName;
			}
			return ExchangeJobItemSubscriptionSettings.CreateFromProperties(propertyValue, text, text, null);
		}

		protected static SnapshotStatus GetSubscriptionStatus(RequestStatus requestStatus, bool suspendFlag, ExceptionSide? exceptionSide)
		{
			switch (requestStatus)
			{
			case RequestStatus.None:
			case RequestStatus.CompletionInProgress:
			case RequestStatus.Completed:
			case RequestStatus.CompletedWithWarning:
				MigrationLogger.Log(MigrationEventType.Error, "unsupported status for merge request: {0}", new object[]
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
					if (!suspendFlag)
					{
						return SnapshotStatus.InProgress;
					}
					if (exceptionSide != null && exceptionSide.Value != ExceptionSide.Source)
					{
						MigrationLogger.Log(MigrationEventType.Error, "exception side {0} was not source, treating as corrupt", new object[]
						{
							exceptionSide.Value
						});
						return SnapshotStatus.Corrupted;
					}
					return SnapshotStatus.Failed;
				}
				break;
			}
			MigrationLogger.Log(MigrationEventType.Error, "unknown status for merge request: {0}", new object[]
			{
				requestStatus
			});
			return SnapshotStatus.Corrupted;
		}

		protected SubscriptionSnapshot RetrieveSubscriptionSnapshot(MrsAccessorCommand command, IMailboxData localMailbox)
		{
			MergeRequestStatistics mergeRequestStatistics = base.Run<MergeRequestStatistics>(command);
			if (mergeRequestStatistics == null)
			{
				return null;
			}
			MRSSubscriptionId id = new MRSSubscriptionId(mergeRequestStatistics.RequestGuid, MigrationType.ExchangeOutlookAnywhere, localMailbox);
			if (mergeRequestStatistics.Status == RequestStatus.None)
			{
				return null;
			}
			ExDateTime createTime;
			if (mergeRequestStatistics.QueuedTimestamp != null)
			{
				createTime = (ExDateTime)mergeRequestStatistics.QueuedTimestamp.Value;
			}
			else
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "subscription {0} hasn't been queued yet", new object[]
				{
					mergeRequestStatistics
				});
				createTime = ExDateTime.UtcNow;
			}
			LocalizedString? errorMessage = null;
			if (!mergeRequestStatistics.IsValid)
			{
				errorMessage = new LocalizedString?(mergeRequestStatistics.ValidationMessage);
			}
			else if (mergeRequestStatistics.Status == RequestStatus.Failed)
			{
				errorMessage = new LocalizedString?(mergeRequestStatistics.Message);
			}
			ExDateTime? lastUpdateTime = new ExDateTime?((ExDateTime)mergeRequestStatistics.LastUpdateTimestamp.Value);
			ExDateTime? lastSyncTime = null;
			if (mergeRequestStatistics.Status == RequestStatus.AutoSuspended)
			{
				lastSyncTime = (ExDateTime?)mergeRequestStatistics.SuspendedTimestamp;
			}
			SubscriptionSnapshot subscriptionSnapshot = new SubscriptionSnapshot(id, MRSMergeRequestAccessor.GetSubscriptionStatus(mergeRequestStatistics.Status, mergeRequestStatistics.Suspend, mergeRequestStatistics.FailureSide), mergeRequestStatistics.InitialSeedingCompletedTimestamp != null, createTime, lastUpdateTime, lastSyncTime, errorMessage, mergeRequestStatistics.BatchName);
			long numberItemsSynced = 0L;
			long numberItemsSkipped = 0L;
			if (mergeRequestStatistics.ItemsTransferred != null)
			{
				numberItemsSynced = MrsAccessorBase.HandleLongOverflow(mergeRequestStatistics.ItemsTransferred.Value, mergeRequestStatistics);
				numberItemsSkipped = (long)(mergeRequestStatistics.BadItemsEncountered + mergeRequestStatistics.LargeItemsEncountered);
			}
			long value = MrsAccessorBase.HandleLongOverflow(mergeRequestStatistics.TotalMailboxItemCount, mergeRequestStatistics);
			subscriptionSnapshot.SetStatistics(numberItemsSynced, numberItemsSkipped, new long?(value));
			subscriptionSnapshot.TotalQueuedDuration = mergeRequestStatistics.TotalQueuedDuration;
			subscriptionSnapshot.TotalInProgressDuration = mergeRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.InitializingMove,
				RequestState.InitialSeeding,
				RequestState.Completion
			});
			subscriptionSnapshot.TotalSyncedDuration = mergeRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.AutoSuspended
			});
			subscriptionSnapshot.TotalStalledDuration = SubscriptionSnapshot.Subtract(mergeRequestStatistics.TimeTracker.GetDisplayDuration(new RequestState[]
			{
				RequestState.Stalled,
				RequestState.TransientFailure,
				RequestState.Suspended,
				RequestState.Failed
			}), subscriptionSnapshot.TotalSyncedDuration);
			subscriptionSnapshot.EstimatedTotalTransferSize = new ByteQuantifiedSize?(mergeRequestStatistics.EstimatedTransferSize);
			subscriptionSnapshot.EstimatedTotalTransferCount = new ulong?(mergeRequestStatistics.EstimatedTransferItemCount);
			subscriptionSnapshot.BytesTransferred = mergeRequestStatistics.BytesTransferred;
			subscriptionSnapshot.CurrentBytesTransferredPerMinute = mergeRequestStatistics.BytesTransferredPerMinute;
			subscriptionSnapshot.AverageBytesTransferredPerHour = ((mergeRequestStatistics.BytesTransferred != null && mergeRequestStatistics.TotalInProgressDuration != null && mergeRequestStatistics.TotalInProgressDuration.Value.Ticks > 0L) ? new ByteQuantifiedSize?(mergeRequestStatistics.BytesTransferred.Value / (ulong)mergeRequestStatistics.TotalInProgressDuration.Value.Ticks * 36000000000UL) : null);
			subscriptionSnapshot.Report = mergeRequestStatistics.Report;
			subscriptionSnapshot.PercentageComplete = new int?(mergeRequestStatistics.PercentComplete);
			return subscriptionSnapshot;
		}

		protected object GetMRSIdentity(IMailboxData targetMailbox)
		{
			MigrationUtil.ThrowOnNullArgument(targetMailbox, "jobItem");
			string mailboxIdentifier = targetMailbox.MailboxIdentifier;
			return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
			{
				mailboxIdentifier,
				this.JobName
			});
		}

		private SubscriptionSnapshot InternalCreateSubscription(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobSubscriptionSettings jobSettings, ExchangeJobItemSubscriptionSettings jobItemSettings, IMailboxData targetMailbox, TimeSpan incrementalSyncInterval, bool whatIf)
		{
			bool useAdmin = endpoint.MailboxPermission == MigrationMailboxPermission.Admin;
			NewMergeRequestCommand newMergeRequestCommand = new NewMergeRequestCommand(endpoint, jobItemSettings, targetMailbox.GetIdParameter<MailboxOrMailUserIdParameter>(), "Simple Migration Merge Request", whatIf, useAdmin);
			if (this.LegacyManualSyncs)
			{
				newMergeRequestCommand.SuspendWhenReadyToComplete = true;
			}
			else
			{
				newMergeRequestCommand.IncrementalSyncInterval = incrementalSyncInterval;
			}
			if (!string.IsNullOrWhiteSpace(base.BatchName))
			{
				newMergeRequestCommand.BatchName = base.BatchName;
			}
			if (jobSettings != null && jobSettings.StartAfter != null)
			{
				newMergeRequestCommand.StartAfter = (DateTime?)jobSettings.StartAfter;
			}
			RequestBase requestBase = this.Run(newMergeRequestCommand);
			if (whatIf)
			{
				return null;
			}
			if (requestBase != null)
			{
				ISubscriptionId id = new MRSSubscriptionId(requestBase.RequestGuid, MigrationType.ExchangeOutlookAnywhere, targetMailbox);
				return SubscriptionSnapshot.CreateId(id);
			}
			SubscriptionSnapshot subscriptionSnapshot = this.RetrieveSubscriptionSnapshot(new MRSAccessorIdCommand("Get-MergeRequestStatistics", null, null, this.GetMRSIdentity(targetMailbox)), targetMailbox);
			MigrationUtil.AssertOrThrow(subscriptionSnapshot != null && subscriptionSnapshot.Id != null, "job endpoint {0} and user {1}", new object[]
			{
				endpoint,
				jobItemSettings
			});
			MigrationLogger.Log(MigrationEventType.Information, "Updating MRS subscription in Create because it already exists: {0} with connection settings {1}", new object[]
			{
				subscriptionSnapshot,
				endpoint
			});
			this.UpdateSubscription((ISubscriptionId)subscriptionSnapshot.Id, endpoint, jobSettings, jobItemSettings);
			if (subscriptionSnapshot.Status != SnapshotStatus.InProgress)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Updating subscription {0} to be in progress, formerly was {1}", new object[]
				{
					subscriptionSnapshot,
					subscriptionSnapshot.Status
				});
				this.ResumeSubscription((ISubscriptionId)subscriptionSnapshot.Id, false);
			}
			return subscriptionSnapshot;
		}

		private const string MRSMergeJobName = "Simple Migration Merge Request";

		private const string GetMergeRequestCommand = "Get-MergeRequest";

		private const string GetMergeRequestStatisticsCommand = "Get-MergeRequestStatistics";

		private const string SuspendMergeRequestCommand = "Suspend-MergeRequest";

		private const string ResumeMergeRequestCommand = "Resume-MergeRequest";

		private const string RemoveMergeRequestCommand = "Remove-MergeRequest";

		private const string SetMergeRequestCommand = "Set-MergeRequest";

		protected class MRSAccessorIdCommandIgnoreMissing : MRSAccessorIdCommand
		{
			public MRSAccessorIdCommandIgnoreMissing(string name, ICollection<Type> transientExceptions, MRSSubscriptionId identity) : base(name, MRSMergeRequestAccessor.MRSAccessorIdCommandIgnoreMissing.ExceptionsToIgnore, transientExceptions, identity)
			{
			}

			private static readonly Type[] ExceptionsToIgnore = new Type[]
			{
				typeof(ManagementObjectNotFoundException)
			};
		}

		protected class ResumeMRSAccessorCommand : MRSAccessorIdCommand
		{
			public ResumeMRSAccessorCommand(string name, MRSSubscriptionId identity, bool useSWRTC = false) : base(name, MRSMergeRequestAccessor.ResumeMRSAccessorCommand.IgnoreExceptionTypes, null, identity)
			{
				if (useSWRTC)
				{
					base.AddParameter("SuspendWhenReadyToComplete", true);
				}
			}

			private static readonly Type[] IgnoreExceptionTypes = new Type[]
			{
				typeof(CannotSetCompletedPermanentException)
			};
		}

		protected class UpdateMRSAccessorCommand : MRSAccessorIdCommand
		{
			public UpdateMRSAccessorCommand(string name, MRSSubscriptionId identity, ExchangeOutlookAnywhereEndpoint settings, ExchangeJobSubscriptionSettings jobSettings, ExchangeJobItemSubscriptionSettings jobItemSettings) : base(name, MRSMergeRequestAccessor.UpdateMRSAccessorCommand.IgnoreExceptionTypes, null, identity)
			{
				this.UpdateSubscriptionSettings(settings, jobSettings, jobItemSettings);
			}

			protected override void UpdateSubscriptionSettings(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobSubscriptionSettings jobSettings, ExchangeJobItemSubscriptionSettings jobItemSettings)
			{
				base.UpdateSubscriptionSettings(endpoint, jobSettings, jobItemSettings);
				if (jobSettings != null && jobSettings.StartAfter != null)
				{
					base.AddParameter("StartAfter", (DateTime?)jobSettings.StartAfter);
				}
			}

			private static readonly Type[] IgnoreExceptionTypes = new Type[]
			{
				typeof(CannotSetCompletedPermanentException)
			};
		}
	}
}
