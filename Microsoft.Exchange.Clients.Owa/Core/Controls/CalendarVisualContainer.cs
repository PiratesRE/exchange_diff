using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal abstract class CalendarVisualContainer
	{
		public CalendarVisualContainer(DateRange dateRange)
		{
			this.dateRange = dateRange;
		}

		public void AddVisual(CalendarVisual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			if (this.visuals == null)
			{
				this.visuals = new List<CalendarVisual>(4);
			}
			this.visuals.Add(visual);
		}

		public DateRange DateRange
		{
			get
			{
				return this.dateRange;
			}
		}

		public CalendarVisual this[int index]
		{
			get
			{
				return this.visuals[index];
			}
		}

		public int Count
		{
			get
			{
				if (this.visuals == null)
				{
					return 0;
				}
				return this.visuals.Count;
			}
		}

		public void SortVisuals(IComparer<CalendarVisual> comparer)
		{
			this.visuals.Sort(comparer);
		}

		public abstract void MapVisuals();

		private DateRange dateRange;

		private List<CalendarVisual> visuals;
	}
}
