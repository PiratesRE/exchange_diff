using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class OwaForm : OwaPage
	{
		internal OwaForm()
		{
			this.owaFormInternal = new OwaFormInternal(base.OwaContext);
		}

		internal OwaForm(bool setNoCacheNoStore) : base(setNoCacheNoStore)
		{
			this.owaFormInternal = new OwaFormInternal(base.OwaContext);
		}

		internal T Initialize<T>(params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.owaFormInternal.Initialize<T>(true, false, prefetchProperties);
		}

		internal T Initialize<T>(bool itemRequired, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.owaFormInternal.Initialize<T>(itemRequired, false, prefetchProperties);
		}

		internal MessageItem InitializeAsMessageItem(params PropertyDefinition[] prefetchProperties)
		{
			return this.owaFormInternal.Initialize<MessageItem>(true, true, prefetchProperties);
		}

		protected void MarkPayloadAsRead()
		{
			this.owaFormInternal.MarkPayloadAsRead();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.owaFormInternal.MarkPayloadAsRead();
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.owaFormInternal != null)
			{
				this.owaFormInternal.Dispose();
				this.owaFormInternal = null;
			}
		}

		protected string RenderEmbeddedUrl()
		{
			return this.owaFormInternal.RenderEmbeddedUrl();
		}

		protected void RenderEmbeddedItemIds()
		{
			this.owaFormInternal.RenderEmbeddedItemIds();
		}

		protected void RenderJavascriptEncodedItemChangeKey()
		{
			this.owaFormInternal.RenderJavascriptEncodedItemChangeKey();
		}

		protected void RenderJavascriptEncodedItemId()
		{
			this.owaFormInternal.RenderJavascriptEncodedItemId();
		}

		protected void RenderMessageInformation(TextWriter writer)
		{
			this.owaFormInternal.RenderMessageInformation(writer);
		}

		protected void RenderSubjectAttributes()
		{
			this.owaFormInternal.RenderSubjectAttributes();
		}

		protected bool ShowAttachmentWell
		{
			get
			{
				return this.owaFormInternal.ShowAttachmentWell;
			}
		}

		internal Item Item
		{
			get
			{
				return this.owaFormInternal.Item;
			}
			set
			{
				this.owaFormInternal.Item = value;
			}
		}

		internal IList<AttachmentLink> AttachmentLinks
		{
			get
			{
				return this.owaFormInternal.AttachmentLinks;
			}
			set
			{
				this.owaFormInternal.AttachmentLinks = value;
			}
		}

		protected bool IsPreviewForm
		{
			get
			{
				return this.owaFormInternal.IsPreviewForm;
			}
			set
			{
				this.owaFormInternal.IsPreviewForm = value;
			}
		}

		protected bool HasCategories
		{
			get
			{
				return this.owaFormInternal.HasCategories;
			}
		}

		protected bool IsEmbeddedItem
		{
			get
			{
				return this.owaFormInternal.IsEmbeddedItem;
			}
		}

		protected virtual bool IsPublicItem
		{
			get
			{
				return this.owaFormInternal.IsPublicItem;
			}
		}

		protected virtual bool IsOtherMailboxItem
		{
			get
			{
				return this.owaFormInternal.IsOtherMailboxItem;
			}
		}

		protected bool IsItemNull
		{
			get
			{
				return this.owaFormInternal.IsItemNull;
			}
		}

		protected virtual bool IsSubjectEditable
		{
			get
			{
				return this.owaFormInternal.IsSubjectEditable;
			}
		}

		protected virtual bool IsItemEditable
		{
			get
			{
				return this.owaFormInternal.IsItemEditable;
			}
		}

		protected bool IsInDeleteItems
		{
			get
			{
				return this.owaFormInternal.IsInDeleteItems;
			}
		}

		protected bool UserCanDeleteItem
		{
			get
			{
				return this.owaFormInternal.UserCanDeleteItem;
			}
		}

		protected bool UserCanEditItem
		{
			get
			{
				return this.owaFormInternal.UserCanEditItem;
			}
		}

		protected string ParentItemIdBase64String
		{
			get
			{
				return this.owaFormInternal.ParentFolderIdBase64String;
			}
		}

		protected string ParentFolderIdBase64String
		{
			get
			{
				return this.owaFormInternal.ParentFolderIdBase64String;
			}
		}

		internal OwaStoreObjectId ParentFolderId
		{
			get
			{
				return this.owaFormInternal.ParentFolderId;
			}
		}

		protected string ItemClassName
		{
			get
			{
				return this.owaFormInternal.ItemClassName;
			}
		}

		protected ClientSMimeControlStatus ClientSMimeControlStatus
		{
			get
			{
				return this.owaFormInternal.ClientSMimeControlStatus;
			}
		}

		protected bool ForceAllowWebBeacon
		{
			get
			{
				return this.IsRequestCallbackForWebBeacons;
			}
		}

		protected bool IsRequestCallbackForWebBeacons
		{
			get
			{
				return Utilities.IsRequestCallbackForWebBeacons(base.Request);
			}
		}

		protected bool ForceEnableItemLink
		{
			get
			{
				return this.IsRequestCallbackForPhishing;
			}
		}

		protected bool IsRequestCallbackForPhishing
		{
			get
			{
				return Utilities.IsRequestCallbackForPhishing(base.Request);
			}
		}

		protected bool IsJunkOrPhishing
		{
			get
			{
				return this.owaFormInternal.IsJunkOrPhishing;
			}
		}

		protected bool IsReportItem
		{
			get
			{
				return this.owaFormInternal.IsReportItem;
			}
		}

		protected bool IsEmbeddedItemInSMimeMessage
		{
			get
			{
				return this.owaFormInternal.IsEmbeddedItemInSMimeMessage;
			}
		}

		protected bool IsEmbeddedItemInNonSMimeItem
		{
			get
			{
				return this.owaFormInternal.IsEmbeddedItemInNonSMimeItem;
			}
		}

		protected virtual bool IsIgnoredConversation
		{
			get
			{
				return this.owaFormInternal.IsIgnoredConversation;
			}
		}

		protected bool ShouldRenderDownloadAllLink
		{
			get
			{
				return this.owaFormInternal.ShouldRenderDownloadAllLink;
			}
		}

		protected void SetShouldRenderDownloadAllLink(ArrayList attachmentWellInfos)
		{
			this.owaFormInternal.SetShouldRenderDownloadAllLink(attachmentWellInfos);
		}

		protected void RenderDownloadAllAttachmentsLink()
		{
			this.owaFormInternal.RenderDownloadAllAttachmentsLink();
		}

		protected void RenderAttachmentWellForReadItem(ArrayList attachmentWellRenderObjects)
		{
			this.owaFormInternal.RenderAttachmentWellForReadItem(attachmentWellRenderObjects);
		}

		protected void RenderActionButtons(bool isInJunkMailFolder, bool isPost)
		{
			this.owaFormInternal.RenderActionButtons(isInJunkMailFolder, isPost);
		}

		private OwaFormInternal owaFormInternal;
	}
}
