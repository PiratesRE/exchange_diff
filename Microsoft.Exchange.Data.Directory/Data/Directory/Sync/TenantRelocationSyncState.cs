using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal enum TenantRelocationSyncState
	{
		PreSyncAllObjects,
		EnumerateConfigUnitLiveObjects,
		EnumerateConfigUnitLinksInPage,
		EnumerateOrganizationalUnitLiveObjects,
		EnumerateOrganizationalUnitLinksInPage,
		EnumerateConfigUnitDeletedObjects,
		EnumerateOrganizationalUnitDeletedObjects,
		EnumerateSpecialObjects,
		Complete
	}
}
