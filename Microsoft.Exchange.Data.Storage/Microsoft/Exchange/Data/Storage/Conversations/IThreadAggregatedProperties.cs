using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IThreadAggregatedProperties
	{
		string Preview { get; }

		ConversationId ThreadId { get; }

		ExDateTime? LastDeliveryTime { get; }

		Participant[] UniqueSenders { get; }

		StoreObjectId[] ItemIds { get; }

		StoreObjectId[] DraftItemIds { get; }

		int ItemCount { get; }

		bool HasAttachments { get; }

		bool HasIrm { get; }

		Importance Importance { get; }

		IconIndex IconIndex { get; }

		FlagStatus FlagStatus { get; }

		int UnreadCount { get; }

		short[] RichContent { get; }

		string[] ItemClasses { get; }
	}
}
