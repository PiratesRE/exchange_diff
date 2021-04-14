using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MoveRequestStatistics : RequestStatisticsBase
	{
		public MoveRequestStatistics()
		{
		}

		internal MoveRequestStatistics(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
		}

		internal MoveRequestStatistics(TransactionalRequestJob requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
		{
			base.CopyNonSchematizedPropertiesFrom(requestJob);
		}

		internal MoveRequestStatistics(RequestJobXML requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
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

		public new Guid? ArchiveGuid
		{
			get
			{
				return base.ArchiveGuid;
			}
			internal set
			{
				base.ArchiveGuid = value;
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

		public new bool IsOffline
		{
			get
			{
				return base.IsOffline;
			}
			internal set
			{
				base.IsOffline = value;
			}
		}

		public new bool Protect
		{
			get
			{
				return base.Protect;
			}
			internal set
			{
				base.Protect = value;
			}
		}

		public bool DoNotPreserveMailboxSignature
		{
			get
			{
				return !base.PreserveMailboxSignature;
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

		public new bool SuspendWhenReadyToComplete
		{
			get
			{
				return base.SuspendWhenReadyToComplete;
			}
			internal set
			{
				base.SuspendWhenReadyToComplete = value;
			}
		}

		public new bool IgnoreRuleLimitErrors
		{
			get
			{
				return base.IgnoreRuleLimitErrors;
			}
			internal set
			{
				base.IgnoreRuleLimitErrors = value;
			}
		}

		public new RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return base.RecipientTypeDetails;
			}
			internal set
			{
				base.RecipientTypeDetails = value;
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

		public new ADObjectId SourceArchiveDatabase
		{
			get
			{
				return base.SourceArchiveDatabase;
			}
			internal set
			{
				base.SourceArchiveDatabase = value;
			}
		}

		public new ServerVersion SourceArchiveVersion
		{
			get
			{
				if (base.SourceArchiveVersion == 0)
				{
					return null;
				}
				return new ServerVersion(base.SourceArchiveVersion);
			}
			internal set
			{
				base.SourceArchiveVersion = ((value != null) ? value.ToInt() : 0);
			}
		}

		public new string SourceArchiveServer
		{
			get
			{
				return base.SourceArchiveServer;
			}
			internal set
			{
				base.SourceArchiveServer = value;
			}
		}

		public new ADObjectId TargetArchiveDatabase
		{
			get
			{
				return base.TargetArchiveDatabase;
			}
			internal set
			{
				base.TargetArchiveDatabase = value;
			}
		}

		public new ServerVersion TargetArchiveVersion
		{
			get
			{
				if (base.TargetArchiveVersion == 0)
				{
					return null;
				}
				return new ServerVersion(base.TargetArchiveVersion);
			}
			internal set
			{
				base.TargetArchiveVersion = ((value != null) ? value.ToInt() : 0);
			}
		}

		public new string TargetArchiveServer
		{
			get
			{
				return base.TargetArchiveServer;
			}
			internal set
			{
				base.TargetArchiveServer = value;
			}
		}

		public new string RemoteHostName
		{
			get
			{
				return base.RemoteHostName;
			}
			internal set
			{
				base.RemoteHostName = value;
			}
		}

		public new string RemoteGlobalCatalog
		{
			get
			{
				return base.RemoteGlobalCatalog;
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

		public DateTime? StartAfter
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.StartAfter);
			}
		}

		public DateTime? CompleteAfter
		{
			get
			{
				return base.TimeTracker.GetDisplayTimestamp(RequestJobTimestamp.CompleteAfter);
			}
		}

		public new string RemoteCredentialUsername
		{
			get
			{
				return base.RemoteCredentialUsername;
			}
			internal set
			{
				base.RemoteCredentialUsername = value;
			}
		}

		public new string RemoteDatabaseName
		{
			get
			{
				return base.RemoteDatabaseName;
			}
			internal set
			{
				base.RemoteDatabaseName = value;
			}
		}

		public new Guid? RemoteDatabaseGuid
		{
			get
			{
				return base.RemoteDatabaseGuid;
			}
			internal set
			{
				base.RemoteDatabaseGuid = value;
			}
		}

		public new string RemoteArchiveDatabaseName
		{
			get
			{
				return base.RemoteArchiveDatabaseName;
			}
			internal set
			{
				base.RemoteArchiveDatabaseName = value;
			}
		}

		public new Guid? RemoteArchiveDatabaseGuid
		{
			get
			{
				return base.RemoteArchiveDatabaseGuid;
			}
			internal set
			{
				base.RemoteArchiveDatabaseGuid = value;
			}
		}

		public new string TargetDeliveryDomain
		{
			get
			{
				return base.TargetDeliveryDomain;
			}
			internal set
			{
				base.TargetDeliveryDomain = value;
			}
		}

		public new string ArchiveDomain
		{
			get
			{
				return base.ArchiveDomain;
			}
			internal set
			{
				base.ArchiveDomain = value;
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

		public new Unlimited<int> LargeItemLimit
		{
			get
			{
				return base.LargeItemLimit;
			}
			internal set
			{
				base.LargeItemLimit = value;
			}
		}

		public new int LargeItemsEncountered
		{
			get
			{
				return base.LargeItemsEncountered;
			}
			internal set
			{
				base.LargeItemsEncountered = value;
			}
		}

		public new bool AllowLargeItems
		{
			get
			{
				return base.AllowLargeItems;
			}
			internal set
			{
				base.AllowLargeItems = value;
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

		public new ByteQuantifiedSize? TotalArchiveSize
		{
			get
			{
				if (base.TotalArchiveSize != null)
				{
					return new ByteQuantifiedSize?(new ByteQuantifiedSize(base.TotalArchiveSize.Value));
				}
				return null;
			}
			internal set
			{
				if (value != null)
				{
					base.TotalArchiveSize = new ulong?(value.Value.ToBytes());
					return;
				}
				base.TotalArchiveSize = null;
			}
		}

		public new ulong? TotalArchiveItemCount
		{
			get
			{
				return base.TotalArchiveItemCount;
			}
			internal set
			{
				base.TotalArchiveItemCount = value;
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
				return base.UserId;
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

		public override string ToString()
		{
			string result;
			if ((result = this.Alias) == null)
			{
				if (this.MailboxIdentity != null)
				{
					return this.MailboxIdentity.ToString();
				}
				result = base.ToString();
			}
			return result;
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
			MoveRequestStatistics.LoadAdditionalPropertiesFromUser(requestJob);
			if (requestJob.CancelRequest || requestJob.Status == RequestStatus.Completed || requestJob.Status == RequestStatus.CompletedWithWarning)
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
			if (requestJob.User.MailboxMoveStatus == RequestStatus.None)
			{
				MrsTracer.Common.Warning("Orphaned RequestJob: AD user {0} is not being moved.", new object[]
				{
					requestJob.User.ToString()
				});
				requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.Orphaned);
				requestJob.ValidationMessage = MrsStrings.ValidationADUserIsNotBeingMoved;
				return;
			}
			if (requestJob.Flags != requestJob.User.MailboxMoveFlags)
			{
				if ((requestJob.Flags & RequestJobBase.StaticFlags) != (requestJob.User.MailboxMoveFlags & RequestJobBase.StaticFlags))
				{
					MrsTracer.Common.Error("Mismatched RequestJob: flags don't match: AD [{0}], workitem queue [{1}]", new object[]
					{
						requestJob.User.MailboxMoveFlags,
						requestJob.Flags
					});
					requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJob.ValidationMessage = MrsStrings.ValidationFlagsMismatch(requestJob.User.MailboxMoveFlags.ToString(), requestJob.Flags.ToString());
					return;
				}
				MrsTracer.Common.Debug("Possibly mismatched RequestJob: flags don't match: AD [{0}], workitem queue [{1}]", new object[]
				{
					requestJob.User.MailboxMoveFlags,
					requestJob.Flags
				});
			}
			if (requestJob.PrimaryIsMoving)
			{
				if (requestJob.SourceIsLocal && (requestJob.SourceDatabase == null || !requestJob.SourceDatabase.Equals(requestJob.User.MailboxMoveSourceMDB)))
				{
					MrsTracer.Common.Error("Mismatched RequestJob: Source database does not match between AD ({0}) and RequestJob ({1})", new object[]
					{
						requestJob.User.MailboxMoveSourceMDB,
						requestJob.SourceDatabase
					});
					requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJob.ValidationMessage = MrsStrings.ValidationSourceMDBMismatch((requestJob.User.MailboxMoveSourceMDB != null) ? requestJob.User.MailboxMoveSourceMDB.ToString() : "(null)", (requestJob.SourceDatabase != null) ? requestJob.SourceDatabase.ToString() : "(null)");
					return;
				}
				if (requestJob.TargetIsLocal && (requestJob.TargetDatabase == null || (!requestJob.RehomeRequest && !requestJob.TargetDatabase.Equals(requestJob.User.MailboxMoveTargetMDB))))
				{
					MrsTracer.Common.Error("Target database does not match between AD ({0}) and RequestJob ({1})", new object[]
					{
						requestJob.User.MailboxMoveTargetMDB,
						requestJob.TargetDatabase
					});
					requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJob.ValidationMessage = MrsStrings.ValidationTargetMDBMismatch((requestJob.User.MailboxMoveTargetMDB != null) ? requestJob.User.MailboxMoveTargetMDB.ToString() : "(null)", (requestJob.TargetDatabase != null) ? requestJob.TargetDatabase.ToString() : "(null)");
					return;
				}
			}
			if (requestJob.JobType >= MRSJobType.RequestJobE14R5_PrimaryOrArchiveExclusiveMoves && requestJob.ArchiveIsMoving)
			{
				if (requestJob.SourceIsLocal && (requestJob.SourceArchiveDatabase == null || !requestJob.SourceArchiveDatabase.Equals(requestJob.User.MailboxMoveSourceArchiveMDB)))
				{
					MrsTracer.Common.Error("Mismatched RequestJob: Source archive database does not match between AD ({0}) and RequestJob ({1})", new object[]
					{
						requestJob.User.MailboxMoveSourceArchiveMDB,
						requestJob.SourceArchiveDatabase
					});
					requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJob.ValidationMessage = MrsStrings.ValidationSourceArchiveMDBMismatch((requestJob.User.MailboxMoveSourceArchiveMDB != null) ? requestJob.User.MailboxMoveSourceArchiveMDB.ToString() : "(null)", (requestJob.SourceArchiveDatabase != null) ? requestJob.SourceArchiveDatabase.ToString() : "(null)");
					return;
				}
				if (requestJob.TargetIsLocal && (requestJob.TargetArchiveDatabase == null || (!requestJob.RehomeRequest && !requestJob.TargetArchiveDatabase.Equals(requestJob.User.MailboxMoveTargetArchiveMDB))))
				{
					MrsTracer.Common.Error("Target archive database does not match between AD ({0}) and RequestJob ({1})", new object[]
					{
						requestJob.User.MailboxMoveTargetArchiveMDB,
						requestJob.TargetArchiveDatabase
					});
					requestJob.ValidationResult = new RequestJobBase.ValidationResultEnum?(RequestJobBase.ValidationResultEnum.DataMismatch);
					requestJob.ValidationMessage = MrsStrings.ValidationTargetArchiveMDBMismatch((requestJob.User.MailboxMoveTargetArchiveMDB != null) ? requestJob.User.MailboxMoveTargetArchiveMDB.ToString() : "(null)", (requestJob.TargetArchiveDatabase != null) ? requestJob.TargetArchiveDatabase.ToString() : "(null)");
					return;
				}
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
	}
}
