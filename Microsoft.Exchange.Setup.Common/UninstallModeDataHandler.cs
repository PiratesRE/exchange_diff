using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UninstallModeDataHandler : ModeDataHandler
	{
		public UninstallModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
			SetupLogger.Log(Strings.ApplyingDefaultRoleSelectionState);
			if (setupContext.IsUnpackedOrInstalledAD("BridgeheadRole"))
			{
				base.SetIsBridgeheadCheckedInternal(false);
			}
			if (setupContext.IsUnpackedOrInstalledAD("ClientAccessRole"))
			{
				base.SetIsClientAccessCheckedInternal(false);
			}
			if (setupContext.IsUnpackedOrInstalledAD("GatewayRole"))
			{
				base.SetIsGatewayCheckedInternal(false);
			}
			if (setupContext.IsUnpackedOrInstalledAD("MailboxRole"))
			{
				base.SetIsMailboxCheckedInternal(false);
			}
			if (setupContext.IsUnpackedOrInstalledAD("UnifiedMessagingRole"))
			{
				base.SetIsUnifiedMessagingCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("FrontendTransportRole"))
			{
				base.SetIsFrontendTransportCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("AdminToolsRole"))
			{
				base.SetIsAdminToolsCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("CafeRole"))
			{
				base.SetIsCafeCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("CentralAdminRole"))
			{
				base.SetIsCentralAdminCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("CentralAdminDatabaseRole"))
			{
				base.SetIsCentralAdminDatabaseCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("CentralAdminFrontEndRole"))
			{
				base.SetIsCentralAdminFrontEndCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("MonitoringRole"))
			{
				base.SetIsMonitoringCheckedInternal(false);
			}
			if (setupContext.IsUnpacked("OSPRole"))
			{
				base.SetIsOSPCheckedInternal(false);
			}
		}

		protected override bool NeedPrePostSetupDataHandlers
		{
			get
			{
				return base.NeedPrePostSetupDataHandlers && !base.SetupContext.HasRemoveProvisionedServerParameters;
			}
		}

		protected override bool NeedFileDataHandler()
		{
			return base.NeedFileDataHandler() && !base.SetupContext.HasRemoveProvisionedServerParameters;
		}

		public override List<string> SelectedInstallableUnits
		{
			get
			{
				base.SelectedInstallableUnits.Clear();
				if (base.SetupContext.HasRemoveProvisionedServerParameters)
				{
					return base.SelectedInstallableUnits;
				}
				if (this.IsMailboxEnabled && !base.IsMailboxChecked)
				{
					base.SelectedInstallableUnits.Add("MailboxRole");
				}
				if (this.IsUnifiedMessagingEnabled && !base.IsUnifiedMessagingChecked)
				{
					base.SelectedInstallableUnits.AddRange(this.GetInstalledUmLanguagePacks());
					base.SelectedInstallableUnits.Add("UnifiedMessagingRole");
				}
				if (this.IsClientAccessEnabled && !base.IsClientAccessChecked)
				{
					base.SelectedInstallableUnits.Add("ClientAccessRole");
				}
				if (this.IsBridgeheadEnabled && !base.IsBridgeheadChecked)
				{
					base.SelectedInstallableUnits.Add("BridgeheadRole");
				}
				if (this.IsGatewayEnabled && !base.IsGatewayChecked)
				{
					base.SelectedInstallableUnits.Add("GatewayRole");
				}
				if (this.IsFrontendTransportEnabled && !base.IsFrontendTransportChecked)
				{
					base.SelectedInstallableUnits.Add("FrontendTransportRole");
				}
				if ((this.IsAdminToolsEnabled || this.IsGatewayEnabled) && !base.IsAdminToolsChecked)
				{
					base.SelectedInstallableUnits.Add("AdminToolsRole");
				}
				if (this.IsCafeEnabled && !base.IsCafeChecked)
				{
					base.SelectedInstallableUnits.Add("CafeRole");
				}
				if (this.IsCentralAdminEnabled && !base.IsCentralAdminChecked)
				{
					base.SelectedInstallableUnits.Add("CentralAdminRole");
				}
				if (this.IsCentralAdminDatabaseEnabled && !base.IsCentralAdminDatabaseChecked)
				{
					base.SelectedInstallableUnits.Add("CentralAdminDatabaseRole");
				}
				if (this.IsMonitoringEnabled && !base.IsMonitoringChecked)
				{
					base.SelectedInstallableUnits.Add("MonitoringRole");
				}
				if (this.IsRemoveAllRoles)
				{
					base.SelectedInstallableUnits.Add("LanguagePacks");
				}
				if (this.IsOSPEnabled && !base.IsOSPChecked)
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
				return InstallationModes.Uninstall;
			}
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new UninstallPreCheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
			}
		}

		protected override void AddNeededFileDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddNeededFileDataHandlers();
			if (this.preFileCopyDataHandler == null)
			{
				this.preFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PreFileCopy, base.SetupContext, base.Connection);
			}
			this.preFileCopyDataHandler.Mode = this.Mode;
			this.preFileCopyDataHandler.PreviousVersion = base.PreviousVersion;
			this.preFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.preFileCopyDataHandler);
			if (this.uninstallFileDataHandler == null)
			{
				this.uninstallFileDataHandler = new UninstallFileDataHandler(base.SetupContext, new ProductMsiConfigurationInfo(), base.Connection);
			}
			this.uninstallFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.uninstallFileDataHandler);
			if (base.SetupContext.IsDatacenter || base.SetupContext.IsDatacenterDedicated)
			{
				UninstallFileDataHandler uninstallFileDataHandler = new UninstallFileDataHandler(base.SetupContext, new DatacenterMsiConfigurationInfo(), base.Connection);
				uninstallFileDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(uninstallFileDataHandler);
			}
			if (this.postFileCopyDataHandler == null)
			{
				this.postFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PostFileCopy, base.SetupContext, base.Connection);
			}
			this.postFileCopyDataHandler.Mode = this.Mode;
			this.postFileCopyDataHandler.PreviousVersion = base.PreviousVersion;
			this.postFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
			base.DataHandlers.Add(this.postFileCopyDataHandler);
			SetupLogger.TraceExit();
		}

		protected override void AddConfigurationDataHandlers()
		{
			base.AddConfigurationDataHandlers();
			if (!this.SelectedInstallableUnits.Contains("GatewayRole"))
			{
				if (this.IsRemoveAllConfiguredServerRoles && base.OrgLevelConfigRequired() && (this.SelectedInstallableUnits.Count != 1 || !this.SelectedInstallableUnits.Contains("AdminToolsRole")))
				{
					if (this.uninstallOrgCfgDataHandler == null)
					{
						this.uninstallOrgCfgDataHandler = new UninstallOrgCfgDataHandler(base.SetupContext, base.Connection);
					}
					this.uninstallOrgCfgDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
					base.DataHandlers.Add(this.uninstallOrgCfgDataHandler);
				}
				if (base.SetupContext.HasRemoveProvisionedServerParameters)
				{
					if (this.provisionServerDataHandler == null)
					{
						this.provisionServerDataHandler = new RemoveProvisionedServerDataHandler(base.SetupContext, base.Connection);
					}
					this.provisionServerDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
					base.DataHandlers.Add(this.provisionServerDataHandler);
				}
			}
			if (this.IsRemoveAllRoles && base.SetupContext.InstalledLanguagePacks.Count > 0 && !base.SetupContext.HasRemoveProvisionedServerParameters)
			{
				RemoveLanguagePackCfgDataHandler item = (RemoveLanguagePackCfgDataHandler)this.GetInstallableUnitConfigurationDataHandler("LanguagePacks");
				if (!(base.DataHandlers[base.DataHandlers.Count - 1] is RemoveLanguagePackCfgDataHandler))
				{
					for (int i = base.DataHandlers.Count - 2; i >= 0; i--)
					{
						if (base.DataHandlers[i] is RemoveLanguagePackCfgDataHandler)
						{
							base.DataHandlers.RemoveAt(i);
							break;
						}
					}
					base.DataHandlers.Add(item);
				}
			}
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler result = null;
			if (base.HasConfigurationDataHandler(installableUnitName))
			{
				if (InstallableUnitConfigurationInfoManager.IsRoleBasedConfigurableInstallableUnit(installableUnitName))
				{
					Role roleByName = RoleManager.GetRoleByName(installableUnitName);
					if (base.SetupContext.IsInstalledLocalOrAD(installableUnitName) || (base.SetupContext.IsPartiallyConfigured(installableUnitName) && roleByName.IsPartiallyInstalled))
					{
						UninstallCfgDataHandler uninstallCfgDataHandler;
						if (!this.uninstallCfgDataHandlers.TryGetValue(installableUnitName, out uninstallCfgDataHandler))
						{
							if (installableUnitName == "AdminToolsRole")
							{
								uninstallCfgDataHandler = new UninstallAtCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
							}
							else if (installableUnitName == "MailboxRole")
							{
								this.uninstallMailboxRole = new UninstallMbxCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
								uninstallCfgDataHandler = this.uninstallMailboxRole;
							}
							else if (installableUnitName == "CentralAdminRole")
							{
								uninstallCfgDataHandler = new UninstallCentralAdminCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
							}
							else if (installableUnitName == "CentralAdminDatabaseRole")
							{
								uninstallCfgDataHandler = new UninstallCentralAdminDatabaseCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
							}
							else
							{
								uninstallCfgDataHandler = new UninstallCfgDataHandler(base.SetupContext, RoleManager.GetRoleByName(installableUnitName), base.Connection);
							}
							this.uninstallCfgDataHandlers.Add(installableUnitName, uninstallCfgDataHandler);
						}
						result = uninstallCfgDataHandler;
					}
				}
				else if (InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName))
				{
					InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
					UmLanguagePackConfigurationInfo umLanguagePackConfigurationInfo = installableUnitConfigurationInfoByName as UmLanguagePackConfigurationInfo;
					if (umLanguagePackConfigurationInfo != null)
					{
						RemoveUmLanguagePackCfgDataHandler removeUmLanguagePackCfgDataHandler;
						if (!this.removeUmLanguagePackDataHandlers.TryGetValue(installableUnitName, out removeUmLanguagePackCfgDataHandler))
						{
							removeUmLanguagePackCfgDataHandler = new RemoveUmLanguagePackCfgDataHandler(base.SetupContext, base.Connection, umLanguagePackConfigurationInfo.Culture);
							this.removeUmLanguagePackDataHandlers.Add(installableUnitName, removeUmLanguagePackCfgDataHandler);
						}
						result = removeUmLanguagePackCfgDataHandler;
					}
				}
				else if (InstallableUnitConfigurationInfoManager.IsLanguagePacksInstallableUnit(installableUnitName))
				{
					result = new RemoveLanguagePackCfgDataHandler(base.SetupContext, base.Connection);
				}
			}
			return result;
		}

		public override bool IsBridgeheadEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("BridgeheadRole") || base.SetupContext.IsInstalledLocal("BridgeheadRole") || base.SetupContext.IsUnpackedOrInstalledAD("BridgeheadRole");
			}
		}

		public override bool IsClientAccessEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("ClientAccessRole") || base.SetupContext.IsInstalledLocal("ClientAccessRole") || base.SetupContext.IsUnpackedOrInstalledAD("ClientAccessRole");
			}
		}

		public override bool IsGatewayEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("GatewayRole") || base.SetupContext.IsInstalledLocal("GatewayRole") || base.SetupContext.IsUnpackedOrInstalledAD("GatewayRole");
			}
		}

		public override bool IsMailboxEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("MailboxRole") || base.SetupContext.IsInstalledLocal("MailboxRole") || base.SetupContext.IsUnpackedOrInstalledAD("MailboxRole");
			}
		}

		public override bool IsUnifiedMessagingEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("UnifiedMessagingRole") || base.SetupContext.IsInstalledLocal("UnifiedMessagingRole") || base.SetupContext.IsUnpackedOrInstalledAD("UnifiedMessagingRole");
			}
		}

		public override bool IsFrontendTransportEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("FrontendTransportRole") || base.SetupContext.IsInstalledLocal("FrontendTransportRole") || base.SetupContext.IsUnpackedOrInstalledAD("FrontendTransportRole");
			}
		}

		public override bool IsCafeEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("CafeRole") || base.SetupContext.IsInstalledLocal("CafeRole") || base.SetupContext.IsUnpackedOrInstalledAD("CafeRole");
			}
		}

		public override bool IsCentralAdminEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("CentralAdminRole") || base.SetupContext.IsInstalledLocal("CentralAdminRole") || base.SetupContext.IsUnpacked("CentralAdminRole");
			}
		}

		public override bool IsCentralAdminDatabaseEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("CentralAdminDatabaseRole") || base.SetupContext.IsInstalledLocal("CentralAdminDatabaseRole") || base.SetupContext.IsUnpacked("CentralAdminDatabaseRole");
			}
		}

		public override bool IsMonitoringEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("MonitoringRole") || base.SetupContext.IsInstalledLocal("MonitoringRole") || base.SetupContext.IsUnpackedOrInstalledAD("MonitoringRole");
			}
		}

		public override bool IsAdminToolsEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("AdminToolsRole") && this.IsRemoveAllServerRoles && !this.IsGatewayEnabled;
			}
		}

		public override bool IsOSPEnabled
		{
			get
			{
				return base.SetupContext.IsPartiallyConfigured("OSPRole") || base.SetupContext.IsInstalledLocal("OSPRole") || base.SetupContext.IsUnpackedOrInstalledAD("OSPRole");
			}
		}

		public bool IsRemoveAllRoles
		{
			get
			{
				return this.IsRemoveAllServerRoles && !base.IsAdminToolsChecked;
			}
		}

		public bool IsRemoveAllServerRoles
		{
			get
			{
				return !base.IsBridgeheadChecked && !base.IsClientAccessChecked && !base.IsGatewayChecked && !base.IsMailboxChecked && !base.IsUnifiedMessagingChecked;
			}
		}

		public bool IsRemoveAllConfiguredServerRoles
		{
			get
			{
				bool result = true;
				foreach (Role role in base.SetupContext.InstalledRolesLocal)
				{
					if (base.HasConfigurationDataHandler(role.RoleName) && !this.SelectedInstallableUnits.Contains(role.RoleName))
					{
						result = false;
						break;
					}
				}
				return result;
			}
		}

		public override decimal RequiredDiskSpace
		{
			get
			{
				return 0m;
			}
		}

		public override bool CanChangeSourceDir
		{
			get
			{
				return false;
			}
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.RemoveServerRoleText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				if (base.SetupContext.HasRemoveProvisionedServerParameters)
				{
					return LocalizedString.Empty;
				}
				return Strings.RemoveRolesToInstall;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.RemoveFailText : Strings.RemoveSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.RemoveFailStatus : Strings.RemoveSuccessStatus;
			}
		}

		protected override void UpdateDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			if (this.IsGatewayEnabled && base.IsGatewayChecked != base.IsAdminToolsChecked)
			{
				base.SetIsAdminToolsCheckedInternal(base.IsGatewayChecked);
			}
			if (base.SetupContext.HasRemoveProvisionedServerParameters)
			{
				ConfigurationContext.Setup.UseAssemblyPathAsInstallPath();
			}
			SetupLogger.Log(Strings.SetupWillRunFromPath(ConfigurationContext.Setup.InstallPath));
			base.DataHandlers.Clear();
			base.AddPreSetupDataHandlers();
			this.AddConfigurationDataHandlers();
			base.AddFileDataHandlers();
			base.AddPostSetupDataHandlers();
			SetupLogger.Log(Strings.UninstallModeDataHandlerCount(base.DataHandlers.Count));
			SetupLogger.TraceExit();
		}

		public override bool HasChanges
		{
			get
			{
				return this.SelectedInstallableUnits.Count > 0 || base.SetupContext.HasRemoveProvisionedServerParameters;
			}
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (!base.IgnoreValidatingRoleChanges)
			{
				SetupLogger.Log(Strings.ValidatingRoleOptions(base.SetupContext.RequestedRoles.Count));
				List<string> list2 = new List<string>();
				foreach (Role role in base.SetupContext.RequestedRoles)
				{
					if (!base.SetupContext.IsInstalledLocal(role.RoleName) && !base.SetupContext.IsPartiallyConfigured(role.RoleName) && !base.SetupContext.IsUnpackedOrInstalledAD(role.RoleName))
					{
						list2.Add(InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(role.RoleName).DisplayName);
					}
				}
				if (list2.Count > 0)
				{
					string missingRoles = string.Join(", ", list2.ToArray());
					list.Add(new SetupValidationError(Strings.RoleNotInstalledError(missingRoles)));
				}
				string admintools = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName("AdminToolsRole").DisplayName;
				bool flag = base.SetupContext.RequestedRoles.Exists((Role current) => current.RoleName == "AdminToolsRole");
				if (base.SetupContext.RequestedRoles.Count < base.SetupContext.UnpackedRoles.Count && flag)
				{
					list.Add(new SetupValidationError(Strings.AdminToolCannotBeUninstalledWhenSomeRolesRemained(admintools)));
				}
				if (!this.HasChanges)
				{
					list.Add(new SetupValidationError(Strings.NoRoleSelectedForUninstall));
				}
				SetupLogger.Log(Strings.UninstallModeDataHandlerHandlersAndWorkUnits(base.DataHandlers.Count, base.WorkUnits.Count));
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		private List<string> GetInstalledUmLanguagePacks()
		{
			List<string> list = new List<string>();
			List<string> result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\LanguagePacks\\"))
			{
				if (registryKey != null)
				{
					foreach (string text in registryKey.GetValueNames())
					{
						if (text.ToLower() == "en-us")
						{
							SetupLogger.Log(Strings.EnglishUSLanguagePackInstalled);
						}
						else
						{
							try
							{
								CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(text);
								string umLanguagePackNameForCultureInfo = UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(cultureInfo);
								list.Add(umLanguagePackNameForCultureInfo);
								SetupLogger.Log(Strings.UnifiedMessagingLanguagePackInstalled(cultureInfo.ToString()));
								if (InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(umLanguagePackNameForCultureInfo) == null)
								{
									SetupLogger.Log(Strings.NoConfigurationInfoFoundForInstallableUnit(umLanguagePackNameForCultureInfo));
									InstallableUnitConfigurationInfoManager.AddInstallableUnit(umLanguagePackNameForCultureInfo, new UmLanguagePackConfigurationInfo(cultureInfo));
									SetupLogger.Log(Strings.AddedConfigurationInfoForInstallableUnit(umLanguagePackNameForCultureInfo));
								}
							}
							catch (ArgumentException)
							{
								SetupLogger.Log(Strings.NonCultureRegistryEntryFound(text));
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		private SetupBindingTaskDataHandler preFileCopyDataHandler;

		private SetupBindingTaskDataHandler postFileCopyDataHandler;

		private RemoveProvisionedServerDataHandler provisionServerDataHandler;

		private UninstallPreCheckDataHandler preCheckDataHandler;

		private UninstallFileDataHandler uninstallFileDataHandler;

		private UninstallMbxCfgDataHandler uninstallMailboxRole;

		private UninstallOrgCfgDataHandler uninstallOrgCfgDataHandler;

		private Dictionary<string, UninstallCfgDataHandler> uninstallCfgDataHandlers = new Dictionary<string, UninstallCfgDataHandler>();

		private Dictionary<string, RemoveUmLanguagePackCfgDataHandler> removeUmLanguagePackDataHandlers = new Dictionary<string, RemoveUmLanguagePackCfgDataHandler>();
	}
}
