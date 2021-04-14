using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class XmlValueAssignedLicense
	{
		public AssignedLicenseValue License
		{
			get
			{
				return this.licenseField;
			}
			set
			{
				this.licenseField = value;
			}
		}

		private AssignedLicenseValue licenseField;
	}
}
