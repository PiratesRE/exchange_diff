using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class Dedicated_RoleGroupDefinition
	{
		internal static RoleGroupRoleMapping[] Definition = new RoleGroupRoleMapping[]
		{
			new RoleGroupRoleMapping("Capacity Destructive Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DataCenterDestructiveOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0200.000", false)
			}),
			new RoleGroupRoleMapping("Compliance Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.InformationRightsManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"IRMPremiumFeaturesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RetentionManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyAuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0145.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Customer Change Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EdgeSubscriptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EmailAddressPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.Regular, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationTransportSettings, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RemoteAndAcceptedDomains, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SendConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"TeamMailboxPermissions"
				}, "15.00.0199.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.WorkloadManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0112.000", false)
			}),
			new RoleGroupRoleMapping("Customer Data Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.AddressLists, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.AuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CmdletExtensionAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseAvailabilityGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseCopies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Databases, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DisasterRecovery, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EdgeSubscriptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EmailAddressPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServerCertificates, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServers, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeVirtualDirectories, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.FederatedSharing, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.GALSynchronizationManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"GALSyncEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.InformationRightsManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"IRMPremiumFeaturesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.Regular, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LawEnforcementRequests, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHold, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailTips, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MailTipsPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxImportExport, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MailboxImportExportRegularRoleAssignmentEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxSearch, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationClientAccess, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationTransportSettings, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.POP3AndIMAP4Protocols, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RemoteAndAcceptedDomains, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RetentionManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SecurityGroupCreationAndMembership, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SendConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Supervision, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SupervisionPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMPrompts, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnifiedMessaging, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UserOptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyAuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0145.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Customer Destructive Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DataCenterDestructiveOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0200.000", false)
			}),
			new RoleGroupRoleMapping("Customer PII Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.PersonallyIdentifiableInformation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0330.000", false)
			}),
			new RoleGroupRoleMapping("DataCenter Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.ActiveDirectoryPermissions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.AddressLists, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.AuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CmdletExtensionAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Custom, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0859.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseAvailabilityGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseCopies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Databases, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DisasterRecovery, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EdgeSubscriptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EmailAddressPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServerCertificates, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServers, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeVirtualDirectories, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.FederatedSharing, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.GALSynchronizationManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"GALSyncEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.InformationRightsManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"IRMPremiumFeaturesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.Regular, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHold, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailTips, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MailTipsPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationClientAccess, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationTransportSettings, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.POP3AndIMAP4Protocols, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RemoteAndAcceptedDomains, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SecurityGroupCreationAndMembership, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SendConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Supervision, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SupervisionPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"TeamMailboxPermissions"
				}, "15.00.0199.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMPrompts, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnifiedMessaging, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UserOptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyAuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0145.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Destructive Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.CentralAdminCredentialManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0158.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterDestructiveOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0200.000", false)
			}),
			new RoleGroupRoleMapping("Discovery Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.LawEnforcementRequests, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHold, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxSearch, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Help Desk", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UserOptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Hygiene Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.ApplicationImpersonation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"ApplicationImpersonationRegularRoleAssignmentEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Management Destructive Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.CentralAdminManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.Reporting, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminSupport, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Management Forest Monitoring", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Reporting, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Management Forest Operator", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.Reporting, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminSupport, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Management Forest Tier 1 Support", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminSupport, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Networking Change Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false)
			}),
			new RoleGroupRoleMapping("Organization Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.ActiveDirectoryPermissions, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ActiveDirectoryPermissions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ActiveMonitoring, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.ActiveMonitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.AddressLists, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0584.000", false),
				new RoleAssignmentDefinition(RoleType.AddressLists, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ApplicationImpersonation, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"ApplicationImpersonationEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ArchiveApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0285.000", false),
				new RoleAssignmentDefinition(RoleType.AuditLogs, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.AuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CentralAdminCredentialManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0158.000", false),
				new RoleAssignmentDefinition(RoleType.CentralAdminCredentialManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0158.000", false),
				new RoleAssignmentDefinition(RoleType.CentralAdminManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CentralAdminManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CmdletExtensionAgents, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.CmdletExtensionAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Custom, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0859.000", false),
				new RoleAssignmentDefinition(RoleType.Custom, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0859.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterDestructiveOperations, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.01.0200.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterDestructiveOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0200.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterOperations, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DataCenterOperations, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.DataLossPrevention, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0256.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseAvailabilityGroups, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseAvailabilityGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseCopies, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DatabaseCopies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Databases, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Databases, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DisasterRecovery, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DisasterRecovery, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EdgeSubscriptions, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EdgeSubscriptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EmailAddressPolicies, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.EmailAddressPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeCrossServiceIntegration, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0696.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServerCertificates, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServerCertificates, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServers, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServers, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeVirtualDirectories, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeVirtualDirectories, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.FederatedSharing, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.FederatedSharing, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.GALSynchronizationManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"GALSyncEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.GALSynchronizationManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"GALSyncEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.InformationRightsManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"IRMPremiumFeaturesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.InformationRightsManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"IRMPremiumFeaturesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.Regular, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LawEnforcementRequests, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHold, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHold, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.LegalHoldApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0285.000", false),
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailTips, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"MailTipsPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailTips, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MailTipsPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxImportExport, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxSearch, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"SearchMessagePermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailboxSearchApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0453.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.NetworkingManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0708.000", false),
				new RoleAssignmentDefinition(RoleType.OfficeExtensionApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0285.000", false),
				new RoleAssignmentDefinition(RoleType.OrgCustomApps, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0469.000", false),
				new RoleAssignmentDefinition(RoleType.OrgCustomApps, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0469.000", false),
				new RoleAssignmentDefinition(RoleType.OrgMarketplaceApps, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0469.000", false),
				new RoleAssignmentDefinition(RoleType.OrgMarketplaceApps, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0469.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationClientAccess, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationClientAccess, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationConfiguration, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationTransportSettings, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.OrganizationTransportSettings, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.POP3AndIMAP4Protocols, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.POP3AndIMAP4Protocols, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PartnerDelegatedTenantManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PersonallyIdentifiableInformation, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0330.000", false),
				new RoleAssignmentDefinition(RoleType.PublicFolders, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RemoteAndAcceptedDomains, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RemoteAndAcceptedDomains, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Reporting, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RetentionManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RetentionManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SecurityGroupCreationAndMembership, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SecurityGroupCreationAndMembership, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SendConnectors, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SendConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Supervision, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"SupervisionPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Supervision, RoleAssignmentDelegationType.Regular, new string[]
				{
					"SupervisionPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SupportDiagnostics, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SupportDiagnostics, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxLifecycleApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0321.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxes, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"TeamMailboxPermissions"
				}, "15.00.0199.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"TeamMailboxPermissions"
				}, "15.00.0199.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportAgents, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.TransportHygiene, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0226.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMMailboxes, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMPrompts, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMPrompts, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnScopedRoleManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnifiedMessaging, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnifiedMessaging, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UserApplication, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "15.00.0285.000", false),
				new RoleAssignmentDefinition(RoleType.UserOptions, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UserOptions, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyAuditLogs, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.01.0145.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyAuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.01.0145.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminSupport, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"PermissionManagementEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRoleManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.WorkloadManagement, RoleAssignmentDelegationType.DelegatingOrgWide, new string[]
				{
					"*"
				}, "15.00.0112.000", false),
				new RoleAssignmentDefinition(RoleType.WorkloadManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0112.000", false)
			}),
			new RoleGroupRoleMapping("Public Folder Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.MailEnabledPublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.PublicFolders, RoleAssignmentDelegationType.Regular, new string[]
				{
					"PublicFoldersEnabled"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Recipient Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DistributionGroups, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipientCreation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MailRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Migration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MoveMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RecipientPolicies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ResetPassword, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TeamMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"TeamMailboxPermissions"
				}, "15.00.0199.000", false)
			}),
			new RoleGroupRoleMapping("Records Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.AuditLogs, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Journaling, RoleAssignmentDelegationType.Regular, new string[]
				{
					"JournalingRulesPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.MessageTracking, RoleAssignmentDelegationType.Regular, new string[]
				{
					"MessageTrackingPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.RetentionManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportRules, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("Server Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.DatabaseCopies, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Databases, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServerCertificates, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeServers, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ExchangeVirtualDirectories, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.POP3AndIMAP4Protocols, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ReceiveConnectors, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.TransportQueues, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("UM Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.UMMailboxes, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UMPrompts, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.UnifiedMessaging, RoleAssignmentDelegationType.Regular, new string[]
				{
					"UMPermissions"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("View-Only Local Server Access", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("View-Only Mgmt Forest Operator", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.Reporting, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyCentralAdminManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRoleManagement, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("View-Only Organization Management", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.Monitoring, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.SupportDiagnostics, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyConfiguration, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false),
				new RoleAssignmentDefinition(RoleType.ViewOnlyRecipients, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "14.00.0583.000", false)
			}),
			new RoleGroupRoleMapping("View-Only PII", new RoleAssignmentDefinition[]
			{
				new RoleAssignmentDefinition(RoleType.PersonallyIdentifiableInformation, RoleAssignmentDelegationType.Regular, new string[]
				{
					"*"
				}, "15.00.0330.000", false)
			})
		};
	}
}
