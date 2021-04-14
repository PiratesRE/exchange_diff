using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class MeetingCancelWriter : MeetingPageWriter
	{
		public MeetingCancelWriter(MeetingCancellation meetingCancellation, UserContext userContext, bool isEmbeddedItem) : base(meetingCancellation, userContext)
		{
			this.meetingCancellation = meetingCancellation;
			if (!isEmbeddedItem && !meetingCancellation.IsDelegated())
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingCancellation, Utilities.IsItemInDefaultFolder(meetingCancellation, DefaultFolderType.Inbox));
			}
			this.messageRecipientWell = new MessageRecipientWell(userContext, meetingCancellation);
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
			return DisposeTracker.Get<MeetingCancelWriter>(this);
		}

		public override void RenderToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, true);
			toolbar.RenderStartForSubToolbar();
			if (!this.isOrganizer)
			{
				toolbar.RenderButton(ToolbarButtons.RemoveFromCalendar);
			}
			else
			{
				toolbar.RenderButton(ToolbarButtons.MeetingNoResponseRequired);
			}
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.ShowCalendar);
			toolbar.RenderFill();
			toolbar.RenderEndForSubToolbar();
		}

		public override int StoreObjectType
		{
			get
			{
				return 13;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return this.messageRecipientWell;
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return this.meetingCancellation.From;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return this.meetingCancellation.Sender;
			}
		}

		internal static readonly PropertyDefinition[] PrefetchProperties = new PropertyDefinition[]
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

		private MeetingCancellation meetingCancellation;

		private MessageRecipientWell messageRecipientWell;

		private bool isOrganizer = true;
	}
}
