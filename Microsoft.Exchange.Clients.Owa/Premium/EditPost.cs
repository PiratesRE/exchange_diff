using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditPost : EditMessageOrPostBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.post = base.Initialize<PostItem>(false, new PropertyDefinition[0]);
			Importance importance;
			if (this.post != null)
			{
				base.DeleteExistingDraft = true;
				this.newItemType = NewItemType.PostReply;
				importance = this.post.Importance;
			}
			else
			{
				base.DeleteExistingDraft = false;
				this.newItemType = NewItemType.New;
				importance = Importance.Normal;
			}
			this.bodyMarkup = BodyConversionUtilities.GetBodyFormatOfEditItem(base.Item, this.newItemType, base.UserContext.UserOptions);
			this.addSignatureToBody = base.ShouldAddSignatureToBody(this.bodyMarkup, this.newItemType);
			this.toolbar = new EditPostToolbar(importance, this.bodyMarkup);
		}

		protected static int StoreObjectTypePost
		{
			get
			{
				return 22;
			}
		}

		protected void RenderFolderDisplayName()
		{
			string text = null;
			if (string.IsNullOrEmpty(text))
			{
				using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, base.TargetFolderId, new PropertyDefinition[0]))
				{
					text = folder.DisplayName;
				}
			}
			Utilities.HtmlEncode(text, base.Response.Output);
		}

		protected void RenderTitle()
		{
			if (this.post == null)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-439597685));
				return;
			}
			string subject = this.post.Subject;
			if (string.IsNullOrEmpty(subject))
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-439597685));
				return;
			}
			Utilities.HtmlEncode(subject, base.Response.Output);
		}

		protected void CreateAttachmentHelpers()
		{
			if (this.post == null)
			{
				return;
			}
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.post, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
			InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
			if (infobarRenderingHelper.HasLevelOne)
			{
				this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
			}
		}

		protected Toolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected void RenderCategories()
		{
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderConversation()
		{
			if (this.post != null)
			{
				Utilities.CropAndRenderText(base.Response.Output, this.post.ConversationTopic, 255);
			}
		}

		protected void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, this.post);
		}

		private PostItem post;

		private EditPostToolbar toolbar;
	}
}
