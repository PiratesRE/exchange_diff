using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class MeetingPage : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "t");
			this.MeetingPageWriterFactory(queryStringParameter, e);
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(base.Item, base.IsEmbeddedItem, base.ForceEnableItemLink, base.UserContext, out this.isInJunkEmailFolder, out this.isSuspectedPhishingItem, out this.itemLinkEnabled, out this.isJunkOrPhishing);
			if (this.isJunkOrPhishing)
			{
				this.bodyMarkup = Markup.PlainText;
			}
			if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(queryStringParameter) && !base.IsPreviewForm)
			{
				MeetingMessage meetingMessage = base.Item as MeetingMessage;
				if (meetingMessage == null)
				{
					throw new OwaInvalidOperationException("Item must be of MeetingMessage type");
				}
				if (meetingMessage != null && !meetingMessage.IsRead)
				{
					meetingMessage.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, meetingMessage), false);
				}
			}
			object obj = base.Item.TryGetProperty(ItemSchema.SentTime);
			if (obj != null && !(obj is PropertyError))
			{
				this.sentTime = (ExDateTime)obj;
			}
			if (this.sharedFromOlderVersion)
			{
				this.meetingPageWriter.FormInfobar.AddMessage(SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(1896884103), new object[]
				{
					this.receiverDisplayName
				}), InfobarMessageType.Informational);
			}
		}

		protected virtual void MeetingPageWriterFactory(string itemType, EventArgs e)
		{
			if (ObjectClass.IsMeetingRequest(itemType))
			{
				this.meetingPageWriter = new MeetingInviteWriter(base.Item = base.Initialize<MeetingRequest>(MeetingPageWriter.MeetingMessagePrefetchProperties), base.UserContext, null, base.IsPreviewForm, base.IsInDeleteItems, base.IsEmbeddedItem, this.isInJunkEmailFolder, this.isSuspectedPhishingItem, this.itemLinkEnabled);
				if (!this.IsDraft)
				{
					this.shouldRenderToolbar = true;
					if (!base.IsEmbeddedItem && this.IsOwnerMailboxSession && (this.IsItemFromOtherMailbox || this.IsDelegated || Utilities.IsItemInExternalSharedInFolder(base.UserContext, base.Item)))
					{
						this.GetPrincipalCalendarFolderId();
					}
				}
			}
			else if (ObjectClass.IsMeetingCancellation(itemType))
			{
				this.meetingPageWriter = new MeetingCancelWriter(base.Item = base.Initialize<MeetingCancellation>(MeetingPageWriter.MeetingMessagePrefetchProperties), base.UserContext, null, base.IsPreviewForm, base.IsInDeleteItems, base.IsEmbeddedItem, this.isInJunkEmailFolder, this.isSuspectedPhishingItem, this.itemLinkEnabled);
				if (!this.IsDraft)
				{
					this.shouldRenderToolbar = true;
					if (!base.IsEmbeddedItem && this.IsOwnerMailboxSession && (this.IsItemFromOtherMailbox || this.IsDelegated || Utilities.IsItemInExternalSharedInFolder(base.UserContext, base.Item)))
					{
						this.GetPrincipalCalendarFolderId();
					}
				}
			}
			else if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(itemType))
			{
				CalendarItemBase calendarItemBase = base.Item = base.Initialize<CalendarItemBase>(MeetingPageWriter.CalendarPrefetchProperties);
				this.isCalendarItem = true;
				this.isMeeting = calendarItemBase.IsMeeting;
				this.meetingPageWriter = new CalendarItemWriter(calendarItemBase, base.UserContext, base.IsPreviewForm, base.IsInDeleteItems, base.IsEmbeddedItem, this.isInJunkEmailFolder, this.isSuspectedPhishingItem, this.itemLinkEnabled);
				this.shouldRenderToolbar = true;
				if (!base.IsEmbeddedItem && this.IsOwnerMailboxSession && (this.IsItemFromOtherMailbox || Utilities.IsItemInExternalSharedInFolder(base.UserContext, base.Item)))
				{
					this.GetPrincipalCalendarFolderId();
				}
			}
			else
			{
				if (!ObjectClass.IsMeetingResponse(itemType))
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Unsupported item type '{0}' for meeting page", itemType);
					throw new OwaInvalidRequestException(string.Format("Unsupported item type '{0}' for edit meeting page", itemType));
				}
				MeetingResponse meetingResponse = base.Item = base.Initialize<MeetingResponse>(MeetingPageWriter.MeetingMessagePrefetchProperties);
				this.meetingPageWriter = new MeetingResponseWriter(meetingResponse, base.UserContext, base.IsPreviewForm, base.IsInDeleteItems, base.IsEmbeddedItem, this.isInJunkEmailFolder, this.isSuspectedPhishingItem, this.itemLinkEnabled);
				if (meetingResponse.From != null && this.IsDraft)
				{
					this.shouldRenderSendOnBehalf = (string.CompareOrdinal(base.UserContext.ExchangePrincipal.LegacyDn, meetingResponse.From.EmailAddress) != 0);
				}
				this.shouldRenderToolbar = true;
			}
			if (this.MeetingPageWriter.ShouldRenderReminder && this.MeetingPageWriter.CalendarItemBase != null)
			{
				this.disableOccurrenceReminderUI = MeetingUtilities.CheckShouldDisableOccurrenceReminderUI(this.MeetingPageWriter.CalendarItemBase);
				if (this.disableOccurrenceReminderUI && !this.IsPublicItem)
				{
					this.MeetingPageWriter.FormInfobar.AddMessage(SanitizedHtmlString.FromStringId(-891369593), InfobarMessageType.Informational);
				}
			}
		}

		private void GetPrincipalCalendarFolderId()
		{
			try
			{
				this.principalCalendarFolderId = this.meetingPageWriter.GetPrincipalCalendarFolderId(this.isCalendarItem);
			}
			catch (OwaSharedFromOlderVersionException)
			{
				this.sharedFromOlderVersion = true;
				if (base.Item is MeetingMessage)
				{
					CalendarUtilities.GetReceiverGSCalendarIdStringAndDisplayName(base.UserContext, (MeetingMessage)base.Item, out this.principalCalendarFolderId, out this.receiverDisplayName);
				}
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			try
			{
				if (this.meetingPageWriter != null)
				{
					this.meetingPageWriter.Dispose();
					this.meetingPageWriter = null;
				}
			}
			finally
			{
				base.OnUnload(e);
			}
		}

		protected virtual void LoadMessageBodyIntoStream(TextWriter writer)
		{
			string action = base.IsPreviewForm ? "Preview" : string.Empty;
			string attachmentUrl = null;
			if (base.IsEmbeddedItemInNonSMimeItem)
			{
				attachmentUrl = base.RenderEmbeddedUrl();
			}
			base.AttachmentLinks = BodyConversionUtilities.GenerateNonEditableMessageBodyAndRenderInfobarMessages(base.Item, writer, base.OwaContext, this.meetingPageWriter.FormInfobar, base.ForceAllowWebBeacon, base.ForceEnableItemLink, base.Item.ClassName, action, string.Empty, base.IsEmbeddedItemInNonSMimeItem, attachmentUrl);
		}

		protected void CreateAttachmentHelpers()
		{
			this.attachmentInformation = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem, base.ForceEnableItemLink);
			this.hasAttachments = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, base.ForceEnableItemLink, this.meetingPageWriter.FormInfobar, this.attachmentInformation);
			base.SetShouldRenderDownloadAllLink(this.attachmentInformation);
		}

		protected void RenderCalendarItemId()
		{
			CalendarItemBase calendarItemBase = this.meetingPageWriter.CalendarItemBase;
			if (calendarItemBase == null || calendarItemBase.Id == null)
			{
				base.Response.Write("null");
				return;
			}
			base.Response.Write("\"");
			Utilities.JavascriptEncode(Utilities.GetIdAsString(calendarItemBase), base.Response.Output);
			base.Response.Write("\"");
		}

		protected void BuildCalendarInfobar()
		{
			if (base.IsPreviewForm || base.IsEmbeddedItem)
			{
				return;
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "clr", false);
			if (queryStringParameter == null)
			{
				return;
			}
			CalendarUtilities.BuildCalendarInfobar(this.meetingPageWriter.FormInfobar, base.UserContext, base.ParentFolderId, CalendarColorManager.ParseColorIndexString(queryStringParameter, true), false);
		}

		protected void RenderReminderDropdownList()
		{
			CalendarUtilities.RenderReminderDropdownList(base.Response.Output, base.Item, this.MeetingPageWriter.ReminderIsSet, base.IsEmbeddedItem || this.DisableOccurrenceReminderUI || this.IsPublicItem || !this.CanEdit);
		}

		protected void RenderBusyTypeDropdownList()
		{
			CalendarUtilities.RenderBusyTypeDropdownList(base.Response.Output, base.Item, base.IsEmbeddedItem || !this.CanEdit);
		}

		protected bool DisableReminderCheckbox
		{
			get
			{
				return base.IsEmbeddedItem || this.DisableOccurrenceReminderUI || this.IsPublicItem || !this.CanEdit;
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderCategories()
		{
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(base.Item, base.Response.Output, "DIV#divBdy");
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedCalendarItemBaseMasterId()
		{
			CalendarItemBase calendarItemBase = this.meetingPageWriter.CalendarItemBase;
			if (this.isCalendarItem && !base.IsEmbeddedItem && !base.IsInDeleteItems && (calendarItemBase.CalendarItemType == CalendarItemType.Occurrence || calendarItemBase.CalendarItemType == CalendarItemType.Exception))
			{
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(calendarItemBase);
				Utilities.JavascriptEncode(owaStoreObjectId.ProviderLevelItemId.ToString(), base.Response.Output);
			}
		}

		protected void RenderJavascriptEncodedPrincipalCalendarFolderId()
		{
			if (this.principalCalendarFolderId != null)
			{
				Utilities.JavascriptEncode(this.principalCalendarFolderId, base.Response.Output);
			}
		}

		protected void RenderSentTime(TextWriter writer)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			this.RenderSentTime(sanitizingStringBuilder);
			writer.Write(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
		}

		protected void RenderSentTime(SanitizingStringBuilder<OwaHtml> stringBuilder)
		{
			if (base.UserContext.IsSenderPhotosFeatureEnabled(Feature.DisplayPhotos))
			{
				stringBuilder.Append("<span class=\"spnSentInSender snt\">");
			}
			stringBuilder.Append("<span>");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(295620541));
			stringBuilder.Append("</span>");
			stringBuilder.Append("<span id=spnSent> ");
			stringBuilder.Append(base.UserContext.DirectionMark);
			RenderingUtilities.RenderSentTime(stringBuilder, this.SentTime, base.UserContext);
			stringBuilder.Append("</span>");
			if (base.UserContext.IsSenderPhotosFeatureEnabled(Feature.DisplayPhotos))
			{
				stringBuilder.Append("</span>");
			}
		}

		protected bool IsFromOlderVersion
		{
			get
			{
				return this.sharedFromOlderVersion;
			}
		}

		protected bool IsMeeting
		{
			get
			{
				return this.isMeeting;
			}
		}

		protected bool MeetingRequestWasSent
		{
			get
			{
				return base.Item is CalendarItemBase && this.IsMeeting && (base.Item as CalendarItemBase).MeetingRequestWasSent;
			}
		}

		protected bool CanEdit
		{
			get
			{
				if (this.canEdit == null)
				{
					this.canEdit = new bool?(ItemUtility.UserCanEditItem(base.Item) && !this.IsInExternalSharedInFolder);
				}
				return this.canEdit.Value;
			}
		}

		protected bool CanDelete
		{
			get
			{
				if (this.canDelete == null)
				{
					if (base.Item is CalendarItemBase)
					{
						this.canDelete = new bool?(CalendarUtilities.UserCanDeleteCalendarItem(base.Item as CalendarItemBase));
					}
					else
					{
						this.canDelete = new bool?(ItemUtility.UserCanDeleteItem(base.Item));
					}
					this.canDelete = new bool?(this.canDelete.Value && !this.IsInExternalSharedInFolder);
				}
				return this.canDelete.Value;
			}
		}

		protected bool CanReply
		{
			get
			{
				return ItemUtility.IsReplySupported(base.Item);
			}
		}

		protected bool CanForward
		{
			get
			{
				return ItemUtility.IsForwardSupported(base.Item);
			}
		}

		protected bool IsInExternalSharedInFolder
		{
			get
			{
				if (this.isInExternalSharedInFolder == null)
				{
					this.isInExternalSharedInFolder = new bool?(Utilities.IsItemInExternalSharedInFolder(base.UserContext, base.Item));
				}
				return this.isInExternalSharedInFolder.Value;
			}
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected MeetingPageWriter MeetingPageWriter
		{
			get
			{
				return this.meetingPageWriter;
			}
			set
			{
				this.meetingPageWriter = value;
			}
		}

		protected bool IsPrivate
		{
			get
			{
				object obj = base.Item.TryGetProperty(ItemSchema.Sensitivity);
				return obj is Sensitivity && (Sensitivity)obj == Sensitivity.Private;
			}
		}

		protected bool IsDraft
		{
			get
			{
				object obj = base.Item.TryGetProperty(MessageItemSchema.IsDraft);
				return obj is bool && (bool)obj;
			}
		}

		protected bool ShouldRenderSentField
		{
			get
			{
				return this.sentTime != ExDateTime.MinValue && this.MeetingPageWriter.ShouldRenderSentField;
			}
		}

		protected ExDateTime SentTime
		{
			get
			{
				return this.sentTime;
			}
		}

		protected bool ShouldRenderReminder
		{
			get
			{
				return this.MeetingPageWriter.ShouldRenderReminder;
			}
		}

		protected bool ShouldRenderInspectorMailToolbar
		{
			get
			{
				return !base.IsPreviewForm && !base.IsEmbeddedItem;
			}
		}

		protected bool ShouldRenderToolbar
		{
			get
			{
				return !this.IsPublicItem && !base.IsEmbeddedItem && this.shouldRenderToolbar;
			}
		}

		protected bool ShouldRenderSendOnBehalf
		{
			get
			{
				return this.shouldRenderSendOnBehalf;
			}
		}

		protected virtual bool HasAttachments
		{
			get
			{
				return this.hasAttachments;
			}
		}

		public ArrayList AttachmentInformation
		{
			get
			{
				return this.attachmentInformation;
			}
		}

		protected string SequenceNumber
		{
			get
			{
				object obj = base.Item.TryGetProperty(CalendarItemBaseSchema.AppointmentSequenceNumber);
				if (obj is int)
				{
					return ((int)obj).ToString(CultureInfo.InvariantCulture);
				}
				return "0";
			}
		}

		protected bool EnablePrivateCheckbox
		{
			get
			{
				return this.CanEdit && !this.IsPublicItem && !base.IsEmbeddedItem && !this.IsItemFromOtherMailbox && (this.meetingPageWriter.CalendarItemBase == null || (this.meetingPageWriter.CalendarItemBase != null && (this.meetingPageWriter.CalendarItemBase.CalendarItemType == CalendarItemType.Single || this.meetingPageWriter.CalendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster)));
			}
		}

		protected bool DisableOccurrenceReminderUI
		{
			get
			{
				return this.disableOccurrenceReminderUI;
			}
		}

		protected bool IsInJunkMailFolder
		{
			get
			{
				return this.isInJunkEmailFolder;
			}
		}

		protected bool IsSuspectedPhishingItem
		{
			get
			{
				return this.isSuspectedPhishingItem;
			}
		}

		protected bool IsSuspectedPhishingItemWithoutLinkEnabled
		{
			get
			{
				return this.isSuspectedPhishingItem && !this.itemLinkEnabled;
			}
		}

		protected bool IsDelegated
		{
			get
			{
				return !this.isCalendarItem && ((MeetingMessage)base.Item).IsDelegated();
			}
		}

		protected bool IsItemFromOtherMailbox
		{
			get
			{
				return base.UserContext.IsInOtherMailbox(base.Item);
			}
		}

		protected bool IsOwnerMailboxSession
		{
			get
			{
				return base.UserContext.MailboxSession.LogonType == LogonType.Owner;
			}
		}

		protected int MeetingPageWriterStoreObjectType
		{
			get
			{
				return this.meetingPageWriter.StoreObjectType;
			}
		}

		protected static int StoreObjectTypeMeetingResponse
		{
			get
			{
				return 12;
			}
		}

		protected static int StoreObjectTypeMeetingRequest
		{
			get
			{
				return 11;
			}
		}

		protected static int StoreObjectTypeMeetingCancellation
		{
			get
			{
				return 13;
			}
		}

		protected static int StoreObjectTypeCalendarItem
		{
			get
			{
				return 15;
			}
		}

		protected RecipientJunkEmailContextMenuType RecipientJunkEmailMenuType
		{
			get
			{
				RecipientJunkEmailContextMenuType result = RecipientJunkEmailContextMenuType.None;
				if (base.UserContext.IsJunkEmailEnabled)
				{
					result = RecipientJunkEmailContextMenuType.SenderAndRecipient;
				}
				return result;
			}
		}

		protected FlagAction FlagAction
		{
			get
			{
				return FlagContextMenu.GetFlagActionForItem(base.UserContext, base.Item);
			}
		}

		private const string TypeString = "t";

		private MeetingPageWriter meetingPageWriter;

		private bool shouldRenderToolbar;

		private Markup bodyMarkup;

		private ArrayList attachmentInformation;

		private bool hasAttachments;

		private ExDateTime sentTime = ExDateTime.MinValue;

		private bool isCalendarItem;

		private bool isMeeting;

		private bool sharedFromOlderVersion;

		private string receiverDisplayName;

		private bool? isInExternalSharedInFolder;

		private bool? canEdit;

		private bool? canDelete;

		private bool disableOccurrenceReminderUI;

		private bool isInJunkEmailFolder;

		private bool isSuspectedPhishingItem;

		private bool itemLinkEnabled;

		private bool isJunkOrPhishing;

		private string principalCalendarFolderId;

		private bool shouldRenderSendOnBehalf;
	}
}
