using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetAppMarketplaceUrlResponseMessageType : ResponseMessageType
	{
		public string AppMarketplaceUrl
		{
			get
			{
				return this.appMarketplaceUrlField;
			}
			set
			{
				this.appMarketplaceUrlField = value;
			}
		}

		private string appMarketplaceUrlField;
	}
}
