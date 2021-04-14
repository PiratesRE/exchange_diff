using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadMessage : OwaFormSubPage, IRegistryOnlyForm
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
			string itemType = base.GetItemType();
			StorePropertyDefinition[] prefetchProperties = new StorePropertyDefinition[]
			{
				ItemSchema.BlockStatus,
				ItemSchema.IsClassified,
				ItemSchema.Classification,
				ItemSchema.ClassificationDescription,
				ItemSchema.ClassificationGuid,
				ItemSchema.EdgePcl,
				ItemSchema.LinkEnabled,
				BodySchema.Codepage,
				BodySchema.InternetCpid,
				MessageItemSchema.SenderTelephoneNumber,
				ItemSchema.FlagStatus,
				ItemSchema.FlagCompleteTime,
				MessageItemSchema.ReplyTime,
				ItemSchema.UtcDueDate,
				ItemSchema.UtcStartDate,
				ItemSchema.ReminderDueBy,
				ItemSchema.ReminderIsSet,
				StoreObjectSchema.EffectiveRights,
				ItemSchema.Categories,
				MessageItemSchema.IsReadReceiptPending,
				MessageItemSchema.ApprovalDecision,
				MessageItemSchema.ApprovalDecisionMaker,
				MessageItemSchema.ApprovalDecisionTime,
				StoreObjectSchema.PolicyTag,
				ItemSchema.RetentionDate,
				MessageItemSchema.TextMessageDeliveryStatus,
				StoreObjectSchema.ParentItemId
			};
			if (ObjectClass.IsMessage(itemType, false))
			{
				this.Message = base.Initialize<MessageItem>(prefetchProperties);
			}
			else
			{
				this.Message = base.InitializeAsMessageItem(prefetchProperties);
			}
			this.IrmItemHelper = new IRMItemHelper(this.Message, base.UserContext, base.IsPreviewForm, base.IsEmbeddedItem);
			this.IrmItemHelper.IrmDecryptIfRestricted();
			if (ObjectClass.IsOfClass(itemType, "IPM.Note.Microsoft.Fax.CA"))
			{
				this.isFaxMessage = true;
			}
			this.InitializeReadMessageFormElements();
			if (!this.IsSMimeItem)
			{
				RenderingUtilities.RenderVotingInfobarMessages(this.Message, this.infobar, base.UserContext);
			}
			object obj = this.Message.TryGetProperty(MessageItemSchema.IsDraft);
			if (obj is bool && (bool)obj)
			{
				this.isDraftMessage = true;
				if (!base.IsPreviewForm)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-1981719796), InfobarMessageType.Informational);
				}
				this.AddIrmMessageToInfobar();
			}
			else
			{
				this.AddMessagesToInfobar();
				if (this.Message.Id != null && !base.IsEmbeddedItem && !this.Message.IsRead && !base.IsPreviewForm)
				{
					this.Message.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, this.Message), false);
				}
			}
			SanitizedHtmlString sanitizedHtmlString = null;
			if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 1))
			{
				if (this.IsClearSignedItem || this.IsOpaqueSignedItem)
				{
					sanitizedHtmlString = SanitizedHtmlString.FromStringId(Utilities.IsSMimeFeatureUsable(base.OwaContext) ? (base.IsPreviewForm ? 1871698343 : 1683614199) : -1329088272);
				}
				else if (this.IsEncryptedItem)
				{
					sanitizedHtmlString = SanitizedHtmlString.FromStringId(Utilities.IsSMimeFeatureUsable(base.OwaContext) ? (base.IsPreviewForm ? 958219031 : 906798671) : -767943720);
				}
			}
			else if (this.IsSMimeItem)
			{
				if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 16))
				{
					if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 2))
					{
						sanitizedHtmlString = SanitizedHtmlString.FromStringId(base.IsPreviewForm ? -1214530702 : 1697878138);
					}
					else if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 4))
					{
						sanitizedHtmlString = SanitizedHtmlString.FromStringId(base.IsPreviewForm ? 1899236370 : 330022834);
					}
				}
				else
				{
					sanitizedHtmlString = SanitizedHtmlString.FromStringId((this.IsClearSignedItem || this.IsOpaqueSignedItem) ? 1965026784 : -514535677);
				}
			}
			if (sanitizedHtmlString != null)
			{
				this.infobar.AddMessage(sanitizedHtmlString, InfobarMessageType.Warning);
			}
		}

		protected void InitializeReadMessageFormElements()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(this.Message, base.IsEmbeddedItem, base.ForceEnableItemLink, base.UserContext, out this.isInJunkEmailFolder, out flag, out flag2, out flag3);
			this.isSuspectedPhishingItemWithoutLinkEnabled = (flag && !flag2);
			this.toolbar = new ReadMessageToolbar(base.IsInDeleteItems, base.IsEmbeddedItem, this.Message, this.isInJunkEmailFolder, flag, flag2, true, this.IrmItemHelper != null && this.IrmItemHelper.IsReplyRestricted, this.IrmItemHelper != null && this.IrmItemHelper.IsReplyAllRestricted, this.IrmItemHelper != null && this.IrmItemHelper.IsForwardRestricted, this.IrmItemHelper != null && this.IrmItemHelper.IsPrintRestricted);
			this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
			if (flag3)
			{
				this.bodyMarkup = Markup.PlainText;
			}
			this.recipientWell = new MessageRecipientWell(this.Message);
			RenderingUtilities.RenderReplyForwardMessageStatus(this.Message, this.infobar, base.UserContext);
		}

		protected virtual void AddMessagesToInfobar()
		{
			InfobarMessageBuilder.AddImportance(this.infobar, this.Message);
			InfobarMessageBuilder.AddSensitivity(this.infobar, this.Message);
			InfobarMessageBuilder.AddFlag(this.infobar, this.Message, base.UserContext);
			InfobarMessageBuilder.AddCompliance(base.UserContext, this.infobar, this.Message, false);
			InfobarMessageBuilder.AddDeletePolicyInformation(this.infobar, this.Message, base.UserContext);
			this.AddIrmMessageToInfobar();
			if (!base.IsEmbeddedItem && !this.IsPublicItem)
			{
				InfobarMessageBuilder.AddReadReceiptNotice(base.UserContext, this.infobar, this.Message);
			}
			if (ObjectClass.IsTaskRequest(this.Message.ClassName))
			{
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(357315796), InfobarMessageType.Informational);
			}
			if (TextMessagingUtilities.NeedToAddUnsyncedMessageInfobar(this.Message.ClassName, this.Message, base.UserContext.MailboxSession))
			{
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(882347163), InfobarMessageType.Informational);
			}
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (this.IrmItemHelper != null && this.IrmItemHelper.IsRestrictedButIrmFeatureDisabledOrDecryptionFailed)
			{
				this.IrmItemHelper.RenderAlternateBodyForIrm(writer, false);
				return;
			}
			string action = base.IsPreviewForm ? "Preview" : string.Empty;
			string attachmentUrl = null;
			if (base.IsEmbeddedItemInNonSMimeItem)
			{
				attachmentUrl = base.RenderEmbeddedUrl();
			}
			base.AttachmentLinks = BodyConversionUtilities.GenerateNonEditableMessageBodyAndRenderInfobarMessages(this.Message, writer, base.OwaContext, this.infobar, base.ForceAllowWebBeacon, base.ForceEnableItemLink, this.Message.ClassName, action, string.Empty, base.IsEmbeddedItemInNonSMimeItem, attachmentUrl);
		}

		protected void CreateAttachmentHelpers()
		{
			if (this.IrmItemHelper != null && this.IrmItemHelper.IsRestrictedButIrmFeatureDisabledOrDecryptionFailed)
			{
				this.shouldRenderAttachmentWell = false;
				return;
			}
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.Message, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem, base.ForceEnableItemLink);
			if (this.attachmentWellRenderObjects != null && this.attachmentWellRenderObjects.Count > 0 && this.IsSMimeItem)
			{
				AttachmentUtility.RemoveSmimeAttachment(this.attachmentWellRenderObjects);
			}
			base.SetShouldRenderDownloadAllLink(this.attachmentWellRenderObjects);
			this.shouldRenderAttachmentWell = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, base.ForceEnableItemLink, this.infobar, this.attachmentWellRenderObjects);
		}

		protected bool HasSender
		{
			get
			{
				return this.Message.Sender != null;
			}
		}

		protected void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (!this.isFaxMessage)
			{
				RenderingUtilities.RenderSender(base.UserContext, writer, this.Message, new RenderSubHeaderDelegate(this.RenderSentTime));
				return;
			}
			writer.Write(UnifiedMessagingUtilities.GetUMSender(base.UserContext, this.Message, "spnSender"));
		}

		protected void RenderSentTime()
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			this.RenderSentTime(sanitizingStringBuilder);
			base.SanitizingResponse.Write(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
		}

		protected void RenderSentTime(SanitizingStringBuilder<OwaHtml> stringBuilder)
		{
			stringBuilder.Append("<span id=\"spnSent\">");
			RenderingUtilities.RenderSentTime(stringBuilder, this.MessageSentTime, base.UserContext);
			stringBuilder.Append("</span>");
		}

		protected void RenderToolbar()
		{
			this.toolbar.Render(base.Response.Output);
		}

		protected void RenderApprovalToolbar()
		{
			if (this.Message.VotingInfo != null)
			{
				IList<VotingInfo.OptionData> optionsDataList = this.Message.VotingInfo.GetOptionsDataList();
				if (optionsDataList == null || optionsDataList.Count != 2)
				{
					return;
				}
				this.approvalRequestToolbar = new ApprovalRequestToolbar(optionsDataList[0].SendPrompt != VotingInfo.SendPrompt.Send, optionsDataList[1].SendPrompt != VotingInfo.SendPrompt.Send);
				this.approvalRequestToolbar.Render(base.Response.Output);
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

		protected virtual void RenderSubject()
		{
			if (this.IsSMimeItem)
			{
				this.RenderEncryptedMessageIcon(base.SanitizingResponse);
			}
			RenderingUtilities.RenderSubject(base.SanitizingResponse, this.Message);
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.SanitizingResponse);
		}

		protected void RenderJavascriptEncodedLocalizedApprovalDecision(bool decision)
		{
			if (this.Message.VotingInfo != null)
			{
				int index = decision ? 0 : 1;
				IList<VotingInfo.OptionData> optionsDataList = this.Message.VotingInfo.GetOptionsDataList();
				if (optionsDataList != null && optionsDataList.Count == 2)
				{
					Utilities.JavascriptEncode(optionsDataList[index].DisplayName, base.SanitizingResponse);
				}
			}
		}

		protected void RenderJavascriptEncodedLocalizedApproval()
		{
			this.RenderJavascriptEncodedLocalizedApprovalDecision(true);
		}

		protected void RenderJavascriptEncodedLocalizedReject()
		{
			this.RenderJavascriptEncodedLocalizedApprovalDecision(false);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.SanitizingResponse);
		}

		protected void RenderJavascriptEncodedMessageChangeKey()
		{
			Utilities.JavascriptEncode(this.Message.Id.ChangeKeyAsBase64String(), base.SanitizingResponse);
		}

		protected void RenderEncryptedMessageIcon(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<span id=\"spEn\" style=\"display:none\">");
			base.UserContext.RenderThemeImage(writer, ThemeFileId.Encrypted, null, new object[]
			{
				"id=\"imgEn\"",
				"title=\"",
				SanitizedHtmlString.FromStringId(1362348905),
				"\""
			});
			writer.Write("</span>");
		}

		protected void RenderSignatureLine(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<span id=\"spSigLi\">");
			writer.Write("<span id=\"spSigPro\" style=\"display:none\">");
			writer.Write("<span id=\"spSPI\">");
			writer.Write("<img id=\"imgSVP\" src=\"");
			base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.ProgressSmall);
			writer.Write("\">");
			writer.Write("</span><span id=\"spSPS\">");
			writer.Write(SanitizedHtmlString.FromStringId(-1793529945));
			writer.Write("</span>");
			writer.Write("</span>");
			writer.Write("<span class=\"sl\" id=\"spnSigRes\" style=\"display:none\" onresize=\"rszSl();\"></span>");
			writer.Write("</span>");
		}

		private void RenderSignatureImage(TextWriter writer, string id, ThemeFileId themeId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id may not be null or empty string");
			}
			base.UserContext.RenderThemeImage(writer, themeId, null, new object[]
			{
				"id=\"" + id + "\"",
				"style=\"display:none\""
			});
		}

		protected void RenderSignatureInfoDiv(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div id=\"divSDtl\" style=\"display:none\">");
			writer.Write("<div class=\"ssiSctHdr\">");
			writer.Write("<div class=\"ssiSctTxt ssiSctTxtFw\">");
			writer.Write(SanitizedHtmlString.FromStringId(40886466));
			writer.Write("</div>");
			writer.Write("</div>");
			writer.Write("<div id=\"siSctBdy\">");
			writer.Write("<table>");
			writer.Write("<tr id=\"trSigDlgEn\" style=\"display:none\">");
			writer.Write("<td>");
			base.UserContext.RenderThemeImage(writer, ThemeFileId.Encrypted);
			writer.Write("</td><td id=\"tdssDlgEI\">");
			writer.Write(SanitizedHtmlString.FromStringId(1362348905));
			writer.Write("</td></tr>");
			writer.Write("<tr>");
			writer.Write("<td>");
			this.RenderSignatureImage(writer, "imgDVS", ThemeFileId.ValidSignature);
			this.RenderSignatureImage(writer, "imgDWS", ThemeFileId.WarningSignature);
			this.RenderSignatureImage(writer, "imgDIVS", ThemeFileId.InvalidSignature);
			writer.Write("</td>");
			writer.Write("<td id=\"tdSsSs\"></td>");
			writer.Write("</tr>");
			writer.Write("<tr>");
			writer.Write("<td></td>");
			writer.Write("<td id=\"tdSsEs\"></td>");
			writer.Write("</tr>");
			writer.Write("</table>");
			writer.Write("</div>");
			writer.Write("<div class=\"ssiSctHdr\">");
			writer.Write("<div class=\"ssiSctTxt ssiSctTxtFw\">");
			writer.Write(SanitizedHtmlString.FromStringId(63112306));
			writer.Write("</div>");
			writer.Write("</div>");
			writer.Write("<div id=\"siInBdy\" class=\"ssiSctSiBdy\">");
			writer.Write("<table id=\"tblSIBdy\">");
			writer.Write("<tr>");
			writer.Write("<td class=\"t\">");
			writer.Write(SanitizedHtmlString.FromStringId(-881075747));
			writer.Write("</td>");
			writer.Write("<td id=\"tdSubj\" nowrap></td>");
			writer.Write("</tr>");
			writer.Write("<tr>");
			writer.Write("<td class=\"t\">");
			writer.Write(SanitizedHtmlString.FromStringId(-1376223345));
			writer.Write("</td>");
			writer.Write("<td id=\"tdFrom\" nowrap></td>");
			writer.Write("</tr>");
			writer.Write("<tr>");
			writer.Write("<td class=\"t\">");
			writer.Write(SanitizedHtmlString.FromStringId(2124841137));
			writer.Write("</td>");
			writer.Write("<td id=\"tdSigBy\" nowrap></td>");
			writer.Write("</tr>");
			writer.Write("<tr>");
			writer.Write("<td class=\"t\">");
			writer.Write(SanitizedHtmlString.FromStringId(46763188));
			writer.Write("</td>");
			writer.Write("<td id=\"tdIssBy\" nowrap></td>");
			writer.Write("</tr>");
			writer.Write("</table>");
			writer.Write("</div>");
			writer.Write("<div id=\"siErrBdy\" class=\"ssiSctSiBdy\">");
			writer.Write(SanitizedHtmlString.FromStringId(1766818386));
			writer.Write("</div>");
			writer.Write("</div>");
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
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

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell || this.IsSMimeControlNeeded;
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

		protected bool IsDraftMessage
		{
			get
			{
				return this.isDraftMessage;
			}
		}

		protected bool IsInJunkMailFolder
		{
			get
			{
				return this.isInJunkEmailFolder;
			}
		}

		protected bool IsSuspectedPhishingItemWithoutLinkEnabled
		{
			get
			{
				return this.isSuspectedPhishingItemWithoutLinkEnabled;
			}
		}

		protected bool ShowBccInSentItems
		{
			get
			{
				return this.recipientWell.HasRecipients(RecipientWellType.Bcc);
			}
		}

		protected int CurrentStoreObjectType
		{
			get
			{
				if (this.Message != null && ObjectClass.IsOfClass(this.Message.ClassName, "IPM.Note.Microsoft.Approval.Request"))
				{
					return 26;
				}
				return 9;
			}
		}

		protected static int StoreObjectTypeApprovalRequest
		{
			get
			{
				return 26;
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

		protected bool IsClearSignedItem
		{
			get
			{
				return Utilities.IsClearSigned(this.Message);
			}
		}

		protected bool IsOpaqueSignedItem
		{
			get
			{
				return Utilities.IsOpaqueSigned(this.Message);
			}
		}

		protected bool IsSMimeItem
		{
			get
			{
				return Utilities.IsSMime(this.Message);
			}
		}

		protected bool IsEncryptedItem
		{
			get
			{
				return Utilities.IsEncrypted(this.Message);
			}
		}

		protected bool IsSMimeControlNeeded
		{
			get
			{
				return !JunkEmailUtilities.IsJunkOrPhishing(base.Item, base.IsEmbeddedItem, base.UserContext) && Utilities.IsClientSMimeControlUsable(base.ClientSMimeControlStatus) && this.IsSMimeItem && Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 16);
			}
		}

		protected SanitizedHtmlString WebBeaconBlockedInfobarText
		{
			get
			{
				return BodyConversionUtilities.GetWebBeaconBlockedInfobarMessage(base.UserContext.Configuration.FilterWebBeaconsAndHtmlForms);
			}
		}

		protected bool IsWebBeaconAllowed
		{
			get
			{
				return base.ForceAllowWebBeacon || (!this.IsPublicItem && Utilities.IsWebBeaconsAllowed(base.Item));
			}
		}

		protected bool ShouldRenderApprovalToolbar
		{
			get
			{
				return Utilities.IsValidUndecidedApprovalRequest(this.Message) && !this.IsOtherMailboxItem;
			}
		}

		protected override bool IsSubjectEditable
		{
			get
			{
				return !this.IsSMimeItem && (!base.UserContext.IsSmsEnabled || !ObjectClass.IsSmsMessage(this.Message.ClassName)) && base.IsSubjectEditable;
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
				return FlagContextMenu.GetFlagActionForItem(base.UserContext, this.Message);
			}
		}

		private void AddIrmMessageToInfobar()
		{
			if (base.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(this.Message))
			{
				InfobarMessageBuilder.AddIrmInformation(this.infobar, this.Message, base.IsPreviewForm, true, this.IrmItemHelper.IsRemoveAllowed, false);
			}
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				foreach (string s in this.externalScriptFiles)
				{
					yield return s;
				}
				if (base.UserContext.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
				{
					foreach (string s2 in this.externalScriptFilesForDiscoveryMailbox)
					{
						yield return s2;
					}
				}
				if (this.IsSMimeControlNeeded)
				{
					foreach (string s3 in this.externalScriptFilesForSMIME)
					{
						yield return s3;
					}
				}
				yield break;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				if (base.UserContext.IsSmsEnabled && ObjectClass.IsSmsMessage(this.Message.ClassName) && this.Message.From != null)
				{
					Participant from = this.Message.From;
					SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>(128);
					if (!string.IsNullOrEmpty(from.DisplayName))
					{
						sanitizingStringBuilder.Append(from.DisplayName);
					}
					if (Utilities.IsMobileRoutingType(from.RoutingType))
					{
						sanitizingStringBuilder.Append<char>(' ');
						sanitizingStringBuilder.Append(base.UserContext.DirectionMark);
						sanitizingStringBuilder.Append<char>('[');
						sanitizingStringBuilder.Append(from.EmailAddress);
						sanitizingStringBuilder.Append<char>(']');
						sanitizingStringBuilder.Append(base.UserContext.DirectionMark);
					}
					return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1856268034), new object[]
					{
						sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>()
					});
				}
				return RenderingUtilities.GetSubject(this.Message, LocalizedStrings.GetNonEncoded(730745110));
			}
		}

		public override string PageType
		{
			get
			{
				return "ReadMessagePage";
			}
		}

		public override string BodyCssClass
		{
			get
			{
				if (!base.IsPreviewForm)
				{
					return "rdFrmBody";
				}
				return string.Empty;
			}
		}

		public override string HtmlAdditionalAttributes
		{
			get
			{
				if (!this.IsSMimeControlNeeded)
				{
					return string.Empty;
				}
				return "xmlns:MIME";
			}
		}

		private MessageItem message;

		private MessageRecipientWell recipientWell;

		private Infobar infobar = new Infobar();

		private ArrayList attachmentWellRenderObjects;

		private ReadMessageToolbar toolbar;

		private ApprovalRequestToolbar approvalRequestToolbar;

		private bool shouldRenderAttachmentWell;

		private bool isFaxMessage;

		private bool isDraftMessage;

		private Markup bodyMarkup;

		private bool isInJunkEmailFolder;

		private bool isSuspectedPhishingItemWithoutLinkEnabled;

		private string[] externalScriptFiles = new string[]
		{
			"freadmsg.js"
		};

		private string[] externalScriptFilesForSMIME = new string[]
		{
			"cattach.js",
			"smallicons.aspx"
		};

		private string[] externalScriptFilesForDiscoveryMailbox = new string[]
		{
			"MessageAnnotationDialog.js"
		};
	}
}
