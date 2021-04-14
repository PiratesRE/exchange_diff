using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class ServiceDiscoveryEndpointValue
	{
		[XmlAttribute]
		public string EndpointId
		{
			get
			{
				return this.endpointIdField;
			}
			set
			{
				this.endpointIdField = value;
			}
		}

		[XmlAttribute]
		public string Capability
		{
			get
			{
				return this.capabilityField;
			}
			set
			{
				this.capabilityField = value;
			}
		}

		[XmlAttribute]
		public string ServiceId
		{
			get
			{
				return this.serviceIdField;
			}
			set
			{
				this.serviceIdField = value;
			}
		}

		[XmlAttribute]
		public string ServiceName
		{
			get
			{
				return this.serviceNameField;
			}
			set
			{
				this.serviceNameField = value;
			}
		}

		[XmlAttribute]
		public string ServiceEndpointUri
		{
			get
			{
				return this.serviceEndpointUriField;
			}
			set
			{
				this.serviceEndpointUriField = value;
			}
		}

		[XmlAttribute]
		public string ServiceResourceId
		{
			get
			{
				return this.serviceResourceIdField;
			}
			set
			{
				this.serviceResourceIdField = value;
			}
		}

		private string endpointIdField;

		private string capabilityField;

		private string serviceIdField;

		private string serviceNameField;

		private string serviceEndpointUriField;

		private string serviceResourceIdField;
	}
}
