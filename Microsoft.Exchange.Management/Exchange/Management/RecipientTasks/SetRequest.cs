using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetRequest<TIdentity> : SetRequestBase<TIdentity> where TIdentity : MRSRequestIdParameter
	{
		public SetRequest()
		{
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override TIdentity Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SkippableMergeComponent[] SkipMerging
		{
			get
			{
				return (SkippableMergeComponent[])(base.Fields["SkipMerging"] ?? null);
			}
			set
			{
				base.Fields["SkipMerging"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = true, ParameterSetName = "Rehome")]
		public SwitchParameter RehomeRequest
		{
			get
			{
				return (SwitchParameter)(base.Fields["RehomeRequest"] ?? false);
			}
			set
			{
				base.Fields["RehomeRequest"] = value;
			}
		}

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

		internal Guid MdbGuid { get; private set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRequest(base.RequestName);
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			base.ValidateRequest(requestJob);
			base.ValidateRequestIsActive(requestJob);
			base.ValidateRequestProtectionStatus(requestJob);
			base.ValidateRequestIsNotCancelled(requestJob);
			if (!base.ParameterSetName.Equals("Rehome"))
			{
				base.ValidateRequestIsRunnable(requestJob);
			}
			if (base.IsFieldSet("BadItemLimit") && this.BadItemLimit < new Unlimited<int>(requestJob.BadItemsEncountered))
			{
				base.WriteError(new BadItemLimitAlreadyExceededPermanentException(requestJob.Name, requestJob.BadItemsEncountered, this.BadItemLimit.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (base.IsFieldSet("LargeItemLimit") && this.LargeItemLimit < new Unlimited<int>(requestJob.LargeItemsEncountered))
			{
				base.WriteError(new LargeItemLimitAlreadyExceededPermanentException(requestJob.Name, requestJob.LargeItemsEncountered, this.LargeItemLimit.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.RehomeRequest && requestJob.RequestQueue != null && requestJob.RequestQueue.Equals(requestJob.OptimalRequestQueue))
			{
				base.WriteError(new RequestJobAlreadyOnProperQueuePermanentException(requestJob.Name, requestJob.RequestQueue.ObjectGuid.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		protected virtual void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			this.MdbGuid = requestJob.WorkItemQueueMdb.ObjectGuid;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SetRequest changed values:");
			if (requestJob.TargetUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.TargetUser.OriginatingServer;
			}
			else if (requestJob.SourceUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.SourceUser.OriginatingServer;
			}
			if (base.IsFieldSet("BadItemLimit"))
			{
				stringBuilder.AppendLine(string.Format("BadItemLimit: {0} -> {1}", requestJob.BadItemLimit, this.BadItemLimit));
				requestJob.BadItemLimit = this.BadItemLimit;
			}
			if (base.IsFieldSet("LargeItemLimit"))
			{
				stringBuilder.AppendLine(string.Format("LargeItemLimit: {0} -> {1}", requestJob.LargeItemLimit, this.LargeItemLimit));
				requestJob.LargeItemLimit = this.LargeItemLimit;
			}
			if (base.IsFieldSet("BatchName"))
			{
				stringBuilder.AppendLine(string.Format("BatchName: {0} -> {1}", requestJob.BatchName, this.BatchName));
				requestJob.BatchName = (this.BatchName ?? string.Empty);
			}
			if (base.IsFieldSet("Priority"))
			{
				stringBuilder.AppendLine(string.Format("Priority: {0} -> {1}", requestJob.Priority, this.Priority));
				requestJob.Priority = this.Priority;
			}
			if (base.IsFieldSet("CompletedRequestAgeLimit"))
			{
				stringBuilder.AppendLine(string.Format("CompletedRequestAgeLimit: {0} -> {1}", requestJob.CompletedRequestAgeLimit, this.CompletedRequestAgeLimit));
				requestJob.CompletedRequestAgeLimit = this.CompletedRequestAgeLimit;
			}
			if (this.RehomeRequest)
			{
				stringBuilder.AppendLine(string.Format("RehomeRequest: {0} -> {1}", requestJob.RehomeRequest, this.RehomeRequest));
				requestJob.RehomeRequest = this.RehomeRequest;
			}
			if (base.IsFieldSet("SkipMerging"))
			{
				RequestJobInternalFlags requestJobInternalFlags = requestJob.RequestJobInternalFlags;
				RequestTaskHelper.SetSkipMerging(this.SkipMerging, requestJob, new Task.TaskErrorLoggingDelegate(base.WriteError));
				stringBuilder.AppendLine(string.Format("InternalFlags: {0} -> {1}", requestJobInternalFlags, requestJob.RequestJobInternalFlags));
			}
			if (base.IsFieldSet("InternalFlags"))
			{
				RequestJobInternalFlags requestJobInternalFlags2 = requestJob.RequestJobInternalFlags;
				RequestTaskHelper.SetInternalFlags(this.InternalFlags, requestJob, new Task.TaskErrorLoggingDelegate(base.WriteError));
				stringBuilder.AppendLine(string.Format("InternalFlags: {0} -> {1}", requestJobInternalFlags2, requestJob.RequestJobInternalFlags));
			}
			if (base.IsFieldSet("RemoteHostName"))
			{
				stringBuilder.AppendLine(string.Format("RemoteHostName: {0} -> {1}", requestJob.RemoteHostName, this.RemoteHostName));
				requestJob.RemoteHostName = this.RemoteHostName;
			}
			if (base.IsFieldSet("RemoteCredential"))
			{
				stringBuilder.AppendLine(string.Format("RemoteCredential: * -> *", new object[0]));
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, new AuthenticationMethod?(AuthenticationMethod.WindowsIntegrated));
			}
			this.ModifyRequestInternal(requestJob, stringBuilder);
			ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
			ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
			reportData.Append(MrsStrings.ReportRequestSet(base.ExecutingUserIdentity), connectivityRec);
			reportData.AppendDebug(stringBuilder.ToString());
			if (this.AcceptLargeDataLoss)
			{
				reportData.Append(MrsStrings.ReportLargeAmountOfDataLossAccepted2(requestJob.BadItemLimit.ToString(), requestJob.LargeItemLimit.ToString(), base.ExecutingUserIdentity));
			}
			reportData.Flush(base.RJProvider.SystemMailbox);
		}

		protected override void PostSaveAction()
		{
			RequestTaskHelper.TickleMRS(this.DataObject, this.RehomeRequest ? MoveRequestNotification.SuspendResume : MoveRequestNotification.Updated, this.MdbGuid, base.ConfigSession, base.UnreachableMrsServers);
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			RequestTaskHelper.ValidateItemLimits(this.BadItemLimit, this.LargeItemLimit, this.AcceptLargeDataLoss, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), base.ExecutingUserIdentity);
			if (this.BatchName != null && this.BatchName.Length > 255)
			{
				base.WriteError(new ParameterLengthExceededPermanentException("BatchName", 255), ErrorCategory.InvalidArgument, this.BatchName);
			}
		}

		public const string ParameterSetRehome = "Rehome";
	}
}
