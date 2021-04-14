using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class OccurrenceInfoType
	{
		[XmlElement]
		[DataMember(IsRequired = true, EmitDefaultValue = true, Order = 1)]
		public ItemId ItemId { get; set; }

		[DataMember(IsRequired = true, EmitDefaultValue = true, Order = 2)]
		[XmlElement]
		[DateTimeString]
		public string Start { get; set; }

		[XmlElement]
		[DataMember(IsRequired = true, EmitDefaultValue = true, Order = 3)]
		[DateTimeString]
		public string End { get; set; }

		[DataMember(IsRequired = true, EmitDefaultValue = true, Order = 4)]
		[DateTimeString]
		[XmlElement]
		public string OriginalStart { get; set; }
	}
}
