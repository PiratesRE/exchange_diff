using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SearchState : uint
	{
		None = 0U,
		Running = 1U,
		Rebuild = 2U,
		Recursive = 4U,
		Foreground = 16U,
		AccurateResults = 4096U,
		PotentiallyInaccurateResults = 8192U,
		Static = 65536U,
		InstantSearch = 131072U,
		StatisticsOnly = 524288U,
		CiOnly = 1048576U,
		FullTextIndexQueryFailed = 2097152U,
		Failed = 2097152U,
		EstimateCountOnly = 4194304U,
		CiTotally = 16777216U,
		CiWithTwirResidual = 33554432U,
		TwirMostly = 67108864U,
		TwirTotally = 134217728U,
		Error = 268435456U
	}
}
