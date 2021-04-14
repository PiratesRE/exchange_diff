using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderMovedNotificationEvent : ObjectMovedCopiedNotificationEvent
	{
		public FolderMovedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId parentFid, ExchangeId oldFid, ExchangeId oldParentFid, string containerClass) : base(database, mailboxNumber, EventType.ObjectMoved, userIdentity, clientType, eventFlags, extendedEventFlags, fid, ExchangeId.Null, parentFid, null, null, oldFid, ExchangeId.Null, oldParentFid, null, containerClass)
		{
			Statistics.NotificationTypes.FolderMoved.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("FolderMovedNotificationEvent");
		}
	}
}
