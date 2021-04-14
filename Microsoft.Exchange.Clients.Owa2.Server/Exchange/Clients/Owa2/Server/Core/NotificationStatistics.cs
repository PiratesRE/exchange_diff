using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class NotificationStatistics
	{
		internal NotificationStatistics()
		{
			this.data = new ConcurrentDictionary<NotificationStatisticsKey, NotificationStatisticsValue>();
			this.startTime = DateTime.UtcNow;
		}

		internal void Update(NotificationLocation location, NotificationPayloadBase payload, Action<NotificationStatisticsValue, NotificationPayloadBase> doUpdate)
		{
			if (location != null && payload != null && doUpdate != null)
			{
				this.UpdateInternal(location, payload, doUpdate);
			}
		}

		internal void Update(NotificationLocation location, IEnumerable<NotificationPayloadBase> payloads, Action<NotificationStatisticsValue, NotificationPayloadBase> doUpdate)
		{
			if (location != null && payloads != null && doUpdate != null)
			{
				foreach (NotificationPayloadBase notificationPayloadBase in payloads)
				{
					if (notificationPayloadBase != null)
					{
						this.UpdateInternal(location, notificationPayloadBase, doUpdate);
					}
				}
			}
		}

		internal void GetAndResetStatisticData(out IDictionary<NotificationStatisticsKey, NotificationStatisticsValue> outputData, out DateTime outputStartTime)
		{
			outputData = new Dictionary<NotificationStatisticsKey, NotificationStatisticsValue>(this.data);
			outputStartTime = this.startTime;
			this.data.Clear();
			this.startTime = DateTime.UtcNow;
		}

		private void UpdateInternal(NotificationLocation location, NotificationPayloadBase payload, Action<NotificationStatisticsValue, NotificationPayloadBase> doUpdate)
		{
			NotificationStatisticsKey key = new NotificationStatisticsKey(location, payload.GetType(), payload.EventType == QueryNotificationType.Reload);
			NotificationStatisticsValue orAdd = this.data.GetOrAdd(key, new NotificationStatisticsValue());
			doUpdate(orAdd, payload);
		}

		private readonly ConcurrentDictionary<NotificationStatisticsKey, NotificationStatisticsValue> data;

		private DateTime startTime;
	}
}
