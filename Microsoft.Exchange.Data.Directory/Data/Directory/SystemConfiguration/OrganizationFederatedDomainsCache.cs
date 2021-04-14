using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationFederatedDomainsCache : OrganizationBaseCache
	{
		public OrganizationFederatedDomainsCache(OrganizationId organizationId, OrganizationFederatedOrganizationIdCache federatedOrganizationIdCache, IConfigurationSession session) : base(organizationId, session)
		{
			this.federatedOrganizationIdCache = federatedOrganizationIdCache;
		}

		public IEnumerable<string> Value
		{
			get
			{
				this.PopulateCacheIfNeeded();
				return this.value;
			}
		}

		public string DefaultDomain
		{
			get
			{
				return this.defaultDomain;
			}
		}

		private void PopulateCacheIfNeeded()
		{
			if (!this.cached)
			{
				this.Populate();
				this.cached = true;
			}
		}

		private void Populate()
		{
			FederatedOrganizationId federatedOrganizationId = this.federatedOrganizationIdCache.Value;
			if (federatedOrganizationId == null)
			{
				OrganizationBaseCache.Tracer.TraceError<OrganizationId>((long)this.GetHashCode(), "Unable to find the FederatedOrganizationId associated with the organization '{0}'", base.OrganizationId);
				return;
			}
			OrganizationBaseCache.Tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Searching for AcceptedDomain instances associated with FederatedOrganizationId '{0}'", federatedOrganizationId.Id);
			AcceptedDomain[] acceptedDomains = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				acceptedDomains = this.Session.FindAcceptedDomainsByFederatedOrgId(federatedOrganizationId);
			});
			if (!adoperationResult.Succeeded)
			{
				OrganizationBaseCache.Tracer.TraceError<ADObjectId, Exception>((long)this.GetHashCode(), "Unable to find AcceptedDomain instances associated with the FederatedOrganizationId '{0}' due to exception: {1}", federatedOrganizationId.Id, adoperationResult.Exception);
				return;
			}
			if (acceptedDomains == null || acceptedDomains.Length == 0)
			{
				OrganizationBaseCache.Tracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Unable to find any federated AcceptedDomain associated with the FederatedOrganizationId '{0}'", federatedOrganizationId.Id);
				return;
			}
			string[] array = new string[acceptedDomains.Length];
			for (int i = 0; i < acceptedDomains.Length; i++)
			{
				array[i] = acceptedDomains[i].DomainName.Domain;
			}
			string text = null;
			foreach (AcceptedDomain acceptedDomain in acceptedDomains)
			{
				if (acceptedDomain.IsDefaultFederatedDomain)
				{
					OrganizationBaseCache.Tracer.TraceDebug<SmtpDomainWithSubdomains>((long)this.GetHashCode(), "Found AcceptedDomain '{0}' as default federated domain", acceptedDomain.DomainName);
					text = acceptedDomain.DomainName.Domain;
					break;
				}
			}
			OrganizationBaseCache.Tracer.TraceDebug<ADObjectId, ArrayTracer<string>>((long)this.GetHashCode(), "Found the following domains associated with FederatedOrganizationId '{0}': {1}", federatedOrganizationId.Id, new ArrayTracer<string>(array));
			this.value = array;
			this.defaultDomain = text;
		}

		private OrganizationFederatedOrganizationIdCache federatedOrganizationIdCache;

		private string[] value;

		private bool cached;

		private string defaultDomain;
	}
}
