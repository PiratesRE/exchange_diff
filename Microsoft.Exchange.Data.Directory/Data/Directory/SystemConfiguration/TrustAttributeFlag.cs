using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum TrustAttributeFlag
	{
		None = 0,
		NonTransitive = 1,
		UpLevelOnly = 2,
		QuarantinedDomain = 4,
		ForestTransitive = 8,
		CrossOrganization = 16,
		WithinForest = 32
	}
}
