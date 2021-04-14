using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetStreamingEventsType : BaseRequestType
	{
		[XmlArrayItem("SubscriptionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] SubscriptionIds
		{
			get
			{
				return this.subscriptionIdsField;
			}
			set
			{
				this.subscriptionIdsField = value;
			}
		}

		public int ConnectionTimeout
		{
			get
			{
				return this.connectionTimeoutField;
			}
			set
			{
				this.connectionTimeoutField = value;
			}
		}

		private string[] subscriptionIdsField;

		private int connectionTimeoutField;
	}
}
