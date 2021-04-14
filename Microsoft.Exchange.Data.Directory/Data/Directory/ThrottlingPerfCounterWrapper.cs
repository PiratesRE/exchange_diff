using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ThrottlingPerfCounterWrapper
	{
		internal static ThrottlingPerfCounterWrapper.ForTestLogMassUserOverBudgetDelegate OnLogMassiveNumberOfUsersOverBudgetDelegate { get; set; }

		public static void Initialize(BudgetType budgetType)
		{
			ThrottlingPerfCounterWrapper.Initialize(budgetType, null, false);
		}

		public static void Initialize(BudgetType budgetType, int? massOverBudgetPercent)
		{
			ThrottlingPerfCounterWrapper.Initialize(budgetType, massOverBudgetPercent, false);
		}

		public static void Initialize(BudgetType budgetType, int? massOverBudgetPercent, bool allowReinitialize)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized && !allowReinitialize)
			{
				throw new InvalidOperationException(string.Format("ThrottlingPerformanceCounters were already initialized with budget type of '{0}'.", ThrottlingPerfCounterWrapper.budgetType));
			}
			if (massOverBudgetPercent != null && (massOverBudgetPercent.Value < 0 || massOverBudgetPercent.Value > 100))
			{
				throw new ArgumentOutOfRangeException("massOverBudgetPercent", massOverBudgetPercent.Value, "massOverBudgetPercent must be between 0 and 100 inclusive");
			}
			ThrottlingPerfCounterWrapper.budgetType = budgetType;
			ThrottlingPerfCounterWrapper.massiveNumberOfUsersOverBudgetPercent = ((massOverBudgetPercent != null) ? massOverBudgetPercent.Value : DefaultThrottlingAlertValues.MassUserOverBudgetPercent(budgetType));
			try
			{
				string instanceName = ThrottlingPerfCounterWrapper.GetInstanceName(budgetType.ToString());
				ThrottlingPerfCounterWrapper.throttlingPerfCounters = MSExchangeThrottling.GetInstance(instanceName);
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters = MSExchangeUserThrottling.GetInstance(instanceName);
				ThrottlingPerfCounterWrapper.PerfCountersInitialized = true;
			}
			catch (Exception ex)
			{
				ThrottlingPerfCounterWrapper.PerfCountersInitialized = false;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_InitializePerformanceCountersFailed, string.Empty, new object[]
				{
					ex.ToString()
				});
				ExTraceGlobals.ClientThrottlingTracer.TraceError<string, string>(0L, "[ThrottlingPerfCounterWrapper.Initialize] Perf counter initialization failed with exception type: {0}, Messsage: {1}", ex.GetType().FullName, ex.Message);
			}
			ThrottlingPerfCounterWrapper.budgetsMicroDelayed = new ExactTimeoutCache<BudgetKey, BudgetKey>(delegate(BudgetKey key, BudgetKey value, RemoveReason reason)
			{
				ThrottlingPerfCounterWrapper.UpdateBudgetsMicroDelayed();
			}, null, null, 1000000, false, CacheFullBehavior.ExpireExisting);
			ThrottlingPerfCounterWrapper.budgetsAtMaximumDelay = new ExactTimeoutCache<BudgetKey, BudgetKey>(delegate(BudgetKey key, BudgetKey value, RemoveReason reason)
			{
				ThrottlingPerfCounterWrapper.UpdateBudgetsAtMaxDelay();
			}, null, null, 1000000, false, CacheFullBehavior.ExpireExisting);
			ThrottlingPerfCounterWrapper.budgetsLockedOut = new ExactTimeoutCache<BudgetKey, BudgetKey>(delegate(BudgetKey key, BudgetKey value, RemoveReason reason)
			{
				ThrottlingPerfCounterWrapper.UpdateBudgetsLockedOut();
			}, null, null, 1000000, false, CacheFullBehavior.ExpireExisting);
			ThrottlingPerfCounterWrapper.budgetsOverBudget = new ExactTimeoutCache<BudgetKey, BudgetKey>(delegate(BudgetKey key, BudgetKey value, RemoveReason reason)
			{
				ThrottlingPerfCounterWrapper.UpdateOverBudget();
			}, null, null, 1000000, false, CacheFullBehavior.ExpireExisting);
			ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency = new HashSet<BudgetKey>();
		}

		internal static int MinUniqueBudgetsForMassOverBudgetAlert
		{
			get
			{
				return ThrottlingPerfCounterWrapper.minUniqueBudgetsForMassiveOverBudgetAlert;
			}
			set
			{
				ThrottlingPerfCounterWrapper.minUniqueBudgetsForMassiveOverBudgetAlert = value;
			}
		}

		public static void IncrementBudgetsLockedOut(BudgetKey key, TimeSpan lockoutTime)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				ThrottlingPerfCounterWrapper.budgetsLockedOut.TryInsertAbsolute(key, key, lockoutTime);
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersLockedOut.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsLockedOut.Count;
			}
		}

		public static void UpdateBudgetsLockedOut()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersLockedOut.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsLockedOut.Count;
			}
		}

		private static void SetNumberAndPercentCounters(ExactTimeoutCache<BudgetKey, BudgetKey> cache, ExPerformanceCounter numberCounter, ExPerformanceCounter percentCounter)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			int budgetCount = ThrottlingPerfCounterWrapper.GetBudgetCount();
			int count = cache.Count;
			numberCounter.RawValue = (long)count;
			int num = (budgetCount == 0) ? 0 : (100 * count / budgetCount);
			if (num > 100)
			{
				num = 100;
			}
			percentCounter.RawValue = (long)num;
		}

		private static string GetInstanceName(string budgetType)
		{
			string str = null;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				str = currentProcess.ProcessName;
				if (currentProcess.ProcessName.Equals("w3wp", StringComparison.OrdinalIgnoreCase))
				{
					using (ServerManager serverManager = new ServerManager())
					{
						foreach (WorkerProcess workerProcess in serverManager.WorkerProcesses)
						{
							if (workerProcess.ProcessId == currentProcess.Id)
							{
								str = workerProcess.AppPoolName;
								break;
							}
						}
					}
				}
			}
			return str + "_" + budgetType;
		}

		public static void IncrementBudgetsMicroDelayed(BudgetKey key)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.budgetsMicroDelayed.TryInsertAbsolute(key, key, ThrottlingPerfCounterWrapper.PerfCounterRefreshWindow);
			ThrottlingPerfCounterWrapper.UpdateBudgetsMicroDelayed();
		}

		private static void UpdateBudgetsMicroDelayed()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.SetNumberAndPercentCounters(ThrottlingPerfCounterWrapper.budgetsMicroDelayed, ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.NumberOfUsersMicroDelayed, ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.PercentageUsersMicroDelayed);
		}

		public static void IncrementBudgetsAtMaxDelay(BudgetKey key)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.budgetsAtMaximumDelay.TryInsertAbsolute(key, key, ThrottlingPerfCounterWrapper.PerfCounterRefreshWindow);
			ThrottlingPerfCounterWrapper.UpdateBudgetsAtMaxDelay();
		}

		private static void UpdateBudgetsAtMaxDelay()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.SetNumberAndPercentCounters(ThrottlingPerfCounterWrapper.budgetsAtMaximumDelay, ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.NumberOfUsersAtMaximumDelay, ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.PercentageUsersAtMaximumDelay);
		}

		public static void IncrementOverBudget(BudgetKey key, TimeSpan backoffTime)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			if (backoffTime == TimeSpan.Zero || backoffTime == TimeSpan.MaxValue)
			{
				backoffTime = ThrottlingPerfCounterWrapper.PerfCounterRefreshWindow;
			}
			ThrottlingPerfCounterWrapper.budgetsOverBudget.TryInsertAbsolute(key, key, backoffTime);
			ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UniqueBudgetsOverBudget.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsOverBudget.Count;
			ThrottlingPerfCounterWrapper.LogEventsIfNecessary();
		}

		private static void UpdateOverBudget()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UniqueBudgetsOverBudget.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsOverBudget.Count;
		}

		public static void IncrementBudgetCount()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.TotalUniqueBudgets.Increment();
			ThrottlingPerfCounterWrapper.UpdateBudgetsAtMaxDelay();
			ThrottlingPerfCounterWrapper.UpdateBudgetsLockedOut();
			ThrottlingPerfCounterWrapper.UpdateBudgetsMicroDelayed();
			ThrottlingPerfCounterWrapper.UpdateOverBudget();
		}

		public static void DecrementBudgetCount()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.TotalUniqueBudgets.Decrement();
			ThrottlingPerfCounterWrapper.UpdateBudgetsAtMaxDelay();
			ThrottlingPerfCounterWrapper.UpdateBudgetsLockedOut();
			ThrottlingPerfCounterWrapper.UpdateBudgetsMicroDelayed();
			ThrottlingPerfCounterWrapper.UpdateOverBudget();
		}

		public static void IncrementBudgetsAtMaxConcurrency(BudgetKey key)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency.Add(key);
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersAtMaxConcurrency.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency.Count;
			}
		}

		public static void DecrementBudgetsAtMaxConcurrency(BudgetKey key)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency.Remove(key);
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersAtMaxConcurrency.RawValue = (long)ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency.Count;
			}
		}

		public static void ClearCaches()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.TotalUniqueBudgets.RawValue = 0L;
				ThrottlingPerfCounterWrapper.budgetsMicroDelayed.Clear();
				ThrottlingPerfCounterWrapper.budgetsAtMaximumDelay.Clear();
				ThrottlingPerfCounterWrapper.budgetsLockedOut.Clear();
				ThrottlingPerfCounterWrapper.budgetsOverBudget.Clear();
				ThrottlingPerfCounterWrapper.budgetsAtMaxConcurrency.Clear();
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.PercentageUsersMicroDelayed.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.PercentageUsersAtMaximumDelay.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.NumberOfUsersMicroDelayed.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.NumberOfUsersAtMaximumDelay.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersLockedOut.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UniqueBudgetsOverBudget.RawValue = 0L;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.UsersAtMaxConcurrency.RawValue = 0L;
			}
		}

		private static void LogEventsIfNecessary()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			lock (ThrottlingPerfCounterWrapper.staticLock)
			{
				int budgetCount = ThrottlingPerfCounterWrapper.GetBudgetCount();
				num3 = ((budgetCount > 0) ? (100 * ThrottlingPerfCounterWrapper.budgetsOverBudget.Count / budgetCount) : 0);
			}
			if (num3 > ThrottlingPerfCounterWrapper.massiveNumberOfUsersOverBudgetPercent && ThrottlingPerfCounterWrapper.budgetsOverBudget.Count > ThrottlingPerfCounterWrapper.minUniqueBudgetsForMassiveOverBudgetAlert)
			{
				bool flag2 = false;
				lock (ThrottlingPerfCounterWrapper.staticLock)
				{
					if (ThrottlingPerfCounterWrapper.budgetsOverBudget.Count > ThrottlingPerfCounterWrapper.minUniqueBudgetsForMassiveOverBudgetAlert)
					{
						flag2 = true;
						num = ThrottlingPerfCounterWrapper.GetBudgetCount();
						num2 = ThrottlingPerfCounterWrapper.budgetsOverBudget.Count;
					}
				}
				if (flag2)
				{
					if (ThrottlingPerfCounterWrapper.OnLogMassiveNumberOfUsersOverBudgetDelegate != null)
					{
						ThrottlingPerfCounterWrapper.OnLogMassiveNumberOfUsersOverBudgetDelegate(num, num3);
					}
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_ExcessiveMassUserThrottling, ThrottlingPerfCounterWrapper.budgetType.ToString(), new object[]
					{
						num2,
						ThrottlingPerfCounterWrapper.budgetType,
						num,
						num3
					});
				}
			}
		}

		public static bool PerfCountersInitialized { get; private set; }

		private static int GetBudgetCount()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return 0;
			}
			return (int)ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.TotalUniqueBudgets.RawValue;
		}

		public static void SetDelayedThreads(long delayedThreads)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.DelayedThreads.RawValue = delayedThreads;
			}
		}

		public static void IncrementActivePowerShellRunspaces()
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.throttlingPerfCounters.ActivePowerShellRunspaces.Increment();
			}
		}

		public static void DecrementActivePowerShellRunspaces()
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.DecrementPerfCounter(ThrottlingPerfCounterWrapper.throttlingPerfCounters.ActivePowerShellRunspaces);
			}
		}

		public static void IncrementExchangeExecutingCmdlets()
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.throttlingPerfCounters.ExchangeExecutingCmdlets.Increment();
			}
		}

		public static void DecrementExchangeExecutingCmdlets()
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.DecrementPerfCounter(ThrottlingPerfCounterWrapper.throttlingPerfCounters.ExchangeExecutingCmdlets);
			}
		}

		private static void DecrementPerfCounter(ExPerformanceCounter counter)
		{
			if (counter.RawValue > 0L)
			{
				counter.Decrement();
			}
		}

		public static void UpdateAverageThreadSleepTime(long newValue)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.throttlingPerfCounters.AverageThreadSleepTime.RawValue = (long)ThrottlingPerfCounterWrapper.averageThreadSleepTime.Update((float)newValue);
			}
		}

		public static MSExchangeThrottlingInstance GetThrottlingCounterForTest()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return null;
			}
			return ThrottlingPerfCounterWrapper.throttlingPerfCounters;
		}

		public static MSExchangeUserThrottlingInstance GetUserThrottlingCounterForTest()
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return null;
			}
			return ThrottlingPerfCounterWrapper.userThrottlingPerfCounters;
		}

		public static void SetFiveMinuteBudgetUsage(int usage999, int usage99, int usage75, int averageUsage)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageFiveMinuteWindow_99_9.RawValue = (long)usage999;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageFiveMinuteWindow_99.RawValue = (long)usage99;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageFiveMinuteWindow_75.RawValue = (long)usage75;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.AverageBudgetUsageFiveMinuteWindow.RawValue = (long)averageUsage;
			}
		}

		public static void SetOneHourBudgetUsage(int usage999, int usage99, int usage75, int averageUsage)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageOneHourWindow_99_9.RawValue = (long)usage999;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageOneHourWindow_99.RawValue = (long)usage99;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.BudgetUsageOneHourWindow_75.RawValue = (long)usage75;
				ThrottlingPerfCounterWrapper.userThrottlingPerfCounters.AverageBudgetUsageOneHourWindow.RawValue = (long)averageUsage;
			}
		}

		public static ICachePerformanceCounters GetOrganizationThrottlingPolicyCacheCounters(long maxCacheSize)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return new CachePerformanceCounters(ThrottlingPerfCounterWrapper.throttlingPerfCounters.OrganizationThrottlingPolicyCacheHitCount, ThrottlingPerfCounterWrapper.throttlingPerfCounters.OrganizationThrottlingPolicyCacheMissCount, ThrottlingPerfCounterWrapper.throttlingPerfCounters.OrganizationThrottlingPolicyCacheLength, ThrottlingPerfCounterWrapper.throttlingPerfCounters.OrganizationThrottlingPolicyCacheLengthPercentage, maxCacheSize);
			}
			return null;
		}

		public static ICachePerformanceCounters GetThrottlingPolicyCacheCounters(long maxCacheSize)
		{
			if (ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return new CachePerformanceCounters(ThrottlingPerfCounterWrapper.throttlingPerfCounters.ThrottlingPolicyCacheHitCount, ThrottlingPerfCounterWrapper.throttlingPerfCounters.ThrottlingPolicyCacheMissCount, ThrottlingPerfCounterWrapper.throttlingPerfCounters.ThrottlingPolicyCacheLength, ThrottlingPerfCounterWrapper.throttlingPerfCounters.ThrottlingPolicyCacheLengthPercentage, maxCacheSize);
			}
			return null;
		}

		private static MSExchangeThrottlingInstance throttlingPerfCounters;

		private static MSExchangeUserThrottlingInstance userThrottlingPerfCounters;

		private static BudgetType budgetType;

		private static RunningAverageFloat averageThreadSleepTime = new RunningAverageFloat(10);

		private static RunningAverageFloat averageTaskWaitTime = new RunningAverageFloat(10);

		private static ExactTimeoutCache<BudgetKey, BudgetKey> budgetsOverBudget;

		private static ExactTimeoutCache<BudgetKey, BudgetKey> budgetsMicroDelayed;

		private static ExactTimeoutCache<BudgetKey, BudgetKey> budgetsLockedOut;

		private static ExactTimeoutCache<BudgetKey, BudgetKey> budgetsAtMaximumDelay;

		private static HashSet<BudgetKey> budgetsAtMaxConcurrency;

		private static object staticLock = new object();

		private static int massiveNumberOfUsersOverBudgetPercent;

		public static readonly TimeSpan PerfCounterRefreshWindow = TimeSpan.FromMinutes(1.0);

		private static int minUniqueBudgetsForMassiveOverBudgetAlert = 100;

		internal delegate void ForTestLogMassUserOverBudgetDelegate(int uniqueBudgetCount, int overBudgetPercent);
	}
}
