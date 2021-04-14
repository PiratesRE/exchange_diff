using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageServicePlanMigrationBase : ManageOrganizationTaskBase
	{
		internal UpgradePhase UpgradePhase
		{
			get
			{
				if (!this.configOnly)
				{
					return UpgradePhase.Cleanup;
				}
				return UpgradePhase.UpdateConfiguration;
			}
		}

		private bool IsCrossSKUMigration { get; set; }

		public virtual OrganizationIdParameter Identity
		{
			get
			{
				return this.orgIdParam;
			}
			set
			{
				this.orgIdParam = value;
			}
		}

		protected override ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return ExchangeRunspaceConfigurationSettings.ExchangeApplication.LowPriorityScripts;
			}
		}

		internal override IConfigurationSession CreateSession()
		{
			this.rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 128, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateServicePlanTask.cs");
		}

		private ITenantConfigurationSession CreateTenantSession(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, orgId, base.ExecutingUserOrganizationId, false);
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, false, ConsistencyMode.FullyConsistent, sessionSettings, 149, "CreateTenantSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateServicePlanTask.cs");
		}

		protected override void InitializeComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.Add("setup\\data\\PreServicePlanUpgrade.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\CommonGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\TransportGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\BridgeheadGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\ClientAccessGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\MailboxGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\UnifiedMessagingGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\ProvisioningFeatureCatalog.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\PostPrepForestGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\PostServicePlanUpgrade.xml");
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.Fields["InstallationMode"] = InstallationModes.Install;
			this.oldServicePlanSettings = null;
			this.newServicePlanSettings = null;
			this.deltaServicePlanSettings = null;
			this.newServicePlan = null;
			this.tenantCU = null;
			this.featuresToApply = null;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected abstract void ResolveTargetOffer();

		protected void LoadTenantCU()
		{
			this.tenantCU = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(this.orgIdParam, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			if (this.tenantCU != null)
			{
				base.CurrentOrganizationId = this.tenantCU.OrganizationId;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			Organization orgContainer = base.Session.GetOrgContainer();
			if (OrganizationId.ForestWideOrgId.Equals(orgContainer.OrganizationId) && orgContainer.ObjectVersion < Organization.OrgConfigurationVersion)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorEnterpriseOrgOutOfDate), (ErrorCategory)1000, null);
			}
			if (this.tenantCU.IsUpdatingServicePlan)
			{
				base.WriteVerbose(Strings.VerbosePendingServicePlanUpgradeDetected(this.Identity.RawIdentity));
			}
			else if (this.tenantCU.OrganizationStatus != OrganizationStatus.Active)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantOrgInUnexpectedState), (ErrorCategory)1002, null);
			}
			if (string.IsNullOrEmpty(this.tenantCU.ServicePlan))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanIsNotSet), (ErrorCategory)1002, null);
			}
			this.tenantConfigurationSession = this.CreateTenantSession(this.tenantCU.OrganizationId);
			this.tenantFQDN = this.tenantConfigurationSession.GetDefaultAcceptedDomain();
			if (this.tenantFQDN == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorNoDefaultAcceptedDomainFound(this.Identity.ToString()));
			}
			Exception ex = null;
			try
			{
				this.oldServicePlanSettings = this.config.GetServicePlanSettings(this.tenantCU.ServicePlan);
				List<ValidationError> list = new List<ValidationError>();
				list.AddRange(ServicePlan.ValidateFileSchema(this.oldServicePlanSettings.Name));
				list.AddRange(this.oldServicePlanSettings.Validate());
				if (list.Count != 0)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanInconsistent2(this.tenantCU.ServicePlan, ValidationError.CombineErrorDescriptions(list))), (ErrorCategory)1000, null);
				}
				this.ResolveTargetOffer();
				this.newServicePlan = this.config.ResolveServicePlanName(this.targetProgramId, this.targetOfferId);
				this.newServicePlanSettings = this.config.GetServicePlanSettings(this.newServicePlan);
				if (!string.IsNullOrEmpty(this.tenantCU.TargetServicePlan) && !this.tenantCU.TargetServicePlan.Equals(this.newServicePlan, StringComparison.OrdinalIgnoreCase))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorTargetServicePlanDifferent(this.tenantCU.ServicePlan, this.tenantCU.TargetServicePlan, this.newServicePlan)), (ErrorCategory)1000, null);
				}
				list.Clear();
				list.AddRange(ServicePlan.ValidateFileSchema(this.newServicePlanSettings.Name));
				list.AddRange(this.newServicePlanSettings.Validate());
				if (list.Count != 0)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanInconsistent(this.newServicePlan, this.targetProgramId, this.targetOfferId, ValidationError.CombineErrorDescriptions(list))), (ErrorCategory)1000, null);
				}
				foreach (ServicePlan.MailboxPlan mailboxPlan in this.oldServicePlanSettings.MailboxPlans)
				{
					bool flag = false;
					foreach (ServicePlan.MailboxPlan mailboxPlan2 in this.newServicePlanSettings.MailboxPlans)
					{
						if (mailboxPlan.Name == mailboxPlan2.Name && mailboxPlan.MailboxPlanIndex == mailboxPlan2.MailboxPlanIndex)
						{
							this.ValidateMailboxPlansCapabilities(mailboxPlan, mailboxPlan2);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						if (ManageServicePlanMigrationBase.IsSupportedCrossSKUMigrationScenario(this.tenantCU.ProgramId, this.targetProgramId, this.tenantCU.OfferId, this.targetOfferId))
						{
							this.IsCrossSKUMigration = true;
						}
						else if (!this.oldServicePlanSettings.Organization.PilotEnabled)
						{
							base.WriteError(new InvalidOperationException(Strings.ErrorServicePlanHasNoMatchingMailboxPlan(mailboxPlan.Name)), (ErrorCategory)1000, null);
						}
					}
				}
				if (!this.oldServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled && this.newServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.PerMBXPlanRoleAssignmentPolicyEnabled.Name, this.oldServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled.ToString(), this.newServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (this.oldServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled && this.oldServicePlanSettings.MailboxPlans.Count > 1 && !this.newServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.PerMBXPlanRoleAssignmentPolicyEnabled.Name, this.oldServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled.ToString(), this.newServicePlanSettings.Organization.PerMBXPlanRoleAssignmentPolicyEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (this.oldServicePlanSettings.Organization.PerMBXPlanRetentionPolicyEnabled != this.newServicePlanSettings.Organization.PerMBXPlanRetentionPolicyEnabled && this.newServicePlanSettings.MailboxPlans.Count > 1)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.PerMBXPlanRetentionPolicyEnabled.Name, this.oldServicePlanSettings.Organization.PerMBXPlanRetentionPolicyEnabled.ToString(), this.newServicePlanSettings.Organization.PerMBXPlanRetentionPolicyEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (this.oldServicePlanSettings.Organization.PerMBXPlanOWAPolicyEnabled != this.newServicePlanSettings.Organization.PerMBXPlanOWAPolicyEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.PerMBXPlanOWAPolicyEnabled.Name, this.oldServicePlanSettings.Organization.PerMBXPlanOWAPolicyEnabled.ToString(), this.newServicePlanSettings.Organization.PerMBXPlanOWAPolicyEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (!this.oldServicePlanSettings.Organization.ShareableConfigurationEnabled && this.newServicePlanSettings.Organization.ShareableConfigurationEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.ShareableConfigurationEnabled.Name, this.oldServicePlanSettings.Organization.ShareableConfigurationEnabled.ToString(), this.newServicePlanSettings.Organization.ShareableConfigurationEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (!this.oldServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled && this.newServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.CommonHydrateableObjectsSharedEnabled.Name, this.oldServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled.ToString(), this.newServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled.ToString())), (ErrorCategory)1000, null);
				}
				if (!this.oldServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled && this.newServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidServicePlanTransition(this.oldServicePlanSettings.Name, this.newServicePlanSettings.Name, OrganizationSettingsSchema.AdvancedHydrateableObjectsSharedEnabled.Name, this.oldServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled.ToString(), this.newServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled.ToString())), (ErrorCategory)1000, null);
				}
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
			base.InternalLocalStaticConfigEnabled = !this.newServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled;
			base.InternalLocalHydrateableConfigEnabled = !this.newServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled;
			base.InternalPilotEnabled = this.newServicePlanSettings.Organization.PilotEnabled;
			TaskLogger.LogExit();
		}

		private void ValidateMailboxPlansCapabilities(ServicePlan.MailboxPlan oldMbxPlan, ServicePlan.MailboxPlan newMailboxPlan)
		{
			if (oldMbxPlan == null)
			{
				throw new ArgumentNullException("oldMbxPlan");
			}
			if (oldMbxPlan == null)
			{
				throw new ArgumentNullException("oldMbxPlan");
			}
			if (oldMbxPlan.SkuCapability != Capability.None && newMailboxPlan.SkuCapability == Capability.None)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidMailboxPlanTransition(oldMbxPlan.Name, oldMbxPlan.SkuCapability.ToString(), newMailboxPlan.Name, newMailboxPlan.SkuCapability.ToString())), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			this.monadConnection.RunspaceProxy.SetVariable(ManageServicePlanMigrationBase.OldServicePlanSettingsVarName, this.oldServicePlanSettings);
			this.monadConnection.RunspaceProxy.SetVariable(ManageServicePlanMigrationBase.NewServicePlanSettingsVarName, this.newServicePlanSettings);
			this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.ServicePlanSettingsVarName, this.newServicePlanSettings);
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			string distinguishedName = this.tenantCU.OrganizationId.OrganizationalUnit.DistinguishedName;
			base.Fields["OrganizationHierarchicalPath"] = OrganizationIdParameter.GetHierarchicalIdentityFromDN(distinguishedName);
			base.Fields["TenantOrganizationDN"] = distinguishedName;
			base.Fields["TenantDomainName"] = this.tenantFQDN.DomainName.ToString();
			base.Fields["TenantOrganizationObjectVersion"] = this.tenantCU.ObjectVersion;
			base.Fields["TenantName"] = this.tenantCU.OrganizationId.OrganizationalUnit.Name;
			base.Fields["IsCrossSKUMigration"] = this.IsCrossSKUMigration;
			base.Fields["IsBPOSLHydration"] = (!this.newServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled && this.oldServicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled);
			base.Fields["IsBPOSSHydration"] = (!this.newServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled && this.oldServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled);
		}

		protected override void FilterComponents()
		{
			base.FilterComponents();
			if (this.deltaServicePlanSettings != null)
			{
				bool recreateCannedRBACRoles = ManageServicePlanMigrationBase.CannedRolesChanged(this.deltaServicePlanSettings);
				bool rolesToRoleGroupAssignmentsChange = ManageServicePlanMigrationBase.RoleToRoleGroupsAssignmentsChanged(this.deltaServicePlanSettings);
				bool shareableConfigurationEnabledChange = ManageServicePlanMigrationBase.RBACConfigPresenceChanged(this.deltaServicePlanSettings);
				bool createNewMailboxPlanObject = (this.conservative || ManageServicePlanMigrationBase.MailboxPlanConfigurationChanged(this.deltaServicePlanSettings)) && !this.IsCrossSKUMigration && !base.InternalPilotEnabled;
				foreach (SetupComponentInfo setupComponentInfo in base.ComponentInfoList)
				{
					setupComponentInfo.Tasks.RemoveAll(delegate(TaskInfo taskInfo)
					{
						ServicePlanTaskInfo servicePlanTaskInfo = taskInfo as ServicePlanTaskInfo;
						if (servicePlanTaskInfo == null)
						{
							return true;
						}
						string featureName;
						switch (featureName = servicePlanTaskInfo.FeatureName)
						{
						case "LoadSetupSnapin":
						case "ServicePlanUpgradeStart":
						case "PerMBXPlanRoleAssignmentPolicyEnabled":
						case "PerMBXPlanRetentionPolicyEnabled":
						case "PerMBXPlanOWAPolicyEnabled":
						case "MailboxPlans":
							return false;
						case "RecreateCannedRBACRoleGroups":
							return !rolesToRoleGroupAssignmentsChange;
						case "RecreateCannedRBACRoles":
							return !recreateCannedRBACRoles && !shareableConfigurationEnabledChange && !rolesToRoleGroupAssignmentsChange;
						case "RecreateCannedRBACRoleAssignments":
							return !rolesToRoleGroupAssignmentsChange && !shareableConfigurationEnabledChange;
						case "CreateNewMailboxPlanObject":
							return !createNewMailboxPlanObject;
						case "UpdateMailboxes":
						{
							bool flag = this.UpgradePhase >= UpgradePhase.UpdateMailboxes;
							bool flag2 = createNewMailboxPlanObject || this.IsCrossSKUMigration;
							return !flag || !flag2 || !this.includeUserUpdatePhase;
						}
						case "CleanupOldMailboxPlan":
						{
							bool flag = this.UpgradePhase == UpgradePhase.Cleanup;
							bool flag2 = createNewMailboxPlanObject || this.IsCrossSKUMigration;
							return !flag || !flag2;
						}
						case "ServicePlanUpgradeEnd":
							return this.configOnly;
						}
						return !this.featuresToApply.Contains(servicePlanTaskInfo.FeatureName);
					});
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool advancedHydrateableObjectsSharedEnabled = this.newServicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled;
			base.InternalCreateSharedConfiguration = (this.oldServicePlanSettings.Organization.ShareableConfigurationEnabled && this.newServicePlanSettings.Organization.ShareableConfigurationEnabled && this.tenantCU.SupportedSharedConfigurations.Count == 0);
			base.InternalIsSharedConfigServicePlan = this.config.IsSharedConfigurationAllowedForServicePlan(this.newServicePlanSettings);
			this.monadConnection.RunspaceProxy.SetVariable("TargetServicePlan", this.newServicePlan);
			this.monadConnection.RunspaceProxy.SetVariable("TargetProgramId", this.targetProgramId);
			this.monadConnection.RunspaceProxy.SetVariable("TargetOfferId", this.targetOfferId);
			if (this.tenantCU.ServicePlan == this.newServicePlan)
			{
				base.WriteVerbose(Strings.VerboseWillSkipUpdateServicePlan(this.Identity.ToString()));
			}
			else if (!ServicePlan.CompareAndCalculateDelta(this.oldServicePlanSettings, this.newServicePlanSettings, this.IsCrossSKUMigration, out this.deltaServicePlanSettings, out this.featuresToApply))
			{
				base.WriteVerbose(Strings.VerboseWillUpgradeServicePlan(this.Identity.ToString(), this.tenantCU.ServicePlan, this.newServicePlan));
				if (advancedHydrateableObjectsSharedEnabled)
				{
					string text;
					if (!this.config.TryGetHydratedOfferId(this.targetProgramId, this.targetOfferId, out text))
					{
						text = this.targetOfferId;
					}
					SharedConfigurationInfo sharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(this.targetProgramId, text);
					OrganizationId organizationId = SharedConfiguration.FindOneSharedConfigurationId(sharedConfigurationInfo, this.tenantCU.OrganizationId.PartitionId);
					if (organizationId == null)
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.ErrorSharedConfigurationNotFound(this.targetProgramId, text, sharedConfigurationInfo.CurrentVersion.ToString())), (ErrorCategory)1000, null);
					}
					else
					{
						this.monadConnection.RunspaceProxy.SetVariable("TargetSharedConfiguration", organizationId.OrganizationalUnit.Name);
					}
				}
				if (ManageServicePlanMigrationBase.MailboxPlanConfigurationChanged(this.deltaServicePlanSettings))
				{
					ManageServicePlanMigrationBase.CopyEnabledMailboxPlanRoleAssignmentFeatures(this.deltaServicePlanSettings, this.newServicePlanSettings);
				}
				this.newServicePlanSettings.Name = "new";
				base.InternalProcessRecord();
				if (this.configOnly)
				{
					this.WriteWarning(Strings.WarningUpgradeIsNotComplete(this.UpgradePhase.ToString()));
				}
			}
			else
			{
				base.WriteVerbose(Strings.VerboseWillSkipUpdateServicePlan(this.Identity.ToString()));
			}
			TaskLogger.LogExit();
		}

		internal static void CopyEnabledMailboxPlanRoleAssignmentFeatures(ServicePlan deltaServicePlan, ServicePlan toServicePlan)
		{
			for (int i = 0; i < deltaServicePlan.MailboxPlans.Count; i++)
			{
				ServicePlan.MailboxPlan mailboxPlan = deltaServicePlan.MailboxPlans[i];
				foreach (object obj in ((IEnumerable)mailboxPlan.Schema))
				{
					FeatureDefinition featureDefinition = (FeatureDefinition)obj;
					if (featureDefinition.Categories.Contains(FeatureCategory.MailboxPlanRoleAssignment) && (bool)toServicePlan.GetMailboxPlanByName(mailboxPlan.Name)[featureDefinition])
					{
						mailboxPlan[featureDefinition] = true;
					}
				}
			}
		}

		internal static bool CannedRolesChanged(ServicePlan deltaServicePlan)
		{
			return ManageServicePlanMigrationBase.FeaturesChanged(deltaServicePlan, FeatureCategory.AdminPermissions) || ManageServicePlanMigrationBase.FeaturesChanged(deltaServicePlan, FeatureCategory.MailboxPlanPermissions);
		}

		internal static bool RoleToRoleGroupsAssignmentsChanged(ServicePlan deltaServicePlan)
		{
			return ManageServicePlanMigrationBase.FeaturesChanged(deltaServicePlan, FeatureCategory.RoleGroupRoleAssignment);
		}

		internal static bool RBACConfigPresenceChanged(ServicePlan deltaServicePlan)
		{
			return deltaServicePlan.Organization.PropertyBag.IsModified(OrganizationSettingsSchema.AdvancedHydrateableObjectsSharedEnabled);
		}

		internal static bool MailboxPlanConfigurationChanged(ServicePlan deltaServicePlan)
		{
			return ManageServicePlanMigrationBase.FeaturesChanged(deltaServicePlan, FeatureCategory.MailboxPlanConfiguration);
		}

		private static bool FeaturesChanged(ServicePlan deltaServicePlan, FeatureCategory featureCategory)
		{
			foreach (object obj in ((IEnumerable)deltaServicePlan.Organization.Schema))
			{
				FeatureDefinition featureDefinition = (FeatureDefinition)obj;
				if (featureDefinition.Categories.Contains(featureCategory) && deltaServicePlan.Organization.PropertyBag.IsModified(featureDefinition))
				{
					return true;
				}
			}
			for (int i = 0; i < deltaServicePlan.MailboxPlans.Count; i++)
			{
				foreach (object obj2 in ((IEnumerable)deltaServicePlan.MailboxPlans[i].Schema))
				{
					FeatureDefinition featureDefinition2 = (FeatureDefinition)obj2;
					if (featureDefinition2.Categories.Contains(featureCategory) && deltaServicePlan.MailboxPlans[i].PropertyBag.IsModified(featureDefinition2))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsSupportedCrossSKUMigrationScenario(string oldProgramId, string newProgramId, string oldOfferId, string newOfferId)
		{
			return ("ExchangeTest".Equals(oldProgramId, StringComparison.OrdinalIgnoreCase) && "ExchangeTest".Equals(newProgramId, StringComparison.OrdinalIgnoreCase)) || ServicePlanConfiguration.CrossSkuSupportedOffers.Contains(Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer(oldProgramId, oldOfferId), new ServicePlanOffer(newProgramId, newOfferId)));
		}

		internal static readonly string NewServicePlanSettingsVarName = "NewServicePlanSettings";

		internal static readonly string OldServicePlanSettingsVarName = "OldServicePlanSettings";

		protected OrganizationIdParameter orgIdParam;

		protected ServicePlan oldServicePlanSettings;

		protected ServicePlan newServicePlanSettings;

		protected ServicePlan deltaServicePlanSettings;

		protected string newServicePlan;

		protected ExchangeConfigurationUnit tenantCU;

		protected AcceptedDomain tenantFQDN;

		private ITenantConfigurationSession tenantConfigurationSession;

		protected bool configOnly;

		protected bool conservative;

		protected bool includeUserUpdatePhase;

		protected string targetProgramId;

		protected string targetOfferId;

		private List<string> featuresToApply;

		internal ServicePlanConfiguration config = ServicePlanConfiguration.GetInstance();

		internal static class UpgradeTaskInfoNames
		{
			internal const string ServicePlanUpgradeStart = "ServicePlanUpgradeStart";

			internal const string ServicePlanUpgradeEnd = "ServicePlanUpgradeEnd";

			internal const string RecreateCannedRBACRoles = "RecreateCannedRBACRoles";

			internal const string RecreateCannedRBACRoleAssignments = "RecreateCannedRBACRoleAssignments";

			internal const string RecreateCannedRBACRoleGroups = "RecreateCannedRBACRoleGroups";

			internal const string CreateNewMailboxPlanObject = "CreateNewMailboxPlanObject";

			internal const string UpdateMailboxes = "UpdateMailboxes";

			internal const string CleanupOldMailboxPlan = "CleanupOldMailboxPlan";

			internal const string MailboxPlans = "MailboxPlans";

			internal const string LoadSetupSnapin = "LoadSetupSnapin";

			internal const string PerMBXPlanRAPEnabled = "PerMBXPlanRoleAssignmentPolicyEnabled";

			internal const string PerMBXPlanRPEnabled = "PerMBXPlanRetentionPolicyEnabled";

			internal const string PerMBXPlanOWAEnabled = "PerMBXPlanOWAPolicyEnabled";

			internal const string ReducedMrmTagsEnabled = "ReducedOutOfTheBoxMrmTagsEnabled";

			internal const string PerMBXPlanOWAPolicyEnabled = "PerMBXPlanOWAPolicyEnabled";

			internal const string OwaInstantMessagingType = "OwaInstantMessagingType";

			internal const string PublicFoldersEnabled = "PublicFoldersEnabled";
		}
	}
}
