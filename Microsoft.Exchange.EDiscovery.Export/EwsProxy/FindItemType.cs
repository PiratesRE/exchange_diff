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
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindItemType : BaseRequestType
	{
		public ItemResponseShapeType ItemShape
		{
			get
			{
				return this.itemShapeField;
			}
			set
			{
				this.itemShapeField = value;
			}
		}

		[XmlElement("CalendarView", typeof(CalendarViewType))]
		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageViewType))]
		[XmlElement("ContactsView", typeof(ContactsViewType))]
		[XmlElement("FractionalPageItemView", typeof(FractionalPageViewType))]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageViewType))]
		public BasePagingType Item
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

		[XmlElement("GroupBy", typeof(GroupByType))]
		[XmlElement("DistinguishedGroupBy", typeof(DistinguishedGroupByType))]
		public BaseGroupByType Item1
		{
			get
			{
				return this.item1Field;
			}
			set
			{
				this.item1Field = value;
			}
		}

		public RestrictionType Restriction
		{
			get
			{
				return this.restrictionField;
			}
			set
			{
				this.restrictionField = value;
			}
		}

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FieldOrderType[] SortOrder
		{
			get
			{
				return this.sortOrderField;
			}
			set
			{
				this.sortOrderField = value;
			}
		}

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] ParentFolderIds
		{
			get
			{
				return this.parentFolderIdsField;
			}
			set
			{
				this.parentFolderIdsField = value;
			}
		}

		public QueryStringType QueryString
		{
			get
			{
				return this.queryStringField;
			}
			set
			{
				this.queryStringField = value;
			}
		}

		[XmlAttribute]
		public ItemQueryTraversalType Traversal
		{
			get
			{
				return this.traversalField;
			}
			set
			{
				this.traversalField = value;
			}
		}

		private ItemResponseShapeType itemShapeField;

		private BasePagingType itemField;

		private BaseGroupByType item1Field;

		private RestrictionType restrictionField;

		private FieldOrderType[] sortOrderField;

		private BaseFolderIdType[] parentFolderIdsField;

		private QueryStringType queryStringField;

		private ItemQueryTraversalType traversalField;
	}
}
