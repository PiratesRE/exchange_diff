using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.ManageDelegation1
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/ManageDelegation/V1.0")]
	[Serializable]
	public class DomainInfo
	{
		public string DomainName
		{
			get
			{
				return this.domainNameField;
			}
			set
			{
				this.domainNameField = value;
			}
		}

		public string AppId
		{
			get
			{
				return this.appIdField;
			}
			set
			{
				this.appIdField = value;
			}
		}

		public DomainState DomainState
		{
			get
			{
				return this.domainStateField;
			}
			set
			{
				this.domainStateField = value;
			}
		}

		private string domainNameField;

		private string appIdField;

		private DomainState domainStateField;
	}
}
