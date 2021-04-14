using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(SetFolderFieldType))]
	[XmlInclude(typeof(AppendToFolderFieldType))]
	[XmlInclude(typeof(DeleteFolderFieldType))]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FolderChangeDescriptionType : ChangeDescriptionType
	{
	}
}
