using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class MessageEntry
	{
		public MessageEntry(int documentId, ExchangeId messageId)
		{
			this.documentId = documentId;
			this.messageId = messageId;
		}

		public int DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		public ExchangeId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public override int GetHashCode()
		{
			return this.DocumentId;
		}

		public override bool Equals(object other)
		{
			MessageEntry messageEntry = other as MessageEntry;
			return messageEntry != null && messageEntry.DocumentId == this.DocumentId;
		}

		private readonly int documentId;

		private readonly ExchangeId messageId;
	}
}
