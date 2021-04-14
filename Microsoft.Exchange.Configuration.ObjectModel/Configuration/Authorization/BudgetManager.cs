using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal abstract class BudgetManager
	{
		protected abstract TimeSpan BudgetTimeout { get; }

		protected IDictionary<string, IPowerShellBudget> Budgets
		{
			get
			{
				return this.budgets;
			}
		}

		protected object InstanceLock
		{
			get
			{
				return this.instanceLock;
			}
		}

		internal int TotalActiveUsers
		{
			get
			{
				int result;
				lock (this.InstanceLock)
				{
					result = this.Budgets.Count((KeyValuePair<string, IPowerShellBudget> x) => x.Value.TotalActiveRunspacesCount > 0);
				}
				return result;
			}
		}

		internal int TotalActiveRunspaces
		{
			get
			{
				int result;
				lock (this.InstanceLock)
				{
					result = this.Budgets.Sum((KeyValuePair<string, IPowerShellBudget> x) => x.Value.TotalActiveRunspacesCount);
				}
				return result;
			}
		}

		internal string GetWSManBudgetUsage(AuthZPluginUserToken userToken)
		{
			string result;
			lock (this.instanceLock)
			{
				IPowerShellBudget budget = this.GetBudget(userToken, false, false);
				if (budget == null)
				{
					result = null;
				}
				else
				{
					result = budget.GetWSManBudgetUsage();
				}
			}
			return result;
		}

		internal bool CheckOverBudget(AuthZPluginUserToken userToken, CostType costType, out OverBudgetException exception)
		{
			exception = null;
			if (!this.ShouldThrottling(userToken))
			{
				return false;
			}
			lock (this.instanceLock)
			{
				IPowerShellBudget budget = this.GetBudget(userToken, false, false);
				if (budget != null)
				{
					return budget.TryCheckOverBudget(costType, out exception);
				}
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Try to check overbudget for key {0}. But the budget doesn't exist.", this.CreateKey(userToken));
			}
			return false;
		}

		internal void RemoveBudgetIfNoActiveRunspace(AuthZPluginUserToken userToken)
		{
			lock (this.instanceLock)
			{
				IPowerShellBudget budget = this.GetBudget(userToken, false, false);
				if (budget != null && budget.TotalActiveRunspacesCount <= 0)
				{
					string text = this.CreateKey(userToken);
					Unlimited<uint> powerShellMaxRunspacesTimePeriod = budget.ThrottlingPolicy.PowerShellMaxRunspacesTimePeriod;
					if (powerShellMaxRunspacesTimePeriod.IsUnlimited)
					{
						ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Key {0} is removed from budgets dictionary immediately.", text);
						this.budgets.Remove(text);
						this.keyToRemoveBudgets.Remove(text);
					}
					else
					{
						int num = (int)(powerShellMaxRunspacesTimePeriod.Value + 5U);
						ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, int>(0L, "Register key {0} to keyToRemoveBudgets, timeout {1} seconds.", text, num);
						this.keyToRemoveBudgets.InsertAbsolute(text, BudgetManager.NormalCleanupCacheValue, TimeSpan.FromSeconds((double)num), new RemoveItemDelegate<string, string>(this.OnKeyToRemoveBudgetsCacheValueRemoved));
					}
				}
			}
			this.UpdateBudgetsPerfCounter(this.budgets.Count);
			this.UpdateKeyToRemoveBudgetsPerfCounter(this.keyToRemoveBudgets.Count);
		}

		internal void HeartBeat(AuthZPluginUserToken userToken)
		{
			lock (this.instanceLock)
			{
				this.HeartBeatImpl(userToken);
			}
		}

		internal void CorrectRunspacesLeakPassively(string key, int leakedValue)
		{
			lock (this.instanceLock)
			{
				IPowerShellBudget powerShellBudget;
				if (this.budgets.TryGetValue(key, out powerShellBudget))
				{
					int totalActiveRunspacesCount = powerShellBudget.TotalActiveRunspacesCount;
					if (totalActiveRunspacesCount > 0)
					{
						ExTraceGlobals.PublicPluginAPITracer.TraceError(0L, "Correct runspaces leak passively for Key {0} in class {1}. Current Value {2}, Leaked value {3}.", new object[]
						{
							key,
							base.GetType().ToString(),
							totalActiveRunspacesCount,
							leakedValue
						});
						AuthZLogger.SafeAppendGenericError("WSManBudgetManagerBase.CorrectRunspacesLeakPassively", string.Format("Correct runspaces leak passively for Key {0} in class {1}. Current Value {2}, Leaked value {3}.", new object[]
						{
							key,
							base.GetType(),
							totalActiveRunspacesCount,
							leakedValue
						}), false);
						TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PSConnectionLeakPassivelyCorrected, null, new object[]
						{
							key,
							base.GetType().ToString(),
							totalActiveRunspacesCount,
							leakedValue
						});
						powerShellBudget.CorrectRunspacesLeak(leakedValue);
					}
				}
			}
		}

		protected virtual CostHandle StartRunspaceImpl(AuthZPluginUserToken userToken)
		{
			IPowerShellBudget budget = this.GetBudget(userToken, true, true);
			if (budget != null)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Start budget tracking for ActiveRunspaces, key {0}", this.CreateKey(userToken));
				return budget.StartActiveRunspace();
			}
			ExTraceGlobals.PublicPluginAPITracer.TraceError<string>(0L, "Try to start budget tracking for ActiveRunspaces, key {0} But the budget doesn't exist.", this.CreateKey(userToken));
			return null;
		}

		protected virtual void HeartBeatImpl(AuthZPluginUserToken userToken)
		{
			this.GetBudget(userToken, false, true);
		}

		protected virtual bool ShouldThrottling(AuthZPluginUserToken userToken)
		{
			return true;
		}

		protected virtual string CreateKey(AuthZPluginUserToken userToken)
		{
			return userToken.UserName;
		}

		protected virtual IPowerShellBudget CreateBudget(AuthZPluginUserToken userToken)
		{
			return userToken.CreateBudget(BudgetType.WSMan);
		}

		protected virtual void UpdateBudgetsPerfCounter(int size)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.PerUserBudgetsDicSize.RawValue = (long)size;
			}
		}

		protected virtual void UpdateKeyToRemoveBudgetsPerfCounter(int size)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.PerUserKeyToRemoveBudgetsCacheSize.RawValue = (long)size;
			}
		}

		protected virtual void UpdateConnectionLeakPerfCounter(int leakedConnection)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.ConnectionLeak.RawValue += (long)leakedConnection;
			}
		}

		protected virtual string CreateRelatedBudgetKey(AuthZPluginUserToken userToken)
		{
			return null;
		}

		protected virtual void CorrectRelatedBudgetWhenLeak(string key, int delta)
		{
		}

		protected IPowerShellBudget GetBudget(AuthZPluginUserToken userToken, bool createIfNotExist, bool isHeartBeat)
		{
			string text = this.CreateKey(userToken);
			if (text == null)
			{
				return null;
			}
			IPowerShellBudget powerShellBudget;
			if (!this.budgets.TryGetValue(text, out powerShellBudget) && createIfNotExist)
			{
				powerShellBudget = this.CreateBudget(userToken);
				if (powerShellBudget != null)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Budget of key {0} is added to budgets.", text);
					this.budgets.Add(text, powerShellBudget);
					this.UpdateBudgetsPerfCounter(this.budgets.Count);
				}
			}
			if (powerShellBudget != null && isHeartBeat)
			{
				this.keyToRemoveBudgets.InsertAbsolute(text, this.CreateRelatedBudgetKey(userToken), this.BudgetTimeout, new RemoveItemDelegate<string, string>(this.OnKeyToRemoveBudgetsCacheValueRemoved));
			}
			return powerShellBudget;
		}

		private void OnKeyToRemoveBudgetsCacheValueRemoved(string key, string value, RemoveReason reason)
		{
			lock (this.instanceLock)
			{
				if (reason != RemoveReason.Removed)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, RemoveReason>(0L, "Key {0} is removed from budgets dictionary after timeout. Remove reason = {1}", key, reason);
					if (!BudgetManager.NormalCleanupCacheValue.Equals(value))
					{
						this.RunspacesLeakDetected(key, value);
					}
					this.budgets.Remove(key);
				}
			}
			if (reason != RemoveReason.Removed)
			{
				this.UpdateBudgetsPerfCounter(this.budgets.Count);
				AuthZPluginHelper.UpdateAuthZPluginPerfCounters(this);
			}
			this.UpdateKeyToRemoveBudgetsPerfCounter(this.keyToRemoveBudgets.Count);
		}

		private void RunspacesLeakDetected(string key, string relatedBudgetKey)
		{
			int num = 0;
			IPowerShellBudget powerShellBudget = null;
			if (this.budgets.TryGetValue(key, out powerShellBudget))
			{
				num = powerShellBudget.TotalActiveRunspacesCount;
			}
			if (powerShellBudget != null)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<string, string, int>(0L, "Connection leak detected for Key {0} in class {1}. Leaked value {2}.", key, base.GetType().ToString(), num);
				if (num > 0)
				{
					AuthZLogger.SafeAppendGenericError("WSManBudgetManagerBase.RunspacesLeakDetected", string.Format("Connection leak detected for Key {0} in class {1}. Leaked value {2}.", key, base.GetType(), num), false);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PSConnectionLeakDetected, null, new object[]
					{
						key,
						base.GetType().ToString(),
						num
					});
					this.UpdateConnectionLeakPerfCounter(num);
				}
				powerShellBudget.Dispose();
			}
			if (num > 0 && relatedBudgetKey != null)
			{
				this.CorrectRelatedBudgetWhenLeak(relatedBudgetKey, num);
			}
		}

		private const int DefaultTimeoutBufferToAdd = 5;

		private readonly IDictionary<string, IPowerShellBudget> budgets = new Dictionary<string, IPowerShellBudget>();

		private readonly TimeoutCache<string, string> keyToRemoveBudgets = new TimeoutCache<string, string>(20, 5000, false);

		private static readonly string NormalCleanupCacheValue = Guid.NewGuid().ToString();

		private readonly object instanceLock = new object();
	}
}
