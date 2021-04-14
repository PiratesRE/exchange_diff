using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class Budget : IReadOnlyPropertyBag
	{
		public static Action<Budget, CostHandle, TimeSpan, TimeSpan> OnLeakDetectionForTest { get; set; }

		public static Action<StringBuilder> OnLeakWatsonInfoForTest { get; set; }

		public static Func<IThrottlingPolicy, IThrottlingPolicy> OnPolicyUpdateForTest { get; set; }

		static Budget()
		{
			Budget.AllCostTypes = Budget.GetAllCostTypes();
			Budget.maximumActionTimes = Budget.GetMaxActionTimesFromConfig();
		}

		internal Budget(BudgetKey key, IThrottlingPolicy policy)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (policy == null)
			{
				throw new ArgumentNullException("policy");
			}
			this.CreationTime = TimeProvider.UtcNow;
			this.Owner = key;
			this.percentileUsage = BudgetUsageTracker.Get(key);
			this.SetPolicy(policy, true);
		}

		internal static TimeSpan GetMaxActionTime(CostType costType)
		{
			return Budget.maximumActionTimes[costType];
		}

		internal static void SetMaxActionTime(CostType costType, TimeSpan timeout)
		{
			if (timeout.TotalMilliseconds <= 0.0)
			{
				throw new ArgumentException("Timeout must be greater than zero.");
			}
			Budget.maximumActionTimes[costType] = timeout;
		}

		private static TimeSpan GetMaxActionTimeDefault(CostType costType)
		{
			if (costType == CostType.ActiveRunspace)
			{
				return Budget.defaultActiveRunspaceMaximumActionTime;
			}
			return Budget.defaultMaximumActionTime;
		}

		private static Dictionary<CostType, TimeSpan> GetMaxActionTimesFromConfig()
		{
			Dictionary<CostType, TimeSpan> dictionary = new Dictionary<CostType, TimeSpan>();
			foreach (CostType costType in Budget.AllCostTypes)
			{
				TimeSpan maxActionTimeDefault = Budget.GetMaxActionTimeDefault(costType);
				string text = costType + "_MaxTimeInMinutes";
				TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry(text, TimeSpanUnit.Minutes, maxActionTimeDefault, ExTraceGlobals.ClientThrottlingTracer);
				if (timeSpanAppSettingsEntry.Value > TimeSpan.Zero)
				{
					dictionary[costType] = timeSpanAppSettingsEntry.Value;
				}
				else
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceError<string, TimeSpan, TimeSpan>(0L, "[Budget.GetMaxActionTimesFromConfig] Invalid value for appSetting {0}.  Read Value: {1}.  Using {2} instead.", text, timeSpanAppSettingsEntry.Value, maxActionTimeDefault);
					dictionary[costType] = maxActionTimeDefault;
				}
			}
			return dictionary;
		}

		private static HashSet<CostType> GetAllCostTypes()
		{
			Array values = Enum.GetValues(typeof(CostType));
			HashSet<CostType> hashSet = new HashSet<CostType>();
			foreach (object obj in values)
			{
				CostType item = (CostType)obj;
				hashSet.Add(item);
			}
			return hashSet;
		}

		internal ITokenBucket CasTokenBucket
		{
			get
			{
				return this.casTokenBucket;
			}
		}

		internal bool IsExpired
		{
			get
			{
				return this.isExpired;
			}
		}

		internal BudgetKey Owner { get; private set; }

		internal SingleComponentThrottlingPolicy ThrottlingPolicy { get; private set; }

		protected object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		public LocalTimeCostHandle StartLocal(Action<CostHandle> onRelease, string callerInfo, TimeSpan preCharge)
		{
			LocalTimeCostHandle result = null;
			lock (this.SyncRoot)
			{
				this.casTokenBucket.Increment();
				result = new LocalTimeCostHandle(this, onRelease, string.Format("Caller: {0}, ThreadID: {1}, PreCharge: {2}ms", callerInfo, Environment.CurrentManagedThreadId, preCharge.TotalMilliseconds), preCharge);
			}
			return result;
		}

		internal void Expire()
		{
			Dictionary<long, CostHandle> dictionary = null;
			lock (this.SyncRoot)
			{
				this.isExpired = true;
				if (this.outstandingActions.Count > 0)
				{
					dictionary = this.outstandingActions;
					this.outstandingActions = new Dictionary<long, CostHandle>();
				}
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<long, CostHandle> keyValuePair in dictionary)
				{
					keyValuePair.Value.Dispose();
				}
			}
			this.AfterExpire();
		}

		protected virtual void AfterExpire()
		{
		}

		internal void End(CostHandle costHandle)
		{
			bool flag = false;
			lock (this.SyncRoot)
			{
				flag = this.outstandingActions.Remove(costHandle.Key);
				if (flag)
				{
					this.AccountForCostHandle(costHandle);
				}
			}
			if (!flag)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError((long)this.GetHashCode(), "[Budget.End] CostHandle was not in outstanding actions collection.  Ignoring.");
			}
		}

		public int OutstandingActionsCount
		{
			get
			{
				return this.outstandingActions.Count;
			}
		}

		internal void AddOutstandingAction(CostHandle costHandle)
		{
			lock (this.SyncRoot)
			{
				this.outstandingActions.Add(costHandle.Key, costHandle);
			}
		}

		public virtual void CheckOverBudget()
		{
			this.CheckOverBudget(Budget.AllCostTypes);
		}

		public virtual void CheckOverBudget(ICollection<CostType> costTypesToConsider)
		{
			OverBudgetException ex = null;
			if (this.TryCheckOverBudget(costTypesToConsider, out ex))
			{
				throw ex;
			}
		}

		internal bool TryCheckOverBudget(ICollection<CostType> costTypesToConsider, out OverBudgetException exception)
		{
			bool flag = false;
			exception = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3670420797U, ref flag);
			return !flag && this.InternalTryCheckOverBudget(costTypesToConsider, out exception);
		}

		internal bool TryCheckOverBudget(out OverBudgetException exception)
		{
			return this.TryCheckOverBudget(Budget.AllCostTypes, out exception);
		}

		internal OverBudgetException CreateOverBudgetException(string reason, string policyValue, int backoffTime)
		{
			ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[Budget.CreateOverBudgetException] user '{0}' is over-budget for {1} budget part '{2}', policy Value '{3}', backoffTime: {4} msec", new object[]
			{
				this.Owner,
				this.Owner.IsServiceAccountBudget ? "service account" : "normal",
				reason,
				policyValue,
				backoffTime
			});
			return new OverBudgetException(this, reason, policyValue, backoffTime);
		}

		internal virtual void AfterCacheHit()
		{
			this.UpdatePolicy();
			this.UpdatePercentileReference();
		}

		private void UpdatePercentileReference()
		{
			if (this.percentileUsage == null || this.percentileUsage.Expired)
			{
				this.percentileUsage = BudgetUsageTracker.Get(this.Owner);
			}
		}

		internal virtual EffectiveThrottlingPolicy GetEffectiveThrottlingPolicy()
		{
			return this.ThrottlingPolicy.FullPolicy as EffectiveThrottlingPolicy;
		}

		internal virtual bool UpdatePolicy()
		{
			EffectiveThrottlingPolicy effectiveThrottlingPolicy = this.GetEffectiveThrottlingPolicy();
			if (effectiveThrottlingPolicy == null && Budget.OnPolicyUpdateForTest == null)
			{
				return false;
			}
			if (TimeProvider.UtcNow - this.lastPolicyCheck >= Budget.PolicyUpdateCheckInterval)
			{
				this.lastPolicyCheck = TimeProvider.UtcNow;
				IThrottlingPolicy throttlingPolicy;
				if (Budget.OnPolicyUpdateForTest != null)
				{
					throttlingPolicy = Budget.OnPolicyUpdateForTest(this.ThrottlingPolicy.FullPolicy);
				}
				else
				{
					switch (effectiveThrottlingPolicy.ThrottlingPolicyScope)
					{
					case ThrottlingPolicyScopeType.Regular:
						throttlingPolicy = ThrottlingPolicyCache.Singleton.Get(effectiveThrottlingPolicy.OrganizationId, effectiveThrottlingPolicy.Id);
						break;
					case ThrottlingPolicyScopeType.Organization:
						throttlingPolicy = ThrottlingPolicyCache.Singleton.Get(effectiveThrottlingPolicy.OrganizationId);
						break;
					case ThrottlingPolicyScopeType.Global:
						throttlingPolicy = ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported enum value {0}.", effectiveThrottlingPolicy.ThrottlingPolicyScope));
					}
				}
				if (!object.ReferenceEquals(effectiveThrottlingPolicy, throttlingPolicy))
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[Budget.UpdatePolicy] Obtained a refreshed policy from the cache.");
					this.SetPolicy(throttlingPolicy, false);
					return true;
				}
			}
			return false;
		}

		internal string GetBalanceForTrace()
		{
			if (!(this.CasTokenBucket is UnthrottledTokenBucket))
			{
				return this.CasTokenBucket.GetBalance().ToString();
			}
			return "$null";
		}

		internal void SetPolicy(IThrottlingPolicy policy, bool resetBudgetValues)
		{
			lock (this.SyncRoot)
			{
				SingleComponentThrottlingPolicy singleComponentPolicy = this.GetSingleComponentPolicy(policy);
				this.ThrottlingPolicy = singleComponentPolicy;
				this.InternalUpdateCachedPolicyValues(resetBudgetValues);
			}
		}

		protected virtual SingleComponentThrottlingPolicy GetSingleComponentPolicy(IThrottlingPolicy policy)
		{
			return new SingleComponentThrottlingPolicy(this.Owner.BudgetType, policy);
		}

		internal bool CanExpire
		{
			get
			{
				bool flag = false;
				lock (this.SyncRoot)
				{
					flag = (this.OutstandingActionsCount == 0 && this.IsTokenBucketFullyRecharged());
				}
				if (flag)
				{
					flag = this.InternalCanExpire;
				}
				if (!flag)
				{
					this.CheckLeakedActions();
					LookupBudgetKey lookupBudgetKey = this.Owner as LookupBudgetKey;
					if (lookupBudgetKey != null)
					{
						IThrottlingPolicy throttlingPolicy = lookupBudgetKey.Lookup();
						if (throttlingPolicy.GetIdentityString() != this.ThrottlingPolicy.FullPolicy.GetIdentityString())
						{
							ExTraceGlobals.ClientThrottlingTracer.TraceDebug<string, string, BudgetKey>((long)this.GetHashCode(), "[Budget.CanExpire] Budget throttling policy reference changed from {0} to {1}.  Updating policy link for account {2}.", this.ThrottlingPolicy.FullPolicy.GetIdentityString(), throttlingPolicy.GetIdentityString(), this.Owner);
							this.SetPolicy(throttlingPolicy, false);
						}
					}
				}
				return flag;
			}
		}

		protected virtual bool InternalCanExpire
		{
			get
			{
				return true;
			}
		}

		private bool IsTokenBucketFullyRecharged()
		{
			bool result;
			lock (this.SyncRoot)
			{
				if (this.ThrottlingPolicy.RechargeRate.IsUnlimited || this.ThrottlingPolicy.MaxBurst.IsUnlimited)
				{
					result = true;
				}
				else
				{
					uint value = this.ThrottlingPolicy.MaxBurst.Value;
					if (this.CasTokenBucket.GetBalance() == value && !this.CasTokenBucket.Locked)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		internal virtual void SetStateFromPolicyForTest(bool resetState)
		{
			this.SetPolicy(this.ThrottlingPolicy.FullPolicy, resetState);
		}

		internal virtual void SetStateFromPolicyForTest()
		{
			this.SetStateFromPolicyForTest(true);
		}

		private protected DateTime CreationTime { protected get; private set; }

		protected virtual void UpdateCachedPolicyValues(bool resetBudgetValues)
		{
		}

		private void InternalUpdateCachedPolicyValues(bool resetBudgetValues)
		{
			lock (this.SyncRoot)
			{
				this.casTokenBucket = TokenBucket.Create(resetBudgetValues ? null : this.casTokenBucket, this.ThrottlingPolicy.MaxBurst, this.ThrottlingPolicy.RechargeRate, this.ThrottlingPolicy.CutoffBalance, this.Owner);
			}
			this.UpdateCachedPolicyValues(resetBudgetValues);
		}

		protected virtual void AccountForCostHandle(CostHandle costHandle)
		{
			if (costHandle.CostType == CostType.CAS)
			{
				TimeSpan timeSpan = costHandle.PreCharge;
				LocalTimeCostHandle localTimeCostHandle = costHandle as LocalTimeCostHandle;
				bool flag = localTimeCostHandle != null && localTimeCostHandle.ReverseBudgetCharge;
				if (flag)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug((long)this.GetHashCode(), "[Budget.AccountForCostHandle] Not charging budget for CAS time per protocol's request.");
					timeSpan += TimeProvider.UtcNow - costHandle.StartTime;
				}
				this.casTokenBucket.Decrement(timeSpan, flag);
				this.percentileUsage.AddUsage((int)(TimeProvider.UtcNow - costHandle.StartTime).TotalMilliseconds);
			}
		}

		protected virtual bool InternalTryCheckOverBudget(ICollection<CostType> costTypes, out OverBudgetException exception)
		{
			exception = null;
			bool flag = false;
			int num = 0;
			string text = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(4238748989U, ref num);
			bool flag2 = false;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3871747389U, ref flag2);
			if (flag2)
			{
				exception = this.CreateOverBudgetException("LocalTime", "faultInjection", num);
				return true;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3165007165U, ref flag);
			if (flag)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2628136253U, ref text);
				if (num != 0 && !string.IsNullOrEmpty(text))
				{
					exception = this.CreateOverBudgetException(text, "faultInjection", num);
					return true;
				}
			}
			DateTime? lockedUntilUtc = this.casTokenBucket.LockedUntilUtc;
			if (lockedUntilUtc != null && costTypes.Contains(CostType.CAS))
			{
				DateTime value = lockedUntilUtc.Value;
				TimeSpan timeSpan = value - TimeProvider.UtcNow;
				if (value == DateTime.MaxValue || timeSpan.TotalMilliseconds > 2147483647.0)
				{
					num = int.MaxValue;
				}
				else
				{
					num = Math.Max(0, (int)timeSpan.TotalMilliseconds);
				}
				if (num > 0)
				{
					this.CheckLeakedActions();
					exception = this.CreateOverBudgetException("LocalTime", this.ThrottlingPolicy.CutoffBalance.ToString(), num);
				}
			}
			return exception != null;
		}

		internal void CheckLeakedActions()
		{
			DateTime utcNow = TimeProvider.UtcNow;
			StringBuilder stringBuilder = null;
			List<CostHandle> list = null;
			if (utcNow - this.lastLeakDetectionUtc >= Budget.LeakDetectionCheckInterval && this.OutstandingActionsCount > 0)
			{
				lock (this.syncRoot)
				{
					if (utcNow - this.lastLeakDetectionUtc >= Budget.LeakDetectionCheckInterval && this.OutstandingActionsCount > 0)
					{
						this.lastLeakDetectionUtc = utcNow;
						foreach (KeyValuePair<long, CostHandle> keyValuePair in this.outstandingActions)
						{
							CostHandle value = keyValuePair.Value;
							TimeSpan timeSpan = utcNow - value.StartTime;
							if (value.CostType == CostType.CAS && timeSpan > value.MaxLiveTime && !value.LeakLogged)
							{
								if (Budget.OnLeakDetectionForTest != null)
								{
									Budget.OnLeakDetectionForTest(this, value, timeSpan, value.MaxLiveTime);
								}
								if (Budget.OnLeakDetectionForTest == null || Budget.OnLeakWatsonInfoForTest != null)
								{
									if (stringBuilder == null)
									{
										stringBuilder = new StringBuilder();
									}
									stringBuilder.AppendLine(string.Format("CostType: {0}, Key: {1}, Limit: {2}, Elapsed: {3}, Actions: {4}, Description: {5}, Snapshot: {6}", new object[]
									{
										value.CostType,
										keyValuePair.Key,
										value.MaxLiveTime,
										timeSpan,
										this.OutstandingActionsCount,
										value.Description,
										this
									}));
								}
								value.LeakLogged = true;
								if (list == null)
								{
									list = new List<CostHandle>();
								}
								list.Add(value);
							}
						}
					}
				}
			}
			if (Budget.OnLeakWatsonInfoForTest != null)
			{
				Budget.OnLeakWatsonInfoForTest(stringBuilder);
			}
			else if (stringBuilder != null)
			{
				try
				{
					throw new LongRunningCostHandleException();
				}
				catch (LongRunningCostHandleException exception)
				{
					ExWatson.SendReport(exception, ReportOptions.None, stringBuilder.ToString());
				}
			}
			if (stringBuilder != null)
			{
				string text = stringBuilder.ToString();
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_BudgetActionExceededExpectedTime, text, new object[]
				{
					"\n" + text
				});
			}
			if (list != null)
			{
				foreach (CostHandle costHandle in list)
				{
					costHandle.Dispose();
				}
			}
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				array[num++] = ((IReadOnlyPropertyBag)this)[propertyDefinition];
			}
			return array;
		}

		object IReadOnlyPropertyBag.this[PropertyDefinition propertyDefinition]
		{
			get
			{
				if (propertyDefinition == BudgetMetadataSchema.Balance)
				{
					return this.CasTokenBucket.GetBalance();
				}
				if (propertyDefinition == BudgetMetadataSchema.InCutoff)
				{
					return this.CasTokenBucket.Locked;
				}
				if (propertyDefinition == BudgetMetadataSchema.InMicroDelay)
				{
					return !this.CasTokenBucket.Locked && this.CasTokenBucket.GetBalance() < 0f;
				}
				if (propertyDefinition == BudgetMetadataSchema.IsGlobalPolicy)
				{
					return this.ThrottlingPolicy.FullPolicy.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Global;
				}
				if (propertyDefinition == BudgetMetadataSchema.IsOrgPolicy)
				{
					return this.ThrottlingPolicy.FullPolicy.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Organization;
				}
				if (propertyDefinition == BudgetMetadataSchema.IsRegularPolicy)
				{
					return this.ThrottlingPolicy.FullPolicy.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Regular;
				}
				if (propertyDefinition == BudgetMetadataSchema.IsServiceAccount)
				{
					return this.Owner.IsServiceAccountBudget;
				}
				if (propertyDefinition == BudgetMetadataSchema.LiveTime)
				{
					return DateTime.UtcNow - this.CreationTime;
				}
				if (propertyDefinition == BudgetMetadataSchema.Name)
				{
					return this.Owner.ToString();
				}
				if (propertyDefinition == BudgetMetadataSchema.NotThrottled)
				{
					return !this.CasTokenBucket.Locked && this.CasTokenBucket.GetBalance() >= 0f;
				}
				if (propertyDefinition == BudgetMetadataSchema.OutstandingActionCount)
				{
					return this.OutstandingActionsCount;
				}
				if (propertyDefinition == BudgetMetadataSchema.ThrottlingPolicy)
				{
					return this.ThrottlingPolicy.FullPolicy.GetShortIdentityString();
				}
				throw new ArgumentException(string.Format("Unexpected property name '{0}'.", propertyDefinition.Name));
			}
		}

		public const int IndefiniteBackoff = 2147483647;

		public const string LocalTimeReason = "LocalTime";

		private const uint LidChangeIsOverBudgetValue = 3165007165U;

		private const uint LidChangeBackoffTimeValue = 4238748989U;

		private const uint LidChangeOverBudgetReason = 2628136253U;

		private const uint LidChangeLocked = 3871747389U;

		private const uint LidChangeIgnoreOverBudget = 3670420797U;

		public static readonly TimeSpan IndefiniteDelay = TimeSpan.MaxValue;

		public static readonly TimeSpan PolicyUpdateCheckInterval = TimeSpan.FromMinutes(1.0);

		public static readonly TimeSpan LeakDetectionCheckInterval = TimeSpan.FromMinutes(10.0);

		private static readonly Dictionary<CostType, TimeSpan> maximumActionTimes;

		internal static readonly HashSet<CostType> AllCostTypes;

		private static readonly TimeSpan defaultMaximumActionTime = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan defaultActiveRunspaceMaximumActionTime = TimeSpan.MaxValue;

		private Dictionary<long, CostHandle> outstandingActions = new Dictionary<long, CostHandle>();

		private DateTime lastLeakDetectionUtc = DateTime.MinValue;

		private DateTime lastPolicyCheck = TimeProvider.UtcNow;

		private PercentileUsage percentileUsage;

		private bool isExpired;

		private ITokenBucket casTokenBucket;

		private object syncRoot = new object();
	}
}
