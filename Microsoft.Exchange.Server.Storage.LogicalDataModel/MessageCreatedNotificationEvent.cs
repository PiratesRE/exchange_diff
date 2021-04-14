using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class MessageCreatedNotificationEvent : ObjectCreatedModifiedNotificationEvent
	{
		public MessageCreatedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int documentId, int? conversationDocumentId, StorePropTag[] changedPropTags, string messageClass, Guid? userIdentityContext) : base(database, mailboxNumber, EventType.ObjectCreated, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, new int?(documentId), conversationDocumentId, changedPropTags, messageClass, userIdentityContext)
		{
			Statistics.NotificationTypes.MessageCreated.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("MessageCreatedNotificationEvent");
		}
	}
}
