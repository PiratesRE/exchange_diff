using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class PrintCalendarVisual
	{
		public static void RenderBackground(TextWriter writer, string cssClass)
		{
			writer.Write("<div class=\"visualBack\">");
			writer.Write("<div class=\"visualBackInner ");
			writer.Write(cssClass);
			writer.Write("\"></div>");
			writer.Write("</div>");
		}

		internal ISessionContext SessionContext { get; private set; }

		internal double Left { get; set; }

		internal double Top { get; set; }

		internal double Width { get; set; }

		internal double Height { get; set; }

		internal BusyTypeWrapper BusyType { get; private set; }

		internal string Subject { get; private set; }

		internal string Location { get; private set; }

		internal string Organizer { get; private set; }

		internal bool IsPrivate { get; private set; }

		internal bool HasAttachment { get; private set; }

		internal CalendarItemTypeWrapper RecurrenceType { get; private set; }

		internal string CssClass { get; private set; }

		internal ExDateTime StartTime { get; private set; }

		internal ExDateTime EndTime { get; private set; }

		public PrintCalendarVisual(ISessionContext sessionContext, CalendarVisual visual, ICalendarDataSource dataSource) : this(sessionContext, visual.Rect.X, visual.Rect.Y, visual.Rect.Width, visual.Rect.Height, visual.DataIndex, dataSource)
		{
		}

		public PrintCalendarVisual(ISessionContext sessionContext, double left, double top, double width, double height, int index, ICalendarDataSource dataSource)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			this.SessionContext = sessionContext;
			this.Left = left;
			this.Top = top;
			this.Width = width;
			this.Height = height;
			this.BusyType = dataSource.GetWrappedBusyType(index);
			this.IsPrivate = dataSource.IsPrivate(index);
			if (this.IsPrivate && dataSource.SharedType != SharedType.None)
			{
				this.Subject = LocalizedStrings.GetNonEncoded(840767634);
			}
			else
			{
				this.Subject = dataSource.GetSubject(index);
				this.Location = dataSource.GetLocation(index);
				this.Organizer = (dataSource.IsMeeting(index) ? dataSource.GetOrganizerDisplayName(index) : null);
			}
			this.HasAttachment = dataSource.HasAttachment(index);
			this.RecurrenceType = dataSource.GetWrappedItemType(index);
			this.CssClass = dataSource.GetCssClassName(index);
			this.StartTime = dataSource.GetStartTime(index);
			this.EndTime = dataSource.GetEndTime(index);
		}

		protected virtual string TimeDescription
		{
			get
			{
				return string.Empty;
			}
		}

		protected bool IsDarkBackground
		{
			get
			{
				return this.CssClass != null && Array.Exists<string>(PrintCalendarVisual.DarkClass, (string v) => v == this.CssClass);
			}
		}

		public virtual void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div class=\"calendarVisual ");
			writer.Write(this.CssClass);
			writer.Write("\" style=\"");
			this.RenderVisualPosition(writer);
			writer.Write("\">");
			this.RenderBackground(writer);
			if (this.BusyType != BusyTypeWrapper.Busy)
			{
				this.RenderFreeBusy(writer, false);
			}
			writer.Write("<table class=\"visualTable\"><tr>");
			if (this.BusyType != BusyTypeWrapper.Busy)
			{
				writer.Write("<td class=\"freeBusy\"></td>");
			}
			writer.Write("<td><div class=\"visualBody ");
			this.RenderExtraClasses(writer);
			writer.Write("\"><div class=\"textContainer\">");
			this.RenderVisualContent(writer);
			writer.Write("</div>");
			writer.Write("</div>");
			writer.Write("</td></tr></table></div>");
		}

		protected virtual void RenderExtraClasses(TextWriter writer)
		{
		}

		protected virtual void RenderVisualPosition(TextWriter writer)
		{
		}

		protected void RenderVisualContent(TextWriter writer)
		{
			this.RenderStringArea(writer, this.TimeDescription, "subject");
			this.RenderIcons(writer);
			this.RenderStringArea(writer, this.Subject, "subject");
			this.RenderStringArea(writer, this.Location, "location");
			this.RenderStringArea(writer, this.Organizer, "organizer");
		}

		protected void RenderStringArea(TextWriter writer, string text, string cssClass)
		{
			if (!string.IsNullOrEmpty(text))
			{
				writer.Write("<span class=\"");
				writer.Write(cssClass);
				writer.Write(" visualText\">");
				writer.Write(text);
				writer.Write(" ");
				writer.Write("</span>");
				writer.Write(this.SessionContext.GetDirectionMark());
			}
		}

		protected void RenderIcons(TextWriter writer)
		{
			this.RenderIcons(writer, false);
		}

		protected void RenderIcons(TextWriter writer, bool noBackground)
		{
			if (this.RecurrenceType == CalendarItemTypeWrapper.Occurrence)
			{
				this.SessionContext.RenderThemeImage(writer, (this.IsDarkBackground && !noBackground) ? ThemeFileId.PrintRecurringAppointmentWhite : ThemeFileId.PrintRecurringAppointment, "imgItemType", new object[0]);
			}
			else if (this.RecurrenceType == CalendarItemTypeWrapper.Exception)
			{
				this.SessionContext.RenderThemeImage(writer, (this.IsDarkBackground && !noBackground) ? ThemeFileId.PrintExceptionWhite : ThemeFileId.PrintException, "imgItemType", new object[0]);
			}
			if (this.IsPrivate)
			{
				this.SessionContext.RenderThemeImage(writer, (this.IsDarkBackground && !noBackground) ? ThemeFileId.PrintPrivateWhite : ThemeFileId.PrintPrivate, "imgPrivate", new object[0]);
			}
			if (this.HasAttachment)
			{
				this.SessionContext.RenderThemeImage(writer, (this.IsDarkBackground && !noBackground) ? ThemeFileId.PrintAttachment3White : ThemeFileId.PrintAttachment3, "imgAttachment", new object[0]);
			}
		}

		protected void RenderFreeBusy(TextWriter writer, bool fixHeight)
		{
			writer.Write("<div class=\"freeBusyContainer");
			if (fixHeight)
			{
				writer.Write(" fixHeightFB");
			}
			writer.Write("\">");
			switch (this.BusyType)
			{
			case BusyTypeWrapper.Free:
				PrintCalendarVisual.RenderBackground(writer, "free");
				break;
			case BusyTypeWrapper.Tentative:
				this.SessionContext.RenderThemeImage(writer, fixHeight ? ThemeFileId.PrintTentativeForAgenda : ThemeFileId.PrintTentative);
				break;
			case BusyTypeWrapper.Busy:
				PrintCalendarVisual.RenderBackground(writer, "busy");
				break;
			case BusyTypeWrapper.OOF:
				PrintCalendarVisual.RenderBackground(writer, "oof");
				break;
			}
			writer.Write("</div>");
		}

		protected void RenderBackground(TextWriter writer)
		{
			PrintCalendarVisual.RenderBackground(writer, this.CssClass);
		}

		private static readonly string[] DarkClass = new string[]
		{
			"cat0",
			"cat1",
			"cat4",
			"cat5",
			"cat6",
			"cat7",
			"cat8",
			"cat9",
			"cat11",
			"cat12",
			"cat13",
			"cat14",
			"cat15",
			"cat16",
			"cat17",
			"cat18",
			"cat19",
			"cat20",
			"cat21",
			"cat22",
			"cat23",
			"cat24"
		};
	}
}
