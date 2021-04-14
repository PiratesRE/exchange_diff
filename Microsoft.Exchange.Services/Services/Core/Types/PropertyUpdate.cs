using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(AppendItemPropertyUpdate))]
	[XmlType(TypeName = "ChangeDescriptionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(AppendFolderPropertyUpdate))]
	[KnownType(typeof(SetItemPropertyUpdate))]
	[XmlInclude(typeof(AppendFolderPropertyUpdate))]
	[XmlInclude(typeof(DeleteItemPropertyUpdate))]
	[KnownType(typeof(DeleteItemPropertyUpdate))]
	[XmlInclude(typeof(DeleteFolderPropertyUpdate))]
	[XmlInclude(typeof(SetItemPropertyUpdate))]
	[KnownType(typeof(AppendItemPropertyUpdate))]
	[XmlInclude(typeof(SetFolderPropertyUpdate))]
	[KnownType(typeof(DeleteFolderPropertyUpdate))]
	[KnownType(typeof(SetFolderPropertyUpdate))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class PropertyUpdate
	{
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		[XmlElement("Path")]
		[DataMember(IsRequired = true, Name = "Path")]
		public PropertyPath PropertyPath { get; set; }
	}
}
