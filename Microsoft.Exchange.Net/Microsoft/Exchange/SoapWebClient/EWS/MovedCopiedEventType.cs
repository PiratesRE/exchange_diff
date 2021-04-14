using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class MovedCopiedEventType : BaseObjectChangedEventType
	{
		[XmlElement("OldFolderId", typeof(FolderIdType))]
		[XmlElement("OldItemId", typeof(ItemIdType))]
		public object Item1;

		public FolderIdType OldParentFolderId;
	}
}
