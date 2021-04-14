using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class InboxFolderMapping : PropTagFolderMapping
	{
		public InboxFolderMapping(WellKnownFolderType wkft, PropTag ptag)
		{
			base.Ptag = ptag;
			base.WKFType = wkft;
		}

		public InboxFolderMapping(WellKnownFolderType wkft, ExtraPropTag ptag)
		{
			base.Ptag = (PropTag)ptag;
			base.WKFType = wkft;
		}
	}
}
