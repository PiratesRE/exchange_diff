using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IActivitySession
	{
		void CaptureActivity(ActivityId activityId, StoreObjectId itemId, StoreObjectId previousItemId, IDictionary<string, string> customProperties);

		void CaptureActivityBeforeItemChange(ItemChangeOperation operation, StoreId objectId, CoreItem item);

		void CaptureActivityAfterFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, IList<StoreObjectId> itemIdsBeforeChange, IList<StoreObjectId> itemIdsAfterChange, StoreObjectId sourceFolder, StoreObjectId targetFolder);

		void CaptureMarkAsUnread(ICollection<StoreObjectId> itemIds);

		void CaptureMarkAsUnread(ICoreItem item);

		void CaptureMarkAsRead(ICollection<StoreObjectId> itemIds);

		void CaptureMarkAsRead(ICoreItem item);

		void CaptureDelivery(StoreObjectId storeObjectId, IDictionary<string, string> deliveryActivityInfo);

		void CaptureMarkAsClutterOrNotClutter(Dictionary<StoreObjectId, bool> itemClutterActions);

		void CaptureRemoteSend(StoreObjectId storeObjectId);

		void CaptureMessageSent(StoreObjectId itemId, string itemSchemaType);

		void CaptureCalendarEventActivity(ActivityId activityId, StoreObjectId id);

		void CaptureClutterNotificationSent(StoreObjectId itemId, IDictionary<string, string> messageProperties);

		void CaptureServerLogonActivity(string result, string exceptionInfo, string userName, string clientIP, string userAgent);
	}
}
