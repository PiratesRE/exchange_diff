using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "Organization", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "OrgScopedParameterSet")]
	public sealed class RemoveOrganization : ManageOrganizationTaskBase
	{
		public RemoveOrganization()
		{
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new RemoveOrganizationTaskModuleFactory();
		}

		protected override bool ShouldExecuteComponentTasks()
		{
			return true;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.RemoveOrganizationDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveOrganization(this.Identity.ToString());
			}
		}

		protected override ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return ExchangeRunspaceConfigurationSettings.ExchangeApplication.DiscretionaryScripts;
			}
		}

		private new Fqdn DomainController
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForReconciliation
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForReconciliation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForReconciliation"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Async
		{
			get
			{
				return (SwitchParameter)(base.Fields["Async"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Async"] = value;
			}
		}

		[Parameter]
		public SwitchParameter AuthoritativeOnly
		{
			get
			{
				return this.authoritativeOnly;
			}
			set
			{
				this.authoritativeOnly = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		private bool PartnerMode
		{
			get
			{
				return (bool)(base.Fields["PartnerMode"] ?? false);
			}
			set
			{
				base.Fields["PartnerMode"] = value;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			if (this.exchangeConfigUnit != null)
			{
				base.Fields["OrganizationHierarchicalPath"] = OrganizationIdParameter.GetHierarchicalIdentityFromDN(this.tenantOU.DistinguishedName);
				base.Fields["RemoveObjectsChunkSize"] = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ProvisioningThrottling", "RemoveObjectsChunkSize", 100);
				base.Fields["RemoveObjectsChunkSleepTime"] = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ProvisioningThrottling", "RemoveObjectsChunkSleepTime", 10);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.ExchangeRunspaceConfig != null)
			{
				this.PartnerMode = base.ExchangeRunspaceConfig.PartnerMode;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			OrganizationId organizationId = OrganizationTaskHelper.ResolveOrganization(this, this.Identity, base.RootOrgContainerId, Strings.ErrorOrganizationIdentityRequired);
			if (this.Identity == null)
			{
				this.Identity = new OrganizationIdParameter(organizationId.OrganizationalUnit.Name);
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(organizationId.PartitionId);
			this.adSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession((base.ServerSettings == null) ? null : base.ServerSettings.PreferredGlobalCatalog(organizationId.PartitionId.ForestFQDN), false, ConsistencyMode.PartiallyConsistent, sessionSettings, 262, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\RemoveOrganization.cs");
			this.tenantOU = OrganizationTaskHelper.GetOUFromOrganizationId(this.Identity, this.adSession, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
			if (this.tenantOU == null)
			{
				base.WriteError(new OrganizationDoesNotExistException(Strings.ErrorOrganizationNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
			this.adSession.UseConfigNC = true;
			this.exchangeConfigUnit = this.adSession.Read<ExchangeConfigurationUnit>(this.tenantOU.ConfigurationUnit);
			if (!OrganizationTaskHelper.CanProceedWithOrganizationTask(this.Identity, this.adSession, RemoveOrganization.ignorableFlagsOnStatusTimeout, new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				base.WriteError(new OrganizationPendingOperationException(Strings.ErrorCannotRemoveNonActiveOrganization(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
			ServicePlan servicePlanSettings = ServicePlanConfiguration.GetInstance().GetServicePlanSettings(this.exchangeConfigUnit.ProgramId, this.exchangeConfigUnit.OfferId);
			base.InternalLocalStaticConfigEnabled = !servicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled;
			base.InternalLocalHydrateableConfigEnabled = !servicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled;
			base.InternalCreateSharedConfiguration = (this.exchangeConfigUnit.SharedConfigurationInfo != null);
			ADSessionSettings sessionSettings2 = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, this.tenantOU.OrganizationId, base.ExecutingUserOrganizationId, false);
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession((base.ServerSettings == null) ? null : base.ServerSettings.PreferredGlobalCatalog(organizationId.PartitionId.ForestFQDN), true, ConsistencyMode.PartiallyConsistent, sessionSettings2, 314, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\RemoveOrganization.cs");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession((base.ServerSettings == null) ? null : base.ServerSettings.PreferredGlobalCatalog(organizationId.PartitionId.ForestFQDN), true, ConsistencyMode.PartiallyConsistent, sessionSettings2, 320, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\RemoveOrganization.cs");
			TransportConfigContainer transportConfigContainer = tenantOrTopologyConfigurationSession.FindSingletonConfigurationObject<TransportConfigContainer>();
			if (transportConfigContainer != null)
			{
				this.organizationFederatedMailboxAlias = transportConfigContainer.OrganizationFederatedMailbox.Local;
			}
			if (!this.Force && this.GetUserMailboxCount(2) > 0)
			{
				base.WriteError(new OrganizationValidationException(Strings.RemoveOrganizationFailWithExistingMailboxes), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (ExchangeConfigurationUnit.RelocationInProgress(this.exchangeConfigUnit))
			{
				base.WriteError(new OrganizationValidationException(Strings.RemoveOrganizationFailRelocationInProgress), (ErrorCategory)1000, this.Identity);
			}
			if (this.exchangeConfigUnit.EnableAsSharedConfiguration)
			{
				base.WriteError(new OrganizationValidationException(Strings.RemoveOrganizationFailWithoutSharedConfigurationParameter), (ErrorCategory)1000, this.Identity);
			}
			if (OrganizationTaskHelper.IsSharedConfigLinkedToOtherTenants(organizationId, this.adSession))
			{
				base.WriteError(new OrganizationValidationException(Strings.RemoveOrganizationFailWithSharedConfigurationBacklinks), (ErrorCategory)1000, this.Identity);
			}
			if (base.IsMSITTenant(organizationId))
			{
				this.authoritativeOnly = true;
			}
			TaskLogger.LogExit();
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			if (this.authoritativeOnly)
			{
				this.monadConnection.RunspaceProxy.SetVariable(StartOrganizationUpgradeTask.AuthoritativeOnlyVarName, true);
			}
			this.monadConnection.RunspaceProxy.SetVariable("ReconciliationMode", this.ForReconciliation);
		}

		protected override void FilterComponents()
		{
			base.FilterComponents();
			if (this.ForReconciliation)
			{
				foreach (SetupComponentInfo setupComponentInfo in base.ComponentInfoList)
				{
					setupComponentInfo.Tasks.RemoveAll(delegate(TaskInfo taskInfo)
					{
						OrgTaskInfo orgTaskInfo = taskInfo as OrgTaskInfo;
						return orgTaskInfo != null && orgTaskInfo.Uninstall != null && orgTaskInfo.Uninstall.Tenant != null && !orgTaskInfo.Uninstall.Tenant.UseForReconciliation;
					});
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			int defaultValue = this.IsTestTopologyDomain() ? 100 : 0;
			int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ProvisioningThrottling", "MailboxCountAsyncRemovalSize", defaultValue);
			if (this.Async && this.GetRecipientCount(value + 1, false, true) > value)
			{
				OrganizationTaskHelper.SetOrganizationStatus(this.adSession, this.exchangeConfigUnit, OrganizationStatus.ReadyForRemoval);
			}
			else
			{
				base.Fields["TenantOrganizationFullPath"] = this.tenantOU.DistinguishedName;
				base.Fields["TenantCUFullPath"] = this.tenantOU.ConfigurationUnit.Parent.DistinguishedName;
				base.Fields[RemoveOrganization.ExternalDirectoryOrganizationIdVarName] = this.exchangeConfigUnit.ExternalDirectoryOrganizationId;
				if (this.exchangeConfigUnit.OrganizationStatus != OrganizationStatus.PendingRemoval)
				{
					OrganizationTaskHelper.SetOrganizationStatus(this.adSession, this.exchangeConfigUnit, OrganizationStatus.PendingRemoval);
				}
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private int GetUserMailboxCount(int maxCount)
		{
			return this.GetRecipientCount(maxCount, true, false);
		}

		private bool IsTestTopologyDomain()
		{
			string forestName = NativeHelpers.GetForestName();
			return !string.IsNullOrEmpty(forestName) && (forestName.EndsWith(".extest.microsoft.com", StringComparison.InvariantCultureIgnoreCase) || forestName.EndsWith(".extest.net", StringComparison.InvariantCultureIgnoreCase));
		}

		internal int GetRecipientCount(int maxCount, bool userMailboxOnly, bool includeSoftDeletedObjects)
		{
			int result = 0;
			IRecipientSession recipientSession = this.recipientSession;
			QueryFilter queryFilter = new NotFilter(new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.ArbitrationMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.MailboxPlan),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.DiscoveryMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.AuditLogMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.Alias, this.organizationFederatedMailboxAlias)
			}));
			if (userMailboxOnly)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
					queryFilter
				});
			}
			if (includeSoftDeletedObjects)
			{
				ADSessionSettings sessionSettings = this.recipientSession.SessionSettings;
				sessionSettings.IncludeSoftDeletedObjects = true;
				recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(this.recipientSession.DomainController, this.recipientSession.SearchRoot, this.recipientSession.Lcid, this.recipientSession.ReadOnly, this.recipientSession.ConsistencyMode, this.recipientSession.NetworkCredential, sessionSettings, ConfigScopes.TenantSubTree, 555, "GetRecipientCount", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\RemoveOrganization.cs");
				recipientSession.EnforceDefaultScope = this.recipientSession.EnforceDefaultScope;
				recipientSession.UseGlobalCatalog = this.recipientSession.UseGlobalCatalog;
				recipientSession.LinkResolutionServer = this.recipientSession.LinkResolutionServer;
			}
			ADRecipient[] array = recipientSession.Find(null, QueryScope.SubTree, queryFilter, null, maxCount);
			if (array != null)
			{
				result = array.Length;
			}
			return result;
		}

		private const string OrgScopedParameterSet = "OrgScopedParameterSet";

		private const string ForReconciliationVarName = "ReconciliationMode";

		private const string RemoveObjectsChunkSize = "RemoveObjectsChunkSize";

		private const string RemoveObjectsChunkSleepTime = "RemoveObjectsChunkSleepTime";

		private const string RecipientCountAsyncRemovalSize = "MailboxCountAsyncRemovalSize";

		internal static readonly string ExternalDirectoryOrganizationIdVarName = "TenantExternalDirectoryOrganizationId";

		private ITenantConfigurationSession adSession;

		private ADOrganizationalUnit tenantOU;

		private ExchangeConfigurationUnit exchangeConfigUnit;

		private IRecipientSession recipientSession;

		private SwitchParameter authoritativeOnly;

		private string organizationFederatedMailboxAlias = string.Empty;

		private static readonly OrganizationStatus[] ignorableFlagsOnStatusTimeout = new OrganizationStatus[]
		{
			OrganizationStatus.PendingRemoval,
			OrganizationStatus.PendingAcceptedDomainAddition,
			OrganizationStatus.PendingAcceptedDomainRemoval,
			OrganizationStatus.ReadyForRemoval,
			OrganizationStatus.SoftDeleted,
			OrganizationStatus.Suspended,
			OrganizationStatus.LockedOut
		};
	}
}
