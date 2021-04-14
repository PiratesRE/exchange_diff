using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class CalendarEvent
	{
		public CalendarEvent()
		{
		}

		public CalendarEvent(string dowOfStartTime, string dateOfStartTime, string timeOfStartTime, string dowOfEndTime, string dateOfEndTime, string timeOfEndTime, string subject, string location)
		{
			this.DayOfWeekOfStartTime = dowOfStartTime;
			this.DateOfStartTime = dateOfStartTime;
			this.TimeOfStartTime = timeOfStartTime;
			this.DayOfWeekOfEndTime = dowOfEndTime;
			this.DateOfEndTime = dateOfEndTime;
			this.TimeOfEndTime = timeOfEndTime;
			this.Subject = subject;
			this.Location = location;
		}

		[XmlElement("DayOfWeekOfStartTime")]
		public string DayOfWeekOfStartTime { get; set; }

		[XmlElement("DateOfStartTime")]
		public string DateOfStartTime { get; set; }

		[XmlElement("TimeOfStartTime")]
		public string TimeOfStartTime { get; set; }

		[XmlElement("DayOfWeekOfEndTime")]
		public string DayOfWeekOfEndTime { get; set; }

		[XmlElement("DateOfEndTime")]
		public string DateOfEndTime { get; set; }

		[XmlElement("TimeOfEndTime")]
		public string TimeOfEndTime { get; set; }

		[XmlElement("Subject")]
		public string Subject { get; set; }

		[XmlElement("Location")]
		public string Location { get; set; }
	}
}
