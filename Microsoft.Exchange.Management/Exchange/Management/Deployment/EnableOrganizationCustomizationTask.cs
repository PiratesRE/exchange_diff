using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Enable", "OrganizationCustomization", SupportsShouldProcess = true, DefaultParameterSetName = "IdentityParameterSet")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class EnableOrganizationCustomizationTask : ManageServicePlanMigrationBase
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.EnableConfigCustomizationsDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableConfigCustomizations(this.orgIdParam.ToString());
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			OrganizationId organizationId = null;
			if (OrganizationId.ForestWideOrgId.Equals(base.ExecutingUserOrganizationId))
			{
				if (this.Identity == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNeedOrgIdentity), (ErrorCategory)1000, null);
				}
				else
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 109, "InternalStateReset", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\EnableConfigCustomizations.cs");
					tenantOrTopologyConfigurationSession.UseConfigNC = false;
					ADOrganizationalUnit oufromOrganizationId = OrganizationTaskHelper.GetOUFromOrganizationId(this.Identity, tenantOrTopologyConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
					organizationId = oufromOrganizationId.OrganizationId;
				}
			}
			else
			{
				organizationId = base.ExecutingUserOrganizationId;
			}
			this.orgIdParam = new OrganizationIdParameter(organizationId.OrganizationalUnit);
			base.LoadTenantCU();
			TaskLogger.LogExit();
		}

		protected override void OnHalt(Exception e)
		{
			TaskLogger.LogEvent(TaskEventLogConstants.Tuple_HydrationTaskFailed, base.CurrentTaskContext.InvocationInfo, this.tenantCU.DistinguishedName, new object[]
			{
				e.ToString()
			});
		}

		protected override void ResolveTargetOffer()
		{
			string targetOfferId;
			if (!this.config.TryGetHydratedOfferId(this.tenantCU.ProgramId, this.tenantCU.OfferId, out targetOfferId))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorChangeConfigurationNotSupported), (ErrorCategory)1000, null);
				return;
			}
			this.targetProgramId = this.tenantCU.ProgramId;
			this.targetOfferId = targetOfferId;
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			if (base.ExchangeRunspaceConfig != null)
			{
				base.ExchangeRunspaceConfig.LoadRoleCmdletInfo();
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new EnableOrganizationCustomizationTaskModuleFactory();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.IsHydrationAllowed())
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorHydrationDisabled(this.tenantCU.ConfigurationUnit.Parent.Name)), (ErrorCategory)1000, null);
			}
			if (this.tenantCU.IsPilotingOrganization)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantIsPiloting), (ErrorCategory)1000, null);
			}
			if (this.tenantCU.AdminDisplayVersion != null && (int)this.tenantCU.AdminDisplayVersion.ExchangeBuild.Major < OrganizationTaskHelper.ManagementDllVersion.FileMajorPart)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantNeedsUpgradeFirst), (ErrorCategory)1000, null);
			}
			if (this.tenantCU.IsUpgradingOrganization)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantIsUpgrading), (ErrorCategory)1000, null);
			}
		}

		private bool IsHydrationAllowed()
		{
			bool config = SlimTenantConfigImpl.GetConfig<bool>("IsHydrationAllowed");
			ExTraceGlobals.SlimTenantTracer.TraceDebug<bool>(0L, "Global Config: EnableOrganizationCustomizationTask::IsHydrationAllowed: {0}.", config);
			return config;
		}
	}
}
