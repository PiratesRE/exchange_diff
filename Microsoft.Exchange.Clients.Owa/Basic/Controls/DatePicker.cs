using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class DatePicker : DatePickerBase
	{
		public DatePicker(OwaContext owaContext, StoreObjectId folderId, ExDateTime selectedDate)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			this.folderId = folderId;
			this.selectedDate = (DateTime)selectedDate;
			this.owaContext = owaContext;
			this.showWeekNumbers = owaContext.UserContext.UserOptions.ShowWeekNumbers;
			this.InitializeDates();
		}

		private static void RenderClass(TextWriter writer, ref bool needClassAttribute, string value)
		{
			if (needClassAttribute)
			{
				writer.Write(" class=\"");
				needClassAttribute = false;
			}
			else
			{
				writer.Write(" ");
			}
			writer.Write(value);
		}

		private void InitializeDates()
		{
			this.currentDate = (DateTime)DateTimeUtilities.GetLocalTime();
			Calendar calendar = new GregorianCalendar();
			int weekStartDay = (int)this.owaContext.UserContext.UserOptions.WeekStartDay;
			this.firstDayInCurrentMonth = new DateTime(this.selectedDate.Year, this.selectedDate.Month, 1, 0, 0, 0, 0, calendar);
			int dayOfWeek = (int)calendar.GetDayOfWeek(this.firstDayInCurrentMonth);
			this.offset = dayOfWeek - weekStartDay;
			this.offset = ((this.offset < 0) ? (7 + this.offset) : this.offset);
			this.indexMonthStart = this.offset;
			this.startDate = calendar.AddDays(this.firstDayInCurrentMonth, -this.offset);
			int daysInMonth = calendar.GetDaysInMonth(calendar.GetYear(this.firstDayInCurrentMonth), calendar.GetMonth(this.firstDayInCurrentMonth));
			this.indexMonthEnd = this.offset + daysInMonth - 1;
		}

		public string Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.RenderMonth(writer, null, null);
			return this.errorMessage;
		}

		public string Render(TextWriter writer, SuggestionDayResult[] dayResults, bool workingHoursOnly)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.RenderMonth(writer, dayResults, new bool?(workingHoursOnly));
			return this.errorMessage;
		}

		private void RenderMonth(TextWriter writer, SuggestionDayResult[] dayResults, bool? workingHoursOnly)
		{
			Calendar calendar = new GregorianCalendar();
			int weekStartDay = (int)this.owaContext.UserContext.UserOptions.WeekStartDay;
			this.dates = new DateTime[42];
			int num = -this.offset;
			for (int i = 0; i < 42; i++)
			{
				this.dates[i] = calendar.AddDays(this.firstDayInCurrentMonth, num);
				num++;
			}
			string arg = (workingHoursOnly == null) ? "dpht" : "dphst";
			writer.Write("<table cellpadding=3 cellspacing=0 class=\"{0}\">", arg);
			writer.Write("<tr");
			if (workingHoursOnly == null && this.selectedDate.Year == this.currentDate.Year && this.selectedDate.Month == this.currentDate.Month)
			{
				writer.Write(" class=\"c\"");
			}
			writer.Write("><td align=\"left\">");
			DateTime dateTime = calendar.AddMonths(this.selectedDate, -1);
			writer.Write("<a href=\"#\" onClick=\"return onClkD({0},{1},{2});\" title=\"{3}\"><img src=\"", new object[]
			{
				dateTime.Year,
				dateTime.Month,
				dateTime.Day,
				LocalizedStrings.GetHtmlEncoded(344592200)
			});
			this.owaContext.UserContext.RenderThemeFileUrl(writer, ThemeFileId.PreviousArrow);
			writer.Write("\" alt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(344592200));
			writer.Write("\"></a></td>");
			writer.Write("<td nowrap align=\"center\" class=\"m\">");
			writer.Write(this.selectedDate.ToString("MMMM yyyy"));
			writer.Write("</td><td align=\"right\">");
			DateTime dateTime2 = calendar.AddMonths(this.selectedDate, 1);
			writer.Write("<a href=\"#\" onClick=\"return onClkD({0},{1},{2});\" title=\"{3}\"><img src=\"", new object[]
			{
				dateTime2.Year,
				dateTime2.Month,
				dateTime2.Day,
				LocalizedStrings.GetHtmlEncoded(1040160067)
			});
			this.owaContext.UserContext.RenderThemeFileUrl(writer, ThemeFileId.NextArrow);
			writer.Write("\" alt=\"{0}\"></a></td></tr></table>", LocalizedStrings.GetHtmlEncoded(1040160067));
			string[] oneLetterDayNames = Culture.GetOneLetterDayNames();
			arg = ((workingHoursOnly == null) ? "dp" : "dpst");
			writer.Write("<table cellspacing=0 cellpadding=0 class=\"{0}\"><tr>", arg);
			if (this.showWeekNumbers)
			{
				writer.Write("<td>&nbsp;</td>");
			}
			int j = 0;
			int num2 = weekStartDay;
			while (j < 7)
			{
				writer.Write("<td align=\"center\">");
				writer.Write(oneLetterDayNames[num2 % 7]);
				writer.Write("</td>");
				j++;
				num2++;
			}
			writer.Write("</tr>");
			this.RenderDays(writer, dayResults, workingHoursOnly);
			writer.Write("</table>");
		}

		private void RenderDays(TextWriter writer, SuggestionDayResult[] dayResults, bool? workingHoursOnly)
		{
			Calendar calendar = new GregorianCalendar();
			DayOfWeek weekStartDay = this.owaContext.UserContext.UserOptions.WeekStartDay;
			CalendarWeekRule firstWeekOfYear = this.owaContext.UserContext.FirstWeekOfYear;
			DayOfWeek dayOfWeek = (weekStartDay + 6) % (DayOfWeek)7;
			int num = 0;
			int num2 = this.dates.Length / 7;
			DateTime dateTime = calendar.AddMonths(this.selectedDate, -1);
			DateTime dateTime2 = calendar.AddMonths(this.selectedDate, 1);
			bool flag = false;
			if (workingHoursOnly != null)
			{
				flag = true;
			}
			if (!flag)
			{
				this.GetFreeBusy();
			}
			int num3 = -1;
			DateTime t = calendar.AddDays(this.selectedDate, 6);
			if (dayResults != null)
			{
				num3 = (dayResults[0].Date - this.startDate).Days;
			}
			bool flag2 = false;
			for (int i = 0; i < num2; i++)
			{
				writer.Write("<tr>");
				if (this.showWeekNumbers)
				{
					int weekOfYear = calendar.GetWeekOfYear(this.dates[num], firstWeekOfYear, weekStartDay);
					writer.Write("<td align=\"center\" class=\"wk");
					if (i == num2 - 1)
					{
						writer.Write(" lst");
					}
					writer.Write("\">");
					writer.Write(weekOfYear);
					writer.Write("</td>");
				}
				for (int j = 0; j < 7; j++)
				{
					writer.Write("<td align=\"center\"");
					bool flag3 = true;
					if (flag)
					{
						if (num3 == -1 || num < num3)
						{
							DatePicker.RenderClass(writer, ref flag3, DatePicker.SuggestionQualityStyles[4]);
						}
						else
						{
							SuggestionDayResult suggestionDayResult = dayResults[num - num3];
							if (suggestionDayResult != null)
							{
								Suggestion[] suggestionArray = suggestionDayResult.SuggestionArray;
								SuggestionQuality suggestionQuality;
								if (0 < suggestionArray.Length)
								{
									suggestionQuality = suggestionArray[0].SuggestionQuality;
								}
								else if (!this.owaContext.UserContext.WorkingHours.IsWorkDay(dayResults[num - num3].Date.DayOfWeek))
								{
									suggestionQuality = (SuggestionQuality)4;
								}
								else
								{
									suggestionQuality = SuggestionQuality.Poor;
								}
								DatePicker.RenderClass(writer, ref flag3, DatePicker.SuggestionQualityStyles[(int)suggestionQuality]);
							}
						}
					}
					else if (this.freeBusyData[num] != '0')
					{
						DatePicker.RenderClass(writer, ref flag3, "fb");
					}
					if (this.indexMonthStart > num || this.indexMonthEnd < num)
					{
						DatePicker.RenderClass(writer, ref flag3, "pn");
					}
					if (this.dates[num].ToShortDateString() == this.currentDate.ToShortDateString())
					{
						DatePicker.RenderClass(writer, ref flag3, "td");
					}
					if (!flag)
					{
						if (this.dates[num].Date == this.selectedDate.Date)
						{
							DatePicker.RenderClass(writer, ref flag3, "sd");
						}
					}
					else if (this.dates[num] >= this.selectedDate && this.dates[num] <= t)
					{
						if (workingHoursOnly != null && workingHoursOnly == true)
						{
							if (this.owaContext.UserContext.WorkingHours.IsWorkDay(this.dates[num].DayOfWeek) || num == 0)
							{
								if (!flag2)
								{
									DatePicker.RenderClass(writer, ref flag3, "sd");
									DatePicker.RenderClass(writer, ref flag3, "tb");
									flag2 = true;
								}
								else
								{
									DatePicker.RenderClass(writer, ref flag3, "tb");
								}
							}
							if (this.dates[num].ToShortDateString() == t.ToShortDateString() && flag2)
							{
								DatePicker.RenderClass(writer, ref flag3, "tb");
								DatePicker.RenderClass(writer, ref flag3, "r");
								flag2 = false;
							}
							if ((!this.owaContext.UserContext.WorkingHours.IsWorkDay(calendar.AddDays(this.dates[num], 1).DayOfWeek) || j == 6) && flag2)
							{
								DatePicker.RenderClass(writer, ref flag3, "tb");
								DatePicker.RenderClass(writer, ref flag3, "r");
								flag2 = false;
							}
						}
						else
						{
							if (!flag2)
							{
								DatePicker.RenderClass(writer, ref flag3, "sd");
								DatePicker.RenderClass(writer, ref flag3, "tb");
								flag2 = true;
							}
							else
							{
								DatePicker.RenderClass(writer, ref flag3, "tb");
							}
							if (this.dates[num].ToShortDateString() == t.ToShortDateString() && flag2)
							{
								DatePicker.RenderClass(writer, ref flag3, "tb");
								DatePicker.RenderClass(writer, ref flag3, "r");
								flag2 = false;
							}
							if (this.dates[num].DayOfWeek == dayOfWeek)
							{
								DatePicker.RenderClass(writer, ref flag3, "tb");
								DatePicker.RenderClass(writer, ref flag3, "r");
								flag2 = false;
							}
						}
					}
					if (!flag3)
					{
						writer.Write("\"");
					}
					writer.Write("><a href=\"#\" onClick=\"return onClkD(");
					if (this.indexMonthStart > num)
					{
						writer.Write("{0},{1},", dateTime.Year, dateTime.Month);
					}
					else if (this.indexMonthEnd < num)
					{
						writer.Write("{0},{1},", dateTime2.Year, dateTime2.Month);
					}
					else
					{
						writer.Write("{0},{1},", this.selectedDate.Year, this.selectedDate.Month);
					}
					writer.Write("{0})\">", this.dates[num].Day);
					string daysFormat = DateTimeUtilities.GetDaysFormat(this.owaContext.UserContext.UserOptions.DateFormat);
					writer.Write(this.dates[num].ToString(daysFormat ?? "%d"));
					writer.Write("</a></td>");
					num++;
				}
				writer.Write("</tr>");
			}
		}

		private void GetFreeBusy()
		{
			DateTime dateTime = this.dates[0];
			DateTime arg = this.dates[41];
			Duration timeWindow = new Duration(dateTime, arg.AddDays(1.0));
			ExTraceGlobals.CalendarTracer.TraceDebug<DateTime, DateTime>((long)this.GetHashCode(), "Getting free/busy data from {0} to {1}", dateTime, arg);
			OwaStoreObjectId calendarFolderId = OwaStoreObjectId.CreateFromSessionFolderId(this.owaContext.UserContext, this.owaContext.UserContext.MailboxSession, this.folderId);
			this.freeBusyData = Utilities.GetFreeBusyDataForDatePicker(timeWindow, calendarFolderId, this.owaContext.UserContext);
		}

		private static readonly string[] SuggestionQualityStyles = new string[]
		{
			"grt",
			"gd",
			"fr",
			"por",
			"pst"
		};

		private StoreObjectId folderId;

		private DateTime selectedDate;

		private static int[] specialLocale = new int[]
		{
			1025,
			2049,
			3073,
			4097,
			5121,
			6145,
			7169,
			8193,
			9217,
			10241,
			11265,
			12289,
			13313,
			14337,
			15361,
			16385
		};

		private static int[] specialLocale2 = new int[]
		{
			1028,
			2052,
			4100,
			5124
		};

		private DateTime currentDate;

		private OwaContext owaContext;

		private DateTime startDate;

		private int indexMonthStart;

		private int indexMonthEnd;

		private int offset;

		private DateTime[] dates;

		private string freeBusyData;

		private string errorMessage = string.Empty;

		private bool showWeekNumbers;

		private DateTime firstDayInCurrentMonth;

		private enum Month
		{
			Previous,
			Current,
			Next
		}

		private enum DayNameIndex
		{
			Zero,
			Two,
			Last
		}
	}
}
