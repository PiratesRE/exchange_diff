using System;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal interface ICacheEntry
	{
		string OrgName { get; }

		string ExternalDirectoryOrganizationId { get; }

		IADSession ADSessionProxy { get; }
	}
}
