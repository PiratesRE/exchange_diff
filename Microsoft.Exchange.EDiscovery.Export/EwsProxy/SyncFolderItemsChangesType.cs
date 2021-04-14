using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderItemsChangesType
	{
		[XmlElement("Create", typeof(SyncFolderItemsCreateOrUpdateType))]
		[XmlElement("Delete", typeof(SyncFolderItemsDeleteType))]
		[XmlElement("ReadFlagChange", typeof(SyncFolderItemsReadFlagType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("Update", typeof(SyncFolderItemsCreateOrUpdateType))]
		public object[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType2[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		private object[] itemsField;

		private ItemsChoiceType2[] itemsElementNameField;
	}
}
