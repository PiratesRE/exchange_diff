using System;
using System.Management.Automation;
using System.Security;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SyncRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSyncRequest : SetRequest<SyncRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNull]
		public string RemoteServerName
		{
			get
			{
				return (string)base.Fields["RemoteServerName"];
			}
			set
			{
				base.Fields["RemoteServerName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int RemoteServerPort
		{
			get
			{
				return (int)base.Fields["RemoteServerPort"];
			}
			set
			{
				base.Fields["RemoteServerPort"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string SmtpServerName
		{
			get
			{
				return (string)base.Fields["SmtpServerName"];
			}
			set
			{
				base.Fields["SmtpServerName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int SmtpServerPort
		{
			get
			{
				return (int)base.Fields["SmtpServerPort"];
			}
			set
			{
				base.Fields["SmtpServerPort"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNull]
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public AuthenticationMethod Authentication
		{
			get
			{
				return (AuthenticationMethod)base.Fields["Authentication"];
			}
			set
			{
				base.Fields["Authentication"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public IMAPSecurityMechanism Security
		{
			get
			{
				return (IMAPSecurityMechanism)base.Fields["Security"];
			}
			set
			{
				base.Fields["Security"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		protected override RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(this.Identity.MailboxId);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.Identity != null && this.Identity.OrganizationId != null)
			{
				base.CurrentOrganizationId = this.Identity.OrganizationId;
			}
			return base.CreateSession();
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			base.ValidateRequest(requestJob);
		}

		protected override void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
			if (base.IsFieldSet("RemoteServerName"))
			{
				changedValuesTracker.AppendLine(string.Format("RemoteServerName: {0} -> {1}", requestJob.RemoteHostName, this.RemoteServerName));
				requestJob.RemoteHostName = this.RemoteServerName;
			}
			if (base.IsFieldSet("RemoteServerPort"))
			{
				changedValuesTracker.AppendLine(string.Format("RemoteServerPort: {0} -> {1}", requestJob.RemoteHostPort, this.RemoteServerPort));
				requestJob.RemoteHostPort = this.RemoteServerPort;
			}
			if (base.IsFieldSet("SmtpServerName"))
			{
				changedValuesTracker.AppendLine(string.Format("SmtpServerName: {0} -> {1}", requestJob.SmtpServerName, this.SmtpServerName));
				requestJob.SmtpServerName = this.SmtpServerName;
			}
			if (base.IsFieldSet("SmtpServerPort"))
			{
				changedValuesTracker.AppendLine(string.Format("SmtpServerPort: {0} -> {1}", requestJob.SmtpServerPort, this.SmtpServerPort));
				requestJob.SmtpServerPort = this.SmtpServerPort;
			}
			if (base.IsFieldSet("Authentication"))
			{
				changedValuesTracker.AppendLine(string.Format("Authentication: {0} -> {1}", requestJob.AuthenticationMethod, this.Authentication));
				requestJob.AuthenticationMethod = new AuthenticationMethod?(this.Authentication);
			}
			if (base.IsFieldSet("Security"))
			{
				changedValuesTracker.AppendLine(string.Format("Security: {0} -> {1}", requestJob.SecurityMechanism, this.Security));
				requestJob.SecurityMechanism = this.Security;
			}
			if (base.IsFieldSet("Password"))
			{
				changedValuesTracker.AppendLine("Password: <secure> -> <secure>");
				PSCredential psCred = new PSCredential(requestJob.RemoteCredential.UserName, this.Password);
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(psCred, requestJob.AuthenticationMethod);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				changedValuesTracker.AppendLine(string.Format("IncrementalSyncInterval: {0} -> {1}", requestJob.IncrementalSyncInterval, this.IncrementalSyncInterval));
				requestJob.IncrementalSyncInterval = this.IncrementalSyncInterval;
			}
			if (base.IsFieldSet("StartAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), this.StartAfter))
			{
				RequestTaskHelper.SetStartAfter(this.StartAfter, requestJob, changedValuesTracker);
			}
			if (base.IsFieldSet("CompleteAfter") && !RequestTaskHelper.CompareUtcTimeWithLocalTime(requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), this.CompleteAfter))
			{
				RequestTaskHelper.SetCompleteAfter(this.CompleteAfter, requestJob, changedValuesTracker);
			}
		}

		public const string ParameterRemoteServerName = "RemoteServerName";

		public const string ParameterRemoteServerPort = "RemoteServerPort";

		public const string ParameterSmtpServerName = "SmtpServerName";

		public const string ParameterSmtpServerPort = "SmtpServerPort";

		public const string ParameterPassword = "Password";

		public const string ParameterAuthentication = "Authentication";

		public const string ParameterSecurity = "Security";

		public const string ParameterIncrementalSyncInterval = "IncrementalSyncInterval";
	}
}
