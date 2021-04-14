using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum OrganizationProvisioningFlags
	{
		None = 0,
		EnableAsSharedConfiguration = 1,
		EnableLicensingEnforcement = 2,
		ExcludedFromBackSync = 4,
		EhfAdminAccountSyncEnabled = 8,
		AllowDeleteOfExternalIdentityUponRemove = 16,
		HostingDeploymentEnabled = 32,
		UseServicePlanAsCounterInstanceName = 64,
		Dehydrated = 128,
		Static = 256,
		AppsForOfficeDisabled = 512,
		ImmutableConfiguration = 1024,
		ExcludedFromForwardSyncEDU2BPOS = 2048,
		EDUEnabled = 4096,
		MSOEnabled = 8192,
		ForwardSyncEnabled = 16384,
		GuidPrefixedLegacyDn = 32768,
		MailboxForcedReplicationDisabled = 65536,
		SyncPropertySetUpgradeAllowed = 131072,
		ProcessEhaMigratedMessagesEnabled = 524288,
		PilotingOrganization = 1048576,
		IsTenantAccessBlocked = 4194304,
		IsUpgradeOperationInProgress = 8388608
	}
}
