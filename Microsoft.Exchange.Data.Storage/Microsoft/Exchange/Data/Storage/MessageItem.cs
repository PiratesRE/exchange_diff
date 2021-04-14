using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessageItem : Item, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MessageItem(ICoreItem coreItem, bool shallowDispose = false) : base(coreItem, shallowDispose)
		{
		}

		public static MessageItem Create(StoreSession session, StoreId destFolderId)
		{
			return MessageItem.InternalCreate(session, destFolderId, CreateMessageType.Normal);
		}

		public static MessageItem CloneMessage(StoreSession session, StoreId parentFolderId, MessageItem itemToClone)
		{
			return MessageItem.CloneMessage(session, parentFolderId, itemToClone, null);
		}

		public static MessageItem CloneMessage(StoreSession session, StoreId parentFolderId, MessageItem itemToClone, ICollection<PropertyDefinition> propsToReturn)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (parentFolderId == null)
			{
				throw new ArgumentNullException("parentFolderId");
			}
			if (itemToClone == null)
			{
				throw new ArgumentNullException("itemToClone");
			}
			return (MessageItem)Microsoft.Exchange.Data.Storage.Item.CloneItem(session, parentFolderId, itemToClone, true, false, propsToReturn);
		}

		public static MessageItem CreateAssociated(StoreSession session, StoreId destFolderId)
		{
			return MessageItem.InternalCreate(session, destFolderId, CreateMessageType.Associated);
		}

		public static MessageItem CreateAggregated(StoreSession session, StoreId destFolderId)
		{
			return MessageItem.InternalCreate(session, destFolderId, CreateMessageType.Aggregated);
		}

		public static MessageItem CreateForDelivery(StoreSession session, StoreId destFolderId, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			return MessageItem.InternalCreateForDelivery(session, destFolderId, CreateMessageType.Normal, internetMessageId, clientSubmitTime);
		}

		public static MessageItem CreateAggregatedForDelivery(StoreSession session, StoreId destFolderId, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			return MessageItem.InternalCreateForDelivery(session, destFolderId, CreateMessageType.Aggregated, internetMessageId, clientSubmitTime);
		}

		public new static MessageItem Bind(StoreSession session, StoreId messageId)
		{
			return MessageItem.Bind(session, messageId, null);
		}

		public new static MessageItem Bind(StoreSession session, StoreId messageId, params PropertyDefinition[] propsToReturn)
		{
			return MessageItem.Bind(session, messageId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static MessageItem Bind(StoreSession session, StoreId messageId, ItemBindOption itemBindOption, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MessageItem>(session, messageId, MessageItemSchema.Instance, null, itemBindOption, propsToReturn);
		}

		public new static MessageItem Bind(StoreSession session, StoreId messageId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MessageItem>(session, messageId, MessageItemSchema.Instance, propsToReturn);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.smimeContent != null)
				{
					this.smimeContent.Dispose();
					this.smimeContent = null;
				}
				base.InternalDispose(disposing);
			}
		}

		private static MessageItem InternalCreate(StoreSession session, StoreId destFolderId, CreateMessageType mapiMessageType)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = ItemBuilder.CreateNewItem<MessageItem>(session, destFolderId, ItemCreateInfo.MessageItemInfo, mapiMessageType);
				disposeGuard.Add<MessageItem>(messageItem);
				messageItem[InternalSchema.ItemClass] = "IPM.Note";
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		private static MessageItem InternalCreateForDelivery(StoreSession session, StoreId destFolderId, CreateMessageType mapiMessageType, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = ItemBuilder.CreateNewItem<MessageItem>(session, ItemCreateInfo.MessageItemInfo, () => Folder.InternalCreateMapiMessageForDelivery(session, destFolderId, mapiMessageType, internetMessageId, clientSubmitTime));
				disposeGuard.Add<MessageItem>(messageItem);
				messageItem[InternalSchema.ItemClass] = "IPM.Note";
				messageItem.SetNoMessageDecoding(true);
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		private static bool MessageMimeChanged(CoreItem coreItem)
		{
			if ((coreItem.IsRecipientCollectionLoaded && coreItem.Recipients.IsDirty) || (((ICoreItem)coreItem).IsAttachmentCollectionLoaded && coreItem.AttachmentCollection.IsDirty))
			{
				return true;
			}
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreItem).AllNativeProperties)
			{
				if (coreItem.PropertyBag.IsPropertyDirty(nativeStorePropertyDefinition) && ItemConversion.DoesPropertyAffectMime(nativeStorePropertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageItem>(this);
		}

		public Participant Sender
		{
			get
			{
				this.CheckDisposed("Sender::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.Sender);
			}
			set
			{
				this.CheckDisposed("Sender::set");
				base.SetOrDeleteProperty(InternalSchema.Sender, value);
			}
		}

		public Participant From
		{
			get
			{
				this.CheckDisposed("From::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.From);
			}
			set
			{
				this.CheckDisposed("From::set");
				base.SetOrDeleteProperty(InternalSchema.From, value);
				if (null == value)
				{
					foreach (PropertyDefinition propertyDefinition in this.ExtraPropertiesToDeleteFrom)
					{
						base.Delete(propertyDefinition);
					}
				}
			}
		}

		public ExDateTime ReceivedTime
		{
			get
			{
				this.CheckDisposed("ReceivedTime::get");
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.ReceivedTime, ExDateTime.MinValue);
			}
		}

		public Participant ReadReceiptAddressee
		{
			get
			{
				this.CheckDisposed("ReadReceiptAddressee::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.ReadReceiptAddressee);
			}
			set
			{
				this.CheckDisposed("ReadReceiptAddressee::set");
				base.SetOrDeleteProperty(InternalSchema.ReadReceiptAddressee, value);
			}
		}

		public bool IsReadReceiptRequested
		{
			get
			{
				this.CheckDisposed("IsReadReceiptRequested::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsReadReceiptRequested);
			}
			set
			{
				this.CheckDisposed("IsReadReceiptRequested::set");
				this[InternalSchema.IsReadReceiptRequested] = value;
			}
		}

		public bool IsDeliveryReceiptRequested
		{
			get
			{
				this.CheckDisposed("IsDeliveryReceiptRequested::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsDeliveryReceiptRequested);
			}
			set
			{
				this.CheckDisposed("IsDeliveryReceiptRequested::set");
				this[InternalSchema.IsDeliveryReceiptRequested] = value;
			}
		}

		public bool IsGroupEscalationMessage
		{
			get
			{
				this.CheckDisposed("IsGroupEscalationMessage::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsGroupEscalationMessage);
			}
			set
			{
				this.CheckDisposed("IsGroupEscalationMessage::set");
				this[InternalSchema.IsGroupEscalationMessage] = value;
			}
		}

		public AutoResponseSuppress AutoResponseSuppress
		{
			get
			{
				this.CheckDisposed("AutoResponseSuppress::get");
				return base.GetValueOrDefault<AutoResponseSuppress>(InternalSchema.AutoResponseSuppress);
			}
			set
			{
				this.CheckDisposed("AutoResponseSuppress::set");
				EnumValidator.ThrowIfInvalid<AutoResponseSuppress>(value);
				this[InternalSchema.AutoResponseSuppress] = value;
			}
		}

		public ExDateTime SentTime
		{
			get
			{
				this.CheckDisposed("SentTime::get");
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.SentTime, ExDateTime.MinValue);
			}
		}

		public IconIndex IconIndex
		{
			get
			{
				this.CheckDisposed("IconIndex::get");
				return base.GetValueOrDefault<IconIndex>(InternalSchema.IconIndex, IconIndex.Default);
			}
			set
			{
				this.CheckDisposed("IconIndex::set");
				EnumValidator.ThrowIfInvalid<IconIndex>(value, "value");
				this[InternalSchema.IconIndex] = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				this.CheckDisposed("InternetMessageId::get");
				return base.GetValueOrDefault<string>(InternalSchema.InternetMessageId, string.Empty);
			}
			set
			{
				this.CheckDisposed("InternetMessageId::set");
				base.CheckSetNull("Message::InternetMessageId", "InternetMessageId", value);
				this[InternalSchema.InternetMessageId] = value;
			}
		}

		public byte[] ConversationIndex
		{
			get
			{
				this.CheckDisposed("ConversationIndex::get");
				return base.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
			}
			set
			{
				this.CheckDisposed("ConversationIndex::set");
				base.CheckSetNull("Message::ConversationIndex", "ConversationIndex", value);
				this[InternalSchema.ConversationIndex] = value;
			}
		}

		public string ConversationTopic
		{
			get
			{
				this.CheckDisposed("ConversationTopic::get");
				return MessageItem.CalculateConversationTopic(this);
			}
			set
			{
				this.CheckDisposed("ConversationTopic::set");
				base.CheckSetNull("Message::ConversationTopic", "ConversationTopic", value);
				this[InternalSchema.ConversationTopic] = value;
			}
		}

		public string InReplyTo
		{
			get
			{
				this.CheckDisposed("InReplyTo::get");
				return base.GetValueOrDefault<string>(InternalSchema.InReplyTo, string.Empty);
			}
			set
			{
				this.CheckDisposed("InReplyTo::set");
				base.CheckSetNull("Message::InReplyTo", "InReplyTo", value);
				this[InternalSchema.InReplyTo] = value;
			}
		}

		public string References
		{
			get
			{
				this.CheckDisposed("References::get");
				return base.GetValueOrDefault<string>(InternalSchema.InternetReferences, string.Empty);
			}
			set
			{
				this.CheckDisposed("References::set");
				base.CheckSetNull("Message::InternetReferences", "InternetReferences", value);
				this[InternalSchema.InternetReferences] = value;
			}
		}

		public IList<Participant> Likers
		{
			get
			{
				this.CheckDisposed("Likers::get");
				if (this.likers == null)
				{
					this.likers = new Likers(base.PropertyBag);
				}
				return this.likers;
			}
		}

		public IList<Participant> ReplyTo
		{
			get
			{
				this.CheckDisposed("ReplyTo::get");
				if (this.replyTo == null)
				{
					this.replyTo = new ReplyTo(base.PropertyBag);
				}
				return this.replyTo;
			}
		}

		public bool IsResponseRequested
		{
			get
			{
				this.CheckDisposed("IsResponseRequested::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsResponseRequested);
			}
			set
			{
				this.CheckDisposed("IsResponseRequested::set");
				this[InternalSchema.IsResponseRequested] = value;
			}
		}

		public bool IsRead
		{
			get
			{
				this.CheckDisposed("IsRead::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsRead);
			}
			set
			{
				this.CheckDisposed("IsRead::set");
				this[InternalSchema.IsRead] = value;
			}
		}

		public bool DeferUnreadFlag
		{
			get
			{
				this.CheckDisposed("DeferUnreadFlag::get");
				return base.GetValueOrDefault<int>(InternalSchema.ItemTemporaryFlag) == 1;
			}
			set
			{
				this.CheckDisposed("DeferUnreadFlag::set");
				if (value)
				{
					this[InternalSchema.ItemTemporaryFlag] = ItemTemporaryFlags.DeferUnread;
					return;
				}
				base.PropertyBag.Delete(InternalSchema.ItemTemporaryFlag);
			}
		}

		public bool WasDeliveredViaBcc
		{
			get
			{
				this.CheckDisposed("WasDeliveredViaBcc::get");
				return base.GetValueOrDefault<bool>(InternalSchema.MessageBccMe);
			}
			set
			{
				this.CheckDisposed("WasDeliveredViaBcc::set");
				this[InternalSchema.MessageBccMe] = value;
			}
		}

		public bool IsFromFavoriteSender
		{
			get
			{
				this.CheckDisposed("IsFromFavoriteSender::get");
				return base.GetValueOrDefault<bool>(MessageItemSchema.IsFromFavoriteSender);
			}
			set
			{
				this.CheckDisposed("IsFromFavoriteSender::set");
				this[MessageItemSchema.IsFromFavoriteSender] = value;
			}
		}

		public bool IsFromPerson
		{
			get
			{
				this.CheckDisposed("IsFromPerson::get");
				return base.GetValueOrDefault<bool>(MessageItemSchema.IsFromPerson);
			}
			set
			{
				this.CheckDisposed("IsFromPerson::set");
				this[MessageItemSchema.IsFromPerson] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MessageItemSchema.Instance;
			}
		}

		public void ExpandRecipientTable()
		{
			this.CheckDisposed("ExpandRecipientTable");
			this.InternalExpandRecipientTable();
		}

		public void Send()
		{
			this.Send(SubmitMessageFlags.None);
		}

		public void Send(SubmitMessageFlags submitFlags)
		{
			this.CheckDisposed("Send");
			base.Session.CheckCapabilities(base.Session.Capabilities.CanSend, "CanSend");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.Message.Send.");
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession != null)
			{
				this.InternalSend(mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems), submitFlags, false);
				return;
			}
			this.InternalSend(null, submitFlags, true);
		}

		public void Send(StoreObjectId saveSentMessageFolder, SaveMode saveMode = SaveMode.ResolveConflicts)
		{
			this.Send(saveSentMessageFolder, SubmitMessageFlags.None, false, saveMode);
		}

		public void Send(StoreObjectId saveSentMessageFolder, SubmitMessageFlags submitFlags)
		{
			this.Send(saveSentMessageFolder, submitFlags, false, SaveMode.ResolveConflicts);
		}

		public bool IsDraft
		{
			get
			{
				this.CheckDisposed("IsDraft::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsDraft);
			}
			set
			{
				this.CheckDisposed("IsDraft::set");
				this[InternalSchema.IsDraft] = value;
			}
		}

		public bool IsResend
		{
			get
			{
				this.CheckDisposed("IsResend::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsResend, false);
			}
			set
			{
				this.CheckDisposed("IsResend::set");
				this[InternalSchema.IsResend] = value;
			}
		}

		public VotingInfo VotingInfo
		{
			get
			{
				if (this.votingInfo == null)
				{
					this.votingInfo = new VotingInfo(this);
				}
				return this.votingInfo;
			}
		}

		public Reminders<ModernReminder> ModernReminders
		{
			get
			{
				this.CheckDisposed("ModernReminders::get");
				if (this.modernReminders == null)
				{
					this.modernReminders = Reminders<ModernReminder>.Get(this, InternalSchema.ModernReminders);
				}
				return this.modernReminders;
			}
			set
			{
				this.CheckDisposed("ModernReminders::set");
				base.Load(new PropertyDefinition[]
				{
					InternalSchema.GlobalObjectId
				});
				if (base.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId, null) == null)
				{
					GlobalObjectId globalObjectId = new GlobalObjectId();
					this[InternalSchema.GlobalObjectId] = globalObjectId.Bytes;
				}
				Reminders<ModernReminder>.Set(this, InternalSchema.ModernReminders, value);
				this.modernReminders = value;
			}
		}

		public RemindersState<ModernReminderState> ModernRemindersState
		{
			get
			{
				this.CheckDisposed("ModernRemindersState::get");
				if (this.modernRemindersState == null)
				{
					this.modernRemindersState = RemindersState<ModernReminderState>.Get(this, InternalSchema.ModernRemindersState);
				}
				return this.modernRemindersState;
			}
			set
			{
				this.CheckDisposed("ModernRemindersState::set");
				RemindersState<ModernReminderState>.Set(this, InternalSchema.ModernRemindersState, value);
				this.modernRemindersState = value;
			}
		}

		public GlobalObjectId GetGlobalObjectId()
		{
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.GlobalObjectId
			});
			byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId, null);
			if (valueOrDefault == null)
			{
				return null;
			}
			return new GlobalObjectId(valueOrDefault);
		}

		public bool IsEventReminderMessage()
		{
			string valueOrDefault = base.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			return ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Reminder.Event");
		}

		public bool IsValidApprovalRequest()
		{
			string valueOrDefault = base.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Approval.Request"))
			{
				return false;
			}
			if (ObjectClass.IsSmime(valueOrDefault))
			{
				return false;
			}
			VotingInfo votingInfo = this.VotingInfo;
			if (votingInfo == null)
			{
				return false;
			}
			string[] array = (string[])votingInfo.GetOptionsList();
			return array != null && array.Length == 2;
		}

		public bool IsValidUndecidedApprovalRequest()
		{
			if (!this.IsValidApprovalRequest())
			{
				return false;
			}
			int? valueAsNullable = base.PropertyBag.GetValueAsNullable<int>(MessageItemSchema.LastVerbExecuted);
			if (valueAsNullable != null && valueAsNullable.Value >= 1 && valueAsNullable.Value < 100)
			{
				return false;
			}
			int? valueAsNullable2 = base.PropertyBag.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision);
			return valueAsNullable2 == null || valueAsNullable2.Value < 1 || valueAsNullable2.Value >= 100;
		}

		public MessageItem CreateForward(StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				string message = "MessageItem::CreateForward: Unable to create reply/forward on non-Mailbox session";
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new NotSupportedException(message);
			}
			return this.CreateForward(mailboxSession, parentFolderId, configuration);
		}

		public virtual MessageItem CreateForward(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateForward");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::CreateReply.");
			return base.InternalCreateForward(session, parentFolderId, configuration);
		}

		public MessageItem CreateReply(StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				string message = "MessageItem::CreateReply: Unable to create reply/forward on non-Mailbox session";
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new NotSupportedException(message);
			}
			return this.CreateReply(mailboxSession, parentFolderId, configuration);
		}

		public MessageItem CreateVotingResponse(string bodyPrefix, BodyFormat format, StoreId parentFolderId, string votingOption)
		{
			this.CheckDisposed("CreateVotingResponse");
			EnumValidator.ThrowIfInvalid<BodyFormat>(format, "format");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(bodyPrefix, "bodyPrefix");
			Util.ThrowOnNullArgument(votingOption, "votingOption");
			if (!this.VotingInfo.GetOptionsList().Contains(votingOption))
			{
				throw new ArgumentException("Invalid VotingOption", votingOption);
			}
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "MessageItem::CreateVotingResponse");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(base.Session, parentFolderId);
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(format);
				replyForwardConfiguration.AddBodyPrefix(bodyPrefix);
				VotingResponse votingResponse = new VotingResponse(this, messageItem, replyForwardConfiguration, votingOption);
				votingResponse.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public virtual MessageItem CreateReply(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReply");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::CreateReply.");
			this.CheckReplyAllowed();
			MessageItem messageItem = base.InternalCreateReply(session, parentFolderId, configuration);
			this.SetApprovalRequestReplyRecipient(messageItem);
			return messageItem;
		}

		public MessageItem CreateReplyAll(StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				string message = "MessageItem::CreateReplyAll: Unable to create reply/forward on non-Mailbox session";
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new NotSupportedException(message);
			}
			return this.CreateReplyAll(mailboxSession, parentFolderId, configuration);
		}

		public virtual MessageItem CreateReplyAll(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReplyAll");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::CreateReplyAll.");
			this.CheckReplyAllowed();
			MessageItem messageItem = base.InternalCreateReplyAll(session, parentFolderId, configuration);
			this.SetApprovalRequestReplyRecipient(messageItem);
			return messageItem;
		}

		public VersionedId ParentMessageId
		{
			get
			{
				this.CheckDisposed("ParentMessageId::get");
				this.GetReplyForwardStatus();
				return this.parentMessageId;
			}
			internal set
			{
				this.CheckDisposed("ParentMessageId::set");
				this.GetReplyForwardStatus();
				this.parentMessageId = value;
			}
		}

		public MessageResponseType MessageResponseType
		{
			get
			{
				this.CheckDisposed("MessageResponseType::get");
				this.GetReplyForwardStatus();
				return this.messageResponseType;
			}
			internal set
			{
				this.CheckDisposed("MessageResponseType::set");
				this.GetReplyForwardStatus();
				this.messageResponseType = value;
			}
		}

		public RecipientCollection Recipients
		{
			get
			{
				this.CheckDisposed("Recipients::get");
				return this.FetchRecipientCollection(true);
			}
		}

		internal RecipientCollection FetchRecipientCollection(bool forceOpen)
		{
			if (this.recipients == null)
			{
				CoreRecipientCollection recipientCollection = base.CoreItem.GetRecipientCollection(forceOpen);
				if (recipientCollection != null)
				{
					this.recipients = new RecipientCollection(recipientCollection);
				}
			}
			return this.recipients;
		}

		internal Item FetchSmimeContent(string imceaDomain)
		{
			if (this.smimeContent != null)
			{
				this.smimeContent.Dispose();
				this.smimeContent = null;
			}
			if (!ItemConversion.TryOpenSMimeContent(this, imceaDomain, out this.smimeContent))
			{
				return null;
			}
			return this.smimeContent;
		}

		public bool IsRestricted
		{
			get
			{
				string contentClass = base.TryGetProperty(InternalSchema.ContentClass) as string;
				if (!ObjectClass.IsRightsManagedContentClass(contentClass))
				{
					return false;
				}
				if (base.AttachmentCollection.Count != 1)
				{
					return false;
				}
				using (Attachment attachment = base.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Stream))
				{
					if (attachment != null)
					{
						return string.Equals(attachment.FileName, "message.rpmsg", StringComparison.OrdinalIgnoreCase);
					}
				}
				return false;
			}
		}

		public void SendReadReceipt()
		{
			this.SetReadFlagsInternal(SetReadFlags.DeferredErrors);
			this.SetReadFlagsInternal(SetReadFlags.GenerateReceiptOnly);
			this.IsRead = true;
			base.ClearFlagsPropertyForSet(InternalSchema.IsRead);
		}

		public void MarkAsRead(bool suppressReadReceipt, bool deferToSave)
		{
			this.CheckDisposed("MarkAsRead");
			if (!deferToSave)
			{
				this.SetReadFlagsInternal(suppressReadReceipt ? SetReadFlags.SuppressReceipt : SetReadFlags.None);
			}
			this.IsRead = true;
			this[InternalSchema.SuppressReadReceipt] = suppressReadReceipt;
			if (!deferToSave)
			{
				base.ClearFlagsPropertyForSet(InternalSchema.IsRead);
				base.ClearFlagsPropertyForSet(InternalSchema.SuppressReadReceipt);
			}
		}

		public void MarkAsUnread(bool deferToSave)
		{
			this.CheckDisposed("MarkAsUnread");
			if (!deferToSave)
			{
				this.SetReadFlagsInternal(SetReadFlags.ClearRead);
			}
			this.IsRead = false;
			if (!deferToSave)
			{
				base.ClearFlagsPropertyForSet(InternalSchema.IsRead);
			}
		}

		public CultureInfo GetPreferredAcceptLanguage()
		{
			this.CheckDisposed("GetPreferredAcceptLanguage");
			string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.AcceptLanguage);
			if (valueOrDefault != null)
			{
				return DsnHumanReadableWriter.DefaultDsnHumanReadableWriter.FindLanguage(valueOrDefault, true);
			}
			return null;
		}

		public void AbortSubmit()
		{
			this.CheckDisposed("AbortSubmit");
			if (base.Session == null)
			{
				throw new InvalidOperationException("Cannot invoke AbortSubmit on an in-memory item.");
			}
			if (base.StoreObjectId != null)
			{
				base.Session.AbortSubmit(base.StoreObjectId);
			}
		}

		public void SetNoMessageDecoding(bool value)
		{
			base.CoreItem.CharsetDetector.NoMessageDecoding = value;
		}

		public void MarkRecipientAsSubmitted(IEnumerable<Participant> submittedParticipants)
		{
			this[MessageItemSchema.NeedSpecialRecipientProcessing] = true;
			foreach (Recipient recipient in this.Recipients)
			{
				recipient.Submitted = recipient.Participant.ExistIn(submittedParticipants);
			}
		}

		public void MarkAllRecipientAsSubmitted()
		{
			this[MessageItemSchema.NeedSpecialRecipientProcessing] = true;
			foreach (Recipient recipient in this.Recipients)
			{
				recipient.Submitted = true;
			}
		}

		public void MarkAsNonDraft()
		{
			if (this.IsDraft || ((MessageFlags)this[MessageItemSchema.Flags] & MessageFlags.IsDraft) == MessageFlags.IsDraft)
			{
				this[MessageItemSchema.Flags] = ((MessageFlags)this[MessageItemSchema.Flags] ^ MessageFlags.IsDraft);
			}
		}

		public void SuppressAllAutoResponses()
		{
			this.AutoResponseSuppress = ~(AutoResponseSuppress.DR | AutoResponseSuppress.NDR);
			this.IsDeliveryReceiptRequested = false;
			this[MessageItemSchema.IsNonDeliveryReceiptRequested] = false;
		}

		internal override VersionedId AssociatedItemId
		{
			get
			{
				this.CheckDisposed("AssociatedItemId::get");
				return ReplyForwardUtils.GetAssociatedId(this);
			}
			set
			{
				this.CheckDisposed("AssociatedItemId::set");
				ReplyForwardUtils.SetAssociatedId(this, value);
			}
		}

		public virtual bool IsReplyAllowed
		{
			get
			{
				this.CheckDisposed("IsReplyAllowed::get");
				return true;
			}
		}

		internal static string CalculateConversationTopic(Item item)
		{
			string text = item.TryGetProperty(InternalSchema.ConversationTopic) as string;
			if (text == null)
			{
				text = item.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
				if (item.PropertyBag is AcrPropertyBag && !((AcrPropertyBag)item.PropertyBag).IsReadOnly)
				{
					item[InternalSchema.ConversationTopic] = text;
				}
			}
			return text;
		}

		public static MessageItem CreateInMemory(PropertyDefinition[] prefetchProperties)
		{
			Util.ThrowOnNullArgument(prefetchProperties, "prefetchProperties");
			return ItemBuilder.ConstructItem<MessageItem>(null, null, null, prefetchProperties, () => new InMemoryPersistablePropertyBag(prefetchProperties), ItemCreateInfo.MessageItemInfo.Creator, Origin.Existing, ItemLevel.TopLevel);
		}

		public void SendWithoutSavingMessage()
		{
			this.SendWithoutSavingMessage(SubmitMessageFlags.None);
		}

		public void SendWithoutSavingMessage(SubmitMessageFlags submitFlags)
		{
			this.CheckDisposed("SendWithoutSavingMessage");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.Message.SendWithoutSavingMessage.");
			this.InternalSend(null, submitFlags, true);
		}

		public void SendWithoutMovingMessage(StoreObjectId folderId, SaveMode saveMode = SaveMode.ResolveConflicts)
		{
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.Message.SendWithoutMovingMessage.");
			if (folderId == null)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				if (mailboxSession != null)
				{
					folderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
				}
			}
			this.Send(folderId, SubmitMessageFlags.None, true, saveMode);
		}

		public void CommitReplyTo()
		{
			if (this.replyTo == null || !this.replyTo.IsDirty)
			{
				return;
			}
			if (base.PropertyBag is AcrPropertyBag)
			{
				((AcrPropertyBag)base.PropertyBag).SetIrresolvableChange();
			}
			this.replyTo.Save();
		}

		public void StampMessageBodyTag()
		{
			int num;
			this[InternalSchema.BodyTag] = base.Body.CalculateBodyTag(out num);
			if (num >= 0)
			{
				this[InternalSchema.LatestMessageWordCount] = num;
			}
		}

		protected override void OnBeforeSave()
		{
			this.CommitReplyTo();
			this.CommitLikers();
			base.OnBeforeSave();
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			base.OnAfterSave(acrResults);
			if (acrResults.SaveStatus == SaveResult.SuccessWithConflictResolution)
			{
				this.recipients = null;
			}
		}

		protected virtual void OnBeforeSend()
		{
			ConflictResolutionResult conflictResolutionResult = base.SaveInternal(this.sendSaveMode, true, null, CoreItemOperation.Send);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(base.InternalObjectId), conflictResolutionResult);
			}
		}

		private void SetReadFlagsInternal(SetReadFlags flags)
		{
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				base.MapiMessage.SetReadFlag(flags);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MessageItem::SetReadFlagsInternal.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MessageItem::SetReadFlagsInternal.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		private void CheckReplyAllowed()
		{
			if (!this.IsReplyAllowed)
			{
				throw new NotSupportedException(ServerStrings.OperationNotAllowed("Reply", base.GetType().Name));
			}
		}

		private void GetReplyForwardStatus()
		{
			if (this.isReplyFwdStatusParsed)
			{
				return;
			}
			this.messageResponseType = MessageResponseType.None;
			this.parentMessageId = null;
			string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.ReplyForwardStatus, string.Empty);
			if (valueOrDefault.Length == 0)
			{
				return;
			}
			object[] array = ReplyForwardUtils.DecodeReplyForwardStatus(valueOrDefault);
			if (array == null || array.Length != 3)
			{
				ExTraceGlobals.StorageTracer.TraceError<int, int>((long)this.GetHashCode(), "Ex12Mesage::GetReplyForwardStatus. The reply/forward status data saved on the new item has been corrupted. Actual field count = {0}, Expect = {1}.", (array == null) ? 0 : array.Length, 3);
				return;
			}
			try
			{
				int num = (int)array[0];
				if (num == 102 || num == 103)
				{
					this.messageResponseType = MessageResponseType.Reply;
				}
				else if (num == 104)
				{
					this.messageResponseType = MessageResponseType.Forward;
				}
				string base64Id = (string)array[2];
				this.parentMessageId = VersionedId.Deserialize(base64Id);
				this.isReplyFwdStatusParsed = true;
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<FormatException>((long)this.GetHashCode(), "Message::InternalParseReplyForwardStatus. The reply forward status data has been corrupted. Exception = {0}.", arg);
			}
		}

		private void CommitLikers()
		{
			if (this.likers != null && this.likers.IsDirty)
			{
				if (base.PropertyBag is AcrPropertyBag)
				{
					((AcrPropertyBag)base.PropertyBag).SetIrresolvableChange();
				}
				this.likers.Save();
			}
		}

		private void InternalExpandRecipientTable()
		{
			int num = this.Recipients.Count;
			for (int i = 0; i < num; i++)
			{
				Recipient recipient = this.Recipients[i];
				if (recipient.Participant.RoutingType == "MAPIPDL")
				{
					recipient.Participant.Validate();
					foreach (Participant participant in DistributionList.ExpandDeep(base.Session, ((StoreParticipantOrigin)recipient.Participant.Origin).OriginItemId, true))
					{
						this.Recipients.Add(participant, recipient.RecipientItemType);
					}
					this.Recipients.Remove(recipient);
					i--;
					num--;
				}
			}
		}

		private void Send(StoreObjectId saveSentMessageFolder, SubmitMessageFlags submitFlags, bool doNotMove, SaveMode saveMode = SaveMode.ResolveConflicts)
		{
			this.CheckDisposed("Send(saveSentMessageFolder, submitFlags, doNotMove, saveMode)");
			base.Session.CheckCapabilities(base.Session.Capabilities.CanSend, "CanSend");
			if (saveSentMessageFolder == null)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Message::Send(saveSentMessageFolder, submitFlags, doNotMove, saveMode). The folder Id parameter is null.");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("saveSentMessageFolder", 1));
			}
			ExTraceGlobals.StorageTracer.Information<string>((long)this.GetHashCode(), "Storage.Message.Send(saveSentMessageFolder, submitFlags, doNotMove, saveMode). saveSentMessageFolder = {0}.", saveSentMessageFolder.ToBase64String());
			this.sendSaveMode = saveMode;
			this.InternalSend(saveSentMessageFolder, submitFlags, doNotMove);
		}

		private void InternalSend(StoreObjectId saveSentMessageFolder, SubmitMessageFlags submitFlags, bool doNotMove)
		{
			this.InternalExpandRecipientTable();
			this.CommitReplyTo();
			ReplyForwardUtils.UpdateOriginalItemProperties(this);
			this[InternalSchema.ClientSubmittedSecurely] = true;
			base.Delete(InternalSchema.ReplyForwardStatus);
			if (saveSentMessageFolder == null)
			{
				this[InternalSchema.DeleteAfterSubmit] = true;
			}
			else
			{
				if (!doNotMove)
				{
					this[InternalSchema.SentMailEntryId] = saveSentMessageFolder.ProviderLevelItemId;
				}
				this.StampMessageBodyTag();
			}
			base.CoreItem.BeforeSend += this.OnBeforeSend;
			try
			{
				base.CoreItem.Submit(submitFlags);
			}
			catch (StoragePermanentException)
			{
				this.AbortSubmitOnFailure();
				throw;
			}
			catch (StorageTransientException)
			{
				this.AbortSubmitOnFailure();
				throw;
			}
			finally
			{
				base.CoreItem.BeforeSend -= this.OnBeforeSend;
			}
		}

		private void AbortSubmitOnFailure()
		{
			try
			{
				base.Load(null);
				this.AbortSubmit();
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
		}

		private void SetApprovalRequestReplyRecipient(MessageItem replyMessage)
		{
			if (this.ClassName.Equals("IPM.Note.Microsoft.Approval.Request", StringComparison.OrdinalIgnoreCase))
			{
				base.Load(new PropertyDefinition[]
				{
					MessageItemSchema.ApprovalRequestor
				});
				string valueOrDefault = base.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor, string.Empty);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					replyMessage.Recipients.Clear();
					replyMessage.Recipients.Add(new Participant(string.Empty, valueOrDefault, "SMTP"));
				}
			}
		}

		internal static void CoreObjectUpdateConversationTopic(CoreItem coreItem)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal);
			string valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ConversationTopic);
			if (valueOrDefault != null && valueOrDefault != valueOrDefault2)
			{
				coreItem.PropertyBag[InternalSchema.ConversationTopic] = valueOrDefault;
			}
		}

		internal static void CoreObjectUpdateConversationIndex(CoreItem coreItem)
		{
			if (coreItem.Session != null && PropertyError.IsPropertyNotFound(coreItem.PropertyBag.TryGetProperty(ItemSchema.ConversationIndex)))
			{
				coreItem.PropertyBag[ItemSchema.ConversationIndex] = Microsoft.Exchange.Data.Storage.ConversationIndex.CreateNew().ToByteArray();
			}
		}

		internal static void CoreObjectUpdateConversationIndexFixup(ICoreItem item, CoreItemOperation operation)
		{
			MessageItem.AggregateMessageInConversation(item, operation);
		}

		internal static void CoreObjectUpdateIconIndex(CoreItem coreItem)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (ObjectClass.IsMeetingRequest(valueOrDefault) || ObjectClass.IsMeetingCancellation(valueOrDefault))
			{
				return;
			}
			IconIndex? valueAsNullable = coreItem.PropertyBag.GetValueAsNullable<IconIndex>(InternalSchema.IconIndex);
			if (valueAsNullable == null)
			{
				return;
			}
			if (ObjectClass.IsMessage(valueOrDefault, false) && (valueAsNullable.Value & IconIndex.BaseAppointment) != (IconIndex)0 && valueAsNullable.Value != IconIndex.Default)
			{
				coreItem.PropertyBag[InternalSchema.IconIndex] = IconIndex.Default;
			}
		}

		internal static void CoreObjectUpdateMimeSkeleton(CoreItem coreItem)
		{
			if (coreItem.PropertyBag.IsPropertyDirty(InternalSchema.MimeSkeleton))
			{
				return;
			}
			if (MessageItem.MessageMimeChanged(coreItem))
			{
				if (!PropertyError.IsPropertyNotFound(coreItem.PropertyBag.TryGetProperty(InternalSchema.MimeSkeleton)))
				{
					coreItem.PropertyBag.Delete(InternalSchema.MimeSkeleton);
				}
				PersistablePropertyBag persistablePropertyBag = coreItem.PropertyBag as PersistablePropertyBag;
				if (persistablePropertyBag != null && !coreItem.IsMoveUser)
				{
					persistablePropertyBag.SetUpdateImapIdFlag();
				}
			}
		}

		public void RemoteSend()
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException("RemoteSend is used for aggregation mailbox");
			}
			if (mailboxSession.ActivitySession == null)
			{
				throw new ActivitySessionIsNullException();
			}
			mailboxSession.ActivitySession.CaptureRemoteSend(base.StoreObjectId);
			MailboxReplicationServiceClientSlim.NotifyToSync(SyncNowNotificationFlags.Send, mailboxSession.MailboxGuid, mailboxSession.MdbGuid);
		}

		private static void AggregateMessageInConversation(ICoreItem item, CoreItemOperation operation)
		{
			ICorePropertyBag propertyBag = item.PropertyBag;
			MailboxSession mailboxSession = item.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return;
			}
			ConversationIndexTrackingEx conversationIndexTrackingEx = ConversationIndexTrackingEx.Create();
			IConversationAggregator conversationAggregator;
			if (ConversationAggregatorFactory.TryInstantiateAggregatorForSave(mailboxSession, operation, item, conversationIndexTrackingEx, out conversationAggregator))
			{
				mailboxSession.Mailbox.Load(new PropertyDefinition[]
				{
					InternalSchema.LogonRightsOnMailbox
				});
				if (mailboxSession.CanActAsOwner || mailboxSession.IsGroupMailbox())
				{
					ConversationAggregationResult conversationAggregationResult = conversationAggregator.Aggregate(item);
					byte[] newValue;
					if (MessageItem.TryCalculateConversationCreatorSid(mailboxSession, conversationAggregationResult, operation, item.PropertyBag, out newValue))
					{
						propertyBag.SetOrDeleteProperty(ItemSchema.ConversationCreatorSID, newValue);
					}
					conversationIndexTrackingEx.TraceVersionAndHeuristics(conversationAggregationResult.Stage.ToString());
					propertyBag[ItemSchema.ConversationIndex] = conversationAggregationResult.ConversationIndex.ToByteArray();
					propertyBag[ItemSchema.ConversationIndexTrackingEx] = conversationIndexTrackingEx.ToString();
					propertyBag[ItemSchema.SupportsSideConversation] = conversationAggregationResult.SupportsSideConversation;
					propertyBag.SetOrDeleteProperty(ItemSchema.ConversationFamilyId, conversationAggregationResult.ConversationFamilyId);
					if (!Microsoft.Exchange.Data.Storage.ConversationIndex.CheckStageValue(conversationAggregationResult.Stage, Microsoft.Exchange.Data.Storage.ConversationIndex.FixupStage.L1))
					{
						propertyBag[ItemSchema.ConversationIndexTracking] = true;
						return;
					}
				}
				else
				{
					string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
					if (ObjectClass.IsMeetingMessage(valueOrDefault) || (string.IsNullOrEmpty(propertyBag.GetValueOrDefault<string>(ItemSchema.InReplyTo)) && string.IsNullOrEmpty(propertyBag.GetValueOrDefault<string>(ItemSchema.InternetReferences)) && string.IsNullOrEmpty(propertyBag.GetValueOrDefault<string>(ItemSchema.SubjectPrefix))))
					{
						propertyBag[ItemSchema.ConversationIndexTracking] = true;
					}
				}
			}
		}

		private static bool TryCalculateConversationCreatorSid(IMailboxSession session, ConversationAggregationResult aggregationResult, CoreItemOperation operation, ICorePropertyBag deliveredMessage, out byte[] conversationCreatorSid)
		{
			conversationCreatorSid = null;
			ConversationCreatorSidCalculatorFactory conversationCreatorSidCalculatorFactory = new ConversationCreatorSidCalculatorFactory(XSOFactory.Default);
			IConversationCreatorSidCalculator conversationCreatorSidCalculator;
			return conversationCreatorSidCalculatorFactory.TryCreate(session, session.MailboxOwner, out conversationCreatorSidCalculator) && conversationCreatorSidCalculator.TryCalculateOnSave(deliveredMessage, aggregationResult.Stage, aggregationResult.ConversationIndex, operation, out conversationCreatorSid);
		}

		private const SaveMode DefaultSendSaveMode = SaveMode.ResolveConflicts;

		private RecipientCollection recipients;

		private ReplyTo replyTo;

		private VersionedId parentMessageId;

		private MessageResponseType messageResponseType;

		private bool isReplyFwdStatusParsed;

		private VotingInfo votingInfo;

		private Item smimeContent;

		private Reminders<ModernReminder> modernReminders;

		private RemindersState<ModernReminderState> modernRemindersState;

		private SaveMode sendSaveMode;

		private Likers likers;

		private readonly PropertyDefinition[] ExtraPropertiesToDeleteFrom = new PropertyDefinition[]
		{
			InternalSchema.SentRepresentingSimpleDisplayName,
			InternalSchema.SentRepresentingOrgAddressType,
			InternalSchema.SentRepresentingOrgEmailAddr,
			InternalSchema.SentRepresentingSID,
			InternalSchema.SentRepresentingGuid
		};
	}
}
