using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindItemType : BaseRequestType
	{
		public ItemResponseShapeType ItemShape;

		[XmlElement("CalendarView", typeof(CalendarViewType))]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageViewType))]
		[XmlElement("ContactsView", typeof(ContactsViewType))]
		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageViewType))]
		[XmlElement("FractionalPageItemView", typeof(FractionalPageViewType))]
		public BasePagingType Item;

		[XmlElement("GroupBy", typeof(GroupByType))]
		[XmlElement("DistinguishedGroupBy", typeof(DistinguishedGroupByType))]
		public BaseGroupByType Item1;

		public RestrictionType Restriction;

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FieldOrderType[] SortOrder;

		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] ParentFolderIds;

		public QueryStringType QueryString;

		[XmlAttribute]
		public ItemQueryTraversalType Traversal;
	}
}
