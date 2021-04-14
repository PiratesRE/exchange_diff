using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class SearchPopulationMessagesLinkedNotificationEvent : ObjectNotificationEvent
	{
		public SearchPopulationMessagesLinkedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, ExchangeId fid) : base(database, mailboxNumber, EventType.MessagesLinked, userIdentity, clientType, EventFlags.SearchFolder, fid, ExchangeId.Null, fid, null, null, null)
		{
			Statistics.NotificationTypes.MessagesLinked.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("SearchPopulationMessagesLinkedNotificationEvent");
		}
	}
}
