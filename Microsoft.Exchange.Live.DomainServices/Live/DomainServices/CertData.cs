using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class CertData
	{
		public string CertIssuer
		{
			get
			{
				return this.certIssuerField;
			}
			set
			{
				this.certIssuerField = value;
			}
		}

		public string CertSubject
		{
			get
			{
				return this.certSubjectField;
			}
			set
			{
				this.certSubjectField = value;
			}
		}

		private string certIssuerField;

		private string certSubjectField;
	}
}
