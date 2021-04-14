using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PlayOnPhoneNotifier : PendingRequestNotifierBase
	{
		public PlayOnPhoneNotifier(UserContext callContext) : base(callContext)
		{
		}

		public void ConnectionAliveTimer()
		{
		}

		public override IList<NotificationPayloadBase> ReadDataAndResetState()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			lock (this)
			{
				foreach (NotificationPayloadBase item in this.payloadList)
				{
					list.Add(item);
				}
				this.payloadList.Clear();
			}
			return list;
		}

		internal virtual void NotifyStateChange(PlayOnPhoneNotificationPayload payload)
		{
			lock (this)
			{
				this.payloadList.Add(payload);
				base.FireDataAvailableEvent();
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			return null;
		}

		private List<NotificationPayloadBase> payloadList = new List<NotificationPayloadBase>();
	}
}
