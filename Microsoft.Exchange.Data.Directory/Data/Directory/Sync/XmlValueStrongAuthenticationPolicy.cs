using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValueStrongAuthenticationPolicy
	{
		[XmlElement(Order = 0)]
		public StrongAuthenticationPolicyValue StrongAuthenticationPolicy
		{
			get
			{
				return this.strongAuthenticationPolicyField;
			}
			set
			{
				this.strongAuthenticationPolicyField = value;
			}
		}

		private StrongAuthenticationPolicyValue strongAuthenticationPolicyField;
	}
}
