using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Calendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Calendar", TypeName = "Exception")]
	public class Exception
	{
		[XmlElement(ElementName = "Deleted")]
		public bool Deleted { get; set; }

		[XmlElement(ElementName = "StartTime")]
		public string StartTime { get; set; }

		[XmlElement(ElementName = "Subject")]
		public string Subject { get; set; }

		[XmlElement(ElementName = "EndTime")]
		public string EndTime { get; set; }

		[XmlElement(ElementName = "ExceptionStartTime")]
		public string ExceptionStartTime { get; set; }

		[XmlElement(ElementName = "BusyStatus")]
		public int BusyStatus { get; set; }

		[XmlElement(ElementName = "AllDayEvent")]
		public bool AllDayEvent { get; set; }

		[XmlElement(ElementName = "Location")]
		public string Location { get; set; }

		[XmlElement(ElementName = "Reminder")]
		public int Reminder { get; set; }
	}
}
