using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderHierarchySyncState : ISyncState
	{
		FolderHierarchySync GetFolderHierarchySync();

		FolderHierarchySync GetFolderHierarchySync(ChangeTrackingDelegate changeTrackingDelegate);
	}
}
