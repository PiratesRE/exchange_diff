using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class CategorisedRowModifiedNotificationEvent : CategorisedViewNotificationEvent
	{
		public CategorisedRowModifiedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid) : base(database, mailboxNumber, EventType.CategRowModified, userIdentity, clientType, eventFlags, fid, mid, parentFid)
		{
			Statistics.NotificationTypes.CategorizedRowModified.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("CategorisedRowModifiedNotificationEvent");
		}
	}
}
