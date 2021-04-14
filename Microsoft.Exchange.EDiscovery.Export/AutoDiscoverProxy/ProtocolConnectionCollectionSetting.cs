using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class ProtocolConnectionCollectionSetting : UserSetting
	{
		[XmlArray(IsNullable = true)]
		public ProtocolConnection[] ProtocolConnections
		{
			get
			{
				return this.protocolConnectionsField;
			}
			set
			{
				this.protocolConnectionsField = value;
			}
		}

		private ProtocolConnection[] protocolConnectionsField;
	}
}
