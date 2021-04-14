using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class RegeneratingPatternBaseType : IntervalRecurrencePatternBaseType
	{
	}
}
