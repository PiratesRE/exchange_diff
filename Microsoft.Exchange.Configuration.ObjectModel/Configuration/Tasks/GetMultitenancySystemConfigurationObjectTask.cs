using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetMultitenancySystemConfigurationObjectTask<TIdentity, TDataObject> : GetSystemConfigurationObjectTask<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
	{
		[Parameter]
		public virtual OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		internal SharedConfiguration SharedConfiguration
		{
			get
			{
				return this.sharedConfig;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.SharedConfiguration != null)
				{
					return this.SharedConfiguration.SharedConfigurationCU.Id;
				}
				if (this.organization != null)
				{
					return this.organization.ConfigurationUnit;
				}
				return base.RootId;
			}
		}

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 630, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\GetAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				this.organization = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return this.organization.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (this.SharedConfiguration != null)
			{
				return base.CreateConfigurationSession(this.SharedConfiguration.GetSharedConfigurationSessionSettings());
			}
			return base.CreateConfigurationSession(sessionSettings);
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.SharedConfiguration != null)
			{
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, this.SharedConfiguration.GetSharedConfigurationSessionSettings(), 694, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\GetAdObjectTask.cs");
			}
			return base.CreateSession();
		}

		protected override void InternalStateReset()
		{
			base.CheckExclusiveParameters(new object[]
			{
				"AccountPartition",
				"Organization"
			});
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.SharedTenantConfigurationMode == SharedTenantConfigurationMode.Static || (this.SharedTenantConfigurationMode == SharedTenantConfigurationMode.Dehydrateable && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId)))
			{
				this.sharedConfig = SharedConfiguration.GetSharedConfiguration(base.CurrentOrganizationId);
			}
			TaskLogger.LogExit();
		}

		protected const string ParameterSetOrganization = "OrganizationSet";

		private ADOrganizationalUnit organization;

		private SharedConfiguration sharedConfig;
	}
}
