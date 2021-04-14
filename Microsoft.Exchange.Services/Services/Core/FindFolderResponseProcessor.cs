using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FindFolderResponseProcessor
	{
		internal static BaseFolderType[] ShiftDefaultFoldersToTop(BaseFolderType[] folderCollection, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			return FolderTreeProcessor.ShiftDefaultFoldersToTop<BaseFolderType>(folderCollection, new Func<BaseFolderType, DistinguishedFolderIdName>(FindFolderResponseProcessor.DistinguishedFolderIdNameExtractor), new Func<BaseFolderType, string>(FindFolderResponseProcessor.ParentFolderIdExtractor), foldersToMoveToTop);
		}

		private static string ParentFolderIdExtractor(BaseFolderType folder)
		{
			if (folder != null && folder.ParentFolderId != null)
			{
				return folder.ParentFolderId.Id;
			}
			return null;
		}

		private static DistinguishedFolderIdName DistinguishedFolderIdNameExtractor(BaseFolderType folder)
		{
			return EnumUtilities.Parse<DistinguishedFolderIdName>(folder.DistinguishedFolderId);
		}
	}
}
