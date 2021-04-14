using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal enum TenantFullSyncState
	{
		EnumerateLiveObjects,
		EnumerateLinksInPage,
		EnumerateDeletedObjects,
		Complete,
		EnumerateSoftDeletedObjects
	}
}
