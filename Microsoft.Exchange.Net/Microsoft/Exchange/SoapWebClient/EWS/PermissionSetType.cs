using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PermissionSetType
	{
		[XmlArrayItem("Permission", IsNullable = false)]
		public PermissionType[] Permissions;

		[XmlArrayItem("UnknownEntry", IsNullable = false)]
		public string[] UnknownEntries;
	}
}
