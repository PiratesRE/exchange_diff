using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(PropertyPath))]
	[KnownType(typeof(DictionaryPropertyUri))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(PropertyUri))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(ConstantValueType))]
	[KnownType(typeof(ExtendedPropertyUri))]
	[Serializable]
	public class FieldURIOrConstantType
	{
		[XmlElement("Constant", typeof(ConstantValueType))]
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		[XmlElement("Path", typeof(PropertyPath))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		public object Item { get; set; }
	}
}
