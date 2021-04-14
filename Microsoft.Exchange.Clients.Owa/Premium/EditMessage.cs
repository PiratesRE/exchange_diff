using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditMessage : EditMessageOrPostBase
	{
		public EditMessage() : base(false)
		{
		}

		internal EditMessage(bool setNoCacheNoStore) : base(setNoCacheNoStore)
		{
		}

		protected virtual bool IsPageCacheable
		{
			get
			{
				return this.IsNewMessage && !this.IsSMimeControlNeeded;
			}
		}

		protected virtual void InitializeMessage()
		{
			this.message = base.Initialize<MessageItem>(false, EditMessage.PrefetchProperties);
			if (this.message != null && base.UserContext.IsIrmEnabled)
			{
				Utilities.IrmDecryptIfRestricted(this.message, base.UserContext, true);
			}
		}

		protected virtual void CreateDraftMessage()
		{
			this.message = Utilities.CreateDraftMessageFromQueryString(base.UserContext, base.Request);
			if (this.message != null)
			{
				this.newItemType = NewItemType.ImplicitDraft;
				base.DeleteExistingDraft = true;
				base.Item = this.message;
			}
		}

		protected virtual bool PageSupportSmime
		{
			get
			{
				return true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.IsPageCacheable)
			{
				Utilities.MakePageCacheable(base.Response);
			}
			else
			{
				Utilities.MakePageNoCacheNoStore(base.Response);
			}
			this.InitializeMessage();
			if (this.PageSupportSmime && this.message != null && Utilities.IsSMime(this.message))
			{
				if (!Utilities.IsSMimeFeatureUsable(base.OwaContext))
				{
					throw new OwaNeedsSMimeControlToEditDraftException(LocalizedStrings.GetNonEncoded(-1507367759));
				}
				if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 1))
				{
					throw new OwaNeedsSMimeControlToEditDraftException(LocalizedStrings.GetNonEncoded(-872682934));
				}
				if (Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 2))
				{
					throw new OwaNeedsSMimeControlToEditDraftException(LocalizedStrings.GetNonEncoded(-1103993212));
				}
				if (!Utilities.CheckSMimeEditFormBasicRequirement(base.ClientSMimeControlStatus, base.OwaContext))
				{
					throw new OwaNeedsSMimeControlToEditDraftException(LocalizedStrings.GetNonEncoded(-1507367759));
				}
			}
			if (!base.UserContext.IsIrmEnabled && this.message != null && Utilities.IsIrmRestricted(this.message))
			{
				SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1049269714), new object[]
				{
					Utilities.GetOfficeDownloadAnchor(Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain, base.UserContext.UserCulture)
				});
				throw new OwaCannotEditIrmDraftException(sanitizedHtmlString.ToString());
			}
			if (this.message != null)
			{
				string action = base.OwaContext.FormsRegistryContext.Action;
				string state = base.OwaContext.FormsRegistryContext.State;
				if (string.CompareOrdinal(action, "Reply") == 0 || string.CompareOrdinal(action, "ReplyAll") == 0)
				{
					this.newItemType = NewItemType.Reply;
				}
				else if (string.CompareOrdinal(action, "Forward") == 0)
				{
					this.newItemType = NewItemType.Forward;
				}
				else if (string.Equals(action, "Open", StringComparison.OrdinalIgnoreCase) && string.Equals(state, "Draft", StringComparison.OrdinalIgnoreCase))
				{
					this.newItemType = NewItemType.ExplicitDraft;
					if (this.message.GetValueOrDefault<bool>(MessageItemSchema.HasBeenSubmitted))
					{
						this.message.AbortSubmit();
					}
				}
				else
				{
					this.newItemType = NewItemType.ImplicitDraft;
					base.DeleteExistingDraft = true;
				}
			}
			else
			{
				this.CreateDraftMessage();
			}
			if (this.message != null && Utilities.IsPublic(this.message))
			{
				throw new OwaInvalidRequestException("No way to open a public message in edit form");
			}
			if (this.IsSMimeControlNeeded && this.PageSupportSmime)
			{
				this.bodyMarkup = Markup.Html;
			}
			else
			{
				this.bodyMarkup = BodyConversionUtilities.GetBodyFormatOfEditItem(base.Item, this.newItemType, base.UserContext.UserOptions);
			}
			this.SetIsOtherFolder();
			this.infobar.SetInfobarClass("infobarEdit");
			this.infobar.SetShouldHonorHideByDefault(true);
			if (this.newItemType != NewItemType.New)
			{
				if (this.newItemType == NewItemType.ExplicitDraft)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-1981719796), InfobarMessageType.Informational);
				}
				InfobarMessageBuilder.AddSensitivity(this.infobar, this.message);
				if (this.newItemType != NewItemType.ImplicitDraft)
				{
					InfobarMessageBuilder.AddCompliance(base.UserContext, this.infobar, this.message, true);
				}
				if (base.UserContext.IsIrmEnabled && this.message != null)
				{
					InfobarMessageBuilder.AddIrmInformation(this.infobar, this.message, false, true, false, this.IsIrmAsAttachment);
				}
				this.recipientWell = new MessageRecipientWell(this.message);
				this.showBcc = (this.recipientWell.HasRecipients(RecipientWellType.Bcc) || base.UserContext.UserOptions.AlwaysShowBcc);
				this.showFrom = (this.recipientWell.HasRecipients(RecipientWellType.From) || base.UserContext.UserOptions.AlwaysShowFrom || this.isOtherFolder);
				this.toolbar = this.BuildToolbar();
				this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
				this.addSignatureToBody = base.ShouldAddSignatureToBody(this.bodyMarkup, this.newItemType);
			}
			else
			{
				this.recipientWell = new MessageRecipientWell();
				this.showBcc = base.UserContext.UserOptions.AlwaysShowBcc;
				this.showFrom = (base.UserContext.UserOptions.AlwaysShowFrom || this.isOtherFolder);
				this.toolbar = new EditMessageToolbar(Importance.Normal, this.bodyMarkup, this.IsSMimeControlMustUpdate, this.IsSMimeControlNeeded, false, false);
				this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
			}
			if (base.OwaContext.UserContext.IsFeatureEnabled(Feature.SMime) && this.PageSupportSmime)
			{
				if (this.IsSMimeControlNeeded && Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 4))
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(330022834), InfobarMessageType.Informational);
				}
				else if (this.IsSMimeControlMustUpdate)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(1697878138), InfobarMessageType.Informational);
				}
				if (Utilities.IsSMimeControlNeededForEditForm(base.ClientSMimeControlStatus, base.OwaContext) && this.ShowFrom)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-1863471683), InfobarMessageType.Informational);
				}
				if (!Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 1) && !Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 16) && !base.OwaContext.UserContext.IsExplicitLogonOthersMailbox)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-1908761042), InfobarMessageType.Warning);
				}
			}
			if (this.Message != null && this.IsRemoveRestricted)
			{
				this.toolbar.IsComplianceButtonEnabledInForm = false;
			}
			if (this.ShowFrom && this.IsFromWellRestricted)
			{
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(885106754), InfobarMessageType.Informational);
			}
			if (this.message != null && this.newItemType == NewItemType.ExplicitDraft && Utilities.IsInArchiveMailbox(base.Item))
			{
				this.toolbar.IsSendButtonEnabledInForm = false;
			}
		}

		protected void CreateAttachmentHelpers()
		{
			if (this.message != null)
			{
				this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.message, base.AttachmentLinks, base.UserContext.IsPublicLogon);
				InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
				if (infobarRenderingHelper.HasLevelOne)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
				}
			}
		}

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				RenderingUtilities.RenderSubject(base.SanitizingResponse, this.message, LocalizedStrings.GetNonEncoded(730745110));
				return;
			}
			RenderingUtilities.RenderSubject(base.SanitizingResponse, this.message);
		}

		protected void RenderJavascriptEncodedMessageChangeKey()
		{
			Utilities.JavascriptEncode(this.message.Id.ChangeKeyAsBase64String(), base.SanitizingResponse);
		}

		protected void RenderJavascriptEncodedSourceItemId()
		{
			Utilities.JavascriptEncode(this.SourceItemIdQueryString, base.SanitizingResponse);
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected virtual bool ShowBcc
		{
			get
			{
				return this.showBcc;
			}
		}

		protected virtual bool ShowFrom
		{
			get
			{
				return this.showFrom;
			}
		}

		protected MessageRecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
			set
			{
				this.recipientWell = value;
			}
		}

		protected void RenderSMimeFromRecipientWell()
		{
			RecipientWell recipientWell = new EditMessage.SMimeFromRecipientWell(base.UserContext);
			recipientWell.Render(base.SanitizingResponse, base.UserContext, RecipientWellType.To, Microsoft.Exchange.Clients.Owa.Premium.Controls.RecipientWell.RenderFlags.Hidden, "SMimeFrom");
		}

		protected EditMessageToolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
			set
			{
				this.toolbar = value;
			}
		}

		protected int MessageSensitivity
		{
			get
			{
				if (this.message == null)
				{
					return 0;
				}
				return (int)this.message.Sensitivity;
			}
		}

		protected bool IsReadReceiptRequested
		{
			get
			{
				return this.message != null && this.message.IsReadReceiptRequested;
			}
		}

		protected bool IsDeliveryReceiptRequested
		{
			get
			{
				return this.message != null && this.message.IsDeliveryReceiptRequested;
			}
		}

		protected string SourceItemIdQueryString
		{
			get
			{
				return Utilities.GetQueryStringParameter(base.Request, "srcId", false) ?? string.Empty;
			}
		}

		protected bool IsReplyForward
		{
			get
			{
				return base.NewItemType == NewItemType.Reply || base.NewItemType == NewItemType.Forward;
			}
		}

		protected bool IsOtherFolder
		{
			get
			{
				return this.isOtherFolder;
			}
		}

		protected bool IsSMimeControlNeeded
		{
			get
			{
				return Utilities.IsSMimeControlNeededForEditForm(base.ClientSMimeControlStatus, base.OwaContext) && !this.ShowFrom && !this.IsOtherFolder && (this.message == null || !Utilities.IsIrmRestricted(this.message));
			}
		}

		protected bool IsSMimeControlMustUpdate
		{
			get
			{
				return Utilities.CheckSMimeEditFormBasicRequirement(base.ClientSMimeControlStatus, base.OwaContext) && Utilities.IsFlagSet((int)base.ClientSMimeControlStatus, 2);
			}
		}

		protected bool ForceFilterWebBeacons
		{
			get
			{
				return base.UserContext.Configuration.FilterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.ForceFilter;
			}
		}

		protected void LoadComposeBody()
		{
			if (this.PageSupportSmime && this.IsSMimeControlNeeded && base.Item != null && base.NewItemType != NewItemType.ImplicitDraft && Utilities.GetQueryStringParameter(base.Request, "srcId", false) == null)
			{
				return;
			}
			base.LoadMessageBodyIntoStream(base.SanitizingResponse);
			if (this.message != null && base.UserContext.IsIrmEnabled)
			{
				Utilities.IrmDecryptIfRestricted(this.message, base.UserContext, true);
			}
		}

		protected void RenderSMimeSavingSendingWarnings(TextWriter writer)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-469017928));
			if (!OwaRegistryKeys.AlwaysEncrypt)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(302877857));
			}
			sanitizingStringBuilder.Append("</div><br><div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1922803157));
			sanitizingStringBuilder.Append("</div>");
			RenderingUtilities.RenderStringVariable(writer, "L_NECSv", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
			sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1408049888));
			if (!OwaRegistryKeys.AlwaysEncrypt)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1751385189));
			}
			sanitizingStringBuilder.Append("</div><br><div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1922803157));
			sanitizingStringBuilder.Append("</div>");
			RenderingUtilities.RenderStringVariable(writer, "L_NECSnd", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
			sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1971020450));
			if (!OwaRegistryKeys.AlwaysSign)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(721900430));
			}
			sanitizingStringBuilder.Append("</div><br><div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(OwaRegistryKeys.AllowUserChoiceOfSigningCertificate ? 2099415568 : 1922347219));
			sanitizingStringBuilder.Append("</div>");
			RenderingUtilities.RenderStringVariable(writer, "L_NSCSv", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
			sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-2072041142));
			if (!OwaRegistryKeys.AlwaysSign)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1301978547));
			}
			sanitizingStringBuilder.Append("</div><br><div>");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(OwaRegistryKeys.AllowUserChoiceOfSigningCertificate ? 2099415568 : 1922347219));
			sanitizingStringBuilder.Append("</div>");
			RenderingUtilities.RenderStringVariable(writer, "L_NSCSnd", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
			if (base.UserContext.UserOptions.UseManuallyPickedSigningCertificate)
			{
				sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append("<div>");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1737209722));
				if (!OwaRegistryKeys.AlwaysSign)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1301978547));
				}
				sanitizingStringBuilder.Append("</div><br><div>");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(2099415568));
				sanitizingStringBuilder.Append("</div>");
				RenderingUtilities.RenderStringVariable(writer, "L_ISCSnd", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
				sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append("<div>");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1737209722));
				if (!OwaRegistryKeys.AlwaysSign)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(721900430));
				}
				sanitizingStringBuilder.Append("</div><br><div>");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(2099415568));
				sanitizingStringBuilder.Append("</div>");
				RenderingUtilities.RenderStringVariable(writer, "L_ISCSv", sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
			}
		}

		protected void RenderDefaultMaximumAttachmentSize(TextWriter writer)
		{
			RenderingUtilities.RenderInteger(writer, "a_iDMAS", 5);
		}

		protected bool IsNewMessage
		{
			get
			{
				return base.OwaContext.FormsRegistryContext.Action.Equals("New", StringComparison.OrdinalIgnoreCase) && base.OwaContext.FormsRegistryContext.Type.Equals("IPM.Note", StringComparison.OrdinalIgnoreCase) && Utilities.GetQueryStringParameter(base.Request, "cc", false) != null;
			}
		}

		protected virtual int CurrentStoreObjectType
		{
			get
			{
				if (this.message != null && (ObjectClass.IsOfClass(this.message.ClassName, "IPM.Note.Microsoft.Approval.Reply.Approve") || ObjectClass.IsOfClass(this.message.ClassName, "IPM.Note.Microsoft.Approval.Reply.Reject")))
				{
					return 27;
				}
				return 9;
			}
		}

		protected void SetIsOtherFolder()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "fOMF", false);
			this.isOtherFolder = (string.CompareOrdinal(queryStringParameter, "1") == 0);
		}

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

		protected bool IsIrmProtected
		{
			get
			{
				return base.UserContext.IsIrmEnabled && this.message != null && Utilities.IsIrmRestricted(this.message);
			}
		}

		protected bool IsIrmAsAttachment
		{
			get
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "fIrmAsAttach", false);
				return string.CompareOrdinal("1", queryStringParameter) == 0;
			}
		}

		protected bool IsRecipientWellRestricted
		{
			get
			{
				if (this.message == null)
				{
					return false;
				}
				if (!base.OwaContext.UserContext.IsIrmEnabled)
				{
					return false;
				}
				if (!Utilities.IsIrmRestrictedAndDecrypted(this.message))
				{
					return false;
				}
				RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)this.message;
				return !rightsManagedMessageItem.UsageRights.IsUsageRightGranted(ContentRight.Forward) || (rightsManagedMessageItem.Restriction.RequiresRepublishingWhenRecipientsChange && !rightsManagedMessageItem.CanRepublish);
			}
		}

		protected bool IsFromWellRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Owner);
			}
		}

		protected bool IsPrintRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Print);
			}
		}

		protected bool IsCopyRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Extract);
			}
		}

		protected bool IsRemoveRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Export);
			}
		}

		protected bool IsNotOwner
		{
			get
			{
				return this.IsIrmProtected && this.IsUsageRightRestricted(ContentRight.Owner);
			}
		}

		protected virtual EditMessageToolbar BuildToolbar()
		{
			return new EditMessageToolbar(this.message.Importance, this.bodyMarkup, this.IsSMimeControlMustUpdate && this.PageSupportSmime, this.IsSMimeControlNeeded && this.PageSupportSmime, this.IsIrmProtected, this.IsNotOwner);
		}

		private bool IsUsageRightRestricted(ContentRight right)
		{
			return this.message != null && base.OwaContext.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(this.message) && !((RightsManagedMessageItem)this.message).UsageRights.IsUsageRightGranted(right);
		}

		internal static readonly StorePropertyDefinition[] PrefetchProperties = new StorePropertyDefinition[]
		{
			ItemSchema.BlockStatus,
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid,
			ItemSchema.EdgePcl,
			ItemSchema.LinkEnabled,
			MessageItemSchema.IsDeliveryReceiptRequested,
			MessageItemSchema.HasBeenSubmitted
		};

		private MessageItem message;

		private MessageRecipientWell recipientWell;

		private EditMessageToolbar toolbar;

		private bool showBcc;

		private bool showFrom;

		private bool isOtherFolder;

		private class SMimeFromRecipientWell : ItemRecipientWell
		{
			internal SMimeFromRecipientWell(UserContext userContext)
			{
				this.userContext = userContext;
			}

			internal override IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type)
			{
				yield return new Participant(this.userContext.MailboxIdentity.GetOWAMiniRecipient());
				yield break;
			}

			private UserContext userContext;
		}
	}
}
