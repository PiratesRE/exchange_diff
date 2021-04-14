using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncFolderHierarchyResponseProcessor
	{
		internal static SyncFolderHierarchyChangeBase[] ShiftChangesToDefaultFoldersToTop(SyncFolderHierarchyChangeBase[] folderChanges, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			return FolderTreeProcessor.ShiftDefaultFoldersToTop<SyncFolderHierarchyChangeBase>(folderChanges, new Func<SyncFolderHierarchyChangeBase, DistinguishedFolderIdName>(SyncFolderHierarchyResponseProcessor.DistinguishedFolderIdNameExtractor), new Func<SyncFolderHierarchyChangeBase, string>(SyncFolderHierarchyResponseProcessor.ParentFolderIdExtractor), foldersToMoveToTop);
		}

		private static string ParentFolderIdExtractor(SyncFolderHierarchyChangeBase folderChange)
		{
			if (folderChange is SyncFolderHierarchyCreateOrUpdateType)
			{
				SyncFolderHierarchyCreateOrUpdateType syncFolderHierarchyCreateOrUpdateType = (SyncFolderHierarchyCreateOrUpdateType)folderChange;
				if (syncFolderHierarchyCreateOrUpdateType != null && syncFolderHierarchyCreateOrUpdateType.Folder != null && syncFolderHierarchyCreateOrUpdateType.Folder.ParentFolderId != null)
				{
					return syncFolderHierarchyCreateOrUpdateType.Folder.ParentFolderId.Id;
				}
			}
			return null;
		}

		private static DistinguishedFolderIdName DistinguishedFolderIdNameExtractor(SyncFolderHierarchyChangeBase folderChange)
		{
			if (folderChange is SyncFolderHierarchyCreateOrUpdateType)
			{
				SyncFolderHierarchyCreateOrUpdateType syncFolderHierarchyCreateOrUpdateType = (SyncFolderHierarchyCreateOrUpdateType)folderChange;
				if (syncFolderHierarchyCreateOrUpdateType != null && syncFolderHierarchyCreateOrUpdateType.Folder != null)
				{
					return EnumUtilities.Parse<DistinguishedFolderIdName>(syncFolderHierarchyCreateOrUpdateType.Folder.DistinguishedFolderId);
				}
			}
			return EnumUtilities.Parse<DistinguishedFolderIdName>(null);
		}
	}
}
