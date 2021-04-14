using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICalendarGroupEntry : IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string CalendarName { get; set; }

		string ParentGroupName { get; }

		Guid ParentGroupClassId { get; set; }

		LegacyCalendarColor LegacyCalendarColor { get; set; }

		CalendarColor CalendarColor { get; set; }

		StoreObjectId CalendarId { get; set; }

		VersionedId CalendarGroupEntryId { get; }

		byte[] CalendarRecordKey { get; set; }

		bool IsLocalMailboxCalendar { get; }

		byte[] SharerAddressBookEntryId { get; set; }

		byte[] UserAddressBookStoreEntryId { get; set; }
	}
}
