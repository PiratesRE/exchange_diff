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
	public class GetClientExtensionUserParametersType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserEnabledExtensions;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserDisabledExtensions;

		[XmlAttribute]
		public string UserId;

		[XmlAttribute]
		public bool EnabledOnly;

		[XmlIgnore]
		public bool EnabledOnlySpecified;
	}
}
