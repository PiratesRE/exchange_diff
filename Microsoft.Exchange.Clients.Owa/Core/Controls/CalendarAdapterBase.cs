using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal abstract class CalendarAdapterBase : DisposeTrackableBase
	{
		public CalendarAdapterBase()
		{
			this.DateRanges = CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(new ExDateTime[]
			{
				ExDateTime.Now
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarAdapterBase>(this);
		}

		public bool UserCanReadItem
		{
			get
			{
				return this.DataSource != null && this.DataSource.UserCanReadItem;
			}
		}

		public ICalendarDataSource DataSource { get; protected set; }

		public string CalendarTitle { get; protected set; }

		public DateRange[] DateRanges { get; protected set; }

		public string ClassName
		{
			get
			{
				return this.DataSource.FolderClassName;
			}
		}

		public abstract string IdentityString { get; }

		public abstract string CalendarOwnerDisplayName { get; }

		public static DateRange[] ConvertDateTimeArrayToDateRangeArray(ExDateTime[] dateTimes)
		{
			return CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(dateTimes, 0, 24);
		}

		public static DateRange[] ConvertDateTimeArrayToDateRangeArray(ExDateTime[] dateTimes, int startHour, int endHour)
		{
			DateRange[] array = new DateRange[dateTimes.Length];
			for (int i = 0; i < dateTimes.Length; i++)
			{
				DateTime startTime = new DateTime(dateTimes[i].Year, dateTimes[i].Month, dateTimes[i].Day, startHour, 0, 0);
				DateTime endTime = (startHour == 0 && endHour == 24) ? startTime.AddDays(1.0) : startTime.AddHours((double)(endHour - startHour));
				array[i] = new DateRange(dateTimes[i].TimeZone, startTime, endTime);
			}
			return array;
		}
	}
}
