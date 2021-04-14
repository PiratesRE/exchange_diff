using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Search.Engine
{
	internal class SearchMemoryModel
	{
		internal SearchMemoryModel(SearchMemoryModel searchMemoryModel)
		{
			this.config = searchMemoryModel.Config;
			this.diagnosticsSession = searchMemoryModel.DiagnosticsSession;
			this.totalPhys = searchMemoryModel.TotalPhys;
			this.availPhys = searchMemoryModel.AvailPhys;
			this.searchDesiredFreeMemory = searchMemoryModel.SearchDesiredFreeMemory;
			this.searchMemoryUsage = searchMemoryModel.SearchMemoryUsage;
			this.searchMemoryUsageDrift = searchMemoryModel.SearchMemoryUsageDrift;
			this.searchMemoryOperation = new SearchMemoryOperation(searchMemoryModel.searchMemoryOperation);
		}

		internal SearchMemoryModel(ISearchServiceConfig config, IDiagnosticsSession diagnosticsSession)
		{
			this.config = config;
			this.diagnosticsSession = diagnosticsSession;
			NativeMethods.MemoryStatusEx memoryStatusEx;
			if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				this.totalPhys = (long)memoryStatusEx.TotalPhys;
				this.availPhys = (long)memoryStatusEx.AvailPhys;
				this.searchDesiredFreeMemory = this.config.SearchWorkingSetMemoryUsageThreshold;
				this.searchMemoryOperation = new SearchMemoryOperation(config, this.totalPhys);
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failed to get the total physical memory with error {0}.", new object[]
			{
				lastWin32Error
			});
			throw new Win32Exception(lastWin32Error);
		}

		internal static float MemoryUsageAdjustmentMultiplier
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Search.MemoryModel.MemoryUsageAdjustmentMultiplier;
			}
		}

		internal IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		internal ISearchServiceConfig Config
		{
			get
			{
				return this.config;
			}
		}

		internal long TotalPhys
		{
			get
			{
				return this.totalPhys;
			}
		}

		internal long AvailPhys
		{
			get
			{
				return this.availPhys;
			}
		}

		internal long SearchMemoryUsage
		{
			get
			{
				return this.searchMemoryUsage;
			}
			set
			{
				this.searchMemoryUsage = value;
			}
		}

		internal long SearchDesiredFreeMemory
		{
			get
			{
				return this.searchDesiredFreeMemory;
			}
		}

		internal long SearchMemoryUsageDrift
		{
			get
			{
				return this.searchMemoryUsageDrift;
			}
			set
			{
				this.searchMemoryUsageDrift = value;
			}
		}

		internal SearchMemoryOperation SearchMemoryOperation
		{
			get
			{
				return this.searchMemoryOperation;
			}
		}

		internal static long GetSearchMemoryUsage()
		{
			if (DateTime.UtcNow > SearchMemoryModel.recentSearchMemoryUsageCacheTimeoutTime)
			{
				lock (SearchMemoryModel.recentSearchMemoryUsageCacheLock)
				{
					if (DateTime.UtcNow > SearchMemoryModel.recentSearchMemoryUsageCacheTimeoutTime)
					{
						SearchMemoryModel.recentSearchMemoryUsageCached = SearchMemoryModel.GetFreshSearchMemoryUsage();
						SearchMemoryModel.recentSearchMemoryUsageCacheTimeoutTime = DateTime.UtcNow.Add(SearchConfig.Instance.RecentSearchMemoryUsageCachedTimeout);
					}
				}
			}
			return SearchMemoryModel.recentSearchMemoryUsageCached;
		}

		internal static long GetFreshSearchMemoryUsage()
		{
			long num = 0L;
			foreach (string processName in SearchMemoryModel.SearchRelatedProcesses)
			{
				Process[] processesByName = Process.GetProcessesByName(processName);
				if (processesByName != null && processesByName.Length > 0)
				{
					foreach (Process process in processesByName)
					{
						num += process.WorkingSet64;
					}
				}
			}
			return num;
		}

		internal long GetExpectedSearchMemoryUsage(long activeItems, long passiveItems, long activeItemsInstantSearchOn, long activeItemsRefinersOn)
		{
			return (long)((float)(this.config.SearchMemoryModelBaseCost + this.config.BaselineCostPerActiveItem * activeItems + this.config.BaselineCostPerPassiveItem * passiveItems + this.config.InstantSearchCostPerActiveItem * activeItemsInstantSearchOn + this.config.RefinersCostPerActiveItem * activeItemsRefinersOn) * SearchMemoryModel.MemoryUsageAdjustmentMultiplier);
		}

		internal bool IsUnderSearchBudget()
		{
			return this.searchMemoryUsage <= this.searchDesiredFreeMemory;
		}

		internal bool IsBetter(long memoryChange)
		{
			long num = this.totalPhys - this.availPhys;
			this.diagnosticsSession.TraceDebug("availPhys: {0}, availableMemoryHighLine: {1}, availableMemoryLowLine: {2}, adjustedUsedPhys: {3}", new object[]
			{
				this.availPhys,
				this.searchMemoryOperation.MemoryUsageHighLine,
				this.searchMemoryOperation.MemoryUsageLowLine,
				num
			});
			if (this.searchMemoryOperation.IsTotalMemoryUsageHigh(num) && (!this.config.ShouldConsiderSearchMemoryUsageBudget || this.searchMemoryOperation.IsSearchMemoryUsageHigh(this.searchMemoryUsage)))
			{
				if (memoryChange < 0L && this.searchMemoryOperation.IsTotalMemoryUsageLow(num + memoryChange))
				{
					this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Memory change causes crossing the low line. memory change:{0}, memory change result:{1}, low line:{2}", new object[]
					{
						memoryChange,
						num + memoryChange,
						this.searchMemoryOperation.MemoryUsageLowLine + this.searchMemoryOperation.MemoryMeasureDrift
					});
				}
				return memoryChange < 0L;
			}
			return (this.searchMemoryOperation.IsTotalMemoryUsageLow(num) || (this.config.ShouldConsiderSearchMemoryUsageBudget && this.searchMemoryOperation.IsSearchMemoryUsageLow(this.searchMemoryUsage))) && memoryChange > 0L && !this.searchMemoryOperation.IsTotalMemoryUsageHigh(num + memoryChange);
		}

		internal void ApplyMemoryChange(long memoryChange, string mdbInfo)
		{
			this.diagnosticsSession.TraceDebug("Available memory- before: {0}, after: {1}; Search memory usage - before: {2}, after: {3}. Affected database - {4}.", new object[]
			{
				this.availPhys,
				this.availPhys - memoryChange,
				this.searchMemoryUsage,
				this.searchMemoryUsage + memoryChange,
				mdbInfo
			});
			this.availPhys -= memoryChange;
			this.searchMemoryUsage += memoryChange;
		}

		internal SearchMemoryModel.ActionDirection GetActionDirection()
		{
			long usedPhys = this.totalPhys - this.availPhys;
			if (this.searchMemoryOperation.IsTotalMemoryUsageHigh(usedPhys) && (!this.config.ShouldConsiderSearchMemoryUsageBudget || this.searchMemoryOperation.IsSearchMemoryUsageHigh(this.searchMemoryUsage)))
			{
				return SearchMemoryModel.ActionDirection.Degrade;
			}
			if (this.searchMemoryOperation.IsTotalMemoryUsageLow(usedPhys) || (this.config.ShouldConsiderSearchMemoryUsageBudget && this.searchMemoryOperation.IsSearchMemoryUsageLow(this.searchMemoryUsage)))
			{
				return SearchMemoryModel.ActionDirection.Restore;
			}
			return SearchMemoryModel.ActionDirection.None;
		}

		private static readonly object recentSearchMemoryUsageCacheLock = new object();

		private static readonly string[] SearchRelatedProcesses = new string[]
		{
			"noderunner",
			"hostcontrollerservice",
			"microsoft.exchange.search.service",
			"parserserver"
		};

		private static long recentSearchMemoryUsageCached;

		private static DateTime recentSearchMemoryUsageCacheTimeoutTime = DateTime.MinValue;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly ISearchServiceConfig config;

		private readonly long totalPhys;

		private readonly long searchDesiredFreeMemory;

		private long availPhys;

		private long searchMemoryUsage;

		private long searchMemoryUsageDrift;

		private SearchMemoryOperation searchMemoryOperation;

		internal enum ActionDirection
		{
			None,
			Degrade,
			Restore
		}
	}
}
