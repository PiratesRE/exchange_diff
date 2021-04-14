using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal static class AttachmentHelper
	{
		internal static string GetAttachment(MailboxSession mailboxSession, string attachmentId, Stream outStream, Unlimited<ByteQuantifiedSize> maxAttachmentSize, ItemIdMapping idmapping, out int total)
		{
			return AttachmentHelper.GetAttachment(mailboxSession, attachmentId, outStream, 0, -1, maxAttachmentSize, idmapping, false, out total);
		}

		internal static string GetAttachment(MailboxSession mailboxSession, string attachmentId, Stream outStream, int offset, int count, Unlimited<ByteQuantifiedSize> maxAttachmentSize, ItemIdMapping idmapping, bool rightsManagementSupport, out int total)
		{
			string[] array = attachmentId.Split(new char[]
			{
				':'
			});
			if (array.Length != 2 && array.Length != 3)
			{
				return AttachmentHelper.GetAttachmentByUrlCompName(mailboxSession, attachmentId, outStream, offset, count, maxAttachmentSize, out total);
			}
			StoreObjectId itemId;
			string text;
			if (array.Length == 2)
			{
				itemId = AttachmentHelper.GetItemId(array[0]);
				text = array[1];
			}
			else
			{
				itemId = AttachmentHelper.GetItemId(idmapping, array[0], array[1]);
				text = array[2];
			}
			if (itemId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentId));
			}
			AirSyncDiagnostics.TraceDebug<StoreObjectId, string>(ExTraceGlobals.RequestsTracer, null, "Getting attachment with itemId {0} and attachmentIndex {1}.", itemId, text);
			return AttachmentHelper.GetAttachment(mailboxSession, itemId, text, outStream, offset, count, maxAttachmentSize, rightsManagementSupport, out total);
		}

		internal static string GetAttachment(MailboxSession mailboxSession, StoreObjectId itemId, string attachmentId, Stream outStream, int offset, int count, Unlimited<ByteQuantifiedSize> maxAttachmentSize, bool rightsManagementSupport, out int total)
		{
			string result;
			using (Item item = Item.Bind(mailboxSession, itemId))
			{
				if ("DRMLicense".Equals(attachmentId, StringComparison.OrdinalIgnoreCase))
				{
					total = AttachmentHelper.WriteDrmLicenseToStream(item, outStream, offset, count, maxAttachmentSize);
					result = "application/x-microsoft-rpmsg-message-license";
				}
				else
				{
					if (rightsManagementSupport)
					{
						Command.CurrentCommand.DecodeIrmMessage(item, true);
					}
					Attachment attachment = null;
					try
					{
						AttachmentCollection attachmentCollection;
						if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
						{
							attachmentCollection = ((RightsManagedMessageItem)item).ProtectedAttachmentCollection;
						}
						else
						{
							attachmentCollection = item.AttachmentCollection;
						}
						if (attachmentId.Length > 8)
						{
							AttachmentId attachmentId2 = EntitySyncItemId.GetAttachmentId(attachmentId);
							if (attachmentId2 != null)
							{
								attachment = attachmentCollection.Open(attachmentId2);
							}
						}
						if (attachment == null)
						{
							int num;
							if (!int.TryParse(attachmentId, NumberStyles.None, CultureInfo.InvariantCulture, out num) || num < 0)
							{
								throw new FormatException("Invalid Attachment Index format: " + attachmentId.ToString(CultureInfo.InvariantCulture));
							}
							IList<AttachmentHandle> handles = attachmentCollection.GetHandles();
							if (num < handles.Count)
							{
								attachment = attachmentCollection.Open(handles[num]);
							}
						}
						if (attachment == null)
						{
							throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentId));
						}
						if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item) && AttachmentHelper.IsProtectedVoiceAttachment(attachment.FileName))
						{
							RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
							rightsManagedMessageItem.UnprotectAttachment(attachment.Id);
							AttachmentId id = attachment.Id;
							attachment.Dispose();
							attachment = rightsManagedMessageItem.ProtectedAttachmentCollection.Open(id);
						}
						if (!maxAttachmentSize.IsUnlimited && attachment.Size > (long)maxAttachmentSize.Value.ToBytes())
						{
							throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
						}
						result = AttachmentHelper.GetAttachmentItself(attachment, outStream, offset, count, out total);
					}
					finally
					{
						if (attachment != null)
						{
							attachment.Dispose();
						}
						if (rightsManagementSupport)
						{
							Command.CurrentCommand.SaveLicense(item);
						}
					}
				}
			}
			return result;
		}

		internal static bool IsProtectedVoiceAttachment(string fileName)
		{
			return !string.IsNullOrEmpty(fileName) && (fileName.EndsWith(".umrmmp3", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".umrmwav", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".umrmwma", StringComparison.OrdinalIgnoreCase));
		}

		internal static bool IsProtectedTranscriptAttachment(string fileName)
		{
			return !string.IsNullOrEmpty(fileName) && fileName.Equals("voicemail.umrmasr", StringComparison.OrdinalIgnoreCase);
		}

		internal static string GetUnprotectedVoiceAttachmentName(string fileName)
		{
			string text = fileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				if (fileName.EndsWith(".umrmmp3", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace(".umrmmp3", ".mp3");
				}
				else if (fileName.EndsWith(".umrmwav", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace(".umrmwav", ".wav");
				}
				else if (fileName.EndsWith(".umrmwma", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace(".umrmwma", ".wma");
				}
			}
			return text;
		}

		internal static void CreateAttachment(IItem parentItem, Attachment16Data attachmentData)
		{
			AirSyncDiagnostics.TraceDebug<byte>(ExTraceGlobals.RequestsTracer, null, "CreateAttachment with AttachMethod:{0}", attachmentData.Method);
			if (attachmentData.Method == 1)
			{
				if (attachmentData.Content == null)
				{
					throw new ConversionException(string.Format(" Attachment content can not be null.", new object[0]));
				}
				IStreamAttachment streamAttachment = parentItem.IAttachmentCollection.CreateIAttachment(AttachmentType.Stream) as IStreamAttachment;
				AttachmentHelper.CopyCommonAttachmentProperties(streamAttachment, attachmentData);
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					contentStream.Write(attachmentData.Content, 0, attachmentData.Content.Length);
				}
				streamAttachment.Save();
			}
			else
			{
				if (attachmentData.Method != 5)
				{
					throw new ConversionException(string.Format("UnSupported Value '{0}' for Attachment Method. Only 1 & 5 is supported AttachemntType", attachmentData.Method));
				}
				ItemAttachment itemAttachment = parentItem.IAttachmentCollection.CreateIAttachment(AttachmentType.EmbeddedMessage) as ItemAttachment;
				AttachmentHelper.CopyCommonAttachmentProperties(itemAttachment, attachmentData);
				using (Stream stream = new MemoryStream(attachmentData.Content))
				{
					stream.Seek(0L, SeekOrigin.Begin);
					InboundConversionOptions inboundConversionOptions = AirSyncUtility.GetInboundConversionOptions();
					inboundConversionOptions.ClearCategories = false;
					try
					{
						using (Item item = itemAttachment.GetItem())
						{
							ItemConversion.ConvertAnyMimeToItem(item, stream, inboundConversionOptions);
							item.Save(SaveMode.NoConflictResolution);
						}
					}
					catch (ExchangeDataException innerException)
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidMIME, innerException, false);
					}
					catch (ConversionFailedException innerException2)
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidMIME, innerException2, false);
					}
				}
				itemAttachment.Save();
			}
			AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, null, "AttachmentHelper:CreateAttachments:: AttachmentCreated successful. AttachmentCount:{0}", parentItem.IAttachmentCollection.Count);
		}

		internal static void DeleteAttachments(IItem parentItem, List<string> fileReferences)
		{
			AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, null, "AttachmentHelper:DeleteAttachments. Try to delete attachment. delete Count:{0}, Actual AttachmentCount:{1}", fileReferences.Count, parentItem.IAttachmentCollection.Count);
			if (parentItem.IAttachmentCollection.Count < fileReferences.Count)
			{
				throw new ConversionException(string.Format("Invalid number of attachments to delete:{0}. Actual Attachment Count:{1}", fileReferences.Count, parentItem.IAttachmentCollection.Count));
			}
			List<int> list = new List<int>();
			foreach (string text in fileReferences)
			{
				string text2 = text.Substring(text.LastIndexOf(':') + 1);
				int num;
				if ("DRMLicense".Equals(text2, StringComparison.OrdinalIgnoreCase) || !int.TryParse(text2, out num) || num < 0)
				{
					throw new ConversionException(string.Format("Invalid attachmentId {0} for delete. AttachmentCount:{1}", text2, parentItem.IAttachmentCollection.Count));
				}
				if (num >= parentItem.IAttachmentCollection.Count)
				{
					throw new ConversionException(string.Format("Invalid attachmentId {0} for delete. AttachmentCount:{1}", text, parentItem.IAttachmentCollection.Count));
				}
				AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, null, "AttachmentHelper:DeleteAttachments. DeleteAttachment with Index {0}.", num);
				list.Add(num);
			}
			foreach (int index in from x in list
			orderby x descending
			select x)
			{
				parentItem.IAttachmentCollection.Remove(parentItem.IAttachmentCollection.GetHandles()[index]);
			}
		}

		private static void CopyCommonAttachmentProperties(IAttachment attachment, Attachment16Data attachmentData)
		{
			if (!string.IsNullOrEmpty(attachmentData.DisplayName))
			{
				attachment.FileName = attachmentData.DisplayName;
				if (!string.IsNullOrEmpty(attachmentData.ContentType))
				{
					attachment.ContentType = attachmentData.ContentType;
				}
				attachment.IsInline = attachmentData.IsInline;
				if (!string.IsNullOrEmpty(attachmentData.ContentId))
				{
					attachment[AttachmentSchema.AttachContentId] = attachmentData.ContentId;
				}
				if (!string.IsNullOrEmpty(attachmentData.ContentLocation))
				{
					attachment[AttachmentSchema.AttachContentLocation] = attachmentData.ContentLocation;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Attachment Properties copied:FileName: {0}, ContentType:{1}, IsInline:{2}, ContentId:{3}, ContentLocation:{4}", new object[]
				{
					attachmentData.DisplayName,
					attachmentData.ContentType,
					attachmentData.IsInline,
					attachmentData.ContentId,
					attachmentData.ContentLocation
				});
				return;
			}
			throw new ConversionException(" Attachment DisplayName can not be null.");
		}

		private static string GetAttachmentByUrlCompName(MailboxSession mailboxSession, string attachmentId, Stream outStream, int offset, int count, Unlimited<ByteQuantifiedSize> maxAttachmentSize, out int total)
		{
			attachmentId = "/" + attachmentId;
			int andCheckLastSlashLocation = AttachmentHelper.GetAndCheckLastSlashLocation(attachmentId, attachmentId);
			string text = attachmentId.Substring(0, andCheckLastSlashLocation);
			string text2 = attachmentId.Substring(andCheckLastSlashLocation + 1);
			andCheckLastSlashLocation = AttachmentHelper.GetAndCheckLastSlashLocation(text, attachmentId);
			string propertyValue = text.Substring(0, andCheckLastSlashLocation);
			StoreId storeId = null;
			QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.UrlName, propertyValue);
			using (Folder folder = Folder.Bind(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, AirSyncUtility.XsoFilters.GetHierarchyFilter, null, AttachmentHelper.folderPropertyIds))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
					{
						object[][] rows = queryResult.GetRows(1);
						storeId = (rows[0][0] as StoreId);
					}
				}
			}
			if (storeId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentId));
			}
			AirSyncDiagnostics.TraceDebug<StoreId>(ExTraceGlobals.RequestsTracer, null, "Getting attachment with parentStoreId {0}.", storeId);
			StoreId storeId2 = null;
			seekFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.UrlName, text);
			using (Folder folder2 = Folder.Bind(mailboxSession, storeId))
			{
				using (QueryResult queryResult2 = folder2.ItemQuery(ItemQueryType.None, null, null, AttachmentHelper.itemPropertyIds))
				{
					if (queryResult2.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
					{
						object[][] rows2 = queryResult2.GetRows(1);
						storeId2 = (rows2[0][0] as StoreId);
					}
				}
			}
			if (storeId2 == null)
			{
				throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentId));
			}
			AirSyncDiagnostics.TraceDebug<StoreId>(ExTraceGlobals.RequestsTracer, null, "Getting attachment with itemStoreId {0}.", storeId2);
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = PropertyTagPropertyDefinition.CreateCustom("UrlCompName", 284360734U);
			string attachmentItself;
			using (Item item = Item.Bind(mailboxSession, storeId2))
			{
				AttachmentCollection attachmentCollection = item.AttachmentCollection;
				attachmentCollection.Load(new PropertyDefinition[]
				{
					propertyTagPropertyDefinition
				});
				AttachmentId attachmentId2 = null;
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = attachmentCollection.Open(handle))
					{
						if (text2.Equals((string)attachment[propertyTagPropertyDefinition], StringComparison.Ordinal))
						{
							attachmentId2 = attachment.Id;
							break;
						}
					}
				}
				if (attachmentId2 == null)
				{
					foreach (AttachmentHandle handle2 in attachmentCollection)
					{
						using (Attachment attachment2 = attachmentCollection.Open(handle2))
						{
							string str = (string)attachment2[propertyTagPropertyDefinition];
							if (text2.EndsWith("_" + str, StringComparison.Ordinal))
							{
								attachmentId2 = attachment2.Id;
								break;
							}
						}
					}
				}
				if (attachmentId2 == null)
				{
					throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentId));
				}
				AirSyncDiagnostics.TraceDebug<AttachmentId>(ExTraceGlobals.RequestsTracer, null, "Getting attachment with attachment ID {0}.", attachmentId2);
				using (Attachment attachment3 = attachmentCollection.Open(attachmentId2))
				{
					if (!maxAttachmentSize.IsUnlimited && attachment3.Size > (long)maxAttachmentSize.Value.ToBytes())
					{
						throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
					}
					attachmentItself = AttachmentHelper.GetAttachmentItself(attachment3, outStream, offset, count, out total);
				}
			}
			return attachmentItself;
		}

		private static int GetAndCheckLastSlashLocation(string test, string attachmentId)
		{
			int num = test.LastIndexOf('/');
			if (num <= 0)
			{
				throw new FormatException("Invalid AttachmentId string format: " + attachmentId);
			}
			return num;
		}

		private static int WriteDrmLicenseToStream(Item item, Stream outStream, int offset, int count, Unlimited<ByteQuantifiedSize> maxAttachmentSize)
		{
			object obj = item.TryGetProperty(MessageItemSchema.DRMLicense);
			byte[][] array = obj as byte[][];
			if (array == null)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Failed to get license property for item: {0} returned: {1}", new object[]
				{
					item.Id.ToBase64String(),
					(obj != null) ? obj.ToString() : "null"
				}));
			}
			if (array.Length <= 0)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid license property for item: {0} length: {1}", new object[]
				{
					item.Id.ToBase64String(),
					array
				}));
			}
			if (!maxAttachmentSize.IsUnlimited && (long)array[0].Length > (long)maxAttachmentSize.Value.ToBytes())
			{
				throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
			}
			int result;
			using (Stream stream = new MemoryStream(array[0]))
			{
				if (stream.Length > 0L)
				{
					if ((long)offset >= stream.Length)
					{
						throw new ArgumentOutOfRangeException("offset");
					}
					int num = (count == -1) ? ((int)stream.Length) : count;
					if (num > GlobalSettings.MaxDocumentDataSize)
					{
						throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
					}
					StreamHelper.CopyStream(stream, outStream, offset, num);
				}
				result = (int)stream.Length;
			}
			return result;
		}

		private static StoreObjectId GetItemId(string item)
		{
			StoreObjectId result;
			try
			{
				result = StoreObjectId.Deserialize(item);
			}
			catch (ArgumentException innerException)
			{
				throw new FormatException("Invalid Attachment StoreObjectId format: " + item, innerException);
			}
			catch (CorruptDataException innerException2)
			{
				throw new FormatException("Invalid Attachment StoreObjectId format: " + item, innerException2);
			}
			return result;
		}

		private static StoreObjectId GetItemId(ItemIdMapping idmapping, string folder, string item)
		{
			string syncId = folder + ":" + item;
			MailboxSyncItemId mailboxSyncItemId = idmapping[syncId] as MailboxSyncItemId;
			if (mailboxSyncItemId != null)
			{
				return (StoreObjectId)mailboxSyncItemId.NativeId;
			}
			return null;
		}

		private static string GetAttachmentItself(Attachment attachment, Stream outStream, int offset, int count, out int total)
		{
			string text = string.Empty;
			total = 0;
			StreamAttachmentBase streamAttachmentBase = attachment as StreamAttachmentBase;
			if (streamAttachmentBase != null)
			{
				OleAttachment oleAttachment = streamAttachmentBase as OleAttachment;
				Stream stream = null;
				try
				{
					if (oleAttachment != null)
					{
						stream = oleAttachment.TryConvertToImage(ImageFormat.Jpeg);
						if (stream != null)
						{
							text = "image/jpeg";
						}
					}
					if (stream == null)
					{
						stream = streamAttachmentBase.GetContentStream();
					}
					if (string.IsNullOrEmpty(text))
					{
						text = attachment.ContentType;
					}
					if (string.IsNullOrEmpty(text))
					{
						text = attachment.CalculatedContentType;
					}
					if (stream.Length > 0L)
					{
						if ((long)offset >= stream.Length)
						{
							throw new ArgumentOutOfRangeException("offset");
						}
						int num = (count == -1) ? ((int)stream.Length) : count;
						if (num > GlobalSettings.MaxDocumentDataSize)
						{
							throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
						}
						StreamHelper.CopyStream(stream, outStream, offset, num);
						total = (int)stream.Length;
					}
				}
				finally
				{
					if (stream != null)
					{
						stream.Dispose();
						stream = null;
					}
				}
				return text;
			}
			ItemAttachment itemAttachment = attachment as ItemAttachment;
			if (itemAttachment != null)
			{
				using (Item item = itemAttachment.GetItem(StoreObjectSchema.ContentConversionProperties))
				{
					text = "message/rfc822";
					OutboundConversionOptions outboundConversionOptions = AirSyncUtility.GetOutboundConversionOptions();
					using (AirSyncStream airSyncStream = new AirSyncStream())
					{
						try
						{
							ItemConversion.ConvertItemToMime(item, airSyncStream, outboundConversionOptions);
						}
						catch (ConversionFailedException innerException)
						{
							throw new FormatException(string.Format(CultureInfo.InvariantCulture, "MIME conversion failed for Attachment {0}!", new object[]
							{
								attachment
							}), innerException);
						}
						if (airSyncStream.Length > 0L)
						{
							if ((long)offset >= airSyncStream.Length)
							{
								throw new ArgumentOutOfRangeException("offset");
							}
							int num2 = (count == -1) ? ((int)airSyncStream.Length) : count;
							if (num2 > GlobalSettings.MaxDocumentDataSize)
							{
								throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
							}
							StreamHelper.CopyStream(airSyncStream, outStream, offset, num2);
							total = (int)airSyncStream.Length;
						}
					}
				}
				return text;
			}
			throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Attachment {0} is of invalid format!", new object[]
			{
				attachment
			}));
		}

		private static readonly StorePropertyDefinition[] folderPropertyIds = new StorePropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.UrlName
		};

		private static readonly StorePropertyDefinition[] itemPropertyIds = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			FolderSchema.UrlName
		};

		private enum FolderPropertyIdType
		{
			Id,
			UrlName
		}
	}
}
