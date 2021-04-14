using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadPost : OwaForm, IRegistryOnlyForm
	{
		protected static int StoreObjectTypePost
		{
			get
			{
				return 22;
			}
		}

		protected static string SaveNamespace
		{
			get
			{
				return "ReadPost";
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.post = base.Initialize<PostItem>(ReadPost.prefetchProperties);
			if (!base.IsPreviewForm && !base.IsEmbeddedItem)
			{
				this.post.MarkAsRead(false);
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(this.post, base.IsEmbeddedItem, base.ForceEnableItemLink, base.UserContext, out this.isInJunkmailFolder, out flag, out flag2, out flag3);
			this.isSuspectedPhishingItemWithoutLinkEnabled = (flag && !flag2);
			this.toolbar = new ReadPostToolbar(base.IsEmbeddedItem, base.Item);
			InfobarMessageBuilder.AddImportance(this.infobar, this.post);
			InfobarMessageBuilder.AddSensitivity(this.infobar, this.post);
			InfobarMessageBuilder.AddFlag(this.infobar, this.post, base.UserContext);
			InfobarMessageBuilder.AddCompliance(base.UserContext, this.infobar, this.post, false);
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, this.post);
		}

		protected void RenderJavascriptEncodedMessageId()
		{
			string s = OwaStoreObjectId.CreateFromStoreObject(this.post).ToBase64String();
			Utilities.JavascriptEncode(s, base.Response.Output);
		}

		protected void RenderJavascriptEncodedMessageChangeKey()
		{
			Utilities.JavascriptEncode(this.post.Id.ChangeKeyAsBase64String(), base.Response.Output);
		}

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

		protected void RenderCategories()
		{
			CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, this.post);
		}

		protected void RenderConversation(TextWriter writer)
		{
			Utilities.HtmlEncode(this.post.ConversationTopic, writer);
		}

		protected void RenderPostedFolder(TextWriter writer)
		{
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, base.ParentFolderId, new PropertyDefinition[0]))
			{
				Utilities.HtmlEncode(folder.DisplayName, writer);
			}
		}

		protected ExDateTime PostedTime
		{
			get
			{
				return this.post.PostedTime;
			}
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected void RenderToolbar()
		{
			this.toolbar.Render(base.Response.Output);
		}

		protected void RenderSender(TextWriter writer)
		{
			RenderingUtilities.RenderSender(base.UserContext, writer, this.post);
		}

		protected bool HasSender
		{
			get
			{
				return this.post.Sender != null;
			}
		}

		protected void CreateAttachmentHelpers()
		{
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.post, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem, base.ForceEnableItemLink);
			this.shouldRenderAttachmentWell = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, base.ForceEnableItemLink, this.infobar, this.attachmentWellRenderObjects);
		}

		protected void LoadPostBodyIntoStream(TextWriter writer)
		{
			string action = base.IsPreviewForm ? "Preview" : string.Empty;
			string attachmentUrl = null;
			if (base.IsEmbeddedItemInNonSMimeItem)
			{
				attachmentUrl = base.RenderEmbeddedUrl();
			}
			base.AttachmentLinks = BodyConversionUtilities.GenerateNonEditableMessageBodyAndRenderInfobarMessages(this.post, writer, base.OwaContext, this.infobar, base.ForceAllowWebBeacon, base.ForceEnableItemLink, this.post.ClassName, action, string.Empty, base.IsEmbeddedItemInNonSMimeItem, attachmentUrl);
		}

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				RenderingUtilities.RenderSubject(base.Response.Output, this.post, LocalizedStrings.GetNonEncoded(-439597685));
				return;
			}
			RenderingUtilities.RenderSubject(base.Response.Output, this.post);
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(this.post, base.Response.Output, "DIV#divBdy");
		}

		protected bool IsSuspectedPhishingItemWithoutLinkEnabled
		{
			get
			{
				return this.isSuspectedPhishingItemWithoutLinkEnabled;
			}
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.Response.Output);
		}

		protected bool IsInJunkMailFolder
		{
			get
			{
				return this.isInJunkmailFolder;
			}
		}

		protected bool CanCreateItemInParentFolder
		{
			get
			{
				return Utilities.CanCreateItemInFolder(base.UserContext, base.ParentFolderId);
			}
		}

		protected FlagAction FlagAction
		{
			get
			{
				return FlagContextMenu.GetFlagActionForItem(base.UserContext, this.post);
			}
		}

		private static readonly StorePropertyDefinition[] prefetchProperties = new StorePropertyDefinition[]
		{
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid,
			ItemSchema.EdgePcl,
			StoreObjectSchema.EffectiveRights,
			ItemSchema.IsClassified,
			ItemSchema.LinkEnabled,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.Categories,
			ItemSchema.FlagCompleteTime,
			ItemSchema.FlagStatus,
			MessageItemSchema.ReplyTime
		};

		private PostItem post;

		private Infobar infobar = new Infobar();

		private ArrayList attachmentWellRenderObjects;

		private ReadPostToolbar toolbar;

		private bool shouldRenderAttachmentWell;

		private bool isSuspectedPhishingItemWithoutLinkEnabled;

		private bool isInJunkmailFolder;

		private Markup bodyMarkup;
	}
}
