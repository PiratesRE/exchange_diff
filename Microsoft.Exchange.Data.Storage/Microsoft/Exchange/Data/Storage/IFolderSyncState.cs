using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderSyncState : ISyncState
	{
		ISyncWatermark Watermark { get; set; }

		FolderSync GetFolderSync();

		FolderSync GetFolderSync(ConflictResolutionPolicy policy);

		void OnCommitStateModifications(FolderSyncStateUtil.CommitStateModificationsDelegate ommitStateModificationsDelegate);
	}
}
