using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public abstract class OwaForm : OwaPage, IRegistryOnlyForm
	{
		protected static void RemoveContactPhoto(ArrayList attachmentWellRenderObjects)
		{
			if (attachmentWellRenderObjects == null)
			{
				throw new ArgumentNullException("attachmentWellRenderObjects");
			}
			for (int i = 0; i < attachmentWellRenderObjects.Count; i++)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)attachmentWellRenderObjects[i];
				using (Attachment attachment = attachmentWellInfo.OpenAttachment())
				{
					attachment.Load(new PropertyDefinition[]
					{
						AttachmentSchema.IsContactPhoto
					});
					if (attachment.IsContactPhoto)
					{
						attachmentWellRenderObjects.RemoveAt(i);
						break;
					}
				}
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (Utilities.GetQueryStringParameter(base.Request, "atttyp", false) != null && Utilities.GetQueryStringParameter(base.Request, "atttyp", false) == "embdd")
			{
				this.isEmbeddedItem = true;
			}
			string text = null;
			string text2 = null;
			this.itemId = QueryStringUtilities.CreateItemStoreObjectId(base.UserContext.MailboxSession, base.Request, false);
			if (this.itemId == null)
			{
				if (Utilities.IsPostRequest(base.Request))
				{
					text = Utilities.GetFormParameter(base.Request, "hidid", false);
					text2 = Utilities.GetFormParameter(base.Request, "hidchk", false);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					this.itemStoreId = Utilities.CreateItemId(base.UserContext.MailboxSession, text, text2);
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.itemId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, text);
				}
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.parentItem != null)
			{
				this.parentItem.Dispose();
				this.parentItem = null;
			}
			if (this.item != null)
			{
				this.item.Dispose();
				this.item = null;
			}
		}

		protected virtual void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string attachmentUrl = null;
			if (this.IsEmbeddedItem)
			{
				attachmentUrl = AttachmentWell.RenderEmbeddedUrl(this.itemId.ToBase64String());
			}
			this.AttachmentLinks = BodyConversionUtilities.GenerateNonEditableMessageBodyAndRenderInfobarMessages(this.item, writer, base.OwaContext, this.infobar, this.IsRequestCallbackForWebBeacons, this.IsRequestCallbackForPhishing, this.ItemType, string.Empty, string.Empty, this.IsEmbeddedItem, attachmentUrl);
		}

		protected void CreateAttachmentHelpers(AttachmentWellType wellType)
		{
			if (JunkEmailUtilities.IsJunkOrPhishing(this.Item, this.IsEmbeddedItem, base.UserContext))
			{
				this.shouldRenderAttachmentWell = false;
				return;
			}
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.Item, this.AttachmentLinks, base.UserContext.IsPublicLogon, this.IsEmbeddedItem);
			if (this.attachmentWellRenderObjects != null && this.attachmentWellRenderObjects.Count > 0 && Utilities.IsClearSigned(this.Item))
			{
				AttachmentUtility.RemoveSmimeAttachment(this.attachmentWellRenderObjects);
			}
			if (this.Item is Contact)
			{
				OwaForm.RemoveContactPhoto(this.attachmentWellRenderObjects);
			}
			InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
			if (wellType == AttachmentWellType.ReadOnly && infobarRenderingHelper.HasLevelOneAndBlock)
			{
				this.infobar.AddMessageText(string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-824680214), new object[]
				{
					infobarRenderingHelper.FileNameStringForLevelOneAndBlock
				}), InfobarMessageType.Informational);
			}
			else if (wellType != AttachmentWellType.ReadOnly && infobarRenderingHelper.HasLevelOneAndBlock)
			{
				this.infobar.AddMessageLocalized(-2118248931, InfobarMessageType.Informational);
			}
			bool flag = AttachmentUtility.IsOutLine(this.attachmentWellRenderObjects);
			this.shouldRenderAttachmentWell = (wellType == AttachmentWellType.ReadWrite || (flag && wellType == AttachmentWellType.ReadOnly && (infobarRenderingHelper.HasLevelTwo || infobarRenderingHelper.HasLevelThree || infobarRenderingHelper.HasWebReadyFirst)));
		}

		protected void RenderEmbeddedItemIds()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "attcnt");
			int num;
			if (!int.TryParse(queryStringParameter, out num))
			{
				throw new OwaInvalidRequestException("Invalid attachment count querystring parameter");
			}
			base.Response.Write("new Array(\"");
			base.Response.Write(Utilities.UrlEncode(this.itemId.ToBase64String()));
			base.Response.Write("\"");
			for (int i = 0; i < num; i++)
			{
				string name = "attid" + i.ToString(CultureInfo.InvariantCulture);
				string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, name);
				base.Response.Write(",\"");
				base.Response.Write(Utilities.JavascriptEncode(Utilities.UrlEncode(queryStringParameter2)));
				base.Response.Write("\"");
			}
			base.Response.Write(")");
		}

		internal void HandleReadReceipt(MessageItem message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (Utilities.GetQueryStringParameter(base.Request, "sndrct", false) != null)
			{
				message.SendReadReceipt();
				this.Infobar.AddMessageLocalized(641302712, InfobarMessageType.Informational);
				return;
			}
			if (!this.IsEmbeddedItem)
			{
				InfobarMessageBuilder.AddSendReceiptNotice(base.UserContext, this.Infobar, message);
			}
		}

		protected virtual string ItemType
		{
			get
			{
				if (base.OwaContext.FormsRegistryContext.Type != null)
				{
					return base.OwaContext.FormsRegistryContext.Type;
				}
				return string.Empty;
			}
		}

		protected virtual string ApplicationElement
		{
			get
			{
				return Convert.ToString(base.OwaContext.FormsRegistryContext.ApplicationElement);
			}
		}

		protected virtual string Action
		{
			get
			{
				if (base.OwaContext.FormsRegistryContext.Action != null)
				{
					return base.OwaContext.FormsRegistryContext.Action;
				}
				return string.Empty;
			}
		}

		protected virtual string State
		{
			get
			{
				if (base.OwaContext.FormsRegistryContext.State != null)
				{
					return base.OwaContext.FormsRegistryContext.State;
				}
				return string.Empty;
			}
		}

		internal StoreObjectId ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		protected virtual bool ShowInfobar
		{
			get
			{
				return this.Infobar.MessageCount > 0;
			}
		}

		internal Item Item
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		internal Item ParentItem
		{
			get
			{
				return this.parentItem;
			}
		}

		internal IList<AttachmentLink> AttachmentLinks
		{
			get
			{
				return this.attachmentLinks;
			}
			set
			{
				this.attachmentLinks = value;
			}
		}

		protected bool IsText
		{
			get
			{
				return this.Item.Body.Format == BodyFormat.TextPlain;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
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

		protected bool IsInDeleteItems
		{
			get
			{
				return !this.isEmbeddedItem && this.Item != null && Utilities.IsItemInDefaultFolder(this.Item, DefaultFolderType.DeletedItems);
			}
		}

		protected bool IsEmbeddedItem
		{
			get
			{
				return this.isEmbeddedItem;
			}
		}

		protected AttachmentWell.AttachmentWellFlags AttachmentWellFlags
		{
			get
			{
				return this.attachmentWellFlags;
			}
		}

		protected bool IsRequestCallbackForPhishing
		{
			get
			{
				return Utilities.IsRequestCallbackForPhishing(base.Request);
			}
		}

		protected bool IsRequestCallbackForWebBeacons
		{
			get
			{
				return Utilities.IsRequestCallbackForWebBeacons(base.Request);
			}
		}

		internal T Initialize<T>(params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.Initialize<T>(true, false, prefetchProperties);
		}

		internal T Initialize<T>(bool itemRequired, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.Initialize<T>(itemRequired, false, prefetchProperties);
		}

		internal MessageItem InitializeAsMessageItem(params PropertyDefinition[] prefetchProperties)
		{
			return this.Initialize<MessageItem>(true, true, prefetchProperties);
		}

		private T Initialize<T>(bool itemRequired, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			object obj = null;
			object preFormActionId = base.OwaContext.PreFormActionId;
			if (preFormActionId != null)
			{
				OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)preFormActionId;
				obj = Utilities.GetItem<T>(base.UserContext, owaStoreObjectId, forceAsMessageItem, prefetchProperties);
			}
			else if (this.isEmbeddedItem)
			{
				obj = Utilities.GetItemForRequest<T>(base.OwaContext, out this.parentItem, forceAsMessageItem, prefetchProperties);
			}
			else if (this.itemStoreId != null)
			{
				obj = Utilities.GetItem<T>(base.UserContext, this.itemStoreId, forceAsMessageItem, prefetchProperties);
			}
			else if (this.itemId != null)
			{
				obj = Utilities.GetItem<T>(base.UserContext, this.itemId, forceAsMessageItem, prefetchProperties);
			}
			else if (itemRequired)
			{
				throw new OwaInvalidRequestException("Missing 'id' URL parameter or 'hidid' form parameter");
			}
			this.item = (obj as Item);
			if (this.isEmbeddedItem && this.parentItem != null)
			{
				this.itemId = this.parentItem.Id.ObjectId;
				this.attachmentWellFlags |= AttachmentWell.AttachmentWellFlags.RenderEmbeddedAttachment;
			}
			if (!this.isEmbeddedItem)
			{
				if (this.IsRequestCallbackForPhishing && this.item != null)
				{
					JunkEmailUtilities.SetLinkEnabled(this.item, prefetchProperties);
				}
				Utilities.SetWebBeaconPolicy(this.IsRequestCallbackForWebBeacons, this.item, prefetchProperties);
			}
			if (obj == null)
			{
				return default(T);
			}
			return (T)((object)obj);
		}

		protected virtual void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None);
			optionsBar.Render(helpFile);
		}

		private const string ItemIdFormParameter = "hidid";

		private const string ChangeKeyFormParameter = "hidchk";

		protected const string OutputDisplayCharset = "utf-8";

		private Item item;

		private IList<AttachmentLink> attachmentLinks;

		protected bool isEmbeddedItem;

		private Infobar infobar = new Infobar();

		private Item parentItem;

		private bool shouldRenderAttachmentWell;

		private ArrayList attachmentWellRenderObjects;

		private StoreObjectId itemId;

		private StoreId itemStoreId;

		private AttachmentWell.AttachmentWellFlags attachmentWellFlags = AttachmentWell.AttachmentWellFlags.RenderEmbeddedItem;
	}
}
