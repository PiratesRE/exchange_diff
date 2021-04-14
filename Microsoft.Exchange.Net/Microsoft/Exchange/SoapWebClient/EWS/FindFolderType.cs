using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindFolderType : BaseRequestType
	{
		public FolderResponseShapeType FolderShape;

		[XmlElement("FractionalPageFolderView", typeof(FractionalPageViewType))]
		[XmlElement("IndexedPageFolderView", typeof(IndexedPageViewType))]
		public BasePagingType Item;

		public RestrictionType Restriction;

		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] ParentFolderIds;

		[XmlAttribute]
		public FolderQueryTraversalType Traversal;
	}
}
