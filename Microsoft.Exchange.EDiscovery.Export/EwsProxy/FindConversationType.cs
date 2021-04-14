using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FindConversationType : BaseRequestType
	{
		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageViewType))]
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

		public TargetFolderIdType ParentFolderId
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

		public MailboxSearchLocationType MailboxScope
		{
			get
			{
				return this.mailboxScopeField;
			}
			set
			{
				this.mailboxScopeField = value;
			}
		}

		[XmlIgnore]
		public bool MailboxScopeSpecified
		{
			get
			{
				return this.mailboxScopeFieldSpecified;
			}
			set
			{
				this.mailboxScopeFieldSpecified = value;
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

		public ConversationResponseShapeType ConversationShape
		{
			get
			{
				return this.conversationShapeField;
			}
			set
			{
				this.conversationShapeField = value;
			}
		}

		[XmlAttribute]
		public ConversationQueryTraversalType Traversal
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

		[XmlIgnore]
		public bool TraversalSpecified
		{
			get
			{
				return this.traversalFieldSpecified;
			}
			set
			{
				this.traversalFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public ViewFilterType ViewFilter
		{
			get
			{
				return this.viewFilterField;
			}
			set
			{
				this.viewFilterField = value;
			}
		}

		[XmlIgnore]
		public bool ViewFilterSpecified
		{
			get
			{
				return this.viewFilterFieldSpecified;
			}
			set
			{
				this.viewFilterFieldSpecified = value;
			}
		}

		private BasePagingType itemField;

		private FieldOrderType[] sortOrderField;

		private TargetFolderIdType parentFolderIdField;

		private MailboxSearchLocationType mailboxScopeField;

		private bool mailboxScopeFieldSpecified;

		private QueryStringType queryStringField;

		private ConversationResponseShapeType conversationShapeField;

		private ConversationQueryTraversalType traversalField;

		private bool traversalFieldSpecified;

		private ViewFilterType viewFilterField;

		private bool viewFilterFieldSpecified;
	}
}
