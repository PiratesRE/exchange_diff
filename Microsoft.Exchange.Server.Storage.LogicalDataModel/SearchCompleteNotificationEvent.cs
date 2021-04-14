using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class SearchCompleteNotificationEvent : ObjectNotificationEvent
	{
		public SearchCompleteNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, ExchangeId fid) : base(database, mailboxNumber, EventType.SearchComplete, userIdentity, clientType, EventFlags.SearchFolder, fid, ExchangeId.Null, fid, null, null, null)
		{
			Statistics.NotificationTypes.SearchComplete.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("SearchCompleteNotificationEvent");
		}
	}
}
