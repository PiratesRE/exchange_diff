using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OutlookServiceSubscriptionItemEnumerator : OutlookServiceSubscriptionItemEnumeratorBase
	{
		public OutlookServiceSubscriptionItemEnumerator(IFolder folder) : base(folder, null)
		{
		}

		public OutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId) : base(folder, appId)
		{
		}

		public OutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, Unlimited<uint> resultSize) : base(folder, appId, resultSize)
		{
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return OutlookServiceSubscriptionItemEnumeratorBase.RefreshTimeUTCDescSortBy;
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
