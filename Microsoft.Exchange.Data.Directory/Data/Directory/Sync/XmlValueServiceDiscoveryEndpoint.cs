using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class XmlValueServiceDiscoveryEndpoint
	{
		[XmlElement(Order = 0)]
		public ServiceDiscoveryEndpointValue ServiceDiscoveryEndpoint
		{
			get
			{
				return this.serviceDiscoveryEndpointField;
			}
			set
			{
				this.serviceDiscoveryEndpointField = value;
			}
		}

		private ServiceDiscoveryEndpointValue serviceDiscoveryEndpointField;
	}
}
