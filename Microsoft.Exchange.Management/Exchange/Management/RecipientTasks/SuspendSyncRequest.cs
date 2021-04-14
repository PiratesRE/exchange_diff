using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "SyncRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendSyncRequest : SuspendRequest<SyncRequestIdParameter>
	{
		protected override IConfigDataProvider CreateSession()
		{
			if (this.Identity != null && this.Identity.OrganizationId != null)
			{
				base.CurrentOrganizationId = this.Identity.OrganizationId;
			}
			return base.CreateSession();
		}

		protected override RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(this.Identity.MailboxId);
			}
		}
	}
}
