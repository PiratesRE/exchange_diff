using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UserConfigurationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ServiceUserConfiguration
	{
		[XmlElement("UserConfigurationName")]
		[DataMember(IsRequired = true, Order = 1)]
		public UserConfigurationNameType UserConfigurationName { get; set; }

		[DataMember(Name = "ItemId", IsRequired = false, EmitDefaultValue = false, Order = 2)]
		[XmlElement("ItemId")]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "Dictionary", IsRequired = false, EmitDefaultValue = false, Order = 3)]
		[XmlArrayItem("DictionaryEntry", IsNullable = false)]
		public UserConfigurationDictionaryEntry[] Dictionary { get; set; }

		[DataMember(Name = "XmlData", IsRequired = false, EmitDefaultValue = false, Order = 4)]
		[XmlElement]
		public string XmlData { get; set; }

		[DataMember(Name = "BinaryData", IsRequired = false, EmitDefaultValue = false, Order = 5)]
		public string BinaryData { get; set; }
	}
}
