using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "MergeRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumeMergeRequest : ResumeRequest<MergeRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public override SwitchParameter SuspendWhenReadyToComplete
		{
			get
			{
				return base.SuspendWhenReadyToComplete;
			}
			set
			{
				base.SuspendWhenReadyToComplete = value;
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			if (!requestJob.SuspendWhenReadyToComplete && this.SuspendWhenReadyToComplete)
			{
				base.WriteError(new SuspendWhenReadyToCompleteNotSupportedException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (requestJob.SuspendWhenReadyToComplete && requestJob.StatusDetail == RequestState.AutoSuspended && !this.SuspendWhenReadyToComplete)
			{
				base.WriteError(new IncrementalMergesRequireSuspendWhenReadyToCompleteException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
			base.ValidateRequest(requestJob);
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			if (requestJob.Suspend)
			{
				DateTime? timestamp = requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
				requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, timestamp);
			}
			base.ModifyRequest(requestJob);
		}
	}
}
