using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://microsoft.com/DRM/LicensingService")]
	[Serializable]
	public class AcquirePreLicenseResponse
	{
		[XmlArrayItem("Certificate")]
		public XmlNode[] Licenses
		{
			get
			{
				return this.licensesField;
			}
			set
			{
				this.licensesField = value;
			}
		}

		[XmlArrayItem("Certificate")]
		public XmlNode[] CertificateChain
		{
			get
			{
				return this.certificateChainField;
			}
			set
			{
				this.certificateChainField = value;
			}
		}

		[XmlArrayItem("Certificate")]
		public XmlNode[] ReferenceCertificates
		{
			get
			{
				return this.referenceCertificatesField;
			}
			set
			{
				this.referenceCertificatesField = value;
			}
		}

		private XmlNode[] licensesField;

		private XmlNode[] certificateChainField;

		private XmlNode[] referenceCertificatesField;
	}
}
