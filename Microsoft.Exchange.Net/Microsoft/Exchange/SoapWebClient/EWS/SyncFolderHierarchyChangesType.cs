using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderHierarchyChangesType
	{
		[XmlElement("Update", typeof(SyncFolderHierarchyCreateOrUpdateType))]
		[XmlElement("Delete", typeof(SyncFolderHierarchyDeleteType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("Create", typeof(SyncFolderHierarchyCreateOrUpdateType))]
		public object[] Items;

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType1[] ItemsElementName;
	}
}
