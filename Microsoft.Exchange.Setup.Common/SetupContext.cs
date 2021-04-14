using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.ServiceProcess;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.Parser;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetupContext : ISetupContext
	{
		public SetupContext(Dictionary<string, object> parsedArguments, ExchangeServer server, bool isCleanMachine, RoleCollection unpackedRoles, RoleCollection unpackedDatacenterRoles, RoleCollection installedRolesLocal, RoleCollection partiallyConfiguredRoles, IOrganizationName organizationName, bool isW3SVCStartOk)
		{
			this.WatsonEnabled = false;
			this.ExchangeCulture = CultureInfo.InstalledUICulture;
			this.SetSetupContext(parsedArguments, server, isCleanMachine, unpackedRoles, unpackedDatacenterRoles, installedRolesLocal, partiallyConfiguredRoles, organizationName, isW3SVCStartOk);
		}

		public SetupContext(Dictionary<string, object> parsedArguments)
		{
			this.WatsonEnabled = false;
			this.ExchangeCulture = CultureInfo.InstalledUICulture;
			bool isE12Schema = false;
			bool isSchemaUpdateRequired = false;
			bool isOrgConfigUpdateRequired = false;
			bool isDomainConfigUpdateRequired = false;
			bool? flag = null;
			bool? flag2 = null;
			bool hostingDeployment = false;
			IndustryType industryType = IndustryType.NotSpecified;
			bool adinitializedSuccessfully;
			LocalizedException adinitializationError;
			string text;
			string gc;
			SetupContext.InitializeAD(out adinitializedSuccessfully, out adinitializationError, out text, out gc, ref isE12Schema, ref isSchemaUpdateRequired, ref isOrgConfigUpdateRequired, ref isDomainConfigUpdateRequired, parsedArguments);
			SetupLogger.Log(Strings.SetupWillUseDomainController(text));
			SetupLogger.Log(Strings.SetupWillUseGlobalCatalog(gc));
			ExchangeConfigurationContainer exchangeConfigurationContainer = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 149, ".ctor", "f:\\15.00.1497\\sources\\dev\\Setup\\src\\Common\\DataHandlers\\SetupContext.cs");
			OrganizationName organizationName = null;
			try
			{
				exchangeConfigurationContainer = topologyConfigurationSession.GetExchangeConfigurationContainer();
				SetupLogger.Log(Strings.ExchangeConfigurationContainerName(exchangeConfigurationContainer.DistinguishedName));
			}
			catch (ExchangeConfigurationContainerNotFoundException ex)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex.Message));
			}
			catch (CannotGetDomainInfoException ex2)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex2.Message));
			}
			catch (ADTransientException ex3)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex3.Message));
			}
			catch (ActiveDirectoryObjectNotFoundException ex4)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex4.Message));
			}
			catch (DataSourceOperationException ex5)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex5.Message));
			}
			catch (DataSourceTransientException ex6)
			{
				SetupLogger.Log(Strings.NoExchangeConfigurationContainerFound(ex6.Message));
			}
			if (exchangeConfigurationContainer != null)
			{
				try
				{
					Organization orgContainer = topologyConfigurationSession.GetOrgContainer();
					SetupLogger.Log(Strings.ExchangeOrganizationContainerName(orgContainer.DistinguishedName));
					if (null != orgContainer.Id.Rdn)
					{
						organizationName = new OrganizationName(orgContainer.Id.Rdn);
					}
					flag = orgContainer.CustomerFeedbackEnabled;
					industryType = orgContainer.Industry;
					hostingDeployment = orgContainer.HostingDeploymentEnabled;
				}
				catch (OrgContainerNotFoundException ex7)
				{
					SetupLogger.Log(Strings.NoExchangeOrganizationContainerFound(ex7.Message));
					SetupLogger.Log(Strings.RemoveMESOObjectLink);
					adinitializationError = new ADInitializationException(ex7.LocalizedString, ex7);
				}
				catch (ADTransientException ex8)
				{
					SetupLogger.Log(Strings.NoExchangeOrganizationContainerFound(ex8.Message));
				}
				catch (FormatException ex9)
				{
					this.OrganizationNameValidationException = new LocalizedException(Strings.InvalidExchangeOrganizationName(ex9.Message));
					SetupLogger.Log(Strings.InvalidExchangeOrganizationName(ex9.Message));
				}
			}
			ExchangeServer server = null;
			Server server2 = null;
			if (exchangeConfigurationContainer != null)
			{
				string machineName = Environment.MachineName;
				if (machineName == Environment.MachineName)
				{
					SetupLogger.Log(Strings.WillSearchForAServerObjectForLocalServer(machineName));
					try
					{
						server2 = topologyConfigurationSession.FindLocalServer();
						goto IL_284;
					}
					catch (ComputerNameNotCurrentlyAvailableException)
					{
						goto IL_284;
					}
					catch (LocalServerNotFoundException)
					{
						goto IL_284;
					}
				}
				SetupLogger.Log(Strings.WillSearchForAServerObjectForServer(machineName));
				try
				{
					server2 = topologyConfigurationSession.FindServerByName(machineName);
				}
				catch (ADTransientException ex10)
				{
					SetupLogger.Log(Strings.AttemptToSearchExchangeServerFailed(machineName, ex10.Message));
				}
				catch (DataSourceOperationException ex11)
				{
					SetupLogger.Log(Strings.AttemptToSearchExchangeServerFailed(machineName, ex11.Message));
				}
				IL_284:
				if (server2 != null)
				{
					SetupLogger.Log(Strings.ExchangeServerFound(server2.DistinguishedName));
					server = new ExchangeServer(server2);
					flag2 = server2.CustomerFeedbackEnabled;
				}
				else
				{
					SetupLogger.Log(Strings.ExchangeServerNotFound(machineName));
				}
			}
			bool flag3 = !ConfigurationContext.Setup.IsUnpacked;
			RoleCollection unpackedRoles = RoleManager.GetUnpackedRoles();
			RoleCollection unpackedDatacenterRoles = RoleManager.GetUnpackedDatacenterRoles();
			RoleCollection installedRoles = RoleManager.GetInstalledRoles();
			RoleCollection roleCollection = new RoleCollection();
			foreach (Role role in RoleManager.Roles)
			{
				if (role.IsPartiallyInstalled || (role.IsUnpacked && !role.IsInstalled))
				{
					roleCollection.Add(role);
				}
			}
			if (!flag3)
			{
				SetupLogger.Log(Strings.TheCurrentServerHasExchangeBits);
			}
			else
			{
				SetupLogger.Log(Strings.TheCurrentServerHasNoExchangeBits);
			}
			bool isW3SVCStartOk = SetupContext.DetectIsW3SVCStartOk();
			this.SetSetupContext(parsedArguments, server, flag3, unpackedRoles, unpackedDatacenterRoles, installedRoles, roleCollection, organizationName, isW3SVCStartOk);
			this.HostingDeployment = hostingDeployment;
			this.ADInitializedSuccessfully = adinitializedSuccessfully;
			this.ADInitializationError = adinitializationError;
			this.DomainController = text;
			this.IsE12Schema = isE12Schema;
			this.IsSchemaUpdateRequired = isSchemaUpdateRequired;
			this.IsOrgConfigUpdateRequired = isOrgConfigUpdateRequired;
			this.IsDomainConfigUpdateRequired = isDomainConfigUpdateRequired;
			this.OriginalGlobalCustomerFeedbackEnabled = flag;
			this.GlobalCustomerFeedbackEnabled = flag;
			this.OriginalServerCustomerFeedbackEnabled = flag2;
			this.ServerCustomerFeedbackEnabled = flag2;
			this.OriginalIndustry = industryType;
			this.Industry = industryType;
			SetupLogger.Log(Strings.AdInitializationStatus(this.ADInitializedSuccessfully));
			SetupLogger.Log(Strings.SchemaUpdateRequired(this.IsSchemaUpdateRequired));
			SetupLogger.Log(Strings.OrgConfigUpdateRequired(this.IsOrgConfigUpdateRequired));
			SetupLogger.Log(Strings.DomainConfigUpdateRequired(this.IsDomainConfigUpdateRequired));
			if (!this.IsCleanMachine)
			{
				this.InstalledPath = NonRootLocalLongFullPath.Parse(ConfigurationContext.Setup.InstallPath);
				try
				{
					this.InstalledVersion = ConfigurationContext.Setup.InstalledVersion;
					SetupLogger.Log(Strings.InstalledVersion(this.InstalledVersion));
				}
				catch (SetupVersionInformationCorruptException ex12)
				{
					SetupLogger.Log(ex12.LocalizedString);
					this.RegistryError = ex12;
				}
			}
			this.TargetDir = this.InstalledPath;
			if (this.TargetDir != null)
			{
				SetupLogger.Log(Strings.TargetInstallationDirectory(this.TargetDir.PathName));
			}
			if (this.ExchangeOrganizationExists)
			{
				ADPagedReader<Server> adpagedReader = topologyConfigurationSession.FindPaged<Server>(exchangeConfigurationContainer.Id, QueryScope.SubTree, null, null, 0);
				Server[] array = adpagedReader.ReadAllPages();
				bool canOrgBeRemoved = (server2 == null && array.Length == 0) || (server2 != null && array.Length == 1);
				foreach (Server dataObject in array)
				{
					ExchangeServer exchangeServer = new ExchangeServer(dataObject);
					bool flag4 = false;
					ValidationError validationError;
					if (DirectoryUtilities.IsPropertyValid(exchangeServer, ServerSchema.IsExchange2007OrLater, out validationError))
					{
						flag4 = exchangeServer.IsExchange2007OrLater;
					}
					else
					{
						SetupLogger.Log(Strings.ExchangeVersionInvalid(exchangeServer.Name, validationError.Description));
					}
					bool flag5 = false;
					if (DirectoryUtilities.IsPropertyValid(exchangeServer, ServerSchema.IsE14OrLater, out validationError))
					{
						flag5 = exchangeServer.IsE14OrLater;
					}
					else
					{
						SetupLogger.Log(Strings.ExchangeVersionInvalid(exchangeServer.Name, validationError.Description));
					}
					if (exchangeServer.IsMailboxServer && flag4)
					{
						this.HasMailboxServers = true;
					}
					if (exchangeServer.IsHubTransportServer && flag4)
					{
						this.HasBridgeheadServers = true;
					}
					if (!flag4)
					{
						this.HasLegacyServers = true;
					}
					if (flag5)
					{
						this.HasE14OrLaterServers = true;
					}
					if (server2 != null && string.Compare(server2.DistinguishedName, exchangeServer.DistinguishedName, true, CultureInfo.InvariantCulture) != 0)
					{
						canOrgBeRemoved = false;
					}
				}
				this.CanOrgBeRemoved = canOrgBeRemoved;
			}
			if (this.ParsedArguments.ContainsKey("enableerrorreporting"))
			{
				this.WatsonEnabled = true;
			}
			else
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15", "DisableErrorReporting", 1);
				if (value != null)
				{
					this.WatsonEnabled = ((int)value == 0);
				}
			}
			this.ExchangeCulture = CultureInfo.GetCultureInfo("en-US");
			this.IsLonghornServer = (Environment.OSVersion.Version.Major >= 6);
			ProvisioningLayer.Disabled = true;
		}

		public string CurrentWizardPageName { get; set; }

		[DefaultValue(false)]
		public bool IsRestoredFromPreviousState { get; set; }

		public string ExchangeServerName { get; set; }

		public Dictionary<string, object> ParsedArguments { get; set; }

		public InstallationModes InstallationMode { get; set; }

		public bool InstallWindowsComponents { get; set; }

		public RoleCollection InstalledRolesLocal { get; set; }

		public RoleCollection InstalledRolesAD { get; set; }

		public RoleCollection PartiallyConfiguredRoles { get; set; }

		public RoleCollection UnpackedRoles { get; set; }

		public RoleCollection UnpackedDatacenterRoles { get; set; }

		public bool WatsonEnabled { get; set; }

		public RoleCollection RequestedRoles
		{
			get
			{
				if (!this.ParsedArguments.ContainsKey("roles"))
				{
					return null;
				}
				return (RoleCollection)this.ParsedArguments["roles"];
			}
			set
			{
				this.ParsedArguments["roles"] = value;
			}
		}

		public bool IsCleanMachine { get; set; }

		public bool IsDatacenter { get; set; }

		public bool IsDatacenterDedicated { get; set; }

		public bool TreatPreReqErrorsAsWarnings { get; set; }

		public bool IsFfo { get; set; }

		public bool IsPartnerHosted { get; set; }

		public bool IsBackupKeyPresent { get; set; }

		public NonRootLocalLongFullPath InstalledPath { get; set; }

		public NonRootLocalLongFullPath BackupInstalledPath { get; set; }

		public Version InstalledVersion { get; set; }

		public Version RunningVersion { get; set; }

		public Version BackupInstalledVersion { get; set; }

		public ushort AdamLdapPort { get; set; }

		public ushort AdamSslPort { get; set; }

		public LongPath SourceDir
		{
			get
			{
				return this.sourceDir;
			}
			set
			{
				this.sourceDir = value;
				if (this.languagePackContext != null && !this.IsLanguagePackOperation && this.SourceDir != null)
				{
					this.LanguagePackPath = this.SourceDir;
				}
			}
		}

		public LongPath UpdatesDir { get; set; }

		public LongPath LanguagePackPath
		{
			get
			{
				return this.languagePackContext.LanguagePackPath;
			}
			set
			{
				this.languagePackContext.LanguagePackPath = value;
			}
		}

		public bool LanguagePackSourceIsBundle
		{
			get
			{
				return this.languagePackContext.LanguagePackSourceIsBundle;
			}
		}

		public Dictionary<string, LanguageInfo> CollectedLanguagePacks
		{
			get
			{
				return this.languagePackContext.CollectedLanguagePacks;
			}
		}

		public Dictionary<string, LanguageInfo> SourceLanguagePacks
		{
			get
			{
				return this.languagePackContext.SourceLanguagePacks;
			}
		}

		public Dictionary<string, Array> LanguagePacksToInstall
		{
			get
			{
				return this.languagePackContext.LanguagePacksToInstall;
			}
		}

		public Dictionary<string, long> LanguagesToInstall
		{
			get
			{
				return this.languagePackContext.LanguagesToInstall;
			}
		}

		public HashSet<string> InstalledLanguagePacks
		{
			get
			{
				return this.languagePackContext.InstalledLanguagePacks;
			}
		}

		public NonRootLocalLongFullPath TargetDir { get; set; }

		public bool IsW3SVCStartOk { get; private set; }

		public string NewProvisionedServerName
		{
			get
			{
				return (string)this.ParsedArguments["newprovisionedserver"];
			}
		}

		public string RemoveProvisionedServerName
		{
			get
			{
				if (!this.HasRemoveProvisionedServerParameters)
				{
					return null;
				}
				if (this.ParsedArguments.ContainsKey("removeprovisionedserver"))
				{
					return ((string)this.ParsedArguments["removeprovisionedserver"]) ?? this.ExchangeServerName;
				}
				return this.ExchangeServerName;
			}
		}

		public bool HasMailboxServers { get; set; }

		public bool HasBridgeheadServers { get; set; }

		public bool HasLegacyServers { get; set; }

		public bool HasE14OrLaterServers { get; set; }

		public bool ExchangeOrganizationExists { get; set; }

		public LocalizedException OrganizationNameValidationException { get; set; }

		public bool IsServerFoundInAD { get; private set; }

		public IOrganizationName OrganizationName { get; set; }

		public IOrganizationName OrganizationNameFoundInAD { get; set; }

		public string DomainController { get; set; }

		public bool ADInitializedSuccessfully { get; set; }

		public LocalizedException ADInitializationError { get; private set; }

		public bool IsE12Schema { get; set; }

		public bool IsSchemaUpdateRequired { get; set; }

		public bool IsOrgConfigUpdateRequired { get; set; }

		public bool IsDomainConfigUpdateRequired { get; private set; }

		public bool StartTransportService { get; set; }

		public bool CanOrgBeRemoved { get; set; }

		public bool HostingDeployment { get; private set; }

		public string TenantOrganizationConfig
		{
			get
			{
				if (!this.ParsedArguments.ContainsKey("tenantorganizationconfig"))
				{
					return null;
				}
				return this.ParsedArguments["tenantorganizationconfig"].ToString();
			}
		}

		public LocalizedException RegistryError { get; private set; }

		public bool IsInstalledLocal(string roleName)
		{
			foreach (Role role in this.InstalledRolesLocal)
			{
				if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInstalledAD(string roleName)
		{
			foreach (Role role in this.InstalledRolesAD)
			{
				if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInstalledLocalOrAD(string roleName)
		{
			return this.IsInstalledLocal(roleName) || this.IsInstalledAD(roleName);
		}

		public bool IsUnpacked(string roleName)
		{
			foreach (Role role in this.UnpackedRoles)
			{
				if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsUnpackedOrInstalledAD(string roleName)
		{
			return this.IsUnpacked(roleName) || this.IsInstalledAD(roleName);
		}

		public bool IsPartiallyConfigured(string roleName)
		{
			foreach (Role role in this.PartiallyConfiguredRoles)
			{
				if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsRequested(string roleName)
		{
			foreach (Role role in this.RequestedRoles)
			{
				if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasPrepareADParameters
		{
			get
			{
				return this.ParsedArguments != null && (this.ParsedArguments.ContainsKey("prepareschema") || this.ParsedArguments.ContainsKey("preparead") || this.ParsedArguments.ContainsKey("preparesct") || this.ParsedArguments.ContainsKey("preparedomain") || this.ParsedArguments.ContainsKey("preparealldomains"));
			}
		}

		public bool HasRolesToInstall
		{
			get
			{
				return this.RequestedRoles.Count > 0;
			}
		}

		public bool HasNewProvisionedServerParameters
		{
			get
			{
				return this.ParsedArguments != null && this.ParsedArguments.ContainsKey("newprovisionedserver");
			}
		}

		public bool HasRemoveProvisionedServerParameters
		{
			get
			{
				return this.ParsedArguments != null && this.ParsedArguments.ContainsKey("removeprovisionedserver");
			}
		}

		public bool IsProvisionedServer
		{
			get
			{
				return this.isProvisionedServer;
			}
		}

		public List<CultureInfo> InstalledUMLanguagePacks { get; set; }

		public List<CultureInfo> SelectedCultures { get; set; }

		public bool IsLanguagePackOperation
		{
			get
			{
				return this.languagePackContext.IsLanguagePackOperation;
			}
			set
			{
				this.languagePackContext.IsLanguagePackOperation = value;
			}
		}

		public bool IsUmLanguagePackOperation { get; set; }

		public CultureInfo ExchangeCulture { get; set; }

		public bool IsLonghornServer { get; set; }

		public bool? OriginalGlobalCustomerFeedbackEnabled { get; set; }

		public bool? GlobalCustomerFeedbackEnabled { get; set; }

		public bool? OriginalServerCustomerFeedbackEnabled { get; set; }

		public bool? ServerCustomerFeedbackEnabled { get; set; }

		public bool? ActiveDirectorySplitPermissions { get; set; }

		public IndustryType OriginalIndustry { get; set; }

		public IndustryType Industry { get; set; }

		public bool IsLanaguagePacksInstalled
		{
			get
			{
				return this.languagePackContext.IsLanaguagePacksInstalled;
			}
			set
			{
				this.languagePackContext.IsLanaguagePacksInstalled = value;
			}
		}

		public bool DisableAMFiltering { get; set; }

		public bool NeedToUpdateLanguagePacks
		{
			get
			{
				return this.languagePackContext.NeedToUpdateLanguagePacks;
			}
			set
			{
				this.languagePackContext.NeedToUpdateLanguagePacks = value;
			}
		}

		public void UpdateIsW3SVCStartOk()
		{
			this.IsW3SVCStartOk = SetupContext.DetectIsW3SVCStartOk();
		}

		private static bool DetectIsW3SVCStartOk()
		{
			bool result;
			using (ServiceController serviceController = new ServiceController("W3SVC"))
			{
				try
				{
					ServiceControllerStatus status = serviceController.Status;
					result = true;
				}
				catch (InvalidOperationException ex)
				{
					Win32Exception ex2 = ex.InnerException as Win32Exception;
					if (ex2 == null || (1060 != ex2.NativeErrorCode && 1072 != ex2.NativeErrorCode && 1058 != ex2.NativeErrorCode))
					{
						throw;
					}
					result = false;
				}
			}
			return result;
		}

		private void SetSetupContext(Dictionary<string, object> parsedArguments, ExchangeServer server, bool isCleanMachine, RoleCollection unpackedRoles, RoleCollection unpackedDatacenterRoles, RoleCollection installedRolesLocal, RoleCollection partiallyConfiguredRoles, IOrganizationName organizationName, bool isW3SVCStartOk)
		{
			this.IsCleanMachine = true;
			this.InstalledRolesLocal = new RoleCollection();
			this.InstalledRolesAD = new RoleCollection();
			this.PartiallyConfiguredRoles = new RoleCollection();
			this.UnpackedRoles = new RoleCollection();
			this.UnpackedDatacenterRoles = new RoleCollection();
			this.ParsedArguments = (parsedArguments ?? new Dictionary<string, object>());
			this.HasMailboxServers = false;
			this.HasBridgeheadServers = false;
			this.HasLegacyServers = false;
			this.CanOrgBeRemoved = false;
			this.DisableAMFiltering = false;
			SetupLogger.Log(Strings.DisplayServerName(Environment.MachineName));
			if (this.ParsedArguments.ContainsKey("disableamfiltering"))
			{
				this.DisableAMFiltering = true;
				SetupLogger.Log(Strings.WillDisableAMFiltering);
			}
			if (!this.ParsedArguments.ContainsKey("roles"))
			{
				this.ParsedArguments["roles"] = new RoleCollection();
			}
			this.IsCleanMachine = isCleanMachine;
			this.isProvisionedServer = false;
			this.OrganizationName = organizationName;
			this.IsW3SVCStartOk = isW3SVCStartOk;
			this.OrganizationNameFoundInAD = organizationName;
			this.IsServerFoundInAD = (null != server);
			if (this.ParsedArguments.ContainsKey("donotstarttransport"))
			{
				this.StartTransportService = false;
				SetupLogger.Log(Strings.WillNotStartTransportService);
			}
			else
			{
				this.StartTransportService = true;
			}
			this.SelectedCultures = new List<CultureInfo>();
			if (this.ParsedArguments.ContainsKey("addumlanguagepack"))
			{
				this.SelectedCultures = (List<CultureInfo>)this.ParsedArguments["addumlanguagepack"];
			}
			else if (this.ParsedArguments.ContainsKey("removeumlanguagepack"))
			{
				this.SelectedCultures = (List<CultureInfo>)this.ParsedArguments["removeumlanguagepack"];
			}
			this.InstalledUMLanguagePacks = new List<CultureInfo>();
			this.InstalledUMLanguagePacks.AddRange(LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.UnifiedMessaging));
			InstallableUnitConfigurationInfoManager.InitializeUmLanguagePacksConfigurationInfo(this.InstalledUMLanguagePacks.ToArray());
			if (!this.ParsedArguments.ContainsKey("sourcedir") && !isCleanMachine)
			{
				string path = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}", "InstallSource", null);
				if (!Directory.Exists(path))
				{
					path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				}
				LongPath value;
				if (LongPath.TryParse(path, out value))
				{
					this.ParsedArguments["sourcedir"] = value;
				}
			}
			if (this.ParsedArguments.ContainsKey("sourcedir"))
			{
				SetupLogger.Log(Strings.SetupSourceDirectory(((LongPath)this.ParsedArguments["sourcedir"]).PathName));
			}
			this.RunningVersion = ConfigurationContext.Setup.GetExecutingVersion();
			if (!this.IsCleanMachine)
			{
				this.UnpackedRoles = unpackedRoles;
			}
			this.InstalledRolesLocal.AddRange(installedRolesLocal);
			this.PartiallyConfiguredRoles.AddRange(partiallyConfiguredRoles);
			if (this.IsUnpacked("AdminToolsRole"))
			{
				this.InstalledRolesAD.Add(RoleManager.GetRoleByName("AdminToolsRole"));
			}
			this.IsDatacenter = Datacenter.IsMicrosoftHostedOnly(false);
			this.TreatPreReqErrorsAsWarnings = Datacenter.TreatPreReqErrorsAsWarnings(false);
			if (this.IsDatacenter)
			{
				this.IsFfo = DatacenterRegistry.IsForefrontForOffice();
			}
			this.IsDatacenterDedicated = Datacenter.IsDatacenterDedicated(false);
			this.IsPartnerHosted = Datacenter.IsPartnerHostedOnly(false);
			if (!this.IsCleanMachine)
			{
				this.UnpackedDatacenterRoles = unpackedDatacenterRoles;
			}
			if (server != null)
			{
				SetupLogger.Log(Strings.WillGetConfiguredRolesFromServerObject(server.DistinguishedName));
				if (server.IsProvisionedServer)
				{
					SetupLogger.Log(Strings.ServerIsProvisioned);
					this.isProvisionedServer = true;
				}
				if (server.IsHubTransportServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("BridgeheadRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("BridgeheadRole"));
				}
				if ((bool)server[ExchangeServerSchema.IsClientAccessServer])
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("ClientAccessRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("ClientAccessRole"));
				}
				if (server.IsEdgeServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("GatewayRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("GatewayRole"));
				}
				if (server.IsMailboxServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("MailboxRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("MailboxRole"));
				}
				if (server.IsUnifiedMessagingServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("UnifiedMessagingRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("UnifiedMessagingRole"));
				}
				if (server.IsCafeServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("CafeRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("CafeRole"));
				}
				if (server.IsFrontendTransportServer)
				{
					SetupLogger.Log(Strings.RoleInstalledOnServer("FrontendTransportRole"));
					this.InstalledRolesAD.Add(RoleManager.GetRoleByName("FrontendTransportRole"));
				}
			}
			else if (this.IsInstalledLocal("GatewayRole"))
			{
				SetupLogger.Log(Strings.EdgeRoleInstalledButServerObjectNotFound);
				this.InstalledRolesAD.Add(RoleManager.GetRoleByName("GatewayRole"));
			}
			this.ReadBackupRegistry();
			if (this.HasPrepareADParameters || this.HasNewProvisionedServerParameters)
			{
				this.InstallationMode = InstallationModes.Install;
			}
			else if (this.HasRemoveProvisionedServerParameters)
			{
				this.InstallationMode = InstallationModes.Uninstall;
			}
			else if (this.ParsedArguments.ContainsKey("addumlanguagepack"))
			{
				this.InstallationMode = InstallationModes.Install;
				this.IsUmLanguagePackOperation = true;
			}
			else if (this.ParsedArguments.ContainsKey("removeumlanguagepack"))
			{
				this.InstallationMode = InstallationModes.Uninstall;
				this.IsUmLanguagePackOperation = true;
			}
			else
			{
				SetupOperations setupOperations;
				if (this.ParsedArguments.ContainsKey("mode"))
				{
					setupOperations = (SetupOperations)this.ParsedArguments["mode"];
				}
				else
				{
					setupOperations = SetupOperations.Install;
				}
				if (this.IsBackupKeyPresent)
				{
					setupOperations = SetupOperations.Upgrade;
				}
				if ((setupOperations & SetupOperations.Install) != SetupOperations.None)
				{
					this.InstallationMode = InstallationModes.Install;
				}
				else if ((setupOperations & SetupOperations.Uninstall) != SetupOperations.None)
				{
					this.InstallationMode = InstallationModes.Uninstall;
				}
				else if ((setupOperations & SetupOperations.Upgrade) != SetupOperations.None)
				{
					this.InstallationMode = InstallationModes.BuildToBuildUpgrade;
				}
				else if ((setupOperations & SetupOperations.RecoverServer) != SetupOperations.None)
				{
					this.InstallationMode = InstallationModes.DisasterRecovery;
				}
			}
			SetupLogger.Log(Strings.InstallationModeSetTo(this.InstallationMode.ToString()));
			if (this.InstallationMode == InstallationModes.Uninstall)
			{
				if (this.RequestedRoles.Count == 0)
				{
					this.RequestedRoles.AddRange(this.UnpackedRoles);
				}
			}
			else if (this.InstallationMode == InstallationModes.BuildToBuildUpgrade)
			{
				this.RequestedRoles.AddRange(this.InstalledRolesAD);
			}
			else if (this.InstallationMode == InstallationModes.DisasterRecovery)
			{
				this.RequestedRoles.AddRange(this.InstalledRolesAD);
			}
			if (this.ParsedArguments.ContainsKey("adamldapport"))
			{
				this.AdamLdapPort = (ushort)this.ParsedArguments["adamldapport"];
			}
			else
			{
				this.AdamLdapPort = 50389;
			}
			if (this.ParsedArguments.ContainsKey("adamsslport"))
			{
				this.AdamSslPort = (ushort)this.ParsedArguments["adamsslport"];
			}
			else
			{
				this.AdamSslPort = 50636;
			}
			if (server != null)
			{
				this.ExchangeServerName = server.Name;
			}
			this.InstallWindowsComponents = this.ParsedArguments.ContainsKey("installwindowscomponents");
			if (this.ParsedArguments.ContainsKey("sourcedir"))
			{
				this.SourceDir = (LongPath)this.ParsedArguments["sourcedir"];
			}
			if (this.ParsedArguments.ContainsKey("updatesdir"))
			{
				this.UpdatesDir = (LongPath)this.ParsedArguments["updatesdir"];
			}
			this.languagePackContext = new LanguagePackContext(this.InstallationMode, this.ParsedArguments.ContainsKey("languagepack"), (LongPath)(this.ParsedArguments.ContainsKey("languagepack") ? this.ParsedArguments["languagepack"] : null), this.IsCleanMachine, this.IsUmLanguagePackOperation, this.SourceDir);
			if (this.OrganizationName == null)
			{
				this.ExchangeOrganizationExists = false;
				if (this.ParsedArguments.ContainsKey("organizationname"))
				{
					this.OrganizationName = this.ParseOrganizationName((string)this.ParsedArguments["organizationname"]);
					SetupLogger.Log(Strings.SettingOrganizationName(this.OrganizationName.EscapedName));
				}
				else
				{
					SetupLogger.Log(Strings.ExchangeOrganizationNameRequired);
				}
			}
			else
			{
				SetupLogger.Log(Strings.ExistingOrganizationName(this.OrganizationName.EscapedName));
				this.ExchangeOrganizationExists = true;
				if (this.ParsedArguments.ContainsKey("organizationname"))
				{
					SetupLogger.LogWarning(Strings.ExchangeOrganizationAlreadyExists(this.OrganizationName.EscapedName, this.ParseOrganizationName((string)this.ParsedArguments["organizationname"]).EscapedName));
				}
			}
			this.ActiveDirectorySplitPermissions = (bool?)(this.ParsedArguments.ContainsKey("ActiveDirectorySplitPermissions") ? this.ParsedArguments["ActiveDirectorySplitPermissions"] : null);
			InstallableUnitConfigurationInfo.SetupContext = this;
		}

		private void ReadBackupRegistry()
		{
			this.IsBackupKeyPresent = false;
			this.BackupInstalledPath = null;
			this.BackupInstalledVersion = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistryConstants.SetupBackupKey))
			{
				if (registryKey != null)
				{
					this.IsBackupKeyPresent = true;
					string text = null;
					try
					{
						text = "MsiInstallPath";
						string text2 = (string)registryKey.GetValue(text);
						if (!string.IsNullOrEmpty(text2))
						{
							this.BackupInstalledPath = NonRootLocalLongFullPath.Parse(text2);
							SetupLogger.Log(Strings.BackupPath(this.BackupInstalledPath.PathName));
						}
						text = "MsiProductMajor";
						int num = (int)registryKey.GetValue(text, -1);
						text = "MsiProductMinor";
						int num2 = (int)registryKey.GetValue(text, -1);
						text = "MsiBuildMajor";
						int num3 = (int)registryKey.GetValue(text, -1);
						text = "MsiBuildMinor";
						int num4 = (int)registryKey.GetValue(text, -1);
						if (num != -1 && num2 != -1 && num3 != -1 && num4 != -1)
						{
							this.BackupInstalledVersion = new Version(num, num2, num3, num4);
							SetupLogger.Log(Strings.BackupVersion(this.BackupInstalledVersion));
						}
					}
					catch (SecurityException innerException)
					{
						throw new BackupKeyInaccessibleException(RegistryConstants.SetupBackupKey, innerException);
					}
					catch (InvalidCastException innerException2)
					{
						throw new BackupKeyIsWrongTypeException(RegistryConstants.SetupBackupKey, text, innerException2);
					}
				}
			}
		}

		private static void InitializeAD(out bool adInitPassed, out LocalizedException adError, out string dc, out string gc, ref bool isE12Schema, ref bool schemaUpdateRequired, ref bool orgConfigUpdateRequired, ref bool domainConfigUpdateRequired, Dictionary<string, object> parsedArguments)
		{
			SetupLogger.TraceEnter(new object[0]);
			adInitPassed = false;
			adError = null;
			dc = null;
			gc = null;
			SetupLogger.Log(Strings.ChoosingDomainController);
			try
			{
				ADServer adserver = null;
				bool flag = false;
				if (ADSession.IsBoundToAdam)
				{
					throw new ADInitializationException(Strings.ADDriverBoundToAdam);
				}
				ADServerSettings adserverSettings = SetupServerSettings.CreateSetupServerSettings();
				ADSessionSettings.SetProcessADContext(new ADDriverContext(adserverSettings, ContextMode.Setup));
				if (parsedArguments.ContainsKey("domaincontroller"))
				{
					string text = (string)parsedArguments["domaincontroller"];
					SetupLogger.Log(Strings.DCAlreadySpecified(text));
					adserver = DirectoryUtilities.DomainControllerFromName(text);
					if (adserver == null)
					{
						throw new ADInitializationException(Strings.UserSpecifiedDCDoesNotExistException(text));
					}
					if (!parsedArguments.ContainsKey("preparedomain") && !parsedArguments.ContainsKey("preparealldomains") && !DirectoryUtilities.InLocalDomain(adserver))
					{
						throw new ADInitializationException(Strings.UserSpecifiedDCIsNotInLocalDomainException(adserver.DnsHostName));
					}
					if (!adserver.IsAvailable())
					{
						throw new ADInitializationException(Strings.UserSpecifiedDCIsNotAvailableException(adserver.DnsHostName));
					}
					flag = true;
				}
				else if (!string.IsNullOrEmpty(ADSession.GetSharedConfigDC()))
				{
					string sharedConfigDC = ADSession.GetSharedConfigDC();
					SetupLogger.Log(Strings.PersistedDomainController(sharedConfigDC));
					adserver = DirectoryUtilities.DomainControllerFromName(sharedConfigDC);
					if (adserver != null)
					{
						if (!DirectoryUtilities.InLocalDomain(adserver))
						{
							SetupLogger.Log(Strings.DCNotInLocalDomain(adserver.DnsHostName));
							adserver = null;
						}
						else if (!adserver.IsAvailable())
						{
							SetupLogger.Log(Strings.DCNotResponding(adserver.DnsHostName));
							adserver = null;
						}
					}
					else
					{
						SetupLogger.Log(Strings.DCNameNotValid(sharedConfigDC));
					}
				}
				if (adserver == null)
				{
					SetupLogger.Log(Strings.PickingDomainController);
					adserver = DirectoryUtilities.PickLocalDomainController();
					SetupLogger.Log(Strings.DomainControllerChosen(adserver.DnsHostName));
				}
				int num;
				if (parsedArguments.ContainsKey("prepareschema") && DirectoryUtilities.TryGetSchemaVersionRangeUpper(adserver.DnsHostName, out num) && num > 15312)
				{
					throw new ADInitializationException(Strings.ADSchemaVersionHigherThanSetupException(num, 15312));
				}
				int num2;
				if (parsedArguments.ContainsKey("preparedomain") && DirectoryUtilities.TryGetLocalDomainConfigVersion(out num2) && num2 > MesoContainer.DomainPrepVersion)
				{
					throw new ADInitializationException(Strings.ADDomainConfigVersionHigherThanSetupException(num2, MesoContainer.DomainPrepVersion));
				}
				int num3;
				if (parsedArguments.ContainsKey("preparead") && DirectoryUtilities.TryGetOrgConfigVersion(adserver.DnsHostName, out num3) && num3 > Organization.OrgConfigurationVersion)
				{
					throw new ADInitializationException(Strings.ADOrgConfigVersionHigherThanSetupException(num3, Organization.OrgConfigurationVersion));
				}
				ADSchemaVersion schemaVersion = DirectoryUtilities.GetSchemaVersion(adserver.DnsHostName);
				isE12Schema = (schemaVersion >= ADSchemaVersion.Exchange2007Rtm);
				schemaUpdateRequired = !DirectoryUtilities.IsSchemaUpToDate(adserver.DnsHostName);
				orgConfigUpdateRequired = !DirectoryUtilities.IsOrgConfigUpToDate(adserver.DnsHostName);
				domainConfigUpdateRequired = !DirectoryUtilities.IsLocalDomainConfigUpToDate();
				if (!schemaUpdateRequired && !orgConfigUpdateRequired)
				{
					dc = adserver.DnsHostName;
					SetupLogger.Log(Strings.ForestPrepAlreadyRun(dc));
				}
				else
				{
					ADServer schemaMasterDomainController;
					try
					{
						schemaMasterDomainController = DirectoryUtilities.GetSchemaMasterDomainController();
					}
					catch (SchemaMasterDCNotFoundException innerException)
					{
						throw new ADInitializationException(Strings.SchemaMasterDCNotFoundException, innerException);
					}
					SetupLogger.Log(Strings.ForestPrepNotRun(schemaMasterDomainController.DnsHostName));
					if (flag && !adserver.Id.Equals(schemaMasterDomainController.Id))
					{
						throw new ADInitializationException(Strings.UserSpecifiedDCIsNotSchemaMasterException(adserver.DnsHostName));
					}
					bool flag2 = schemaMasterDomainController.IsAvailable();
					if (flag2)
					{
						SetupLogger.Log(Strings.SchemaMasterAvailable);
					}
					else
					{
						SetupLogger.Log(Strings.SchemaMasterNotAvailable);
					}
					if (DirectoryUtilities.InSameDomain(adserver, schemaMasterDomainController) && DirectoryUtilities.InSameSite(adserver, schemaMasterDomainController))
					{
						if (!flag2)
						{
							throw new ADInitializationException(Strings.SchemaMasterDCNotAvailableException(schemaMasterDomainController.DnsHostName));
						}
						dc = schemaMasterDomainController.DnsHostName;
						SetupLogger.Log(Strings.SchemaMasterIsLocalDC(dc));
						schemaUpdateRequired = !DirectoryUtilities.IsSchemaUpToDate(schemaMasterDomainController.DnsHostName);
						orgConfigUpdateRequired = !DirectoryUtilities.IsOrgConfigUpToDate(schemaMasterDomainController.DnsHostName);
					}
					else
					{
						if (!flag2)
						{
							throw new ADInitializationException(Strings.ForestPrepNotRunOrNotReplicatedException(schemaMasterDomainController.DnsHostName));
						}
						if (DirectoryUtilities.IsSchemaUpToDate(schemaMasterDomainController.DnsHostName) && DirectoryUtilities.IsOrgConfigUpToDate(schemaMasterDomainController.DnsHostName))
						{
							throw new ADInitializationException(Strings.WaitForForestPrepReplicationToLocalDomainException);
						}
						throw new ADInitializationException(Strings.RunForestPrepInSchemaMasterDomainException(schemaMasterDomainController.DomainId.Name, schemaMasterDomainController.Site.Name));
					}
				}
				SetupLogger.Log(Strings.ChoosingGlobalCatalog);
				gc = DirectoryUtilities.PickGlobalCatalog(dc).DnsHostName;
				SetupLogger.Log(Strings.GCChosen(gc));
				adserverSettings.SetConfigurationDomainController(TopologyProvider.LocalForestFqdn, new Fqdn(dc));
				adserverSettings.SetPreferredGlobalCatalog(TopologyProvider.LocalForestFqdn, new Fqdn(gc));
				adserverSettings.AddPreferredDC(new Fqdn(dc));
				ADSessionSettings.SetProcessADContext(new ADDriverContext(adserverSettings, ContextMode.Setup));
				adInitPassed = true;
			}
			catch (DataSourceOperationException ex)
			{
				adError = new ADInitializationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				adError = new ADInitializationException(ex2.LocalizedString, ex2);
			}
			catch (DataValidationException ex3)
			{
				adError = new ADInitializationException(ex3.LocalizedString, ex3);
			}
			catch (ADInitializationException ex4)
			{
				adError = ex4;
			}
			finally
			{
				if (adError != null)
				{
					SetupLogger.LogError(adError);
				}
				SetupLogger.TraceExit();
			}
		}

		public IOrganizationName ParseOrganizationName(string name)
		{
			try
			{
				this.OrganizationName = new OrganizationName(name);
			}
			catch (FormatException innerException)
			{
				throw new InvalidOrganizationNameException(name, innerException);
			}
			return this.OrganizationName;
		}

		public static object ParseFqdnForPrepareLegacyPermissions(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn) || SmtpAddress.IsValidDomain(fqdn))
			{
				return fqdn;
			}
			throw new InvalidFqdnException(fqdn);
		}

		private const string watsonConfigurationKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string watsonConfigurationName = "DisableErrorReporting";

		private LongPath sourceDir;

		private bool isProvisionedServer;

		private LanguagePackContext languagePackContext;
	}
}
