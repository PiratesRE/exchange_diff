using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class OrganizationMailboxDatabaseCache : DisposeTrackableBase
	{
		public OrganizationMailboxDatabaseCache(TransportAppConfig.PerTenantCacheConfig settings, ProcessTransportRole processTransportRole)
		{
			this.cache = new TenantConfigurationCache<PerTenantOrganizationMailboxDatabases>((long)settings.OrganizationMailboxDatabaseCacheMaxSize.ToBytes(), settings.OrganizationMailboxDatabaseCacheExpiryInterval, settings.OrganizationMailboxDatabaseCacheCleanupInterval, new PerTenantCacheTracer(ProxyHubSelectorComponent.Tracer, "OrganizationMailboxDatabases"), new PerTenantCachePerformanceCounters(processTransportRole, "OrganizationMailboxDatabases"));
		}

		protected OrganizationMailboxDatabaseCache()
		{
		}

		public virtual bool TryGetOrganizationMailboxDatabases(OrganizationId organizationId, out IList<ADObjectId> databaseIds)
		{
			databaseIds = null;
			if (this.cache == null)
			{
				throw new ObjectDisposedException("OrganizationMailboxDatabaseCache has been disposed");
			}
			PerTenantOrganizationMailboxDatabases perTenantOrganizationMailboxDatabases;
			if (!this.cache.TryGetValue(organizationId, out perTenantOrganizationMailboxDatabases))
			{
				return false;
			}
			databaseIds = perTenantOrganizationMailboxDatabases.Databases;
			return true;
		}

		public virtual void Clear()
		{
			if (this.cache != null)
			{
				this.cache.Clear();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.cache != null)
			{
				this.cache.Dispose();
				this.cache = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OrganizationMailboxDatabaseCache>(this);
		}

		private TenantConfigurationCache<PerTenantOrganizationMailboxDatabases> cache;
	}
}
