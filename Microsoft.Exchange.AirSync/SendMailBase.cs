using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class SendMailBase : Command
	{
		public SendMailBase()
		{
			this.sendAsManager = new SendAsManager();
		}

		internal static string DefaultDomain
		{
			get
			{
				string text = null;
				OrganizationId currentOrganizationId = Command.CurrentOrganizationId;
				if (SendMailBase.defaultDomainTable.TryGetValue(currentOrganizationId, out text))
				{
					return text;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, null, "Trying to access AD for DefaulDomain info...");
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(currentOrganizationId), 148, "DefaultDomain", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\SendMailBase.cs");
				ADPagedReader<EmailAddressPolicy> adpagedReader = tenantOrTopologyConfigurationSession.FindAllPaged<EmailAddressPolicy>();
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrTopologyConfigurationSession.LastUsedDc);
				foreach (EmailAddressPolicy emailAddressPolicy in adpagedReader)
				{
					if (emailAddressPolicy.Priority == EmailAddressPolicyPriority.Lowest)
					{
						text = emailAddressPolicy.EnabledPrimarySMTPAddressTemplate;
						int num = text.IndexOf('@');
						if (num > 0)
						{
							text = text.Substring(num);
						}
					}
				}
				SendMailBase.defaultDomainTable.Add(currentOrganizationId, text);
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, null, "DefaultDomain '{0}' has already been gotten.", text);
				return text;
			}
		}

		internal string CollectionId
		{
			get
			{
				if (base.Version < 140)
				{
					string legacyUrlParameter = base.Request.GetLegacyUrlParameter("CollectionId");
					if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CollectionIdInvalid");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, null, false);
					}
					return legacyUrlParameter;
				}
				else
				{
					XmlElement xmlElement = base.XmlRequest["Source"];
					if (xmlElement == null)
					{
						return null;
					}
					xmlElement = xmlElement["FolderId"];
					if (xmlElement == null)
					{
						return null;
					}
					return xmlElement.InnerText;
				}
			}
		}

		internal string ItemId
		{
			get
			{
				if (base.Version < 140)
				{
					string legacyUrlParameter = base.Request.GetLegacyUrlParameter("ItemId");
					if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemIdInvalid");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, null, false);
					}
					return legacyUrlParameter;
				}
				else
				{
					XmlElement xmlElement = base.XmlRequest["Source"];
					if (xmlElement == null)
					{
						return null;
					}
					xmlElement = xmlElement["ItemId"];
					if (xmlElement == null)
					{
						return null;
					}
					return xmlElement.InnerText;
				}
			}
		}

		internal string LongId
		{
			get
			{
				if (base.Version < 140)
				{
					string legacyUrlParameter = base.Request.GetLegacyUrlParameter("LongId");
					if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LongIdInvalid");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, null, false);
					}
					return legacyUrlParameter;
				}
				else
				{
					XmlElement xmlElement = base.XmlRequest["Source"];
					if (xmlElement == null)
					{
						return null;
					}
					xmlElement = xmlElement["LongId"];
					if (xmlElement == null)
					{
						return null;
					}
					return HttpUtility.UrlDecode(xmlElement.InnerText);
				}
			}
		}

		internal override bool ShouldSaveSyncStatus
		{
			get
			{
				return base.Request.Version >= 140;
			}
		}

		protected internal bool SaveInSentItems
		{
			get
			{
				if (base.Version < 140)
				{
					return string.Compare(base.Request.GetLegacyUrlParameter("SaveInSent"), "T", StringComparison.OrdinalIgnoreCase) == 0;
				}
				return base.XmlRequest["SaveInSentItems"] != null;
			}
		}

		protected internal bool ReplaceMime
		{
			get
			{
				return base.Version >= 140 && base.XmlRequest["ReplaceMime"] != null;
			}
		}

		protected internal ExDateTime Occurrence
		{
			get
			{
				if (base.Version < 140)
				{
					string legacyUrlParameter = base.Request.GetLegacyUrlParameter("Occurrence");
					if (legacyUrlParameter == null)
					{
						return ExDateTime.MinValue;
					}
					ExDateTime result;
					if (!ExDateTime.TryParseExact(legacyUrlParameter, "yyyyMMddHHmm", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidDateTimeFormat");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false);
					}
					return result;
				}
				else
				{
					XmlElement xmlElement = base.XmlRequest["Source"];
					if (xmlElement == null)
					{
						return ExDateTime.MinValue;
					}
					xmlElement = xmlElement["InstanceId"];
					if (xmlElement == null)
					{
						return ExDateTime.MinValue;
					}
					if (base.Version >= 160)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "OccurrenceNotSupported");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false);
					}
					string innerText = xmlElement.InnerText;
					ExDateTime result2;
					if (!ExDateTime.TryParseExact(innerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result2))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidDateTimeFormat2");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false);
					}
					return result2;
				}
			}
		}

		protected internal string AccountId
		{
			get
			{
				if (base.Version < 141)
				{
					return null;
				}
				XmlElement xmlElement = base.XmlRequest["AccountId"];
				if (xmlElement == null)
				{
					return null;
				}
				return xmlElement.InnerText;
			}
		}

		private string TemplateID
		{
			get
			{
				if (base.Version < 140)
				{
					return null;
				}
				XmlElement xmlElement = base.XmlRequest["TemplateID"];
				if (xmlElement != null)
				{
					return xmlElement.InnerText;
				}
				return null;
			}
		}

		protected void CopyMessageContents(MessageItem source, MessageItem destination, bool copyOriginalRecipients, Item attachmentItem)
		{
			source.Load();
			string inReplyTo = destination.InReplyTo;
			string references = destination.References;
			destination.Subject = ((source.TryGetProperty(ItemSchema.SubjectPrefix) as string) ?? string.Empty) + ((source.TryGetProperty(ItemSchema.NormalizedSubject) as string) ?? string.Empty);
			destination.InReplyTo = inReplyTo;
			destination.Importance = source.Importance;
			destination.InternetMessageId = source.InternetMessageId;
			destination.Categories.Clear();
			destination.Categories.AddRange(source.Categories);
			destination.IconIndex = source.IconIndex;
			destination.IsRead = source.IsRead;
			destination.IsReadReceiptRequested = source.IsReadReceiptRequested;
			destination.References = references;
			if (copyOriginalRecipients)
			{
				this.originalRecipients = new HashSet<RecipientId>();
				foreach (Recipient recipient in destination.Recipients)
				{
					this.originalRecipients.Add(recipient.Id);
				}
			}
			destination.Recipients.Clear();
			foreach (Recipient recipient2 in source.Recipients)
			{
				destination.Recipients.Add(recipient2.Participant, recipient2.RecipientItemType);
			}
			if (source.ReplyTo != null)
			{
				foreach (Participant item in source.ReplyTo)
				{
					destination.ReplyTo.Add(item);
				}
			}
			RightsManagedMessageItem rightsManagedMessageItem = destination as RightsManagedMessageItem;
			AttachmentCollection attachmentCollection = (rightsManagedMessageItem != null) ? rightsManagedMessageItem.ProtectedAttachmentCollection : destination.AttachmentCollection;
			if (rightsManagedMessageItem == null && attachmentItem != null)
			{
				throw new InvalidOperationException("attachmentItem must be null for non-IRM messages");
			}
			if (this.ReplaceMime || attachmentItem != null)
			{
				if (rightsManagedMessageItem == null)
				{
					Body.CopyBody(source, destination, GlobalSettings.DisableCharsetDetectionInCopyMessageContents);
				}
				else
				{
					using (Stream stream = source.Body.OpenReadStream(new BodyReadConfiguration(source.Body.Format, source.Body.Charset)))
					{
						using (Stream stream2 = rightsManagedMessageItem.ProtectedBody.OpenWriteStream(new BodyWriteConfiguration(source.Body.Format, source.Body.Charset)))
						{
							Util.StreamHandler.CopyStreamData(stream, stream2);
						}
					}
				}
			}
			foreach (AttachmentHandle handle in source.AttachmentCollection)
			{
				using (Attachment attachment = source.AttachmentCollection.Open(handle))
				{
					int num = 0;
					if (attachment is StreamAttachment)
					{
						num = 1;
					}
					else if (attachment is OleAttachment)
					{
						num = 2;
					}
					if (num > 0)
					{
						Microsoft.Exchange.Data.Storage.AttachmentType type = (num == 1) ? Microsoft.Exchange.Data.Storage.AttachmentType.Stream : Microsoft.Exchange.Data.Storage.AttachmentType.Ole;
						StreamAttachmentBase streamAttachmentBase = (StreamAttachmentBase)attachment;
						using (StreamAttachmentBase streamAttachmentBase2 = (StreamAttachmentBase)attachmentCollection.Create(type))
						{
							using (Stream contentStream = streamAttachmentBase2.GetContentStream())
							{
								using (Stream contentStream2 = streamAttachmentBase.GetContentStream())
								{
									int num2 = 4096;
									byte[] buffer = new byte[num2];
									int count;
									while ((count = contentStream2.Read(buffer, 0, num2)) > 0)
									{
										contentStream.Write(buffer, 0, count);
									}
									streamAttachmentBase2.ContentType = streamAttachmentBase.ContentType;
									streamAttachmentBase2[AttachmentSchema.DisplayName] = streamAttachmentBase.DisplayName;
									streamAttachmentBase2.FileName = streamAttachmentBase.FileName;
									streamAttachmentBase2[AttachmentSchema.AttachContentId] = streamAttachmentBase[AttachmentSchema.AttachContentId];
									streamAttachmentBase2.IsInline = streamAttachmentBase.IsInline;
								}
							}
							streamAttachmentBase2.Save();
							continue;
						}
					}
					ItemAttachment itemAttachment = (ItemAttachment)attachment;
					using (Item item2 = itemAttachment.GetItem())
					{
						using (ItemAttachment itemAttachment2 = attachmentCollection.AddExistingItem(item2))
						{
							itemAttachment2.Save();
						}
					}
				}
			}
			if (attachmentItem != null)
			{
				using (ItemAttachment itemAttachment3 = attachmentCollection.AddExistingItem(attachmentItem))
				{
					itemAttachment3.Save();
				}
			}
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			return null;
		}

		internal override bool ValidateXml()
		{
			bool result;
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.SendMailValidateXML))
			{
				if (base.Version < 140)
				{
					result = true;
				}
				else
				{
					bool flag = base.ValidateXml();
					if (!flag)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FailedXsdValidation");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false);
					}
					result = flag;
				}
			}
			return result;
		}

		internal virtual void ParseXmlRequest()
		{
			string collectionId = this.CollectionId;
			string itemId = this.ItemId;
			string longId = this.LongId;
			if (longId == null && itemId == null)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoMessageId");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
			}
			if (longId != null && (itemId != null || collectionId != null))
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "TooManyIds");
				string.Format("Send request isn't valid because it contains a LongId {0} and ItemId {1} or CollectionId {2}", longId, (itemId != null) ? itemId : "<null>", (collectionId != null) ? collectionId : "<null>");
				AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
				throw ex;
			}
			ExDateTime occurrence = this.Occurrence;
		}

		protected void ValidateBody()
		{
			if (base.Version < 140)
			{
				if (string.Compare(base.Request.ContentType, "message/rfc822", StringComparison.OrdinalIgnoreCase) != 0 || base.Request.ContentLength == 0)
				{
					base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ContentIsNotMIME");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.First140Error, null, false);
				}
			}
			else
			{
				XmlElement xmlElement = base.XmlRequest["ClientId"];
				if (xmlElement != null && base.SyncStatusSyncData != null)
				{
					this.currentClientId = xmlElement.InnerText;
					if (base.SyncStatusSyncData.ContainsClientId(this.currentClientId))
					{
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MessagePreviouslySent");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MessagePreviouslySent, null, false);
					}
				}
			}
		}

		protected void ParseMimeToMessage(MessageItem message)
		{
			if (message == null)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemCreationFailed");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed, null, false);
			}
			Stream stream;
			if (base.Version < 140)
			{
				stream = base.InputStream;
			}
			else
			{
				AirSyncBlobXmlNode airSyncBlobXmlNode = base.Request.CommandXml["Mime"] as AirSyncBlobXmlNode;
				stream = airSyncBlobXmlNode.Stream;
				stream.Seek(0L, SeekOrigin.Begin);
			}
			InboundConversionOptions inboundConversionOptions = AirSyncUtility.GetInboundConversionOptions();
			inboundConversionOptions.ClearCategories = false;
			try
			{
				ItemConversion.ConvertAnyMimeToItem(message, stream, inboundConversionOptions);
			}
			catch (ExchangeDataException innerException)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidCharSetError");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidMIME, innerException, false);
			}
			catch (ConversionFailedException innerException2)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SendMailMimeError");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidMIME, innerException2, false);
			}
			message.From = null;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(Command.CurrentOrganizationId), 854, "ParseMimeToMessage", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\SendMailBase.cs");
			TransportConfigContainer transportConfigContainer = tenantOrTopologyConfigurationSession.FindSingletonConfigurationObject<TransportConfigContainer>();
			base.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrTopologyConfigurationSession.LastUsedDc);
			bool flag = transportConfigContainer == null || !transportConfigContainer.OpenDomainRoutingEnabled;
			if (flag && SendMailBase.DefaultDomain != null)
			{
				IList<Recipient> list = new List<Recipient>();
				foreach (Recipient recipient in message.Recipients)
				{
					if (recipient.Participant.RoutingType == null || (recipient.Participant.RoutingType == "SMTP" && recipient.Participant.EmailAddress.IndexOf('@') == -1))
					{
						list.Add(recipient);
					}
				}
				foreach (Recipient recipient2 in list)
				{
					Participant participant = this.ResolveUnresolvedParticipant(recipient2.Participant);
					message.Recipients.Remove(recipient2);
					message.Recipients.Add(participant, recipient2.RecipientItemType);
				}
				IList<Participant> list2 = new List<Participant>();
				foreach (Participant participant2 in message.ReplyTo)
				{
					if (participant2.RoutingType == null || (participant2.RoutingType == "SMTP" && participant2.EmailAddress.IndexOf('@') == -1))
					{
						list2.Add(participant2);
					}
				}
				foreach (Participant participant3 in list2)
				{
					Participant item = this.ResolveUnresolvedParticipant(participant3);
					message.ReplyTo.Remove(participant3);
					message.ReplyTo.Add(item);
				}
			}
			if (message.Recipients.Count == 0)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MessageHasNoRecipient");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MessageHasNoRecipient, null, false);
			}
		}

		protected StoreObjectId GetSmartItemId()
		{
			this.ParseXmlRequest();
			string collectionId = this.CollectionId;
			string itemId = this.ItemId;
			string longId = this.LongId;
			StoreObjectId storeObjectId = null;
			FolderSyncState folderSyncState = null;
			StoreObjectId result;
			try
			{
				if (longId != null)
				{
					if (base.Version < 120)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LongIdSupportedinV12Only");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
					}
					try
					{
						storeObjectId = StoreObjectId.Deserialize(longId);
						goto IL_1F1;
					}
					catch (ArgumentException innerException)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, innerException, false);
					}
					catch (FormatException innerException2)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId2");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, innerException2, false);
					}
					catch (CorruptDataException innerException3)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId3");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, innerException3, false);
					}
				}
				if (collectionId == null)
				{
					StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
					MailboxSyncProviderFactory syncProviderFactory = new MailboxSyncProviderFactory(base.MailboxSession, defaultFolderId);
					folderSyncState = base.SyncStateStorage.GetFolderSyncState(syncProviderFactory);
				}
				else
				{
					SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(collectionId);
					if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SendFromVirtualFolder");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
					}
					folderSyncState = base.SyncStateStorage.GetFolderSyncState(collectionId);
				}
				if (folderSyncState == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SyncStateNotFound");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateNotFound, null, false);
				}
				ItemIdMapping itemIdMapping = (ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping];
				if (itemIdMapping == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemIdMappingNotFound");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemNotFound, null, false);
				}
				MailboxSyncItemId mailboxSyncItemId = itemIdMapping[itemId] as MailboxSyncItemId;
				storeObjectId = ((mailboxSyncItemId == null) ? null : ((StoreObjectId)mailboxSyncItemId.NativeId));
				if (storeObjectId == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemIdMappingNotFound");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemNotFound, null, false);
				}
				folderSyncState.Dispose();
				folderSyncState = null;
				IL_1F1:
				result = storeObjectId;
			}
			finally
			{
				if (folderSyncState != null)
				{
					folderSyncState.Dispose();
				}
			}
			return result;
		}

		protected Item GetSmartItem()
		{
			return this.GetSmartItem(this.GetSmartItemId());
		}

		protected Item GetSmartItem(StoreObjectId smartId)
		{
			Item item = null;
			try
			{
				item = Item.Bind(base.MailboxSession, smartId, null);
			}
			catch (ObjectNotFoundException innerException)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemNotFound");
				string.Format("The message send request isn't valid because the message for ID '{0}' couldn't be found", smartId.ToString());
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemNotFound, innerException, false);
			}
			item.OpenAsReadWrite();
			return item;
		}

		protected void SendMessage(MessageItem sendMessage)
		{
			try
			{
				SendAsError sendAsError = SendAsError.Success;
				if (this.AccountId != null)
				{
					Guid subscriptionGuid;
					if (!GuidHelper.TryParseGuid(this.AccountId, out subscriptionGuid))
					{
						sendAsError = SendAsError.InvalidSubscriptionGuid;
					}
					else
					{
						sendAsError = this.sendAsManager.MarkMessageForSendAs(sendMessage, subscriptionGuid, base.MailboxSession);
					}
				}
				if (this.SaveInSentItems)
				{
					sendMessage.Send();
				}
				else
				{
					sendMessage.SendWithoutSavingMessage();
				}
				if (this.currentClientId != null)
				{
					base.SyncStatusSyncData.AddClientId(this.currentClientId);
				}
				bool flag = this.originalRecipients == null;
				if (this.originalRecipients != null)
				{
					foreach (Recipient recipient in sendMessage.Recipients)
					{
						if (!this.originalRecipients.Contains(recipient.Id))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					base.UpdateRecipientInfoCache(sendMessage.Recipients, this.originalRecipients);
				}
				if (sendAsError != SendAsError.Success)
				{
					if (sendAsError == SendAsError.InvalidSubscriptionGuid)
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Invalid subscription GUID from client {0}!", this.AccountId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidSubscriptionGuid");
						throw new AirSyncPermanentException(StatusCode.InvalidAccountId, false);
					}
					if (sendAsError == SendAsError.SubscriptionDisabledForSendAs)
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "SendAs disabled for GUID from client {0}!", this.AccountId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SendAsDisabled");
						throw new AirSyncPermanentException(StatusCode.AccountSendDisabled, false);
					}
				}
			}
			catch (QuotaExceededException innerException)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SendQuotaExceeded");
				throw new AirSyncPermanentException((HttpStatusCode)507, StatusCode.SendQuotaExceeded, innerException, false);
			}
			catch (StoragePermanentException ex)
			{
				string value = "MailSubmissionException:" + ex.GetType();
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, value);
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed, ex, false);
			}
		}

		protected void DeleteMessage(MessageItem message, DeleteItemFlags deletedItemFlag = DeleteItemFlags.HardDelete)
		{
			message.Load();
			if (message.Id != null)
			{
				AggregateOperationResult aggregateOperationResult = base.MailboxSession.Delete(deletedItemFlag, new StoreId[]
				{
					message.Id.ObjectId
				});
				if (OperationResult.Succeeded != aggregateOperationResult.OperationResult)
				{
					AirSyncDiagnostics.TraceDebug<MessageItem>(ExTraceGlobals.RequestsTracer, this, "Failed to delete {0}", message);
				}
			}
		}

		protected CalendarItemBase GetCalendarItemBaseToReplyOrForward(CalendarItem item)
		{
			ExDateTime occurrence = this.Occurrence;
			CalendarItemBase result = null;
			if (occurrence == ExDateTime.MinValue)
			{
				result = item;
			}
			else
			{
				if (item.Recurrence == null)
				{
					AirSyncDiagnostics.TraceError<string, ExDateTime>(ExTraceGlobals.RequestsTracer, this, "Failed to get occurrence of calendar item with subject: \"{0}\" with Start Time: {1}", item.Subject, occurrence);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoRecurrenceInCalendar");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.NoRecurrenceInCalendar, null, false);
				}
				try
				{
					CalendarItemOccurrence calendarItemOccurrence = item.OpenOccurrenceByOriginalStartTime(occurrence, null);
					if (!item.IsMeeting)
					{
						calendarItemOccurrence.MakeModifiedOccurrence();
					}
					result = calendarItemOccurrence;
				}
				catch (OccurrenceNotFoundException innerException)
				{
					AirSyncDiagnostics.TraceError<string, ExDateTime>(ExTraceGlobals.RequestsTracer, this, "OccurrenceNotFoundException getting occurrence of calendar item with subject: \"{0}\" with Start Time: {1}", item.Subject, occurrence);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoInstanceInCalendar");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.NoRecurrenceInCalendar, innerException, false);
				}
			}
			return result;
		}

		protected MessageItem GetRightsManagedReplyForward(MessageItem smartReply, SendMailBase.IrmAction irmAction, RmsTemplate rmsTemplate)
		{
			if (smartReply == null)
			{
				throw new ArgumentNullException("smartReply");
			}
			if (irmAction != SendMailBase.IrmAction.CreateNewPublishingLicense && irmAction != SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody && irmAction != SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected irmAction value: {0}", new object[]
				{
					irmAction
				}));
			}
			RightsManagedMessageItem rightsManagedMessageItem = smartReply as RightsManagedMessageItem;
			if (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody)
			{
				rightsManagedMessageItem.SetRestriction(rmsTemplate);
			}
			else if (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicense || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
			{
				rightsManagedMessageItem = RightsManagedMessageItem.Create(smartReply, AirSyncUtility.GetOutboundConversionOptions());
				rightsManagedMessageItem.SetRestriction(rmsTemplate);
				rightsManagedMessageItem.Sender = new Participant(base.MailboxSession.MailboxOwner);
			}
			return rightsManagedMessageItem;
		}

		protected SendMailBase.IrmAction GetIrmAction(SendMailBase.GetIrmActionCallback irmActionDelegate, ref Item smartItem, out RmsTemplate rmsTemplate)
		{
			if (irmActionDelegate == null)
			{
				throw new ArgumentNullException("irmActionDelegate");
			}
			if (smartItem == null)
			{
				throw new ArgumentNullException("smartItem");
			}
			SendMailBase.IrmAction irmAction = SendMailBase.IrmAction.None;
			rmsTemplate = null;
			Guid guid;
			if (this.IsIrmOperation(out guid))
			{
				RightsManagedMessageItem rightsManagedMessageItem = smartItem as RightsManagedMessageItem;
				if (rightsManagedMessageItem != null)
				{
					if (!rightsManagedMessageItem.IsRestricted || !rightsManagedMessageItem.CanDecode)
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Either the original IRM message was not properly formatted or it could not be decoded");
						throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, false)
						{
							ErrorStringForProtocolLogger = "smbGiaOperationNotPermitted"
						};
					}
					rightsManagedMessageItem.Decode(AirSyncUtility.GetOutboundConversionOptions(), true);
					irmActionDelegate(rightsManagedMessageItem);
					irmAction = SendMailBase.GetIrmActionForReplyForward(rightsManagedMessageItem.UsageRights, this.ReplaceMime, rightsManagedMessageItem.Restriction.Id == guid);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "GetIrmActionForReplyForward: originalMessageRights={0}; originalTemplate={1}; newTemplate={2}; irmAction={3}", new object[]
					{
						rightsManagedMessageItem.UsageRights,
						rightsManagedMessageItem.Restriction.Id,
						guid,
						irmAction
					});
					if (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicense || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
					{
						smartItem.Dispose();
						smartItem = this.GetSmartItem();
					}
				}
				else
				{
					irmAction = SendMailBase.IrmAction.CreateNewPublishingLicense;
				}
				if (irmAction == SendMailBase.IrmAction.None)
				{
					throw new InvalidOperationException("irmAction should not be None here.");
				}
				if (irmAction != SendMailBase.IrmAction.ReusePublishingLicense && irmAction != SendMailBase.IrmAction.ReusePublishingLicenseInlineOriginalBody && guid != Guid.Empty)
				{
					rmsTemplate = RmsTemplateReaderCache.LookupRmsTemplate(base.User.OrganizationId, guid);
					if (rmsTemplate == null)
					{
						AirSyncDiagnostics.TraceError<Guid>(ExTraceGlobals.RequestsTracer, this, "Template {0} not found in cache", guid);
						throw new AirSyncPermanentException(StatusCode.IRM_InvalidTemplateID, false)
						{
							ErrorStringForProtocolLogger = "smbGiaInvalidTemplateID"
						};
					}
				}
			}
			if ((irmAction == SendMailBase.IrmAction.ReusePublishingLicense && !this.ReplaceMime) || ((irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.ReusePublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage) && this.ReplaceMime))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "irmAction = {0}, ReplaceMime = {1}", new object[]
				{
					irmAction,
					this.ReplaceMime
				}));
			}
			return irmAction;
		}

		protected bool IsIrmOperation(out Guid templateId)
		{
			string templateID = this.TemplateID;
			if (templateID == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "IRM operation not requested by device");
				templateId = Guid.Empty;
				return false;
			}
			AirSyncCounters.NumberOfSendIRMMails.Increment();
			if (!base.User.IrmEnabled || !base.Request.IsSecureConnection)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "IRM feature disabled for user {0}", base.User.DisplayName);
				throw new AirSyncPermanentException(StatusCode.IRM_FeatureDisabled, false)
				{
					ErrorStringForProtocolLogger = "smbIioFeatureDisabled"
				};
			}
			if (!DrmClientUtils.TryParseGuid(templateID, out templateId))
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Invalid template Guid {0}", templateID);
				throw new AirSyncPermanentException(StatusCode.IRM_InvalidTemplateID, false)
				{
					ErrorStringForProtocolLogger = "smbIioInvalidTemplateID"
				};
			}
			return true;
		}

		protected override bool HandleQuarantinedState()
		{
			base.Context.Response.SetErrorResponse(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed);
			return false;
		}

		private Participant ResolveUnresolvedParticipant(Participant participant)
		{
			Participant result;
			if (participant.RoutingType == null && participant.DisplayName != null && participant.EmailAddress == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "DefaultDomain = '{0}', OriginalDisplayName = '{1}', NewEmailAddress = '{2}{3}'", new object[]
				{
					SendMailBase.DefaultDomain,
					participant.DisplayName,
					participant.DisplayName,
					SendMailBase.DefaultDomain
				});
				result = Participant.Parse(participant.DisplayName + SendMailBase.DefaultDomain);
			}
			else
			{
				if (!(participant.RoutingType == "SMTP") || participant.EmailAddress == null || participant.EmailAddress.IndexOf('@') != -1)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CouldNotResolveAllRecipients");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MessageRecipientUnresolved, null, false);
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "DefaultDomain = '{0}', OriginalEmailAddress = '{1}', NewEmailAddress = '{2}{3}'", new object[]
				{
					SendMailBase.DefaultDomain,
					participant.EmailAddress,
					participant.EmailAddress,
					SendMailBase.DefaultDomain
				});
				result = new Participant(participant.DisplayName, participant.EmailAddress + SendMailBase.DefaultDomain, participant.RoutingType);
			}
			return result;
		}

		internal static SendMailBase.IrmAction GetIrmActionForReplyForward(ContentRight originalMessageRights, bool replaceMime, bool sameTemplateId)
		{
			bool flag = originalMessageRights.IsUsageRightGranted(ContentRight.Export);
			bool flag2 = originalMessageRights.IsUsageRightGranted(ContentRight.Edit);
			SendMailBase.IrmAction result;
			if (flag2)
			{
				if (replaceMime)
				{
					result = (sameTemplateId ? SendMailBase.IrmAction.ReusePublishingLicense : SendMailBase.IrmAction.CreateNewPublishingLicense);
				}
				else if (sameTemplateId)
				{
					result = SendMailBase.IrmAction.ReusePublishingLicenseInlineOriginalBody;
				}
				else
				{
					result = (flag ? SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody : SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage);
				}
			}
			else
			{
				result = (replaceMime ? SendMailBase.IrmAction.CreateNewPublishingLicense : SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage);
			}
			return result;
		}

		private static MruDictionaryCache<OrganizationId, string> defaultDomainTable = new MruDictionaryCache<OrganizationId, string>(5, 50000, 2);

		private string currentClientId;

		private HashSet<RecipientId> originalRecipients;

		private SendAsManager sendAsManager;

		protected delegate void GetIrmActionCallback(RightsManagedMessageItem originalRightsManagedItem);

		internal enum IrmAction
		{
			None,
			CreateNewPublishingLicense,
			CreateNewPublishingLicenseInlineOriginalBody,
			ReusePublishingLicense,
			ReusePublishingLicenseInlineOriginalBody,
			CreateNewPublishingLicenseAttachOriginalMessage
		}
	}
}
