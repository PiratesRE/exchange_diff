using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class MeetingCancelWriter : MeetingPageWriter
	{
		public MeetingCancelWriter(MeetingCancellation meetingCancellation, UserContext userContext, string toolbarId, bool isPreviewForm, bool isInDeletedItems, bool isEmbeddedItem, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled) : base(meetingCancellation, userContext, isPreviewForm, isInDeletedItems, isEmbeddedItem, isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled)
		{
			this.meetingCancellation = meetingCancellation;
			this.isDelegated = meetingCancellation.IsDelegated();
			if (toolbarId == null)
			{
				toolbarId = "mpToolbar";
			}
			this.isOrganizer = true;
			if (!Utilities.IsPublic(meetingCancellation) && !this.IsDraft && !isEmbeddedItem)
			{
				this.isOrganizer = base.ProcessMeetingMessage(meetingCancellation, Utilities.IsItemInDefaultFolder(meetingCancellation, DefaultFolderType.Inbox));
				this.isOutOfDate = MeetingUtilities.MeetingCancellationIsOutOfDate(meetingCancellation);
			}
			this.recipientWell = new MessageRecipientWell(meetingCancellation);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingCancelWriter>(this);
		}

		protected internal override void BuildInfobar()
		{
			if (this.meetingCancellation.Importance == Importance.High)
			{
				this.FormInfobar.AddMessage(-788473393, InfobarMessageType.Informational);
			}
			else if (this.meetingCancellation.Importance == Importance.Low)
			{
				this.FormInfobar.AddMessage(-1193056027, InfobarMessageType.Informational);
			}
			if (this.isDelegated)
			{
				this.FormInfobar.AddMessage(SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(-1205864060), new object[]
				{
					MeetingUtilities.GetReceivedOnBehalfOfDisplayName(this.meetingCancellation)
				}), InfobarMessageType.Informational);
			}
			if (!this.isOutOfDate)
			{
				this.FormInfobar.AddMessage(-161808760, InfobarMessageType.Informational);
				return;
			}
			this.FormInfobar.AddMessage(21101307, InfobarMessageType.Informational);
		}

		protected override void RenderOpenCalendarToolbar(TextWriter writer)
		{
			writer.Write("<div id=\"divWhenL\" class=\"roWellLabel pvwLabel\">");
			writer.Write(SanitizedHtmlString.FromStringId(-524211323));
			writer.Write("</div>");
		}

		public override void RenderTitle(TextWriter writer)
		{
			RenderingUtilities.RenderSubject(writer, this.meetingCancellation, LocalizedStrings.GetNonEncoded(-1500721828));
		}

		protected override string GetMeetingInfoClass()
		{
			return "mtgCancel";
		}

		public override void RenderSubject(TextWriter writer, bool disableEdit)
		{
			writer.Write(disableEdit ? "<div id=\"divSubj\">" : "<div id=\"divSubj\" tabindex=0 _editable=1>");
			RenderingUtilities.RenderSubject(writer, this.meetingCancellation);
			writer.Write("</div>");
		}

		protected override void RenderMeetingInfoHeader(TextWriter writer)
		{
			MeetingCancelWriter.RenderCancelledMeetingHeader(writer, this.isOrganizer, base.IsInDeletedItems);
		}

		public static void RenderCancelledMeetingHeader(TextWriter writer, bool isOrganizer, bool isDeleted)
		{
			writer.Write("<div id=\"divMtgInfoHeader\">");
			MeetingInfoHeaderToolbar meetingInfoHeaderToolbar = new MeetingInfoHeaderToolbar();
			meetingInfoHeaderToolbar.Render(writer);
			writer.Write("<div id=\"divMtgHeaderTxt\" class=\"mtgCanceled\">");
			writer.Write(SanitizedHtmlString.FromStringId(-383210701));
			writer.Write("</div>");
			if (!isOrganizer && !isDeleted)
			{
				writer.Write("<div id=\"divMtgCancelLink\">");
				writer.Write(SanitizedHtmlString.FromStringId(-2115983576));
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		public override int StoreObjectType
		{
			get
			{
				return 13;
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
				object obj = this.meetingCancellation.TryGetProperty(MessageItemSchema.IsDraft);
				return !(obj is bool) || (bool)obj;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
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

		public override bool ShouldRenderReminder
		{
			get
			{
				return false;
			}
		}

		private MeetingCancellation meetingCancellation;

		private MessageRecipientWell recipientWell;

		private bool isDelegated;

		private bool isOutOfDate;

		private bool isOrganizer;
	}
}
