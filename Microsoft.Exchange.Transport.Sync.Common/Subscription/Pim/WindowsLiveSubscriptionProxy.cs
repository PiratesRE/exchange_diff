using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[Serializable]
	public abstract class WindowsLiveSubscriptionProxy : PimSubscriptionProxy
	{
		internal WindowsLiveSubscriptionProxy(WindowsLiveServiceAggregationSubscription subscription) : base(subscription)
		{
		}

		public override ValidationError[] Validate()
		{
			ICollection<ValidationError> collection = PimSubscriptionValidator.Validate(this);
			ValidationError[] array = new ValidationError[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		public override void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public void SetLiveAccountPuid(string puid)
		{
			((WindowsLiveServiceAggregationSubscription)base.Subscription).Puid = puid;
		}
	}
}
