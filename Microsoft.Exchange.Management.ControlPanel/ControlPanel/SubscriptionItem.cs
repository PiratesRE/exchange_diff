using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SubscriptionItem : EnumValue
	{
		public SubscriptionItem(PimSubscriptionProxy subscription) : base(subscription.EmailAddress.ToString(), subscription.Subscription.SubscriptionIdentity.ToString())
		{
			this.IsValid = subscription.IsValid;
		}

		internal bool IsValid { get; set; }
	}
}
