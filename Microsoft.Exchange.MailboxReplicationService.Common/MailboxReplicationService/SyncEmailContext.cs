using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct SyncEmailContext
	{
		public bool? IsRead { get; set; }

		public bool? IsDraft { get; set; }

		public SyncMessageResponseType? ResponseType { get; set; }

		public string SyncMessageId { get; set; }
	}
}
