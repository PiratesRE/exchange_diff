using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class UpdateItemInRecoverableItemsType : BaseRequestType
	{
		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		[XmlArrayItem("AppendToItemField", typeof(AppendToItemFieldType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("SetItemField", typeof(SetItemFieldType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DeleteItemField", typeof(DeleteItemFieldType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
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

		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public AttachmentType[] Attachments
		{
			get
			{
				return this.attachmentsField;
			}
			set
			{
				this.attachmentsField = value;
			}
		}

		public bool MakeItemImmutable
		{
			get
			{
				return this.makeItemImmutableField;
			}
			set
			{
				this.makeItemImmutableField = value;
			}
		}

		[XmlIgnore]
		public bool MakeItemImmutableSpecified
		{
			get
			{
				return this.makeItemImmutableFieldSpecified;
			}
			set
			{
				this.makeItemImmutableFieldSpecified = value;
			}
		}

		private ItemIdType itemIdField;

		private ItemChangeDescriptionType[] updatesField;

		private AttachmentType[] attachmentsField;

		private bool makeItemImmutableField;

		private bool makeItemImmutableFieldSpecified;
	}
}
