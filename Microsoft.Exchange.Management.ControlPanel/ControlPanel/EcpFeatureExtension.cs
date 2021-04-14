using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class EcpFeatureExtension
	{
		static EcpFeatureExtension()
		{
			new EcpFeatureDescriptor(EcpFeature.DefaultPage, "~/default.aspx", "~/", false).Register();
			new EcpFeatureDescriptor(EcpFeature.AdminRbac, "~/UsersGroups/AdminRoleGroups.slab", "~/?p=AdminRoleGroups", false).Register();
			new EcpFeatureDescriptor(EcpFeature.MessageTracking, "~/Reporting/DeliveryReports.slab", "~/?p=AdminDeliveryReports", false).Register();
			new EcpFeatureDescriptor(EcpFeature.Mailboxes, "~/UsersGroups/Mailboxes.slab", "~/?p=Mailboxes", false).Register();
			new EcpFeatureDescriptor(EcpFeature.MailboxSearches, "~/Reporting/MailboxSearches.aspx", "~/?p=MailboxSearches", false).Register();
			new EcpFeatureDescriptor(EcpFeature.TransportRules, "~/RulesEditor/TransportRules.slab", "~/?p=TransportRules", false).Register();
			new EcpFeatureDescriptor(EcpFeature.EmailMigration, "~/Migration/MigrationBatches.slab", "~/?p=EmailMigration", false).Register();
			new EcpFeatureDescriptor(EcpFeature.UserRoles, "~/UsersGroups/RoleAssignmentPolicies.slab", "~/?p=UserRoles", false).Register();
			new EcpFeatureDescriptor(EcpFeature.UMManagement, "~/UnifiedMessaging/UMDialPlanService.slab", "~/?p=UMDialPlan", false).Register();
			new EcpFeatureDescriptor(EcpFeature.ResourceMailboxes, "~/UsersGroups/Mailboxes.slab", "~/?p=Mailboxes&q=vw%3dRoomMailbox", false).Register();
			new EcpFeatureDescriptor(EcpFeature.MailboxPropertyPage, "~/UsersGroups/EditMailbox.aspx", "~/UsersGroups/EditMailbox.aspx?id=RegisterFeature(objectid}", false).Register();
			new EcpFeatureDescriptor(EcpFeature.GroupPropertyPage, "~/UsersGroups/EditDistributionGroup.aspx", "~/UsersGroups/EditDistributionGroup.aspx?id=RegisterFeature(objectid}", false).Register();
			new EcpFeatureDescriptor(EcpFeature.ContactPropertyPage, "~/UsersGroups/EditContact.aspx", "~/UsersGroups/EditContact.aspx?id=RegisterFeature(objectid}", false).Register();
			new EcpFeatureDescriptor(EcpFeature.UMCallSummaryReport, "~/UnifiedMessaging/UMCallSummaryReport.aspx", "~/UnifiedMessaging/UMCallSummaryReport.aspx", true).Register();
			new EcpFeatureDescriptor(EcpFeature.UserCallLogs, "~/UnifiedMessaging/UMCallDataRecord.aspx", "~/UnifiedMessaging/UMCallDataRecord.aspx", true).Register();
			new EcpFeatureDescriptor(EcpFeature.DistributionGroups, "~/UsersGroups/DistributionGroups.slab", "~/?p=DistributionGroups", false).Register();
			new EcpFeatureDescriptor(EcpFeature.Contacts, "~/UsersGroups/Contacts.slab", "~/?p=Contacts", false).Register();
			new EcpFeatureDescriptor(EcpFeature.InstallExtensionCallBack, ExtensionData.OfficeCallBackUrl, ExtensionData.OfficeCallBackUrl, false).Register();
			new EcpFeatureDescriptor(EcpFeature.LinkedInSetup, "~/Connect/LinkedInSetup.aspx", "~/Connect/LinkedInSetup.aspx", false).Register();
			new EcpFeatureDescriptor(EcpFeature.FacebookSetup, "~/Connect/FacebookSetup.aspx", "~/Connect/FacebookSetup.aspx", false).Register();
			new EcpFeatureDescriptor(EcpFeature.TeamMailbox, "~/TeamMailbox/TeamMailbox.slab", "~/TeamMailbox/TeamMailbox.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.TeamMailboxCreating, "~/TeamMailbox/NewSharePointTeamMailbox.aspx", "~/TeamMailbox/NewSharePointTeamMailbox.aspx", true).Register();
			new EcpFeatureDescriptor(EcpFeature.TeamMailboxEditing, "~/TeamMailbox/EditTeamMailbox.aspx", "~/TeamMailbox/EditTeamMailbox.aspx", true).Register();
			new EcpFeatureDescriptor(EcpFeature.OrgInstallExtensionCallBack, "~/Extension/OrgInstallFromURL.slab", "~/Extension/OrgInstallFromURL.slab", false).Register();
			new EcpFeatureDescriptor(EcpFeature.Onboarding, "~/Migration/NewMigrationBatch.aspx", "~/Migration/NewMigrationBatch.aspx?migration=onboarding", true).Register();
			new EcpFeatureDescriptor(EcpFeature.SetupHybridConfiguration, "~/Hybrid/HybridConfiguration.slab", "~/Hybrid/HybridConfiguration.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.AntiMalwarePolicy, "~/Antimalware/Antimalware.slab", "~/Antimalware/Antimalware.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.SpamConnectionFilter, "~/Antispam/ConnectionFilter.slab", "~/Antispam/ConnectionFilter.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.SpamContentFilter, "~/Antispam/SpamContentFilter.slab", "~/Antispam/SpamContentFilter.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.OutboundSpam, "~/Antispam/OutboundSpam.slab", "~/Antispam/OutboundSpam.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.DLPPolicy, "~/DLPPolicy/ExoDLPPolicy.slab", "~/DLPPolicy/ExoDLPPolicy.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.AdminAuditing, "~/Reporting/AuditReports.slab", "~/Reporting/AuditReports.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.SharedMailboxes, "~/UsersGroups/SharedMailboxes.slab", "~/?p=SharedMailboxes", false).Register();
			new EcpFeatureDescriptor(EcpFeature.PublicFolders, "~/PublicFolders/PublicFolders.slab", "~/PublicFolders/PublicFolders.slab", true).Register();
			new EcpFeatureDescriptor(EcpFeature.FFOMigrationStatus, "~/Migration/FFOMigrationStatus.aspx", "~/Migration/FFOMigrationStatus.aspx", true).Register();
			new EcpFeatureDescriptor(EcpFeature.MessageTrace, "~/MessageTrace/MessageTrace.slab", "~/MessageTrace/MessageTrace.slab", true).Register();
		}

		public static EcpFeatureDescriptor GetFeatureDescriptor(this EcpFeature ecpFeature)
		{
			return EcpFeatureExtension.ecpFeatures[ecpFeature];
		}

		public static string GetName(this EcpFeature ecpFeature)
		{
			return ecpFeature.ToString();
		}

		private static void Register(this EcpFeatureDescriptor feature)
		{
			EcpFeatureExtension.ecpFeatures.Add(feature.Id, feature);
		}

		private static readonly Dictionary<EcpFeature, EcpFeatureDescriptor> ecpFeatures = new Dictionary<EcpFeature, EcpFeatureDescriptor>(37);
	}
}
