using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlRoot("ExchangeImpersonation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ExchangeImpersonationType : SoapHeader
	{
		public ConnectingSIDType ConnectingSID
		{
			get
			{
				return this.connectingSIDField;
			}
			set
			{
				this.connectingSIDField = value;
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

		private ConnectingSIDType connectingSIDField;

		private XmlAttribute[] anyAttrField;
	}
}
