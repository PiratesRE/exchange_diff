using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationOrganizationRelationshipCache : OrganizationBaseCache
	{
		public OrganizationOrganizationRelationshipCache(OrganizationId organizationId, IConfigurationSession session) : base(organizationId, session)
		{
			this.cache = new Dictionary<string, OrganizationRelationship>(StringComparer.OrdinalIgnoreCase);
		}

		public OrganizationRelationship Get(string domain)
		{
			Dictionary<string, OrganizationRelationship> dictionary = this.cache;
			OrganizationRelationship organizationRelationship;
			lock (dictionary)
			{
				if (!dictionary.TryGetValue(domain, out organizationRelationship))
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "OrganizationRelationship cache miss for: {0} in organization {1}", domain, base.OrganizationId);
					organizationRelationship = base.Session.GetOrganizationRelationship(domain);
					dictionary.Add(domain, organizationRelationship);
					OrganizationBaseCache.Tracer.TraceDebug((long)this.GetHashCode(), "Cached OrganizationRelationship {0}", new object[]
					{
						(organizationRelationship == null) ? "<null>" : organizationRelationship.Id
					});
				}
				else
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, object>((long)this.GetHashCode(), "Found OrganizationRelationship in cache for domain {0}: {1}", domain, (organizationRelationship == null) ? "<null>" : organizationRelationship.Id);
				}
			}
			return organizationRelationship;
		}

		private Dictionary<string, OrganizationRelationship> cache;
	}
}
