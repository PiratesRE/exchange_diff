using System;

namespace Microsoft.Exchange.Management.Analysis
{
	public class GlobalParameters
	{
		public GlobalParameters(string targetDir, Version exchangeVersion, int adamPort, int adamSSLPort, bool createPublicDB, bool customerFeedbackEnabled, string newProvisionedServerName, string removeProvisionedServerName, string globalCatalog, string domainController, string prepareDomain, bool prepareSCT, bool prepareOrganization, bool prepareSchema, bool prepareAllDomains, string adInitError, string languagePackDir, bool languagesAvailableToInstall, bool sufficientLanguagePackDiskSpace, bool languagePacksInstalled, string alreadyInstalledUMLanguages, bool languagePackVersioning, bool activeDirectorySplitPermissions, string[] setupRoles, bool ignoreFileInUse, bool hostingDeploymentEnabled, string pathToDCHybridConfigFile, bool isDatacenter)
		{
			this.TargetDir = targetDir;
			this.ExchangeVersion = exchangeVersion;
			this.AdamPort = adamPort;
			this.AdamSSLPort = adamSSLPort;
			this.CreatePublicDB = createPublicDB;
			this.CustomerFeedbackEnabled = customerFeedbackEnabled;
			this.NewProvisionedServerName = newProvisionedServerName;
			this.RemoveProvisionedServerName = removeProvisionedServerName;
			this.GlobalCatalog = globalCatalog;
			this.DomainController = domainController;
			this.PrepareDomain = prepareDomain;
			this.PrepareSCT = prepareSCT;
			this.PrepareOrganization = prepareOrganization;
			this.PrepareSchema = prepareSchema;
			this.PrepareAllDomains = prepareAllDomains;
			this.AdInitError = adInitError;
			this.LanguagePackDir = languagePackDir;
			this.LanguagesAvailableToInstall = languagesAvailableToInstall;
			this.SufficientLanguagePackDiskSpace = sufficientLanguagePackDiskSpace;
			this.LanguagePacksInstalled = languagePacksInstalled;
			this.AlreadyInstalledUMLanguages = alreadyInstalledUMLanguages;
			this.LanguagePackVersioning = languagePackVersioning;
			this.ActiveDirectorySplitPermissions = activeDirectorySplitPermissions;
			this.SetupRoles = setupRoles;
			this.IgnoreFileInUse = ignoreFileInUse;
			this.HostingDeploymentEnabled = hostingDeploymentEnabled;
			this.PathToDCHybridConfigFile = pathToDCHybridConfigFile;
			this.IsDatacenter = isDatacenter;
		}

		public bool IsDatacenter { get; private set; }

		public string PathToDCHybridConfigFile { get; private set; }

		public string TargetDir { get; private set; }

		public Version ExchangeVersion { get; private set; }

		public int AdamPort { get; private set; }

		public int AdamSSLPort { get; private set; }

		public bool CreatePublicDB { get; private set; }

		public bool CustomerFeedbackEnabled { get; private set; }

		public string NewProvisionedServerName { get; private set; }

		public string RemoveProvisionedServerName { get; private set; }

		public string GlobalCatalog { get; private set; }

		public string DomainController { get; private set; }

		public string PrepareDomain { get; private set; }

		public bool PrepareSCT { get; private set; }

		public bool PrepareOrganization { get; private set; }

		public bool PrepareSchema { get; private set; }

		public bool PrepareAllDomains { get; private set; }

		public string AdInitError { get; private set; }

		public string LanguagePackDir { get; private set; }

		public bool LanguagesAvailableToInstall { get; private set; }

		public bool SufficientLanguagePackDiskSpace { get; private set; }

		public bool LanguagePacksInstalled { get; private set; }

		public string AlreadyInstalledUMLanguages { get; private set; }

		public bool LanguagePackVersioning { get; private set; }

		public bool ActiveDirectorySplitPermissions { get; private set; }

		public string[] SetupRoles { get; private set; }

		public bool IgnoreFileInUse { get; private set; }

		public bool HostingDeploymentEnabled { get; private set; }
	}
}
