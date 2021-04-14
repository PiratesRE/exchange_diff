using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxRootFolderMapping : PropTagFolderMapping
	{
		public MailboxRootFolderMapping(WellKnownFolderType wkft, PropTag ptag)
		{
			base.Ptag = ptag;
			base.WKFType = wkft;
		}

		public MailboxRootFolderMapping(WellKnownFolderType wkft, ExtraPropTag ptag)
		{
			base.Ptag = (PropTag)ptag;
			base.WKFType = wkft;
		}
	}
}
