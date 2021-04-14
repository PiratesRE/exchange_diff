using System;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class MeetingResponseWriter : MeetingPageWriter
	{
		public MeetingResponseWriter(MeetingResponse meetingResponse, UserContext userContext, bool isEmbeddedItem) : base(meetingResponse, userContext)
		{
			this.meetingResponse = meetingResponse;
			if (!isEmbeddedItem && !meetingResponse.IsDelegated())
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingResponse, Utilities.IsItemInDefaultFolder(meetingResponse, DefaultFolderType.Inbox));
				if (this.isOrganizer)
				{
					this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(userContext, base.CalendarItemBase);
				}
			}
			this.recipientWell = new MessageRecipientWell(userContext, meetingResponse);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			try
			{
				if (isDisposing && base.CalendarItemBase != null)
				{
					base.CalendarItemBase.Dispose();
					base.CalendarItemBase = null;
				}
			}
			finally
			{
				base.InternalDispose(isDisposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingResponseWriter>(this);
		}

		public override int StoreObjectType
		{
			get
			{
				return 12;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		public override bool HasToolbar
		{
			get
			{
				return !this.isOrganizer;
			}
		}

		public override bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return this.isOrganizer && base.CalendarItemBase != null;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return this.meetingResponse.Sender;
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return this.meetingResponse.From;
			}
		}

		internal static PropertyDefinition[] PrefetchProperties = new PropertyDefinition[]
		{
			MessageItemSchema.IsRead,
			MessageItemSchema.IsDraft,
			MeetingMessageSchema.CalendarProcessed,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet
		};

		private MeetingResponse meetingResponse;

		private MessageRecipientWell recipientWell;

		private bool isOrganizer;
	}
}
