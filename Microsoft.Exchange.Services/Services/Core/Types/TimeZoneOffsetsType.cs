using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TimeZoneOffsetsType
	{
		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string TimeZoneId { get; set; }

		[XmlArrayItem(ElementName = "OffsetRange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlArray(ElementName = "OffsetRanges")]
		public TimeZoneRangeType[] OffsetRanges { get; set; }
	}
}
