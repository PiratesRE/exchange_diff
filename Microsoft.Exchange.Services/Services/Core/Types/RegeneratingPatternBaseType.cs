using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(MonthlyRegeneratingPatternType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(YearlyRegeneratingPatternType))]
	[KnownType(typeof(DailyRegeneratingPatternType))]
	[KnownType(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[Serializable]
	public abstract class RegeneratingPatternBaseType : IntervalRecurrencePatternBaseType
	{
	}
}
