using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class RecurringDayTransitionType : RecurringTimeTransitionType
	{
		public string DayOfWeek
		{
			get
			{
				return this.dayOfWeekField;
			}
			set
			{
				this.dayOfWeekField = value;
			}
		}

		public int Occurrence
		{
			get
			{
				return this.occurrenceField;
			}
			set
			{
				this.occurrenceField = value;
			}
		}

		private string dayOfWeekField;

		private int occurrenceField;
	}
}
