using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.PushNotifications;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UnsubscribeToPushNotificationRequest : BaseSubscribeToPushNotificationRequest
	{
		public UnsubscribeToPushNotificationRequest()
		{
		}

		public UnsubscribeToPushNotificationRequest(PushNotificationSubscription subscription) : base(subscription)
		{
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UnsubscribeToPushNotification(callContext, this);
		}
	}
}
