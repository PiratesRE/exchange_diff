using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetConversationItemsType : BaseRequestType
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

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] FoldersToIgnore
		{
			get
			{
				return this.foldersToIgnoreField;
			}
			set
			{
				this.foldersToIgnoreField = value;
			}
		}

		public int MaxItemsToReturn
		{
			get
			{
				return this.maxItemsToReturnField;
			}
			set
			{
				this.maxItemsToReturnField = value;
			}
		}

		[XmlIgnore]
		public bool MaxItemsToReturnSpecified
		{
			get
			{
				return this.maxItemsToReturnFieldSpecified;
			}
			set
			{
				this.maxItemsToReturnFieldSpecified = value;
			}
		}

		public ConversationNodeSortOrder SortOrder
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

		[XmlIgnore]
		public bool SortOrderSpecified
		{
			get
			{
				return this.sortOrderFieldSpecified;
			}
			set
			{
				this.sortOrderFieldSpecified = value;
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

		[XmlArrayItem("Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ConversationRequestType[] Conversations
		{
			get
			{
				return this.conversationsField;
			}
			set
			{
				this.conversationsField = value;
			}
		}

		private ItemResponseShapeType itemShapeField;

		private BaseFolderIdType[] foldersToIgnoreField;

		private int maxItemsToReturnField;

		private bool maxItemsToReturnFieldSpecified;

		private ConversationNodeSortOrder sortOrderField;

		private bool sortOrderFieldSpecified;

		private MailboxSearchLocationType mailboxScopeField;

		private bool mailboxScopeFieldSpecified;

		private ConversationRequestType[] conversationsField;
	}
}
