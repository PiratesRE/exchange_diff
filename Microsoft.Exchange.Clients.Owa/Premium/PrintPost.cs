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
	public class PrintPost : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string type = base.OwaContext.FormsRegistryContext.Type;
			if (ObjectClass.IsPost(type))
			{
				this.post = base.Initialize<PostItem>(PrintPost.prefetchProperties);
			}
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.post, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
			this.shouldRenderAttachmentWell = PrintAttachmentWell.ShouldRenderAttachments(this.attachmentWellRenderObjects);
			if (this.post.Importance == Importance.High)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(-77932258);
			}
			else if (this.post.Importance == Importance.Low)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(1502599728);
			}
			switch (this.post.Sensitivity)
			{
			case Sensitivity.Personal:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(567923294);
				break;
			case Sensitivity.Private:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(-1268489823);
				break;
			case Sensitivity.CompanyConfidential:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(-819101664);
				break;
			}
			this.categoriesString = ItemUtility.GetCategoriesAsString(this.post);
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			BodyConversionUtilities.GeneratePrintMessageBody(this.post, writer, base.OwaContext, base.IsEmbeddedItem, base.IsEmbeddedItem ? base.RenderEmbeddedUrl() : null, base.ForceAllowWebBeacon, base.ForceEnableItemLink);
		}

		protected void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Utilities.IsOnBehalfOf(this.post.Sender, this.post.From))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetDisplaySenderName(this.post.Sender), RenderingUtilities.GetDisplaySenderName(this.post.From));
				return;
			}
			writer.Write(RenderingUtilities.GetDisplaySenderName(this.post.Sender));
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

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected string ImportanceString
		{
			get
			{
				return this.importanceString;
			}
		}

		protected string SensitivityString
		{
			get
			{
				return this.sensitivityString;
			}
		}

		protected string CategoriesString
		{
			get
			{
				return this.categoriesString;
			}
		}

		protected ExDateTime MessageSentTime
		{
			get
			{
				return this.post.PostedTime;
			}
		}

		protected void RenderJavascriptEncodedFolderId()
		{
			if (this.folderId != null)
			{
				Utilities.JavascriptEncode(this.folderId.ToBase64String(), base.Response.Output);
			}
		}

		protected void RenderPostedFolder(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, base.ParentFolderId, new PropertyDefinition[0]))
			{
				string displayName = folder.DisplayName;
				Utilities.HtmlEncode(displayName, writer);
				this.folderId = base.ParentFolderId;
			}
		}

		protected void RenderConversation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			Utilities.HtmlEncode(this.post.ConversationTopic, writer);
		}

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.BlockStatus,
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime
		};

		private PostItem post;

		private OwaStoreObjectId folderId;

		private bool shouldRenderAttachmentWell;

		private ArrayList attachmentWellRenderObjects;

		private string sensitivityString;

		private string importanceString;

		private string categoriesString;
	}
}
