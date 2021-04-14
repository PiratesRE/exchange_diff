using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionInformation
	{
		Guid DatabaseGuid { get; }

		Guid MailboxGuid { get; }

		Guid SubscriptionGuid { get; }

		Guid TenantGuid { get; }

		Guid ExternalDirectoryOrgId { get; }

		AggregationSubscriptionType SubscriptionType { get; }

		AggregationType AggregationType { get; }

		string IncomingServerName { get; }

		SyncPhase SyncPhase { get; }

		bool Disabled { get; }

		ExDateTime? LastSuccessfulDispatchTime { get; }

		ExDateTime? LastSyncCompletedTime { get; }

		string HubServerDispatched { get; }

		bool SupportsSerialization { get; }

		SerializedSubscription SerializedSubscription { get; }
	}
}
