using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TimeChangeType
	{
		[XmlElement]
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string Offset { get; set; }

		[XmlElement]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public string AbsoluteDate { get; set; }

		[XmlElement]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3)]
		public RelativeYearlyRecurrencePatternType RelativeYearlyRecurrence { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 4)]
		[XmlElement]
		public string Time { get; set; }

		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string TimeZoneName { get; set; }
	}
}
