using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class MSExchangeUserThrottlingInstance : PerformanceCounterInstance
	{
		internal MSExchangeUserThrottlingInstance(string instanceName, MSExchangeUserThrottlingInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange User Throttling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.UniqueBudgetsOverBudget = new ExPerformanceCounter(base.CategoryName, "Unique Budgets OverBudget", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UniqueBudgetsOverBudget);
				this.TotalUniqueBudgets = new ExPerformanceCounter(base.CategoryName, "Total Unique Budgets", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalUniqueBudgets);
				this.DelayedThreads = new ExPerformanceCounter(base.CategoryName, "Delayed Threads", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayedThreads);
				this.UsersAtMaxConcurrency = new ExPerformanceCounter(base.CategoryName, "Users At MaxConcurrency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersAtMaxConcurrency);
				this.UsersLockedOut = new ExPerformanceCounter(base.CategoryName, "Users Locked Out", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersLockedOut);
				this.PercentageUsersMicroDelayed = new ExPerformanceCounter(base.CategoryName, "Percentage Users Micro Delayed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageUsersMicroDelayed);
				this.PercentageUsersAtMaximumDelay = new ExPerformanceCounter(base.CategoryName, "Percentage Users At Maximum Delay", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageUsersAtMaximumDelay);
				this.NumberOfUsersAtMaximumDelay = new ExPerformanceCounter(base.CategoryName, "Number Of Users At Maximum Delay", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUsersAtMaximumDelay);
				this.NumberOfUsersMicroDelayed = new ExPerformanceCounter(base.CategoryName, "Number Of Users Micro Delayed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUsersMicroDelayed);
				this.BudgetUsageFiveMinuteWindow_99_9 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 99.9%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_99_9);
				this.BudgetUsageFiveMinuteWindow_99 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 99%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_99);
				this.BudgetUsageFiveMinuteWindow_75 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 75%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_75);
				this.AverageBudgetUsageFiveMinuteWindow = new ExPerformanceCounter(base.CategoryName, "Average Budget Usage Five Minute Window", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageBudgetUsageFiveMinuteWindow);
				this.BudgetUsageOneHourWindow_99_9 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 99.9%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_99_9);
				this.BudgetUsageOneHourWindow_99 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 99%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_99);
				this.BudgetUsageOneHourWindow_75 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 75%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_75);
				this.AverageBudgetUsageOneHourWindow = new ExPerformanceCounter(base.CategoryName, "Average Budget Usage One Hour Window", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageBudgetUsageOneHourWindow);
				long num = this.UniqueBudgetsOverBudget.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeUserThrottlingInstance(string instanceName) : base(instanceName, "MSExchange User Throttling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.UniqueBudgetsOverBudget = new ExPerformanceCounter(base.CategoryName, "Unique Budgets OverBudget", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UniqueBudgetsOverBudget);
				this.TotalUniqueBudgets = new ExPerformanceCounter(base.CategoryName, "Total Unique Budgets", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalUniqueBudgets);
				this.DelayedThreads = new ExPerformanceCounter(base.CategoryName, "Delayed Threads", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayedThreads);
				this.UsersAtMaxConcurrency = new ExPerformanceCounter(base.CategoryName, "Users At MaxConcurrency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersAtMaxConcurrency);
				this.UsersLockedOut = new ExPerformanceCounter(base.CategoryName, "Users Locked Out", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.UsersLockedOut);
				this.PercentageUsersMicroDelayed = new ExPerformanceCounter(base.CategoryName, "Percentage Users Micro Delayed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageUsersMicroDelayed);
				this.PercentageUsersAtMaximumDelay = new ExPerformanceCounter(base.CategoryName, "Percentage Users At Maximum Delay", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageUsersAtMaximumDelay);
				this.NumberOfUsersAtMaximumDelay = new ExPerformanceCounter(base.CategoryName, "Number Of Users At Maximum Delay", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUsersAtMaximumDelay);
				this.NumberOfUsersMicroDelayed = new ExPerformanceCounter(base.CategoryName, "Number Of Users Micro Delayed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfUsersMicroDelayed);
				this.BudgetUsageFiveMinuteWindow_99_9 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 99.9%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_99_9);
				this.BudgetUsageFiveMinuteWindow_99 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 99%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_99);
				this.BudgetUsageFiveMinuteWindow_75 = new ExPerformanceCounter(base.CategoryName, "Budget Usage Five Minute Window 75%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageFiveMinuteWindow_75);
				this.AverageBudgetUsageFiveMinuteWindow = new ExPerformanceCounter(base.CategoryName, "Average Budget Usage Five Minute Window", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageBudgetUsageFiveMinuteWindow);
				this.BudgetUsageOneHourWindow_99_9 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 99.9%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_99_9);
				this.BudgetUsageOneHourWindow_99 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 99%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_99);
				this.BudgetUsageOneHourWindow_75 = new ExPerformanceCounter(base.CategoryName, "Budget Usage One Hour Window 75%", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BudgetUsageOneHourWindow_75);
				this.AverageBudgetUsageOneHourWindow = new ExPerformanceCounter(base.CategoryName, "Average Budget Usage One Hour Window", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageBudgetUsageOneHourWindow);
				long num = this.UniqueBudgetsOverBudget.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter UniqueBudgetsOverBudget;

		public readonly ExPerformanceCounter TotalUniqueBudgets;

		public readonly ExPerformanceCounter DelayedThreads;

		public readonly ExPerformanceCounter UsersAtMaxConcurrency;

		public readonly ExPerformanceCounter UsersLockedOut;

		public readonly ExPerformanceCounter PercentageUsersMicroDelayed;

		public readonly ExPerformanceCounter PercentageUsersAtMaximumDelay;

		public readonly ExPerformanceCounter NumberOfUsersAtMaximumDelay;

		public readonly ExPerformanceCounter NumberOfUsersMicroDelayed;

		public readonly ExPerformanceCounter BudgetUsageFiveMinuteWindow_99_9;

		public readonly ExPerformanceCounter BudgetUsageFiveMinuteWindow_99;

		public readonly ExPerformanceCounter BudgetUsageFiveMinuteWindow_75;

		public readonly ExPerformanceCounter AverageBudgetUsageFiveMinuteWindow;

		public readonly ExPerformanceCounter BudgetUsageOneHourWindow_99_9;

		public readonly ExPerformanceCounter BudgetUsageOneHourWindow_99;

		public readonly ExPerformanceCounter BudgetUsageOneHourWindow_75;

		public readonly ExPerformanceCounter AverageBudgetUsageOneHourWindow;
	}
}
