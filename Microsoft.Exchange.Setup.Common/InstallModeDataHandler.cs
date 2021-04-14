using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.Parser;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class InstallModeDataHandler : ModeDataHandler
	{
		public InstallModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
			SetupLogger.Log(Strings.ApplyingDefaultRoleSelectionState);
			foreach (Role role in base.SetupContext.RequestedRoles)
			{
				bool flag = true;
				if (base.SetupContext.IsInstalledLocal(role.RoleName))
				{
					flag = false;
				}
				string roleName;
				switch (roleName = role.RoleName)
				{
				case "BridgeheadRole":
					base.SetIsBridgeheadCheckedInternal(flag);
					break;
				case "ClientAccessRole":
					base.SetIsClientAccessCheckedInternal(flag);
					break;
				case "GatewayRole":
					base.SetIsGatewayCheckedInternal(flag);
					break;
				case "MailboxRole":
					base.SetIsMailboxCheckedInternal(flag);
					break;
				case "UnifiedMessagingRole":
					base.SetIsUnifiedMessagingCheckedInternal(flag);
					break;
				case "FrontendTransportRole":
					base.SetIsFrontendTransportCheckedInternal(flag);
					break;
				case "CafeRole":
					base.SetIsCafeCheckedInternal(flag);
					break;
				case "AdminToolsRole":
					base.SetIsAdminToolsCheckedInternal(flag);
					break;
				case "CentralAdminRole":
					base.SetIsCentralAdminCheckedInternal(flag);
					break;
				case "CentralAdminDatabaseRole":
					base.SetIsCentralAdminDatabaseCheckedInternal(flag);
					break;
				case "CentralAdminFrontEndRole":
					base.SetIsCentralAdminFrontEndCheckedInternal(flag);
					break;
				case "MonitoringRole":
					base.SetIsMonitoringCheckedInternal(flag);
					break;
				case "OSPRole":
					base.SetIsOSPCheckedInternal(true);
					break;
				}
			}
			this.SetPartiallyConfiguredRoleChecked();
			if (base.CanChangeInstallationPath && base.SetupContext.ParsedArguments.ContainsKey("targetdir"))
			{
				base.InstallationPath = (NonRootLocalLongFullPath)base.SetupContext.ParsedArguments["targetdir"];
				SetupLogger.Log(Strings.UserSpecifiedTargetDir(base.InstallationPath.PathName));
			}
			this.watsonEnabled = base.SetupContext.WatsonEnabled;
			this.hasRolesToInstall = base.SetupContext.HasRolesToInstall;
			if (base.SetupContext.ParsedArguments.ContainsKey("customerfeedbackenabled"))
			{
				this.CustomerFeedbackEnabled = new bool?((bool)base.SetupContext.ParsedArguments["customerfeedbackenabled"]);
			}
			base.SetupContext.Industry = IndustryType.NotSpecified;
		}

		public static object ParseFqdn(string fqdnStr)
		{
			Fqdn result;
			try
			{
				result = Fqdn.Parse(fqdnStr);
			}
			catch (FormatException innerException)
			{
				throw new ParseException(Strings.NotAValidFqdn(fqdnStr), innerException);
			}
			return result;
		}

		public override List<string> SelectedInstallableUnits
		{
			get
			{
				base.SelectedInstallableUnits.Clear();
				bool flag = false;
				if (base.IsAdminToolsChecked)
				{
					Role roleByName = RoleManager.GetRoleByName("AdminToolsRole");
					if (this.IsAdminToolsEnabled || !roleByName.IsInstalled)
					{
						base.SelectedInstallableUnits.Add("AdminToolsRole");
						flag = true;
					}
				}
				if (this.IsBridgeheadEnabled && base.IsBridgeheadChecked)
				{
					base.SelectedInstallableUnits.Add("BridgeheadRole");
					flag = true;
				}
				if (this.IsClientAccessEnabled && base.IsClientAccessChecked)
				{
					base.SelectedInstallableUnits.Add("ClientAccessRole");
					flag = true;
				}
				if (this.IsGatewayEnabled && base.IsGatewayChecked)
				{
					base.SelectedInstallableUnits.Add("GatewayRole");
					flag = true;
				}
				if (this.IsUnifiedMessagingEnabled && base.IsUnifiedMessagingChecked)
				{
					base.SelectedInstallableUnits.Add("UnifiedMessagingRole");
					flag = true;
				}
				if (this.IsMailboxEnabled && base.IsMailboxChecked)
				{
					base.SelectedInstallableUnits.Add("MailboxRole");
					flag = true;
				}
				if (this.IsFrontendTransportEnabled && base.IsFrontendTransportChecked)
				{
					base.SelectedInstallableUnits.Add("FrontendTransportRole");
					flag = true;
				}
				if (this.IsCafeEnabled && base.IsCafeChecked)
				{
					base.SelectedInstallableUnits.Add("CafeRole");
					flag = true;
				}
				if (this.IsCentralAdminDatabaseEnabled && base.IsCentralAdminDatabaseChecked)
				{
					base.SelectedInstallableUnits.Add("CentralAdminDatabaseRole");
					flag = true;
				}
				if (this.IsCentralAdminEnabled && base.IsCentralAdminChecked)
				{
					base.SelectedInstallableUnits.Add("CentralAdminRole");
					flag = true;
				}
				if (this.IsCentralAdminFrontEndEnabled && base.IsCentralAdminFrontEndChecked)
				{
					base.SelectedInstallableUnits.Add("CentralAdminFrontEndRole");
					flag = true;
				}
				if (this.IsMonitoringEnabled && base.IsMonitoringChecked)
				{
					base.SelectedInstallableUnits.Add("MonitoringRole");
					flag = true;
				}
				if ((flag || base.SetupContext.IsLanguagePackOperation) && base.IsLanguagePacksChecked && base.SetupContext.NeedToUpdateLanguagePacks)
				{
					base.SelectedInstallableUnits.Insert(0, "LanguagePacks");
				}
				if (this.IsOSPEnabled && base.IsOSPChecked)
				{
					base.SelectedInstallableUnits.Add("OSPRole");
				}
				return base.SelectedInstallableUnits;
			}
		}

		public override InstallationModes Mode
		{
			get
			{
				return InstallationModes.Install;
			}
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new InstallPreCheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
			}
		}

		protected override bool NeedPrePostSetupDataHandlers
		{
			get
			{
				return base.NeedPrePostSetupDataHandlers && !base.SetupContext.HasPrepareADParameters && !base.SetupContext.HasNewProvisionedServerParameters;
			}
		}

		protected override bool NeedFileDataHandler()
		{
			if (!base.IsLanguagePackOnlyInstallation)
			{
				return base.NeedFileDataHandler() && !base.SetupContext.HasPrepareADParameters && !base.SetupContext.HasNewProvisionedServerParameters && !this.RequestedRolesAreAllUnpacked();
			}
			return base.NeedFileDataHandler() && !base.SetupContext.HasPrepareADParameters && !base.SetupContext.HasNewProvisionedServerParameters;
		}

		private bool RequestedRolesAreAllUnpacked()
		{
			foreach (string roleName in this.SelectedInstallableUnits)
			{
				Role roleByName = RoleManager.GetRoleByName(roleName);
				if (roleByName != null && (!base.SetupContext.UnpackedRoles.Contains(roleByName) || (base.SetupContext.IsDatacenter && !base.SetupContext.UnpackedDatacenterRoles.Contains(roleByName))))
				{
					return false;
				}
			}
			return true;
		}

		protected override void AddNeededFileDataHandlers()
		{
			base.AddNeededFileDataHandlers();
			if (this.preFileCopyDataHandler == null)
			{
				this.preFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PreFileCopy, base.SetupContext, base.Connection);
			}
			this.preFileCopyDataHandler.Mode = this.Mode;
			this.preFileCopyDataHandler.PreviousVersion = base.PreviousVersion;
			this.preFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.preFileCopyDataHandler);
			if (!base.IsLanguagePackOnlyInstallation)
			{
				if (this.installFileDataHandler == null)
				{
					this.installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new ProductMsiConfigurationInfo(), base.Connection);
				}
				this.installFileDataHandler.SelectedInstallableUnits.Clear();
				foreach (string text in this.SelectedInstallableUnits.ToArray())
				{
					Role roleByName = RoleManager.GetRoleByName(text);
					if (roleByName == null || !RoleManager.GetRoleByName(text).IsDatacenterOnly)
					{
						this.installFileDataHandler.SelectedInstallableUnits.Add(text);
					}
				}
				this.installFileDataHandler.TargetDirectory = base.InstallationPath.PathName;
				this.installFileDataHandler.WatsonEnabled = this.WatsonEnabled;
				base.DataHandlers.Add(this.installFileDataHandler);
			}
			if (base.SetupContext.IsDatacenter || base.SetupContext.IsDatacenterDedicated)
			{
				InstallFileDataHandler installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new DatacenterMsiConfigurationInfo(), base.Connection);
				installFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				installFileDataHandler.TargetDirectory = base.InstallationPath.PathName;
				base.DataHandlers.Add(installFileDataHandler);
			}
		}

		private void AddOrgLevelConfigurationDataHandlers()
		{
			if (!this.SelectedInstallableUnits.Contains("GatewayRole"))
			{
				if (base.SetupContext.HasNewProvisionedServerParameters)
				{
					if (this.provisionServerDataHandler == null)
					{
						this.provisionServerDataHandler = new ProvisionServerDataHandler(base.SetupContext, base.Connection);
					}
					this.provisionServerDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
					base.DataHandlers.Add(this.provisionServerDataHandler);
					return;
				}
				if (base.OrgLevelConfigRequired())
				{
					base.DataHandlers.Add(this.InstallOrgCfgDataHandler);
				}
			}
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler result = null;
			switch (installableUnitName)
			{
			case "BridgeheadRole":
				return this.InstallBhdCfgDataHandler;
			case "ClientAccessRole":
				return this.InstallCacCfgDataHandler;
			case "GatewayRole":
				return this.InstallGwyCfgDataHandler;
			case "MailboxRole":
				return this.InstallMbxCfgDataHandler;
			case "UnifiedMessagingRole":
				return this.InstallUMCfgDataHandler;
			case "FrontendTransportRole":
				return this.InstallFetCfgDataHandler;
			case "CafeRole":
				return this.InstallCafeCfgDataHandler;
			case "AdminToolsRole":
				return this.InstallAtCfgDataHandler;
			case "LanguagePacks":
				return this.AddLanguagePackCfgDataHandler;
			case "CentralAdminRole":
				return this.InstallCentralAdminCfgDataHandler;
			case "CentralAdminDatabaseRole":
				return this.InstallCentralAdminDatabaseCfgDataHandler;
			case "CentralAdminFrontEndRole":
				return this.InstallCentralAdminFrontEndCfgDataHandler;
			case "MonitoringRole":
				return this.InstallMonitoringCfgDataHandler;
			case "OSPRole":
				return this.InstallOSPCfgDataHandler;
			}
			if (InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName))
			{
				InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
				UmLanguagePackConfigurationInfo umLanguagePackConfigurationInfo = installableUnitConfigurationInfoByName as UmLanguagePackConfigurationInfo;
				AddUmLanguagePackCfgDataHandler addUmLanguagePackCfgDataHandler;
				if (!this.addUmLanguagePackDataHandlers.TryGetValue(installableUnitName, out addUmLanguagePackCfgDataHandler))
				{
					addUmLanguagePackCfgDataHandler = new AddUmLanguagePackCfgDataHandler(base.SetupContext, base.Connection, umLanguagePackConfigurationInfo.Culture, base.UmSourceDir);
					this.addUmLanguagePackDataHandlers.Add(installableUnitName, addUmLanguagePackCfgDataHandler);
				}
				result = addUmLanguagePackCfgDataHandler;
			}
			return result;
		}

		public InstallOrgCfgDataHandler InstallOrgCfgDataHandler
		{
			get
			{
				if (this.installOrgCfgDataHandler == null)
				{
					this.installOrgCfgDataHandler = new InstallOrgCfgDataHandler(base.SetupContext, base.Connection);
				}
				this.installOrgCfgDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				return this.installOrgCfgDataHandler;
			}
		}

		public InstallBhdCfgDataHandler InstallBhdCfgDataHandler
		{
			get
			{
				if (this.installBhdCfgDataHandler == null)
				{
					this.installBhdCfgDataHandler = new InstallBhdCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installBhdCfgDataHandler;
			}
		}

		public InstallCacCfgDataHandler InstallCacCfgDataHandler
		{
			get
			{
				if (this.installCacCfgDataHandler == null)
				{
					this.installCacCfgDataHandler = new InstallCacCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installCacCfgDataHandler;
			}
		}

		public InstallGwyCfgDataHandler InstallGwyCfgDataHandler
		{
			get
			{
				if (this.installGwyCfgDataHandler == null)
				{
					this.installGwyCfgDataHandler = new InstallGwyCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installGwyCfgDataHandler;
			}
		}

		public InstallMbxCfgDataHandler InstallMbxCfgDataHandler
		{
			get
			{
				if (this.installMbxCfgDataHandler == null)
				{
					this.installMbxCfgDataHandler = new InstallMbxCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installMbxCfgDataHandler;
			}
		}

		public InstallUMCfgDataHandler InstallUMCfgDataHandler
		{
			get
			{
				if (this.installUMCfgDataHandler == null)
				{
					this.installUMCfgDataHandler = new InstallUMCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installUMCfgDataHandler;
			}
		}

		public InstallFetCfgDataHandler InstallFetCfgDataHandler
		{
			get
			{
				if (this.installFetCfgDataHandler == null)
				{
					this.installFetCfgDataHandler = new InstallFetCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installFetCfgDataHandler;
			}
		}

		public InstallCafeCfgDataHandler InstallCafeCfgDataHandler
		{
			get
			{
				if (this.installCafeCfgDataHandler == null)
				{
					this.installCafeCfgDataHandler = new InstallCafeCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installCafeCfgDataHandler;
			}
		}

		public InstallAtCfgDataHandler InstallAtCfgDataHandler
		{
			get
			{
				if (this.installAtCfgDataHandler == null)
				{
					this.installAtCfgDataHandler = new InstallAtCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installAtCfgDataHandler;
			}
		}

		public InstallCentralAdminDatabaseCfgDataHandler InstallCentralAdminDatabaseCfgDataHandler
		{
			get
			{
				if (this.installCentralAdminDatabaseCfgDataHandler == null)
				{
					this.installCentralAdminDatabaseCfgDataHandler = new InstallCentralAdminDatabaseCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installCentralAdminDatabaseCfgDataHandler;
			}
		}

		public InstallCentralAdminCfgDataHandler InstallCentralAdminCfgDataHandler
		{
			get
			{
				if (this.installCentralAdminCfgDataHandler == null)
				{
					this.installCentralAdminCfgDataHandler = new InstallCentralAdminCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installCentralAdminCfgDataHandler;
			}
		}

		public InstallCentralAdminFrontEndCfgDataHandler InstallCentralAdminFrontEndCfgDataHandler
		{
			get
			{
				if (this.installCentralAdminFrontEndCfgDataHandler == null)
				{
					this.installCentralAdminFrontEndCfgDataHandler = new InstallCentralAdminFrontEndCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installCentralAdminFrontEndCfgDataHandler;
			}
		}

		public InstallMonitoringCfgDataHandler InstallMonitoringCfgDataHandler
		{
			get
			{
				if (this.installMonitoringCfgDataHandler == null)
				{
					this.installMonitoringCfgDataHandler = new InstallMonitoringCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installMonitoringCfgDataHandler;
			}
		}

		public AddLanguagePackCfgDataHandler AddLanguagePackCfgDataHandler
		{
			get
			{
				if (this.addLanguagePackCfgDataHandler == null)
				{
					this.addLanguagePackCfgDataHandler = new AddLanguagePackCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.addLanguagePackCfgDataHandler;
			}
		}

		public InstallOSPCfgDataHandler InstallOSPCfgDataHandler
		{
			get
			{
				if (this.installOSPCfgDataHandler == null)
				{
					this.installOSPCfgDataHandler = new InstallOSPCfgDataHandler(base.SetupContext, base.Connection);
				}
				return this.installOSPCfgDataHandler;
			}
		}

		public bool TypicalInstallation
		{
			get
			{
				return this.typicalInstallation;
			}
			set
			{
				if (value || value != this.typicalInstallation)
				{
					this.typicalInstallation = value;
					base.IsBridgeheadChecked = this.TypicalInstallation;
					base.IsMailboxChecked = this.TypicalInstallation;
					base.IsGatewayChecked = false;
					base.IsUnifiedMessagingChecked = false;
					base.IsClientAccessChecked = this.TypicalInstallation;
					base.IsAdminToolsChecked = this.TypicalInstallation;
					base.IsLanguagePacksChecked = this.TypicalInstallation;
				}
			}
		}

		public override bool IsBridgeheadEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocalOrAD("BridgeheadRole") | base.SetupContext.IsPartiallyConfigured("BridgeheadRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsClientAccessEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocalOrAD("ClientAccessRole") | base.SetupContext.IsPartiallyConfigured("ClientAccessRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsGatewayEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocalOrAD("GatewayRole") | base.SetupContext.IsPartiallyConfigured("GatewayRole")) && !base.IsBridgeheadChecked && !base.IsClientAccessChecked && !base.IsMailboxChecked && !base.IsUnifiedMessagingChecked;
			}
		}

		public override bool IsMailboxEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocalOrAD("MailboxRole") | base.SetupContext.IsPartiallyConfigured("MailboxRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsUnifiedMessagingEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocalOrAD("UnifiedMessagingRole") | base.SetupContext.IsPartiallyConfigured("UnifiedMessagingRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsFrontendTransportEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("FrontendTransportRole") | base.SetupContext.IsPartiallyConfigured("FrontendTransportRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsCentralAdminEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("CentralAdminRole") | base.SetupContext.IsPartiallyConfigured("CentralAdminRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsCentralAdminDatabaseEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("CentralAdminDatabaseRole") | base.SetupContext.IsPartiallyConfigured("CentralAdminDatabaseRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsCentralAdminFrontEndEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("CentralAdminFrontEndRole") | base.SetupContext.IsPartiallyConfigured("CentralAdminFrontEndRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsMonitoringEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("MonitoringRole") | base.SetupContext.IsPartiallyConfigured("MonitoringRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsCafeEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("CafeRole") | base.SetupContext.IsPartiallyConfigured("CafeRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool IsAdminToolsEnabled
		{
			get
			{
				return !base.SetupContext.IsInstalledLocal("AdminToolsRole") && !base.IsMailboxChecked && !base.IsClientAccessChecked && !base.IsBridgeheadChecked && !base.IsGatewayChecked && !base.IsUnifiedMessagingChecked;
			}
		}

		public override bool IsOSPEnabled
		{
			get
			{
				return (!base.SetupContext.IsInstalledLocal("OSPRole") | base.SetupContext.IsPartiallyConfigured("OSPRole")) && !base.IsGatewayChecked;
			}
		}

		public override bool CanChangeSourceDir
		{
			get
			{
				return !base.SetupContext.IsCleanMachine;
			}
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.AddServerRoleText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				if (this.hasRolesToInstall)
				{
					return Strings.AddRolesToInstall;
				}
				if (base.IsLanguagePackOnlyInstallation)
				{
					return Strings.LanguagePacksToInstall;
				}
				if (base.SetupContext.IsLanguagePackOperation)
				{
					return Strings.LanguagePacksUpToDate;
				}
				return Strings.NoServerRolesToInstall;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				if (base.IsLanguagePackOnlyInstallation)
				{
					return base.WorkUnits.HasFailures ? Strings.AddLanguagePacksFailText : Strings.AddLanguagePacksSuccessText;
				}
				return base.WorkUnits.HasFailures ? Strings.AddFailText : Strings.AddSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.AddFailStatus : Strings.AddSuccessStatus;
			}
		}

		public bool WatsonEnabled
		{
			get
			{
				return this.watsonEnabled;
			}
			set
			{
				this.watsonEnabled = value;
			}
		}

		public bool WillSetGlobalCEIP
		{
			get
			{
				return base.SetupContext.ParsedArguments.ContainsKey("preparead") || !base.ExchangeOrganizationExists || (base.SetupContext.IsOrgConfigUpdateRequired && !base.SetupContext.HasE14OrLaterServers);
			}
		}

		public LocalizedString OptDescription
		{
			get
			{
				if (this.WillSetGlobalCEIP)
				{
					return Strings.GlobalOptDescriptionText;
				}
				return Strings.ServerOptDescriptionText;
			}
		}

		public IndustryType Industry
		{
			get
			{
				return base.SetupContext.Industry;
			}
		}

		public bool? OriginalGlobalCustomerFeedbackEnabled
		{
			get
			{
				return base.SetupContext.OriginalGlobalCustomerFeedbackEnabled;
			}
		}

		public bool? CustomerFeedbackEnabled
		{
			get
			{
				if (this.WillSetGlobalCEIP)
				{
					return base.SetupContext.GlobalCustomerFeedbackEnabled;
				}
				if (base.SetupContext.ServerCustomerFeedbackEnabled == null && base.SetupContext.IsCleanMachine)
				{
					return base.SetupContext.GlobalCustomerFeedbackEnabled;
				}
				return base.SetupContext.ServerCustomerFeedbackEnabled;
			}
			set
			{
				if (this.CustomerFeedbackEnabled != value)
				{
					if (this.WillSetGlobalCEIP)
					{
						base.SetupContext.GlobalCustomerFeedbackEnabled = value;
						base.SetupContext.ServerCustomerFeedbackEnabled = value;
						return;
					}
					base.SetupContext.ServerCustomerFeedbackEnabled = value;
				}
			}
		}

		public override bool RebootRequired
		{
			get
			{
				if (!base.WorkUnits.IsDataChanged)
				{
					return false;
				}
				bool flag = base.SetupContext.IsUnpacked("AdminToolsRole") && base.SetupContext.UnpackedRoles.Count == 1;
				bool flag2 = (base.SetupContext.IsCleanMachine || flag) && base.ServerRoleIsSelected;
				bool flag3 = this.SelectedInstallableUnits.Contains("MailboxRole") && !base.SetupContext.IsUnpacked("MailboxRole");
				return flag2 || flag3;
			}
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			SetupLogger.TraceEnter(new object[0]);
			if (!base.IsLanguagePackOnlyInstallation && this.SelectedInstallableUnits.Contains("AdminToolsRole") && !base.ServerRoleIsSelected)
			{
				PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
				instance.AddRoleByUnitName("AdminToolsRole");
				instance.TargetDir = base.SetupContext.TargetDir;
				instance.AddSelectedInstallableUnits(this.SelectedInstallableUnits);
			}
			SetupLogger.TraceExit();
		}

		protected override void UpdateDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.DataHandlers.Clear();
			if (!base.IsLanguagePacksChecked && (base.SetupContext.IsCleanMachine || base.SetupContext.IsLanguagePackOperation || base.SetupContext.InstalledLanguagePacks.Count == 0 || base.SetupContext.LanguagePacksToInstall.Keys.Count != 0 || !base.SetupContext.InstalledLanguagePacks.Contains("{DEDFFB64-42EC-4E26-0409-430E86DF378C}") || !base.SetupContext.InstalledLanguagePacks.Contains("{521E6064-B4B1-4CBC-0409-25AD697801FA}")))
			{
				base.SetIsLanguagePacksCheckedInternal(true);
			}
			this.AddOrgLevelConfigurationDataHandlers();
			if (base.DataHandlers.Count > 0 || base.SetupContext.IsCleanMachine)
			{
				ConfigurationContext.Setup.UseAssemblyPathAsInstallPath();
			}
			SetupLogger.Log(Strings.SetupWillRunFromPath(ConfigurationContext.Setup.InstallPath));
			base.AddPreSetupDataHandlers();
			base.AddFileDataHandlers();
			if (base.IsLanguagePacksChecked)
			{
				base.AddLanguagePackFileDataHandlers();
			}
			base.AddPostCopyFileDataHandlers();
			this.AddConfigurationDataHandlers();
			base.AddPostSetupDataHandlers();
			SetupLogger.Log(Strings.InstallModeDataHandlerCount(base.DataHandlers.Count));
			SetupLogger.TraceExit();
		}

		public override bool HasChanges
		{
			get
			{
				return base.SetupContext.HasPrepareADParameters || base.SetupContext.HasNewProvisionedServerParameters || this.SelectedInstallableUnits.Count > 0;
			}
		}

		public bool WillInstallOnlyAdminAndLanguagePacks
		{
			get
			{
				bool result = false;
				if (this.SelectedInstallableUnits.Count <= 2)
				{
					int num = 0;
					if (this.SelectedInstallableUnits.Contains("AdminToolsRole"))
					{
						num++;
					}
					if (this.SelectedInstallableUnits.Contains("LanguagePacks"))
					{
						num++;
					}
					if (num == this.SelectedInstallableUnits.Count)
					{
						result = true;
					}
				}
				return result;
			}
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (!base.IgnoreValidatingRoleChanges)
			{
				SetupLogger.Log(Strings.ValidatingOptionsForRoles(base.SetupContext.RequestedRoles.Count));
				bool flag = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "GatewayRole") || this.SelectedInstallableUnits.Contains("GatewayRole");
				bool flag2;
				if (!base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName != "GatewayRole" && current.RoleName != "AdminToolsRole" && current.RoleName != "LanguagePacks"))
				{
					flag2 = this.SelectedInstallableUnits.Exists((string current) => current != "GatewayRole" && current != "AdminToolsRole" && current != "LanguagePacks");
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag && flag3)
				{
					list.Add(new SetupValidationError(Strings.AddConflictedRolesError));
				}
				else if (base.SetupContext.IsInstalledLocal("GatewayRole"))
				{
					if (base.SetupContext.RequestedRoles.Count > 0)
					{
						list.Add(new SetupValidationError(Strings.AddOtherRolesError));
					}
				}
				else if (base.SetupContext.IsUnpacked("GatewayRole"))
				{
					if (flag3)
					{
						list.Add(new SetupValidationError(Strings.AddOtherRolesError));
					}
				}
				else if (flag && base.SetupContext.UnpackedRoles.Count > 1)
				{
					list.Add(new SetupValidationError(Strings.AddGatewayAloneError));
				}
				if (!this.HasChanges || (base.SetupContext.IsCleanMachine && this.SelectedInstallableUnits.Count == 1 && (this.SelectedInstallableUnits.Contains("LanguagePacks") || this.SelectedInstallableUnits.Contains("UmLanguagePack"))))
				{
					list.Add(new SetupValidationError(Strings.NoRoleSelectedForInstall));
				}
			}
			if (!base.CanChangeInstallationPath && base.SetupContext.ParsedArguments.ContainsKey("targetdir") && (NonRootLocalLongFullPath)base.SetupContext.ParsedArguments["targetdir"] != base.InstallationPath)
			{
				list.Add(new SetupValidationError(Strings.AddCannotChangeTargetDirectoryError));
			}
			if (this.IsUnifiedMessagingEnabled && base.IsUnifiedMessagingChecked && base.UmSourceDir != null && !Directory.Exists(base.UmSourceDir.PathName))
			{
				list.Add(new SetupValidationError(Strings.CannotFindPath(base.UmSourceDir.PathName)));
			}
			if (!base.SetupContext.IsDatacenter)
			{
				foreach (Role role in base.SetupContext.RequestedRoles)
				{
					if (role.IsDatacenterOnly)
					{
						list.Add(new SetupValidationError(Strings.CannotInstallDatacenterRole(role.RoleName)));
					}
				}
			}
			if (!base.SetupContext.IsDatacenter)
			{
				bool flag4 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "FrontendTransportRole");
				bool flag5 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "CafeRole");
				bool flag6 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "BridgeheadRole");
				bool flag7 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "MailboxRole");
				bool flag8 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "ClientAccessRole");
				bool flag9 = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "UnifiedMessagingRole");
				if (flag4 && !flag5)
				{
					list.Add(new SetupValidationError(Strings.CannotInstallDatacenterRole("FrontendTransportRole")));
				}
				if (flag6 && !flag7 && !flag8 && !flag9)
				{
					list.Add(new SetupValidationError(Strings.CannotInstallDatacenterRole("BridgeheadRole")));
				}
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("customerfeedbackenabled"))
			{
				if (base.SetupContext.ParsedArguments.ContainsKey("preparead"))
				{
					if (base.ExchangeOrganizationExists && base.SetupContext.HasE14OrLaterServers)
					{
						list.Add(new SetupValidationError(Strings.CannotSpecifyCEIPWhenOrganizationHasE14OrLaterServersDuringPrepareAD));
					}
				}
				else if (!this.WillSetGlobalCEIP)
				{
					if (base.SetupContext.OriginalGlobalCustomerFeedbackEnabled == false)
					{
						list.Add(new SetupValidationError(Strings.CannotSpecifyServerCEIPWhenGlobalCEIPIsOptedOutDuringServerInstallation));
					}
					else if (!base.SetupContext.IsCleanMachine)
					{
						list.Add(new SetupValidationError(Strings.CannotSpecifyServerCEIPWhenMachineIsNotCleanDuringServerInstallation));
					}
					else if (this.WillInstallOnlyAdminAndLanguagePacks)
					{
						list.Add(new SetupValidationError(Strings.DoesNotSupportCEIPForAdminOnlyInstallation));
					}
				}
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		private void SetPartiallyConfiguredRoleChecked()
		{
			if (base.SetupContext.IsPartiallyConfigured("BridgeheadRole"))
			{
				base.SetIsBridgeheadCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("ClientAccessRole"))
			{
				base.SetIsClientAccessCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("GatewayRole"))
			{
				base.SetIsGatewayCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("MailboxRole"))
			{
				base.SetIsMailboxCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("UnifiedMessagingRole"))
			{
				base.SetIsUnifiedMessagingCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("FrontendTransportRole"))
			{
				base.SetIsFrontendTransportCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("AdminToolsRole"))
			{
				base.SetIsAdminToolsCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("CafeRole"))
			{
				base.SetIsCafeCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("CentralAdminRole"))
			{
				base.SetIsCentralAdminCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("CentralAdminDatabaseRole"))
			{
				base.SetIsCentralAdminDatabaseCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("MonitoringRole"))
			{
				base.SetIsMonitoringCheckedInternal(true);
			}
			if (base.SetupContext.IsPartiallyConfigured("OSPRole"))
			{
				base.SetIsOSPCheckedInternal(true);
			}
		}

		private SetupBindingTaskDataHandler preFileCopyDataHandler;

		private InstallFileDataHandler installFileDataHandler;

		private InstallOrgCfgDataHandler installOrgCfgDataHandler;

		private ProvisionServerDataHandler provisionServerDataHandler;

		private bool watsonEnabled;

		private bool typicalInstallation;

		private readonly bool hasRolesToInstall;

		private readonly Dictionary<string, AddUmLanguagePackCfgDataHandler> addUmLanguagePackDataHandlers = new Dictionary<string, AddUmLanguagePackCfgDataHandler>();

		private InstallPreCheckDataHandler preCheckDataHandler;

		private InstallBhdCfgDataHandler installBhdCfgDataHandler;

		private InstallCacCfgDataHandler installCacCfgDataHandler;

		private InstallGwyCfgDataHandler installGwyCfgDataHandler;

		private InstallMbxCfgDataHandler installMbxCfgDataHandler;

		private InstallUMCfgDataHandler installUMCfgDataHandler;

		private InstallFetCfgDataHandler installFetCfgDataHandler;

		private InstallCafeCfgDataHandler installCafeCfgDataHandler;

		private InstallAtCfgDataHandler installAtCfgDataHandler;

		private InstallCentralAdminDatabaseCfgDataHandler installCentralAdminDatabaseCfgDataHandler;

		private InstallCentralAdminCfgDataHandler installCentralAdminCfgDataHandler;

		private InstallCentralAdminFrontEndCfgDataHandler installCentralAdminFrontEndCfgDataHandler;

		private InstallMonitoringCfgDataHandler installMonitoringCfgDataHandler;

		private AddLanguagePackCfgDataHandler addLanguagePackCfgDataHandler;

		private InstallOSPCfgDataHandler installOSPCfgDataHandler;
	}
}
