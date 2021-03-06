using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ResolveNamesType : BaseRequestType
	{
		public ResolveNamesType()
		{
			this.SearchScope = ResolveNamesSearchScopeType.ActiveDirectoryContacts;
		}

		[XmlArrayItem("FolderId", typeof(FolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderIdType[] ParentFolderIds;

		public string UnresolvedEntry;

		[XmlAttribute]
		public bool ReturnFullContactData;

		[DefaultValue(ResolveNamesSearchScopeType.ActiveDirectoryContacts)]
		[XmlAttribute]
		public ResolveNamesSearchScopeType SearchScope;

		[XmlAttribute]
		public DefaultShapeNamesType ContactDataShape;

		[XmlIgnore]
		public bool ContactDataShapeSpecified;
	}
}
