using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class DailyViewPayloadWriter : CalendarViewPayloadWriter
	{
		public DailyViewPayloadWriter(ISessionContext sessionContext, TextWriter output, ExDateTime[] days, params CalendarAdapterBase[] adapters) : base(sessionContext, output)
		{
			if (adapters == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			if (days == null)
			{
				throw new ArgumentNullException("days");
			}
			this.days = days;
			this.adapters = adapters;
		}

		public DailyViewPayloadWriter(ISessionContext sessionContext, TextWriter output, DailyView dailyView) : base(sessionContext, output)
		{
			if (dailyView == null)
			{
				throw new ArgumentNullException("dailyView");
			}
			this.dailyView = dailyView;
		}

		public override void Render(int viewWidth, CalendarViewType viewType, ReadingPanePosition readingPanePosition, ReadingPanePosition requestReadingPanePosition)
		{
			SanitizedHtmlString sharedData = null;
			base.Output.Write("{\"rgCalendar\":[");
			if (this.dailyView != null)
			{
				this.RenderCalendar(0, this.dailyView);
				sharedData = this.GetSharedDataString(this.dailyView, viewWidth, viewType, readingPanePosition, requestReadingPanePosition);
			}
			else
			{
				base.RenderCalendars(this.adapters, this.days, delegate(CalendarAdapterBase calendarAdapter, int index)
				{
					if (index > 0)
					{
						this.Output.Write(",");
					}
					DailyView view = new DailyView(this.SessionContext, calendarAdapter);
					if (sharedData == null)
					{
						sharedData = this.GetSharedDataString(view, viewWidth, viewType, readingPanePosition, requestReadingPanePosition);
					}
					this.RenderCalendar(index, view);
				});
			}
			base.Output.Write("]");
			if (sharedData != null)
			{
				base.Output.Write(",");
				base.Output.Write(sharedData);
			}
			base.Output.Write("}");
		}

		private SanitizedHtmlString GetSharedDataString(DailyView view, int viewWidth, CalendarViewType viewType, ReadingPanePosition readingPanePosition, ReadingPanePosition requestReadingPanePosition)
		{
			SanitizedHtmlString result;
			using (SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>())
			{
				base.RenderValue(sanitizingStringWriter, "sFId", base.SessionContext.CalendarFolderOwaIdString);
				base.RenderValue(sanitizingStringWriter, "sTimeZone", base.SessionContext.TimeZone.Id);
				base.RenderValue(sanitizingStringWriter, "iVT", (int)viewType);
				base.RenderValue(sanitizingStringWriter, "iVW", viewWidth);
				base.RenderValue(sanitizingStringWriter, "iRowH", 24);
				base.RenderValue(sanitizingStringWriter, "iRRP", (int)requestReadingPanePosition);
				base.RenderValue(sanitizingStringWriter, "iNRP", (int)readingPanePosition);
				this.RenderTimeSwoopVariables(sanitizingStringWriter, view);
				sanitizingStringWriter.Write("\"rgDay\":[");
				base.RenderDay(sanitizingStringWriter, view.DateRanges, viewType);
				sanitizingStringWriter.Write("]");
				result = sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
			}
			return result;
		}

		private void RenderCalendar(int index, DailyView view)
		{
			base.Output.Write("{");
			base.RenderCalendarProperties(view);
			base.RenderValue(base.Output, "cERow", view.EventAreaRowCount);
			base.Output.Write("\"rgCalDay\":[");
			this.RenderCalDay(view);
			base.Output.Write("],\"rgItm\":[");
			base.RenderData(view, view.SelectedItemId);
			base.Output.Write("],\"rgVisData\":[");
			this.RenderScheduleAreaVis(index, view);
			base.Output.Write("],\"rgEvtData\":[");
			base.RenderEventAreaVisual(index, view, view.EventArea);
			base.Output.Write("]}");
		}

		private void RenderCalDay(DailyView view)
		{
			for (int i = 0; i < view.ViewDays.Length; i++)
			{
				if (i > 0)
				{
					base.Output.Write(",");
				}
				SchedulingAreaVisualContainer schedulingAreaVisualContainer = view.ViewDays[i];
				base.Output.Write("new CalendarDay(");
				base.Output.Write(i.ToString(CultureInfo.InvariantCulture));
				base.Output.Write(", new Array(");
				int num = 0;
				for (int j = 0; j < view.RowFreeBusy[i].Length; j++)
				{
					if (j > 0)
					{
						base.Output.Write(",");
					}
					BusyTypeWrapper busyTypeWrapper = view.RowFreeBusy[i][j];
					int num2 = (int)busyTypeWrapper;
					if (num2 > num)
					{
						num = num2;
					}
					base.Output.Write(num2);
				}
				base.Output.Write("),");
				base.Output.Write(num);
				int minutesPerCell = (view.TimeStripMode == TimeStripMode.FifteenMinutes) ? 15 : 30;
				WorkingHours workingHours = view.DataSource.WorkingHours;
				WorkingHours.WorkingPeriod[] workingHoursOnDay = workingHours.GetWorkingHoursOnDay(schedulingAreaVisualContainer.DateRange.Start);
				if (workingHoursOnDay.Length > 0)
				{
					ExDateTime date = schedulingAreaVisualContainer.DateRange.Start.Date;
					base.Output.Write(",");
					base.Output.Write(this.GetCellIndex(workingHoursOnDay[0].Start, minutesPerCell, date));
					base.Output.Write(",");
					base.Output.Write(this.GetCellIndex(workingHoursOnDay[0].End, minutesPerCell, date));
					if (workingHoursOnDay.Length > 1)
					{
						base.Output.Write(",");
						base.Output.Write(this.GetCellIndex(workingHoursOnDay[1].Start, minutesPerCell, date));
						base.Output.Write(",");
						base.Output.Write(this.GetCellIndex(workingHoursOnDay[1].End, minutesPerCell, date));
					}
				}
				base.Output.Write(")");
			}
		}

		private int GetCellIndex(ExDateTime time, int minutesPerCell, ExDateTime day)
		{
			int num;
			if (time.Date < day.Date)
			{
				num = 0;
			}
			else if (time.Date > day.Date)
			{
				num = 1440;
			}
			else
			{
				num = (int)time.TimeOfDay.TotalMinutes;
			}
			return num / minutesPerCell;
		}

		private void RenderScheduleAreaVis(int iCalIdx, DailyView view)
		{
			bool flag = true;
			for (int i = 0; i < view.ViewDays.Length; i++)
			{
				SchedulingAreaVisualContainer schedulingAreaVisualContainer = view.ViewDays[i];
				for (int j = 0; j < schedulingAreaVisualContainer.Count; j++)
				{
					SchedulingAreaVisual schedulingAreaVisual = (SchedulingAreaVisual)schedulingAreaVisualContainer[j];
					if (!view.IsItemRemoved(schedulingAreaVisual.DataIndex))
					{
						if (!flag)
						{
							base.Output.Write(",");
						}
						flag = false;
						base.Output.Write("new VisData(");
						base.Output.Write(iCalIdx);
						base.Output.Write(",");
						int num = (base.ItemIndex != null) ? ((int)base.ItemIndex[schedulingAreaVisual.DataIndex]) : schedulingAreaVisual.DataIndex;
						base.Output.Write(num);
						base.Output.Write(",");
						base.Output.Write(schedulingAreaVisual.Rect.X.ToString("0.####", CultureInfo.InvariantCulture));
						base.Output.Write(",");
						base.Output.Write(schedulingAreaVisual.Rect.Y.ToString("0.####", CultureInfo.InvariantCulture));
						base.Output.Write(",");
						base.Output.Write(schedulingAreaVisual.Rect.Width.ToString("0.####", CultureInfo.InvariantCulture));
						base.Output.Write(",");
						base.Output.Write(schedulingAreaVisual.Rect.Height.ToString("0.####", CultureInfo.InvariantCulture));
						base.Output.Write(",");
						base.Output.Write("0,0");
						if (num == base.SelectedItemIndex)
						{
							base.Output.Write(",1,");
						}
						else
						{
							base.Output.Write(",0,");
						}
						base.Output.Write(i);
						base.Output.Write(")");
					}
				}
			}
		}

		private void RenderTimeSwoopVariables(TextWriter writer, DailyView view)
		{
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			ExDateTime date = localTime.Date;
			double num;
			if (!DateRange.IsDateInRangeArray(localTime, view.DateRanges))
			{
				num = -1.0;
			}
			else
			{
				num = (double)(((DateTime)localTime).Ticks - ((DateTime)date).Ticks) / 600000000.0;
			}
			base.RenderValue(writer, "flTSW", num, "0.####");
			if (num >= 0.0)
			{
				double value = -1.0;
				double num2 = -1.0;
				DaylightTime daylightChanges = base.SessionContext.TimeZone.GetDaylightChanges(localTime.Year);
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (localTime.Day == daylightChanges.Start.Day && localTime.Month == daylightChanges.Start.Month)
				{
					value = (double)(ExTimeZone.UtcTimeZone.ConvertDateTime(new ExDateTime(base.SessionContext.TimeZone, daylightChanges.Start)).UtcTicks - utcNow.UtcTicks) / 600000000.0;
				}
				if (localTime.Day == daylightChanges.End.Day && localTime.Month == daylightChanges.End.Month)
				{
					num2 = (double)(ExTimeZone.UtcTimeZone.ConvertDateTime(new ExDateTime(base.SessionContext.TimeZone, daylightChanges.End)).UtcTicks - utcNow.UtcTicks) / 600000000.0;
					num2 -= daylightChanges.Delta.TotalMinutes;
				}
				base.RenderValue(writer, "flDSO", value, "0.####");
				base.RenderValue(writer, "flDEO", num2, "0.####");
				base.RenderValue(writer, "flDSD", daylightChanges.Delta.TotalMinutes, "0.####");
			}
		}

		private const string StrFormat = "0.####";

		private readonly CalendarAdapterBase[] adapters;

		private ExDateTime[] days;

		private DailyView dailyView;
	}
}
