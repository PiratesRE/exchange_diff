using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal class NotificationBrokerClientLogEvent : ILogEvent
	{
		public NotificationBrokerClientLogEvent(ConsumerId consumerId, string action)
		{
			this.consumerId = consumerId;
			this.eventId = action;
		}

		public string EventId
		{
			get
			{
				return this.eventId;
			}
		}

		public Guid? SubscriptionId { get; set; }

		public BrokerStatus? Status { get; set; }

		public Guid? NotificationId { get; set; }

		public int? SequenceId { get; set; }

		public long Latency { get; set; }

		public Exception Exception { get; set; }

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ConsumerId", this.consumerId.ToString());
			if (this.SubscriptionId != null)
			{
				dictionary.Add("SubId", this.SubscriptionId.Value.ToString());
			}
			if (this.Status != null)
			{
				dictionary.Add("Status", this.Status.ToString());
			}
			if (this.NotificationId != null)
			{
				dictionary.Add("NtfId", this.NotificationId.Value.ToString());
			}
			if (this.SequenceId != null)
			{
				dictionary.Add("SeqId", this.SequenceId.ToString());
			}
			dictionary.Add("Latency", this.Latency.ToString());
			if (this.Exception != null)
			{
				dictionary.Add("Ex", this.Exception.ToString());
			}
			return dictionary;
		}

		private readonly ConsumerId consumerId;

		private readonly string eventId;
	}
}
