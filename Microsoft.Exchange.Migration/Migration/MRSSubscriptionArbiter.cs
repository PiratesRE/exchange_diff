using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSSubscriptionArbiter : SubscriptionArbiterBase
	{
		private MRSSubscriptionArbiter()
		{
		}

		public static MRSSubscriptionArbiter Instance
		{
			get
			{
				return MRSSubscriptionArbiter.soleInstance;
			}
		}

		private static readonly MRSSubscriptionArbiter soleInstance = new MRSSubscriptionArbiter();
	}
}
