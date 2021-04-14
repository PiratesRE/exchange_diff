using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MailboxFoldersSubscription : NotificationSubscription
	{
		public MailboxFoldersSubscription(SubscriptionKind kind, NotificationContext notificationContext, StoreDatabase database, int mailboxNumber, EventType eventTypeMask, NotificationCallback callback) : base(kind, notificationContext, database, mailboxNumber, (int)eventTypeMask, callback)
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
			if ((nev.EventTypeValue & 8126716) != 0)
			{
				ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
				return objectNotificationEvent != null && objectNotificationEvent.IsFolderEvent;
			}
			return false;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MailboxFoldersSubscription");
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
