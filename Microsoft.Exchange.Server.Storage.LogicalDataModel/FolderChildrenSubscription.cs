using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderChildrenSubscription : NotificationSubscription
	{
		public FolderChildrenSubscription(SubscriptionKind kind, NotificationContext notificationContext, StoreDatabase database, int mailboxNumber, EventType eventTypeMask, NotificationCallback callback, ExchangeId fid) : base(kind, notificationContext, database, mailboxNumber, (int)eventTypeMask, callback)
		{
			this.fid = fid;
		}

		public EventType EventTypeMask
		{
			get
			{
				return (EventType)base.EventTypeValueMask;
			}
		}

		public ExchangeId Fid
		{
			get
			{
				return this.fid;
			}
		}

		public override bool IsInterested(NotificationEvent nev)
		{
			if ((nev.EventTypeValue & 92012670) != 0)
			{
				ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
				if (objectNotificationEvent != null)
				{
					if ((objectNotificationEvent.EventType & (EventType.NewMail | EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.CategRowAdded | EventType.CategRowModified | EventType.CategRowDeleted | EventType.BeginLongOperation | EventType.EndLongOperation | EventType.MessageUnlinked)) != (EventType)0)
					{
						if (objectNotificationEvent.ParentFid == this.fid)
						{
							return true;
						}
					}
					else if ((objectNotificationEvent.EventType & EventType.MessagesLinked) != (EventType)0)
					{
						if (objectNotificationEvent.Fid == this.fid)
						{
							return true;
						}
					}
					else
					{
						if (objectNotificationEvent.ParentFid == this.fid)
						{
							return true;
						}
						ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent = objectNotificationEvent as ObjectMovedCopiedNotificationEvent;
						if (objectMovedCopiedNotificationEvent != null && objectMovedCopiedNotificationEvent.OldParentFid == this.fid)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("FolderChildrenSubscription");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" EventTypeMask:[");
			sb.Append(this.EventTypeMask);
			sb.Append("] Fid:[");
			sb.Append(this.Fid);
			sb.Append("]");
		}

		private ExchangeId fid;
	}
}
