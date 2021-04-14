using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SubscriberConcurrencyTracker
	{
		public static SubscriberConcurrencyTracker Instance
		{
			get
			{
				return SubscriberConcurrencyTracker.instance;
			}
		}

		private SubscriberConcurrencyTracker()
		{
			this.groupToConcurrentStatsMap = new Dictionary<Tuple<string, NotificationType>, Tuple<long, int>>();
		}

		public void OnSubscribe(string mailbox, NotificationType notificationType)
		{
			Tuple<string, NotificationType> key = new Tuple<string, NotificationType>(mailbox, notificationType);
			long elapsedMilliseconds = this.groupConcurrencyStopWatch.ElapsedMilliseconds;
			long elapsedTime;
			int num;
			int num2;
			lock (this.groupConcurrencyStatsSyncObject)
			{
				Tuple<long, int> tuple;
				if (!this.groupToConcurrentStatsMap.TryGetValue(key, out tuple))
				{
					elapsedTime = 0L;
					num = 0;
				}
				else
				{
					elapsedTime = elapsedMilliseconds - tuple.Item1;
					num = tuple.Item2;
				}
				num2 = num + 1;
				this.groupToConcurrentStatsMap[key] = new Tuple<long, int>(elapsedMilliseconds, num2);
			}
			this.logEventQueue.Enqueue(new GroupConcurrencyLogEvent(mailbox, notificationType, elapsedTime, num, num2));
			this.StartGroupConcurrencyLogThread();
		}

		public void OnUnsubscribe(string mailbox, NotificationType notificationType)
		{
			this.OnUnsubscribe(mailbox, notificationType, 1);
		}

		public void OnUnsubscribe(string mailbox, NotificationType notificationType, int count)
		{
			Tuple<string, NotificationType> key = new Tuple<string, NotificationType>(mailbox, notificationType);
			long elapsedMilliseconds = this.groupConcurrencyStopWatch.ElapsedMilliseconds;
			long elapsedTime;
			int item;
			int num;
			lock (this.groupConcurrencyStatsSyncObject)
			{
				Tuple<long, int> tuple;
				if (!this.groupToConcurrentStatsMap.TryGetValue(key, out tuple))
				{
					return;
				}
				elapsedTime = elapsedMilliseconds - tuple.Item1;
				item = tuple.Item2;
				num = item - count;
				if (num == 0)
				{
					this.groupToConcurrentStatsMap.Remove(key);
				}
				else
				{
					this.groupToConcurrentStatsMap[key] = new Tuple<long, int>(elapsedMilliseconds, num);
				}
			}
			this.logEventQueue.Enqueue(new GroupConcurrencyLogEvent(mailbox, notificationType, elapsedTime, item, num));
			this.StartGroupConcurrencyLogThread();
		}

		internal void OnResubscribe(string mailbox, NotificationType notificationType)
		{
			Tuple<string, NotificationType> key = new Tuple<string, NotificationType>(mailbox, notificationType);
			long elapsedMilliseconds = this.groupConcurrencyStopWatch.ElapsedMilliseconds;
			Tuple<long, int> tuple;
			lock (this.groupConcurrencyStatsSyncObject)
			{
				if (!this.groupToConcurrentStatsMap.TryGetValue(key, out tuple))
				{
					return;
				}
			}
			this.logEventQueue.Enqueue(new GroupConcurrencyLogEvent(mailbox, notificationType, elapsedMilliseconds - tuple.Item1, tuple.Item2, tuple.Item2));
			this.StartGroupConcurrencyLogThread();
		}

		private void StartGroupConcurrencyLogThread()
		{
			if (this.loggerThreadAlive != 0)
			{
				return;
			}
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				while (Interlocked.CompareExchange(ref this.loggerThreadAlive, 1, 0) == 0)
				{
					ILogEvent logEvent;
					while (this.logEventQueue.TryDequeue(out logEvent))
					{
						OwaServerLogger.AppendToLog(logEvent);
					}
					Interlocked.Exchange(ref this.loggerThreadAlive, 0);
					if (this.logEventQueue.Count == 0)
					{
						return;
					}
				}
			});
		}

		private static readonly SubscriberConcurrencyTracker instance = new SubscriberConcurrencyTracker();

		private readonly object groupConcurrencyStatsSyncObject = new object();

		private readonly Dictionary<Tuple<string, NotificationType>, Tuple<long, int>> groupToConcurrentStatsMap;

		private readonly Stopwatch groupConcurrencyStopWatch = Stopwatch.StartNew();

		private readonly ConcurrentQueue<ILogEvent> logEventQueue = new ConcurrentQueue<ILogEvent>();

		private int loggerThreadAlive;
	}
}
