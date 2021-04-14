using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetDomainSettingsResponse : AutodiscoverResponse
	{
		[XmlArray(IsNullable = true)]
		public DomainResponse[] DomainResponses
		{
			get
			{
				return this.domainResponsesField;
			}
			set
			{
				this.domainResponsesField = value;
			}
		}

		private DomainResponse[] domainResponsesField;
	}
}
