using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class MonthlyViewVisualComparer : IComparer<CalendarVisual>
	{
		public MonthlyViewVisualComparer(ICalendarDataSource dataSource)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			this.dataSource = dataSource;
		}

		public int Compare(CalendarVisual visual1, CalendarVisual visual2)
		{
			if (object.ReferenceEquals(visual1, visual2))
			{
				return 0;
			}
			if (visual1.Rect.X == visual2.Rect.X)
			{
				TimeSpan t = this.Duration(visual1);
				TimeSpan t2 = this.Duration(visual2);
				ExDateTime exDateTime = this.StartTime(visual1);
				ExDateTime exDateTime2 = this.StartTime(visual2);
				int num = this.FreeBusyStatus(visual1);
				int num2 = this.FreeBusyStatus(visual2);
				if (t.Days >= 1 || t2.Days >= 1)
				{
					if (t != t2)
					{
						if (!(t > t2))
						{
							return 1;
						}
						return -1;
					}
					else if (num != num2)
					{
						if (num <= num2)
						{
							return 1;
						}
						return -1;
					}
					else if (exDateTime != exDateTime2)
					{
						return ExDateTime.Compare(exDateTime, exDateTime2);
					}
				}
				else
				{
					if (exDateTime != exDateTime2)
					{
						return ExDateTime.Compare(exDateTime, exDateTime2);
					}
					if (num != num2)
					{
						if (num <= num2)
						{
							return 1;
						}
						return -1;
					}
					else if (t != t2)
					{
						if (!(t > t2))
						{
							return 1;
						}
						return -1;
					}
				}
				string subject = this.dataSource.GetSubject(visual1.DataIndex);
				string subject2 = this.dataSource.GetSubject(visual2.DataIndex);
				return string.Compare(subject, subject2, StringComparison.CurrentCulture);
			}
			if (visual1.Rect.X <= visual2.Rect.X)
			{
				return -1;
			}
			return 1;
		}

		private ExDateTime StartTime(CalendarVisual visual)
		{
			return this.dataSource.GetStartTime(visual.DataIndex);
		}

		private TimeSpan Duration(CalendarVisual visual)
		{
			return this.dataSource.GetEndTime(visual.DataIndex) - this.StartTime(visual);
		}

		private int FreeBusyStatus(CalendarVisual visual)
		{
			return (int)this.dataSource.GetWrappedBusyType(visual.DataIndex);
		}

		private ICalendarDataSource dataSource;
	}
}
