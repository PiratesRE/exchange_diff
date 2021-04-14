using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ClutterState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ClutterState
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		[XmlElement]
		public bool IsClutterEnabled { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		[XmlElement]
		public bool IsClutterEligible { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		[XmlElement]
		public bool IsClassificationEnabled { get; set; }
	}
}
