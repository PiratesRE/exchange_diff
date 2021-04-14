using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Flags]
	internal enum ForwardSyncDivergenceProvisioningFlags
	{
		None = 0,
		Temporary = 1,
		IncrementalOnly = 2,
		LinkRelated = 4,
		IgnoredInHaltCondition = 8,
		TenantWideDivergence = 16,
		ValidationDivergence = 32,
		Retriable = 64
	}
}
