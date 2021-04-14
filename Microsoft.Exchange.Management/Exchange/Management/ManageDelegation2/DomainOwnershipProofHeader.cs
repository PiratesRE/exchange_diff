using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.ManageDelegation2
{
	[XmlRoot(Namespace = "http://domains.live.com/Service/ManageDelegation2/V1.0", IsNullable = false)]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://domains.live.com/Service/ManageDelegation2/V1.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DomainOwnershipProofHeader : SoapHeader
	{
		public string Domain;

		public string HashAlgorithm;

		public string Signature;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;
	}
}
