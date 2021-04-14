using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateGroupNotifier : PendingRequestNotifierBase
	{
		internal CreateGroupNotifier(IMailboxContext userContext) : base(CreateGroupNotifier.CreateGroupNotifierId, userContext)
		{
		}

		internal CreateGroupNotificationPayload Payload
		{
			get
			{
				CreateGroupNotificationPayload result;
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

		public static string CreateGroupNotifierId = NotificationType.GroupCreateNotification.ToString();

		private readonly object lockObject = new object();

		private CreateGroupNotificationPayload payload;
	}
}
