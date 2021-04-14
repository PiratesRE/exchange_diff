using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class UnsubscribeToNotificationRequest
	{
		[DataMember(Name = "subscriptionData", IsRequired = true)]
		public SubscriptionData[] SubscriptionData { get; set; }
	}
}
