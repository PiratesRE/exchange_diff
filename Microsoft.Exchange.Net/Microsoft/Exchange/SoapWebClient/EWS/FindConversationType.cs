using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class FindConversationType : BaseRequestType
	{
		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageViewType))]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageViewType))]
		public BasePagingType Item;

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FieldOrderType[] SortOrder;

		public TargetFolderIdType ParentFolderId;

		public MailboxSearchLocationType MailboxScope;

		[XmlIgnore]
		public bool MailboxScopeSpecified;

		public QueryStringType QueryString;

		public ConversationResponseShapeType ConversationShape;

		[XmlAttribute]
		public ConversationQueryTraversalType Traversal;

		[XmlIgnore]
		public bool TraversalSpecified;

		[XmlAttribute]
		public ViewFilterType ViewFilter;

		[XmlIgnore]
		public bool ViewFilterSpecified;
	}
}
