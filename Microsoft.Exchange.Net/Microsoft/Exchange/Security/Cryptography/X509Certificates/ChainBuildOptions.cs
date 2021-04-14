using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum ChainBuildOptions : uint
	{
		CacheEndCert = 1U,
		CacheOnlyUrlRetrieval = 4U,
		RevocationCheckEndCert = 268435456U,
		RevocationCheckChain = 536870912U,
		RevocationCheckChainExcludeRoot = 1073741824U,
		RevocationCheckCacheOnly = 2147483648U,
		RevocationAccumulativeTimeout = 134217728U,
		DisablePass1QualityFiltering = 64U,
		ReturnLowerQualityContexts = 128U,
		DisableAuthRootAutoUpdate = 256U,
		TimestampTime = 512U,
		DisableAia = 8192U
	}
}
