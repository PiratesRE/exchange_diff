using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public abstract class MeetingPageWriter : DisposeTrackableBase
	{
		internal MeetingPageWriter(Item meetingPageItem, UserContext userContext)
		{
			this.meetingPageItem = meetingPageItem;
			this.userContext = userContext;
		}

		internal bool ProcessMeetingMessage(MeetingMessage meetingMessage, bool doCalendarItemSave)
		{
			this.CalendarItemBase = MeetingUtilities.TryPreProcessCalendarItem(meetingMessage, this.userContext, doCalendarItemSave);
			if (this.CalendarItemBase != null)
			{
				return this.CalendarItemBase.IsOrganizer();
			}
			return meetingMessage.IsOrganizer();
		}

		protected static void RenderResponseEditTypeSelectToolbar(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table class=\"stb\" cellpadding=0 cellspacing=0><tr>");
			for (int i = 0; i <= 2; i++)
			{
				writer.Write("<td class=\"btncl\"><input class=\"rdobtn\" type=\"radio\" name=\"");
				writer.Write("rdoRsp");
				writer.Write("\" id=\"");
				writer.Write("rdoRsp");
				writer.Write(i + 1);
				writer.Write("\" value=\"");
				writer.Write(i);
				writer.Write("\"");
				if (i == 1)
				{
					writer.Write(" checked");
				}
				writer.Write("><label for=\"");
				writer.Write("rdoRsp");
				writer.Write(i + 1);
				writer.Write("\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(MeetingPageWriter.responseEditTypeStringIds[i]));
				writer.Write("</label></td>");
			}
			writer.Write("<td class=\"w100\"></tr></table>");
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingPageWriter>(this);
		}

		public virtual void RenderSubject(TextWriter writer)
		{
			RenderingUtilities.RenderSubject(writer, this.meetingPageItem);
		}

		public virtual void RenderToolbar(TextWriter writer)
		{
		}

		public virtual void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Utilities.IsOnBehalfOf(this.ActualSender, this.OriginalSender))
			{
				RenderingUtilities.RenderSenderOnBehalfOf(this.ActualSender, this.OriginalSender, writer, this.userContext);
				return;
			}
			RenderingUtilities.RenderSender(this.userContext, writer, this.OriginalSender);
		}

		public virtual void RenderSentTime(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(LocalizedStrings.GetHtmlEncoded(295620541));
			writer.Write("&nbsp;");
			ExDateTime property = ItemUtility.GetProperty<ExDateTime>(this.meetingPageItem, ItemSchema.SentTime, ExDateTime.MinValue);
			if (property != ExDateTime.MinValue)
			{
				RenderingUtilities.RenderSentTime(writer, property, this.userContext);
			}
		}

		public virtual bool ShouldRenderRecipientWell(RecipientWellType recipientWellType)
		{
			return this.RecipientWell.HasRecipients(recipientWellType);
		}

		public virtual void RenderWhen(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<td class=\"hdmsb\">");
			writer.Write(SanitizedHtmlString.FromStringId(-524211323));
			writer.Write("</td><td class=\"hdmtxt\"><span>");
			writer.Write(this.When);
			writer.Write("</span></td>");
		}

		public virtual void RenderLocation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<td class=\"hdmsb\">");
			writer.Write(SanitizedHtmlString.FromStringId(-1134349396));
			writer.Write("</td><td class=\"hdmtxt\"><span>");
			writer.Write(this.Location);
			writer.Write("</span></td>");
		}

		public abstract RecipientWell RecipientWell { get; }

		public abstract int StoreObjectType { get; }

		public virtual RecipientWell AttendeeResponseWell
		{
			get
			{
				return this.attendeeResponseWell;
			}
			set
			{
				this.attendeeResponseWell = (value as CalendarItemAttendeeResponseRecipientWell);
			}
		}

		public virtual bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasToolbar
		{
			get
			{
				return true;
			}
		}

		protected SanitizedHtmlString When
		{
			get
			{
				return Utilities.SanitizeHtmlEncode(Utilities.GenerateWhen(this.meetingPageItem));
			}
		}

		protected virtual SanitizedHtmlString Location
		{
			get
			{
				string text = this.meetingPageItem.TryGetProperty(CalendarItemBaseSchema.Location) as string;
				if (text != null)
				{
					return Utilities.SanitizeHtmlEncode(text);
				}
				return SanitizedHtmlString.Empty;
			}
		}

		internal abstract Participant OriginalSender { get; }

		internal abstract Participant ActualSender { get; }

		internal CalendarItemBase CalendarItemBase
		{
			get
			{
				return this.calendarItemBase;
			}
			set
			{
				this.calendarItemBase = value;
			}
		}

		private const bool ShouldRenderAttendeeResponseWellsValue = false;

		internal static readonly PropertyDefinition[] CalendarPrefetchProperties = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.CalendarItemType,
			ItemSchema.SentTime,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet
		};

		private static Strings.IDs[] responseEditTypeStringIds = new Strings.IDs[]
		{
			-114654491,
			1050381195,
			-990767046
		};

		private CalendarItemBase calendarItemBase;

		private Item meetingPageItem;

		private CalendarItemAttendeeResponseRecipientWell attendeeResponseWell;

		protected UserContext userContext;
	}
}
