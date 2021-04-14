using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics.Components.Setup;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Management.Analysis.Builders;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class PrereqAnalysis : Analysis
	{
		public PrereqAnalysis(IDataProviderFactory providers, SetupMode mode, SetupRole role, GlobalParameters globalParameters) : base(providers, delegate(AnalysisMember x)
		{
			AppliesToModeFeature appliesToModeFeature = x.Features.OfType<AppliesToModeFeature>().FirstOrDefault<AppliesToModeFeature>();
			AppliesToRoleFeature appliesToRoleFeature = x.Features.OfType<AppliesToRoleFeature>().FirstOrDefault<AppliesToRoleFeature>();
			return (appliesToModeFeature == null || appliesToModeFeature.Contains(mode)) && (appliesToRoleFeature == null || appliesToRoleFeature.Contains(role));
		}, delegate(AnalysisMember x)
		{
			AppliesToModeFeature appliesToModeFeature = x.Features.OfType<AppliesToModeFeature>().FirstOrDefault<AppliesToModeFeature>();
			AppliesToRoleFeature appliesToRoleFeature = x.Features.OfType<AppliesToRoleFeature>().FirstOrDefault<AppliesToRoleFeature>();
			return x is Rule && (appliesToModeFeature == null || appliesToModeFeature.Contains(mode)) && (appliesToRoleFeature == null || appliesToRoleFeature.Contains(role));
		})
		{
			PrereqAnalysis <>4__this = this;
			if (globalParameters == null)
			{
				throw new ArgumentNullException("globalParameters");
			}
			this.globalParameters = globalParameters;
			this.CreateGlobalParameterPrereqProperties();
			this.ComputerNameNetBIOS = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.ManagedMethodProvider.GetComputerNameEx(ValidationConstant.ComputerNameFormat.ComputerNameNetBIOS)));
			this.ComputerNameDnsHostname = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.ManagedMethodProvider.GetComputerNameEx(ValidationConstant.ComputerNameFormat.ComputerNameDnsHostname)));
			this.ComputerNameDnsDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.ManagedMethodProvider.GetComputerNameEx(ValidationConstant.ComputerNameFormat.ComputerNameDnsDomain)));
			this.ComputerNameDnsFullyQualified = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.ManagedMethodProvider.GetComputerNameEx(ValidationConstant.ComputerNameFormat.ComputerNameDnsFullyQualified)));
			this.ComputerNameDiscrepancy = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.NetBIOSNameNotMatchesDNSHostName).Condition((Result<object> x) => new RuleResult(!string.Equals(this.ComputerNameDnsHostname.Results.Value, this.ComputerNameNetBIOS.Results.Value, StringComparison.CurrentCultureIgnoreCase)));
			this.FqdnMissing = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.FqdnMissing).Condition((Result<object> x) => new RuleResult(string.IsNullOrEmpty(this.ComputerNameDnsDomain.Results.Value)));
			this.DNSDomainNameNotValid = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidDNSDomainName).Condition((Result<object> x) => new RuleResult(!Regex.IsMatch(this.ComputerNameDnsDomain.Results.Value, "^[A-Za-z0-9\\-\\.]*$")));
			this.OrgMicrosoftExchServicesConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName",
						"msExchMixedMode",
						"msExchVersion"
					}, "objectClass=msExchOrganizationContainer", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.CreateMonadPrereqProperties();
			this.ShortServerName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_ComputerSystem")[0].TryGetValue("Name", out obj))
				{
					return new Result<string>(obj.ToString());
				}
				return new Result<string>(string.Empty);
			});
			this.LangPackBundleVersioning = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.LanguagePacks).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.LangPackBundleVersioning).Condition((Result<object> x) => new RuleResult(!this.globalParameters.LanguagePackVersioning));
			this.LangPackBundleCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.LanguagePacks).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.LangPackBundleCheck).Condition((Result<object> x) => new RuleResult(!this.globalParameters.LanguagesAvailableToInstall));
			this.LangPackDiskSpaceCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.LanguagePacks).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.LangPackDiskSpaceCheck).Condition((Result<object> x) => new RuleResult(!this.globalParameters.SufficientLanguagePackDiskSpace));
			this.LangPackUpgradeVersioning = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.LanguagePacks).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.LangPackUpgradeVersioning).Condition((Result<object> x) => new RuleResult(!this.globalParameters.LanguagePackVersioning));
			this.LangPackInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.LanguagePacks).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Warning<RuleBuilder<object>>().Message((Result x) => Strings.LangPackInstalled).Condition((Result<object> x) => new RuleResult(this.globalParameters.LanguagePacksInstalled));
			this.AlreadyInstalledUMLangPacks = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UmLanguagePack).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.AlreadyInstalledUMLangPacks(this.globalParameters.AlreadyInstalledUMLanguages)).Condition((Result<object> x) => new RuleResult(this.globalParameters.AlreadyInstalledUMLanguages != null && this.globalParameters.AlreadyInstalledUMLanguages != string.Empty));
			this.UMLangPackDiskSpaceCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UmLanguagePack).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.LangPackDiskSpaceCheck).Condition((Result<object> x) => new RuleResult(!this.globalParameters.SufficientLanguagePackDiskSpace));
			this.ComputerDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_ComputerSystem")[0].TryGetValue("Domain", out obj))
				{
					return new Result<string>(obj.ToString());
				}
				return new Result<string>(string.Empty);
			});
			this.CreateActiveDirectoryPrereqProperties();
			this.CreateNativePrereqProperties();
			this.CreateWmiPrereqProperties();
			this.CreateRegistryPrereqProperties();
			this.Iis32BitMode = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.Iis32BitMode).Condition((Result<object> x) => new RuleResult(base.Providers.WebAdminDataProvider.Enable32BitAppOnWin64));
			this.HostingModeNotAvailable = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.HostingModeNotAvailable).Condition((Result<object> x) => new RuleResult(this.globalParameters.HostingDeploymentEnabled));
			this.DidTenantSettingCreatedAnException = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message(delegate(Result x)
			{
				if (!this.IsHybridObjectFoundOnPremises.Results.Result.HasException)
				{
					return this.IsExchangeVersionCorrectForTenant.Results.Result.Exception.Message;
				}
				return this.IsHybridObjectFoundOnPremises.Results.Result.Exception.Message;
			}).Condition((Result<object> x) => new RuleResult(this.IsHybridObjectFoundOnPremises.Results.ValueOrDefault && this.IsExchangeVersionCorrectForTenant.Results.Result.HasException));
			this.DidOnPremisesSettingCreatedAnException = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => this.IsHybridObjectFoundOnPremises.Results.Result.Exception.Message).Condition((Result<object> x) => new RuleResult(this.IsHybridObjectFoundOnPremises.Results.Result.HasException));
			this.HybridIsEnabledAndTenantVersionIsNotUpgraded = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.OrgConfigHashDoesNotExist).Condition((Result<object> x) => new RuleResult(!this.IsHybridObjectFoundOnPremises.Results.Result.HasException && !this.IsExchangeVersionCorrectForTenant.Results.Result.HasException && this.IsHybridObjectFoundOnPremises.Results.ValueOrDefault && !this.IsExchangeVersionCorrectForTenant.Results.ValueOrDefault));
			this.IsHybridObjectFoundOnPremises = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).SetValue((Result<object> x) => new Result<bool>(!this.WasSetupStartedFromGUI.Results.ValueOrDefault && base.Providers.HybridConfigurationDetectionProvider.RunOnPremisesHybridTest()));
			this.WasSetupStartedFromGUI = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				Process parentProcess = SetupPrereqChecks.GetParentProcess();
				string fileName = parentProcess.MainModule.FileName;
				parentProcess.Dispose();
				if (string.IsNullOrEmpty(fileName))
				{
					return new Result<bool>(false);
				}
				return new Result<bool>(fileName.Contains("SetupUI.exe"));
			});
			this.IsExchangeVersionCorrectForTenant = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install | SetupMode.DisasterRecovery).SetValue((Result<object> x) => new Result<bool>(base.Providers.HybridConfigurationDetectionProvider.RunTenantHybridTest(this.globalParameters.PathToDCHybridConfigFile)));
			this.ValidOSSuite = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if ((this.SuiteMask.Results.ValueOrDefault & 2U) > 0U || (this.SuiteMask.Results.ValueOrDefault & 128U) > 0U || this.SuiteMask.Results.ValueOrDefault == 272U)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.Windows2K8R2Version = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				OperatingSystem osversion = Environment.OSVersion;
				int num = 0;
				if (!string.IsNullOrEmpty(osversion.ServicePack))
				{
					MatchCollection matchCollection = Regex.Matches(osversion.ServicePack, "(\\d+\\.?\\d*|\\.\\d+)");
					if (matchCollection.Count == 1)
					{
						int.TryParse(matchCollection[0].Value, out num);
					}
				}
				if (this.OSProductType.Results.ValueOrDefault > 1U && this.ValidOSSuite.Results.ValueOrDefault && osversion.Version.Major == 6 && osversion.Version.Minor == 1 && num >= 1)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.Windows8Version = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				OperatingSystem osversion = Environment.OSVersion;
				if (this.OSProductType.Results.ValueOrDefault > 1U && osversion.Version.Major == 6 && osversion.Version.Minor >= 2)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.Windows8ClientVersion = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				OperatingSystem osversion = Environment.OSVersion;
				if (this.OSProductType.Results.ValueOrDefault == 1U && osversion.Version.Major == 6 && osversion.Version.Minor >= 2)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.Windows7ClientVersion = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				OperatingSystem osversion = Environment.OSVersion;
				int num = 0;
				if (!string.IsNullOrEmpty(osversion.ServicePack))
				{
					MatchCollection matchCollection = Regex.Matches(osversion.ServicePack, "(\\d+\\.?\\d*|\\.\\d+)");
					if (matchCollection.Count == 1)
					{
						int.TryParse(matchCollection[0].Value, out num);
					}
				}
				if (this.OSProductType.Results.ValueOrDefault == 1U && osversion.Version.Major == 6 && osversion.Version.Minor == 1 && num >= 1)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.InstalledWindowsFeatures = Setting<object[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				string[] array = new string[]
				{
					"WinRM-IIS-Ext",
					"RSAT-Web-Server",
					"Web-Mgmt-Console",
					"NET-Framework",
					"NET-Framework-45-Features",
					"Web-Net-Ext45",
					"Web-ISAPI-Ext",
					"Web-ASP-NET45",
					"RPC-over-HTTP-proxy",
					"Server-Gui-Mgmt-Infra",
					"NET-WCF-HTTP-Activation45",
					"RSAT-ADDS-Tools",
					"RSAT-Clustering",
					"RSAT-Clustering-Mgmt",
					"RSAT-Clustering-PowerShell",
					"RSAT-Clustering-CmdInterface"
				};
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Import-Module 'ServerManager';");
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append("'-'");
					stringBuilder.Append("+");
					stringBuilder.Append(string.Format("(Get-WindowsFeature '{0}').Installed;", array[i]));
				}
				return new Result<object[]>(base.Providers.MonadDataProvider.ExecuteCommand(stringBuilder.ToString()));
			});
			this.IsWinRMIISExtensionInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 0)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[0];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRSATWebServerInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 1)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[1];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsNETFrameworkInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 3)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[3];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsNETFramework45FeaturesInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 4)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[4];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsWebNetExt45Installed = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 5)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[5];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsWebISAPIExtInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 6)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[6];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsWebASPNET45Installed = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 7)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[7];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRPCOverHTTPproxyInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 8)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[8];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsServerGuiMgmtInfraInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 9)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[9];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsWcfHttpActivation45Installed = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 10)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[10];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRsatAddsToolsInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 11)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[11];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRsatClusteringInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 12)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[12];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRsatClusteringMgmtInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 13)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[13];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRsatClusteringPowerShellInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 14)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[14];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.IsRsatClusteringCmdInterfaceInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.InstalledWindowsFeatures.Results.Value != null && this.InstalledWindowsFeatures.Results.Value.Length > 15)
				{
					string text = (string)this.InstalledWindowsFeatures.Results.Value[15];
					if (text.Length > 1)
					{
						value = Convert.ToBoolean(text.Substring(1));
					}
				}
				return new Result<bool>(value);
			});
			this.VCRedist2012Installed = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Gateway).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.FileVersionAtl110.Results.Value.CompareTo(new Version("11.00.50727.1")) >= 0)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.VCRedist2013Installed = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox | SetupRole.Bridgehead).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				if (this.FileVersionMsvcr120.Results.Value.CompareTo(new Version("12.00.21005.1")) >= 0)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.ValidOSVersion = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidOSVersion).Condition((Result<object> x) => new RuleResult(!this.Windows2K8R2Version.Results.ValueOrDefault && !this.Windows8Version.Results.ValueOrDefault));
			this.ValidOSVersionForAdminTools = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidOSVersionForAdminTools).Condition((Result<object> x) => new RuleResult(!this.Windows7ClientVersion.Results.ValueOrDefault && !this.Windows2K8R2Version.Results.ValueOrDefault && !this.Windows8ClientVersion.Results.ValueOrDefault && !this.Windows8Version.Results.ValueOrDefault));
			this.VC2012RedistDependencyRequirement = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.VC2012PrereqMissing).Condition((Result<object> x) => new RuleResult(!this.VCRedist2012Installed.Results.ValueOrDefault));
			this.VC2013RedistDependencyRequirement = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.VC2013PrereqMissing).Condition((Result<object> x) => new RuleResult(!this.VCRedist2013Installed.Results.ValueOrDefault));
		}

		public Rule HostingModeNotAvailable { get; set; }

		public Rule OSCheckedBuild { get; set; }

		public Rule EventSystemStopped { get; set; }

		public Rule MSDTCStopped { get; set; }

		public Rule MpsSvcStopped { get; set; }

		public Rule NetTcpPortSharingSvcNotAuto { get; set; }

		public Rule ComputerNotPartofDomain { get; set; }

		public Rule WarningInstallExchangeRolesOnDomainController { get; set; }

		public Rule InstallOnDCInADSplitPermissionMode { get; set; }

		public Rule SetADSplitPermissionWhenExchangeServerRolesOnDC { get; set; }

		public Rule ServerNameNotValid { get; set; }

		public Rule LocalComputerIsDCInChildDomain { get; set; }

		public Rule LoggedOntoDomain { get; set; }

		public Rule PrimaryDNSTestFailed { get; set; }

		public Rule HostRecordMissing { get; set; }

		public Rule LocalDomainModeMixed { get; set; }

		public Rule DomainPrepRequired { get; set; }

		public Rule ComputerRODC { get; set; }

		public Rule InvalidADSite { get; set; }

		public Rule W2K8R2PrepareSchemaLdifdeNotInstalled { get; set; }

		public Rule W2K8R2PrepareAdLdifdeNotInstalled { get; set; }

		public Rule MailboxRoleAlreadyExists { get; set; }

		public Rule ClientAccessRoleAlreadyExists { get; set; }

		public Rule UnifiedMessagingRoleAlreadyExists { get; set; }

		public Rule BridgeheadRoleAlreadyExists { get; set; }

		public Rule CafeRoleAlreadyExists { get; set; }

		public Rule FrontendTransportRoleAlreadyExists { get; set; }

		public Rule ServerWinWebEdition { get; set; }

		public Rule BridgeheadRoleNotPresentInSite { get; set; }

		public Rule ClientAccessRoleNotPresentInSite { get; set; }

		public Rule LonghornWmspdmoxNotInstalled { get; set; }

		public Rule Exchange2000or2003PresentInOrg { get; set; }

		public Rule RebootPending { get; set; }

		public Rule MinimumFrameworkNotInstalled { get; set; }

		public Rule Win7RpcHttpAssocCookieGuidUpdateNotInstalled { get; set; }

		public Rule SearchFoundationAssemblyLoaderKBNotInstalled { get; set; }

		public Rule Win2k12UrefsUpdateNotInstalled { get; set; }

		public Rule Win2k12RefsUpdateNotInstalled { get; set; }

		public Rule Win2k12RollupUpdateNotInstalled { get; set; }

		public Rule UnifiedMessagingRoleNotInstalled { get; set; }

		public Rule BridgeheadRoleNotInstalled { get; set; }

		public Rule NotLocalAdmin { get; set; }

		public Rule FirstSGFilesExist { get; set; }

		public Rule SecondSGFilesExist { get; set; }

		public Rule ExchangeVersionBlock { get; set; }

		public Rule VoiceMessagesInQueue { get; set; }

		public Rule ProcessNeedsToBeClosedOnUpgrade { get; set; }

		public Rule ProcessNeedsToBeClosedOnUninstall { get; set; }

		public Rule SendConnectorException { get; set; }

		public Rule MailboxLogDriveDoesNotExist { get; set; }

		public Rule MailboxEDBDriveDoesNotExist { get; set; }

		public Rule ServerIsSourceForSendConnector { get; set; }

		public Rule ServerIsGroupExpansionServer { get; set; }

		public Rule ServerIsDynamicGroupExpansionServer { get; set; }

		public Rule MemberOfDatabaseAvailabilityGroup { get; set; }

		public Rule DrMinVersionCheck { get; set; }

		public Rule RemoteRegException { get; set; }

		public Rule WinRMIISExtensionInstalled { get; set; }

		public Rule LangPackBundleVersioning { get; set; }

		public Rule LangPackBundleCheck { get; set; }

		public Rule LangPackDiskSpaceCheck { get; set; }

		public Rule LangPackInstalled { get; set; }

		public Rule AlreadyInstalledUMLangPacks { get; set; }

		public Rule UMLangPackDiskSpaceCheck { get; set; }

		public Rule LangPackUpgradeVersioning { get; set; }

		public Rule PendingRebootWindowsComponents { get; set; }

		public Rule Iis32BitMode { get; set; }

		public Rule SchemaUpdateRequired { get; set; }

		public Rule AdUpdateRequired { get; set; }

		public Rule GlobalUpdateRequired { get; set; }

		public Rule DomainPrepWithoutADUpdate { get; set; }

		public Rule LocalDomainPrep { get; set; }

		public Rule GlobalServerInstall { get; set; }

		public Rule DelegatedBridgeheadFirstInstall { get; set; }

		public Rule DelegatedCafeFirstInstall { get; set; }

		public Rule DelegatedFrontendTransportFirstInstall { get; set; }

		public Rule DelegatedMailboxFirstInstall { get; set; }

		public Rule DelegatedClientAccessFirstInstall { get; set; }

		public Rule DelegatedUnifiedMessagingFirstInstall { get; set; }

		public Rule DelegatedBridgeheadFirstSP1upgrade { get; set; }

		public Rule DelegatedUnifiedMessagingFirstSP1upgrade { get; set; }

		public Rule DelegatedClientAccessFirstSP1upgrade { get; set; }

		public Rule DelegatedMailboxFirstSP1upgrade { get; set; }

		public Rule CannotUninstallDelegatedServer { get; set; }

		public Rule PrepareDomainNotAdmin { get; set; }

		public Rule NoE12ServerWarning { get; set; }

		public Rule NoE14ServerWarning { get; set; }

		public Rule NotInSchemaMasterDomain { get; set; }

		public Rule NotInSchemaMasterSite { get; set; }

		public Rule ProvisionedUpdateRequired { get; set; }

		public Rule SchemaFSMONotWin2003SPn { get; set; }

		public Rule CannotUninstallClusterNode { get; set; }

		public Rule CannotUninstallOABServer { get; set; }

		public Rule DomainControllerIsOutOfSite { get; set; }

		public Rule ComputerNameDiscrepancy { get; set; }

		public Rule FqdnMissing { get; set; }

		public Rule DNSDomainNameNotValid { get; set; }

		public Rule DidTenantSettingCreatedAnException { get; set; }

		public Rule DidOnPremisesSettingCreatedAnException { get; set; }

		public Rule HybridIsEnabledAndTenantVersionIsNotUpgraded { get; set; }

		public Rule AdcFound { get; set; }

		public Rule AdInitErrorRule { get; set; }

		public Rule NoConnectorToStar { get; set; }

		public Rule DuplicateShortProvisionedName { get; set; }

		public Rule ForestLevelNotWin2003Native { get; set; }

		public Rule InhBlockPublicFolderTree { get; set; }

		public Rule PrepareDomainNotFound { get; set; }

		public Rule PrepareDomainModeMixed { get; set; }

		public Rule RusMissing { get; set; }

		public Rule ServerFQDNMatchesSMTPPolicy { get; set; }

		public Rule SmtpAddressLiteral { get; set; }

		public Rule UnwillingToRemoveMailboxDatabase { get; set; }

		public Rule RootDomainModeMixed { get; set; }

		public Rule ServerRemoveProvisioningCheck { get; set; }

		public Rule InconsistentlyConfiguredDomain { get; set; }

		public Rule OffLineABServerDeleted { get; set; }

		public Rule ResourcePropertySchemaException { get; set; }

		public Rule MessagesInQueue { get; set; }

		public Rule AdditionalUMLangPackExists { get; set; }

		public Rule ExchangeAlreadyInstalled { get; set; }

		public Rule InstallWatermark { get; set; }

		public Rule InterruptedUninstallNotContinued { get; set; }

		public Rule W3SVCDisabledOrNotInstalled { get; set; }

		public Rule ShouldReRunSetupForW3SVC { get; set; }

		public Rule SMTPSvcInstalled { get; set; }

		public Rule ClusSvcInstalledRoleBlock { get; set; }

		public Rule LonghornIIS6MetabaseNotInstalled { get; set; }

		public Rule LonghornIIS6MgmtConsoleNotInstalled { get; set; }

		public Rule LonghornIIS7HttpCompressionDynamicNotInstalled { get; set; }

		public Rule LonghornIIS7HttpCompressionStaticNotInstalled { get; set; }

		public Rule LonghornWASProcessModelInstalled { get; set; }

		public Rule LonghornIIS7BasicAuthNotInstalled { get; set; }

		public Rule LonghornIIS7WindowsAuthNotInstalled { get; set; }

		public Rule LonghornIIS7DigestAuthNotInstalled { get; set; }

		public Rule LonghornIIS7NetExt { get; set; }

		public Rule LonghornIIS6WMICompatibility { get; set; }

		public Rule LonghornASPNET { get; set; }

		public Rule LonghornISAPIFilter { get; set; }

		public Rule LonghornClientCertificateMappingAuthentication { get; set; }

		public Rule LonghornDirectoryBrowse { get; set; }

		public Rule LonghornHttpErrors { get; set; }

		public Rule LonghornHttpLogging { get; set; }

		public Rule LonghornHttpRedirect { get; set; }

		public Rule LonghornHttpTracing { get; set; }

		public Rule LonghornRequestMonitor { get; set; }

		public Rule LonghornStaticContent { get; set; }

		public Rule ManagementServiceInstalled { get; set; }

		public Rule HttpActivationInstalled { get; set; }

		public Rule WcfHttpActivation45Installed { get; set; }

		public Rule RsatAddsToolsInstalled { get; set; }

		public Rule RsatClusteringInstalled { get; set; }

		public Rule RsatClusteringMgmtInstalled { get; set; }

		public Rule RsatClusteringPowerShellInstalled { get; set; }

		public Rule RsatClusteringCmdInterfaceInstalled { get; set; }

		public Rule UcmaRedistMsi { get; set; }

		public Rule SpeechRedistMsi { get; set; }

		public Rule Win7WindowsIdentityFoundationUpdateNotInstalled { get; set; }

		public Rule Win8WindowsIdentityFoundationUpdateNotInstalled { get; set; }

		public Rule MailboxRoleNotInstalled { get; set; }

		public Rule MailboxMinVersionCheck { get; set; }

		public Rule MailboxUpgradeMinVersionBlock { get; set; }

		public Rule UnifiedMessagingMinVersionCheck { get; set; }

		public Rule UnifiedMessagingUpgradeMinVersionBlock { get; set; }

		public Rule BridgeheadMinVersionCheck { get; set; }

		public Rule BridgeheadUpgradeMinVersionBlock { get; set; }

		public Rule Exchange2013AnyOnExchange2007Server { get; set; }

		public Rule Exchange2010ServerOnExchange2007AdminTools { get; set; }

		public Rule UpdateNeedsReboot { get; set; }

		public Rule CannotAccessAD { get; set; }

		public Rule ConfigDCHostNameMismatch { get; set; }

		public Rule OldADAMInstalled { get; set; }

		public Rule ADAMWin7ServerInstalled { get; set; }

		public Rule UpgradeGateway605Block { get; set; }

		public Rule GatewayMinVersionCheck { get; set; }

		public Rule GatewayUpgradeMinVersionBlock { get; set; }

		public Rule ADAMSvcStopped { get; set; }

		public Rule TargetPathCompressed { get; set; }

		public Rule GatewayUpgrade605Block { get; set; }

		public Rule ADAMDataPathExists { get; set; }

		public Rule EdgeSubscriptionExists { get; set; }

		public Rule ADAMPortAlreadyInUse { get; set; }

		public Rule ADAMSSLPortAlreadyInUse { get; set; }

		public Rule ServerIsLastHubForEdgeSubscription { get; set; }

		public Rule LonghornIIS7ManagementConsoleInstalled { get; set; }

		public Rule WindowsInstallerServiceDisabledOrNotInstalled { get; set; }

		public Rule WinRMServiceDisabledOrNotInstalled { get; set; }

		public Rule RSATWebServerNotInstalled { get; set; }

		public Rule NETFrameworkNotInstalled { get; set; }

		public Rule NETFramework45FeaturesNotInstalled { get; set; }

		public Rule WebNetExt45NotInstalled { get; set; }

		public Rule WebISAPIExtNotInstalled { get; set; }

		public Rule WebASPNET45NotInstalled { get; set; }

		public Rule RPCOverHTTPproxyNotInstalled { get; set; }

		public Rule ServerGuiMgmtInfraNotInstalled { get; set; }

		public Rule E15E14CoexistenceMinVersionRequirement { get; set; }

		public Rule E15E14CoexistenceMinMajorVersionRequirement { get; set; }

		public Rule E15E12CoexistenceMinVersionRequirement { get; set; }

		public Rule E15E14CoexistenceMinVersionRequirementForDC { get; set; }

		public Rule AllServersOfHigherVersionRule { get; set; }

		public Rule WindowsServer2008CoreServerEdition { get; set; }

		public Rule ValidOSVersion { get; set; }

		public Rule ValidOSVersionForAdminTools { get; set; }

		public Rule Exchange2013AnyOnExchange2010Server { get; set; }

		public Rule ServicesAreMarkedForDeletion { get; set; }

		public Rule PowerShellExecutionPolicyCheckSet { get; set; }

		public Rule VC2012RedistDependencyRequirement { get; set; }

		public Rule VC2013RedistDependencyRequirement { get; set; }

		public Setting<MailboxServer> CmdletGetMailboxServerResult { get; set; }

		public Setting<ExchangeServer> CmdletGetExchangeServerResult { get; set; }

		public Setting<string> ShortServerName { get; set; }

		public Setting<bool> DebugVersion { get; set; }

		public Setting<ushort> DomainRole { get; set; }

		public Setting<bool> LocalComputerIsDomainController { get; set; }

		public Setting<bool> ADSplitPermissionMode { get; set; }

		public Setting<bool> EventSystemStarted { get; set; }

		public Setting<bool> MSDTCStarted { get; set; }

		public Setting<bool> MpsSvcStarted { get; set; }

		public Setting<string> NetTcpPortSharingStartMode { get; set; }

		public Setting<string> WindowsVersion { get; set; }

		public Setting<string> WindowsBuild { get; set; }

		public Setting<ResultPropertyCollection> MsExServicesConfigDNOtherWellKnownObjects { get; set; }

		public Setting<string> MsExServicesConfigDNOtherWellKnownObjectsEWPDN { get; set; }

		public Setting<string> MsExServicesConfigDNOtherWellKnownObjectsETSDN { get; set; }

		public Setting<string> EWPDn { get; set; }

		public Setting<string> ETSDn { get; set; }

		public Setting<bool> ETSIsMemberOfEWP { get; set; }

		public Setting<string> DomainControllerCN { get; set; }

		public Setting<int> ExchangeServerRolesOnDomainController { get; set; }

		public Setting<string> LocalServerName { get; set; }

		public Setting<bool> IsGlobalCatalogReady { get; set; }

		public Setting<string> CurrentLogOn { get; set; }

		public Setting<ushort> AddressWidth { get; set; }

		public Setting<bool> AddressWidth32Bit { get; set; }

		public Setting<bool> AddressWidth64Bit { get; set; }

		public Setting<string> NicCaption { get; set; }

		public Setting<Dictionary<string, object>> NicConfiguration { get; set; }

		public Setting<string> IPAddress { get; set; }

		public Setting<string> IPv4Address { get; set; }

		public Setting<bool> IPv6Enabled { get; set; }

		public Setting<string> DnsAddress { get; set; }

		public Setting<string> PrimaryDns { get; set; }

		public Setting<bool> PrimaryDNSPortAvailable { get; set; }

		public Setting<Dictionary<string, object[]>> HostRecord { get; set; }

		public Setting<string> ComputerDomain { get; set; }

		public Setting<string> ComputerDomainDN { get; set; }

		public Setting<int> NtMixedDomainComputerDomainDN { get; set; }

		public Setting<bool> LocalDomainIsPrepped { get; set; }

		public Setting<string> SiteName { get; set; }

		public Setting<string> ReadOnlyDC { get; set; }

		public Setting<string> ServerRef { get; set; }

		public Setting<string> ADServerDN { get; set; }

		public Setting<string> OperatingSystemVersion { get; set; }

		public Setting<string> WindowsPath { get; set; }

		public Setting<Version> FileVersionLdifde { get; set; }

		public Setting<uint> OSProductSuite { get; set; }

		public Setting<string> BridgeheadRoleInCurrentADSite { get; set; }

		public Setting<string> ClientAccessRoleInCurrentADSite { get; set; }

		public Setting<Version> FileVersionWmspdmod { get; set; }

		public Setting<Version> FileVersionMSXML6 { get; set; }

		public Setting<Version> FileVersionWmspdmoe { get; set; }

		public Setting<string> ExchangeSerialNumber { get; set; }

		public Setting<bool> Exchange12 { get; set; }

		public Setting<bool> Exchange200x { get; set; }

		public Setting<int?> ClrReleaseNumber { get; set; }

		public Setting<Version> FileVersionMSCorLib { get; set; }

		public Setting<Version> FileVersionSystemServiceModel { get; set; }

		public Setting<Version> FileVersionSystemWeb { get; set; }

		public Setting<Version> FileVersionSystemWebServices { get; set; }

		public Setting<Version> FileVersionNtoskrnl { get; set; }

		public Setting<Version> FileVersionSecProc { get; set; }

		public Setting<Version> FileVersionRmActivate { get; set; }

		public Setting<Version> FileVersionRpcRT4 { get; set; }

		public Setting<Version> FileVersionRpcHttp { get; set; }

		public Setting<Version> FileVersionRpcProxy { get; set; }

		public Setting<Version> FileVersionLbService { get; set; }

		public Setting<Version> FileVersionTCPIPSYS { get; set; }

		public Setting<Version> FileVersionAdsiis { get; set; }

		public Setting<Version> FileVersionIisext { get; set; }

		public Setting<Version> FileVersionKernel32 { get; set; }

		public Setting<Version> FileVersionUrefs { get; set; }

		public Setting<Version> FileVersionRefs { get; set; }

		public Setting<Version> FileVersionDiscan { get; set; }

		public Setting<Version> FileVersionAtl110 { get; set; }

		public Setting<Version> FileVersionMsvcr120 { get; set; }

		public Setting<bool> LocalAdmin { get; set; }

		public Setting<string> FirstSGFiles { get; set; }

		public Setting<string> SecondSGFiles { get; set; }

		public Setting<float> ExchangeVersionPrefix { get; set; }

		public Setting<string> VoiceMailPath { get; set; }

		public Setting<int> VoiceMessages { get; set; }

		public Setting<long> RemoteRegistryServiceId { get; set; }

		public Setting<long> OneCopyAlertProcessId { get; set; }

		public Setting<Process> OpenProcesses { get; set; }

		public Setting<Process> OpenProcessesOnUpgrade { get; set; }

		public Setting<Process> OpenProcessesOnUninstall { get; set; }

		public Setting<object> SendConnector { get; set; }

		public Setting<object> GroupDN { get; set; }

		public Setting<object> DynamicGroupDN { get; set; }

		public Setting<bool> SchemaAdmin { get; set; }

		public Setting<bool> EnterpriseAdmin { get; set; }

		public Setting<string> ExtendedRightsNtSecurityDescriptor { get; set; }

		public Setting<bool> HasExtendedRightsCreateChildPerms { get; set; }

		public Setting<List<string>> RootDSEProperties { get; set; }

		public Setting<string> ConfigurationNamingContext { get; set; }

		public Setting<string> RootNamingContext { get; set; }

		public Setting<string> SchemaNamingContext { get; set; }

		public Setting<string> ObjectSid { get; set; }

		public Setting<int> NtMixedDomain { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesAdminGroupConfig { get; set; }

		public Setting<ResultPropertyCollection> OrgMicrosoftExchServicesConfig { get; set; }

		public Setting<bool> ExchangeMixedMode { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesAdminGroupsConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsMailboxRoleConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsBridgeheadRoleConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsClientAccessRoleConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsUnifiedMessagingRoleConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsCafeRoleConfig { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchAdminGroupsFrontendTransportRoleConfig { get; set; }

		public Setting<string> MicrosoftExchServicesConfigDistinguishedName { get; set; }

		public Setting<string> MicrosoftExchServicesAdminGroupConfigDistinguishedName { get; set; }

		public Setting<string> MailboxRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> BridgeheadRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> ClientAccessRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> UnifiedMessagingRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> CafeRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> FrontendTransportRoleOfEqualOrHigherVersion { get; set; }

		public Setting<string> OrgDistinguishedName { get; set; }

		public Setting<int?> SchemaVersionRangeUpper { get; set; }

		public Setting<string> ExchangeServersGroupOtherWellKnownObjects { get; set; }

		public Setting<string> ExchangeOrgAdminsGroupOtherWellKnownObjects { get; set; }

		public Setting<ResultPropertyCollection> OrgDN { get; set; }

		public Setting<string> ExOrgAdminAccountName { get; set; }

		public Setting<string> SidExOrgAdmins { get; set; }

		public Setting<string> ServerManagementGroupOtherWellKnownObjects { get; set; }

		public Setting<ResultPropertyCollection> ServerManagementGroupDN { get; set; }

		public Setting<string> SidServerManagementGroup { get; set; }

		public Setting<ResultPropertyCollection> ExchangeServicesConfigExchangeServersGroup { get; set; }

		public Setting<string> ExchangeServersGroupAMAccountName { get; set; }

		public Setting<bool> ExOrgAdmin { get; set; }

		public Setting<bool> ServerManagement { get; set; }

		public Setting<string> ExchangeServersGroupNtSecurityDescriptor { get; set; }

		public Setting<bool> HasExchangeServersUSGWritePerms { get; set; }

		public Setting<bool> HasExchangeServersUSGBasicAccess { get; set; }

		public Setting<string> ExchangeSecurityGroupsOrgUnit { get; set; }

		public Setting<string> AllowedChildClassesEffective { get; set; }

		public Setting<ResultPropertyCollection> LocalComputerDomainDN { get; set; }

		public Setting<string> LocalDomainNtSecurityDescriptor { get; set; }

		public Setting<bool> LocalDomainAdmin { get; set; }

		public Setting<string> ExchangeServers { get; set; }

		public Setting<int> ServerSetupRoles { get; set; }

		public Setting<string> PrereqServerLegacyDN { get; set; }

		public Setting<string> PrereqServerDN { get; set; }

		public Setting<string> NtSecurityDescriptor { get; set; }

		public Setting<bool> ServerAlreadyExists { get; set; }

		public Setting<int> ExchangeCurrentServerRoles { get; set; }

		public Setting<bool> MailboxRoleInstalled { get; set; }

		public Setting<bool> ClientAccessRoleInstalled { get; set; }

		public Setting<bool> CafeRoleInstalled { get; set; }

		public Setting<bool> FrontendTransportRoleInstalled { get; set; }

		public Setting<bool> UnifiedMessagingRoleInstalled { get; set; }

		public Setting<bool> BridgeheadRoleInstalled { get; set; }

		public Setting<bool> GatewayRoleInstalled { get; set; }

		public Setting<bool> ServerIsProvisioned { get; set; }

		public Setting<string> ComputerNameNetBIOS { get; set; }

		public Setting<string> ComputerNameDnsHostname { get; set; }

		public Setting<string> ComputerNameDnsDomain { get; set; }

		public Setting<string> ComputerNameDnsFullyQualified { get; set; }

		public Setting<bool> E12SP1orHigherHubAlreadyExist { get; set; }

		public Setting<bool> E12SP1orHigherUMAlreadyExists { get; set; }

		public Setting<bool> E12SP1orHigherCASAlreadyExists { get; set; }

		public Setting<bool> E12SP1orHigherMBXAlreadyExists { get; set; }

		public Setting<uint> HasServerDelegatedPermsBlocked { get; set; }

		public Setting<string> ConnectorToStar { get; set; }

		public Setting<string> ExchangeConfigurationUnitsConfiguration { get; set; }

		public Setting<string> ExchangeConfigurationUnitsDomain { get; set; }

		public Setting<string> E15ServerInTopology { get; set; }

		public Setting<string> E14ServerInTopology { get; set; }

		public Setting<string> E12ServerInTopology { get; set; }

		public Setting<ResultPropertyCollection> SchemaDN { get; set; }

		public Setting<string> SmoRoleOwner { get; set; }

		public Setting<ResultPropertyCollection> SmoSchemaDN { get; set; }

		public Setting<string> DnsHostName { get; set; }

		public Setting<string> ServerReference { get; set; }

		public Setting<string> SmoSchemaDomain { get; set; }

		public Setting<ResultPropertyCollection> SmoRoleSchemaRef { get; set; }

		public Setting<string> SmoOperatingSystemVersion { get; set; }

		public Setting<string> SmoOperatingSystemServicePack { get; set; }

		public Setting<bool> Win2003FSMOSchemaServer { get; set; }

		public Setting<bool> SmoSchemaServicePack { get; set; }

		public Setting<string> SMOSchemaSiteName { get; set; }

		public Setting<string> OabDN { get; set; }

		public Setting<string> OtherPotentialOABServers { get; set; }

		public Setting<string> OtherPotentialExpansionServers { get; set; }

		public Setting<string> DomainControllerSiteName { get; set; }

		public Setting<string> DomainControllerRef { get; set; }

		public Setting<string> DomainControllerOS { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigBridgeheadRoleInTopology { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigCafeRoleInTopology { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigUMRoleInTopology { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigCASRoleInTopology { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigMBXRoleInTopology { get; set; }

		public Setting<string> BridgeheadRoleInTopology { get; set; }

		public Setting<bool> E12SP1orHigherHubAlreadyExists { get; set; }

		public Setting<bool> E15orHigherHubAlreadyExists { get; set; }

		public Setting<string> CafeRoleInTopology { get; set; }

		public Setting<string> UnifiedMessagingRoleInTopology { get; set; }

		public Setting<string> ClientAccessRoleInTopology { get; set; }

		public Setting<string> MailboxRoleInTopology { get; set; }

		public Setting<string> AdcServer { get; set; }

		public Setting<string> ShortProvisionedName { get; set; }

		public Setting<int> MsDSBehaviorVersion { get; set; }

		public Setting<ResultPropertyCollection> ExchangeRecipientPolicyConfiguration { get; set; }

		public Setting<string> RecipientPolicyName { get; set; }

		public Setting<string> ExchNonAuthoritativeDomains { get; set; }

		public Setting<string> DisabledGatewayProxy { get; set; }

		public Setting<string> EnabledSMTPDomain { get; set; }

		public Setting<string> GatewayProxy { get; set; }

		public Setting<string> MicrosoftExchServicesConfigAdminGroupDistinguishedName { get; set; }

		public Setting<ResultPropertyCollection> MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig { get; set; }

		public Setting<string> MicrosoftExchServicesConfigAdminGroupPublicFoldersDistinguishedName { get; set; }

		public Setting<byte[]> MicrosoftExchServicesConfigAdminGroupPublicFoldersNtSecurityDescriptor { get; set; }

		public Setting<bool> PrepareDomainAdmin { get; set; }

		public Setting<string> PrepareDomainNCName { get; set; }

		public Setting<ResultPropertyCollection> PrepareDomainNCNameConfig { get; set; }

		public Setting<string> SidPrepareDomain { get; set; }

		public Setting<int> NtMixedDomainPrepareDomain { get; set; }

		public Setting<string> DomainNCName { get; set; }

		public Setting<string> MicrosoftExchangeSystemObjectsCN { get; set; }

		public Setting<string> RusName { get; set; }

		public Setting<List<ResultPropertyCollection>> ExchangePrivateMDB { get; set; }

		public Setting<List<string>> PrivateDatabaseName { get; set; }

		public Setting<List<string>> PrivateDatabaseNameDN { get; set; }

		public Setting<List<string>> PrivateDatabaseEdbDrive { get; set; }

		public Setting<List<string>> PrivateDatabaseLogDrive { get; set; }

		public Setting<List<string>> MailboxEDBDriveNotExistList { get; set; }

		public Setting<List<string>> MailboxLogDriveNotExistList { get; set; }

		public Setting<List<string>> RemoveMailboxDatabaseException { get; set; }

		public Setting<bool> IgnoreFileInUse { get; set; }

		public Setting<ResultPropertyCollection> RootDN { get; set; }

		public Setting<string> SidRootDomain { get; set; }

		public Setting<int> NtMixedDomainRoot { get; set; }

		public Setting<string> RemoveServerName { get; set; }

		public Setting<int> ExchCurrentServerRoles { get; set; }

		public Setting<string[]> NonAuthoritativeDomainsArray { get; set; }

		public Setting<string[]> DisabledGatewayProxyArray { get; set; }

		public Setting<string[]> EnabledGatewayProxyArray { get; set; }

		public Setting<ResultPropertyCollection> ExchOab { get; set; }

		public Setting<string> OabName { get; set; }

		public Setting<string> OffLineABServer { get; set; }

		public Setting<string> TargetDir { get; set; }

		public Setting<Version> ExchangeVersion { get; set; }

		public Setting<int> AdamPort { get; set; }

		public Setting<int> AdamSSLPort { get; set; }

		public Setting<bool> CreatePublicDB { get; set; }

		public Setting<bool> CustomerFeedbackEnabled { get; set; }

		public Setting<string> NewProvisionedServerName { get; set; }

		public Setting<string> RemoveProvisionedServerName { get; set; }

		public Setting<string> GlobalCatalog { get; set; }

		public Setting<string> DomainController { get; set; }

		public Setting<string> PrepareDomain { get; set; }

		public Setting<bool> PrepareOrganization { get; set; }

		public Setting<bool> PrepareSchema { get; set; }

		public Setting<bool> PrepareAllDomains { get; set; }

		public Setting<string> AdInitError { get; set; }

		public Setting<string> LanguagePackDir { get; set; }

		public Setting<bool> LanguagesAvailableToInstall { get; set; }

		public Setting<bool> SufficientLanguagePackDiskSpace { get; set; }

		public Setting<bool> LanguagePacksInstalled { get; set; }

		public Setting<string> AlreadyInstalledUMLanguages { get; set; }

		public Setting<bool> LanguagePackVersioning { get; set; }

		public Setting<bool> ActiveDirectorySplitPermissions { get; set; }

		public Setting<string> SetupRoles { get; set; }

		public Setting<bool> E12SP1orHigher { get; set; }

		public Setting<int?> NewestBuild { get; set; }

		public Setting<string> ServicesPath { get; set; }

		public Setting<string> MsiInstallPath { get; set; }

		public Setting<string> Roles { get; set; }

		public Setting<string> ServerRoleUnpacked { get; set; }

		public Setting<string> Watermarks { get; set; }

		public Setting<string> FilteredRoles { get; set; }

		public Setting<string> Actions { get; set; }

		public Setting<string> UcmaRedistVersion { get; set; }

		public Setting<string[]> SpeechRedist { get; set; }

		public Setting<string> WindowsSPLevel { get; set; }

		public Setting<string> WindowsProductName { get; set; }

		public Setting<string> ADAMVersion { get; set; }

		public Setting<string> SMTPSvcStartMode { get; set; }

		public Setting<string> SMTPSvcDisplayName { get; set; }

		public Setting<string[]> IISCommonFiles { get; set; }

		public Setting<int?> IIS6MetabaseStatus { get; set; }

		public Setting<int?> IIS6ManagementConsoleStatus { get; set; }

		public Setting<int?> IIS7CompressionDynamic { get; set; }

		public Setting<int?> IIS7CompressionStatic { get; set; }

		public Setting<int?> IIS7ManagedCodeAssemblies { get; set; }

		public Setting<int?> WASProcessModel { get; set; }

		public Setting<int?> IIS7BasicAuthentication { get; set; }

		public Setting<int?> IIS7WindowAuthentication { get; set; }

		public Setting<int?> IIS7DigestAuthentication { get; set; }

		public Setting<int?> IIS7NetExt { get; set; }

		public Setting<int?> IIS6WMICompatibility { get; set; }

		public Setting<int?> ASPNET { get; set; }

		public Setting<int?> ISAPIFilter { get; set; }

		public Setting<int?> ClientCertificateMappingAuthentication { get; set; }

		public Setting<int?> DirectoryBrowse { get; set; }

		public Setting<int?> HttpErrors { get; set; }

		public Setting<int?> HttpLogging { get; set; }

		public Setting<int?> HttpRedirect { get; set; }

		public Setting<int?> HttpTracing { get; set; }

		public Setting<int?> RequestMonitor { get; set; }

		public Setting<int?> StaticContent { get; set; }

		public Setting<int?> ManagementService { get; set; }

		public Setting<int> W3SVCStartMode { get; set; }

		public Setting<int> ClusSvcStartMode { get; set; }

		public Setting<string[]> Wif35Installed { get; set; }

		public Setting<string> MailboxConfiguredVersion { get; set; }

		public Setting<string> MailboxUnpackedVersion { get; set; }

		public Setting<bool> MailboxPreviousBuild { get; set; }

		public Setting<string> UnifiedMessagingConfiguredVersion { get; set; }

		public Setting<string> UnifiedMessagingUnpackedVersion { get; set; }

		public Setting<bool> UnifiedMessagingPreviousBuild { get; set; }

		public Setting<string> ClientAccessConfiguredVersion { get; set; }

		public Setting<string> BridgeheadConfiguredVersion { get; set; }

		public Setting<string> BridgeheadUnpackedVersion { get; set; }

		public Setting<bool> BridgeheadPreviousBuild { get; set; }

		public Setting<bool> PreviousBuildDetected { get; set; }

		public Setting<string> GatewayConfiguredVersion { get; set; }

		public Setting<string> GatewayUnpackedVersion { get; set; }

		public Setting<string[]> AdminToolsInstallation { get; set; }

		public Setting<string[]> PendingFileRenames { get; set; }

		public Setting<bool> DST2007Enabled { get; set; }

		public Setting<string[]> DynamicDSTKey { get; set; }

		public Setting<string[]> HTTPActivation { get; set; }

		public Setting<bool> IsWcfHttpActivation45Installed { get; set; }

		public Setting<bool> IsRsatAddsToolsInstalled { get; set; }

		public Setting<bool> IsRsatClusteringInstalled { get; set; }

		public Setting<bool> IsRsatClusteringMgmtInstalled { get; set; }

		public Setting<bool> IsRsatClusteringPowerShellInstalled { get; set; }

		public Setting<bool> IsRsatClusteringCmdInterfaceInstalled { get; set; }

		public Setting<string> NNTPSvcStartMode { get; set; }

		public Setting<string> ProgramFilePath { get; set; }

		public Setting<string> FrameworkPath { get; set; }

		public Setting<string> AdamDataPath { get; set; }

		public Setting<string> ConfigDCHostName { get; set; }

		public Setting<object> CmdletGetQueueResult { get; set; }

		public Setting<UMServer> CmdletGetUMServerResult { get; set; }

		public Setting<string> LanguageInUMServer { get; set; }

		public Setting<string> SiteCanonicalName { get; set; }

		public Setting<object> EdgeSubscriptionForSite { get; set; }

		public Setting<string> HubTransportRoleInCurrentADSite { get; set; }

		public Setting<bool> HttpLocationAccessible { get; set; }

		public Setting<bool> IsHybridObjectFoundOnPremises { get; set; }

		public Setting<bool> WasSetupStartedFromGUI { get; set; }

		public Setting<bool> IsExchangeVersionCorrectForTenant { get; set; }

		public Setting<int?> IIS7ManagementConsole { get; set; }

		public Setting<int> WindowsInstallerServiceStartMode { get; set; }

		public Setting<int> WinRMServiceStartMode { get; set; }

		public Setting<int> VersionNumber { get; set; }

		public Setting<bool> Windows2K8R2Version { get; set; }

		public Setting<bool> Windows8Version { get; set; }

		public Setting<bool> Windows8ClientVersion { get; set; }

		public Setting<bool> Windows7ClientVersion { get; set; }

		public Setting<object[]> InstalledWindowsFeatures { get; set; }

		public Setting<bool> IsWinRMIISExtensionInstalled { get; set; }

		public Setting<bool> IsRSATWebServerInstalled { get; set; }

		public Setting<bool> IsNETFrameworkInstalled { get; set; }

		public Setting<bool> IsNETFramework45FeaturesInstalled { get; set; }

		public Setting<bool> IsWebNetExt45Installed { get; set; }

		public Setting<bool> IsWebISAPIExtInstalled { get; set; }

		public Setting<bool> IsWebASPNET45Installed { get; set; }

		public Setting<bool> IsRPCOverHTTPproxyInstalled { get; set; }

		public Setting<bool> IsServerGuiMgmtInfraInstalled { get; set; }

		public Setting<string> E12ServersNotMinVersionRequirement { get; set; }

		public Setting<string> E14ServersNotMinVersionRequirement { get; set; }

		public Setting<string> E14ServersNotMinMajorVersionRequirement { get; set; }

		public Setting<string> AllServersOfHigherVersion { get; set; }

		public Setting<bool> IsCoreServer { get; set; }

		public Setting<uint> OSProductType { get; set; }

		public Setting<uint> SuiteMask { get; set; }

		public Setting<bool> ValidOSSuite { get; set; }

		public Setting<string> ServiceMarkedForDeletion { get; set; }

		public Setting<bool> PowerShellExecutionPolicy { get; set; }

		public Setting<bool> VCRedist2012Installed { get; set; }

		public Setting<bool> VCRedist2013Installed { get; set; }

		protected override void OnAnalysisStart()
		{
			lock (this.logLock)
			{
				SetupLogger.Log(Strings.PrereqAnalysisStarted);
			}
		}

		protected override void OnAnalysisStop()
		{
			TimeSpan duration = base.StopTime - base.StartTime;
			lock (this.logLock)
			{
				SetupLogger.Log(Strings.PrereqAnalysisStopped(duration));
				foreach (Result result in base.Errors)
				{
					string name = result.Source.Name;
					string message = Strings.PrereqAnalysisNullValue;
					MessageFeature messageFeature = result.Source.Features.OfType<MessageFeature>().FirstOrDefault<MessageFeature>();
					if (messageFeature != null)
					{
						message = messageFeature.Text(result);
					}
					SetupLogger.Log(Strings.PrereqAnalysisFailedRule(name, message));
				}
			}
		}

		protected override void OnAnalysisMemberStart(AnalysisMember member)
		{
			string name = member.Name;
			string name2 = member.Parent.Name;
			if (member is Rule)
			{
				RuleTypeFeature ruleTypeFeature = member.Features.OfType<RuleTypeFeature>().FirstOrDefault<RuleTypeFeature>();
				string ruleType = (ruleTypeFeature == null) ? RuleType.None.ToString() : ruleTypeFeature.RuleType.ToString();
				lock (this.logLock)
				{
					SetupLogger.Log(Strings.PrereqAnalysisRuleStarted(name, name2, ruleType));
				}
				return;
			}
			string name3 = member.ValueType.Name;
			lock (this.logLock)
			{
				SetupLogger.Log(Strings.PrereqAnalysisSettingStarted(name, name2, name3));
			}
		}

		protected override void OnAnalysisMemberStop(AnalysisMember member)
		{
			string memberType = (member is Rule) ? "Rule" : "Setting";
			string name = member.Name;
			TimeSpan duration = member.StopTime - member.StartTime;
			lock (this.logLock)
			{
				SetupLogger.Log(Strings.PrereqAnalysisMemberStopped(memberType, name, duration));
			}
		}

		protected override void OnAnalysisMemberEvaluate(AnalysisMember member, Result result)
		{
			string memberType = (member is Rule) ? "Rule" : "Setting";
			string name = member.Name;
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			TimeSpan duration = result.StopTime - result.StartTime;
			bool hasException = result.HasException;
			string value = this.FormatValueOrExceptionText(result);
			string parentValue = result.Parent.HasException ? Strings.PrereqAnalysisParentExceptionValue : this.FormatValueOrExceptionText(result.Parent);
			lock (this.logLock)
			{
				SetupLogger.Log(Strings.PrereqAnalysisMemberEvaluated(memberType, name, hasException, value, parentValue, managedThreadId, duration));
			}
		}

		private string FormatValueOrExceptionText(Result result)
		{
			string text = Strings.PrereqAnalysisNullValue;
			if (!result.HasException)
			{
				return string.Format("\"{0}\"", (result.ValueAsObject == null) ? text : result.ValueAsObject.ToString());
			}
			Exception ex = result.Exception;
			StringBuilder stringBuilder = new StringBuilder();
			while (ex != null)
			{
				AnalysisException ex2 = ex as AnalysisException;
				if (ex2 != null)
				{
					string name = ex2.AnalysisMemberSource.Name;
					if (ex2 is FailureException)
					{
						stringBuilder.Append(Environment.NewLine + Strings.PrereqAnalysisExpectedFailure(name, ex2.Message));
						break;
					}
					stringBuilder.Append(Environment.NewLine + Strings.PrereqAnalysisFailureToAccessResults(name, ex2.Message));
					ex = ex.InnerException;
				}
				else
				{
					stringBuilder.Append(Environment.NewLine + ex.ToString());
					ex = ex.InnerException;
				}
			}
			return stringBuilder.ToString() + Environment.NewLine;
		}

		private void CreateActiveDirectoryPrereqProperties()
		{
			this.RootDSEProperties = Setting<List<string>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<string> value = null;
				if (this.SetupRoles.Results.Count((Result<string> w) => w.Value.Equals(SetupRole.Gateway.ToString(), StringComparison.InvariantCultureIgnoreCase)) == 0)
				{
					value = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/RootDSE", this.GlobalCatalog.Results.Value));
				}
				return new Result<List<string>>(value);
			});
			this.ConfigurationNamingContext = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.RootDSEProperties.Results.ValueOrDefault != null)
				{
					return new Result<string>(this.RootDSEProperties.Results.Value[0]);
				}
				return new Result<string>(string.Empty);
			});
			this.RootNamingContext = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.RootDSEProperties.Results.ValueOrDefault != null)
				{
					return new Result<string>(this.RootDSEProperties.Results.Value[1]);
				}
				return new Result<string>(string.Empty);
			});
			this.SchemaNamingContext = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.RootDSEProperties.Results.ValueOrDefault != null)
				{
					return new Result<string>(this.RootDSEProperties.Results.Value[2]);
				}
				return new Result<string>(string.Empty);
			});
			this.IsGlobalCatalogReady = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				if (this.RootDSEProperties.Results.ValueOrDefault != null)
				{
					bool value = false;
					bool.TryParse(this.RootDSEProperties.Results.Value[3], out value);
					return new Result<bool>(value);
				}
				return new Result<bool>(false);
			});
			this.SchemaDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.SchemaNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.SchemaNamingContext.Results.Value), new string[]
					{
						"fsmoroleowner"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.DomainControllerRef = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.DomainController.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Sites,{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"serverReference"
					}, string.Format("(&(objectClass=server)(|(cn={0})(dNSHostName={1})))", this.DomainController.Results.Value, this.DomainController.Results.Value), SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["serverReference"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.DomainControllerOS = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.DomainControllerRef.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.DomainControllerRef.Results.Value), new string[]
					{
						"operatingSystemVersion"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["operatingSystemVersion"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ComputerDomainDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.ComputerDomain.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Partitions,{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"nCName"
					}, string.Format("(&(dnsRoot={0})(systemFlags=3))", this.ComputerDomain.Results.Value), SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["nCName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.LocalComputerDomainDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ComputerDomainDN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.DomainController.Results.Value, this.ComputerDomainDN.Results.Value), new string[]
					{
						"nTMixedDomain",
						"objectSid"
					}, string.Empty, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.LocalDomainNtSecurityDescriptor = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.LocalComputerDomainDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.LocalComputerDomainDN.Results.Value["objectSid"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertToStringSid(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SmoRoleOwner = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.SchemaDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.SchemaDN.Results.Value["fsmoroleowner"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(AnalysisHelpers.Replace(obj.ToString(), "CN=NTDS Settings,CN=(.*?),.*", "$1"));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SmoSchemaDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.SmoRoleOwner.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Sites, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"dNSHostName",
						"serverReference"
					}, string.Format("(&(objectClass=server)(cn={0}))", this.SmoRoleOwner.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.DnsHostName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.SmoSchemaDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.SmoSchemaDN.Results.Value["dNSHostName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ServerReference = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.SmoSchemaDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.SmoSchemaDN.Results.Value["serverReference"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SmoSchemaDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ServerReference.Results.ValueOrDefault))
				{
					return new Result<string>(this.ServerReference.Results.Value.Substring(this.ServerReference.Results.Value.IndexOf("DC=")));
				}
				return new Result<string>(string.Empty);
			});
			this.SmoRoleSchemaRef = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ServerReference.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ServerReference.Results.Value), new string[]
					{
						"operatingSystemVersion",
						"operatingSystemServicePack"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.SmoOperatingSystemVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.SmoRoleSchemaRef.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.SmoRoleSchemaRef.Results.Value["operatingSystemVersion"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SmoOperatingSystemServicePack = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.SmoRoleSchemaRef.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.SmoRoleSchemaRef.Results.Value["operatingSystemServicePack"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.Win2003FSMOSchemaServer = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(this.SmoOperatingSystemVersion.Results.Value.StartsWith("5.2")));
			this.SmoSchemaServicePack = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(this.SmoOperatingSystemServicePack.Results.Value.StartsWith("Service Pack 2")));
			this.MicrosoftExchServicesConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Microsoft Exchange,cn=Services, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"distinguishedName",
						"otherWellKnownObjects"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigDistinguishedName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfig.Results.Value["distinguishedName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchServicesAdminGroupsConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					string[] listOfPropertiesToCollect = new string[]
					{
						"distinguishedName",
						"cn",
						"msExchCurrentServerRoles",
						"legacyExchangeDN",
						"nTSecurityDescriptor"
					};
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), listOfPropertiesToCollect, string.Format("(&(objectClass=msExchExchangeServer)(cn={0}))", this.ShortServerName.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.ServerAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupsConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupsConfig.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<bool>(!string.IsNullOrEmpty(obj.ToString()));
						}
					}
				}
				return new Result<bool>(false);
			});
			this.PrereqServerDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupsConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupsConfig.Results.Value["distinguishedName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.PrereqServerLegacyDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupsConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupsConfig.Results.Value["legacyExchangeDN"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeCurrentServerRoles = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupsConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupsConfig.Results.Value["msExchCurrentServerRoles"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<int>((int)obj);
						}
					}
				}
				return new Result<int>(0);
			});
			this.NtSecurityDescriptor = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupsConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupsConfig.Results.Value["nTSecurityDescriptor"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertBinaryToString(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MailboxRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 2) == 2));
			this.ClientAccessRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 4) == 4));
			this.CafeRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 1) == 1));
			this.UnifiedMessagingRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 16) == 16));
			this.BridgeheadRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 32) == 32));
			this.GatewayRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 64) == 64));
			this.FrontendTransportRoleInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 16384) == 16384));
			this.ServerIsProvisioned = Setting<bool>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((this.ExchangeCurrentServerRoles.Results.Value & 4096) == 4096));
			this.ExtendedRightsNtSecurityDescriptor = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Extended-Rights, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"nTSecurityDescriptor"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["nTSecurityDescriptor"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object val = enumerator2.Current;
									return new Result<string>(AnalysisHelpers.ConvertBinaryToString(val));
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeOrgAdminsGroupOtherWellKnownObjects = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfig.Results.Value["otherWellKnownObjects"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string value = AnalysisHelpers.Replace(obj.ToString(), "(^B:32:C262A929D691B74A9E068728F8F842EA:(?'dn'.*))?.*$", "${dn}");
						if (!string.IsNullOrEmpty(value))
						{
							return new Result<string>(value);
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.OrgDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ExchangeOrgAdminsGroupOtherWellKnownObjects.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ExchangeOrgAdminsGroupOtherWellKnownObjects.Results.Value), new string[]
					{
						"sAMAccountName",
						"objectSid"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.ExOrgAdminAccountName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.OrgDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.OrgDN.Results.Value["sAMAccountName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SidExOrgAdmins = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.OrgDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.OrgDN.Results.Value["objectSid"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertToStringSid(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ServerManagementGroupOtherWellKnownObjects = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfig.Results.Value["otherWellKnownObjects"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string value = AnalysisHelpers.Replace(obj.ToString(), "(^B:32:4DB8E7754EB6C1439565612E69A80A4F:(?'dn'.*))?.*$", "${dn}");
						if (!string.IsNullOrEmpty(value))
						{
							return new Result<string>(value);
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ServerManagementGroupDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ServerManagementGroupOtherWellKnownObjects.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ServerManagementGroupOtherWellKnownObjects.Results.Value), new string[]
					{
						"sAMAccountName",
						"objectSid"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.SidServerManagementGroup = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.ServerManagementGroupDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ServerManagementGroupDN.Results.Value["objectSid"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertToStringSid(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeServersGroupOtherWellKnownObjects = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfig.Results.Value["otherWellKnownObjects"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string value = AnalysisHelpers.Replace(obj.ToString(), "(^B:32:A7D2016C83F003458132789EEB127B84:(?'dn'.*))?.*$", "${dn}");
						if (!string.IsNullOrEmpty(value))
						{
							return new Result<string>(value);
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeServicesConfigExchangeServersGroup = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ExchangeServersGroupOtherWellKnownObjects.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ExchangeServersGroupOtherWellKnownObjects.Results.Value), new string[]
					{
						"sAMAccountName",
						"nTSecurityDescriptor"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.ExchangeServersGroupAMAccountName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeServicesConfigExchangeServersGroup.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeServicesConfigExchangeServersGroup.Results.Value["sAMAccountName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeServersGroupNtSecurityDescriptor = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeServicesConfigExchangeServersGroup.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeServicesConfigExchangeServersGroup.Results.Value["nTSecurityDescriptor"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertBinaryToString(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.SchemaVersionRangeUpper = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.SchemaNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=ms-Exch-Schema-Version-Pt, {1}", this.GlobalCatalog.Results.Value, this.SchemaNamingContext.Results.Value), new string[]
					{
						"rangeUpper"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["rangeUpper"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<int?>(new int?((int)obj2));
								}
							}
						}
					}
				}
				return new Result<int?>(null);
			});
			this.AdcServer = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Sites,{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"cn"
					}, "(objectClass=msExchActiveDirectoryConnector)", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							list.Add(obj2.ToString());
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.OrgDistinguishedName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.OrgMicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.OrgMicrosoftExchServicesConfig.Results.Value["distinguishedName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeMixedMode = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.OrgMicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.OrgMicrosoftExchServicesConfig.Results.Value["msExchMixedMode"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<bool>(Convert.ToBoolean(obj.ToString()));
						}
					}
				}
				return new Result<bool>(false);
			});
			this.ConnectorToStar = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchRoutingSMTPConnector)(routingList=SMTP:\\2a;*))", SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeVersionPrefix = Setting<float>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.OrgMicrosoftExchServicesConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.OrgMicrosoftExchServicesConfig.Results.Value["msExchVersion"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string[] array = obj.ToString().Split(new string[]
						{
							":"
						}, StringSplitOptions.RemoveEmptyEntries);
						if (array != null && array.Length > 1)
						{
							return new Result<float>(float.Parse(array[0]));
						}
					}
				}
				return new Result<float>(0f);
			});
			this.PrepareDomainNCName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.PrepareDomain.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Partitions,{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"nCName"
					}, string.Format("(&(systemFlags=3)(|(cn={0})(dnsRoot={1})))", this.PrepareDomain.Results.Value, this.PrepareDomain.Results.Value), SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["nCName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.PrepareDomainNCNameConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.PrepareDomainNCName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.PrepareDomainNCName.Results.Value), new string[]
					{
						"objectSid",
						"nTMixedDomain"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.SidPrepareDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.PrepareDomainNCNameConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.PrepareDomainNCNameConfig.Results.Value["objectSid"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertToStringSid(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.NtMixedDomainPrepareDomain = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.PrepareDomainNCNameConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.PrepareDomainNCNameConfig.Results.Value["nTMixedDomain"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<int>((int)obj);
						}
					}
				}
				return new Result<int>(0);
			});
			this.ExchangeServers = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "objectClass=msExchExchangeServer", SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							foreach (object obj2 in resultPropertyValueCollection)
							{
								list.Add(obj2.ToString());
							}
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.RootDN = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.RootNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.RootNamingContext.Results.Value), new string[]
					{
						"objectSid",
						"nTMixedDomain"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.SidRootDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.RootDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.RootDN.Results.Value["objectSid"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object val = enumerator.Current;
							return new Result<string>(AnalysisHelpers.ConvertToStringSid(val));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.NtMixedDomainRoot = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.RootDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.RootDN.Results.Value["nTMixedDomain"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<int>((int)obj);
						}
					}
				}
				return new Result<int>(0);
			});
			this.ServerSetupRoles = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<int>(this.globalParameters.SetupRoles.Count((string sr) => sr != "Admin Tools")));
			this.MicrosoftExchServicesAdminGroupConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName"
					}, "(objectClass=msExchAdminGroup)", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesAdminGroupConfigDistinguishedName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesAdminGroupConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesAdminGroupConfig.Results.Value["distinguishedName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchAdminGroupsMailboxRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=2)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MailboxRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsMailboxRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsMailboxRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MicrosoftExchAdminGroupsBridgeheadRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.BridgeheadRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsBridgeheadRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsBridgeheadRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MicrosoftExchAdminGroupsClientAccessRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=4)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.ClientAccessRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsClientAccessRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsClientAccessRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MicrosoftExchAdminGroupsUnifiedMessagingRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=16)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.UnifiedMessagingRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsUnifiedMessagingRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsUnifiedMessagingRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MicrosoftExchAdminGroupsCafeRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=1)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.CafeRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsCafeRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsCafeRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MicrosoftExchAdminGroupsFrontendTransportRoleConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=16384)(versionNumber>={0}))", this.VersionNumber.Results.Value.ToString()), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.FrontendTransportRoleOfEqualOrHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.MicrosoftExchAdminGroupsFrontendTransportRoleConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchAdminGroupsFrontendTransportRoleConfig.Results.Value["cn"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.OabDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Offline Address Lists,cn=Address Lists Container, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName"
					}, string.Format("(&(objectClass=msExchOAB)(offlineABServer={0}))", this.PrereqServerDN.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.OtherPotentialOABServers = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchExchangeServer)(|(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=2)(!(msExchCurrentServerRoles=*))))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							foreach (object obj2 in resultPropertyValueCollection)
							{
								list.Add(obj2.ToString());
							}
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.OtherPotentialExpansionServers = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesAdminGroupConfigDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchExchangeServer)(|(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32)(!(msExchCurrentServerRoles=*))))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							foreach (object obj2 in resultPropertyValueCollection)
							{
								list.Add(obj2.ToString());
							}
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.E12ServerInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchExchangeServer)(serialNumber=Version 8.*))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.E14ServerInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchExchangeServer)(serialNumber=Version 14.*))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.E15ServerInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=msExchExchangeServer)(serialNumber=Version 15.*))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchServicesConfigBridgeheadRoleInTopology = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"serialNumber"
					}, "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigCafeRoleInTopology = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"serialNumber"
					}, "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=8))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigUMRoleInTopology = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"serialNumber"
					}, "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=16))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigCASRoleInTopology = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"serialNumber"
					}, "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=4))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigMBXRoleInTopology = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"serialNumber"
					}, "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=2))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.BridgeheadRoleInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.CafeRoleInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigCafeRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigCafeRoleInTopology.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.UnifiedMessagingRoleInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ClientAccessRoleInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MailboxRoleInTopology = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.E12SP1orHigherHubAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.Value["serialNumber"];
					foreach (object obj in resultPropertyValueCollection)
					{
						Version v;
						Version.TryParse(AnalysisHelpers.Replace(obj.ToString(), "^Version (\\d+\\.\\d+).*$", "$1"), out v);
						if (v != null && v >= new Version(8, 1))
						{
							return new Result<bool>(true);
						}
					}
				}
				return new Result<bool>(false);
			});
			this.E15orHigherHubAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigBridgeheadRoleInTopology.Results.Value["serialNumber"];
					foreach (object obj in resultPropertyValueCollection)
					{
						Version v;
						Version.TryParse(AnalysisHelpers.Replace(obj.ToString(), "^Version (\\d+\\.\\d+).*$", "$1"), out v);
						if (v != null && v >= new Version(15, 0))
						{
							return new Result<bool>(true);
						}
					}
				}
				return new Result<bool>(false);
			});
			this.E12SP1orHigherUMAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["serialNumber"];
					foreach (object obj in resultPropertyValueCollection)
					{
						Version v;
						Version.TryParse(AnalysisHelpers.Replace(obj.ToString(), "^Version (\\d+\\.\\d+).*$", "$1"), out v);
						if (v != null && v >= new Version(8, 1))
						{
							return new Result<bool>(true);
						}
					}
				}
				return new Result<bool>(false);
			});
			this.E12SP1orHigherCASAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["serialNumber"];
					foreach (object obj in resultPropertyValueCollection)
					{
						Version v;
						Version.TryParse(AnalysisHelpers.Replace(obj.ToString(), "^Version (\\d+\\.\\d+).*$", "$1"), out v);
						if (v != null && v >= new Version(8, 1))
						{
							return new Result<bool>(true);
						}
					}
				}
				return new Result<bool>(false);
			});
			this.E12SP1orHigherMBXAlreadyExists = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigUMRoleInTopology.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigUMRoleInTopology.Results.Value["serialNumber"];
					foreach (object obj in resultPropertyValueCollection)
					{
						Version v;
						Version.TryParse(AnalysisHelpers.Replace(obj.ToString(), "^Version (\\d+\\.\\d+).*$", "$1"), out v);
						if (v != null && v >= new Version(8, 1))
						{
							return new Result<bool>(true);
						}
					}
				}
				return new Result<bool>(false);
			});
			this.ExchangeConfigurationUnitsConfiguration = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/CN=ConfigurationUnits,{1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeConfigurationUnitsDomain = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.RootNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/CN=ConfigurationUnits,{1}", this.GlobalCatalog.Results.Value, this.RootNamingContext.Results.Value), new string[]
					{
						"distinguishedName"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ShortProvisionedName = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.All).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.RootNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.NewProvisionedServerName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("GC://{0}/{1}", this.GlobalCatalog.Results.Value, this.RootNamingContext.Results.Value), new string[0], string.Format("(&(objectClass=computer)(name={0}))", this.NewProvisionedServerName.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							foreach (object obj2 in searchResult.Properties)
							{
								ResultPropertyValueCollection resultPropertyValueCollection = (ResultPropertyValueCollection)obj2;
								foreach (object obj3 in resultPropertyValueCollection)
								{
									list.Add(obj3.ToString());
								}
							}
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.MsDSBehaviorVersion = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Partitions,{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"msDS-Behavior-Version"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["msDS-Behavior-Version"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<int>((int)obj2);
								}
							}
						}
					}
				}
				return new Result<int>(0);
			});
			this.MicrosoftExchServicesConfigAdminGroupDistinguishedName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName"
					}, "(&(objectCategory=msExchAdminGroup)(cn=*))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigAdminGroupDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigAdminGroupDistinguishedName.Results.Value), new string[]
					{
						"distinguishedName",
						"ntSecurityDescriptor"
					}, "(&(objectCategory=msExchAdminGroup)(cn=*))", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MicrosoftExchServicesConfigAdminGroupPublicFoldersDistinguishedName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig.Results.Value["distinguishedName"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchServicesConfigAdminGroupPublicFoldersNtSecurityDescriptor = Setting<byte[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MicrosoftExchServicesConfigAdminGroupPublicFoldersConfig.Results.Value["ntSecurityDescriptor"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<byte[]>((byte[])obj);
						}
					}
				}
				return new Result<byte[]>(new byte[0]);
			});
			this.ExchOab = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Offline Address Lists,cn=Address Lists Container, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"offLineABServer"
					}, "objectClass=msExchOAB", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.OabName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchOab.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchOab.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.OffLineABServer = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (this.ExchOab.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchOab.Results.Value["offLineABServer"];
					foreach (object obj in resultPropertyValueCollection)
					{
						list.Add(obj.ToString());
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.ExchangeRecipientPolicyConfiguration = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Recipient Policies, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"msExchNonAuthoritativeDomains",
						"disabledGatewayProxy",
						"gatewayProxy"
					}, "objectClass=msExchRecipientPolicy", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.RecipientPolicyName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeRecipientPolicyConfiguration.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeRecipientPolicyConfiguration.Results.Value["cn"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(obj.ToString());
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchNonAuthoritativeDomains = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeRecipientPolicyConfiguration.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeRecipientPolicyConfiguration.Results.Value["msExchNonAuthoritativeDomains"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(AnalysisHelpers.Replace(obj.ToString().ToLower(), "^smtp:.*\\@(?'domain'.*))?.*$", "${domain}"));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.DisabledGatewayProxy = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeRecipientPolicyConfiguration.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeRecipientPolicyConfiguration.Results.Value["disabledGatewayProxy"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(AnalysisHelpers.Replace(obj.ToString().ToLower(), "^smtp:.*\\@(?'domain'.*))?.*$", "${domain}"));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.EnabledSMTPDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeRecipientPolicyConfiguration.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeRecipientPolicyConfiguration.Results.Value["gatewayProxy"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(AnalysisHelpers.Replace(obj.ToString().ToLower(), "^smtp:.*\\@(?'domain'.*))?.*$", "${domain}"));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.GatewayProxy = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (this.ExchangeRecipientPolicyConfiguration.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.ExchangeRecipientPolicyConfiguration.Results.Value["gatewayProxy"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<string>(AnalysisHelpers.Replace(obj.ToString(), "^((?i:smtp)\\:.*\\@(?'smtpaddress'.*))?.*$", "${smtpaddress}"));
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.NonAuthoritativeDomainsArray = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ExchNonAuthoritativeDomains.Results.ValueOrDefault))
				{
					return new Result<string[]>(this.ExchNonAuthoritativeDomains.Results.Value.Split(new char[]
					{
						' '
					}));
				}
				return new Result<string[]>(new string[0]);
			});
			this.DisabledGatewayProxyArray = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.DisabledGatewayProxy.Results.ValueOrDefault))
				{
					new Result<string[]>(this.DisabledGatewayProxy.Results.Value.Split(new char[]
					{
						' '
					}));
				}
				return new Result<string[]>(new string[0]);
			});
			this.EnabledGatewayProxyArray = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.Install).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.EnabledSMTPDomain.Results.ValueOrDefault))
				{
					new Result<string[]>(this.EnabledSMTPDomain.Results.Value.Split(new char[]
					{
						' '
					}));
				}
				return new Result<string[]>(new string[0]);
			});
			this.DomainNCName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Partitions, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"nCName"
					}, "(systemFlags=3)", SearchScope.OneLevel);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["nCName"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MicrosoftExchangeSystemObjectsCN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.DomainNCName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Microsoft Exchange System Objects, {1}", this.GlobalCatalog.Results.Value, this.DomainNCName.Results.Value), new string[]
					{
						"cn"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.RusName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Global).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.MicrosoftExchangeSystemObjectsCN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Microsoft Exchange,cn=Services, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchAddressListService)(msExchDomainLink={0}))", this.MicrosoftExchangeSystemObjectsCN.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<string>(obj2.ToString());
								}
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangePrivateMDB = Setting<List<ResultPropertyCollection>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<ResultPropertyCollection> list = new List<ResultPropertyCollection>();
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn",
						"distinguishedName",
						"msExchESEParamLogFilePath",
						"msExchEDBFile"
					}, string.Format("(&(objectClass=msExchPrivateMDB)(msExchOwningServer={0}))", this.PrereqServerDN.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							list.Add(searchResult.Properties);
						}
					}
				}
				return new Result<List<ResultPropertyCollection>>(list);
			});
			this.PrivateDatabaseName = Setting<List<string>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (ResultPropertyCollection resultPropertyCollection in this.ExchangePrivateMDB.Results.Value)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = resultPropertyCollection["cn"];
					if (!AnalysisHelpers.IsNullOrEmpty(resultPropertyValueCollection))
					{
						list.Add(resultPropertyValueCollection[0].ToString());
					}
				}
				return new Result<List<string>>(list);
			});
			this.PrivateDatabaseNameDN = Setting<List<string>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (ResultPropertyCollection resultPropertyCollection in this.ExchangePrivateMDB.Results.Value)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = resultPropertyCollection["distinguishedName"];
					if (!AnalysisHelpers.IsNullOrEmpty(resultPropertyValueCollection))
					{
						list.Add(resultPropertyValueCollection[0].ToString());
					}
				}
				return new Result<List<string>>(list);
			});
			this.PrivateDatabaseEdbDrive = Setting<List<string>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (ResultPropertyCollection resultPropertyCollection in this.ExchangePrivateMDB.Results.Value)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = resultPropertyCollection["msExchEDBFile"];
					if (!AnalysisHelpers.IsNullOrEmpty(resultPropertyValueCollection))
					{
						list.Add(resultPropertyValueCollection[0].ToString());
					}
				}
				return new Result<List<string>>(list);
			});
			this.PrivateDatabaseLogDrive = Setting<List<string>>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (ResultPropertyCollection resultPropertyCollection in this.ExchangePrivateMDB.Results.Value)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = resultPropertyCollection["msExchESEParamLogFilePath"];
					if (!AnalysisHelpers.IsNullOrEmpty(resultPropertyValueCollection))
					{
						list.Add(resultPropertyValueCollection[0].ToString());
					}
				}
				return new Result<List<string>>(list);
			});
			this.ExchCurrentServerRoles = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MicrosoftExchServicesConfigDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.MicrosoftExchServicesConfigDistinguishedName.Results.Value), new string[]
					{
						"msExchCurrentServerRoles"
					}, string.Format("(&(objectClass=msExchExchangeServer)(cn={0}))", this.RemoveServerName.Results.Value), SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["msExchCurrentServerRoles"];
							using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
							{
								if (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									return new Result<int>((int)obj2);
								}
							}
						}
					}
				}
				return new Result<int>(0);
			});
			this.AllServersOfHigherVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = this.OrgDistinguishedName.Results.ValueOrDefault;
				if (string.IsNullOrEmpty(text))
				{
					List<string> list = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/RootDSE", this.GlobalCatalog.Results.Value));
					string text2 = list[0];
					if (!string.IsNullOrEmpty(text2))
					{
						SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Microsoft Exchange,cn=Services, {1}", this.GlobalCatalog.Results.Value, text2), new string[]
						{
							"distinguishedName"
						}, null, SearchScope.Base);
						if (searchResultCollection != null)
						{
							string arg = searchResultCollection[0].Properties["distinguishedName"][0].ToString();
							SearchResultCollection searchResultCollection2 = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, arg), new string[]
							{
								"distinguishedName"
							}, "objectClass=msExchOrganizationContainer", SearchScope.OneLevel);
							if (searchResultCollection2 != null)
							{
								text = searchResultCollection2[0].Properties["distinguishedName"][0].ToString();
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					int num = new ServerVersion(ValidationConstant.AllServersOfHigherVersionMinimum.Major, ValidationConstant.AllServersOfHigherVersionMinimum.Minor, 0, 0).ToInt();
					SearchResultCollection searchResultCollection3 = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, text), new string[]
					{
						"cn",
						"versionnumber"
					}, "(&(objectClass=msExchExchangeServer)(!msExchCurrentServerRoles=0))", SearchScope.OneLevel);
					foreach (object obj in searchResultCollection3)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["versionnumber"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							int num2 = int.Parse(obj2.ToString());
							if (num2 >= num)
							{
								ResultPropertyValueCollection resultPropertyValueCollection2 = searchResult.Properties["cn"];
								using (IEnumerator enumerator3 = resultPropertyValueCollection2.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										object obj3 = enumerator3.Current;
										if (stringBuilder.Length > 0)
										{
											stringBuilder.Append(",");
										}
										stringBuilder.Append(obj3.ToString());
									}
									continue;
								}
							}
							return new Result<string>("");
						}
					}
				}
				return new Result<string>(stringBuilder.ToString());
			});
			this.AllServersOfHigherVersionRule = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.AllServersOfHigherVersionFailure(this.AllServersOfHigherVersion.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (!this.globalParameters.IsDatacenter && !string.IsNullOrEmpty(this.AllServersOfHigherVersion.Results.ValueOrDefault))
				{
					value = true;
				}
				return new RuleResult(value);
			});
			this.CannotUninstallDelegatedServer = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.Uninstall).Error<RuleBuilder<object>>().Message((Result x) => Strings.CannotUninstallDelegatedServer).Condition((Result<object> x) => new RuleResult(this.ServerSetupRoles.Results.Value == this.Roles.Results.Count<Result<string>>() && this.ExchangeServers.Results.Value.Count<char>() == 1 && !this.HasExchangeServersUSGWritePerms.Results.Value && this.SetupRoles.Results.Value.Count<char>() == 1 && this.SetupRoles.Results.Value[0].ToString() != "AdminTools"));
			this.DomainPrepWithoutADUpdate = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADUpdateForDomainPrep("Exchange 2010")).Condition((Result<object> x) => new RuleResult((this.SchemaVersionRangeUpper.Results.ValueOrDefault == null || this.SchemaVersionRangeUpper.Results.Value < 14622 || string.IsNullOrEmpty(this.ExchangeServersGroupAMAccountName.Results.ValueOrDefault)) && !this.PrepareSchema.Results.Value && !this.PrepareOrganization.Results.Value && (!string.IsNullOrEmpty(this.PrepareDomain.Results.ValueOrDefault) || this.PrepareAllDomains.Results.Value)));
			this.AdUpdateRequired = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.SchemaNotPreparedExtendedRights).Condition((Result<object> x) => new RuleResult(this.PrepareOrganization.Results.Value && !this.HasExtendedRightsCreateChildPerms.Results.Value && !this.GlobalUpdateRequired.Results.Value));
			this.SchemaUpdateRequired = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.SchemaNotPrepared).Condition((Result<object> x) => new RuleResult(this.PrepareSchema.Results.Value && (!this.SchemaAdmin.Results.Value || !this.EnterpriseAdmin.Results.Value)));
			this.GlobalUpdateRequired = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerNotPrepared).Condition((Result<object> x) => new RuleResult((this.PrepareOrganization.Results.Value || this.PrepareAllDomains.Results.Value) && !this.EnterpriseAdmin.Results.Value));
			this.SchemaFSMONotWin2003SPn = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.OSMinVersionForFSMONotMet).Condition((Result<object> x) => new RuleResult(this.Win2003FSMOSchemaServer.Results.Value && !this.SmoSchemaServicePack.Results.Value));
			this.NotInSchemaMasterDomain = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerNotInSchemaMasterDomain(this.SmoSchemaDomain.Results.Value)).Condition((Result<object> x) => new RuleResult((this.PrepareSchema.Results.Value || this.PrepareOrganization.Results.Value) && !string.Equals(this.ComputerDomainDN.Results.Value, this.SmoSchemaDomain.Results.Value, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(this.ComputerDomainDN.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.SmoSchemaDomain.Results.ValueOrDefault)));
			this.NotInSchemaMasterSite = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerNotInSchemaMasterSite(this.SMOSchemaSiteName.Results.Value)).Condition((Result<object> x) => new RuleResult((this.PrepareSchema.Results.Value || this.PrepareOrganization.Results.Value) && !string.Equals(this.SiteName.Results.Value, this.SMOSchemaSiteName.Results.Value, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(this.SiteName.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.SMOSchemaSiteName.Results.ValueOrDefault)));
			this.LocalDomainPrep = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.LocalDomainNotPrepared).Condition((Result<object> x) => new RuleResult(this.PrepareDomain.Results.Value == "F63C3A12-7852-4654-B208-125C32EB409A" && (!this.LocalDomainAdmin.Results.Value || !this.HasExchangeServersUSGBasicAccess.Results.Value) && !this.EnterpriseAdmin.Results.Value));
			this.NoE12ServerWarning = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.NoE12ServerWarning).Condition((Result<object> x) => new RuleResult(this.PrepareOrganization.Results.Value && this.E12ServerInTopology.Results.Value.Count<char>() == 0 && this.E15ServerInTopology.Results.Value.Count<char>() == 0));
			this.NoE14ServerWarning = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.NoE14ServerWarning).Condition((Result<object> x) => new RuleResult(this.PrepareOrganization.Results.Value && this.E14ServerInTopology.Results.Value.Count<char>() == 0 && this.E15ServerInTopology.Results.Value.Count<char>() == 0));
			this.DelegatedMailboxFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedMailboxFirstInstall).Condition((Result<object> x) => new RuleResult(this.MailboxRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.DelegatedBridgeheadFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedBridgeheadFirstInstall).Condition((Result<object> x) => new RuleResult(this.BridgeheadRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.DelegatedClientAccessFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedClientAccessFirstInstall).Condition((Result<object> x) => new RuleResult(this.ClientAccessRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.DelegatedUnifiedMessagingFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedUnifiedMessagingFirstInstall).Condition((Result<object> x) => new RuleResult(this.UnifiedMessagingRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.DelegatedCafeFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedCafeFirstInstall).Condition((Result<object> x) => new RuleResult(this.CafeRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.DelegatedFrontendTransportFirstInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedCafeFirstInstall).Condition((Result<object> x) => new RuleResult(this.FrontendTransportRoleOfEqualOrHigherVersion.Results.Count<Result<string>>() == 0 && !this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value));
			this.CannotUninstallClusterNode = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Uninstall).Error<RuleBuilder<object>>().Message((Result x) => Strings.CannotUninstallClusterNode).Condition((Result<object> x) => new RuleResult(this.ClusSvcStartMode.Results.ValueOrDefault == 2));
			this.CannotUninstallOABServer = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Uninstall).Error<RuleBuilder<object>>().Message((Result x) => Strings.CannotUninstallOABServer).Condition((Result<object> x) => new RuleResult(this.OabDN.Results.Value.Count<char>() > 0 && this.OtherPotentialOABServers.Results.Count<Result<string>>() > 1));
			this.CannotAccessAD = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.CannotAccessAD).Condition((Result<object> x) => new RuleResult(string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault)));
			this.DelegatedBridgeheadFirstSP1upgrade = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedBridgeheadFirstSP1upgrade).Condition((Result<object> x) => new RuleResult(this.E12SP1orHigher.Results.Value && !this.ExOrgAdmin.Results.Value && this.E12SP1orHigherHubAlreadyExist.Results.Value));
			this.DelegatedUnifiedMessagingFirstSP1upgrade = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedUnifiedMessagingFirstSP1upgrade).Condition((Result<object> x) => new RuleResult(this.E12SP1orHigher.Results.Value && !this.ExOrgAdmin.Results.Value && this.E12SP1orHigherUMAlreadyExists.Results.Value));
			this.DelegatedClientAccessFirstSP1upgrade = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedClientAccessFirstSP1upgrade).Condition((Result<object> x) => new RuleResult(this.E12SP1orHigher.Results.Value && !this.ExOrgAdmin.Results.Value && this.E12SP1orHigherCASAlreadyExists.Results.Value));
			this.DelegatedMailboxFirstSP1upgrade = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.DelegatedMailboxFirstSP1upgrade).Condition((Result<object> x) => new RuleResult(this.E12SP1orHigher.Results.Value && !this.ExOrgAdmin.Results.Value && this.E12SP1orHigherMBXAlreadyExists.Results.Value));
			this.AdcFound = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.AdcFound).Condition((Result<object> x) => new RuleResult(this.AdcServer.Results.Count<Result<string>>() > 0));
			this.ProvisionedUpdateRequired = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ProvisionedUpdateRequired).Condition((Result<object> x) => new RuleResult(!string.IsNullOrEmpty(this.NewProvisionedServerName.Results.ValueOrDefault) && string.IsNullOrEmpty(this.ExOrgAdminAccountName.Results.ValueOrDefault)));
			this.GlobalServerInstall = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.GlobalServerInstall).Condition((Result<object> x) => new RuleResult(!this.ExOrgAdmin.Results.Value && !this.EnterpriseAdmin.Results.Value && (!this.ServerAlreadyExists.Results.Value || (this.HasServerDelegatedPermsBlocked.Results.Count<Result<uint>>() != 0 && !this.ServerManagement.Results.Value))));
			this.NoConnectorToStar = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Warning<RuleBuilder<object>>().Message((Result x) => Strings.NoConnectorToStar).Condition((Result<object> x) => new RuleResult(this.E15orHigherHubAlreadyExists.Results.Value && string.IsNullOrEmpty(this.ConnectorToStar.Results.ValueOrDefault)));
			this.DuplicateShortProvisionedName = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.DuplicateShortProvisionedName(this.NewProvisionedServerName.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (!string.IsNullOrEmpty(this.NewProvisionedServerName.Results.ValueOrDefault))
				{
					value = (this.ShortProvisionedName.Results.Count<Result<string>>() > 1);
				}
				return new RuleResult(value);
			});
			this.ForestLevelNotWin2003Native = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ForestLevelNotWin2003Native).Condition((Result<object> x) => new RuleResult(this.MsDSBehaviorVersion.Results.Value < 2));
			this.ServerFQDNMatchesSMTPPolicy = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidLocalComputerFQDN(this.RecipientPolicyName.Results.Value)).Condition((Result<object> x) => new RuleResult(string.Equals(this.GatewayProxy.Results.Value, this.ComputerNameDnsFullyQualified.Results.Value, StringComparison.CurrentCultureIgnoreCase)));
			this.SmtpAddressLiteral = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead | SetupRole.Global).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.NotSupportedRecipientPolicyAddressFormatValidator(this.RecipientPolicyName.Results.Value, "^\\[\\d+\\.\\d+\\.\\d+\\.\\d+\\]$")).Condition((Result<object> x) => new RuleResult(AnalysisHelpers.Match("^\\[\\d+\\.\\d+\\.\\d+\\.\\d+\\]$", new string[]
			{
				this.GatewayProxy.Results.Value
			})));
			this.InhBlockPublicFolderTree = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.InhBlockPublicFolderTree("Public Folder tree", this.MicrosoftExchServicesConfigAdminGroupPublicFoldersDistinguishedName.Results.Value)).Condition((Result<object> x) => new RuleResult((AnalysisHelpers.SdGet(this.MicrosoftExchServicesConfigAdminGroupPublicFoldersNtSecurityDescriptor.Results.Value) & 4096) == 4096));
			this.PrepareDomainNotFound = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidDomainToPrepare(this.PrepareDomain.Results.Value)).Condition(delegate(Result<object> x)
			{
				if (this.PrepareDomain.Results.Value != "F63C3A12-7852-4654-B208-125C32EB409A" && !string.IsNullOrEmpty(this.PrepareDomain.Results.ValueOrDefault))
				{
					return new RuleResult(string.IsNullOrEmpty(this.PrepareDomainNCName.Results.ValueOrDefault));
				}
				return new RuleResult(false);
			});
			this.PrepareDomainNotAdmin = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PrepareDomainNotAdmin(this.PrepareDomain.Results.Value)).Condition(delegate(Result<object> x)
			{
				if (this.PrepareDomain.Results.Value != "F63C3A12-7852-4654-B208-125C32EB409A" && !string.IsNullOrEmpty(this.PrepareDomain.Results.ValueOrDefault))
				{
					new RuleResult(!string.IsNullOrEmpty(this.PrepareDomainNCName.Results.ValueOrDefault) && (!this.PrepareDomainAdmin.Results.Value || !this.HasExchangeServersUSGBasicAccess.Results.Value) && !this.EnterpriseAdmin.Results.Value);
				}
				return new RuleResult(false);
			});
			this.PrepareDomainModeMixed = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PrepareDomainModeMixed(this.PrepareDomainNCName.Results.Value)).Condition((Result<object> x) => new RuleResult(this.NtMixedDomainPrepareDomain.Results.Value == 1));
			this.RusMissing = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Global).Mode(SetupMode.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.RecipientUpdateServiceNotAvailable(this.DomainNCName.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value;
				if (string.IsNullOrEmpty(this.RusName.Results.ValueOrDefault))
				{
					value = this.Exchange200x.Results.Any((Result<bool> w) => w.Value);
				}
				else
				{
					value = false;
				}
				return new RuleResult(value);
			});
			this.UnwillingToRemoveMailboxDatabase = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Uninstall).Error<RuleBuilder<object>>().Message((Result x) => Strings.UnwillingToRemoveMailboxDatabase(string.Join(Environment.NewLine, this.RemoveMailboxDatabaseException.Results.Value))).Condition((Result<object> x) => new RuleResult(this.RemoveMailboxDatabaseException.Results.Value.Count > 0));
			this.ExchangeVersionBlock = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.ExchangeVersionBlock).Condition((Result<object> x) => new RuleResult((double)this.ExchangeVersionPrefix.Results.Value >= 3.0));
			this.RootDomainModeMixed = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.PrepareDomainModeMixed(this.RootNamingContext.Results.Value)).Condition((Result<object> x) => new RuleResult(this.NtMixedDomainRoot.Results.Value == 1));
			this.ServerRemoveProvisioningCheck = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerRemoveProvisioningCheck(this.RemoveServerName.Results.Value)).Condition((Result<object> x) => new RuleResult((this.ExchCurrentServerRoles.Results.Value & 4096) == 4096 && !string.IsNullOrEmpty(this.RemoveServerName.Results.ValueOrDefault)));
			this.InconsistentlyConfiguredDomain = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.InconsistentlyConfiguredDomain(this.ExchNonAuthoritativeDomains.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (this.NonAuthoritativeDomainsArray.Results.Value.Count<string>() > 0 && (this.DisabledGatewayProxyArray.Results.Value.Count<string>() > 0 || this.EnabledGatewayProxyArray.Results.Value.Count<string>() > 0))
				{
					string[] array = this.ExchNonAuthoritativeDomains.Results.Value.Split(new char[]
					{
						' '
					});
					foreach (string value2 in array)
					{
						if (this.EnabledGatewayProxyArray.Results.Value.Contains(value2))
						{
							value = true;
							break;
						}
						if (this.DisabledGatewayProxyArray.Results.Value.Contains(value2))
						{
							value = true;
							break;
						}
					}
				}
				return new RuleResult(value);
			});
			this.OffLineABServerDeleted = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.ClientAccess | SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.OffLineABServerDeleted(this.OabName.Results.Value)).Condition((Result<object> x) => new RuleResult(this.OffLineABServer.Results.Any((Result<string> w) => w.Value.Contains("DEL:"))));
			this.SiteCanonicalName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Uninstall).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.SiteName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn={1},cn=Sites, {2}", this.GlobalCatalog.Results.Value, this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"canonicalName"
					}, null, SearchScope.Base);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["canonicalName"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.HubTransportRoleInCurrentADSite = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Uninstall).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.SiteName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups,{1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchServerSite=cn={0},cn=Sites,{1})(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32))", this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.E12ServersNotMinVersionRequirement = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					int num = new ServerVersion(ValidationConstant.E12MinCoExistVersionNumber.Major, ValidationConstant.E12MinCoExistVersionNumber.Minor, ValidationConstant.E12MinCoExistVersionNumber.Build, ValidationConstant.E12MinCoExistVersionNumber.Revision).ToInt();
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(!msExchCurrentServerRoles=0)(&(serialNumber=Version 8.*)(!(versionnumber>={0}))))", num), SearchScope.OneLevel);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							text += (string.IsNullOrEmpty(text) ? obj2.ToString() : string.Format(", {0}", obj2.ToString()));
						}
					}
				}
				return new Result<string>(text);
			});
			this.E14ServersNotMinVersionRequirement = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					int num = new ServerVersion(ValidationConstant.E14MinCoExistVersionNumber.Major, ValidationConstant.E14MinCoExistVersionNumber.Minor, ValidationConstant.E14MinCoExistVersionNumber.Build, ValidationConstant.E14MinCoExistVersionNumber.Revision).ToInt();
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(!msExchCurrentServerRoles=0)(&(serialNumber=Version 14.*)(!(versionnumber>={0}))))", num), SearchScope.OneLevel);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							text += (string.IsNullOrEmpty(text) ? obj2.ToString() : string.Format(", {0}", obj2.ToString()));
						}
					}
				}
				return new Result<string>(text);
			});
			this.E14ServersNotMinMajorVersionRequirement = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					int num = new ServerVersion(ValidationConstant.E14MinCoExistMajorVersionNumber.Major, ValidationConstant.E14MinCoExistMajorVersionNumber.Minor, ValidationConstant.E14MinCoExistMajorVersionNumber.Build, ValidationConstant.E14MinCoExistMajorVersionNumber.Revision).ToInt();
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(!msExchCurrentServerRoles=0)(&(serialNumber=Version 14.*)(!(versionnumber>={0}))))", num), SearchScope.OneLevel);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							text += (string.IsNullOrEmpty(text) ? obj2.ToString() : string.Format(", {0}", obj2.ToString()));
						}
					}
				}
				return new Result<string>(text);
			});
			this.E15E12CoexistenceMinVersionRequirement = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.E15E12CoexistenceMinOSReqFailure(this.E12ServersNotMinVersionRequirement.Results.Value)).Condition((Result<object> x) => new RuleResult(!string.IsNullOrEmpty(this.E12ServersNotMinVersionRequirement.Results.ValueOrDefault)));
			this.E15E14CoexistenceMinVersionRequirementForDC = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.E15E14CoexistenceMinOSReqFailureInDC(this.E14ServersNotMinVersionRequirement.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (this.globalParameters.IsDatacenter && !string.IsNullOrEmpty(this.E14ServersNotMinVersionRequirement.Results.ValueOrDefault))
				{
					value = true;
				}
				return new RuleResult(value);
			});
			this.E15E14CoexistenceMinVersionRequirement = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.E15E14CoexistenceMinOSReqFailure(this.E14ServersNotMinVersionRequirement.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (!this.globalParameters.IsDatacenter && !string.IsNullOrEmpty(this.E14ServersNotMinVersionRequirement.Results.ValueOrDefault))
				{
					value = true;
				}
				return new RuleResult(value);
			});
			this.E15E14CoexistenceMinMajorVersionRequirement = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.E15E14CoexistenceMinOSReqFailure(this.E14ServersNotMinMajorVersionRequirement.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (!this.globalParameters.IsDatacenter && !string.IsNullOrEmpty(this.E14ServersNotMinMajorVersionRequirement.Results.ValueOrDefault))
				{
					value = true;
				}
				return new RuleResult(value);
			});
		}

		private void CreateGlobalParameterPrereqProperties()
		{
			this.TargetDir = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.TargetDir));
			this.ExchangeVersion = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<Version>(this.globalParameters.ExchangeVersion));
			this.VersionNumber = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				Version value = this.ExchangeVersion.Results.Value;
				return new Result<int>(new ServerVersion(value.Major, value.Minor, value.Build, value.Revision).ToInt());
			});
			this.AdamPort = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int>(this.globalParameters.AdamPort));
			this.AdamSSLPort = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int>(this.globalParameters.AdamSSLPort));
			this.CreatePublicDB = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.CreatePublicDB));
			this.CustomerFeedbackEnabled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.CustomerFeedbackEnabled));
			this.NewProvisionedServerName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.NewProvisionedServerName));
			this.RemoveProvisionedServerName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.RemoveProvisionedServerName));
			this.GlobalCatalog = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.GlobalCatalog));
			this.DomainController = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.DomainController));
			this.PrepareDomain = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.PrepareDomain ?? "F63C3A12-7852-4654-B208-125C32EB409A"));
			this.PrepareOrganization = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.PrepareOrganization));
			this.PrepareSchema = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.PrepareSchema));
			this.PrepareAllDomains = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.PrepareAllDomains));
			this.AdInitError = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.AdInitError));
			this.LanguagePackDir = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.LanguagePackDir));
			this.LanguagesAvailableToInstall = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.LanguagesAvailableToInstall));
			this.SufficientLanguagePackDiskSpace = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.SufficientLanguagePackDiskSpace));
			this.LanguagePacksInstalled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.LanguagePacksInstalled));
			this.AlreadyInstalledUMLanguages = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(this.globalParameters.AlreadyInstalledUMLanguages));
			this.LanguagePackVersioning = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.LanguagePackVersioning));
			this.ActiveDirectorySplitPermissions = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.ActiveDirectorySplitPermissions));
			this.SetupRoles = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetMultipleValues((Result<object> x) => from w in this.globalParameters.SetupRoles
			select new Result<string>(w));
			this.IgnoreFileInUse = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(this.globalParameters.IgnoreFileInUse));
			this.RemoveServerName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(AnalysisHelpers.Replace(this.RemoveProvisionedServerName.Results.Value, "^(.*?)\\..*$", "$1")));
			this.AdInitErrorRule = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => this.AdInitError.Results.Value).Condition((Result<object> x) => new RuleResult(!string.IsNullOrEmpty(this.AdInitError.Results.ValueOrDefault)));
		}

		private void CreateMonadPrereqProperties()
		{
			this.CmdletGetMailboxServerResult = Setting<MailboxServer>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.Mailbox).Mode(SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				MailboxServer value = null;
				object[] array = new object[0];
				try
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("get-MailboxServer -Identity {0}", this.ShortServerName.Results.Value));
				}
				catch (Exception ex)
				{
					if (ex.InnerException.Message.StartsWith("Couldn't find the Enterprise Organization container"))
					{
						return new Result<MailboxServer>(value);
					}
					throw;
				}
				if (array != null && array.Length > 0)
				{
					value = (MailboxServer)array[0];
				}
				return new Result<MailboxServer>(value);
			});
			this.MemberOfDatabaseAvailabilityGroup = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox).Mode(SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.MemberOfDatabaseAvailabilityGroup).Condition((Result<object> x) => new RuleResult(this.CmdletGetMailboxServerResult.Results.Value.DatabaseAvailabilityGroup != null));
			this.CmdletGetExchangeServerResult = Setting<ExchangeServer>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall | SetupMode.DisasterRecovery).SetValue(delegate(Result<object> x)
			{
				ExchangeServer value = null;
				object[] array = new object[0];
				try
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("get-ExchangeServer -Identity {0}", this.ShortServerName.Results.Value));
				}
				catch (Exception ex)
				{
					if (ex.InnerException.Message.StartsWith("Couldn't find the Enterprise Organization container"))
					{
						return new Result<ExchangeServer>(value);
					}
					throw;
				}
				if (array != null && array.Length > 0)
				{
					value = (ExchangeServer)array[0];
				}
				return new Result<ExchangeServer>(value);
			});
			this.DrMinVersionCheck = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.All).Mode(SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DRMinVersionNotMet(this.CmdletGetExchangeServerResult.Results.Value.AdminDisplayVersion.ToString())).Condition((Result<object> x) => new RuleResult(this.globalParameters.ExchangeVersion.CompareTo(this.CmdletGetExchangeServerResult.Results.Value.AdminDisplayVersion) < 0));
			this.RemoteRegException = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.RemoteRegException).Condition(delegate(Result<object> x)
			{
				try
				{
					base.Providers.MonadDataProvider.ExecuteCommand("[Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, [System.Net.Dns]::GetHostEntry([System.Net.Dns]::GetHostName()).HostName)");
				}
				catch (Exception)
				{
					return new RuleResult(true);
				}
				return new RuleResult(false);
			});
			this.WinRMIISExtensionInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.WinRMIISExtensionInstalled).Condition((Result<object> x) => new RuleResult(this.IsWinRMIISExtensionInstalled.Results.Value));
			this.ResourcePropertySchemaException = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Global).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.ResourcePropertySchemaException(x.Exception.Message.ToString())).Condition(delegate(Result<object> x)
			{
				try
				{
					base.Providers.MonadDataProvider.ExecuteCommand("$rsCfg = Get-ResourceConfig; Set-ResourceConfig -ResourcePropertySchema $rsCfg.ResourcePropertySchema -whatif");
				}
				catch (Exception)
				{
					return new RuleResult(true);
				}
				return new RuleResult(false);
			});
			this.CmdletGetQueueResult = Setting<object>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.Bridgehead | SetupRole.Gateway).Mode(SetupMode.Uninstall).SetMultipleValues((Result<object> x) => from w in base.Providers.MonadDataProvider.ExecuteCommand("get-Queue")
			select new Result<object>(w));
			this.MessagesInQueue = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead | SetupRole.Gateway).Mode(SetupMode.Uninstall).Warning<RuleBuilder<object>>().Message(delegate(Result x)
			{
				List<string> list = new List<string>();
				foreach (Result<object> result in this.CmdletGetQueueResult.Results)
				{
					if (result.Value != null && Convert.ToInt64(AnalysisHelpers.GetObjectPropertyByName(result.Value, "MessageCount")) > 0L)
					{
						list.Add(Convert.ToString(AnalysisHelpers.GetObjectPropertyByName(result.Value, "Identity")));
					}
				}
				return Strings.MessagesInQueue(string.Join("\", \"", list));
			}).Condition((Result<object> x) => new RuleResult(this.CmdletGetQueueResult.Results.Count((Result<object> w) => w.Value != null && Convert.ToInt64(AnalysisHelpers.GetObjectPropertyByName(w.Value, "MessageCount")) > 0L) > 0));
			this.VoiceMailPath = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade).Role(SetupRole.UnifiedMessaging).SetValue((Result<object> x) => new Result<string>(string.Format("{0}\\unifiedmessaging\\voicemail", this.TargetDir.Results.Value)));
			this.VoiceMessages = Setting<int>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade).Role(SetupRole.UnifiedMessaging).SetValue(delegate(Result<object> x)
			{
				string value = this.VoiceMailPath.Results.Value;
				if (Directory.Exists(value))
				{
					object[] array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("get-childitem -name -force '{0}'", value));
					if (array != null)
					{
						return new Result<int>(array.Count<object>());
					}
				}
				return new Result<int>(0);
			});
			this.VoiceMessagesInQueue = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Upgrade).Role(SetupRole.UnifiedMessaging).Error<RuleBuilder<object>>().Message((Result x) => Strings.VoiceMessagesInQueue(this.VoiceMailPath.Results.Value)).Condition((Result<object> x) => new RuleResult(this.VoiceMessages.Results.Value > 0));
			this.RemoteRegistryServiceId = Setting<long>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object[] array = base.Providers.MonadDataProvider.ExecuteCommand("get-process | where {$_.ProcessName -like 'svchost'} | where{$_.Modules | where {$_.ModuleName -like 'regsvc.dll*'}}");
				if (!AnalysisHelpers.IsNullOrEmpty(array))
				{
					return new Result<long>((long)((Process)array[0]).Id);
				}
				return new Result<long>(-1L);
			});
			this.OneCopyAlertProcessId = Setting<long>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Process WHERE Name='powershell.exe' AND CommandLine LIKE '%CheckDatabaseRedundancy%'")[0].TryGetValue("ProcessId", out obj))
				{
					return new Result<long>((long)((ulong)((uint)obj)));
				}
				return new Result<long>(-1L);
			});
			this.OpenProcesses = Setting<Process>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).SetMultipleValues(delegate(Result<object> x)
			{
				object[] array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("get-process | ?{{$_.ProcessName -notmatch '{0}'}} | ?{{Get-ProcessModule -ProcessId $_.Id | ?{{$_ -like '{1}*'}}}}", this.globalParameters.IsDatacenter ? "^(Setup|Exsetup|ExSetupUI|WmiPrvSE|MOM|MonitoringHost|w3wp|msftesql|msftefd|EdgeTransport|mad|store|umservice|UMWorkerProcess|TranscodingService|SESWorker|ExBPA|ExFBA|wsbexchange|hostcontrollerservice|noderunner|parserserver|Microsoft\\.Exchange\\..*|MSExchange.*|fms|scanningprocess|FSCConfigurationServer|updateservice|ScanEngineTest|EngineUpdateLogger|sftracing|ForefrontActiveDirectoryConnector|rundll32|MSMessageTracingClient|wsmprovhost|Microsoft.Office.BigData.DataLoader)$" : "^(Setup|Exsetup|ExSetupUI|WmiPrvSE|MOM|MonitoringHost|w3wp|msftesql|msftefd|EdgeTransport|mad|store|umservice|UMWorkerProcess|TranscodingService|SESWorker|ExBPA|ExFBA|wsbexchange|hostcontrollerservice|noderunner|parserserver|Microsoft\\.Exchange\\..*|MSExchange.*|fms|scanningprocess|FSCConfigurationServer|updateservice|ScanEngineTest|EngineUpdateLogger|sftracing|ForefrontActiveDirectoryConnector|rundll32|MSMessageTracingClient)$", this.TargetDir.Results.Value));
				if (array == null)
				{
					array = new object[0];
				}
				return from w in array
				select new Result<Process>((Process)w);
			});
			this.OpenProcessesOnUpgrade = Setting<Process>.Build().WithParent<Process>(() => this.OpenProcesses).In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue(delegate(Result<Process> x)
			{
				if (!this.IgnoreFileInUse.Results.Value && x.ValueOrDefault != null && (long)x.Value.Id != this.RemoteRegistryServiceId.Results.Value && (long)x.Value.Id != this.OneCopyAlertProcessId.Results.Value)
				{
					return new Result<Process>(x.Value);
				}
				return new Result<Process>(null);
			});
			this.ProcessNeedsToBeClosedOnUpgrade = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Upgrade).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message(delegate(Result x)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Result<Process> result in this.OpenProcessesOnUpgrade.Results)
				{
					if (result.Value != null)
					{
						stringBuilder.Append(result.Value.ProcessName);
						stringBuilder.Append(" (");
						stringBuilder.Append(result.Value.Id);
						stringBuilder.Append("), ");
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder = stringBuilder.Remove(stringBuilder.Length - 2, 2);
				}
				return Strings.ProcessNeedsToBeClosedOnUpgrade(stringBuilder.ToString());
			}).Condition((Result<object> x) => new RuleResult(this.OpenProcessesOnUpgrade.Results.Count((Result<Process> w) => w.Value != null) > 0));
			this.OpenProcessesOnUninstall = Setting<Process>.Build().WithParent<Process>(() => this.OpenProcesses).In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue(delegate(Result<Process> x)
			{
				if (x.ValueOrDefault != null && (long)x.Value.Id != this.RemoteRegistryServiceId.Results.Value)
				{
					return new Result<Process>(x.Value);
				}
				return new Result<Process>(null);
			});
			this.ProcessNeedsToBeClosedOnUninstall = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message(delegate(Result x)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Result<Process> result in this.OpenProcessesOnUpgrade.Results)
				{
					if (result.Value != null)
					{
						stringBuilder.Append(result.Value.ProcessName);
						stringBuilder.Append(" (");
						stringBuilder.Append(result.Value.Id);
						stringBuilder.Append("), ");
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder = stringBuilder.Remove(stringBuilder.Length - 2, 2);
				}
				return Strings.ProcessNeedsToBeClosedOnUninstall(stringBuilder.ToString());
			}).Condition((Result<object> x) => new RuleResult(this.OpenProcessesOnUninstall.Results.Count((Result<Process> w) => w.Value != null) > 0));
			this.SendConnectorException = Rule.Build().WithParent<ResultPropertyCollection>(() => this.OrgMicrosoftExchServicesConfig).In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Bridgehead).Error<RuleBuilder<ResultPropertyCollection>>().Message((Result x) => Strings.SendConnectorException).Condition(delegate(Result<ResultPropertyCollection> x)
			{
				if (x.Value != null)
				{
					try
					{
						base.Providers.MonadDataProvider.ExecuteCommand("Get-SendConnector");
					}
					catch (Exception)
					{
						return new RuleResult(true);
					}
				}
				return new RuleResult(false);
			});
			this.MailboxLogDriveNotExistList = Setting<List<string>>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (string text in this.PrivateDatabaseLogDrive.Results.Value)
				{
					object[] array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("test-path '{0}'", Path.GetPathRoot(text)));
					if (!AnalysisHelpers.IsNullOrEmpty(array) && !(bool)array[0])
					{
						list.Add(text);
					}
				}
				return new Result<List<string>>(list);
			});
			this.MailboxLogDriveDoesNotExist = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).Error<RuleBuilder<object>>().Message((Result x) => Strings.MailboxLogDriveDoesNotExist(string.Join(",", this.MailboxLogDriveNotExistList.Results.Value.ToArray()))).Condition((Result<object> x) => new RuleResult(this.MailboxLogDriveNotExistList.Results.Value.Count > 0));
			this.MailboxEDBDriveNotExistList = Setting<List<string>>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (string text in this.PrivateDatabaseEdbDrive.Results.Value)
				{
					object[] array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("test-path '{0}'", Path.GetPathRoot(text)));
					if (!AnalysisHelpers.IsNullOrEmpty(array) && !(bool)array[0])
					{
						list.Add(text);
					}
				}
				return new Result<List<string>>(list);
			});
			this.MailboxEDBDriveDoesNotExist = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).Error<RuleBuilder<object>>().Message((Result x) => Strings.MailboxEDBDriveDoesNotExist(string.Join(",", this.MailboxLogDriveNotExistList.Results.Value.ToArray()))).Condition((Result<object> x) => new RuleResult(this.MailboxEDBDriveNotExistList.Results.Value.Count > 0));
			this.RemoveMailboxDatabaseException = Setting<List<string>>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.Mailbox).Mode(SetupMode.Uninstall).SetValue(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				foreach (string arg in this.PrivateDatabaseNameDN.Results.Value)
				{
					try
					{
						base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Remove-MailboxDatabase '{0}' -whatif", arg));
					}
					catch (Exception ex)
					{
						list.Add(ex.Message.ToString());
					}
				}
				return new Result<List<string>>(list);
			});
			this.CmdletGetUMServerResult = Setting<UMServer>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Upgrade).SetValue(delegate(Result<object> x)
			{
				UMServer value = null;
				if (!string.IsNullOrEmpty(this.ShortServerName.Results.ValueOrDefault))
				{
					object[] array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Get-UMService -Identity {0}", this.ShortServerName.Results.Value));
					if (array != null && array.Length > 0)
					{
						value = (UMServer)array[0];
					}
				}
				return new Result<UMServer>(value);
			});
			this.LanguageInUMServer = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Upgrade).SetMultipleValues((Result<object> x) => from w in this.CmdletGetUMServerResult.Results.Value.Languages
			where w.Culture.Name.ToUpper() != "EN-US"
			select new Result<string>(w.Culture.Name));
			this.AdditionalUMLangPackExists = Rule.Build().WithParent<string>(() => this.LanguageInUMServer).In(this).AsSync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Upgrade).Error<RuleBuilder<string>>().Message((Result x) => Strings.AdditionalUMLangPackExists(x.AncestorOfType<string>(this.LanguageInUMServer).Value)).Condition((Result<string> x) => new RuleResult(true));
			this.SendConnector = Setting<object>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).SetMultipleValues(delegate(Result<object> x)
			{
				object[] array = null;
				if (!string.IsNullOrEmpty(this.ShortServerName.Results.ValueOrDefault))
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Get-SendConnector | where {{$_.SourceTransportServers -match '^{0}$'}}", this.ShortServerName.Results.Value));
				}
				if (array == null)
				{
					array = new object[0];
				}
				return from w in array
				select new Result<object>((Process)w);
			});
			this.GroupDN = Setting<object>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).SetMultipleValues(delegate(Result<object> x)
			{
				object[] array = null;
				if (!string.IsNullOrEmpty(this.PrereqServerLegacyDN.Results.ValueOrDefault))
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Get-DistributionGroup | where {{$_.ExpansionServer -eq '{0}'}}", this.PrereqServerLegacyDN.Results.Value));
				}
				if (array == null)
				{
					array = new object[0];
				}
				return from w in array
				select new Result<object>((Process)w);
			});
			this.DynamicGroupDN = Setting<object>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).SetMultipleValues(delegate(Result<object> x)
			{
				object[] array = null;
				if (!string.IsNullOrEmpty(this.PrereqServerLegacyDN.Results.ValueOrDefault))
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Get-DynamicDistributionGroup | where {{$_.ExpansionServer -eq '{0}'}}", this.PrereqServerLegacyDN.Results.Value));
				}
				if (array == null)
				{
					array = new object[0];
				}
				return from w in array
				select new Result<object>((Process)w);
			});
			this.ServerIsSourceForSendConnector = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerIsSourceForSendConnector(this.SendConnector.Results.Count<Result<object>>())).Condition((Result<object> x) => new RuleResult(this.SendConnector.Results.Count<Result<object>>() > 0));
			this.ServerIsGroupExpansionServer = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerIsGroupExpansionServer(this.GroupDN.Results.Count<Result<object>>())).Condition((Result<object> x) => new RuleResult(this.GroupDN.Results.Count<Result<object>>() > 0));
			this.ServerIsDynamicGroupExpansionServer = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerIsDynamicGroupExpansionServer(this.DynamicGroupDN.Results.Count<Result<object>>())).Condition((Result<object> x) => new RuleResult(this.DynamicGroupDN.Results.Count<Result<object>>() > 0));
			this.ServerIsLastHubForEdgeSubscription = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServerIsLastHubForEdgeSubscription(this.SiteName.Results.Value)).Condition((Result<object> x) => new RuleResult(this.EdgeSubscriptionForSite.Results.Count<Result<object>>() > 0 && !string.IsNullOrEmpty(this.HubTransportRoleInCurrentADSite.Results.ValueOrDefault)));
			this.EdgeSubscriptionExists = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Uninstall).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.EdgeSubscriptionExists).Condition(delegate(Result<object> x)
			{
				bool value = false;
				object[] array = base.Providers.MonadDataProvider.ExecuteCommand("get-EdgeSubscription");
				if (array != null && array.Length > 0 && string.Compare(AnalysisHelpers.GetObjectPropertyByName(array[0], "Identity").ToString(), this.ComputerNameNetBIOS.Results.ValueOrDefault, true) == 0)
				{
					value = true;
				}
				return new RuleResult(value);
			});
			this.EdgeSubscriptionForSite = Setting<object>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Bridgehead).SetMultipleValues(delegate(Result<object> x)
			{
				object[] array = null;
				if (!string.IsNullOrEmpty(this.SiteCanonicalName.Results.ValueOrDefault))
				{
					array = base.Providers.MonadDataProvider.ExecuteCommand(string.Format("get-EdgeSubscription | where {{$_.Site -eq '{0}'}}", this.SiteCanonicalName.Results.Value));
				}
				if (array == null)
				{
					array = new object[0];
				}
				return from w in array
				select new Result<object>((Process)w);
			});
			this.RSATWebServerNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-Web-Server")).Condition(delegate(Result<object> x)
			{
				bool value = false;
				if (this.Windows2K8R2Version.Results.ValueOrDefault)
				{
					value = !this.IsRSATWebServerInstalled.Results.Value;
				}
				return new RuleResult(value);
			});
			this.NETFrameworkNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("NET-Framework")).Condition((Result<object> x) => new RuleResult(this.Windows2K8R2Version.Results.ValueOrDefault && !this.IsNETFrameworkInstalled.Results.Value));
			this.NETFramework45FeaturesNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("NET-Framework-45-Features")).Condition((Result<object> x) => new RuleResult((this.Windows8Version.Results.ValueOrDefault || this.Windows8ClientVersion.Results.ValueOrDefault) && !this.IsNETFramework45FeaturesInstalled.Results.Value));
			this.WebNetExt45NotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("Web-Net-Ext45")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsWebNetExt45Installed.Results.Value));
			this.WebISAPIExtNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("Web-ISAPI-Ext")).Condition((Result<object> x) => new RuleResult(!this.IsWebISAPIExtInstalled.Results.Value));
			this.WebASPNET45NotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("Web-ASP-NET45")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsWebASPNET45Installed.Results.Value));
			this.RPCOverHTTPproxyNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RPC-over-HTTP-proxy")).Condition((Result<object> x) => new RuleResult(!this.IsRPCOverHTTPproxyInstalled.Results.Value));
			this.ServerGuiMgmtInfraNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("Server-Gui-Mgmt-Infra")).Condition((Result<object> x) => new RuleResult((this.Windows8Version.Results.ValueOrDefault || this.Windows8ClientVersion.Results.ValueOrDefault) && !this.IsServerGuiMgmtInfraInstalled.Results.Value));
			this.WcfHttpActivation45Installed = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("NET-WCF-HTTP-Activation45")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsWcfHttpActivation45Installed.Results.Value));
			this.RsatAddsToolsInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Global).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-ADDS-Tools")).Condition((Result<object> x) => new RuleResult((this.PrepareSchema.Results.Value || this.PrepareOrganization.Results.Value) && !this.IsRsatAddsToolsInstalled.Results.Value));
			this.RsatClusteringInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-Clustering")).Condition((Result<object> x) => new RuleResult(!this.IsRsatClusteringInstalled.Results.Value));
			this.RsatClusteringMgmtInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-Clustering-Mgmt")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsRsatClusteringMgmtInstalled.Results.Value));
			this.RsatClusteringPowerShellInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-Clustering-PowerShell")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsRsatClusteringPowerShellInstalled.Results.Value));
			this.RsatClusteringCmdInterfaceInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequiredToInstall("RSAT-Clustering-CmdInterface")).Condition((Result<object> x) => new RuleResult(this.Windows8Version.Results.ValueOrDefault && !this.IsRsatClusteringCmdInterfaceInstalled.Results.Value));
			this.PowerShellExecutionPolicyCheckSet = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PowerShellExecutionPolicyCheck).Condition((Result<object> x) => new RuleResult(this.PowerShellExecutionPolicy.Results.Value));
			this.PowerShellExecutionPolicy = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool flag = new string[]
				{
					"MachinePolicy",
					"UserPolicy"
				}.All((string a) => !base.Providers.MonadDataProvider.ExecuteCommand(string.Format("Get-ExecutionPolicy -Scope {0}", a)).First<object>().ToString().Equals("Restricted", StringComparison.InvariantCultureIgnoreCase));
				return new Result<bool>(!flag);
			});
		}

		private void CreateNativePrereqProperties()
		{
			this.HasExchangeServersUSGWritePerms = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((base.Providers.NativeMethodProvider.GetAccessCheck(this.ExchangeServersGroupNtSecurityDescriptor.Results.Value, string.Empty) & PrereqAnalysis.adWritePermissions) == PrereqAnalysis.adWritePermissions));
			this.HasExchangeServersUSGBasicAccess = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				bool value = false;
				uint num = base.Providers.NativeMethodProvider.GetAccessCheck(this.ExchangeServersGroupNtSecurityDescriptor.Results.Value, string.Empty) & PrereqAnalysis.adBasicPermissions;
				if (num == PrereqAnalysis.adBasicPermissions)
				{
					value = true;
				}
				return new Result<bool>(value);
			});
			this.HasExtendedRightsCreateChildPerms = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>((base.Providers.NativeMethodProvider.GetAccessCheck(this.ExtendedRightsNtSecurityDescriptor.Results.Value, string.Empty) & 1U) == 1U));
			this.HasServerDelegatedPermsBlocked = Setting<uint>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<uint> list = new List<uint>();
				if (!string.IsNullOrEmpty(this.NtSecurityDescriptor.Results.Value))
				{
					uint num = base.Providers.NativeMethodProvider.GetAccessCheck(this.NtSecurityDescriptor.Results.Value, "'0;a8df74a7-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74b2-c5ea-11d1-bbcb-0080c76670c0'|'0;bf967a8b-0de6-11d0-a285-00aa003049e2'|'0;28630ec1-41d5-11d1-a9c1-0000f80367c1'|'0;031b371a-a981-11d2-a9ff-00c04f8eedd8'|'0;3435244a-a982-11d2-a9ff-00c04f8eedd8'|'0;36145cf4-a982-11d2-a9ff-00c04f8eedd8'|'0;966540a1-75f7-4d27-ace9-3858b5dea688'|'0;9432cae6-b09e-11d2-aa06-00c04f8eedd8'|'0;93da93e4-b09e-11d2-aa06-00c04f8eedd8'|'0;a8df74d1-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74c5-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74ce-c5ea-11d1-bbcb-0080c76670c0'|'0;3378ca84-a982-11d2-a9ff-00c04f8eedd8'|'0;33bb8c5c-a982-11d2-a9ff-00c04f8eedd8'|'0;3397c916-a982-11d2-a9ff-00c04f8eedd8'|'0;8ef628c6-b093-11d2-aa06-00c04f8eedd8'|'0;8ef628c6-b093-11d2-aa06-00c04f8eedd8'|'0;93bb9552-b09e-11d2-aa06-00c04f8eedd8'|'0;44601346-776a-46e7-b4a4-2472e1c66806'|'0;20309cbd-0ae3-4876-9114-5738c65f845c'") & PrereqAnalysis.adWritePermissions;
					if (num != PrereqAnalysis.adWritePermissions)
					{
						list.Add(num);
					}
				}
				return from w in list
				select new Result<uint>(w);
			});
			this.EnterpriseAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(string.Format("{0}-519", this.SidRootDomain.Results.Value))));
			this.SchemaAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(string.Format("{0}-518", this.SidRootDomain.Results.Value))));
			this.LocalDomainAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(string.Format("{0}-512", this.LocalDomainNtSecurityDescriptor.Results.Value))));
			this.PrepareDomainAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(string.Format("{0}-512", this.SidPrepareDomain.Results.Value))));
			this.SMOSchemaSiteName = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.NativeMethodProvider.GetSiteName(this.DnsHostName.Results.ValueOrDefault)));
			this.ExOrgAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.SidExOrgAdmins.Results.ValueOrDefault))
				{
					return new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(this.SidExOrgAdmins.Results.Value));
				}
				return new Result<bool>(false);
			});
			this.ServerManagement = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.SidServerManagementGroup.Results.ValueOrDefault))
				{
					return new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck(this.SidServerManagementGroup.Results.Value));
				}
				return new Result<bool>(false);
			});
			this.SiteName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<string>(base.Providers.NativeMethodProvider.GetSiteName(string.Empty)));
			this.DomainControllerSiteName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue((Result<object> x) => new Result<string>(base.Providers.NativeMethodProvider.GetSiteName(this.DomainController.Results.Value)));
			this.DomainControllerIsOutOfSite = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.DomainControllerOutOfSiteValidator(this.DomainController.Results.Value, this.DomainControllerSiteName.Results.Value, this.SiteName.Results.Value)).Condition((Result<object> x) => new RuleResult(!string.Equals(this.SiteName.Results.Value, this.DomainControllerSiteName.Results.Value, StringComparison.CurrentCultureIgnoreCase)));
			this.LocalAdmin = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.TokenMembershipCheck("S-1-5-32-544")));
			this.NotLocalAdmin = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.NotLocalAdmin(this.CurrentLogOn.Results.Value)).Condition((Result<object> x) => new RuleResult(!this.LocalAdmin.Results.Value));
			this.IsCoreServer = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.NativeMethodProvider.IsCoreServer()));
			this.WindowsServer2008CoreServerEdition = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.WindowsServer2008CoreServerEdition).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.1" && this.IsCoreServer.Results.Value));
		}

		private void CreateRegistryPrereqProperties()
		{
			this.ExchangeAlreadyInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PreviousVersionOfExchangeAlreadyInstalled).Condition(delegate(Result<object> x)
			{
				if (this.NewestBuild.Results.ValueOrDefault != null)
				{
					return new RuleResult(this.NewestBuild.Results.Value < 10000 && this.ServicesPath.Results.ValueOrDefault != null);
				}
				return new RuleResult(false);
			});
			this.NewestBuild = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "NewestBuild")));
			this.ServicesPath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "Services")));
			this.Roles = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetMultipleValues((Result<object> x) => from w in (string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15", null)
			select new Result<string>(w));
			this.ServerRoleUnpacked = Setting<string>.Build().WithParent<string>(() => this.Roles).In(this).AsSync().Role(SetupRole.All).SetValue((Result<string> x) => new Result<string>(AnalysisHelpers.Replace(x.Value, "(.*Role)", "$1")));
			this.Watermarks = Setting<string>.Build().WithParent<string>(() => this.ServerRoleUnpacked).In(this).AsSync().Role(SetupRole.All).SetValue((Result<string> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + x.Value, "Watermark")));
			this.InstallWatermark = Rule.Build().WithParent<string>(() => this.Watermarks).In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.DisasterRecovery).Error<RuleBuilder<string>>().Message((Result x) => Strings.WatermarkPresent(x.AncestorOfType<string>(this.ServerRoleUnpacked).Value)).Condition(delegate(Result<string> x)
			{
				bool value = false;
				if (x.ValueOrDefault != null)
				{
					value = true;
				}
				return new RuleResult(value);
			});
			this.Actions = Setting<string>.Build().WithParent<string>(() => this.ServerRoleUnpacked).In(this).AsSync().Role(SetupRole.All).SetValue((Result<string> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + x.Value, "Action")));
			this.FilteredRoles = Setting<string>.Build().WithParent<string>(() => this.Roles).In(this).AsSync().Role(SetupRole.All).SetValue((Result<string> x) => new Result<string>(AnalysisHelpers.Replace(x.Value, "^.*\\\\(.*)Role$", "$1")));
			this.InterruptedUninstallNotContinued = Rule.Build().WithParent<string>(() => this.ServerRoleUnpacked).In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.Uninstall).Error<RuleBuilder<string>>().Message((Result x) => Strings.InterruptedUninstallNotContinued(x.AncestorOfType<string>(this.ServerRoleUnpacked).Value)).Condition(delegate(Result<string> x)
			{
				if (this.Actions.Results.ValueOrDefault != null)
				{
					bool value;
					if (this.Actions.Results.Value == "Uninstall")
					{
						value = (this.SetupRoles.Results.Count((Result<string> w) => w.Value.Equals(SetupRole.Global.ToString(), StringComparison.InvariantCultureIgnoreCase)) <= 0 && !this.SetupRoles.Results.Value.Contains(this.FilteredRoles.Results.Value));
					}
					else
					{
						value = false;
					}
					return new RuleResult(value);
				}
				return new RuleResult(false);
			});
			this.UcmaRedistVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\UCMA\\{902F4F35-D5DC-4363-8671-D5EF0D26C21D}", "Version")));
			this.SpeechRedist = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Classes\\Installer\\Assemblies\\Global", "Microsoft.Speech,version=\"11.0.0.0\",culture=\"neutral\",publicKeyToken=\"31BF3856AD364E35\",processorArchitecture=\"MSIL\"")));
			this.WindowsVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "CurrentVersion")));
			this.WindowsBuild = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "CurrentBuild")));
			this.WindowsSPLevel = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "CSDVersion")));
			this.WindowsProductName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "ProductName")));
			this.ADAMVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.Gateway).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\ADAM_Shared", "InstalledVersion")));
			this.OldADAMInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.OldADAMInstalled).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ADAMVersion.Results.ValueOrDefault))
				{
					new RuleResult(this.ADAMVersion.Results.Value.StartsWith("1.1.3790") && new Version(this.ADAMVersion.Results.Value).Revision < 2075);
				}
				return new RuleResult(false);
			});
			this.SMTPSvcStartMode = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\SMTPSVC", "Start")));
			this.SMTPSvcDisplayName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\SMTPSVC", "DisplayName")));
			this.MsiInstallPath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath")));
			this.IISCommonFiles = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp", null)));
			this.IIS6MetabaseStatus = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "Metabase")));
			this.IIS6ManagementConsoleStatus = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "LegacySnapin")));
			this.IIS7CompressionDynamic = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpCompressionDynamic")));
			this.IIS7CompressionStatic = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpCompressionStatic")));
			this.IIS7ManagedCodeAssemblies = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ManagedCodeAssemblies")));
			this.WASProcessModel = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ProcessModel")));
			this.IIS7BasicAuthentication = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "BasicAuthentication")));
			this.IIS7WindowAuthentication = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "WindowsAuthentication")));
			this.IIS7DigestAuthentication = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "DigestAuthentication")));
			this.IIS7NetExt = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "NetFxExtensibility")));
			this.IIS6WMICompatibility = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "WMICompatibility")));
			this.ASPNET = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ASPNET")));
			this.ISAPIFilter = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ISAPIFilter")));
			this.ClientCertificateMappingAuthentication = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ClientCertificateMappingAuthentication")));
			this.DirectoryBrowse = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "DirectoryBrowse")));
			this.HttpErrors = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpErrors")));
			this.HttpLogging = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpLogging")));
			this.HttpRedirect = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpRedirect")));
			this.HttpTracing = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "HttpTracing")));
			this.RequestMonitor = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "RequestMonitor")));
			this.StaticContent = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "StaticContent")));
			this.ManagementService = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "AdminService")));
			this.W3SVCDisabledOrNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.W3SVCDisabledOrNotInstalled).Condition((Result<object> x) => new RuleResult(this.W3SVCStartMode.Results.ValueOrDefault == 0 || this.W3SVCStartMode.Results.ValueOrDefault == 4));
			this.W3SVCStartMode = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int>((int)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\W3SVC", "Start")));
			this.ShouldReRunSetupForW3SVC = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ShouldReRunSetupForW3SVC).Condition((Result<object> x) => new RuleResult(this.W3SVCStartMode.Results.Value != 2));
			this.SMTPSvcInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.SMTPSvcInstalled).Condition((Result<object> x) => new RuleResult(this.SMTPSvcStartMode.Results.ValueOrDefault != null && !this.ExchangeAlreadyInstalled.Results.Value));
			this.ClusSvcInstalledRoleBlock = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.ClusSvcInstalledRoleBlock).Condition((Result<object> x) => new RuleResult(this.ClusSvcStartMode.Results.ValueOrDefault == 2));
			this.ClusSvcStartMode = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int>((int)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\ClusSvc", "Start")));
			this.LonghornIIS6MetabaseNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 6 Metabase Compatibility")).Condition((Result<object> x) => new RuleResult(this.IIS6MetabaseStatus.Results.ValueOrDefault == null || this.IIS6MetabaseStatus.Results.Value == 0));
			this.LonghornIIS6MgmtConsoleNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 6 Management Console")).Condition((Result<object> x) => new RuleResult(this.IIS6ManagementConsoleStatus.Results.ValueOrDefault == null || this.IIS6ManagementConsoleStatus.Results.Value == 0));
			this.LonghornIIS7HttpCompressionDynamicNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 Dynamic Content Compression")).Condition((Result<object> x) => new RuleResult(this.IIS7CompressionDynamic.Results.ValueOrDefault == null || this.IIS7CompressionDynamic.Results.Value == 0));
			this.LonghornIIS7HttpCompressionStaticNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 Static Content Compression")).Condition((Result<object> x) => new RuleResult(this.IIS7CompressionStatic.Results.ValueOrDefault == null || this.IIS7CompressionStatic.Results.Value == 0));
			this.LonghornWASProcessModelInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Windows Process Activation Service Process Model")).Condition((Result<object> x) => new RuleResult(this.WASProcessModel.Results.ValueOrDefault == null || this.WASProcessModel.Results.Value == 0));
			this.LonghornIIS7BasicAuthNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 Basic Authentication")).Condition((Result<object> x) => new RuleResult(this.IIS7BasicAuthentication.Results.ValueOrDefault == null || this.IIS7BasicAuthentication.Results.Value == 0));
			this.LonghornIIS7WindowsAuthNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 Windows Authentication")).Condition((Result<object> x) => new RuleResult(this.IIS7WindowAuthentication.Results.ValueOrDefault == null || this.IIS7WindowAuthentication.Results.Value == 0));
			this.LonghornIIS7DigestAuthNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 Digest Authentication")).Condition((Result<object> x) => new RuleResult(this.IIS7DigestAuthentication.Results.ValueOrDefault == null || this.IIS7DigestAuthentication.Results.Value == 0));
			this.LonghornIIS7NetExt = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 7 .NET Extensibility")).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.1" && (this.IIS7NetExt.Results.ValueOrDefault == null || this.IIS7NetExt.Results.Value == 0)));
			this.LonghornIIS6WMICompatibility = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS 6 WMI Compatibility")).Condition((Result<object> x) => new RuleResult(this.IIS6WMICompatibility.Results.ValueOrDefault == null || this.IIS6WMICompatibility.Results.Value == 0));
			this.LonghornASPNET = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("ASP .NET")).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.1" && (this.ASPNET.Results.ValueOrDefault == null || this.ASPNET.Results.Value == 0)));
			this.LonghornISAPIFilter = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("ISAPI Filter")).Condition((Result<object> x) => new RuleResult(this.ISAPIFilter.Results.ValueOrDefault == null || this.ISAPIFilter.Results.Value == 0));
			this.LonghornClientCertificateMappingAuthentication = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Client Certificate Mapping Authentication")).Condition((Result<object> x) => new RuleResult(this.ClientCertificateMappingAuthentication.Results.ValueOrDefault == null || this.ClientCertificateMappingAuthentication.Results.Value == 0));
			this.LonghornDirectoryBrowse = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Directory Browsing")).Condition((Result<object> x) => new RuleResult(this.DirectoryBrowse.Results.ValueOrDefault == null || this.DirectoryBrowse.Results.Value == 0));
			this.LonghornHttpErrors = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("HTTP Errors")).Condition((Result<object> x) => new RuleResult(this.HttpErrors.Results.ValueOrDefault == null || this.HttpErrors.Results.Value == 0));
			this.LonghornHttpLogging = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("HTTP Logging")).Condition((Result<object> x) => new RuleResult(this.HttpLogging.Results.ValueOrDefault == null || this.HttpLogging.Results.Value == 0));
			this.LonghornHttpRedirect = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("HTTP Redirection")).Condition((Result<object> x) => new RuleResult(this.HttpRedirect.Results.ValueOrDefault == null || this.HttpRedirect.Results.Value == 0));
			this.LonghornHttpTracing = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Tracing")).Condition((Result<object> x) => new RuleResult(this.HttpTracing.Results.ValueOrDefault == null || this.HttpTracing.Results.Value == 0));
			this.LonghornRequestMonitor = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Request Monitor")).Condition((Result<object> x) => new RuleResult(this.RequestMonitor.Results.ValueOrDefault == null || this.RequestMonitor.Results.Value == 0));
			this.LonghornStaticContent = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Static Content")).Condition((Result<object> x) => new RuleResult(this.StaticContent.Results.ValueOrDefault == null || this.StaticContent.Results.Value == 0));
			this.ManagementServiceInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Web-Mgmt-Service")).Condition((Result<object> x) => new RuleResult(this.ManagementService.Results.ValueOrDefault == null || this.ManagementService.Results.Value == 0));
			this.ADAMWin7ServerInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADAMLonghornWin7ServerNotInstalled).Condition((Result<object> x) => new RuleResult(string.IsNullOrEmpty(this.ADAMVersion.Results.ValueOrDefault)));
			string ucmaVersion = "5.0.8308.0";
			this.UcmaRedistMsi = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UnifiedMessaging | SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.UcmaRedistMsi).Condition((Result<object> x) => new RuleResult(this.UcmaRedistVersion.Results.ValueOrDefault == null || AnalysisHelpers.VersionCompare(this.UcmaRedistVersion.Results.Value, ucmaVersion) < 0));
			this.SpeechRedistMsi = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox | SetupRole.UnifiedMessaging | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.SpeechRedistMsi).Condition((Result<object> x) => new RuleResult(AnalysisHelpers.VersionCompare(this.UcmaRedistVersion.Results.Value, ucmaVersion) >= 0 && this.SpeechRedist.Results.ValueOrDefault == null));
			this.Wif35Installed = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows Identity Foundation\\Setup\\v3.5", null)));
			this.ClrReleaseNumber = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Cafe).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", "Release")));
			this.Win7WindowsIdentityFoundationUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.Win7WindowsIdentityFoundationUpdateNotInstalled).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.1" && this.Wif35Installed.Results.ValueOrDefault == null));
			this.Win8WindowsIdentityFoundationUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Windows-Identity-Foundation")).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.2" && this.Wif35Installed.Results.ValueOrDefault == null));
			this.HttpActivationInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.ClientAccess | SetupRole.Cafe).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("Net-Http-Activation")).Condition((Result<object> x) => new RuleResult(this.WindowsVersion.Results.Value == "6.1" && this.HTTPActivation.Results.ValueOrDefault == null));
			this.MailboxRoleNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.MailboxRoleNotInstalled).Condition((Result<object> x) => new RuleResult(!this.MailboxRoleInstalled.Results.Value && this.ClusSvcStartMode.Results.Value >= 0));
			this.MailboxConfiguredVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailBoxRole", "ConfiguredVersion")));
			this.MailboxUnpackedVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailBoxRole", "UnpackedVersion")));
			this.MailboxPreviousBuild = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MailboxUnpackedVersion.Results.ValueOrDefault))
				{
					return new Result<bool>(AnalysisHelpers.VersionCompare(this.MailboxUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) < 0);
				}
				return new Result<bool>(false);
			});
			this.MailboxMinVersionCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.MinVersionCheck).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MailboxUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.MailboxConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.MailboxUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) >= 0 && this.MailboxUnpackedVersion.Results.Value == this.MailboxConfiguredVersion.Results.Value && !this.PreviousBuildDetected.Results.ValueOrDefault);
				}
				return new RuleResult(this.PreviousBuildDetected.Results.ValueOrDefault);
			});
			this.MailboxUpgradeMinVersionBlock = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Mailbox).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.UpgradeMinVersionBlock).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MailboxUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.MailboxConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.MailboxUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) > 0 && this.MailboxUnpackedVersion.Results.Value == this.MailboxConfiguredVersion.Results.Value);
				}
				return new RuleResult(false);
			});
			this.UpgradeGateway605Block = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.UpgradeGateway605Block).Condition((Result<object> x) => new RuleResult(AnalysisHelpers.VersionCompare(this.GatewayConfiguredVersion.Results.Value, "8.0.606.0") < 0));
			this.UnifiedMessagingConfiguredVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", "ConfiguredVersion")));
			this.UnifiedMessagingUnpackedVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", "UnpackedVersion")));
			this.UnifiedMessagingPreviousBuild = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.UnifiedMessagingUnpackedVersion.Results.ValueOrDefault))
				{
					return new Result<bool>(AnalysisHelpers.VersionCompare(this.UnifiedMessagingUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) < 0);
				}
				return new Result<bool>(false);
			});
			this.UnifiedMessagingMinVersionCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.MinVersionCheck).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.UnifiedMessagingUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.UnifiedMessagingConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.UnifiedMessagingUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) >= 0 && this.UnifiedMessagingUnpackedVersion.Results.Value == this.UnifiedMessagingConfiguredVersion.Results.Value && !this.PreviousBuildDetected.Results.ValueOrDefault);
				}
				return new RuleResult(this.PreviousBuildDetected.Results.ValueOrDefault);
			});
			this.UnifiedMessagingUpgradeMinVersionBlock = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.UnifiedMessaging).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.UpgradeMinVersionBlock).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.UnifiedMessagingUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.UnifiedMessagingConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.UnifiedMessagingUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) > 0 && this.UnifiedMessagingUnpackedVersion.Results.Value == this.UnifiedMessagingConfiguredVersion.Results.Value);
				}
				return new RuleResult(false);
			});
			this.ClientAccessConfiguredVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ClientAccessRole", "ConfiguredVersion")));
			this.BridgeheadConfiguredVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string value = null;
				try
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BridgeheadRole", "ConfiguredVersion");
				}
				catch (FailureException)
				{
				}
				if (string.IsNullOrEmpty(value))
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HubTransportRole", "ConfiguredVersion");
				}
				return new Result<string>(value);
			});
			this.BridgeheadUnpackedVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string value = null;
				try
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BridgeheadRole", "UnpackedVersion");
				}
				catch (FailureException)
				{
				}
				if (string.IsNullOrEmpty(value))
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HubTransportRole", "UnpackedVersion");
				}
				return new Result<string>(value);
			});
			this.BridgeheadPreviousBuild = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.BridgeheadUnpackedVersion.Results.ValueOrDefault))
				{
					return new Result<bool>(AnalysisHelpers.VersionCompare(this.BridgeheadUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) < 0);
				}
				return new Result<bool>(false);
			});
			this.BridgeheadMinVersionCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.MinVersionCheck).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.BridgeheadUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.BridgeheadConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.BridgeheadUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) >= 0 && this.BridgeheadUnpackedVersion.Results.Value == this.BridgeheadConfiguredVersion.Results.Value && !this.PreviousBuildDetected.Results.ValueOrDefault);
				}
				return new RuleResult(this.PreviousBuildDetected.Results.ValueOrDefault);
			});
			this.BridgeheadUpgradeMinVersionBlock = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Bridgehead).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.UpgradeMinVersionBlock).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.BridgeheadUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.BridgeheadConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.BridgeheadUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) > 0 && this.BridgeheadUnpackedVersion.Results.Value == this.BridgeheadConfiguredVersion.Results.Value);
				}
				return new RuleResult(false);
			});
			this.PreviousBuildDetected = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).SetValue((Result<object> x) => new Result<bool>(this.MailboxPreviousBuild.Results.ValueOrDefault || this.UnifiedMessagingPreviousBuild.Results.ValueOrDefault || this.BridgeheadPreviousBuild.Results.ValueOrDefault));
			this.GatewayConfiguredVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string value = null;
				try
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole", "ConfiguredVersion");
				}
				catch (FailureException)
				{
				}
				if (string.IsNullOrEmpty(value))
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\GatewayRole", "ConfiguredVersion");
				}
				return new Result<string>(value);
			});
			this.GatewayUnpackedVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string value = null;
				try
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole", "UnpackedVersion");
				}
				catch (FailureException)
				{
				}
				if (string.IsNullOrEmpty(value))
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\GatewayRole", "UnpackedVersion");
				}
				return new Result<string>(value);
			});
			this.GatewayMinVersionCheck = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.MinVersionCheck).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.GatewayConfiguredVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.GatewayUnpackedVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.GatewayUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) >= 0 && this.GatewayUnpackedVersion.Results.Value == this.GatewayConfiguredVersion.Results.Value);
				}
				return new RuleResult(false);
			});
			this.GatewayUpgrade605Block = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.GatewayUpgrade605Block).Condition((Result<object> x) => new RuleResult(AnalysisHelpers.VersionCompare(this.GatewayUnpackedVersion.Results.Value, "8.0.605.11") < 0 && AnalysisHelpers.VersionCompare(this.globalParameters.ExchangeVersion.ToString(), "8.0.606.0") >= 0));
			this.GatewayUpgradeMinVersionBlock = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.Gateway).Mode(SetupMode.Upgrade).Error<RuleBuilder<object>>().Message((Result x) => Strings.UpgradeMinVersionBlock).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.GatewayUnpackedVersion.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.GatewayConfiguredVersion.Results.ValueOrDefault))
				{
					return new RuleResult(AnalysisHelpers.VersionCompare(this.GatewayUnpackedVersion.Results.Value, this.globalParameters.ExchangeVersion.ToString()) > 0 && this.GatewayUnpackedVersion.Results.Value == this.GatewayConfiguredVersion.Results.Value);
				}
				return new RuleResult(false);
			});
			this.AdminToolsInstallation = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools", null)));
			this.Exchange2013AnyOnExchange2007Server = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.Exchange2013AnyOnExchange2007or2010Server).Condition(delegate(Result<object> x)
			{
				string[] second = new string[]
				{
					"AdminTools",
					"HubTransportRole",
					"MailboxRole",
					"UnifiedMessagingRole",
					"ClientAccessRole",
					"Hygiene"
				};
				string[] first = ((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Exchange\\v8.0", null)) ?? new string[0];
				return new RuleResult(first.Intersect(second).Any((string r) => !string.IsNullOrEmpty((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, string.Format("{0}\\{1}", "SOFTWARE\\Microsoft\\Exchange\\v8.0", r), "UnpackedVersion"))));
			});
			this.Exchange2013AnyOnExchange2010Server = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install).Error<RuleBuilder<object>>().Message((Result x) => Strings.Exchange2013AnyOnExchange2007or2010Server).Condition(delegate(Result<object> x)
			{
				string[] second = new string[]
				{
					"AdminTools",
					"HubTransportRole",
					"MailboxRole",
					"UnifiedMessagingRole",
					"ClientAccessRole",
					"Hygiene"
				};
				string[] first = ((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v14", null)) ?? new string[0];
				return new RuleResult(first.Intersect(second).Any((string r) => !string.IsNullOrEmpty((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, string.Format("{0}\\{1}", "SOFTWARE\\Microsoft\\ExchangeServer\\v14", r), "UnpackedVersion"))));
			});
			this.PendingFileRenames = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string[] value = null;
				object registryKeyValue = base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Control\\Session Manager", "PendingFileRenameOperations");
				if (registryKeyValue != null)
				{
					string text = registryKeyValue as string;
					if (text == null)
					{
						value = (registryKeyValue as string[]);
					}
					else if (text != string.Empty)
					{
						value = new string[]
						{
							text
						};
					}
				}
				return new Result<string[]>(value);
			});
			this.UpdateNeedsReboot = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Condition(delegate(Result<object> x)
			{
				string value = string.Empty;
				try
				{
					value = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Updates\\UpdateExeVolatile", "Flags");
					if (!string.IsNullOrEmpty(value) && Convert.ToInt32(value) != 0)
					{
						return new RuleResult(true);
					}
				}
				catch (FailureException)
				{
				}
				return new RuleResult(false);
			});
			this.PendingRebootWindowsComponents = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PendingRebootWindowsComponents).Condition(delegate(Result<object> x)
			{
				try
				{
					string[] source = (string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing", null);
					if (source.Contains("RebootPending"))
					{
						return new RuleResult(true);
					}
				}
				catch (FailureException)
				{
				}
				return new RuleResult(false);
			});
			this.DST2007Enabled = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<bool>(base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\Pacific Standard Time", "TZI").ToString().ToUpper() == "E001000000000000C4FFFFFF00000B0000000100020000000000000000000300000002000200000000000000"));
			this.DynamicDSTKey = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\Pacific Standard Time\\Dynamic DST", null)));
			this.HTTPActivation = Setting<string[]>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string[]>((string[])base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0\\Setup\\Windows Communication Foundation\\HTTPActivation", null)));
			this.NNTPSvcStartMode = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\NntpSvc", "Start")));
			this.ProgramFilePath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion", "ProgramFilesDir").ToString()));
			this.FrameworkPath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\.NETFramework", "InstallRoot").ToString()));
			this.AdamDataPath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MsExchange", "DataFilesPath").ToString()));
			this.ConfigDCHostName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>((string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\MSExchange ADAccess\\Instance0", "ConfigDCHostName")));
			this.ConfigDCHostNameMismatch = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ConfigDCHostNameMismatch(this.globalParameters.DomainController, this.ConfigDCHostName.Results.Value)).Condition((Result<object> x) => new RuleResult(!string.IsNullOrEmpty(this.ConfigDCHostName.Results.ValueOrDefault) && this.ConfigDCHostName.Results.Value.ToString().ToLower() != this.globalParameters.DomainController.ToLower()));
			this.IIS7ManagementConsole = Setting<int?>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).SetValue((Result<object> x) => new Result<int?>((int?)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "ManagementConsole")));
			this.LonghornIIS7ManagementConsoleInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComponentIsRequired("IIS Management Console")).Condition(delegate(Result<object> x)
			{
				if (this.SetupRoles.Results.Count((Result<string> w) => w.Value.Equals(SetupRole.AdminTools.ToString(), StringComparison.InvariantCultureIgnoreCase)) > 0 || this.Windows8Version.Results.ValueOrDefault)
				{
					return new RuleResult(this.IIS7ManagementConsole.Results.ValueOrDefault == null || this.IIS7ManagementConsole.Results.Value == 0);
				}
				return new RuleResult(false);
			});
			this.WindowsInstallerServiceDisabledOrNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.WindowsInstallerServiceDisabledOrNotInstalled).Condition((Result<object> x) => new RuleResult(this.WindowsInstallerServiceStartMode.Results.ValueOrDefault == 0 || this.WindowsInstallerServiceStartMode.Results.ValueOrDefault == 4));
			this.WindowsInstallerServiceStartMode = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<int>((int)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\msiserver", "Start")));
			this.WinRMServiceDisabledOrNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.WinRMDisabledOrNotInstalled).Condition((Result<object> x) => new RuleResult(this.WinRMServiceStartMode.Results.ValueOrDefault == 0 || this.WinRMServiceStartMode.Results.ValueOrDefault == 4));
			this.WinRMServiceStartMode = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).SetValue((Result<object> x) => new Result<int>((int)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "System\\CurrentControlSet\\Services\\winrm", "Start")));
			this.MinimumFrameworkNotInstalled = Rule.Build().AsRootRule().In(this).AsSync().Role(SetupRole.All).Mode(SetupMode.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.MinimumFrameworkNotInstalled).Condition((Result<object> x) => new RuleResult(this.ClrReleaseNumber.Results.ValueOrDefault == null || this.ClrReleaseNumber.Results.ValueOrDefault.Value < 461808));
		}

		private void CreateWmiPrereqProperties()
		{
			this.W2K8R2PrepareSchemaLdifdeNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.W2K8R2PrepareSchemaLdifdeNotInstalled).Condition((Result<object> x) => new RuleResult(!this.PendingRebootWindowsComponents.Results.Value && this.PrepareSchema.Results.Value && this.FileVersionLdifde.Results.Value.CompareTo(new Version()) == 0));
			this.W2K8R2PrepareAdLdifdeNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.W2K8R2PrepareAdLdifdeNotInstalled).Condition((Result<object> x) => new RuleResult(!this.PendingRebootWindowsComponents.Results.Value && this.PrepareOrganization.Results.Value && this.FileVersionLdifde.Results.Value.CompareTo(new Version()) == 0));
			this.SiteName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(base.Providers.NativeMethodProvider.GetSiteName(string.Empty)));
			this.LonghornWmspdmoxNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.InstallViaServerManager("Windows Media Audio Voice Codec")).Condition((Result<object> x) => new RuleResult(!this.PendingRebootWindowsComponents.Results.Value && (this.FileVersionWmspdmod.Results.Value.CompareTo(new Version()) == 0 || (this.FileVersionWmspdmod.Results.Value.ToString(3).Equals("10.00.00", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionWmspdmod.Results.Value.Revision < 3804) || this.FileVersionWmspdmoe.Results.Value.CompareTo(new Version()) == 0 || (this.FileVersionWmspdmoe.Results.Value.ToString(3).Equals("10.00.00", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionWmspdmoe.Results.Value.Revision < 3804))));
			this.Exchange2000or2003PresentInOrg = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.Exchange2000or2003PresentInOrg).Condition((Result<object> x) => new RuleResult(this.Exchange200x.Results.Any((Result<bool> w) => w.Value)));
			this.ExchangeSerialNumber = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetMultipleValues(delegate(Result<object> x)
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"serialNumber"
					}, "objectClass=msExchExchangeServer", SearchScope.Subtree);
					if (searchResultCollection != null)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["serialNumber"];
							foreach (object obj2 in resultPropertyValueCollection)
							{
								list.Add(obj2.ToString());
							}
						}
					}
				}
				return from w in list
				select new Result<string>(w);
			});
			this.Exchange12 = Setting<bool>.Build().WithParent<string>(() => this.ExchangeSerialNumber).In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue((Result<string> x) => new Result<bool>(x.Value.Contains("Version 8")));
			this.Exchange200x = Setting<bool>.Build().WithParent<string>(() => this.ExchangeSerialNumber).In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue((Result<string> x) => new Result<bool>(x.Value.Contains("Version 6")));
			this.RebootPending = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.PendingReboot).Condition((Result<object> x) => new RuleResult((this.PendingFileRenames.Results.ValueOrDefault != null && this.PendingFileRenames.Results.ValueOrDefault.Length > 0) || this.UpdateNeedsReboot.Results.Value));
			this.Win7RpcHttpAssocCookieGuidUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox | SetupRole.ClientAccess | SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.Win7RpcHttpAssocCookieGuidUpdateNotInstalled).Condition((Result<object> x) => new RuleResult(this.FileVersionRpcHttp.Results.Value.CompareTo(new Version()) != 0 && ((this.WindowsBuild.Results.Value.Equals("7600", StringComparison.InvariantCultureIgnoreCase) && (this.FileVersionRpcHttp.Results.Value.CompareTo(new Version("6.1.7600.21085")) < 0 || this.FileVersionRpcRT4.Results.Value.CompareTo(new Version("6.1.7600.21085")) < 0)) || (this.WindowsBuild.Results.Value.Equals("7601", StringComparison.InvariantCultureIgnoreCase) && (this.FileVersionRpcHttp.Results.Value.CompareTo(new Version("6.1.7601.21855")) < 0 || this.FileVersionRpcRT4.Results.Value.CompareTo(new Version("6.1.7601.21855")) < 0)))));
			this.SearchFoundationAssemblyLoaderKBNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.SearchFoundationAssemblyLoaderKBNotInstalled).Condition((Result<object> x) => new RuleResult(this.FileVersionKernel32.Results.Value.CompareTo(new Version()) != 0 && ((this.WindowsBuild.Results.Value.Equals("7600", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionKernel32.Results.Value.CompareTo(new Version("6.1.7600.16816")) < 0) || (this.WindowsBuild.Results.Value.Equals("7601", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionKernel32.Results.Value.CompareTo(new Version("6.1.7601.17617")) < 0))));
			this.Win2k12UrefsUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).Warning<RuleBuilder<object>>().Message((Result x) => Strings.Win2k12UrefsUpdateNotInstalled).Condition((Result<object> x) => new RuleResult(this.FileVersionUrefs.Results.Value.CompareTo(new Version()) != 0 && this.WindowsBuild.Results.Value.Equals("9200", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionUrefs.Results.Value.CompareTo(new Version("6.2.9200.20810")) < 0));
			this.Win2k12RefsUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).Warning<RuleBuilder<object>>().Message((Result x) => Strings.Win2k12RefsUpdateNotInstalled).Condition((Result<object> x) => new RuleResult(this.FileVersionRefs.Results.Value.CompareTo(new Version()) != 0 && this.WindowsBuild.Results.Value.Equals("9200", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionRefs.Results.Value.CompareTo(new Version("6.2.9200.20838")) < 0));
			this.Win2k12RollupUpdateNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox).Warning<RuleBuilder<object>>().Message((Result x) => Strings.Win2k12RollupUpdateNotInstalled).Condition((Result<object> x) => new RuleResult(this.FileVersionDiscan.Results.Value.CompareTo(new Version()) != 0 && this.WindowsBuild.Results.Value.Equals("9200", StringComparison.InvariantCultureIgnoreCase) && this.FileVersionDiscan.Results.Value.CompareTo(new Version("6.2.9200.16548")) < 0));
			this.UnifiedMessagingRoleNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.UnifiedMessaging).Error<RuleBuilder<object>>().Message((Result x) => Strings.UnifiedMessagingRoleNotInstalled).Condition((Result<object> x) => new RuleResult(!this.UnifiedMessagingRoleInstalled.Results.Value));
			this.BridgeheadRoleNotInstalled = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.BridgeheadRoleNotInstalled).Condition((Result<object> x) => new RuleResult(!this.BridgeheadRoleInstalled.Results.Value));
			this.EventSystemStarted = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service WHERE Name='EventSystem'")[0].TryGetValue("Started", out obj))
				{
					return new Result<bool>(bool.Parse(obj.ToString()));
				}
				return new Result<bool>(false);
			});
			this.EventSystemStopped = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.ClientAccess | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.EventSystemStopped).Condition((Result<object> x) => new RuleResult(!this.EventSystemStarted.Results.Value));
			this.MailboxRoleAlreadyExists = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).Error<RuleBuilder<object>>().Message((Result x) => Strings.MailboxRoleAlreadyExists).Condition((Result<object> x) => new RuleResult(this.ServerAlreadyExists.Results.Value && this.MailboxRoleInstalled.Results.Value));
			this.ClientAccessRoleAlreadyExists = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.ClientAccess).Error<RuleBuilder<object>>().Message((Result x) => Strings.ClientAccessRoleAlreadyExists).Condition((Result<object> x) => new RuleResult(this.ServerAlreadyExists.Results.Value && this.ClientAccessRoleInstalled.Results.Value));
			this.UnifiedMessagingRoleAlreadyExists = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.UnifiedMessaging).Error<RuleBuilder<object>>().Message((Result x) => Strings.UnifiedMessagingRoleAlreadyExists).Condition((Result<object> x) => new RuleResult(this.ServerAlreadyExists.Results.Value && this.UnifiedMessagingRoleInstalled.Results.Value));
			this.BridgeheadRoleAlreadyExists = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Bridgehead).Error<RuleBuilder<object>>().Message((Result x) => Strings.BridgeheadRoleAlreadyExists).Condition((Result<object> x) => new RuleResult(this.ServerAlreadyExists.Results.Value && this.BridgeheadRoleInstalled.Results.Value));
			this.CafeRoleAlreadyExists = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Cafe).Error<RuleBuilder<object>>().Message((Result x) => Strings.CafeRoleAlreadyExists).Condition((Result<object> x) => new RuleResult(this.ServerAlreadyExists.Results.Value && this.CafeRoleInstalled.Results.Value));
			this.ServerWinWebEdition = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.OSWebEditionValidator("Exchange 15")).Condition((Result<object> x) => new RuleResult((this.OSProductSuite.Results.Value & 1024U) != 0U));
			this.BridgeheadRoleNotPresentInSite = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).Warning<RuleBuilder<object>>().Message((Result x) => Strings.BridgeheadRoleNotPresentInSite(this.SiteName.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value;
				if (string.IsNullOrEmpty(this.BridgeheadRoleInCurrentADSite.Results.ValueOrDefault))
				{
					value = (this.SetupRoles.Results.Count((Result<string> w) => w.Value.Contains("Bridgehead")) == 0);
				}
				else
				{
					value = false;
				}
				return new RuleResult(value);
			});
			this.ClientAccessRoleNotPresentInSite = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).Warning<RuleBuilder<object>>().Message((Result x) => Strings.ClientAccessRoleNotPresentInSite(this.SiteName.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value;
				if (string.IsNullOrEmpty(this.ClientAccessRoleInCurrentADSite.Results.ValueOrDefault))
				{
					value = (this.SetupRoles.Results.Count((Result<string> w) => w.Value.Contains("ClientAccess")) == 0);
				}
				else
				{
					value = false;
				}
				return new RuleResult(value);
			});
			this.BridgeheadRoleInCurrentADSite = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchServerSite=cn={0},cn=Sites,{1})(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32))", this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), SearchScope.OneLevel);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ClientAccessRoleInCurrentADSite = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.OrgDistinguishedName.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Servers,cn=Exchange Administrative Group (FYDIBOHF23SPDLT),cn=Administrative Groups, {1}", this.GlobalCatalog.Results.Value, this.OrgDistinguishedName.Results.Value), new string[]
					{
						"cn"
					}, string.Format("(&(objectClass=msExchExchangeServer)(msExchServerSite=cn={0},cn=Sites,{1})(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=4))", this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), SearchScope.OneLevel);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.WindowsPath = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string text = (string)base.Providers.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "SystemRoot");
				if (!AnalysisHelpers.IsNullOrEmpty(text))
				{
					return new Result<string>(text);
				}
				return new Result<string>(string.Empty);
			});
			this.FileVersionNtoskrnl = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\ntoskrnl.exe'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionLdifde = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\ldifde.exe'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionWmspdmod = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\wmspdmod.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionWmspdmoe = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\wmspdmoe.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionMSXML6 = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.UnifiedMessaging).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\msxml6.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionSecProc = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\secproc.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionRmActivate = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\RmActivate.exe'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionRpcRT4 = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\rpcrt4.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionRpcHttp = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\rpchttp.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionRpcProxy = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\RpcProxy\\rpcproxy.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionLbService = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\RpcProxy\\lbservice.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionTCPIPSYS = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\drivers\\tcpip.sys'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionAdsiis = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.ClientAccess).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\inetsrv\\adsiis.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionIisext = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.ClientAccess).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\system32\\inetsrv\\iisext.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					return new Result<Version>(new Version(obj.ToString()));
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionMSCorLib = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.UnifiedMessaging).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name = '{0}v4.0.30319\\mscorlib.dll'", this.FrameworkPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionSystemServiceModel = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\v4.0.30319\\System.ServiceModel.dll'", this.FrameworkPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionSystemWeb = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name = '{0}v4.0.30319\\System.Web.dll'", this.FrameworkPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionSystemWebServices = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}v4.0.30319\\System.Web.Services.dll'", this.FrameworkPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionKernel32 = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\Kernel32.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionUrefs = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\uReFS.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionRefs = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\Drivers\\refs.sys'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionDiscan = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\discan.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionAtl110 = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Gateway).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\atl110.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.FileVersionMsvcr120 = Setting<Version>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM CIM_Datafile WHERE Name='{0}\\System32\\msvcr120.dll'", this.WindowsPath.Results.Value))[0].TryGetValue("Version", out obj))
				{
					string text = AnalysisHelpers.Replace(obj.ToString(), "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$", "$1");
					if (!string.IsNullOrEmpty(text))
					{
						return new Result<Version>(new Version(text));
					}
				}
				return new Result<Version>(new Version());
			});
			this.MSDTCStarted = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service WHERE Name='MSDTC'")[0].TryGetValue("Started", out obj))
				{
					return new Result<bool>(bool.Parse(obj.ToString()));
				}
				return new Result<bool>(false);
			});
			this.MSDTCStopped = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.Uninstall).Role(SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.MSDTCStopped).Condition((Result<object> x) => new RuleResult(!this.MSDTCStarted.Results.Value));
			this.MpsSvcStarted = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service WHERE Name='MpsSvc'")[0].TryGetValue("Started", out obj))
				{
					return new Result<bool>(bool.Parse(obj.ToString()));
				}
				return new Result<bool>(false);
			});
			this.MpsSvcStopped = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.FrontendTransport).Warning<RuleBuilder<object>>().Message((Result x) => Strings.MpsSvcStopped).Condition((Result<object> x) => new RuleResult(!this.MpsSvcStarted.Results.Value));
			this.ADAMSvcStopped = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADAMSvcStopped).Condition(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service WHERE Name='ADAM_MSExchange'")[0].TryGetValue("Started", out obj))
				{
					return new RuleResult(!bool.Parse(obj.ToString()));
				}
				return new RuleResult(false);
			});
			this.NetTcpPortSharingStartMode = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service WHERE Name='NetTcpPortSharing'")[0].TryGetValue("StartMode", out obj))
				{
					return new Result<string>(obj.ToString());
				}
				return new Result<string>(string.Empty);
			});
			this.NetTcpPortSharingSvcNotAuto = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade).Role(SetupRole.ClientAccess | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.NetTcpPortSharingSvcNotAuto).Condition((Result<object> x) => new RuleResult(!this.NetTcpPortSharingStartMode.Results.Value.Equals("Auto", StringComparison.InvariantCultureIgnoreCase)));
			this.AddressWidth = Setting<ushort>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Processor")[0].TryGetValue("AddressWidth", out obj))
				{
					return new Result<ushort>((ushort)obj);
				}
				return new Result<ushort>(0);
			});
			this.AddressWidth32Bit = Setting<bool>.Build().AsRootSetting().In(this).AsSync().SetValue((Result<object> x) => new Result<bool>(this.AddressWidth.Results.Value == 32));
			this.AddressWidth64Bit = Setting<bool>.Build().AsRootSetting().In(this).AsSync().SetValue((Result<object> x) => new Result<bool>(this.AddressWidth.Results.Value == 64));
			this.NicCaption = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetMultipleValues((Result<object> x) => base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_NetworkAdapter WHERE AdapterTypeID='0'").Select(delegate(Dictionary<string, object> w)
			{
				object obj;
				if (w.TryGetValue("Caption", out obj))
				{
					return new Result<string>(obj.ToString());
				}
				return new Result<string>(string.Empty);
			}));
			this.NicConfiguration = Setting<Dictionary<string, object>>.Build().WithParent<string>(() => this.NicCaption).In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue((Result<string> x) => new Result<Dictionary<string, object>>(base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Caption='{0}'", x.Value))[0]));
			this.IPAddress = Setting<string>.Build().WithParent<Dictionary<string, object>>(() => this.NicConfiguration).In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetMultipleValues(delegate(Result<Dictionary<string, object>> x)
			{
				string[] source = new string[0];
				object obj;
				if (x.Value.TryGetValue("IPAddress", out obj) && obj != null)
				{
					source = (string[])obj;
				}
				return from w in source
				select new Result<string>(w);
			});
			this.IPv4Address = Setting<string>.Build().WithParent<string>(() => this.IPAddress).In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue((Result<string> x) => new Result<string>(AnalysisHelpers.Match("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}).*$", new string[]
			{
				x.Value
			}) ? x.Value : string.Empty));
			this.IPv6Enabled = Setting<bool>.Build().WithParent<string>(() => this.IPAddress).In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue((Result<string> x) => new Result<bool>(x.Value.Count((char w) => w.Equals(':')) > 0));
			this.DnsAddress = Setting<string>.Build().WithParent<Dictionary<string, object>>(() => this.NicConfiguration).In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetMultipleValues(delegate(Result<Dictionary<string, object>> x)
			{
				string[] source = new string[0];
				object obj;
				if (x.Value.TryGetValue("DNSServerSearchOrder", out obj) && obj != null)
				{
					source = AnalysisHelpers.Replace((string[])obj, "^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}).*$", "$1");
				}
				return from w in source
				select new Result<string>(w);
			});
			this.PrimaryDns = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!AnalysisHelpers.IsNullOrEmpty(this.DnsAddress.Results))
				{
					return new Result<string>(this.DnsAddress.Results[0].Value);
				}
				return new Result<string>(string.Empty);
			});
			this.PrimaryDNSPortAvailable = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				Dictionary<string, List<string>> commands = new Dictionary<string, List<string>>
				{
					{
						"PrimaryDNSPort",
						new List<string>
						{
							"53",
							string.Empty,
							string.Empty
						}
					}
				};
				Dictionary<string, object[]> dictionary = base.Providers.ManagedMethodProvider.PortAvailable(this.PrimaryDns.Results.Value, commands);
				object[] array;
				if (!AnalysisHelpers.IsNullOrEmpty(dictionary) && dictionary.TryGetValue("PrimaryDNSPort", out array))
				{
					return new Result<bool>(array != null && array.Length > 1 && bool.Parse(array[1] as string));
				}
				return new Result<bool>(false);
			});
			this.PrimaryDNSTestFailed = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).Warning<RuleBuilder<object>>().Message((Result x) => Strings.PrimaryDNSTestFailed(this.PrimaryDns.Results.Value)).Condition(delegate(Result<object> x)
			{
				bool value;
				if (!this.PrimaryDNSPortAvailable.Results.Value)
				{
					value = (this.IPv6Enabled.Results.Count((Result<bool> w) => w.Value) <= 0);
				}
				else
				{
					value = false;
				}
				return new RuleResult(value);
			});
			this.HostRecord = Setting<Dictionary<string, object[]>>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.All).SetValue((Result<object> x) => new Result<Dictionary<string, object[]>>(base.Providers.ManagedMethodProvider.CheckDNS(this.PrimaryDns.Results.Value, this.ComputerNameDnsFullyQualified.Results.Value)));
			this.HostRecordMissing = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Warning<RuleBuilder<object>>().Message((Result x) => Strings.MissingDNSHostRecord(this.PrimaryDns.Results.Value)).Condition((Result<object> x) => new RuleResult(((string)this.HostRecord.Results.Value["A"].GetValue(3)).Equals(string.Format("DNS Query Result = {0};", this.PrimaryDns.Results.Value), StringComparison.InvariantCultureIgnoreCase)));
			this.DebugVersion = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_OperatingSystem")[0].TryGetValue("Debug", out obj))
				{
					return new Result<bool>(bool.Parse(obj.ToString()));
				}
				return new Result<bool>(false);
			});
			this.OSCheckedBuild = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Warning<RuleBuilder<object>>().Message((Result x) => Strings.OSCheckedBuild).Condition((Result<object> x) => new RuleResult(this.DebugVersion.Results.Value));
			this.OSProductSuite = Setting<uint>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_OperatingSystem")[0].TryGetValue("OSProductSuite", out obj))
				{
					return new Result<uint>((uint)obj);
				}
				return new Result<uint>(0U);
			});
			this.DomainRole = Setting<ushort>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue((Result<object> x) => new Result<ushort>((ushort)base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_ComputerSystem")[0]["DomainRole"]));
			this.ComputerNotPartofDomain = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComputerNotPartofDomain).Condition((Result<object> x) => new RuleResult(this.DomainRole.Results.Value == 0 || this.DomainRole.Results.Value == 2));
			this.LocalComputerIsDomainController = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue((Result<object> x) => new Result<bool>(this.DomainRole.Results.Value == 4 || this.DomainRole.Results.Value == 5));
			this.WarningInstallExchangeRolesOnDomainController = Rule.Build().AsRootRule().In(this).AsAsync().Mode(SetupMode.Install | SetupMode.DisasterRecovery).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Warning<RuleBuilder<object>>().Message((Result x) => Strings.InstallExchangeRolesOnDomainController).Condition((Result<object> x) => new RuleResult(this.LocalComputerIsDomainController.Results.Value));
			this.ADSplitPermissionMode = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).SetValue((Result<object> x) => new Result<bool>(!string.IsNullOrEmpty(this.EWPDn.Results.Value) && !string.IsNullOrEmpty(this.ETSDn.Results.Value) && !this.ETSIsMemberOfEWP.Results.Value));
			this.InstallOnDCInADSplitPermissionMode = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.InstallOnDCInADSplitPermissionMode).Condition((Result<object> x) => new RuleResult(this.LocalComputerIsDomainController.Results.Value && (this.ActiveDirectorySplitPermissions.Results.ValueOrDefault || this.ADSplitPermissionMode.Results.ValueOrDefault)));
			this.SetADSplitPermissionWhenExchangeServerRolesOnDC = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.SetADSplitPermissionWhenExchangeServerRolesOnDC).Condition((Result<object> x) => new RuleResult(this.ActiveDirectorySplitPermissions.Results.Value && this.ExchangeServerRolesOnDomainController.Results.Value > 0));
			this.LocalServerName = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_ComputerSystem")[0].TryGetValue("Name", out obj))
				{
					return new Result<string>(obj.ToString());
				}
				return new Result<string>(string.Empty);
			});
			this.ServerNameNotValid = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidDNSDomainName).Condition((Result<object> x) => new RuleResult(!Regex.IsMatch(this.LocalServerName.Results.Value, "^[A-Za-z0-9\\-]*$")));
			this.LocalComputerIsDCInChildDomain = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.DisasterRecovery).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.LocalComputerIsDCInChildDomain).Condition((Result<object> x) => new RuleResult((this.DomainRole.Results.Value == 4 || this.DomainRole.Results.Value == 5) && !this.IsGlobalCatalogReady.Results.Value && !this.RootNamingContext.Results.Value.Equals(this.ComputerDomainDN.Results.Value, StringComparison.InvariantCultureIgnoreCase)));
			this.CurrentLogOn = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue((Result<object> x) => new Result<string>(base.Providers.ManagedMethodProvider.GetUserNameEx(ValidationConstant.ExtendedNameFormat.NameSamCompatible)));
			this.LoggedOntoDomain = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.NotLoggedOntoDomain).Condition((Result<object> x) => new RuleResult(this.CurrentLogOn.Results.Value.StartsWith(this.LocalServerName.Results.Value + "\\", StringComparison.InvariantCultureIgnoreCase)));
			this.MsExServicesConfigDNOtherWellKnownObjects = Setting<ResultPropertyCollection>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn=Microsoft Exchange,cn=Services, {1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"otherWellKnownObjects"
					}, null, SearchScope.Base);
					if (searchResultCollection != null)
					{
						using (IEnumerator enumerator = searchResultCollection.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SearchResult searchResult = (SearchResult)enumerator.Current;
								return new Result<ResultPropertyCollection>(searchResult.Properties);
							}
						}
					}
				}
				return new Result<ResultPropertyCollection>(null);
			});
			this.MsExServicesConfigDNOtherWellKnownObjectsEWPDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (this.MsExServicesConfigDNOtherWellKnownObjects.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MsExServicesConfigDNOtherWellKnownObjects.Results.Value["otherWellKnownObjects"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string value = AnalysisHelpers.Replace(obj.ToString(), "(^B:32:4C17D0117EBE6642AFAEE03BC66D381F:(?'dn'.*))?.*$", "${dn}");
						if (!string.IsNullOrEmpty(value))
						{
							return new Result<string>(value);
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.MsExServicesConfigDNOtherWellKnownObjectsETSDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (this.MsExServicesConfigDNOtherWellKnownObjects.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.MsExServicesConfigDNOtherWellKnownObjects.Results.Value["otherWellKnownObjects"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string value = AnalysisHelpers.Replace(obj.ToString(), "(^B:32:EA876A58DB6DD04C9006939818F800EB:(?'dn'.*))?.*$", "${dn}");
						if (!string.IsNullOrEmpty(value))
						{
							return new Result<string>(value);
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.EWPDn = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MsExServicesConfigDNOtherWellKnownObjectsEWPDN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.MsExServicesConfigDNOtherWellKnownObjectsEWPDN.Results.Value), new string[]
					{
						"distinguishedName"
					}, null, SearchScope.Base);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ETSDn = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.MsExServicesConfigDNOtherWellKnownObjectsETSDN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.MsExServicesConfigDNOtherWellKnownObjectsETSDN.Results.Value), new string[]
					{
						"distinguishedName"
					}, null, SearchScope.Base);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ETSIsMemberOfEWP = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.EWPDn.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.MsExServicesConfigDNOtherWellKnownObjectsETSDN.Results.Value), new string[]
					{
						"memberOf"
					}, null, SearchScope.Base);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["memberOf"];
						foreach (object obj2 in resultPropertyValueCollection)
						{
							if (obj2.ToString().Contains(this.EWPDn.Results.Value))
							{
								return new Result<bool>(true);
							}
						}
					}
				}
				return new Result<bool>(false);
			});
			this.DomainControllerCN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Global).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"cn"
					}, "(&(objectClass=server)(dNSHostName=*))", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["cn"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ExchangeServerRolesOnDomainController = Setting<int>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.DomainControllerCN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"msExchCurrentServerRoles"
					}, string.Format("(&(objectCategory=msExchExchangeServer)(cn={0}))", this.DomainControllerCN.Results.Value), SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["msExchCurrentServerRoles"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								int num = int.Parse(obj2.ToString());
								return new Result<int>((num & 2) | (num & 4) | (num & 16) | (num & 32));
							}
						}
					}
				}
				return new Result<int>(0);
			});
			this.NtMixedDomainComputerDomainDN = Setting<int>.Build().AsRootSetting().In(this).AsSync().Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Mode(SetupMode.All).SetValue(delegate(Result<object> x)
			{
				if (this.LocalComputerDomainDN.Results.ValueOrDefault != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = this.LocalComputerDomainDN.Results.Value["nTMixedDomain"];
					using (IEnumerator enumerator = resultPropertyValueCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							return new Result<int>((int)obj);
						}
					}
				}
				return new Result<int>(0);
			});
			this.LocalDomainModeMixed = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Global).Error<RuleBuilder<object>>().Message((Result x) => Strings.LocalDomainMixedMode(this.ComputerDomainDN.Results.Value, "Exchange 15")).Condition((Result<object> x) => new RuleResult(this.PrepareDomain.Results.ValueOrDefault != null && this.PrepareDomain.Results.Value.Equals("F63C3A12-7852-4654-B208-125C32EB409A", StringComparison.InvariantCultureIgnoreCase) && this.NtMixedDomainComputerDomainDN.Results.Value == 1));
			this.DomainPrepRequired = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.DomainPrepRequired).Condition((Result<object> x) => new RuleResult(!this.LocalDomainIsPrepped.Results.Value && !this.ComputerDomainDN.Results.Value.Equals(this.RootNamingContext.Results.Value, StringComparison.InvariantCultureIgnoreCase)));
			this.LocalDomainIsPrepped = Setting<bool>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.DomainController.Results.ValueOrDefault) && !string.IsNullOrEmpty(this.ComputerDomainDN.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.DomainController.Results.Value, this.ComputerDomainDN.Results.Value), new string[]
					{
						"objectVersion"
					}, "(objectCategory=msExchSystemObjectsContainer)", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["objectVersion"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<bool>((int)obj2 >= 12433);
							}
						}
					}
				}
				return new Result<bool>(false);
			});
			this.ReadOnlyDC = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn={1},cn=Servers,cn={2},cn=Sites,{3}", new object[]
					{
						this.GlobalCatalog.Results.Value,
						this.ShortServerName.Results.Value,
						this.SiteName.Results.Value,
						this.ConfigurationNamingContext.Results.Value
					}), new string[]
					{
						"distinguishedName"
					}, "(objectCategory=nTDSDSARO)", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ComputerRODC = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.ComputerRODC).Condition((Result<object> x) => new RuleResult(!string.IsNullOrEmpty(this.ReadOnlyDC.Results.ValueOrDefault)));
			this.ServerRef = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn={1},cn=Sites,{2}", this.GlobalCatalog.Results.Value, this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"serverReference"
					}, "(&(objectClass=server)(dNSHostName=*))", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["serverReference"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.ADServerDN = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ConfigurationNamingContext.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/cn={1},cn=Sites,{2}", this.GlobalCatalog.Results.Value, this.SiteName.Results.Value, this.ConfigurationNamingContext.Results.Value), new string[]
					{
						"distinguishedName"
					}, "(&(objectClass=server)(dNSHostName=*))", SearchScope.Subtree);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["distinguishedName"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.OperatingSystemVersion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ServerRef.Results.ValueOrDefault))
				{
					SearchResultCollection searchResultCollection = base.Providers.ADDataProvider.Run(false, string.Format("LDAP://{0}/{1}", this.GlobalCatalog.Results.Value, this.ServerRef.Results.Value), new string[]
					{
						"operatingSystemVersion"
					}, string.Empty, SearchScope.Base);
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["operatingSystemVersion"];
						using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								return new Result<string>(obj2.ToString());
							}
						}
					}
				}
				return new Result<string>(string.Empty);
			});
			this.InvalidADSite = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).Error<RuleBuilder<object>>().Message((Result x) => Strings.InvalidADSite).Condition((Result<object> x) => new RuleResult(string.IsNullOrEmpty(this.SiteName.Results.ValueOrDefault)));
			this.FirstSGFilesExist = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).Error<RuleBuilder<object>>().Message((Result x) => Strings.SGFilesExist(string.Format("{0}\\Mailbox\\First Storage Group", this.TargetDir.Results.Value))).Condition((Result<object> x) => new RuleResult(this.FirstSGFiles.Results.Count<Result<string>>() > 0));
			this.SecondSGFilesExist = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).Error<RuleBuilder<object>>().Message((Result x) => Strings.SGFilesExist(string.Format("{0}\\Mailbox\\Second Storage Group", this.TargetDir.Results.Value))).Condition((Result<object> x) => new RuleResult(this.SecondSGFiles.Results.Count<Result<string>>() > 0));
			this.FirstSGFiles = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).SetMultipleValues(delegate(Result<object> x)
			{
				string path = string.Format("{0}\\Mailbox\\First Storage Group", this.TargetDir.Results.Value);
				string[] source = new string[0];
				if (Directory.Exists(path))
				{
					source = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
				}
				return from w in source
				select new Result<string>(w);
			});
			this.SecondSGFiles = Setting<string>.Build().AsRootSetting().In(this).AsAsync().Mode(SetupMode.Install).Role(SetupRole.Mailbox).SetMultipleValues(delegate(Result<object> x)
			{
				string path = string.Format("{0}\\Mailbox\\Second Storage Group", this.TargetDir.Results.Value);
				string[] source = new string[0];
				if (this.CreatePublicDB.Results.Value && Directory.Exists(path))
				{
					source = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
				}
				return from w in source
				select new Result<string>(w);
			});
			this.TargetPathCompressed = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install | SetupMode.Upgrade).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.TargetPathCompressed(this.globalParameters.TargetDir)).Condition(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM Win32_Directory WHERE Name='{0}'", this.globalParameters.TargetDir))[0].TryGetValue("Compressed", out obj))
				{
					return new RuleResult(bool.Parse(obj.ToString()));
				}
				return new RuleResult(false);
			});
			this.ADAMDataPathExists = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADAMDataPathExists(this.AdamDataPath.Results.ValueOrDefault)).Condition(delegate(Result<object> x)
			{
				object obj;
				if (!string.IsNullOrEmpty(this.AdamDataPath.Results.ValueOrDefault) && base.Providers.WMIDataProvider.Run(string.Format("SELECT * FROM Win32_Directory WHERE Name='{0}'", this.AdamDataPath.Results.ValueOrDefault))[0].TryGetValue("Name", out obj) && !string.IsNullOrEmpty(obj.ToString()))
				{
					return new RuleResult(true);
				}
				return new RuleResult(false);
			});
			this.ADAMPortAlreadyInUse = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADAMPortAlreadyInUse(this.AdamPort.Results.Value.ToString())).Condition(delegate(Result<object> x)
			{
				Dictionary<string, List<string>> commands = new Dictionary<string, List<string>>
				{
					{
						this.AdamPort.Results.Value.ToString(),
						new List<string>
						{
							this.AdamPort.Results.Value.ToString(),
							string.Empty,
							string.Empty
						}
					}
				};
				Dictionary<string, object[]> dictionary = base.Providers.ManagedMethodProvider.PortAvailable(string.Empty, commands);
				object[] array;
				if (!AnalysisHelpers.IsNullOrEmpty(dictionary) && dictionary.TryGetValue(this.AdamPort.Results.Value.ToString(), out array))
				{
					return new RuleResult(array != null && array.Length > 1 && bool.Parse(array[1] as string));
				}
				return new RuleResult(false);
			});
			this.ADAMSSLPortAlreadyInUse = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.Gateway).Error<RuleBuilder<object>>().Message((Result x) => Strings.ADAMSSLPortAlreadyInUse(this.AdamSSLPort.Results.Value.ToString())).Condition(delegate(Result<object> x)
			{
				Dictionary<string, List<string>> commands = new Dictionary<string, List<string>>
				{
					{
						this.AdamSSLPort.Results.Value.ToString(),
						new List<string>
						{
							this.AdamSSLPort.Results.Value.ToString(),
							string.Empty,
							string.Empty
						}
					}
				};
				Dictionary<string, object[]> dictionary = base.Providers.ManagedMethodProvider.PortAvailable(string.Empty, commands);
				object[] array;
				if (!AnalysisHelpers.IsNullOrEmpty(dictionary) && dictionary.TryGetValue(this.AdamSSLPort.Results.Value.ToString(), out array))
				{
					return new RuleResult(array != null && array.Length > 1 && bool.Parse(array[1] as string));
				}
				return new RuleResult(false);
			});
			this.OSProductType = Setting<uint>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object obj;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_OperatingSystem")[0].TryGetValue("ProductType", out obj))
				{
					return new Result<uint>((uint)obj);
				}
				return new Result<uint>(0U);
			});
			this.SuiteMask = Setting<uint>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.All).Role(SetupRole.AdminTools | SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport).SetValue(delegate(Result<object> x)
			{
				object value;
				if (base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_OperatingSystem")[0].TryGetValue("SuiteMask", out value))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest<object>(3406179645U, ref value);
					return new Result<uint>(Convert.ToUInt32(value));
				}
				return new Result<uint>(0U);
			});
			this.ServiceMarkedForDeletion = Setting<string>.Build().AsRootSetting().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.All).SetValue(delegate(Result<object> x)
			{
				string text = string.Empty;
				Dictionary<string, object>[] array = base.Providers.WMIDataProvider.Run("SELECT * FROM Win32_Service");
				for (int i = 0; i < array.Length; i++)
				{
					object obj;
					if (array[i].TryGetValue("Name", out obj))
					{
						using (ServiceController serviceController = new ServiceController(obj.ToString()))
						{
							try
							{
								ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(ExceptionInjectionCallback.Win32InvalidOperationException));
								ExTraceGlobals.FaultInjectionTracer.TraceTest(3439734077U);
								ServiceControllerStatus status = serviceController.Status;
							}
							catch (InvalidOperationException ex)
							{
								Win32Exception ex2 = ex.InnerException as Win32Exception;
								if (ex2 != null && 1072 == ex2.NativeErrorCode)
								{
									text += (string.IsNullOrEmpty(text) ? obj.ToString() : (", " + obj.ToString()));
								}
							}
						}
					}
				}
				return new Result<string>(text);
			});
			this.ServicesAreMarkedForDeletion = Rule.Build().AsRootRule().In(this).AsSync().Mode(SetupMode.Install).Role(SetupRole.All).Error<RuleBuilder<object>>().Message((Result x) => Strings.ServicesAreMarkedForDeletion(this.ServiceMarkedForDeletion.Results.Value)).Condition(delegate(Result<object> x)
			{
				if (!string.IsNullOrEmpty(this.ServiceMarkedForDeletion.Results.Value))
				{
					return new RuleResult(true);
				}
				return new RuleResult(false);
			});
		}

		private object logLock = new object();

		private GlobalParameters globalParameters;

		private static uint adBasicPermissions = 983485U;

		private static uint adWritePermissions = 983295U;
	}
}
