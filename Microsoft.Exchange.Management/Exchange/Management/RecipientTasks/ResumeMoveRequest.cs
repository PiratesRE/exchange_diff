using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "MoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumeMoveRequest : SetMoveRequestBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter SuspendWhenReadyToComplete
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageResumeRequest(base.LocalADUser.ToString());
			}
		}

		protected override void ValidateMoveRequest(TransactionalRequestJob moveRequest)
		{
			base.ValidateMoveRequestIsActive(moveRequest);
			base.ValidateMoveRequestProtectionStatus(moveRequest);
			base.ValidateMoveRequestIsSettable(moveRequest);
			if (!moveRequest.Suspend)
			{
				base.WriteVerbose(Strings.MoveNotSuspended(base.LocalADUser.ToString()));
			}
			if (RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Cleanup))
			{
				base.WriteError(new SuspendWhenReadyToCompleteCannotBeUsedDuringCompletionException(moveRequest.Identity.ToString()), ErrorCategory.InvalidArgument, moveRequest.Identity);
			}
			if (moveRequest.JobType >= MRSJobType.RequestJobE15_AutoResume && this.SuspendWhenReadyToComplete)
			{
				base.WriteError(new SuspendWhenReadyToCompleteCannotBeUsedOnAutoResumeJobsException(moveRequest.Identity.ToString()), ErrorCategory.InvalidArgument, moveRequest.Identity);
			}
		}

		protected override void ModifyMoveRequest(TransactionalRequestJob moveRequest)
		{
			this.mdbGuid = moveRequest.WorkItemQueueMdb.ObjectGuid;
			if (base.LocalADUser != null)
			{
				moveRequest.DomainControllerToUpdate = base.LocalADUser.OriginatingServer;
			}
			moveRequest.PoisonCount = 0;
			if (moveRequest.Suspend)
			{
				moveRequest.Suspend = false;
				moveRequest.Message = LocalizedString.Empty;
				DateTime? timestamp = moveRequest.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
				moveRequest.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, timestamp);
				moveRequest.TotalRetryCount = 0;
				LocalizedString msg;
				if (this.SuspendWhenReadyToComplete)
				{
					moveRequest.SuspendWhenReadyToComplete = true;
					msg = MrsStrings.ReportRequestResumedWithSuspendWhenReadyToComplete(base.ExecutingUserIdentity);
				}
				else
				{
					msg = MrsStrings.ReportMoveRequestResumed(base.ExecutingUserIdentity);
				}
				ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(msg, connectivityRec);
				reportData.Flush(base.MRProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			using (MailboxReplicationServiceClient mailboxReplicationServiceClient = this.DataObject.CreateMRSClient(base.ConfigSession, this.mdbGuid, base.UnreachableMrsServers))
			{
				mailboxReplicationServiceClient.RefreshMoveRequest(base.LocalADUser.ExchangeGuid, this.mdbGuid, MoveRequestNotification.SuspendResume);
			}
			base.WriteVerbose(Strings.ResumeSuccessInformationalMessage(base.LocalADUser.ToString()));
		}

		private Guid mdbGuid;
	}
}
