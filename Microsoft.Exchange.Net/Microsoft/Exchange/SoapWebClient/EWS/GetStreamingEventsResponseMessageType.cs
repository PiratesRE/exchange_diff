using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetStreamingEventsResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Notification", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public NotificationType[] Notifications;

		[XmlArrayItem("SubscriptionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] ErrorSubscriptionIds;

		public ConnectionStatusType ConnectionStatus;

		[XmlIgnore]
		public bool ConnectionStatusSpecified;
	}
}
