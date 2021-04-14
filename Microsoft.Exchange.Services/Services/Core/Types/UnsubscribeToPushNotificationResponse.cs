using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UnsubscribeToPushNotificationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UnsubscribeToPushNotificationResponse : ResponseMessage
	{
		public UnsubscribeToPushNotificationResponse()
		{
		}

		internal UnsubscribeToPushNotificationResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.UnsubscribeToPushNotificationResponseMessage;
		}
	}
}
