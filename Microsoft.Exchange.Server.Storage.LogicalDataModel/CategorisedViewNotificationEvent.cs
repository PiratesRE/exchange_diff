using System;
using System.Security.Principal;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class CategorisedViewNotificationEvent : ObjectNotificationEvent
	{
		public CategorisedViewNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid) : base(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, fid, mid, parentFid, null, null, null)
		{
		}
	}
}
