using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetDiscoverySearchConfigurationResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("DiscoverySearchConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public DiscoverySearchConfigurationType[] DiscoverySearchConfigurations;
	}
}
