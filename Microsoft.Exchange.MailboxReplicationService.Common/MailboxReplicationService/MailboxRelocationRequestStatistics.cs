using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MailboxRelocationRequestStatistics : RequestStatisticsBase
	{
		public MailboxRelocationRequestStatistics()
		{
		}

		internal MailboxRelocationRequestStatistics(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
		}

		internal MailboxRelocationRequestStatistics(TransactionalRequestJob requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
		{
			base.CopyNonSchematizedPropertiesFrom(requestJob);
		}

		internal MailboxRelocationRequestStatistics(RequestJobXML requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
		{
			base.CopyNonSchematizedPropertiesFrom(requestJob);
		}

		public ADObjectId MailboxIdentity
		{
			get
			{
				return base.UserId;
			}
			internal set
			{
				base.UserId = value;
			}
		}

		public new string DistinguishedName
		{
			get
			{
				return base.DistinguishedName;
			}
			internal set
			{
				base.DistinguishedName = value;
			}
		}

		public new string DisplayName
		{
			get
			{
				return base.DisplayName;
			}
			internal set
			{
				base.DisplayName = value;
			}
		}

		public new string Alias
		{
			get
			{
				return base.Alias;
			}
			internal set
			{
				base.Alias = value;
			}
		}

		public new Guid ExchangeGuid
		{
			get
			{
				return base.ExchangeGuid;
			}
			internal set
			{
				base.ExchangeGuid = value;
			}
		}

		public new RequestStatus Status
		{
			get
			{
				return base.Status;
			}
			internal set
			{
				base.Status = value;
			}
		}

		public new RequestState StatusDetail
		{
			get
			{
				return base.StatusDetail;
			}
		}

		public new SyncStage SyncStage
		{
			get
			{
				return base.SyncStage;
			}
			internal set
			{
				base.SyncStage = value;
			}
		}

		public new RequestFlags Flags
		{
			get
			{
				return base.Flags;
			}
			internal set
			{
				base.Flags = value;
			}
		}

		public new RequestStyle RequestStyle
		{
			get
			{
				return base.RequestStyle;
			}
			internal set
			{
				base.RequestStyle = value;
			}
		}

		public new RequestDirection Direction
		{
			get
			{
				return base.Direction;
			}
			internal set
			{
				base.Direction = value;
			}
		}

		public new RequestPriority Priority
		{
			get
			{
				return base.Priority;
			}
			internal set
			{
				base.Priority = value;
			}
		}

		public new RequestWorkloadType WorkloadType
		{
			get
			{
				return base.WorkloadType;
			}
			internal set
			{
				base.WorkloadType = value;
			}
		}

		public new bool Suspend
		{
			get
			{
				return base.Suspend;
			}
			internal set
			{
				base.Suspend = value;
			}
		}

		public new ServerVersion SourceVersion
		{
			get
			{
				if (base.SourceVersion == 0)
				{
					return null;
				}
				return new ServerVersion(base.SourceVersion);
			}
			internal set
			{
				base.SourceVersion = ((value != null) ? value.ToInt() : 0);
			}
		}

		public new ADObjectId SourceDatabase
		{
			get
			{
				return base.SourceDatabase;
			}
			internal set
			{
				base.SourceDatabase = value;
			}
		}

		public new string SourceServer
		{
			get
			{
				return base.SourceServer;
			}
			internal set
			{
				base.SourceServer = value;
			}
		}

		public new ServerVersion TargetVersion
		{
			get
			{
				if (base.TargetVersion == 0)
				{
					return null;
				}
				return new ServerVersion(base.TargetVersion);
			}
			set
			{
				base.TargetVersion = ((value != null) ? value.ToInt() : 0);
			}
		}

		public new ADObjectId TargetDatabase
		{
			get
			{
				return base.TargetDatabase;
			}
			internal set
			{
				base.TargetDatabase = value;
			}
		}

		public new Guid? TargetContainerGuid
		{
			get
			{
				return base.TargetContainerGuid;
			}
			internal set
			{
				base.TargetContainerGuid = value;
			}
		}

		public new CrossTenantObjectId TargetUnifiedMailboxId
		{
			get
			{
				return base.TargetUnifiedMailboxId;
			}
			internal set
			{
				base.TargetUnifiedMailboxId = value;
			}
		}

		public new string TargetServer
		{
			get
			{
				return base.TargetServer;
			}
			internal set
			{
				base.TargetServer = value;
			}
		}

		public new string BatchName
		{
			get
			{
				return base.BatchName;
			}
			internal set
			{
				base.BatchName = value;
			}
		}

		public new Unlimited<int> BadItemLimit
		{
			get
			{
				return base.BadItemLimit;
			}
			internal set
			{
				base.BadItemLimit = value;
			}
		}

		public new int BadItemsEncountered
		{
			get
			{
				return base.BadItemsEncountered;
			}
			internal set
			{
				base.BadItemsEncountered = value;
			}
		}

		public DateTime? QueuedTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.Creation);
			}
		}

		public DateTime? StartTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.Start);
			}
		}

		public DateTime? LastUpdateTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.LastUpdate);
			}
		}

		public DateTime? InitialSeedingCompletedTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.InitialSeedingCompleted);
			}
		}

		public DateTime? FinalSyncTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.FinalSync);
			}
		}

		public DateTime? CompletionTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.Completion);
			}
		}

		public DateTime? SuspendedTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.Suspended);
			}
		}

		public EnhancedTimeSpan? OverallDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.OverallMove).Duration);
			}
		}

		public EnhancedTimeSpan? TotalFinalizationDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.Finalization).Duration);
			}
		}

		public EnhancedTimeSpan? TotalDataReplicationWaitDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.DataReplicationWait).Duration);
			}
		}

		public EnhancedTimeSpan? TotalSuspendedDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.Suspended).Duration);
			}
		}

		public EnhancedTimeSpan? TotalFailedDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.Failed).Duration);
			}
		}

		public EnhancedTimeSpan? TotalQueuedDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.Queued).Duration);
			}
		}

		public EnhancedTimeSpan? TotalInProgressDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.InProgress).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToCIDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToCI).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToHADuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToHA).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToMailboxLockedDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToMailboxLock).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToReadThrottle
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToReadThrottle).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToWriteThrottle
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToWriteThrottle).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToReadCpu
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToReadCpu).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToWriteCpu
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToWriteCpu).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToReadUnknown
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToReadUnknown).Duration);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToWriteUnknown
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.StalledDueToWriteUnknown).Duration);
			}
		}

		public EnhancedTimeSpan? TotalTransientFailureDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.TransientFailure).Duration);
			}
		}

		public EnhancedTimeSpan? TotalProxyBackoffDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.ProxyBackoff).Duration);
			}
		}

		public EnhancedTimeSpan? TotalIdleDuration
		{
			get
			{
				return new EnhancedTimeSpan?(base.TimeTracker.GetDisplayDuration(RequestState.Idle).Duration);
			}
		}

		public new string MRSServerName
		{
			get
			{
				return base.MRSServerName;
			}
			internal set
			{
				base.MRSServerName = value;
			}
		}

		public new ByteQuantifiedSize TotalMailboxSize
		{
			get
			{
				return new ByteQuantifiedSize(base.TotalMailboxSize);
			}
			internal set
			{
				base.TotalMailboxSize = value.ToBytes();
			}
		}

		public new ulong TotalMailboxItemCount
		{
			get
			{
				return base.TotalMailboxItemCount;
			}
			internal set
			{
				base.TotalMailboxItemCount = value;
			}
		}

		public override ByteQuantifiedSize? BytesTransferred
		{
			get
			{
				return base.BytesTransferred;
			}
		}

		public override ByteQuantifiedSize? BytesTransferredPerMinute
		{
			get
			{
				return base.BytesTransferredPerMinute;
			}
		}

		public override ulong? ItemsTransferred
		{
			get
			{
				return base.ItemsTransferred;
			}
		}

		public new int PercentComplete
		{
			get
			{
				return base.PercentComplete;
			}
			internal set
			{
				base.PercentComplete = value;
			}
		}

		public new Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
		{
			get
			{
				return base.CompletedRequestAgeLimit;
			}
			internal set
			{
				base.CompletedRequestAgeLimit = value;
			}
		}

		public override LocalizedString PositionInQueue
		{
			get
			{
				return base.PositionInQueue;
			}
			internal set
			{
				base.PositionInQueue = value;
			}
		}

		public RequestJobInternalFlags InternalFlags
		{
			get
			{
				return base.RequestJobInternalFlags;
			}
			internal set
			{
				base.RequestJobInternalFlags = value;
			}
		}

		public new int? FailureCode
		{
			get
			{
				return base.FailureCode;
			}
			internal set
			{
				base.FailureCode = value;
			}
		}

		public new string FailureType
		{
			get
			{
				return base.FailureType;
			}
			internal set
			{
				base.FailureType = value;
			}
		}

		public new ExceptionSide? FailureSide
		{
			get
			{
				return base.FailureSide;
			}
			internal set
			{
				base.FailureSide = value;
			}
		}

		public new LocalizedString Message
		{
			get
			{
				return base.Message;
			}
			internal set
			{
				base.Message = value;
			}
		}

		public DateTime? FailureTimestamp
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.Failure);
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.IsValid;
			}
		}

		public new LocalizedString ValidationMessage
		{
			get
			{
				return base.ValidationMessage;
			}
			internal set
			{
				base.ValidationMessage = value;
			}
		}

		public new Guid RequestGuid
		{
			get
			{
				return base.RequestGuid;
			}
			internal set
			{
				base.RequestGuid = value;
			}
		}

		public new ADObjectId RequestQueue
		{
			get
			{
				return base.RequestQueue;
			}
			internal set
			{
				base.RequestQueue = value;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
			internal set
			{
				base.Identity = (RequestJobObjectId)value;
			}
		}

		public new string DiagnosticInfo
		{
			get
			{
				return base.DiagnosticInfo;
			}
		}

		public override Report Report
		{
			get
			{
				return base.Report;
			}
			internal set
			{
				base.Report = value;
			}
		}

		public static bool IsTerminalState(RequestJobBase requestJobBase)
		{
			return requestJobBase.CancelRequest || MailboxRelocationRequestStatistics.IsTerminalState(requestJobBase.Status) || requestJobBase.SyncStage >= SyncStage.Cleanup;
		}

		public static bool IsTerminalState(RequestStatus requestStatus)
		{
			return requestStatus == RequestStatus.Completed || requestStatus == RequestStatus.CompletedWithWarning;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(base.Name) && !string.IsNullOrEmpty(this.Alias))
			{
				return string.Format("{0}\\{1}", this.Alias, base.Name);
			}
			return base.ToString();
		}

		internal static void ValidateRequestJob(RequestJobBase requestJob)
		{
			if (requestJob.IsFake || requestJob.WorkItemQueueMdb == null)
			{
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMissing);
				requestJob.ValidationMessage = MrsStrings.ValidationMoveRequestNotDeserialized;
				return;
			}
			if (requestJob.OriginatingMDBGuid != Guid.Empty && requestJob.OriginatingMDBGuid != requestJob.WorkItemQueueMdb.ObjectGuid)
			{
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Orphaned);
				requestJob.ValidationMessage = MrsStrings.ValidationMoveRequestInWrongMDB(requestJob.OriginatingMDBGuid, requestJob.WorkItemQueueMdb.ObjectGuid);
				return;
			}
			if (requestJob.OriginatingMDBGuid == Guid.Empty)
			{
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Valid);
				requestJob.ValidationMessage = LocalizedString.Empty;
				return;
			}
			MailboxRelocationRequestStatistics.LoadAdditionalPropertiesFromUser(requestJob);
			if (MailboxRelocationRequestStatistics.IsTerminalState(requestJob))
			{
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Valid);
				requestJob.ValidationMessage = LocalizedString.Empty;
				return;
			}
			if (!requestJob.ValidateUser(requestJob.User, requestJob.UserId))
			{
				return;
			}
			Guid guid;
			Guid guid2;
			RequestIndexEntryProvider.GetMoveGuids(requestJob.User, out guid, out guid2);
			if (guid != requestJob.ExchangeGuid)
			{
				MrsTracer.Common.Error("Orphaned RequestJob: mailbox guid does not match between AD {0} and workitem queue {1}.", new object[]
				{
					requestJob.User.ExchangeGuid,
					requestJob.ExchangeGuid
				});
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
				requestJob.ValidationMessage = MrsStrings.ValidationMailboxGuidsDontMatch(guid, requestJob.ExchangeGuid);
				return;
			}
			if (!MailboxRelocationRequestStatistics.ValidateNoOtherRequests(requestJob))
			{
				return;
			}
			if (CommonUtils.IsImplicitSplit(requestJob.Flags, requestJob.User))
			{
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
				requestJob.ValidationMessage = MrsStrings.ErrorImplicitSplit;
				return;
			}
			requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Valid);
			requestJob.ValidationMessage = LocalizedString.Empty;
		}

		private static void LoadAdditionalPropertiesFromUser(RequestJobBase requestJob)
		{
			if (requestJob.User != null)
			{
				requestJob.Alias = requestJob.User.Alias;
				requestJob.DisplayName = requestJob.User.DisplayName;
				requestJob.RecipientTypeDetails = requestJob.User.RecipientTypeDetails;
				requestJob.UserId = requestJob.User.Id;
			}
		}

		private static bool ValidateNoOtherRequests(RequestJobBase requestJobBase)
		{
			IEnumerable<RequestIndexId> source = from i in requestJobBase.IndexEntries
			select i.RequestIndexId into i
			where i.Location == RequestIndexLocation.Mailbox
			select i;
			if (source.Any((RequestIndexId i) => i.Mailbox.Equals(requestJobBase.UserId)))
			{
				string otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests(requestJobBase.User, new Guid?(requestJobBase.RequestGuid));
				if (!string.IsNullOrEmpty(otherRequests))
				{
					requestJobBase.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJobBase.ValidationMessage = MrsStrings.ValidationObjectInvolvedInMultipleRelocations(MrsStrings.Mailbox, otherRequests);
					return false;
				}
			}
			if (requestJobBase.User.UnifiedMailbox != null)
			{
				ADRecipient tempRecipient;
				if (ADRecipient.TryGetFromCrossTenantObjectId(requestJobBase.User.UnifiedMailbox, out tempRecipient).Succeeded && source.Any((RequestIndexId i) => i.Mailbox.Equals(tempRecipient.Id)))
				{
					string otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests((ADUser)tempRecipient, new Guid?(requestJobBase.RequestGuid));
					if (!string.IsNullOrEmpty(otherRequests))
					{
						requestJobBase.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
						requestJobBase.ValidationMessage = MrsStrings.ValidationObjectInvolvedInMultipleRelocations(MrsStrings.SourceContainer, otherRequests);
						return false;
					}
				}
			}
			if (requestJobBase.TargetUnifiedMailboxId != null)
			{
				ADRecipient tempRecipient;
				if (ADRecipient.TryGetFromCrossTenantObjectId(requestJobBase.TargetUnifiedMailboxId, out tempRecipient).Succeeded && source.Any((RequestIndexId i) => i.Mailbox.Equals(tempRecipient.Id)))
				{
					string otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests((ADUser)tempRecipient, new Guid?(requestJobBase.RequestGuid));
					if (!string.IsNullOrEmpty(otherRequests))
					{
						requestJobBase.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
						requestJobBase.ValidationMessage = MrsStrings.ValidationObjectInvolvedInMultipleRelocations(MrsStrings.TargetContainer, otherRequests);
						return false;
					}
				}
			}
			return true;
		}
	}
}
