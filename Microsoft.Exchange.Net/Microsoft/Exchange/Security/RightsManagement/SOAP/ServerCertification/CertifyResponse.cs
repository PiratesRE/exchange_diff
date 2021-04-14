using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://microsoft.com/DRM/CertificationService")]
	[Serializable]
	public class CertifyResponse
	{
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

		public QuotaResponse Quota
		{
			get
			{
				return this.quotaField;
			}
			set
			{
				this.quotaField = value;
			}
		}

		private XmlNode[] certificateChainField;

		private QuotaResponse quotaField;
	}
}
