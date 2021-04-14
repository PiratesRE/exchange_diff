using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Calendar
{
	[XmlType(Namespace = "Calendar", TypeName = "Attendee")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Attendee
	{
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "Email")]
		public string Email { get; set; }

		[XmlElement(ElementName = "AttendeeStatus")]
		public byte? AttendeeStatus { get; set; }

		[XmlElement(ElementName = "AttendeeType")]
		public byte? AttendeeType { get; set; }

		[XmlIgnore]
		public bool AttendeeStatusSpecified
		{
			get
			{
				return this.AttendeeStatus != null;
			}
		}

		[XmlIgnore]
		public bool AttendeeTypeSpecified
		{
			get
			{
				return this.AttendeeType != null;
			}
		}
	}
}
