using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class WeeklyRecurrencePatternType : IntervalRecurrencePatternBaseType
	{
		public string DaysOfWeek
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

		public string FirstDayOfWeek
		{
			get
			{
				return this.firstDayOfWeekField;
			}
			set
			{
				this.firstDayOfWeekField = value;
			}
		}

		private string daysOfWeekField;

		private string firstDayOfWeekField;
	}
}
