using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AmBcsCheckDefinitions
	{
		internal const AmBcsChecks DatabaseNeverMountedChecks = AmBcsChecks.IsPassiveCopy;

		internal const AmBcsChecks Phase1Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;

		internal const AmBcsChecks Phase2Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;

		internal const AmBcsChecks Phase3Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;

		internal const AmBcsChecks Phase4Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;

		internal const AmBcsChecks Phase5Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit;

		internal const AmBcsChecks Phase6Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit;

		internal const AmBcsChecks Phase7Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.CopyQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit;

		internal const AmBcsChecks Phase8Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit;

		internal const AmBcsChecks Phase9Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit;

		internal const AmBcsChecks Phase10Checks = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.MaxActivesUnderHighestLimit;
	}
}
