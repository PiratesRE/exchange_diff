using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://microsoft.com/DRM/CertificationService")]
	[Serializable]
	public class CertifyParams
	{
		[XmlArrayItem("Certificate")]
		public XmlNode[] MachineCertificateChain
		{
			get
			{
				return this.machineCertificateChainField;
			}
			set
			{
				this.machineCertificateChainField = value;
			}
		}

		public bool Persistent
		{
			get
			{
				return this.persistentField;
			}
			set
			{
				this.persistentField = value;
			}
		}

		private XmlNode[] machineCertificateChainField;

		private bool persistentField;
	}
}
