using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Calendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Calendar", TypeName = "Recurrence")]
	public class Recurrence
	{
		[XmlElement(ElementName = "Type")]
		public int Type { get; set; }

		[XmlElement(ElementName = "Interval")]
		public int Interval { get; set; }

		[XmlElement(ElementName = "DayOfWeek")]
		public int DayOfWeek { get; set; }

		[XmlElement(ElementName = "WeekOfMonth")]
		public int WeekOfMonth { get; set; }

		[XmlElement(ElementName = "DayOfMonth")]
		public int DayOfMonth { get; set; }

		[XmlElement(ElementName = "MonthOfYear")]
		public int MonthOfYear { get; set; }

		[XmlElement(ElementName = "Occurrences")]
		public int Occurrences { get; set; }

		[XmlElement(ElementName = "Until", Namespace = "Calendar")]
		public string Until { get; set; }
	}
}
