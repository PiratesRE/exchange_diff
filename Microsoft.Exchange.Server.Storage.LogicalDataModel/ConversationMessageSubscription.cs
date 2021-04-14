using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationMessageSubscription : NotificationSubscription
	{
		public ConversationMessageSubscription(SubscriptionKind kind, NotificationContext notificationContext, StoreDatabase database, int mailboxNumber, EventType eventTypeMask, NotificationCallback callback) : base(kind, notificationContext, database, mailboxNumber, (int)eventTypeMask, callback)
		{
		}

		public EventType EventTypeMask
		{
			get
			{
				return (EventType)base.EventTypeValueMask;
			}
		}

		public override bool IsInterested(NotificationEvent nev)
		{
			bool result = false;
			EventType eventType = EventType.NewMail | EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved | EventType.ObjectCopied;
			if ((nev.EventTypeValue & (int)eventType) != 0)
			{
				ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
				if (objectNotificationEvent != null && objectNotificationEvent.IsMessageEvent && (objectNotificationEvent.EventType & eventType) != (EventType)0 && (objectNotificationEvent.EventFlags & (EventFlags.SearchFolder | EventFlags.Conversation)) == EventFlags.None)
				{
					result = true;
				}
			}
			return result;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("ConversationMessageSubscription");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" EventTypeMask:[");
			sb.Append(this.EventTypeMask);
			sb.Append("]");
		}
	}
}
