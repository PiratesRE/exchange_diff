using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class GetFederationInformationResponse : AutodiscoverResponse
	{
		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string ApplicationUri
		{
			get
			{
				return this.applicationUriField;
			}
			set
			{
				this.applicationUriField = value;
			}
		}

		[XmlArray(IsNullable = true)]
		public TokenIssuer[] TokenIssuers
		{
			get
			{
				return this.tokenIssuersField;
			}
			set
			{
				this.tokenIssuersField = value;
			}
		}

		[XmlArray(IsNullable = true)]
		[XmlArrayItem("Domain")]
		public string[] Domains
		{
			get
			{
				return this.domainsField;
			}
			set
			{
				this.domainsField = value;
			}
		}

		private string applicationUriField;

		private TokenIssuer[] tokenIssuersField;

		private string[] domainsField;
	}
}
