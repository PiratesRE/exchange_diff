using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "RootOrganizationContainer")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallRootOrganizationContainer : NewFixedNameSystemConfigurationObjectTask<Organization>
	{
		protected override IConfigDataProvider CreateSession()
		{
			if (Datacenter.IsMultiTenancyEnabled())
			{
				return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 47, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallRootOrganizationContainer.cs");
			}
			return base.CreateSession();
		}

		protected override void InternalValidate()
		{
		}

		protected override void InternalProcessRecord()
		{
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(base.DomainController, null);
			Organization organization = topologyConfigurationSession.Read<Organization>(rootOrgContainerId);
			if (organization == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(rootOrgContainerId)), ErrorCategory.ObjectNotFound, rootOrgContainerId);
			}
			if (organization.ExchangeVersion == ExchangeObjectVersion.Exchange2003 && !topologyConfigurationSession.HasAnyServer())
			{
				base.WriteVerbose(Strings.VerboseBumpOrganizationExchangeVersion(organization.Identity.ToString(), organization.ExchangeVersion.ToString(), Organization.CurrentExchangeRootOrgVersion.ToString()));
				organization.SetExchangeVersion(Organization.CurrentExchangeRootOrgVersion);
				organization[ADLegacyVersionableObjectSchema.MinAdminVersion] = Organization.CurrentExchangeRootOrgVersion.ExchangeBuild.ToExchange2003FormatInt32();
				if (Datacenter.IsMicrosoftHostedOnly(true))
				{
					organization[OrganizationSchema.ForestMode] = ForestModeFlags.TenantConfigInDomainNC;
				}
				topologyConfigurationSession.Save(organization);
			}
		}
	}
}
