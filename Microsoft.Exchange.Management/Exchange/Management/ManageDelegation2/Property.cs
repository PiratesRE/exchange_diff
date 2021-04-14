using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.ManageDelegation2
{
	[XmlType(Namespace = "http://domains.live.com/Service/ManageDelegation2/V1.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class Property
	{
		public string Name;

		public string Value;
	}
}
