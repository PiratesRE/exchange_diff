using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ListenerChannelsManager
	{
		public static ListenerChannelsManager Instance
		{
			get
			{
				return ListenerChannelsManager.instance;
			}
		}

		public static string GeneratedChannelId()
		{
			return Guid.NewGuid().ToString();
		}

		public void AddPendingGetChannel(string channelId, PendingRequestManager pendingRequestManager)
		{
			lock (this.syncRoot)
			{
				if (!this.channelIdChannelsMapping.ContainsKey(channelId))
				{
					this.channelIdChannelsMapping.Add(channelId, pendingRequestManager);
				}
			}
		}

		public void RemovePendingGetChannel(string channelId)
		{
			lock (this.syncRoot)
			{
				this.channelIdChannelsMapping.Remove(channelId);
			}
		}

		public PendingRequestManager GetPendingGetManager(string channelId)
		{
			PendingRequestManager result = null;
			this.channelIdChannelsMapping.TryGetValue(channelId, out result);
			return result;
		}

		private static ListenerChannelsManager instance = new ListenerChannelsManager();

		private readonly object syncRoot = new object();

		private readonly Dictionary<string, PendingRequestManager> channelIdChannelsMapping = new Dictionary<string, PendingRequestManager>();
	}
}
