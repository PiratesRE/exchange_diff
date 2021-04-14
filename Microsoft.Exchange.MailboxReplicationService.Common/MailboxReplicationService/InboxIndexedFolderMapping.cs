using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class InboxIndexedFolderMapping : InboxFolderMapping
	{
		public InboxIndexedFolderMapping(WellKnownFolderType wkft, ExtraPropTag ptag, int entryIndex) : base(wkft, ptag)
		{
			this.EntryIndex = entryIndex;
		}

		public int EntryIndex { get; protected set; }
	}
}
