using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(DeleteItemFieldType))]
	[XmlInclude(typeof(DeleteFolderFieldType))]
	[XmlInclude(typeof(SetFolderFieldType))]
	[XmlInclude(typeof(ItemChangeDescriptionType))]
	[XmlInclude(typeof(AppendToItemFieldType))]
	[XmlInclude(typeof(FolderChangeDescriptionType))]
	[XmlInclude(typeof(SetItemFieldType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AppendToFolderFieldType))]
	[Serializable]
	public abstract class ChangeDescriptionType
	{
		[XmlElement("FieldURI", typeof(PathToUnindexedFieldType))]
		[XmlElement("ExtendedFieldURI", typeof(PathToExtendedFieldType))]
		[XmlElement("IndexedFieldURI", typeof(PathToIndexedFieldType))]
		public BasePathToElementType Item;
	}
}
