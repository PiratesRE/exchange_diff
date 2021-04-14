using System;
using System.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncResourceMonitor
	{
		public SyncResourceMonitor(SyncLogSession syncLogSession, ResourceKey resourceKey, SyncResourceMonitorType syncResourceMonitorType)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.SyncLogSession = syncLogSession;
			this.resourceKey = resourceKey;
			this.syncResourceMonitorType = syncResourceMonitorType;
			this.resourceHealthMonitor = this.CreateResourceHealthMonitor(resourceKey);
			this.disabled = AggregationConfiguration.Instance.SyncResourceMonitorsDisabled.Contains(syncResourceMonitorType);
		}

		internal IResourceLoadMonitor ResourceHealthMonitor
		{
			get
			{
				return this.resourceHealthMonitor;
			}
		}

		internal ResourceKey ResourceKey
		{
			get
			{
				return this.resourceKey;
			}
		}

		internal SyncResourceMonitorType SyncResourceMonitorType
		{
			get
			{
				return this.syncResourceMonitorType;
			}
		}

		private protected SyncLogSession SyncLogSession { protected get; private set; }

		internal static SyncResourceMonitorType IsAnyResourceUnhealthyOrUnknown(AggregationWorkItem workItem, IEnumerable syncResourceMonitors, out bool isAnyResourceUnhealthy, out bool isAnyResourceUnknown)
		{
			isAnyResourceUnhealthy = false;
			isAnyResourceUnknown = false;
			SyncResourceMonitorType result = SyncResourceMonitorType.Unknown;
			foreach (object obj in syncResourceMonitors)
			{
				SyncResourceMonitor syncResourceMonitor = (SyncResourceMonitor)obj;
				ResourceLoad resourceLoadIfEnabled = syncResourceMonitor.GetResourceLoadIfEnabled(workItem);
				isAnyResourceUnhealthy |= (resourceLoadIfEnabled.State == ResourceLoadState.Critical);
				if (resourceLoadIfEnabled.State == ResourceLoadState.Critical)
				{
					result = syncResourceMonitor.syncResourceMonitorType;
				}
				isAnyResourceUnknown |= (resourceLoadIfEnabled.State == ResourceLoadState.Unknown);
			}
			return result;
		}

		protected virtual ResourceLoad GetResourceLoad(AggregationWorkItem workItem)
		{
			ResourceLoadDelayInfo.Initialize();
			return this.ResourceHealthMonitor.GetResourceLoad(WorkloadType.TransportSync, false, null);
		}

		protected virtual IResourceLoadMonitor CreateResourceHealthMonitor(ResourceKey resourceKey)
		{
			SyncUtilities.ThrowIfArgumentNull("resourceKey", resourceKey);
			return ResourceHealthMonitorManager.Singleton.Get(resourceKey);
		}

		private ResourceLoad GetResourceLoadIfEnabled(AggregationWorkItem workItem)
		{
			if (this.disabled)
			{
				return ResourceLoad.Zero;
			}
			return this.GetResourceLoad(workItem);
		}

		private readonly bool disabled;

		private ResourceKey resourceKey;

		private SyncResourceMonitorType syncResourceMonitorType;

		private IResourceLoadMonitor resourceHealthMonitor;
	}
}
