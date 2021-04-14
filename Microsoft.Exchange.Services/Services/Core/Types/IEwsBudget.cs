using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IEwsBudget : IStandardBudget, IBudget, IDisposable
	{
		bool SleepIfNecessary();

		bool SleepIfNecessary(out int sleepTime, out float cpuPercent);

		void LogEndStateToIIS();

		bool TryIncrementFoundObjectCount(uint foundCount, out int maxPossible);

		bool CanAllocateFoundObjects(uint foundCount, out int maxPossible);

		uint TotalRpcRequestCount { get; }

		ulong TotalRpcRequestLatency { get; }

		uint TotalLdapRequestCount { get; }

		long TotalLdapRequestLatency { get; }

		void StartPerformanceContext();

		void StopPerformanceContext();
	}
}
