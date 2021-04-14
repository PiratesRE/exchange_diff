using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class WebClientUrlCollectionSetting : UserSetting
	{
		[XmlArray(IsNullable = true)]
		public WebClientUrl[] WebClientUrls
		{
			get
			{
				return this.webClientUrlsField;
			}
			set
			{
				this.webClientUrlsField = value;
			}
		}

		private WebClientUrl[] webClientUrlsField;
	}
}
