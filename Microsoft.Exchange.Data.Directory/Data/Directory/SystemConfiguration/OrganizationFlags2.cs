using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum OrganizationFlags2
	{
		None = 0,
		TenantRelocationsAllowed = 1,
		IsUpgradeOperationInProgress = 2,
		OfflineOrgIdEnabled = 8,
		OfflineOrgIdAsPrimaryAuth = 24,
		PublicComputersDetectionEnabled = 32,
		OpenTenantFull = 64,
		RmsoSubscriptionStatus = 896,
		MapiHttpEnabled = 1024,
		TemplateTenant = 2048,
		IntuneManagedStatus = 4096,
		HybridConfigurationStatus = 122880,
		OAuth2ClientProfileEnabled = 262144,
		ACLableSyncedObjectEnabled = 2097152
	}
}
