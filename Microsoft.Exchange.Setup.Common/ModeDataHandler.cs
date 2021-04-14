using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.Common
{
	internal abstract class ModeDataHandler : SetupSingleTaskDataHandler, IPrecheckEnabled
	{
		protected void SetIsBridgeheadCheckedInternal(bool value)
		{
			this.isBridgeheadChecked = value;
		}

		protected void SetIsClientAccessCheckedInternal(bool value)
		{
			this.isClientAccessChecked = value;
		}

		protected void SetIsGatewayCheckedInternal(bool value)
		{
			this.isGatewayChecked = value;
		}

		protected void SetIsMailboxCheckedInternal(bool value)
		{
			this.isMailboxChecked = value;
		}

		protected void SetIsUnifiedMessagingCheckedInternal(bool value)
		{
			this.isUnifiedMessagingChecked = value;
		}

		protected void SetIsFrontendTransportCheckedInternal(bool value)
		{
			this.isFrontendTransportChecked = value;
		}

		protected void SetIsCafeCheckedInternal(bool value)
		{
			this.isCafeChecked = value;
		}

		protected void SetIsAdminToolsCheckedInternal(bool value)
		{
			this.isAdminToolsChecked = value;
		}

		protected void SetIsCentralAdminCheckedInternal(bool value)
		{
			this.isCentralAdminChecked = value;
		}

		protected void SetIsCentralAdminDatabaseCheckedInternal(bool value)
		{
			this.isCentralAdminDatabaseChecked = value;
		}

		protected void SetIsCentralAdminFrontEndCheckedInternal(bool value)
		{
			this.isCentralAdminFrontEndChecked = value;
		}

		protected void SetIsMonitoringCheckedInternal(bool value)
		{
			this.isMonitoringChecked = value;
		}

		protected void SetIsLanguagePacksCheckedInternal(bool value)
		{
			this.isLanguagePacksChecked = value;
		}

		protected void SetIsOSPCheckedInternal(bool value)
		{
			this.isOSPChecked = value;
		}

		public ModeDataHandler(ISetupContext context, MonadConnection connection) : base(context, "", connection)
		{
			this.ResetInstallSettings();
			if (base.SetupContext.IsInstalledLocalOrAD("BridgeheadRole"))
			{
				this.isBridgeheadChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("FrontendTransportRole"))
			{
				this.isFrontendTransportChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("ClientAccessRole"))
			{
				this.isClientAccessChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("GatewayRole"))
			{
				this.isGatewayChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("MailboxRole"))
			{
				this.isMailboxChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("UnifiedMessagingRole"))
			{
				this.isUnifiedMessagingChecked = true;
			}
			if (base.SetupContext.IsInstalledLocal("AdminToolsRole"))
			{
				this.isAdminToolsChecked = true;
			}
			if (base.SetupContext.IsInstalledLocalOrAD("CafeRole"))
			{
				this.isCafeChecked = true;
			}
		}

		internal void ResetInstallSettings()
		{
			this.isBridgeheadChecked = false;
			this.isClientAccessChecked = false;
			this.isGatewayChecked = false;
			this.isMailboxChecked = false;
			this.isUnifiedMessagingChecked = false;
			this.isAdminToolsChecked = false;
			this.isCafeChecked = false;
			this.selectedInstallableUnits = new List<string>();
		}

		protected Version PreviousVersion
		{
			get
			{
				return this.previousVersion;
			}
			set
			{
				this.previousVersion = value;
			}
		}

		public bool IsInstalledLocal(string roleName)
		{
			return base.SetupContext.IsInstalledLocal(roleName);
		}

		public bool ExchangeOrganizationExists
		{
			get
			{
				return base.SetupContext.ExchangeOrganizationExists;
			}
		}

		public bool ExchangeOrganizationFoundInAD
		{
			get
			{
				return base.SetupContext.OrganizationNameFoundInAD != null && !string.IsNullOrEmpty(base.SetupContext.OrganizationNameFoundInAD.EscapedName);
			}
		}

		public bool IsADPrepTasksRequired
		{
			get
			{
				return base.SetupContext.IsSchemaUpdateRequired || base.SetupContext.IsOrgConfigUpdateRequired;
			}
		}

		public bool IsInstalledAD(string roleName)
		{
			return base.SetupContext.IsInstalledAD(roleName);
		}

		public bool HasMailboxServers
		{
			get
			{
				return base.SetupContext.HasMailboxServers;
			}
		}

		public bool HasLegacyServers
		{
			get
			{
				return base.SetupContext.HasLegacyServers;
			}
		}

		public bool HasBridgeheadServers
		{
			get
			{
				return base.SetupContext.HasBridgeheadServers;
			}
		}

		public bool InstallWindowsComponents
		{
			get
			{
				return base.SetupContext.InstallWindowsComponents;
			}
			set
			{
				base.SetupContext.InstallWindowsComponents = value;
			}
		}

		public bool InstallWindowsComponentsEnabled
		{
			get
			{
				return this.Mode != InstallationModes.Uninstall;
			}
		}

		public virtual List<string> SelectedInstallableUnits
		{
			get
			{
				return this.selectedInstallableUnits;
			}
		}

		protected bool ServerRoleIsSelected
		{
			get
			{
				return this.SelectedInstallableUnits.Contains("ClientAccessRole") || this.SelectedInstallableUnits.Contains("BridgeheadRole") || this.SelectedInstallableUnits.Contains("UnifiedMessagingRole") || this.SelectedInstallableUnits.Contains("MailboxRole") || this.SelectedInstallableUnits.Contains("GatewayRole") || this.SelectedInstallableUnits.Contains("FrontendTransportRole") || this.SelectedInstallableUnits.Contains("CafeRole");
			}
		}

		public abstract InstallationModes Mode { get; }

		public abstract PreCheckDataHandler PreCheckDataHandler { get; }

		public bool IsLanguagePackOnlyInstallation
		{
			get
			{
				bool result = false;
				if (!base.SetupContext.IsCleanMachine && this.Mode == InstallationModes.Install)
				{
					int num = 0;
					if (this.SelectedInstallableUnits.Contains("LanguagePacks"))
					{
						num++;
					}
					result = (num > 0 && num == this.SelectedInstallableUnits.Count);
				}
				return result;
			}
		}

		protected void AddFileDataHandlers()
		{
			if (this.NeedFileDataHandler())
			{
				this.AddNeededFileDataHandlers();
			}
		}

		protected void AddPreSetupDataHandlers()
		{
			if (this.NeedPrePostSetupDataHandlers)
			{
				if (this.preSetupDataHandler == null)
				{
					this.preSetupDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PreSetup, base.SetupContext, base.Connection);
				}
				this.preSetupDataHandler.Mode = this.Mode;
				this.preSetupDataHandler.PreviousVersion = this.previousVersion;
				this.preSetupDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(this.preSetupDataHandler);
			}
		}

		protected void AddPostSetupDataHandlers()
		{
			if (this.NeedPrePostSetupDataHandlers)
			{
				if (this.postSetupDataHandler == null)
				{
					this.postSetupDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PostSetup, base.SetupContext, base.Connection);
				}
				this.postSetupDataHandler.Mode = this.Mode;
				this.postSetupDataHandler.PreviousVersion = this.previousVersion;
				this.postSetupDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(this.postSetupDataHandler);
			}
		}

		protected virtual bool NeedPrePostSetupDataHandlers
		{
			get
			{
				return this.SelectedInstallableUnits.Count != 0;
			}
		}

		protected virtual bool NeedFileDataHandler()
		{
			return this.SelectedInstallableUnits.Count != 0;
		}

		protected virtual void AddNeededFileDataHandlers()
		{
		}

		protected void AddPostCopyFileDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			if (this.NeedFileDataHandler())
			{
				if (this.postFileCopyDataHandler == null)
				{
					this.postFileCopyDataHandler = new SetupBindingTaskDataHandler(BindingCategory.PostFileCopy, base.SetupContext, base.Connection);
				}
				this.postFileCopyDataHandler.Mode = this.Mode;
				this.postFileCopyDataHandler.PreviousVersion = this.PreviousVersion;
				this.postFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(this.postFileCopyDataHandler);
			}
			SetupLogger.TraceExit();
		}

		protected abstract void UpdateDataHandlers();

		protected virtual void AddConfigurationDataHandlers()
		{
			foreach (string installableUnitName in this.SelectedInstallableUnits.ToArray())
			{
				ConfigurationDataHandler installableUnitConfigurationDataHandler = this.GetInstallableUnitConfigurationDataHandler(installableUnitName);
				if (installableUnitConfigurationDataHandler != null)
				{
					installableUnitConfigurationDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
					base.DataHandlers.Add(installableUnitConfigurationDataHandler);
				}
			}
		}

		protected void AddLanguagePackFileDataHandlers()
		{
			if (this.NeedFileDataHandler())
			{
				AddLanguagePackFileCopyDataHandler addLanguagePackFileCopyDataHandler = new AddLanguagePackFileCopyDataHandler(base.SetupContext, base.Connection);
				addLanguagePackFileCopyDataHandler.SelectedInstallableUnits = this.SelectedInstallableUnits;
				base.DataHandlers.Add(addLanguagePackFileCopyDataHandler);
			}
		}

		protected abstract ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName);

		public abstract string RoleSelectionDescription { get; }

		public abstract LocalizedString ConfigurationSummary { get; }

		public bool IsBridgeheadChecked
		{
			get
			{
				return this.isBridgeheadChecked;
			}
			set
			{
				if (value != this.IsBridgeheadChecked)
				{
					this.isBridgeheadChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsClientAccessChecked
		{
			get
			{
				return this.isClientAccessChecked;
			}
			set
			{
				if (value != this.IsClientAccessChecked)
				{
					this.isClientAccessChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsGatewayChecked
		{
			get
			{
				return this.isGatewayChecked;
			}
			set
			{
				if (value != this.IsGatewayChecked)
				{
					this.isGatewayChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsMailboxChecked
		{
			get
			{
				return this.isMailboxChecked;
			}
			set
			{
				if (value != this.IsMailboxChecked)
				{
					this.isMailboxChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsCafeChecked
		{
			get
			{
				return this.isCafeChecked;
			}
			set
			{
				if (value != this.IsCafeChecked)
				{
					this.isCafeChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsUnifiedMessagingChecked
		{
			get
			{
				return this.isUnifiedMessagingChecked;
			}
			set
			{
				if (value != this.IsUnifiedMessagingChecked)
				{
					this.isUnifiedMessagingChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsFrontendTransportChecked
		{
			get
			{
				return this.isFrontendTransportChecked;
			}
			set
			{
				if (value != this.IsFrontendTransportChecked)
				{
					this.isFrontendTransportChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsCentralAdminChecked
		{
			get
			{
				return this.isCentralAdminChecked;
			}
			set
			{
				if (value != this.IsCentralAdminChecked)
				{
					this.isCentralAdminChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsCentralAdminDatabaseChecked
		{
			get
			{
				return this.isCentralAdminDatabaseChecked;
			}
			set
			{
				if (value != this.IsCentralAdminDatabaseChecked)
				{
					this.isCentralAdminDatabaseChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsCentralAdminFrontEndChecked
		{
			get
			{
				return this.isCentralAdminFrontEndChecked;
			}
			set
			{
				if (value != this.IsCentralAdminFrontEndChecked)
				{
					this.isCentralAdminFrontEndChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsMonitoringChecked
		{
			get
			{
				return this.isMonitoringChecked;
			}
			set
			{
				if (value != this.IsMonitoringChecked)
				{
					this.isMonitoringChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsAdminToolsChecked
		{
			get
			{
				return (!base.SetupContext.HasPrepareADParameters && (this.IsBridgeheadChecked || this.IsClientAccessChecked || this.IsGatewayChecked || this.IsMailboxChecked || this.IsUnifiedMessagingChecked || this.IsFrontendTransportChecked || this.IsCafeChecked || this.IsCentralAdminChecked || this.IsCentralAdminDatabaseChecked || this.IsCentralAdminFrontEndChecked || this.IsMonitoringChecked || this.IsOSPChecked)) || this.isAdminToolsChecked;
			}
			set
			{
				if (value != this.IsAdminToolsChecked)
				{
					this.isAdminToolsChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsLanguagePacksChecked
		{
			get
			{
				return this.isLanguagePacksChecked;
			}
			set
			{
				if (value != this.IsLanguagePacksChecked)
				{
					this.isLanguagePacksChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public bool IsOSPChecked
		{
			get
			{
				return this.isOSPChecked;
			}
			set
			{
				if (value != this.IsOSPChecked)
				{
					this.isOSPChecked = value;
					this.UpdateDataHandlers();
				}
			}
		}

		public virtual bool IsBridgeheadEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("BridgeheadRole");
			}
		}

		public virtual bool IsClientAccessEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("ClientAccessRole");
			}
		}

		public virtual bool IsGatewayEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("GatewayRole");
			}
		}

		public virtual bool IsMailboxEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("MailboxRole");
			}
		}

		public virtual bool IsCentralAdminDatabaseEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CentralAdminDatabaseRole");
			}
		}

		public virtual bool IsCentralAdminEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CentralAdminRole");
			}
		}

		public virtual bool IsCentralAdminFrontEndEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CentralAdminFrontEndRole");
			}
		}

		public virtual bool IsMonitoringEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("MonitoringRole");
			}
		}

		protected bool IsADDependentRoleChecked
		{
			get
			{
				return this.IsCafeChecked || this.IsFrontendTransportChecked || this.IsBridgeheadChecked || this.IsClientAccessChecked || this.IsMailboxChecked || this.IsUnifiedMessagingChecked;
			}
		}

		public virtual bool IsUnifiedMessagingEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("UnifiedMessagingRole");
			}
		}

		public virtual bool IsFrontendTransportEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("FrontendTransportRole");
			}
		}

		public virtual bool IsCafeEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("CafeRole");
			}
		}

		public virtual bool IsAdminToolsEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("AdminToolsRole");
			}
		}

		public virtual bool IsOSPEnabled
		{
			get
			{
				return base.SetupContext.IsInstalledLocal("OSPRole");
			}
		}

		public virtual decimal RequiredDiskSpace
		{
			get
			{
				decimal num = 0m;
				foreach (string installableUnitName in this.SelectedInstallableUnits)
				{
					num += InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName).Size;
				}
				return num;
			}
		}

		public NonRootLocalLongFullPath InstallationPath
		{
			get
			{
				if (base.SetupContext.TargetDir == null)
				{
					if (!base.SetupContext.IsCleanMachine)
					{
						base.SetupContext.TargetDir = base.SetupContext.InstalledPath;
					}
					else if (base.SetupContext.BackupInstalledPath != null)
					{
						base.SetupContext.TargetDir = base.SetupContext.BackupInstalledPath;
					}
					else
					{
						string path = "V" + 15.ToString();
						string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
						string path2 = Path.Combine("Microsoft\\Exchange Server", path);
						base.SetupContext.TargetDir = NonRootLocalLongFullPath.Parse(Path.Combine(folderPath, path2));
					}
				}
				return base.SetupContext.TargetDir;
			}
			set
			{
				if (this.InstallationPath != value && this.CanChangeInstallationPath)
				{
					base.SetupContext.TargetDir = value;
				}
			}
		}

		public bool CanChangeInstallationPath
		{
			get
			{
				return base.SetupContext.IsCleanMachine;
			}
		}

		public abstract bool CanChangeSourceDir { get; }

		public LongPath SourceDir
		{
			get
			{
				return base.SetupContext.SourceDir;
			}
			set
			{
				if (null == value)
				{
					throw new SourceDirNotSpecifiedException();
				}
				base.SetupContext.SourceDir = value;
			}
		}

		public decimal AvailableDiskSpace
		{
			get
			{
				decimal result = 0.0m;
				if (this.InstallationPath != null)
				{
					try
					{
						DriveInfo driveInfo = new DriveInfo(this.InstallationPath.DriveName);
						result = driveInfo.AvailableFreeSpace / 1048576m;
					}
					catch (ArgumentException)
					{
					}
					catch (IOException)
					{
					}
				}
				return result;
			}
		}

		public virtual bool HasChanges
		{
			get
			{
				return this.SelectedInstallableUnits.Count > 0;
			}
		}

		public virtual bool RebootRequired
		{
			get
			{
				return false;
			}
		}

		public static ValidationError[] ValidateInstallationPath(string path, bool skipDriveFormatCheck = false)
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			try
			{
				DriveInfo driveInfo = new DriveInfo(path);
				if (driveInfo.DriveType != DriveType.Fixed)
				{
					list.Add(new SetupValidationError(Strings.InstallationPathInvalidDriveTypeInformation(path)));
				}
				if (!skipDriveFormatCheck && driveInfo.DriveFormat != "NTFS")
				{
					list.Add(new SetupValidationError(Strings.InstallationPathInvalidDriveFormatInformation(path)));
				}
				string targetPath = ModeDataHandler.NormalizePath(path);
				if (ModeDataHandler.UnderPaths(targetPath, new string[]
				{
					ModeDataHandler.GetProfilesDirectory()
				}))
				{
					list.Add(new SetupValidationError(Strings.InstallationPathInvalidProfilesInformation(path)));
				}
				if (ModeDataHandler.UnderPaths(targetPath, ModeDataHandler.GetShellFolders()))
				{
					list.Add(new SetupValidationError(Strings.InstallationPathUnderUserProfileInformation(path)));
				}
			}
			catch (IOException)
			{
				list.Add(new SetupValidationError(Strings.InstallationPathInvalidInformation(path)));
			}
			catch (ArgumentException)
			{
				list.Add(new SetupValidationError(Strings.InstallationPathInvalidInformation(path)));
			}
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		private static string NormalizePath(string path)
		{
			string text = null;
			if (path != null)
			{
				text = path.Trim();
				if (text != string.Empty && text[text.Length - 1] != Path.DirectorySeparatorChar)
				{
					StringBuilder stringBuilder = new StringBuilder(text);
					stringBuilder.Append(Path.DirectorySeparatorChar);
					text = stringBuilder.ToString();
				}
			}
			return text;
		}

		private static string GetProfilesDirectory()
		{
			string path = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList", "ProfilesDirectory", string.Empty);
			return ModeDataHandler.NormalizePath(path);
		}

		private static string[] GetShellFolders()
		{
			List<string> list = new List<string>();
			string[] result;
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"))
			{
				if (registryKey != null)
				{
					string[] valueNames = registryKey.GetValueNames();
					foreach (string name in valueNames)
					{
						string path = registryKey.GetValue(name) as string;
						list.Add(ModeDataHandler.NormalizePath(path));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		private static bool UnderPaths(string targetPath, params string[] paths)
		{
			bool flag = false;
			foreach (string text in paths)
			{
				flag = (!string.IsNullOrEmpty(text) && targetPath.StartsWith(text, true, CultureInfo.InvariantCulture));
				if (flag)
				{
					SetupLogger.Log(Strings.TheFirstPathUnderTheSecondPath(targetPath, text));
					break;
				}
			}
			return flag;
		}

		public ValidationError[] CheckForADErrors(bool throwADErrors)
		{
			List<ValidationError> list = new List<ValidationError>();
			if (((this.IsADDependentRoleChecked || base.SetupContext.HasPrepareADParameters || base.SetupContext.HasNewProvisionedServerParameters || base.SetupContext.HasRemoveProvisionedServerParameters) && !this.IgnoreValidatingRoleChanges && !base.SetupContext.ADInitializedSuccessfully) || (this.IgnoreValidatingRoleChanges && !base.SetupContext.ADInitializedSuccessfully))
			{
				LocalizedException ex;
				if (base.SetupContext.ADInitializationError != null)
				{
					ex = base.SetupContext.ADInitializationError;
				}
				else
				{
					ex = new ADRelatedUnknownErrorException();
				}
				SetupLogger.Log(ex.LocalizedString);
				if (throwADErrors)
				{
					throw ex;
				}
				list.Add(new SetupValidationError(ex.LocalizedString));
			}
			return list.ToArray();
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(ModeDataHandler.ValidateInstallationPath(this.InstallationPath.PathName, base.SetupContext.InstallationMode == InstallationModes.Uninstall));
			if (!this.IgnoreValidatingRoleChanges && this.RequiredDiskSpace > this.AvailableDiskSpace)
			{
				list.Add(new SetupValidationError(Strings.NotEnoughSpace(Math.Round(this.RequiredDiskSpace / 1024m, 2, MidpointRounding.AwayFromZero).ToString())));
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public bool IgnoreValidatingRoleChanges
		{
			get
			{
				return this.ignoreValidatingRoleChanges;
			}
			set
			{
				this.ignoreValidatingRoleChanges = value;
			}
		}

		protected bool HasConfigurationDataHandler(string installableUnitName)
		{
			return InstallableUnitConfigurationInfoManager.IsRoleBasedConfigurableInstallableUnit(installableUnitName) || installableUnitName == "LanguagePacks" || InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName);
		}

		public virtual void UpdatePreCheckTaskDataHandler()
		{
		}

		public override void UpdateWorkUnits()
		{
			SetupLogger.TraceEnter(new object[0]);
			this.UpdateDataHandlers();
			base.UpdateWorkUnits();
			SetupLogger.TraceExit();
		}

		protected bool OrgLevelConfigRequired()
		{
			bool result = false;
			if (this.Mode == InstallationModes.Install || this.Mode == InstallationModes.BuildToBuildUpgrade)
			{
				if (new InstallOrgCfgDataHandler(base.SetupContext, base.Connection)
				{
					SelectedInstallableUnits = this.SelectedInstallableUnits
				}.WillDataHandlerDoAnyWork())
				{
					result = true;
				}
			}
			else if (this.Mode == InstallationModes.Uninstall)
			{
				UninstallOrgCfgDataHandler uninstallOrgCfgDataHandler = new UninstallOrgCfgDataHandler(base.SetupContext, base.Connection);
				if (uninstallOrgCfgDataHandler.WillDataHandlerDoAnyWork())
				{
					result = true;
				}
			}
			return result;
		}

		[Conditional("DEBUG")]
		protected void AssertDataHandlerIsAddedAtMostOnce(DataHandler datahandler, string name)
		{
			int num = 0;
			foreach (DataHandler dataHandler in base.DataHandlers)
			{
				if (dataHandler == datahandler)
				{
					num++;
				}
			}
		}

		protected LongPath UmSourceDir
		{
			get
			{
				if (base.SetupContext.SourceDir == null)
				{
					return null;
				}
				string path = Path.Combine(base.SetupContext.SourceDir.PathName, "en");
				LongPath result;
				if (!LongPath.TryParse(path, out result))
				{
					return null;
				}
				return result;
			}
		}

		private const string DefaultInstallationPath = "Microsoft\\Exchange Server";

		private bool isBridgeheadChecked;

		private bool isClientAccessChecked;

		private bool isGatewayChecked;

		private bool isMailboxChecked;

		private bool isUnifiedMessagingChecked;

		private bool isFrontendTransportChecked;

		private bool isCafeChecked;

		private bool isAdminToolsChecked;

		private bool isLanguagePacksChecked;

		private bool isCentralAdminChecked;

		private bool isCentralAdminDatabaseChecked;

		private bool isCentralAdminFrontEndChecked;

		private bool isMonitoringChecked;

		private bool isOSPChecked;

		private List<string> selectedInstallableUnits;

		private SetupBindingTaskDataHandler preSetupDataHandler;

		private SetupBindingTaskDataHandler postSetupDataHandler;

		private SetupBindingTaskDataHandler postFileCopyDataHandler;

		private Version previousVersion;

		private bool ignoreValidatingRoleChanges;
	}
}
