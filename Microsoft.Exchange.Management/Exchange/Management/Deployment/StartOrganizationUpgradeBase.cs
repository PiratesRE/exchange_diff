using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public abstract class StartOrganizationUpgradeBase : ManageOrganizationUpgradeTaskBase
	{
		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.servicePlanSettings = null;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			Exception ex = null;
			try
			{
				if (string.IsNullOrEmpty(this.tenantCU.ServicePlan))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanIsNotSet), (ErrorCategory)1002, null);
				}
				if (ServicePlanConfiguration.IsDeprecatedServicePlan(this.tenantCU.ServicePlan))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanIsDeprecated(this.tenantCU.Name, this.tenantCU.ServicePlan, this.tenantCU.ProgramId)), (ErrorCategory)1002, null);
				}
				this.servicePlanSettings = ServicePlanConfiguration.GetInstance().GetServicePlanSettings(this.tenantCU.ServicePlan);
				base.InternalLocalStaticConfigEnabled = !this.servicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled;
				base.InternalLocalHydrateableConfigEnabled = !this.servicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled;
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				base.WriteError(ex, (ErrorCategory)1000, null);
			}
			if (!string.IsNullOrEmpty(this.tenantCU.TargetServicePlan))
			{
				string value = ServicePlanConfiguration.GetInstance().ResolveServicePlanName(this.tenantCU.ProgramId, this.TargetOfferId);
				if (!this.tenantCU.TargetServicePlan.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorOOBUpgradeInProgress(this.tenantCU.ServicePlan, this.tenantCU.TargetServicePlan)), (ErrorCategory)1002, null);
				}
			}
			base.InternalIsSharedConfigServicePlan = ServicePlanConfiguration.GetInstance().IsSharedConfigurationAllowedForServicePlan(this.tenantCU.ProgramId, this.tenantCU.OfferId);
			if (base.InternalIsSharedConfigServicePlan)
			{
				if (this.tenantCU.SharedConfigurationInfo == null)
				{
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(this.tenantCU.Id), 210, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\StartOrganizationUpgradeTask.cs");
					Result<ExchangeConfigurationUnit>[] array = tenantConfigurationSession.ReadMultiple<ExchangeConfigurationUnit>(this.tenantCU.SupportedSharedConfigurations.ToArray());
					string offerId;
					if (!ServicePlanConfiguration.GetInstance().TryGetHydratedOfferId(this.tenantCU.ProgramId, this.tenantCU.OfferId, out offerId))
					{
						offerId = this.tenantCU.OfferId;
					}
					SharedConfigurationInfo sharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(this.tenantCU.ProgramId, offerId);
					foreach (Result<ExchangeConfigurationUnit> result in array)
					{
						if (result.Data != null)
						{
							ExchangeConfigurationUnit data = result.Data;
							if (data.SharedConfigurationInfo.Equals(sharedConfigurationInfo))
							{
								base.InternalSharedConfigurationId = data.OrganizationId;
								break;
							}
						}
					}
					if (base.InternalSharedConfigurationId == null)
					{
						base.InternalSharedConfigurationId = SharedConfiguration.FindOneSharedConfigurationId(sharedConfigurationInfo, base.CurrentOrganizationId.PartitionId);
					}
					if (base.InternalSharedConfigurationId == null)
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.ErrorSharedConfigurationNotFound(this.tenantCU.ProgramId, offerId, sharedConfigurationInfo.CurrentVersion.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				else
				{
					base.WriteError(new SharedConfigurationValidationException(Strings.ErrorSharedConfigurationUpgradeNotSupported), ExchangeErrorCategory.Client, null);
				}
			}
			Exception ex4 = Utils.ValidateTransportRuleRegexesForMigratingTenants(this.tenantCU.OrganizationId);
			if (ex4 != null)
			{
				base.WriteError(new SharedConfigurationValidationException(Strings.ErrorE14TenantRulesNeedUpdateBeforeMigratingToE15(ex4.Message)), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			if (this.servicePlanSettings != null)
			{
				this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.ServicePlanSettingsVarName, this.servicePlanSettings);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.tenantCU.ObjectVersion < Organization.OrgConfigurationVersion)
			{
				base.WriteVerbose(Strings.VerboseWillStartOrganizationUpgrade(base.Identity.ToString(), this.tenantCU.ObjectVersion, Organization.OrgConfigurationVersion));
			}
			base.Fields[RemoveOrganization.ExternalDirectoryOrganizationIdVarName] = this.tenantCU.ExternalDirectoryOrganizationId;
			base.Fields["UpdateSupportedSharedConfigurations"] = (base.InternalIsSharedConfigServicePlan && this.tenantCU.SharedConfigurationInfo == null && base.InternalSharedConfigurationId != null && !this.tenantCU.SupportedSharedConfigurations.Contains(base.InternalSharedConfigurationId.ConfigurationUnit));
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private ServicePlan servicePlanSettings;
	}
}
