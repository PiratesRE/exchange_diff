using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderRecWrapperDataContext : DataContext
	{
		public FolderRecWrapperDataContext(FolderRecWrapper folderRecWrapper)
		{
			this.folderRecWrapper = folderRecWrapper;
		}

		public override string ToString()
		{
			switch (this.folderRecWrapper.FolderType)
			{
			case FolderType.Root:
				return string.Format("Root folder: entryId {0}", TraceUtils.DumpEntryId(this.folderRecWrapper.EntryId));
			case FolderType.Generic:
				return string.Format("Folder: '{0}', entryId {1}, parentId {2}", this.folderRecWrapper.FullFolderName, TraceUtils.DumpEntryId(this.folderRecWrapper.EntryId), TraceUtils.DumpEntryId(this.folderRecWrapper.ParentId));
			case FolderType.Search:
				return string.Format("Search folder: '{0}', entryId {1}, parentId {2}", this.folderRecWrapper.FullFolderName, TraceUtils.DumpEntryId(this.folderRecWrapper.EntryId), TraceUtils.DumpEntryId(this.folderRecWrapper.ParentId));
			default:
				return string.Format("Folder: entryId {0}", TraceUtils.DumpEntryId(this.folderRecWrapper.EntryId));
			}
		}

		private FolderRecWrapper folderRecWrapper;
	}
}
