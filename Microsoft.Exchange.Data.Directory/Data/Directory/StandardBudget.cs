using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class StandardBudget : Budget
	{
		public static IStandardBudget Acquire(BudgetKey budgetKey)
		{
			StandardBudget innerBudget = StandardBudgetCache.Singleton.Get(budgetKey);
			return new StandardBudgetWrapper(innerBudget);
		}

		public static IStandardBudget Acquire(SecurityIdentifier budgetSid, BudgetType budgetType, bool isServiceAccount, ADSessionSettings settings)
		{
			SidBudgetKey budgetKey = new SidBudgetKey(budgetSid, budgetType, isServiceAccount, settings);
			return StandardBudget.Acquire(budgetKey);
		}

		public static IStandardBudget Acquire(SecurityIdentifier budgetSid, BudgetType budgetType, ADSessionSettings settings)
		{
			return StandardBudget.Acquire(budgetSid, budgetType, false, settings);
		}

		public static IStandardBudget AcquireFallback(string identifier, BudgetType budgetType)
		{
			StringBudgetKey budgetKey = new StringBudgetKey(identifier, false, budgetType);
			return StandardBudget.Acquire(budgetKey);
		}

		public static IStandardBudget AcquireUnthrottledBudget(string identifier, BudgetType budgetType)
		{
			UnthrottledBudgetKey budgetKey = new UnthrottledBudgetKey(identifier, budgetType);
			return StandardBudget.Acquire(budgetKey);
		}

		internal StandardBudget(BudgetKey owner, IThrottlingPolicy policy) : base(owner, policy)
		{
		}

		public CostHandle StartConnection(Action<CostHandle> onRelease, string callerInfo)
		{
			CostHandle result;
			lock (base.SyncRoot)
			{
				int num = this.connections + 1;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3701878077U, ref num);
				bool flag2 = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2630233405U, ref flag2);
				if (num > this.maxConcurrency || flag2)
				{
					ThrottlingPerfCounterWrapper.IncrementBudgetsAtMaxConcurrency(base.Owner);
					throw base.CreateOverBudgetException("MaxConcurrency", flag2 ? "FaultInjection" : this.maxConcurrency.ToString(), 0);
				}
				this.connections++;
				result = new CostHandle(this, CostType.Connection, onRelease, callerInfo, default(TimeSpan));
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("Owner:{0},Conn:{1},MaxConn:{2},MaxBurst:{3},Balance:{4},Cutoff:{5},RechargeRate:{6},Policy:{7},IsServiceAccount:{8},LiveTime:{9}", new object[]
			{
				base.Owner,
				this.Connections,
				base.ThrottlingPolicy.MaxConcurrency,
				base.ThrottlingPolicy.MaxBurst,
				base.GetBalanceForTrace(),
				base.ThrottlingPolicy.CutoffBalance,
				base.ThrottlingPolicy.RechargeRate,
				base.ThrottlingPolicy.FullPolicy.GetShortIdentityString(),
				base.ThrottlingPolicy.FullPolicy.IsServiceAccount,
				TimeProvider.UtcNow - base.CreationTime
			});
		}

		protected override void AccountForCostHandle(CostHandle costHandle)
		{
			if (costHandle.CostType != CostType.Connection)
			{
				base.AccountForCostHandle(costHandle);
				return;
			}
			if (this.connections <= 0)
			{
				throw new InvalidOperationException("[StandardBudget.AccountForCostHandle] End for Connections was called, but there are no outstanding Connections.");
			}
			this.connections--;
			ThrottlingPerfCounterWrapper.DecrementBudgetsAtMaxConcurrency(base.Owner);
		}

		internal int Connections
		{
			get
			{
				return this.connections;
			}
		}

		protected override void UpdateCachedPolicyValues(bool resetBudgetValues)
		{
			base.UpdateCachedPolicyValues(resetBudgetValues);
			lock (base.SyncRoot)
			{
				this.maxConcurrency = (int)(base.ThrottlingPolicy.MaxConcurrency.IsUnlimited ? 2147483647U : base.ThrottlingPolicy.MaxConcurrency.Value);
				if (resetBudgetValues)
				{
					this.connections = 0;
					ThrottlingPerfCounterWrapper.DecrementBudgetsAtMaxConcurrency(base.Owner);
				}
			}
		}

		public const string MaxConcurrencyPart = "MaxConcurrency";

		private const string FormatString = "Owner:{0},Conn:{1},MaxConn:{2},MaxBurst:{3},Balance:{4},Cutoff:{5},RechargeRate:{6},Policy:{7},IsServiceAccount:{8},LiveTime:{9}";

		private const uint LidChangeConnectionValue = 3701878077U;

		private const uint LidChangeMaxConnExceeded = 2630233405U;

		private int connections;

		private int maxConcurrency;
	}
}
