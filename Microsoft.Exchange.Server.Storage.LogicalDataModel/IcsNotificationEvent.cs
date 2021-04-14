using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class IcsNotificationEvent : ObjectNotificationEvent
	{
		public IcsNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags) : base(database, mailboxNumber, EventType.StatusObjectModified, userIdentity, clientType, eventFlags, ExchangeId.Null, ExchangeId.Null, ExchangeId.Null, null, null, null)
		{
			Statistics.NotificationTypes.Ics.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("IcsNotificationEvent");
		}
	}
}
