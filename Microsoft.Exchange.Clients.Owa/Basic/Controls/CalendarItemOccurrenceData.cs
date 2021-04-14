using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class CalendarItemOccurrenceData : CalendarItemBaseData
	{
		public ExDateTime OriginalStartTime
		{
			get
			{
				return this.originalStartTime;
			}
			set
			{
				this.originalStartTime = value;
			}
		}

		public CalendarItemOccurrenceData()
		{
		}

		public CalendarItemOccurrenceData(CalendarItemOccurrence calendarItem)
		{
			this.SetFrom(calendarItem);
		}

		public CalendarItemOccurrenceData(CalendarItemBase calendarItemBase)
		{
			this.SetFrom(calendarItemBase);
		}

		public CalendarItemOccurrenceData(CalendarItemOccurrenceData other) : base(other)
		{
			this.originalStartTime = other.originalStartTime;
		}

		public static EditCalendarItemHelper.CalendarItemUpdateFlags Compare(CalendarItemOccurrence calendarItem)
		{
			return EditCalendarItemHelper.CalendarItemUpdateFlags.None;
		}

		public override void SetFrom(CalendarItemBase calendarItemBase)
		{
			base.SetFrom(calendarItemBase);
			CalendarItemOccurrence calendarItemOccurrence = calendarItemBase as CalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				this.originalStartTime = calendarItemOccurrence.OriginalStartTime;
			}
		}

		public override EditCalendarItemHelper.CalendarItemUpdateFlags CopyTo(CalendarItemBase calendarItemBase)
		{
			return base.CopyTo(calendarItemBase);
		}

		private ExDateTime originalStartTime;
	}
}
