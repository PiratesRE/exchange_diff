using System;
using System.Security.Principal;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class ReliableEventNotificationSubscriber
	{
		public static void Subscribe(Context context)
		{
			ReliableEventNotificationSubscriber.subscription = new ReliableEventPublishingSubscription(new NotificationCallback(ReliableEventNotificationSubscriber.PublishReliableEvent));
			ReliableEventNotificationSubscriber.subscription.Register(context);
		}

		public static void Unsubscribe()
		{
			if (ReliableEventNotificationSubscriber.subscription != null)
			{
				ReliableEventNotificationSubscriber.subscription.Unregister();
				ReliableEventNotificationSubscriber.subscription = null;
			}
		}

		private static void PublishReliableEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev)
		{
			string objectClass = null;
			byte[] fid = null;
			byte[] mid = null;
			byte[] parentFid = null;
			byte[] oldFid = null;
			byte[] oldMid = null;
			byte[] oldParentFid = null;
			int? itemCount = null;
			int? unreadCount = null;
			ExtendedEventFlags? extendedFlags = null;
			byte[] array = null;
			int? documentId = null;
			EventType eventType;
			int mailboxNumber;
			ClientType clientType;
			SecurityIdentifier securityIdentifier;
			EventFlags eventFlags;
			if ((nev.EventTypeValue & 33814528) != 0)
			{
				MailboxNotificationEvent mailboxNotificationEvent = nev as MailboxNotificationEvent;
				eventType = mailboxNotificationEvent.EventType;
				if (eventType == EventType.MailboxModified)
				{
					eventType = EventType.ObjectModified;
				}
				mailboxNumber = mailboxNotificationEvent.MailboxNumber;
				clientType = mailboxNotificationEvent.ClientType;
				securityIdentifier = ((mailboxNotificationEvent.UserIdentity == null) ? null : mailboxNotificationEvent.UserIdentity.User);
				eventFlags = mailboxNotificationEvent.EventFlags;
			}
			else if ((nev.EventTypeValue & 16778270) != 0)
			{
				ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
				if ((objectNotificationEvent.EventFlags & EventFlags.Conversation) != EventFlags.None)
				{
					return;
				}
				eventType = objectNotificationEvent.EventType;
				if (eventType == EventType.MessageUnlinked)
				{
					eventType = EventType.ObjectDeleted;
				}
				mailboxNumber = objectNotificationEvent.MailboxNumber;
				objectClass = objectNotificationEvent.ObjectClass;
				fid = objectNotificationEvent.Fid.To24ByteArray();
				mid = (objectNotificationEvent.Mid.IsValid ? objectNotificationEvent.Mid.To24ByteArray() : null);
				parentFid = (objectNotificationEvent.ParentFid.IsValid ? objectNotificationEvent.ParentFid.To24ByteArray() : null);
				documentId = objectNotificationEvent.DocumentId;
				clientType = objectNotificationEvent.ClientType;
				securityIdentifier = ((objectNotificationEvent.UserIdentity == null) ? null : objectNotificationEvent.UserIdentity.User);
				eventFlags = objectNotificationEvent.EventFlags;
				extendedFlags = objectNotificationEvent.ExtendedEventFlags;
				if (objectNotificationEvent.EventType == EventType.ObjectModified && objectNotificationEvent.IsFolderEvent)
				{
					FolderModifiedNotificationEvent folderModifiedNotificationEvent = objectNotificationEvent as FolderModifiedNotificationEvent;
					if (folderModifiedNotificationEvent != null)
					{
						itemCount = ((folderModifiedNotificationEvent.MessageCount < 0) ? null : new int?(folderModifiedNotificationEvent.MessageCount));
						unreadCount = ((folderModifiedNotificationEvent.UnreadMessageCount < 0) ? null : new int?(folderModifiedNotificationEvent.UnreadMessageCount));
					}
				}
			}
			else
			{
				if ((nev.EventTypeValue & 96) == 0)
				{
					return;
				}
				ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent = nev as ObjectMovedCopiedNotificationEvent;
				eventType = objectMovedCopiedNotificationEvent.EventType;
				mailboxNumber = objectMovedCopiedNotificationEvent.MailboxNumber;
				objectClass = objectMovedCopiedNotificationEvent.ObjectClass;
				fid = objectMovedCopiedNotificationEvent.Fid.To24ByteArray();
				mid = (objectMovedCopiedNotificationEvent.Mid.IsValid ? objectMovedCopiedNotificationEvent.Mid.To24ByteArray() : null);
				parentFid = (objectMovedCopiedNotificationEvent.ParentFid.IsValid ? objectMovedCopiedNotificationEvent.ParentFid.To24ByteArray() : null);
				documentId = objectMovedCopiedNotificationEvent.DocumentId;
				oldFid = objectMovedCopiedNotificationEvent.OldFid.To24ByteArray();
				oldMid = (objectMovedCopiedNotificationEvent.OldMid.IsValid ? objectMovedCopiedNotificationEvent.OldMid.To24ByteArray() : null);
				oldParentFid = (objectMovedCopiedNotificationEvent.OldParentFid.IsValid ? objectMovedCopiedNotificationEvent.OldParentFid.To24ByteArray() : null);
				clientType = objectMovedCopiedNotificationEvent.ClientType;
				securityIdentifier = ((objectMovedCopiedNotificationEvent.UserIdentity == null) ? null : objectMovedCopiedNotificationEvent.UserIdentity.User);
				eventFlags = objectMovedCopiedNotificationEvent.EventFlags;
				extendedFlags = objectMovedCopiedNotificationEvent.ExtendedEventFlags;
			}
			if (securityIdentifier != null)
			{
				array = new byte[securityIdentifier.BinaryLength];
				securityIdentifier.GetBinaryForm(array, 0);
			}
			EventHistory eventHistory = EventHistory.GetEventHistory(transactionContext.Database);
			long num;
			eventHistory.InsertOneEvent(transactionContext, 0, eventType, mailboxNumber, objectClass, fid, mid, parentFid, oldFid, oldMid, oldParentFid, itemCount, unreadCount, eventFlags, extendedFlags, clientType, array, documentId, out num);
		}

		private static ReliableEventPublishingSubscription subscription;
	}
}
