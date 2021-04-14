using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class TokenIssuer
	{
		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string Uri
		{
			get
			{
				return this.uriField;
			}
			set
			{
				this.uriField = value;
			}
		}

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string Endpoint
		{
			get
			{
				return this.endpointField;
			}
			set
			{
				this.endpointField = value;
			}
		}

		private string uriField;

		private string endpointField;
	}
}
