using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetMultitenancySingletonSystemConfigurationObjectTask<TDataObject> : GetSingletonSystemConfigurationObjectTask<TDataObject> where TDataObject : ADObject, new()
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OrganizationIdParameter Identity
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

		internal SharedConfiguration SharedConfiguration
		{
			get
			{
				return this.sharedConfig;
			}
		}

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
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

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Identity != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 961, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\GetAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				this.organization = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Identity, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Identity.ToString())));
				return this.organization.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.SharedConfiguration != null)
			{
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, this.SharedConfiguration.GetSharedConfigurationSessionSettings(), 998, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\GetAdObjectTask.cs");
			}
			return base.CreateSession();
		}

		private ADOrganizationalUnit organization;

		private SharedConfiguration sharedConfig;
	}
}
