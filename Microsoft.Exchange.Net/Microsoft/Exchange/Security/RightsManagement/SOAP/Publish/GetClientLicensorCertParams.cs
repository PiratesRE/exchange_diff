using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Publish
{
	[XmlType(Namespace = "http://microsoft.com/DRM/PublishingService")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetClientLicensorCertParams
	{
		[XmlArrayItem("Certificate")]
		public XmlNode[] PersonaCerts
		{
			get
			{
				return this.personaCertsField;
			}
			set
			{
				this.personaCertsField = value;
			}
		}

		private XmlNode[] personaCertsField;
	}
}
