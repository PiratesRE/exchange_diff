using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class MeetingPageWriter : DisposeTrackableBase
	{
		internal MeetingPageWriter(Item meetingPageItem, UserContext userContext, bool isPreviewForm, bool isInDeletedItems, bool isEmbeddedItem, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled)
		{
			this.meetingPageItem = meetingPageItem;
			this.UserContext = userContext;
			this.isPreviewForm = isPreviewForm;
			this.isInDeletedItems = isInDeletedItems;
			this.isEmbeddedItem = isEmbeddedItem;
			this.isInJunkEmailFolder = isInJunkEmailFolder;
			this.isSuspectedPhishingItem = isSuspectedPhishingItem;
			this.isLinkEnabled = isLinkEnabled;
		}

		internal bool ProcessMeetingMessage(MeetingMessage meetingMessage, bool doCalendarItemSave)
		{
			this.CalendarItemBase = MeetingUtilities.TryPreProcessCalendarItem(meetingMessage, this.UserContext, doCalendarItemSave);
			if (this.CalendarItemBase != null)
			{
				return this.CalendarItemBase.IsOrganizer();
			}
			return meetingMessage.IsOrganizer();
		}

		protected internal abstract void BuildInfobar();

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.CalendarItemBase != null)
			{
				this.CalendarItemBase.Dispose();
				this.CalendarItemBase = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingPageWriter>(this);
		}

		public virtual void RenderInspectorMailToolbar(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ReadMessageToolbar readMessageToolbar = new ReadMessageToolbar(this.IsInDeletedItems, this.IsEmbeddedItem, this.meetingPageItem, this.IsInJunkEmailFolder, this.IsSuspectedPhishingItem, this.IsLinkEnabled);
			readMessageToolbar.Render(writer);
		}

		public abstract void RenderTitle(TextWriter writer);

		public virtual void RenderMeetingInfoArea(TextWriter writer, bool shouldRenderToolbars)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div id=\"divMtgInfo\" ");
			string value = this.GetMeetingInfoClass();
			if (!string.IsNullOrEmpty(value))
			{
				writer.Write("class =\"");
				writer.Write(value);
				writer.Write("\"");
			}
			writer.Write(">");
			this.RenderMeetingInfoHeader(writer);
			writer.Write("<div id=\"divMtgInfoToolbars\"");
			value = this.GetMeetingToolbarClass();
			if (!string.IsNullOrEmpty(value))
			{
				writer.Write(" class =\"");
				writer.Write(value);
				writer.Write("\"");
			}
			writer.Write(">");
			if (shouldRenderToolbars)
			{
				this.RenderOpenCalendarToolbar(writer);
				this.RenderWhen(writer);
				this.RenderMeetingActionsToolbar(writer);
			}
			else
			{
				writer.Write("<div id=\"divWhenL\" class=\"roWellLabel pvwLabel\">");
				writer.Write(SanitizedHtmlString.FromStringId(-524211323));
				writer.Write("</div>");
				this.RenderWhen(writer);
			}
			writer.Write("</div>");
			this.RenderLocation(writer);
			writer.Write("</div>");
		}

		protected virtual void RenderMeetingInfoHeader(TextWriter writer)
		{
		}

		protected virtual string GetMeetingInfoClass()
		{
			return string.Empty;
		}

		protected virtual string GetMeetingToolbarClass()
		{
			return string.Empty;
		}

		protected virtual void RenderMeetingActionsToolbar(TextWriter writer)
		{
			if (this.Toolbar != null)
			{
				this.Toolbar.Render(writer);
			}
		}

		protected virtual void RenderOpenCalendarToolbar(TextWriter writer)
		{
			OpenCalendarToolbar openCalendarToolbar = new OpenCalendarToolbar();
			openCalendarToolbar.Render(writer);
		}

		public virtual void RenderSender(TextWriter writer)
		{
			this.RenderSender(writer, null);
		}

		public void RenderSender(TextWriter writer, RenderSubHeaderDelegate renderSubHeader)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Utilities.IsOnBehalfOf(this.ActualSender, this.OriginalSender))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetSender(this.UserContext, this.ActualSender, "spnFrom", renderSubHeader), RenderingUtilities.GetSender(this.UserContext, this.OriginalSender, "spnOrg", null));
				return;
			}
			RenderingUtilities.RenderSender(this.UserContext, writer, this.OriginalSender, renderSubHeader);
		}

		public abstract void RenderSubject(TextWriter writer, bool disableEdit);

		public virtual void RenderSendOnBehalf(TextWriter writer)
		{
		}

		public virtual void RenderWhen(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div id=\"divMtgTbWhen\">");
			writer.Write(this.When);
			writer.Write("</div>");
		}

		public virtual void RenderLocation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div id=\"divMtgTbWhere\">");
			writer.Write("<div id=\"divLocationL\" class=\"roWellLabel pvwLabel");
			writer.Write("\">");
			writer.Write(SanitizedHtmlString.FromStringId(1666265192));
			writer.Write("</div><div id=\"divLoc\">");
			writer.Write(this.Location);
			writer.Write("</div>");
			writer.Write("</div>");
		}

		public virtual void RenderDescription(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (!string.IsNullOrEmpty(this.DescriptionTag))
			{
				writer.Write("<div class=dscrp>");
				Utilities.HtmlEncode(this.DescriptionTag, writer);
				writer.Write("</div>");
			}
		}

		public virtual void RenderStartTime(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			object obj = this.meetingPageItem.TryGetProperty(CalendarItemInstanceSchema.StartTime);
			if (obj is ExDateTime)
			{
				RenderingUtilities.RenderDateTimeScriptObject(writer, (ExDateTime)obj);
				return;
			}
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			RenderingUtilities.RenderDateTimeScriptObject(writer, localTime);
		}

		public virtual bool ShouldRenderRecipientWell(RecipientWellType recipientWellType)
		{
			return this.RecipientWell.HasRecipients(recipientWellType);
		}

		public abstract int StoreObjectType { get; }

		public abstract RecipientWell RecipientWell { get; }

		public virtual RecipientWell AttendeeResponseWell
		{
			get
			{
				return this.attendeeResponseWell;
			}
			set
			{
				this.attendeeResponseWell = value;
			}
		}

		public virtual bool ShouldRenderSentField
		{
			get
			{
				return true;
			}
		}

		public virtual bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return false;
			}
		}

		public string GetPrincipalCalendarFolderId(bool isCalendarItem)
		{
			string result = null;
			ExchangePrincipal exchangePrincipal;
			if (this.CalendarItemBase != null && this.CalendarItemBase.ParentId != null)
			{
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(this.CalendarItemBase.ParentId, Utilities.GetMailboxSessionLegacyDN(this.CalendarItemBase));
				result = owaStoreObjectId.ToBase64String();
			}
			else if (!isCalendarItem && this.UserContext.DelegateSessionManager.TryGetExchangePrincipal((this.meetingPageItem as MeetingMessage).ReceivedRepresenting.EmailAddress, out exchangePrincipal))
			{
				using (OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(exchangePrincipal, this.UserContext))
				{
					OwaStoreObjectId owaStoreObjectId2 = Utilities.TryGetDefaultFolderId(this.UserContext, owaStoreObjectIdSessionHandle.Session as MailboxSession, DefaultFolderType.Calendar);
					if (owaStoreObjectId2 == null)
					{
						throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(995407892));
					}
					result = owaStoreObjectId2.ToBase64String();
				}
			}
			return result;
		}

		protected internal virtual SanitizedHtmlString Location
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

		protected internal SanitizedHtmlString When
		{
			get
			{
				return Utilities.SanitizeHtmlEncode(Utilities.GenerateWhen(this.meetingPageItem));
			}
		}

		protected internal virtual string OldWhen
		{
			get
			{
				return string.Empty;
			}
		}

		protected internal virtual string OldLocation
		{
			get
			{
				return string.Empty;
			}
		}

		internal Item MeetingPageItem
		{
			get
			{
				return this.meetingPageItem;
			}
		}

		internal virtual CalendarItemBase CalendarItemBase
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

		protected internal virtual UserContext MeetingPageUserContext
		{
			get
			{
				return this.UserContext;
			}
		}

		public virtual Infobar FormInfobar
		{
			get
			{
				if (this.infobar == null)
				{
					this.infobar = new Infobar();
					this.BuildInfobar();
				}
				return this.infobar;
			}
		}

		public virtual bool ReminderIsSet
		{
			get
			{
				object obj = this.meetingPageItem.TryGetProperty(ItemSchema.ReminderIsSet);
				return obj is bool && (bool)obj;
			}
		}

		public abstract bool ShouldRenderReminder { get; }

		protected internal virtual bool IsPreviewForm
		{
			get
			{
				return this.isPreviewForm;
			}
		}

		protected internal bool IsInDeletedItems
		{
			get
			{
				return this.isInDeletedItems;
			}
		}

		protected internal bool IsEmbeddedItem
		{
			get
			{
				return this.isEmbeddedItem;
			}
		}

		protected internal bool IsInJunkEmailFolder
		{
			get
			{
				return this.isInJunkEmailFolder;
			}
		}

		protected internal bool IsSuspectedPhishingItem
		{
			get
			{
				return this.isSuspectedPhishingItem;
			}
		}

		protected internal bool IsLinkEnabled
		{
			get
			{
				return this.isLinkEnabled;
			}
		}

		internal abstract Participant OriginalSender { get; }

		internal abstract Participant ActualSender { get; }

		protected internal virtual string DescriptionTag
		{
			get
			{
				return LocalizedStrings.GetNonEncoded(-1033607801);
			}
		}

		public virtual string MeetingStatus
		{
			get
			{
				string result = null;
				if (this.calendarItemBase != null)
				{
					if (this.calendarItemBase.IsOrganizer())
					{
						if (!Utilities.IsPublic(this.CalendarItemBase))
						{
							result = LocalizedStrings.GetNonEncoded(-323372768);
						}
					}
					else if (this.calendarItemBase.IsMeeting)
					{
						switch (this.calendarItemBase.ResponseType)
						{
						case ResponseType.Tentative:
							result = LocalizedStrings.GetNonEncoded(1798747159);
							break;
						case ResponseType.Accept:
							result = LocalizedStrings.GetNonEncoded(988533680);
							break;
						case ResponseType.Decline:
							result = LocalizedStrings.GetNonEncoded(884780479);
							break;
						case ResponseType.NotResponded:
							result = LocalizedStrings.GetNonEncoded(-244705250);
							break;
						}
					}
				}
				return result;
			}
		}

		internal virtual Toolbar Toolbar
		{
			get
			{
				return null;
			}
		}

		protected internal const string MeetingPageToolbarId = "mpToolbar";

		protected internal const string StartSubjectMarkup = "<div id=\"divSubj\" tabindex=0 _editable=1>";

		protected internal const string StartSubjectMarkupNoEdit = "<div id=\"divSubj\">";

		protected internal const string EndDivMarkup = "</div>";

		private const bool ShouldRenderAttendeeResponseWellsValue = false;

		protected internal const string OneButtonToolbarClass = "oneBtnTb";

		protected internal const string ThreeButtonsToolbarClass = "threeBtnTb";

		internal static readonly PropertyDefinition[] MeetingMessagePrefetchProperties = new PropertyDefinition[]
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
			ItemSchema.ReminderIsSet,
			StoreObjectSchema.EffectiveRights
		};

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
			ItemSchema.ReminderIsSet,
			StoreObjectSchema.EffectiveRights,
			CalendarItemBaseSchema.MeetingRequestWasSent
		};

		protected internal UserContext UserContext;

		private Item meetingPageItem;

		private CalendarItemBase calendarItemBase;

		private Infobar infobar;

		private RecipientWell attendeeResponseWell;

		private bool isPreviewForm;

		private bool isInDeletedItems;

		private bool isEmbeddedItem;

		private bool isInJunkEmailFolder;

		private bool isSuspectedPhishingItem;

		private bool isLinkEnabled;
	}
}
