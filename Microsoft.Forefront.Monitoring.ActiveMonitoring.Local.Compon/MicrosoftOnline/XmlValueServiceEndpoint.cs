using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class XmlValueServiceEndpoint
	{
		public ServiceEndpointValue ServiceEndpoint
		{
			get
			{
				return this.serviceEndpointField;
			}
			set
			{
				this.serviceEndpointField = value;
			}
		}

		private ServiceEndpointValue serviceEndpointField;
	}
}
