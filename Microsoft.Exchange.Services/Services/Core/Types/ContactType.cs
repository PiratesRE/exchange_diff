using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContactType : BaseEntityType
	{
		[XmlElement]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string PersonName { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string BusinessName { get; set; }

		[XmlArrayItem(ElementName = "Phone", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(PhoneType))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public PhoneType[] PhoneNumbers { get; set; }

		[XmlArrayItem(ElementName = "Url", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string[] Urls { get; set; }

		[XmlArrayItem(ElementName = "EmailAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string[] EmailAddresses { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "Address", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] Addresses { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement]
		public string ContactString { get; set; }
	}
}
