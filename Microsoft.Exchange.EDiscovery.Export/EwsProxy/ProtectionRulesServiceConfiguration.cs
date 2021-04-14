using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class ProtectionRulesServiceConfiguration : ServiceConfiguration
	{
		[XmlArrayItem("Rule", IsNullable = false)]
		public ProtectionRuleType[] Rules
		{
			get
			{
				return this.rulesField;
			}
			set
			{
				this.rulesField = value;
			}
		}

		[XmlArrayItem("Domain", IsNullable = false)]
		public SmtpDomain[] InternalDomains
		{
			get
			{
				return this.internalDomainsField;
			}
			set
			{
				this.internalDomainsField = value;
			}
		}

		[XmlAttribute]
		public int RefreshInterval
		{
			get
			{
				return this.refreshIntervalField;
			}
			set
			{
				this.refreshIntervalField = value;
			}
		}

		private ProtectionRuleType[] rulesField;

		private SmtpDomain[] internalDomainsField;

		private int refreshIntervalField;
	}
}
