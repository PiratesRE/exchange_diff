using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct EasSyncResult
	{
		public List<MessageRec> MessageRecs { get; set; }

		public string SyncKeyRequested { get; set; }

		public string NewSyncKey { get; set; }

		public bool HasMoreAvailable { get; set; }
	}
}
