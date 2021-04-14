using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://microsoft.com/DRM/LicensingService")]
	[Serializable]
	public class AcquirePreLicenseParams
	{
		public string[] LicenseeIdentities
		{
			get
			{
				return this.licenseeIdentitiesField;
			}
			set
			{
				this.licenseeIdentitiesField = value;
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

		private string[] licenseeIdentitiesField;

		private XmlNode[] issuanceLicenseField;

		private XmlNode applicationDataField;
	}
}
