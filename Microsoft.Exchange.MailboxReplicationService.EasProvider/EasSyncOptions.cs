using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct EasSyncOptions
	{
		public string SyncKey { get; set; }

		public bool RecentOnly { get; set; }

		public int MaxNumberOfMessage { get; set; }
	}
}
