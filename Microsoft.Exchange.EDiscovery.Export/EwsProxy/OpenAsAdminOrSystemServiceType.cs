using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.1")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[XmlRoot("OpenAsAdminOrSystemService", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[Serializable]
	public class OpenAsAdminOrSystemServiceType : SoapHeader
	{
		public ConnectingSIDType ConnectingSID { get; set; }

		[XmlAttribute]
		public SpecialLogonType LogonType { get; set; }

		[XmlAttribute]
		public int BudgetType { get; set; }

		[XmlIgnore]
		public bool BudgetTypeSpecified { get; set; }

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr { get; set; }
	}
}
