using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal static class ADHelper
	{
		internal static OrganizationId ResolveOrganization(OrganizationIdParameter organization, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			if (organization == null)
			{
				return null;
			}
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, currentOrganizationId, executingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 48, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FfoReporting\\Common\\ADHelper.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = null;
			LocalizedString? localizedString = null;
			IEnumerable<ADOrganizationalUnit> objects = organization.GetObjects<ADOrganizationalUnit>(null, tenantOrTopologyConfigurationSession, null, out localizedString);
			using (IEnumerator<ADOrganizationalUnit> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(organization.ToString()));
				}
				adorganizationalUnit = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorOrganizationNotUnique(organization.ToString()));
				}
			}
			return adorganizationalUnit.OrganizationId;
		}

		internal static IConfigDataProvider CreateConfigSession(OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(ScopeSet.GetOrgWideDefaultScopeSet(currentOrganizationId), ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), currentOrganizationId, executingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 102, "CreateConfigSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FfoReporting\\Common\\ADHelper.cs");
		}

		internal static Guid GetExternalDirectoryOrganizationId(OrganizationIdParameter organization, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			OrganizationId organizationId = ADHelper.ResolveOrganization(organization, currentOrganizationId, executingUserOrganizationId);
			return ADHelper.GetExternalDirectoryOrganizationId(organizationId);
		}

		internal static Guid GetExternalDirectoryOrganizationId(OrganizationId organizationId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 139, "GetExternalDirectoryOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FfoReporting\\Common\\ADHelper.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
			if (exchangeConfigurationUnit == null)
			{
				throw new ArgumentException("External configuration unit is null.");
			}
			if (string.IsNullOrEmpty(exchangeConfigurationUnit.ExternalDirectoryOrganizationId))
			{
				throw new ArgumentException("External directory organization is either null or empty.");
			}
			return Guid.Parse(exchangeConfigurationUnit.ExternalDirectoryOrganizationId);
		}
	}
}
