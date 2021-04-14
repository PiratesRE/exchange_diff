using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeWebServicesTags
	{
		public const int CalendarAlgorithm = 0;

		public const int CalendarData = 1;

		public const int CalendarCall = 2;

		public const int CommonAlgorithm = 3;

		public const int FolderAlgorithm = 4;

		public const int FolderData = 5;

		public const int FolderCall = 6;

		public const int ItemAlgorithm = 7;

		public const int ItemData = 8;

		public const int ItemCall = 9;

		public const int Exception = 10;

		public const int SessionCache = 11;

		public const int ExchangePrincipalCache = 12;

		public const int Search = 13;

		public const int UtilAlgorithm = 14;

		public const int UtilData = 15;

		public const int UtilCall = 16;

		public const int ServerToServerAuthZ = 17;

		public const int ServiceCommandBaseCall = 18;

		public const int ServiceCommandBaseData = 19;

		public const int FacadeBaseCall = 20;

		public const int CreateItemCall = 21;

		public const int GetItemCall = 22;

		public const int UpdateItemCall = 23;

		public const int DeleteItemCall = 24;

		public const int SendItemCall = 25;

		public const int MoveCopyCommandBaseCall = 26;

		public const int MoveCopyItemCommandBaseCall = 27;

		public const int CopyItemCall = 28;

		public const int MoveItemCall = 29;

		public const int CreateFolderCall = 30;

		public const int GetFolderCall = 31;

		public const int UpdateFolderCall = 32;

		public const int DeleteFolderCall = 33;

		public const int MoveCopyFolderCommandBaseCall = 34;

		public const int CopyFolderCall = 35;

		public const int MoveFolderCall = 36;

		public const int FindCommandBaseCall = 37;

		public const int FindItemCall = 38;

		public const int FindFolderCall = 39;

		public const int UtilCommandBaseCall = 40;

		public const int ExpandDLCall = 41;

		public const int ResolveNamesCall = 42;

		public const int SubscribeCall = 43;

		public const int UnsubscribeCall = 44;

		public const int GetEventsCall = 45;

		public const int Subscriptions = 46;

		public const int SubscriptionBase = 47;

		public const int PushSubscription = 48;

		public const int SyncFolderHierarchyCall = 49;

		public const int SyncFolderItemsCall = 50;

		public const int Synchronization = 51;

		public const int PerformanceMonitor = 52;

		public const int ConvertIdCall = 53;

		public const int GetDelegateCall = 54;

		public const int AddDelegateCall = 55;

		public const int RemoveDelegateCall = 56;

		public const int UpdateDelegateCall = 57;

		public const int ProxyEvaluator = 58;

		public const int GetMailTipsCall = 60;

		public const int AllRequests = 61;

		public const int Authentication = 62;

		public const int WCF = 63;

		public const int GetUserConfigurationCall = 64;

		public const int CreateUserConfigurationCall = 65;

		public const int DeleteUserConfigurationCall = 66;

		public const int UpdateUserConfigurationCall = 67;

		public const int Throttling = 68;

		public const int ExternalUser = 69;

		public const int GetOrganizationConfigurationCall = 70;

		public const int GetRoomsCall = 71;

		public const int GetFederationInformation = 72;

		public const int ParticipantLookupBatching = 73;

		public const int AllResponses = 74;

		public const int FaultInjection = 75;

		public const int GetInboxRulesCall = 76;

		public const int UpdateInboxRulesCall = 77;

		public const int GetCASMailbox = 78;

		public const int FastTransfer = 79;

		public const int SyncConversationCall = 80;

		public const int ELC = 81;

		public const int ActivityConverter = 82;

		public const int SyncPeopleCall = 83;

		public const int GetCalendarFoldersCall = 84;

		public const int GetRemindersCall = 85;

		public const int SyncCalendarCall = 86;

		public const int PerformReminderActionCall = 87;

		public const int ProvisionCall = 88;

		public const int RenameCalendarGroupCall = 89;

		public const int DeleteCalendarGroupCall = 90;

		public const int CreateCalendarCall = 91;

		public const int RenameCalendarCall = 92;

		public const int DeleteCalendarCall = 93;

		public const int SetCalendarColorCall = 94;

		public const int SetCalendarGroupOrderCall = 95;

		public const int CreateCalendarGroupCall = 96;

		public const int MoveCalendarCall = 97;

		public const int GetFavoritesCall = 98;

		public const int UpdateFavoriteFolderCall = 99;

		public const int GetTimeZoneOffsetsCall = 100;

		public const int Authorization = 101;

		public const int SendCalendarSharingInviteCall = 102;

		public const int GetCalendarSharingRecipientInfoCall = 103;

		public const int AddSharedCalendarCall = 104;

		public const int FindPeopleCall = 105;

		public const int FindPlacesCall = 106;

		public const int UserPhotos = 107;

		public const int GetPersonaCall = 108;

		public const int GetExtensibilityContextCall = 109;

		public const int SubscribeInternalCalendarCall = 110;

		public const int SubscribeInternetCalendarCall = 111;

		public const int GetUserAvailabilityInternalCall = 112;

		public const int ApplyConversationActionCall = 113;

		public const int GetCalendarSharingPermissionsCall = 114;

		public const int SetCalendarSharingPermissionsCall = 115;

		public const int SetCalendarPublishingCall = 116;

		public const int UCS = 117;

		public const int GetTaskFoldersCall = 118;

		public const int CreateTaskFolderCall = 119;

		public const int RenameTaskFolderCall = 120;

		public const int DeleteTaskFolderCall = 121;

		public const int MasterCategoryListCall = 122;

		public const int GetCalendarFolderConfigurationCall = 123;

		public const int OnlineMeeting = 124;

		public const int ModernGroups = 125;

		public const int CreateUnifiedMailbox = 126;

		public const int AddAggregatedAccount = 127;

		public const int Reminders = 128;

		public const int GetAggregatedAccount = 129;

		public const int RemoveAggregatedAccount = 130;

		public const int SetAggregatedAccount = 131;

		public const int Weather = 132;

		public const int FederatedDirectory = 133;

		public const int GetPeopleIKnowGraphCall = 134;

		public const int AddEventToMyCalendar = 135;

		public const int ConversationAggregation = 136;

		public const int IsOffice365Domain = 137;

		public const int RefreshGALContactsFolder = 138;

		public const int Options = 139;

		public const int OpenTenantManager = 140;

		public const int MarkAllItemsAsRead = 141;

		public const int GetConversationItems = 142;

		public const int GetLikers = 143;

		public const int GetUserUnifiedGroups = 144;

		public const int PeopleICommunicateWith = 145;

		public const int SyncPersonaContactsBase = 146;

		public const int SyncAutoCompleteRecipients = 147;

		public static Guid guid = new Guid("9041df24-db8f-4561-9ce6-75ee8dc21732");
	}
}
