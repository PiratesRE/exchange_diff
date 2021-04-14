using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MailboxMoveFailedNotificationEvent : MailboxNotificationEvent
	{
		public MailboxMoveFailedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags) : base(database, mailboxNumber, EventType.MailboxMoveFailed, userIdentity, clientType, eventFlags)
		{
			Statistics.NotificationTypes.MailboxMoveFailed.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MailboxMoveFailedNotificationEvent");
		}
	}
}
