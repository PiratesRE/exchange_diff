using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Attachments")]
	internal sealed class AttachmentEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(AttachmentEventHandler));
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("FId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("IT", typeof(StoreObjectType))]
		[OwaEvent("CreateImplicitDraftItem")]
		public void CreateImplicitDraftItem()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "AttachmentEventHandler.CreateImplicitDraftItem");
			StoreObjectType itemType = (StoreObjectType)base.GetParameter("IT");
			OwaStoreObjectId folderId = null;
			if (base.IsParameterSet("FId"))
			{
				folderId = (OwaStoreObjectId)base.GetParameter("FId");
			}
			Item item = this.CreateImplicitDraftItemHelper(itemType, folderId);
			base.WriteNewItemId(item);
			base.WriteChangeKey(item);
			item.Dispose();
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("CleanupUnlinkedAttachments")]
		[OwaEventParameter("BodyMarkup", typeof(string))]
		public void CleanupUnlinkedAttachments()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "AttachmentEventHandler.CleanupUnlinkedAttachments");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			string bodyMarkup = (string)base.GetParameter("BodyMarkup");
			using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, changeKey, new PropertyDefinition[0]))
			{
				if (base.UserContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(item, base.UserContext, true);
				}
				item.OpenAsReadWrite();
				AttachmentUtility.RemoveUnlinkedInlineAttachments(item, bodyMarkup);
				ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new OwaEventHandlerException("Could not save item due to conflict resolution failure", LocalizedStrings.GetNonEncoded(-482397486), OwaEventHandlerErrorCode.ConflictResolution);
				}
				item.Load();
				this.RenderTotalAttachmentSize(item);
				base.WriteChangeKey(item);
			}
		}

		[OwaEvent("RefreshWell")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		public void RefreshWell()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "AttachmentEventHandler.RefreshWell");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			this.RenderAttachments(owaStoreObjectId);
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("Delete")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("AttId", typeof(string))]
		public void Delete()
		{
			Item item = null;
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "AttachmentEventHandler.Delete");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			try
			{
				item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, changeKey, new PropertyDefinition[0]);
				if (base.UserContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(item, base.UserContext, true);
				}
				AttachmentId attachmentId = item.CreateAttachmentId((string)base.GetParameter("AttId"));
				AttachmentUtility.RemoveAttachment(item, attachmentId);
				ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new OwaEventHandlerException("Could not save item due to conflict resolution failure", LocalizedStrings.GetNonEncoded(-482397486), OwaEventHandlerErrorCode.ConflictResolution);
				}
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsUpdated.Increment();
				}
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
					this.RenderAttachments(owaStoreObjectId);
				}
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("RenderImage")]
		[OwaEventParameter("AttId", typeof(string), false, true)]
		[OwaEventParameter("em", typeof(string), false, true)]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void RenderImage()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Attachments.RenderImage");
			OwaStoreObjectId storeId = null;
			string attId = string.Empty;
			string email = string.Empty;
			if (base.IsParameterSet("Id"))
			{
				storeId = (OwaStoreObjectId)base.GetParameter("Id");
			}
			if (base.IsParameterSet("AttId"))
			{
				attId = (string)base.GetParameter("AttId");
			}
			if (base.IsParameterSet("em"))
			{
				email = (string)base.GetParameter("em");
			}
			string empty = string.Empty;
			using (Stream contactPictureStream = this.GetContactPictureStream(storeId, attId, email, out empty))
			{
				this.OutputImage(contactPictureStream, empty);
			}
			Utilities.MakePageCacheable(this.HttpContext.Response, new int?(3));
		}

		[OwaEvent("ClearCachePic")]
		public void ClearCachedPicture()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Attachments.ClearCachedPicture");
			base.UserContext.UploadedDisplayPicture = null;
		}

		[OwaEventParameter("FC", typeof(string))]
		[OwaEventParameter("Dpc", typeof(string))]
		[OwaEventParameter("em", typeof(string))]
		[OwaEventParameter("rt", typeof(string))]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("RenderADPhoto")]
		public void RenderADPhoto()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Attachments.RenderADPhoto");
			bool flag = base.IsParameterSet("FC") && !string.IsNullOrEmpty((string)base.GetParameter("FC"));
			string email = string.Empty;
			string routingType = string.Empty;
			if (!flag)
			{
				if (base.IsParameterSet("em"))
				{
					email = (string)base.GetParameter("em");
				}
				if (base.IsParameterSet("rt"))
				{
					routingType = (string)base.GetParameter("rt");
				}
			}
			using (Stream pictureStream = this.GetPictureStream(flag, email, routingType))
			{
				this.OutputImage(pictureStream, "image/jpeg");
			}
			if (!flag)
			{
				Utilities.MakePageCacheable(this.HttpContext.Response, new int?(3));
				return;
			}
			Utilities.MakePageNoCacheNoStore(this.HttpContext.Response);
		}

		private static string GetContentType(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			string extension = Path.GetExtension(path);
			if (string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase))
			{
				return "image/bmp";
			}
			if (string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
			{
				return "image/gif";
			}
			if (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase))
			{
				return "image/jpeg";
			}
			if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
			{
				return "image/png";
			}
			return string.Empty;
		}

		private Stream GetContactPictureStream(OwaStoreObjectId storeId, string attId, string email, out string contentType)
		{
			contentType = string.Empty;
			if (storeId != null)
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, storeId, new PropertyDefinition[0]))
				{
					return this.GetContactPictureStream(item, attId, out contentType);
				}
			}
			using (ContactsFolder contactsFolder = ContactsFolder.Bind(base.UserContext.MailboxSession, DefaultFolderType.Contacts))
			{
				using (FindInfo<Contact> findInfo = contactsFolder.FindByEmailAddress(email, new PropertyDefinition[0]))
				{
					if (findInfo.FindStatus == FindStatus.Found)
					{
						return this.GetContactPictureStream(findInfo.Result, attId, out contentType);
					}
				}
			}
			return new MemoryStream();
		}

		private Stream GetContactPictureStream(Item item, string attId, out string contentType)
		{
			contentType = string.Empty;
			if (item == null)
			{
				return new MemoryStream();
			}
			if (string.IsNullOrEmpty(attId))
			{
				attId = RenderingUtilities.GetContactPictureAttachmentId(item);
			}
			if (string.IsNullOrEmpty(attId))
			{
				return new MemoryStream();
			}
			AttachmentId id = item.CreateAttachmentId(attId);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, true, base.UserContext);
			Stream result;
			using (StreamAttachment streamAttachment = attachmentCollection.Open(id) as StreamAttachment)
			{
				if (streamAttachment == null)
				{
					throw new OwaInvalidRequestException("Attachment is not a stream attachment");
				}
				AttachmentPolicy.Level attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(streamAttachment, base.UserContext);
				if (attachmentLevel == AttachmentPolicy.Level.Block)
				{
					result = new MemoryStream();
				}
				else
				{
					contentType = AttachmentEventHandler.GetContentType(streamAttachment.FileName);
					if (contentType.Length == 0)
					{
						ExTraceGlobals.ContactsTracer.TraceDebug<string>((long)this.GetHashCode(), "Cannot determine image type for file: {0}", streamAttachment.FileName);
						result = new MemoryStream();
					}
					else
					{
						result = streamAttachment.GetContentStream();
					}
				}
			}
			return result;
		}

		private Stream GetPictureStream(bool fromCache, string email, string routingType)
		{
			Stream result;
			if (fromCache)
			{
				byte[] uploadedDisplayPicture = base.UserContext.UploadedDisplayPicture;
				if (uploadedDisplayPicture == null)
				{
					throw new OwaInvalidRequestException("Object not found");
				}
				result = new MemoryStream(uploadedDisplayPicture);
			}
			else
			{
				result = this.GetADPictureStream(email, routingType);
			}
			return result;
		}

		private Stream GetADPictureStream(string email, string routingType)
		{
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, base.UserContext);
			byte[] array = null;
			bool flag = string.Equals(email, base.UserContext.ExchangePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase);
			string stringHash = Utilities.GetStringHash(email);
			bool flag2 = DisplayPictureUtility.IsInRecipientsNegativeCache(stringHash);
			if (!flag2 || flag)
			{
				ProxyAddress proxyAddress = null;
				try
				{
					if (string.Equals(routingType, "EX", StringComparison.Ordinal))
					{
						proxyAddress = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.LegacyDN, email, true);
					}
					else if (string.Equals(routingType, "SMTP", StringComparison.Ordinal))
					{
						proxyAddress = new SmtpProxyAddress(email, true);
					}
					if (proxyAddress != null)
					{
						if (Globals.ArePerfCountersEnabled)
						{
							OwaSingleCounters.SenderPhotosTotalLDAPCalls.Increment();
						}
						ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(proxyAddress, new PropertyDefinition[]
						{
							ADRecipientSchema.ThumbnailPhoto
						});
						if (adrawEntry != null)
						{
							array = (adrawEntry[ADRecipientSchema.ThumbnailPhoto] as byte[]);
						}
					}
					goto IL_10F;
				}
				catch (NonUniqueRecipientException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "GetADPictureStream: NonUniqueRecipientException was thrown by FindByProxyAddress: {0}", ex.Message);
					throw new OwaEventHandlerException("Unable to retrieve recipient data.", LocalizedStrings.GetNonEncoded(-1953304495));
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.SenderPhotosDataFromNegativeCacheCount.Increment();
			}
			IL_10F:
			bool flag3 = array != null && array.Length > 0;
			if (flag)
			{
				base.UserContext.HasPicture = new bool?(flag3);
			}
			if (flag3)
			{
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.SenderPhotosTotalLDAPCallsWithPicture.Increment();
				}
				if (flag2)
				{
					DisplayPictureUtility.RecipientsNegativeCache.Remove(stringHash);
				}
				return new MemoryStream(array);
			}
			if (!flag2)
			{
				int num = DisplayPictureUtility.RecipientsNegativeCache.AddAndCount(stringHash, DateTime.UtcNow);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.SenderPhotosNegativeCacheCount.RawValue = (long)num;
				}
			}
			return new MemoryStream();
		}

		private Item CreateImplicitDraftItemHelper(StoreObjectType itemType, OwaStoreObjectId folderId)
		{
			if (itemType < StoreObjectType.Message)
			{
				throw new OwaInvalidRequestException("Item type provided is for a folder type.");
			}
			if (folderId == null && itemType == StoreObjectType.Post)
			{
				folderId = OwaStoreObjectId.CreateFromMailboxFolderId(base.UserContext.TryGetMyDefaultFolderId(DefaultFolderType.Drafts));
			}
			Item item = Utilities.CreateImplicitDraftItem(itemType, folderId);
			item.Save(SaveMode.ResolveConflicts);
			item.Load();
			return item;
		}

		private void RenderTotalAttachmentSize(Item item)
		{
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, base.UserContext);
			int totalAttachmentSize = AttachmentUtility.GetTotalAttachmentSize(attachmentCollection);
			this.SanitizingWriter.Write("<div id=attSz>");
			this.SanitizingWriter.Write(totalAttachmentSize);
			this.SanitizingWriter.Write("</div>");
		}

		private void RenderAttachments(OwaStoreObjectId owaStoreObjectId)
		{
			Item item = null;
			try
			{
				item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]);
				if (base.UserContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(item, base.UserContext, true);
				}
				this.Writer.Write("<div id=attWD>");
				ArrayList attachmentInformation = AttachmentWell.GetAttachmentInformation(item, null, base.UserContext.IsPublicLogon);
				AttachmentWell.RenderAttachments(this.Writer, AttachmentWellType.ReadWrite, attachmentInformation, base.UserContext);
				this.Writer.Write("</div>");
				this.Writer.Write("<div id=iBData>");
				AttachmentWell.RenderInfobar(this.Writer, attachmentInformation, base.UserContext);
				this.Writer.Write("</div>");
				base.WriteChangeKey(item);
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
		}

		private void OutputImage(Stream inputStream, string contentType)
		{
			HttpWriter httpWriter = this.Writer as HttpWriter;
			Stream outputStream = httpWriter.OutputStream;
			this.HttpContext.Response.ContentType = contentType;
			byte[] array = new byte[32768];
			int count;
			while ((count = inputStream.Read(array, 0, array.Length)) > 0)
			{
				outputStream.Write(array, 0, count);
			}
		}

		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("ExIds", typeof(OwaStoreObjectId), true, true)]
		[OwaEvent("AttachItems")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("IT", typeof(StoreObjectType))]
		public void AttachItems()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string text = (string)base.GetParameter("CK");
			bool flag = false;
			Item item;
			if (!string.IsNullOrEmpty(text) && owaStoreObjectId != null)
			{
				item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, text, new PropertyDefinition[0]);
				if (base.UserContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(item, base.UserContext, true);
				}
			}
			else
			{
				flag = true;
				StoreObjectType itemType = (StoreObjectType)base.GetParameter("IT");
				item = this.CreateImplicitDraftItemHelper(itemType, null);
			}
			List<OwaStoreObjectId> itemsToAttachIds = this.GetItemsToAttachIds();
			SanitizedHtmlString errorInAttachments = AttachmentUtility.AddExistingItems(item, itemsToAttachIds, base.UserContext);
			item.Load();
			if (base.UserContext.IsIrmEnabled)
			{
				Utilities.IrmDecryptIfRestricted(item, base.UserContext, true);
			}
			ArrayList attachmentInformation = AttachmentWell.GetAttachmentInformation(item, null, base.UserContext.IsPublicLogon);
			RenderingUtilities.RenderAttachmentItems(this.SanitizingWriter, attachmentInformation, base.UserContext);
			this.SanitizingWriter.Write("<div id=attIB>");
			AttachmentWell.RenderInfobar(this.SanitizingWriter, attachmentInformation, errorInAttachments, base.UserContext);
			this.SanitizingWriter.Write("</div>");
			if (flag)
			{
				base.WriteNewItemId(item);
			}
			this.RenderTotalAttachmentSize(item);
			base.WriteChangeKey(item);
			if (item != null)
			{
				item.Dispose();
			}
		}

		private List<OwaStoreObjectId> GetItemsToAttachIds()
		{
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("ExIds");
			if (ConversationUtilities.ContainsConversationItem(base.UserContext, array))
			{
				OwaStoreObjectId localFolderId = (OwaStoreObjectId)base.GetParameter("FId");
				array = ConversationUtilities.GetLocalItemIds(base.UserContext, array, localFolderId);
			}
			if (array != null)
			{
				return new List<OwaStoreObjectId>(array);
			}
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AttachmentEventHandler>(this);
		}

		public const string EventNamespace = "Attachments";

		public const string MethodAttachItems = "AttachItems";

		public const string MethodCreateImplicitDraftItem = "CreateImplicitDraftItem";

		public const string MethodCleanupUnlinkedAttachments = "CleanupUnlinkedAttachments";

		public const string MethodRefreshWell = "RefreshWell";

		public const string MethodDelete = "Delete";

		public const string MethodRenderImage = "RenderImage";

		public const string MethodRenderADPhoto = "RenderADPhoto";

		public const string MethodClearCachedPicture = "ClearCachePic";

		public const string Email = "em";

		public const string RoutingType = "rt";

		public const string Id = "Id";

		public const string Ck = "CK";

		public const string ItemType = "IT";

		public const string BodyMarkup = "BodyMarkup";

		public const string AttId = "AttId";

		public const string ExIds = "ExIds";

		public const string FolderId = "FId";

		public const string DisplayPictureCanary = "Dpc";

		public const string FromCache = "FC";
	}
}
