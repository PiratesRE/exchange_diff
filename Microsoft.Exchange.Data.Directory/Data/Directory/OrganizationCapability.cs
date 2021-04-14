using System;

namespace Microsoft.Exchange.Data.Directory
{
	public enum OrganizationCapability
	{
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityClientExtensions)]
		ClientExtensions = 44,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityGMGen)]
		GMGen = 43,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMailRouting)]
		MailRouting = 47,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityOABGen)]
		OABGen = 42,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMDataStorage)]
		UMDataStorage = 41,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMGrammar)]
		UMGrammar = 40,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityUMGrammarReady)]
		UMGrammarReady = 46,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityManagement)]
		Management = 48,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityTenantUpgrade)]
		TenantUpgrade,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityScaleOut)]
		ScaleOut,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMessageTracking)]
		MessageTracking,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityPstProvider)]
		PstProvider,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilitySuiteServiceStorage)]
		SuiteServiceStorage,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityOfficeMessageEncryption)]
		OfficeMessageEncryption,
		[LocDescription(DirectoryStrings.IDs.OrganizationCapabilityMigration)]
		Migration
	}
}
