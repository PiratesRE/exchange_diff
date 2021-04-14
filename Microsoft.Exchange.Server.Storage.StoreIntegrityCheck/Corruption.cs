using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public struct Corruption
	{
		public Corruption(CorruptionType corruptionType, ExchangeId? folderId, ExchangeId? messageId, bool isFixed)
		{
			this.corruptionType = corruptionType;
			this.folderId = folderId;
			this.messageId = messageId;
			this.isFixed = isFixed;
		}

		public CorruptionType CorruptionType
		{
			get
			{
				return this.corruptionType;
			}
		}

		public ExchangeId? FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public ExchangeId? MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public bool IsFixed
		{
			get
			{
				return this.isFixed;
			}
		}

		private CorruptionType corruptionType;

		private ExchangeId? folderId;

		private ExchangeId? messageId;

		private bool isFixed;
	}
}
