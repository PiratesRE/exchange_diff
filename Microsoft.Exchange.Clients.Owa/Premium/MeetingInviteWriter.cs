using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class MeetingInviteWriter : MeetingPageWriter
	{
		public MeetingInviteWriter(MeetingRequest meetingRequest, UserContext userContext, string toolbarId, bool isPreviewForm, bool isInDeletedItems, bool isEmbeddedItem, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled) : base(meetingRequest, userContext, isPreviewForm, isInDeletedItems, isEmbeddedItem, isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled)
		{
			this.meetingRequest = meetingRequest;
			this.isDelegated = meetingRequest.IsDelegated();
			if (toolbarId == null)
			{
				toolbarId = "mpToolbar";
			}
			if (!Utilities.IsPublic(meetingRequest) && !this.IsDraft && !isEmbeddedItem)
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingRequest, Utilities.IsItemInDefaultFolder(meetingRequest, DefaultFolderType.Inbox));
				if (this.isOrganizer)
				{
					this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(this.CalendarItemBase);
				}
			}
			this.meetingMessageType = meetingRequest.MeetingRequestType;
			if (!this.IsDraft)
			{
				if (this.meetingMessageType == MeetingMessageType.Outdated)
				{
					this.infobarResponseString = new Strings.IDs?(1771878760);
					this.meetingMessageType = MeetingMessageType.Outdated;
				}
				else if (this.isOrganizer)
				{
					this.infobarResponseString = new Strings.IDs?(247559721);
					this.meetingMessageType = MeetingMessageType.InformationalUpdate;
				}
				else if (this.meetingMessageType == MeetingMessageType.InformationalUpdate)
				{
					if (this.CalendarItemBase != null && this.CalendarItemBase.ResponseType != ResponseType.NotResponded)
					{
						this.infobarResponseString = new Strings.IDs?(689325109);
					}
					else
					{
						this.meetingMessageType = MeetingMessageType.FullUpdate;
					}
				}
			}
			InfobarMessageBuilder.AddFlag(this.FormInfobar, meetingRequest, userContext);
			this.meetingRequestRecipientWell = new MeetingRequestRecipientWell(meetingRequest);
			this.toolbar = new EditMeetingInviteToolbar(toolbarId, meetingRequest.IsResponseRequested, Utilities.IsInArchiveMailbox(meetingRequest), this.meetingMessageType);
			this.toolbar.ToolbarType = ToolbarType.Preview;
		}

		protected internal override void BuildInfobar()
		{
			this.BuildInfobar(this.FormInfobar);
		}

		internal void BuildInfobar(Infobar infobar)
		{
			if (this.IsPhishingItemWithEnabledLinks())
			{
				string s = "<a id=\"aIbBlk\" href=\"#\" " + string.Format(CultureInfo.InvariantCulture, ">{0}</a> {1} ", new object[]
				{
					LocalizedStrings.GetHtmlEncoded(-672110188),
					LocalizedStrings.GetHtmlEncoded(-1020475744)
				});
				string s2 = string.Format(CultureInfo.InvariantCulture, "<a href=\"#\" " + Utilities.GetScriptHandler("onclick", "opnHlp(\"" + Utilities.JavascriptEncode(Utilities.BuildEhcHref(HelpIdsLight.EmailSafetyLight.ToString())) + "\");") + ">{0}</a>", new object[]
				{
					LocalizedStrings.GetHtmlEncoded(338562664)
				});
				infobar.AddMessage(SanitizedHtmlString.Format("{0}{1}{2}", new object[]
				{
					LocalizedStrings.GetNonEncoded(1581910613),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s2)
				}), InfobarMessageType.Phishing);
			}
			if (this.isDelegated)
			{
				infobar.AddMessage(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1205864060), new object[]
				{
					MeetingUtilities.GetReceivedOnBehalfOfDisplayName(this.meetingRequest)
				}), InfobarMessageType.Informational);
			}
			if (this.meetingRequest.MeetingRequestType == MeetingMessageType.PrincipalWantsCopy)
			{
				infobar.AddMessage(SanitizedHtmlString.FromStringId(-332743944), InfobarMessageType.Informational);
			}
			if (!this.IsDraft && this.meetingRequest.MeetingRequestType != MeetingMessageType.Outdated && this.CalendarItemBase != null)
			{
				CalendarUtilities.AddCalendarInfobarMessages(infobar, this.CalendarItemBase, this.meetingRequest, this.UserContext);
			}
			if (this.infobarResponseString != null)
			{
				infobar.AddMessage(this.infobarResponseString.Value, InfobarMessageType.Informational);
			}
		}

		private bool IsPhishingItemWithEnabledLinks()
		{
			return base.IsSuspectedPhishingItem && !JunkEmailUtilities.IsItemLinkEnabled(this.meetingRequest);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingInviteWriter>(this);
		}

		public override void RenderTitle(TextWriter writer)
		{
			RenderingUtilities.RenderSubject(writer, this.meetingRequest, LocalizedStrings.GetNonEncoded(-1500721828));
		}

		public override void RenderSubject(TextWriter writer, bool disableEdit)
		{
			writer.Write("<div id=\"divSubj\"");
			if ((this.ChangeHighlight & 16) != 0)
			{
				writer.Write(" class=\"updNew\"");
			}
			if (disableEdit)
			{
				writer.Write(">");
			}
			else
			{
				writer.Write(" tabindex=0 _editable=1>");
			}
			RenderingUtilities.RenderSubject(writer, this.meetingRequest);
			writer.Write("</div>");
		}

		public override void RenderWhen(TextWriter writer)
		{
			bool flag = 0 != (this.ChangeHighlight & 7);
			writer.Write("<div id=\"divMtgTbWhen\"><div");
			if (flag)
			{
				writer.Write(" class=\"updNew\"");
			}
			writer.Write(">");
			if (!string.IsNullOrEmpty(base.When.ToString()))
			{
				writer.Write(base.When);
			}
			writer.Write("</div>");
			if (flag && this.OldWhen.Length > 0)
			{
				writer.Write("<div class=\"updOld\">");
				writer.Write(this.MeetingPageUserContext.DirectionMark);
				writer.Write("(");
				writer.Write(this.OldWhen);
				writer.Write(")");
				writer.Write(this.MeetingPageUserContext.DirectionMark);
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		public override void RenderLocation(TextWriter writer)
		{
			bool flag = (this.ChangeHighlight & 8) != 0;
			writer.Write("<div id=\"divMtgTbWhere\"><div id=\"divLocationL\" class=\"roWellLabel pvwLabel");
			if (flag)
			{
				writer.Write(" updNew");
			}
			writer.Write("\">");
			writer.Write(LocalizedStrings.GetNonEncoded(1666265192));
			writer.Write("</div><div id=\"divLoc\">");
			writer.Write("<div");
			if (flag)
			{
				writer.Write(" class=\"updNew\"");
			}
			writer.Write(">");
			writer.Write(this.Location);
			writer.Write("</div>");
			if (flag && this.OldLocation.Length > 0)
			{
				writer.Write("<div class=\"updOld\">");
				writer.Write(this.MeetingPageUserContext.DirectionMark);
				writer.Write("(");
				writer.Write(this.OldLocation);
				writer.Write(")");
				writer.Write(this.MeetingPageUserContext.DirectionMark);
				writer.Write("</div>");
			}
			writer.Write("</div>");
			writer.Write("</div>");
		}

		protected override string GetMeetingInfoClass()
		{
			if (this.meetingRequest.MeetingRequestType == MeetingMessageType.Outdated)
			{
				return "mtgOutdated";
			}
			return string.Empty;
		}

		protected override string GetMeetingToolbarClass()
		{
			if (this.meetingMessageType == MeetingMessageType.NewMeetingRequest || this.meetingMessageType == MeetingMessageType.FullUpdate)
			{
				return "threeBtnTb";
			}
			return "oneBtnTb";
		}

		public override void RenderDescription(TextWriter writer)
		{
			writer.Write("<div class=\"dscrp ");
			if ((this.ChangeHighlight & 128) != 0)
			{
				writer.Write("updNew");
			}
			writer.Write("\">");
			Utilities.HtmlEncode(this.DescriptionTag, writer);
			writer.Write("</div>");
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
				return this.isOrganizer && this.CalendarItemBase != null;
			}
		}

		public override bool ShouldRenderSentField
		{
			get
			{
				return !this.IsDraft;
			}
		}

		private bool IsDraft
		{
			get
			{
				object obj = this.meetingRequest.TryGetProperty(MessageItemSchema.IsDraft);
				return !(obj is bool) || (bool)obj;
			}
		}

		private int ChangeHighlight
		{
			get
			{
				return (int)this.meetingRequest.ChangeHighlight;
			}
		}

		protected internal override string OldLocation
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

		protected internal override string OldWhen
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
				return false;
			}
		}

		private MeetingRequest meetingRequest;

		private EditMeetingInviteToolbar toolbar;

		private MeetingRequestRecipientWell meetingRequestRecipientWell;

		private bool isOrganizer;

		private Strings.IDs? infobarResponseString;

		private bool isDelegated;

		private MeetingMessageType meetingMessageType;
	}
}
