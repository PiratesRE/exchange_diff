using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[XmlRoot(Namespace = "http://domains.live.com/Service/DomainServices/V1.0", IsNullable = false)]
	[Serializable]
	public class ManagementCertificateAuthHeader : SoapHeader
	{
		public CertData ManagementCertificate
		{
			get
			{
				return this.managementCertificateField;
			}
			set
			{
				this.managementCertificateField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private CertData managementCertificateField;

		private XmlAttribute[] anyAttrField;
	}
}
