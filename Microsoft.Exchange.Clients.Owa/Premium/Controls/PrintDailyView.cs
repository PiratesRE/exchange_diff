using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PrintDailyView : DailyViewBase, IPrintCalendarViewControl
	{
		public static ExDateTime[] GetEffectiveDates(DateRange[] dateRanges)
		{
			ExDateTime[] array = new ExDateTime[dateRanges.Length];
			for (int i = 0; i < dateRanges.Length; i++)
			{
				array[i] = dateRanges[i].Start;
			}
			return array;
		}

		public PrintDailyView(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter, int startTime, int endTime, bool renderNotes) : base(sessionContext, calendarAdapter)
		{
			this.startTime = startTime;
			this.endTime = endTime;
			this.renderNotes = renderNotes;
			this.printDateRange = calendarAdapter.DateRanges;
		}

		public void RenderView(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table>");
			writer.Write("<tr class=\"dayHeader\">");
			writer.Write("<td class=\"timeStrip\">&nbsp;</td>");
			this.RenderDayHeader(writer);
			writer.Write("</tr>");
			if (base.EventAreaRowCount > 0)
			{
				this.RenderEventArea(writer);
			}
			writer.Write("<tr><td><table class=\"dailyViewInnerTable\">");
			for (int i = this.startTime; i < this.endTime; i++)
			{
				writer.Write("<tr><td>");
				DateTime dateTime = new DateTime(2000, 1, 1, i, 0, 0);
				writer.Write(dateTime.ToString("t", base.SessionContext.UserCulture));
				writer.Write("</td></tr>");
			}
			writer.Write("</table></td>");
			for (int j = 0; j < this.printDateRange.Length; j++)
			{
				ExDateTime date = this.printDateRange[j].Start.Date;
				WorkingHours.WorkingPeriod[] workingHoursOnDay = base.DataSource.WorkingHours.GetWorkingHoursOnDay(date);
				writer.Write("<td><div class=\"printVisualContainer\">");
				writer.Write("<table class=\"dailyViewInnerTable\">");
				for (int k = this.startTime; k < this.endTime; k++)
				{
					writer.Write("<tr><td><div class=\"printVisualContainer bgContainer\">");
					ExDateTime t = new ExDateTime(date.TimeZone, date.Year, date.Month, date.Day, k, 30, 0);
					if ((workingHoursOnDay.Length > 0 && t >= workingHoursOnDay[0].Start && t <= workingHoursOnDay[0].End) || (workingHoursOnDay.Length == 2 && t >= workingHoursOnDay[1].Start && t <= workingHoursOnDay[1].End))
					{
						PrintCalendarVisual.RenderBackground(writer, "bgWorkingHour");
					}
					else
					{
						PrintCalendarVisual.RenderBackground(writer, "bgFreeHour");
					}
					writer.Write("</div>");
					if (base.SessionContext.BrowserType == BrowserType.IE)
					{
						writer.Write("&nbsp;");
					}
					writer.Write("</td></tr>");
				}
				writer.Write("</table>");
				for (int l = 0; l < base.ViewDays[j].Count; l++)
				{
					SchedulingAreaVisual schedulingAreaVisual = (SchedulingAreaVisual)base.ViewDays[j][l];
					if (!base.IsItemRemoved(schedulingAreaVisual.DataIndex))
					{
						new PrintSchedulingAreaVisual(base.SessionContext, schedulingAreaVisual, base.DataSource, this.startTime, this.endTime).Render(writer);
					}
				}
				writer.Write("</div>");
				writer.Write("</td>");
			}
			writer.Write("</tr></table>");
		}

		private void RenderDayHeader(TextWriter writer)
		{
			string format = DateTimeUtilities.GetDaysFormat(base.SessionContext.DateFormat) ?? "%d";
			for (int i = 0; i < this.printDateRange.Length; i++)
			{
				ExDateTime start = this.printDateRange[i].Start;
				int dayOfWeek = (int)start.DayOfWeek;
				writer.Write("<td class=\"dayHeader\"><table class=\"innerTable\">");
				writer.Write("<tr><td class=\"dayName\">");
				writer.Write(start.ToString(format, base.SessionContext.UserCulture));
				writer.Write("</td><td class=\"weekdayName\">");
				writer.Write(base.SessionContext.UserCulture.DateTimeFormat.DayNames[dayOfWeek]);
				writer.Write("</td></tr></table></td>");
			}
			if (this.renderNotes)
			{
				writer.Write("<td class=\"notes\">");
				writer.Write(SanitizedHtmlString.FromStringId(331392989));
				writer.Write("</td>");
			}
		}

		private void RenderEventArea(TextWriter writer)
		{
			writer.Write("<tr><td class=\"allDay\">");
			writer.Write(SanitizedHtmlString.FromStringId(1607325677));
			writer.Write("</td><td colspan=\"");
			writer.Write(this.printDateRange.Length);
			writer.Write("\" style=\"height: ");
			writer.Write(20 * base.EventAreaRowCount);
			writer.Write("px;\"><div class=\"printVisualContainer\">");
			for (int i = 0; i < base.EventArea.Count; i++)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)base.EventArea[i];
				if (!base.IsItemRemoved(eventAreaVisual.DataIndex))
				{
					new PrintEventAreaVisual(base.SessionContext, eventAreaVisual, base.DataSource, this.printDateRange.Length).Render(writer);
				}
			}
			writer.Write("</div></td></tr>");
		}

		public ExDateTime[] GetEffectiveDates()
		{
			return PrintDailyView.GetEffectiveDates(this.printDateRange);
		}

		public override int MaxEventAreaRows
		{
			get
			{
				return 22;
			}
		}

		public override int MaxItemsPerView
		{
			get
			{
				return 300;
			}
		}

		public override int MaxConflictingItems
		{
			get
			{
				return 25;
			}
		}

		public OwaStoreObjectId SelectedItemId { get; set; }

		public int Count
		{
			get
			{
				return base.VisualCount;
			}
		}

		public string DateDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				ExDateTime start = this.printDateRange[0].Start;
				ExDateTime start2 = this.printDateRange[this.printDateRange.Length - 1].Start;
				if (this.printDateRange.Length == 1)
				{
					stringBuilder.Append(start.ToString("D", base.SessionContext.UserCulture));
				}
				else
				{
					stringBuilder.Append(start.ToString("d", base.SessionContext.UserCulture));
					stringBuilder.Append(" - ");
					stringBuilder.Append(start2.ToString("d", base.SessionContext.UserCulture));
				}
				return stringBuilder.ToString();
			}
		}

		public string CalendarName
		{
			get
			{
				return base.CalendarAdapter.CalendarTitle;
			}
		}

		private int startTime;

		private int endTime;

		private bool renderNotes;

		private DateRange[] printDateRange;
	}
}
