using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MessageMovedNotificationEvent : ObjectMovedCopiedNotificationEvent
	{
		public MessageMovedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int documentId, int? conversationDocumentId, ExchangeId oldFid, ExchangeId oldMid, ExchangeId oldParentFid, int? oldConversationDocumentId, string messageClass) : base(database, mailboxNumber, EventType.ObjectMoved, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, new int?(documentId), conversationDocumentId, oldFid, oldMid, oldParentFid, oldConversationDocumentId, messageClass)
		{
			Statistics.NotificationTypes.MessageMoved.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MessageMovedNotificationEvent");
		}
	}
}
