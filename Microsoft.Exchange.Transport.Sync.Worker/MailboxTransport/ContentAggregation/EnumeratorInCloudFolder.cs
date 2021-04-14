using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EnumeratorInCloudFolder : EnumeratorInFolder<string, UnifiedCustomSyncStateItem>
	{
		internal EnumeratorInCloudFolder(IEnumerator<UnifiedCustomSyncStateItem> underlyingEnumerator, string cloudFolderId) : base(underlyingEnumerator, cloudFolderId)
		{
		}

		protected override bool SkipCurrent(UnifiedCustomSyncStateItem item)
		{
			return item.CloudFolderId == null;
		}

		protected override string GetCurrent(UnifiedCustomSyncStateItem item)
		{
			return item.CloudId;
		}

		protected override string GetCurrentFolder(UnifiedCustomSyncStateItem item)
		{
			return item.CloudFolderId;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EnumeratorInCloudFolder>(this);
		}
	}
}
