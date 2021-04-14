using System;
using System.Collections;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class DateRange : IComparer
	{
		public DateRange(ExDateTime start, ExDateTime end)
		{
			if (start > end)
			{
				throw new ArgumentException("start can't be larger than end");
			}
			this.start = start;
			this.end = end;
		}

		public DateRange(ExTimeZone timeZone, DateTime startTime, DateTime endTime)
		{
			if (startTime > endTime)
			{
				throw new ArgumentException("start can't be larger than end");
			}
			this.visualStart = new DateTime?(startTime);
			this.visualEnd = new DateTime?(endTime);
			this.start = new ExDateTime(timeZone, startTime);
			this.end = new ExDateTime(timeZone, endTime);
		}

		public static bool IsDateInRangeArray(ExDateTime date, DateRange[] ranges)
		{
			if (ranges == null || ranges.Length <= 0)
			{
				throw new ArgumentException("ranges may not be null or empty array");
			}
			for (int i = 0; i < ranges.Length; i++)
			{
				if (ranges[i].IsDateInRange(date))
				{
					return true;
				}
			}
			return false;
		}

		public static ExDateTime GetMinStartTimeInRangeArray(DateRange[] dateRanges)
		{
			if (dateRanges == null || dateRanges.Length <= 0)
			{
				throw new ArgumentException("ranges may not be null or empty array");
			}
			ExDateTime exDateTime = dateRanges[0].Start;
			for (int i = 0; i < dateRanges.Length; i++)
			{
				if (ExDateTime.Compare(dateRanges[i].Start, exDateTime) < 0)
				{
					exDateTime = dateRanges[i].Start;
				}
			}
			return exDateTime;
		}

		public static ExDateTime GetMaxEndTimeInRangeArray(DateRange[] dateRanges)
		{
			if (dateRanges == null || dateRanges.Length <= 0)
			{
				throw new ArgumentException("ranges may not be null or empty array");
			}
			ExDateTime exDateTime = dateRanges[0].End;
			for (int i = 0; i < dateRanges.Length; i++)
			{
				if (ExDateTime.Compare(dateRanges[i].End, exDateTime) > 0)
				{
					exDateTime = dateRanges[i].End;
				}
			}
			return exDateTime;
		}

		public ExDateTime Start
		{
			get
			{
				return this.start;
			}
		}

		public ExDateTime End
		{
			get
			{
				return this.end;
			}
		}

		public DateTime? VisualStart
		{
			get
			{
				return this.visualStart;
			}
		}

		public DateTime? VisualEnd
		{
			get
			{
				return this.visualEnd;
			}
		}

		public int Compare(object x, object y)
		{
			DateRange dateRange = x as DateRange;
			DateRange dateRange2 = y as DateRange;
			return ExDateTime.Compare(dateRange.Start, dateRange2.Start);
		}

		public bool Intersects(ExDateTime start, ExDateTime end)
		{
			return (start < this.End && end > this.Start) || (start == end && start < this.End && end >= this.Start);
		}

		public bool Includes(ExDateTime start, ExDateTime end)
		{
			return start >= this.Start && end <= this.End;
		}

		public bool IsDateInRange(ExDateTime date)
		{
			return date >= this.Start && date < this.End;
		}

		private ExDateTime start;

		private ExDateTime end;

		private DateTime? visualStart;

		private DateTime? visualEnd;
	}
}
