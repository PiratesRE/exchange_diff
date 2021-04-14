using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Calendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Calendar", TypeName = "Recurrence")]
	public class Recurrence
	{
		[XmlElement(ElementName = "Type")]
		public byte Type { get; set; }

		[XmlElement(ElementName = "Interval")]
		public ushort? Interval { get; set; }

		[XmlElement(ElementName = "DayOfWeek")]
		public ushort? DayOfWeek { get; set; }

		[XmlElement(ElementName = "WeekOfMonth")]
		public byte? WeekOfMonth { get; set; }

		[XmlElement(ElementName = "DayOfMonth")]
		public byte? DayOfMonth { get; set; }

		[XmlElement(ElementName = "MonthOfYear")]
		public byte? MonthOfYear { get; set; }

		[XmlElement(ElementName = "Occurrences")]
		public ushort? Occurrences { get; set; }

		[XmlElement(ElementName = "Until", Namespace = "Calendar")]
		public string Until { get; set; }

		[XmlIgnore]
		public bool IntervalSpecified
		{
			get
			{
				return this.Interval != null;
			}
		}

		[XmlIgnore]
		public bool DayOfWeekSpecified
		{
			get
			{
				return this.DayOfWeek != null;
			}
		}

		[XmlIgnore]
		public bool WeekOfMonthSpecified
		{
			get
			{
				return this.WeekOfMonth != null;
			}
		}

		[XmlIgnore]
		public bool DayOfMonthSpecified
		{
			get
			{
				return this.DayOfMonth != null;
			}
		}

		[XmlIgnore]
		public bool MonthOfYearSpecified
		{
			get
			{
				return this.MonthOfYear != null;
			}
		}

		[XmlIgnore]
		public bool OccurrencesSpecified
		{
			get
			{
				return this.Occurrences != null;
			}
		}
	}
}
