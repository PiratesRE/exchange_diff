using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class MonthlyViewPayloadWriter : CalendarViewPayloadWriter
	{
		public MonthlyViewPayloadWriter(ISessionContext sessionContext, TextWriter output, ExDateTime[] days, params CalendarAdapterBase[] adapters) : base(sessionContext, output)
		{
			if (adapters == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			this.days = days;
			this.adapters = adapters;
		}

		public MonthlyViewPayloadWriter(ISessionContext sessionContext, TextWriter output, MonthlyView monthlyView) : base(sessionContext, output)
		{
			if (monthlyView == null)
			{
				throw new ArgumentNullException("monthlyView");
			}
			this.monthlyView = monthlyView;
		}

		public override void Render(int viewWidth, CalendarViewType viewType, ReadingPanePosition readingPanePosition, ReadingPanePosition requestReadingPanePosition)
		{
			SanitizedHtmlString sharedData = null;
			base.Output.Write("{\"rgCalendar\":[");
			if (this.monthlyView != null)
			{
				this.RenderCalendar(0, this.monthlyView);
				sharedData = this.GetSharedDataString(this.monthlyView, requestReadingPanePosition);
			}
			else
			{
				base.RenderCalendars(this.adapters, this.days, delegate(CalendarAdapterBase calendarAdapter, int index)
				{
					if (index > 0)
					{
						this.Output.Write(",");
					}
					MonthlyView view = new MonthlyView(this.SessionContext, calendarAdapter);
					if (sharedData == null)
					{
						sharedData = this.GetSharedDataString(view, requestReadingPanePosition);
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

		private SanitizedHtmlString GetSharedDataString(MonthlyView view, ReadingPanePosition requestReadingPanePosition)
		{
			SanitizedHtmlString result;
			using (SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>())
			{
				base.RenderValue(sanitizingStringWriter, "sFId", base.SessionContext.CalendarFolderOwaIdString);
				base.RenderValue(sanitizingStringWriter, "sTimeZone", base.SessionContext.TimeZone.Id);
				base.RenderValue(sanitizingStringWriter, "iRowH", 20);
				base.RenderValue(sanitizingStringWriter, "iVT", 4);
				base.RenderValue(sanitizingStringWriter, "iVW", 0);
				base.RenderValue(sanitizingStringWriter, "sPrevMonth", view.PreviousMonthName);
				base.RenderValue(sanitizingStringWriter, "sCurrMonth", view.CurrentMonthName);
				base.RenderValue(sanitizingStringWriter, "sNextMonth", view.NextMonthName);
				base.RenderValue(sanitizingStringWriter, "iRRP", (int)requestReadingPanePosition);
				base.RenderValue(sanitizingStringWriter, "iNRP", 0);
				this.RenderWeekNumbers(sanitizingStringWriter, view);
				sanitizingStringWriter.Write("\"rgDay\":[");
				base.RenderDay(sanitizingStringWriter, view.DateRanges, CalendarViewType.Monthly);
				sanitizingStringWriter.Write("]");
				result = sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
			}
			return result;
		}

		private void RenderCalendar(int index, MonthlyView view)
		{
			base.Output.Write("{");
			base.RenderCalendarProperties(view);
			this.RenderDayStyles(view);
			base.Output.Write("\"rgItm\":[");
			base.RenderData(view, view.SelectedItemId);
			base.Output.Write("],\"rgEvtData\":[");
			base.RenderEventAreaVisual(index, view, view.VisualContainer);
			base.Output.Write("]}");
		}

		private void RenderWeekNumbers(TextWriter writer, MonthlyView view)
		{
			if (base.SessionContext.ShowWeekNumbers)
			{
				Calendar calendar = new GregorianCalendar();
				writer.Write("\"rgWkNm\":[");
				int num = 0;
				int num2 = 0;
				int num3 = view.DateRanges.Length / 7;
				for (int i = 0; i < num3; i++)
				{
					ExDateTime start = view.DateRanges[i * 7].Start;
					if (start.Year != num)
					{
						num = start.Year;
						int weekOfYear = calendar.GetWeekOfYear((DateTime)start, base.SessionContext.FirstWeekOfYear, base.SessionContext.WeekStartDay);
						if (num2++ > 0)
						{
							writer.Write(',');
						}
						writer.Write(weekOfYear);
					}
				}
				writer.Write("],");
				return;
			}
			writer.Write("\"rgWkNm\":0,");
		}

		private void RenderDayStyles(MonthlyView view)
		{
			bool[] array = new bool[view.DayCount];
			int[] array2 = new int[view.DayCount];
			bool flag = true;
			for (int i = 0; i < view.DayCount; i++)
			{
				if (view.DateRanges[i].Start.Day == 1)
				{
					flag = !flag;
				}
				array[i] = flag;
			}
			int j = 0;
			while (j < view.VisualContainer.Count)
			{
				CalendarVisual calendarVisual = view.VisualContainer[j];
				if (calendarVisual.Rect.Width != 1.0)
				{
					goto IL_B7;
				}
				ExDateTime startTime = view.DataSource.GetStartTime(calendarVisual.DataIndex);
				ExDateTime endTime = view.DataSource.GetEndTime(calendarVisual.DataIndex);
				if ((endTime - startTime).Days != 0)
				{
					goto IL_B7;
				}
				IL_10E:
				j++;
				continue;
				IL_B7:
				int wrappedBusyType = (int)view.DataSource.GetWrappedBusyType(calendarVisual.DataIndex);
				int num = (int)calendarVisual.Rect.X;
				while ((double)num < calendarVisual.Rect.X + calendarVisual.Rect.Width)
				{
					if (array2[num] < wrappedBusyType)
					{
						array2[num] = wrappedBusyType;
					}
					num++;
				}
				goto IL_10E;
			}
			base.Output.Write("\"rgDayS\":[");
			for (int k = 0; k < view.DayCount; k++)
			{
				if (k == 0)
				{
					base.Output.Write('"');
				}
				else
				{
					base.Output.Write(",\"");
				}
				base.Output.Write(array[k] ? 'o' : 'i');
				base.Output.Write('n');
				switch (array2[k])
				{
				case 1:
					base.Output.Write('t');
					break;
				case 2:
					base.Output.Write('b');
					break;
				case 3:
					base.Output.Write('o');
					break;
				default:
					base.Output.Write('f');
					break;
				}
				base.Output.Write('"');
			}
			base.Output.Write("],");
		}

		private readonly CalendarAdapterBase[] adapters;

		private ExDateTime[] days;

		private MonthlyView monthlyView;
	}
}
