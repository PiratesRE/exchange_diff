using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(SetFolderFieldType))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DeleteFolderFieldType))]
	[XmlInclude(typeof(AppendToFolderFieldType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FolderChangeDescriptionType : ChangeDescriptionType
	{
	}
}
