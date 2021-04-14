using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class PowerShellBudget : Budget
	{
		public static IPowerShellBudget Acquire(BudgetKey budgetKey)
		{
			if (budgetKey == null)
			{
				throw new ArgumentNullException("budgetKey");
			}
			PowerShellBudget.VerifyCorrectBudgetType(budgetKey.BudgetType);
			PowerShellBudget innerBudget = PowerShellBudgetCache.Singleton.Get(budgetKey);
			return new PowerShellBudgetWrapper(innerBudget);
		}

		public static IPowerShellBudget Acquire(SecurityIdentifier callerSid, BudgetType budgetType, ADSessionSettings settings)
		{
			PowerShellBudget.VerifyCorrectBudgetType(budgetType);
			PowerShellBudget innerBudget = PowerShellBudgetCache.Singleton.Get(new SidBudgetKey(callerSid, budgetType, false, settings));
			return new PowerShellBudgetWrapper(innerBudget);
		}

		public static IPowerShellBudget AcquireFallback(string identifier, BudgetType budgetType)
		{
			PowerShellBudget.VerifyCorrectBudgetType(budgetType);
			PowerShellBudget innerBudget = PowerShellBudgetCache.Singleton.Get(new StringBudgetKey(identifier, false, budgetType));
			return new PowerShellBudgetWrapper(innerBudget);
		}

		internal PowerShellBudget(BudgetKey owner, IThrottlingPolicy policy) : base(owner, policy)
		{
		}

		public CostHandle StartCmdlet(string cmdLetName, Action<CostHandle> onRelease)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, string>((long)this.GetHashCode(), "[PowerShellBudget.StartCmdlet] Start called for user {0}, cmdlet: {1}", base.Owner, cmdLetName);
			this.Update();
			if (this.IsCmdletPerTimePeriodEnabled())
			{
				Interlocked.Decrement(ref this.cmdletsRemaining);
			}
			if (this.IsExchangeCmdletPerTimePeriodEnabled())
			{
				Interlocked.Decrement(ref this.exchangeCmdletsRemaining);
			}
			if (this.IsDestructiveCmdletPerTimePeriodEnabled() && !string.IsNullOrEmpty(cmdLetName) && PowerShellBudget.DestructiveCmdlets.Contains(cmdLetName.ToLower()))
			{
				Interlocked.Decrement(ref this.destructiveCmdletsRemaining);
			}
			ThrottlingPerfCounterWrapper.IncrementExchangeExecutingCmdlets();
			return new CostHandle(this, CostType.CMDLET, onRelease, string.Format("PowerShellBudgetCache.StartCmdlet.{0}", cmdLetName), default(TimeSpan));
		}

		public CostHandle StartActiveRunspace(Action<CostHandle> onRelease)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>((long)this.GetHashCode(), "[PowerShellBudget.StartActiveRunspace] Start called for user {0}", base.Owner);
			this.Update();
			if (this.IsMaxRunspacesPerTimePeriodEnabled())
			{
				Interlocked.Decrement(ref this.runspacesRemaining);
			}
			Interlocked.Increment(ref this.activeRunspaces);
			if (this.TrackActiveRunspacePerfCounter)
			{
				ThrottlingPerfCounterWrapper.IncrementActivePowerShellRunspaces();
			}
			return new CostHandle(this, CostType.ActiveRunspace, onRelease, "PowerShellBudget.StartActiveRunspace", default(TimeSpan));
		}

		internal int TotalActiveRunspacesCount
		{
			get
			{
				return this.activeRunspaces;
			}
		}

		public void CorrectRunspacesLeak(int leakedValue)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[PowerShellBudget.CorrectRunspacesBudget] called for user {0}, budget type {1}. Original-ActiveRunspaces = {2}, leaked value = {3}.", new object[]
			{
				base.Owner,
				base.Owner.BudgetType,
				this.activeRunspaces,
				leakedValue
			});
			if (this.activeRunspaces < leakedValue)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[PowerShellBudget.CorrectRunspacesBudget] Leaked value is largerr than current activeRunspaces value, change leaked value to be activerunspaces value.");
				leakedValue = this.activeRunspaces;
			}
			if (leakedValue > 0)
			{
				Interlocked.Add(ref this.activeRunspaces, -1 * leakedValue);
			}
		}

		protected override void AccountForCostHandle(CostHandle costHandle)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, CostType, BudgetType>((long)this.GetHashCode(), "[PowerShellBudget.AccountForCostHandle] End called for user {0}, cost type {1}, budget type {2}", base.Owner, costHandle.CostType, base.Owner.BudgetType);
			base.AccountForCostHandle(costHandle);
			switch (costHandle.CostType)
			{
			case CostType.CMDLET:
				ThrottlingPerfCounterWrapper.DecrementExchangeExecutingCmdlets();
				return;
			case CostType.ActiveRunspace:
				Interlocked.Decrement(ref this.activeRunspaces);
				if (this.TrackActiveRunspacePerfCounter)
				{
					ThrottlingPerfCounterWrapper.DecrementActivePowerShellRunspaces();
				}
				return;
			default:
				return;
			}
		}

		private static void VerifyCorrectBudgetType(BudgetType budgetType)
		{
			if (budgetType != BudgetType.PowerShell && budgetType != BudgetType.WSMan && budgetType != BudgetType.WSManTenant)
			{
				throw new ArgumentException("PowerShell budgets can only be used with BudgetTypes PowerShell, WSMan and WSManTenant.  Passed Key: " + budgetType.ToString());
			}
		}

		private int GetBackoffTime(uint timePeriod, DateTime lastTime)
		{
			return (int)((timePeriod - (TimeProvider.UtcNow - lastTime).TotalSeconds) * 1000.0);
		}

		private void Update()
		{
			if (Interlocked.Exchange(ref this.updatingLastUpdate, 1) == 0)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>((long)this.GetHashCode(), "[PowerShellBudget.Update] Update called for user {0}.", base.Owner);
				try
				{
					DateTime utcNow = TimeProvider.UtcNow;
					TimeSpan timeSpan = utcNow - this.lastTimeFrame;
					TimeSpan timeSpan2 = utcNow - this.lastTimeFrameDestructiveCmdlets;
					TimeSpan timeSpan3 = utcNow - this.lastTimeFrameRunspaces;
					this.lastUpdate = utcNow;
					this.lastUpdateDestructiveCmdlets = utcNow;
					this.lastUpdateMaxRunspaces = utcNow;
					if (this.IsCmdletPerTimePeriodEnabled() && (this.firstTimeUpdate || timeSpan.TotalSeconds > this.powerShellMaxCmdletsTimePeriodPolicyValue.Value))
					{
						this.UpdateTimeFrameAndCmdletsRemaining(utcNow, (T)this.powerShellMaxCmdletsPolicyValue);
					}
					if (this.IsExchangeCmdletPerTimePeriodEnabled() && (this.firstTimeUpdate || timeSpan.TotalSeconds > this.powerShellMaxCmdletsTimePeriodPolicyValue.Value))
					{
						this.UpdateTimeFrameAndExchangeCmdletsRemaining(utcNow, (T)this.exchangeMaxCmdletsPolicyValue);
					}
					if (this.IsDestructiveCmdletPerTimePeriodEnabled() && (this.firstTimeUpdate || timeSpan2.TotalSeconds > this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue.Value))
					{
						this.UpdateTimeFrameAndDestructiveCmdletsRemaining(utcNow, (T)this.powerShellMaxDestructiveCmdletsPolicyValue);
					}
					if (this.IsMaxRunspacesPerTimePeriodEnabled() && (this.firstTimeUpdate || timeSpan3.TotalSeconds > this.powerShellMaxRunspacesTimePeriodPolicyValue.Value))
					{
						this.UpdateTimeFrameAndRunspacesRemaining(utcNow, (T)this.powerShellMaxRunspacesPolicyValue);
					}
				}
				finally
				{
					Interlocked.Exchange(ref this.updatingLastUpdate, 0);
					if (this.firstTimeUpdate)
					{
						this.firstTimeUpdate = false;
					}
				}
			}
		}

		public override string ToString()
		{
			string result;
			lock (base.SyncRoot)
			{
				TimeSpan timeSpan = (base.CasTokenBucket.LockedUntilUtc != null) ? (base.CasTokenBucket.LockedUntilUtc.Value - TimeProvider.UtcNow) : TimeSpan.Zero;
				result = string.Format("Owner:\t{0}\r\nBudgetType:\t{1}\r\nActiveRunspaces:\t{2}/{3}\r\nBalance:\t{4}/{5}/{6}\r\nPowerShellCmdletsLeft:\t{7}/{8}\r\nExchangeCmdletsLeft:\t{9}/{10}\r\nCmdletTimePeriod:\t{11}\r\nDestructiveCmdletsLeft:\t{12}/{13}\r\nDestructiveCmdletTimePeriod:\t{14}\r\nQueueDepth:\t{15}\r\nMaxRunspacesTimePeriod:\t{16}\r\nRunSpacesRemaining:\t{17}/{18}\r\nLastTimeFrameUpdate:\t{19}\r\nLastTimeFrameUpdateDestructiveCmdlets:\t{20}\r\nLastTimeFrameUpdateMaxRunspaces:\t{21}\r\nLocked:\t{22}\r\nLockRemaining:\t{23}\r\n", new object[]
				{
					base.Owner,
					base.Owner.BudgetType,
					this.activeRunspaces,
					this.activeRunspacesPolicyValue,
					base.GetBalanceForTrace(),
					base.CasTokenBucket.RechargeRate,
					base.CasTokenBucket.MinimumBalance,
					this.cmdletsRemaining,
					this.powerShellMaxCmdletsPolicyValue,
					this.exchangeCmdletsRemaining,
					this.exchangeMaxCmdletsPolicyValue,
					this.powerShellMaxCmdletsTimePeriodPolicyValue,
					this.destructiveCmdletsRemaining,
					this.powerShellMaxDestructiveCmdletsPolicyValue,
					this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue,
					this.powerShellMaxCmdletQueueDepthPolicyValue,
					this.powerShellMaxRunspacesTimePeriodPolicyValue,
					this.runspacesRemaining,
					this.powerShellMaxRunspacesPolicyValue,
					this.lastTimeFrame,
					this.lastTimeFrameDestructiveCmdlets,
					this.lastTimeFrameRunspaces,
					base.CasTokenBucket.Locked,
					timeSpan
				});
			}
			return result;
		}

		public string GetWSManBudgetUsage()
		{
			string result;
			lock (base.SyncRoot)
			{
				if (this.activeRunspaces == 2147483647 && this.runspacesRemaining == 9223372036854775807L && this.cmdletsRemaining == 9223372036854775807L)
				{
					result = null;
				}
				else
				{
					result = string.Format("Concurrency:{0}/{1} RunSpaces:{2}/{3}/{4} PowerShellCmdlet:{5}/{6}/{7}", new object[]
					{
						this.activeRunspaces,
						this.activeRunspacesPolicyValue,
						PowerShellBudget.GetUserFriendlyLongValue(this.runspacesRemaining),
						this.powerShellMaxRunspacesPolicyValue,
						this.powerShellMaxRunspacesTimePeriodPolicyValue,
						PowerShellBudget.GetUserFriendlyLongValue(this.cmdletsRemaining),
						this.powerShellMaxCmdletsPolicyValue,
						this.powerShellMaxCmdletsTimePeriodPolicyValue
					});
				}
			}
			return result;
		}

		public string GetCmdletBudgetUsage()
		{
			string result;
			lock (base.SyncRoot)
			{
				if (this.cmdletsRemaining == 9223372036854775807L && this.destructiveCmdletsRemaining == 9223372036854775807L && base.CasTokenBucket is UnthrottledTokenBucket)
				{
					result = null;
				}
				else
				{
					result = string.Format("ExchangeCmdlet:{0}/{1}/{2} DestructiveCmdlet:{3}/{4}/{5} Balance:{6}/{7}/{8}", new object[]
					{
						PowerShellBudget.GetUserFriendlyLongValue(this.exchangeCmdletsRemaining),
						this.exchangeMaxCmdletsPolicyValue,
						this.powerShellMaxCmdletsTimePeriodPolicyValue,
						PowerShellBudget.GetUserFriendlyLongValue(this.destructiveCmdletsRemaining),
						this.powerShellMaxDestructiveCmdletsPolicyValue,
						this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue,
						base.GetBalanceForTrace(),
						base.CasTokenBucket.RechargeRate,
						base.CasTokenBucket.MinimumBalance
					});
				}
			}
			return result;
		}

		private bool IsMaxConcurrencyEnabled()
		{
			return !this.activeRunspacesPolicyValue.IsUnlimited;
		}

		private bool IsCmdletPerTimePeriodEnabled()
		{
			return !this.powerShellMaxCmdletsPolicyValue.IsUnlimited && !this.powerShellMaxCmdletsTimePeriodPolicyValue.IsUnlimited;
		}

		private bool IsExchangeCmdletPerTimePeriodEnabled()
		{
			return !this.exchangeMaxCmdletsPolicyValue.IsUnlimited && !this.powerShellMaxCmdletsTimePeriodPolicyValue.IsUnlimited;
		}

		private bool IsDestructiveCmdletPerTimePeriodEnabled()
		{
			return !this.powerShellMaxDestructiveCmdletsPolicyValue.IsUnlimited && !this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue.IsUnlimited;
		}

		private bool IsMaxRunspacesPerTimePeriodEnabled()
		{
			return !this.powerShellMaxRunspacesPolicyValue.IsUnlimited && !this.powerShellMaxRunspacesTimePeriodPolicyValue.IsUnlimited;
		}

		private void UpdateTimeFrameAndExchangeCmdletsRemaining(DateTime newTimeFrame, uint maxCmdlets)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, DateTime, uint>((long)this.GetHashCode(), "[PowerShellBudget.UpdateTimeFrameAndExchangeCmdletsRemaining] User {0}, newTimeFrame {1}, maxCmdlets {2}.", base.Owner, newTimeFrame, maxCmdlets);
			lock (base.SyncRoot)
			{
				this.lastTimeFrame = newTimeFrame;
				Interlocked.Exchange(ref this.exchangeCmdletsRemaining, (long)((ulong)maxCmdlets));
			}
		}

		private void UpdateTimeFrameAndCmdletsRemaining(DateTime newTimeFrame, uint maxCmdlets)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, DateTime, uint>((long)this.GetHashCode(), "[PowerShellBudget.UpdateTimeFrameAndCmdletsRemaining] User {0}, newTimeFrame {1}, maxCmdlets {2}.", base.Owner, newTimeFrame, maxCmdlets);
			lock (base.SyncRoot)
			{
				this.lastTimeFrame = newTimeFrame;
				Interlocked.Exchange(ref this.cmdletsRemaining, (long)((ulong)maxCmdlets));
			}
		}

		private void UpdateTimeFrameAndDestructiveCmdletsRemaining(DateTime newTimeFrame, uint maxDestructiveCmdlets)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, DateTime, uint>((long)this.GetHashCode(), "[PowerShellBudget.UpdateTimeFrameAndDestructiveCmdletsRemaining] User {0}, newTimeFrame {1}, maxCmdlets {2}.", base.Owner, newTimeFrame, maxDestructiveCmdlets);
			lock (base.SyncRoot)
			{
				this.lastTimeFrameDestructiveCmdlets = newTimeFrame;
				Interlocked.Exchange(ref this.destructiveCmdletsRemaining, (long)((ulong)maxDestructiveCmdlets));
			}
		}

		private void UpdateTimeFrameAndRunspacesRemaining(DateTime newTimeFrame, uint maxRunspaces)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, DateTime, uint>((long)this.GetHashCode(), "[PowerShellBudget.UpdateTimeFrameAndDestructiveCmdletsRemaining] User {0}, newTimeFrame {1}, maxCmdlets {2}.", base.Owner, newTimeFrame, maxRunspaces);
			lock (base.SyncRoot)
			{
				this.lastTimeFrameRunspaces = newTimeFrame;
				Interlocked.Exchange(ref this.runspacesRemaining, (long)((ulong)maxRunspaces));
			}
		}

		protected override bool InternalCanExpire
		{
			get
			{
				lock (base.SyncRoot)
				{
					this.Update();
					if (this.IsCmdletPerTimePeriodEnabled() && this.cmdletsRemaining < (long)((ulong)this.powerShellMaxCmdletsPolicyValue.Value))
					{
						return false;
					}
					if (this.IsExchangeCmdletPerTimePeriodEnabled() && this.exchangeCmdletsRemaining < (long)((ulong)this.exchangeMaxCmdletsPolicyValue.Value))
					{
						return false;
					}
					if (this.IsDestructiveCmdletPerTimePeriodEnabled() && this.destructiveCmdletsRemaining < (long)((ulong)this.powerShellMaxDestructiveCmdletsPolicyValue.Value))
					{
						return false;
					}
				}
				return true;
			}
		}

		internal bool TryCheckOverBudget(CostType costType, out OverBudgetException exception)
		{
			exception = null;
			this.Update();
			if (base.CasTokenBucket.Locked)
			{
				int num = (int)(base.CasTokenBucket.LockedUntilUtc.Value - TimeProvider.UtcNow).TotalMilliseconds;
				if (num > 0)
				{
					exception = base.CreateOverBudgetException("LocalTime", base.ThrottlingPolicy.CutoffBalance.ToString(), num);
					return true;
				}
			}
			switch (costType)
			{
			case CostType.CMDLET:
			{
				int num2 = 0;
				string text = null;
				string policyValue = null;
				if (this.IsDestructiveCmdletPerTimePeriodEnabled() && this.destructiveCmdletsRemaining < 0L)
				{
					int backoffTime = this.GetBackoffTime(this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue.Value, this.lastTimeFrameDestructiveCmdlets);
					if (backoffTime > num2)
					{
						num2 = backoffTime;
						text = "MaxDestructiveCmdletsTimePeriod";
						policyValue = string.Format("{0}/{1}s", this.powerShellMaxDestructiveCmdletsPolicyValue, this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue);
					}
				}
				bool flag = this.IsCmdletPerTimePeriodEnabled() && this.cmdletsRemaining < 0L;
				bool flag2 = this.IsExchangeCmdletPerTimePeriodEnabled() && this.exchangeCmdletsRemaining < 0L;
				if (flag || flag2)
				{
					int backoffTime = this.GetBackoffTime(this.powerShellMaxCmdletsTimePeriodPolicyValue.Value, this.lastTimeFrame);
					if (backoffTime > num2)
					{
						num2 = backoffTime;
						text = (flag ? "PowerShellMaxCmdlets" : "ExchangeMaxCmdlets");
						policyValue = string.Format("{0}/{1}s", flag ? this.powerShellMaxCmdletsPolicyValue : this.exchangeMaxCmdletsPolicyValue, this.powerShellMaxCmdletsTimePeriodPolicyValue);
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					exception = base.CreateOverBudgetException(text, policyValue, num2);
				}
				return exception != null;
			}
			case CostType.ActiveRunspace:
				if (this.IsMaxConcurrencyEnabled() && (long)this.activeRunspaces >= (long)((ulong)this.activeRunspacesPolicyValue.Value))
				{
					exception = base.CreateOverBudgetException(this.MaxConcurrencyOverBudgetReason, this.activeRunspacesPolicyValue.Value.ToString(), int.MaxValue);
				}
				else if (this.IsMaxRunspacesPerTimePeriodEnabled() && this.runspacesRemaining <= 0L)
				{
					int backoffTime2 = this.GetBackoffTime(this.powerShellMaxRunspacesTimePeriodPolicyValue.Value, this.lastTimeFrameRunspaces);
					exception = base.CreateOverBudgetException(this.MaxRunspacesTimePeriodOverBudgetReason, string.Format("{0}/{1}s", this.powerShellMaxRunspacesPolicyValue, this.powerShellMaxRunspacesTimePeriodPolicyValue), backoffTime2);
				}
				return exception != null;
			default:
				throw new ArgumentException("[PowerShellBudget.TryCheckOverBudget] Unsupported CostType: " + costType.ToString(), "costType");
			}
		}

		protected override bool InternalTryCheckOverBudget(ICollection<CostType> consideredCostTypes, out OverBudgetException exception)
		{
			this.Update();
			OverBudgetException ex = null;
			OverBudgetException ex2 = null;
			OverBudgetException ex3 = null;
			base.InternalTryCheckOverBudget(consideredCostTypes, out ex);
			if (consideredCostTypes.Contains(CostType.ActiveRunspace))
			{
				this.TryCheckOverBudget(CostType.ActiveRunspace, out ex2);
			}
			if (consideredCostTypes.Contains(CostType.CMDLET))
			{
				this.TryCheckOverBudget(CostType.CMDLET, out ex3);
			}
			int num = 0;
			OverBudgetException ex4 = null;
			if (ex != null && ex.BackoffTime > num)
			{
				ex4 = ex;
				num = ex.BackoffTime;
			}
			if (ex2 != null && (ex2.BackoffTime > num || ex2.BackoffTime == -1))
			{
				ex4 = ex2;
				num = ex2.BackoffTime;
			}
			if (ex3 != null && (ex3.BackoffTime > num || ex3.BackoffTime == -1))
			{
				ex4 = ex3;
				num = ex3.BackoffTime;
			}
			exception = ex4;
			return exception != null;
		}

		protected override void UpdateCachedPolicyValues(bool resetBudgetValues)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, bool, PowerShellBudget>((long)this.GetHashCode(), "[PowerShellBudget.UpdateCachedPolicyValues] Start called for user {0}, resetBudgetValues {1}, Before Budget State {2}.", base.Owner, resetBudgetValues, this);
			lock (base.SyncRoot)
			{
				this.UpdatePolicyValueTakingEffectInThisBudget(base.ThrottlingPolicy);
				if (this.IsCmdletPerTimePeriodEnabled())
				{
					this.UpdateTimeFrameAndCmdletsRemaining(this.lastUpdate, (T)this.powerShellMaxCmdletsPolicyValue);
				}
				if (this.IsExchangeCmdletPerTimePeriodEnabled())
				{
					this.UpdateTimeFrameAndExchangeCmdletsRemaining(this.lastUpdate, (T)this.exchangeMaxCmdletsPolicyValue);
				}
				if (this.IsDestructiveCmdletPerTimePeriodEnabled())
				{
					this.UpdateTimeFrameAndDestructiveCmdletsRemaining(this.lastUpdateDestructiveCmdlets, (T)this.powerShellMaxDestructiveCmdletsPolicyValue);
				}
				if (this.IsMaxRunspacesPerTimePeriodEnabled())
				{
					this.UpdateTimeFrameAndRunspacesRemaining(this.lastUpdateMaxRunspaces, (T)this.powerShellMaxRunspacesTimePeriodPolicyValue);
				}
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, bool, PowerShellBudget>((long)this.GetHashCode(), "[PowerShellBudget.UpdateCachedPolicyValues] End called for user {0}, resetBudgetValues {1}, After Budget State {2}.", base.Owner, resetBudgetValues, this);
			}
		}

		protected virtual void UpdatePolicyValueTakingEffectInThisBudget(SingleComponentThrottlingPolicy policy)
		{
			bool flag = this.IsRwsApplication();
			if (flag)
			{
				this.exchangeMaxCmdletsPolicyValue = (policy.ExchangeMaxCmdlets.IsUnlimited ? Unlimited<uint>.UnlimitedValue : Math.Max(policy.ExchangeMaxCmdlets.Value, policy.ExchangeMaxCmdlets.Value * 2U));
				this.powerShellMaxCmdletQueueDepthPolicyValue = policy.PowerShellMaxCmdletQueueDepth;
				this.powerShellMaxCmdletsPolicyValue = (policy.PowerShellMaxCmdlets.IsUnlimited ? Unlimited<uint>.UnlimitedValue : Math.Max(policy.PowerShellMaxCmdlets.Value, policy.PowerShellMaxCmdlets.Value * 2U));
				this.powerShellMaxCmdletsTimePeriodPolicyValue = policy.PowerShellMaxCmdletsTimePeriod;
				this.powerShellMaxDestructiveCmdletsPolicyValue = policy.PowerShellMaxDestructiveCmdlets;
				this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue = policy.PowerShellMaxDestructiveCmdletsTimePeriod;
				this.activeRunspacesPolicyValue = (policy.MaxConcurrency.IsUnlimited ? Unlimited<uint>.UnlimitedValue : Math.Max(policy.MaxConcurrency.Value, policy.MaxConcurrency.Value * 3U));
				this.powerShellMaxRunspacesPolicyValue = (policy.PowerShellMaxRunspaces.IsUnlimited ? Unlimited<uint>.UnlimitedValue : Math.Max(policy.PowerShellMaxRunspaces.Value, policy.PowerShellMaxRunspaces.Value * 3U));
				this.powerShellMaxRunspacesTimePeriodPolicyValue = policy.PowerShellMaxRunspacesTimePeriod;
				return;
			}
			this.exchangeMaxCmdletsPolicyValue = policy.ExchangeMaxCmdlets;
			this.powerShellMaxCmdletQueueDepthPolicyValue = policy.PowerShellMaxCmdletQueueDepth;
			this.powerShellMaxCmdletsPolicyValue = policy.PowerShellMaxCmdlets;
			this.powerShellMaxCmdletsTimePeriodPolicyValue = policy.PowerShellMaxCmdletsTimePeriod;
			this.powerShellMaxDestructiveCmdletsPolicyValue = policy.PowerShellMaxDestructiveCmdlets;
			this.powerShellMaxDestructiveCmdletsTimePeriodPolicyValue = policy.PowerShellMaxDestructiveCmdletsTimePeriod;
			this.activeRunspacesPolicyValue = policy.MaxConcurrency;
			this.powerShellMaxRunspacesPolicyValue = policy.PowerShellMaxRunspaces;
			this.powerShellMaxRunspacesTimePeriodPolicyValue = policy.PowerShellMaxRunspacesTimePeriod;
		}

		private bool IsRwsApplication()
		{
			StackTrace stackTrace = new StackTrace();
			return stackTrace.ToString().Contains("Microsoft.Exchange.Management.ReportingWebService");
		}

		protected virtual string MaxConcurrencyOverBudgetReason
		{
			get
			{
				return "MaxConcurrency";
			}
		}

		protected virtual string MaxRunspacesTimePeriodOverBudgetReason
		{
			get
			{
				return "MaxRunspacesTimePeriod";
			}
		}

		protected virtual bool TrackActiveRunspacePerfCounter
		{
			get
			{
				return false;
			}
		}

		private static string GetUserFriendlyLongValue(long value)
		{
			if (value == 9223372036854775807L)
			{
				return "Unlimited";
			}
			return value.ToString();
		}

		public const string MaxConcurrencyPart = "MaxConcurrency";

		public const string MaxTenantConcurrencyPart = "MaxTenantConcurrency";

		public const string MaxCmdletsTimePeriodPart = "PowerShellMaxCmdlets";

		public const string MaxExchangeCmdletsTimePeriodPart = "ExchangeMaxCmdlets";

		public const string MaxCmdletQueueDepthPart = "MaxCmdletQueueDepth";

		public const string MaxDestructiveCmdletsPart = "MaxDestructiveCmdlets";

		public const string MaxDestructiveCmdletsTimePeriodPart = "MaxDestructiveCmdletsTimePeriod";

		public const string MaxRunspacesTimePeriodPart = "MaxRunspacesTimePeriod";

		private const string StateTraceFormatString = "Owner:\t{0}\r\nBudgetType:\t{1}\r\nActiveRunspaces:\t{2}/{3}\r\nBalance:\t{4}/{5}/{6}\r\nPowerShellCmdletsLeft:\t{7}/{8}\r\nExchangeCmdletsLeft:\t{9}/{10}\r\nCmdletTimePeriod:\t{11}\r\nDestructiveCmdletsLeft:\t{12}/{13}\r\nDestructiveCmdletTimePeriod:\t{14}\r\nQueueDepth:\t{15}\r\nMaxRunspacesTimePeriod:\t{16}\r\nRunSpacesRemaining:\t{17}/{18}\r\nLastTimeFrameUpdate:\t{19}\r\nLastTimeFrameUpdateDestructiveCmdlets:\t{20}\r\nLastTimeFrameUpdateMaxRunspaces:\t{21}\r\nLocked:\t{22}\r\nLockRemaining:\t{23}\r\n";

		private const string WSManBudgetUsageFormatString = "Concurrency:{0}/{1} RunSpaces:{2}/{3}/{4} PowerShellCmdlet:{5}/{6}/{7}";

		private const string CmdletUsageFormatString = "ExchangeCmdlet:{0}/{1}/{2} DestructiveCmdlet:{3}/{4}/{5} Balance:{6}/{7}/{8}";

		private static readonly string[] DestructiveCmdlets = new string[]
		{
			"disable-mailbox",
			"move-activemailboxdatabase",
			"remove-accepteddomain",
			"remove-mailbox",
			"remove-mailuser",
			"remove-organization",
			"remove-secondarydomain",
			"remove-syncmailbox",
			"remove-syncmailuser",
			"start-organizationupgrade",
			"update-mailboxdatabasecopy"
		};

		private int updatingLastUpdate;

		private int activeRunspaces;

		private DateTime lastTimeFrame = TimeProvider.UtcNow;

		private DateTime lastUpdate = TimeProvider.UtcNow;

		private DateTime lastTimeFrameDestructiveCmdlets = TimeProvider.UtcNow;

		private DateTime lastUpdateDestructiveCmdlets = TimeProvider.UtcNow;

		private DateTime lastTimeFrameRunspaces = TimeProvider.UtcNow;

		private DateTime lastUpdateMaxRunspaces = TimeProvider.UtcNow;

		private long cmdletsRemaining = long.MaxValue;

		private long exchangeCmdletsRemaining = long.MaxValue;

		private long destructiveCmdletsRemaining = long.MaxValue;

		private long runspacesRemaining = long.MaxValue;

		protected Unlimited<uint> exchangeMaxCmdletsPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxCmdletQueueDepthPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxCmdletsPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxCmdletsTimePeriodPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> activeRunspacesPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxDestructiveCmdletsPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxDestructiveCmdletsTimePeriodPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxRunspacesPolicyValue = Unlimited<uint>.UnlimitedValue;

		protected Unlimited<uint> powerShellMaxRunspacesTimePeriodPolicyValue = Unlimited<uint>.UnlimitedValue;

		private bool firstTimeUpdate = true;
	}
}
