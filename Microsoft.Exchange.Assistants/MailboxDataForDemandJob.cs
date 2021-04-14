using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MailboxDataForDemandJob : StoreMailboxData
	{
		public MailboxDataForDemandJob(Guid mailboxGuid, Guid databaseGuid, OrganizationId organizationId, string parameters) : this(mailboxGuid, databaseGuid, organizationId, parameters, null)
		{
		}

		public MailboxDataForDemandJob(Guid mailboxGuid, Guid databaseGuid, OrganizationId organizationId, string parameters, TenantPartitionHint tenantPartitionHint) : base(mailboxGuid, databaseGuid, "(unknown)", organizationId, tenantPartitionHint)
		{
			this.Parameters = parameters;
		}

		public string Parameters { get; private set; }
	}
}
