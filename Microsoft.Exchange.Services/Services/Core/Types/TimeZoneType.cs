using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class TimeZoneType
	{
		[XmlElement]
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string BaseOffset { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public TimeChangeType Standard { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3)]
		public TimeChangeType Daylight { get; set; }

		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 0)]
		public string TimeZoneName { get; set; }
	}
}
