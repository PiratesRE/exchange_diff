using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SubscribeToPushNotificationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SubscribeToPushNotificationResponse : ResponseMessage
	{
		public SubscribeToPushNotificationResponse()
		{
		}

		internal SubscribeToPushNotificationResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SubscribeToPushNotificationResponseMessage;
		}
	}
}
