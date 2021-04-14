using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValueStrongAuthenticationRequirement
	{
		[XmlElement(Order = 0)]
		public StrongAuthenticationRequirementValue StrongAuthenticationRequirement
		{
			get
			{
				return this.strongAuthenticationRequirementField;
			}
			set
			{
				this.strongAuthenticationRequirementField = value;
			}
		}

		private StrongAuthenticationRequirementValue strongAuthenticationRequirementField;
	}
}
