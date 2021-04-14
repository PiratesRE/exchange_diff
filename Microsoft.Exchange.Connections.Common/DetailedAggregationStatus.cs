using System;

namespace Microsoft.Exchange.Connections.Common
{
	public enum DetailedAggregationStatus
	{
		None,
		AuthenticationError,
		ConnectionError,
		CommunicationError,
		RemoteMailboxQuotaWarning,
		LabsMailboxQuotaWarning,
		MaxedOutSyncRelationshipsError,
		Corrupted,
		LeaveOnServerNotSupported,
		RemoteAccountDoesNotExist,
		RemoteServerIsSlow,
		TooManyFolders,
		Finalized,
		RemoteServerIsBackedOff,
		RemoteServerIsPoisonous,
		SyncStateSizeError,
		ConfigurationError,
		RemoveSubscription,
		ProviderException
	}
}
