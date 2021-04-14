using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintEventList : IPrintCalendarViewControl
	{
		public PrintEventList(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter, CalendarViewType viewType, bool workingDayOnly)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (calendarAdapter == null)
			{
				throw new ArgumentNullException("calendarAdapter");
			}
			this.calendarAdapter = calendarAdapter;
			this.sessionContext = sessionContext;
			this.workingDayOnly = workingDayOnly;
			this.viewType = viewType;
			this.visuals = new Dictionary<ExDateTime, List<PrintCalendarVisual>>();
			foreach (ExDateTime exDateTime in this.GetEffectiveDates())
			{
				this.visuals.Add(exDateTime.Date, new List<PrintCalendarVisual>());
			}
			for (int j = 0; j < calendarAdapter.DataSource.Count; j++)
			{
				ExDateTime startTime = calendarAdapter.DataSource.GetStartTime(j);
				ExDateTime endTime = calendarAdapter.DataSource.GetEndTime(j);
				ExDateTime date = startTime.Date;
				ExDateTime date2 = endTime.Date;
				ExDateTime exDateTime2 = date;
				while (exDateTime2 <= date2 && (!(exDateTime2 >= endTime) || !(startTime != endTime)))
				{
					if (this.visuals.ContainsKey(exDateTime2))
					{
						this.visuals[exDateTime2].Add(this.GetVisual(j, date.Equals(exDateTime2)));
					}
					exDateTime2 = exDateTime2.IncrementDays(1);
				}
			}
			foreach (List<PrintCalendarVisual> list in this.visuals.Values)
			{
				list.Sort((PrintCalendarVisual a, PrintCalendarVisual b) => a.StartTime.CompareTo(b.StartTime));
			}
		}

		public virtual void RenderView(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table class=\"eventListTable\">");
			writer.Write("<col class=\"eventListFB\">");
			writer.Write("<col>");
			foreach (ExDateTime key in this.visuals.Keys)
			{
				writer.Write("<tr><td colspan=\"2\"><span class=\"eventListDay\">");
				writer.Write(key.ToString("D", this.sessionContext.UserCulture));
				writer.Write("</span></td></tr>");
				foreach (PrintCalendarVisual printCalendarVisual in this.visuals[key])
				{
					PrintEventListVisual printEventListVisual = (PrintEventListVisual)printCalendarVisual;
					printEventListVisual.Render(writer);
				}
				writer.Write("<tr><td>&nbsp;</td></tr>");
			}
			writer.Write("</table>");
		}

		protected virtual PrintCalendarVisual GetVisual(int index, bool isFirst)
		{
			return new PrintEventListVisual(this.sessionContext, index, this.calendarAdapter.DataSource, isFirst);
		}

		public string DateDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				DateRange[] dateRanges = this.calendarAdapter.DateRanges;
				ExDateTime start = dateRanges[0].Start;
				ExDateTime start2 = dateRanges[dateRanges.Length - 1].Start;
				if (dateRanges.Length == 1)
				{
					stringBuilder.Append(start.ToString("D", this.sessionContext.UserCulture));
				}
				else
				{
					stringBuilder.Append(start.ToString("d", this.sessionContext.UserCulture));
					stringBuilder.Append(" - ");
					stringBuilder.Append(start2.ToString("d", this.sessionContext.UserCulture));
				}
				return stringBuilder.ToString();
			}
		}

		public string CalendarName
		{
			get
			{
				return this.calendarAdapter.CalendarTitle;
			}
		}

		public ExDateTime[] GetEffectiveDates()
		{
			if (this.viewType == CalendarViewType.Monthly)
			{
				return PrintMonthlyView.GetEffectiveDates(this.calendarAdapter, this.sessionContext, this.workingDayOnly);
			}
			return PrintDailyView.GetEffectiveDates(this.calendarAdapter.DateRanges);
		}

		protected ISessionContext sessionContext;

		protected CalendarAdapterBase calendarAdapter;

		protected Dictionary<ExDateTime, List<PrintCalendarVisual>> visuals;

		protected bool workingDayOnly;

		protected CalendarViewType viewType;
	}
}
