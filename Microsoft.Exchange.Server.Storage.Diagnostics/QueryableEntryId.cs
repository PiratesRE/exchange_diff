using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableEntryId
	{
		public QueryableEntryId(Guid mailboxInstanceGuid, EntryIdHelpers.EIDType eidType, ExchangeId folderId, ExchangeId messageId)
		{
			this.MailboxInstanceGuid = mailboxInstanceGuid;
			this.EidType = eidType.ToString();
			this.FolderId = folderId.To26ByteArray();
			if (messageId == ExchangeId.Null)
			{
				this.MessageId = null;
				return;
			}
			this.MessageId = messageId.To26ByteArray();
		}

		public Guid MailboxInstanceGuid { get; private set; }

		public string EidType { get; private set; }

		public byte[] FolderId { get; private set; }

		public byte[] MessageId { get; private set; }
	}
}
