using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SetSearchCriteriaFlags : uint
	{
		None = 0U,
		Stop = 1U,
		Restart = 2U,
		Recursive = 4U,
		Shallow = 8U,
		Foreground = 16U,
		Background = 32U,
		UseCIForComplexQueries = 16384U,
		ContentIndexed = 65536U,
		NonContentIndexed = 131072U,
		Static = 262144U,
		FailOnForeignEID = 8388608U,
		StatisticsOnly = 16777216U,
		FailNonContentIndexedSearch = 33554432U,
		EstimateCountOnly = 67108864U
	}
}
