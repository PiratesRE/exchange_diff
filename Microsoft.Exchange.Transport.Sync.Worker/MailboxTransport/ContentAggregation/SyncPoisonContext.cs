using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncPoisonContext
	{
		public SyncPoisonContext(Guid subscriptionId)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			this.subscriptionId = subscriptionId;
			this.hasSubscriptionContextOnly = true;
		}

		public SyncPoisonContext(Guid subscriptionId, SyncPoisonItem item)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfArgumentNull("item", item);
			this.subscriptionId = subscriptionId;
			this.item = item;
			this.hasSubscriptionContextOnly = false;
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		public SyncPoisonItem Item
		{
			get
			{
				return this.item;
			}
		}

		public bool HasSubscriptionContextOnly
		{
			get
			{
				return this.hasSubscriptionContextOnly;
			}
		}

		public override string ToString()
		{
			if (this.cachedToString == null)
			{
				this.cachedToString = string.Format(CultureInfo.InvariantCulture, "SubscriptionId: {0}, Item: {1}", new object[]
				{
					this.subscriptionId,
					this.item
				});
			}
			return this.cachedToString;
		}

		private const string FormatString = "SubscriptionId: {0}, Item: {1}";

		private readonly Guid subscriptionId;

		private readonly SyncPoisonItem item;

		private bool hasSubscriptionContextOnly;

		private string cachedToString;
	}
}
