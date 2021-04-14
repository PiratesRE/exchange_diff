using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PrintWeeklyAgendaView : PrintEventList
	{
		public PrintWeeklyAgendaView(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter, CalendarViewType viewType, bool isHorizontalLayout) : base(sessionContext, calendarAdapter, viewType, viewType == CalendarViewType.WorkWeeklyAgenda)
		{
			this.isHorizontalLayout = isHorizontalLayout;
		}

		public override void RenderView(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=\"2\" cellspacing=\"0\" class=\"agendaTable\">");
			int num = 2;
			int num2 = (this.visuals.Count == 7) ? 3 : ((int)Math.Ceiling((double)((float)this.visuals.Count / 2f)));
			int num3 = (this.visuals.Count == 1) ? 1 : num;
			for (int i = 0; i < num2; i++)
			{
				writer.Write("<tr class=\"agendaTableTr\">");
				if (this.visuals.Count == 7 && i == num2 - 1)
				{
					int dayIndex = this.isHorizontalLayout ? (i * num) : i;
					writer.Write("<td class=\"agendaTableTd\">");
					this.RenderDay(writer, dayIndex);
					writer.Write("</td>");
					writer.Write("<td class=\"agendaTableTd\">");
					writer.Write("<table cellpadding=\"0\" cellspacing=\"0\" class=\"subTable\">");
					writer.Write("<tr class=\"upperTr\"><td class=\"upperTd\">");
					this.RenderDay(writer, i * num + 1);
					writer.Write("</td></tr><tr class=\"lowerTr\"><td class=\"lowerTd\">");
					this.RenderDay(writer, i * num + 2);
					writer.Write("</td></tr></table></td>");
				}
				else
				{
					for (int j = 0; j < num3; j++)
					{
						int num4 = this.isHorizontalLayout ? (i * num + j) : (j * num2 + i);
						writer.Write("<td class=\"agendaTableTd\">");
						if (num4 < this.visuals.Count)
						{
							this.RenderDay(writer, num4);
						}
						writer.Write("</td>");
					}
				}
				writer.Write("</tr>");
			}
			writer.Write("</table>");
		}

		protected override PrintCalendarVisual GetVisual(int index, bool isFirst)
		{
			return new PrintWeeklyAgendaVisual(this.sessionContext, index, this.calendarAdapter.DataSource, isFirst);
		}

		private void RenderDay(TextWriter writer, int dayIndex)
		{
			if (dayIndex >= this.calendarAdapter.DateRanges.Length)
			{
				return;
			}
			ExDateTime date = this.calendarAdapter.DateRanges[dayIndex].Start.Date;
			writer.Write("<div id=\"divDay");
			writer.Write(dayIndex);
			writer.Write("\" class=\"printVisualContainer\"><div class=\"agendaVisualContainer\"><div class=\"agendaDayTitle\">");
			writer.Write(date.ToLongDateString());
			writer.Write("</div>");
			writer.Write("<div>");
			writer.Write("<table id=\"tblEvents");
			writer.Write(dayIndex);
			writer.Write("\" style=\"\">");
			foreach (PrintCalendarVisual printCalendarVisual in this.visuals[date])
			{
				printCalendarVisual.Render(writer);
			}
			writer.Write("<tr style=\"height:*\"></tr>");
			writer.Write("</table>");
			writer.Write("</div></div>");
			writer.Write("</div>");
		}

		private bool isHorizontalLayout;
	}
}
