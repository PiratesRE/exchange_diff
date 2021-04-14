using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.AcquireLanguagePack;
using Microsoft.Exchange.Setup.SignatureVerification;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PrereqBaseTaskDataHandler : SetupSingleTaskDataHandler
	{
		public PrereqBaseTaskDataHandler(string command, string workUnitText, Icon workUnitIcon, ISetupContext context, MonadConnection connection) : base(context, command, connection)
		{
			base.WorkUnit.Text = workUnitText;
			base.WorkUnit.Icon = workUnitIcon;
		}

		public PrereqBaseTaskDataHandler(string command, string role, string workUnitText, Icon workUnitIcon, ISetupContext context, MonadConnection connection) : base(context, command, connection)
		{
			base.WorkUnit.Text = workUnitText;
			base.WorkUnit.Icon = workUnitIcon;
			this.Roles = new List<string>(new string[]
			{
				role
			});
			this.InitializeParameters();
		}

		public PrereqBaseTaskDataHandler(string command, string installableUnitName, ISetupContext context, MonadConnection connection) : base(context, command, connection)
		{
			InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
			base.WorkUnit.Text = installableUnitConfigurationInfoByName.DisplayName + " " + Strings.Prereqs;
			base.WorkUnit.Icon = null;
			base.WorkUnit.CanShowExecutedCommand = false;
			this.Roles = new List<string>(new string[]
			{
				this.GetRoleName(installableUnitConfigurationInfoByName.Name)
			});
			this.InitializeParameters();
		}

		public bool HasRoles
		{
			get
			{
				return this.Roles != null && this.Roles.Count != 0;
			}
		}

		public bool HasSelectedInstallableUnits
		{
			get
			{
				return this.SelectedInstallableUnits != null && this.SelectedInstallableUnits.Count != 0;
			}
		}

		public void AddRole(string role)
		{
			if (!this.Roles.Contains(role))
			{
				this.Roles.Add(role);
			}
		}

		public void AddRoleByUnitName(string installableUnitName)
		{
			InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
			this.AddRole(this.GetRoleName(installableUnitConfigurationInfoByName.Name));
		}

		public void AddSelectedInstallableUnit(string unit)
		{
			if (!this.SelectedInstallableUnits.Contains(unit))
			{
				this.SelectedInstallableUnits.Add(unit);
			}
		}

		public void AddSelectedInstallableUnits(IEnumerable<string> units)
		{
			this.SelectedInstallableUnits.AddRange(units);
			this.SelectedInstallableUnits = this.SelectedInstallableUnits.Distinct<string>().ToList<string>();
		}

		public void InitializeParameters()
		{
			if (!this.HasRoles)
			{
				throw new ArgumentNullException("this.Roles should not be null or empty.");
			}
			this.ScanType = this.GetScanType(base.SetupContext.InstallationMode);
			this.DomainController = base.SetupContext.DomainController;
			this.RunningVersion = base.SetupContext.RunningVersion;
			string roleName = this.GetRoleName("AdminToolsRole");
			string roleName2 = this.GetRoleName("GatewayRole");
			if (base.SetupContext.ADInitializationError != null && base.SetupContext.ADInitializationError.GetType() == typeof(ADInitializationException) && (this.Roles.Count != 2 || !this.Roles.Contains(roleName) || !this.Roles.Contains(roleName2)) && (this.Roles.Count != 1 || (!this.Roles.Contains(roleName) && !this.Roles.Contains(roleName2))))
			{
				this.ADInitError = base.SetupContext.ADInitializationError.Message;
			}
			if (this.Roles.Contains(this.GetRoleName("LanguagePacks")))
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.LanguagePackDir] = base.SetupContext.LanguagePackPath;
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.LanguagesAvailableToInstall] = (base.SetupContext.LanguagePacksToInstall.Count > 0);
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.SufficientLanguagePackDiskSpace] = this.IsDiskSpaceSufficientForLanguagePackInstallation;
				bool flag = true;
				if (this.Roles.Contains(this.GetRoleName("MailboxRole")))
				{
					flag = this.IsLPVersionCompatible;
				}
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.LanguagePackVersioning] = flag;
			}
			if (this.Roles.Contains(this.GetRoleName("UmLanguagePack")))
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.AlreadyInstallUMLanguages] = this.UMLanguagePackInstalledFromSelectedCultures;
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.LanguagePacksInstalled] = this.IsClientLanguagePackInstalledForSelectedCultures;
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.SufficientLanguagePackDiskSpace] = this.IsDiskSpaceSufficientForLanguagePackInstallation;
			}
			this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.HostingDeploymentEnabled] = base.SetupContext.HostingDeployment;
			this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PathToDCHybridConfigFile] = base.SetupContext.TenantOrganizationConfig;
		}

		internal List<string> Roles
		{
			get
			{
				if (this.GetValueOrDefault(PrereqBaseTaskDataHandler.MandatoryParameterNames.Roles, null) == null)
				{
					this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.Roles] = new List<string>();
				}
				return (List<string>)this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.Roles];
			}
			private set
			{
				this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.Roles] = value;
			}
		}

		private ScanType ScanType
		{
			get
			{
				return (ScanType)this.GetValueOrDefault(PrereqBaseTaskDataHandler.MandatoryParameterNames.ScanType, ScanType.PrecheckInstall);
			}
			set
			{
				this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.ScanType] = value;
			}
		}

		public List<string> SelectedInstallableUnits
		{
			get
			{
				return this.selectedInstallableUnits;
			}
			set
			{
				this.selectedInstallableUnits = value;
			}
		}

		public string DomainController
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.MandatoryParameterNames.DomainController, null);
			}
			set
			{
				this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.DomainController] = value;
			}
		}

		public Version RunningVersion
		{
			get
			{
				return (Version)this.GetValueOrDefault(PrereqBaseTaskDataHandler.MandatoryParameterNames.ExchangeVersion, null);
			}
			set
			{
				this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.ExchangeVersion] = value;
			}
		}

		public ushort AdamLdapPort
		{
			get
			{
				return (ushort)((int)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.AdamPort, 0));
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.AdamPort] = value;
			}
		}

		public ushort AdamSslPort
		{
			get
			{
				return (ushort)((int)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.AdamSslPort, 0));
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.AdamSslPort] = value;
			}
		}

		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.CustomerFeedbackEnabled, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.CustomerFeedbackEnabled] = value;
			}
		}

		public string NewProvisionedServerName
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.NewProvisionedServerName, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.NewProvisionedServerName] = value;
			}
		}

		public string RemoveProvisionedServerName
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.RemoveProvisionedServerName, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.RemoveProvisionedServerName] = value;
			}
		}

		public LocalLongFullPath TargetDir
		{
			get
			{
				return (LocalLongFullPath)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.TargetDir, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.TargetDir] = value;
			}
		}

		public bool PrepareAllDomains
		{
			get
			{
				return (bool)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareAllDomains, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareAllDomains] = value;
			}
		}

		public bool PrepareSCT
		{
			get
			{
				return (bool)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareSCT, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareSCT] = value;
			}
		}

		public bool PrepareDomain
		{
			get
			{
				return this.setPrepareDomain;
			}
			set
			{
				this.setPrepareDomain = value;
				if (this.setPrepareDomain)
				{
					if (!this.optionalParameters.ContainsKey(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain))
					{
						this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain] = null;
						return;
					}
				}
				else
				{
					this.optionalParameters.Remove(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain);
				}
			}
		}

		public string PrepareDomainTarget
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain] = value;
			}
		}

		public bool PrepareSchema
		{
			get
			{
				return (bool)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareSchema, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareSchema] = value;
			}
		}

		public bool PrepareOrganization
		{
			get
			{
				return (bool)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareOrganization, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareOrganization] = value;
			}
		}

		public string ADInitError
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.ADInitError, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.ADInitError] = value;
			}
		}

		public bool? ActiveDirectorySplitPermissions
		{
			get
			{
				return (bool?)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.ActiveDirectorySplitPermissions, null);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.ActiveDirectorySplitPermissions] = value;
			}
		}

		public bool HostingDeploymentEnabled
		{
			get
			{
				return (bool)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.HostingDeploymentEnabled, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.HostingDeploymentEnabled] = value;
			}
		}

		public string PathToDCHybridConfigFile
		{
			get
			{
				return (string)this.GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames.PathToDCHybridConfigFile, false);
			}
			set
			{
				this.optionalParameters[PrereqBaseTaskDataHandler.OptionalParameterNames.PathToDCHybridConfigFile] = value;
			}
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			if (base.SetupContext.IsDatacenter)
			{
				base.Parameters.AddWithValue("IsDatacenter", base.SetupContext.IsDatacenter);
			}
			if (this.Roles.Contains(this.GetRoleName("ClientAccessRole")) || this.Roles.Contains(this.GetRoleName("MailboxRole")) || this.Roles.Contains(this.GetRoleName("CafeRole")) || this.Roles.Contains(this.GetRoleName("FrontendTransportRole")))
			{
				base.SetupContext.UpdateIsW3SVCStartOk();
			}
			List<string> list = new List<string>();
			if (this.HasSelectedInstallableUnits)
			{
				foreach (string installableUnitName in this.SelectedInstallableUnits)
				{
					list.Add(this.GetRoleName(installableUnitName));
				}
			}
			if (this.Roles.Contains("Global"))
			{
				list.Add("Global");
			}
			this.commonParameters[PrereqBaseTaskDataHandler.MandatoryParameterNames.SetupRoles] = list;
			List<string> list2 = new List<string>();
			list2.AddRange(Enum.GetNames(typeof(PrereqBaseTaskDataHandler.MandatoryParameterNames)));
			list2.Sort();
			foreach (string value in list2)
			{
				PrereqBaseTaskDataHandler.MandatoryParameterNames mandatoryParameterNames = (PrereqBaseTaskDataHandler.MandatoryParameterNames)Enum.Parse(typeof(PrereqBaseTaskDataHandler.MandatoryParameterNames), value);
				object value2 = this.commonParameters[mandatoryParameterNames];
				base.Parameters.AddWithValue(mandatoryParameterNames.ToString(), value2);
			}
			List<PrereqBaseTaskDataHandler.OptionalParameterNames> list3 = new List<PrereqBaseTaskDataHandler.OptionalParameterNames>();
			list3.AddRange(this.optionalParameters.Keys);
			list3.Sort();
			foreach (PrereqBaseTaskDataHandler.OptionalParameterNames optionalParameterNames in list3)
			{
				object obj = this.optionalParameters[optionalParameterNames];
				if (obj != null || (optionalParameterNames == PrereqBaseTaskDataHandler.OptionalParameterNames.PrepareDomain && this.setPrepareDomain))
				{
					base.Parameters.AddWithValue(optionalParameterNames.ToString(), obj);
				}
			}
			SetupLogger.TraceExit();
		}

		private ScanType GetScanType(InstallationModes mode)
		{
			ScanType result;
			switch (base.SetupContext.InstallationMode)
			{
			case InstallationModes.Install:
				result = ScanType.PrecheckInstall;
				break;
			case InstallationModes.BuildToBuildUpgrade:
				result = ScanType.PrecheckUpgrade;
				break;
			case InstallationModes.DisasterRecovery:
				result = ScanType.PrecheckDR;
				break;
			case InstallationModes.Uninstall:
				result = ScanType.PrecheckUninstall;
				break;
			default:
				throw new ArgumentException("There is no scan type for this mode", "mode");
			}
			return result;
		}

		private string GetRoleName(string installableUnitName)
		{
			if (InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName))
			{
				return "UmLanguagePack";
			}
			return installableUnitName.Replace("Role", "");
		}

		private object GetValueOrDefault(PrereqBaseTaskDataHandler.OptionalParameterNames parameterName, object defaultValue)
		{
			object result;
			if (this.optionalParameters.TryGetValue(parameterName, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private object GetValueOrDefault(PrereqBaseTaskDataHandler.MandatoryParameterNames parameterName, object defaultValue)
		{
			object result;
			if (this.commonParameters.TryGetValue(parameterName, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private bool IsClientLanguagePackInstalledForSelectedCultures
		{
			get
			{
				bool flag = true;
				foreach (CultureInfo language in base.SetupContext.SelectedCultures)
				{
					flag = (flag && this.IsClientLanguageInstalledForCulture(language));
				}
				return flag;
			}
		}

		private bool IsClientLanguageInstalledForCulture(CultureInfo language)
		{
			List<CultureInfo> supportedClientCultures = UmCultures.GetSupportedClientCultures();
			return UmCultures.GetBestSupportedCulture(supportedClientCultures, language) != null;
		}

		private string UMLanguagePackInstalledFromSelectedCultures
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (CultureInfo cultureInfo in base.SetupContext.SelectedCultures)
				{
					if (base.SetupContext.InstalledUMLanguagePacks.Contains(cultureInfo))
					{
						stringBuilder.Append(cultureInfo.ToString()).Append(" ");
					}
				}
				return stringBuilder.ToString().Trim();
			}
		}

		private bool IsDiskSpaceSufficientForLanguagePackInstallation
		{
			get
			{
				long num = 0L;
				DriveInfo driveInfo = new DriveInfo(base.SetupContext.TargetDir.DriveName.Substring(0, 1));
				if (this.Roles.Contains(this.GetRoleName("LanguagePacks")))
				{
					foreach (long num2 in base.SetupContext.LanguagesToInstall.Values)
					{
						num += num2;
					}
				}
				if (this.Roles.Contains(this.GetRoleName("UmLanguagePack")))
				{
					foreach (CultureInfo culture in base.SetupContext.SelectedCultures)
					{
						LongPath umlanguagePackFilename = UMLanguagePackHelper.GetUMLanguagePackFilename(base.SetupContext.SourceDir.PathName, culture);
						if (umlanguagePackFilename != null && File.Exists(umlanguagePackFilename.PathName))
						{
							long length = new FileInfo(umlanguagePackFilename.PathName).Length;
							num += length * 4L;
						}
					}
				}
				return driveInfo.AvailableFreeSpace > num;
			}
		}

		private bool IsLPVersionCompatible
		{
			get
			{
				bool result;
				try
				{
					bool flag = true;
					string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LPVersioning.xml");
					LanguagePackVersion languagePackVersion = null;
					if (base.SetupContext.LanguagePackSourceIsBundle)
					{
						string text2 = Path.Combine(Path.GetTempPath(), "LBVersioningFromBundle");
						EmbeddedCabWrapper.ExtractFiles(base.SetupContext.LanguagePackPath.ToString(), text2, "LPVersioning.xml");
						text2 = Path.Combine(text2, "LPVersioning.xml");
						if (!File.Exists(text2))
						{
							throw new LanguagePackBundleLoadException(Strings.LPVersioningExtractionFailed(text2));
						}
						languagePackVersion = new LanguagePackVersion(text, text2);
						flag = languagePackVersion.IsExchangeInApplicableRange(new Version(LanguagePackVersion.GetBuildVersion(text2)));
					}
					else if (base.SetupContext.InstallationMode == InstallationModes.BuildToBuildUpgrade && base.SetupContext.IsLanaguagePacksInstalled)
					{
						languagePackVersion = new LanguagePackVersion(text, text);
					}
					if (flag && (base.SetupContext.LanguagePackSourceIsBundle || (base.SetupContext.InstallationMode == InstallationModes.BuildToBuildUpgrade && base.SetupContext.IsLanaguagePacksInstalled)))
					{
						string text3 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LPVersioning.xml");
						if (text3 != languagePackVersion.LanguagePackVersioningPath)
						{
							File.Copy(languagePackVersion.LanguagePackVersioningPath, text3, true);
						}
					}
					result = flag;
				}
				catch (SignatureVerificationException e)
				{
					SetupLogger.LogError(e);
					result = true;
				}
				catch (LPVersioningValueException e2)
				{
					SetupLogger.LogError(e2);
					result = false;
				}
				catch (LanguagePackBundleLoadException e3)
				{
					SetupLogger.LogError(e3);
					result = false;
				}
				catch (CabUtilityWrapperException e4)
				{
					SetupLogger.LogError(e4);
					result = false;
				}
				return result;
			}
		}

		public const string OrganizationRole = "Global";

		private List<string> selectedInstallableUnits = new List<string>();

		private Dictionary<PrereqBaseTaskDataHandler.MandatoryParameterNames, object> commonParameters = new Dictionary<PrereqBaseTaskDataHandler.MandatoryParameterNames, object>();

		private Dictionary<PrereqBaseTaskDataHandler.OptionalParameterNames, object> optionalParameters = new Dictionary<PrereqBaseTaskDataHandler.OptionalParameterNames, object>();

		private bool setPrepareDomain;

		private enum MandatoryParameterNames
		{
			DomainController,
			ExchangeVersion,
			Roles,
			ScanType,
			SetupRoles
		}

		private enum OptionalParameterNames
		{
			AdamPort,
			AdamSslPort,
			CreatePublicDB,
			NewProvisionedServerName,
			RemoveProvisionedServerName,
			TargetDir,
			PrepareAllDomains,
			PrepareDomain,
			PrepareSCT,
			PrepareOrganization,
			PrepareSchema,
			ADInitError,
			CustomerFeedbackEnabled,
			LanguagePackDir,
			LanguagesAvailableToInstall,
			SufficientLanguagePackDiskSpace,
			LanguagePackVersioning,
			ActiveDirectorySplitPermissions,
			LanguagePacksInstalled,
			AlreadyInstallUMLanguages,
			HostingDeploymentEnabled,
			PathToDCHybridConfigFile
		}
	}
}
