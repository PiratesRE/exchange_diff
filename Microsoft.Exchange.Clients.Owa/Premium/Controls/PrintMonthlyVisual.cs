using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintMonthlyVisual : PrintCalendarVisual
	{
		public PrintMonthlyVisual(ISessionContext sessionContext, EventAreaVisual visual, ICalendarDataSource dataSource, bool isFirst) : base(sessionContext, visual, dataSource)
		{
			this.isFirst = isFirst;
			this.leftBreak = visual.LeftBreak;
		}

		protected override string TimeDescription
		{
			get
			{
				string text = string.Empty;
				if (this.isFirst)
				{
					if (this.leftBreak)
					{
						text = base.StartTime.ToString(base.SessionContext.DateFormat);
					}
					if (base.StartTime.Minute != 0 || base.StartTime.Hour != 0)
					{
						if (!string.IsNullOrEmpty(text))
						{
							text += " ";
						}
						text += base.StartTime.ToString(base.SessionContext.TimeFormat);
					}
				}
				return text;
			}
		}

		public override void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div class=\"monthlyVisual ");
			writer.Write(base.CssClass);
			writer.Write("\">");
			writer.Write("<table><tr>");
			if (base.BusyType != BusyTypeWrapper.Busy)
			{
				writer.Write("<td class=\"freeBusy\">");
				base.RenderFreeBusy(writer, false);
				writer.Write("</td>");
			}
			writer.Write("<td class=\"monthlyViewTextContainer\">");
			writer.Write("<div class=\"noFixHeightBGContainer\">");
			base.RenderBackground(writer);
			writer.Write("</div>");
			base.RenderVisualContent(writer);
			writer.Write("</td></tr></table>");
			writer.Write("</div>");
		}

		public const int RowHeight = 19;

		private bool isFirst;

		private bool leftBreak;
	}
}
