using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TimeZoneContext : SoapHeader
	{
		[XmlAnyElement]
		public XmlElement TimeZoneDefinitionValue
		{
			get
			{
				return this.timeZoneDefinitionValue;
			}
			set
			{
				this.timeZoneDefinitionValue = value;
			}
		}

		private XmlElement timeZoneDefinitionValue;
	}
}
