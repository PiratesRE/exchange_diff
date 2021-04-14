using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	internal class EventTimeAdjuster
	{
		public EventTimeAdjuster(DateTimeHelper dateTimeHelper)
		{
			this.DateTimeHelper = dateTimeHelper;
		}

		private protected virtual DateTimeHelper DateTimeHelper { protected get; private set; }

		public virtual void AdjustTimeProperties(ICalendarItemBase calendarItem)
		{
			EventTimeAdjuster.TimeProperties initialValues = new EventTimeAdjuster.TimeProperties
			{
				IsAllDay = this.GetValue<ICalendarItemBase, bool>(calendarItem, CalendarItemAccessors.IsAllDay),
				Start = this.GetValue<ICalendarItemBase, ExDateTime>(calendarItem, CalendarItemAccessors.StartTime),
				End = this.GetValue<ICalendarItemBase, ExDateTime>(calendarItem, CalendarItemAccessors.EndTime),
				IntendedStartTimeZone = this.GetValue<ICalendarItemBase, ExTimeZone>(calendarItem, CalendarItemAccessors.StartTimeZone),
				IntendedEndTimeZone = this.GetValue<ICalendarItemBase, ExTimeZone>(calendarItem, CalendarItemAccessors.EndTimeZone)
			};
			EventTimeAdjuster.TimeProperties timeProperties = this.AdjustTimeProperties(initialValues, calendarItem.Session.ExTimeZone);
			CalendarItemAccessors.IsAllDay.Set(calendarItem, timeProperties.IsAllDay);
			CalendarItemAccessors.StartTime.Set(calendarItem, timeProperties.Start);
			CalendarItemAccessors.EndTime.Set(calendarItem, timeProperties.End);
			CalendarItemAccessors.StartTimeZone.Set(calendarItem, timeProperties.IntendedStartTimeZone);
			CalendarItemAccessors.EndTimeZone.Set(calendarItem, timeProperties.IntendedEndTimeZone);
		}

		public virtual Event AdjustTimeProperties(Event theEvent, ExTimeZone sessionTimeZone)
		{
			ExTimeZone timeZoneOrDefault = this.DateTimeHelper.GetTimeZoneOrDefault(theEvent.IntendedStartTimeZoneId, ExTimeZone.UtcTimeZone);
			ExTimeZone timeZoneOrDefault2 = this.DateTimeHelper.GetTimeZoneOrDefault(theEvent.IntendedEndTimeZoneId, timeZoneOrDefault);
			EventTimeAdjuster.TimeProperties initialValues = new EventTimeAdjuster.TimeProperties
			{
				IsAllDay = theEvent.IsAllDay,
				Start = theEvent.Start,
				End = theEvent.End,
				IntendedStartTimeZone = timeZoneOrDefault,
				IntendedEndTimeZone = timeZoneOrDefault2
			};
			EventTimeAdjuster.TimeProperties timeProperties = this.AdjustTimeProperties(initialValues, sessionTimeZone);
			theEvent.IsAllDay = timeProperties.IsAllDay;
			theEvent.Start = timeProperties.Start;
			theEvent.End = timeProperties.End;
			theEvent.IntendedStartTimeZoneId = timeProperties.IntendedStartTimeZone.Id;
			theEvent.IntendedEndTimeZoneId = timeProperties.IntendedEndTimeZone.Id;
			return theEvent;
		}

		public virtual ExDateTime FloatTime(ExDateTime time, ExTimeZone intendedTimeZone, ExTimeZone sessionTimeZone)
		{
			ExDateTime time2 = this.DateTimeHelper.ChangeTimeZone(time, intendedTimeZone, true);
			return this.DateTimeHelper.ChangeTimeZone(time2, sessionTimeZone, false);
		}

		protected virtual TValue GetValue<TContainer, TValue>(TContainer container, IPropertyAccessor<TContainer, TValue> accessor)
		{
			TValue result;
			if (!accessor.TryGetValue(container, out result))
			{
				return default(TValue);
			}
			return result;
		}

		private EventTimeAdjuster.TimeProperties AdjustTimeProperties(EventTimeAdjuster.TimeProperties initialValues, ExTimeZone sessionTimeZone)
		{
			if (initialValues.IsAllDay)
			{
				ExDateTime time = this.FloatTime(initialValues.Start, initialValues.IntendedStartTimeZone, sessionTimeZone);
				initialValues.Start = EventDataProvider.EnforceMidnightTime(time, MidnightEnforcementOption.MoveBackward);
				ExDateTime time2 = this.FloatTime(initialValues.End, initialValues.IntendedEndTimeZone, sessionTimeZone);
				initialValues.End = EventDataProvider.EnforceMidnightTime(time2, MidnightEnforcementOption.MoveForward);
				initialValues.IntendedStartTimeZone = initialValues.Start.TimeZone;
				initialValues.IntendedEndTimeZone = initialValues.Start.TimeZone;
			}
			return initialValues;
		}

		private struct TimeProperties
		{
			public bool IsAllDay;

			public ExDateTime Start;

			public ExDateTime End;

			public ExTimeZone IntendedStartTimeZone;

			public ExTimeZone IntendedEndTimeZone;
		}
	}
}
