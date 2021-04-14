using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MailboxMoveStartedNotificationEvent : MailboxNotificationEvent
	{
		public MailboxMoveStartedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags) : base(database, mailboxNumber, EventType.MailboxMoveStarted, userIdentity, clientType, eventFlags)
		{
			Statistics.NotificationTypes.MailboxMoveStarted.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MailboxMoveStartedNotificationEvent");
		}
	}
}
