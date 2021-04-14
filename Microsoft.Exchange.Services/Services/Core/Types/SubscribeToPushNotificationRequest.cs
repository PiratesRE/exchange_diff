using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.PushNotifications;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SubscribeToPushNotificationRequest : BaseSubscribeToPushNotificationRequest
	{
		public SubscribeToPushNotificationRequest()
		{
		}

		public SubscribeToPushNotificationRequest(PushNotificationSubscription subscription) : base(subscription)
		{
		}

		[DataMember(Name = "LastUnseenEmailCount", IsRequired = false)]
		public int LastUnseenEmailCount { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SubscribeToPushNotification(callContext, this);
		}
	}
}
