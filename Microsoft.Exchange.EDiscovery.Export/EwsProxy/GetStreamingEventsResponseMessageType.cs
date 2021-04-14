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
	public class GetStreamingEventsResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Notification", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public NotificationType[] Notifications
		{
			get
			{
				return this.notificationsField;
			}
			set
			{
				this.notificationsField = value;
			}
		}

		[XmlArrayItem("SubscriptionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] ErrorSubscriptionIds
		{
			get
			{
				return this.errorSubscriptionIdsField;
			}
			set
			{
				this.errorSubscriptionIdsField = value;
			}
		}

		public ConnectionStatusType ConnectionStatus
		{
			get
			{
				return this.connectionStatusField;
			}
			set
			{
				this.connectionStatusField = value;
			}
		}

		[XmlIgnore]
		public bool ConnectionStatusSpecified
		{
			get
			{
				return this.connectionStatusFieldSpecified;
			}
			set
			{
				this.connectionStatusFieldSpecified = value;
			}
		}

		private NotificationType[] notificationsField;

		private string[] errorSubscriptionIdsField;

		private ConnectionStatusType connectionStatusField;

		private bool connectionStatusFieldSpecified;
	}
}
