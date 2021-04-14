using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PhoneType
	{
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string OriginalPhoneString { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string PhoneString { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Type { get; set; }
	}
}
