using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoSessionSettingsFactory : ADSessionSettings.SessionSettingsFactory
	{
		internal override ADSessionSettings FromAllTenantsPartitionId(PartitionId partitionId)
		{
			return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(OrganizationId.ForestWideOrgId), null, OrganizationId.ForestWideOrgId, null, ConfigScopes.AllTenants, PartitionId.LocalForest);
		}

		internal override ADSessionSettings FromAllTenantsObjectId(ADObjectId id)
		{
			return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(OrganizationId.ForestWideOrgId), id, OrganizationId.ForestWideOrgId, null, ConfigScopes.AllTenants, PartitionId.LocalForest);
		}

		internal override ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(ADObjectId id)
		{
			if (id.DomainId == null)
			{
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			if (!ADSessionSettings.IsForefrontObject(id))
			{
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			return ADSessionSettings.FromAllTenantsObjectId(id);
		}

		internal override ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(OrganizationId orgId)
		{
			if (!OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(OrganizationId.ForestWideOrgId), null, orgId, null, ConfigScopes.AllTenants, PartitionId.LocalForest);
			}
			return ADSessionSettings.FromRootOrgScopeSet();
		}

		internal override ADSessionSettings FromTenantPartitionHint(TenantPartitionHint partitionHint)
		{
			throw new NotImplementedException();
		}

		internal override ADSessionSettings FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			ADObjectId adobjectId = new ADObjectId(DalHelper.GetTenantDistinguishedName(externalDirectoryOrganizationId.ToString()), externalDirectoryOrganizationId);
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			adpropertyBag[ADObjectSchema.ConfigurationUnit] = adobjectId;
			adpropertyBag[ADObjectSchema.OrganizationalUnitRoot] = adobjectId;
			OrganizationId organizationId = (OrganizationId)ADObject.OrganizationIdGetter(adpropertyBag);
			return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(organizationId), adobjectId, organizationId, null, ConfigScopes.TenantLocal, PartitionId.LocalForest);
		}

		internal override ADSessionSettings FromTenantForestAndCN(string exoAccountForest, string exoTenantContainer)
		{
			throw new NotImplementedException();
		}

		internal override ADSessionSettings FromTenantCUName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			FfoTenant tenantByName = globalConfigSession.GetTenantByName(name);
			if (tenantByName == null)
			{
				throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantName(name));
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = FfoConfigurationSession.GetExchangeConfigurationUnit(tenantByName);
			return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(OrganizationId.ForestWideOrgId), null, exchangeConfigurationUnit.OrganizationId, null, ConfigScopes.TenantLocal, PartitionId.LocalForest);
		}

		internal override ADSessionSettings FromTenantAcceptedDomain(string domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string>((long)domain.GetHashCode(), "FromTenantAcceptedDomain(): Resolving domainName '{0}' into partition", domain);
			string text = null;
			Guid empty = Guid.Empty;
			ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(domain, out text, out empty);
			if (empty != Guid.Empty)
			{
				return ADSessionSettings.SessionSettingsFactory.Default.FromExternalDirectoryOrganizationId(empty);
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string>((long)domain.GetHashCode(), "FromTenantAcceptedDomain(): Failed to resolve domainName '{0}' to partition", domain);
			throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(domain));
		}

		internal override ADSessionSettings FromTenantMSAUser(string msaUserNetID)
		{
			throw new CannotResolveMSAUserNetIDException(DirectoryStrings.CannotFindTenantByMSAUserNetID(msaUserNetID));
		}

		internal override bool InDomain()
		{
			if (this.inDomain == null)
			{
				try
				{
					NativeHelpers.GetDomainName();
					this.inDomain = new bool?(true);
				}
				catch (CannotGetDomainInfoException)
				{
					this.inDomain = new bool?(false);
				}
			}
			return this.inDomain.Value;
		}

		protected override OrganizationId RehomeScopingOrganizationIdIfNeeded(OrganizationId currentOrganizationId)
		{
			return currentOrganizationId;
		}

		private bool? inDomain;
	}
}
