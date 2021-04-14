using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ItemChangeType
	{
		[XmlElement("OccurrenceItemId", typeof(OccurrenceItemIdType))]
		[XmlElement("ItemId", typeof(ItemIdType))]
		[XmlElement("RecurringMasterItemId", typeof(RecurringMasterItemIdType))]
		public BaseItemIdType Item
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

		[XmlArrayItem("DeleteItemField", typeof(DeleteItemFieldType), IsNullable = false)]
		[XmlArrayItem("SetItemField", typeof(SetItemFieldType), IsNullable = false)]
		[XmlArrayItem("AppendToItemField", typeof(AppendToItemFieldType), IsNullable = false)]
		public ItemChangeDescriptionType[] Updates
		{
			get
			{
				return this.updatesField;
			}
			set
			{
				this.updatesField = value;
			}
		}

		private BaseItemIdType itemField;

		private ItemChangeDescriptionType[] updatesField;
	}
}
