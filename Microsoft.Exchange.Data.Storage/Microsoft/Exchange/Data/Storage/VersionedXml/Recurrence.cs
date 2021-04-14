using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class Recurrence
	{
		public Recurrence()
		{
		}

		public Recurrence(RecurrenceType type, uint interval, uint nthDayInMonth, DaysOfWeek daysOfWeek, WeekOrderInMonth weekOrderInMonth, uint monthOrder)
		{
			this.Type = type;
			this.Interval = interval;
			this.NthDayInMonth = nthDayInMonth;
			this.DaysOfWeek = daysOfWeek;
			this.WeekOrderInMonth = weekOrderInMonth;
			this.MonthOrder = monthOrder;
		}

		[XmlElement("Type")]
		public RecurrenceType Type { get; set; }

		[XmlElement("Interval")]
		public uint Interval { get; set; }

		[XmlElement("NthDayInMonth")]
		public uint NthDayInMonth { get; set; }

		[XmlElement("DaysOfWeek")]
		public DaysOfWeek DaysOfWeek { get; set; }

		[XmlElement("WeekOrderInMonth")]
		public WeekOrderInMonth WeekOrderInMonth { get; set; }

		[XmlElement("MonthOrder")]
		public uint MonthOrder { get; set; }
	}
}
