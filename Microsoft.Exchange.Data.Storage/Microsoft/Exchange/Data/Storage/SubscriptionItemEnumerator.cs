using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionItemEnumerator : SubscriptionItemEnumeratorBase
	{
		public SubscriptionItemEnumerator(IFolder folder) : base(folder)
		{
		}

		public SubscriptionItemEnumerator(IFolder folder, Unlimited<uint> resultSize) : base(folder, resultSize)
		{
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return SubscriptionItemEnumeratorBase.RefreshTimeUTCDescSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			return false;
		}

		protected override bool ShouldSkipItem(IStorePropertyBag item)
		{
			return false;
		}
	}
}
