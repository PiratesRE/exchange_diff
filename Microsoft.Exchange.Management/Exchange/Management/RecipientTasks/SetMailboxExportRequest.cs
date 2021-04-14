using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MailboxExportRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxExportRequest : SetRequest<MailboxExportRequestIdParameter>
	{
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public new Fqdn RemoteHostName
		{
			get
			{
				return base.RemoteHostName;
			}
			set
			{
				base.RemoteHostName = value;
			}
		}

		protected override void ModifyRequestInternal(TransactionalRequestJob requestJob, StringBuilder changedValuesTracker)
		{
			if (base.IsFieldSet("RemoteCredential"))
			{
				string arg = (requestJob.RemoteCredential == null) ? null : requestJob.RemoteCredential.UserName;
				string arg2 = (this.RemoteCredential == null) ? null : this.RemoteCredential.UserName;
				changedValuesTracker.AppendLine(string.Format("UserName of RemoteCredential: {0} -> {1}", arg, arg2));
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, null);
			}
			if (base.IsFieldSet("RemoteHostName"))
			{
				changedValuesTracker.AppendLine(string.Format("RemoteHostName: {0} -> {1}", requestJob.RemoteHostName, this.RemoteHostName));
				requestJob.RemoteHostName = this.RemoteHostName;
			}
			base.ModifyRequestInternal(requestJob, changedValuesTracker);
		}
	}
}
