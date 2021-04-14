using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class FreeBusyQueryResult : BaseQueryResult
	{
		public FreeBusyViewType View
		{
			get
			{
				return this.view;
			}
		}

		public string MergedFreeBusy
		{
			get
			{
				return this.mergedFreeBusy;
			}
		}

		public CalendarEvent[] CalendarEventArray
		{
			get
			{
				if (this.view != FreeBusyViewType.MergedOnly)
				{
					return this.calendarEventArray;
				}
				return null;
			}
		}

		public WorkingHours WorkingHours
		{
			get
			{
				return this.workingHours;
			}
		}

		public string CurrentOofMessage
		{
			get
			{
				return null;
			}
		}

		internal CalendarEvent[] CalendarEventArrayInternal
		{
			get
			{
				return this.calendarEventArray;
			}
		}

		public FreeBusyQueryResult(FreeBusyViewType view, CalendarEvent[] calendarEventArray, string mergedFreeBusy, WorkingHours workingHours)
		{
			this.view = view;
			this.calendarEventArray = calendarEventArray;
			this.mergedFreeBusy = mergedFreeBusy;
			this.workingHours = workingHours;
		}

		internal string GetFreeBusyByDay(Duration timeWindow, ExTimeZone timeZone)
		{
			ExDateTime windowStart = new ExDateTime(timeZone, timeWindow.StartTime);
			ExDateTime exDateTime = new ExDateTime(timeZone, timeWindow.EndTime);
			int num = (int)(exDateTime.Date - windowStart.Date).TotalDays + 1;
			char[] array = new char[num];
			char c = (base.ExceptionInfo != null) ? '5' : '0';
			for (int i = 0; i < num; i++)
			{
				array[i] = c;
			}
			if (this.CalendarEventArray != null)
			{
				for (int j = 0; j < this.CalendarEventArray.Length; j++)
				{
					CalendarEvent calendarEvent = this.CalendarEventArray[j];
					ExDateTime exDateTime2 = new ExDateTime(timeZone, calendarEvent.StartTime.Date);
					ExDateTime t = new ExDateTime(timeZone, calendarEvent.EndTime.Date);
					while (exDateTime2 < t)
					{
						this.AddEventTime(array, windowStart, exDateTime2, calendarEvent.BusyType);
						exDateTime2 = exDateTime2.AddDays(1.0);
					}
					if (calendarEvent.EndTime.TimeOfDay != TimeSpan.Zero)
					{
						this.AddEventTime(array, windowStart, new ExDateTime(timeZone, calendarEvent.EndTime), calendarEvent.BusyType);
					}
				}
			}
			return new string(array);
		}

		private void AddEventTime(char[] freeBusyValues, ExDateTime windowStart, ExDateTime eventTime, BusyType busyType)
		{
			int num = (int)Math.Round((eventTime.Date - windowStart.Date).TotalDays);
			if (num >= 0 && num < freeBusyValues.Length)
			{
				char c = (char)(busyType + 48);
				if (freeBusyValues[num] < c)
				{
					freeBusyValues[num] = c;
				}
			}
		}

		public FreeBusyQueryResult(LocalizedException exception) : base(exception)
		{
			this.view = FreeBusyViewType.None;
		}

		private FreeBusyViewType view;

		private string mergedFreeBusy;

		private CalendarEvent[] calendarEventArray;

		private WorkingHours workingHours;
	}
}
