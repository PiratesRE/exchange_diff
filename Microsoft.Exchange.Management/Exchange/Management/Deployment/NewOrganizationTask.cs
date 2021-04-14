using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "Organization", SupportsShouldProcess = true, DefaultParameterSetName = "DatacenterParameterSet")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class NewOrganizationTask : ManageOrganizationTaskBase
	{
		public NewOrganizationTask()
		{
			base.Fields["InstallationMode"] = InstallationModes.Install;
			base.Fields["PrepareOrganization"] = true;
		}

		private ServicePlanConfiguration ServicePlanConfig
		{
			get
			{
				if (this.servicePlanConfig == null)
				{
					this.servicePlanConfig = ServicePlanConfiguration.GetInstance();
				}
				return this.servicePlanConfig;
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.NewOrganizationDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOrganizationNoPath(this.Name, this.DomainName);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SharedConfigurationParameterSet")]
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "DatacenterParameterSet")]
		public string Name
		{
			get
			{
				return (string)base.Fields["TenantName"];
			}
			set
			{
				base.Fields["TenantName"] = MailboxTaskHelper.GetNameOfAcceptableLengthForMultiTenantMode(value, out this.nameWarning);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SharedConfigurationParameterSet")]
		[Parameter(Mandatory = true, ParameterSetName = "DatacenterParameterSet")]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["TenantDomainName"];
			}
			set
			{
				base.Fields["TenantDomainName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public WindowsLiveId Administrator
		{
			get
			{
				return (WindowsLiveId)base.Fields["TenantAdministrator"];
			}
			set
			{
				base.Fields["TenantAdministrator"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NetID AdministratorNetID
		{
			get
			{
				return (NetID)base.Fields["TenantAdministratorNetID"];
			}
			set
			{
				base.Fields["TenantAdministratorNetID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString AdministratorPassword
		{
			get
			{
				return this.tenantAdministratorPassword;
			}
			set
			{
				this.tenantAdministratorPassword = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ProgramId
		{
			get
			{
				return (string)base.Fields["TenantProgramId"];
			}
			set
			{
				base.Fields["TenantProgramId"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string OfferId
		{
			get
			{
				return (string)base.Fields["TenantOfferId"];
			}
			set
			{
				base.Fields["TenantOfferId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Location
		{
			get
			{
				return (string)base.Fields["TenantLocation"];
			}
			set
			{
				base.Fields["TenantLocation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)base.Fields["TenantExternalDirectoryOrganizationId"];
			}
			set
			{
				base.Fields["TenantExternalDirectoryOrganizationId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSyncRunning
		{
			get
			{
				return (bool)(base.Fields["TenantIsDirSyncRunning"] ?? false);
			}
			set
			{
				base.Fields["TenantIsDirSyncRunning"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncStatus
		{
			get
			{
				return (string)base.Fields["TenantDirSyncStatus"];
			}
			set
			{
				base.Fields["TenantDirSyncStatus"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> CompanyTags
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["TenantCompanyTags"];
			}
			set
			{
				base.Fields["TenantCompanyTags"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)base.Fields["TenantPersistedCapabilities"];
			}
			set
			{
				base.Fields["TenantPersistedCapabilities"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutBoundOnly { get; set; }

		[Parameter(Mandatory = false)]
		public AuthenticationType AuthenticationType
		{
			get
			{
				return (AuthenticationType)(base.Fields["AuthenticationType"] ?? AuthenticationType.Managed);
			}
			set
			{
				base.Fields["AuthenticationType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LiveIdInstanceType LiveIdInstanceType
		{
			get
			{
				return (LiveIdInstanceType)(base.Fields["LiveIdInstanceType"] ?? LiveIdInstanceType.Consumer);
			}
			set
			{
				base.Fields["LiveIdInstanceType"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DatacenterParameterSet")]
		public SwitchParameter HotmailMigration
		{
			get
			{
				return (SwitchParameter)(base.Fields["HotmailMigration"] ?? false);
			}
			set
			{
				base.Fields["HotmailMigration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncServiceInstance
		{
			get
			{
				return (string)base.Fields["TenantDirSyncServiceInstance"];
			}
			set
			{
				base.Fields["TenantDirSyncServiceInstance"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SharedConfigurationParameterSet")]
		public SwitchParameter CreateSharedConfiguration
		{
			get
			{
				return (SwitchParameter)(base.Fields[ManageOrganizationTaskBase.ParameterCreateSharedConfig] ?? false);
			}
			set
			{
				base.Fields[ManageOrganizationTaskBase.ParameterCreateSharedConfig] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields["AccountPartition"];
			}
			set
			{
				base.Fields["AccountPartition"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] RMSOnlineConfig
		{
			get
			{
				return (byte[])base.Fields[NewOrganizationTask.ParameterRMSOnlineConfig];
			}
			set
			{
				base.Fields[NewOrganizationTask.ParameterRMSOnlineConfig] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Hashtable RMSOnlineKeys
		{
			get
			{
				return (Hashtable)base.Fields[NewOrganizationTask.ParameterRMSOnlineKeys];
			}
			set
			{
				base.Fields[NewOrganizationTask.ParameterRMSOnlineKeys] = value;
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

		private ITenantConfigurationSession WritableAllTenantsSessionForPartition(PartitionId partitionId)
		{
			return (partitionId == this.partition) ? this.WritableAllTenantsSession : DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 469, "WritableAllTenantsSessionForPartition", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\NewOrganizationTask.cs");
		}

		private ITenantConfigurationSession WritableAllTenantsSession
		{
			get
			{
				if (this.writableAllTenantsSession == null)
				{
					this.writableAllTenantsSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession((base.ServerSettings == null) ? null : base.ServerSettings.PreferredGlobalCatalog(this.partition.ForestFQDN), false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(this.partition), 485, "WritableAllTenantsSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\NewOrganizationTask.cs");
				}
				return this.writableAllTenantsSession;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new NewOrganizationTaskModuleFactory();
		}

		private static PartitionId ChoosePartition(string name, bool createSharedConfiguration, Task.TaskErrorLoggingDelegate errorLogger)
		{
			PartitionId[] allAccountPartitionIdsEnabledForProvisioning = ADAccountPartitionLocator.GetAllAccountPartitionIdsEnabledForProvisioning();
			if (createSharedConfiguration)
			{
				if (allAccountPartitionIdsEnabledForProvisioning.Length == 1)
				{
					return allAccountPartitionIdsEnabledForProvisioning[0];
				}
				if (string.IsNullOrEmpty(name))
				{
					return PartitionId.LocalForest;
				}
				errorLogger(new ArgumentException(Strings.ErrorAccountPartitionRequired), ErrorCategory.InvalidArgument, null);
			}
			return ADAccountPartitionLocator.SelectAccountPartitionForNewTenant(name);
		}

		private static PartitionId ResolvePartitionId(AccountPartitionIdParameter accountPartitionIdParameter, Task.TaskErrorLoggingDelegate errorLogger)
		{
			PartitionId result = null;
			LocalizedString? localizedString;
			IEnumerable<AccountPartition> objects = accountPartitionIdParameter.GetObjects<AccountPartition>(null, DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.SessionSettingsFactory.Default.FromRootOrgScopeSet(), 548, "ResolvePartitionId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\NewOrganizationTask.cs"), null, out localizedString);
			Exception ex = null;
			using (IEnumerator<AccountPartition> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AccountPartition accountPartition = enumerator.Current;
					if (!accountPartition.TryGetPartitionId(out result))
					{
						ex = new NotSupportedException(Strings.ErrorCorruptedPartition(accountPartitionIdParameter.ToString()));
					}
					else if (enumerator.MoveNext())
					{
						ex = new ManagementObjectAmbiguousException(Strings.ErrorObjectNotUnique(accountPartitionIdParameter.ToString()));
					}
					if (accountPartition.IsSecondary)
					{
						ex = new ArgumentException(Strings.ErrorSecondaryPartitionNotEnabledForProvisioning(accountPartitionIdParameter.RawIdentity));
					}
				}
				else
				{
					ex = new ManagementObjectNotFoundException(localizedString ?? Strings.ErrorObjectNotFound(accountPartitionIdParameter.ToString()));
				}
			}
			if (ex != null)
			{
				errorLogger(ex, ErrorCategory.InvalidArgument, accountPartitionIdParameter);
			}
			return result;
		}

		private void WriteWrappedError(Exception exception, ErrorCategory category, object target)
		{
			OrganizationValidationException exception2 = new OrganizationValidationException(Strings.ErrorValidationException(exception.ToString()), exception);
			base.WriteError(exception2, category, target);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.nameWarning != LocalizedString.Empty)
			{
				this.WriteWarning(this.nameWarning);
			}
			base.InternalBeginProcessing();
			if (this.Administrator != null)
			{
				OrganizationTaskHelper.ValidateParamString("Administrator", this.Administrator.ToString(), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.AdministratorNetID != null)
			{
				OrganizationTaskHelper.ValidateParamString("AdministratorNetID", this.AdministratorNetID.ToString(), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.AdministratorNetID != null && this.Administrator != null && this.Administrator.NetId != null && !this.AdministratorNetID.Equals(this.Administrator.NetId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNetIDValuesDoNotMatch(this.AdministratorNetID.ToString(), this.Administrator.NetId.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (this.AdministratorNetID != null && this.Administrator == null)
			{
				this.Administrator = new WindowsLiveId(this.AdministratorNetID.ToString());
			}
			if (base.Fields.IsModified("TenantDirSyncServiceInstance") && !string.IsNullOrEmpty(this.DirSyncServiceInstance) && !ServiceInstanceId.IsValidServiceInstanceId(this.DirSyncServiceInstance))
			{
				base.WriteError(new InvalidServiceInstanceIdException(this.DirSyncServiceInstance), ExchangeErrorCategory.Client, null);
			}
			if (this.ServicePlanConfig.IsPilotOffer(this.ProgramId, this.OfferId) && !this.CreateSharedConfiguration)
			{
				base.WriteError(new ArgumentException(Strings.ErrorPilotServicePlanCanBeUsedToCreateSharedOrgsOnly(this.ProgramId, this.OfferId)), (ErrorCategory)1000, null);
			}
			Exception ex = null;
			string text = null;
			if (base.Fields["TenantExternalDirectoryOrganizationId"] == null && !this.CreateSharedConfiguration)
			{
				base.Fields["TenantExternalDirectoryOrganizationId"] = Guid.NewGuid();
			}
			try
			{
				bool flag = this.ServicePlanConfig.TryGetHydratedOfferId(this.ProgramId, this.OfferId, out text);
				if (!this.CreateSharedConfiguration && this.Name == null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorNameNotSet), (ErrorCategory)1000, null);
				}
				this.partition = ((this.AccountPartition != null) ? NewOrganizationTask.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError)) : NewOrganizationTask.ChoosePartition(this.Name, this.CreateSharedConfiguration, new Task.TaskErrorLoggingDelegate(base.WriteError)));
				if (this.CreateSharedConfiguration && flag)
				{
					this.OfferId = text;
					this.shouldCreateSCT = NewOrganizationTask.ShouldCreateSharedConfiguration(this.ProgramId, this.OfferId, this.partition, out this.sctConfigUnit);
				}
				string text2 = this.ServicePlanConfig.ResolveServicePlanName(this.ProgramId, this.OfferId);
				this.servicePlanSettings = this.ServicePlanConfig.GetServicePlanSettings(text2);
				bool flag2 = this.ServicePlanConfig.IsTemplateTenantServicePlan(this.servicePlanSettings);
				if (flag2)
				{
					this.shouldCreateSCT = NewOrganizationTask.ShouldCreateTenantTemplate(this.ProgramId, this.OfferId, this.partition, out this.sctConfigUnit);
				}
				if (this.CreateSharedConfiguration)
				{
					if (!this.shouldCreateSCT)
					{
						this.WriteWarning(Strings.WarningSharedConfigurationAlreadyExists(this.ProgramId, this.OfferId));
						return;
					}
					if (string.IsNullOrEmpty(this.Name))
					{
						this.Name = (flag2 ? TemplateTenantConfiguration.CreateSharedConfigurationName(this.ProgramId, this.OfferId) : SharedConfiguration.CreateSharedConfigurationName(this.ProgramId, this.OfferId));
					}
					if (this.DomainName == null)
					{
						this.DomainName = (flag2 ? TemplateTenantConfiguration.CreateSharedConfigurationDomainName(this.Name) : SharedConfiguration.CreateSharedConfigurationDomainName(this.Name));
					}
				}
				OrganizationTaskHelper.ValidateParamString("Name", this.Name, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
				ADOrganizationalUnit adorganizationalUnit = new ADOrganizationalUnit();
				adorganizationalUnit[ADObjectSchema.Name] = this.Name;
				base.InternalIsSharedConfigServicePlan = this.ServicePlanConfig.IsSharedConfigurationAllowedForServicePlan(this.servicePlanSettings);
				if (this.CreateSharedConfiguration && !base.InternalIsSharedConfigServicePlan && !this.ServicePlanConfig.IsHydratedOffer(text2))
				{
					base.WriteError(new SharedConfigurationValidationException(Strings.ErrorServicePlanDoesntAllowSharedConfiguration(this.ProgramId, this.OfferId)), (ErrorCategory)1000, null);
				}
				else if (!flag && base.InternalIsSharedConfigServicePlan)
				{
					text = this.OfferId;
				}
				if (this.CreateSharedConfiguration)
				{
					base.InternalCreateSharedConfiguration = true;
				}
				else if (!this.CreateSharedConfiguration && base.InternalIsSharedConfigServicePlan)
				{
					SharedConfigurationInfo sharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(this.ProgramId, text);
					base.InternalSharedConfigurationId = SharedConfiguration.FindOneSharedConfigurationId(sharedConfigurationInfo, this.partition);
					if (base.InternalSharedConfigurationId == null)
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.ErrorSharedConfigurationNotFound(this.ProgramId, text, sharedConfigurationInfo.CurrentVersion.ToString())), (ErrorCategory)1000, null);
					}
					base.InternalCreateSharedConfiguration = false;
				}
				List<ValidationError> list = new List<ValidationError>();
				list.AddRange(ServicePlan.ValidateFileSchema(text2));
				list.AddRange(this.servicePlanSettings.Validate());
				if (list.Count != 0)
				{
					ex = new ArgumentException(Strings.ErrorServicePlanInconsistent(text2, this.ProgramId, this.OfferId, ValidationError.CombineErrorDescriptions(list)));
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
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
			base.InternalLocalStaticConfigEnabled = (!this.servicePlanSettings.Organization.AdvancedHydrateableObjectsSharedEnabled || this.CreateSharedConfiguration);
			base.InternalLocalHydrateableConfigEnabled = (!this.servicePlanSettings.Organization.CommonHydrateableObjectsSharedEnabled || this.CreateSharedConfiguration);
			TaskLogger.LogExit();
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			if (this.servicePlanSettings != null)
			{
				this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.ServicePlanSettingsVarName, this.servicePlanSettings);
			}
			if (this.tenantAdministratorPassword != null)
			{
				this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.TenantAdministratorPasswordVarName, this.tenantAdministratorPassword);
			}
			this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.ParameterRMSOnlineConfig, this.RMSOnlineConfig);
			this.monadConnection.RunspaceProxy.SetVariable(NewOrganizationTask.ParameterRMSOnlineKeys, this.RMSOnlineKeys);
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			if (this.servicePlanSettings != null)
			{
				base.Fields[NewOrganizationTask.ServicePlanVarName] = this.servicePlanSettings.Name;
			}
			base.Fields["PreferredServer"] = base.ServerSettings.PreferredGlobalCatalog(this.partition.ForestFQDN);
			base.Fields["OrganizationHierarchicalPath"] = this.Name;
			base.Fields["AuthenticationType"] = this.AuthenticationType;
			base.Fields["LiveIdInstanceType"] = this.LiveIdInstanceType;
			base.Fields["OutBoundOnly"] = this.OutBoundOnly;
			base.Fields["Partition"] = this.partition.PartitionObjectId;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.CreateSharedConfiguration && !this.shouldCreateSCT)
			{
				return;
			}
			if (this.ProgramId.IndexOf(".") != -1 || this.OfferId.IndexOf(".") != -1)
			{
				base.WriteError(new ArgumentException(Strings.ErrorParametersCannotHaveEmbeddedDot), ErrorCategory.InvalidArgument, null);
			}
			this.CheckForDuplicateExistingOrganization();
			NewAcceptedDomain.ValidateDomainName(new AcceptedDomain
			{
				DomainName = new SmtpDomainWithSubdomains(this.DomainName, false),
				DomainType = AcceptedDomainType.Authoritative
			}, new Task.TaskErrorLoggingDelegate(this.WriteWrappedError));
			bool flag = this.ServicePlanConfig.IsTemplateTenantServicePlan(this.servicePlanSettings);
			bool flag2 = TemplateTenantConfiguration.IsTemplateTenantName(this.Name);
			if (flag)
			{
				if (this.partition != PartitionId.LocalForest)
				{
					this.WriteWarning(Strings.ErrorLocalAccountPartitionRequiredForTT);
				}
				if (!this.CreateSharedConfiguration)
				{
					base.WriteError(new ArgumentException(Strings.CreateSharedConfigurationRequiredForTT), ErrorCategory.InvalidArgument, null);
				}
				if (!flag2)
				{
					base.WriteError(new ArgumentException(Strings.CalculatedNameRequiredForTT(TemplateTenantConfiguration.TopLevelDomain)), ErrorCategory.InvalidArgument, null);
				}
			}
			else if (flag2)
			{
				base.WriteError(new ArgumentException(Strings.TTNameWithNonTTServiceplan(TemplateTenantConfiguration.TopLevelDomain)), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.CreateSharedConfiguration && !this.shouldCreateSCT)
			{
				if (this.sctConfigUnit != null)
				{
					TenantOrganizationPresentationObject sendToPipeline = new TenantOrganizationPresentationObject(this.sctConfigUnit);
					base.WriteObject(sendToPipeline);
				}
				return;
			}
			try
			{
				base.InternalProcessRecord();
			}
			catch (Exception)
			{
				this.CleanupOrganization(this.partition);
				throw;
			}
			bool flag = !base.HasErrors;
			if (flag)
			{
				ITenantConfigurationSession tenantConfigurationSession = this.WritableAllTenantsSession;
				tenantConfigurationSession.UseConfigNC = false;
				ADObjectId childId = tenantConfigurationSession.GetHostedOrganizationsRoot().GetChildId("OU", this.Name);
				ADOrganizationalUnit adorganizationalUnit = tenantConfigurationSession.Read<ADOrganizationalUnit>(childId);
				tenantConfigurationSession.UseConfigNC = true;
				ExchangeConfigurationUnit dataObject = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(adorganizationalUnit.ConfigurationUnit);
				TenantOrganizationPresentationObject sendToPipeline2 = new TenantOrganizationPresentationObject(dataObject);
				base.WriteObject(sendToPipeline2);
			}
			else
			{
				this.CleanupOrganization(this.partition);
			}
			TaskLogger.LogExit();
		}

		private void CleanupOrganization(PartitionId partitionId)
		{
			ITenantConfigurationSession tenantConfigurationSession = this.WritableAllTenantsSessionForPartition(partitionId);
			ADObjectId childId = tenantConfigurationSession.GetHostedOrganizationsRoot().GetChildId("OU", this.Name);
			ADOrganizationalUnit adorganizationalUnit = this.ReadADOrganizationalUnit(tenantConfigurationSession, childId);
			Container objectToDelete = null;
			if (adorganizationalUnit != null && adorganizationalUnit.ConfigurationUnit != null)
			{
				tenantConfigurationSession.UseConfigNC = true;
				ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(adorganizationalUnit.ConfigurationUnit);
				if (exchangeConfigurationUnit != null)
				{
					if (ExchangeConfigurationUnit.IsOrganizationActive(exchangeConfigurationUnit.OrganizationStatus))
					{
						base.WriteError(new OrganizationExistsException(Strings.ErrorDuplicateActiveOrganizationExists(this.Name)), ErrorCategory.InvalidArgument, null);
					}
					try
					{
						exchangeConfigurationUnit.ExternalDirectoryOrganizationId = string.Empty;
						tenantConfigurationSession.Save(exchangeConfigurationUnit);
					}
					catch (Exception)
					{
					}
					objectToDelete = this.ReadContainer(tenantConfigurationSession, adorganizationalUnit.ConfigurationUnit.Parent);
				}
			}
			try
			{
				this.DeleteTree(tenantConfigurationSession, adorganizationalUnit, false);
			}
			catch (Exception)
			{
			}
			try
			{
				this.DeleteTree(tenantConfigurationSession, objectToDelete, true);
			}
			catch (Exception)
			{
			}
		}

		private static bool ShouldCreateSharedConfiguration(string programId, string offerId, PartitionId partitionId, out ExchangeConfigurationUnit sctConfigUnit)
		{
			sctConfigUnit = null;
			ADDriverContext processADContext = ADSessionSettings.GetProcessADContext();
			if (processADContext == null || processADContext.Mode != ContextMode.Setup)
			{
				return true;
			}
			SharedConfigurationInfo sci = SharedConfigurationInfo.FromInstalledVersion(programId, offerId);
			sctConfigUnit = SharedConfiguration.FindOneSharedConfiguration(sci, partitionId);
			return sctConfigUnit == null;
		}

		private static bool ShouldCreateTenantTemplate(string programId, string offerId, PartitionId partitionId, out ExchangeConfigurationUnit sctConfigUnit)
		{
			sctConfigUnit = null;
			ADDriverContext processADContext = ADSessionSettings.GetProcessADContext();
			if (processADContext == null || processADContext.Mode != ContextMode.Setup)
			{
				return true;
			}
			ADPagedReader<ExchangeConfigurationUnit> adpagedReader = TemplateTenantConfiguration.FindAllTempateTenants(programId, offerId, partitionId);
			foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in adpagedReader)
			{
				if (exchangeConfigurationUnit.SharedConfigurationInfo != null && ((IComparable)TemplateTenantConfiguration.RequiredTemplateTenantVersion).CompareTo(exchangeConfigurationUnit.SharedConfigurationInfo.CurrentVersion) <= 0)
				{
					sctConfigUnit = exchangeConfigurationUnit;
					return false;
				}
			}
			return true;
		}

		private void CheckForDuplicateExistingOrganization()
		{
			ITenantConfigurationSession tenantConfigurationSession = this.WritableAllTenantsSessionForPartition(this.partition);
			ADObjectId childId = tenantConfigurationSession.GetHostedOrganizationsRoot().GetChildId("OU", this.Name);
			ADOrganizationalUnit oufromOrganizationId = OrganizationTaskHelper.GetOUFromOrganizationId(new OrganizationIdParameter(childId), tenantConfigurationSession, null, false);
			if (oufromOrganizationId == null)
			{
				this.CleanupOrganization(this.partition);
				return;
			}
			if (OrganizationTaskHelper.CanProceedWithOrganizationTask(new OrganizationIdParameter(this.Name), tenantConfigurationSession, NewOrganizationTask.ignorableFlagsOnStatusTimeout, new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				this.CleanupOrganization(this.partition);
				return;
			}
			base.WriteError(new OrganizationPendingOperationException(Strings.ErrorDuplicateNonActiveOrganizationExists(this.Name)), ErrorCategory.InvalidArgument, null);
		}

		private void DeleteContainer(ADConfigurationObject container, bool useConfigNC)
		{
			if (container != null)
			{
				PartitionId partitionId = new PartitionId(container.Id.PartitionGuid);
				ITenantConfigurationSession session = this.WritableAllTenantsSessionForPartition(partitionId);
				this.DeleteTree(session, container, useConfigNC);
			}
		}

		private ADOrganizationalUnit ReadADOrganizationalUnit(ITenantConfigurationSession session, ADObjectId objectId)
		{
			ADOrganizationalUnit result = null;
			bool useConfigNC = session.UseConfigNC;
			session.UseConfigNC = false;
			try
			{
				result = session.Read<ADOrganizationalUnit>(objectId);
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
			return result;
		}

		private Container ReadContainer(ITenantConfigurationSession session, ADObjectId objectId)
		{
			Container result = null;
			bool useConfigNC = session.UseConfigNC;
			session.UseConfigNC = true;
			try
			{
				result = session.Read<Container>(objectId);
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
			return result;
		}

		private void DeleteTree(ITenantConfigurationSession session, ADConfigurationObject objectToDelete, bool useConfigNC)
		{
			if (objectToDelete != null)
			{
				bool useConfigNC2 = session.UseConfigNC;
				session.UseConfigNC = useConfigNC;
				try
				{
					session.DeleteTree(objectToDelete, null);
				}
				finally
				{
					session.UseConfigNC = useConfigNC2;
				}
			}
		}

		private const string SharedConfigurationParameterSet = "SharedConfigurationParameterSet";

		private const string DatacenterParameterSet = "DatacenterParameterSet";

		private ITenantConfigurationSession writableAllTenantsSession;

		private ServicePlan servicePlanSettings;

		private bool shouldCreateSCT;

		private ExchangeConfigurationUnit sctConfigUnit;

		private ServicePlanConfiguration servicePlanConfig;

		internal static readonly string ServicePlanSettingsVarName = "ServicePlanSettings";

		internal static readonly string ServicePlanVarName = "TenantServicePlanName";

		internal static readonly string TenantAdministratorPasswordVarName = "TenantAdministratorPassword";

		internal static readonly string ParameterRMSOnlineConfig = "RMSOnlineConfig";

		internal static readonly string ParameterRMSOnlineKeys = "RMSOnlineKeys";

		private LocalizedString nameWarning = LocalizedString.Empty;

		private static OrganizationStatus[] ignorableFlagsOnStatusTimeout = new OrganizationStatus[]
		{
			OrganizationStatus.PendingCompletion
		};

		private SecureString tenantAdministratorPassword;

		private PartitionId partition;
	}
}
