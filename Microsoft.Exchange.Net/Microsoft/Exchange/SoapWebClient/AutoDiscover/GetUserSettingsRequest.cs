using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetUserSettingsRequest : AutodiscoverRequest
	{
		[XmlArray(IsNullable = true)]
		public User[] Users;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem("Setting")]
		public string[] RequestedSettings;

		[XmlElement(IsNullable = true)]
		public ExchangeVersion? RequestedVersion;
	}
}
