using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderDeletedNotificationEvent : ObjectNotificationEvent
	{
		public FolderDeletedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId parentFid, string containerClass) : base(database, mailboxNumber, EventType.ObjectDeleted, userIdentity, clientType, eventFlags, extendedEventFlags, fid, ExchangeId.Null, parentFid, null, null, containerClass)
		{
			Statistics.NotificationTypes.FolderDeleted.Bump();
		}

		public override NotificationEvent.RedundancyStatus GetRedundancyStatus(NotificationEvent oldNev)
		{
			ObjectNotificationEvent objectNotificationEvent = oldNev as ObjectNotificationEvent;
			if (objectNotificationEvent != null)
			{
				if (base.IsSameObject(objectNotificationEvent))
				{
					if (objectNotificationEvent.EventType == EventType.ObjectModified)
					{
						return NotificationEvent.RedundancyStatus.DropOldAndStop;
					}
					if (objectNotificationEvent.EventType == EventType.ObjectCreated)
					{
						return NotificationEvent.RedundancyStatus.DropBothAndStop;
					}
					return NotificationEvent.RedundancyStatus.FlagStopSearch;
				}
				else if (objectNotificationEvent.EventType == EventType.ObjectCopied)
				{
					FolderCopiedNotificationEvent folderCopiedNotificationEvent = oldNev as FolderCopiedNotificationEvent;
					if (folderCopiedNotificationEvent != null && base.Fid == folderCopiedNotificationEvent.OldFid)
					{
						return NotificationEvent.RedundancyStatus.FlagStopSearch;
					}
				}
			}
			return NotificationEvent.RedundancyStatus.Continue;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("FolderDeletedNotificationEvent");
		}
	}
}
