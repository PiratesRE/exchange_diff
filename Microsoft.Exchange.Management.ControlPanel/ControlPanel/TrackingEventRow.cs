using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class TrackingEventRow
	{
		internal TrackingEventRow(TrackingEventType trackingEvent, DateTime eventDateTime, string eventTypeDescription, string eventDescription, string server, string[] eventData)
		{
			this.TrackingEvent = trackingEvent;
			this.EventDateTime = ((eventDateTime == DateTime.MinValue) ? string.Empty : eventDateTime.UtcToUserDateTimeString());
			this.EventTypeDescription = eventTypeDescription;
			this.EventDescription = eventDescription;
			this.Server = server;
			this.EventData = eventData;
		}

		[DataMember]
		public TrackingEventType TrackingEvent { get; private set; }

		[DataMember]
		public string EventTypeDescription { get; private set; }

		[DataMember]
		public string EventDescription { get; private set; }

		[DataMember]
		public string EventDateTime { get; private set; }

		[DataMember]
		public string[] EventData { get; private set; }

		public string Server { get; private set; }
	}
}
