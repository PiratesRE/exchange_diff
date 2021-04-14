using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal static class DnsTimer
	{
		public static void RegisterTimer(Request resolver, DateTime timeout)
		{
			int num = DnsTimer.CalculateBucket(timeout);
			ExTraceGlobals.DNSTracer.TraceDebug<int>((long)resolver.GetHashCode(), "RegisterTimer {0}", num);
			lock (DnsTimer.timerDictionary)
			{
				List<Request> list;
				if (!DnsTimer.timerDictionary.TryGetValue(num, out list))
				{
					list = new List<Request>();
					DnsTimer.timerDictionary.Add(num, list);
				}
				list.Add(resolver);
			}
		}

		private static void CheckTimeouts(object ignored)
		{
			if (DnsTimer.timerDictionary.Count == 0)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(DnsTimer.timeoutTimer))
				{
					DnsTimer.timeoutTimer.Change(-1, -1);
					int num = DnsTimer.CalculateBucket(DateTime.UtcNow);
					ExTraceGlobals.DNSTracer.TraceDebug<int, int>(0L, "DnsTimer, count {0}, it is now {1}", DnsTimer.timerDictionary.Count, num);
					List<int> list = null;
					List<List<Request>> list2 = null;
					lock (DnsTimer.timerDictionary)
					{
						foreach (KeyValuePair<int, List<Request>> keyValuePair in DnsTimer.timerDictionary)
						{
							if (keyValuePair.Key < num)
							{
								if (list == null)
								{
									list = new List<int>();
									list2 = new List<List<Request>>();
								}
								list.Add(keyValuePair.Key);
								list2.Add(keyValuePair.Value);
							}
						}
						if (list != null)
						{
							foreach (int num2 in list)
							{
								ExTraceGlobals.DNSTracer.TraceDebug<int>(0L, "CheckTimeouts expiring bucket {0}", num2);
								DnsTimer.timerDictionary.Remove(num2);
							}
						}
					}
					if (list2 != null)
					{
						foreach (List<Request> list3 in list2)
						{
							foreach (Request request in list3)
							{
								request.CheckForTimeout();
							}
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(DnsTimer.timeoutTimer))
				{
					Monitor.Exit(DnsTimer.timeoutTimer);
					DnsTimer.timeoutTimer.Change(1000, 1000);
				}
			}
		}

		private static int CalculateBucket(DateTime timeout)
		{
			return (int)((timeout - DnsTimer.TimeZero).Ticks / 10000000L);
		}

		private const int TimeoutFrequency = 1000;

		private static readonly DateTime TimeZero = DateTime.UtcNow;

		private static Timer timeoutTimer = new Timer(new TimerCallback(DnsTimer.CheckTimeouts), null, 1000, 1000);

		private static Dictionary<int, List<Request>> timerDictionary = new Dictionary<int, List<Request>>();
	}
}
