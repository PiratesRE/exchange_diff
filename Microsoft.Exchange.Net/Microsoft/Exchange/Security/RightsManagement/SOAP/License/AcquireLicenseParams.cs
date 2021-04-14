using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://microsoft.com/DRM/LicensingService")]
	[Serializable]
	public class AcquireLicenseParams
	{
		[XmlArrayItem("Certificate")]
		public XmlNode[] LicenseeCerts
		{
			get
			{
				return this.licenseeCertsField;
			}
			set
			{
				this.licenseeCertsField = value;
			}
		}

		[XmlArrayItem("Certificate")]
		public XmlNode[] IssuanceLicense
		{
			get
			{
				return this.issuanceLicenseField;
			}
			set
			{
				this.issuanceLicenseField = value;
			}
		}

		public XmlNode ApplicationData
		{
			get
			{
				return this.applicationDataField;
			}
			set
			{
				this.applicationDataField = value;
			}
		}

		private XmlNode[] licenseeCertsField;

		private XmlNode[] issuanceLicenseField;

		private XmlNode applicationDataField;
	}
}
