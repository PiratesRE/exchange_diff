using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class TaskRecurrenceType
	{
		[XmlElement("YearlyRegeneration", typeof(YearlyRegeneratingPatternType))]
		[XmlElement("RelativeMonthlyRecurrence", typeof(RelativeMonthlyRecurrencePatternType))]
		[XmlElement("RelativeYearlyRecurrence", typeof(RelativeYearlyRecurrencePatternType))]
		[XmlElement("MonthlyRegeneration", typeof(MonthlyRegeneratingPatternType))]
		[XmlElement("WeeklyRegeneration", typeof(WeeklyRegeneratingPatternType))]
		[XmlElement("AbsoluteMonthlyRecurrence", typeof(AbsoluteMonthlyRecurrencePatternType))]
		[DataMember(Name = "RecurrencePattern", EmitDefaultValue = false, Order = 1)]
		[XmlElement("WeeklyRecurrence", typeof(WeeklyRecurrencePatternType))]
		[XmlElement("AbsoluteYearlyRecurrence", typeof(AbsoluteYearlyRecurrencePatternType))]
		[XmlElement("DailyRecurrence", typeof(DailyRecurrencePatternType))]
		[XmlElement("DailyRegeneration", typeof(DailyRegeneratingPatternType))]
		public RecurrencePatternBaseType RecurrencePattern { get; set; }

		[XmlElement("EndDateRecurrence", typeof(EndDateRecurrenceRangeType))]
		[DataMember(Name = "RecurrenceRange", EmitDefaultValue = false, Order = 2)]
		[XmlElement("NoEndRecurrence", typeof(NoEndRecurrenceRangeType))]
		[XmlElement("NumberedRecurrence", typeof(NumberedRecurrenceRangeType))]
		public RecurrenceRangeBaseType RecurrenceRange { get; set; }
	}
}
