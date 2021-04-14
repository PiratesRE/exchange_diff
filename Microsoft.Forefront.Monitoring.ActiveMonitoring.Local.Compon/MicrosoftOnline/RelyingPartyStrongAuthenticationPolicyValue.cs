using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class RelyingPartyStrongAuthenticationPolicyValue
	{
		public RelyingPartyStrongAuthenticationPolicyValue()
		{
			this.enabledField = true;
		}

		[XmlArrayItem("RelyingParty", DataType = "token", IsNullable = false)]
		public string[] RelyingParties
		{
			get
			{
				return this.relyingPartiesField;
			}
			set
			{
				this.relyingPartiesField = value;
			}
		}

		[XmlArrayItem("Rule", IsNullable = false)]
		public StrongAuthenticationRuleValue[] Rules
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

		[XmlAttribute]
		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return this.enabledField;
			}
			set
			{
				this.enabledField = value;
			}
		}

		private string[] relyingPartiesField;

		private StrongAuthenticationRuleValue[] rulesField;

		private bool enabledField;
	}
}
