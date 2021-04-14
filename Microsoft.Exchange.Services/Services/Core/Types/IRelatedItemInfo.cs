using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IRelatedItemInfo
	{
		SingleRecipientType From { get; set; }

		ItemId ItemId { get; set; }

		ItemId ConversationId { get; set; }

		string Preview { get; set; }

		string ItemClass { get; set; }
	}
}
