using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Complete", "OrganizationUpgrade", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class CompleteOrganizationUpgradeTask : ManageOrganizationUpgradeTaskBase
	{
		protected override void InitializeComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.Add("setup\\data\\DatacenterCompleteTenantUpgrade.xml");
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.CompleteOrganizationUpgradeDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageCompleteOrganizationUpgrade(base.Identity.ToString());
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new CompleteOrganizationUpgradeTaskModuleFactory();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.tenantCU.IsUpgradingOrganization)
			{
				base.WriteError(new OrganizationUpgradeException(Strings.ErrorOrganizationIsNotInUpgradeState), ExchangeErrorCategory.Client, base.Identity);
			}
			ServicePlan servicePlanSettings = ServicePlanConfiguration.GetInstance().GetServicePlanSettings(this.tenantCU.ProgramId, this.tenantCU.OfferId);
			base.InternalIsSharedConfigServicePlan = ServicePlanConfiguration.GetInstance().IsSharedConfigurationAllowedForServicePlan(servicePlanSettings);
			base.InternalLocalStaticConfigEnabled = !servicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled;
			base.InternalLocalHydrateableConfigEnabled = !servicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled;
			if (base.InternalIsSharedConfigServicePlan)
			{
				if (this.tenantCU.SharedConfigurationInfo != null)
				{
					base.WriteError(new SharedConfigurationValidationException(Strings.ErrorSharedConfigurationUpgradeNotSupported), ExchangeErrorCategory.Client, base.Identity);
					return;
				}
				base.InternalCreateSharedConfiguration = false;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (!base.HasErrors && base.InternalIsSharedConfigServicePlan && this.tenantCU.SupportedSharedConfigurations.Count != 0)
			{
				IConfigurationSession configurationSession = base.CreateTenantSession(this.tenantCU.OrganizationId, false, ConsistencyMode.PartiallyConsistent);
				this.tenantCU = configurationSession.Read<ExchangeConfigurationUnit>(this.tenantCU.Id);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(this.tenantCU.Id), 134, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\CompleteOrganizationUpgradeTask.cs");
				Result<ExchangeConfigurationUnit>[] array = tenantConfigurationSession.ReadMultiple<ExchangeConfigurationUnit>(this.tenantCU.SupportedSharedConfigurations.ToArray());
				ServerVersion serverVersion = new ServerVersion(ServerVersion.InstalledVersion.Major, ServerVersion.InstalledVersion.Minor, ServerVersion.InstalledVersion.Build, 0);
				bool flag = false;
				foreach (Result<ExchangeConfigurationUnit> result in array)
				{
					if (result.Data != null)
					{
						ExchangeConfigurationUnit data = result.Data;
						ServerVersion serverVersion2 = new ServerVersion(data.SharedConfigurationInfo.CurrentVersion.Major, data.SharedConfigurationInfo.CurrentVersion.Minor, data.SharedConfigurationInfo.CurrentVersion.Build, 0);
						if (serverVersion2.Major < serverVersion.Major || (ServerVersion.Compare(serverVersion2, serverVersion) < 0 && this.tenantCU.SupportedSharedConfigurations.Count > 1 && !this.TryGetServerForRoleAndVersion(ServerRole.Mailbox, serverVersion2)))
						{
							this.tenantCU.SupportedSharedConfigurations.Remove(data.Id);
							flag = true;
						}
					}
				}
				if (flag)
				{
					configurationSession.Save(this.tenantCU);
				}
			}
			TaskLogger.LogExit();
		}

		private bool TryGetServerForRoleAndVersion(ServerRole serverRole, ServerVersion version)
		{
			QueryFilter queryFilter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)serverRole));
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.AdminDisplayVersion, version);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 199, "TryGetServerForRoleAndVersion", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\CompleteOrganizationUpgradeTask.cs");
			MiniServer[] array = topologyConfigurationSession.Find<MiniServer>(null, QueryScope.SubTree, filter, null, 1, null);
			return array != null && array.Length > 0;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is ProvisioningMailboxNotFoundException || exception is MultipleProvisioningMailboxesException || exception is CsvValidationException || exception is StorageTransientException || exception is StoragePermanentException || exception is MigrationTransientException || exception is MigrationPermanentException || exception is MigrationDataCorruptionException || exception is BulkProvisioningTransientException || base.IsKnownException(exception);
		}
	}
}
