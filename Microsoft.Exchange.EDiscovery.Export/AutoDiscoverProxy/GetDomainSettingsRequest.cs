using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetDomainSettingsRequest : AutodiscoverRequest
	{
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

		[XmlArrayItem("Setting")]
		[XmlArray(IsNullable = true)]
		public string[] RequestedSettings
		{
			get
			{
				return this.requestedSettingsField;
			}
			set
			{
				this.requestedSettingsField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public ExchangeVersion? RequestedVersion
		{
			get
			{
				return this.requestedVersionField;
			}
			set
			{
				this.requestedVersionField = value;
			}
		}

		private string[] domainsField;

		private string[] requestedSettingsField;

		private ExchangeVersion? requestedVersionField;
	}
}
