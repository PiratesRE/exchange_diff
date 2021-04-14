using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors
{
	internal static class CalendarGroupEntryAccessors
	{
		public static readonly IStoragePropertyAccessor<ICalendarGroupEntry, CalendarColor> CalendarColor = new DelegatedStoragePropertyAccessor<ICalendarGroupEntry, CalendarColor>(delegate(ICalendarGroupEntry container, out CalendarColor value)
		{
			value = container.CalendarColor;
			return true;
		}, delegate(ICalendarGroupEntry entry, CalendarColor color)
		{
			entry.CalendarColor = color;
		}, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<ICalendarGroupEntry, StoreId> CalendarId = new DelegatedStoragePropertyAccessor<ICalendarGroupEntry, StoreId>(delegate(ICalendarGroupEntry container, out StoreId value)
		{
			value = container.CalendarId;
			return true;
		}, null, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<ICalendarGroupEntry, string> CalendarName = new DefaultStoragePropertyAccessor<ICalendarGroupEntry, string>(CalendarGroupEntrySchema.CalendarName, false);

		public static readonly IStoragePropertyAccessor<ICalendarGroupEntry, byte[]> CalendarRecordKey = new DefaultStoragePropertyAccessor<ICalendarGroupEntry, byte[]>(CalendarGroupEntrySchema.NodeRecordKey, false);
	}
}
