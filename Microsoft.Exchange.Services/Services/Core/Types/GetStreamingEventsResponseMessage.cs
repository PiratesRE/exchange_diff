using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetStreamingEventsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetStreamingEventsResponseMessage : ResponseMessage
	{
		public GetStreamingEventsResponseMessage()
		{
			this.ConnectionStatusSpecified = false;
		}

		internal GetStreamingEventsResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		[DataMember(Name = "Notifications", IsRequired = true)]
		[XmlArrayItem(ElementName = "Notification", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArray(ElementName = "Notifications", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public EwsNotificationType[] Notifications { get; set; }

		[XmlArrayItem(ElementName = "SubscriptionId", Type = typeof(string), Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArray(ElementName = "ErrorSubscriptionIds", IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string[] ErrorSubscriptionIds { get; set; }

		[XmlElement(ElementName = "ConnectionStatus", IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Type = typeof(ConnectionStatus))]
		public ConnectionStatus ConnectionStatus { get; set; }

		[XmlIgnore]
		public bool ConnectionStatusSpecified { get; set; }

		internal void AddNotifications(List<EwsNotificationType> notifications)
		{
			if (notifications.Count > 0)
			{
				this.Notifications = notifications.ToArray();
				return;
			}
			this.Notifications = null;
		}

		internal void AddErrorSubscriptionIds(IEnumerable<string> ids)
		{
			if (ids != null)
			{
				this.ErrorSubscriptionIds = ids.ToArray<string>();
			}
		}

		internal void SetConnectionStatus(ConnectionStatus status)
		{
			this.ConnectionStatus = status;
			this.ConnectionStatusSpecified = true;
		}
	}
}
