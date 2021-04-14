using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderItemsChangesType
	{
		[XmlElement("ReadFlagChange", typeof(SyncFolderItemsReadFlagType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("Delete", typeof(SyncFolderItemsDeleteType))]
		[XmlElement("Create", typeof(SyncFolderItemsCreateOrUpdateType))]
		[XmlElement("Update", typeof(SyncFolderItemsCreateOrUpdateType))]
		public object[] Items;

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType2[] ItemsElementName;
	}
}
