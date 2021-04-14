using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Publish
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://microsoft.com/DRM/PublishingService")]
	[Serializable]
	public class AcquireIssuanceLicenseParams
	{
		public XmlNode UnsignedIssuanceLicense
		{
			get
			{
				return this.unsignedIssuanceLicenseField;
			}
			set
			{
				this.unsignedIssuanceLicenseField = value;
			}
		}

		private XmlNode unsignedIssuanceLicenseField;
	}
}
