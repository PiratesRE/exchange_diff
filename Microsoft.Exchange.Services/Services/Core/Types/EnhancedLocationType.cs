using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "EnhancedLocation")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "EnhancedLocationType")]
	[Serializable]
	public class EnhancedLocationType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string DisplayName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Annotation { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public PostalAddress PostalAddress { get; set; }
	}
}
