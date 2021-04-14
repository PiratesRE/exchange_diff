using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class WebMethodMetadata
	{
		public static readonly Dictionary<string, WebMethodEntry> Entries = new Dictionary<string, WebMethodEntry>
		{
			{
				"AddAggregatedAccount",
				new WebMethodEntry("AddAggregatedAccount", "OutlookService", "AddAggregatedAccount", true, false, false, null)
			},
			{
				"AddDelegate",
				new WebMethodEntry("AddDelegate", "Delegate", "AddDelegate", true, false, false, null)
			},
			{
				"AddDistributionGroupToImList",
				new WebMethodEntry("AddDistributionGroupToImList", "People", "AddDistributionGroupToImList", true, false, false, null)
			},
			{
				"AddImContactToGroup",
				new WebMethodEntry("AddImContactToGroup", "People", "AddImContactToGroup", true, false, false, null)
			},
			{
				"AddImGroup",
				new WebMethodEntry("AddImGroup", "People", "AddImGroup", true, false, false, null)
			},
			{
				"AddNewImContactToGroup",
				new WebMethodEntry("AddNewImContactToGroup", "People", "AddNewImContactToGroup", true, false, false, null)
			},
			{
				"AddNewTelUriContactToGroup",
				new WebMethodEntry("AddNewTelUriContactToGroup", "People", "AddNewTelUriContactToGroup", true, false, false, null)
			},
			{
				"ApplyConversationAction",
				new WebMethodEntry("ApplyConversationAction", "Mailbox", "ApplyConversationAction", true, false, false, null)
			},
			{
				"ArchiveItem",
				new WebMethodEntry("ArchiveItem", "Mailbox", "ArchiveItem", true, false, false, null)
			},
			{
				"CompleteFindInGALSpeechRecognition",
				new WebMethodEntry("CompleteFindInGALSpeechRecognition", "UM", "CompleteFindInGALSpeechRecognition", false, false, false, null)
			},
			{
				"ConvertId",
				new WebMethodEntry("ConvertId", "Mailbox", "ConvertId", true, false, false, null)
			},
			{
				"CopyFolder",
				new WebMethodEntry("CopyFolder", "Mailbox", "CopyFolder", true, false, false, null)
			},
			{
				"CopyItem",
				new WebMethodEntry("CopyItem", "Mailbox", "CopyItem", true, false, false, null)
			},
			{
				"CreateAttachment",
				new WebMethodEntry("CreateAttachment", "Mailbox", "CreateAttachment", true, false, false, null)
			},
			{
				"CreateFolder",
				new WebMethodEntry("CreateFolder", "Mailbox", "CreateFolder", true, false, false, null)
			},
			{
				"CreateFolderPath",
				new WebMethodEntry("CreateFolderPath", "Mailbox", "CreateFolderPath", true, false, false, null)
			},
			{
				"CreateItem",
				new WebMethodEntry("CreateItem", "Mailbox", "CreateItem", true, false, false, null)
			},
			{
				"CreateManagedFolder",
				new WebMethodEntry("CreateManagedFolder", "Mailbox", "CreateManagedFolder", true, false, false, null)
			},
			{
				"CreateUMCallDataRecord",
				new WebMethodEntry("CreateUMCallDataRecord", "UM", "CreateUMCallDataRecord", false, false, false, null)
			},
			{
				"CreateUMPrompt",
				new WebMethodEntry("CreateUMPrompt", "UM", "CreateUMPrompt", false, false, false, null)
			},
			{
				"CreateUnifiedMailbox",
				new WebMethodEntry("CreateUnifiedMailbox", "OutlookService", "CreateUnifiedMailbox", true, false, false, null)
			},
			{
				"CreateUserConfiguration",
				new WebMethodEntry("CreateUserConfiguration", "User Configuration", "CreateUserConfiguration", true, false, false, null)
			},
			{
				"DeleteAttachment",
				new WebMethodEntry("DeleteAttachment", "Mailbox", "DeleteAttachment", true, false, false, null)
			},
			{
				"DeleteFolder",
				new WebMethodEntry("DeleteFolder", "Mailbox", "DeleteFolder", true, false, false, null)
			},
			{
				"DeleteItem",
				new WebMethodEntry("DeleteItem", "Mailbox", "DeleteItem", true, false, false, null)
			},
			{
				"DeleteUMPrompts",
				new WebMethodEntry("DeleteUMPrompts", "UM", "DeleteUMPrompts", false, false, false, null)
			},
			{
				"DeleteUserConfiguration",
				new WebMethodEntry("DeleteUserConfiguration", "User Configuration", "DeleteUserConfiguration", true, false, false, null)
			},
			{
				"DisableApp",
				new WebMethodEntry("DisableApp", "OWA Extension", "DisableApp", true, false, false, null)
			},
			{
				"Disconnect",
				new WebMethodEntry("Disconnect", "UM", "Disconnect", true, false, false, null)
			},
			{
				"DisconnectPhoneCall",
				new WebMethodEntry("DisconnectPhoneCall", "UM", "DisconnectPhoneCall", true, false, false, null)
			},
			{
				"EmptyFolder",
				new WebMethodEntry("EmptyFolder", "Mailbox", "EmptyFolder", true, false, false, null)
			},
			{
				"EndInstantSearchSession",
				new WebMethodEntry("EndInstantSearchSession", "Mailbox", "EndInstantSearchSession", true, false, false, null)
			},
			{
				"ExecuteDiagnosticMethod",
				new WebMethodEntry("ExecuteDiagnosticMethod", "Utilities", "ExecuteDiagnosticMethod", true, false, false, null)
			},
			{
				"ExpandDL",
				new WebMethodEntry("ExpandDL", "Directory", "ExpandDL", true, false, false, null)
			},
			{
				"ExportItems",
				new WebMethodEntry("ExportItems", "Mailbox", "ExportItems", true, false, false, null)
			},
			{
				"FindConversation",
				new WebMethodEntry("FindConversation", "Mailbox", "FindConversation", true, false, false, null)
			},
			{
				"FindFolder",
				new WebMethodEntry("FindFolder", "Mailbox", "FindFolder", true, false, false, null)
			},
			{
				"FindItem",
				new WebMethodEntry("FindItem", "Mailbox", "FindItem", true, false, false, null)
			},
			{
				"FindMailboxStatisticsByKeywords",
				new WebMethodEntry("FindMailboxStatisticsByKeywords", "Compliance", "FindMailboxStatisticsByKeywords", true, true, false, "MailboxSearch")
			},
			{
				"FindMessageTrackingReport",
				new WebMethodEntry("FindMessageTrackingReport", "Mailbox", "FindMessageTrackingReport", true, false, false, null)
			},
			{
				"FindPeople",
				new WebMethodEntry("FindPeople", "People", "FindPeople", true, false, false, null)
			},
			{
				"GetAggregatedAccount",
				new WebMethodEntry("GetAggregatedAccount", "OutlookService", "GetAggregatedAccount", true, false, false, null)
			},
			{
				"GetAppManifests",
				new WebMethodEntry("GetAppManifests", "OWA Extension", "GetAppManifests", true, false, false, null)
			},
			{
				"GetAppMarketplaceUrl",
				new WebMethodEntry("GetAppMarketplaceUrl", "OWA Extension", "GetAppMarketplaceUrl", true, true, false, "MyMarketplaceApps")
			},
			{
				"GetAttachment",
				new WebMethodEntry("GetAttachment", "Mailbox", "GetAttachment", true, false, false, null)
			},
			{
				"GetBposNavBarData",
				new WebMethodEntry("GetBposNavBarData", "User Configuration", "GetBposNavBarData", true, false, false, null)
			},
			{
				"GetCalendarFolders",
				new WebMethodEntry("GetCalendarFolders", "Calendar", "GetCalendarFolders", true, false, false, null)
			},
			{
				"GetCallInfo",
				new WebMethodEntry("GetCallInfo", "UM", "GetCallInfo", true, false, false, null)
			},
			{
				"GetClientAccessToken",
				new WebMethodEntry("GetClientAccessToken", "OWA Extension", "GetClientAccessToken", true, false, false, null)
			},
			{
				"GetClientExtension",
				new WebMethodEntry("GetClientExtension", "OWA Extension", "GetClientExtension", false, false, false, null)
			},
			{
				"GetClientIntent",
				new WebMethodEntry("GetClientIntent", "Calendar", "GetClientIntent", false, false, false, null)
			},
			{
				"GetClutterState",
				new WebMethodEntry("GetClutterState", "Inference", "GetClutterState", true, false, false, null)
			},
			{
				"GetConversationItems",
				new WebMethodEntry("GetConversationItems", "Mailbox", "GetConversationItems", true, false, false, null)
			},
			{
				"GetDelegate",
				new WebMethodEntry("GetDelegate", "Delegate", "GetDelegate", true, false, false, null)
			},
			{
				"GetDiscoverySearchConfiguration",
				new WebMethodEntry("GetDiscoverySearchConfiguration", "Compliance", "GetDiscoverySearchConfiguration", true, true, false, "MailboxSearch")
			},
			{
				"GetEncryptionConfiguration",
				new WebMethodEntry("GetEncryptionConfiguration", "Encryption Configuration", "GetEncryptionConfiguration", false, false, false, null)
			},
			{
				"GetEvents",
				new WebMethodEntry("GetEvents", "Mailbox", "GetEvents", true, false, false, null)
			},
			{
				"GetFileAttachment",
				new WebMethodEntry("GetFileAttachment", "Mailbox", "GetFileAttachment", true, false, false, null)
			},
			{
				"GetFolder",
				new WebMethodEntry("GetFolder", "Mailbox", "GetFolder", true, false, false, null)
			},
			{
				"GetHoldOnMailboxes",
				new WebMethodEntry("GetHoldOnMailboxes", "Compliance", "GetHoldOnMailboxes", true, true, false, "LegalHold")
			},
			{
				"GetImItemList",
				new WebMethodEntry("GetImItemList", "People", "GetImItemList", true, false, false, null)
			},
			{
				"GetImItems",
				new WebMethodEntry("GetImItems", "People", "GetImItems", true, false, false, null)
			},
			{
				"GetInboxRules",
				new WebMethodEntry("GetInboxRules", "User Configuration", "GetInboxRules", true, false, false, null)
			},
			{
				"GetItem",
				new WebMethodEntry("GetItem", "Mailbox", "GetItem", true, false, false, null)
			},
			{
				"GetMailTips",
				new WebMethodEntry("GetMailTips", "User Configuration", "GetMailTips", true, false, false, null)
			},
			{
				"GetMessageTrackingReport",
				new WebMethodEntry("GetMessageTrackingReport", "Mailbox", "GetMessageTrackingReport", true, false, false, null)
			},
			{
				"GetNonIndexableItemDetails",
				new WebMethodEntry("GetNonIndexableItemDetails", "Compliance", "GetNonIndexableItemDetails", true, true, false, "MailboxSearch")
			},
			{
				"GetNonIndexableItemStatistics",
				new WebMethodEntry("GetNonIndexableItemStatistics", "Compliance", "GetNonIndexableItemStatistics", true, true, false, "MailboxSearch")
			},
			{
				"GetNotesForPersona",
				new WebMethodEntry("GetNotesForPersona", "People", "GetNotesForPersona", true, false, false, null)
			},
			{
				"GetOrganizationHierarchyForPersona",
				new WebMethodEntry("GetOrganizationHierarchyForPersona", "People", "GetOrganizationHierarchyForPersona", true, false, false, null)
			},
			{
				"GetOwaUserConfiguration",
				new WebMethodEntry("GetOwaUserConfiguration", "People", "GetOwaUserConfiguration", true, false, false, null)
			},
			{
				"GetPasswordExpirationDate",
				new WebMethodEntry("GetPasswordExpirationDate", "Directory", "GetPasswordExpirationDate", true, false, false, null)
			},
			{
				"GetPersona",
				new WebMethodEntry("GetPersona", "People", "GetPersona", true, false, false, null)
			},
			{
				"GetPersonaNotes",
				new WebMethodEntry("GetPersonaNotes", "People", "GetPersonaNotes", true, false, false, null)
			},
			{
				"GetPersonaOrganizationHierarchy",
				new WebMethodEntry("GetPersonaOrganizationHierarchy", "People", "GetPersonaOrganizationHierarchy", true, false, false, null)
			},
			{
				"GetPersonaPhoto",
				new WebMethodEntry("GetPersonaPhoto", "People", "GetPersonaPhoto", true, false, false, null)
			},
			{
				"GetPhoneCallInformation",
				new WebMethodEntry("GetPhoneCallInformation", "UM", "GetPhoneCallInformation", true, false, false, null)
			},
			{
				"GetReminders",
				new WebMethodEntry("GetReminders", "Calendar", "GetReminders", true, false, false, null)
			},
			{
				"GetRoomLists",
				new WebMethodEntry("GetRoomLists", "Directory", "GetRoomLists", true, false, false, null)
			},
			{
				"GetRooms",
				new WebMethodEntry("GetRooms", "Directory", "GetRooms", true, false, false, null)
			},
			{
				"GetSearchableMailboxes",
				new WebMethodEntry("GetSearchableMailboxes", "Compliance", "GetSearchableMailboxes", true, true, false, "MailboxSearch")
			},
			{
				"GetServerTimeZones",
				new WebMethodEntry("GetServerTimeZones", "Utilities", "GetServerTimeZones", true, false, false, null)
			},
			{
				"GetServiceConfiguration",
				new WebMethodEntry("GetServiceConfiguration", "Utilities", "GetServiceConfiguration", true, false, false, null)
			},
			{
				"GetSharingFolder",
				new WebMethodEntry("GetSharingFolder", "Calendar", "GetSharingFolder", true, false, false, null)
			},
			{
				"GetSharingMetadata",
				new WebMethodEntry("GetSharingMetadata", "Calendar", "GetSharingMetadata", true, false, false, null)
			},
			{
				"GetStreamingEvents",
				new WebMethodEntry("GetStreamingEvents", "Mailbox", "GetStreamingEvents", true, false, false, null)
			},
			{
				"GetUMCallDataRecords",
				new WebMethodEntry("GetUMCallDataRecords", "UM", "GetUMCallDataRecords", false, false, false, null)
			},
			{
				"GetUMCallSummary",
				new WebMethodEntry("GetUMCallSummary", "UM", "GetUMCallSummary", false, false, false, null)
			},
			{
				"GetUMPin",
				new WebMethodEntry("GetUMPin", "UM", "GetUMPin", false, false, false, null)
			},
			{
				"GetUMPrompt",
				new WebMethodEntry("GetUMPrompt", "UM", "GetUMPrompt", false, false, false, null)
			},
			{
				"GetUMPromptNames",
				new WebMethodEntry("GetUMPromptNames", "UM", "GetUMPromptNames", false, false, false, null)
			},
			{
				"GetUMProperties",
				new WebMethodEntry("GetUMProperties", "UM", "GetUMProperties", true, false, false, null)
			},
			{
				"GetUMSubscriberCallAnsweringData",
				new WebMethodEntry("GetUMSubscriberCallAnsweringData", "UM", "GetUMSubscriberCallAnsweringData", false, false, false, null)
			},
			{
				"GetUserAvailabilityRequest",
				new WebMethodEntry("GetUserAvailability", "Calendar", "GetUserAvailabilityRequest", true, false, false, null)
			},
			{
				"GetUserConfiguration",
				new WebMethodEntry("GetUserConfiguration", "User Configuration", "GetUserConfiguration", true, false, false, null)
			},
			{
				"GetUserOofSettingsRequest",
				new WebMethodEntry("GetUserOofSettings", "User Configuration", "GetUserOofSettingsRequest", true, false, false, null)
			},
			{
				"GetUserPhoto",
				new WebMethodEntry("GetUserPhoto", "Mailbox", "GetUserPhoto", true, false, false, null)
			},
			{
				"GetUserPhoto:GET",
				new WebMethodEntry("GetUserPhoto:GET", "Mailbox", "GetUserPhoto:GET", true, false, false, null)
			},
			{
				"GetUserRetentionPolicyTags",
				new WebMethodEntry("GetUserRetentionPolicyTags", "User Configuration", "GetUserRetentionPolicyTags", true, false, false, null)
			},
			{
				"GetUserUnifiedGroups",
				new WebMethodEntry("GetUserUnifiedGroups", "GroupMailbox", "GetUserUnifiedGroups", true, false, false, null)
			},
			{
				"InitUMMailbox",
				new WebMethodEntry("InitUMMailbox", "UM", "InitUMMailbox", false, false, false, null)
			},
			{
				"InstallApp",
				new WebMethodEntry("InstallApp", "OWA Extension", "InstallApp", true, true, false, null)
			},
			{
				"IsUMEnabled",
				new WebMethodEntry("IsUMEnabled", "UM", "IsUMEnabled", true, false, false, null)
			},
			{
				"LogDatapoint",
				new WebMethodEntry("LogDatapoint", "People", "LogDatapoint", true, false, false, null)
			},
			{
				"MarkAllItemsAsRead",
				new WebMethodEntry("MarkAllItemsAsRead", "Mailbox", "MarkAllItemsAsRead", true, false, false, null)
			},
			{
				"MarkAsJunk",
				new WebMethodEntry("MarkAsJunk", "Mailbox", "MarkAsJunk", true, false, false, null)
			},
			{
				"MoveFolder",
				new WebMethodEntry("MoveFolder", "Mailbox", "MoveFolder", true, false, false, null)
			},
			{
				"MoveItem",
				new WebMethodEntry("MoveItem", "Mailbox", "MoveItem", true, false, false, null)
			},
			{
				"PerformInstantSearch",
				new WebMethodEntry("PerformInstantSearch", "Mailbox", "PerformInstantSearch", true, false, false, null)
			},
			{
				"PerformReminderAction",
				new WebMethodEntry("PerformReminderAction", "Calendar", "PerformReminderAction", true, false, false, null)
			},
			{
				"PlayOnPhone",
				new WebMethodEntry("PlayOnPhone", "UM", "PlayOnPhone", true, false, false, null)
			},
			{
				"PlayOnPhoneGreeting",
				new WebMethodEntry("PlayOnPhoneGreeting", "UM", "PlayOnPhoneGreeting", true, false, false, null)
			},
			{
				"PostModernGroupItem",
				new WebMethodEntry("PostModernGroupItem", "GroupMailbox", "PostModernGroupItem", false, false, false, null)
			},
			{
				"ProcessSuiteStorage",
				new WebMethodEntry("ProcessSuiteStorage", "User Configuration", "ProcessSuiteStorage", true, false, false, null)
			},
			{
				"RefreshSharingFolder",
				new WebMethodEntry("RefreshSharingFolder", "Calendar", "RefreshSharingFolder", true, false, false, null)
			},
			{
				"RemoveAggregatedAccount",
				new WebMethodEntry("RemoveAggregatedAccount", "OutlookService", "RemoveAggregatedAccount", true, false, false, null)
			},
			{
				"RemoveContactFromImList",
				new WebMethodEntry("RemoveContactFromImList", "People", "RemoveContactFromImList", true, false, false, null)
			},
			{
				"RemoveDelegate",
				new WebMethodEntry("RemoveDelegate", "Delegate", "RemoveDelegate", true, false, false, null)
			},
			{
				"RemoveDistributionGroupFromImList",
				new WebMethodEntry("RemoveDistributionGroupFromImList", "People", "RemoveDistributionGroupFromImList", true, false, false, null)
			},
			{
				"RemoveImContactFromGroup",
				new WebMethodEntry("RemoveImContactFromGroup", "People", "RemoveImContactFromGroup", true, false, false, null)
			},
			{
				"RemoveImGroup",
				new WebMethodEntry("RemoveImGroup", "People", "RemoveImGroup", true, false, false, null)
			},
			{
				"ResetPIN",
				new WebMethodEntry("ResetPIN", "UM", "ResetPIN", true, false, false, null)
			},
			{
				"ResetUMMailbox",
				new WebMethodEntry("ResetUMMailbox", "UM", "ResetUMMailbox", false, false, false, null)
			},
			{
				"ResolveNames",
				new WebMethodEntry("ResolveNames", "Directory", "ResolveNames", true, false, false, null)
			},
			{
				"SaveUMPin",
				new WebMethodEntry("SaveUMPin", "UM", "SaveUMPin", false, false, false, null)
			},
			{
				"SearchMailboxes",
				new WebMethodEntry("SearchMailboxes", "Compliance", "SearchMailboxes", true, true, false, "MailboxSearch")
			},
			{
				"SendItem",
				new WebMethodEntry("SendItem", "Mailbox", "SendItem", true, false, false, null)
			},
			{
				"SetAggregatedAccount",
				new WebMethodEntry("SetAggregatedAccount", "OutlookService", "SetAggregatedAccount", true, false, false, null)
			},
			{
				"SetClientExtension",
				new WebMethodEntry("SetClientExtension", "OWA Extension", "SetClientExtension", false, false, false, null)
			},
			{
				"SetClutterState",
				new WebMethodEntry("SetClutterState", "Inference", "SetClutterState", true, false, false, null)
			},
			{
				"SetEncryptionConfiguration",
				new WebMethodEntry("SetEncryptionConfiguration", "Encryption Configuration", "SetEncryptionConfiguration", false, false, false, null)
			},
			{
				"SetHoldOnMailboxes",
				new WebMethodEntry("SetHoldOnMailboxes", "Compliance", "SetHoldOnMailboxes", true, true, false, "LegalHold")
			},
			{
				"SetImGroup",
				new WebMethodEntry("SetImGroup", "People", "SetImGroup", true, false, false, null)
			},
			{
				"SetImListMigrationCompleted",
				new WebMethodEntry("SetImListMigrationCompleted", "People", "SetImListMigrationCompleted", true, false, false, null)
			},
			{
				"SetMissedCallNotificationEnabled",
				new WebMethodEntry("SetMissedCallNotificationEnabled", "UM", "SetMissedCallNotificationEnabled", true, false, false, null)
			},
			{
				"SetNotificationSettings",
				new WebMethodEntry("SetNotificationSettings", "User Configuration", "SetNotificationSettings", true, false, false, null)
			},
			{
				"SetOofStatus",
				new WebMethodEntry("SetOofStatus", "UM", "SetOofStatus", true, false, false, null)
			},
			{
				"SetPlayOnPhoneDialString",
				new WebMethodEntry("SetPlayOnPhoneDialString", "UM", "SetPlayOnPhoneDialString", true, false, false, null)
			},
			{
				"SetTeamMailbox",
				new WebMethodEntry("SetTeamMailbox", "TeamMailbox", "SetTeamMailbox", true, false, true, null)
			},
			{
				"SetTelephoneAccessFolderEmail",
				new WebMethodEntry("SetTelephoneAccessFolderEmail", "UM", "SetTelephoneAccessFolderEmail", true, false, false, null)
			},
			{
				"SetUserOofSettingsRequest",
				new WebMethodEntry("SetUserOofSettings", "User Configuration", "SetUserOofSettingsRequest", true, false, false, null)
			},
			{
				"StartFindInGALSpeechRecognition",
				new WebMethodEntry("StartFindInGALSpeechRecognition", "UM", "StartFindInGALSpeechRecognition", false, false, false, null)
			},
			{
				"Subscribe",
				new WebMethodEntry("Subscribe", "Mailbox", "Subscribe", true, false, false, null)
			},
			{
				"SyncFolderHierarchy",
				new WebMethodEntry("SyncFolderHierarchy", "Mailbox", "SyncFolderHierarchy", true, false, false, null)
			},
			{
				"SyncFolderItems",
				new WebMethodEntry("SyncFolderItems", "Mailbox", "SyncFolderItems", true, false, false, null)
			},
			{
				"TagImContact",
				new WebMethodEntry("TagImContact", "People", "TagImContact", true, false, false, null)
			},
			{
				"UninstallApp",
				new WebMethodEntry("UninstallApp", "OWA Extension", "UninstallApp", true, false, false, null)
			},
			{
				"UnpinTeamMailbox",
				new WebMethodEntry("UnpinTeamMailbox", "TeamMailbox", "UnpinTeamMailbox", true, false, false, null)
			},
			{
				"Unsubscribe",
				new WebMethodEntry("Unsubscribe", "Mailbox", "Unsubscribe", true, false, false, null)
			},
			{
				"UpdateDelegate",
				new WebMethodEntry("UpdateDelegate", "Delegate", "UpdateDelegate", true, false, false, null)
			},
			{
				"UpdateFolder",
				new WebMethodEntry("UpdateFolder", "Mailbox", "UpdateFolder", true, false, false, null)
			},
			{
				"UpdateGroupMailbox",
				new WebMethodEntry("UpdateGroupMailbox", "GroupMailbox", "UpdateGroupMailbox", false, false, false, null)
			},
			{
				"UpdateInboxRules",
				new WebMethodEntry("UpdateInboxRules", "User Configuration", "UpdateInboxRules", true, false, false, null)
			},
			{
				"UpdateItem",
				new WebMethodEntry("UpdateItem", "Mailbox", "UpdateItem", true, false, false, null)
			},
			{
				"UpdateItemInRecoverableItems",
				new WebMethodEntry("UpdateItemInRecoverableItems", "Compliance", "UpdateItemInRecoverableItems", true, false, true, null)
			},
			{
				"UpdateMailboxAssociation",
				new WebMethodEntry("UpdateMailboxAssociation", "GroupMailbox", "UpdateMailboxAssociation", false, false, false, null)
			},
			{
				"UpdateUserConfiguration",
				new WebMethodEntry("UpdateUserConfiguration", "User Configuration", "UpdateUserConfiguration", true, false, false, null)
			},
			{
				"UploadItems",
				new WebMethodEntry("UploadItems", "Mailbox", "UploadItems", true, false, false, null)
			},
			{
				"ValidateUMPin",
				new WebMethodEntry("ValidateUMPin", "UM", "ValidateUMPin", false, false, false, null)
			}
		};
	}
}
