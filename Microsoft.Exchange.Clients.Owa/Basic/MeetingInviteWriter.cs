using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class MeetingInviteWriter : MeetingPageWriter
	{
		public MeetingInviteWriter(MeetingRequest meetingRequest, UserContext userContext, bool isEmbeddedItem) : base(meetingRequest, userContext)
		{
			this.meetingRequest = meetingRequest;
			if (!isEmbeddedItem && !meetingRequest.IsDelegated())
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingRequest, Utilities.IsItemInDefaultFolder(meetingRequest, DefaultFolderType.Inbox));
				if (this.isOrganizer)
				{
					this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(userContext, base.CalendarItemBase);
				}
			}
			this.meetingMessageType = meetingRequest.MeetingRequestType;
			if (this.meetingMessageType != MeetingMessageType.Outdated)
			{
				if (this.isOrganizer)
				{
					this.meetingMessageType = MeetingMessageType.InformationalUpdate;
				}
				else if (this.meetingMessageType == MeetingMessageType.InformationalUpdate && base.CalendarItemBase != null && base.CalendarItemBase.ResponseType == ResponseType.NotResponded)
				{
					this.meetingMessageType = MeetingMessageType.FullUpdate;
				}
			}
			this.meetingRequestRecipientWell = new MeetingRequestRecipientWell(userContext, meetingRequest);
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
			return DisposeTracker.Get<MeetingInviteWriter>(this);
		}

		public override void RenderWhen(TextWriter writer)
		{
			bool flag = 0 != (this.ChangeHighlight & 7);
			writer.Write("<td class=\"hdmsb");
			if (flag)
			{
				writer.Write(" updnw");
			}
			writer.Write("\">");
			writer.Write(SanitizedHtmlString.FromStringId(-524211323));
			writer.Write("</td><td class=\"hdmtxt\">");
			writer.Write("<span");
			if (flag)
			{
				writer.Write(" class=\"updnw\"");
			}
			writer.Write(">");
			if (!SanitizedStringBase<OwaHtml>.IsNullOrEmpty(base.When))
			{
				writer.Write(base.When);
			}
			writer.Write("</span>");
			if (flag && this.OldWhen.Length > 0)
			{
				writer.Write("<div class=\"updold\">");
				writer.Write("(");
				writer.Write(this.OldWhen);
				writer.Write(")");
				writer.Write("</div>");
			}
			writer.Write("</td>");
		}

		public override void RenderLocation(TextWriter writer)
		{
			bool flag = (this.ChangeHighlight & 8) != 0;
			writer.Write("<td class=\"hdmsb");
			if (flag)
			{
				writer.Write(" updnw");
			}
			writer.Write("\">");
			writer.Write(SanitizedHtmlString.FromStringId(-1134349396));
			writer.Write("</td><td class=\"hdmtxt\">");
			writer.Write("<span");
			if (flag)
			{
				writer.Write(" class=\"updnw\"");
			}
			writer.Write(">");
			writer.Write(this.Location);
			writer.Write("</span>");
			if (flag && this.OldLocation.Length > 0)
			{
				writer.Write("<div class=\"updold\">");
				writer.Write("(");
				writer.Write(this.OldLocation);
				writer.Write(")");
				writer.Write("</div>");
			}
			writer.Write("</td>");
		}

		public override void RenderToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, true);
			MeetingMessageType meetingMessageType = this.meetingMessageType;
			if (meetingMessageType > MeetingMessageType.FullUpdate)
			{
				if (meetingMessageType != MeetingMessageType.InformationalUpdate)
				{
					if (meetingMessageType == MeetingMessageType.Outdated)
					{
						toolbar.RenderStartForSubToolbar();
						toolbar.RenderButton(ToolbarButtons.MeetingOutOfDate);
						toolbar.RenderDivider();
						toolbar.RenderButton(ToolbarButtons.ShowCalendar);
						toolbar.RenderFill();
						toolbar.RenderEndForSubToolbar();
						return;
					}
					if (meetingMessageType != MeetingMessageType.PrincipalWantsCopy)
					{
						return;
					}
				}
				toolbar.RenderStartForSubToolbar();
				toolbar.RenderButton(ToolbarButtons.MeetingNoResponseRequired);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.ShowCalendar);
				toolbar.RenderFill();
				toolbar.RenderEndForSubToolbar();
				return;
			}
			if (meetingMessageType != MeetingMessageType.NewMeetingRequest && meetingMessageType != MeetingMessageType.FullUpdate)
			{
				return;
			}
			toolbar.RenderStartForSubToolbar();
			toolbar.RenderButton(ToolbarButtons.MeetingAccept);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.MeetingTentative);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.MeetingDecline);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.ShowCalendar);
			toolbar.RenderFill();
			toolbar.RenderEndForSubToolbar();
			if (this.meetingRequest.IsResponseRequested)
			{
				MeetingPageWriter.RenderResponseEditTypeSelectToolbar(writer);
				return;
			}
		}

		public override int StoreObjectType
		{
			get
			{
				return 11;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return this.meetingRequestRecipientWell;
			}
		}

		public override bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return this.isOrganizer && base.CalendarItemBase != null;
			}
		}

		public override bool HasToolbar
		{
			get
			{
				return !this.isOrganizer;
			}
		}

		private int ChangeHighlight
		{
			get
			{
				return (int)this.meetingRequest.ChangeHighlight;
			}
		}

		private string OldLocation
		{
			get
			{
				string oldLocation = this.meetingRequest.OldLocation;
				if (oldLocation != null)
				{
					return Utilities.HtmlEncode(oldLocation);
				}
				return string.Empty;
			}
		}

		private string OldWhen
		{
			get
			{
				string text = this.meetingRequest.GenerateOldWhen();
				if (text != null)
				{
					return Utilities.HtmlEncode(text);
				}
				return string.Empty;
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return this.meetingRequest.From;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return this.meetingRequest.Sender;
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

		private MeetingRequest meetingRequest;

		private MeetingRequestRecipientWell meetingRequestRecipientWell;

		private bool isOrganizer;

		private MeetingMessageType meetingMessageType;
	}
}
