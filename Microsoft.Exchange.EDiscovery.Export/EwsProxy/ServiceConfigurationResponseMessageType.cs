using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class ServiceConfigurationResponseMessageType : ResponseMessageType
	{
		public MailTipsServiceConfiguration MailTipsConfiguration
		{
			get
			{
				return this.mailTipsConfigurationField;
			}
			set
			{
				this.mailTipsConfigurationField = value;
			}
		}

		public UnifiedMessageServiceConfiguration UnifiedMessagingConfiguration
		{
			get
			{
				return this.unifiedMessagingConfigurationField;
			}
			set
			{
				this.unifiedMessagingConfigurationField = value;
			}
		}

		public ProtectionRulesServiceConfiguration ProtectionRulesConfiguration
		{
			get
			{
				return this.protectionRulesConfigurationField;
			}
			set
			{
				this.protectionRulesConfigurationField = value;
			}
		}

		public PolicyNudgeRulesServiceConfiguration PolicyNudgeRulesConfiguration
		{
			get
			{
				return this.policyNudgeRulesConfigurationField;
			}
			set
			{
				this.policyNudgeRulesConfigurationField = value;
			}
		}

		private MailTipsServiceConfiguration mailTipsConfigurationField;

		private UnifiedMessageServiceConfiguration unifiedMessagingConfigurationField;

		private ProtectionRulesServiceConfiguration protectionRulesConfigurationField;

		private PolicyNudgeRulesServiceConfiguration policyNudgeRulesConfigurationField;
	}
}
