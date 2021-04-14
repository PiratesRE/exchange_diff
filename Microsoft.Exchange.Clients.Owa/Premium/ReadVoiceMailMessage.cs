using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.ClientAccess;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadVoiceMailMessage : OwaForm, IRegistryOnlyForm
	{
		internal MessageItem Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.Message = base.Initialize<MessageItem>(new PropertyDefinition[]
			{
				MessageItemSchema.IsRead,
				BodySchema.Codepage,
				BodySchema.InternetCpid,
				MessageItemSchema.MessageAudioNotes,
				MessageItemSchema.SenderTelephoneNumber,
				ItemSchema.FlagStatus,
				ItemSchema.FlagCompleteTime,
				MessageItemSchema.ReplyTime,
				MessageItemSchema.RequireProtectedPlayOnPhone,
				ItemSchema.UtcDueDate,
				ItemSchema.UtcStartDate,
				ItemSchema.ReminderDueBy,
				ItemSchema.ReminderIsSet,
				StoreObjectSchema.EffectiveRights
			});
			this.IrmItemHelper = new IRMItemHelper(this.Message, base.UserContext, base.IsPreviewForm, base.IsEmbeddedItem);
			using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
			{
				this.isUMEnabled = umclientCommon.IsUMEnabled();
				this.isPlayOnPhoneEnabled = umclientCommon.IsPlayOnPhoneEnabled();
			}
			this.IrmItemHelper.IrmDecryptIfRestricted();
			bool isSuspectedPhishingItem = false;
			bool isLinkEnabled = false;
			bool flag = false;
			this.isMacintoshPlatform = (Utilities.GetBrowserPlatform(base.Request.UserAgent) == BrowserPlatform.Macintosh);
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(this.Message, base.IsEmbeddedItem, base.ForceEnableItemLink, base.UserContext, out this.isInJunkEmailFolder, out isSuspectedPhishingItem, out isLinkEnabled, out flag);
			this.toolbar = new ReadMessageToolbar(base.IsInDeleteItems, base.IsEmbeddedItem, this.Message, this.isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled, false, this.IrmItemHelper.IsReplyRestricted, this.IrmItemHelper.IsReplyAllRestricted, this.IrmItemHelper.IsForwardRestricted, this.IrmItemHelper.IsPrintRestricted);
			this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
			this.recipientWell = new MessageRecipientWell(this.Message);
			if (flag)
			{
				this.bodyMarkup = Markup.PlainText;
			}
			InfobarMessageBuilder.AddImportance(this.infobar, this.Message);
			InfobarMessageBuilder.AddFlag(this.infobar, this.Message, base.UserContext);
			InfobarMessageBuilder.AddSensitivity(this.infobar, this.Message);
			if (base.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(this.Message))
			{
				InfobarMessageBuilder.AddIrmInformation(this.infobar, this.Message, base.IsPreviewForm, false, false, false);
			}
			if (!this.Message.IsRead && !base.IsPreviewForm && !base.IsEmbeddedItem)
			{
				this.Message.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, this.Message), false);
			}
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (this.IrmItemHelper.IsRestrictedButIrmFeatureDisabledOrDecryptionFailed)
			{
				this.IrmItemHelper.RenderAlternateBodyForIrm(writer, true);
				return;
			}
			string action = base.IsPreviewForm ? "Preview" : string.Empty;
			string attachmentUrl = null;
			if (base.IsEmbeddedItemInNonSMimeItem)
			{
				attachmentUrl = base.RenderEmbeddedUrl();
			}
			base.AttachmentLinks = BodyConversionUtilities.GenerateNonEditableMessageBodyAndRenderInfobarMessages(this.Message, writer, base.OwaContext, this.infobar, base.ForceAllowWebBeacon, base.ForceEnableItemLink, "IPM.Note", action, string.Empty, base.IsEmbeddedItemInNonSMimeItem, attachmentUrl);
		}

		protected void CreateAttachmentHelpers()
		{
			if (this.Message.IsRestricted)
			{
				this.shouldRenderAttachmentWell = false;
				return;
			}
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.Message, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem, base.ForceEnableItemLink);
			this.shouldRenderAttachmentWell = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, base.ForceEnableItemLink, this.infobar, this.attachmentWellRenderObjects);
		}

		protected void RenderVoiceMailPlayer(TextWriter writer)
		{
			if (this.isUMEnabled && !base.IsEmbeddedItem && base.UserContext.BrowserType == BrowserType.IE && !this.isMacintoshPlatform)
			{
				string text = this.Message.TryGetProperty(MessageItemSchema.RequireProtectedPlayOnPhone) as string;
				if (this.IrmItemHelper.IsRestrictedAndIrmFeatureEnabled && !string.IsNullOrEmpty(text) && text.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				using (Attachment latestVoiceMailAttachment = Utilities.GetLatestVoiceMailAttachment(this.Message, base.UserContext))
				{
					if (latestVoiceMailAttachment != null)
					{
						writer.Write("<object id=oMpf classid=\"clsid:6bf52a52-394a-11d3-b153-00c04f79faa6\" ");
						writer.Write("type=\"application/x-oleobject\" width=\"212\" height=\"45\">");
						writer.Write("<param name=\"URL\" value=\"");
						Utilities.WriteLatestUrlToAttachment(writer, Utilities.GetIdAsString(this.Message), latestVoiceMailAttachment.FileExtension);
						writer.Write("\">");
						writer.Write("<param name=\"autoStart\" value=\"false\"><param name=\"EnableContextMenu\" value=\"0\">");
						writer.Write("<param name=\"InvokeURLs\" value=\"0\"></object>");
					}
					else
					{
						this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-229902107), InfobarMessageType.Informational);
					}
				}
			}
		}

		protected void RenderSender()
		{
			UnifiedMessagingUtilities.RenderSender(base.UserContext, base.Response.Output, this.Message);
		}

		protected void RenderToolbar()
		{
			this.toolbar.Render(base.Response.Output);
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

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				RenderingUtilities.RenderSubject(base.Response.Output, this.Message, LocalizedStrings.GetNonEncoded(730745110));
				return;
			}
			RenderingUtilities.RenderSubject(base.Response.Output, this.Message);
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedMessageChangeKey()
		{
			Utilities.JavascriptEncode(this.Message.Id.ChangeKeyAsBase64String(), base.Response.Output);
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected static string SaveNamespace
		{
			get
			{
				return "ReadMessage";
			}
		}

		protected IRMItemHelper IrmItemHelper { get; set; }

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell;
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected string AudioNotes
		{
			get
			{
				string text = this.Message.TryGetProperty(MessageItemSchema.MessageAudioNotes) as string;
				if (string.IsNullOrEmpty(text))
				{
					return LocalizedStrings.GetHtmlEncoded(-1207478783);
				}
				return Utilities.HtmlEncode(text);
			}
		}

		protected bool IsAudioNotesPresent
		{
			get
			{
				string value = this.Message.TryGetProperty(MessageItemSchema.MessageAudioNotes) as string;
				return !string.IsNullOrEmpty(value);
			}
		}

		protected bool IsAudioNotesEditable
		{
			get
			{
				return this.IsItemEditable && !base.IsEmbeddedItem;
			}
		}

		protected bool IsPlayOnPhoneEnabled
		{
			get
			{
				return this.isPlayOnPhoneEnabled;
			}
		}

		protected bool IsInJunkMailFolder
		{
			get
			{
				return this.isInJunkEmailFolder;
			}
		}

		protected static int StoreObjectTypeMessage
		{
			get
			{
				return 9;
			}
		}

		protected ExDateTime MessageSentTime
		{
			get
			{
				return this.Message.SentTime;
			}
		}

		protected bool IsMacintoshPlatform
		{
			get
			{
				return this.isMacintoshPlatform;
			}
		}

		protected string PhoneNumber
		{
			get
			{
				string result = string.Empty;
				using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
				{
					if (umclientCommon.IsUMEnabled())
					{
						result = umclientCommon.GetUMProperties().PlayOnPhoneDialString;
					}
				}
				return result;
			}
		}

		protected static int UMCallStateIdle
		{
			get
			{
				return 0;
			}
		}

		protected static int UMCallStateConnecting
		{
			get
			{
				return 1;
			}
		}

		protected static int UMCallStateConnected
		{
			get
			{
				return 3;
			}
		}

		protected static int UMCallStateDisconnected
		{
			get
			{
				return 4;
			}
		}

		protected FlagAction FlagAction
		{
			get
			{
				return FlagContextMenu.GetFlagActionForItem(base.UserContext, this.Message);
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

		private MessageItem message;

		private MessageRecipientWell recipientWell;

		private Infobar infobar = new Infobar();

		private ArrayList attachmentWellRenderObjects;

		private ReadMessageToolbar toolbar;

		private bool shouldRenderAttachmentWell;

		private bool isPlayOnPhoneEnabled;

		private bool isUMEnabled;

		private Markup bodyMarkup;

		private bool isInJunkEmailFolder;

		private bool isMacintoshPlatform;
	}
}
