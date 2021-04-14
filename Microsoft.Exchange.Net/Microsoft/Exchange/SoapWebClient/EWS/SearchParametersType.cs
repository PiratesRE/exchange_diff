using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class SearchParametersType
	{
		public RestrictionType Restriction;

		[XmlArrayItem("FolderId", typeof(FolderIdType), IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), IsNullable = false)]
		public BaseFolderIdType[] BaseFolderIds;

		[XmlAttribute]
		public SearchFolderTraversalType Traversal;

		[XmlIgnore]
		public bool TraversalSpecified;
	}
}
