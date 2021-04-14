using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Calendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Calendar", TypeName = "Attendee")]
	public class Attendee
	{
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "Email")]
		public string Email { get; set; }

		[XmlElement(ElementName = "AttendeeStatus")]
		public int AttendeeStatus { get; set; }

		[XmlElement(ElementName = "AttendeeType")]
		public int AttendeeType { get; set; }
	}
}
