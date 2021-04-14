using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class Company
	{
		public string DomainPrefix
		{
			get
			{
				return this.domainPrefixField;
			}
			set
			{
				this.domainPrefixField = value;
			}
		}

		public string DomainSuffix
		{
			get
			{
				return this.domainSuffixField;
			}
			set
			{
				this.domainSuffixField = value;
			}
		}

		public CompanyProfile CompanyProfile
		{
			get
			{
				return this.companyProfileField;
			}
			set
			{
				this.companyProfileField = value;
			}
		}

		private string domainPrefixField;

		private string domainSuffixField;

		private CompanyProfile companyProfileField;
	}
}
