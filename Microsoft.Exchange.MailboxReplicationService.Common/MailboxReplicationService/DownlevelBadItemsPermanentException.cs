using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DownlevelBadItemsPermanentException : MailboxReplicationPermanentException
	{
		public DownlevelBadItemsPermanentException(List<BadMessageRec> badItems) : base(new LocalizedString("DownlevelBadItemsPermanentException"))
		{
			this.BadItems = badItems;
		}

		public List<BadMessageRec> BadItems { get; private set; }
	}
}
