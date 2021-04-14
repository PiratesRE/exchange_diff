using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMoveRequest : SetMoveRequestBase
	{
		[Parameter(Mandatory = false)]
		public bool SuspendWhenReadyToComplete
		{
			get
			{
				return (bool)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Fqdn RemoteGlobalCatalog
		{
			get
			{
				return (Fqdn)base.Fields["RemoteGlobalCatalog"];
			}
			set
			{
				base.Fields["RemoteGlobalCatalog"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["BadItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["BadItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["LargeItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["LargeItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AcceptLargeDataLoss
		{
			get
			{
				return (SwitchParameter)(base.Fields["AcceptLargeDataLoss"] ?? false);
			}
			set
			{
				base.Fields["AcceptLargeDataLoss"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Fqdn RemoteHostName
		{
			get
			{
				return (Fqdn)base.Fields["RemoteHostName"];
			}
			set
			{
				base.Fields["RemoteHostName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public PSCredential RemoteCredential
		{
			get
			{
				return (PSCredential)base.Fields["RemoteCredential"];
			}
			set
			{
				base.Fields["RemoteCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Protect
		{
			get
			{
				return (bool)(base.Fields["Protect"] ?? false);
			}
			set
			{
				base.Fields["Protect"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IgnoreRuleLimitErrors
		{
			get
			{
				return (bool)(base.Fields["IgnoreRuleLimitErrors"] ?? false);
			}
			set
			{
				base.Fields["IgnoreRuleLimitErrors"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BatchName
		{
			get
			{
				return (string)base.Fields["BatchName"];
			}
			set
			{
				base.Fields["BatchName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RequestPriority Priority
		{
			get
			{
				return (RequestPriority)(base.Fields["Priority"] ?? RequestPriority.Normal);
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)(base.Fields["CompletedRequestAgeLimit"] ?? RequestTaskHelper.DefaultCompletedRequestAgeLimit);
			}
			set
			{
				base.Fields["CompletedRequestAgeLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PreventCompletion
		{
			get
			{
				return (bool)(base.Fields["PreventCompletion"] ?? false);
			}
			set
			{
				base.Fields["PreventCompletion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SkippableMoveComponent[] SkipMoving
		{
			get
			{
				return (SkippableMoveComponent[])(base.Fields["SkipMoving"] ?? null);
			}
			set
			{
				base.Fields["SkipMoving"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InternalMrsFlag[] InternalFlags
		{
			get
			{
				return (InternalMrsFlag[])(base.Fields["InternalFlags"] ?? null);
			}
			set
			{
				base.Fields["InternalFlags"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartAfter
		{
			get
			{
				return (DateTime?)base.Fields["StartAfter"];
			}
			set
			{
				base.Fields["StartAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? CompleteAfter
		{
			get
			{
				return (DateTime?)base.Fields["CompleteAfter"];
			}
			set
			{
				base.Fields["CompleteAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)base.Fields["IncrementalSyncInterval"];
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public DatabaseIdParameter TargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["TargetDatabase"];
			}
			set
			{
				base.Fields["TargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public DatabaseIdParameter ArchiveTargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["ArchiveTargetDatabase"];
			}
			set
			{
				base.Fields["ArchiveTargetDatabase"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRequest(base.LocalADUser.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			RequestTaskHelper.ValidateItemLimits(this.BadItemLimit, this.LargeItemLimit, this.AcceptLargeDataLoss, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), base.ExecutingUserIdentity);
			if (this.BatchName != null && this.BatchName.Length > 255)
			{
				base.WriteError(new ParameterLengthExceededPermanentException("BatchName", 255), ErrorCategory.InvalidArgument, this.BatchName);
			}
			if (this.TargetDatabase != null)
			{
				this.specifiedTargetMDB = this.LocateAndVerifyMdb(this.TargetDatabase, out this.newTargetServerVersion);
			}
			if (this.ArchiveTargetDatabase != null)
			{
				this.specifiedArchiveTargetMDB = this.LocateAndVerifyMdb(this.ArchiveTargetDatabase, out this.newArchiveTargetServerVersion);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.InternalValidate();
				TransactionalRequestJob dataObject = this.DataObject;
				if (base.IsFieldSet("TargetDatabase"))
				{
					ServerVersion serverVersion = new ServerVersion(MapiUtils.FindServerForMdb(dataObject.TargetDatabase.ObjectGuid, null, null, FindServerFlags.None).ServerVersion);
					if (this.newTargetServerVersion.Major != serverVersion.Major || this.newTargetServerVersion.Minor != serverVersion.Minor)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotRetargetToDifferentVersionServerThanOriginal(this.newTargetServerVersion.ToString(), serverVersion.ToString())), ErrorCategory.InvalidArgument, base.Identity);
					}
				}
				if (base.IsFieldSet("ArchiveTargetDatabase"))
				{
					ServerVersion serverVersion2 = new ServerVersion(MapiUtils.FindServerForMdb(dataObject.TargetArchiveDatabase.ObjectGuid, null, null, FindServerFlags.None).ServerVersion);
					if (this.newArchiveTargetServerVersion.Major != serverVersion2.Major || this.newArchiveTargetServerVersion.Minor != serverVersion2.Minor)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotRetargetToDifferentVersionArchiveServerThanOriginal(this.newArchiveTargetServerVersion.ToString(), serverVersion2.ToString())), ErrorCategory.InvalidArgument, base.Identity);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void ValidateMoveRequest(TransactionalRequestJob moveRequest)
		{
			base.ValidateMoveRequestIsActive(moveRequest);
			base.ValidateMoveRequestProtectionStatus(moveRequest);
			base.ValidateMoveRequestIsSettable(moveRequest);
			if (base.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				if (moveRequest.IsOffline)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorNoOfflineSuspendWhenReadyToComplete(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
				if (RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Completion))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotAutoSuspendMoveAlreadyCompleting(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
			}
			if (base.IsFieldSet("RemoteCredential") && moveRequest.RequestStyle == RequestStyle.IntraOrg)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNoRemoteCredentialSettingForLocalMove(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (base.IsFieldSet("RemoteGlobalCatalog") && moveRequest.RequestStyle == RequestStyle.IntraOrg)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNoRemoteGlobalCatalogSettingForLocalMove(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (base.IsFieldSet("RemoteHostName") && moveRequest.RequestStyle == RequestStyle.IntraOrg)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNoRemoteHostNameSettingForLocalMove(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (base.IsFieldSet("BadItemLimit") && this.BadItemLimit < new Unlimited<int>(moveRequest.BadItemsEncountered))
			{
				base.WriteError(new BadItemLimitAlreadyExceededPermanentException(moveRequest.Name, moveRequest.BadItemsEncountered, this.BadItemLimit.ToString()), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (base.IsFieldSet("LargeItemLimit") && moveRequest.AllowLargeItems)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorIncompatibleParameters("AllowLargeItems", "LargeItemLimit")), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (base.IsFieldSet("LargeItemLimit") && this.LargeItemLimit < new Unlimited<int>(moveRequest.LargeItemsEncountered))
			{
				base.WriteError(new LargeItemLimitAlreadyExceededPermanentException(moveRequest.Name, moveRequest.LargeItemsEncountered, this.LargeItemLimit.ToString()), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (this.PreventCompletion)
			{
				if (moveRequest.IsOffline)
				{
					base.WriteError(new CannotPreventCompletionForOfflineMovePermanentException(), ErrorCategory.InvalidArgument, this.PreventCompletion);
				}
				if (RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Completion))
				{
					base.WriteError(new CannotPreventCompletionForCompletingMovePermanentException(), ErrorCategory.InvalidArgument, this.PreventCompletion);
				}
				if (moveRequest.JobType >= MRSJobType.RequestJobE15_AutoResume)
				{
					base.WriteError(new SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException(), ErrorCategory.InvalidArgument, this.PreventCompletion);
				}
			}
			DateTime? timestamp = moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
			bool flag = RequestTaskHelper.CompareUtcTimeWithLocalTime(timestamp, this.StartAfter);
			DateTime? timestamp2 = moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter);
			bool flag2 = RequestTaskHelper.CompareUtcTimeWithLocalTime(timestamp2, this.CompleteAfter);
			bool flag3 = base.IsFieldSet("StartAfter") && !flag;
			bool flag4 = base.IsFieldSet("CompleteAfter") && !flag2;
			if (flag3 || flag4 || base.IsFieldSet("IncrementalSyncInterval"))
			{
				this.CheckMoveStatusWithStartAfterAndCompleteAfterIncrementalSyncInterval(moveRequest);
			}
			if (flag3)
			{
				this.CheckMoveStatusInQueuedForStartAfterSet(moveRequest);
			}
			bool flag5 = base.IsFieldSet("SuspendWhenReadyToComplete") ? this.SuspendWhenReadyToComplete : moveRequest.SuspendWhenReadyToComplete;
			if (flag5 && moveRequest.JobType >= MRSJobType.RequestJobE15_AutoResume)
			{
				base.WriteError(new SuspendWRTCPreventCompletionCannotBeSetOnNewRequestsException(), ErrorCategory.InvalidArgument, this.SuspendWhenReadyToComplete);
			}
			DateTime utcNow = DateTime.UtcNow;
			if (flag3 && this.StartAfter != null)
			{
				RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), utcNow);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.IsFieldSet("StartAfter") && flag)
			{
				this.WriteWarning(Strings.WarningScheduledTimeIsUnchanged("StartAfter"));
			}
			if (base.IsFieldSet("CompleteAfter") && flag2)
			{
				this.WriteWarning(Strings.WarningScheduledTimeIsUnchanged("CompleteAfter"));
			}
			if (base.IsFieldSet("TargetDatabase") || base.IsFieldSet("ArchiveTargetDatabase"))
			{
				if (!moveRequest.TargetIsLocal)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotRetargetOutboundMoves(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
				if (!RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Queued))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCanRetargetOnlyQueuedMoves(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
				if (base.IsFieldSet("TargetDatabase") && moveRequest.ArchiveOnly)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotRetargetPrimaryForArchiveOnlyMoves(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
				if (base.IsFieldSet("ArchiveTargetDatabase") && moveRequest.PrimaryOnly)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotRetargetArchiveForPrimaryOnlyMoves(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
				}
			}
		}

		private void CheckMoveStatusInQueuedForStartAfterSet(TransactionalRequestJob moveRequest)
		{
			if (!RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Queued))
			{
				base.WriteError(new ErrorStartAfterCanBeSetOnlyInQueuedException(), ErrorCategory.InvalidArgument, base.Identity);
			}
		}

		private void CheckMoveStatusWithStartAfterAndCompleteAfterIncrementalSyncInterval(TransactionalRequestJob moveRequest)
		{
			if (moveRequest.IsOffline)
			{
				base.WriteError(new StartAfterOrCompleteAfterCannotBeSetForOfflineMovesException(), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Completion) && !RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.IncrementalSync))
			{
				base.WriteError(new StartAfterOrCompleteAfterCannotBeSetWhenJobCompletingException(), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (moveRequest.JobType < MRSJobType.RequestJobE15_AutoResume)
			{
				base.WriteError(new StartAfterOrCompleteAfterCannotBeSetOnLegacyRequestsException(), ErrorCategory.InvalidArgument, base.Identity);
			}
		}

		protected override void ModifyMoveRequest(TransactionalRequestJob moveRequest)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Set-MoveRequest changed values:");
			this.mdbGuid = moveRequest.WorkItemQueueMdb.ObjectGuid;
			if (base.LocalADUser != null)
			{
				moveRequest.DomainControllerToUpdate = base.LocalADUser.OriginatingServer;
			}
			if (base.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				stringBuilder.AppendLine(string.Format("SWRTC: {0} -> {1}", moveRequest.SuspendWhenReadyToComplete, this.SuspendWhenReadyToComplete));
				moveRequest.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
			}
			if (base.IsFieldSet("RemoteCredential"))
			{
				string remoteCredentialUsername = moveRequest.RemoteCredentialUsername;
				moveRequest.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, null);
				stringBuilder.AppendLine(string.Format("RemoteCredential: {0}:<pwd> -> {1}:<pwd>", remoteCredentialUsername, moveRequest.RemoteCredentialUsername));
				if ((moveRequest.Flags & RequestFlags.RemoteLegacy) != RequestFlags.None)
				{
					if (moveRequest.Direction == RequestDirection.Pull)
					{
						moveRequest.SourceCredential = moveRequest.RemoteCredential;
					}
					else
					{
						moveRequest.TargetCredential = moveRequest.RemoteCredential;
					}
				}
			}
			if (base.IsFieldSet("RemoteGlobalCatalog"))
			{
				string arg;
				if ((moveRequest.Flags & RequestFlags.RemoteLegacy) != RequestFlags.None)
				{
					if (moveRequest.Direction == RequestDirection.Pull)
					{
						arg = moveRequest.SourceDCName;
						moveRequest.SourceDCName = this.RemoteGlobalCatalog;
					}
					else
					{
						arg = moveRequest.TargetDCName;
						moveRequest.TargetDCName = this.RemoteGlobalCatalog;
					}
				}
				else
				{
					arg = moveRequest.RemoteDomainControllerToUpdate;
					moveRequest.RemoteDomainControllerToUpdate = this.RemoteGlobalCatalog;
				}
				stringBuilder.AppendLine(string.Format("RemoteGC: {0} -> {1}", arg, this.RemoteGlobalCatalog));
			}
			if (base.IsFieldSet("RemoteHostName"))
			{
				stringBuilder.AppendLine(string.Format("RemoteHostName: {0} -> {1}", moveRequest.RemoteHostName, this.RemoteHostName));
				moveRequest.RemoteHostName = this.RemoteHostName;
			}
			if (base.IsFieldSet("BadItemLimit"))
			{
				stringBuilder.AppendLine(string.Format("BadItemLimit: {0} -> {1}", moveRequest.BadItemLimit, this.BadItemLimit));
				moveRequest.BadItemLimit = this.BadItemLimit;
			}
			if (base.IsFieldSet("LargeItemLimit"))
			{
				stringBuilder.AppendLine(string.Format("LargeItemLimit: {0} -> {1}", moveRequest.LargeItemLimit, this.LargeItemLimit));
				moveRequest.LargeItemLimit = this.LargeItemLimit;
			}
			if (base.IsFieldSet("Protect"))
			{
				stringBuilder.AppendLine(string.Format("Protect: {0} -> {1}", moveRequest.Protect, this.Protect));
				moveRequest.Protect = this.Protect;
			}
			if (base.IsFieldSet("IgnoreRuleLimitErrors"))
			{
				stringBuilder.AppendLine(string.Format("IgnoreRuleLimitErrors: {0} -> {1}", moveRequest.IgnoreRuleLimitErrors, this.IgnoreRuleLimitErrors));
				moveRequest.IgnoreRuleLimitErrors = this.IgnoreRuleLimitErrors;
			}
			if (base.IsFieldSet("BatchName"))
			{
				stringBuilder.AppendLine(string.Format("BatchName: {0} -> {1}", moveRequest.BatchName, this.BatchName));
				moveRequest.BatchName = this.BatchName;
			}
			if (base.IsFieldSet("Priority"))
			{
				stringBuilder.AppendLine(string.Format("Priority: {0} -> {1}", moveRequest.Priority, this.Priority));
				moveRequest.Priority = this.Priority;
			}
			if (base.IsFieldSet("CompletedRequestAgeLimit"))
			{
				stringBuilder.AppendLine(string.Format("CompletedRequestAgeLimit: {0} -> {1}", moveRequest.CompletedRequestAgeLimit, this.CompletedRequestAgeLimit));
				moveRequest.CompletedRequestAgeLimit = this.CompletedRequestAgeLimit;
			}
			if (base.IsFieldSet("PreventCompletion"))
			{
				stringBuilder.AppendLine(string.Format("PreventCompletion: {0} -> {1}", moveRequest.PreventCompletion, this.PreventCompletion));
				moveRequest.PreventCompletion = this.PreventCompletion;
			}
			if (base.IsFieldSet("StartAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), this.StartAfter))
			{
				RequestTaskHelper.SetStartAfter(this.StartAfter, moveRequest, stringBuilder);
			}
			if (base.IsFieldSet("CompleteAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), this.CompleteAfter))
			{
				RequestTaskHelper.SetCompleteAfter(this.CompleteAfter, moveRequest, stringBuilder);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				moveRequest.IncrementalSyncInterval = this.IncrementalSyncInterval;
			}
			RequestTaskHelper.ValidateStartAfterCompleteAfterWithSuspendWhenReadyToComplete(moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), moveRequest.SuspendWhenReadyToComplete, new Task.TaskErrorLoggingDelegate(base.WriteError));
			RequestTaskHelper.ValidateStartAfterComesBeforeCompleteAfter(moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (base.IsFieldSet("SkipMoving"))
			{
				RequestJobInternalFlags requestJobInternalFlags = moveRequest.RequestJobInternalFlags;
				RequestTaskHelper.SetSkipMoving(this.SkipMoving, moveRequest, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
				stringBuilder.AppendLine(string.Format("InternalFlags: {0} -> {1}", requestJobInternalFlags, moveRequest.RequestJobInternalFlags));
			}
			if (base.IsFieldSet("InternalFlags"))
			{
				RequestJobInternalFlags requestJobInternalFlags2 = moveRequest.RequestJobInternalFlags;
				RequestTaskHelper.SetInternalFlags(this.InternalFlags, moveRequest, new Task.TaskErrorLoggingDelegate(base.WriteError));
				stringBuilder.AppendLine(string.Format("InternalFlags: {0} -> {1}", requestJobInternalFlags2, moveRequest.RequestJobInternalFlags));
			}
			ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
			ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
			reportData.Append(MrsStrings.ReportMoveRequestSet(base.ExecutingUserIdentity), connectivityRec);
			reportData.AppendDebug(stringBuilder.ToString());
			if (this.AcceptLargeDataLoss)
			{
				reportData.Append(MrsStrings.ReportLargeAmountOfDataLossAccepted2(moveRequest.BadItemLimit.ToString(), moveRequest.LargeItemLimit.ToString(), base.ExecutingUserIdentity));
			}
			if (base.IsFieldSet("TargetDatabase") || base.IsFieldSet("ArchiveTargetDatabase"))
			{
				moveRequest.RehomeRequest = true;
				if (base.IsFieldSet("TargetDatabase"))
				{
					moveRequest.TargetDatabase = this.specifiedTargetMDB.Id;
				}
				if (base.IsFieldSet("ArchiveTargetDatabase"))
				{
					moveRequest.TargetArchiveDatabase = this.specifiedArchiveTargetMDB.Id;
				}
			}
			reportData.Flush(base.MRProvider.SystemMailbox);
		}

		protected override void PostSaveAction()
		{
			using (MailboxReplicationServiceClient mailboxReplicationServiceClient = this.DataObject.CreateMRSClient(base.ConfigSession, this.mdbGuid, base.UnreachableMrsServers))
			{
				mailboxReplicationServiceClient.RefreshMoveRequest(base.LocalADUser.ExchangeGuid, this.mdbGuid, MoveRequestNotification.Updated);
			}
		}

		private MailboxDatabase LocateAndVerifyMdb(DatabaseIdParameter databaseId, out ServerVersion targetServerVersion)
		{
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseId, base.ConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(databaseId.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(databaseId.ToString())));
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mailboxDatabase.Id.ObjectGuid, null, null, FindServerFlags.None);
			targetServerVersion = new ServerVersion(databaseInformation.ServerVersion);
			this.EnsureSupportedServerVersion(databaseInformation.ServerVersion);
			return mailboxDatabase;
		}

		private void EnsureSupportedServerVersion(int serverVersion)
		{
			if (serverVersion < Server.E15MinVersion)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMovingToOldExchangeDatabaseUnsupported), ErrorCategory.InvalidArgument, this.TargetDatabase);
			}
		}

		private Guid mdbGuid;

		private MailboxDatabase specifiedTargetMDB;

		private MailboxDatabase specifiedArchiveTargetMDB;

		private ServerVersion newTargetServerVersion;

		private ServerVersion newArchiveTargetServerVersion;
	}
}
