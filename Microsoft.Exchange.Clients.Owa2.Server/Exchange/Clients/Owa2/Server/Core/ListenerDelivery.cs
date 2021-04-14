using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ListenerDelivery
	{
		internal ListenerDelivery(ListenerChannelsManager listenerChannelsManager)
		{
			this.listenerChannelsManager = listenerChannelsManager;
		}

		internal string[] DeliverRemoteNotification(IEnumerable<string> channelIds, RemoteNotificationPayload remoteNotificationPayload)
		{
			List<string> list = new List<string>();
			Dictionary<PendingRequestManager, bool> dictionary = new Dictionary<PendingRequestManager, bool>();
			foreach (string text in channelIds)
			{
				PendingRequestManager pendingGetManager = this.listenerChannelsManager.GetPendingGetManager(text);
				if (pendingGetManager == null)
				{
					list.Add(text);
				}
				else if (!dictionary.ContainsKey(pendingGetManager))
				{
					pendingGetManager.GetRemoteNotifier.AddRemoteNotificationPayload(remoteNotificationPayload);
					pendingGetManager.GetRemoteNotifier.PickupData();
					dictionary.Add(pendingGetManager, true);
				}
			}
			return list.ToArray();
		}

		private ListenerChannelsManager listenerChannelsManager;
	}
}
