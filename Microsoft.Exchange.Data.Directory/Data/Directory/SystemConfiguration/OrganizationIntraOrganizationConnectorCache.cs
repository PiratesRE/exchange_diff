using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationIntraOrganizationConnectorCache : OrganizationBaseCache
	{
		public OrganizationIntraOrganizationConnectorCache(OrganizationId organizationId, IConfigurationSession session) : base(organizationId, session)
		{
			this.cache = new Dictionary<string, IntraOrganizationConnector>(StringComparer.OrdinalIgnoreCase);
		}

		public IntraOrganizationConnector Get(string domainName)
		{
			Dictionary<string, IntraOrganizationConnector> dictionary = this.cache;
			IntraOrganizationConnector intraOrganizationConnector = null;
			lock (dictionary)
			{
				if (!dictionary.TryGetValue(domainName, out intraOrganizationConnector))
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "OrganizationRelationship cache miss for: {0} in organization {1}", domainName, base.OrganizationId);
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, IntraOrganizationConnectorSchema.TargetAddressDomains, domainName);
					IntraOrganizationConnector[] array = base.Session.Find<IntraOrganizationConnector>(IntraOrganizationConnector.GetContainerId(base.Session), QueryScope.SubTree, filter, null, 1);
					if (array.Length == 1)
					{
						intraOrganizationConnector = array[0];
					}
					dictionary.Add(domainName, intraOrganizationConnector);
					OrganizationBaseCache.Tracer.TraceDebug((long)this.GetHashCode(), "Cached IntraOrganizationConnector {0}", new object[]
					{
						(intraOrganizationConnector == null) ? "<null>" : intraOrganizationConnector.Id
					});
				}
				else
				{
					OrganizationBaseCache.Tracer.TraceDebug<string, object>((long)this.GetHashCode(), "Found IntraOrganizationConnector in cache for domain {0}: {1}", domainName, (intraOrganizationConnector == null) ? "<null>" : intraOrganizationConnector.Id);
				}
			}
			return intraOrganizationConnector;
		}

		private Dictionary<string, IntraOrganizationConnector> cache;
	}
}
