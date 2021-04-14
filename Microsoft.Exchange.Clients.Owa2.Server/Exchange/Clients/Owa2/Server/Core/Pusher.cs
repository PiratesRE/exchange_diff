using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class Pusher
	{
		public static Pusher Instance
		{
			get
			{
				return Pusher.instance;
			}
		}

		protected Pusher()
		{
			this.remoteNotificationManager = this.GetRemoteNotificationManager();
			this.remoteNotificationRequester = this.GetRemoteNotificationRequester();
		}

		public void Distribute(List<NotificationPayloadBase> payloads, string subscriptionContextKey, string subscriptionId)
		{
			if (payloads != null && payloads.Count > 0)
			{
				IEnumerable<IDestinationInfo> destinations = this.remoteNotificationManager.GetDestinations(subscriptionContextKey, subscriptionId);
				int num = 0;
				lock (this.syncRoot)
				{
					foreach (IDestinationInfo destinationInfo in destinations)
					{
						num++;
						this.AddToQueue(destinationInfo, payloads);
					}
				}
				OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.Distribute)
				{
					OriginationUserContextKey = subscriptionContextKey,
					DestinationCount = num,
					PayloadCount = payloads.Count
				});
				if (ExTraceGlobals.NotificationsCallTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					IEnumerable<string> values = from destination in destinations
					select string.Format("[Destination:{0}; ChannelId:{1}]", destination.Destination, string.Join(",", destination.ChannelIds));
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Pusher - Distribute notification from mailbox referred by user context key {0} to destinations {1}.", subscriptionContextKey, string.Join(",", values));
				}
			}
		}

		public virtual RemoteNotificationManager GetRemoteNotificationManager()
		{
			return RemoteNotificationManager.Instance;
		}

		public virtual RemoteNotificationRequester GetRemoteNotificationRequester()
		{
			return RemoteNotificationRequester.Instance;
		}

		public virtual PusherQueue CreatePusherQueue(IDestinationInfo destinationInfo)
		{
			return new PusherQueue(destinationInfo.Destination, new Action<PusherQueue>(this.QueueReady));
		}

		private void AddToQueue(IDestinationInfo destinationInfo, List<NotificationPayloadBase> payloads)
		{
			PusherQueue pusherQueue;
			if (!this.pusherQueues.TryGetValue(destinationInfo.Destination.Authority, out pusherQueue))
			{
				pusherQueue = (this.pusherQueues[destinationInfo.Destination.Authority] = this.CreatePusherQueue(destinationInfo));
			}
			pusherQueue.Enqueue(payloads, destinationInfo.ChannelIds);
		}

		internal void QueueReady(PusherQueue pusherQueue)
		{
			lock (this.syncRoot)
			{
				this.readyQueue.Enqueue(pusherQueue);
				this.StartPusherThread();
			}
		}

		private void StartPusherThread()
		{
			if (this.isPusherThreadActive == 0)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueues));
				return;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Pusher Thread is already active. Not starting one.");
		}

		private void ProcessQueues(object state)
		{
			int num = -1;
			bool flag = false;
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			Exception ex = null;
			try
			{
				num = Interlocked.CompareExchange(ref this.isPusherThreadActive, 1, 0);
				if (num == 0)
				{
					OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.PusherThreadStart)
					{
						ThreadId = managedThreadId
					});
					List<Task> list = new List<Task>();
					PusherQueue nextQueue;
					while ((nextQueue = this.GetNextQueue(list.Count, out flag)) != null)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Processing Pusher Queue. Destination: {0}", nextQueue.DestinationUrl);
						list.Add(this.remoteNotificationRequester.SendNotificationsAsync(nextQueue));
						this.remoteNotificationRequester.UnderRequestLimitEvent.Wait();
					}
					OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.PusherThreadCleanup)
					{
						ThreadId = managedThreadId,
						TaskCount = list.Count
					});
					lock (this.syncRoot)
					{
						foreach (string key in this.pusherQueues.Keys.ToArray<string>())
						{
							if (this.pusherQueues[key].TotalPayloads == 0)
							{
								this.pusherQueues.Remove(key);
							}
						}
					}
					Task.WaitAll(list.ToArray());
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
				ExWatson.SendReport(ex, ReportOptions.None, null);
			}
			finally
			{
				if (num == 0)
				{
					OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.PusherThreadEnd)
					{
						ThreadId = managedThreadId,
						HandledException = ex
					});
					if (!flag)
					{
						Interlocked.CompareExchange(ref this.isPusherThreadActive, 0, 1);
						this.StartPusherThread();
					}
					this.EndOfPusherThreadTestIndicator();
				}
			}
		}

		protected virtual void EndOfPusherThreadTestIndicator()
		{
		}

		private PusherQueue GetNextQueue(int taskCount, out bool wasPusherThreadActiveFlagReset)
		{
			PusherQueue result;
			lock (this.syncRoot)
			{
				PusherQueue pusherQueue = null;
				wasPusherThreadActiveFlagReset = false;
				if (taskCount < 10000 && this.readyQueue.Count > 0)
				{
					pusherQueue = this.readyQueue.Dequeue();
				}
				else
				{
					this.isPusherThreadActive = 0;
					wasPusherThreadActiveFlagReset = true;
				}
				result = pusherQueue;
			}
			return result;
		}

		private const int MaxTasksForPusherThread = 10000;

		private static readonly Pusher instance = new Pusher();

		private readonly Dictionary<string, PusherQueue> pusherQueues = new Dictionary<string, PusherQueue>();

		private readonly Queue<PusherQueue> readyQueue = new Queue<PusherQueue>();

		private readonly object syncRoot = new object();

		private readonly RemoteNotificationManager remoteNotificationManager;

		private readonly RemoteNotificationRequester remoteNotificationRequester;

		private int isPusherThreadActive;
	}
}
