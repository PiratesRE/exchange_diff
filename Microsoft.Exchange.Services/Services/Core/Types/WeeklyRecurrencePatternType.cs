using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "WeeklyRecurrence")]
	[Serializable]
	public class WeeklyRecurrencePatternType : IntervalRecurrencePatternBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string DaysOfWeek { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 2)]
		public string FirstDayOfWeek { get; set; }
	}
}
