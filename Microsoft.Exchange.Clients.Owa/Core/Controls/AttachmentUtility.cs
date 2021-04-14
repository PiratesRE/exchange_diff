using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Common.Sniff;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal static class AttachmentUtility
	{
		private static uint[] GenerateCrc32Table()
		{
			int num = 256;
			uint[] array = new uint[num];
			for (int i = 0; i < num; i++)
			{
				uint num2 = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					if ((num2 & 1U) != 0U)
					{
						num2 = (3988292384U ^ num2 >> 1);
					}
					else
					{
						num2 >>= 1;
					}
				}
				array[i] = num2;
			}
			return array;
		}

		public static AttachmentAddResult AddAttachment(Item item, HttpFileCollection files, UserContext userContext)
		{
			List<SanitizedHtmlString> list;
			return AttachmentUtility.AddAttachment(item, files, userContext, false, null, out list);
		}

		public static AttachmentAddResult AddAttachment(Item item, HttpFileCollection files, UserContext userContext, bool isInlineImage, string bodyMarkup)
		{
			List<SanitizedHtmlString> list;
			return AttachmentUtility.AddAttachment(item, files, userContext, isInlineImage, bodyMarkup, out list);
		}

		public static AttachmentAddResult AddAttachment(Item item, HttpFileCollection files, UserContext userContext, bool isInlineImage, string bodyMarkup, out List<SanitizedHtmlString> attachmentLinks)
		{
			attachmentLinks = new List<SanitizedHtmlString>(files.Count);
			List<AttachmentId> list = new List<AttachmentId>(files.Count);
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Utilities.MakeModifiedCalendarItemOccurrence(item);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, userContext);
			int num = AttachmentUtility.GetTotalAttachmentSize(attachmentCollection);
			int maximumFileSize = AttachmentUtility.GetMaximumFileSize(userContext);
			int num2 = AttachmentUtility.ToByteSize(maximumFileSize);
			int num3 = 0;
			StringBuilder stringBuilder = null;
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = null;
			bool flag = false;
			for (int i = 0; i < files.Count; i++)
			{
				if (!string.IsNullOrEmpty(files[i].FileName))
				{
					string text = AttachmentUtility.AttachGetFileName(files[i].FileName);
					if (!string.IsNullOrEmpty(text))
					{
						if (num + files[i].ContentLength > num2 && !string.IsNullOrEmpty(bodyMarkup) && !flag)
						{
							AttachmentUtility.RemoveUnlinkedInlineAttachments(item, bodyMarkup);
							num = AttachmentUtility.GetTotalAttachmentSize(attachmentCollection);
							flag = true;
						}
						num += files[i].ContentLength;
						if (num > num2)
						{
							num -= files[i].ContentLength;
							if (stringBuilder == null)
							{
								stringBuilder = new StringBuilder(text);
							}
							else
							{
								stringBuilder.Append(", ");
								stringBuilder.Append(text);
							}
						}
						else if (isInlineImage && !AttachmentUtility.IsSupportedImageContentType(files[i]))
						{
							if (sanitizingStringBuilder == null)
							{
								sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>(text);
							}
							else
							{
								sanitizingStringBuilder.Append(", ");
								sanitizingStringBuilder.Append(text);
							}
							num3++;
						}
						else
						{
							int num4;
							AttachmentId item2;
							SanitizedHtmlString item3;
							AttachmentAddResult attachmentAddResult = AttachmentUtility.AddAttachmentFromStream(item, text, files[i].ContentType, files[i].InputStream, userContext, isInlineImage, out num4, out item2, out item3);
							if (attachmentAddResult.ResultCode != AttachmentAddResultCode.NoError)
							{
								return attachmentAddResult;
							}
							list.Add(item2);
							attachmentLinks.Add(item3);
						}
					}
				}
			}
			AttachmentAddResult noError = AttachmentAddResult.NoError;
			ConflictResolutionResult conflictResolutionResult = null;
			bool flag2 = false;
			try
			{
				conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
				item.Load();
				if (!userContext.IsBasicExperience && userContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(item, userContext, true);
				}
			}
			catch (MessageTooBigException)
			{
				flag2 = true;
			}
			bool flag3 = false;
			if (conflictResolutionResult != null && conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				noError.SetResult(AttachmentAddResultCode.IrresolvableConflictWhenSaving, SanitizedHtmlString.FromStringId(-482397486));
				flag3 = true;
			}
			if (num3 != 0)
			{
				SanitizedHtmlString message = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1293887935), new object[]
				{
					sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>()
				});
				noError.SetResult(AttachmentAddResultCode.InsertingNonImageAttachment, message);
			}
			if (noError.ResultCode == AttachmentAddResultCode.NoError && stringBuilder != null)
			{
				SanitizedHtmlString message2 = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-178989031), new object[]
				{
					stringBuilder.ToString(),
					maximumFileSize
				});
				noError.SetResult(AttachmentAddResultCode.AttachmentExcceedsSizeLimit, message2);
			}
			else if (flag2)
			{
				noError.SetResult(AttachmentAddResultCode.ItemExcceedsSizeLimit, SanitizedHtmlString.FromStringId(-124437133));
			}
			if (flag3)
			{
				foreach (AttachmentId attachmentId in list)
				{
					attachmentCollection.Remove(attachmentId);
				}
			}
			return noError;
		}

		private static string AttachGetFileName(string userFileName)
		{
			int num = userFileName.LastIndexOfAny(AttachmentUtility.directorySeparatorCharacters);
			if (num == -1)
			{
				return userFileName;
			}
			return userFileName.Substring(num + 1);
		}

		public static AttachmentAddResult AddAttachmentFromStream(Item item, string attachmentFileName, string contentType, Stream inputStream, UserContext userContext, out int sizeInBytes)
		{
			AttachmentId attachmentId;
			SanitizedHtmlString sanitizedHtmlString;
			return AttachmentUtility.AddAttachmentFromStream(item, attachmentFileName, contentType, inputStream, userContext, false, out sizeInBytes, out attachmentId, out sanitizedHtmlString);
		}

		public static AttachmentAddResult AddAttachmentFromStream(Item item, string attachmentFileName, string contentType, Stream inputStream, UserContext userContext, bool isInlineImage, out int sizeInBytes, out AttachmentId attachmentId, out SanitizedHtmlString attachmentLinkUrl)
		{
			AttachmentAddResult noError = AttachmentAddResult.NoError;
			attachmentLinkUrl = null;
			attachmentId = null;
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(attachmentFileName))
			{
				throw new ArgumentNullException("attachmentFileName");
			}
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, userContext);
			using (StreamAttachment streamAttachment = (StreamAttachment)attachmentCollection.Create(AttachmentType.Stream))
			{
				streamAttachment.FileName = attachmentFileName;
				streamAttachment[AttachmentSchema.DisplayName] = attachmentFileName;
				streamAttachment.ContentType = contentType;
				if (isInlineImage)
				{
					streamAttachment.IsInline = isInlineImage;
					streamAttachment.ContentId = Guid.NewGuid().ToString();
				}
				sizeInBytes = 0;
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					byte[] array = new byte[32768];
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					int num;
					while ((num = inputStream.Read(array, 0, array.Length)) > 0)
					{
						contentStream.Write(array, 0, num);
						sizeInBytes += num;
						UserContextManager.TouchUserContext(userContext, stopwatch);
					}
					contentStream.Close();
				}
				try
				{
					streamAttachment.Save();
					streamAttachment.Load();
					attachmentLinkUrl = AttachmentUtility.GetInlineAttachmentLink(streamAttachment, item);
					attachmentId = streamAttachment.Id;
				}
				catch (ObjectNotFoundException)
				{
					if (ExTraceGlobals.DocumentsTracer.IsTraceEnabled(TraceType.DebugTrace) && item.Id != null)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug<string>(0L, "Attachment was not saved in item.  ItemId = {0}  ", item.Id.ToBase64String());
					}
					noError.SetResult(AttachmentAddResultCode.GeneralErrorWhenSaving, SanitizedHtmlString.FromStringId(-2102593951));
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.AttachmentsUploaded.Increment();
			}
			return noError;
		}

		public static SanitizedHtmlString AddExistingItems(Item item, List<OwaStoreObjectId> itemsToAttachIds, UserContext userContext)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (itemsToAttachIds == null)
			{
				throw new ArgumentNullException("itemsToAttachIds");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			int maximumFileSize = AttachmentUtility.GetMaximumFileSize(userContext);
			OwaStoreObjectId obj = OwaStoreObjectId.CreateFromStoreObject(item);
			Item item2 = null;
			ItemAttachment itemAttachment = null;
			List<AttachmentId> list = new List<AttachmentId>(itemsToAttachIds.Count);
			SanitizedHtmlString sanitizedHtmlString = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Utilities.MakeModifiedCalendarItemOccurrence(item);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, userContext);
			int count = attachmentCollection.Count;
			if (count + itemsToAttachIds.Count > 499)
			{
				return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1025276934), new object[]
				{
					499
				});
			}
			long num = (long)AttachmentUtility.GetTotalAttachmentSize(attachmentCollection);
			int i = 0;
			while (i < itemsToAttachIds.Count)
			{
				if (itemsToAttachIds[i].Equals(obj))
				{
					item2 = Item.CloneItem(userContext.MailboxSession, userContext.TryGetMyDefaultFolderId(DefaultFolderType.Drafts), item, false, false, null);
					Utilities.SaveItem(item2);
					item2.Load();
					flag3 = true;
					goto IL_111;
				}
				try
				{
					item2 = Utilities.GetItem<Item>(userContext, itemsToAttachIds[i], new PropertyDefinition[0]);
				}
				catch (ObjectNotFoundException)
				{
					flag2 = true;
					goto IL_1D1;
				}
				goto IL_111;
				IL_1D1:
				i++;
				continue;
				IL_111:
				using (item2)
				{
					num += item2.Size();
					if (num > (long)AttachmentUtility.ToByteSize(maximumFileSize))
					{
						flag = true;
					}
					else if (ObjectClass.IsOfClass(item2.ClassName, "IPM.Note.Microsoft.Approval.Request"))
					{
						flag2 = true;
					}
					else
					{
						try
						{
							itemAttachment = attachmentCollection.AddExistingItem(item2);
						}
						catch (ObjectNotFoundException)
						{
							flag2 = true;
							goto IL_1D1;
						}
						catch (StoragePermanentException)
						{
							flag2 = true;
							goto IL_1D1;
						}
						catch (StorageTransientException)
						{
							flag2 = true;
							goto IL_1D1;
						}
						finally
						{
							if (itemAttachment == null)
							{
								flag2 = true;
							}
							else
							{
								itemAttachment.Save();
								itemAttachment.Load();
								list.Add(itemAttachment.Id);
								itemAttachment.Dispose();
								itemAttachment = null;
							}
						}
						if (flag3)
						{
							Utilities.Delete(userContext, DeleteItemFlags.HardDelete, new OwaStoreObjectId[]
							{
								(item2.Id == null) ? null : OwaStoreObjectId.CreateFromStoreObject(item2)
							});
							flag3 = false;
						}
					}
				}
				goto IL_1D1;
			}
			ConflictResolutionResult conflictResolutionResult = null;
			bool flag4 = false;
			try
			{
				conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			}
			catch (MessageTooBigException)
			{
				flag4 = true;
			}
			bool flag5 = false;
			if (conflictResolutionResult != null && conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				sanitizedHtmlString = SanitizedHtmlString.FromStringId(-482397486);
				flag5 = true;
			}
			if (SanitizedStringBase<OwaHtml>.IsNullOrEmpty(sanitizedHtmlString) && flag4)
			{
				flag5 = true;
				sanitizedHtmlString = SanitizedHtmlString.FromStringId(543046074);
			}
			else if (flag)
			{
				sanitizedHtmlString = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1099394719), new object[]
				{
					maximumFileSize
				});
			}
			else if (flag2)
			{
				sanitizedHtmlString = SanitizedHtmlString.FromStringId(445087080);
			}
			if (flag5)
			{
				foreach (AttachmentId attachmentId in list)
				{
					attachmentCollection.Remove(attachmentId);
				}
			}
			return sanitizedHtmlString;
		}

		public static void RemoveAttachment(Item item, ArrayList attachmentId)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (attachmentId == null)
			{
				throw new ArgumentNullException("attachmentId");
			}
			if (attachmentId.Count > 0)
			{
				Utilities.MakeModifiedCalendarItemOccurrence(item);
			}
			for (int i = 0; i < attachmentId.Count; i++)
			{
				AttachmentId attachmentId2 = (AttachmentId)attachmentId[i];
				AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, OwaContext.Current.UserContext);
				attachmentCollection.Remove(attachmentId2);
			}
		}

		public static void RemoveAttachment(Item item, AttachmentId attachmentId)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (attachmentId == null)
			{
				throw new ArgumentNullException("attachmentId");
			}
			Utilities.MakeModifiedCalendarItemOccurrence(item);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, OwaContext.Current.UserContext);
			attachmentCollection.Remove(attachmentId);
		}

		internal static bool RemoveUnlinkedInlineAttachments(Item item, string bodyMarkup)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (bodyMarkup == null)
			{
				throw new ArgumentNullException("bodyMarkup");
			}
			StoreObjectType storeObjectType = StoreObjectType.Unknown;
			if (ObjectClass.IsMessage(item.ClassName, false))
			{
				storeObjectType = StoreObjectType.Message;
			}
			else if (ObjectClass.IsMeetingMessage(item.ClassName))
			{
				storeObjectType = StoreObjectType.MeetingMessage;
			}
			else if (ObjectClass.IsCalendarItemOrOccurrence(item.ClassName))
			{
				storeObjectType = StoreObjectType.CalendarItem;
			}
			return storeObjectType != StoreObjectType.Unknown && BodyConversionUtilities.SetBody(item, bodyMarkup, Markup.Html, storeObjectType, OwaContext.Current.UserContext);
		}

		public static ArrayList GetAttachmentList(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem, bool discardInline)
		{
			return AttachmentUtility.GetAttachmentList(item, attachmentLinks, isLoggedOnFromPublicComputer, isEmbeddedItem, discardInline, false);
		}

		public static ArrayList GetAttachmentList(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem, bool discardInline, bool forceEnableItemLink)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (item is ReportMessage || ObjectClass.IsSmsMessage(item.ClassName))
			{
				return null;
			}
			UserContext userContext = UserContextManager.GetUserContext();
			bool isJunkOrPhishing = JunkEmailUtilities.IsJunkOrPhishing(item, isEmbeddedItem, forceEnableItemLink, userContext);
			bool isSharingMessage = AttachmentUtility.IsSharingMessage(item.ClassName);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, true, userContext);
			ArrayList result = new ArrayList();
			if (attachmentLinks != null)
			{
				result = AttachmentUtility.CreateAttachmentList(attachmentLinks, attachmentCollection, isLoggedOnFromPublicComputer, isJunkOrPhishing, discardInline, isSharingMessage);
			}
			else
			{
				result = AttachmentUtility.CreateAttachmentList(attachmentCollection, isLoggedOnFromPublicComputer, isJunkOrPhishing, discardInline, isSharingMessage);
			}
			return result;
		}

		internal static ArrayList GetAttachmentListForZip(Item item, bool discardInline, UserContext userContext)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (item is ReportMessage || ObjectClass.IsSmsMessage(item.ClassName))
			{
				return null;
			}
			bool isSharingMessage = AttachmentUtility.IsSharingMessage(item.ClassName);
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, true, userContext);
			return AttachmentUtility.CreateAttachmentListForZip(attachmentCollection, discardInline, isSharingMessage, userContext);
		}

		private static ArrayList CreateAttachmentListForZip(AttachmentCollection attachmentCollection, bool discardInline, bool isSharingMessage, UserContext userContext)
		{
			ArrayList arrayList = new ArrayList();
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if (!discardInline || (!(attachment is OleAttachment) && !attachment.IsInline))
					{
						if (!isSharingMessage || !AttachmentUtility.IsSharingMessageAttachment(attachment.ContentType, attachment.DisplayName))
						{
							AttachmentPolicy.Level attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(attachment, userContext);
							if (attachmentLevel != AttachmentPolicy.Level.Block)
							{
								string contentType = AttachmentUtility.CalculateContentType(attachment);
								if (!AttachmentUtility.IsMhtmlAttachment(contentType, attachment.FileExtension))
								{
									if (!AttachmentUtility.IsSmimeAttachment(contentType, attachment.FileName))
									{
										AttachmentWellInfo value = AttachmentUtility.CreateAttachmentInfoObject(attachmentCollection, attachment, false, false);
										arrayList.Add(value);
									}
								}
							}
						}
					}
				}
			}
			return arrayList;
		}

		public static int GetCountForDownloadAttachments(ArrayList attachmentWellInfos)
		{
			int num = 0;
			foreach (object obj in attachmentWellInfos)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				if (attachmentWellInfo.AttachmentLevel != AttachmentPolicy.Level.Block && !AttachmentUtility.IsMhtmlAttachment(attachmentWellInfo.MimeType, attachmentWellInfo.FileExtension))
				{
					num++;
				}
			}
			return num;
		}

		private static ArrayList CreateAttachmentList(AttachmentCollection attachmentCollection, bool isLoggedOnFromPublicComputer, bool isJunkOrPhishing, bool discardInline, bool isSharingMessage)
		{
			ArrayList arrayList = new ArrayList();
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if (!discardInline || (!(attachment is OleAttachment) && !attachment.IsInline))
					{
						if (!isSharingMessage || !AttachmentUtility.IsSharingMessageAttachment(attachment.ContentType, attachment.DisplayName))
						{
							AttachmentWellInfo value = AttachmentUtility.CreateAttachmentInfoObject(attachmentCollection, attachment, isLoggedOnFromPublicComputer, isJunkOrPhishing);
							arrayList.Add(value);
						}
					}
				}
			}
			return arrayList;
		}

		private static ArrayList CreateAttachmentList(IList<AttachmentLink> attachmentLinks, AttachmentCollection attachmentCollection, bool isLoggedOnFromPublicComputer, bool isJunkOrPhishing, bool discardInline, bool isSharingMessage)
		{
			ArrayList arrayList = new ArrayList();
			foreach (AttachmentLink attachmentLink in attachmentLinks)
			{
				if (attachmentCollection.Contains(attachmentLink.AttachmentId) && (!discardInline || (attachmentLink.AttachmentType != AttachmentType.Ole && !(attachmentLink.IsMarkedInline == true))) && (!isSharingMessage || !AttachmentUtility.IsSharingMessageAttachment(attachmentLink.ContentType, attachmentLink.DisplayName)))
				{
					AttachmentWellInfo value = AttachmentUtility.CreateAttachmentInfoObject(attachmentCollection, attachmentLink, isLoggedOnFromPublicComputer, isJunkOrPhishing);
					arrayList.Add(value);
				}
			}
			return arrayList;
		}

		public static bool IsSharingMessage(string className)
		{
			return ObjectClass.IsOfClass(className, "IPM.Sharing");
		}

		private static bool IsSharingMessageAttachment(string contentType, string displayName)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(contentType, "application/x-sharing-metadata-xml") == 0 && 0 == StringComparer.OrdinalIgnoreCase.Compare(displayName, "sharing_metadata.xml");
		}

		private static bool IsSmimeAttachment(string contentType, string fileName)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(fileName, "smime.p7m") == 0 || 0 == StringComparer.OrdinalIgnoreCase.Compare(contentType, "multipart/signed");
		}

		public static ArrayList GetAttachmentList(OwaStoreObjectId owaConversationId, ItemPart itemPart, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem, bool discardInline, bool forceEnableItemLink)
		{
			if (owaConversationId == null)
			{
				throw new ArgumentNullException("owaConversationId");
			}
			if (itemPart == null)
			{
				throw new ArgumentNullException("itemPart");
			}
			if (!owaConversationId.IsConversationId)
			{
				throw new ArgumentException("owaConversationId");
			}
			string text = string.Empty;
			object obj = itemPart.StorePropertyBag.TryGetProperty(StoreObjectSchema.ItemClass);
			if (!(obj is PropertyError))
			{
				text = (string)obj;
			}
			if (ObjectClass.IsReport(text) || ObjectClass.IsSmsMessage(text))
			{
				return null;
			}
			ArrayList arrayList = null;
			bool isJunkOrPhishing = JunkEmailUtilities.IsJunkOrPhishing(itemPart.StorePropertyBag, isEmbeddedItem, forceEnableItemLink, UserContextManager.GetUserContext());
			bool flag = AttachmentUtility.IsSharingMessage(text);
			arrayList = new ArrayList();
			foreach (AttachmentInfo attachmentInfo in itemPart.Attachments)
			{
				if ((!discardInline || (attachmentInfo.AttachmentType != AttachmentType.Ole && !attachmentInfo.IsInline)) && (!flag || !AttachmentUtility.IsSharingMessageAttachment(attachmentInfo.ContentType, attachmentInfo.DisplayName)))
				{
					AttachmentWellInfo value = AttachmentUtility.CreateAttachmentInfoObject(owaConversationId, attachmentInfo, isLoggedOnFromPublicComputer, isJunkOrPhishing);
					arrayList.Add(value);
				}
			}
			return arrayList;
		}

		internal static string CalculateContentType(Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment");
			}
			if (!string.IsNullOrEmpty(attachment.ContentType))
			{
				return attachment.ContentType;
			}
			return attachment.CalculatedContentType;
		}

		internal static void SetResponseHeadersForZipAttachments(HttpContext httpContext, string fileName)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new OwaInvalidInputException("Argument fileName may not be null or empty string");
			}
			httpContext.Response.ContentType = "application/zip; authoritative=true;";
			AttachmentUtility.SetZipContentDispositionResponseHeader(httpContext, fileName);
			httpContext.Response.Cache.SetExpires(AttachmentUtility.GetAttachmentExpiryDate());
		}

		private static void SetZipContentDispositionResponseHeader(HttpContext httpContext, string fileName)
		{
			string str = AttachmentUtility.ToHexString(fileName);
			httpContext.Response.AppendHeader("Content-Disposition", "filename=" + str);
		}

		internal static void SetContentDispositionResponseHeader(HttpContext httpContext, string fileName, bool isInline)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new OwaInvalidInputException("Argument fileName may not be null or empty string");
			}
			string text = AttachmentUtility.ToHexString(fileName);
			string value = string.Empty;
			BrowserType browserType = Utilities.GetBrowserType(httpContext.Request.UserAgent);
			if (browserType == BrowserType.Firefox)
			{
				value = string.Format(CultureInfo.InvariantCulture, "{0}; filename*=\"{1}\"", new object[]
				{
					isInline ? "inline" : "attachment",
					text
				});
			}
			else
			{
				value = string.Format(CultureInfo.InvariantCulture, "{0}; filename=\"{1}\"", new object[]
				{
					isInline ? "inline" : "attachment",
					text
				});
			}
			httpContext.Response.AppendHeader("Content-Disposition", value);
		}

		internal static DateTime GetAttachmentExpiryDate()
		{
			ExDateTime exDateTime = DateTimeUtilities.GetLocalTime().IncrementDays(-1);
			return (DateTime)exDateTime;
		}

		private static string ToHexString(string fileName)
		{
			StringBuilder stringBuilder = new StringBuilder(fileName.Length);
			byte[] bytes = Encoding.UTF8.GetBytes(fileName);
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] >= 0 && bytes[i] <= 127 && !AttachmentUtility.IsMIMEAttributeSpecialChar((char)bytes[i]))
				{
					stringBuilder.Append((char)bytes[i]);
				}
				else
				{
					stringBuilder.AppendFormat("%{0}", Convert.ToString(bytes[i], 16));
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsMIMEAttributeSpecialChar(char c)
		{
			if (char.IsControl(c))
			{
				return true;
			}
			switch (c)
			{
			case ' ':
			case '"':
			case '%':
			case '\'':
			case '(':
			case ')':
			case '*':
			case ',':
			case '/':
				break;
			case '!':
			case '#':
			case '$':
			case '&':
			case '+':
			case '-':
			case '.':
				return false;
			default:
				switch (c)
				{
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
					break;
				default:
					switch (c)
					{
					case '[':
					case '\\':
					case ']':
						break;
					default:
						return false;
					}
					break;
				}
				break;
			}
			return true;
		}

		internal static void UpdateAcceptEncodingHeader(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (!string.IsNullOrEmpty(httpContext.Request.Headers["Accept-Encoding"]))
			{
				httpContext.Request.Headers["Accept-Encoding"] = string.Empty;
			}
		}

		internal static BlockStatus GetItemBlockStatus(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			BlockStatus result = BlockStatus.DontKnow;
			object obj = item.TryGetProperty(ItemSchema.BlockStatus);
			if (obj is BlockStatus)
			{
				result = (BlockStatus)obj;
			}
			return result;
		}

		internal static bool IsMhtmlAttachment(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return contentType.ToLowerInvariant().Contains("multipart/related") || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".mht") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".mhtml") == 0;
		}

		internal static bool IsXmlAttachment(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return contentType.ToLowerInvariant().Contains("text/xml") || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xml") == 0;
		}

		internal static bool IsHtmlAttachment(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return contentType.ToLowerInvariant().Contains("text/html") || contentType.ToLowerInvariant().Contains("application/xhtml+xml") || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".htm") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".html") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xhtml") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xht") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".shtml") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".shtm") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".stm") == 0;
		}

		internal static bool DoNeedToFilterHtml(string contentType, string fileExtension, AttachmentPolicy.Level level, UserContext userContext)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			bool flag = AttachmentUtility.IsHtmlAttachment(contentType, fileExtension);
			bool flag2 = AttachmentPolicy.Level.ForceSave == level;
			bool result = false;
			bool flag3 = userContext.IsFeatureEnabled(Feature.ForceSaveAttachmentFiltering);
			if (flag)
			{
				result = (!flag2 || (flag2 && flag3));
			}
			return result;
		}

		internal static void UpdateContentTypeForNeedToFilter(out string contentType, Charset charset)
		{
			Encoding encoding = null;
			if (charset != null && charset.TryGetEncoding(out encoding))
			{
				contentType = "text/html; charset=" + charset.Name;
				return;
			}
			contentType = Utilities.GetContentTypeString(OwaEventContentType.Html);
		}

		internal static StreamAttachmentBase GetStreamAttachment(Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment");
			}
			return attachment as StreamAttachmentBase;
		}

		internal static Stream GetStream(StreamAttachmentBase streamAttachment)
		{
			if (streamAttachment == null)
			{
				throw new ArgumentNullException("streamAttachment");
			}
			OleAttachment oleAttachment = streamAttachment as OleAttachment;
			Stream result;
			if (oleAttachment != null)
			{
				result = oleAttachment.TryConvertToImage(ImageFormat.Jpeg);
			}
			else
			{
				result = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly);
			}
			return result;
		}

		internal static bool GetIsHtmlOrXml(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return AttachmentUtility.IsXmlAttachment(contentType, fileExtension) || AttachmentUtility.IsHtmlAttachment(contentType, fileExtension);
		}

		internal static bool GetDoNotSniff(AttachmentPolicy.Level level, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return AttachmentPolicy.Level.ForceSave == level && !userContext.IsFeatureEnabled(Feature.ForceSaveAttachmentFiltering);
		}

		internal static uint WriteFilteredResponse(HttpContext httpContext, Stream stream, Charset charset, BlockStatus blockStatus)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			uint result = 0U;
			try
			{
				using (Stream filteredStream = AttachmentUtility.GetFilteredStream(httpContext, stream, charset, blockStatus))
				{
					result = AttachmentUtility.WriteOutputStream(httpContext.Response.OutputStream, filteredStream);
				}
			}
			catch (ExchangeDataException innerException)
			{
				throw new OwaBodyConversionFailedException("Sanitize Html Failed", innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new OwaBodyConversionFailedException("Safe Html Attachment Conversion Failed", innerException2);
			}
			catch (StorageTransientException innerException3)
			{
				throw new OwaBodyConversionFailedException("Safe Html Attachment Conversion Failed", innerException3);
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
			return result;
		}

		internal static Stream GetFilteredStream(HttpContext httpContext, Stream inputStream, Charset charset, BlockStatus blockStatus)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			OwaContext owaContext = OwaContext.Get(httpContext);
			UserContext userContext = owaContext.UserContext;
			HtmlToHtml htmlToHtml = new HtmlToHtml();
			TextConvertersInternalHelpers.SetPreserveDisplayNoneStyle(htmlToHtml, true);
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = userContext.Configuration.FilterWebBeaconsAndHtmlForms;
			OwaSafeHtmlOutboundCallbacks @object;
			if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || blockStatus == BlockStatus.NoNeverAgain)
			{
				@object = new OwaSafeHtmlAllowWebBeaconCallbacks(owaContext, true);
			}
			else
			{
				@object = new OwaSafeHtmlOutboundCallbacks(owaContext, false);
			}
			Encoding encoding = null;
			if (charset != null && charset.TryGetEncoding(out encoding))
			{
				htmlToHtml.DetectEncodingFromMetaTag = false;
				htmlToHtml.InputEncoding = encoding;
				htmlToHtml.OutputEncoding = encoding;
			}
			else
			{
				htmlToHtml.DetectEncodingFromMetaTag = true;
				htmlToHtml.InputEncoding = Encoding.ASCII;
				htmlToHtml.OutputEncoding = null;
			}
			htmlToHtml.FilterHtml = true;
			htmlToHtml.HtmlTagCallback = new HtmlTagCallback(@object.ProcessTag);
			return new ConverterStream(inputStream, htmlToHtml, ConverterStreamAccess.Read);
		}

		internal static uint WriteUnfilteredResponse(HttpContext httpContext, Stream stream, string fileName, bool isNotHtmlandNotXml, bool doNotSniff)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			uint num = 0U;
			byte[] array = new byte[512];
			int num2 = stream.Read(array, 0, 512);
			if (num2 == 0)
			{
				return 0U;
			}
			try
			{
				if (!doNotSniff && isNotHtmlandNotXml && AttachmentUtility.CheckShouldRemoveContents(array, num2))
				{
					if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>(0L, "AttachmentUtility.WriteUnfilteredResponse: Return contents removed for attachment {0}: mailbox {1}", fileName, AttachmentUtility.TryGetMailboxIdentityName(httpContext));
					}
					num = AttachmentUtility.WriteContentsRemoved(httpContext.Response.OutputStream);
				}
				else
				{
					if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>(0L, "AttachmentUtility.WriteUnfilteredResponse: Skip data sniff for attachment {0}: mailbox {1}", fileName, AttachmentUtility.TryGetMailboxIdentityName(httpContext));
					}
					httpContext.Response.OutputStream.Write(array, 0, num2);
					num = AttachmentUtility.WriteOutputStream(httpContext.Response.OutputStream, stream);
					num += (uint)num2;
				}
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
			return num;
		}

		internal static bool CheckShouldRemoveContents(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new OwaInvalidInputException("Stream is required to support Seek operations, and does not");
			}
			byte[] array = new byte[512];
			int bytesToRead = stream.Read(array, 0, 512);
			bool result = AttachmentUtility.CheckShouldRemoveContents(array, bytesToRead);
			stream.Seek(0L, SeekOrigin.Begin);
			return result;
		}

		private static bool CheckShouldRemoveContents(byte[] bytesToSniff, int bytesToRead)
		{
			bool result;
			using (MemoryStream memoryStream = new MemoryStream(bytesToSniff, 0, bytesToRead))
			{
				DataSniff dataSniff = new DataSniff(256);
				string x = dataSniff.FindMimeFromData(memoryStream);
				result = (StringComparer.OrdinalIgnoreCase.Compare(x, "text/xml") == 0 || 0 == StringComparer.OrdinalIgnoreCase.Compare(x, "text/html"));
			}
			return result;
		}

		private static uint WriteContentsRemoved(Stream outputStream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(LocalizedStrings.GetNonEncoded(-1868113279));
			outputStream.Write(bytes, 0, bytes.Length);
			return (uint)bytes.Length;
		}

		internal static uint WriteOutputStream(Stream outputStream, Stream inputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			uint num = 0U;
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(AttachmentUtility.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			try
			{
				int num2;
				while ((num2 = inputStream.Read(array, 0, array.Length)) > 0)
				{
					outputStream.Write(array, 0, num2);
					num += (uint)num2;
				}
			}
			finally
			{
				if (array != null)
				{
					bufferPool.Release(array);
				}
			}
			return num;
		}

		internal static uint[] CompressAndWriteOutputStream(Stream outputStream, Stream inputStream, bool doComputeCrc)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			uint num = 0U;
			int num2 = 0;
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(AttachmentUtility.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			uint num3 = 0U;
			using (Stream stream = Streams.CreateTemporaryStorageStream())
			{
				try
				{
					int num4;
					using (Stream stream2 = new DeflateStream(stream, CompressionMode.Compress, true))
					{
						while ((num4 = inputStream.Read(array, 0, array.Length)) > 0)
						{
							if (doComputeCrc)
							{
								num3 = AttachmentUtility.ComputeCrc32FromBytes(array, num4, num3);
							}
							num2 += num4;
							stream2.Write(array, 0, num4);
						}
						stream2.Flush();
					}
					stream.Seek(0L, SeekOrigin.Begin);
					while ((num4 = stream.Read(array, 0, array.Length)) > 0)
					{
						outputStream.Write(array, 0, num4);
						num += (uint)num4;
					}
				}
				finally
				{
					if (array != null)
					{
						bufferPool.Release(array);
					}
				}
			}
			return new uint[]
			{
				num,
				num3,
				(uint)num2
			};
		}

		internal static uint ComputeCrc32FromStream(Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new OwaInvalidInputException("Stream is required to support Seek operations, and does not");
			}
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(AttachmentUtility.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			uint num = 0U;
			try
			{
				int bytesToRead;
				while ((bytesToRead = stream.Read(array, 0, array.Length)) > 0)
				{
					num = AttachmentUtility.ComputeCrc32FromBytes(array, bytesToRead, num);
				}
			}
			finally
			{
				if (array != null)
				{
					bufferPool.Release(array);
				}
			}
			stream.Seek(0L, SeekOrigin.Begin);
			return num;
		}

		internal static uint ComputeCrc32FromBytes(byte[] data, int bytesToRead, uint seed)
		{
			uint num = seed ^ uint.MaxValue;
			for (int i = 0; i < bytesToRead; i++)
			{
				num = (AttachmentUtility.CrcTable[(int)((UIntPtr)((num ^ (uint)data[i]) & 255U))] ^ num >> 8);
			}
			return num ^ uint.MaxValue;
		}

		internal static string CalculateAttachmentName(string displayName, string filename)
		{
			if (!string.IsNullOrEmpty(displayName))
			{
				return displayName;
			}
			if (!string.IsNullOrEmpty(filename))
			{
				return filename;
			}
			return Strings.UntitledAttachment;
		}

		internal static int GetEmbeddedItemNestingLevel(HttpRequest request)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "attcnt");
			int num;
			if (!int.TryParse(queryStringParameter, out num))
			{
				throw new OwaInvalidRequestException("Invalid attachment count querystring parameter");
			}
			if (num > AttachmentPolicy.MaxEmbeddedDepth)
			{
				throw new OwaInvalidRequestException("Accessing embedded attachment beyond maximum embedded depth");
			}
			return num;
		}

		internal static void RenderDownloadAllAttachmentsLink(SanitizingTextWriter<OwaHtml> sanitizingWriter, HttpRequest request, string urlEncodedItemId, bool isEmbeddedItemInNonSMimeItem, UserContext userContext, int count)
		{
			sanitizingWriter.Write("<div class=\"roWellWrap attZip\"><span class=\"fltBefore spnZipAtts\">");
			sanitizingWriter.Write(userContext.DirectionMark);
			sanitizingWriter.Write(LocalizedStrings.GetHtmlEncoded(6409762));
			sanitizingWriter.Write(count);
			sanitizingWriter.Write(LocalizedStrings.GetHtmlEncoded(-1023695022));
			sanitizingWriter.Write(userContext.DirectionMark);
			sanitizingWriter.Write("</span><a id=\"lnkZipAtts\" class=\"tbfz fltBefore tbfHz\" href=\"");
			AttachmentUtility.RenderDownloadAllAttachmentsLinkUrl(sanitizingWriter, request, urlEncodedItemId, isEmbeddedItemInNonSMimeItem);
			sanitizingWriter.Write("\" target=_self name=\"lnkZipAtts\">");
			sanitizingWriter.Write(SanitizedHtmlString.FromStringId(-792355597));
			sanitizingWriter.Write("</a></div>");
		}

		private static void RenderDownloadAllAttachmentsLinkUrl(SanitizingTextWriter<OwaHtml> sanitizingWriter, HttpRequest request, string urlEncodedItemId, bool isEmbeddedItemInNonSMimeItem)
		{
			sanitizingWriter.Write("attachment.ashx?id=");
			sanitizingWriter.Write(urlEncodedItemId);
			if (isEmbeddedItemInNonSMimeItem)
			{
				int embeddedItemNestingLevel = AttachmentUtility.GetEmbeddedItemNestingLevel(request);
				for (int i = 0; i < embeddedItemNestingLevel; i++)
				{
					string text = "attid" + i.ToString(CultureInfo.InvariantCulture);
					string queryStringParameter = Utilities.GetQueryStringParameter(request, text);
					sanitizingWriter.Write("&");
					sanitizingWriter.Write(text);
					sanitizingWriter.Write("=");
					sanitizingWriter.Write(queryStringParameter);
				}
				sanitizingWriter.Write("&attcnt=");
				sanitizingWriter.Write(embeddedItemNestingLevel);
			}
			sanitizingWriter.Write("&dla=1");
			if (isEmbeddedItemInNonSMimeItem)
			{
				sanitizingWriter.Write("&femb=1");
			}
		}

		internal static Item GetEmbeddedItem(Item parentItem, HttpRequest request, UserContext userContext)
		{
			if (parentItem == null)
			{
				throw new ArgumentNullException("parentItem");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Item result = null;
			using (Attachment attachment = Utilities.GetAttachment(parentItem, request, userContext))
			{
				using (ItemAttachment itemAttachment = attachment as ItemAttachment)
				{
					if (itemAttachment == null)
					{
						return null;
					}
					result = itemAttachment.GetItem(null);
				}
			}
			return result;
		}

		private static AttachmentWellInfo CreateAttachmentInfoObject(AttachmentCollection collection, Attachment attachment, bool isLoggedOnFromPublicComputer, bool isJunkOrPhishing)
		{
			return new AttachmentWellInfo(collection, attachment, isJunkOrPhishing);
		}

		private static AttachmentWellInfo CreateAttachmentInfoObject(AttachmentCollection collection, AttachmentLink attachmentLink, bool isLoggedOnFromPublicComputer, bool isJunkOrPhishing)
		{
			return new AttachmentWellInfo(collection, attachmentLink, isJunkOrPhishing);
		}

		private static AttachmentWellInfo CreateAttachmentInfoObject(OwaStoreObjectId owaConversationId, AttachmentInfo attachmentInfo, bool isLoggedOnFromPublicComputer, bool isJunkOrPhishing)
		{
			return new AttachmentWellInfo(owaConversationId, attachmentInfo, isJunkOrPhishing);
		}

		public static bool IsSupportedImageContentType(HttpPostedFile file)
		{
			string contentType = file.ContentType;
			return !string.IsNullOrEmpty(contentType) && (contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/pjpeg", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/bmp", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/x-png", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsOutLine(ArrayList attachmentList)
		{
			if (attachmentList == null)
			{
				return false;
			}
			for (int i = 0; i < attachmentList.Count; i++)
			{
				AttachmentWellInfo attachmentWellInfo = attachmentList[i] as AttachmentWellInfo;
				if (attachmentWellInfo.AttachmentType != AttachmentType.Ole && !attachmentWellInfo.IsInline)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsLevelOneAndBlock(ArrayList attachmentList)
		{
			if (attachmentList == null)
			{
				return false;
			}
			int count = attachmentList.Count;
			if (count == 0)
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)attachmentList[i];
				if (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.Block && !AttachmentUtility.IsWebReadyDocument(attachmentWellInfo.FileExtension, attachmentWellInfo.MimeType))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsLevelOneOnly(ArrayList attachmentList)
		{
			if (attachmentList == null)
			{
				return true;
			}
			foreach (object obj in attachmentList)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				if (attachmentWellInfo.AttachmentLevel != AttachmentPolicy.Level.Block)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsLevelOneAndBlockOnly(ArrayList attachmentList)
		{
			if (attachmentList == null)
			{
				return true;
			}
			foreach (object obj in attachmentList)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				if (attachmentWellInfo.AttachmentLevel != AttachmentPolicy.Level.Block || AttachmentUtility.IsWebReadyDocument(attachmentWellInfo.FileExtension, attachmentWellInfo.MimeType))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsWebReadyDocument(string fileExtension, string mimeType)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			AttachmentPolicy attachmentPolicy = userContext.AttachmentPolicy;
			if (!attachmentPolicy.WebReadyDocumentViewingEnable)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(fileExtension))
			{
				if ((attachmentPolicy.WebReadyDocumentViewingForAllSupportedTypes || attachmentPolicy.Contains(fileExtension, AttachmentPolicy.LookupTypeSignifer.FileArray)) && attachmentPolicy.Contains(fileExtension, AttachmentPolicy.LookupTypeSignifer.SupportedFileArray))
				{
					ExTraceGlobals.TranscodingTracer.TraceDebug<string>(0L, "File extension: {0} passes.", fileExtension);
					return true;
				}
			}
			else if (!string.IsNullOrEmpty(mimeType) && (attachmentPolicy.WebReadyDocumentViewingForAllSupportedTypes || attachmentPolicy.Contains(mimeType, AttachmentPolicy.LookupTypeSignifer.MimeArray)) && attachmentPolicy.Contains(mimeType, AttachmentPolicy.LookupTypeSignifer.SupportedMimeArray))
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>(0L, "Mime type: {0} passes.", mimeType);
				return true;
			}
			return false;
		}

		public static AttachmentUtility.AttachmentLinkFlags GetAttachmentLinkFlag(AttachmentWellType wellType, AttachmentWellInfo attachmentInfoObject)
		{
			if (attachmentInfoObject == null)
			{
				throw new ArgumentNullException("attachmentInfoObject");
			}
			AttachmentUtility.AttachmentLinkFlags attachmentLinkFlags = AttachmentUtility.AttachmentLinkFlags.None;
			UserContext userContext = UserContextManager.GetUserContext();
			AttachmentPolicy attachmentPolicy = userContext.AttachmentPolicy;
			int embeddedDepth = Utilities.GetEmbeddedDepth(HttpContext.Current.Request);
			bool flag = AttachmentUtility.IsWebReadyDocument(attachmentInfoObject.FileExtension, attachmentInfoObject.MimeType);
			if (wellType == AttachmentWellType.ReadOnly && attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.Block && !flag)
			{
				return AttachmentUtility.AttachmentLinkFlags.Skip;
			}
			if (embeddedDepth < AttachmentPolicy.MaxEmbeddedDepth && (attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.ForceSave || attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.Allow) && (!attachmentPolicy.ForceWebReadyDocumentViewingFirst || !flag))
			{
				attachmentLinkFlags |= AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink;
			}
			if (flag && embeddedDepth < AttachmentPolicy.MaxEmbeddedDepth)
			{
				attachmentLinkFlags |= AttachmentUtility.AttachmentLinkFlags.OpenAsWebPageLink;
			}
			return attachmentLinkFlags;
		}

		public static string GetEmbeddedAttachmentDisplayName(Item item)
		{
			string className = item.ClassName;
			string text;
			if (ObjectClass.IsContact(className) || ObjectClass.IsDistributionList(className))
			{
				text = ItemUtility.GetProperty<string>(item, StoreObjectSchema.DisplayName, string.Empty);
			}
			else
			{
				text = ItemUtility.GetProperty<string>(item, ItemSchema.Subject, string.Empty);
			}
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
			{
				text = LocalizedStrings.GetNonEncoded(1797976510);
			}
			return text;
		}

		public static string TrimAttachmentDisplayName(string attachmentDisplayName, ArrayList previousAttachmentDisplayNames, bool isEmbeddedItem)
		{
			if (attachmentDisplayName == null)
			{
				throw new ArgumentNullException("attachmentDisplayName");
			}
			if (attachmentDisplayName.Length <= 32)
			{
				return attachmentDisplayName;
			}
			string text = "~";
			int num = 1;
			int num2 = attachmentDisplayName.LastIndexOf('.');
			string text2;
			string text3;
			if (num2 > 0)
			{
				text2 = attachmentDisplayName.Substring(num2);
				text3 = attachmentDisplayName.Substring(0, num2);
			}
			else
			{
				text3 = attachmentDisplayName;
				text2 = string.Empty;
			}
			int num3;
			if (isEmbeddedItem)
			{
				num3 = 29;
			}
			else if (previousAttachmentDisplayNames != null)
			{
				num3 = 32 - text2.Length - 2;
			}
			else
			{
				num3 = 32 - text2.Length - 3;
			}
			if (num3 > 0)
			{
				num3 = Math.Min(num3, text3.Length);
				text3 = text3.Substring(0, num3);
			}
			else
			{
				text3 = string.Empty;
			}
			if (isEmbeddedItem)
			{
				attachmentDisplayName = text3 + "...";
			}
			else if (previousAttachmentDisplayNames != null)
			{
				for (int i = 0; i < previousAttachmentDisplayNames.Count; i++)
				{
					if (previousAttachmentDisplayNames[i].Equals(text3))
					{
						num++;
					}
				}
				text += num.ToString();
				attachmentDisplayName = text3 + text + text2;
				previousAttachmentDisplayNames.Add(text3);
			}
			else
			{
				attachmentDisplayName = text3 + "..." + text2;
			}
			return attachmentDisplayName;
		}

		public static SanitizedHtmlString GetInlineAttachmentLink(Attachment attachment, Item item)
		{
			string format = "attachment.ashx?id={0}&attcnt=1&attid0={1}&attcid0={2}";
			return SanitizedHtmlString.Format(format, new object[]
			{
				Utilities.UrlEncode(Utilities.GetIdAsString(item)),
				Utilities.UrlEncode(attachment.Id.ToBase64String()),
				Utilities.UrlEncode(attachment.ContentId)
			});
		}

		public static bool PromoteInlineAttachments(Item item)
		{
			bool flag = false;
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, OwaContext.Current.UserContext);
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if (attachment.IsInline || attachment.RenderingPosition != -1)
					{
						attachment.IsInline = false;
						attachment.RenderingPosition = -1;
						attachment.Save();
						flag = true;
					}
				}
			}
			if (flag)
			{
				Utilities.SaveItem(item, false);
			}
			return flag;
		}

		public static bool VerifyInlineAttachmentUrlValidity(string imageUrlEncoded, Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (string.IsNullOrEmpty(imageUrlEncoded))
			{
				return false;
			}
			string text = HttpUtility.UrlDecode(imageUrlEncoded);
			int num = imageUrlEncoded.IndexOf("attachment.ashx?id=", StringComparison.Ordinal);
			num += "attachment.ashx?id=".Length;
			if (num >= imageUrlEncoded.Length)
			{
				return false;
			}
			int num2 = imageUrlEncoded.IndexOf('&', num);
			if (num2 == -1 || num2 < num)
			{
				return false;
			}
			string str = imageUrlEncoded.Substring(num, num2 - num);
			if (string.CompareOrdinal(Utilities.GetIdAsString(item), HttpUtility.UrlDecode(str)) != 0)
			{
				return false;
			}
			int num3 = text.IndexOf("&attid0=", StringComparison.Ordinal);
			return num3 != -1 && num3 < text.Length;
		}

		public static string ParseInlineAttachmentContentId(string imageUrl, out int contentIdIndex)
		{
			if (imageUrl == null)
			{
				throw new ArgumentNullException("imageUrl");
			}
			contentIdIndex = -1;
			int num = imageUrl.IndexOf("&attid0=", StringComparison.Ordinal);
			if (num != -1 && num < imageUrl.Length)
			{
				string empty = string.Empty;
				contentIdIndex = imageUrl.IndexOf("&attcid0=", num);
				if (contentIdIndex != -1 && contentIdIndex < imageUrl.Length)
				{
					return imageUrl.Substring(contentIdIndex + "&attcid0=".Length);
				}
			}
			return null;
		}

		public static string ParseInlineAttachmentIdString(string imageUrl, int contentIdIndex)
		{
			if (imageUrl == null)
			{
				throw new ArgumentNullException("imageUrl");
			}
			int num = imageUrl.IndexOf("&attid0=", StringComparison.Ordinal);
			int num2 = num + "&attid0=".Length;
			string result;
			if (contentIdIndex != -1)
			{
				result = imageUrl.Substring(num2, contentIdIndex - num2);
			}
			else
			{
				result = imageUrl.Substring(num2);
			}
			return result;
		}

		public static AttachmentLink GetAttachmentLink(string attachmentIdString, string contentId, Item item, ConversionCallbackBase converstionCallback)
		{
			if (attachmentIdString == null)
			{
				throw new ArgumentNullException("attachmentIdString");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (converstionCallback == null)
			{
				throw new ArgumentNullException("converstionCallback");
			}
			AttachmentId attachmentId = null;
			try
			{
				attachmentId = item.CreateAttachmentId(attachmentIdString);
			}
			catch (CorruptDataException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "OwaSafeHtmlInboundCallbacks - failed to decipher attachment for URL = ({0})", attachmentIdString);
				return null;
			}
			return converstionCallback.FindAttachmentByIdOrContentId(attachmentId, contentId);
		}

		public static string GetOrGenerateAttachContentId(AttachmentLink attachmentLink)
		{
			if (attachmentLink == null)
			{
				throw new ArgumentNullException("attachmentLink");
			}
			if (attachmentLink.AttachmentType == AttachmentType.Ole)
			{
				attachmentLink.ConvertToImage();
			}
			if (string.IsNullOrEmpty(attachmentLink.ContentId))
			{
				attachmentLink.ContentId = Guid.NewGuid().ToString();
			}
			attachmentLink.MarkInline(true);
			return attachmentLink.ContentId;
		}

		public static bool ApplyAttachmentsUpdates(Item item, ConversionCallbackBase converstionCallback)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (converstionCallback == null)
			{
				throw new ArgumentNullException("converstionCallback");
			}
			bool flag = false;
			item.OpenAsReadWrite();
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			if (calendarItemBase != null)
			{
				Utilities.ValidateCalendarItemBaseStoreObject(calendarItemBase);
			}
			try
			{
				flag = converstionCallback.SaveChanges();
				flag = true;
			}
			catch (AccessDeniedException)
			{
			}
			if (flag)
			{
				try
				{
					Utilities.SaveItem(item, false);
				}
				catch (AccessDeniedException)
				{
				}
			}
			return flag;
		}

		public static void RemoveSmimeAttachment(ArrayList attachmentWellRenderObjects)
		{
			if (attachmentWellRenderObjects == null)
			{
				throw new ArgumentNullException("attachmentWellRenderObjects");
			}
			if (attachmentWellRenderObjects.Count == 0)
			{
				throw new ArgumentException("attachmentWellRenderObjects is empty.");
			}
			for (int i = 0; i < attachmentWellRenderObjects.Count; i++)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)attachmentWellRenderObjects[i];
				if (string.Equals(attachmentWellInfo.FileName, "smime.p7m", StringComparison.OrdinalIgnoreCase) || string.Equals(attachmentWellInfo.MimeType, "multipart/signed", StringComparison.OrdinalIgnoreCase))
				{
					attachmentWellRenderObjects.RemoveAt(i);
					return;
				}
			}
		}

		public static int GetTotalAttachmentSize(AttachmentCollection attachmentCollection)
		{
			int num = 0;
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					num += (int)attachment.Size;
				}
			}
			return num;
		}

		public static string TryGetMailboxIdentityName(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			string result = string.Empty;
			if (OwaContext.Get(httpContext).MailboxIdentity != null)
			{
				result = OwaContext.Get(httpContext).MailboxIdentity.SafeGetRenderableName();
			}
			return result;
		}

		private static int GetMaximumFileSize(UserContext userContext)
		{
			int? maximumMessageSize = Utilities.GetMaximumMessageSize(userContext);
			if (maximumMessageSize == null)
			{
				return 5;
			}
			return maximumMessageSize.Value / 1024;
		}

		private static int ToByteSize(int megaByteSize)
		{
			return megaByteSize * 1048576;
		}

		private const string DefaultSmimeAttachmentName = "smime.p7m";

		private const string MultiPartSigned = "multipart/signed";

		private const string ContentDispositionHeader = "Content-Disposition";

		public const string AttachmentPrefix = "&attid0=";

		public const string AttachmentContentIdPrefix = "&attcid0=";

		public const string AttachmentBaseUrl = "attachment.ashx?id=";

		public const int AttachmentCopyBufferSize = 32768;

		public const int DefaultMaxFileSize = 5;

		public const int MaxAttachments = 499;

		public const int EstimatedMessageOverheadExcludingAttachments = 65536;

		public const string AuthoritativeTrueHeader = "; authoritative=true;";

		public const string ContentType = "Content-Type";

		public const string XDownloadOptions = "X-Download-Options";

		public const string XDownloadOptionsNoOpen = "noopen";

		private const int DataSniffByteCount = 512;

		public static uint[] CrcTable = AttachmentUtility.GenerateCrc32Table();

		private static char[] directorySeparatorCharacters = new char[]
		{
			'\\',
			'/'
		};

		public static BufferPoolCollection.BufferSize CopyBufferSize = BufferPoolCollection.BufferSize.Size2K;

		[Flags]
		public enum AttachmentLinkFlags
		{
			None = 0,
			Skip = 1,
			OpenAsWebPageLink = 2,
			AttachmentClickLink = 4
		}
	}
}
