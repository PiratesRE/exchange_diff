using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class ServiceInstanceInfoValue
	{
		[XmlElement("Endpoint", Order = 0)]
		public ServiceEndpointValue[] Endpoint
		{
			get
			{
				return this.endpointField;
			}
			set
			{
				this.endpointField = value;
			}
		}

		[XmlAnyElement(Order = 1)]
		public XmlElement[] Any
		{
			get
			{
				return this.anyField;
			}
			set
			{
				this.anyField = value;
			}
		}

		private ServiceEndpointValue[] endpointField;

		private XmlElement[] anyField;
	}
}
