using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CalendarPermissionSetType
	{
		[XmlArrayItem("CalendarPermission", IsNullable = false)]
		public CalendarPermissionType[] CalendarPermissions;

		[XmlArrayItem("UnknownEntry", IsNullable = false)]
		public string[] UnknownEntries;
	}
}
