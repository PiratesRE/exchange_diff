using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PusherQueue
	{
		static PusherQueue()
		{
			if (Globals.IsPreCheckinApp)
			{
				PusherQueue.ListenerVdir = HostingEnvironment.ApplicationVirtualPath;
			}
		}

		public int TotalPayloads
		{
			get
			{
				int result;
				lock (this.syncRoot)
				{
					result = this.payloadQueue.Count + ((this.inTransitQueue == null) ? 0 : this.inTransitQueue.Count);
				}
				return result;
			}
		}

		public string DestinationUrl { get; private set; }

		public int FailureCount { get; private set; }

		public PusherQueue(Uri destination, Action<PusherQueue> readyCallback)
		{
			this.readyCallback = readyCallback;
			this.DestinationUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}/remotenotification.ashx", new object[]
			{
				destination.Scheme,
				destination.Authority,
				PusherQueue.ListenerVdir
			});
			this.payloadQueue = new Queue<PusherQueuePayload>();
			this.remoteNotificationManager = this.GetRemoteNotificationManager();
		}

		public virtual void Enqueue(List<NotificationPayloadBase> payloads, IEnumerable<string> channelIds)
		{
			bool flag = false;
			lock (this.syncRoot)
			{
				if (this.TotalPayloads + payloads.Count <= 200)
				{
					flag = (this.TotalPayloads == 0);
					foreach (NotificationPayloadBase payload2 in payloads)
					{
						this.payloadQueue.Enqueue(new PusherQueuePayload(payload2, channelIds));
					}
					NotificationStatisticsManager.Instance.NotificationQueued(payloads);
				}
				else
				{
					HashSet<string> hashSet = new HashSet<string>();
					foreach (PusherQueuePayload pusherQueuePayload in this.payloadQueue.Concat(this.inTransitQueue ?? Array<PusherQueuePayload>.Empty).Concat(from payload in payloads
					select new PusherQueuePayload(payload, channelIds)))
					{
						hashSet.UnionWith(pusherQueuePayload.ChannelIds);
					}
					IEnumerable<NotificationPayloadBase> payloads2 = from p in this.payloadQueue
					select p.Payload;
					NotificationStatisticsManager.Instance.NotificationDropped(payloads2, NotificationState.Queued);
					this.payloadQueue.Clear();
					this.inTransitQueue = null;
					ReloadAllNotificationPayload reloadAllNotificationPayload = new ReloadAllNotificationPayload();
					reloadAllNotificationPayload.Source = new TypeLocation(base.GetType());
					this.payloadQueue.Enqueue(new PusherQueuePayload(reloadAllNotificationPayload, hashSet));
					NotificationStatisticsManager.Instance.NotificationCreated(reloadAllNotificationPayload);
					NotificationStatisticsManager.Instance.NotificationQueued(reloadAllNotificationPayload);
				}
			}
			if (flag)
			{
				this.readyCallback(this);
			}
		}

		public virtual PusherQueuePayload[] GetPayloads()
		{
			PusherQueuePayload[] result;
			lock (this.syncRoot)
			{
				if (this.inTransitQueue == null)
				{
					this.inTransitQueue = this.payloadQueue;
					this.payloadQueue = new Queue<PusherQueuePayload>();
				}
				else
				{
					while (this.payloadQueue.Count > 0)
					{
						this.inTransitQueue.Enqueue(this.payloadQueue.Dequeue());
					}
				}
				result = this.inTransitQueue.ToArray();
			}
			return result;
		}

		public void SendComplete(bool success)
		{
			bool flag = false;
			string[] array = null;
			lock (this.syncRoot)
			{
				if (success)
				{
					this.inTransitQueue = null;
					this.FailureCount = 0;
				}
				else if (++this.FailureCount >= 5)
				{
					array = (from payload in this.payloadQueue.Concat(this.inTransitQueue ?? Array<PusherQueuePayload>.Empty)
					select payload.ChannelIds).Aggregate((IEnumerable<string> workingset, IEnumerable<string> next) => workingset.Union(next)).ToArray<string>();
					this.payloadQueue.Clear();
					this.inTransitQueue = null;
				}
				flag = (this.TotalPayloads > 0);
			}
			if (flag)
			{
				this.readyCallback(this);
			}
			if (array != null)
			{
				foreach (string channelId in array)
				{
					this.remoteNotificationManager.CleanUpChannel(channelId);
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>(0L, "Pusher removed implicity lost channels. LostChannelIds: {0}", string.Join(",", array));
			}
		}

		public virtual RemoteNotificationManager GetRemoteNotificationManager()
		{
			return RemoteNotificationManager.Instance;
		}

		public const int MaxQueueSize = 200;

		private const int MaxFailureCount = 5;

		private static readonly string ListenerVdir = "/owa";

		private readonly Action<PusherQueue> readyCallback;

		private readonly object syncRoot = new object();

		private readonly RemoteNotificationManager remoteNotificationManager;

		protected Queue<PusherQueuePayload> payloadQueue;

		private Queue<PusherQueuePayload> inTransitQueue;
	}
}
