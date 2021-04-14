using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpgradeModeDataHandler : ModeDataHandler
	{
		public UpgradeModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
		}

		public override List<string> SelectedInstallableUnits
		{
			get
			{
				base.SelectedInstallableUnits.Clear();
				bool flag = false;
				if (this.IsBridgeheadEnabled && base.IsBridgeheadChecked)
				{
					base.SelectedInstallableUnits.Add("BridgeheadRole");
					flag = true;
				}
				if (this.IsFrontendTransportEnabled && base.IsFrontendTransportChecked)
				{
					base.SelectedInstallableUnits.Add("FrontendTransportRole");
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
				if (this.IsMonitoringEnabled)
				{
					base.SelectedInstallableUnits.Add("MonitoringRole");
					flag = true;
				}
				if (this.IsAdminToolsEnabled && base.IsAdminToolsChecked)
				{
					base.SelectedInstallableUnits.Add("AdminToolsRole");
					flag = true;
				}
				if (this.IsCafeEnabled && base.IsCafeChecked)
				{
					base.SelectedInstallableUnits.Add("CafeRole");
					flag = true;
				}
				if (this.IsCentralAdminDatabaseEnabled)
				{
					base.SelectedInstallableUnits.Add("CentralAdminDatabaseRole");
					flag = true;
				}
				if (this.IsCentralAdminEnabled)
				{
					base.SelectedInstallableUnits.Add("CentralAdminRole");
					flag = true;
				}
				if ((flag || base.SetupContext.IsLanguagePackOperation) && base.SetupContext.NeedToUpdateLanguagePacks)
				{
					base.SelectedInstallableUnits.Insert(0, "LanguagePacks");
				}
				return base.SelectedInstallableUnits;
			}
		}

		public override InstallationModes Mode
		{
			get
			{
				return InstallationModes.BuildToBuildUpgrade;
			}
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new UpgradePrecheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
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

		protected override void AddNeededFileDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddNeededFileDataHandlers();
			string text = null;
			if (this.targetDir != null)
			{
				text = this.targetDir.PathName;
			}
			if (this.preFileCopyDataHandler == null)
			{
				this.preFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PreFileCopy, base.SetupContext, base.Connection);
			}
			this.preFileCopyDataHandler.Mode = this.Mode;
			this.preFileCopyDataHandler.PreviousVersion = base.PreviousVersion;
			this.preFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.preFileCopyDataHandler);
			if (!base.SetupContext.IsCleanMachine)
			{
				if (base.SetupContext.NeedToUpdateLanguagePacks && base.SetupContext.InstalledLanguagePacks.Count > 0)
				{
					if (this.removeLanguagePackDataHandler == null)
					{
						this.removeLanguagePackDataHandler = new RemoveLanguagePackCfgDataHandler(base.SetupContext, base.Connection);
					}
					base.DataHandlers.Add(this.removeLanguagePackDataHandler);
				}
				if (this.uninstallFileDataHandler == null)
				{
					this.uninstallFileDataHandler = new UninstallFileDataHandler(base.SetupContext, new ProductMsiConfigurationInfo(), base.Connection);
				}
				this.uninstallFileDataHandler.IsUpgrade = true;
				if (base.SetupContext.IsDatacenter || base.SetupContext.IsDatacenterDedicated)
				{
					UninstallFileDataHandler uninstallFileDataHandler = new UninstallFileDataHandler(base.SetupContext, new DatacenterMsiConfigurationInfo(), base.Connection);
					uninstallFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
					uninstallFileDataHandler.IsUpgrade = true;
					base.DataHandlers.Add(uninstallFileDataHandler);
				}
				base.DataHandlers.Add(this.uninstallFileDataHandler);
			}
			else
			{
				SetupLogger.Log(Strings.MSINotPresent(text));
			}
			if (this.midFileCopyDataHandler == null)
			{
				this.midFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.MidFileCopy, base.SetupContext, base.Connection);
			}
			this.midFileCopyDataHandler.Mode = this.Mode;
			this.midFileCopyDataHandler.PreviousVersion = base.PreviousVersion;
			this.midFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.midFileCopyDataHandler);
			if (this.installFileDataHandler == null)
			{
				this.installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new ProductMsiConfigurationInfo(), base.Connection);
			}
			this.installFileDataHandler.SelectedInstallableUnits.Clear();
			foreach (string text2 in this.SelectedInstallableUnits.ToArray())
			{
				Role roleByName = RoleManager.GetRoleByName(text2);
				if (roleByName == null || !RoleManager.GetRoleByName(text2).IsDatacenterOnly)
				{
					this.installFileDataHandler.SelectedInstallableUnits.Add(text2);
				}
			}
			this.installFileDataHandler.TargetDirectory = text;
			base.DataHandlers.Add(this.installFileDataHandler);
			if (base.SetupContext.IsDatacenter || base.SetupContext.IsDatacenterDedicated)
			{
				InstallFileDataHandler installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new DatacenterMsiConfigurationInfo(), base.Connection);
				installFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				installFileDataHandler.TargetDirectory = base.InstallationPath.PathName;
				base.DataHandlers.Add(installFileDataHandler);
			}
			SetupLogger.TraceExit();
		}

		private void AddOrgLevelConfigurationDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			if (!this.SelectedInstallableUnits.Contains("GatewayRole") && this.SelectedInstallableUnits.Count > 1 && base.OrgLevelConfigRequired())
			{
				if (this.installOrgCfgDataHandler == null)
				{
					this.installOrgCfgDataHandler = new InstallOrgCfgDataHandler(base.SetupContext, base.Connection);
				}
				this.installOrgCfgDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(this.installOrgCfgDataHandler);
			}
			SetupLogger.TraceExit();
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler configurationDataHandler = null;
			if (base.HasConfigurationDataHandler(installableUnitName) && !this.upgradeConfigDataHandlers.TryGetValue(installableUnitName, out configurationDataHandler))
			{
				if (InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName))
				{
					InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
					UmLanguagePackConfigurationInfo umLanguagePackConfigurationInfo = installableUnitConfigurationInfoByName as UmLanguagePackConfigurationInfo;
					configurationDataHandler = new AddUmLanguagePackCfgDataHandler(base.SetupContext, base.Connection, umLanguagePackConfigurationInfo.Culture, base.UmSourceDir);
				}
				else if (InstallableUnitConfigurationInfoManager.IsLanguagePacksInstallableUnit(installableUnitName))
				{
					configurationDataHandler = new AddLanguagePackCfgDataHandler(base.SetupContext, base.Connection);
				}
				else if (installableUnitName == "AdminToolsRole")
				{
					configurationDataHandler = new UpgradeAtCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				}
				else if (installableUnitName == "CentralAdminDatabaseRole")
				{
					configurationDataHandler = new UpgradeCentralAdminDatabaseCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				}
				else if (installableUnitName == "CentralAdminRole")
				{
					configurationDataHandler = new UpgradeCentralAdminCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				}
				else if (installableUnitName == "MonitoringRole")
				{
					configurationDataHandler = new UpgradeMonitoringCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				}
				else
				{
					configurationDataHandler = new UpgradeCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				}
				this.upgradeConfigDataHandlers.Add(installableUnitName, configurationDataHandler);
			}
			return configurationDataHandler;
		}

		public override bool IsBridgeheadEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("BridgeheadRole");
			}
		}

		public override bool IsClientAccessEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("ClientAccessRole");
			}
		}

		public override bool IsGatewayEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("GatewayRole");
			}
		}

		public override bool IsMailboxEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("MailboxRole");
			}
		}

		public override bool IsMonitoringEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("MonitoringRole");
			}
		}

		public override bool IsUnifiedMessagingEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("UnifiedMessagingRole");
			}
		}

		public override bool IsAdminToolsEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("AdminToolsRole");
			}
		}

		public override bool IsCafeEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CafeRole");
			}
		}

		public override bool IsCentralAdminEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CentralAdminRole");
			}
		}

		public override bool IsCentralAdminDatabaseEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CentralAdminDatabaseRole");
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
				return Strings.UpgradeServerRoleText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				return Strings.UpgradeRolesToInstall;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.UpgradeFailText : Strings.UpgradeSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.UpgradeFailStatus : Strings.UpgradeSuccessStatus;
			}
		}

		protected override void UpdateDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			this.targetDir = (base.SetupContext.InstalledPath ?? base.SetupContext.BackupInstalledPath);
			base.PreviousVersion = (base.SetupContext.InstalledVersion ?? base.SetupContext.BackupInstalledVersion);
			this.fileUpgradeNeeded = (base.SetupContext.IsCleanMachine || base.SetupContext.InstalledVersion < base.SetupContext.RunningVersion);
			base.DataHandlers.Clear();
			if (!base.IsLanguagePacksChecked && (base.SetupContext.IsCleanMachine || base.SetupContext.IsLanguagePackOperation || base.SetupContext.LanguagePacksToInstall.Keys.Count != 0))
			{
				base.SetIsLanguagePacksCheckedInternal(true);
			}
			this.AddOrgLevelConfigurationDataHandlers();
			if (this.fileUpgradeNeeded)
			{
				ConfigurationContext.Setup.UseAssemblyPathAsInstallPath();
				base.AddPreSetupDataHandlers();
				base.AddFileDataHandlers();
			}
			else
			{
				SetupLogger.Log(Strings.MSIIsCurrent);
			}
			if (base.SetupContext.NeedToUpdateLanguagePacks)
			{
				base.AddLanguagePackFileDataHandlers();
			}
			base.AddPostCopyFileDataHandlers();
			this.AddConfigurationDataHandlers();
			base.AddPostSetupDataHandlers();
			foreach (DataHandler dataHandler in base.DataHandlers)
			{
				SetupSingleTaskDataHandler setupSingleTaskDataHandler = (SetupSingleTaskDataHandler)dataHandler;
				ExTraceGlobals.TraceTracer.Information(0L, setupSingleTaskDataHandler.CommandText);
			}
			SetupLogger.TraceExit();
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			SetupLogger.Log(Strings.ValidatingRoleOptions(base.SetupContext.RequestedRoles.Count));
			List<string> list2 = new List<string>();
			foreach (Role role in base.SetupContext.RequestedRoles)
			{
				if (!base.SetupContext.IsInstalledAD(role.RoleName))
				{
					list2.Add(InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(role.RoleName).DisplayName);
				}
			}
			if (list2.Count > 0)
			{
				string missingRoles = string.Join(", ", list2.ToArray());
				list.Add(new SetupValidationError(Strings.RoleNotInstalledError(missingRoles)));
			}
			if (this.targetDir == null)
			{
				list.Add(new SetupValidationError(Strings.UnknownDestinationPath));
			}
			if (base.PreviousVersion == null)
			{
				list.Add(new SetupValidationError(Strings.UnknownPreviousVersion));
			}
			SetupLogger.Log(Strings.UpgradeModeDataHandlerHandlersAndWorkUnits(base.DataHandlers.Count, base.WorkUnits.Count));
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		private SetupBindingTaskDataHandler preFileCopyDataHandler;

		private SetupBindingTaskDataHandler midFileCopyDataHandler;

		private InstallOrgCfgDataHandler installOrgCfgDataHandler;

		private Dictionary<string, ConfigurationDataHandler> upgradeConfigDataHandlers = new Dictionary<string, ConfigurationDataHandler>();

		private NonRootLocalLongFullPath targetDir;

		private bool fileUpgradeNeeded;

		private UpgradePrecheckDataHandler preCheckDataHandler;

		private InstallFileDataHandler installFileDataHandler;

		private UninstallFileDataHandler uninstallFileDataHandler;

		private RemoveLanguagePackCfgDataHandler removeLanguagePackDataHandler;
	}
}
