using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Tools
{
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CustomSupportedVersion : SupportedVersion
	{
		[XmlAttribute]
		public string minTenantVersion
		{
			get
			{
				return this.minTenantVersionField;
			}
			set
			{
				this.minTenantVersionField = value;
			}
		}

		[XmlAttribute]
		public string maxTenantVersion
		{
			get
			{
				return this.maxTenantVersionField;
			}
			set
			{
				this.maxTenantVersionField = value;
			}
		}

		private string minTenantVersionField;

		private string maxTenantVersionField;
	}
}
