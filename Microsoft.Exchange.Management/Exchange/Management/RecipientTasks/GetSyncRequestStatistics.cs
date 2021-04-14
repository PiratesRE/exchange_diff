using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SyncRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetSyncRequestStatistics : GetRequestStatistics<SyncRequestIdParameter, SyncRequestStatistics>
	{
		protected override RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(base.Identity.MailboxId);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (base.Identity != null && base.Identity.OrganizationId != null)
			{
				base.CurrentOrganizationId = base.Identity.OrganizationId;
			}
			return base.CreateSession();
		}

		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryTargetUserNotNull(index);
		}
	}
}
