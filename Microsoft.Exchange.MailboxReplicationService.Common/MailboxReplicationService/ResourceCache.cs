using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ResourceCache<TResource> : ExactTimeoutCache<Guid, TResource> where TResource : ResourceBase
	{
		public ResourceCache(Func<Guid, TResource> createResourceDelegate) : base(null, new ShouldRemoveDelegate<Guid, TResource>(ResourceCache<TResource>.ShouldRemoveResource), null, 10000, false)
		{
			this.createResourceDelegate = createResourceDelegate;
		}

		public void ForEach(Action<TResource> action)
		{
			lock (this.locker)
			{
				foreach (TResource obj2 in base.Values)
				{
					action(obj2);
				}
			}
		}

		public TResource GetInstance(Guid resourceID)
		{
			TResource tresource;
			lock (this.locker)
			{
				if (!base.TryGetValue(resourceID, out tresource))
				{
					tresource = this.createResourceDelegate(resourceID);
					base.TryInsertSliding(resourceID, tresource, ResourceCache<TResource>.Timeout);
				}
			}
			return tresource;
		}

		private static bool ShouldRemoveResource(Guid resourceID, TResource resource)
		{
			return resource.Utilization == 0;
		}

		private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5.0);

		private object locker = new object();

		private Func<Guid, TResource> createResourceDelegate;
	}
}
