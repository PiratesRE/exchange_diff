using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SuspendRequest<TIdentity> : SetRequestBase<TIdentity> where TIdentity : MRSRequestIdParameter
	{
		public SuspendRequest()
		{
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNull]
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

		[Parameter(Mandatory = false)]
		public string SuspendComment
		{
			get
			{
				return (string)base.Fields["SuspendComment"];
			}
			set
			{
				base.Fields["SuspendComment"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSuspendRequest(base.RequestName);
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			base.ValidateRequest(requestJob);
			base.ValidateRequestIsActive(requestJob);
			base.ValidateRequestProtectionStatus(requestJob);
			base.ValidateRequestIsRunnable(requestJob);
			base.ValidateRequestIsNotCancelled(requestJob);
			if (requestJob.RequestJobState == JobProcessingState.InProgress && RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.Completion) && !RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.IncrementalSync) && requestJob.IdleTime < TimeSpan.FromMinutes(60.0))
			{
				base.WriteError(new CannotModifyCompletingRequestPermanentException(base.RequestName), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (requestJob.Suspend)
			{
				base.WriteVerbose(Strings.RequestAlreadySuspended(base.RequestName));
			}
			if (!string.IsNullOrEmpty(this.SuspendComment) && this.SuspendComment.Length > 4096)
			{
				base.WriteError(new ParameterLengthExceededPermanentException("SuspendComment", 4096), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			this.mdbGuid = requestJob.WorkItemQueueMdb.ObjectGuid;
			if (requestJob.TargetUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.TargetUser.OriginatingServer;
			}
			else if (requestJob.SourceUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.SourceUser.OriginatingServer;
			}
			if (!requestJob.Suspend)
			{
				requestJob.Suspend = true;
				if (!string.IsNullOrEmpty(this.SuspendComment))
				{
					requestJob.Message = MrsStrings.MoveRequestMessageInformational(new LocalizedString(this.SuspendComment));
				}
				ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(MrsStrings.ReportRequestSuspended(base.ExecutingUserIdentity), connectivityRec);
				reportData.Flush(base.RJProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			RequestTaskHelper.TickleMRS(this.DataObject, MoveRequestNotification.SuspendResume, this.mdbGuid, base.ConfigSession, base.UnreachableMrsServers);
			base.WriteVerbose(Strings.SuspendSuccessInformationalMessage(base.RequestName));
		}

		public const string ParameterSuspendComment = "SuspendComment";

		private Guid mdbGuid;
	}
}
