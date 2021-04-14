using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "FolderMoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetFolderMoveRequest : SetRequest<FolderMoveRequestIdParameter>
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
			if (base.IsFieldSet("CompleteAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), this.CompleteAfter))
			{
				RequestTaskHelper.SetCompleteAfter(this.CompleteAfter, requestJob, changedValuesTracker);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				requestJob.IncrementalSyncInterval = this.IncrementalSyncInterval;
			}
		}

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";
	}
}
