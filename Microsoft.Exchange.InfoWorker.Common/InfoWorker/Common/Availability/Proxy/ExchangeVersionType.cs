using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ExchangeVersionType
	{
		Exchange2007,
		Exchange2007_SP1,
		Exchange2009,
		Exchange2010,
		Exchange2010_SP1,
		Exchange2012
	}
}
