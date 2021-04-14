using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "MoveRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendMoveRequest : SetMoveRequestBase
	{
		public SuspendMoveRequest()
		{
			this.moveIsOffline = false;
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
				if (this.moveIsOffline)
				{
					return Strings.ConfirmationMessageSuspendOfflineMoveRequest(base.LocalADUser.ToString());
				}
				return Strings.ConfirmationMessageSuspendOnlineMoveRequest(base.LocalADUser.ToString());
			}
		}

		protected override void ValidateMoveRequest(TransactionalRequestJob moveRequest)
		{
			this.moveIsOffline = moveRequest.IsOffline;
			base.ValidateMoveRequestIsActive(moveRequest);
			base.ValidateMoveRequestProtectionStatus(moveRequest);
			base.ValidateMoveRequestIsSettable(moveRequest);
			if (moveRequest.RequestJobState == JobProcessingState.InProgress && RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Cleanup) && moveRequest.IdleTime < TimeSpan.FromMinutes(60.0))
			{
				base.WriteError(new CannotModifyCompletingRequestPermanentException(base.LocalADUser.ToString()), ErrorCategory.InvalidArgument, base.Identity);
			}
			if (moveRequest.Suspend)
			{
				base.WriteVerbose(Strings.MoveAlreadySuspended(base.LocalADUser.ToString()));
			}
		}

		protected override void ModifyMoveRequest(TransactionalRequestJob moveRequest)
		{
			this.mdbGuid = moveRequest.WorkItemQueueMdb.ObjectGuid;
			if (base.LocalADUser != null)
			{
				moveRequest.DomainControllerToUpdate = base.LocalADUser.OriginatingServer;
			}
			moveRequest.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(DateTime.MaxValue));
			if (!moveRequest.Suspend)
			{
				moveRequest.Suspend = true;
				if (!string.IsNullOrEmpty(this.SuspendComment))
				{
					moveRequest.Message = MrsStrings.MoveRequestMessageInformational(new LocalizedString(this.SuspendComment));
				}
				ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(MrsStrings.ReportMoveRequestSuspended(base.ExecutingUserIdentity), connectivityRec);
				reportData.Flush(base.MRProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			using (MailboxReplicationServiceClient mailboxReplicationServiceClient = this.DataObject.CreateMRSClient(base.ConfigSession, this.mdbGuid, base.UnreachableMrsServers))
			{
				mailboxReplicationServiceClient.RefreshMoveRequest(base.LocalADUser.ExchangeGuid, this.mdbGuid, MoveRequestNotification.SuspendResume);
			}
			base.WriteVerbose(Strings.SuspendSuccessInformationalMessage(base.LocalADUser.ToString()));
		}

		public const string ParameterSuspendComment = "SuspendComment";

		private Guid mdbGuid;

		private bool moveIsOffline;
	}
}
