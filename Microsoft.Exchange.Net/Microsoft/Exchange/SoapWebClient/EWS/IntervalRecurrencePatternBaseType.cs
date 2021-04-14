using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(DailyRecurrencePatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRecurrencePatternType))]
	[XmlInclude(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RelativeMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RegeneratingPatternBaseType))]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[Serializable]
	public abstract class IntervalRecurrencePatternBaseType : RecurrencePatternBaseType
	{
		public int Interval;
	}
}
