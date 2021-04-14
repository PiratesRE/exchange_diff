using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UserConfigurationDictionaryEntryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UserConfigurationDictionaryEntry
	{
		[DataMember(IsRequired = true)]
		[XmlElement]
		public UserConfigurationDictionaryObject DictionaryKey { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement(IsNullable = true)]
		public UserConfigurationDictionaryObject DictionaryValue { get; set; }
	}
}
