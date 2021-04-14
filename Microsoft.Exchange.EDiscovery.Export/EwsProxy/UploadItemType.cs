using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UploadItemType
	{
		public FolderIdType ParentFolderId
		{
			get
			{
				return this.parentFolderIdField;
			}
			set
			{
				this.parentFolderIdField = value;
			}
		}

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

		[XmlElement(DataType = "base64Binary")]
		public byte[] Data
		{
			get
			{
				return this.dataField;
			}
			set
			{
				this.dataField = value;
			}
		}

		[XmlAttribute]
		public CreateActionType CreateAction
		{
			get
			{
				return this.createActionField;
			}
			set
			{
				this.createActionField = value;
			}
		}

		[XmlAttribute]
		public bool IsAssociated
		{
			get
			{
				return this.isAssociatedField;
			}
			set
			{
				this.isAssociatedField = value;
			}
		}

		[XmlIgnore]
		public bool IsAssociatedSpecified
		{
			get
			{
				return this.isAssociatedFieldSpecified;
			}
			set
			{
				this.isAssociatedFieldSpecified = value;
			}
		}

		private FolderIdType parentFolderIdField;

		private ItemIdType itemIdField;

		private byte[] dataField;

		private CreateActionType createActionField;

		private bool isAssociatedField;

		private bool isAssociatedFieldSpecified;
	}
}
