using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EnumeratorInNativeFolder : EnumeratorInFolder<StoreObjectId, UnifiedCustomSyncStateItem>
	{
		internal EnumeratorInNativeFolder(IEnumerator<UnifiedCustomSyncStateItem> underlyingEnumerator, StoreObjectId nativeFolderId) : base(underlyingEnumerator, nativeFolderId)
		{
		}

		protected override bool SkipCurrent(UnifiedCustomSyncStateItem item)
		{
			return item.NativeFolderId == null;
		}

		protected override StoreObjectId GetCurrent(UnifiedCustomSyncStateItem item)
		{
			return item.NativeId;
		}

		protected override StoreObjectId GetCurrentFolder(UnifiedCustomSyncStateItem item)
		{
			return item.NativeFolderId;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EnumeratorInNativeFolder>(this);
		}
	}
}
