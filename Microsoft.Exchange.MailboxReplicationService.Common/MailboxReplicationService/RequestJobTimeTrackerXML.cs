using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class RequestJobTimeTrackerXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "CurrentState")]
		public string CurrentState { get; set; }

		[XmlElement(ElementName = "LastStateChangeTimeStamp")]
		public string LastStateChangeTimeStamp { get; set; }

		[XmlElement(ElementName = "Timestamp")]
		public List<RequestJobTimeTrackerXML.TimestampRec> Timestamps { get; set; }

		[XmlElement(ElementName = "Duration")]
		public List<RequestJobTimeTrackerXML.DurationRec> Durations { get; set; }

		public void AddTimestamp(RequestJobTimestamp mrt, DateTime value)
		{
			if (this.Timestamps == null)
			{
				this.Timestamps = new List<RequestJobTimeTrackerXML.TimestampRec>();
			}
			this.Timestamps.Add(new RequestJobTimeTrackerXML.TimestampRec
			{
				Type = mrt.ToString(),
				Timestamp = value
			});
		}

		public sealed class TimestampRec : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "Type")]
			public string Type { get; set; }

			[XmlAttribute(AttributeName = "Time")]
			public DateTime Timestamp { get; set; }
		}

		public sealed class DurationRec : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "State")]
			public string State { get; set; }

			[XmlAttribute(AttributeName = "Dur")]
			public string Duration { get; set; }

			[XmlArray("PerMinute")]
			[XmlArrayItem("Minute")]
			public TimeSlotXML[] PerMinute { get; set; }

			[XmlArrayItem("Hour")]
			[XmlArray("PerHour")]
			public TimeSlotXML[] PerHour { get; set; }

			[XmlArray("PerDay")]
			[XmlArrayItem("Day")]
			public TimeSlotXML[] PerDay { get; set; }

			[XmlArray("PerMonth")]
			[XmlArrayItem("Month")]
			public TimeSlotXML[] PerMonth { get; set; }

			[XmlElement(ElementName = "Duration")]
			public List<RequestJobTimeTrackerXML.DurationRec> ChildNodes { get; set; }
		}
	}
}
