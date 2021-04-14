using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class ResumeRequest<TIdentity> : SetRequestBase<TIdentity> where TIdentity : MRSRequestIdParameter
	{
		public ResumeRequest()
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

		public virtual SwitchParameter SuspendWhenReadyToComplete
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
				return Strings.ConfirmationMessageResumeRequest(base.RequestName);
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			base.ValidateRequest(requestJob);
			base.ValidateRequestIsActive(requestJob);
			base.ValidateRequestProtectionStatus(requestJob);
			base.ValidateRequestIsRunnable(requestJob);
			base.ValidateRequestIsNotCancelled(requestJob);
			if (!requestJob.Suspend)
			{
				base.WriteVerbose(Strings.RequestNotSuspended(requestJob.Name));
			}
			if (RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.Completion) && !RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.IncrementalSync))
			{
				base.WriteError(new SuspendWhenReadyToCompleteCannotBeUsedDuringCompletionException(requestJob.Name), ErrorCategory.InvalidArgument, requestJob.Identity);
			}
			using (MailboxReplicationServiceClient mailboxReplicationServiceClient = requestJob.CreateMRSClient(base.ConfigSession, requestJob.WorkItemQueueMdb.ObjectGuid, base.UnreachableMrsServers))
			{
				LocalizedString message = requestJob.Message;
				requestJob.Message = LocalizedString.Empty;
				try
				{
					List<ReportEntry> entries = null;
					using (mailboxReplicationServiceClient.ValidateAndPopulateRequestJob(requestJob, out entries))
					{
						RequestTaskHelper.WriteReportEntries(requestJob.Name, entries, requestJob.Identity, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
				finally
				{
					requestJob.Message = message;
				}
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
			requestJob.PoisonCount = 0;
			if (requestJob.Suspend)
			{
				requestJob.Suspend = false;
				requestJob.Message = LocalizedString.Empty;
				requestJob.TotalRetryCount = 0;
				LocalizedString msg;
				if (this.SuspendWhenReadyToComplete)
				{
					requestJob.SuspendWhenReadyToComplete = true;
					msg = MrsStrings.ReportRequestResumedWithSuspendWhenReadyToComplete(base.ExecutingUserIdentity);
				}
				else
				{
					msg = MrsStrings.ReportRequestResumed(base.ExecutingUserIdentity);
				}
				ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(msg, connectivityRec);
				reportData.Flush(base.RJProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			RequestTaskHelper.TickleMRS(this.DataObject, MoveRequestNotification.SuspendResume, this.mdbGuid, base.ConfigSession, base.UnreachableMrsServers);
			base.WriteVerbose(Strings.ResumeSuccessInformationalMessage(this.DataObject.Name));
		}

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		private Guid mdbGuid;
	}
}
