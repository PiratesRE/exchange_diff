using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MailboxDisconnectedNotificationEvent : MailboxNotificationEvent
	{
		public MailboxDisconnectedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags) : base(database, mailboxNumber, EventType.MailboxDisconnected, userIdentity, clientType, eventFlags)
		{
			Statistics.NotificationTypes.MailboxDisconnected.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MailboxDisconnectedNotificationEvent");
		}
	}
}
