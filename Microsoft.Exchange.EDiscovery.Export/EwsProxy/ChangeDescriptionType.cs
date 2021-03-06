using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(FolderChangeDescriptionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AppendToFolderFieldType))]
	[XmlInclude(typeof(DeleteFolderFieldType))]
	[XmlInclude(typeof(SetFolderFieldType))]
	[XmlInclude(typeof(ItemChangeDescriptionType))]
	[XmlInclude(typeof(AppendToItemFieldType))]
	[XmlInclude(typeof(DeleteItemFieldType))]
	[XmlInclude(typeof(SetItemFieldType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class ChangeDescriptionType
	{
		[XmlElement("IndexedFieldURI", typeof(PathToIndexedFieldType))]
		[XmlElement("FieldURI", typeof(PathToUnindexedFieldType))]
		[XmlElement("ExtendedFieldURI", typeof(PathToExtendedFieldType))]
		public BasePathToElementType Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		private BasePathToElementType itemField;
	}
}
