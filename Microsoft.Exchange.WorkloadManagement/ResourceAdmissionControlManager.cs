using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ResourceAdmissionControlManager : LazyLookupExactTimeoutCache<ResourceKey, IResourceAdmissionControl>
	{
		public ResourceAdmissionControlManager() : this(null)
		{
		}

		public ResourceAdmissionControlManager(ResourceAvailabilityChangeDelegate availabilityChangeDelegate) : this(availabilityChangeDelegate, null)
		{
			this.availabilityChangeDelegate = availabilityChangeDelegate;
		}

		public ResourceAdmissionControlManager(ResourceAvailabilityChangeDelegate availabilityChangeDelegate, Func<ResourceAdmissionControlManager, ResourceKey, IResourceAdmissionControl> initializationDelegate) : this(availabilityChangeDelegate, initializationDelegate, TimeSpan.FromMinutes(10.0))
		{
		}

		public ResourceAdmissionControlManager(ResourceAvailabilityChangeDelegate availabilityChangeDelegate, Func<ResourceAdmissionControlManager, ResourceKey, IResourceAdmissionControl> initializationDelegate, TimeSpan unusedAdmissionExpiration) : base(10000, false, unusedAdmissionExpiration, TimeSpan.MaxValue, CacheFullBehavior.FailNew)
		{
			this.availabilityChangeDelegate = availabilityChangeDelegate;
			this.initializationDelegate = initializationDelegate;
		}

		protected override bool HandleShouldRemove(ResourceKey resourceKey, IResourceAdmissionControl admissionControl)
		{
			return !admissionControl.IsAcquired;
		}

		protected override IResourceAdmissionControl CreateOnCacheMiss(ResourceKey resourceKey, ref bool shouldAdd)
		{
			if (this.initializationDelegate != null)
			{
				return this.initializationDelegate(this, resourceKey);
			}
			return new DefaultAdmissionControl(resourceKey, delegate(ResourceKey key)
			{
				base.Remove(key);
			}, this.availabilityChangeDelegate, "ResourceAdmissionControlManager");
		}

		private const string AdmissionControlOwner = "ResourceAdmissionControlManager";

		private const int ArbitraryCacheLimit = 10000;

		private ResourceAvailabilityChangeDelegate availabilityChangeDelegate;

		private Func<ResourceAdmissionControlManager, ResourceKey, IResourceAdmissionControl> initializationDelegate;
	}
}
