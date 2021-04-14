using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class MonitoringEvent
	{
		internal MonitoringEvent(string eventSource, int eventIdentifier, EventTypeEnumeration eventType, string eventMessage) : this(eventSource, eventIdentifier, eventType, eventMessage, null)
		{
		}

		internal MonitoringEvent(string eventSource, int eventIdentifier, EventTypeEnumeration eventType, string eventMessage, string eventInstanceName)
		{
			if (string.IsNullOrEmpty(eventSource))
			{
				throw new ArgumentNullException("eventSource");
			}
			if (string.IsNullOrEmpty(eventMessage))
			{
				throw new ArgumentNullException("eventMessage");
			}
			this.eventSource = eventSource;
			this.eventIdentifier = eventIdentifier;
			this.eventType = eventType;
			this.eventMessage = eventMessage;
			this.eventInstanceName = eventInstanceName;
		}

		public string EventSource
		{
			get
			{
				return this.eventSource;
			}
		}

		public int EventIdentifier
		{
			get
			{
				return this.eventIdentifier;
			}
		}

		public EventTypeEnumeration EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public string EventMessage
		{
			get
			{
				return this.eventMessage;
			}
		}

		public string EventInstanceName
		{
			get
			{
				return this.eventInstanceName;
			}
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.EventInstanceName))
			{
				return Strings.MonitoringEventString(this.EventSource, this.EventIdentifier, Enum.GetName(this.EventType.GetType(), this.EventType), this.EventMessage);
			}
			return Strings.MonitoringEventStringWithInstanceName(this.EventSource, this.EventIdentifier, Enum.GetName(this.EventType.GetType(), this.EventType), this.EventMessage, this.EventInstanceName);
		}

		public const string EventSourcePrefix = "MSExchange Monitoring ";

		private string eventSource;

		private int eventIdentifier;

		private EventTypeEnumeration eventType;

		private string eventMessage;

		private string eventInstanceName;
	}
}
