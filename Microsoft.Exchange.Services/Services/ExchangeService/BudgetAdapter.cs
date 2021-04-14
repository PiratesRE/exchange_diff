using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class BudgetAdapter : IEwsBudget, IStandardBudget, IBudget, IDisposable
	{
		public BudgetAdapter(IStandardBudget budget)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			this.InnerBudget = budget;
		}

		public IStandardBudget InnerBudget { get; private set; }

		public void StartLocal(string callerInfo, TimeSpan preCharge = default(TimeSpan))
		{
		}

		public void EndLocal()
		{
		}

		public LocalTimeCostHandle LocalCostHandle
		{
			get
			{
				return this.InnerBudget.LocalCostHandle;
			}
		}

		public DelayInfo GetDelay()
		{
			return this.InnerBudget.GetDelay();
		}

		public DelayInfo GetDelay(ICollection<CostType> consideredCostTypes)
		{
			return this.InnerBudget.GetDelay(consideredCostTypes);
		}

		public void CheckOverBudget()
		{
			this.InnerBudget.CheckOverBudget();
		}

		public void CheckOverBudget(ICollection<CostType> consideredCostTypes)
		{
			this.InnerBudget.CheckOverBudget(consideredCostTypes);
		}

		public bool TryCheckOverBudget(out OverBudgetException exception)
		{
			exception = null;
			return this.InnerBudget.TryCheckOverBudget(out exception);
		}

		public bool TryCheckOverBudget(ICollection<CostType> consideredCostTypes, out OverBudgetException exception)
		{
			exception = null;
			return this.InnerBudget.TryCheckOverBudget(consideredCostTypes, out exception);
		}

		public BudgetKey Owner
		{
			get
			{
				return this.InnerBudget.Owner;
			}
		}

		public IThrottlingPolicy ThrottlingPolicy
		{
			get
			{
				return this.InnerBudget.ThrottlingPolicy;
			}
		}

		public TimeSpan ResourceWorkAccomplished
		{
			get
			{
				return this.InnerBudget.ResourceWorkAccomplished;
			}
		}

		public void ResetWorkAccomplished()
		{
			this.InnerBudget.ResetWorkAccomplished();
		}

		public bool TryGetBudgetBalance(out string budgetBalance)
		{
			budgetBalance = null;
			return this.InnerBudget.TryGetBudgetBalance(out budgetBalance);
		}

		public CostHandle StartConnection(string callerInfo)
		{
			return this.InnerBudget.StartConnection(callerInfo);
		}

		public void EndConnection()
		{
			this.InnerBudget.EndConnection();
		}

		public void Dispose()
		{
			this.InnerBudget.Dispose();
		}

		public bool SleepIfNecessary()
		{
			return false;
		}

		public bool SleepIfNecessary(out int sleepTime, out float cpuPercent)
		{
			cpuPercent = 0f;
			sleepTime = 0;
			return false;
		}

		public void LogEndStateToIIS()
		{
		}

		public bool TryIncrementFoundObjectCount(uint foundCount, out int maxPossible)
		{
			maxPossible = (int)foundCount;
			return true;
		}

		public bool CanAllocateFoundObjects(uint foundCount, out int maxPossible)
		{
			maxPossible = (int)foundCount;
			return true;
		}

		public uint TotalRpcRequestCount
		{
			get
			{
				return 0U;
			}
		}

		public ulong TotalRpcRequestLatency
		{
			get
			{
				return 0UL;
			}
		}

		public uint TotalLdapRequestCount
		{
			get
			{
				return 0U;
			}
		}

		public long TotalLdapRequestLatency
		{
			get
			{
				return 0L;
			}
		}

		public void StartPerformanceContext()
		{
		}

		public void StopPerformanceContext()
		{
		}
	}
}
