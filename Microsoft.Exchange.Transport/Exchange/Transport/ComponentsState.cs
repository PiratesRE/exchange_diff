using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum ComponentsState : ulong
	{
		None = 0UL,
		AllowInboundMailSubmissionFromHubs = 1UL,
		AllowInboundMailSubmissionFromInternet = 2UL,
		AllowInboundMailSubmissionFromPickupAndReplayDirectory = 4UL,
		AllowInboundMailSubmissionFromMailbox = 8UL,
		AllowOutboundMailDeliveryToRemoteDomains = 16UL,
		AllowBootScannerRunning = 64UL,
		AllowContentAggregation = 128UL,
		AllowMessageRepositoryResubmission = 256UL,
		AllowShadowRedundancyResubmission = 512UL,
		TransportServicePaused = 1024UL,
		AllowSmtpInComponentToRecvFromInternetAndHubs = 3UL,
		AllowAllComponents = 991UL,
		AllowIncomingTraffic = 143UL,
		HighResourcePressureState = 80UL,
		MediumResourcePressureState = 89UL
	}
}
