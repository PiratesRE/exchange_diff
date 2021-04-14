using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AbsoluteYearlyRecurrence")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AbsoluteYearlyRecurrencePatternType : RecurrencePatternBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public int DayOfMonth { get; set; }

		[DataMember(Name = "Month", IsRequired = true, Order = 2)]
		[XmlElement("Month")]
		public string Month { get; set; }
	}
}
