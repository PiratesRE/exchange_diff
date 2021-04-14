using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MimeContentType
	{
		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string CharacterSet { get; set; }

		[XmlText]
		[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 1)]
		public string Value { get; set; }
	}
}
