using System;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class NotificationEvents
	{
		public static MailboxMoveStartedNotificationEvent CreateMailboxMoveStartedNotificationEvent(Context context, Mailbox mailbox, bool forSource)
		{
			return new MailboxMoveStartedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, forSource ? EventFlags.Source : EventFlags.Destination);
		}

		public static MailboxMoveSucceededNotificationEvent CreateMailboxMoveSucceededNotificationEvent(Context context, Mailbox mailbox, bool forSource)
		{
			return new MailboxMoveSucceededNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, forSource ? EventFlags.Source : EventFlags.Destination);
		}

		public static MailboxMoveFailedNotificationEvent CreateMailboxMoveFailedNotificationEvent(Context context, Mailbox mailbox, bool forSource)
		{
			return new MailboxMoveFailedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, forSource ? EventFlags.Source : EventFlags.Destination);
		}

		public static MailboxCreatedNotificationEvent CreateMailboxCreatedNotificationEvent(Context context, Mailbox mailbox)
		{
			return new MailboxCreatedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, EventFlags.None);
		}

		public static MailboxDeletedNotificationEvent CreateMailboxDeletedNotificationEvent(Context context, Mailbox mailbox)
		{
			return new MailboxDeletedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, EventFlags.None);
		}

		public static MailboxDisconnectedNotificationEvent CreateMailboxDisconnectedNotificationEvent(Context context, Mailbox mailbox)
		{
			return new MailboxDisconnectedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, EventFlags.None);
		}

		public static MailboxReconnectedNotificationEvent CreateMailboxReconnectedNotificationEvent(Context context, Mailbox mailbox)
		{
			return new MailboxReconnectedNotificationEvent(mailbox.Database, mailbox.MailboxNumber, null, context.ClientType, EventFlags.None);
		}

		public static MessageCreatedNotificationEvent CreateMessageCreatedEvent(Context context, TopMessage message)
		{
			return new MessageCreatedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, message.GetIsHidden(context) ? EventFlags.Associated : EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.GetFolderId(context), message.GetId(context), message.GetFolderId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), null, message.GetMessageClass(context), null);
		}

		public static MessageCreatedNotificationEvent CreateMessageCreatedEvent(Context context, TopMessage message, Folder searchFolder, Guid? userIdentityContext)
		{
			EventFlags eventFlags = EventFlags.SearchFolder;
			if (message.GetIsHidden(context))
			{
				eventFlags |= EventFlags.Associated;
			}
			return new MessageCreatedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, message, null, searchFolder), message.GetFolderId(context), message.GetId(context), searchFolder.GetId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), null, message.GetMessageClass(context), userIdentityContext);
		}

		public static MessageModifiedNotificationEvent CreateMessageModifiedEvent(Context context, TopMessage message, Guid? userIdentityContext)
		{
			EventFlags eventFlags = message.GetIsHidden(context) ? EventFlags.Associated : EventFlags.None;
			if (0UL != (message.ChangedGroups & 36028797018963968UL))
			{
				eventFlags |= EventFlags.CommonCategorizationPropertyChanged;
			}
			return new MessageModifiedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.GetFolderId(context), message.GetId(context), message.GetFolderId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), message.GetOriginalConversationDocumentId(context), null, message.GetMessageClass(context), userIdentityContext);
		}

		public static MessageModifiedNotificationEvent CreateMessageModifiedEvent(Context context, TopMessage message, SearchFolder searchFolder, Guid? userIdentityContext)
		{
			EventFlags eventFlags = EventFlags.SearchFolder;
			if (message.GetIsHidden(context))
			{
				eventFlags |= EventFlags.Associated;
			}
			if (0UL != (message.ChangedGroups & 36028797018963968UL))
			{
				eventFlags |= EventFlags.CommonCategorizationPropertyChanged;
			}
			return new MessageModifiedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, message, null, searchFolder), message.GetFolderId(context), message.GetId(context), searchFolder.GetId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), message.GetOriginalConversationDocumentId(context), null, message.GetMessageClass(context), userIdentityContext);
		}

		public static MessageDeletedNotificationEvent CreateMessageDeletedEvent(Context context, TopMessage message)
		{
			return new MessageDeletedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, message.GetIsHidden(context) ? EventFlags.Associated : EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.OriginalFolder.GetId(context), message.OriginalMessageID, message.OriginalFolder.GetId(context), message.GetDocumentId(context), message.GetOriginalConversationDocumentId(context), message.GetMessageClass(context), null, null);
		}

		public static MessageDeletedNotificationEvent CreateMessageDeletedEvent(Context context, TopMessage message, Folder searchFolder, Guid? userIdentityContext)
		{
			EventFlags eventFlags = EventFlags.SearchFolder;
			if (message.GetIsHidden(context))
			{
				eventFlags |= EventFlags.Associated;
			}
			return new MessageDeletedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, message, null, searchFolder), message.GetOriginalFolderId(context), message.OriginalMessageID, searchFolder.GetId(context), message.GetDocumentId(context), message.GetOriginalConversationDocumentId(context), message.GetMessageClass(context), null, userIdentityContext);
		}

		public static MessageMovedNotificationEvent CreateMessageMovedEvent(Context context, TopMessage message, Folder originalFolder)
		{
			return new MessageMovedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, message.GetIsHidden(context) ? EventFlags.Associated : EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message, originalFolder, null), message.GetFolderId(context), message.GetId(context), message.GetFolderId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), originalFolder.GetId(context), message.OriginalMessageID, originalFolder.GetId(context), message.GetOriginalConversationDocumentId(context), message.GetMessageClass(context));
		}

		public static MessageCopiedNotificationEvent CreateMessageCopiedEvent(Context context, TopMessage message, Folder originalFolder, ExchangeId originalMessageId)
		{
			return new MessageCopiedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.GetFolderId(context), message.GetId(context), message.GetFolderId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), originalFolder.GetId(context), originalMessageId, originalFolder.GetId(context), message.GetOriginalConversationDocumentId(context), message.GetMessageClass(context));
		}

		public static NewMailNotificationEvent CreateNewMailEvent(Context context, TopMessage message)
		{
			return NotificationEvents.CreateNewMailEvent(context, message, message.GetMessageClass(context), MessageFlags.None);
		}

		public static NewMailNotificationEvent CreateNewMailEvent(Context context, TopMessage message, string messageClass, MessageFlags messageFlags)
		{
			return new NewMailNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.GetFolderId(context), message.GetId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), messageClass.ToUpperInvariant(), (int)messageFlags);
		}

		public static MailSubmittedNotificationEvent CreateMailSubmittedEvent(Context context, TopMessage message)
		{
			return new MailSubmittedNotificationEvent(message.Mailbox.Database, message.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeMessageExtendedEventFlags(context, message), message.GetFolderId(context), message.GetId(context), message.GetDocumentId(context), message.GetConversationDocumentId(context), message.GetMessageClass(context));
		}

		public static MessageCreatedNotificationEvent CreateConversationCreatedEvent(Context context, Folder parentFolder, ConversationItem conversationItem)
		{
			EventFlags eventFlags = EventFlags.Conversation;
			if (parentFolder is SearchFolder)
			{
				eventFlags |= EventFlags.SearchFolder;
			}
			return new MessageCreatedNotificationEvent(conversationItem.Mailbox.Database, conversationItem.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, conversationItem), conversationItem.GetFolderId(context), conversationItem.GetId(context), parentFolder.GetId(context), conversationItem.GetDocumentId(context), null, null, null, null);
		}

		public static MessageModifiedNotificationEvent CreateConversationModifiedEvent(Context context, Folder parentFolder, ConversationItem conversationItem)
		{
			EventFlags eventFlags = EventFlags.Conversation;
			if (parentFolder is SearchFolder)
			{
				eventFlags |= EventFlags.SearchFolder;
			}
			return new MessageModifiedNotificationEvent(conversationItem.Mailbox.Database, conversationItem.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, conversationItem), conversationItem.GetFolderId(context), conversationItem.GetId(context), parentFolder.GetId(context), conversationItem.GetDocumentId(context), null, null, null, null, null);
		}

		public static MessageDeletedNotificationEvent CreateConversationDeletedEvent(Context context, Folder parentFolder, ConversationItem conversationItem)
		{
			EventFlags eventFlags = EventFlags.Conversation;
			if (parentFolder is SearchFolder)
			{
				eventFlags |= EventFlags.SearchFolder;
			}
			return new MessageDeletedNotificationEvent(conversationItem.Mailbox.Database, conversationItem.Mailbox.MailboxNumber, null, context.ClientType, eventFlags, NotificationEvents.ComputeMessageExtendedEventFlags(context, conversationItem), conversationItem.GetFolderId(context), conversationItem.GetId(context), parentFolder.GetId(context), conversationItem.GetDocumentId(context), null, null, (byte[])conversationItem.GetPropertyValue(context, PropTag.Message.ConversationId), null);
		}

		public static FolderCreatedNotificationEvent CreateFolderCreatedEvent(Context context, Folder folder)
		{
			return new FolderCreatedNotificationEvent(folder.Mailbox.Database, folder.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeFolderExtendedEventFlags(context, folder, ExtendedEventFlags.None), folder.GetId(context), folder.GetParentFolderId(context), null, folder.GetContainerClass(context));
		}

		public static FolderModifiedNotificationEvent CreateFolderModifiedEvent(Context context, Folder folder, EventFlags flags, ExtendedEventFlags additionalExtentedFlags, int totalMessageCount, int unreadMessageCount)
		{
			return new FolderModifiedNotificationEvent(folder.Mailbox.Database, folder.Mailbox.MailboxNumber, null, context.ClientType, flags, NotificationEvents.ComputeFolderExtendedEventFlags(context, folder, additionalExtentedFlags), folder.GetId(context), folder.GetParentFolderId(context), null, folder.GetContainerClass(context), totalMessageCount, unreadMessageCount);
		}

		public static FolderDeletedNotificationEvent CreateFolderDeletedEvent(Context context, Folder folder)
		{
			return new FolderDeletedNotificationEvent(folder.Mailbox.Database, folder.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeFolderExtendedEventFlags(context, folder, ExtendedEventFlags.None), folder.GetId(context), folder.GetParentFolderId(context), folder.GetContainerClass(context));
		}

		public static FolderMovedNotificationEvent CreateFolderMovedEvent(Context context, Folder folder, ExchangeId originalParentFolderId)
		{
			return new FolderMovedNotificationEvent(folder.Mailbox.Database, folder.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeFolderExtendedEventFlags(context, folder, ExtendedEventFlags.None), folder.GetId(context), folder.GetParentFolderId(context), folder.GetId(context), originalParentFolderId, folder.GetContainerClass(context));
		}

		public static FolderCopiedNotificationEvent CreateFolderCopiedEvent(Context context, Folder folder, ExchangeId originalFolderId, ExchangeId originalParentFolderId)
		{
			return new FolderCopiedNotificationEvent(folder.Mailbox.Database, folder.Mailbox.MailboxNumber, null, context.ClientType, EventFlags.None, NotificationEvents.ComputeFolderExtendedEventFlags(context, folder, ExtendedEventFlags.None), folder.GetId(context), folder.GetParentFolderId(context), originalFolderId, originalParentFolderId, folder.GetContainerClass(context));
		}

		private static ExtendedEventFlags ComputeMessageExtendedEventFlags(Context context, TopMessage message)
		{
			return NotificationEvents.ComputeMessageExtendedEventFlags(context, message, null, null);
		}

		private static ExtendedEventFlags ComputeMessageExtendedEventFlags(Context context, TopMessage message, Folder originalFolder, Folder searchFolder)
		{
			ExtendedEventFlags extendedEventFlags;
			if (searchFolder != null)
			{
				extendedEventFlags = NotificationEvents.ComputeFolderExtendedEventFlags(context, searchFolder, ExtendedEventFlags.None);
			}
			else
			{
				extendedEventFlags = NotificationEvents.ComputeFolderExtendedEventFlags(context, message.ParentFolder, ExtendedEventFlags.None);
				if (originalFolder != null)
				{
					extendedEventFlags |= NotificationEvents.ComputeFolderExtendedEventFlags(context, originalFolder, ExtendedEventFlags.None);
				}
			}
			extendedEventFlags &= (ExtendedEventFlags.NonIPMFolder | ExtendedEventFlags.IPMFolder | ExtendedEventFlags.InternalAccessFolder);
			if ((extendedEventFlags & (ExtendedEventFlags)((ulong)-2147483648)) == (ExtendedEventFlags)((ulong)-2147483648))
			{
				extendedEventFlags &= ~ExtendedEventFlags.NonIPMFolder;
			}
			if (0UL == (message.ChangedGroups & 4611686018427387904UL))
			{
				extendedEventFlags |= ExtendedEventFlags.NoReminderPropertyModified;
			}
			bool flag = message.ParentFolder.IsPartOfContentIndexing(context);
			if (0UL == (message.ChangedGroups & 2305843009213693952UL) || !flag)
			{
				extendedEventFlags |= ExtendedEventFlags.NoCIPropertyModified;
			}
			if (0UL != (message.ChangedGroups & 576460752303423488UL))
			{
				extendedEventFlags |= ExtendedEventFlags.RetentionTagModified;
			}
			if (0UL != (message.ChangedGroups & 288230376151711744UL))
			{
				extendedEventFlags |= ExtendedEventFlags.RetentionPropertiesModified;
			}
			if (0UL == (message.ChangedGroups & 144115188075855872UL))
			{
				extendedEventFlags |= ExtendedEventFlags.AppointmentTimeNotModified;
			}
			if (0UL == (message.ChangedGroups & 72057594037927936UL))
			{
				extendedEventFlags |= ExtendedEventFlags.AppointmentFreeBusyNotModified;
			}
			if (message.NeedsGroupExpansion(context))
			{
				extendedEventFlags |= ExtendedEventFlags.NeedGroupExpansion;
			}
			if (message.IsInferenceProcessingNeeded(context))
			{
				extendedEventFlags |= ExtendedEventFlags.InferenceProcessingNeeded;
			}
			if (0UL != (message.ChangedGroups & 18014398509481984UL))
			{
				extendedEventFlags |= ExtendedEventFlags.ModernRemindersChanged;
			}
			if (!flag)
			{
				extendedEventFlags |= ExtendedEventFlags.FolderIsNotPartOfContentIndexing;
			}
			if (0UL != (message.ChangedGroups & 9007199254740992UL) && message.TimerEventFired)
			{
				extendedEventFlags |= ExtendedEventFlags.TimerEventFired;
			}
			return extendedEventFlags;
		}

		private static ExtendedEventFlags ComputeFolderExtendedEventFlags(Context context, Folder folder, ExtendedEventFlags additionalFlags)
		{
			ExtendedEventFlags extendedEventFlags = additionalFlags | (ExtendedEventFlags)(folder.IsIpmFolder(context) ? ((ulong)int.MinValue) : 1073741824UL);
			if (!folder.IsPartOfContentIndexing(context))
			{
				extendedEventFlags |= ExtendedEventFlags.FolderIsNotPartOfContentIndexing;
			}
			if (folder.IsInternalAccess(context))
			{
				extendedEventFlags |= ExtendedEventFlags.InternalAccessFolder;
			}
			return extendedEventFlags;
		}
	}
}
