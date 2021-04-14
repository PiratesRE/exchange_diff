using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum OpenState
	{
		Disabled,
		Closed,
		Open,
		PendingClose,
		PendingOpen
	}
}
