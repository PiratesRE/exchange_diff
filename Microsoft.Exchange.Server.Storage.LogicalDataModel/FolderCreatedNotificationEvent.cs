using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderCreatedNotificationEvent : ObjectCreatedModifiedNotificationEvent
	{
		public FolderCreatedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId parentFid, StorePropTag[] changedPropTags, string containerClass) : base(database, mailboxNumber, EventType.ObjectCreated, userIdentity, clientType, eventFlags, extendedEventFlags, fid, ExchangeId.Null, parentFid, null, null, changedPropTags, containerClass, null)
		{
			Statistics.NotificationTypes.FolderCreated.Bump();
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("FolderCreatedNotificationEvent");
		}
	}
}
