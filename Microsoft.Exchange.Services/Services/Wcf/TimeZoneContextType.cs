using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Name = "TimeZoneContext", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlRoot(ElementName = "TimeZoneContext", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TimeZoneContextType
	{
		[DataMember(Name = "TimeZoneDefinition", EmitDefaultValue = false, Order = 1)]
		[XmlElement]
		public TimeZoneDefinitionType TimeZoneDefinition { get; set; }
	}
}
