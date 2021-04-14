using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetDiscoverySearchConfigurationResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("DiscoverySearchConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public DiscoverySearchConfigurationType[] DiscoverySearchConfigurations
		{
			get
			{
				return this.discoverySearchConfigurationsField;
			}
			set
			{
				this.discoverySearchConfigurationsField = value;
			}
		}

		private DiscoverySearchConfigurationType[] discoverySearchConfigurationsField;
	}
}
