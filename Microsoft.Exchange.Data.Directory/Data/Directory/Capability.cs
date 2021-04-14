using System;

namespace Microsoft.Exchange.Data.Directory
{
	public enum Capability
	{
		[LocDescription(DirectoryStrings.IDs.CapabilityNone)]
		None,
		[SKUCapability("4a82b400-a79f-41a4-b4e2-e94f5787b113")]
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSDeskless)]
		BPOS_S_Deskless,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSStandard)]
		[SKUCapability("9aaf7827-d63c-4b61-89c3-182f06f82e5c")]
		BPOS_S_Standard,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSEnterprise)]
		[SKUCapability("efb87545-963c-4e0d-99df-69c6916d9eb0")]
		BPOS_S_Enterprise,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSArchive)]
		[SKUCapability("da040e0a-b393-4bea-bb76-928b3fa1cf5a", AddOnSKU = true)]
		BPOS_S_Archive = 5,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSLite)]
		[SKUCapability("d42bdbd6-c335-4231-ab3d-c8f348d5aff5")]
		BPOS_L_Standard,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSBasic)]
		[SKUCapability("90927877-dcff-4af6-b346-2332c0b15bb7")]
		BPOS_B_Standard,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSBasicCustomDomain)]
		[SKUCapability("e4ed42b9-801e-4374-bffa-9bca1d5ceb28")]
		BPOS_B_CustomDomain,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSMidSize)]
		[SKUCapability("FC52CC4B-ED7D-472d-BBE7-B081C23ECC56")]
		BPOS_S_MidSize,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityBPOSSArchiveAddOn)]
		[SKUCapability("176a09a6-7ec5-4039-ac02-b2791c6ba793", AddOnSKU = true)]
		BPOS_S_ArchiveAddOn,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityEOPStandardAddOn)]
		[SKUCapability("326e2b78-9d27-42c9-8509-46c827743a17", AddOnSKU = true)]
		BPOS_S_EopStandardAddOn,
		[SKUCapability("75badc48-628e-4446-8460-41344d73abd6", AddOnSKU = true)]
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityEOPPremiumAddOn)]
		BPOS_S_EopPremiumAddOn,
		[LocDescription(DirectoryStrings.IDs.SKUCapabilityUnmanaged)]
		[SKUCapability("6C5DEA44-8DA9-4EA5-A8BF-AF72ED983FAC")]
		BPOS_Unmanaged,
		[LocDescription(DirectoryStrings.IDs.CapabilityTOUSigned)]
		TOU_Signed = 32,
		[LocDescription(DirectoryStrings.IDs.CapabilityFederatedUser)]
		FederatedUser,
		[LocDescription(DirectoryStrings.IDs.CapabilityPartnerManaged)]
		Partner_Managed,
		[LocDescription(DirectoryStrings.IDs.CapabilityMasteredOnPremise)]
		MasteredOnPremise,
		[LocDescription(DirectoryStrings.IDs.CapabilityResourceMailbox)]
		ResourceMailbox,
		[LocDescription(DirectoryStrings.IDs.CapabilityExcludedFromBackSync)]
		ExcludedFromBackSync,
		[LocDescription(DirectoryStrings.IDs.CapabilityUMFeatureRestricted)]
		UMFeatureRestricted,
		[LocDescription(DirectoryStrings.IDs.CapabilityRichCoexistence)]
		RichCoexistence,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMGrammar)]
		OrganizationCapabilityUMGrammar,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMDataStorage)]
		OrganizationCapabilityUMDataStorage,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityOABGen)]
		OrganizationCapabilityOABGen,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityGMGen)]
		OrganizationCapabilityGMGen,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityClientExtensions)]
		OrganizationCapabilityClientExtensions,
		[LocDescription(DirectoryStrings.IDs.CapabilityBEVDirLockdown)]
		BEVDirLockdown,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMGrammarReady)]
		OrganizationCapabilityUMGrammarReady,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMailRouting)]
		OrganizationCapabilityMailRouting,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityManagement)]
		OrganizationCapabilityManagement,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityTenantUpgrade)]
		OrganizationCapabilityTenantUpgrade,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityScaleOut)]
		OrganizationCapabilityScaleOut,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMessageTracking)]
		OrganizationCapabilityMessageTracking,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityPstProvider)]
		OrganizationCapabilityPstProvider,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilitySuiteServiceStorage)]
		OrganizationCapabilitySuiteServiceStorage,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityOfficeMessageEncryption)]
		OrganizationCapabilityOfficeMessageEncryption,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMigration)]
		OrganizationCapabilityMigration
	}
}
