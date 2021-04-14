using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class MonthlyViewVisualMapper : EventAreaVisualMapper
	{
		public MonthlyViewVisualMapper(CalendarViewBase parentView, IComparer<CalendarVisual> comparer, CalendarVisualContainer container) : base(parentView, comparer, container)
		{
		}

		protected override void MapVisualsX()
		{
			List<EventAreaVisual> list = new List<EventAreaVisual>();
			for (int i = 0; i < base.VisualContainer.Count; i++)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)base.VisualContainer[i];
				eventAreaVisual.Rect.Width = 0.0;
				ExDateTime startTime = base.DataSource.GetStartTime(eventAreaVisual.DataIndex);
				ExDateTime endTime = base.DataSource.GetEndTime(eventAreaVisual.DataIndex);
				TimeSpan timeSpan = endTime - startTime;
				DateRange[] dateRanges = base.ParentView.DateRanges;
				for (int j = 0; j < base.ParentView.DayCount; j++)
				{
					if (dateRanges[j].Intersects(startTime, endTime))
					{
						eventAreaVisual.Rect.Width += 1.0;
						if (eventAreaVisual.Rect.Width == 1.0)
						{
							eventAreaVisual.Rect.X = (double)j;
							if (timeSpan.TotalSeconds > 0.0 && timeSpan.Days < 1 && startTime.Day != endTime.AddSeconds(-1.0).Day)
							{
								EventAreaVisual eventAreaVisual2 = new EventAreaVisual(eventAreaVisual.DataIndex);
								if (startTime > dateRanges[j].Start.Date)
								{
									eventAreaVisual2.Rect.X = eventAreaVisual.Rect.X + 1.0;
								}
								else
								{
									eventAreaVisual2.Rect.X = eventAreaVisual.Rect.X - 1.0;
								}
								eventAreaVisual2.Rect.Width = 1.0;
								list.Add(eventAreaVisual2);
								break;
							}
							if (startTime < dateRanges[j].Start.Date)
							{
								eventAreaVisual.LeftBreak = true;
							}
						}
					}
				}
				int num = (int)(eventAreaVisual.Rect.X + eventAreaVisual.Rect.Width - 1.0);
				if (dateRanges[num].End.Date < endTime)
				{
					eventAreaVisual.RightBreak = true;
				}
			}
			foreach (EventAreaVisual visual in list)
			{
				base.VisualContainer.AddVisual(visual);
			}
		}
	}
}
