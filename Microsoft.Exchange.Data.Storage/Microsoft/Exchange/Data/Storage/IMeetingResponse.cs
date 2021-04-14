using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMeetingResponse : IMeetingMessageInstance, IMeetingMessage, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		ResponseType ResponseType { get; }

		StoreObjectId AssociatedMeetingRequestId { get; }

		ExDateTime AttendeeCriticalChangeTime { get; }

		string Location { get; set; }

		ExDateTime ProposedStart { get; }

		ExDateTime ProposedEnd { get; }
	}
}
