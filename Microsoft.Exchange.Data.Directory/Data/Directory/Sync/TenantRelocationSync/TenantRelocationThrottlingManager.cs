using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationThrottlingManager : DisposeTrackableBase
	{
		public string PartitionFqdn
		{
			get
			{
				return this.partitionFqdn;
			}
		}

		public TenantRelocationThrottlingManager(string partitionFqdn)
		{
			this.partitionFqdn = partitionFqdn;
			this.adMonitor = ResourceHealthMonitorManager.Singleton.Get(ADResourceKey.Key);
		}

		public int Throttle()
		{
			ResourceLoadState resourceLoadState;
			int healthAndCalculateDelay = this.GetHealthAndCalculateDelay(out resourceLoadState);
			Thread.Sleep(healthAndCalculateDelay);
			return healthAndCalculateDelay;
		}

		public int GetHealthAndCalculateDelay(out ResourceLoadState health)
		{
			if (this.TryReadRegistryHealthOverride(out health))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<ResourceLoadState, string>((long)this.GetHashCode(), "TenantRelocationThrottlingManager.Throttle() - override detected, override value:{0}. Current forest: {1}", health, this.partitionFqdn);
			}
			else
			{
				health = this.GetWlmADHealthMetric();
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<ResourceLoadState, string>((long)this.GetHashCode(), "TenantRelocationThrottlingManager.Throttle() - AD Health monitor returned value:{0}. Current forest: {1}", health, this.partitionFqdn);
			}
			int num;
			switch (health)
			{
			case ResourceLoadState.Unknown:
			case ResourceLoadState.Underloaded:
				num = TenantRelocationConfigImpl.GetConfig<int>("LoadStateNoDelayMs");
				break;
			case ResourceLoadState.Full:
				num = TenantRelocationConfigImpl.GetConfig<int>("LoadStateDefaultDelayMs");
				break;
			case ResourceLoadState.Overloaded:
				num = TenantRelocationConfigImpl.GetConfig<int>("LoadStateOverloadedDelayMs");
				break;
			case ResourceLoadState.Critical:
				num = TenantRelocationConfigImpl.GetConfig<int>("LoadStateCriticalDelayMs");
				break;
			default:
				throw new NotImplementedException();
			}
			if (num < 100)
			{
				num = 100;
			}
			return num;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TenantRelocationThrottlingManager>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.adMonitor = null;
			}
		}

		private ResourceLoadState GetWlmADHealthMetric()
		{
			return this.adMonitor.GetResourceLoad(WorkloadClassification.Discretionary, false, this.partitionFqdn).State;
		}

		private bool TryReadRegistryHealthOverride(out ResourceLoadState healthValue)
		{
			uint num;
			bool int32ValueFromRegistryValue = TenantRelocationSyncCoordinator.GetInt32ValueFromRegistryValue("ADHealthOverrideForTenantRelocation", out num);
			if (int32ValueFromRegistryValue)
			{
				healthValue = (ResourceLoadState)num;
			}
			else
			{
				healthValue = ResourceLoadState.Unknown;
			}
			return int32ValueFromRegistryValue;
		}

		private IResourceLoadMonitor adMonitor;

		private readonly string partitionFqdn;
	}
}
