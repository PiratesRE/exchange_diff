using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderPropertyRestriction : PropertyRestriction
	{
		public FolderPropertyRestriction()
		{
			this.BlockBeforeLink.Add(FolderSchema.LinkedId);
			this.BlockBeforeLink.Add(FolderSchema.LinkedUrl);
			this.BlockBeforeLink.Add(FolderSchema.LinkedSiteUrl);
			this.BlockBeforeLink.Add(FolderSchema.LinkedListId);
			this.BlockBeforeLink.Add(FolderSchema.SharePointChangeToken);
			this.BlockBeforeLink.Add(FolderSchema.IsDocumentLibraryFolder);
			this.BlockBeforeLink.Add(FolderSchema.LinkedSiteAuthorityUrl);
			this.BlockAfterLink.Add(FolderSchema.LinkedId);
			this.BlockAfterLink.Add(FolderSchema.LinkedUrl);
			this.BlockAfterLink.Add(FolderSchema.LinkedSiteUrl);
			this.BlockAfterLink.Add(FolderSchema.LinkedListId);
			this.BlockAfterLink.Add(FolderSchema.SharePointChangeToken);
			this.BlockAfterLink.Add(FolderSchema.IsDocumentLibraryFolder);
			this.BlockAfterLink.Add(FolderSchema.DisplayName);
			this.BlockAfterLink.Add(StoreObjectSchema.ContainerClass);
			this.BlockAfterLink.Add(FolderSchema.IsHidden);
			this.BlockAfterLink.Add(FolderSchema.LinkedSiteAuthorityUrl);
		}

		public static FolderPropertyRestriction Instance = new FolderPropertyRestriction();
	}
}
