using System;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Engine
{
	internal class SearchMemoryOperation
	{
		internal SearchMemoryOperation(SearchMemoryOperation searchMemoryOperation)
		{
			this.maxRestoreAmount = searchMemoryOperation.MaxRestoreAmount;
			this.memoryMeasureDrift = searchMemoryOperation.MemoryMeasureDrift;
			this.memoryUsageHighLine = searchMemoryOperation.MemoryUsageHighLine;
			this.memoryUsageLowLine = searchMemoryOperation.MemoryUsageLowLine;
			this.searchMemoryUsageBudgetHighLine = searchMemoryOperation.SearchMemoryUsageBudgetHighLine;
			this.searchMemoryUsageBudgetLowLine = searchMemoryOperation.SearchMemoryUsageBudgetLowLine;
		}

		internal SearchMemoryOperation(ISearchServiceConfig config, long totalPhys)
		{
			this.maxRestoreAmount = config.MaxRestoreAmount;
			this.memoryMeasureDrift = config.MemoryMeasureDrift;
			this.memoryUsageHighLine = totalPhys - totalPhys * (long)config.LowAvailableSystemWorkingSetMemoryRatio / 100L;
			this.memoryUsageLowLine = Math.Max(this.memoryUsageHighLine - config.MemoryMeasureDrift * 2L - this.maxRestoreAmount, 0L);
			this.searchMemoryUsageBudgetHighLine = config.SearchMemoryUsageBudget;
			this.searchMemoryUsageBudgetLowLine = config.SearchMemoryUsageBudget - config.SearchMemoryUsageBudgetFloatingAmount;
		}

		internal long MemoryUsageHighLine
		{
			get
			{
				return this.memoryUsageHighLine;
			}
			set
			{
				this.memoryUsageHighLine = value;
			}
		}

		internal long MemoryUsageLowLine
		{
			get
			{
				return this.memoryUsageLowLine;
			}
			set
			{
				this.memoryUsageLowLine = value;
			}
		}

		internal long SearchMemoryUsageBudgetHighLine
		{
			get
			{
				return this.searchMemoryUsageBudgetHighLine;
			}
			set
			{
				this.searchMemoryUsageBudgetHighLine = value;
			}
		}

		internal long SearchMemoryUsageBudgetLowLine
		{
			get
			{
				return this.searchMemoryUsageBudgetLowLine;
			}
			set
			{
				this.searchMemoryUsageBudgetLowLine = value;
			}
		}

		internal long MaxRestoreAmount
		{
			get
			{
				return this.maxRestoreAmount;
			}
			set
			{
				this.maxRestoreAmount = value;
			}
		}

		internal long MemoryMeasureDrift
		{
			get
			{
				return this.memoryMeasureDrift;
			}
			set
			{
				this.memoryMeasureDrift = value;
			}
		}

		internal bool IsTotalMemoryUsageHigh(long usedPhys)
		{
			return usedPhys > this.memoryUsageHighLine - this.memoryMeasureDrift;
		}

		internal bool IsTotalMemoryUsageLow(long usedPhys)
		{
			return usedPhys < this.memoryUsageLowLine + this.memoryMeasureDrift;
		}

		internal bool IsSearchMemoryUsageHigh(long searchUsage)
		{
			return searchUsage > this.searchMemoryUsageBudgetHighLine;
		}

		internal bool IsSearchMemoryUsageLow(long searchUsage)
		{
			return searchUsage < this.searchMemoryUsageBudgetLowLine;
		}

		private long searchMemoryUsageBudgetHighLine;

		private long searchMemoryUsageBudgetLowLine;

		private long memoryUsageHighLine;

		private long memoryUsageLowLine;

		private long maxRestoreAmount;

		private long memoryMeasureDrift;
	}
}
