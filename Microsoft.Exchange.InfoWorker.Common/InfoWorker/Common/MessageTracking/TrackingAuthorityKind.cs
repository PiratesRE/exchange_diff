using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal enum TrackingAuthorityKind
	{
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = -1)]
		CurrentSite,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = 500)]
		RemoteSiteInCurrentOrg,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = 1000)]
		RemoteForest,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = 2000)]
		RemoteTrustedOrg,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = -1)]
		RemoteOrg,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = -1)]
		LegacyExchangeServer,
		[TrackingAuthorityKindInformation(ExpectedConnectionLatencyMSec = -1)]
		Undefined
	}
}
