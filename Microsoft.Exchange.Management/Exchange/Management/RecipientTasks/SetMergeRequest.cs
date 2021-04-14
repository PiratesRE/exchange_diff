using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MergeRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMergeRequest : SetRequest<MergeRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNull]
		public string RemoteSourceMailboxServerLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteSourceMailboxServerLegacyDN"];
			}
			set
			{
				base.Fields["RemoteSourceMailboxServerLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNull]
		public Fqdn OutlookAnywhereHostName
		{
			get
			{
				return (Fqdn)base.Fields["OutlookAnywhereHostName"];
			}
			set
			{
				base.Fields["OutlookAnywhereHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool IsAdministrativeCredential
		{
			get
			{
				return (bool)(base.Fields["IsAdministrativeCredential"] ?? false);
			}
			set
			{
				base.Fields["IsAdministrativeCredential"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)(base.Fields["IncrementalSyncInterval"] ?? TimeSpan.Zero);
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public new PSCredential RemoteCredential
		{
			get
			{
				return base.RemoteCredential;
			}
			set
			{
				base.RemoteCredential = value;
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			DateTime? timestamp = requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter);
			bool flag = RequestTaskHelper.CompareUtcTimeWithLocalTime(timestamp, this.StartAfter);
			bool flag2 = base.IsFieldSet("StartAfter") && !flag;
			if (flag2)
			{
				this.CheckJobStatusInQueuedForStartAfterSet(requestJob);
			}
			DateTime utcNow = DateTime.UtcNow;
			if (flag2 && this.StartAfter != null)
			{
				RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.Value.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), utcNow);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				if (requestJob.IncrementalSyncInterval == TimeSpan.Zero || requestJob.JobType < MRSJobType.RequestJobE15_AutoResume)
				{
					base.WriteError(new IncrementalSyncIntervalCannotBeSetOnNonIncrementalRequestsException(), ErrorCategory.InvalidArgument, this.Identity);
				}
				RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.IsFieldSet("StartAfter") && flag)
			{
				this.WriteWarning(Strings.WarningScheduledTimeIsUnchanged("StartAfter"));
			}
			base.ValidateRequest(requestJob);
		}

		protected override void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
			if (base.IsFieldSet("RemoteSourceMailboxServerLegacyDN"))
			{
				changedValuesTracker.AppendLine(string.Format("RemoteMailboxServerLegacyDN: {0} -> {1}", requestJob.RemoteMailboxServerLegacyDN, this.RemoteSourceMailboxServerLegacyDN));
				requestJob.RemoteMailboxServerLegacyDN = this.RemoteSourceMailboxServerLegacyDN;
			}
			if (base.IsFieldSet("OutlookAnywhereHostName"))
			{
				changedValuesTracker.AppendLine(string.Format("OutlookAnywhereHostName: {0} -> {1}", requestJob.OutlookAnywhereHostName, this.OutlookAnywhereHostName));
				requestJob.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
			}
			if (base.IsFieldSet("RemoteCredential"))
			{
				changedValuesTracker.AppendLine("RemoteCredential: <secure> -> <secure>");
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, requestJob.AuthenticationMethod);
			}
			if (base.IsFieldSet("IsAdministrativeCredential"))
			{
				changedValuesTracker.AppendLine(string.Format("IsAdministrativeCredential: {0} -> {1}", requestJob.IsAdministrativeCredential, this.IsAdministrativeCredential));
				requestJob.IsAdministrativeCredential = new bool?(this.IsAdministrativeCredential);
			}
			if (base.IsFieldSet("StartAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), this.StartAfter))
			{
				RequestTaskHelper.SetStartAfter(this.StartAfter, requestJob, changedValuesTracker);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				changedValuesTracker.AppendLine(string.Format("IncrementalSyncInterval: {0} -> {1}", requestJob.IncrementalSyncInterval, this.IncrementalSyncInterval));
				requestJob.IncrementalSyncInterval = this.IncrementalSyncInterval;
			}
		}

		private void CheckJobStatusInQueuedForStartAfterSet(TransactionalRequestJob requestJob)
		{
			if (!RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.Queued))
			{
				base.WriteError(new ErrorStartAfterCanBeSetOnlyInQueuedException(), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		public const string ParameterRemoteSourceMailboxServerLegacyDN = "RemoteSourceMailboxServerLegacyDN";

		public const string ParameterOutlookAnywhereHostName = "OutlookAnywhereHostName";

		public const string ParameterIsAdministrativeCredential = "IsAdministrativeCredential";

		public const string ParameterIncrementalSyncInterval = "IncrementalSyncInterval";
	}
}
