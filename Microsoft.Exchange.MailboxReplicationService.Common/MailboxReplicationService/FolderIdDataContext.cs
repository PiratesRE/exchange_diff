using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderIdDataContext : DataContext
	{
		public FolderIdDataContext(byte[] folderId)
		{
			this.folderId = folderId;
		}

		public override string ToString()
		{
			return string.Format("Folder: entryId {0}", TraceUtils.DumpEntryId(this.folderId));
		}

		private byte[] folderId;
	}
}
