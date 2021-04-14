using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class DomainResponse : AutodiscoverResponse
	{
		[XmlArray(IsNullable = true)]
		public DomainSettingError[] DomainSettingErrors;

		[XmlArray(IsNullable = true)]
		public DomainSetting[] DomainSettings;

		[XmlElement(IsNullable = true)]
		public string RedirectTarget;
	}
}
