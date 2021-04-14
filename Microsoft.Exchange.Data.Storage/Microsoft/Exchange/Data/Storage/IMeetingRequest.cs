using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMeetingRequest : IMeetingMessage, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		CalendarItemBase GetCorrelatedItem();

		bool TryUpdateCalendarItem(ref CalendarItemBase originalCalendarItem, bool canUpdatePrincipalCalendar);

		bool IsDelegated();

		VersionedId FetchCorrelatedItemId(CalendarFolder calendarFolder, bool shouldDetectDuplicateIds, out IEnumerable<VersionedId> detectedDuplicatesIds);
	}
}
