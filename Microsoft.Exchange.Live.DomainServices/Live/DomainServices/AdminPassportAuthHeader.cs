using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[XmlRoot(Namespace = "http://domains.live.com/Service/DomainServices/V1.0", IsNullable = false)]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[Serializable]
	public class AdminPassportAuthHeader : SoapHeader
	{
		public string NetId
		{
			get
			{
				return this.netIdField;
			}
			set
			{
				this.netIdField = value;
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

		private string netIdField;

		private XmlAttribute[] anyAttrField;
	}
}
