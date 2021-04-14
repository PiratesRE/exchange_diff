using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationFederatedOrganizationIdCache : OrganizationBaseCache
	{
		public OrganizationFederatedOrganizationIdCache(OrganizationId organizationId, IConfigurationSession session) : base(organizationId, session)
		{
		}

		public FederatedOrganizationId Value
		{
			get
			{
				this.PopulateCacheIfNeeded();
				return this.value;
			}
		}

		private void PopulateCacheIfNeeded()
		{
			if (!this.cached)
			{
				OrganizationBaseCache.Tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Cache miss, get the FederatedOrganizationId id for: {0}", base.OrganizationId);
				this.value = base.Session.GetFederatedOrganizationId(base.Session.SessionSettings.CurrentOrganizationId);
				this.cached = true;
			}
		}

		private FederatedOrganizationId value;

		private bool cached;
	}
}
