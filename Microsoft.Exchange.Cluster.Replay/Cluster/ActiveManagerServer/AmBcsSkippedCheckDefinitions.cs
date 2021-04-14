using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AmBcsSkippedCheckDefinitions
	{
		internal static AmBcsChecks[] SkipClientExperienceChecks = new AmBcsChecks[]
		{
			AmBcsChecks.IsCatalogStatusHealthy,
			AmBcsChecks.IsCatalogStatusCrawling,
			AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource,
			AmBcsChecks.ManagedAvailabilityAllHealthy,
			AmBcsChecks.ManagedAvailabilityUptoNormalHealthy,
			AmBcsChecks.ManagedAvailabilityAllBetterThanSource,
			AmBcsChecks.ManagedAvailabilitySameAsSource
		};

		internal static AmBcsChecks[] SkipHealthChecks = new AmBcsChecks[]
		{
			AmBcsChecks.IsHealthyOrDisconnected
		};

		internal static AmBcsChecks[] SkipLagChecks = new AmBcsChecks[]
		{
			AmBcsChecks.CopyQueueLength,
			AmBcsChecks.ReplayQueueLength,
			AmBcsChecks.IsSeedingSource,
			AmBcsChecks.TotalQueueLengthMaxAllowed
		};

		internal static AmBcsChecks[] SkipMaximumActiveDatabasesChecks = new AmBcsChecks[]
		{
			AmBcsChecks.MaxActivesUnderPreferredLimit,
			AmBcsChecks.MaxActivesUnderHighestLimit
		};
	}
}
