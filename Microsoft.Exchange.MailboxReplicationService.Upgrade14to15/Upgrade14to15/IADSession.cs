using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal interface IADSession
	{
		IEnumerable<RecipientWrapper> FindPagedMiniRecipient(UpgradeBatchCreatorScheduler.MailboxType mailboxType, ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties);

		IEnumerable<RecipientWrapper> FindPilotUsersADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties);

		QueryFilter BuildE14MailboxQueryFilter();
	}
}
