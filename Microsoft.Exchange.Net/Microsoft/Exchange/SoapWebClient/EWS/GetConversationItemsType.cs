using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetConversationItemsType : BaseRequestType
	{
		public ItemResponseShapeType ItemShape;

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] FoldersToIgnore;

		public int MaxItemsToReturn;

		[XmlIgnore]
		public bool MaxItemsToReturnSpecified;

		public ConversationNodeSortOrder SortOrder;

		[XmlIgnore]
		public bool SortOrderSpecified;

		public MailboxSearchLocationType MailboxScope;

		[XmlIgnore]
		public bool MailboxScopeSpecified;

		[XmlArrayItem("Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ConversationRequestType[] Conversations;
	}
}
