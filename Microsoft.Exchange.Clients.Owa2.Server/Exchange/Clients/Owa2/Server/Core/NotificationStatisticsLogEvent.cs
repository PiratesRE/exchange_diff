using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NotificationStatisticsLogEvent : ILogEvent
	{
		internal NotificationStatisticsLogEvent(NotificationStatisticsEventType eventType, DateTime startTime, NotificationStatisticsKey key, NotificationStatisticsValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.eventType = eventType;
			this.startTime = startTime;
			this.key = key;
			this.value = value;
		}

		public string EventId
		{
			get
			{
				if (this.eventType != NotificationStatisticsEventType.Incoming)
				{
					return "OutgoingNotifications";
				}
				return "IncomingNotifications";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			ICollection<KeyValuePair<string, object>> collection = new List<KeyValuePair<string, object>>();
			collection.Add(new KeyValuePair<string, object>("StartTime", this.startTime.ToString("o")));
			collection.Add(new KeyValuePair<string, object>("PayloadType", this.key.PayloadType.Name));
			collection.Add(new KeyValuePair<string, object>("IsReload", this.key.IsReload.ToString()));
			collection.Add(this.key.Location.GetEventData());
			foreach (KeyValuePair<string, object> item in this.value.GetEventData())
			{
				collection.Add(item);
			}
			return collection;
		}

		private const string IncomingNotificationEventId = "IncomingNotifications";

		private const string OutgoingNotificationEventId = "OutgoingNotifications";

		private const string PayloadTypeKey = "PayloadType";

		private const string StartTimeKey = "StartTime";

		private const string IsReloadKey = "IsReload";

		private readonly NotificationStatisticsEventType eventType;

		private readonly DateTime startTime;

		private readonly NotificationStatisticsKey key;

		private readonly NotificationStatisticsValue value;
	}
}
