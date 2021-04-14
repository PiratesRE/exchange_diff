using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class UnJournalAgent : StoreDriverDeliveryAgent
	{
		public UnJournalAgent()
		{
			base.OnInitializedMessage += this.OnInitializedMessageHandler;
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
			base.OnDeliveredMessage += this.OnDeliveredMessageHandler;
		}

		public void OnInitializedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string arg = args.MailItem.FromAddress.ToString();
			string text = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnInitializedMessageHandler: MessageId {0}, Invoked Sender {1}", text, arg);
			if (this.ShouldProcess(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text) && this.IsSender(args))
			{
				AddressHeader addressHeader = args.MailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-BCC") as AddressHeader;
				UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "OnInitializedMessageHandler: MessageId {0}, GetBccRecipients Sender {1}, BccHeader {2} = ", text, arg, (addressHeader == null) ? string.Empty : addressHeader.ToString());
				this.bccRecipients = this.GetBccRecipients(addressHeader);
			}
		}

		private bool IsLegacyArchiveJournalingEnabled(OrganizationId orgId, DeliverableMailItem mailItem, string messageId)
		{
			string text = mailItem.FromAddress.ToString();
			PerTenantTransportSettings tenantConfig = this.GetTenantConfig(orgId, messageId, text);
			if (tenantConfig != null)
			{
				UnJournalAgent.Tracer.TraceDebug(0L, "IsLegacyArchiveJournalingEnabled: MessageId {0}, Invoked, Sender {1}, Legacy archive journaling setting for organization {2} is set to {3}", new object[]
				{
					messageId,
					text,
					orgId,
					tenantConfig.LegacyArchiveJournalingEnabled
				});
				return tenantConfig.LegacyArchiveJournalingEnabled || tenantConfig.LegacyArchiveLiveJournalingEnabled || tenantConfig.JournalArchivingEnabled;
			}
			UnJournalAgent.Tracer.TraceDebug<string, string, OrganizationId>(0L, "IsLegacyArchiveJournalingEnabled: MessageId {0}, Invoked, Sender {1}, Legacy archive journaling setting for organization {2} not found", messageId, text, orgId);
			return false;
		}

		private PerTenantTransportSettings GetTenantConfig(OrganizationId organizationId, string messageId, string sender)
		{
			PerTenantTransportSettings result;
			if (!Components.Configuration.TryGetTransportSettings(organizationId, out result))
			{
				UnJournalAgent.Tracer.TraceDebug<string, string, OrganizationId>(0L, "GetTenantConfig: MessageId {0}, Invoked, Sender {1}, Unable to retrieve pertenanttransportsettings for organization {2}", messageId, sender, organizationId);
				return null;
			}
			UnJournalAgent.Tracer.TraceDebug<string, string, OrganizationId>(0L, "GetTenantConfig: MessageId {0}, Invoked, Sender {1}, Loaded tenant config: {2}", messageId, sender, organizationId);
			return result;
		}

		private List<MimeRecipient> GetBccRecipients(AddressHeader bccHeader)
		{
			List<MimeRecipient> list = new List<MimeRecipient>();
			if (bccHeader != null)
			{
				foreach (AddressItem addressItem in bccHeader)
				{
					MimeRecipient item = addressItem as MimeRecipient;
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string arg = args.MailItem.FromAddress.ToString();
			string text = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Invoked, Sender {1}", text, arg);
			if (this.ShouldProcess(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text))
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Sender {1}: This message is coming from unjournal routing agent, we need to process it", text, arg);
				if (this.IsSender(args))
				{
					UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Sender {1} : This is a sender, trying to add bcc recipients if needed", text, arg);
					this.AddBccRecipientsToMessage(args, ((StoreDriverDeliveryEventArgsImpl)args).MessageItem);
				}
				MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
				ExDateTime exDateTime;
				if (this.TryGetExpiryDateForMigratedMessage(storeDriverDeliveryEventArgsImpl.MailItem, out exDateTime))
				{
					UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Sender {1}: This is a migrated message , expiry date exists on this message. ExpiryDate = {2}", text, arg, exDateTime.ToString());
					MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
					if (RetentonPolicyTagProcessingAgent.IsRetentionPolicyEnabled(storeDriverDeliveryEventArgsImpl.ADRecipientCache, storeDriverDeliveryEventArgsImpl.MailRecipient.Email))
					{
						messageItem.DeleteProperties(new PropertyDefinition[]
						{
							StoreObjectSchema.PolicyTag,
							StoreObjectSchema.ArchiveTag,
							ItemSchema.RetentionDate,
							ItemSchema.ArchiveDate
						});
						UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Sender {1}: Deleted Retention Policy Guids", text, arg);
					}
					messageItem[ItemSchema.EHAMigrationExpiryDate] = exDateTime;
					UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnCreatedMessageHandler: MessageId {0}, Sender {1}: Stamped migration date", text, arg);
				}
			}
		}

		private void CreateMessageWithBcc(MailboxSession session, MessageItem deliveredMessage, StoreId folderId, StoreDriverDeliveryEventArgs args, bool isRead)
		{
			using (MessageItem messageItem = MessageItem.Create(session, folderId))
			{
				this.AddBccRecipientsToMessage(args, messageItem);
				deliveredMessage.CoreItem.CopyItem(messageItem.CoreItem, CopyPropertiesFlags.Move, CopySubObjects.Copy, new List<NativeStorePropertyDefinition>().ToArray());
				messageItem.CoreItem.Recipients.CopyRecipientsFrom(deliveredMessage.CoreItem.Recipients);
				messageItem.CoreItem.Recipients.Save();
				if (isRead)
				{
					messageItem.MarkAsRead(true, true);
				}
				messageItem[InternalSchema.IsDraft] = false;
				messageItem.Save(SaveMode.NoConflictResolution);
				messageItem.Load();
			}
		}

		private void AddBccRecipientsToMessage(StoreDriverDeliveryEventArgs args, MessageItem messageItem)
		{
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = (ADRecipientCache<TransportMiniRecipient>)args.MailItem.RecipientCache;
			foreach (MimeRecipient mimeRecipient in this.bccRecipients)
			{
				Participant participant = null;
				Result<TransportMiniRecipient> result;
				if (adrecipientCache.TryGetValue(ProxyAddress.Parse(mimeRecipient.Email), out result))
				{
					TransportMiniRecipient data = result.Data;
					if (data != null)
					{
						participant = new Participant(data);
					}
				}
				if (participant == null)
				{
					participant = new Participant(mimeRecipient.DisplayName, mimeRecipient.Email, "SMTP");
				}
				messageItem.Recipients.Add(participant, RecipientItemType.Bcc);
			}
		}

		public void OnDeliveredMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string arg = args.MailItem.FromAddress.ToString();
			string text = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1}", text, arg);
			if (this.ShouldProcess(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text))
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1} : This message is coming from unjournal routing agent, we need to process it.", text, arg);
				MSExchangeStoreDriver.TotalUnjournaledItemsDelivered.Increment();
				MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
				MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
				ExDateTime exDateTime;
				if (this.TryGetOriginalDateForMigratedMessage(args.MailItem, out exDateTime, text))
				{
					UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1} : Found Migration message original received time, update the received time", text, arg);
					messageItem.Load(new PropertyDefinition[]
					{
						InternalSchema.ReceivedTime
					});
					messageItem[InternalSchema.ReceivedTime] = exDateTime;
					if (!this.IsSender(args) || this.IsSenderARecipient(args.MailItem, text))
					{
						messageItem.MarkAsUnread(true);
					}
					messageItem[InternalSchema.IsDraft] = false;
					messageItem.Save(SaveMode.NoConflictResolution);
				}
				if (this.IsSender(args))
				{
					if (this.IsSenderARecipient(args.MailItem, text))
					{
						UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1} : This delivery is for sender which is a recipient too.", text, arg);
						messageItem.Load();
						StoreId sentItemsFolderIdBySource = this.GetSentItemsFolderIdBySource(args.MailItem, text, mailboxSession);
						this.CreateMessageWithBcc(mailboxSession, messageItem, sentItemsFolderIdBySource, args, true);
						MSExchangeStoreDriver.TotalUnjournaledItemsDelivered.Increment();
						if (this.IsSourceEhaMigration(args.MailItem, text))
						{
							this.UpdateEhaMessageMigrationFolderCount(mailboxSession);
						}
					}
					else
					{
						UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1}: This delivery is for sender only.", text, arg);
						if (this.bccRecipients.Count > 0)
						{
							UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1} : Bcc Recipients found, update the bcc information on the message.", text, arg);
							messageItem.Load();
							StoreId sentItemsFolderIdBySource2 = this.GetSentItemsFolderIdBySource(args.MailItem, text, mailboxSession);
							this.CreateMessageWithBcc(mailboxSession, messageItem, sentItemsFolderIdBySource2, args, true);
							mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
							{
								messageItem.Id
							});
						}
						else
						{
							UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnDeliveredMessageHandler: MessageId {0}, Sender {1}  : No bcc information found, marking the message as unread.", text, arg);
							messageItem.MarkAsRead(true, false);
						}
					}
				}
				if (this.IsSourceEhaMigration(args.MailItem, text))
				{
					this.UpdateEhaMessageMigrationFolderCount(mailboxSession);
				}
			}
		}

		private void UpdateEhaMessageMigrationFolderCount(MailboxSession session)
		{
			using (Folder folder = Folder.Create(session, session.GetDefaultFolderId(DefaultFolderType.Configuration), StoreObjectType.Folder, "MigrationFolder", CreateMode.OpenIfExists))
			{
				folder.Save();
				long num = 0L;
				folder.Load(new PropertyDefinition[]
				{
					FolderSchema.EhaMigrationMessageCount
				});
				object obj = folder.TryGetProperty(FolderSchema.EhaMigrationMessageCount);
				if (obj != null && obj is long)
				{
					num = (long)obj;
				}
				num += 1L;
				folder.SetOrDeleteProperty(FolderSchema.EhaMigrationMessageCount, num);
				folder.Save();
			}
		}

		private StoreId GetSentItemsFolderIdBySource(DeliverableMailItem mailItem, string messageId, MailboxSession session)
		{
			if (this.IsSourceEhaMigration(mailItem, messageId))
			{
				return this.GetCreateFolderIdUnderGivenParentFolder("MigrationFolder", DefaultFolderType.SentItems.ToString(), session);
			}
			StoreId storeId = session.GetDefaultFolderId(DefaultFolderType.SentItems);
			if (storeId == null)
			{
				storeId = session.CreateDefaultFolder(DefaultFolderType.SentItems);
			}
			return storeId;
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string arg = args.MailItem.FromAddress.ToString();
			string text = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "OnPromotedMessageHandler: MessageId {0}, Sender {1}", text, arg);
			if (this.IsSender(args) && this.ShouldProcess(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text) && !this.IsSenderARecipient(args.MailItem, text))
			{
				this.ChangeDestinationFolder(DefaultFolderType.SentItems, args);
				return;
			}
			if (this.ShouldProcessWithNdrCheck(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text))
			{
				this.ChangeDestinationFolder(DefaultFolderType.LegacyArchiveJournals, args);
				return;
			}
			if (this.ShouldProcess(storeDriverDeliveryEventArgsImpl.ADRecipientCache.OrganizationId, args.MailItem, text))
			{
				this.ChangeDestinationFolder(DefaultFolderType.Inbox, args);
			}
		}

		private void ChangeDestinationFolder(DefaultFolderType folderType, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string text = args.MailItem.FromAddress.ToString();
			MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
			string text2 = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			bool flag = this.IsSourceEhaMigration(storeDriverDeliveryEventArgsImpl.MailItem, text2);
			UnJournalAgent.Tracer.TraceDebug(0L, "ChangeDestinationFolder: MessageId {0}, Sender {1}, isSourceEha {2}, folderType {3} ", new object[]
			{
				text2,
				text,
				flag.ToString(),
				folderType.ToString()
			});
			if (DefaultFolderType.LegacyArchiveJournals == folderType || !flag)
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "ChangeDestinationFolder: MessageId {0}, Sender {1}, Source is not EHA or this is directed to legacyarchivejournals folder in journalndr mailbox", text2, text);
				StoreId storeId = mailboxSession.GetDefaultFolderId(folderType);
				if (storeId == null)
				{
					storeId = mailboxSession.CreateDefaultFolder(folderType);
				}
				if (storeId != null)
				{
					storeDriverDeliveryEventArgsImpl.DeliverToFolder = storeId;
					return;
				}
			}
			else
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "ChangeDestinationFolder: MessageId {0}, Sender {1}, This is a migration message from EHA, put it in the hidden folder", text2, text);
				storeDriverDeliveryEventArgsImpl.DeliverToFolder = this.GetCreateFolderIdUnderGivenParentFolder("MigrationFolder", folderType.ToString(), mailboxSession);
			}
		}

		private StoreId GetCreateFolderIdUnderGivenParentFolder(string parentFolderName, string childFolderName, MailboxSession session)
		{
			StoreId result = null;
			using (Folder folder = Folder.Create(session, session.GetDefaultFolderId(DefaultFolderType.Configuration), StoreObjectType.Folder, parentFolderName, CreateMode.OpenIfExists))
			{
				folder.Save();
				folder.Load(new PropertyDefinition[]
				{
					FolderSchema.Id
				});
				using (Folder folder2 = Folder.Create(session, folder.Id, StoreObjectType.Folder, childFolderName, CreateMode.OpenIfExists))
				{
					folder2.Save();
					folder2.Load(new PropertyDefinition[]
					{
						FolderSchema.Id
					});
					result = folder2.Id;
				}
			}
			return result;
		}

		private bool CheckIfMigrationWasNewlyCreated(Folder migrationFolder)
		{
			return !migrationFolder.HasSubfolders;
		}

		private void StampMailboxForELCEHAProcessing(MailboxSession session)
		{
			string clientInfoString = "Client=TimeBased;Action=TransportEHAMailboxStamping";
			using (MailboxSession mailboxSession = MailboxSession.OpenAsTransport(session.MailboxOwner, clientInfoString))
			{
				mailboxSession.MarkAsEhaMailbox();
			}
		}

		private bool TryGetExpiryDateForMigratedMessage(DeliverableMailItem mailItem, out ExDateTime migrationExpiryDate)
		{
			migrationExpiryDate = ExDateTime.MaxValue;
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.MessageExpiryDate);
			if (header != null)
			{
				string value = header.Value;
				if (!string.IsNullOrEmpty(value))
				{
					return ExDateTime.TryParse(value, out migrationExpiryDate);
				}
			}
			return false;
		}

		private void RemoveBcc(MessageItem messageItem)
		{
			List<RecipientId> list = new List<RecipientId>();
			foreach (Recipient recipient in messageItem.Recipients)
			{
				if (recipient.RecipientItemType == RecipientItemType.Bcc)
				{
					list.Add(recipient.Id);
				}
			}
			foreach (RecipientId id in list)
			{
				messageItem.Recipients.Remove(id);
			}
		}

		private bool IsSender(StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			string arg = storeDriverDeliveryEventArgsImpl.MailRecipient.MsgId.ToString();
			MailRecipient mailRecipient = storeDriverDeliveryEventArgsImpl.MailRecipient;
			DeliverableMailItem mailItem = args.MailItem;
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.SenderAddress);
			bool result;
			if (header != null)
			{
				string value = header.Value;
				if (!string.IsNullOrEmpty(value))
				{
					if (string.Compare(value, mailRecipient.Email.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsSender: MessageId {0}, Original Sender found in header matched with the current recipient address {1}", arg, value);
						result = true;
					}
					else
					{
						UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "IsSender: MessageId {0}, Original Sender found in header doesnot match with the current recipient address : Original sender {1} , Current Recipient {2}", arg, value, mailRecipient.Email.ToString());
						result = false;
					}
				}
				else
				{
					UnJournalAgent.Tracer.TraceDebug<string>(0L, "IsSender: MessageId {0}, Original Sender not found in mailitem unjournal headers, header value is empty", arg);
					result = false;
				}
			}
			else
			{
				UnJournalAgent.Tracer.TraceDebug<string>(0L, "IsSender: MessageId {0}, Original Sender not found in mailitem unjournal headers, header doesnot exist", arg);
				result = false;
			}
			return result;
		}

		private bool IsSenderARecipient(DeliverableMailItem mailItem, string messageId)
		{
			if (mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.SenderIsRecipient) == null)
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsSenderARecipient: MessageId {0}, Sender {1}, Sender is not the recipient", messageId, mailItem.FromAddress.ToString());
				return false;
			}
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsSenderARecipient: MessageId {0}, Sender {1}, Sender is a recipient", messageId, mailItem.FromAddress.ToString());
			return true;
		}

		private bool TryGetOriginalDateForMigratedMessage(DeliverableMailItem mailItem, out ExDateTime migrationDate, string messageId)
		{
			migrationDate = ExDateTime.MinValue;
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.MessageOriginalDate);
			if (header != null)
			{
				string value = header.Value;
				if (!string.IsNullOrEmpty(value))
				{
					UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "TryGetOriginalDateForMigratedMessage: MessageId {0}, Sender {1}, Original message date found : {2}", messageId, mailItem.FromAddress.ToString(), value);
					if (ExDateTime.TryParse(value, out migrationDate))
					{
						UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "TryGetOriginalDateForMigratedMessage: MessageId {0}, Sender {1}, Original message date parsed successfully: {2}", messageId, mailItem.FromAddress.ToString(), migrationDate.ToString());
						return true;
					}
					UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "TryGetOriginalDateForMigratedMessage: MessageId {0}, Sender {1}, Unable to parse original message date from the string: {2}", messageId, mailItem.FromAddress.ToString(), value);
					return false;
				}
				else
				{
					UnJournalAgent.Tracer.TraceDebug(0L, "TryGetOriginalDateForMigratedMessage: MessageId {0}, Sender {1}, Original message receive date is empty , {2} header found , header value is {3}", new object[]
					{
						messageId,
						mailItem.FromAddress.ToString(),
						UnJournalAgent.UnjournalHeaders.MessageOriginalDate,
						value
					});
				}
			}
			else
			{
				UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "TryGetOriginalDateForMigratedMessage: MessageId {0}, Sender {1}, Original receive date header not found : header name is  {2}", messageId, mailItem.FromAddress.ToString(), UnJournalAgent.UnjournalHeaders.MessageOriginalDate);
			}
			return false;
		}

		private bool ShouldProcess(OrganizationId orgId, DeliverableMailItem mailItem, string messageId)
		{
			UnJournalAgent.Tracer.TraceDebug<string>(0L, "IsProcessedByUnJournalAgent: MessageId {0}, Invoked ShouldProcess", messageId);
			return this.IsLegacyArchiveJournalingEnabled(orgId, mailItem, messageId) && this.IsProcessedByUnJournalAgent(mailItem, messageId);
		}

		private bool ShouldProcessWithNdrCheck(OrganizationId orgId, DeliverableMailItem mailItem, string messageId)
		{
			UnJournalAgent.Tracer.TraceDebug<string>(0L, "IsProcessedByUnJournalAgent: MessageId {0}, Invoked ShouldProcess", messageId);
			return this.IsLegacyArchiveJournalingEnabled(orgId, mailItem, messageId) && this.IsProcessedByUnJournalAgentForNdr(mailItem, messageId);
		}

		private bool IsProcessedByUnJournalAgent(DeliverableMailItem mailItem, string messageId)
		{
			string text = mailItem.FromAddress.ToString();
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsProcessedByUnJournalAgent: MessageId {0}, Invoked, Sender {1}", messageId, text);
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.ProcessedByUnjournal);
			Header header2 = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.ProcessedByUnjournalForNdr);
			if (header != null && header2 == null)
			{
				UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "IsProcessedByUnJournalAgent: MessageId {0}, Sender {1}, Able to find '{2}' header on mailItem", messageId, text, UnJournalAgent.UnjournalHeaders.ProcessedByUnjournal);
				return true;
			}
			UnJournalAgent.Tracer.TraceDebug(0L, "IsProcessedByUnJournalAgent: MessageId {0}, Sender {1}, processedByUnjournal value {2}, processedByUnjournalAgentForNdrHeader value {3}", new object[]
			{
				messageId,
				text,
				(header == null) ? "null" : header.ToString(),
				(header2 == null) ? "null" : header2.ToString()
			});
			return false;
		}

		private bool IsSourceEhaMigration(DeliverableMailItem mailItem, string messageId)
		{
			if (mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.MessageOriginalDate) == null)
			{
				UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "IsSourceEhaMigration: MessageId {0}, Sender {1}, Header '{2}' not found.", messageId, mailItem.FromAddress.ToString(), UnJournalAgent.UnjournalHeaders.MessageOriginalDate);
				return false;
			}
			UnJournalAgent.Tracer.TraceDebug<string, string, string>(0L, "IsSourceEhaMigration: MessageId {0}, Sender {1}, Header '{2}' found.", messageId, mailItem.FromAddress.ToString(), UnJournalAgent.UnjournalHeaders.MessageOriginalDate);
			return true;
		}

		private bool IsProcessedByUnJournalAgentForNdr(DeliverableMailItem mailItem, string messageId)
		{
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.ProcessedByUnjournal);
			Header header2 = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.ProcessedByUnjournalForNdr);
			if (header == null && header2 != null)
			{
				UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsSourceEhaMigration: MessageId {0}, Sender {1}, This is a journal report targeted for journal ndr mailbox", messageId, mailItem.FromAddress.ToString());
				return true;
			}
			UnJournalAgent.Tracer.TraceDebug<string, string>(0L, "IsSourceEhaMigration: MessageId {0}, Sender {1}, This is NOT a journal ndr report targeted for journal ndr mailbox", messageId, mailItem.FromAddress.ToString());
			return false;
		}

		private const string MigrationFolderName = "MigrationFolder";

		private static readonly Trace Tracer = ExTraceGlobals.UnJournalDeliveryAgentTracer;

		private List<MimeRecipient> bccRecipients = new List<MimeRecipient>();

		internal static class UnjournalHeaders
		{
			public static string EHAMessageID = "X-MS-EHAMessageID";

			public static string MessageOriginalDate = "X-MS-Exchange-Organization-Unjournal-OriginalReceiveDate";

			public static string MessageExpiryDate = "X-MS-Exchange-Organization-Unjournal-OriginalExpiryDate";

			public static string SenderAddress = "X-MS-Exchange-Organization-Unjournal-SenderAddress";

			public static string SenderIsRecipient = "X-MS-Exchange-Organization-Unjournal-SenderIsRecipient";

			public static string ProcessedByUnjournal = "X-MS-Exchange-Organization-Unjournal-Processed";

			public static string ProcessedByUnjournalForNdr = "X-MS-Exchange-Organization-Unjournal-ProcessedNdr";
		}
	}
}
