using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class MeetingResponseWriter : MeetingPageWriter
	{
		public MeetingResponseWriter(MeetingResponse meetingResponse, UserContext userContext, bool isPreviewForm, bool isInDeletedItems, bool isEmbeddedItem, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled) : base(meetingResponse, userContext, isPreviewForm, isInDeletedItems, isEmbeddedItem, isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled)
		{
			this.meetingResponse = meetingResponse;
			this.isEmbeddedItem = isEmbeddedItem;
			object obj = meetingResponse.TryGetProperty(MessageItemSchema.IsDraft);
			this.isDraft = (obj is bool && (bool)obj);
			this.isDelegated = meetingResponse.IsDelegated();
			if (!Utilities.IsPublic(meetingResponse) && !this.isDraft && !isEmbeddedItem)
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingResponse, Utilities.IsItemInDefaultFolder(meetingResponse, DefaultFolderType.Inbox));
				if (this.isOrganizer)
				{
					this.AttendeeResponseWell = new CalendarItemAttendeeResponseRecipientWell(this.CalendarItemBase);
				}
			}
			this.recipientWell = new MessageRecipientWell(meetingResponse);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingResponseWriter>(this);
		}

		protected internal override void BuildInfobar()
		{
			InfobarMessageBuilder.AddFlag(this.FormInfobar, this.meetingResponse, this.UserContext);
			if (this.isDraft)
			{
				this.FormInfobar.AddMessage(-1981719796, InfobarMessageType.Informational);
				string format = string.Empty;
				switch (this.meetingResponse.ResponseType)
				{
				case ResponseType.Tentative:
					format = LocalizedStrings.GetHtmlEncoded(-588720585);
					break;
				case ResponseType.Accept:
					format = LocalizedStrings.GetHtmlEncoded(-14610226);
					break;
				case ResponseType.Decline:
					format = LocalizedStrings.GetHtmlEncoded(-1615218790);
					break;
				}
				SanitizedHtmlString messageHtml;
				if (this.meetingResponse.From != null && string.CompareOrdinal(this.UserContext.ExchangePrincipal.LegacyDn, this.meetingResponse.From.EmailAddress) != 0)
				{
					ADSessionSettings adSettings = Utilities.CreateScopedADSessionSettings(this.UserContext.LogonIdentity.DomainName);
					string displayName = ExchangePrincipal.FromLegacyDN(adSettings, this.meetingResponse.From.EmailAddress).MailboxInfo.DisplayName;
					messageHtml = SanitizedHtmlString.Format(format, new object[]
					{
						displayName
					});
				}
				else
				{
					messageHtml = SanitizedHtmlString.Format(format, new object[]
					{
						LocalizedStrings.GetNonEncoded(372029413)
					});
				}
				this.FormInfobar.AddMessage(messageHtml, InfobarMessageType.Informational);
				return;
			}
			string s = string.Empty;
			string arg = string.Empty;
			if (this.OriginalSender == null || string.IsNullOrEmpty(this.OriginalSender.DisplayName))
			{
				arg = LocalizedStrings.GetNonEncoded(-342979842);
			}
			else
			{
				arg = this.OriginalSender.DisplayName;
			}
			switch (this.meetingResponse.ResponseType)
			{
			case ResponseType.Tentative:
				s = string.Format(Strings.InfoAttendeeTentative, arg);
				break;
			case ResponseType.Accept:
				s = string.Format(Strings.InfoAttendeeAccepted, arg);
				break;
			case ResponseType.Decline:
				s = string.Format(Strings.InfoAttendeeDecline, arg);
				break;
			}
			this.FormInfobar.AddMessage(Utilities.SanitizeHtmlEncode(s), InfobarMessageType.Informational);
			InfobarMessageBuilder.AddImportance(this.FormInfobar, this.meetingResponse);
			InfobarMessageBuilder.AddSensitivity(this.FormInfobar, this.meetingResponse);
			if (this.isDelegated)
			{
				this.FormInfobar.AddMessage(Utilities.SanitizeHtmlEncode(string.Format(LocalizedStrings.GetNonEncoded(-1205864060), MeetingUtilities.GetReceivedOnBehalfOfDisplayName(this.meetingResponse))), InfobarMessageType.Informational);
			}
			if (!this.isEmbeddedItem && !Utilities.IsPublic(this.meetingResponse))
			{
				InfobarMessageBuilder.AddReadReceiptNotice(this.UserContext, this.FormInfobar, this.meetingResponse);
			}
		}

		protected override void RenderMeetingInfoHeader(TextWriter writer)
		{
			writer.Write("<div id=\"divMtgInfoHeader\">");
			MeetingInfoHeaderToolbar meetingInfoHeaderToolbar = new MeetingInfoHeaderToolbar(this.meetingResponse.ResponseType);
			meetingInfoHeaderToolbar.Render(writer);
			string value = string.Empty;
			string value2 = string.Empty;
			switch (this.meetingResponse.ResponseType)
			{
			case ResponseType.Tentative:
				value = "respTentative";
				value2 = LocalizedStrings.GetNonEncoded(1798747159);
				break;
			case ResponseType.Accept:
				value = "respAccept";
				value2 = LocalizedStrings.GetNonEncoded(988533680);
				break;
			case ResponseType.Decline:
				value = "respDecline";
				value2 = LocalizedStrings.GetNonEncoded(884780479);
				break;
			}
			writer.Write("<div id=\"divMtgHeaderTxt\" class=\"");
			writer.Write(value);
			writer.Write("\">");
			writer.Write(value2);
			writer.Write("</div>");
			writer.Write("</div>");
		}

		protected override void RenderOpenCalendarToolbar(TextWriter writer)
		{
			writer.Write("<div id=\"divWhenL\" class=\"roWellLabel pvwLabel\">");
			writer.Write(SanitizedHtmlString.FromStringId(-524211323));
			writer.Write("</div>");
		}

		public override void RenderTitle(TextWriter writer)
		{
			RenderingUtilities.RenderSubject(writer, this.meetingResponse, Strings.UntitledMeeting);
		}

		protected override string GetMeetingInfoClass()
		{
			return "mtgResponse";
		}

		public override void RenderSubject(TextWriter writer, bool disableEdit)
		{
			writer.Write(disableEdit ? "<div id=\"divSubj\">" : "<div id=\"divSubj\" tabindex=0 _editable=1>");
			RenderingUtilities.RenderSubject(writer, this.meetingResponse);
			writer.Write("</div>");
		}

		public override void RenderSendOnBehalf(TextWriter writer)
		{
			RenderingUtilities.RenderSendOnBehalf(writer, this.UserContext, this.meetingResponse.From);
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
				return !this.isDraft;
			}
		}

		protected internal override string DescriptionTag
		{
			get
			{
				return null;
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

		public override bool ShouldRenderReminder
		{
			get
			{
				return false;
			}
		}

		private MeetingResponse meetingResponse;

		private MessageRecipientWell recipientWell;

		private bool isDraft;

		private bool isOrganizer;

		private bool isDelegated;

		private bool isEmbeddedItem;
	}
}
