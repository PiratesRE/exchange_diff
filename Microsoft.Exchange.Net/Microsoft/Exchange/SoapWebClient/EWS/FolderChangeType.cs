using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FolderChangeType
	{
		[XmlElement("FolderId", typeof(FolderIdType))]
		[XmlElement("DistinguishedFolderId", typeof(DistinguishedFolderIdType))]
		public BaseFolderIdType Item;

		[XmlArrayItem("SetFolderField", typeof(SetFolderFieldType), IsNullable = false)]
		[XmlArrayItem("AppendToFolderField", typeof(AppendToFolderFieldType), IsNullable = false)]
		[XmlArrayItem("DeleteFolderField", typeof(DeleteFolderFieldType), IsNullable = false)]
		public FolderChangeDescriptionType[] Updates;
	}
}
