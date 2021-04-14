using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(RelativeMonthlyRecurrencePatternType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AbsoluteYearlyRecurrencePatternType))]
	[XmlInclude(typeof(RelativeYearlyRecurrencePatternType))]
	[XmlInclude(typeof(IntervalRecurrencePatternBaseType))]
	[XmlInclude(typeof(DailyRecurrencePatternType))]
	[XmlInclude(typeof(WeeklyRecurrencePatternType))]
	[XmlInclude(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RegeneratingPatternBaseType))]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class RecurrencePatternBaseType
	{
	}
}
