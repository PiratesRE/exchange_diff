using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class ItemChange
	{
		public ItemChange(ItemChangeType changeType, ItemIdType id)
		{
			this.ChangeType = changeType;
			this.Id = id;
		}

		public ItemChangeType ChangeType { get; private set; }

		public ItemIdType Id { get; private set; }
	}
}
