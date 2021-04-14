using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SubscribeType : BaseRequestType
	{
		[XmlElement("PullSubscriptionRequest", typeof(PullSubscriptionRequestType))]
		[XmlElement("StreamingSubscriptionRequest", typeof(StreamingSubscriptionRequestType))]
		[XmlElement("PushSubscriptionRequest", typeof(PushSubscriptionRequestType))]
		public object Item;
	}
}
