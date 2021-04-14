using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class StrongAuthenticationPolicyValue
	{
		[XmlArrayItem("RelyingPartyStrongAuthenticationPolicy", IsNullable = false)]
		[XmlArray(Order = 0)]
		public RelyingPartyStrongAuthenticationPolicyValue[] RelyingPartyStrongAuthenticationPolicies
		{
			get
			{
				return this.relyingPartyStrongAuthenticationPoliciesField;
			}
			set
			{
				this.relyingPartyStrongAuthenticationPoliciesField = value;
			}
		}

		private RelyingPartyStrongAuthenticationPolicyValue[] relyingPartyStrongAuthenticationPoliciesField;
	}
}
