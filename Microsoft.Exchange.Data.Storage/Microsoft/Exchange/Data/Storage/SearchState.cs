using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SearchState : uint
	{
		None = 0U,
		Running = 1U,
		Rebuild = 2U,
		Recursive = 4U,
		Foreground = 8U,
		UseCiForComplexQueries = 16384U,
		Static = 65536U,
		MaybeStatic = 131072U,
		ImpliedRestrictions = 262144U,
		StatisticsOnly = 524288U,
		FailNonContentIndexedSearch = 1048576U,
		Failed = 2097152U,
		EstimateCountOnly = 4194304U,
		CiTotally = 16777216U,
		CiWithTwirResidual = 33554432U,
		TwirMostly = 67108864U,
		TwirTotally = 134217728U,
		Error = 268435456U
	}
}
