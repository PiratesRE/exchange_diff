using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderModifiedNotificationEvent : ObjectCreatedModifiedNotificationEvent
	{
		public FolderModifiedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId parentFid, StorePropTag[] changedPropTags, string containerClass, int messageCount, int unreadMessageCount) : base(database, mailboxNumber, EventType.ObjectModified, userIdentity, clientType, eventFlags, extendedEventFlags, fid, ExchangeId.Null, parentFid, null, null, changedPropTags, containerClass, null)
		{
			Statistics.NotificationTypes.FolderModified.Bump();
			this.messageCount = messageCount;
			this.unreadMessageCount = unreadMessageCount;
		}

		public int MessageCount
		{
			get
			{
				return this.messageCount;
			}
		}

		public int UnreadMessageCount
		{
			get
			{
				return this.unreadMessageCount;
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
						FolderModifiedNotificationEvent folderModifiedNotificationEvent = oldNev as FolderModifiedNotificationEvent;
						if (ObjectCreatedModifiedNotificationEvent.PropTagArraysEqual(folderModifiedNotificationEvent.ChangedPropTags, base.ChangedPropTags))
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
					FolderCopiedNotificationEvent folderCopiedNotificationEvent = oldNev as FolderCopiedNotificationEvent;
					if (folderCopiedNotificationEvent != null && base.Fid == folderCopiedNotificationEvent.OldFid)
					{
						return NotificationEvent.RedundancyStatus.FlagStopSearch;
					}
				}
			}
			return NotificationEvent.RedundancyStatus.Continue;
		}

		public override NotificationEvent MergeWithOldEvent(NotificationEvent oldNev)
		{
			return new FolderModifiedNotificationEvent(base.Database, base.MailboxNumber, base.UserIdentity, base.ClientType, base.EventFlags, (base.ExtendedEventFlags != null) ? base.ExtendedEventFlags.Value : Microsoft.Exchange.Server.Storage.LogicalDataModel.ExtendedEventFlags.None, base.Fid, base.ParentFid, ObjectCreatedModifiedNotificationEvent.MergeChangedPropTagArrays((oldNev as FolderModifiedNotificationEvent).ChangedPropTags, base.ChangedPropTags), base.ObjectClass, this.MessageCount, this.UnreadMessageCount);
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("FolderModifiedNotificationEvent");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" MessageCount:[");
			sb.Append(this.messageCount);
			sb.Append("] UnreadMessageCount:[");
			sb.Append(this.unreadMessageCount);
			sb.Append("]");
		}

		private int messageCount;

		private int unreadMessageCount;
	}
}
