using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class RelativeMonthlyRecurrencePatternType : IntervalRecurrencePatternBaseType
	{
		public DayOfWeekType DaysOfWeek
		{
			get
			{
				return this.daysOfWeekField;
			}
			set
			{
				this.daysOfWeekField = value;
			}
		}

		public DayOfWeekIndexType DayOfWeekIndex
		{
			get
			{
				return this.dayOfWeekIndexField;
			}
			set
			{
				this.dayOfWeekIndexField = value;
			}
		}

		private DayOfWeekType daysOfWeekField;

		private DayOfWeekIndexType dayOfWeekIndexField;
	}
}
