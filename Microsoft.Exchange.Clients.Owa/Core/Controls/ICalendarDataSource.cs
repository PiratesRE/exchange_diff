using System;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal interface ICalendarDataSource
	{
		int Count { get; }

		WorkingHours WorkingHours { get; }

		bool UserCanReadItem { get; }

		bool UserCanCreateItem { get; }

		string FolderClassName { get; }

		SharedType SharedType { get; }

		OwaStoreObjectId GetItemId(int index);

		string GetChangeKey(int index);

		ExDateTime GetStartTime(int index);

		ExDateTime GetEndTime(int index);

		string GetSubject(int index);

		string GetLocation(int index);

		bool IsMeeting(int index);

		bool IsCancelled(int index);

		bool HasAttachment(int index);

		bool IsPrivate(int index);

		CalendarItemTypeWrapper GetWrappedItemType(int index);

		string GetOrganizerDisplayName(int index);

		BusyTypeWrapper GetWrappedBusyType(int index);

		bool IsOrganizer(int index);

		string[] GetCategories(int index);

		string GetCssClassName(int index);

		string GetInviteesDisplayNames(int index);
	}
}
