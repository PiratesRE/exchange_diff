﻿using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class AvailableWebServiceRoleEntries_Dedicated
	{
		internal static RoleEntry[] RoleEntries = new RoleEntry[]
		{
			RoleEntry.Parse("w,AddAggregatedAccount"),
			RoleEntry.Parse("w,AddDelegate"),
			RoleEntry.Parse("w,AddDistributionGroupToImList"),
			RoleEntry.Parse("w,AddImContactToGroup"),
			RoleEntry.Parse("w,AddImGroup"),
			RoleEntry.Parse("w,AddNewImContactToGroup"),
			RoleEntry.Parse("w,AddNewTelUriContactToGroup"),
			RoleEntry.Parse("w,ApplyConversationAction"),
			RoleEntry.Parse("w,ArchiveItem"),
			RoleEntry.Parse("w,ConvertId"),
			RoleEntry.Parse("w,CopyFolder"),
			RoleEntry.Parse("w,CopyItem"),
			RoleEntry.Parse("w,CreateAttachment"),
			RoleEntry.Parse("w,CreateFolder"),
			RoleEntry.Parse("w,CreateFolderPath"),
			RoleEntry.Parse("w,CreateItem"),
			RoleEntry.Parse("w,CreateManagedFolder"),
			RoleEntry.Parse("w,CreateUnifiedMailbox"),
			RoleEntry.Parse("w,CreateUserConfiguration"),
			RoleEntry.Parse("w,DeleteAttachment"),
			RoleEntry.Parse("w,DeleteFolder"),
			RoleEntry.Parse("w,DeleteItem"),
			RoleEntry.Parse("w,DeleteUserConfiguration"),
			RoleEntry.Parse("w,DisableApp"),
			RoleEntry.Parse("w,Disconnect"),
			RoleEntry.Parse("w,DisconnectPhoneCall"),
			RoleEntry.Parse("w,EmptyFolder"),
			RoleEntry.Parse("w,EndInstantSearchSession"),
			RoleEntry.Parse("w,ExpandDL"),
			RoleEntry.Parse("w,ExportItems"),
			RoleEntry.Parse("w,FindConversation"),
			RoleEntry.Parse("w,FindFolder"),
			RoleEntry.Parse("w,FindItem"),
			RoleEntry.Parse("w,FindMailboxStatisticsByKeywords"),
			RoleEntry.Parse("w,FindMessageTrackingReport"),
			RoleEntry.Parse("w,FindPeople"),
			RoleEntry.Parse("w,GetAggregatedAccount"),
			RoleEntry.Parse("w,GetAppManifests"),
			RoleEntry.Parse("w,GetAppMarketplaceUrl"),
			RoleEntry.Parse("w,GetAttachment"),
			RoleEntry.Parse("w,GetBposNavBarData"),
			RoleEntry.Parse("w,GetCalendarFolders"),
			RoleEntry.Parse("w,GetCallInfo"),
			RoleEntry.Parse("w,GetClutterState"),
			RoleEntry.Parse("w,GetConversationItems"),
			RoleEntry.Parse("w,GetDelegate"),
			RoleEntry.Parse("w,GetDiscoverySearchConfiguration"),
			RoleEntry.Parse("w,GetEvents"),
			RoleEntry.Parse("w,GetFileAttachment"),
			RoleEntry.Parse("w,GetFolder"),
			RoleEntry.Parse("w,GetHoldOnMailboxes"),
			RoleEntry.Parse("w,GetImItemList"),
			RoleEntry.Parse("w,GetImItems"),
			RoleEntry.Parse("w,GetInboxRules"),
			RoleEntry.Parse("w,GetItem"),
			RoleEntry.Parse("w,GetMailTips"),
			RoleEntry.Parse("w,GetMessageTrackingReport"),
			RoleEntry.Parse("w,GetNonIndexableItemDetails"),
			RoleEntry.Parse("w,GetNonIndexableItemStatistics"),
			RoleEntry.Parse("w,GetNotesForPersona"),
			RoleEntry.Parse("w,GetOrganizationHierarchyForPersona"),
			RoleEntry.Parse("w,GetOwaUserConfiguration"),
			RoleEntry.Parse("w,GetPasswordExpirationDate"),
			RoleEntry.Parse("w,GetPersona"),
			RoleEntry.Parse("w,GetPersonaNotes"),
			RoleEntry.Parse("w,GetPersonaOrganizationHierarchy"),
			RoleEntry.Parse("w,GetPersonaPhoto"),
			RoleEntry.Parse("w,GetPhoneCallInformation"),
			RoleEntry.Parse("w,GetReminders"),
			RoleEntry.Parse("w,GetRoomLists"),
			RoleEntry.Parse("w,GetRooms"),
			RoleEntry.Parse("w,GetSearchableMailboxes"),
			RoleEntry.Parse("w,GetServerTimeZones"),
			RoleEntry.Parse("w,GetServiceConfiguration"),
			RoleEntry.Parse("w,GetSharingFolder"),
			RoleEntry.Parse("w,GetSharingMetadata"),
			RoleEntry.Parse("w,GetStreamingEvents"),
			RoleEntry.Parse("w,GetUMProperties"),
			RoleEntry.Parse("w,GetUserAvailability"),
			RoleEntry.Parse("w,GetUserConfiguration"),
			RoleEntry.Parse("w,GetUserOofSettings"),
			RoleEntry.Parse("w,GetUserPhoto"),
			RoleEntry.Parse("w,GetUserPhoto:GET"),
			RoleEntry.Parse("w,GetUserRetentionPolicyTags"),
			RoleEntry.Parse("w,GetUserUnifiedGroups"),
			RoleEntry.Parse("w,InstallApp"),
			RoleEntry.Parse("w,IsUMEnabled"),
			RoleEntry.Parse("w,LogDatapoint"),
			RoleEntry.Parse("w,MarkAllItemsAsRead"),
			RoleEntry.Parse("w,MarkAsJunk"),
			RoleEntry.Parse("w,MoveFolder"),
			RoleEntry.Parse("w,MoveItem"),
			RoleEntry.Parse("w,PerformInstantSearch"),
			RoleEntry.Parse("w,PerformReminderAction"),
			RoleEntry.Parse("w,PlayOnPhone"),
			RoleEntry.Parse("w,PlayOnPhoneGreeting"),
			RoleEntry.Parse("w,PostModernGroupItem"),
			RoleEntry.Parse("w,ProcessSuiteStorage"),
			RoleEntry.Parse("w,RefreshSharingFolder"),
			RoleEntry.Parse("w,RemoveAggregatedAccount"),
			RoleEntry.Parse("w,RemoveContactFromImList"),
			RoleEntry.Parse("w,RemoveDelegate"),
			RoleEntry.Parse("w,RemoveDistributionGroupFromImList"),
			RoleEntry.Parse("w,RemoveImContactFromGroup"),
			RoleEntry.Parse("w,RemoveImGroup"),
			RoleEntry.Parse("w,ResetPIN"),
			RoleEntry.Parse("w,ResolveNames"),
			RoleEntry.Parse("w,SearchMailboxes"),
			RoleEntry.Parse("w,SendItem"),
			RoleEntry.Parse("w,SetAggregatedAccount"),
			RoleEntry.Parse("w,SetClutterState"),
			RoleEntry.Parse("w,SetHoldOnMailboxes"),
			RoleEntry.Parse("w,SetImGroup"),
			RoleEntry.Parse("w,SetImListMigrationCompleted"),
			RoleEntry.Parse("w,SetMissedCallNotificationEnabled"),
			RoleEntry.Parse("w,SetNotificationSettings"),
			RoleEntry.Parse("w,SetOofStatus"),
			RoleEntry.Parse("w,SetPlayOnPhoneDialString"),
			RoleEntry.Parse("w,SetTeamMailbox"),
			RoleEntry.Parse("w,SetTelephoneAccessFolderEmail"),
			RoleEntry.Parse("w,SetUserOofSettings"),
			RoleEntry.Parse("w,Subscribe"),
			RoleEntry.Parse("w,SyncFolderHierarchy"),
			RoleEntry.Parse("w,SyncFolderItems"),
			RoleEntry.Parse("w,TagImContact"),
			RoleEntry.Parse("w,UninstallApp"),
			RoleEntry.Parse("w,UnpinTeamMailbox"),
			RoleEntry.Parse("w,Unsubscribe"),
			RoleEntry.Parse("w,UpdateDelegate"),
			RoleEntry.Parse("w,UpdateFolder"),
			RoleEntry.Parse("w,UpdateInboxRules"),
			RoleEntry.Parse("w,UpdateItem"),
			RoleEntry.Parse("w,UpdateItemInRecoverableItems"),
			RoleEntry.Parse("w,UpdateUserConfiguration"),
			RoleEntry.Parse("w,UploadItems")
		};
	}
}
