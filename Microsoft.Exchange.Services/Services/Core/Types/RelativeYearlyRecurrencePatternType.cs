using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RelativeYearlyRecurrence")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RelativeYearlyRecurrencePatternType : RecurrencePatternBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string DaysOfWeek { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public DayOfWeekIndexType DayOfWeekIndex { get; set; }

		[XmlIgnore]
		[DataMember(Name = "DayOfWeekIndex", EmitDefaultValue = false, IsRequired = true, Order = 2)]
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

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 3)]
		public string Month { get; set; }
	}
}
