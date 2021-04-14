using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(IsGreaterThanOrEqualToType))]
	[KnownType(typeof(IsLessThanType))]
	[XmlInclude(typeof(IsEqualToType))]
	[KnownType(typeof(IsNotEqualToType))]
	[XmlInclude(typeof(IsLessThanOrEqualToType))]
	[XmlInclude(typeof(IsLessThanType))]
	[XmlInclude(typeof(IsGreaterThanType))]
	[XmlInclude(typeof(IsNotEqualToType))]
	[KnownType(typeof(IsEqualToType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(IsLessThanOrEqualToType))]
	[KnownType(typeof(IsGreaterThanOrEqualToType))]
	[KnownType(typeof(IsGreaterThanType))]
	[Serializable]
	public abstract class TwoOperandExpressionType : SearchExpressionType
	{
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		public PropertyPath Item { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public FieldURIOrConstantType FieldURIOrConstant { get; set; }
	}
}
