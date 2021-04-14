using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MailboxImportRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxImportRequest : SetRequest<MailboxImportRequestIdParameter>
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
				changedValuesTracker.AppendLine("RemoteCredential: <secure> -> <secure>");
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, null);
			}
			if (base.IsFieldSet("RemoteHostName"))
			{
				changedValuesTracker.AppendLine(string.Format("RemoteHostName: {0} -> {1}", requestJob.RemoteHostName, this.RemoteHostName));
				requestJob.RemoteHostName = this.RemoteHostName;
			}
			base.ModifyRequestInternal(requestJob, changedValuesTracker);
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			bool flag = !OrganizationId.ForestWideOrgId.Equals(base.ExecutingUserOrganizationId);
			bool flag2 = !string.IsNullOrEmpty(base.IsFieldSet("RemoteHostName") ? this.RemoteHostName : requestJob.RemoteHostName);
			if (flag && !flag2)
			{
				base.WriteError(new RemoteMailboxImportNeedRemoteProxyException(), ErrorCategory.InvalidArgument, this);
			}
			base.ValidateRequest(requestJob);
		}
	}
}
