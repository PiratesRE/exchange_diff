using System;
using System.Text;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class EventLogSubscription
	{
		public string Name { get; set; }

		public EventMatchingRule GreenEvents { get; set; }

		public EventMatchingRule RedEvents { get; set; }

		public TimeSpan AutoResetInterval { get; set; }

		public EventLogSubscription.CustomAction OnGreenEvents { get; set; }

		public EventLogSubscription.CustomAction OnRedEvents { get; set; }

		public EventLogSubscription(string name, EventMatchingRule redEvents, EventMatchingRule greenEvents = null, EventLogSubscription.CustomAction onGreen = null, EventLogSubscription.CustomAction onRed = null) : this(name, EventLogSubscription.NoAutoReset, redEvents, greenEvents, onGreen, onRed)
		{
		}

		public EventLogSubscription(string name, TimeSpan autoReset, EventMatchingRule redEvents, EventMatchingRule greenEvents = null, EventLogSubscription.CustomAction onGreen = null, EventLogSubscription.CustomAction onRed = null)
		{
			this.Name = name;
			this.GreenEvents = greenEvents;
			this.RedEvents = redEvents;
			this.AutoResetInterval = autoReset;
			this.OnGreenEvents = onGreen;
			this.OnRedEvents = onRed;
		}

		public string GetContentHash()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Name={0}", this.Name);
			stringBuilder.AppendFormat("redEvents={0}", (this.RedEvents == null) ? "NULL" : this.RedEvents.ToString());
			stringBuilder.AppendFormat("greenEvents={0}", (this.GreenEvents == null) ? "NULL" : this.GreenEvents.ToString());
			stringBuilder.AppendFormat("AutoReset={0}s", this.AutoResetInterval.TotalSeconds);
			return stringBuilder.ToString();
		}

		public static TimeSpan NoAutoReset = TimeSpan.MinValue;

		public delegate void CustomAction(EventLogNotification.EventRecordInternal record, EventLogNotification.EventNotificationMetadata enm);
	}
}
