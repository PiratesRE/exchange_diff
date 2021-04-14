using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OrganizationIdCacheValue
	{
		public OrganizationIdCacheValue(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, organizationId, organizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, sessionSettings, 53, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\OrganizationIdCacheValue.cs");
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, organizationId, organizationId, false);
			adsessionSettings.IsSharedConfigChecked = true;
			IConfigurationSession tenantOrTopologyConfigurationSession2 = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, adsessionSettings, 68, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\OrganizationIdCacheValue.cs");
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 72, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\OrganizationIdCacheValue.cs");
			this.organizationId = organizationId;
			this.federatedOrganizationIdCache = new OrganizationFederatedOrganizationIdCache(organizationId, tenantOrTopologyConfigurationSession2);
			this.federatedDomainsCache = new OrganizationFederatedDomainsCache(organizationId, this.federatedOrganizationIdCache, tenantOrTopologyConfigurationSession);
			this.acceptedDomainsCache = new OrganizationAcceptedDomainsCache(organizationId, tenantOrTopologyConfigurationSession);
			this.organizationRelationshipNonAdPropertiesCache = new OrganizationOrganizationRelationshipCache(organizationId, tenantOrTopologyConfigurationSession);
			this.availabilityConfigCache = new OrganizationAvailabilityConfigCache(organizationId, tenantOrTopologyConfigurationSession, tenantOrRootOrgRecipientSession);
			this.availabilityAddressSpaceCache = new OrganizationAvailabilityAddressSpaceCache(organizationId, tenantOrTopologyConfigurationSession);
			this.intraOrganizationConnectorCache = new OrganizationIntraOrganizationConnectorCache(organizationId, tenantOrTopologyConfigurationSession);
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public FederatedOrganizationId FederatedOrganizationId
		{
			get
			{
				return this.federatedOrganizationIdCache.Value;
			}
		}

		public IEnumerable<string> FederatedDomains
		{
			get
			{
				return this.federatedDomainsCache.Value;
			}
		}

		public string DefaultFederatedDomain
		{
			get
			{
				return this.federatedDomainsCache.DefaultDomain;
			}
		}

		public OrganizationRelationship GetOrganizationRelationship(string domain)
		{
			return this.organizationRelationshipNonAdPropertiesCache.Get(domain);
		}

		public IntraOrganizationConnector GetIntraOrganizationConnector(string domain)
		{
			return this.intraOrganizationConnectorCache.Get(domain);
		}

		public IDictionary<string, AuthenticationType> NamespaceAuthenticationTypeHash
		{
			get
			{
				return this.acceptedDomainsCache.Value;
			}
		}

		public AuthenticationType GetNamespaceAuthenticationType(string domain)
		{
			AuthenticationType result;
			if (this.NamespaceAuthenticationTypeHash.TryGetValue(domain, out result))
			{
				return result;
			}
			return AuthenticationType.Managed;
		}

		public ADRecipient GetAvailabilityConfigOrgWideAccount()
		{
			return this.availabilityConfigCache.GetOrgWideAccountObject();
		}

		public ADRecipient GetAvailabilityConfigPerUserAccount()
		{
			return this.availabilityConfigCache.GetPerUserAccountObject();
		}

		public AvailabilityAddressSpace GetAvailabilityAddressSpace(string domain)
		{
			return this.availabilityAddressSpaceCache.Get(domain);
		}

		private OrganizationId organizationId;

		private OrganizationFederatedOrganizationIdCache federatedOrganizationIdCache;

		private OrganizationFederatedDomainsCache federatedDomainsCache;

		private OrganizationAcceptedDomainsCache acceptedDomainsCache;

		private OrganizationOrganizationRelationshipCache organizationRelationshipNonAdPropertiesCache;

		private OrganizationAvailabilityConfigCache availabilityConfigCache;

		private OrganizationAvailabilityAddressSpaceCache availabilityAddressSpaceCache;

		private OrganizationIntraOrganizationConnectorCache intraOrganizationConnectorCache;
	}
}
