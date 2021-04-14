using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MessageModifiedNotificationEvent : ObjectCreatedModifiedNotificationEvent
	{
		public MessageModifiedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int documentId, int? conversationDocumentId, int? oldConversationDocumentId, StorePropTag[] changedPropTags, string messageClass, Guid? userIdentityContext) : base(database, mailboxNumber, EventType.ObjectModified, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, new int?(documentId), conversationDocumentId, changedPropTags, messageClass, userIdentityContext)
		{
			this.oldConversationDocumentId = oldConversationDocumentId;
			Statistics.NotificationTypes.MessageModified.Bump();
		}

		public int? OldConversationDocumentId
		{
			get
			{
				return this.oldConversationDocumentId;
			}
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
						MessageModifiedNotificationEvent messageModifiedNotificationEvent = oldNev as MessageModifiedNotificationEvent;
						if (ObjectCreatedModifiedNotificationEvent.PropTagArraysEqual(messageModifiedNotificationEvent.ChangedPropTags, base.ChangedPropTags))
						{
							return NotificationEvent.RedundancyStatus.ReplaceOldAndStop;
						}
						return NotificationEvent.RedundancyStatus.MergeReplaceOldAndStop;
					}
					else
					{
						if (objectNotificationEvent.EventType == EventType.ObjectCreated)
						{
							return NotificationEvent.RedundancyStatus.DropNewAndStop;
						}
						return NotificationEvent.RedundancyStatus.FlagStopSearch;
					}
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

		public override NotificationEvent MergeWithOldEvent(NotificationEvent oldNev)
		{
			return new MessageModifiedNotificationEvent(base.Database, base.MailboxNumber, base.UserIdentity, base.ClientType, base.EventFlags, (base.ExtendedEventFlags != null) ? base.ExtendedEventFlags.Value : Microsoft.Exchange.Server.Storage.LogicalDataModel.ExtendedEventFlags.None, base.Fid, base.Mid, base.ParentFid, base.DocumentId.Value, base.ConversationDocumentId, this.OldConversationDocumentId, ObjectCreatedModifiedNotificationEvent.MergeChangedPropTagArrays((oldNev as MessageModifiedNotificationEvent).ChangedPropTags, base.ChangedPropTags), base.ObjectClass, base.UserIdentityContext);
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MessageModifiedNotificationEvent");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" OldConversationDocumentId:[");
			sb.Append(this.oldConversationDocumentId);
			sb.Append("]");
		}

		private readonly int? oldConversationDocumentId;
	}
}
