using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class EventAreaVisualMapper
	{
		public EventAreaVisualMapper(CalendarViewBase parentView, IComparer<CalendarVisual> comparer, CalendarVisualContainer visualContainer)
		{
			if (parentView == null)
			{
				throw new ArgumentNullException("parentView");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			if (visualContainer == null)
			{
				throw new ArgumentNullException("visualContainer");
			}
			this.visualContainer = visualContainer;
			this.parentView = parentView;
			this.dataSource = parentView.DataSource;
			this.comparer = comparer;
		}

		public EventAreaVisualMapper(CalendarViewBase parentView, CalendarVisualContainer visualContainer) : this(parentView, new EventAreaVisualMapper.EventAreaVisualComparer(parentView.DataSource), visualContainer)
		{
		}

		protected CalendarVisualContainer VisualContainer
		{
			get
			{
				return this.visualContainer;
			}
		}

		protected ICalendarDataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		protected CalendarViewBase ParentView
		{
			get
			{
				return this.parentView;
			}
		}

		public void MapVisuals()
		{
			if (this.visualContainer.Count == 0)
			{
				return;
			}
			this.MapVisualsX();
			this.MapVisualsY();
		}

		protected virtual void MapVisualsX()
		{
			for (int i = 0; i < this.visualContainer.Count; i++)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)this.visualContainer[i];
				eventAreaVisual.Rect.Width = 0.0;
				ExDateTime startTime = this.dataSource.GetStartTime(eventAreaVisual.DataIndex);
				ExDateTime endTime = this.dataSource.GetEndTime(eventAreaVisual.DataIndex);
				DateRange[] dateRanges = this.parentView.DateRanges;
				for (int j = 0; j < this.parentView.DayCount; j++)
				{
					if (dateRanges[j].Intersects(startTime, endTime))
					{
						eventAreaVisual.Rect.Width += 1.0;
						if (eventAreaVisual.Rect.Width == 1.0)
						{
							if (startTime < dateRanges[j].Start.Date)
							{
								eventAreaVisual.LeftBreak = true;
							}
							eventAreaVisual.Rect.X = (double)j;
						}
						else if (eventAreaVisual.Rect.Width > 1.0 && dateRanges[j - 1].Start.Date.IncrementDays(1) != dateRanges[j].Start.Date)
						{
							eventAreaVisual.SetInnerBreak((int)((double)j - eventAreaVisual.Rect.X));
						}
					}
				}
				if (eventAreaVisual.Rect.Width != 0.0)
				{
					int num = (int)(eventAreaVisual.Rect.X + eventAreaVisual.Rect.Width - 1.0);
					if (dateRanges[num].End.Date < endTime)
					{
						eventAreaVisual.RightBreak = true;
					}
				}
			}
		}

		private void MapVisualsY()
		{
			if (this.visualContainer.Count == 1)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)this.visualContainer[0];
				eventAreaVisual.Rect.Y = 0.0;
			}
			this.visualContainer.SortVisuals(this.comparer);
			this.matrix = new EventAreaVisualMapper.EventAreaMatrix(this.parentView.DayCount);
			for (int i = 0; i < this.visualContainer.Count; i++)
			{
				EventAreaVisual eventAreaVisual2 = (EventAreaVisual)this.visualContainer[i];
				int j;
				for (j = 0; j < this.matrix.RowCount; j++)
				{
					if (this.matrix.FitsInRow(j, eventAreaVisual2.Rect))
					{
						this.matrix.AddToRow(j, eventAreaVisual2.Rect);
						eventAreaVisual2.Rect.Y = (double)j;
						break;
					}
				}
				if (j == this.matrix.RowCount)
				{
					if (this.RowCount >= this.parentView.MaxEventAreaRows)
					{
						this.parentView.RemoveItemFromView(eventAreaVisual2.DataIndex);
					}
					else
					{
						this.matrix.AddRow();
						this.matrix.AddToRow(this.matrix.RowCount - 1, eventAreaVisual2.Rect);
						eventAreaVisual2.Rect.Y = (double)(this.matrix.RowCount - 1);
					}
				}
			}
		}

		public int RowCount
		{
			get
			{
				if (this.matrix != null)
				{
					return this.matrix.RowCount;
				}
				return 0;
			}
		}

		private CalendarVisualContainer visualContainer;

		private CalendarViewBase parentView;

		private ICalendarDataSource dataSource;

		private EventAreaVisualMapper.EventAreaMatrix matrix;

		private IComparer<CalendarVisual> comparer;

		private sealed class EventAreaMatrix
		{
			public EventAreaMatrix(int columnCount)
			{
				this.rows = new ArrayList(2);
				this.columnCount = columnCount;
				this.AddRow();
			}

			public void AddRow()
			{
				this.rows.Add(new bool[this.columnCount]);
			}

			public int RowCount
			{
				get
				{
					return this.rows.Count;
				}
			}

			public bool FitsInRow(int rowIndex, Rect rect)
			{
				if (rect == null)
				{
					throw new ArgumentNullException("rect");
				}
				int val = (int)rect.X;
				int val2 = (int)(rect.X + rect.Width);
				bool[] array = (bool[])this.rows[rowIndex];
				for (int i = Math.Max(0, val); i < Math.Min(val2, array.Length); i++)
				{
					if (array[i])
					{
						return false;
					}
				}
				return true;
			}

			public void AddToRow(int rowIndex, Rect rect)
			{
				if (rect == null)
				{
					throw new ArgumentNullException("rect");
				}
				int val = (int)rect.X;
				int val2 = (int)(rect.X + rect.Width);
				bool[] array = (bool[])this.rows[rowIndex];
				for (int i = Math.Max(0, val); i < Math.Min(val2, array.Length); i++)
				{
					array[i] = true;
				}
			}

			private ArrayList rows;

			private int columnCount;
		}

		private sealed class EventAreaVisualComparer : IComparer<CalendarVisual>
		{
			public EventAreaVisualComparer(ICalendarDataSource dataSource)
			{
				if (dataSource == null)
				{
					throw new ArgumentNullException("dataSource");
				}
				this.dataSource = dataSource;
			}

			public int Compare(CalendarVisual visual1, CalendarVisual visual2)
			{
				if (visual1.Rect.X == visual2.Rect.X)
				{
					string subject = this.dataSource.GetSubject(visual1.DataIndex);
					string subject2 = this.dataSource.GetSubject(visual2.DataIndex);
					return string.Compare(subject, subject2, StringComparison.CurrentCulture);
				}
				if (visual1.Rect.X > visual2.Rect.X)
				{
					return 1;
				}
				return -1;
			}

			private ICalendarDataSource dataSource;
		}
	}
}
