using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal enum EcpFeature
	{
		DefaultPage,
		AdminRbac,
		MessageTracking,
		Mailboxes,
		MailboxSearches,
		TransportRules,
		EmailMigration,
		UserRoles,
		UMManagement,
		ResourceMailboxes,
		MailboxPropertyPage,
		GroupPropertyPage,
		ContactPropertyPage,
		UMCallSummaryReport,
		UserCallLogs,
		DistributionGroups,
		Contacts,
		InstallExtensionCallBack,
		LinkedInSetup,
		FacebookSetup,
		TeamMailbox,
		TeamMailboxCreating,
		TeamMailboxEditing,
		OrgInstallExtensionCallBack = 24,
		Onboarding,
		SetupHybridConfiguration,
		AntiMalwarePolicy,
		SpamConnectionFilter,
		DLPPolicy,
		AdminAuditing,
		SpamContentFilter,
		OutboundSpam,
		SharedMailboxes,
		PublicFolders,
		FFOMigrationStatus,
		MessageTrace,
		MaxValue = 36
	}
}
