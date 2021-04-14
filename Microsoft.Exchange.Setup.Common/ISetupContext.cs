using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.CommonBase;

namespace Microsoft.Exchange.Setup.Common
{
	internal interface ISetupContext
	{
		bool? ActiveDirectorySplitPermissions { get; set; }

		ushort AdamLdapPort { get; set; }

		ushort AdamSslPort { get; set; }

		LocalizedException ADInitializationError { get; }

		bool ADInitializedSuccessfully { get; set; }

		NonRootLocalLongFullPath BackupInstalledPath { get; set; }

		Version BackupInstalledVersion { get; set; }

		bool CanOrgBeRemoved { get; set; }

		Dictionary<string, LanguageInfo> CollectedLanguagePacks { get; }

		string CurrentWizardPageName { get; set; }

		bool DisableAMFiltering { get; set; }

		string DomainController { get; set; }

		CultureInfo ExchangeCulture { get; set; }

		bool ExchangeOrganizationExists { get; set; }

		string ExchangeServerName { get; set; }

		bool? GlobalCustomerFeedbackEnabled { get; set; }

		bool HasBridgeheadServers { get; set; }

		bool HasE14OrLaterServers { get; set; }

		bool HasLegacyServers { get; set; }

		bool HasMailboxServers { get; set; }

		bool HasNewProvisionedServerParameters { get; }

		bool HasPrepareADParameters { get; }

		bool HasRemoveProvisionedServerParameters { get; }

		bool HasRolesToInstall { get; }

		bool HostingDeployment { get; }

		IndustryType Industry { get; set; }

		InstallationModes InstallationMode { get; set; }

		HashSet<string> InstalledLanguagePacks { get; }

		NonRootLocalLongFullPath InstalledPath { get; set; }

		RoleCollection InstalledRolesAD { get; set; }

		RoleCollection InstalledRolesLocal { get; set; }

		List<CultureInfo> InstalledUMLanguagePacks { get; set; }

		Version InstalledVersion { get; set; }

		bool InstallWindowsComponents { get; set; }

		bool IsBackupKeyPresent { get; set; }

		bool IsCleanMachine { get; set; }

		bool IsDatacenter { get; set; }

		bool IsDatacenterDedicated { get; set; }

		bool TreatPreReqErrorsAsWarnings { get; set; }

		bool IsDomainConfigUpdateRequired { get; }

		bool IsE12Schema { get; set; }

		bool IsFfo { get; set; }

		bool IsLanaguagePacksInstalled { get; set; }

		bool IsLanguagePackOperation { get; set; }

		bool IsLonghornServer { get; set; }

		bool IsOrgConfigUpdateRequired { get; set; }

		bool IsPartnerHosted { get; set; }

		bool IsProvisionedServer { get; }

		bool IsRestoredFromPreviousState { get; set; }

		bool IsSchemaUpdateRequired { get; set; }

		bool IsServerFoundInAD { get; }

		bool IsUmLanguagePackOperation { get; set; }

		bool IsW3SVCStartOk { get; }

		LongPath LanguagePackPath { get; set; }

		bool LanguagePackSourceIsBundle { get; }

		Dictionary<string, Array> LanguagePacksToInstall { get; }

		Dictionary<string, long> LanguagesToInstall { get; }

		bool NeedToUpdateLanguagePacks { get; set; }

		string NewProvisionedServerName { get; }

		IOrganizationName OrganizationName { get; set; }

		IOrganizationName OrganizationNameFoundInAD { get; set; }

		LocalizedException OrganizationNameValidationException { get; set; }

		bool? OriginalGlobalCustomerFeedbackEnabled { get; set; }

		IndustryType OriginalIndustry { get; set; }

		bool? OriginalServerCustomerFeedbackEnabled { get; set; }

		Dictionary<string, object> ParsedArguments { get; set; }

		RoleCollection PartiallyConfiguredRoles { get; set; }

		string RemoveProvisionedServerName { get; }

		RoleCollection RequestedRoles { get; set; }

		Version RunningVersion { get; set; }

		List<CultureInfo> SelectedCultures { get; set; }

		bool? ServerCustomerFeedbackEnabled { get; set; }

		LongPath SourceDir { get; set; }

		Dictionary<string, LanguageInfo> SourceLanguagePacks { get; }

		bool StartTransportService { get; set; }

		NonRootLocalLongFullPath TargetDir { get; set; }

		RoleCollection UnpackedDatacenterRoles { get; set; }

		RoleCollection UnpackedRoles { get; set; }

		LongPath UpdatesDir { get; set; }

		bool WatsonEnabled { get; set; }

		LocalizedException RegistryError { get; }

		string TenantOrganizationConfig { get; }

		bool IsInstalledAD(string roleName);

		bool IsInstalledLocal(string roleName);

		bool IsInstalledLocalOrAD(string roleName);

		bool IsPartiallyConfigured(string roleName);

		bool IsRequested(string roleName);

		bool IsUnpacked(string roleName);

		void UpdateIsW3SVCStartOk();

		bool IsUnpackedOrInstalledAD(string roleName);

		IOrganizationName ParseOrganizationName(string name);
	}
}
