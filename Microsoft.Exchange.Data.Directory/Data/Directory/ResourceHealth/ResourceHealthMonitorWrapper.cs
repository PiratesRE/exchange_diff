using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal class ResourceHealthMonitorWrapper : IResourceLoadMonitor
	{
		internal ResourceHealthMonitorWrapper(CacheableResourceHealthMonitor monitor)
		{
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor");
			}
			this.wrappedMonitor = monitor;
		}

		public ResourceKey Key
		{
			get
			{
				this.CheckExpired();
				return this.wrappedMonitor.Key;
			}
		}

		public int GetWrappedHashCode()
		{
			this.CheckExpired();
			return this.wrappedMonitor.GetHashCode();
		}

		internal T GetWrappedMonitor<T>() where T : CacheableResourceHealthMonitor
		{
			this.CheckExpired();
			return this.wrappedMonitor as T;
		}

		protected internal void CheckExpired()
		{
			while (this.wrappedMonitor.Expired)
			{
				lock (this.instanceLock)
				{
					if (this.wrappedMonitor.Expired)
					{
						IResourceLoadMonitor resourceLoadMonitor = ResourceHealthMonitorManager.Singleton.Get(this.wrappedMonitor.Key);
						this.wrappedMonitor = (resourceLoadMonitor as ResourceHealthMonitorWrapper).wrappedMonitor;
					}
				}
				if (this.wrappedMonitor.Expired)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorWrapper.CheckExpired] Retrieved expired monitor from cache '{0}'.  Will try again.", this.wrappedMonitor.Key);
					Thread.Yield();
				}
			}
		}

		public DateTime LastUpdateUtc
		{
			get
			{
				this.CheckExpired();
				return this.wrappedMonitor.LastUpdateUtc;
			}
		}

		public ResourceLoad GetResourceLoad(WorkloadType type, bool raw = false, object optionalData = null)
		{
			this.CheckExpired();
			return this.wrappedMonitor.GetResourceLoad(type, raw, optionalData);
		}

		public ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null)
		{
			this.CheckExpired();
			return this.wrappedMonitor.GetResourceLoad(classification, raw, optionalData);
		}

		private CacheableResourceHealthMonitor wrappedMonitor;

		private object instanceLock = new object();
	}
}
