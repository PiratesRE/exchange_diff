using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisasterRecoveryModeDataHandler : ModeDataHandler
	{
		public DisasterRecoveryModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
			SetupLogger.TraceEnter(new object[0]);
			if (base.SetupContext.ParsedArguments.ContainsKey("targetdir"))
			{
				base.InstallationPath = (NonRootLocalLongFullPath)base.SetupContext.ParsedArguments["targetdir"];
				SetupLogger.Log(Strings.UserSpecifiedTargetDir(base.InstallationPath.PathName));
			}
			if (setupContext.IsInstalledAD("BridgeheadRole"))
			{
				base.SetIsBridgeheadCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("ClientAccessRole"))
			{
				base.SetIsClientAccessCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("GatewayRole"))
			{
				base.SetIsGatewayCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("MailboxRole"))
			{
				base.SetIsMailboxCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("UnifiedMessagingRole"))
			{
				base.SetIsUnifiedMessagingCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("CafeRole"))
			{
				base.SetIsCafeCheckedInternal(true);
			}
			if (setupContext.IsInstalledAD("FrontendTransportRole"))
			{
				base.SetIsFrontendTransportCheckedInternal(true);
			}
			if (base.IsBridgeheadChecked || base.IsClientAccessChecked || base.IsGatewayChecked || base.IsMailboxChecked || base.IsUnifiedMessagingChecked || base.IsCafeChecked || base.IsFrontendTransportChecked)
			{
				base.SetIsAdminToolsCheckedInternal(true);
			}
			SetupLogger.TraceExit();
		}

		public override List<string> SelectedInstallableUnits
		{
			get
			{
				base.SelectedInstallableUnits.Clear();
				base.SelectedInstallableUnits.Add("LanguagePacks");
				if (this.IsBridgeheadEnabled && base.IsBridgeheadChecked)
				{
					base.SelectedInstallableUnits.Add("BridgeheadRole");
				}
				if (this.IsClientAccessEnabled && base.IsClientAccessChecked)
				{
					base.SelectedInstallableUnits.Add("ClientAccessRole");
				}
				if (this.IsGatewayEnabled && base.IsGatewayChecked)
				{
					base.SelectedInstallableUnits.Add("GatewayRole");
				}
				if (this.IsUnifiedMessagingEnabled && base.IsUnifiedMessagingChecked)
				{
					base.SelectedInstallableUnits.Add("UnifiedMessagingRole");
				}
				if (this.IsMailboxEnabled && base.IsMailboxChecked)
				{
					base.SelectedInstallableUnits.Add("MailboxRole");
				}
				if (base.IsAdminToolsChecked)
				{
					base.SelectedInstallableUnits.Add("AdminToolsRole");
				}
				if (base.IsCafeChecked)
				{
					base.SelectedInstallableUnits.Add("CafeRole");
				}
				if (base.IsFrontendTransportChecked)
				{
					base.SelectedInstallableUnits.Add("FrontendTransportRole");
				}
				return base.SelectedInstallableUnits;
			}
		}

		public override InstallationModes Mode
		{
			get
			{
				return InstallationModes.DisasterRecovery;
			}
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

		public override bool IsUnifiedMessagingEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("UnifiedMessagingRole");
			}
		}

		public override bool IsCafeEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("CafeRole");
			}
		}

		public override bool IsFrontendTransportEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledAD("FrontendTransportRole");
			}
		}

		public override bool IsAdminToolsEnabled
		{
			get
			{
				return true;
			}
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new DisasterRecoveryPrecheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
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
			if (this.installFileDataHandler == null)
			{
				this.installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new ProductMsiConfigurationInfo(), base.Connection);
			}
			this.installFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			this.installFileDataHandler.TargetDirectory = base.InstallationPath.PathName;
			base.DataHandlers.Add(this.installFileDataHandler);
			if (base.SetupContext.IsDatacenter || base.SetupContext.IsDatacenterDedicated)
			{
				InstallFileDataHandler installFileDataHandler = new InstallFileDataHandler(base.SetupContext, new DatacenterMsiConfigurationInfo(), base.Connection);
				installFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				installFileDataHandler.TargetDirectory = base.InstallationPath.PathName;
				base.DataHandlers.Add(installFileDataHandler);
			}
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler configurationDataHandler = null;
			if (base.HasConfigurationDataHandler(installableUnitName) && !this.drConfigDataHandlers.TryGetValue(installableUnitName, out configurationDataHandler))
			{
				if (installableUnitName != null)
				{
					if (installableUnitName == "BridgeheadRole")
					{
						configurationDataHandler = new DisasterRecoveryBhdCfgDataHandler(base.SetupContext, base.Connection);
						goto IL_CD;
					}
					if (installableUnitName == "GatewayRole")
					{
						configurationDataHandler = new DisasterRecoveryGwyCfgDataHandler(base.SetupContext, base.Connection);
						goto IL_CD;
					}
					if (installableUnitName == "AdminToolsRole")
					{
						configurationDataHandler = new DisasterRecoveryAtCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
						goto IL_CD;
					}
					if (installableUnitName == "LanguagePacks")
					{
						configurationDataHandler = new AddLanguagePackCfgDataHandler(base.SetupContext, base.Connection);
						goto IL_CD;
					}
				}
				configurationDataHandler = new DisasterRecoveryCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
				IL_CD:
				this.drConfigDataHandlers.Add(installableUnitName, configurationDataHandler);
			}
			return configurationDataHandler;
		}

		public override bool CanChangeSourceDir
		{
			get
			{
				return true;
			}
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.DRServerRoleText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				return Strings.DRRolesToInstall;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.DRFailText : Strings.DRSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.DRFailStatus : Strings.DRSuccessStatus;
			}
		}

		protected override void UpdateDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.DataHandlers.Clear();
			base.AddPreSetupDataHandlers();
			ConfigurationContext.Setup.UseAssemblyPathAsInstallPath();
			SetupLogger.Log(Strings.SetupWillRunFromPath(ConfigurationContext.Setup.InstallPath));
			base.AddFileDataHandlers();
			base.AddLanguagePackFileDataHandlers();
			base.AddPostCopyFileDataHandlers();
			this.AddConfigurationDataHandlers();
			base.AddPostSetupDataHandlers();
			SetupLogger.Log(Strings.DRModeDataHandlerCount(base.DataHandlers.Count));
			SetupLogger.TraceExit();
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (!base.SetupContext.IsServerFoundInAD)
			{
				list.Add(new SetupValidationError(Strings.DRServerNotFoundInAD));
			}
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
			if (base.SetupContext.UnpackedRoles.Count != 0)
			{
				bool flag = true;
				foreach (Role role2 in base.SetupContext.UnpackedRoles)
				{
					bool flag2 = false;
					if (role2 is AdminToolsRole)
					{
						break;
					}
					foreach (Role role3 in base.SetupContext.PartiallyConfiguredRoles)
					{
						if (string.Compare(role2.RoleName, role3.RoleName, true) == 0)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					SetupLogger.Log(Strings.HasConfiguredRoles);
					string[] value = base.SetupContext.UnpackedRoles.ConvertAll<string>((Role r) => r.RoleName).ToArray();
					list.Add(new SetupValidationError(Strings.DRRoleAlreadyInstalledError(string.Join(", ", value))));
				}
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		private SetupBindingTaskDataHandler preFileCopyDataHandler;

		private DisasterRecoveryPrecheckDataHandler preCheckDataHandler;

		private InstallFileDataHandler installFileDataHandler;

		private Dictionary<string, ConfigurationDataHandler> drConfigDataHandlers = new Dictionary<string, ConfigurationDataHandler>();
	}
}
