using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Flags]
	internal enum AmBcsChecks
	{
		None = 0,
		IsHealthyOrDisconnected = 1,
		IsCatalogStatusHealthy = 2,
		CopyQueueLength = 4,
		ReplayQueueLength = 8,
		IsCatalogStatusCrawling = 16,
		IsPassiveCopy = 32,
		IsSeedingSource = 64,
		TotalQueueLengthMaxAllowed = 128,
		ManagedAvailabilityInitiatorBetterThanSource = 256,
		ManagedAvailabilityAllHealthy = 512,
		ManagedAvailabilityUptoNormalHealthy = 1024,
		ManagedAvailabilityAllBetterThanSource = 2048,
		ManagedAvailabilitySameAsSource = 4096,
		ActivationEnabled = 8192,
		MaxActivesUnderHighestLimit = 16384,
		MaxActivesUnderPreferredLimit = 32768
	}
}
