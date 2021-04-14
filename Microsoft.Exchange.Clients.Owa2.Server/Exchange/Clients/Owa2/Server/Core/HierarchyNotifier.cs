using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class HierarchyNotifier : PendingRequestNotifierBase
	{
		internal HierarchyNotifier(string subscriptionId, UserContext userContext) : base(subscriptionId, userContext)
		{
			this.folderCountTable = new Dictionary<StoreObjectId, HierarchyNotificationPayload>();
		}

		public void Clear(bool clearRefreshPayload)
		{
			this.folderCountTable.Clear();
			if (clearRefreshPayload)
			{
				this.refreshPayload = null;
			}
		}

		internal void AddFolderCountPayload(StoreObjectId folderId, HierarchyNotificationPayload payload)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotifier.AddFolderCountPayload Start");
			lock (this)
			{
				if (this.refreshPayload == null)
				{
					this.folderCountTable[folderId] = payload;
					if (this.folderCountTable.Count > 200)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotifier.AddFolderCountPayload Exceeded maxfoldercount");
						this.AddRefreshPayload(payload.SubscriptionId);
					}
				}
			}
		}

		internal void AddRefreshPayload(string subscriptionId)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotifier.ReadDataAndResetStateInternal Start");
			lock (this)
			{
				this.Clear(false);
				if (this.refreshPayload == null)
				{
					this.refreshPayload = new HierarchyNotificationPayload
					{
						EventType = QueryNotificationType.Reload,
						SubscriptionId = subscriptionId
					};
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotifier.ReadDataAndResetStateInternal. User: {0}", base.UserContext.PrimarySmtpAddress);
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			if (this.refreshPayload != null)
			{
				list.Add(this.refreshPayload);
			}
			else if (this.folderCountTable.Count > 0)
			{
				foreach (KeyValuePair<StoreObjectId, HierarchyNotificationPayload> keyValuePair in this.folderCountTable)
				{
					list.Add(keyValuePair.Value);
				}
			}
			this.Clear(true);
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.folderCountTable.Count > 0 || this.refreshPayload != null;
		}

		private const int MaxFolderCountTableSize = 200;

		private Dictionary<StoreObjectId, HierarchyNotificationPayload> folderCountTable;

		private HierarchyNotificationPayload refreshPayload;
	}
}
