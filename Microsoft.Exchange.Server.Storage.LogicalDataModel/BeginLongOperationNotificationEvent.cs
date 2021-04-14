using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class BeginLongOperationNotificationEvent : ObjectNotificationEvent
	{
		public BeginLongOperationNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExchangeId fid) : base(database, mailboxNumber, EventType.BeginLongOperation, userIdentity, clientType, eventFlags, fid, ExchangeId.Null, fid, null, null, null)
		{
			Statistics.NotificationTypes.BeginLongOperation.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("BeginLongOperationNotificationEvent");
		}
	}
}
