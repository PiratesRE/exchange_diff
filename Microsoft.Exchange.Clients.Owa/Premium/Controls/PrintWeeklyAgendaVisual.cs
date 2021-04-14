using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintWeeklyAgendaVisual : PrintCalendarVisual
	{
		public PrintWeeklyAgendaVisual(ISessionContext sessionContext, int index, ICalendarDataSource dataSource, bool isFirst) : base(sessionContext, 0.0, 0.0, 1.0, 1.0, index, dataSource)
		{
			this.isFirst = isFirst;
		}

		public override void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<tr><td class=\"agendaPrintEventIcon\">");
			base.RenderFreeBusy(writer, true);
			writer.Write("</td><td>");
			writer.Write("<div class=\"noFixHeightBGContainer\">");
			base.RenderBackground(writer);
			writer.Write("</div>");
			writer.Write("<div id=\"divEventContent\"><strong>");
			base.RenderStringArea(writer, this.TimeDescription, string.Empty);
			base.RenderIcons(writer);
			base.RenderStringArea(writer, base.Subject, string.Empty);
			writer.Write("</strong>");
			string value = string.Format(base.SessionContext.UserCulture, " ({0}) - {1}", new object[]
			{
				base.Location,
				base.Organizer
			});
			writer.Write(value);
			writer.Write("</div></td></tr>");
		}

		protected override string TimeDescription
		{
			get
			{
				string text = this.isFirst ? base.StartTime.ToString("t", base.SessionContext.UserCulture) : base.StartTime.ToString("g", base.SessionContext.UserCulture);
				string text2 = base.StartTime.Date.Equals(base.EndTime.Date) ? base.EndTime.ToString("t", base.SessionContext.UserCulture) : base.EndTime.ToString("g", base.SessionContext.UserCulture);
				return string.Format(base.SessionContext.UserCulture, LocalizedStrings.GetNonEncoded(954837806), new object[]
				{
					text,
					text2
				});
			}
		}

		private bool isFirst;
	}
}
