using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Net
{
	internal static class NetworkTimer
	{
		public static void Add(NetworkConnection nc)
		{
			lock (NetworkTimer.networkConnections)
			{
				NetworkTimer.networkConnections.Add(nc.ConnectionId, new WeakReference(nc));
			}
		}

		public static void Remove(NetworkConnection nc)
		{
			lock (NetworkTimer.networkConnections)
			{
				NetworkTimer.networkConnections.Remove(nc.ConnectionId);
			}
		}

		private static void CheckTimeouts(object ignored)
		{
			if (NetworkTimer.networkConnections.Count == 0)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(NetworkTimer.timeoutTimer))
				{
					NetworkTimer.timeoutTimer.Change(-1, -1);
					lock (NetworkTimer.networkConnections)
					{
						List<long> list = null;
						long ticks = DateTime.UtcNow.Ticks;
						foreach (KeyValuePair<long, WeakReference> keyValuePair in NetworkTimer.networkConnections)
						{
							NetworkConnection networkConnection = (NetworkConnection)keyValuePair.Value.Target;
							if (networkConnection != null)
							{
								networkConnection.CheckForTimeouts(ticks);
							}
							else
							{
								if (list == null)
								{
									list = new List<long>();
								}
								list.Add(keyValuePair.Key);
							}
						}
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								NetworkTimer.networkConnections.Remove(list[i]);
							}
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(NetworkTimer.timeoutTimer))
				{
					Monitor.Exit(NetworkTimer.timeoutTimer);
					NetworkTimer.timeoutTimer.Change(1000, 1000);
				}
			}
		}

		private const int TimeoutFrequency = 1000;

		private static Dictionary<long, WeakReference> networkConnections = new Dictionary<long, WeakReference>();

		private static Timer timeoutTimer = new Timer(new TimerCallback(NetworkTimer.CheckTimeouts), null, 1000, 1000);
	}
}
