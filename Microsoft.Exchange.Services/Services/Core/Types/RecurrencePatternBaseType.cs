using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(WeeklyRecurrencePatternType))]
	[XmlInclude(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RelativeMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RegeneratingPatternBaseType))]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(AbsoluteYearlyRecurrencePatternType))]
	[KnownType(typeof(RelativeYearlyRecurrencePatternType))]
	[KnownType(typeof(IntervalRecurrencePatternBaseType))]
	[KnownType(typeof(DailyRecurrencePatternType))]
	[XmlInclude(typeof(AbsoluteYearlyRecurrencePatternType))]
	[KnownType(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[KnownType(typeof(RelativeMonthlyRecurrencePatternType))]
	[KnownType(typeof(RegeneratingPatternBaseType))]
	[KnownType(typeof(YearlyRegeneratingPatternType))]
	[KnownType(typeof(MonthlyRegeneratingPatternType))]
	[KnownType(typeof(WeeklyRegeneratingPatternType))]
	[KnownType(typeof(DailyRegeneratingPatternType))]
	[XmlInclude(typeof(RelativeYearlyRecurrencePatternType))]
	[XmlInclude(typeof(IntervalRecurrencePatternBaseType))]
	[XmlInclude(typeof(DailyRecurrencePatternType))]
	[XmlInclude(typeof(WeeklyRecurrencePatternType))]
	[Serializable]
	public abstract class RecurrencePatternBaseType
	{
	}
}
