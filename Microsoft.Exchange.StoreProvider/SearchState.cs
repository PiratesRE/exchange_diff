using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SearchState
	{
		None = 0,
		Running = 1,
		Rebuild = 2,
		Recursive = 4,
		Foreground = 8,
		UseCiForComplexQueries = 16384,
		Static = 65536,
		MaybeStatic = 131072,
		ImpliedRestrictions = 262144,
		StatisticsOnly = 524288,
		CiOnly = 1048576,
		Failed = 2097152,
		EstimateCountOnly = 4194304,
		CiTotally = 16777216,
		CiWithTwirResidual = 33554432,
		TwirMostly = 67108864,
		TwirTotally = 134217728,
		Error = 268435456
	}
}
