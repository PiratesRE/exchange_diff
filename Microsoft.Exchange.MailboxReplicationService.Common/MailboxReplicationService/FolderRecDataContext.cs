using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderRecDataContext : DataContext
	{
		public FolderRecDataContext(FolderRec folderRec)
		{
			this.folderRec = folderRec;
		}

		public override string ToString()
		{
			switch (this.folderRec.FolderType)
			{
			case FolderType.Root:
				return string.Format("Root folder: entryId {0}", TraceUtils.DumpEntryId(this.folderRec.EntryId));
			case FolderType.Generic:
				return string.Format("Folder: '{0}', entryId {1}, parentId {2}", this.folderRec.FolderName, TraceUtils.DumpEntryId(this.folderRec.EntryId), TraceUtils.DumpEntryId(this.folderRec.ParentId));
			case FolderType.Search:
				return string.Format("Search folder: '{0}', entryId {1}, parentId {2}", this.folderRec.FolderName, TraceUtils.DumpEntryId(this.folderRec.EntryId), TraceUtils.DumpEntryId(this.folderRec.ParentId));
			default:
				return string.Format("Folder: entryId {0}", TraceUtils.DumpEntryId(this.folderRec.EntryId));
			}
		}

		private FolderRec folderRec;
	}
}
