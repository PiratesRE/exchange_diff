using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class CalendarItemWriter : MeetingPageWriter
	{
		public CalendarItemWriter(CalendarItemBase calendarItemBase, UserContext userContext) : base(calendarItemBase, userContext)
		{
			base.CalendarItemBase = calendarItemBase;
			this.messageRecipientWell = new CalendarItemRecipientWell(userContext, base.CalendarItemBase);
		}

		public override void RenderToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, true);
			toolbar.RenderStartForSubToolbar();
			if (base.CalendarItemBase.IsCancelled)
			{
				toolbar.RenderButton(ToolbarButtons.RemoveFromCalendar);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.ShowCalendar);
				toolbar.RenderFill();
				toolbar.RenderEndForSubToolbar();
				return;
			}
			MeetingMessageType meetingMessageType = this.meetingMessageType;
			if (meetingMessageType != MeetingMessageType.NewMeetingRequest && meetingMessageType != MeetingMessageType.FullUpdate)
			{
				if (meetingMessageType != MeetingMessageType.Outdated)
				{
					return;
				}
				toolbar.RenderButton(ToolbarButtons.MeetingOutOfDate);
				toolbar.RenderFill();
				toolbar.RenderEndForSubToolbar();
			}
			else
			{
				toolbar.RenderButton(ToolbarButtons.MeetingAccept);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.MeetingTentative);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.MeetingDecline);
				toolbar.RenderDivider();
				toolbar.RenderFill();
				toolbar.RenderEndForSubToolbar();
				if (this.IsResponseRequested)
				{
					MeetingPageWriter.RenderResponseEditTypeSelectToolbar(writer);
					return;
				}
			}
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
				return this.messageRecipientWell;
			}
		}

		private bool IsResponseRequested
		{
			get
			{
				return ItemUtility.GetProperty<bool>(base.CalendarItemBase, ItemSchema.IsResponseRequested, false);
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return base.CalendarItemBase.Organizer;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return base.CalendarItemBase.Organizer;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarItemWriter>(this);
		}

		private CalendarItemRecipientWell messageRecipientWell;

		private MeetingMessageType meetingMessageType = MeetingMessageType.NewMeetingRequest;
	}
}
