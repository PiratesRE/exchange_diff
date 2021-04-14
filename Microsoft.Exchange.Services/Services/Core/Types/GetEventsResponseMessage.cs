using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetEventsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetEventsResponseMessage : ResponseMessage
	{
		public GetEventsResponseMessage()
		{
		}

		internal GetEventsResponseMessage(ServiceResultCode code, ServiceError error, EwsNotificationType notification) : base(code, error)
		{
			this.Notification = notification;
		}

		[DataMember(Name = "Notification", IsRequired = true)]
		[XmlElement("Notification")]
		public EwsNotificationType Notification { get; set; }
	}
}
