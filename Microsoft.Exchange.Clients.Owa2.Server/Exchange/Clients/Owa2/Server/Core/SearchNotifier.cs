using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SearchNotifier : PendingRequestNotifierBase
	{
		internal SearchNotifier(IMailboxContext userContext) : base("SearchNotification", userContext)
		{
		}

		internal SearchNotificationPayload Payload
		{
			get
			{
				SearchNotificationPayload result;
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

		public const string SearchId = "SearchNotification";

		private SearchNotificationPayload payload;

		private object lockObject = new object();
	}
}
