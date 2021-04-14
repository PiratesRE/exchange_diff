using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UploadItemType
	{
		public FolderIdType ParentFolderId;

		public ItemIdType ItemId;

		[XmlElement(DataType = "base64Binary")]
		public byte[] Data;

		[XmlAttribute]
		public CreateActionType CreateAction;

		[XmlAttribute]
		public bool IsAssociated;

		[XmlIgnore]
		public bool IsAssociatedSpecified;
	}
}
