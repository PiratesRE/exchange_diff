using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditMeetingResponse : EditItemForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.infobar.SetInfobarClass("infobarEdit");
			this.infobar.SetShouldHonorHideByDefault(true);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "a", false);
			string type = base.OwaContext.FormsRegistryContext.Type;
			if (ObjectClass.IsMeetingResponse(type))
			{
				this.isNew = string.Equals(queryStringParameter, "New", StringComparison.OrdinalIgnoreCase);
				if (this.isNew)
				{
					this.newItemType = NewItemType.New;
				}
				base.Item = base.Initialize<MeetingResponse>(EditMeetingResponse.prefetchProperties);
				MeetingMessage meetingMessage = (MeetingMessage)base.Item;
				if (meetingMessage.From != null)
				{
					this.isSendOnBehalfOf = (string.CompareOrdinal(base.UserContext.ExchangePrincipal.LegacyDn, meetingMessage.From.EmailAddress) != 0);
				}
				this.InitializeMeetingResponse();
			}
			else if (ObjectClass.IsMeetingRequest(type))
			{
				this.itemType = StoreObjectType.MeetingRequest;
				this.isNew = string.Equals(queryStringParameter, "Forward", StringComparison.OrdinalIgnoreCase);
				if (this.isNew)
				{
					this.newItemType = NewItemType.Forward;
				}
				base.Item = base.Initialize<MeetingRequest>(EditMeetingResponse.prefetchProperties);
			}
			else
			{
				if (!ObjectClass.IsMeetingCancellation(type))
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Unsupported item type '{0}' for edit meeting page", type);
					throw new OwaInvalidRequestException(string.Format("Unsupported item type '{0}' for edit meeting page", type));
				}
				this.itemType = StoreObjectType.MeetingCancellation;
				this.isNew = string.Equals(queryStringParameter, "Forward", StringComparison.OrdinalIgnoreCase);
				if (this.isNew)
				{
					this.newItemType = NewItemType.Forward;
				}
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-161808760), InfobarMessageType.Informational);
				base.Item = base.Initialize<MeetingCancellation>(EditMeetingResponse.prefetchProperties);
			}
			base.DeleteExistingDraft = this.isNew;
			if (!this.isNew && base.Item is MessageItem)
			{
				MessageItem messageItem = (MessageItem)base.Item;
				if (messageItem.GetValueOrDefault<bool>(MessageItemSchema.HasBeenSubmitted))
				{
					messageItem.AbortSubmit();
				}
			}
			this.bodyMarkup = BodyConversionUtilities.GetBodyFormatOfEditItem(base.Item, this.newItemType, base.UserContext.UserOptions);
			this.toolbar = new EditMessageToolbar(((MeetingMessage)base.Item).Importance, this.bodyMarkup);
			this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
			this.toolbar.IsComplianceButtonAllowedInForm = false;
			this.messageRecipientWell = new MessageRecipientWell((MeetingMessage)base.Item);
			this.showBcc = this.messageRecipientWell.HasRecipients(RecipientWellType.Bcc);
		}

		private void InitializeMeetingResponse()
		{
			MeetingResponse meetingResponse = (MeetingResponse)base.Item;
			this.responseType = meetingResponse.ResponseType;
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "mid", false);
			if (queryStringParameter != null)
			{
				this.meetingRequestId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			}
			this.isMeetingInviteInDeleteItems = (Utilities.GetQueryStringParameter(base.Request, "d", false) != null);
			this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-1981719796), InfobarMessageType.Informational);
			string format = string.Empty;
			switch (this.responseType)
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
			if (this.isSendOnBehalfOf)
			{
				messageHtml = SanitizedHtmlString.Format(format, new object[]
				{
					meetingResponse.From.DisplayName
				});
			}
			else
			{
				messageHtml = SanitizedHtmlString.Format(format, new object[]
				{
					LocalizedStrings.GetNonEncoded(372029413)
				});
			}
			this.infobar.AddMessage(messageHtml, InfobarMessageType.Informational);
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			bool flag = BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(base.Item, writer, this.newItemType, base.OwaContext, ref this.shouldPromptUserForUnblockingOnFormLoad, ref this.hasInlineImages, this.infobar, base.IsRequestCallbackForWebBeacons, this.bodyMarkup);
			if (flag)
			{
				base.Item.Load();
			}
		}

		protected void CreateAttachmentHelpers()
		{
			bool isPublicLogon = base.UserContext.IsPublicLogon;
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, isPublicLogon);
			InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
			if (infobarRenderingHelper.HasLevelOne)
			{
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
			}
		}

		protected void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			MeetingMessage meetingMessage = base.Item as MeetingMessage;
			if (Utilities.IsOnBehalfOf(meetingMessage.Sender, meetingMessage.From))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetSender(base.UserContext, meetingMessage.Sender, "spnFrom", false, SenderDisplayMode.NameOnly), RenderingUtilities.GetSender(base.UserContext, meetingMessage.From, "spnOrg", false, SenderDisplayMode.NameOnly));
				return;
			}
			RenderingUtilities.RenderSender(base.UserContext, writer, meetingMessage.From, SenderDisplayMode.NameOnly, null);
		}

		protected void RenderSendOnBehalf(TextWriter writer)
		{
			MeetingMessage meetingMessage = (MeetingMessage)base.Item;
			if (this.isSendOnBehalfOf)
			{
				RenderingUtilities.RenderSendOnBehalf(writer, base.UserContext, meetingMessage.From);
			}
		}

		protected void RenderToolbar()
		{
			this.toolbar.Render(base.SanitizingResponse);
		}

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				RenderingUtilities.RenderSubject(base.SanitizingResponse, base.Item, LocalizedStrings.GetNonEncoded(-1500721828));
				return;
			}
			RenderingUtilities.RenderSubject(base.SanitizingResponse, base.Item);
		}

		protected void RenderJavascriptEncodedMeetingRequestId()
		{
			if (this.meetingRequestId != null)
			{
				Utilities.JavascriptEncode(this.meetingRequestId.ToBase64String(), base.SanitizingResponse);
			}
		}

		protected int CurrentStoreObjectType
		{
			get
			{
				return (int)this.itemType;
			}
		}

		protected NewItemType ItemState
		{
			get
			{
				return this.newItemType;
			}
		}

		protected int MeetingResponseType
		{
			get
			{
				return (int)this.responseType;
			}
		}

		protected bool IsSendOnBehalfOf
		{
			get
			{
				return this.isSendOnBehalfOf;
			}
		}

		protected SanitizedHtmlString When
		{
			get
			{
				return Utilities.SanitizeHtmlEncode(Utilities.GenerateWhen(base.Item));
			}
		}

		protected SanitizedHtmlString Location
		{
			get
			{
				string text = base.Item.TryGetProperty(CalendarItemBaseSchema.Location) as string;
				if (text != null)
				{
					return Utilities.SanitizeHtmlEncode(text);
				}
				return SanitizedHtmlString.Empty;
			}
		}

		protected bool ShowBcc
		{
			get
			{
				return this.showBcc;
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.messageRecipientWell;
			}
		}

		protected EditMessageToolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected bool IsMeetingInviteInDeleteItems
		{
			get
			{
				return this.isMeetingInviteInDeleteItems;
			}
		}

		protected bool IsReadReceiptRequested
		{
			get
			{
				return !this.isNew && ((MessageItem)base.Item).IsReadReceiptRequested;
			}
		}

		protected bool IsDeliveryReceiptRequested
		{
			get
			{
				return !this.isNew && ((MessageItem)base.Item).IsDeliveryReceiptRequested;
			}
		}

		protected bool ShouldPromptUser
		{
			get
			{
				return this.shouldPromptUserForUnblockingOnFormLoad;
			}
		}

		protected bool HasInlineImages
		{
			get
			{
				return this.hasInlineImages;
			}
		}

		protected static int StoreObjectTypeMessage
		{
			get
			{
				return 9;
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

		protected bool IsMeetingRequestIdNull
		{
			get
			{
				return this.meetingRequestId == null;
			}
		}

		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected int ItemSensitivity
		{
			get
			{
				return (int)base.Item.Sensitivity;
			}
		}

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			MessageItemSchema.IsDraft,
			MessageItemSchema.IsDeliveryReceiptRequested,
			MessageItemSchema.HasBeenSubmitted
		};

		private MessageRecipientWell messageRecipientWell;

		private ArrayList attachmentWellRenderObjects;

		private Infobar infobar = new Infobar();

		private EditMessageToolbar toolbar;

		private bool showBcc;

		private bool isNew;

		private Markup bodyMarkup;

		private bool shouldPromptUserForUnblockingOnFormLoad;

		private bool hasInlineImages;

		private StoreObjectType itemType = StoreObjectType.MeetingResponse;

		private OwaStoreObjectId meetingRequestId;

		private ResponseType responseType = ResponseType.Tentative;

		private bool isMeetingInviteInDeleteItems;

		private NewItemType newItemType = NewItemType.ExplicitDraft;

		private bool isSendOnBehalfOf;
	}
}
