using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class Duration
	{
		public Duration()
		{
		}

		public Duration(DurationType type, uint interval, bool useWorkHoursTimeSlot, DateTime startTimeInDay, DateTime endTimeInDay, bool nonWorkHoursExcluded)
		{
			this.Type = type;
			this.Interval = interval;
			this.UseWorkHoursTimeSlot = useWorkHoursTimeSlot;
			this.StartTimeInDay = startTimeInDay;
			this.EndTimeInDay = endTimeInDay;
			this.NonWorkHoursExcluded = nonWorkHoursExcluded;
		}

		[XmlElement("Type")]
		public DurationType Type { get; set; }

		[XmlElement("Interval")]
		public uint Interval { get; set; }

		[XmlElement("UseWorkHoursTimeSlot")]
		public bool UseWorkHoursTimeSlot { get; set; }

		[XmlElement("StartTimeInDay")]
		public DateTime StartTimeInDay { get; set; }

		[XmlElement("EndTimeInDay")]
		public DateTime EndTimeInDay { get; set; }

		[XmlElement("NonWorkHoursExcluded")]
		public bool NonWorkHoursExcluded { get; set; }
	}
}
