using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SearchCriteriaFlags
	{
		None = 0,
		Stop = 1,
		Restart = 2,
		Recursive = 4,
		Shallow = 8,
		Foreground = 16,
		Background = 32,
		UseCiForComplexQueries = 16384,
		ContentIndexed = 65536,
		NonContentIndexed = 131072,
		Static = 262144,
		ImpliedRestrictions = 524288,
		FailOnForeignEID = 8388608,
		StatisticsOnly = 16777216,
		ContentIndexedOnly = 33554432,
		EstimateCountOnly = 67108864
	}
}
