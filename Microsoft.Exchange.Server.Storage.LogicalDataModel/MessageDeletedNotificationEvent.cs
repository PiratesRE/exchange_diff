using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MessageDeletedNotificationEvent : ObjectNotificationEvent
	{
		public byte[] ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		public MessageDeletedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int documentId, int? conversationDocumentId, string messageClass, byte[] conversationId, Guid? userIdentityContext) : base(database, mailboxNumber, EventType.ObjectDeleted, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, new int?(documentId), conversationDocumentId, messageClass, userIdentityContext)
		{
			Statistics.NotificationTypes.MessageDeleted.Bump();
			this.conversationId = conversationId;
		}

		public override NotificationEvent.RedundancyStatus GetRedundancyStatus(NotificationEvent oldNev)
		{
			ObjectNotificationEvent objectNotificationEvent = oldNev as ObjectNotificationEvent;
			if (objectNotificationEvent != null)
			{
				if (base.IsSameObject(objectNotificationEvent))
				{
					if (objectNotificationEvent.EventType == EventType.ObjectModified)
					{
						return NotificationEvent.RedundancyStatus.DropOldAndStop;
					}
					if (objectNotificationEvent.EventType == EventType.ObjectCreated)
					{
						return NotificationEvent.RedundancyStatus.DropBothAndStop;
					}
					return NotificationEvent.RedundancyStatus.FlagStopSearch;
				}
				else if (objectNotificationEvent.EventType == EventType.ObjectCopied)
				{
					MessageCopiedNotificationEvent messageCopiedNotificationEvent = oldNev as MessageCopiedNotificationEvent;
					if (messageCopiedNotificationEvent != null && base.Mid == messageCopiedNotificationEvent.OldMid && base.Fid == messageCopiedNotificationEvent.OldFid)
					{
						return NotificationEvent.RedundancyStatus.FlagStopSearch;
					}
				}
			}
			return NotificationEvent.RedundancyStatus.Continue;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MessageDeletedNotificationEvent");
		}

		private byte[] conversationId;
	}
}
