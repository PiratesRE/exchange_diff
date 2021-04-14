using System;

namespace Microsoft.Exchange.Services
{
	internal enum EwsMetadata
	{
		CorrelationGuid,
		PrimaryOrProxyServer,
		TaskType,
		RemoteBackendCount,
		LocalMailboxCount,
		RemoteMailboxCount,
		LocalIdCount,
		RemoteIdCount,
		ClientStatistics,
		ProxyAsUser,
		ActAsUser,
		FrontEndServer,
		VersionInfo,
		MailboxTypeCacheHitCount,
		MailboxTypeCacheMissCount,
		ParticipantResolveLatency
	}
}
