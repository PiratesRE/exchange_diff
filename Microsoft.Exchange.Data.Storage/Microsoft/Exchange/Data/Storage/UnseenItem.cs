using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnseenItem : IUnseenItem
	{
		public UnseenItem(StoreId storeId, ExDateTime receivedTime)
		{
			ArgumentValidator.ThrowIfNull("storeId", storeId);
			this.Id = storeId;
			this.ReceivedTime = receivedTime;
		}

		public StoreId Id { get; private set; }

		public ExDateTime ReceivedTime { get; private set; }
	}
}
