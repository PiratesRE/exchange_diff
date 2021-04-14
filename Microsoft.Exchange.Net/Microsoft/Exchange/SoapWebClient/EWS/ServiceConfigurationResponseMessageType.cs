using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ServiceConfigurationResponseMessageType : ResponseMessageType
	{
		public MailTipsServiceConfiguration MailTipsConfiguration;

		public UnifiedMessageServiceConfiguration UnifiedMessagingConfiguration;

		public ProtectionRulesServiceConfiguration ProtectionRulesConfiguration;

		public PolicyNudgeRulesServiceConfiguration PolicyNudgeRulesConfiguration;
	}
}
