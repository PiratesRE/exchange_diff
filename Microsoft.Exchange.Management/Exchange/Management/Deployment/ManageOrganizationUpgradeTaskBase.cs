using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageOrganizationUpgradeTaskBase : ManageOrganizationTaskBase
	{
		protected ManageOrganizationUpgradeTaskBase()
		{
			this.Identity = null;
			base.Fields["InstallationMode"] = InstallationModes.BuildToBuildUpgrade;
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public OrganizationIdParameter Identity { get; set; }

		protected override ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return ExchangeRunspaceConfigurationSettings.ExchangeApplication.LowPriorityScripts;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.tenantCU = null;
			this.tenantFQDN = null;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			Organization orgContainer = base.Session.GetOrgContainer();
			if (OrganizationId.ForestWideOrgId.Equals(orgContainer.OrganizationId) && orgContainer.ObjectVersion < Organization.OrgConfigurationVersion)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorEnterpriseOrgOutOfDate), (ErrorCategory)1000, null);
			}
			IEnumerable<ExchangeConfigurationUnit> objects = this.Identity.GetObjects<ExchangeConfigurationUnit>(null, base.Session);
			using (IEnumerator<ExchangeConfigurationUnit> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(this.Identity.ToString()));
				}
				this.tenantCU = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(this.Identity.ToString()));
				}
			}
			if (this.tenantCU != null)
			{
				base.CurrentOrganizationId = this.tenantCU.OrganizationId;
			}
			this.tenantFQDN = this.CreateTenantSession(this.tenantCU.OrganizationId, true, ConsistencyMode.FullyConsistent).GetDefaultAcceptedDomain();
			if (this.tenantFQDN == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorNoDefaultAcceptedDomainFound(this.Identity.ToString()));
			}
			if (this.tenantCU.OrganizationStatus != OrganizationStatus.Active && this.tenantCU.OrganizationStatus != OrganizationStatus.Suspended && this.tenantCU.OrganizationStatus != OrganizationStatus.LockedOut)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorOrganizationNotUpgradable(this.Identity.ToString(), this.tenantCU.OrganizationStatus.ToString())), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			this.monadConnection.RunspaceProxy.SetVariable("TargetProgramId", this.tenantCU.ProgramId);
			this.monadConnection.RunspaceProxy.SetVariable("TargetOfferId", this.TargetOfferId);
			this.monadConnection.RunspaceProxy.SetVariable("EnableUpdateThrottling", TenantUpgradeConfigImpl.GetConfig<bool>("EnableUpdateThrottling"));
		}

		protected virtual string TargetOfferId
		{
			get
			{
				if (this.upgradeOfferId == null && !ServicePlanConfiguration.GetInstance().TryGetReversePilotOfferId(this.tenantCU.ProgramId, this.tenantCU.OfferId, out this.upgradeOfferId))
				{
					this.upgradeOfferId = this.tenantCU.OfferId;
				}
				return this.upgradeOfferId;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			if (this.tenantCU != null)
			{
				string distinguishedName = this.tenantCU.OrganizationId.OrganizationalUnit.DistinguishedName;
				base.Fields["TenantOrganizationDN"] = distinguishedName;
				base.Fields["OrganizationHierarchicalPath"] = OrganizationIdParameter.GetHierarchicalIdentityFromDN(distinguishedName);
				base.Fields["TenantOrganizationObjectVersion"] = this.tenantCU.ObjectVersion;
				base.Fields["TenantDomainName"] = this.tenantFQDN.DomainName.ToString();
				base.Fields["TenantExternalDirectoryOrganizationId"] = this.tenantCU.ExternalDirectoryOrganizationId;
				base.Fields["Partition"] = ADAccountPartitionLocator.GetAccountPartitionGuidByPartitionId(new PartitionId(this.tenantCU.Id));
			}
		}

		internal IConfigurationSession CreateTenantSession(OrganizationId orgId, bool isReadonly, ConsistencyMode consistencyMode)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, orgId, base.ExecutingUserOrganizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, isReadonly, consistencyMode, sessionSettings, 221, "CreateTenantSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\ManageOrganizationUpgradeTaskBase.cs");
		}

		protected AcceptedDomain tenantFQDN;

		protected ExchangeConfigurationUnit tenantCU;

		private string upgradeOfferId;
	}
}
