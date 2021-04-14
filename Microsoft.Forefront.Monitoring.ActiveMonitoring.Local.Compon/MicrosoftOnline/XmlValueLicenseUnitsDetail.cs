using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValueLicenseUnitsDetail
	{
		public LicenseUnitsDetailValue LicenseUnitsDetail
		{
			get
			{
				return this.licenseUnitsDetailField;
			}
			set
			{
				this.licenseUnitsDetailField = value;
			}
		}

		private LicenseUnitsDetailValue licenseUnitsDetailField;
	}
}
