using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Flags]
	internal enum AmBcsSkipFlags
	{
		None = 0,
		LegacySkipAllChecks = 1,
		SkipClientExperienceChecks = 2,
		SkipHealthChecks = 4,
		SkipLagChecks = 8,
		SkipMaximumActiveDatabasesChecks = 16,
		SkipActiveCopyChecks = 32,
		SkipAll = 62
	}
}
