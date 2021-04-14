using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionWorkItemEventResult : SubscriptionEventResult
	{
		public bool DeleteSubscription
		{
			get
			{
				return this.deleteSubscription;
			}
		}

		public void SetDeleteSubscription()
		{
			this.deleteSubscription = true;
		}

		private bool deleteSubscription;
	}
}
