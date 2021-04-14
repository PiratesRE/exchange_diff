using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditMeetingInviteToolbar : Toolbar
	{
		internal EditMeetingInviteToolbar(string id, bool isInArchiveMailbox) : base(id, ToolbarType.Form)
		{
			this.isOrganizer = true;
			this.isInArchiveMailbox = isInArchiveMailbox;
		}

		internal EditMeetingInviteToolbar(string id, bool isResponseRequested, bool isInArchiveMailbox) : base(id, ToolbarType.Form)
		{
			this.isResponseRequested = isResponseRequested;
			this.isInArchiveMailbox = isInArchiveMailbox;
		}

		internal EditMeetingInviteToolbar(string id, bool isResponseRequested, bool isInArchiveMailbox, MeetingMessageType meetingMessageType) : base(id, ToolbarType.Form)
		{
			this.isResponseRequested = isResponseRequested;
			this.isInArchiveMailbox = isInArchiveMailbox;
			this.meetingMessageType = meetingMessageType;
		}

		public override bool HasBigButton
		{
			get
			{
				return true;
			}
		}

		protected override void RenderButtons()
		{
			if (!this.isOrganizer)
			{
				MeetingMessageType meetingMessageType = this.meetingMessageType;
				if (meetingMessageType > MeetingMessageType.FullUpdate)
				{
					if (meetingMessageType != MeetingMessageType.InformationalUpdate)
					{
						if (meetingMessageType == MeetingMessageType.Outdated)
						{
							base.RenderButton(ToolbarButtons.MeetingOutOfDate);
							return;
						}
						if (meetingMessageType != MeetingMessageType.PrincipalWantsCopy)
						{
							return;
						}
					}
					base.RenderButton(ToolbarButtons.MeetingNoResponseRequired);
					return;
				}
				if (meetingMessageType != MeetingMessageType.NewMeetingRequest && meetingMessageType != MeetingMessageType.FullUpdate)
				{
					return;
				}
				ToolbarButtonFlags toolbarButtonFlags = ToolbarButtonFlags.None;
				if (this.isInArchiveMailbox)
				{
					toolbarButtonFlags |= ToolbarButtonFlags.Disabled;
				}
				if (!this.isResponseRequested)
				{
					base.RenderButton(ToolbarButtons.MeetingAccept, toolbarButtonFlags);
					base.RenderButton(ToolbarButtons.MeetingTentative, toolbarButtonFlags);
					base.RenderButton(ToolbarButtons.MeetingDecline, toolbarButtonFlags);
					return;
				}
				base.RenderButton(ToolbarButtons.MeetingAcceptMenu, toolbarButtonFlags, new Toolbar.RenderMenuItems(this.RenderMeetingResponseMenuItems));
				base.RenderButton(ToolbarButtons.MeetingTentativeMenu, toolbarButtonFlags, new Toolbar.RenderMenuItems(this.RenderMeetingResponseMenuItems));
				base.RenderButton(ToolbarButtons.MeetingDeclineMenu, toolbarButtonFlags, new Toolbar.RenderMenuItems(this.RenderMeetingResponseMenuItems));
				return;
			}
		}

		private void RenderMeetingResponseMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.MeetingEditResponse);
			base.RenderMenuItem(ToolbarButtons.MeetingSendResponseNow);
			base.RenderMenuItem(ToolbarButtons.MeetingNoResponse);
		}

		private bool isResponseRequested = true;

		private MeetingMessageType meetingMessageType = MeetingMessageType.NewMeetingRequest;

		private bool isOrganizer;

		private bool isInArchiveMailbox;
	}
}
