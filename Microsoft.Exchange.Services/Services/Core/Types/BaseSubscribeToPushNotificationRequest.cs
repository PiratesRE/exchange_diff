using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.PushNotifications;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class BaseSubscribeToPushNotificationRequest : BaseRequest
	{
		public BaseSubscribeToPushNotificationRequest()
		{
		}

		public BaseSubscribeToPushNotificationRequest(PushNotificationSubscription subscription)
		{
			this.SubscriptionRequest = subscription;
		}

		[DataMember(Name = "SubscriptionRequest", IsRequired = true)]
		public PushNotificationSubscription SubscriptionRequest { get; set; }

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
