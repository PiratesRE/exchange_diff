using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class IOCConfigurationTask : SessionTask
	{
		public IOCConfigurationTask() : base(HybridStrings.IOCConfigurationTaskName, 2)
		{
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.HybridInfoBasePrereqsFailed));
				return false;
			}
			return true;
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			return !Configuration.RestrictIOCToSP1OrGreater(taskContext.HybridConfigurationObject.ServiceInstance) || (taskContext.OnPremisesSession.GetIntraOrganizationConfiguration().DeploymentIsCompleteIOCReady ?? false);
		}

		public override bool Configure(ITaskContext taskContext)
		{
			if (!base.Configure(taskContext))
			{
				return false;
			}
			string intraOrganizationConnectorName = this.GetIntraOrganizationConnectorName();
			IntraOrganizationConnector intraOrganizationConnector = taskContext.OnPremisesSession.GetIntraOrganizationConnector(intraOrganizationConnectorName);
			if (intraOrganizationConnector != null)
			{
				taskContext.OnPremisesSession.RemoveIntraOrganizationConnector(intraOrganizationConnectorName);
			}
			IntraOrganizationConnector intraOrganizationConnector2 = taskContext.TenantSession.GetIntraOrganizationConnector(intraOrganizationConnectorName);
			if (intraOrganizationConnector2 != null)
			{
				taskContext.TenantSession.RemoveIntraOrganizationConnector(intraOrganizationConnectorName);
			}
			IntraOrganizationConfiguration intraOrganizationConfiguration = taskContext.OnPremisesSession.GetIntraOrganizationConfiguration();
			IntraOrganizationConfiguration intraOrganizationConfiguration2 = taskContext.TenantSession.GetIntraOrganizationConfiguration();
			taskContext.OnPremisesSession.NewIntraOrganizationConnector(this.GetIntraOrganizationConnectorName(), intraOrganizationConfiguration2.OnlineDiscoveryEndpoint.ToString(), intraOrganizationConfiguration2.OnlineTargetAddress, true);
			taskContext.TenantSession.NewIntraOrganizationConnector(this.GetIntraOrganizationConnectorName(), intraOrganizationConfiguration.OnPremiseDiscoveryEndpoint.ToString(), intraOrganizationConfiguration2.OnPremiseTargetAddresses, true);
			if (!taskContext.Parameters.Get<bool>("_suppressOAuthWarning"))
			{
				base.AddLocalizedStringWarning(HybridStrings.WarningOAuthNeedsConfiguration(Configuration.OAuthConfigurationUrl(taskContext.HybridConfigurationObject.ServiceInstance)));
			}
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			if (!base.ValidateConfiguration(taskContext))
			{
				return false;
			}
			string intraOrganizationConnectorName = this.GetIntraOrganizationConnectorName();
			IntraOrganizationConnector intraOrganizationConnector = taskContext.OnPremisesSession.GetIntraOrganizationConnector(intraOrganizationConnectorName);
			IntraOrganizationConnector intraOrganizationConnector2 = taskContext.TenantSession.GetIntraOrganizationConnector(intraOrganizationConnectorName);
			return intraOrganizationConnector != null && intraOrganizationConnector2 != null && string.Equals(intraOrganizationConnector.Name, intraOrganizationConnectorName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(intraOrganizationConnector2.Name, intraOrganizationConnectorName, StringComparison.InvariantCultureIgnoreCase);
		}

		private IOrganizationConfig GetOnPremOrgConfig()
		{
			if (this.onPremOrgConfig == null)
			{
				this.onPremOrgConfig = base.OnPremisesSession.GetOrganizationConfig();
			}
			return this.onPremOrgConfig;
		}

		private string GetIntraOrganizationConnectorName()
		{
			return string.Format("HybridIOC - {0}", this.GetOnPremOrgConfig().Guid.ToString());
		}

		private const string IOCNameTemplate = "HybridIOC - {0}";

		private IOrganizationConfig onPremOrgConfig;
	}
}
