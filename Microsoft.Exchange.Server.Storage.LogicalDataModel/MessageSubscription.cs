using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MessageSubscription : NotificationSubscription
	{
		public MessageSubscription(SubscriptionKind kind, NotificationContext notificationContext, StoreDatabase database, int mailboxNumber, EventType eventTypeMask, NotificationCallback callback, ExchangeId fid, ExchangeId mid) : base(kind, notificationContext, database, mailboxNumber, (int)eventTypeMask, callback)
		{
			this.fid = fid;
			this.mid = mid;
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

		public ExchangeId Mid
		{
			get
			{
				return this.mid;
			}
		}

		public override bool IsInterested(NotificationEvent nev)
		{
			if ((nev.EventTypeValue & 16777336) != 0)
			{
				ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
				if (objectNotificationEvent != null && objectNotificationEvent.IsMessageEvent)
				{
					if ((objectNotificationEvent.EventType & (EventType.ObjectDeleted | EventType.ObjectModified | EventType.MessageUnlinked)) != (EventType)0)
					{
						if (objectNotificationEvent.Fid == this.fid && objectNotificationEvent.Mid == this.mid)
						{
							return true;
						}
					}
					else
					{
						ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent = objectNotificationEvent as ObjectMovedCopiedNotificationEvent;
						if (objectMovedCopiedNotificationEvent != null && objectMovedCopiedNotificationEvent.OldFid == this.fid && objectMovedCopiedNotificationEvent.OldMid == this.mid)
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
			sb.Append("MessageSubscription");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" EventTypeMask:[");
			sb.Append(this.EventTypeMask);
			sb.Append("] Fid:[");
			sb.Append(this.Fid);
			sb.Append("] Mid:[");
			sb.Append(this.Mid);
			sb.Append("]");
		}

		private ExchangeId fid;

		private ExchangeId mid;
	}
}
