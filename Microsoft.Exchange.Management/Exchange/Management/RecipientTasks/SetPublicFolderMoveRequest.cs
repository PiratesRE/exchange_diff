using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "PublicFolderMoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPublicFolderMoveRequest : SetRequest<PublicFolderMoveRequestIdParameter>
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

		private new SwitchParameter RehomeRequest
		{
			get
			{
				return base.RehomeRequest;
			}
			set
			{
				base.RehomeRequest = value;
			}
		}

		private new string BatchName
		{
			get
			{
				return base.BatchName;
			}
			set
			{
				base.BatchName = value;
			}
		}

		protected override void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
			if (base.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				changedValuesTracker.AppendLine(string.Format("SWRTC: {0} -> {1}", requestJob.SuspendWhenReadyToComplete, this.SuspendWhenReadyToComplete));
				requestJob.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
			}
		}

		protected override void CheckIndexEntry()
		{
		}

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";
	}
}
