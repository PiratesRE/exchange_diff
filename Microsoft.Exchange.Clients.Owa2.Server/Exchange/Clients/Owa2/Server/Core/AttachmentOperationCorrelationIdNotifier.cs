using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentOperationCorrelationIdNotifier : PendingRequestNotifierBase
	{
		internal AttachmentOperationCorrelationIdNotifier(UserContext userContext, string subscriptionId) : base(subscriptionId, userContext)
		{
		}

		internal AttachmentOperationCorrelationIdNotificationPayload Payload
		{
			get
			{
				AttachmentOperationCorrelationIdNotificationPayload result;
				lock (this.lockObject)
				{
					result = this.payload;
				}
				return result;
			}
			set
			{
				lock (this.lockObject)
				{
					this.payload = value;
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			lock (this.lockObject)
			{
				if (this.Payload != null)
				{
					list.Add(this.Payload);
					this.Payload = null;
				}
			}
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.Payload != null;
		}

		private AttachmentOperationCorrelationIdNotificationPayload payload;

		private object lockObject = new object();
	}
}
