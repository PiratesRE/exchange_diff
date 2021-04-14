using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class CannedWebServiceRoles_Enterprise
	{
		private static RoleCmdlet[] ArchiveApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "UpdateItemInRecoverableItems", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] ExchangeCrossServiceIntegration_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "EndInstantSearchSession", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformInstantSearch", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PostModernGroupItem", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] Legal_Hold_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "GetDiscoverySearchConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetHoldOnMailboxes", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetHoldOnMailboxes", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] LegalHoldApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "GetDiscoverySearchConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetHoldOnMailboxes", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetHoldOnMailboxes", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] Mailbox_Search_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "EndInstantSearchSession", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ExportItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindMailboxStatisticsByKeywords", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetDiscoverySearchConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFileAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetNonIndexableItemDetails", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetNonIndexableItemStatistics", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetSearchableMailboxes", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformInstantSearch", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SearchMailboxes", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] MailboxSearchApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "EndInstantSearchSession", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ExportItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetDiscoverySearchConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFileAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetNonIndexableItemDetails", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetNonIndexableItemStatistics", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetSearchableMailboxes", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformInstantSearch", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SearchMailboxes", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] My_Custom_Apps_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "InstallApp", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] My_Marketplace_Apps_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "GetAppMarketplaceUrl", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] OfficeExtensionApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "CopyItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "EndInstantSearchSession", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindConversation", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetConversationItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFileAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MarkAsJunk", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MoveItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformInstantSearch", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PostModernGroupItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SendItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateItem", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] TeamMailboxLifecycleApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "SetTeamMailbox", new RoleParameters[0], "w")
		};

		private static RoleCmdlet[] UserApplication_Cmdlets = new RoleCmdlet[]
		{
			new RoleCmdlet("", "AddAggregatedAccount", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddDelegate", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddDistributionGroupToImList", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddImContactToGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddImGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddNewImContactToGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "AddNewTelUriContactToGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ApplyConversationAction", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ArchiveItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ConvertId", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CopyFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CopyItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateFolderPath", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateManagedFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateUnifiedMailbox", new RoleParameters[0], "w"),
			new RoleCmdlet("", "CreateUserConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DeleteAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DeleteFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DeleteItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DeleteUserConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DisableApp", new RoleParameters[0], "w"),
			new RoleCmdlet("", "Disconnect", new RoleParameters[0], "w"),
			new RoleCmdlet("", "DisconnectPhoneCall", new RoleParameters[0], "w"),
			new RoleCmdlet("", "EmptyFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "EndInstantSearchSession", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ExpandDL", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ExportItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindConversation", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindMessageTrackingReport", new RoleParameters[0], "w"),
			new RoleCmdlet("", "FindPeople", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetAggregatedAccount", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetAppManifests", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetBposNavBarData", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetCalendarFolders", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetCallInfo", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetClutterState", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetConversationItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetDelegate", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetEvents", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFileAttachment", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetImItemList", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetImItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetInboxRules", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetMailTips", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetMessageTrackingReport", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetNotesForPersona", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetOrganizationHierarchyForPersona", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetOwaUserConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPasswordExpirationDate", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPersona", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPersonaNotes", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPersonaOrganizationHierarchy", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPersonaPhoto", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetPhoneCallInformation", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetReminders", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetRoomLists", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetRooms", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetServerTimeZones", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetServiceConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetSharingFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetSharingMetadata", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetStreamingEvents", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUMProperties", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserAvailability", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserOofSettings", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserPhoto", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserPhoto:GET", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserRetentionPolicyTags", new RoleParameters[0], "w"),
			new RoleCmdlet("", "GetUserUnifiedGroups", new RoleParameters[0], "w"),
			new RoleCmdlet("", "IsUMEnabled", new RoleParameters[0], "w"),
			new RoleCmdlet("", "LogDatapoint", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MarkAllItemsAsRead", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MarkAsJunk", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MoveFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "MoveItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformInstantSearch", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PerformReminderAction", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PlayOnPhone", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PlayOnPhoneGreeting", new RoleParameters[0], "w"),
			new RoleCmdlet("", "PostModernGroupItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ProcessSuiteStorage", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RefreshSharingFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveAggregatedAccount", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveContactFromImList", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveDelegate", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveDistributionGroupFromImList", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveImContactFromGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "RemoveImGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ResetPIN", new RoleParameters[0], "w"),
			new RoleCmdlet("", "ResolveNames", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SendItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetAggregatedAccount", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetClutterState", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetImGroup", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetImListMigrationCompleted", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetMissedCallNotificationEnabled", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetNotificationSettings", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetOofStatus", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetPlayOnPhoneDialString", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetTelephoneAccessFolderEmail", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SetUserOofSettings", new RoleParameters[0], "w"),
			new RoleCmdlet("", "Subscribe", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SyncFolderHierarchy", new RoleParameters[0], "w"),
			new RoleCmdlet("", "SyncFolderItems", new RoleParameters[0], "w"),
			new RoleCmdlet("", "TagImContact", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UninstallApp", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UnpinTeamMailbox", new RoleParameters[0], "w"),
			new RoleCmdlet("", "Unsubscribe", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateDelegate", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateFolder", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateInboxRules", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateItem", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UpdateUserConfiguration", new RoleParameters[0], "w"),
			new RoleCmdlet("", "UploadItems", new RoleParameters[0], "w")
		};

		internal static RoleDefinition[] Definition = new RoleDefinition[]
		{
			new RoleDefinition("ArchiveApplication", RoleType.ArchiveApplication, CannedWebServiceRoles_Enterprise.ArchiveApplication_Cmdlets),
			new RoleDefinition("ExchangeCrossServiceIntegration", RoleType.ExchangeCrossServiceIntegration, CannedWebServiceRoles_Enterprise.ExchangeCrossServiceIntegration_Cmdlets),
			new RoleDefinition("Legal Hold", RoleType.LegalHold, CannedWebServiceRoles_Enterprise.Legal_Hold_Cmdlets),
			new RoleDefinition("LegalHoldApplication", RoleType.LegalHoldApplication, CannedWebServiceRoles_Enterprise.LegalHoldApplication_Cmdlets),
			new RoleDefinition("Mailbox Search", RoleType.MailboxSearch, CannedWebServiceRoles_Enterprise.Mailbox_Search_Cmdlets),
			new RoleDefinition("MailboxSearchApplication", RoleType.MailboxSearchApplication, CannedWebServiceRoles_Enterprise.MailboxSearchApplication_Cmdlets),
			new RoleDefinition("My Custom Apps", RoleType.MyCustomApps, CannedWebServiceRoles_Enterprise.My_Custom_Apps_Cmdlets),
			new RoleDefinition("My Marketplace Apps", RoleType.MyMarketplaceApps, CannedWebServiceRoles_Enterprise.My_Marketplace_Apps_Cmdlets),
			new RoleDefinition("OfficeExtensionApplication", RoleType.OfficeExtensionApplication, CannedWebServiceRoles_Enterprise.OfficeExtensionApplication_Cmdlets),
			new RoleDefinition("TeamMailboxLifecycleApplication", RoleType.TeamMailboxLifecycleApplication, CannedWebServiceRoles_Enterprise.TeamMailboxLifecycleApplication_Cmdlets),
			new RoleDefinition("UserApplication", RoleType.UserApplication, CannedWebServiceRoles_Enterprise.UserApplication_Cmdlets)
		};
	}
}
