using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RoleType
	{
		[Description("Custom")]
		Custom = -2147483648,
		[Description("UnScoped")]
		UnScoped = 256,
		[Obsolete("Role type deprecated. Use instead: Mail Recipients,Federated Sharing,Database Availability Groups,Databases,Public Folders,Address Lists,CAS Mailbox Policies,Disaster Recovery,Monitoring,Database Copies,Unified Messaging,Journaling,Remote and Accepted Domains,E-Mail Address Policies,Transport Rules,Send Connectors,Edge Subscriptions,Organization Transport Settings,Exchange Servers,Exchange Virtual Directories,Exchange Server Certificates,POP3 And IMAP4 Protocols,Receive Connectors,UM Mailboxes,User Options,Security Group Creation and Membership,Mail Recipient Creation,Message Tracking,Role Management,View-Only Recipients,View-Only Configuration,Distribution Groups,Mail Enabled Public Folders,Move Mailboxes,Reset Password,Retention Management,Audit Logs,Support Diagnostics,Mailbox Search,Information Rights Management,Legal Hold,Mail Tips,Public Folder Replication,Active Directory Permissions,UM Prompts,Migration,Transport Hygiene,DataCenter Operations,Transport Queues,Supervision,Cmdlet Extension Agents,Organization Configuration,Organization Client Access,Exchange Connectors,Transport Agents")]
		[Description("OrganizationManagement")]
		OrganizationManagement = 1,
		[Description("RecipientManagement")]
		[Obsolete("Role type deprecated. Use instead: Mail Recipients,Distribution Groups,Mail Enabled Public Folders,Move Mailboxes,Reset Password,Mail Recipient Creation,Message Tracking,CAS Mailbox Policies,Migration.")]
		RecipientManagement = 3,
		[Description("ViewOnlyOrganizationManagement")]
		[Obsolete("Role type deprecated. Use instead: View-Only Recipients, View-Only Configuration or Monitoring.")]
		ViewOnlyOrganizationManagement,
		[Description("DistributionGroupManagement")]
		DistributionGroupManagement,
		[Description("MyDistributionGroups")]
		MyDistributionGroups,
		[Description("MyDistributionGroupMembership")]
		MyDistributionGroupMembership,
		[Obsolete("Role type deprecated. Use instead: Unified Messaging, UM Mailboxes, UM Prompts.")]
		[Description("UmManagement")]
		UmManagement,
		[Description("RecordsManagement")]
		[Obsolete("Role type deprecated. Use instead: Retention Management, Journaling, Transport Rules, Message Tracking.")]
		RecordsManagement,
		[Description("MyBaseOptions")]
		MyBaseOptions,
		[Description("UmRecipientManagement")]
		[Obsolete("Role type deprecated. Use instead: Unified Messaging, UM Mailboxes, UM Prompts.")]
		UmRecipientManagement,
		[Description("HelpdeskRecipientManagement")]
		HelpdeskRecipientManagement,
		[Description("GALSynchronizationManagement")]
		GALSynchronizationManagement,
		[Description("ApplicationImpersonation")]
		ApplicationImpersonation,
		[Obsolete("Role type deprecated. Use instead: Unified Messaging, UM Mailboxes, UM Prompts.")]
		[Description("UMPromptManagement")]
		UMPromptManagement,
		[Description("PartnerDelegatedTenantManagement")]
		PartnerDelegatedTenantManagement,
		[Obsolete("Role type deprecated. Use instead: Mailbox Search, Legal Hold.")]
		[Description("DiscoveryManagement")]
		DiscoveryManagement,
		[Description("CentralAdminManagement")]
		CentralAdminManagement,
		[Description("UnScopedRoleManagement")]
		UnScopedRoleManagement,
		[Description("MyContactInformation")]
		MyContactInformation,
		[Description("MyProfileInformation")]
		MyProfileInformation,
		[Description("MyVoiceMail")]
		MyVoiceMail,
		[Description("MyTextMessaging")]
		MyTextMessaging,
		[Description("MyMailSubscriptions")]
		MyMailSubscriptions,
		[Description("MyRetentionPolicies")]
		MyRetentionPolicies,
		[Obsolete("Role type deprecated. Use instead: MyBaseOptions, MyContactInformation, MyProfileInformation, MyVoiceMail, MyTextMessaging, MyMailSubscriptions or MyRetentionPolicies.")]
		[Description("MyOptions")]
		MyOptions = 2,
		[Description("Mail Recipients")]
		MailRecipients = 26,
		[Description("Federated Sharing")]
		FederatedSharing,
		[Description("Database Availability Groups")]
		DatabaseAvailabilityGroups,
		[Description("Databases")]
		Databases,
		[Description("Public Folders")]
		PublicFolders,
		[Description("Address Lists")]
		AddressLists,
		[Description("Recipient Policies")]
		RecipientPolicies,
		[Description("Disaster Recovery")]
		DisasterRecovery,
		[Description("Monitoring")]
		Monitoring,
		[Description("Database Copies")]
		DatabaseCopies,
		[Description("Unified Messaging")]
		UnifiedMessaging,
		[Description("Journaling")]
		Journaling,
		[Description("Remote and Accepted Domains")]
		RemoteAndAcceptedDomains,
		[Description("E-Mail Address Policies")]
		EmailAddressPolicies,
		[Description("Transport Rules")]
		TransportRules,
		[Description("Send Connectors")]
		SendConnectors,
		[Description("Edge Subscriptions")]
		EdgeSubscriptions,
		[Description("Organization Transport Settings")]
		OrganizationTransportSettings,
		[Description("Exchange Servers")]
		ExchangeServers,
		[Description("Exchange Virtual Directories")]
		ExchangeVirtualDirectories,
		[Description("Exchange Server Certificates")]
		ExchangeServerCertificates,
		[Description("POP3 And IMAP4 Protocols")]
		POP3AndIMAP4Protocols,
		[Description("Receive Connectors")]
		ReceiveConnectors,
		[Description("UM Mailboxes")]
		UMMailboxes,
		[Description("User Options")]
		UserOptions,
		[Description("Security Group Creation and Membership")]
		SecurityGroupCreationAndMembership,
		[Description("Mail Recipient Creation")]
		MailRecipientCreation,
		[Description("Message Tracking")]
		MessageTracking,
		[Description("Role Management")]
		RoleManagement,
		[Description("View-Only Recipients")]
		ViewOnlyRecipients,
		[Description("View-Only Configuration")]
		ViewOnlyConfiguration,
		[Description("Distribution Groups")]
		DistributionGroups,
		[Description("Mail Enabled Public Folders")]
		MailEnabledPublicFolders,
		[Description("Move Mailboxes")]
		MoveMailboxes,
		[Description("WorkloadManagement")]
		WorkloadManagement,
		[Description("Reset Password")]
		ResetPassword,
		[Description("Audit Logs")]
		AuditLogs,
		[Description("Retention Management")]
		RetentionManagement,
		[Description("Support Diagnostics")]
		SupportDiagnostics,
		[Description("Mailbox Search")]
		MailboxSearch,
		[Description("Legal Hold")]
		LegalHold,
		[Description("Mail Tips")]
		MailTips,
		[Obsolete("Role type deprecated. Use instead: Mail Enabled Public Folders,Public Folders")]
		[Description("Public Folder Replication")]
		PublicFolderReplication,
		[Description("Active Directory Permissions")]
		ActiveDirectoryPermissions,
		[Description("UM Prompts")]
		UMPrompts,
		[Description("Migration")]
		Migration,
		[Description("DataCenter Operations")]
		DataCenterOperations,
		[Description("Transport Hygiene")]
		TransportHygiene,
		[Description("Transport Queues")]
		TransportQueues,
		[Description("Supervision")]
		Supervision,
		[Description("Cmdlet Extension Agents")]
		CmdletExtensionAgents,
		[Description("Organization Configuration")]
		OrganizationConfiguration,
		[Description("Organization Client Access")]
		OrganizationClientAccess,
		[Description("Exchange Connectors")]
		ExchangeConnectors,
		[Description("Mailbox Import Export")]
		MailboxImportExport,
		[Description("View-Only Central Admin Management")]
		ViewOnlyCentralAdminManagement,
		[Description("View-Only Central Admin Support")]
		ViewOnlyCentralAdminSupport,
		[Description("View-Only Role Management")]
		ViewOnlyRoleManagement,
		[Description("Reporting")]
		Reporting,
		[Description("View-Only Audit Logs")]
		ViewOnlyAuditLogs,
		[Description("Transport Agents")]
		TransportAgents,
		[Description("DataCenter Destructive Operations")]
		DataCenterDestructiveOperations,
		[Description("Information Rights Management")]
		InformationRightsManagement,
		[Description("Law Enforcement Requests")]
		LawEnforcementRequests,
		[Description("MyDiagnostics")]
		MyDiagnostics,
		[Description("MyMailboxDelegation")]
		MyMailboxDelegation,
		[Description("TeamMailboxes")]
		TeamMailboxes,
		[Description("MyTeamMailboxes")]
		MyTeamMailboxes,
		[Description("ActiveMonitoring")]
		ActiveMonitoring,
		[Description("DataLossPrevention")]
		DataLossPrevention,
		[Obsolete("Availability of Facebook feature is governed by OWA mailbox policy.")]
		[Description("MyFacebookEnabled")]
		MyFacebookEnabled,
		[Obsolete("Availability of LinkedIn feature is governed by OWA mailbox policy.")]
		[Description("MyLinkedInEnabled")]
		MyLinkedInEnabled,
		[Description("UserApplication")]
		UserApplication = 99,
		[Description("ArchiveApplication")]
		ArchiveApplication,
		[Description("LegalHoldApplication")]
		LegalHoldApplication,
		[Description("OfficeExtensionApplication")]
		OfficeExtensionApplication,
		[Description("TeamMailboxLifecycleApplication")]
		TeamMailboxLifecycleApplication,
		[Description("CentralAdminCredentialManagement")]
		CentralAdminCredentialManagement,
		[Description("PersonallyIdentifiableInformation")]
		PersonallyIdentifiableInformation,
		[Description("MailboxSearchApplication")]
		MailboxSearchApplication,
		[Description("MyMarketplaceApps")]
		MyMarketplaceApps,
		[Description("MyCustomApps")]
		MyCustomApps,
		[Description("OrgMarketplaceApps")]
		OrgMarketplaceApps,
		[Description("OrgCustomApps")]
		OrgCustomApps,
		[Description("ExchangeCrossServiceIntegration")]
		ExchangeCrossServiceIntegration,
		[Description("NetworkingManagement")]
		NetworkingManagement,
		[Description("Access To Customer Data - DC Only")]
		AccessToCustomerDataDCOnly,
		[Description("Datacenter Operations - DC Only")]
		DatacenterOperationsDCOnly,
		[Description("My ReadWriteMailbox Apps")]
		MyReadWriteMailboxApps
	}
}
