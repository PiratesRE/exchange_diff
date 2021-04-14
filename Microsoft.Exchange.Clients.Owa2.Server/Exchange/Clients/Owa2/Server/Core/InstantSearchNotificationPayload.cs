using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class InstantSearchNotificationPayload : NotificationPayloadBase
	{
		public InstantSearchNotificationPayload(string subscriptionId, InstantSearchPayloadType instantSearchPayload)
		{
			base.SubscriptionId = subscriptionId;
			this.InstantSearchPayload = instantSearchPayload;
		}

		[DataMember]
		public InstantSearchPayloadType InstantSearchPayload { get; private set; }
	}
}
