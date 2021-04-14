using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FailedItemEnumeratorInCloudFolder : EnumeratorInFolder<string, KeyValuePair<string, string>>
	{
		internal FailedItemEnumeratorInCloudFolder(IEnumerator<KeyValuePair<string, string>> underlyingEnumerator, string cloudFolderId) : base(underlyingEnumerator, cloudFolderId)
		{
		}

		protected override bool SkipCurrent(KeyValuePair<string, string> item)
		{
			return item.Value == null;
		}

		protected override string GetCurrent(KeyValuePair<string, string> item)
		{
			return item.Key;
		}

		protected override string GetCurrentFolder(KeyValuePair<string, string> item)
		{
			return item.Value;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FailedItemEnumeratorInCloudFolder>(this);
		}
	}
}
