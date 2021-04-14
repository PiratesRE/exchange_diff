using System;
using Microsoft.Exchange.Transport.Pickup;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class PickupResourceLevelObserver : ResourceLevelObserver
	{
		public PickupResourceLevelObserver(PickupComponent pickupComponent) : base("Pickup", pickupComponent, null)
		{
		}

		internal const string ResourceObserverName = "Pickup";
	}
}
