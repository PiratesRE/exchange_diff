using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class BudgetWrapper<T> : IBudget, IDisposable where T : Budget
	{
		internal BudgetWrapper(T innerBudget)
		{
			if (innerBudget == null)
			{
				throw new ArgumentNullException("innerBudget");
			}
			this.innerBudget = innerBudget;
			WorkloadManagementLogger.SetBudgetType(this.BudgetType.ToString(), null);
		}

		public BudgetKey Owner
		{
			get
			{
				return this.innerBudget.Owner;
			}
		}

		protected void HandleCostHandleRelease(CostHandle costHandle)
		{
			lock (this.instanceLock)
			{
				this.CalculateElapsedTime(costHandle);
				this.myActions.Remove(costHandle.Key);
				if (costHandle == this.localCostHandle)
				{
					this.localCostHandle = null;
				}
			}
		}

		public BudgetType BudgetType
		{
			get
			{
				return this.innerBudget.Owner.BudgetType;
			}
		}

		internal Dictionary<long, CostHandle> OutstandingActions
		{
			get
			{
				return this.myActions;
			}
		}

		internal T GetInnerBudget()
		{
			this.CheckExpired();
			return this.innerBudget;
		}

		protected void CheckExpired()
		{
			if (this.innerBudget.IsExpired)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, int>((long)this.GetHashCode(), "[BudgetWrapper.CheckExpired] Budget has expired for owner: {0}.  Outstanding actions ignored: {1}", this.innerBudget.Owner, this.innerBudget.OutstandingActionsCount);
				lock (this.instanceLock)
				{
					if (this.innerBudget.IsExpired)
					{
						this.innerBudget = this.ReacquireBudget();
						this.myActions.Clear();
						this.localCostHandle = null;
					}
				}
			}
		}

		protected abstract T ReacquireBudget();

		private void CloseAllActions()
		{
			this.CheckExpired();
			if (this.myActions.Count > 0)
			{
				Dictionary<long, CostHandle> dictionary = null;
				lock (this.instanceLock)
				{
					dictionary = this.myActions;
					this.myActions = new Dictionary<long, CostHandle>();
				}
				foreach (KeyValuePair<long, CostHandle> keyValuePair in dictionary)
				{
					keyValuePair.Value.Dispose();
				}
			}
		}

		public void Dispose()
		{
			string budgetBalance;
			if (this.TryGetBudgetBalance(out budgetBalance))
			{
				WorkloadManagementLogger.SetBudgetBalance(budgetBalance, null);
			}
			this.CloseAllActions();
			this.AfterDispose();
		}

		protected virtual void AfterDispose()
		{
		}

		public IThrottlingPolicy ThrottlingPolicy
		{
			get
			{
				T t = this.GetInnerBudget();
				return t.ThrottlingPolicy.FullPolicy;
			}
		}

		public bool TryGetBudgetBalance(out string budgetBalance)
		{
			budgetBalance = null;
			if (!(this.innerBudget.CasTokenBucket is UnthrottledTokenBucket))
			{
				budgetBalance = this.innerBudget.CasTokenBucket.GetBalance().ToString();
				return true;
			}
			return false;
		}

		protected void AddAction(CostHandle costHandle)
		{
			lock (this.instanceLock)
			{
				this.myActions.Add(costHandle.Key, costHandle);
			}
		}

		public void CheckOverBudget()
		{
			this.CheckOverBudget(Budget.AllCostTypes);
		}

		public void CheckOverBudget(ICollection<CostType> consideredCostTypes)
		{
			T t = this.GetInnerBudget();
			t.CheckOverBudget(consideredCostTypes);
		}

		public bool TryCheckOverBudget(out OverBudgetException exception)
		{
			return this.TryCheckOverBudget(Budget.AllCostTypes, out exception);
		}

		public bool TryCheckOverBudget(ICollection<CostType> consideredCostTypes, out OverBudgetException exception)
		{
			T t = this.GetInnerBudget();
			return t.TryCheckOverBudget(consideredCostTypes, out exception);
		}

		public void StartLocal(string callerInfo, TimeSpan preCharge = default(TimeSpan))
		{
			this.CheckExpired();
			lock (this.instanceLock)
			{
				this.StartLocalImpl(callerInfo, preCharge);
			}
		}

		protected virtual void StartLocalImpl(string callerInfo, TimeSpan preCharge)
		{
			if (this.localCostHandle != null)
			{
				throw new InvalidOperationException("[BudgetWrapper.StartLocal] Only one outstanding LocalTime cost handle can be active on a budget wrapper.");
			}
			this.localCostHandle = this.InternalStartLocal(callerInfo, preCharge);
			this.AddAction(this.localCostHandle);
		}

		public void EndLocal()
		{
			this.CheckExpired();
			lock (this.instanceLock)
			{
				if (this.localCostHandle != null)
				{
					LocalTimeCostHandle localTimeCostHandle = this.localCostHandle;
					this.localCostHandle = null;
					localTimeCostHandle.Dispose();
				}
			}
		}

		protected virtual LocalTimeCostHandle InternalStartLocal(string callerInfo, TimeSpan preCharge)
		{
			return this.innerBudget.StartLocal(new Action<CostHandle>(this.HandleCostHandleRelease), callerInfo, preCharge);
		}

		public LocalTimeCostHandle LocalCostHandle
		{
			get
			{
				return this.localCostHandle;
			}
			protected set
			{
				this.localCostHandle = value;
			}
		}

		public override string ToString()
		{
			T t = this.GetInnerBudget();
			return t.ToString();
		}

		public DelayInfo GetDelay()
		{
			return this.GetDelay(Budget.AllCostTypes);
		}

		public DelayInfo GetDelay(ICollection<CostType> consideredCostTypes)
		{
			DelayInfo faultInjectionDelay = this.GetFaultInjectionDelay();
			if (faultInjectionDelay != DelayInfo.NoDelay)
			{
				return faultInjectionDelay;
			}
			BudgetTypeSetting budgetTypeSetting = BudgetTypeSettings.Get(this.Owner.BudgetType);
			DelayInfo hardQuotaDelay = this.GetHardQuotaDelay(consideredCostTypes, budgetTypeSetting);
			if (hardQuotaDelay.Delay == budgetTypeSetting.MaxDelay)
			{
				return hardQuotaDelay;
			}
			DelayInfo microDelay = this.GetMicroDelay(consideredCostTypes, budgetTypeSetting);
			if (hardQuotaDelay.Delay >= microDelay.Delay)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[BudgetWrapper.GetDelay] UserQuota delay '{0}' was greater than micro delay '{1}'", hardQuotaDelay.Delay, microDelay.Delay);
				return hardQuotaDelay;
			}
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[BudgetWrapper.GetDelay] Micro delay '{0}' was greater than user quota delay '{1}'", microDelay.Delay, hardQuotaDelay.Delay);
			return microDelay;
		}

		private DelayInfo GetFaultInjectionDelay()
		{
			string text = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3645254973U, ref text);
			if (string.IsNullOrEmpty(text))
			{
				return DelayInfo.NoDelay;
			}
			bool flag = false;
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3645254973U, ref flag);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2571513149U, ref num);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)num);
			if (flag)
			{
				TimeSpan delay = timeSpan;
				T t = this.GetInnerBudget();
				return new UserQuotaDelayInfo(delay, t.CreateOverBudgetException(text, "faultInjection", num), flag);
			}
			return new DelayInfo(timeSpan, flag);
		}

		private DelayInfo GetHardQuotaDelay(ICollection<CostType> consideredCostTypes, BudgetTypeSetting budgetTypeSetting)
		{
			OverBudgetException ex = null;
			if (this.TryCheckOverBudget(consideredCostTypes, out ex))
			{
				int backoffTime = ex.BackoffTime;
				TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)backoffTime);
				TimeSpan delay = (timeSpan > budgetTypeSetting.MaxDelay) ? budgetTypeSetting.MaxDelay : timeSpan;
				return new UserQuotaDelayInfo(delay, ex, true);
			}
			return DelayInfo.NoDelay;
		}

		private DelayInfo GetMicroDelay(ICollection<CostType> consideredCostTypes, BudgetTypeSetting budgetTypeSetting)
		{
			if (this.microDelayWorthyWork == TimeSpan.Zero || !consideredCostTypes.Contains(CostType.CAS))
			{
				return DelayInfo.NoDelay;
			}
			float balance = this.innerBudget.CasTokenBucket.GetBalance();
			if (balance < 0f)
			{
				SingleComponentThrottlingPolicy throttlingPolicy = this.innerBudget.ThrottlingPolicy;
				int num = (int)this.microDelayWorthyWork.TotalMilliseconds;
				int num2 = num * (int)(3600000U / throttlingPolicy.RechargeRate.Value);
				float num3 = -balance / throttlingPolicy.RechargeRate.Value;
				TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)((float)num2 * num3));
				TimeSpan timeSpan2 = (BudgetWrapper<T>.MinimumMicroDelay > timeSpan) ? BudgetWrapper<T>.MinimumMicroDelay : timeSpan;
				TimeSpan timeSpan3 = timeSpan2;
				TimeSpan timeSpan4 = (budgetTypeSetting.MaxMicroDelayMultiplier == int.MaxValue) ? TimeSpan.MaxValue : TimeSpan.FromMilliseconds((double)(num * budgetTypeSetting.MaxMicroDelayMultiplier));
				if (timeSpan3 > timeSpan4)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[BudgetWrapper.GetDelay] Budget '{0}' calculated an overBudgetFactor of '{1}', but used registry cap of '{2}' instead.  Budget Snapshot: '{3}'", new object[]
					{
						this.Owner,
						num3,
						budgetTypeSetting.MaxMicroDelayMultiplier,
						this
					});
					timeSpan3 = timeSpan4;
				}
				if (timeSpan3 > budgetTypeSetting.MaxDelay)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[BudgetWrapper.GetDelay] Budget '{0}' calculated a cappedDelay of '{1}' which was higher than registry MaxDelay of '{2}'.  Using MaxDelay instead.", this.Owner, timeSpan3, budgetTypeSetting.MaxDelay);
					ThrottlingPerfCounterWrapper.IncrementBudgetsAtMaxDelay(this.Owner);
					timeSpan3 = budgetTypeSetting.MaxDelay;
				}
				ThrottlingPerfCounterWrapper.IncrementBudgetsMicroDelayed(this.Owner);
				DelayInfo.TraceMicroDelays(this, TimeSpan.FromMilliseconds((double)num), timeSpan3);
				return new DelayInfo(timeSpan3, false);
			}
			return DelayInfo.NoDelay;
		}

		private void CalculateElapsedTime(CostHandle costHandle)
		{
			LocalTimeCostHandle localTimeCostHandle = costHandle as LocalTimeCostHandle;
			if (localTimeCostHandle != null)
			{
				TimeSpan unaccountedForTime = localTimeCostHandle.UnaccountedForTime;
				if (unaccountedForTime > TimeSpan.Zero)
				{
					T t = this.GetInnerBudget();
					if (t.CasTokenBucket.GetBalance() < 0f)
					{
						this.microDelayWorthyWork += unaccountedForTime;
					}
					else
					{
						this.microDelayWorthyWork = TimeSpan.Zero;
					}
					this.allWork += unaccountedForTime;
					WorkloadManagementLogger.SetBudgetUsage(unaccountedForTime, null, null);
				}
			}
		}

		public TimeSpan ResourceWorkAccomplished
		{
			get
			{
				TimeSpan result;
				lock (this.instanceLock)
				{
					result = ((this.localCostHandle != null) ? (this.allWork + this.localCostHandle.UnaccountedForTime) : this.allWork);
				}
				return result;
			}
		}

		public void ResetWorkAccomplished()
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>((long)this.GetHashCode(), "[BudgetWrapper.ResetWorkAccomplished] Resetting work for budget '{0}'", this.Owner);
			lock (this.instanceLock)
			{
				this.microDelayWorthyWork = TimeSpan.Zero;
				this.allWork = TimeSpan.Zero;
				if (this.localCostHandle != null)
				{
					this.localCostHandle.UnaccountedStartTime = TimeProvider.UtcNow;
				}
			}
		}

		private TimeSpan MicroDelayWorkAccomplished
		{
			get
			{
				TimeSpan result;
				lock (this.instanceLock)
				{
					T t = this.GetInnerBudget();
					if (t.CasTokenBucket.GetBalance() < 0f)
					{
						result = TimeSpan.Zero;
					}
					else
					{
						result = ((this.localCostHandle != null) ? (this.microDelayWorthyWork + this.localCostHandle.UnaccountedForTime) : this.microDelayWorthyWork);
					}
				}
				return result;
			}
		}

		private const uint LidDelayInfoUserQuotaBackoff = 2571513149U;

		private const uint LidDelayInfoUserQuotaReason = 3645254973U;

		private const uint LidDelayInfoUserQuotaStrict = 3645254973U;

		private const int MsecInOneHour = 3600000;

		public static readonly TimeSpan MinimumMicroDelay = TimeSpan.FromMilliseconds(15.0);

		private T innerBudget;

		private Dictionary<long, CostHandle> myActions = new Dictionary<long, CostHandle>();

		private TimeSpan microDelayWorthyWork = TimeSpan.Zero;

		private TimeSpan allWork = TimeSpan.Zero;

		private volatile LocalTimeCostHandle localCostHandle;

		protected object instanceLock = new object();
	}
}
