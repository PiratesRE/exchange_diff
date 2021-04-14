using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Assistants
{
	internal class StoreMailboxDataExtended : StoreMailboxData
	{
		public StoreMailboxDataExtended(Guid guid, Guid databaseGuid, string displayName, OrganizationId organizationId, TenantPartitionHint tenantPartitionHint, bool isArchiveMailbox, bool isGroupMailbox, bool isTeamSiteMailbox, bool isSharedMailbox) : base(guid, databaseGuid, displayName, organizationId, tenantPartitionHint)
		{
			this.IsArchiveMailbox = isArchiveMailbox;
			this.IsGroupMailbox = isGroupMailbox;
			this.IsTeamSiteMailbox = isTeamSiteMailbox;
			this.IsSharedMailbox = isSharedMailbox;
		}

		public bool IsArchiveMailbox { get; private set; }

		public bool IsGroupMailbox { get; private set; }

		public bool IsTeamSiteMailbox { get; private set; }

		public bool IsSharedMailbox { get; private set; }
	}
}
