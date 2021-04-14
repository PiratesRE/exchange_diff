using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Migration
{
	internal interface IMigrationJobCache
	{
		bool Add(string mailboxLegacyDn, Guid mdbGuid, OrganizationId organizationId, bool refresh);

		void Remove(MigrationCacheEntry cacheEntry);

		bool SyncWithStore();

		List<MigrationCacheEntry> Get();
	}
}
