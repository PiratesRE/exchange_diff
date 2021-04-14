using System;
using System.Collections.Generic;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WlmResourceCache<TResource> where TResource : ResourceBase
	{
		public WlmResourceCache(Func<Guid, WorkloadType, TResource> createResourceDelegate)
		{
			this.createResourceDelegate = createResourceDelegate;
			this.resourceCaches = new Dictionary<WorkloadType, ResourceCache<TResource>>(4);
		}

		public void ForEach(Action<TResource> action)
		{
			lock (this.locker)
			{
				foreach (ResourceCache<TResource> resourceCache in this.resourceCaches.Values)
				{
					foreach (TResource obj2 in resourceCache.Values)
					{
						action(obj2);
					}
				}
			}
		}

		public TResource GetInstance(Guid resourceID, WorkloadType workloadType)
		{
			ResourceCache<TResource> resourceCache;
			lock (this.locker)
			{
				if (!this.resourceCaches.TryGetValue(workloadType, out resourceCache))
				{
					resourceCache = new ResourceCache<TResource>((Guid id) => this.createResourceDelegate(id, workloadType));
					this.resourceCaches.Add(workloadType, resourceCache);
				}
			}
			return resourceCache.GetInstance(resourceID);
		}

		private readonly Dictionary<WorkloadType, ResourceCache<TResource>> resourceCaches;

		private readonly object locker = new object();

		private Func<Guid, WorkloadType, TResource> createResourceDelegate;
	}
}
