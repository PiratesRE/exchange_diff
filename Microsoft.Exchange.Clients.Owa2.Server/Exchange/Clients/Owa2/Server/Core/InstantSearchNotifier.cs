using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantSearchNotifier : PendingRequestNotifierBase
	{
		internal InstantSearchNotifier(string subscriptionId, UserContext userContext) : base(subscriptionId, userContext)
		{
			this.payloadCollection = new List<InstantSearchNotificationPayload>(3);
		}

		public override bool ShouldThrottle
		{
			get
			{
				return false;
			}
		}

		protected override bool IsDataAvailableForPickup()
		{
			bool result;
			lock (this.payloadCollection)
			{
				result = (this.payloadCollection.Count > 0);
			}
			return result;
		}

		internal void AddPayload(InstantSearchNotificationPayload payload)
		{
			lock (this.payloadCollection)
			{
				payload.InstantSearchPayload.SearchPerfMarkerContainer.SetPerfMarker(InstantSearchPerfKey.NotificationQueuedTime);
				this.payloadCollection.Add(payload);
			}
			this.PickupData();
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			lock (this.payloadCollection)
			{
				foreach (InstantSearchNotificationPayload instantSearchNotificationPayload in this.payloadCollection)
				{
					instantSearchNotificationPayload.InstantSearchPayload.SearchPerfMarkerContainer.SetPerfMarker(InstantSearchPerfKey.NotificationPickupFromQueueTime);
				}
				list.AddRange(this.payloadCollection);
				this.payloadCollection.Clear();
			}
			return list;
		}

		private List<InstantSearchNotificationPayload> payloadCollection;
	}
}
