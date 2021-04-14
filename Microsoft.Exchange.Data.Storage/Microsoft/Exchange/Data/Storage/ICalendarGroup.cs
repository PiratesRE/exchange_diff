using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICalendarGroup : IFolderTreeData, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		Guid GroupClassId { get; }

		string GroupName { get; set; }

		CalendarGroupType GroupType { get; }

		ReadOnlyCollection<CalendarGroupEntryInfo> GetChildCalendars();

		CalendarGroupInfo GetCalendarGroupInfo();

		CalendarGroupEntryInfo FindSharedGSCalendaryEntry(string sharerLegacyDN);

		CalendarGroupEntryInfo FindSharedCalendaryEntry(StoreObjectId folderId);
	}
}
