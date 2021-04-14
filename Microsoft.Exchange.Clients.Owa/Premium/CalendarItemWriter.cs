using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class CalendarItemWriter : MeetingPageWriter
	{
		internal CalendarItemWriter(CalendarItemBase calendarItemBase, UserContext userContext, bool isPreviewForm, bool isInDeletedItems, bool isEmbeddedItem, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled) : base(calendarItemBase, userContext, isPreviewForm, isInDeletedItems, isEmbeddedItem, isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled)
		{
			this.CalendarItemBase = calendarItemBase;
			this.isOrganizer = this.CalendarItemBase.IsOrganizer();
			this.isMeeting = this.CalendarItemBase.IsMeeting;
			bool flag = Utilities.IsPublic(calendarItemBase);
			this.calendarRecipientWell = new CalendarItemRecipientWell(this.CalendarItemBase);
			if (flag)
			{
				if (this.isMeeting)
				{
					this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(this.CalendarItemBase);
					return;
				}
			}
			else
			{
				bool isInArchiveMailbox = Utilities.IsInArchiveMailbox(this.CalendarItemBase);
				if (this.isMeeting)
				{
					if (this.isOrganizer)
					{
						this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(this.CalendarItemBase);
					}
					else if (!this.CalendarItemBase.IsCancelled)
					{
						this.toolbar = new EditMeetingInviteToolbar("mpToolbar", this.IsResponseRequested, isInArchiveMailbox);
					}
				}
				else if (!this.isOrganizer)
				{
					this.toolbar = new EditMeetingInviteToolbar("mpToolbar", isInArchiveMailbox);
				}
				if (this.toolbar != null)
				{
					this.toolbar.ToolbarType = ToolbarType.Preview;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarItemWriter>(this);
		}

		protected internal override void BuildInfobar()
		{
			CalendarUtilities.AddCalendarInfobarMessages(this.FormInfobar, this.CalendarItemBase, null, this.UserContext);
		}

		public override void RenderTitle(TextWriter writer)
		{
			RenderingUtilities.RenderSubject(writer, this.CalendarItemBase, LocalizedStrings.GetNonEncoded(-1500721828));
		}

		public override void RenderSubject(TextWriter writer, bool disableEdit)
		{
			writer.Write(disableEdit ? "<div id=\"divSubj\">" : "<div id=\"divSubj\" tabindex=0 _editable=1>");
			RenderingUtilities.RenderSubject(writer, this.CalendarItemBase);
			writer.Write("</div>");
		}

		protected override void RenderOpenCalendarToolbar(TextWriter writer)
		{
			if (!this.CalendarItemBase.IsCancelled)
			{
				base.RenderOpenCalendarToolbar(writer);
				return;
			}
			writer.Write("<div id=\"divWhenL\" class=\"roWellLabel pvwLabel\">");
			writer.Write(SanitizedHtmlString.FromStringId(-524211323));
			writer.Write("</div>");
		}

		protected override void RenderMeetingInfoHeader(TextWriter writer)
		{
			if (!this.CalendarItemBase.IsCancelled)
			{
				return;
			}
			MeetingCancelWriter.RenderCancelledMeetingHeader(writer, this.isOrganizer, base.IsInDeletedItems);
		}

		protected override string GetMeetingInfoClass()
		{
			if (this.CalendarItemBase.IsCancelled)
			{
				return "mtgCancel";
			}
			return base.GetMeetingInfoClass();
		}

		protected override string GetMeetingToolbarClass()
		{
			if (this.toolbar == null)
			{
				return string.Empty;
			}
			return "threeBtnTb";
		}

		public override bool ShouldRenderRecipientWell(RecipientWellType recipientWellType)
		{
			return this.CalendarItemBase.IsMeeting && base.ShouldRenderRecipientWell(recipientWellType);
		}

		public override int StoreObjectType
		{
			get
			{
				return 15;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return this.calendarRecipientWell;
			}
		}

		public override bool ShouldRenderSentField
		{
			get
			{
				return (this.isMeeting && !this.isOrganizer) || this.CalendarItemBase.MeetingRequestWasSent;
			}
		}

		public override bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return this.isMeeting && this.isOrganizer && this.CalendarItemBase.MeetingRequestWasSent;
			}
		}

		protected internal override string DescriptionTag
		{
			get
			{
				if (this.isMeeting && !this.isOrganizer)
				{
					return base.DescriptionTag;
				}
				return null;
			}
		}

		private bool IsResponseRequested
		{
			get
			{
				object obj = this.CalendarItemBase.TryGetProperty(ItemSchema.IsResponseRequested);
				return obj is bool && (bool)obj;
			}
		}

		private bool IsDelegateForwardedMeetingRequest
		{
			get
			{
				return false;
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return this.CalendarItemBase.Organizer;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return this.CalendarItemBase.Organizer;
			}
		}

		internal override Toolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		public override bool ShouldRenderReminder
		{
			get
			{
				return true;
			}
		}

		private Toolbar toolbar;

		private CalendarItemRecipientWell calendarRecipientWell;

		private bool isOrganizer;

		private bool isMeeting;
	}
}
