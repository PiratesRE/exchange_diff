using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class WebClientUrl
	{
		[XmlElement(IsNullable = true)]
		public string AuthenticationMethods
		{
			get
			{
				return this.authenticationMethodsField;
			}
			set
			{
				this.authenticationMethodsField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string Url
		{
			get
			{
				return this.urlField;
			}
			set
			{
				this.urlField = value;
			}
		}

		private string authenticationMethodsField;

		private string urlField;
	}
}
