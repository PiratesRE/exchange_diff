using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class RelyingPartyStrongAuthenticationPolicyValue
	{
		public RelyingPartyStrongAuthenticationPolicyValue()
		{
			this.enabledField = true;
		}

		[XmlArrayItem("RelyingParty", DataType = "token", IsNullable = false)]
		[XmlArray(Order = 0)]
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
		[XmlArray(Order = 1)]
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
