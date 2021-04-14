using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class FreeBusyVisualMapper : IComparer<CalendarVisual>
	{
		public FreeBusyVisualMapper(DailyViewBase parentView, FreeBusyVisualContainer dayFreeBusy)
		{
			if (dayFreeBusy == null)
			{
				throw new ArgumentNullException("dayFreeBusy");
			}
			this.dayFreeBusy = dayFreeBusy;
			this.parentView = parentView;
			this.dataSource = parentView.DataSource;
		}

		public void MapVisuals()
		{
			if (this.dayFreeBusy.Count == 0)
			{
				return;
			}
			this.MapVisualsY();
			this.MapVisualsZ();
		}

		private void MapVisualsY()
		{
			long ticks = ((DateTime)this.dayFreeBusy.DateRange.Start).Ticks;
			long num = ((DateTime)this.dayFreeBusy.DateRange.End).Ticks - ticks;
			for (int i = 0; i < this.dayFreeBusy.Count; i++)
			{
				FreeBusyVisual freeBusyVisual = (FreeBusyVisual)this.dayFreeBusy[i];
				freeBusyVisual.FreeBusyIndex = this.dataSource.GetWrappedBusyType(freeBusyVisual.DataIndex);
				if (freeBusyVisual.FreeBusyIndex == BusyTypeWrapper.Unknown)
				{
					freeBusyVisual.FreeBusyIndex = BusyTypeWrapper.Free;
				}
				ExDateTime startTime = this.dataSource.GetStartTime(freeBusyVisual.DataIndex);
				ExDateTime endTime = this.dataSource.GetEndTime(freeBusyVisual.DataIndex);
				long num2 = ((DateTime)startTime).Ticks - ticks;
				long num3 = ((DateTime)endTime).Ticks - ticks;
				int num4 = (this.parentView.TimeStripMode == TimeStripMode.FifteenMinutes) ? 96 : 48;
				double num5 = (double)num2 * (double)num4 / (double)num;
				freeBusyVisual.Rect.Y = Math.Floor(num5);
				if (freeBusyVisual.Rect.Y < 0.0)
				{
					freeBusyVisual.Rect.Y = 0.0;
					num5 = 0.0;
				}
				double a = (double)num3 * (double)num4 / (double)num;
				freeBusyVisual.Rect.Height = Math.Ceiling(a) - num5;
				if (freeBusyVisual.Rect.Y + freeBusyVisual.Rect.Height > (double)num4)
				{
					freeBusyVisual.Rect.Height = (double)num4 - freeBusyVisual.Rect.Y;
				}
			}
		}

		private void MapVisualsZ()
		{
			this.dayFreeBusy.SortVisuals(this);
		}

		public int Compare(CalendarVisual x, CalendarVisual y)
		{
			FreeBusyVisual freeBusyVisual = (FreeBusyVisual)x;
			FreeBusyVisual freeBusyVisual2 = (FreeBusyVisual)y;
			return freeBusyVisual.FreeBusyIndex - freeBusyVisual2.FreeBusyIndex;
		}

		private FreeBusyVisualContainer dayFreeBusy;

		private DailyViewBase parentView;

		private ICalendarDataSource dataSource;
	}
}
