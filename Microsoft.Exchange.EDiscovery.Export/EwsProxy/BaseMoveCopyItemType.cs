using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(CopyItemType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(MoveItemType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class BaseMoveCopyItemType : BaseRequestType
	{
		public TargetFolderIdType ToFolderId
		{
			get
			{
				return this.toFolderIdField;
			}
			set
			{
				this.toFolderIdField = value;
			}
		}

		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseItemIdType[] ItemIds
		{
			get
			{
				return this.itemIdsField;
			}
			set
			{
				this.itemIdsField = value;
			}
		}

		public bool ReturnNewItemIds
		{
			get
			{
				return this.returnNewItemIdsField;
			}
			set
			{
				this.returnNewItemIdsField = value;
			}
		}

		[XmlIgnore]
		public bool ReturnNewItemIdsSpecified
		{
			get
			{
				return this.returnNewItemIdsFieldSpecified;
			}
			set
			{
				this.returnNewItemIdsFieldSpecified = value;
			}
		}

		private TargetFolderIdType toFolderIdField;

		private BaseItemIdType[] itemIdsField;

		private bool returnNewItemIdsField;

		private bool returnNewItemIdsFieldSpecified;
	}
}
