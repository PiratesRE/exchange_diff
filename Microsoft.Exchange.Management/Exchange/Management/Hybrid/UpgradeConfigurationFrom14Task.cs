using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class UpgradeConfigurationFrom14Task : SessionTask
	{
		public UpgradeConfigurationFrom14Task(ExchangeObjectVersion currentVersion) : base(HybridStrings.HybridUpgradeFrom14TaskName, 1)
		{
			this.currentVersion = currentVersion;
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoHybridConfigurationObjectVersion(taskContext.HybridConfigurationObject.ExchangeVersion));
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoHybridConfigurationEngineVersion(this.currentVersion));
			bool flag = base.NeedsConfiguration(taskContext) || (taskContext.HybridConfigurationObject.ExchangeVersion.ExchangeBuild.Major == this.upgradeFrom.ExchangeBuild.Major && this.currentVersion.ExchangeBuild.Major == this.upgradeTo.ExchangeBuild.Major);
			if (!flag)
			{
				taskContext.Logger.LogInformation(HybridStrings.HybridInfoNoNeedToUpgrade);
			}
			return flag;
		}

		public override bool Configure(ITaskContext taskContext)
		{
			if (!base.Configure(taskContext))
			{
				return false;
			}
			this.PromptUserForPermissionToUpgrade(taskContext);
			this.EnsureAllTransportServersAreCurrent(taskContext);
			this.EnsureTenantHasAlreadyBeenUpgraded(taskContext);
			this.UpgradeFopeConnectors(taskContext);
			this.RemoveUnnecessaryRemoteDomains(taskContext);
			this.ClearUnusedConfigurationAttributes(taskContext);
			this.UpgradeHybridConfigurationObjectVersion(taskContext);
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			if (!base.ValidateConfiguration(taskContext))
			{
				return false;
			}
			this.ValidateFopeConnectorsAreUpgraded(taskContext);
			this.ValidateUnnecessaryRemoteDomainsAreRemoved(taskContext);
			this.ValidateUnusedConfigurationAttributesAreCleared(taskContext);
			this.ValidateHybridConfigurationObjectVersionUpgraded(taskContext);
			return true;
		}

		private void PromptUserForPermissionToUpgrade(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoCheckForPermissionToUpgrade);
			if (!taskContext.Parameters.Get<bool>("_forceUpgrade") && !taskContext.UI.ShouldContinue(HybridStrings.HybridUpgradePrompt))
			{
				throw new LocalizedException(HybridStrings.ErrorHybridMustBeUpgraded);
			}
		}

		private void EnsureAllTransportServersAreCurrent(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoVerifyTransportServers);
			List<ADObjectId> list = taskContext.HybridConfigurationObject.SendingTransportServers.ToList<ADObjectId>();
			list.AddRange(taskContext.HybridConfigurationObject.ReceivingTransportServers);
			foreach (ADObjectId adobjectId in list)
			{
				IExchangeServer exchangeServer = taskContext.OnPremisesSession.GetExchangeServer(adobjectId.Name);
				ServerVersion adminDisplayVersion = exchangeServer.AdminDisplayVersion;
				if (adminDisplayVersion.Major < (int)this.upgradeTo.ExchangeBuild.Major)
				{
					throw new LocalizedException(HybridStrings.ErrorHybridUpgradeNotAllTransportServersProperVersion);
				}
			}
		}

		private void EnsureTenantHasAlreadyBeenUpgraded(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoVerifyTenantHasBeenUpgraded);
			IOrganizationConfig organizationConfig = taskContext.TenantSession.GetOrganizationConfig();
			if (organizationConfig.AdminDisplayVersion.ExchangeBuild < this.upgradeTo.ExchangeBuild || organizationConfig.IsUpgradingOrganization)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridTenenatUpgradeRequired);
			}
		}

		private void UpgradeFopeConnectors(ITaskContext taskContext)
		{
			MultiValuedProperty<SmtpDomain> multiValuedProperty = new MultiValuedProperty<SmtpDomain>();
			foreach (SmtpDomain item in base.TaskContext.HybridConfigurationObject.Domains)
			{
				multiValuedProperty.Add(item);
			}
			IOrganizationConfig organizationConfig = base.OnPremisesSession.GetOrganizationConfig();
			List<string> domains = new List<string>();
			OrganizationRelationship organizationRelationship = TaskCommon.GetOrganizationRelationship(base.OnPremisesSession, Configuration.OnPremGetOrgRel, domains);
			OrganizationRelationship organizationRelationship2 = TaskCommon.GetOrganizationRelationship(base.TenantSession, Configuration.TenantGetOrgRel, domains);
			if (organizationRelationship2 == null || organizationRelationship == null)
			{
				throw new LocalizedException(HybridStrings.InvalidOrganizationRelationship);
			}
			string onPremOrgRelationshipName = TaskCommon.GetOnPremOrgRelationshipName(organizationConfig);
			string tenantOrgRelationshipName = TaskCommon.GetTenantOrgRelationshipName(organizationConfig);
			SessionParameters sessionParameters = new SessionParameters();
			SessionParameters sessionParameters2 = new SessionParameters();
			sessionParameters.Set("Name", onPremOrgRelationshipName);
			sessionParameters2.Set("Name", tenantOrgRelationshipName);
			base.OnPremisesSession.SetOrganizationRelationship(organizationRelationship.Identity, sessionParameters);
			base.TenantSession.SetOrganizationRelationship(organizationRelationship2.Identity, sessionParameters2);
			organizationRelationship2 = TaskCommon.GetOrganizationRelationship(base.TenantSession, tenantOrgRelationshipName, domains);
			if (organizationRelationship2 == null)
			{
				throw new LocalizedException(HybridStrings.InvalidOrganizationRelationship);
			}
			IInboundConnector inboundConnector = base.TenantSession.GetInboundConnectors().FirstOrDefault((IInboundConnector x) => x.ConnectorSource == TenantConnectorSource.HybridWizard);
			if (inboundConnector == null)
			{
				throw new LocalizedException(HybridStrings.ErrorNoInboundConnector);
			}
			base.TenantSession.RenameInboundConnector(inboundConnector, Configuration.InboundConnectorName(organizationConfig.Guid.ToString()));
			IOutboundConnector outboundConnector = base.TenantSession.GetOutboundConnectors().FirstOrDefault((IOutboundConnector x) => x.ConnectorSource == TenantConnectorSource.HybridWizard);
			if (outboundConnector == null)
			{
				throw new LocalizedException(HybridStrings.ErrorNoOutboundConnector);
			}
			base.TenantSession.RenameOutboundConnector(outboundConnector, Configuration.OutboundConnectorName(organizationConfig.Guid.ToString()));
			base.TenantSession.NewOnPremisesOrganization(organizationConfig, multiValuedProperty, inboundConnector, outboundConnector, organizationRelationship2);
		}

		private void ValidateFopeConnectorsAreUpgraded(ITaskContext taskContext)
		{
			IOrganizationConfig organizationConfig = base.OnPremisesSession.GetOrganizationConfig();
			IOnPremisesOrganization onPremisesOrganization = base.TenantSession.GetOnPremisesOrganization(organizationConfig.Guid);
			IInboundConnector inboundConnector = base.TenantSession.GetInboundConnector(onPremisesOrganization.InboundConnector.ToString());
			IOutboundConnector outboundConnector = base.TenantSession.GetOutboundConnector(onPremisesOrganization.OutboundConnector.ToString());
			if (inboundConnector.ConnectorSource != TenantConnectorSource.HybridWizard || outboundConnector.ConnectorSource != TenantConnectorSource.HybridWizard)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridOnPremisesOrganizationWasNotCreatedWithUpgradedConnectors);
			}
		}

		private void RemoveUnnecessaryRemoteDomains(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoRemovingUnnecessaryRemoteDomains);
			foreach (DomainContentConfig domainContentConfig in from x in taskContext.OnPremisesSession.GetRemoteDomain()
			where (x.Name ?? string.Empty).StartsWith("Hybrid Domain -")
			select x)
			{
				taskContext.OnPremisesSession.RemoveRemoteDomain(domainContentConfig.Identity);
			}
			foreach (DomainContentConfig domainContentConfig2 in from x in taskContext.TenantSession.GetRemoteDomain()
			where (x.Name ?? string.Empty).StartsWith("Hybrid Domain -")
			select x)
			{
				taskContext.TenantSession.RemoveRemoteDomain(domainContentConfig2.Identity);
			}
		}

		private void ValidateUnnecessaryRemoteDomainsAreRemoved(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoValidatingUnnecessaryRemoteDomainsAreRemoved);
			DomainContentConfig domainContentConfig = (from x in taskContext.OnPremisesSession.GetRemoteDomain()
			where (x.Name ?? string.Empty).StartsWith("Hybrid Domain -")
			select x).FirstOrDefault<DomainContentConfig>();
			if (domainContentConfig != null)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridOnPremRemoteDomainNotRemoved(domainContentConfig.Name));
			}
			DomainContentConfig domainContentConfig2 = (from x in taskContext.TenantSession.GetRemoteDomain()
			where (x.Name ?? string.Empty).StartsWith("Hybrid Domain -")
			select x).FirstOrDefault<DomainContentConfig>();
			if (domainContentConfig2 != null)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridTenantRemoteDomainNotRemoved(domainContentConfig2.Name));
			}
		}

		private void ClearUnusedConfigurationAttributes(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoClearingUnusedHybridConfigurationProperties);
			taskContext.HybridConfigurationObject.ClientAccessServers.Clear();
			taskContext.HybridConfigurationObject.ExternalIPAddresses.Clear();
			try
			{
				taskContext.HybridConfigurationObject.Session.Save(taskContext.HybridConfigurationObject);
			}
			catch (InvalidOperationException)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridUpgradedTo2013);
			}
		}

		private void ValidateUnusedConfigurationAttributesAreCleared(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoValidateUnusedConfigurationAttributesAreCleared);
			if (taskContext.HybridConfigurationObject.ClientAccessServers.Count != 0)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridClientAccessServersNotCleared);
			}
			if (taskContext.HybridConfigurationObject.ExternalIPAddresses.Count != 0)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridExternalIPAddressesNotCleared);
			}
		}

		private void UpgradeHybridConfigurationObjectVersion(ITaskContext taskContext)
		{
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoUpdatingHybridConfigurationVersion);
			taskContext.HybridConfigurationObject.SetExchangeVersion(this.upgradeTo);
			try
			{
				taskContext.HybridConfigurationObject.Session.Save(taskContext.HybridConfigurationObject);
			}
			catch (InvalidOperationException)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridUpgradedTo2013);
			}
		}

		private void ValidateHybridConfigurationObjectVersionUpgraded(ITaskContext taskContext)
		{
			if (taskContext.HybridConfigurationObject.ExchangeVersion != this.upgradeTo)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridConfigurationVersionNotUpdated);
			}
		}

		public const string E14RemoteDomainPrefix = "Hybrid Domain -";

		private readonly ExchangeObjectVersion upgradeFrom = ExchangeObjectVersion.Exchange2010;

		private readonly ExchangeObjectVersion upgradeTo = ExchangeObjectVersion.Exchange2012;

		private readonly ExchangeObjectVersion currentVersion;
	}
}
