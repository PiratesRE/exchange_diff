using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	public enum ProcessingStage
	{
		ObjectFullSyncConfiguration,
		RecipientTypeFilter,
		OrganizationFilter,
		RecipientDeletedDuringOrganizationDeletionFilter,
		RelocationStageFilter,
		RelocationPartOfRelocationSyncFilter
	}
}
