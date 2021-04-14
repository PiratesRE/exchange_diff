using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompanyVerifiedDomainValue : CompanyDomainValue
	{
		public CompanyVerifiedDomainValue()
		{
			this.defaultField = false;
			this.initialField = false;
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool Default
		{
			get
			{
				return this.defaultField;
			}
			set
			{
				this.defaultField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool Initial
		{
			get
			{
				return this.initialField;
			}
			set
			{
				this.initialField = value;
			}
		}

		private bool defaultField;

		private bool initialField;
	}
}
