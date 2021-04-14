using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Flags]
	[Serializable]
	public enum ServiceConfigurationType
	{
		MailTips = 1,
		UnifiedMessagingConfiguration = 2,
		ProtectionRules = 4,
		PolicyNudges = 8
	}
}
