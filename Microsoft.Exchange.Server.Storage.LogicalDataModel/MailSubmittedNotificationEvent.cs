using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MailSubmittedNotificationEvent : ObjectNotificationEvent
	{
		public MailSubmittedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, int documentId, int? conversationDocumentId, string messageClass) : base(database, mailboxNumber, EventType.MailSubmitted, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, fid, new int?(documentId), conversationDocumentId, messageClass)
		{
			Statistics.NotificationTypes.MailSubmitted.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MailSubmittedNotificationEvent");
		}
	}
}
