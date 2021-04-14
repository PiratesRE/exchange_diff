using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MailboxRelocationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxRelocationRequest : SetRequest<MailboxRelocationRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		private new Unlimited<int> LargeItemLimit
		{
			get
			{
				return base.LargeItemLimit;
			}
			set
			{
				base.LargeItemLimit = value;
			}
		}

		private new SkippableMergeComponent[] SkipMerging
		{
			get
			{
				return base.SkipMerging;
			}
			set
			{
				base.SkipMerging = value;
			}
		}

		protected override void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
			base.ModifyRequestInternal(requestJob, changedValuesTracker);
			if (base.IsFieldSet("SkipMoving"))
			{
				RequestJobInternalFlags requestJobInternalFlags = requestJob.RequestJobInternalFlags;
				RequestTaskHelper.SetSkipMoving(this.SkipMoving, requestJob, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
				changedValuesTracker.AppendLine(string.Format("InternalFlags: {0} -> {1}", requestJobInternalFlags, requestJob.RequestJobInternalFlags));
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			base.ValidateRequest(requestJob);
			if (base.IsFieldSet("LargeItemLimit") && requestJob.AllowLargeItems)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorIncompatibleParameters("AllowLargeItems", "LargeItemLimit")), ErrorCategory.InvalidArgument, this.Identity);
			}
		}
	}
}
