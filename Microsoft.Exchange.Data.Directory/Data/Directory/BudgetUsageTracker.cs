using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class BudgetUsageTracker
	{
		public static PercentileUsage Get(BudgetKey key)
		{
			PercentileUsage percentileUsage = null;
			if (!BudgetUsageTracker.budgetUsage.TryGetValue(key, out percentileUsage) || percentileUsage == null)
			{
				lock (BudgetUsageTracker.staticLock)
				{
					if (!BudgetUsageTracker.budgetUsage.TryGetValue(key, out percentileUsage) || percentileUsage == null)
					{
						percentileUsage = new PercentileUsage();
						BudgetUsageTracker.budgetUsage[key] = percentileUsage;
					}
				}
			}
			return percentileUsage;
		}

		public static void ClearForTest()
		{
			lock (BudgetUsageTracker.staticLock)
			{
				BudgetUsageTracker.budgetUsage.Clear();
			}
		}

		private static void HandleTimer(object state)
		{
			bool isOneHour = false;
			BudgetUsageTracker.budgetUsageClearIndex++;
			if (BudgetUsageTracker.budgetUsageClearIndex % 12 == 0)
			{
				BudgetUsageTracker.budgetUsageClearIndex = 0;
				isOneHour = true;
			}
			BudgetUsageTracker.Update(isOneHour);
		}

		public static void Update(bool isOneHour)
		{
			if (!ThrottlingPerfCounterWrapper.PerfCountersInitialized)
			{
				return;
			}
			int[] array = null;
			int averageUsage = 0;
			int[] array2 = null;
			int averageUsage2 = 0;
			PercentileUsage[] array3 = null;
			if (BudgetUsageTracker.budgetUsage.Count > 0)
			{
				List<BudgetKey> list = null;
				lock (BudgetUsageTracker.staticLock)
				{
					if (BudgetUsageTracker.budgetUsage.Count > 0)
					{
						array3 = new PercentileUsage[BudgetUsageTracker.budgetUsage.Count];
						int num = 0;
						foreach (KeyValuePair<BudgetKey, PercentileUsage> keyValuePair in BudgetUsageTracker.budgetUsage)
						{
							array3[num++] = new PercentileUsage(keyValuePair.Value);
							if (keyValuePair.Value.FiveMinuteUsage == 0 && keyValuePair.Value.OneHourUsage == 0 && TimeProvider.UtcNow - keyValuePair.Value.CreationTime > BudgetUsageTracker.PeriodicLoggingInterval)
							{
								if (list == null)
								{
									list = new List<BudgetKey>();
								}
								list.Add(keyValuePair.Key);
							}
							keyValuePair.Value.Clear(isOneHour);
						}
						if (list != null)
						{
							foreach (BudgetKey key in list)
							{
								PercentileUsage percentileUsage;
								if (BudgetUsageTracker.budgetUsage.TryGetValue(key, out percentileUsage))
								{
									percentileUsage.Expired = true;
									BudgetUsageTracker.budgetUsage.Remove(key);
								}
							}
						}
					}
				}
			}
			if (array3 != null)
			{
				BudgetUsageTracker.GetPercentileUsage(array3, false, out array, out averageUsage);
				ThrottlingPerfCounterWrapper.SetFiveMinuteBudgetUsage(array[0], array[1], array[2], averageUsage);
				if (isOneHour)
				{
					BudgetUsageTracker.GetPercentileUsage(array3, true, out array2, out averageUsage2);
					ThrottlingPerfCounterWrapper.SetOneHourBudgetUsage(array2[0], array2[1], array2[2], averageUsage2);
					return;
				}
			}
			else
			{
				if (isOneHour)
				{
					ThrottlingPerfCounterWrapper.SetOneHourBudgetUsage(0, 0, 0, 0);
					return;
				}
				ThrottlingPerfCounterWrapper.SetFiveMinuteBudgetUsage(0, 0, 0, 0);
			}
		}

		private static void GetPercentileUsage(PercentileUsage[] usages, bool isOneHour, out int[] percentiles, out int averageUsage)
		{
			Array.Sort<PercentileUsage>(usages, isOneHour ? new Comparison<PercentileUsage>(PercentileUsage.OneHourComparer) : new Comparison<PercentileUsage>(PercentileUsage.FiveMinuteComparer));
			percentiles = new int[3];
			percentiles[0] = BudgetUsageTracker.GetUsageAtPercentage(isOneHour, 0.999f, usages);
			percentiles[1] = BudgetUsageTracker.GetUsageAtPercentage(isOneHour, 0.99f, usages);
			percentiles[2] = BudgetUsageTracker.GetUsageAtPercentage(isOneHour, 0.75f, usages);
			double num = 0.0;
			double num2 = 1.0 / (double)usages.Length;
			foreach (PercentileUsage percentileUsage in usages)
			{
				num += num2 * (double)(isOneHour ? percentileUsage.OneHourUsage : percentileUsage.FiveMinuteUsage);
			}
			averageUsage = (int)num;
		}

		private static int GetUsageAtPercentage(bool isOneHour, float factor, PercentileUsage[] usages)
		{
			int num = (int)Math.Round((double)(factor * (float)(usages.Length - 1)));
			if (!isOneHour)
			{
				return usages[num].FiveMinuteUsage;
			}
			return usages[num].OneHourUsage;
		}

		private const float Factor99_9 = 0.999f;

		private const float Factor99 = 0.99f;

		private const float Factor75 = 0.75f;

		private static readonly TimeSpan PeriodicLoggingInterval = TimeSpan.FromMinutes(5.0);

		private static Dictionary<BudgetKey, PercentileUsage> budgetUsage = new Dictionary<BudgetKey, PercentileUsage>();

		private static int budgetUsageClearIndex;

		private static object staticLock = new object();

		private static Timer timer = new Timer(new TimerCallback(BudgetUsageTracker.HandleTimer), null, BudgetUsageTracker.PeriodicLoggingInterval, BudgetUsageTracker.PeriodicLoggingInterval);
	}
}
