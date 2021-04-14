using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission
{
	internal enum SubscriptionSubmissionPropTag : uint
	{
		InArgSubmissionType = 2684354562U,
		InArgUserLegacyDN = 2684420127U,
		InArgSubscriptionMessageID = 2684485890U,
		InArgSubscriptionGuid = 2684551240U,
		InArgRecoverySync = 2684616715U,
		InArgDatabaseGuid = 2684682312U,
		InArgUserMailboxGuid = 2684747848U,
		InArgMailboxServer = 2684813343U,
		InArgTenantGuid = 2684878920U,
		InArgAggregationType = 2684944386U,
		InArgInitialSync = 2685009931U,
		InArgSubscription = 2685075714U,
		InArgIsSyncNow = 2685141003U,
		InArgSyncWatermark = 2685206559U,
		InArgMailboxServerGuid = 2685272136U,
		InArgSyncPhase = 2685337602U,
		OutArgErrorCode = 2835349507U
	}
}
