using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlRoot(ElementName = "SendNotification", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlType("SendNotificationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SendNotificationResponse : BaseInfoResponse
	{
		public SendNotificationResponse() : base(ResponseType.SendNotificationResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new SendNotificationResponseMessage(code, error, value as EwsNotificationType);
		}
	}
}
