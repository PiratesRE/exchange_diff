using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationAvailabilityAddressSpaceCache : OrganizationBaseCache
	{
		public OrganizationAvailabilityAddressSpaceCache(OrganizationId organizationId, IConfigurationSession session) : base(organizationId, session)
		{
			this.cache = new Dictionary<string, AvailabilityAddressSpace>(StringComparer.OrdinalIgnoreCase);
		}

		public AvailabilityAddressSpace Get(string domain)
		{
			Dictionary<string, AvailabilityAddressSpace> dictionary = this.cache;
			AvailabilityAddressSpace availabilityAddressSpace;
			lock (dictionary)
			{
				if (!dictionary.TryGetValue(domain, out availabilityAddressSpace))
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "AvailabilityAddressSpace cache miss for: {0} in organization {1}", domain, base.OrganizationId);
					availabilityAddressSpace = base.Session.GetAvailabilityAddressSpace(domain);
					dictionary.Add(domain, availabilityAddressSpace);
					OrganizationBaseCache.Tracer.TraceDebug((long)this.GetHashCode(), "Cached AvailabilityAddressSpace {0}", new object[]
					{
						(availabilityAddressSpace == null) ? "<null>" : availabilityAddressSpace.Id
					});
				}
				else
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, object>((long)this.GetHashCode(), "Found AvailabilityAddressSpace in cache for domain {0}: {1}", domain, (availabilityAddressSpace == null) ? "<null>" : availabilityAddressSpace.Id);
				}
			}
			return availabilityAddressSpace;
		}

		private Dictionary<string, AvailabilityAddressSpace> cache;
	}
}
