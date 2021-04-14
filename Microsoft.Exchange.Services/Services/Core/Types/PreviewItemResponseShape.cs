using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "PreviewItemResponseShape", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PreviewItemResponseShapeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PreviewItemResponseShape
	{
		public PreviewItemResponseShape()
		{
		}

		internal PreviewItemResponseShape(PreviewItemBaseShape baseShape, ExtendedPropertyUri[] additionalProperties)
		{
			this.BaseShape = baseShape;
			this.AdditionalProperties = additionalProperties;
		}

		[DataMember(Name = "BaseShape", IsRequired = true)]
		[XmlElement("BaseShape")]
		public PreviewItemBaseShape BaseShape { get; set; }

		[XmlArrayItem(ElementName = "ExtendedFieldURI", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ExtendedPropertyUri))]
		[IgnoreDataMember]
		[XmlArray(ElementName = "AdditionalProperties", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ExtendedPropertyUri[] AdditionalProperties { get; set; }
	}
}
