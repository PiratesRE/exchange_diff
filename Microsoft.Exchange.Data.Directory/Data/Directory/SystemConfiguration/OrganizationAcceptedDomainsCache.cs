using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationAcceptedDomainsCache : OrganizationBaseCache
	{
		public OrganizationAcceptedDomainsCache(OrganizationId organizationId, IConfigurationSession session) : base(organizationId, session)
		{
		}

		public Dictionary<string, AuthenticationType> Value
		{
			get
			{
				this.PopulateCacheIfNeeded();
				return this.namespaceAuthenticationTypeHash;
			}
		}

		private void PopulateCacheIfNeeded()
		{
			if (!this.cached)
			{
				this.cached = this.PopulateCache();
			}
		}

		private bool PopulateCache()
		{
			OrganizationBaseCache.Tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Searching for AcceptedDomain instances associated with OrganizationId '{0}'", base.OrganizationId);
			AcceptedDomain[] acceptedDomains = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				acceptedDomains = ((ITenantConfigurationSession)this.Session).FindAllAcceptedDomainsInOrg(this.OrganizationId.ConfigurationUnit);
			});
			if (!adoperationResult.Succeeded)
			{
				OrganizationBaseCache.Tracer.TraceError<OrganizationId, Exception>((long)this.GetHashCode(), "Unable to find AcceptedDomain instances associated with the OrganizationId '{0}' due to exception: {1}", base.OrganizationId, adoperationResult.Exception);
				throw adoperationResult.Exception;
			}
			if (acceptedDomains == null || acceptedDomains.Length == 0)
			{
				OrganizationBaseCache.Tracer.TraceError<OrganizationId>((long)this.GetHashCode(), "Unable to find any AcceptedDomain associated with the OrganizationId '{0}'", base.OrganizationId);
				return adoperationResult.Succeeded;
			}
			Dictionary<string, AuthenticationType> dictionary = new Dictionary<string, AuthenticationType>(acceptedDomains.Length, StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < acceptedDomains.Length; i++)
			{
				dictionary[acceptedDomains[i].DomainName.Domain] = acceptedDomains[i].RawAuthenticationType;
			}
			this.namespaceAuthenticationTypeHash = dictionary;
			OrganizationBaseCache.Tracer.TraceDebug<EnumerableTracer<string>>((long)this.GetHashCode(), "Found the following Accepted Domains: {0}", new EnumerableTracer<string>(this.namespaceAuthenticationTypeHash.Keys));
			return adoperationResult.Succeeded;
		}

		private Dictionary<string, AuthenticationType> namespaceAuthenticationTypeHash;

		private bool cached;
	}
}
