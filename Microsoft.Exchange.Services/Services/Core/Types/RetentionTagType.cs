using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RetentionTagType
	{
		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public bool IsExplicit { get; set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 2)]
		[XmlText]
		public string Value { get; set; }
	}
}
