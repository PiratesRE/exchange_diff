using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ResourceLoadDelayInfo : DelayInfo
	{
		public ResourceLoadDelayInfo(TimeSpan delay, ResourceKey resourceKey, ResourceLoad resourceLoad, TimeSpan workAccomplished, bool required) : base(delay, required)
		{
			this.ResourceKey = resourceKey;
			this.ResourceLoad = resourceLoad;
			this.WorkAccomplished = workAccomplished;
		}

		public static bool IgnoreSleepsForTest { get; set; }

		public static Func<DelayEnforcementResults> EnforceDelayTestHook { get; set; }

		public ResourceKey ResourceKey { get; private set; }

		public ResourceLoad ResourceLoad { get; private set; }

		public TimeSpan WorkAccomplished { get; private set; }

		public static void Initialize()
		{
			SettingOverrideSync.Instance.Start(true);
		}

		public static void CheckResourceHealth(IBudget budget, WorkloadSettings settings, ResourceKey[] resourcesToAccess)
		{
			ResourceUnhealthyException ex = null;
			if (ResourceLoadDelayInfo.TryCheckResourceHealth(budget, settings, resourcesToAccess, out ex))
			{
				throw ex;
			}
		}

		public static bool TryCheckResourceHealth(IBudget budget, WorkloadSettings settings, ResourceKey[] resourcesToAccess, out ResourceUnhealthyException resourceUnhealthyException)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			resourceUnhealthyException = null;
			if (budget.ThrottlingPolicy.IsServiceAccount || settings.IsBackgroundLoad)
			{
				ResourceKey resourceKey;
				ResourceLoad resourceLoad;
				ResourceLoadDelayInfo.GetWorstResource(settings.WorkloadType, resourcesToAccess, out resourceKey, out resourceLoad);
				if (resourceKey != null && resourceLoad.State == ResourceLoadState.Critical)
				{
					resourceUnhealthyException = new ResourceUnhealthyException(resourceKey);
				}
			}
			return resourceUnhealthyException != null;
		}

		public static DelayInfo GetDelay(IBudget budget, WorkloadSettings settings, ResourceKey[] resourcesToAccess, bool scopeDelay = true)
		{
			return ResourceLoadDelayInfo.GetDelay(budget, settings, Budget.AllCostTypes, resourcesToAccess, scopeDelay);
		}

		public static DelayInfo GetDelay(IBudget budget, WorkloadSettings settings, ICollection<CostType> costTypesToConsider, ResourceKey[] resourcesToAccess, bool scopeDelay = true)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			ResourceLoadDelayInfo resourceLoadDelayInfo = null;
			DelayInfo delay = budget.GetDelay(costTypesToConsider);
			DelayInfo delayInfo = delay;
			if (delay.Delay == Budget.IndefiniteDelay)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>(0L, "[ResourceLoadDelayInfo.GetDelay] The user delay for '{0}' was Int32.MaxValue, so no need to consider resource health based delay.", budget.Owner);
			}
			else
			{
				if (settings.IsBackgroundLoad || budget.ThrottlingPolicy.IsServiceAccount)
				{
					resourceLoadDelayInfo = ResourceLoadDelayInfo.GetResourceLoadDelayInfo(budget, settings.WorkloadType, resourcesToAccess);
				}
				else
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug(0L, "[ResourceLoadDelayInfo.GetWorstResource] The work is interactive and therefore will not consider the health of the resources.");
				}
				if (resourceLoadDelayInfo != null && resourceLoadDelayInfo.Delay > delay.Delay)
				{
					delayInfo = resourceLoadDelayInfo;
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug(0L, "[ResourceLoadDelayInfo.GetDelay] Resource delay for '{0}' was greater than user delay for '{1}' of '{2}'. Using resource delay '{3}'.", new object[]
					{
						resourceLoadDelayInfo.ResourceKey,
						budget.Owner,
						delay.Delay,
						delayInfo.Delay
					});
				}
			}
			if (delayInfo != null)
			{
				string instance = ResourceLoadDelayInfo.GetInstance(delayInfo);
				bool flag = delayInfo is ResourceLoadDelayInfo;
				if (delayInfo.Delay == Budget.IndefiniteDelay && flag)
				{
					WorkloadManagementLogger.SetResourceBlocked(instance, null);
				}
				else if (delayInfo.Delay != TimeSpan.Zero && scopeDelay)
				{
					WorkloadManagementLogger.SetThrottlingValues(delayInfo.Delay, !flag, instance, null);
				}
			}
			return delayInfo;
		}

		public static DelayEnforcementResults EnforceDelay(IBudget budget, WorkloadSettings settings, ResourceKey[] resourcesToAccess, TimeSpan preferredMaxDelay, Func<DelayInfo, bool> onBeforeDelay)
		{
			return ResourceLoadDelayInfo.EnforceDelay(budget, settings, Budget.AllCostTypes, resourcesToAccess, preferredMaxDelay, onBeforeDelay);
		}

		public static DelayEnforcementResults EnforceDelay(IBudget budget, WorkloadSettings settings, ICollection<CostType> costTypesToConsider, ResourceKey[] resourcesToAccess, TimeSpan preferredMaxDelay, Func<DelayInfo, bool> onBeforeDelay)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			if (ResourceLoadDelayInfo.EnforceDelayTestHook != null)
			{
				return ResourceLoadDelayInfo.EnforceDelayTestHook();
			}
			budget.EndLocal();
			DelayInfo delay = ResourceLoadDelayInfo.GetDelay(budget, settings, costTypesToConsider, resourcesToAccess, false);
			string text = null;
			TimeSpan timeSpan = TimeSpan.Zero;
			if (delay.Delay == TimeSpan.Zero)
			{
				text = "No Delay Necessary";
			}
			else if (delay.Required && delay.Delay > preferredMaxDelay)
			{
				text = "Strict Delay Exceeds Preferred Delay";
			}
			else
			{
				timeSpan = ((delay.Delay > preferredMaxDelay) ? preferredMaxDelay : delay.Delay);
				if (timeSpan.TotalMilliseconds > 2147483647.0)
				{
					text = "Delay Too Long";
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, string>(0L, "[ResourceLoadDelayInfo.EnforceDelay] Budget: {0}.  Not enforcing delay for reason {1}", budget.Owner, text);
				return new DelayEnforcementResults(delay, text);
			}
			bool flag = false;
			DelayEnforcementResults result;
			try
			{
				BudgetTypeSetting budgetTypeSetting = BudgetTypeSettings.Get(budget.Owner.BudgetType);
				if (onBeforeDelay == null || onBeforeDelay(delay))
				{
					lock (ResourceLoadDelayInfo.staticLock)
					{
						if (ResourceLoadDelayInfo.delayedThreads >= budgetTypeSetting.MaxDelayedThreads - 1)
						{
							ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, string>(0L, "[ResourceLoadDelayInfo.EnforceDelay] Budget: {0}.  Not enforcing delay for reason {1}", budget.Owner, "Max Delayed Threads Exceeded");
							return new DelayEnforcementResults(delay, "Max Delayed Threads Exceeded");
						}
						ResourceLoadDelayInfo.delayedThreads++;
						ThrottlingPerfCounterWrapper.SetDelayedThreads((long)ResourceLoadDelayInfo.delayedThreads);
						flag = true;
					}
					budget.ResetWorkAccomplished();
					if (!ResourceLoadDelayInfo.IgnoreSleepsForTest)
					{
						ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey, TimeSpan, TimeSpan>(0L, "[ResourceLoadDelayInfo.EnforceDelay] Budget: {0} sleeping for {1}.  Capped Delay: {2}.", budget.Owner, timeSpan, delay.Delay);
						string instance = ResourceLoadDelayInfo.GetInstance(delay);
						WorkloadManagementLogger.SetThrottlingValues(timeSpan, !(delay is ResourceLoadDelayInfo), instance, null);
						Thread.Sleep(timeSpan);
					}
					result = new DelayEnforcementResults(delay, timeSpan);
				}
				else
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>(0L, "[ResourceLoadDelayInfo.EnforceDelay] Budget: {0} did not sleep because OnBeforeDelay callback returned false.", budget.Owner);
					result = new DelayEnforcementResults(delay, "OnBeforeDelay delegate returned false");
				}
			}
			finally
			{
				if (flag)
				{
					lock (ResourceLoadDelayInfo.staticLock)
					{
						ResourceLoadDelayInfo.delayedThreads--;
						ThrottlingPerfCounterWrapper.SetDelayedThreads((long)ResourceLoadDelayInfo.delayedThreads);
					}
				}
			}
			return result;
		}

		public static string GetInstance(DelayInfo delayInfo)
		{
			if (delayInfo is ResourceLoadDelayInfo)
			{
				return ((ResourceLoadDelayInfo)delayInfo).ResourceKey.ToString();
			}
			return null;
		}

		public static void GetWorstResource(WorkloadType workloadType, ResourceKey[] resourcesToAccess, out ResourceKey worstResource, out ResourceLoad worstLoad)
		{
			ResourceLoad[] array = null;
			ResourceLoadDelayInfo.GetWorstResourceHelper(workloadType, resourcesToAccess, false, out array, out worstResource, out worstLoad);
		}

		public static void GetWorstResourceAndAllHealthValues(WorkloadType workloadType, ResourceKey[] resourcesToAccess, out ResourceLoad[] resourceLoadList, out ResourceKey worstResource, out ResourceLoad worstLoad)
		{
			ResourceLoadDelayInfo.GetWorstResourceHelper(workloadType, resourcesToAccess, true, out resourceLoadList, out worstResource, out worstLoad);
		}

		private static void GetWorstResourceHelper(WorkloadType workloadType, ResourceKey[] resourcesToAccess, bool getAllResourceLoads, out ResourceLoad[] resourceLoadList, out ResourceKey worstResource, out ResourceLoad worstLoad)
		{
			resourceLoadList = null;
			worstResource = null;
			worstLoad = ResourceLoad.Zero;
			WorkloadPolicy workloadPolicy = new WorkloadPolicy(workloadType);
			if (resourcesToAccess == null || resourcesToAccess.Length == 0)
			{
				return;
			}
			resourceLoadList = (getAllResourceLoads ? new ResourceLoad[resourcesToAccess.Length] : null);
			for (int i = 0; i < resourcesToAccess.Length; i++)
			{
				IResourceLoadMonitor resourceLoadMonitor = ResourceHealthMonitorManager.Singleton.Get(resourcesToAccess[i]);
				if (resourceLoadMonitor == null)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug<ResourceKey, string>(0L, "[ResourceLoadDelayInfo.GetWorstResource] Monitor '{0}' does not implement IResourceLoadMonitor.  Ignoring for delay calculation.  Actual type: {1}", resourcesToAccess[i], resourcesToAccess[i].GetType().FullName);
				}
				ResourceLoad resourceLoad = resourceLoadMonitor.GetResourceLoad(workloadPolicy.Classification, false, null);
				if (resourceLoadList != null)
				{
					resourceLoadList[i] = resourceLoad;
				}
				if (resourceLoad != ResourceLoad.Unknown && resourceLoad.LoadRatio > worstLoad.LoadRatio)
				{
					worstLoad = resourceLoad;
					worstResource = resourcesToAccess[i];
					if (worstLoad == ResourceLoad.Critical)
					{
						return;
					}
				}
			}
		}

		private static ResourceLoadDelayInfo GetResourceLoadDelayInfo(IBudget budget, WorkloadType workloadType, ResourceKey[] resourcesToAccess)
		{
			ResourceKey resourceKey = null;
			ResourceLoad zero = ResourceLoad.Zero;
			ResourceLoadDelayInfo.GetWorstResource(workloadType, resourcesToAccess, out resourceKey, out zero);
			if (resourceKey != null && (zero.State == ResourceLoadState.Overloaded || zero.State == ResourceLoadState.Critical))
			{
				BudgetTypeSetting budgetTypeSetting = BudgetTypeSettings.Get(budget.Owner.BudgetType);
				TimeSpan timeSpan = Budget.IndefiniteDelay;
				TimeSpan delay = Budget.IndefiniteDelay;
				if (zero.State != ResourceLoadState.Critical)
				{
					try
					{
						timeSpan = TimeSpan.FromMilliseconds((zero.LoadRatio - 1.0) * budget.ResourceWorkAccomplished.TotalMilliseconds);
					}
					catch (OverflowException)
					{
						timeSpan = Budget.IndefiniteDelay;
					}
					delay = ((timeSpan > budgetTypeSetting.MaxDelay) ? budgetTypeSetting.MaxDelay : timeSpan);
				}
				return new ResourceLoadDelayInfo(delay, resourceKey, zero, budget.ResourceWorkAccomplished, true);
			}
			return null;
		}

		private static object staticLock = new object();

		private static int delayedThreads = 0;
	}
}
