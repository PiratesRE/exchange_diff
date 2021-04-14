using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class CategorisedRowDeletedNotificationEvent : CategorisedViewNotificationEvent
	{
		public CategorisedRowDeletedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid) : base(database, mailboxNumber, EventType.CategRowDeleted, userIdentity, clientType, eventFlags, fid, mid, parentFid)
		{
			Statistics.NotificationTypes.CategorizedRowDeleted.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("CategorisedRowDeletedNotificationEvent");
		}
	}
}
