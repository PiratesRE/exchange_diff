using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class CalendarItemTrackingTab : OwaForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.calendarItemBase = base.Initialize<CalendarItemBase>(new PropertyDefinition[0]);
			if (this.calendarItemBase == null)
			{
				throw new OwaInvalidRequestException("Should not be loading this form without a calendar item base");
			}
			if (!this.calendarItemBase.MeetingRequestWasSent)
			{
				throw new OwaInvalidRequestException("Should not be loading this form with a calendar item that has never sent invitation");
			}
			this.attendeeCollection = new List<Attendee>(this.calendarItemBase.AttendeeCollection);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "sc", false);
			if (queryStringParameter != null)
			{
				int num;
				if (!int.TryParse(queryStringParameter, out num))
				{
					throw new OwaInvalidRequestException("Invalid URL parameter value");
				}
				this.trackingTableSortedColumn = (CalendarItemTrackingTab.TrackingTableColumn)num;
				switch (this.trackingTableSortedColumn)
				{
				case CalendarItemTrackingTab.TrackingTableColumn.Name:
					this.attendeeCollection.Sort(new CalendarItemTrackingTab.NameComparer());
					break;
				case CalendarItemTrackingTab.TrackingTableColumn.Attendance:
					break;
				case CalendarItemTrackingTab.TrackingTableColumn.Response:
					this.attendeeCollection.Sort(new CalendarItemTrackingTab.ResponseTypeComparer());
					return;
				default:
					return;
				}
			}
		}

		protected void RenderTrackResponsesHeader()
		{
			this.RenderColumnHeader(CalendarItemTrackingTab.TrackingTableColumn.Name, LocalizedStrings.GetHtmlEncoded(1006362642), "trkName fltBefore trkCell");
			this.RenderColumnHeader(CalendarItemTrackingTab.TrackingTableColumn.Response, LocalizedStrings.GetHtmlEncoded(807705055), "trkResponse fltAfter trkCell");
			this.RenderColumnHeader(CalendarItemTrackingTab.TrackingTableColumn.Attendance, LocalizedStrings.GetHtmlEncoded(-161922525), "trkCell");
		}

		private void RenderColumnHeader(CalendarItemTrackingTab.TrackingTableColumn trackingTableColumn, string header, string className)
		{
			base.Response.Write("<div class=\"");
			base.Response.Write(className);
			if (this.trackingTableSortedColumn == trackingTableColumn)
			{
				base.Response.Write(" sch\"");
			}
			else
			{
				base.Response.Write("\" ");
				Utilities.RenderScriptHandler(base.Response.Output, "onclick", "onClkTrkSrt(" + (int)trackingTableColumn + ");");
			}
			base.Response.Write(">");
			base.Response.Write(header);
			if (this.trackingTableSortedColumn == trackingTableColumn)
			{
				base.UserContext.RenderThemeImage(base.Response.Output, ThemeFileId.SortAscending);
			}
			base.Response.Write("</div>");
		}

		protected void RenderResponsesList()
		{
			foreach (Attendee attendee in this.attendeeCollection)
			{
				base.Response.Write("<div id=\"divTrk\"><div class=\"trkName trkCell fltBefore");
				base.Response.Write((this.trackingTableSortedColumn == CalendarItemTrackingTab.TrackingTableColumn.Name) ? " trkSortColumn" : string.Empty);
				base.Response.Write("\">");
				base.Response.Write("<a");
				base.Response.Write(" class=\"lnk\" ");
				Utilities.RenderScriptHandler(base.Response.Output, "onclick", "onClkResponseRcp(_this);");
				base.Response.Write(" em=\"");
				Utilities.HtmlEncode(attendee.Participant.EmailAddress, base.Response.Output);
				base.Response.Write("\" dn=\"");
				Utilities.HtmlEncode(attendee.Participant.DisplayName, base.Response.Output);
				base.Response.Write("\">");
				Utilities.HtmlEncode(attendee.Participant.DisplayName, base.Response.Output);
				base.Response.Write("</a></div>");
				string htmlEncoded;
				switch (attendee.ResponseType)
				{
				case ResponseType.Tentative:
					htmlEncoded = LocalizedStrings.GetHtmlEncoded(1798747159);
					break;
				case ResponseType.Accept:
					htmlEncoded = LocalizedStrings.GetHtmlEncoded(988533680);
					break;
				case ResponseType.Decline:
					htmlEncoded = LocalizedStrings.GetHtmlEncoded(884780479);
					break;
				default:
					htmlEncoded = LocalizedStrings.GetHtmlEncoded(378898093);
					break;
				}
				base.Response.Write("<div class=\"trkResponse  trkCell fltAfter");
				base.Response.Write((this.trackingTableSortedColumn == CalendarItemTrackingTab.TrackingTableColumn.Response) ? " trkSortColumn" : string.Empty);
				base.Response.Write("\">");
				base.Response.Write(htmlEncoded);
				base.Response.Write("</div>");
				string s = null;
				switch (attendee.AttendeeType)
				{
				case AttendeeType.Required:
					s = LocalizedStrings.GetHtmlEncoded(956546969);
					break;
				case AttendeeType.Optional:
					s = LocalizedStrings.GetHtmlEncoded(401962758);
					break;
				case AttendeeType.Resource:
					s = LocalizedStrings.GetHtmlEncoded(2051574762);
					break;
				}
				base.Response.Write("<div class=\"trkCell");
				base.Response.Write((this.trackingTableSortedColumn == CalendarItemTrackingTab.TrackingTableColumn.Attendance) ? " trkSortColumn" : string.Empty);
				base.Response.Write("\">");
				base.Response.Write(s);
				base.Response.Write("</div></div>");
			}
		}

		private const string SortColumnQueryParameter = "sc";

		private CalendarItemBase calendarItemBase;

		private List<Attendee> attendeeCollection;

		private CalendarItemTrackingTab.TrackingTableColumn trackingTableSortedColumn = CalendarItemTrackingTab.TrackingTableColumn.Attendance;

		private class AttendeeTypeComparer : IComparer<Attendee>
		{
			public int Compare(Attendee attendeeX, Attendee attendeeY)
			{
				if (attendeeX == null)
				{
					throw new ArgumentNullException("attendeeX");
				}
				if (attendeeY == null)
				{
					throw new ArgumentNullException("attendeeY");
				}
				return ((int)attendeeX.AttendeeType).CompareTo((int)attendeeY.AttendeeType);
			}
		}

		private class ResponseTypeComparer : IComparer<Attendee>
		{
			public int Compare(Attendee attendeeX, Attendee attendeeY)
			{
				if (attendeeX == null)
				{
					throw new ArgumentNullException("attendeeX");
				}
				if (attendeeY == null)
				{
					throw new ArgumentNullException("attendeeY");
				}
				if (attendeeX.ResponseType == attendeeY.ResponseType)
				{
					return this.attendeeTypeComparer.Compare(attendeeX, attendeeY);
				}
				if (attendeeX.ResponseType == ResponseType.Accept)
				{
					return -1;
				}
				if (attendeeY.ResponseType == ResponseType.Accept)
				{
					return 1;
				}
				if (attendeeX.ResponseType == ResponseType.Tentative)
				{
					return -1;
				}
				if (attendeeY.ResponseType == ResponseType.Tentative)
				{
					return 1;
				}
				if (attendeeX.ResponseType < attendeeY.ResponseType)
				{
					return 1;
				}
				return -1;
			}

			private CalendarItemTrackingTab.AttendeeTypeComparer attendeeTypeComparer = new CalendarItemTrackingTab.AttendeeTypeComparer();
		}

		private class NameComparer : IComparer<Attendee>
		{
			public int Compare(Attendee attendeeX, Attendee attendeeY)
			{
				if (attendeeX == null)
				{
					throw new ArgumentNullException("attendeeX");
				}
				if (attendeeY == null)
				{
					throw new ArgumentNullException("attendeeY");
				}
				int num = string.Compare(attendeeX.Participant.DisplayName, attendeeY.Participant.DisplayName, StringComparison.CurrentCulture);
				if (num == 0)
				{
					num = this.attendeeTypeComparer.Compare(attendeeX, attendeeY);
				}
				return num;
			}

			private CalendarItemTrackingTab.AttendeeTypeComparer attendeeTypeComparer = new CalendarItemTrackingTab.AttendeeTypeComparer();
		}

		public enum TrackingTableColumn
		{
			None,
			Name,
			Attendance,
			Response
		}
	}
}
