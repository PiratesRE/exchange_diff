using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RelativeMonthlyRecurrence")]
	[Serializable]
	public class RelativeMonthlyRecurrencePatternType : IntervalRecurrencePatternBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string DaysOfWeek { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public DayOfWeekIndexType DayOfWeekIndex { get; set; }

		[DataMember(Name = "DayOfWeekIndex", EmitDefaultValue = false, IsRequired = true, Order = 1)]
		[XmlIgnore]
		public string DayOfWeekIndexString
		{
			get
			{
				return EnumUtilities.ToString<DayOfWeekIndexType>(this.DayOfWeekIndex);
			}
			set
			{
				this.DayOfWeekIndex = EnumUtilities.Parse<DayOfWeekIndexType>(value);
			}
		}
	}
}
