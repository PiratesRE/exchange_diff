using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AbsoluteMonthlyRecurrencePatternType : IntervalRecurrencePatternBaseType
	{
		public int DayOfMonth
		{
			get
			{
				return this.dayOfMonthField;
			}
			set
			{
				this.dayOfMonthField = value;
			}
		}

		private int dayOfMonthField;
	}
}
