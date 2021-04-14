using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public enum ExchangeVersion
	{
		Exchange2010,
		Exchange2010_SP1,
		Exchange2010_SP2,
		Exchange2013,
		Exchange2013_SP1
	}
}
