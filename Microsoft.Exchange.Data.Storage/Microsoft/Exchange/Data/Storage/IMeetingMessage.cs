using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMeetingMessage : IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		bool IsArchiveMigratedMessage { get; }

		Participant ReceivedRepresenting { get; }

		string CalendarOriginatorId { get; }

		bool IsRepairUpdateMessage { get; }

		bool CalendarProcessed { get; set; }

		bool IsRecurringMaster { get; }

		GlobalObjectId GlobalObjectId { get; }
	}
}
