using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class EventAttachmentDataProvider : AttachmentDataProvider
	{
		public EventAttachmentDataProvider(EventReference scope, StoreId parentItemId) : base(scope, parentItemId)
		{
		}

		protected override void BeforeParentItemSave(IItem parentItem)
		{
			ICalendarItemBase calendarItemBase = parentItem as ICalendarItemBase;
			if (calendarItemBase != null)
			{
				bool flag;
				CalendarItemAccessors.HasAttendees.TryGetValue(calendarItemBase, out flag);
				if (flag)
				{
					calendarItemBase[CalendarItemBaseSchema.IsDraft] = true;
					return;
				}
				calendarItemBase[CalendarItemBaseSchema.IsDraft] = false;
			}
		}

		protected override AttachmentCollection GetAttachmentCollection(IItem parentItem)
		{
			ICalendarItemOccurrence calendarItemOccurrence = parentItem as ICalendarItemOccurrence;
			if (calendarItemOccurrence != null && !calendarItemOccurrence.IsException)
			{
				calendarItemOccurrence.MakeModifiedOccurrence();
			}
			return base.GetAttachmentCollection(parentItem);
		}
	}
}
