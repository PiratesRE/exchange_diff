using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	[Flags]
	public enum DistributionGroupExpansionError
	{
		NoError = 0,
		ToGroupExpansionFailed = 1,
		CcGroupExpansionFailed = 2,
		BccGroupExpansionFailed = 4,
		ToGroupExpansionHitRecipientsLimit = 8,
		CcGroupExpansionHitRecipientsLimit = 16,
		BccGroupExpansionHitRecipientsLimit = 32,
		ToGroupExpansionHitDepthsLimit = 64,
		CcGroupExpansionHitDepthsLimit = 128,
		BccGroupExpansionHitDepthsLimit = 256
	}
}
