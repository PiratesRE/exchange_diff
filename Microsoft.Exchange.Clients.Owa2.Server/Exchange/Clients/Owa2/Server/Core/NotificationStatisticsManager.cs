using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class NotificationStatisticsManager
	{
		static NotificationStatisticsManager()
		{
			bool incomingNotificationStatisticsEnabled;
			if (!bool.TryParse(ConfigurationManager.AppSettings["IncomingNotificationStatisticsEnabled"], out incomingNotificationStatisticsEnabled))
			{
				incomingNotificationStatisticsEnabled = true;
			}
			bool outgoingNotificationStatisticsEnabled;
			if (!bool.TryParse(ConfigurationManager.AppSettings["OutgoingNotificationStatisticsEnabled"], out outgoingNotificationStatisticsEnabled))
			{
				outgoingNotificationStatisticsEnabled = true;
			}
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["NotificationStatisticsLoggingIntervalSeconds"], out num))
			{
				num = 900;
			}
			NotificationStatisticsManager.IncomingNotificationStatisticsEnabled = incomingNotificationStatisticsEnabled;
			NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled = outgoingNotificationStatisticsEnabled;
			NotificationStatisticsManager.LoggingInterval = TimeSpan.FromSeconds((double)num);
		}

		private NotificationStatisticsManager()
		{
			this.incomingNotifications = new NotificationStatistics();
			this.outgoingNotifications = new NotificationStatistics();
			this.isLogThreadActive = 0;
			this.lastLoggedTime = DateTime.UtcNow;
		}

		public static NotificationStatisticsManager Instance
		{
			get
			{
				if (NotificationStatisticsManager.instance == null)
				{
					lock (NotificationStatisticsManager.InstanceLock)
					{
						if (NotificationStatisticsManager.instance == null)
						{
							NotificationStatisticsManager.instance = new NotificationStatisticsManager();
						}
					}
				}
				return NotificationStatisticsManager.instance;
			}
		}

		internal Action<ILogEvent> TestLogEventCreated { get; set; }

		public void NotificationCreated(NotificationPayloadBase payload)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && NotificationStatisticsManager.IsStatisticable(payload))
			{
				this.incomingNotifications.Update(payload.Source, payload, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationCreated));
				this.TriggerLogCheck();
			}
		}

		public void NotificationCreated(Guid mailboxGuid, IEnumerable<NotificationPayloadBase> payloads)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && mailboxGuid != Guid.Empty)
			{
				this.incomingNotifications.Update(new MailboxLocation(mailboxGuid), NotificationStatisticsManager.GetStatisticablePayloads(payloads), new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationCreated));
				this.TriggerLogCheck();
			}
		}

		public void NotificationReceived(NotificationPayloadBase payload)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && NotificationStatisticsManager.IsStatisticable(payload))
			{
				this.incomingNotifications.Update(payload.Source, payload, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationReceived));
				this.TriggerLogCheck();
			}
		}

		public void NotificationReceived(string serverName, IEnumerable<NotificationPayloadBase> payloads)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && !string.IsNullOrEmpty(serverName))
			{
				this.incomingNotifications.Update(new ServerLocation(serverName), NotificationStatisticsManager.GetStatisticablePayloads(payloads), new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationReceived));
				this.TriggerLogCheck();
			}
		}

		public void NotificationQueued(NotificationPayloadBase payload)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && NotificationStatisticsManager.IsStatisticable(payload))
			{
				this.incomingNotifications.Update(payload.Source, payload, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationQueued));
				this.TriggerLogCheck();
			}
		}

		public void NotificationQueued(IEnumerable<NotificationPayloadBase> payloads)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
			{
				this.BulkUpdateIncomingNotifications(payloads, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationQueued));
				this.TriggerLogCheck();
			}
		}

		public void NotificationDispatched(string channelId, NotificationPayloadBase payload)
		{
			if ((NotificationStatisticsManager.IncomingNotificationStatisticsEnabled || NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled) && !string.IsNullOrEmpty(channelId) && NotificationStatisticsManager.IsStatisticable(payload))
			{
				if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
				{
					this.incomingNotifications.Update(payload.Source, payload, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationDispatched));
				}
				if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
				{
					this.outgoingNotifications.Update(new ChannelLocation(channelId), payload, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationDispatched));
				}
				this.TriggerLogCheck();
			}
		}

		public void NotificationDispatched(string channelId, IEnumerable<NotificationPayloadBase> payloads)
		{
			if ((NotificationStatisticsManager.IncomingNotificationStatisticsEnabled || NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled) && !string.IsNullOrEmpty(channelId))
			{
				if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
				{
					this.BulkUpdateIncomingNotifications(payloads, new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationDispatched));
				}
				if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
				{
					this.outgoingNotifications.Update(new ChannelLocation(channelId), NotificationStatisticsManager.GetStatisticablePayloads(payloads), new Action<NotificationStatisticsValue, NotificationPayloadBase>(NotificationStatisticsManager.UpdateNotificationDispatched));
				}
				this.TriggerLogCheck();
			}
		}

		public void NotificationPushed(string destinationUrl, NotificationPayloadBase payload, DateTime pushTime)
		{
			if ((NotificationStatisticsManager.IncomingNotificationStatisticsEnabled || NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled) && !string.IsNullOrEmpty(destinationUrl) && NotificationStatisticsManager.IsStatisticable(payload))
			{
				if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
				{
					this.incomingNotifications.Update(payload.Source, payload, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
					{
						NotificationStatisticsManager.UpdateNotificationPushed(v, p, pushTime);
					});
				}
				if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
				{
					this.outgoingNotifications.Update(new ServerLocation(destinationUrl), payload, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
					{
						NotificationStatisticsManager.UpdateNotificationPushed(v, p, pushTime);
					});
				}
				this.TriggerLogCheck();
			}
		}

		public void NotificationPushed(string destinationUrl, IEnumerable<NotificationPayloadBase> payloads, DateTime pushTime)
		{
			if ((NotificationStatisticsManager.IncomingNotificationStatisticsEnabled || NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled) && !string.IsNullOrEmpty(destinationUrl))
			{
				if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
				{
					this.BulkUpdateIncomingNotifications(payloads, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
					{
						NotificationStatisticsManager.UpdateNotificationPushed(v, p, pushTime);
					});
				}
				if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
				{
					this.outgoingNotifications.Update(new ServerLocation(destinationUrl), NotificationStatisticsManager.GetStatisticablePayloads(payloads), delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
					{
						NotificationStatisticsManager.UpdateNotificationPushed(v, p, pushTime);
					});
				}
				this.TriggerLogCheck();
			}
		}

		public void NotificationDropped(NotificationPayloadBase payload, NotificationState state)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled && NotificationStatisticsManager.IsStatisticable(payload))
			{
				this.incomingNotifications.Update(payload.Source, payload, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
				{
					NotificationStatisticsManager.UpdateNotificationDropped(v, p, state);
				});
				this.TriggerLogCheck();
			}
			if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
			{
				RemoteNotificationPayload remoteNotificationPayload = payload as RemoteNotificationPayload;
				if (remoteNotificationPayload != null && remoteNotificationPayload.ChannelIds != null)
				{
					foreach (string channelId in remoteNotificationPayload.ChannelIds)
					{
						this.outgoingNotifications.Update(new ChannelLocation(channelId), payload, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
						{
							NotificationStatisticsManager.UpdateNotificationDropped(v, p, state);
						});
					}
				}
			}
		}

		public void NotificationDropped(IEnumerable<NotificationPayloadBase> payloads, NotificationState state)
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
			{
				this.BulkUpdateIncomingNotifications(payloads, delegate(NotificationStatisticsValue v, NotificationPayloadBase p)
				{
					NotificationStatisticsManager.UpdateNotificationDropped(v, p, state);
				});
			}
		}

		internal void LogNotificationStatisticsDataInternal()
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled)
			{
				this.WriteNotificationStatisticsData(NotificationStatisticsEventType.Incoming, this.incomingNotifications);
			}
			if (NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
			{
				this.WriteNotificationStatisticsData(NotificationStatisticsEventType.Outgoing, this.outgoingNotifications);
			}
		}

		private void BulkUpdateIncomingNotifications(IEnumerable<NotificationPayloadBase> payloads, Action<NotificationStatisticsValue, NotificationPayloadBase> doUpdate)
		{
			if (payloads != null && doUpdate != null)
			{
				foreach (NotificationPayloadBase notificationPayloadBase in payloads)
				{
					if (NotificationStatisticsManager.IsStatisticable(notificationPayloadBase))
					{
						this.incomingNotifications.Update(notificationPayloadBase.Source, notificationPayloadBase, doUpdate);
					}
				}
				this.TriggerLogCheck();
			}
		}

		private static void UpdateNotificationCreated(NotificationStatisticsValue value, NotificationPayloadBase payload)
		{
			payload.CreatedTime = new DateTime?(DateTime.UtcNow);
			value.Created++;
		}

		private static void UpdateNotificationReceived(NotificationStatisticsValue value, NotificationPayloadBase payload)
		{
			payload.ReceivedTime = new DateTime?(DateTime.UtcNow);
			value.Received++;
		}

		private static void UpdateNotificationQueued(NotificationStatisticsValue value, NotificationPayloadBase payload)
		{
			payload.QueuedTime = new DateTime?(DateTime.UtcNow);
			value.Queued++;
			if (payload.CreatedTime != null)
			{
				value.CreatedAndQueued.Add((payload.QueuedTime.Value - payload.CreatedTime.Value).TotalMilliseconds);
			}
		}

		private static void UpdateNotificationDispatched(NotificationStatisticsValue value, NotificationPayloadBase payload)
		{
			value.Dispatched++;
			DateTime utcNow = DateTime.UtcNow;
			if (payload.CreatedTime != null)
			{
				value.CreatedAndDispatched.Add((utcNow - payload.CreatedTime.Value).TotalMilliseconds);
				return;
			}
			if (payload.ReceivedTime != null)
			{
				value.ReceivedAndDispatched.Add((utcNow - payload.ReceivedTime.Value).TotalMilliseconds);
			}
		}

		private static void UpdateNotificationPushed(NotificationStatisticsValue value, NotificationPayloadBase payload, DateTime pushTime)
		{
			value.Pushed++;
			if (payload.CreatedTime != null)
			{
				value.CreatedAndPushed.Add((pushTime - payload.CreatedTime.Value).TotalMilliseconds);
			}
			if (payload.QueuedTime != null)
			{
				value.QueuedAndPushed.Add((pushTime - payload.QueuedTime.Value).TotalMilliseconds);
			}
		}

		private static void UpdateNotificationDropped(NotificationStatisticsValue value, NotificationPayloadBase payload, NotificationState state)
		{
			value.Dropped++;
			switch (state)
			{
			case NotificationState.CreatedOrReceived:
				if (payload.ReceivedTime != null)
				{
					value.ReceivedAndDropped++;
					return;
				}
				if (payload.CreatedTime != null)
				{
					value.CreatedAndDropped++;
					return;
				}
				break;
			case NotificationState.Queued:
				value.QueuedAndDropped++;
				return;
			case NotificationState.Dispatching:
				value.DispatchingAndDropped++;
				break;
			default:
				return;
			}
		}

		private static bool IsStatisticable(NotificationPayloadBase payload)
		{
			return payload != null && payload.Source != null && NotificationStatisticsManager.StatisticablePayloadTypes.Contains(payload.GetType());
		}

		private static IEnumerable<NotificationPayloadBase> GetStatisticablePayloads(IEnumerable<NotificationPayloadBase> payloads)
		{
			if (payloads == null)
			{
				return null;
			}
			return from payload in payloads
			where NotificationStatisticsManager.IsStatisticable(payload)
			select payload;
		}

		private void TriggerLogCheck()
		{
			if (this.TestLogEventCreated != null)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow - this.lastLoggedTime > NotificationStatisticsManager.LoggingInterval)
			{
				lock (this.lastLoggedTimeLock)
				{
					if (utcNow - this.lastLoggedTime <= NotificationStatisticsManager.LoggingInterval)
					{
						return;
					}
					this.lastLoggedTime = utcNow;
				}
				this.StartLogThread();
			}
		}

		private void StartLogThread()
		{
			if (NotificationStatisticsManager.IncomingNotificationStatisticsEnabled || NotificationStatisticsManager.OutgoingNotificationStatisticsEnabled)
			{
				if (this.isLogThreadActive == 0)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.LogNotificationStatisticsData));
					return;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Notification statistics log thread is already active. Not starting one.");
			}
		}

		private void WriteNotificationStatisticsData(NotificationStatisticsEventType eventType, NotificationStatistics statistics)
		{
			IDictionary<NotificationStatisticsKey, NotificationStatisticsValue> dictionary;
			DateTime startTime;
			statistics.GetAndResetStatisticData(out dictionary, out startTime);
			foreach (KeyValuePair<NotificationStatisticsKey, NotificationStatisticsValue> keyValuePair in dictionary)
			{
				NotificationStatisticsLogEvent notificationStatisticsLogEvent = new NotificationStatisticsLogEvent(eventType, startTime, keyValuePair.Key, keyValuePair.Value);
				if (this.TestLogEventCreated != null)
				{
					this.TestLogEventCreated(notificationStatisticsLogEvent);
				}
				else
				{
					OwaServerLogger.AppendToLog(notificationStatisticsLogEvent);
				}
			}
		}

		private void LogNotificationStatisticsData(object state)
		{
			int num = 0;
			try
			{
				num = Interlocked.CompareExchange(ref this.isLogThreadActive, 1, 0);
				if (num == 0)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "New notification statistics log thread started.");
					this.LogNotificationStatisticsDataInternal();
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Notification statistics log thread quit. Exception: {0}", arg);
			}
			finally
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Notification statistics log thread completed.");
				if (num == 0)
				{
					Interlocked.CompareExchange(ref this.isLogThreadActive, 0, 1);
				}
			}
		}

		private const string IncomingNotificationStatisticsEnabledAppSettingKey = "IncomingNotificationStatisticsEnabled";

		private const string OutgoingNotificationStatisticsEnabledAppSettingKey = "OutgoingNotificationStatisticsEnabled";

		private const string NotificationStatisticsLoggingIntervalSecondsAppSettingKey = "NotificationStatisticsLoggingIntervalSeconds";

		private const int DefaultNotificationStatisticsLoggingIntervalSeconds = 900;

		private static readonly Type[] StatisticablePayloadTypes = new Type[]
		{
			typeof(GroupAssociationNotificationPayload),
			typeof(ReloadAllNotificationPayload),
			typeof(RemoteNotificationPayload),
			typeof(RowNotificationPayload),
			typeof(UnseenItemNotificationPayload)
		};

		private static readonly object InstanceLock = new object();

		private static NotificationStatisticsManager instance;

		private static readonly bool IncomingNotificationStatisticsEnabled;

		private static readonly bool OutgoingNotificationStatisticsEnabled;

		private static readonly TimeSpan LoggingInterval;

		private readonly object lastLoggedTimeLock = new object();

		private DateTime lastLoggedTime;

		private int isLogThreadActive;

		private readonly NotificationStatistics incomingNotifications;

		private readonly NotificationStatistics outgoingNotifications;
	}
}
