using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal abstract class DailyViewBase : CalendarViewBase
	{
		public abstract int MaxConflictingItems { get; }

		protected virtual TimeStripMode GetTimeStripMode()
		{
			return DailyViewBase.GetPersistedTimeStripMode(base.SessionContext);
		}

		protected DailyViewBase(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter) : base(sessionContext, calendarAdapter)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "DailyViewBase.DailyViewBase");
			this.timeStripMode = this.GetTimeStripMode();
			if (calendarAdapter.DataSource != null)
			{
				if (base.DateRanges != null)
				{
					Array.Sort(base.DateRanges, base.DateRanges[0]);
				}
				ExTraceGlobals.CalendarTracer.TraceDebug(0L, "Creating and mapping visuals in the view");
				this.CreateVisuals();
				this.MapVisuals();
			}
		}

		private void CreateVisuals()
		{
			this.viewDays = new SchedulingAreaVisualContainer[base.DayCount];
			this.eventArea = new EventAreaVisualContainer(this, null);
			this.rolledUpFreeBusy = new FreeBusyVisualContainer[base.DayCount];
			this.rowFreeBusy = new BusyTypeWrapper[base.DayCount][];
			for (int i = 0; i < base.DayCount; i++)
			{
				this.viewDays[i] = new SchedulingAreaVisualContainer(this, base.DateRanges[i]);
				this.rolledUpFreeBusy[i] = new FreeBusyVisualContainer(this, base.DateRanges[i]);
			}
			for (int i = 0; i < base.DataSource.Count; i++)
			{
				if (i > this.MaxItemsPerView)
				{
					base.RemoveItemFromView(i);
				}
				else
				{
					ExDateTime startTime = base.DataSource.GetStartTime(i);
					ExDateTime endTime = base.DataSource.GetEndTime(i);
					if ((endTime - startTime).Days > 0)
					{
						this.eventArea.AddVisual(new EventAreaVisual(i));
						for (int j = 0; j < base.DateRanges.Length; j++)
						{
							if (base.DateRanges[j].Intersects(startTime, endTime))
							{
								this.rolledUpFreeBusy[j].AddVisual(new FreeBusyVisual(i));
							}
						}
					}
					else
					{
						int k = 0;
						while (k < base.DateRanges.Length)
						{
							if (base.DateRanges[k].Intersects(startTime, endTime))
							{
								this.viewDays[k].AddVisual(new SchedulingAreaVisual(i));
								if (startTime >= base.DateRanges[k].Start && endTime > base.DateRanges[k].End && k < base.DateRanges.Length - 1 && endTime > base.DateRanges[k + 1].Start)
								{
									this.viewDays[k + 1].AddVisual(new SchedulingAreaVisual(i));
									break;
								}
								break;
							}
							else
							{
								k++;
							}
						}
					}
				}
			}
		}

		private void MapVisuals()
		{
			for (int i = 0; i < this.viewDays.Length; i++)
			{
				this.viewDays[i].MapVisuals();
			}
			this.eventArea.MapVisuals();
			if (base.DataSource != null)
			{
				this.transformedRolledUpFreeBusy = new ArrayList[base.DayCount];
				for (int j = 0; j < base.DayCount; j++)
				{
					this.rolledUpFreeBusy[j].MapVisuals();
					this.TransformRolledUpFreeBusyVisuals(j);
				}
			}
		}

		private void TransformRolledUpFreeBusyVisuals(int iDay)
		{
			ArrayList arrayList = null;
			this.rowFreeBusy[iDay] = new BusyTypeWrapper[24 * ((this.timeStripMode == TimeStripMode.FifteenMinutes) ? 4 : 2)];
			BusyTypeWrapper[] array = this.rowFreeBusy[iDay];
			FreeBusyVisualContainer freeBusyVisualContainer = this.rolledUpFreeBusy[iDay];
			for (int i = 0; i < freeBusyVisualContainer.Count; i++)
			{
				FreeBusyVisual freeBusyVisual = (FreeBusyVisual)freeBusyVisualContainer[i];
				if (!base.IsItemRemoved(freeBusyVisual.DataIndex))
				{
					int num = 0;
					while ((double)num < freeBusyVisual.Rect.Height)
					{
						if (array[(int)freeBusyVisual.Rect.Y + num] < freeBusyVisual.FreeBusyIndex)
						{
							array[(int)freeBusyVisual.Rect.Y + num] = freeBusyVisual.FreeBusyIndex;
						}
						num++;
					}
				}
			}
			if (base.DataSource != null)
			{
				int num2 = 0;
				int num3 = 1;
				BusyTypeWrapper busyTypeWrapper = array[num2];
				for (int j = 1; j < array.Length; j++)
				{
					if (array[j] == busyTypeWrapper)
					{
						num3++;
					}
					else
					{
						if (arrayList == null)
						{
							arrayList = new ArrayList();
						}
						arrayList.Add(new FreeBusyVisual(0)
						{
							FreeBusyIndex = busyTypeWrapper,
							Rect = 
							{
								Y = (double)num2,
								Height = (double)num3
							}
						});
						num2 = j;
						num3 = 1;
						busyTypeWrapper = array[j];
					}
				}
				if (arrayList == null)
				{
					arrayList = new ArrayList();
				}
				arrayList.Add(new FreeBusyVisual(0)
				{
					FreeBusyIndex = busyTypeWrapper,
					Rect = 
					{
						Y = (double)num2,
						Height = (double)num3
					}
				});
				this.transformedRolledUpFreeBusy[iDay] = arrayList;
			}
		}

		public static TimeStripMode GetPersistedTimeStripMode(ISessionContext sessionContext)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			int hourIncrement = sessionContext.HourIncrement;
			if (hourIncrement != 15)
			{
				return TimeStripMode.ThirtyMinutes;
			}
			return TimeStripMode.FifteenMinutes;
		}

		public override SanitizedHtmlString FolderDateDescription
		{
			get
			{
				if (this.dateDescription == null)
				{
					DateRange[] dateRanges = base.CalendarAdapter.DateRanges;
					ExDateTime start = dateRanges[0].Start;
					ExDateTime start2 = dateRanges[dateRanges.Length - 1].Start;
					string text = start.ToString("y", base.SessionContext.UserCulture);
					if (dateRanges.Length > 1 && (start.Month != start2.Month || start.Year != start2.Year))
					{
						text = text + " - " + start2.ToString("y", base.SessionContext.UserCulture);
					}
					this.dateDescription = SanitizedHtmlString.GetSanitizedStringWithoutEncoding(text);
				}
				return this.dateDescription;
			}
		}

		public int EventAreaRowCount
		{
			get
			{
				if (this.eventArea != null)
				{
					return this.eventArea.Mapper.RowCount;
				}
				return 0;
			}
		}

		public int VisualCount
		{
			get
			{
				int num = this.eventArea.Count;
				for (int i = 0; i < this.viewDays.Length; i++)
				{
					num += this.viewDays[i].Count;
				}
				return num;
			}
		}

		public TimeStripMode TimeStripMode
		{
			get
			{
				return this.timeStripMode;
			}
		}

		public SchedulingAreaVisualContainer[] ViewDays
		{
			get
			{
				return this.viewDays;
			}
		}

		protected ArrayList[] TransformedRolledUpFreeBusy
		{
			get
			{
				return this.transformedRolledUpFreeBusy;
			}
		}

		public BusyTypeWrapper[][] RowFreeBusy
		{
			get
			{
				return this.rowFreeBusy;
			}
		}

		public EventAreaVisualContainer EventArea
		{
			get
			{
				return this.eventArea;
			}
		}

		public bool IsLimitHit
		{
			get
			{
				return base.RemovedItemCount > 0;
			}
		}

		private SchedulingAreaVisualContainer[] viewDays;

		private FreeBusyVisualContainer[] rolledUpFreeBusy;

		private ArrayList[] transformedRolledUpFreeBusy;

		private EventAreaVisualContainer eventArea;

		private TimeStripMode timeStripMode = TimeStripMode.ThirtyMinutes;

		private BusyTypeWrapper[][] rowFreeBusy;

		private SanitizedHtmlString dateDescription;
	}
}
