using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PrintMonthlyView : MonthlyViewBase, IPrintCalendarViewControl
	{
		public static ExDateTime[] GetEffectiveDates(CalendarAdapterBase calendarAdapter, ISessionContext sessionContext, bool workingDayOnly)
		{
			List<ExDateTime> list = new List<ExDateTime>();
			bool flag = false;
			int workingDays = PrintMonthlyView.GetWorkingDays(calendarAdapter, sessionContext);
			for (int i = 0; i < calendarAdapter.DateRanges.Length; i++)
			{
				ExDateTime start = calendarAdapter.DateRanges[i].Start;
				if (start.Day == 1)
				{
					if (flag)
					{
						break;
					}
					flag = true;
				}
				if (flag && PrintMonthlyView.ShouldRenderDay(start, workingDays, workingDayOnly))
				{
					list.Add(start);
				}
			}
			return list.ToArray();
		}

		private static bool ShouldRenderDay(ExDateTime day, int workDays, bool workingDayOnly)
		{
			return DateTimeUtilities.IsWorkingDay(day, workDays) || !workingDayOnly;
		}

		public PrintMonthlyView(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter, bool workingDayOnly) : base(sessionContext, calendarAdapter)
		{
			this.showWeekNumbers = sessionContext.ShowWeekNumbers;
			this.calendar = new GregorianCalendar();
			this.daysFormat = (DateTimeUtilities.GetDaysFormat(sessionContext.DateFormat) ?? "%d");
			this.firstDayFormat = "MMM %d";
			if (CalendarUtilities.FullMonthNameRequired(sessionContext.UserCulture))
			{
				this.firstDayFormat = string.Format("MMMM {0}", this.daysFormat);
			}
			this.workingDayOnly = workingDayOnly;
			this.sessionContext = sessionContext;
			this.workDays = PrintMonthlyView.GetWorkingDays(calendarAdapter, sessionContext);
		}

		public static int GetWorkingDays(CalendarAdapterBase calendarAdapter, ISessionContext sessionContext)
		{
			int result;
			if (calendarAdapter.DataSource.SharedType == SharedType.AnonymousAccess)
			{
				result = sessionContext.WorkingHours.WorkDays;
			}
			else if (calendarAdapter is CalendarAdapter)
			{
				CalendarFolder folder = ((CalendarAdapter)calendarAdapter).Folder;
				OwaStoreObjectId folderId = ((CalendarAdapter)calendarAdapter).FolderId;
				if (folder != null && Utilities.IsOtherMailbox(folder) && folderId.IsGSCalendar)
				{
					result = calendarAdapter.DataSource.WorkingHours.WorkDays;
				}
				else
				{
					result = sessionContext.WorkingHours.WorkDays;
				}
			}
			else
			{
				result = sessionContext.WorkingHours.WorkDays;
			}
			return result;
		}

		public void RenderView(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			Dictionary<ExDateTime, List<PrintCalendarVisual>> dictionary = new Dictionary<ExDateTime, List<PrintCalendarVisual>>();
			for (int i = 0; i < base.VisualContainer.Count; i++)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)base.VisualContainer[i];
				if (!base.IsItemRemoved(eventAreaVisual.DataIndex))
				{
					int num = (int)eventAreaVisual.Rect.X;
					if (num >= 0 && num < base.DateRanges.Length)
					{
						for (int j = num; j < (int)(eventAreaVisual.Rect.X + eventAreaVisual.Rect.Width); j++)
						{
							ExDateTime date = base.DateRanges[j].Start.Date;
							if (this.ShouldRenderDay(date))
							{
								if (!dictionary.ContainsKey(date))
								{
									dictionary.Add(date, new List<PrintCalendarVisual>());
								}
								dictionary[date].Add(new PrintMonthlyVisual(base.SessionContext, eventAreaVisual, base.DataSource, j == (int)eventAreaVisual.Rect.X));
							}
						}
					}
				}
			}
			writer.Write("<table>");
			this.RenderDayHeader(writer);
			bool flag = false;
			for (int k = 0; k < base.DateRanges.Length / 7; k++)
			{
				int num2 = k * 7;
				writer.Write("<tr>");
				this.RenderWeekNumber(writer, num2);
				for (int l = 0; l < 7; l++)
				{
					ExDateTime date2 = base.DateRanges[num2 + l].Start.Date;
					if (date2.Day == 1)
					{
						flag = !flag;
					}
					if (this.ShouldRenderDay(date2))
					{
						this.RenderCell(writer, date2, dictionary.ContainsKey(date2) ? dictionary[date2] : null, flag);
					}
				}
				writer.Write("</tr>");
			}
			writer.Write("</table>");
		}

		private bool ShouldRenderDay(ExDateTime day)
		{
			return PrintMonthlyView.ShouldRenderDay(day, this.workDays, this.workingDayOnly);
		}

		private void RenderDayHeader(TextWriter writer)
		{
			writer.Write("<tr class=\"dayHeader\">");
			writer.Write("<td class=\"");
			writer.Write(this.showWeekNumbers ? "weekNumber" : "weekNumberPlaceHolder");
			writer.Write("\"></td>");
			for (int i = 0; i < 7; i++)
			{
				ExDateTime start = base.DateRanges[i].Start;
				int dayOfWeek = (int)start.DayOfWeek;
				if (this.ShouldRenderDay(start))
				{
					writer.Write("<td class=\"weekDayName\">");
					writer.Write(base.SessionContext.UserCulture.DateTimeFormat.DayNames[dayOfWeek]);
					writer.Write("</td>");
				}
			}
			writer.Write("</tr>");
		}

		private void RenderWeekNumber(TextWriter writer, int startDayIndex)
		{
			writer.Write("<td>");
			if (this.showWeekNumbers)
			{
				writer.Write(this.calendar.GetWeekOfYear((DateTime)base.DateRanges[startDayIndex].Start, base.SessionContext.FirstWeekOfYear, base.SessionContext.WeekStartDay));
			}
			else
			{
				writer.Write("&nbsp;");
			}
			writer.Write("</td>");
		}

		private void RenderCell(TextWriter writer, ExDateTime date, IList<PrintCalendarVisual> visuals, bool inThisMonth)
		{
			writer.Write("<td");
			if (date.Equals(ExDateTime.Now.Date))
			{
				writer.Write(" class=\"today\"");
			}
			writer.Write(">");
			writer.Write("<div class=\"printVisualContainer\">");
			if (!inThisMonth)
			{
				PrintCalendarVisual.RenderBackground(writer, "bgNotInThisMonth");
			}
			writer.Write("<div class=\"monthlyViewDay\">");
			writer.Write("<div class=\"monthlyViewDayName\">");
			writer.Write(date.ToString((date.Day == 1) ? this.firstDayFormat : this.daysFormat, base.SessionContext.UserCulture));
			writer.Write("</div>");
			if (visuals != null)
			{
				foreach (PrintCalendarVisual printCalendarVisual in visuals)
				{
					printCalendarVisual.Render(writer);
				}
			}
			writer.Write("</div>");
			writer.Write("</div></td>");
		}

		public ExDateTime[] GetEffectiveDates()
		{
			return PrintMonthlyView.GetEffectiveDates(base.CalendarAdapter, this.sessionContext, this.workingDayOnly);
		}

		public string DateDescription
		{
			get
			{
				return this.monthName;
			}
		}

		public string CalendarName
		{
			get
			{
				return base.CalendarAdapter.CalendarTitle;
			}
		}

		public const int HeaderHeight = 18;

		private Calendar calendar;

		private bool showWeekNumbers;

		private string daysFormat;

		private string firstDayFormat;

		private bool workingDayOnly;

		private int workDays;

		private ISessionContext sessionContext;
	}
}
