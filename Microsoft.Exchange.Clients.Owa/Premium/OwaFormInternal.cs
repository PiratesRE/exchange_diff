using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class OwaFormInternal : DisposeTrackableBase
	{
		private OwaContext OwaContext { get; set; }

		private UserContext UserContext
		{
			get
			{
				return this.OwaContext.UserContext;
			}
		}

		private HttpRequest Request { get; set; }

		internal OwaFormInternal(OwaContext owaContext)
		{
			this.responseWriter = owaContext.HttpContext.Response.Output;
			this.sanitizingResponseWriter = owaContext.SanitizingResponseWriter;
			this.OwaContext = owaContext;
			this.Request = this.OwaContext.HttpContext.Request;
		}

		internal T Initialize<T>(bool itemRequired, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.Initialize<T>(itemRequired, forceAsMessageItem, Utilities.GetQueryStringParameter(this.Request, "id", false), this.OwaContext.FormsRegistryContext.Action, prefetchProperties);
		}

		internal T Initialize<T>(bool itemRequired, bool forceAsMessageItem, string id, string action, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			object obj = null;
			object preFormActionId = this.OwaContext.PreFormActionId;
			if (preFormActionId != null)
			{
				OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)preFormActionId;
				obj = Utilities.GetItem<T>(this.UserContext, owaStoreObjectId, forceAsMessageItem, prefetchProperties);
			}
			else if (!string.IsNullOrEmpty(id))
			{
				obj = Utilities.GetItemById<T>(this.OwaContext, out this.parentItem, id, forceAsMessageItem, prefetchProperties);
			}
			else if (itemRequired)
			{
				throw new OwaInvalidRequestException("Missing 'id' URL parameter");
			}
			this.item = (obj as Item);
			this.isEmbeddedItemInNonSMimeItem = (Utilities.GetQueryStringParameter(this.Request, "attcnt", false) != null);
			this.isEmbeddedItemInSMimeMessage = (Utilities.GetQueryStringParameter(this.Request, "smemb", false) != null);
			this.isPreviewForm = (action != null && action.Equals("Preview"));
			if (this.item != null && !this.IsEmbeddedItem && !this.IsPublicItem)
			{
				if (this.IsRequestCallbackForPhishing)
				{
					JunkEmailUtilities.SetLinkEnabled(this.item, prefetchProperties);
				}
				Utilities.SetWebBeaconPolicy(this.IsRequestCallbackForWebBeacons, this.item, prefetchProperties);
				if (this.IsRequestCallbackForRemoveIRM)
				{
					Utilities.IrmRemoveRestriction(this.item, this.UserContext);
				}
			}
			if (obj == null)
			{
				return default(T);
			}
			return (T)((object)obj);
		}

		internal void MarkPayloadAsRead()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(this.Request, "mrd", false);
			if (queryStringParameter != null)
			{
				JunkEmailStatus junkEmailStatus = JunkEmailStatus.Unknown;
				string queryStringParameter2 = Utilities.GetQueryStringParameter(this.Request, "JS", false);
				int num;
				if (queryStringParameter2 != null && int.TryParse(queryStringParameter2, out num) && (num == 1 || num == 0))
				{
					junkEmailStatus = (JunkEmailStatus)num;
				}
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter);
				OwaStoreObjectId[] localItemIds = ConversationUtilities.GetLocalItemIds(this.UserContext, new OwaStoreObjectId[]
				{
					owaStoreObjectId
				}, null, new PropertyDefinition[]
				{
					MessageItemSchema.IsRead
				}, (IStorePropertyBag propertyBag) => !ItemUtility.GetProperty<bool>(propertyBag, MessageItemSchema.IsRead, true));
				if (localItemIds.Length > 0)
				{
					Utilities.MarkItemsAsRead(this.UserContext, localItemIds, junkEmailStatus, false);
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
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
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaFormInternal>(this);
		}

		internal string RenderEmbeddedUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("attachment.ashx?attcnt=");
			stringBuilder.Append(this.EmbeddedItemNestingLevel + 1);
			stringBuilder.Append("&id=");
			stringBuilder.Append(Utilities.UrlEncode(this.ParentItemIdBase64String));
			for (int i = 0; i < this.EmbeddedItemNestingLevel; i++)
			{
				string text = "attid" + i.ToString(CultureInfo.InvariantCulture);
				string queryStringParameter = Utilities.GetQueryStringParameter(this.Request, text);
				stringBuilder.Append("&");
				stringBuilder.Append(text);
				stringBuilder.Append("=");
				stringBuilder.Append(Utilities.UrlEncode(queryStringParameter));
			}
			stringBuilder.Append("&attid" + this.EmbeddedItemNestingLevel.ToString(CultureInfo.InvariantCulture) + "=");
			return stringBuilder.ToString();
		}

		internal void RenderEmbeddedItemIds()
		{
			this.responseWriter.Write("new Array(\"");
			Utilities.JavascriptEncode(this.ParentItemIdBase64String, this.responseWriter);
			this.responseWriter.Write("\"");
			for (int i = 0; i < this.EmbeddedItemNestingLevel; i++)
			{
				string name = "attid" + i.ToString(CultureInfo.InvariantCulture);
				string queryStringParameter = Utilities.GetQueryStringParameter(this.Request, name);
				this.responseWriter.Write(",\"");
				Utilities.JavascriptEncode(queryStringParameter, this.responseWriter);
				this.responseWriter.Write("\"");
			}
			this.responseWriter.Write(")");
		}

		internal void RenderJavascriptEncodedItemChangeKey()
		{
			Utilities.JavascriptEncode(this.Item.Id.ChangeKeyAsBase64String(), this.sanitizingResponseWriter);
		}

		internal void RenderJavascriptEncodedItemId()
		{
			if (this.Item != null)
			{
				Utilities.JavascriptEncode(Utilities.GetIdAsString(this.Item), this.sanitizingResponseWriter);
			}
		}

		internal void RenderMessageInformation(TextWriter writer)
		{
			writer.WriteLine("var a_fEnIL = {0};", this.IsRequestCallbackForPhishing ? 1 : 0);
			writer.WriteLine("var a_fJoP = {0};", (this.Item == null) ? 0 : (this.IsJunkOrPhishing ? 1 : 0));
			writer.WriteLine("var a_fEmb = {0};", this.IsEmbeddedItem ? 1 : 0);
			writer.WriteLine("var a_fRp = {0};", (this.Item == null) ? 0 : (this.IsReportItem ? 1 : 0));
			writer.WriteLine("var a_iEmbD = {0};", Utilities.GetEmbeddedDepth(HttpContext.Current.Request));
		}

		internal void RenderSubjectAttributes()
		{
			this.sanitizingResponseWriter.Write(" _fAllwCM=\"1\"");
			if (this.IsSubjectEditable)
			{
				this.sanitizingResponseWriter.Write(" TABINDEX=0 _editable=1");
			}
			if (this.UserContext.IsSmsEnabled && ObjectClass.IsSmsMessage(this.ItemClassName))
			{
				this.sanitizingResponseWriter.Write(" style=\"display:none\"");
			}
		}

		internal void RenderDownloadAllAttachmentsLink()
		{
			if (this.shouldRenderDownloadAllLink)
			{
				string urlEncodedItemId;
				if (this.IsEmbeddedItemInNonSMimeItem)
				{
					urlEncodedItemId = Utilities.UrlEncode(this.ParentItemIdBase64String);
				}
				else
				{
					urlEncodedItemId = Utilities.UrlEncode(Utilities.GetIdAsString(this.item));
				}
				AttachmentUtility.RenderDownloadAllAttachmentsLink((SanitizingTextWriter<OwaHtml>)this.sanitizingResponseWriter, this.Request, urlEncodedItemId, this.IsEmbeddedItemInNonSMimeItem, this.UserContext, this.downloadAllCount);
			}
		}

		internal void RenderAttachmentWellForReadItem(ArrayList attachmentWellRenderObjects)
		{
			AttachmentWell.RenderAttachmentWell(this.responseWriter, AttachmentWellType.ReadOnly, attachmentWellRenderObjects, this.UserContext);
		}

		internal int EmbeddedItemNestingLevel
		{
			get
			{
				if (this.IsEmbeddedItem && this.embeddedItemNestingLevel == null)
				{
					this.embeddedItemNestingLevel = new int?(AttachmentUtility.GetEmbeddedItemNestingLevel(this.Request));
				}
				if (this.embeddedItemNestingLevel == null)
				{
					return 0;
				}
				return this.embeddedItemNestingLevel.Value;
			}
		}

		internal bool ShowAttachmentWell
		{
			get
			{
				return this.Item != null && this.Item.AttachmentCollection != null && this.Item.AttachmentCollection.Count > 0;
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

		internal bool IsPreviewForm
		{
			get
			{
				return this.isPreviewForm;
			}
			set
			{
				this.isPreviewForm = value;
			}
		}

		internal bool HasCategories
		{
			get
			{
				return this.item != null && ItemUtility.HasCategories(this.item);
			}
		}

		internal bool IsEmbeddedItem
		{
			get
			{
				return this.IsEmbeddedItemInNonSMimeItem || this.IsEmbeddedItemInSMimeMessage;
			}
		}

		internal bool IsPublicItem
		{
			get
			{
				return Utilities.IsPublic(this.item);
			}
		}

		internal bool IsOtherMailboxItem
		{
			get
			{
				return !this.IsItemNull && this.UserContext.IsInOtherMailbox(this.item);
			}
		}

		internal bool IsItemNull
		{
			get
			{
				return this.item == null;
			}
		}

		internal bool IsSubjectEditable
		{
			get
			{
				return false;
			}
		}

		internal bool IsItemEditable
		{
			get
			{
				return !this.IsEmbeddedItem && ItemUtility.UserCanEditItem(this.Item);
			}
		}

		internal bool IsInDeleteItems
		{
			get
			{
				return this.item != null && !this.IsEmbeddedItemInNonSMimeItem && Utilities.IsItemInDefaultFolder(this.item, DefaultFolderType.DeletedItems);
			}
		}

		internal bool UserCanDeleteItem
		{
			get
			{
				return this.IsItemNull || (!this.IsPublicItem && !this.IsOtherMailboxItem) || ItemUtility.UserCanDeleteItem(this.Item);
			}
		}

		internal bool UserCanEditItem
		{
			get
			{
				return this.IsItemNull || (!this.IsPublicItem && !this.IsOtherMailboxItem) || ItemUtility.UserCanEditItem(this.Item);
			}
		}

		internal string ParentItemIdBase64String
		{
			get
			{
				return OwaStoreObjectId.CreateFromStoreObject(this.parentItem).ToString();
			}
		}

		internal string ParentFolderIdBase64String
		{
			get
			{
				return this.ParentFolderId.ToBase64String();
			}
		}

		internal OwaStoreObjectId ParentFolderId
		{
			get
			{
				Item item = this.IsEmbeddedItemInNonSMimeItem ? this.parentItem : this.item;
				OwaStoreObjectIdType objectIdType = OwaStoreObjectIdType.MailBoxObject;
				string legacyDN = null;
				if (Utilities.IsPublic(item))
				{
					objectIdType = OwaStoreObjectIdType.PublicStoreFolder;
				}
				else if (this.UserContext.IsInOtherMailbox(item))
				{
					objectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
					legacyDN = Utilities.GetMailboxSessionLegacyDN(item);
				}
				else if (Utilities.IsInArchiveMailbox(item))
				{
					objectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
					legacyDN = Utilities.GetMailboxSessionLegacyDN(item);
				}
				return OwaStoreObjectId.CreateFromFolderId(item.ParentId, objectIdType, legacyDN);
			}
		}

		internal string ItemClassName
		{
			get
			{
				return this.Item.ClassName;
			}
		}

		internal ClientSMimeControlStatus ClientSMimeControlStatus
		{
			get
			{
				if (this.clientSMimeControlStatus == ClientSMimeControlStatus.None)
				{
					this.clientSMimeControlStatus = Utilities.CheckClientSMimeControlStatus(Utilities.GetQueryStringParameter(this.Request, "smime", false), this.OwaContext);
				}
				return this.clientSMimeControlStatus;
			}
		}

		internal bool IsJunkOrPhishing
		{
			get
			{
				return JunkEmailUtilities.IsJunkOrPhishing(this.Item, this.IsEmbeddedItem, this.IsRequestCallbackForPhishing, this.UserContext);
			}
		}

		internal bool IsReportItem
		{
			get
			{
				return this.Item is ReportMessage;
			}
		}

		internal bool IsEmbeddedItemInSMimeMessage
		{
			get
			{
				return this.isEmbeddedItemInSMimeMessage;
			}
		}

		internal bool IsEmbeddedItemInNonSMimeItem
		{
			get
			{
				return this.isEmbeddedItemInNonSMimeItem;
			}
		}

		internal bool IsRequestCallbackForPhishing
		{
			get
			{
				return Utilities.IsRequestCallbackForPhishing(this.Request);
			}
		}

		internal bool IsRequestCallbackForWebBeacons
		{
			get
			{
				return Utilities.IsRequestCallbackForWebBeacons(this.Request);
			}
		}

		internal bool IsIgnoredConversation
		{
			get
			{
				return ConversationUtilities.IsConversationIgnored(this.item);
			}
		}

		protected bool IsRequestCallbackForRemoveIRM
		{
			get
			{
				return !string.IsNullOrEmpty(Utilities.GetQueryStringParameter(this.Request, "rr", false));
			}
		}

		internal bool ShouldRenderDownloadAllLink
		{
			get
			{
				return this.shouldRenderDownloadAllLink;
			}
		}

		internal void SetShouldRenderDownloadAllLink(ArrayList attachmentWellInfos)
		{
			if (!this.IsJunkOrPhishing && attachmentWellInfos != null && attachmentWellInfos.Count > 0)
			{
				this.downloadAllCount = AttachmentUtility.GetCountForDownloadAttachments(attachmentWellInfos);
				this.shouldRenderDownloadAllLink = (this.downloadAllCount > 1);
			}
		}

		internal void RenderActionButtons(bool isInJunkMailFolder, bool isPost)
		{
			if (this.IsPreviewForm)
			{
				if (isInJunkMailFolder)
				{
					RenderingUtilities.RenderJunkMailActionIcons(this.responseWriter, this.UserContext);
					return;
				}
				RenderingUtilities.RenderActiveActionIcons(this.responseWriter, this.UserContext, isPost);
			}
		}

		internal const string PreviewFormString = "Preview";

		public const string MarkAsReadPiggyBack = "mrd";

		private Item item;

		private Item parentItem;

		private bool isPreviewForm;

		private bool isEmbeddedItemInNonSMimeItem;

		private bool isEmbeddedItemInSMimeMessage;

		private bool shouldRenderDownloadAllLink;

		private int downloadAllCount;

		private int? embeddedItemNestingLevel;

		private ClientSMimeControlStatus clientSMimeControlStatus;

		private IList<AttachmentLink> attachmentLinks;

		private TextWriter responseWriter;

		private TextWriter sanitizingResponseWriter;
	}
}
