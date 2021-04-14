using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(WeeklyRecurrencePatternType))]
	[XmlInclude(typeof(DailyRegeneratingPatternType))]
	[KnownType(typeof(DailyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRecurrencePatternType))]
	[XmlInclude(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RelativeMonthlyRecurrencePatternType))]
	[XmlInclude(typeof(RegeneratingPatternBaseType))]
	[XmlInclude(typeof(YearlyRegeneratingPatternType))]
	[XmlInclude(typeof(MonthlyRegeneratingPatternType))]
	[XmlInclude(typeof(WeeklyRegeneratingPatternType))]
	[XmlInclude(typeof(DailyRecurrencePatternType))]
	[KnownType(typeof(DailyRecurrencePatternType))]
	[KnownType(typeof(AbsoluteMonthlyRecurrencePatternType))]
	[KnownType(typeof(RelativeMonthlyRecurrencePatternType))]
	[KnownType(typeof(RegeneratingPatternBaseType))]
	[KnownType(typeof(YearlyRegeneratingPatternType))]
	[KnownType(typeof(MonthlyRegeneratingPatternType))]
	[KnownType(typeof(WeeklyRegeneratingPatternType))]
	[Serializable]
	public abstract class IntervalRecurrencePatternBaseType : RecurrencePatternBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public int Interval { get; set; }
	}
}
