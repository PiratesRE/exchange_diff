using System;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[Flags]
	internal enum SyncOption : uint
	{
		Default = 0U,
		CurrentDocumentLibsOnly = 1U,
		RefreshTeamMailboxCacheEntry = 2U,
		IgnoreNextAllowedSyncTimeRestriction = 4U,
		FullSync = 8U
	}
}
