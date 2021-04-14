using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OnSyncCompletedEventArgs : EventArgs
	{
		internal OnSyncCompletedEventArgs(SubscriptionCompletionData subscriptionCompletionData)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionCompletionData", subscriptionCompletionData);
			this.subscriptionCompletionData = subscriptionCompletionData;
		}

		internal SubscriptionCompletionData SubscriptionCompletionData
		{
			get
			{
				return this.subscriptionCompletionData;
			}
		}

		private readonly SubscriptionCompletionData subscriptionCompletionData;
	}
}
