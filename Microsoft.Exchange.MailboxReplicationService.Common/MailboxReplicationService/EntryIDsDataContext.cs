using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class EntryIDsDataContext : DataContext
	{
		public EntryIDsDataContext(byte[][] entryIDs)
		{
			this.entryIDs = entryIDs;
		}

		public EntryIDsDataContext(List<byte[]> entryIDs)
		{
			this.entryIDs = entryIDs.ToArray();
		}

		public EntryIDsDataContext(byte[] entryID)
		{
			this.entryIDs = new byte[][]
			{
				entryID
			};
		}

		public override string ToString()
		{
			return string.Format("EntryIDs: {0}", CommonUtils.ConcatEntries<byte[]>(this.entryIDs, new Func<byte[], string>(TraceUtils.DumpEntryId)));
		}

		private byte[][] entryIDs;
	}
}
