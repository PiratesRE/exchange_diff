using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[MessageInspectorBehavior]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class OWAService : IOWAService, IJsonServiceContract, IOWAStreamingService, IJsonStreamingServiceContract
	{
		public OWAService()
		{
			this.jsonService = new JsonService();
		}

		public ExtensibilityContext GetExtensibilityContext(GetExtensibilityContextParameters request)
		{
			return this.jsonService.GetExtensibilityContext(request);
		}

		public bool AddBuddy(Buddy buddy)
		{
			return this.jsonService.AddBuddy(buddy);
		}

		public GetBuddyListResponse GetBuddyList()
		{
			return this.jsonService.GetBuddyList();
		}

		public IAsyncResult BeginFindPlaces(FindPlacesRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindPlaces(request, asyncCallback, asyncState);
		}

		public Persona[] EndFindPlaces(IAsyncResult result)
		{
			return this.jsonService.EndFindPlaces(result);
		}

		public DeletePlaceJsonResponse DeletePlace(DeletePlaceRequest request)
		{
			return this.jsonService.DeletePlace(request);
		}

		public CalendarActionResponse AddEventToMyCalendar(AddEventToMyCalendarRequest request)
		{
			return this.jsonService.AddEventToMyCalendar(request);
		}

		public bool AddTrustedSender(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return this.jsonService.AddTrustedSender(itemId);
		}

		public GetFavoritesResponse GetFavorites()
		{
			return this.jsonService.GetFavorites();
		}

		public UpdateFavoriteFolderResponse UpdateFavoriteFolder(UpdateFavoriteFolderRequest request)
		{
			return this.jsonService.UpdateFavoriteFolder(request);
		}

		public GetPersonaModernGroupMembershipJsonResponse GetPersonaModernGroupMembership(GetPersonaModernGroupMembershipJsonRequest request)
		{
			return this.jsonService.GetPersonaModernGroupMembership(request);
		}

		public GetModernGroupJsonResponse GetModernGroup(GetModernGroupJsonRequest request)
		{
			return this.jsonService.GetModernGroup(request);
		}

		public GetModernGroupJsonResponse GetRecommendedModernGroup(GetModernGroupJsonRequest request)
		{
			return this.jsonService.GetRecommendedModernGroup(request);
		}

		public GetModernGroupsJsonResponse GetModernGroups()
		{
			return this.jsonService.GetModernGroups();
		}

		public SetModernGroupPinStateJsonResponse SetModernGroupPinState(string smtpAddress, bool isPinned)
		{
			return this.jsonService.SetModernGroupPinState(smtpAddress, isPinned);
		}

		public SetModernGroupMembershipJsonResponse SetModernGroupMembership(SetModernGroupMembershipJsonRequest request)
		{
			return this.jsonService.SetModernGroupMembership(request);
		}

		public bool SetModernGroupSubscription()
		{
			return this.jsonService.SetModernGroupSubscription();
		}

		public GetModernGroupUnseenItemsJsonResponse GetModernGroupUnseenItems(GetModernGroupUnseenItemsJsonRequest request)
		{
			return this.jsonService.GetModernGroupUnseenItems(request);
		}

		public GetPeopleIKnowGraphResponse GetPeopleIKnowGraphCommand(GetPeopleIKnowGraphRequest request)
		{
			return this.jsonService.GetPeopleIKnowGraphCommand(request);
		}

		public UpdateMasterCategoryListResponse UpdateMasterCategoryList(UpdateMasterCategoryListRequest request)
		{
			return this.jsonService.UpdateMasterCategoryList(request);
		}

		public MasterCategoryListActionResponse GetMasterCategoryList(GetMasterCategoryListRequest request)
		{
			return this.jsonService.GetMasterCategoryList(request);
		}

		public GetTaskFoldersResponse GetTaskFolders()
		{
			return this.jsonService.GetTaskFolders();
		}

		public TaskFolderActionFolderIdResponse CreateTaskFolder(string newTaskFolderName, string parentGroupGuid)
		{
			return new CreateTaskFolderCommand(CallContext.Current, newTaskFolderName, parentGroupGuid).Execute();
		}

		public TaskFolderActionFolderIdResponse RenameTaskFolder(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newTaskFolderName)
		{
			return new RenameTaskFolderCommand(CallContext.Current, itemId, newTaskFolderName).Execute();
		}

		public TaskFolderActionResponse DeleteTaskFolder(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new DeleteTaskFolderCommand(CallContext.Current, itemId).Execute();
		}

		public PeopleFilter[] GetPeopleFilters()
		{
			return new GetPeopleFilters(CallContext.Current).Execute();
		}

		public string CreateAttachmentFromUri(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string uri, string name, string subscriptionId)
		{
			return new CreateAttachmentFromUri(CallContext.Current, itemId, uri, name, subscriptionId).Execute();
		}

		public GetUserAvailabilityInternalJsonResponse GetUserAvailabilityInternal(GetUserAvailabilityInternalJsonRequest request)
		{
			return this.jsonService.GetUserAvailabilityInternal(request);
		}

		public OptionSummary GetOptionSummary()
		{
			return new GetOptionSummary(CallContext.Current).Execute();
		}

		public UserOofSettingsType GetOwaUserOofSettings()
		{
			return new GetUserOofSettings(CallContext.Current, EWSSettings.RequestTimeZone).Execute();
		}

		public bool SetOwaUserOofSettings(UserOofSettingsType userOofSettings)
		{
			return new SetUserOofSettings(CallContext.Current, userOofSettings).Execute();
		}

		public EmailSignatureConfiguration GetOwaUserEmailSignature()
		{
			return new GetEmailSignature(CallContext.Current).Execute();
		}

		public ScopeFlightsSetting[] GetFlightsSettings()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext);
			if (!userContext.FeaturesManager.ClientServerSettings.FlightsView.Enabled)
			{
				throw new OwaNotSupportedException("This method is not supported.");
			}
			return new GetFlightsSettings(CallContext.Current, new ScopeFlightsSettingsProvider()).Execute();
		}

		public bool SetOwaUserEmailSignature(EmailSignatureConfiguration userEmailSignature)
		{
			return new SetEmailSignature(CallContext.Current, userEmailSignature).Execute();
		}

		public int GetDaysUntilPasswordExpiration()
		{
			return new GetDaysUntilPasswordExpiration(CallContext.Current).Execute();
		}

		public EwsRoomType[] GetRoomsInternal(string roomList)
		{
			return new GetRoomsInternal(CallContext.Current, roomList).Execute().Rooms;
		}

		public ThemeSelectionInfoType GetThemes()
		{
			return new GetThemes(CallContext.Current).Execute();
		}

		public bool SetTheme(string theme)
		{
			SetUserThemeRequest request = new SetUserThemeRequest
			{
				ThemeId = theme,
				SkipO365Call = false
			};
			SetUserThemeResponse setUserThemeResponse = new SetUserTheme(CallContext.Current, request).Execute();
			return setUserThemeResponse.OwaSuccess;
		}

		public SetUserThemeResponse SetUserTheme(SetUserThemeRequest request)
		{
			return new SetUserTheme(CallContext.Current, request).Execute();
		}

		public TimeZoneConfiguration GetTimeZone(bool needTimeZoneList)
		{
			return new GetTimeZone(CallContext.Current, needTimeZoneList).Execute();
		}

		public bool SetTimeZone(string timezone)
		{
			return new SetTimeZone(CallContext.Current, timezone).Execute();
		}

		public bool SetUserLocale(string userLocale, bool localizeFolders)
		{
			return new SetUserLocale(CallContext.Current, userLocale, localizeFolders).Execute();
		}

		public AddSharedCalendarResponse AddSharedCalendar(AddSharedCalendarRequest request)
		{
			return new AddSharedCalendarCommand(CallContext.Current, request).Execute();
		}

		public CalendarActionFolderIdResponse SubscribeInternalCalendar(SubscribeInternalCalendarRequest request)
		{
			return new SubscribeInternalCalendarCommand(CallContext.Current, request).Execute();
		}

		public CalendarActionFolderIdResponse SubscribeInternetCalendar(SubscribeInternetCalendarRequest request)
		{
			return new SubscribeInternetCalendarCommand(CallContext.Current, request).Execute();
		}

		public GetCalendarSharingRecipientInfoResponse GetCalendarSharingRecipientInfo(GetCalendarSharingRecipientInfoRequest request)
		{
			return new GetCalendarSharingRecipientInfoCommand(CallContext.Current, request).Execute();
		}

		public GetCalendarSharingPermissionsResponse GetCalendarSharingPermissions(GetCalendarSharingPermissionsRequest request)
		{
			return new GetCalendarSharingPermissionsCommand(CallContext.Current, request).Execute();
		}

		public CalendarActionResponse SetCalendarSharingPermissions(SetCalendarSharingPermissionsRequest request)
		{
			return new SetCalendarSharingPermissionsCommand(CallContext.Current, request).Execute();
		}

		public SetCalendarPublishingResponse SetCalendarPublishing(SetCalendarPublishingRequest request)
		{
			return new SetCalendarPublishingCommand(CallContext.Current, request).Execute();
		}

		public CalendarShareInviteResponse SendCalendarSharingInvite(CalendarShareInviteRequest request)
		{
			return new SendCalendarSharingInviteCommand(CallContext.Current, request).Execute();
		}

		public CalendarActionGroupIdResponse CreateCalendarGroup(string newGroupName)
		{
			return new CreateCalendarGroupCommand(CallContext.Current, newGroupName).Execute();
		}

		public CalendarActionGroupIdResponse RenameCalendarGroup(Microsoft.Exchange.Services.Core.Types.ItemId groupId, string newGroupName)
		{
			return new RenameCalendarGroupCommand(CallContext.Current, groupId, newGroupName).Execute();
		}

		public CalendarActionResponse DeleteCalendarGroup(string groupId)
		{
			return new DeleteCalendarGroupCommand(CallContext.Current, groupId).Execute();
		}

		public CalendarActionFolderIdResponse CreateCalendar(string newCalendarName, string parentGroupGuid, string emailAddress)
		{
			return new CreateCalendarCommand(CallContext.Current, newCalendarName, parentGroupGuid, emailAddress).Execute();
		}

		public CalendarActionFolderIdResponse RenameCalendar(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newCalendarName)
		{
			return new RenameCalendarCommand(CallContext.Current, itemId, newCalendarName).Execute();
		}

		public CalendarActionResponse DeleteCalendar(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new DeleteCalendarCommand(CallContext.Current, itemId).Execute();
		}

		public CalendarActionItemIdResponse SetCalendarColor(Microsoft.Exchange.Services.Core.Types.ItemId itemId, CalendarColor calendarColor)
		{
			return new SetCalendarColorCommand(CallContext.Current, itemId, calendarColor).Execute();
		}

		public CalendarActionResponse MoveCalendar(FolderId calendarToMove, string parentGroupId, FolderId calendarBefore)
		{
			return new MoveCalendarCommand(CallContext.Current, calendarToMove, parentGroupId, calendarBefore).Execute();
		}

		public CalendarActionResponse SetCalendarGroupOrder(string groupToPosition, string beforeGroup)
		{
			return new SetCalendarGroupOrderCommand(CallContext.Current, groupToPosition, beforeGroup).Execute();
		}

		public GetCalendarFoldersResponse GetCalendarFolders()
		{
			return new GetCalendarFoldersCommand(CallContext.Current).Execute();
		}

		public GetCalendarFolderConfigurationResponse GetCalendarFolderConfiguration(GetCalendarFolderConfigurationRequest request)
		{
			return this.jsonService.GetCalendarFolderConfiguration(request);
		}

		public GetModernAttachmentsResponse GetModernAttachments(GetModernAttachmentsRequest request)
		{
			return new GetModernAttachmentsCommand(CallContext.Current, request).Execute();
		}

		public GetPersonaNotesResponse GetNotesForPersona(GetNotesForPersonaRequest getNotesForPersonaRequest)
		{
			return new GetPersonaNotesCommand(CallContext.Current, getNotesForPersonaRequest.PersonaId, getNotesForPersonaRequest.EmailAddress, getNotesForPersonaRequest.MaxBytesToFetch, getNotesForPersonaRequest.ParentFolderId).Execute();
		}

		public GetPersonaOrganizationHierarchyResponse GetOrganizationHierarchyForPersona(GetOrganizationHierarchyForPersonaRequest getOrganizationHierarchyForPersonaRequest)
		{
			return new GetPersonaOrganizationHierarchyCommand(CallContext.Current, getOrganizationHierarchyForPersonaRequest.GalObjectGuid, getOrganizationHierarchyForPersonaRequest.EmailAddress).Execute();
		}

		public GetPersonaOrganizationHierarchyResponse GetPersonaOrganizationHierarchy(string galObjectGuid)
		{
			return new GetPersonaOrganizationHierarchyCommand(CallContext.Current, galObjectGuid, null).Execute();
		}

		public GetPersonaNotesResponse GetPersonaNotes(string personaId, int maxBytesToFetch)
		{
			return new GetPersonaNotesCommand(CallContext.Current, personaId, null, maxBytesToFetch, null).Execute();
		}

		public GetGroupResponse GetGroup(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string adObjectId, EmailAddressWrapper emailAddress, IndexedPageView paging, GetGroupResultSet resultSet)
		{
			return new GetGroupCommand(CallContext.Current, itemId, adObjectId, emailAddress, paging, resultSet, null).Execute();
		}

		public GetGroupResponse GetGroupInfo(GetGroupInfoRequest getGroupInfoRequest)
		{
			return new GetGroupCommand(CallContext.Current, getGroupInfoRequest.ItemId, getGroupInfoRequest.AdObjectId, getGroupInfoRequest.EmailAddress, getGroupInfoRequest.Paging, getGroupInfoRequest.ResultSet, getGroupInfoRequest.ParentFolderId).Execute();
		}

		public IAsyncResult BeginGetDlpPolicyTips(GetDlpPolicyTipsRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			GetDlpPolicyTipsAsyncResult getDlpPolicyTipsAsyncResult = new GetDlpPolicyTipsAsyncResult(asyncCallback, asyncState);
			getDlpPolicyTipsAsyncResult.GetDlpPolicyTipsCommand(request);
			return getDlpPolicyTipsAsyncResult;
		}

		public GetDlpPolicyTipsResponse EndGetDlpPolicyTips(IAsyncResult result)
		{
			GetDlpPolicyTipsAsyncResult getDlpPolicyTipsAsyncResult = result as GetDlpPolicyTipsAsyncResult;
			if (getDlpPolicyTipsAsyncResult != null)
			{
				return getDlpPolicyTipsAsyncResult.Response;
			}
			throw new InvalidOperationException("IAsyncResult is null or not of type GetDlpPolicyTipsAsyncResult");
		}

		public IAsyncResult BeginExecuteEwsProxy(EwsProxyRequestParameters request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginExecuteEwsProxy(request, asyncCallback, asyncState);
		}

		public EwsProxyResponse EndExecuteEwsProxy(IAsyncResult result)
		{
			return this.jsonService.EndExecuteEwsProxy(result);
		}

		public bool LogDatapoint(Datapoint[] datapoints)
		{
			UserContext userContext = OWAService.GetUserContext();
			return new LogDatapoint(CallContext.Current, datapoints, new Action<IEnumerable<ILogEvent>>(OwaClientLogger.AppendToLog), new Action<IEnumerable<ILogEvent>>(OwaClientTraceLogger.AppendToLog), this.registerType, true, userContext.CurrentOwaVersion).Execute();
		}

		public bool ConnectedAccountsNotification(bool isOWALogon)
		{
			return new ConnectedAccountsNotification(CallContext.Current, isOWALogon).Execute();
		}

		public UploadPhotoResponse UploadPhoto(UploadPhotoRequest request)
		{
			return new UploadPhoto(CallContext.Current, request).Execute();
		}

		public UploadPhotoResponse UploadPhotoFromForm()
		{
			return new UploadPhotoFromForm(CallContext.Current, HttpContext.Current.Request).Execute();
		}

		public GetFlowConversationResponse GetFlowConversation(BaseFolderId folderId, int conversationCount)
		{
			return new GetFlowConversation(CallContext.Current, folderId, conversationCount).Execute();
		}

		public FindFlowConversationItemResponse FindFlowConversationItem(BaseFolderId folderId, string flowConversationId, int itemCount)
		{
			return new FindFlowConversationItem(CallContext.Current, folderId, flowConversationId, itemCount).Execute();
		}

		public int VerifyCert(string certRawData)
		{
			return new VerifyCert(CallContext.Current, certRawData).Execute();
		}

		[Obsolete]
		public GetCertsResponse GetCerts(GetCertsRequest request)
		{
			return new GetCerts(CallContext.Current, request).Execute();
		}

		public GetCertsResponse GetEncryptionCerts(GetCertsRequest request)
		{
			return new GetEncryptionCerts(CallContext.Current, request).Execute();
		}

		public GetCertsInfoResponse GetCertsInfo(string certRawData, bool isSend)
		{
			return new GetCertsInfo(CallContext.Current, certRawData, isSend).Execute();
		}

		public string GetMime(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new GetMime(CallContext.Current, itemId).Execute();
		}

		public AttachmentDataProvider AddAttachmentDataProvider(AttachmentDataProvider attachmentDataProvider)
		{
			return new AddAttachmentDataProvider(CallContext.Current, attachmentDataProvider).Execute();
		}

		public AttachmentDataProvider[] GetAttachmentDataProviders()
		{
			return this.GetAllAttachmentDataProviders(null);
		}

		public AttachmentDataProvider[] GetAllAttachmentDataProviders(GetAttachmentDataProvidersRequest request)
		{
			return new GetAttachmentDataProviders(CallContext.Current, request).Execute();
		}

		public AttachmentDataProviderType GetAttachmentDataProviderTypes()
		{
			return new GetAttachmentDataProviderTypes(CallContext.Current).Execute();
		}

		public GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderItems(GetAttachmentDataProviderItemsRequest request)
		{
			return new GetAttachmentDataProviderItems(CallContext.Current, request).Execute();
		}

		public GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderRecentItems()
		{
			return new GetAttachmentDataProvidersRecentItems(CallContext.Current).Execute();
		}

		public GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderGroups()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext);
			if (!userContext.FeaturesManager.ClientServerSettings.ModernGroups.Enabled)
			{
				throw new OwaNotSupportedException("This method is not supported.");
			}
			return new GetAttachmentDataProviderGroups(CallContext.Current).Execute();
		}

		public bool CancelAttachment(string cancellationId)
		{
			return new CancelAttachment(CallContext.Current, cancellationId).Execute();
		}

		public string CreateReferenceAttachmentFromLocalFile(CreateReferenceAttachmentFromLocalFileRequest requestObject)
		{
			return new CreateReferenceAttachmentFromLocalFile(CallContext.Current, requestObject, true).Execute();
		}

		public string CreateAttachmentFromAttachmentDataProvider(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string attachmentDataProviderId, string location, string attachmentId, string subscriptionId, string channelId, string dataProviderParentItemId, string providerEndpointUrl, string cancellationId = null)
		{
			return new CreateAttachmentFromAttachmentDataProvider(CallContext.Current, itemId, attachmentDataProviderId, location, attachmentId, subscriptionId, dataProviderParentItemId, providerEndpointUrl, channelId, cancellationId).Execute();
		}

		public CreateAttachmentResponse CreateReferenceAttachmentFromAttachmentDataProvider(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string attachmentDataProviderId, string location, string attachmentId, string dataProviderParentItemId, string providerEndpointUrl, string cancellationId = null)
		{
			return new CreateReferenceAttachmentFromAttachmentDataProvider(CallContext.Current, itemId, attachmentDataProviderId, location, attachmentId, dataProviderParentItemId, providerEndpointUrl, null).Execute();
		}

		public string GetAttachmentDataProviderUploadFolderName()
		{
			return new GetAttachmentDataProviderUploadFolderName(CallContext.Current).Execute();
		}

		public bool SetFolderMruConfiguration(TargetFolderMruConfiguration folderMruConfiguration)
		{
			return new SetFolderMruConfiguration(CallContext.Current, folderMruConfiguration).Execute();
		}

		public int InstantMessageSignIn(bool signedInManually)
		{
			return new InstantMessageSignIn(CallContext.Current, signedInManually).Execute();
		}

		public int InstantMessageSignOut(bool reserved)
		{
			return new InstantMessageSignOut(CallContext.Current).Execute();
		}

		public int SendChatMessage(ChatMessage message)
		{
			return new SendChatMessage(CallContext.Current, message).Execute();
		}

		public bool TerminateChatSession(int chatSessionId)
		{
			return new TerminateChatSession(CallContext.Current, chatSessionId).Execute();
		}

		public int AcceptChatSession(int chatSessionId)
		{
			return new AcceptChatSession(CallContext.Current, chatSessionId).Execute();
		}

		public bool AcceptBuddy(InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup)
		{
			return new AcceptBuddy(CallContext.Current, instantMessageBuddy, instantMessageGroup).Execute();
		}

		public bool AddImBuddy(InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup)
		{
			return new AddImBuddy(CallContext.Current, instantMessageBuddy, instantMessageGroup).Execute();
		}

		public bool DeclineBuddy(InstantMessageBuddy instantMessageBuddy)
		{
			return new DeclineBuddy(CallContext.Current, instantMessageBuddy).Execute();
		}

		public bool RemoveBuddy(InstantMessageBuddy instantMessageBuddy, Microsoft.Exchange.Services.Core.Types.ItemId contactId)
		{
			return new RemoveBuddy(CallContext.Current, instantMessageBuddy, contactId).Execute();
		}

		public bool AddFavorite(InstantMessageBuddy instantMessageBuddy)
		{
			return new AddFavoriteCommand(CallContext.Current, instantMessageBuddy).Execute();
		}

		public bool RemoveFavorite(Microsoft.Exchange.Services.Core.Types.ItemId personaId)
		{
			return new RemoveFavoriteCommand(CallContext.Current, personaId).Execute();
		}

		public bool NotifyAppWipe(DataWipeReason wipeReason)
		{
			return new NotifyAppWipe(CallContext.Current, wipeReason).Execute();
		}

		public bool NotifyTyping(int chatSessionId, bool typingCancelled)
		{
			return new NotifyTyping(CallContext.Current, chatSessionId, typingCancelled).Execute();
		}

		public int SetPresence(InstantMessagePresence presenceSetting)
		{
			return new ChangePresence(CallContext.Current, new InstantMessagePresenceType?(presenceSetting.Presence)).Execute();
		}

		public int ResetPresence()
		{
			return new ResetPresence(CallContext.Current).Execute();
		}

		public int GetPresence(string[] sipUris)
		{
			return new GetPresence(CallContext.Current, sipUris).Execute();
		}

		public int SubscribeForPresenceUpdates(string[] sipUris)
		{
			return new SubscribeForPresenceUpdates(CallContext.Current, sipUris).Execute();
		}

		public int UnsubscribeFromPresenceUpdates(string sipUri)
		{
			return new UnsubscribeFromPresenceUpdates(CallContext.Current, sipUri).Execute();
		}

		public ProxySettings[] GetInstantMessageProxySettings(string[] userPrincipalNames)
		{
			return new GetInstantMessageProxySettings(CallContext.Current, userPrincipalNames).Execute();
		}

		public SubscriptionResponseData[] SubscribeToNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData)
		{
			return new SubscribeToNotification(request, CallContext.Current, subscriptionData).Execute();
		}

		public bool UnsubscribeToNotification(SubscriptionData[] subscriptionData)
		{
			return new UnsubscribeToNotification(CallContext.Current, subscriptionData).Execute();
		}

		public SubscriptionResponseData[] SubscribeToGroupNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData)
		{
			return new SubscribeToGroupNotification(request, CallContext.Current, subscriptionData).Execute();
		}

		public bool UnsubscribeToGroupNotification(SubscriptionData[] subscriptionData)
		{
			return new UnsubscribeToGroupNotification(CallContext.Current, subscriptionData).Execute();
		}

		public SubscriptionResponseData[] SubscribeToGroupUnseenNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData)
		{
			return new SubscribeToGroupUnseenNotification(request, CallContext.Current, subscriptionData).Execute();
		}

		public bool UnsubscribeToGroupUnseenNotification(SubscriptionData[] subscriptionData)
		{
			return new UnsubscribeToGroupUnseenNotification(CallContext.Current, subscriptionData).Execute();
		}

		public bool AddSharedFolders(string displayName, string primarySMTPAddress)
		{
			return new AddSharedFolders(CallContext.Current, displayName, primarySMTPAddress).Execute();
		}

		public bool RemoveSharedFolders(string primarySMTPAddress)
		{
			return new RemoveSharedFolders(CallContext.Current, primarySMTPAddress).Execute();
		}

		public OwaOtherMailboxConfiguration GetOtherMailboxConfiguration()
		{
			return new GetOtherMailboxConfiguration(CallContext.Current).Execute();
		}

		public NavBarData GetBposNavBarData()
		{
			return new GetBposNavBarData(CallContext.Current).Execute();
		}

		public NavBarData GetBposShellInfoNavBarData()
		{
			return new GetBposShellInfoNavBarData(CallContext.Current).Execute();
		}

		public OwaUserConfiguration GetOwaUserConfiguration()
		{
			return new GetOwaUserConfiguration(CallContext.Current, PlacesConfigurationCache.Instance, WeatherConfigurationCache.Instance, this.registerType, () => VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled).Execute();
		}

		public Alert[] GetSystemAlerts()
		{
			return new GetSystemAlerts(CallContext.Current).Execute();
		}

		public bool SetNotificationSettings(NotificationSettingsJsonRequest settings)
		{
			return new SetNotificationSettings(CallContext.Current, settings.Body).Execute();
		}

		public ComplianceConfiguration GetComplianceConfiguration()
		{
			return new GetComplianceConfiguration(CallContext.Current).Execute();
		}

		public TargetFolderMruConfiguration GetFolderMruConfiguration()
		{
			return new GetFolderMruConfiguration(CallContext.Current).Execute();
		}

		public UcwaUserConfiguration GetUcwaUserConfiguration(string sipUri)
		{
			return new GetUcwaUserConfiguration(CallContext.Current, sipUri).Execute();
		}

		public OnlineMeetingType CreateOnlineMeeting(string sipUri, Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new CreateOnlineMeeting(CallContext.Current, sipUri, itemId, false).Execute();
		}

		public OnlineMeetingType CreateMeetNow(string sipUri, string subject)
		{
			return new CreateMeetNow(CallContext.Current, sipUri, subject, true).Execute();
		}

		public string GetWacIframeUrl(string attachmentId)
		{
			return new GetWacIframeUrl(CallContext.Current, attachmentId).Execute();
		}

		public string GetWacIframeUrlForOneDrive(GetWacIframeUrlForOneDriveRequest request)
		{
			return new GetWacIframeUrlForOneDrive(CallContext.Current, request).Execute();
		}

		public WacAttachmentType GetWacAttachmentInfo(string attachmentId, bool isEdit, string draftId)
		{
			return new GetWacAttachmentInfo(CallContext.Current, attachmentId, isEdit, draftId).Execute();
		}

		public GetWellKnownShapesResponse GetWellKnownShapes()
		{
			return new GetWellKnownShapes(CallContext.Current).Execute();
		}

		public string CreateResendDraft(string ndrMessageId, string draftsFolderId)
		{
			return new CreateResendDraft(CallContext.Current, ndrMessageId, draftsFolderId).Execute();
		}

		public SaveExtensionSettingsResponse SaveExtensionSettings(SaveExtensionSettingsParameters request)
		{
			return this.jsonService.SaveExtensionSettings(request);
		}

		public CreateAttachmentJsonResponse CreateAttachmentFromLocalFile(CreateAttachmentJsonRequest request)
		{
			return new CreateAttachmentJsonResponse
			{
				Body = new CreateAttachmentFromLocalFile(CallContext.Current, request.Body).Execute()
			};
		}

		public CreateAttachmentJsonResponse CreateAttachmentFromForm()
		{
			return new CreateAttachmentJsonResponse
			{
				Body = new CreateAttachmentFromForm(CallContext.Current, HttpContext.Current.Request).Execute()
			};
		}

		public string UploadAndShareAttachmentFromForm()
		{
			return new CreateReferenceAttachmentFromLocalFile(CallContext.Current, CreateAttachmentHelper.CreateReferenceAttachmentRequest(HttpContext.Current.Request), false).Execute();
		}

		public string UpdateAttachmentPermissions(UpdateAttachmentPermissionsRequest permissionsRequest)
		{
			return new UpdateAttachmentPermissions(CallContext.Current, permissionsRequest).Execute();
		}

		public LoadExtensionCustomPropertiesResponse LoadExtensionCustomProperties(LoadExtensionCustomPropertiesParameters request)
		{
			return this.jsonService.LoadExtensionCustomProperties(request);
		}

		public SaveExtensionCustomPropertiesResponse SaveExtensionCustomProperties(SaveExtensionCustomPropertiesParameters request)
		{
			return this.jsonService.SaveExtensionCustomProperties(request);
		}

		public Persona UpdatePersona(UpdatePersonaJsonRequest request)
		{
			return this.jsonService.UpdatePersona(request);
		}

		public DeletePersonaJsonResponse DeletePersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, BaseFolderId folderId)
		{
			return this.jsonService.DeletePersona(personaId, folderId);
		}

		public MaskAutoCompleteRecipientResponse MaskAutoCompleteRecipient(MaskAutoCompleteRecipientRequest request)
		{
			return this.jsonService.MaskAutoCompleteRecipient(request);
		}

		public Persona CreatePersona(CreatePersonaJsonRequest request)
		{
			return this.jsonService.CreatePersona(request);
		}

		public CreateModernGroupResponse CreateModernGroup(CreateModernGroupRequest request)
		{
			UserContext userContext = OWAService.GetUserContext();
			bool groupCreationEnabledFromOwaMailboxPolicy = OWAService.GetGroupCreationEnabledFromOwaMailboxPolicy();
			if (userContext.FeaturesManager.ClientServerSettings.ModernGroups.Enabled && groupCreationEnabledFromOwaMailboxPolicy)
			{
				return new CreateModernGroupCommand(CallContext.Current, request).Execute();
			}
			throw new OwaNotSupportedException("This method is not supported.");
		}

		public CreateUnifiedGroupResponse CreateUnifiedGroup(CreateUnifiedGroupRequest request)
		{
			UserContext userContext = OWAService.GetUserContext();
			bool groupCreationEnabledFromOwaMailboxPolicy = OWAService.GetGroupCreationEnabledFromOwaMailboxPolicy();
			if (userContext.FeaturesManager.ClientServerSettings.ModernGroups.Enabled && groupCreationEnabledFromOwaMailboxPolicy)
			{
				return new CreateUnifiedGroupCommand(CallContext.Current, request).Execute();
			}
			throw new OwaNotSupportedException("This method is not supported.");
		}

		public FindMembersInUnifiedGroupResponse FindMembersInUnifiedGroup(FindMembersInUnifiedGroupRequest request)
		{
			return new FindMembersInUnifiedGroupCommand(CallContext.Current, request).Execute();
		}

		public GetRegionalConfigurationResponse GetRegionalConfiguration(GetRegionalConfigurationRequest request)
		{
			return new GetRegionalConfiguration(CallContext.Current, request).Execute();
		}

		public AddMembersToUnifiedGroupResponse AddMembersToUnifiedGroup(AddMembersToUnifiedGroupRequest request)
		{
			return new AddMembersToUnifiedGroupCommand(CallContext.Current, request).Execute();
		}

		public UpdateModernGroupResponse UpdateModernGroup(UpdateModernGroupRequest request)
		{
			return new UpdateModernGroupCommand(CallContext.Current, request).Execute();
		}

		public RemoveModernGroupResponse RemoveModernGroup(RemoveModernGroupRequest request)
		{
			return new RemoveModernGroupCommand(CallContext.Current, request).Execute();
		}

		public ModernGroupMembershipRequestMessageDetailsResponse ModernGroupMembershipRequestMessageDetails(ModernGroupMembershipRequestMessageDetailsRequest request)
		{
			return new ModernGroupMembershipRequestMessageDetailsCommand(CallContext.Current, request).Execute();
		}

		public ValidateModernGroupAliasResponse ValidateModernGroupAlias(ValidateModernGroupAliasRequest request)
		{
			return new ValidateModernGroupAliasCommand(CallContext.Current, request).Execute();
		}

		public GetModernGroupDomainResponse GetModernGroupDomain()
		{
			return new GetModernGroupDomainCommand(CallContext.Current).Execute();
		}

		public Microsoft.Exchange.Services.Core.Types.ItemId[] GetPersonaSuggestions(Microsoft.Exchange.Services.Core.Types.ItemId personaId)
		{
			return this.jsonService.GetPersonaSuggestions(personaId);
		}

		public Persona UnlinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId contactId)
		{
			return this.jsonService.UnlinkPersona(personaId, contactId);
		}

		public Persona AcceptPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId linkToPersonaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId)
		{
			return this.jsonService.AcceptPersonaLinkSuggestion(linkToPersonaId, suggestedPersonaId);
		}

		public Persona LinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId linkToPersonaId, Microsoft.Exchange.Services.Core.Types.ItemId personaIdToBeLinked)
		{
			return this.jsonService.LinkPersona(linkToPersonaId, personaIdToBeLinked);
		}

		public Persona RejectPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId)
		{
			return this.jsonService.RejectPersonaLinkSuggestion(personaId, suggestedPersonaId);
		}

		public SyncCalendarResponse SyncCalendar(SyncCalendarParameters request)
		{
			return this.jsonService.SyncCalendar(request);
		}

		public bool SendReadReceipt(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return this.jsonService.SendReadReceipt(itemId);
		}

		public IAsyncResult BeginRequestDeviceRegistrationChallenge(RequestDeviceRegistrationChallengeJsonRequest deviceRegistrationChallengeRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRequestDeviceRegistrationChallenge(deviceRegistrationChallengeRequest, asyncCallback, asyncState);
		}

		public RequestDeviceRegistrationChallengeJsonResponse EndRequestDeviceRegistrationChallenge(IAsyncResult result)
		{
			return this.jsonService.EndRequestDeviceRegistrationChallenge(result);
		}

		public IAsyncResult BeginSubscribeToPushNotification(SubscribeToPushNotificationJsonRequest pushNotificationSubscription, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSubscribeToPushNotification(pushNotificationSubscription, asyncCallback, asyncState);
		}

		public SubscribeToPushNotificationJsonResponse EndSubscribeToPushNotification(IAsyncResult result)
		{
			return this.jsonService.EndSubscribeToPushNotification(result);
		}

		public IAsyncResult BeginUnsubscribeToPushNotification(UnsubscribeToPushNotificationJsonRequest pushNotificationSubscription, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUnsubscribeToPushNotification(pushNotificationSubscription, asyncCallback, asyncState);
		}

		public UnsubscribeToPushNotificationJsonResponse EndUnsubscribeToPushNotification(IAsyncResult result)
		{
			return this.jsonService.EndUnsubscribeToPushNotification(result);
		}

		public int PingOwa()
		{
			return new PingOwa(CallContext.Current).Execute();
		}

		public string SanitizeHtml(string input)
		{
			return new SanitizeHtmlCommand(CallContext.Current, input).Execute();
		}

		public IAsyncResult BeginSynchronizeWacAttachment(SynchronizeWacAttachmentRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return new SynchronizeWacAttachment(CallContext.Current, request.AttachmentId, asyncCallback, asyncState).Execute();
		}

		public SynchronizeWacAttachmentResponse EndSynchronizeWacAttachment(IAsyncResult result)
		{
			ServiceAsyncResult<SynchronizeWacAttachmentResponse> serviceAsyncResult = result as ServiceAsyncResult<SynchronizeWacAttachmentResponse>;
			if (serviceAsyncResult != null)
			{
				return serviceAsyncResult.Data;
			}
			throw new InvalidOperationException("IAsyncResult is null or not of type ServiceAsyncResult<SynchronizeWacAttachmentResponse>");
		}

		public IAsyncResult BeginPublishO365Notification(O365Notification notification, AsyncCallback asyncCallback, object asyncState)
		{
			return new PublishO365Notification(CallContext.Current, notification, asyncCallback, asyncState).Execute();
		}

		public bool EndPublishO365Notification(IAsyncResult result)
		{
			ServiceAsyncResult<bool> serviceAsyncResult = result as ServiceAsyncResult<bool>;
			return serviceAsyncResult.Data;
		}

		[OperationBehavior(AutoDisposeParameters = false)]
		public Stream GetFileAttachment(string id, bool isImagePreview, bool asDataUri)
		{
			return new GetAttachment(CallContext.Current, id, isImagePreview, asDataUri).Execute();
		}

		[OperationBehavior(AutoDisposeParameters = false)]
		public Stream GetAllAttachmentsAsZip(string id)
		{
			return new GetAllAttachmentsAsZip(CallContext.Current, id).Execute();
		}

		[OperationBehavior(AutoDisposeParameters = false)]
		public Stream GetPersonaPhoto(string personId, string adObjectId, string email, string singleSourceId, UserPhotoSize size)
		{
			return new GetPersonaPhoto(CallContext.Current, personId, adObjectId, email, singleSourceId, size).Execute();
		}

		public IAsyncResult BeginAddDelegate(AddDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddDelegate(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddDistributionGroupToImList(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAddImContactToGroup(AddImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddImContactToGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAddImGroup(AddImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddImGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddNewImContactToGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginAddNewTelUriContactToGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginApplyConversationAction(ApplyConversationActionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginApplyConversationAction(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginConvertId(ConvertIdJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginConvertId(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCopyFolder(CopyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCopyFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCopyItem(CopyItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCopyItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateAttachment(CreateAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateAttachment(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateFolder(CreateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateItem(CreateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginPostModernGroupItem(PostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginPostModernGroupItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateAndPostModernGroupItem(UpdateAndPostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateAndPostModernGroupItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateResponseFromModernGroup(CreateResponseFromModernGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateResponseFromModernGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateManagedFolder(CreateManagedFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateManagedFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCreateUserConfiguration(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDeleteAttachment(DeleteAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDeleteAttachment(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDeleteFolder(DeleteFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDeleteFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDeleteItem(DeleteItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDeleteItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDeleteUserConfiguration(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDisconnectPhoneCall(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginEmptyFolder(EmptyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginEmptyFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginExecuteDiagnosticMethod(ExecuteDiagnosticMethodJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginExecuteDiagnosticMethod(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginExpandDL(ExpandDLJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginExpandDL(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginExportItems(ExportItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginExportItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindConversation(FindConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			bool isSearchCall = SearchUtil.IsSearch(request.Body.QueryString);
			this.RegisterOwaCallback(isSearchCall);
			return this.jsonService.BeginFindConversation(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindTrendingConversation(FindTrendingConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindTrendingConversation(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindFolder(FindFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindItem(FindItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			bool isSearchCall = SearchUtil.IsSearch(request.Body.QueryString);
			this.RegisterOwaCallback(isSearchCall);
			return this.jsonService.BeginFindItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindMailboxStatisticsByKeywords(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindMessageTrackingReport(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginFindPeople(FindPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindPeople(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSyncPeople(SyncPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSyncPeople(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSyncAutoCompleteRecipients(SyncAutoCompleteRecipientsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSyncAutoCompleteRecipients(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetAttachment(GetAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetAttachment(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetConversationItems(GetConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetConversationItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetThreadedConversationItems(GetThreadedConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetThreadedConversationItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetConversationItemsDiagnostics(GetConversationItemsDiagnosticsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetConversationItemsDiagnostics(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetDelegate(GetDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetDelegate(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetEvents(GetEventsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetEvents(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetFolder(GetFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetHoldOnMailboxes(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetImItemList(GetImItemListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetImItemList(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetImItems(GetImItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetImItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetInboxRules(GetInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetInboxRules(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetClientAccessToken(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetItem(GetItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetMailTips(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetMessageTrackingReport(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetPasswordExpirationDate(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetPersona(GetPersonaJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetPersona(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetPhoneCallInformation(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetReminders(GetRemindersJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetReminders(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginPerformReminderAction(PerformReminderActionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginPerformReminderAction(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetRoomLists(GetRoomListsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetRoomLists(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetRooms(GetRoomsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetRooms(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetSearchableMailboxes(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetServerTimeZones(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetServiceConfiguration(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetSharingFolder(GetSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetSharingFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetSharingMetadata(GetSharingMetadataJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetSharingMetadata(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilityJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetUserAvailability(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetUserConfiguration(GetUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetUserConfiguration(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetUserOofSettings(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetModernConversationAttachments(GetModernConversationAttachmentsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetModernConversationAttachments(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetUserRetentionPolicyTags(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginMarkAllItemsAsRead(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginMarkAsJunk(MarkAsJunkJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginMarkAsJunk(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginMoveFolder(MoveFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginMoveFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginMoveItem(MoveItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginMoveItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginPlayOnPhone(PlayOnPhoneJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginPlayOnPhone(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginProvision(ProvisionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginProvision(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginDeprovision(DeprovisionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginDeprovision(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRefreshSharingFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRemoveContactFromImList(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRemoveDelegate(RemoveDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRemoveDelegate(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRemoveDistributionGroupFromImList(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRemoveImContactFromGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginRemoveImGroup(RemoveImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginRemoveImGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginResolveNames(ResolveNamesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginResolveNames(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSearchMailboxes(SearchMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSearchMailboxes(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSendItem(SendItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSendItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSetImGroup(SetImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSetImGroup(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSetImListMigrationCompleted(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSetHoldOnMailboxes(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSetUserOofSettings(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSubscribe(SubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSubscribe(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchyJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSyncFolderHierarchy(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSyncFolderItems(SyncFolderItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSyncFolderItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSyncConversation(SyncConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSyncConversation(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUnsubscribe(UnsubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUnsubscribe(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateDelegate(UpdateDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateDelegate(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateFolder(UpdateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateFolder(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateInboxRules(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateItem(UpdateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateItem(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUpdateUserConfiguration(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginUploadItems(UploadItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginUploadItems(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginLogPushNotificationData(LogPushNotificationDataJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginLogPushNotificationData(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetUserUnifiedGroups(GetUserUnifiedGroupsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetUserUnifiedGroups(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginGetClutterState(GetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetClutterState(request, asyncCallback, asyncState);
		}

		public IAsyncResult BeginSetClutterState(SetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginSetClutterState(request, asyncCallback, asyncState);
		}

		public LikeItemResponse LikeItem(LikeItemRequest request)
		{
			return this.jsonService.LikeItem(request);
		}

		public GetLikersResponseMessage GetLikers(GetLikersRequest request)
		{
			return this.jsonService.GetLikers(request);
		}

		public GetAggregatedAccountResponse GetAggregatedAccount(GetAggregatedAccountRequest request)
		{
			return this.jsonService.GetAggregatedAccount(request);
		}

		public AddAggregatedAccountResponse AddAggregatedAccount(AddAggregatedAccountRequest request)
		{
			return this.jsonService.AddAggregatedAccount(request);
		}

		public AddDelegateJsonResponse EndAddDelegate(IAsyncResult result)
		{
			return this.jsonService.EndAddDelegate(result);
		}

		public AddDistributionGroupToImListJsonResponse EndAddDistributionGroupToImList(IAsyncResult result)
		{
			return this.jsonService.EndAddDistributionGroupToImList(result);
		}

		public AddImContactToGroupJsonResponse EndAddImContactToGroup(IAsyncResult result)
		{
			return this.jsonService.EndAddImContactToGroup(result);
		}

		public AddImGroupJsonResponse EndAddImGroup(IAsyncResult result)
		{
			return this.jsonService.EndAddImGroup(result);
		}

		public AddNewImContactToGroupJsonResponse EndAddNewImContactToGroup(IAsyncResult result)
		{
			return this.jsonService.EndAddNewImContactToGroup(result);
		}

		public AddNewTelUriContactToGroupJsonResponse EndAddNewTelUriContactToGroup(IAsyncResult result)
		{
			return this.jsonService.EndAddNewTelUriContactToGroup(result);
		}

		public ApplyConversationActionJsonResponse EndApplyConversationAction(IAsyncResult result)
		{
			return this.jsonService.EndApplyConversationAction(result);
		}

		public ConvertIdJsonResponse EndConvertId(IAsyncResult result)
		{
			return this.jsonService.EndConvertId(result);
		}

		public CopyFolderJsonResponse EndCopyFolder(IAsyncResult result)
		{
			return this.jsonService.EndCopyFolder(result);
		}

		public CopyItemJsonResponse EndCopyItem(IAsyncResult result)
		{
			return this.jsonService.EndCopyItem(result);
		}

		public CreateAttachmentJsonResponse EndCreateAttachment(IAsyncResult result)
		{
			return this.jsonService.EndCreateAttachment(result);
		}

		public CreateFolderJsonResponse EndCreateFolder(IAsyncResult result)
		{
			return this.jsonService.EndCreateFolder(result);
		}

		public CreateItemJsonResponse EndCreateItem(IAsyncResult result)
		{
			return this.jsonService.EndCreateItem(result);
		}

		public PostModernGroupItemJsonResponse EndPostModernGroupItem(IAsyncResult result)
		{
			return this.jsonService.EndPostModernGroupItem(result);
		}

		public UpdateAndPostModernGroupItemJsonResponse EndUpdateAndPostModernGroupItem(IAsyncResult result)
		{
			return this.jsonService.EndUpdateAndPostModernGroupItem(result);
		}

		public CreateResponseFromModernGroupJsonResponse EndCreateResponseFromModernGroup(IAsyncResult result)
		{
			return this.jsonService.EndCreateResponseFromModernGroup(result);
		}

		public CreateManagedFolderJsonResponse EndCreateManagedFolder(IAsyncResult result)
		{
			return this.jsonService.EndCreateManagedFolder(result);
		}

		public CreateUserConfigurationJsonResponse EndCreateUserConfiguration(IAsyncResult result)
		{
			return this.jsonService.EndCreateUserConfiguration(result);
		}

		public DeleteAttachmentJsonResponse EndDeleteAttachment(IAsyncResult result)
		{
			return this.jsonService.EndDeleteAttachment(result);
		}

		public DeleteFolderJsonResponse EndDeleteFolder(IAsyncResult result)
		{
			return this.jsonService.EndDeleteFolder(result);
		}

		public DeleteItemJsonResponse EndDeleteItem(IAsyncResult result)
		{
			return this.jsonService.EndDeleteItem(result);
		}

		public DeleteUserConfigurationJsonResponse EndDeleteUserConfiguration(IAsyncResult result)
		{
			return this.jsonService.EndDeleteUserConfiguration(result);
		}

		public DisconnectPhoneCallJsonResponse EndDisconnectPhoneCall(IAsyncResult result)
		{
			return this.jsonService.EndDisconnectPhoneCall(result);
		}

		public EmptyFolderJsonResponse EndEmptyFolder(IAsyncResult result)
		{
			return this.jsonService.EndEmptyFolder(result);
		}

		public ExecuteDiagnosticMethodJsonResponse EndExecuteDiagnosticMethod(IAsyncResult result)
		{
			return this.jsonService.EndExecuteDiagnosticMethod(result);
		}

		public ExpandDLJsonResponse EndExpandDL(IAsyncResult result)
		{
			return this.jsonService.EndExpandDL(result);
		}

		public ExportItemsJsonResponse EndExportItems(IAsyncResult result)
		{
			return this.jsonService.EndExportItems(result);
		}

		public FindConversationJsonResponse EndFindConversation(IAsyncResult result)
		{
			return this.jsonService.EndFindConversation(result);
		}

		public FindConversationJsonResponse EndFindTrendingConversation(IAsyncResult result)
		{
			return this.jsonService.EndFindTrendingConversation(result);
		}

		public FindFolderJsonResponse EndFindFolder(IAsyncResult result)
		{
			return this.jsonService.EndFindFolder(result);
		}

		public FindItemJsonResponse EndFindItem(IAsyncResult result)
		{
			return this.jsonService.EndFindItem(result);
		}

		public FindMailboxStatisticsByKeywordsJsonResponse EndFindMailboxStatisticsByKeywords(IAsyncResult result)
		{
			return this.jsonService.EndFindMailboxStatisticsByKeywords(result);
		}

		public FindMessageTrackingReportJsonResponse EndFindMessageTrackingReport(IAsyncResult result)
		{
			return this.jsonService.EndFindMessageTrackingReport(result);
		}

		public FindPeopleJsonResponse EndFindPeople(IAsyncResult result)
		{
			return this.jsonService.EndFindPeople(result);
		}

		public SyncPeopleJsonResponse EndSyncPeople(IAsyncResult result)
		{
			return this.jsonService.EndSyncPeople(result);
		}

		public SyncAutoCompleteRecipientsJsonResponse EndSyncAutoCompleteRecipients(IAsyncResult result)
		{
			return this.jsonService.EndSyncAutoCompleteRecipients(result);
		}

		public GetAttachmentJsonResponse EndGetAttachment(IAsyncResult result)
		{
			return this.jsonService.EndGetAttachment(result);
		}

		public GetConversationItemsJsonResponse EndGetConversationItems(IAsyncResult result)
		{
			return this.jsonService.EndGetConversationItems(result);
		}

		public GetThreadedConversationItemsJsonResponse EndGetThreadedConversationItems(IAsyncResult result)
		{
			return this.jsonService.EndGetThreadedConversationItems(result);
		}

		public GetConversationItemsDiagnosticsJsonResponse EndGetConversationItemsDiagnostics(IAsyncResult result)
		{
			return this.jsonService.EndGetConversationItemsDiagnostics(result);
		}

		public GetModernConversationAttachmentsJsonResponse EndGetModernConversationAttachments(IAsyncResult result)
		{
			return this.jsonService.EndGetModernConversationAttachments(result);
		}

		public GetDelegateJsonResponse EndGetDelegate(IAsyncResult result)
		{
			return this.jsonService.EndGetDelegate(result);
		}

		public GetEventsJsonResponse EndGetEvents(IAsyncResult result)
		{
			return this.jsonService.EndGetEvents(result);
		}

		public GetFolderJsonResponse EndGetFolder(IAsyncResult result)
		{
			return this.jsonService.EndGetFolder(result);
		}

		public GetHoldOnMailboxesJsonResponse EndGetHoldOnMailboxes(IAsyncResult result)
		{
			return this.jsonService.EndGetHoldOnMailboxes(result);
		}

		public GetImItemListJsonResponse EndGetImItemList(IAsyncResult result)
		{
			return this.jsonService.EndGetImItemList(result);
		}

		public GetImItemsJsonResponse EndGetImItems(IAsyncResult result)
		{
			return this.jsonService.EndGetImItems(result);
		}

		public GetInboxRulesJsonResponse EndGetInboxRules(IAsyncResult result)
		{
			return this.jsonService.EndGetInboxRules(result);
		}

		public GetClientAccessTokenJsonResponse EndGetClientAccessToken(IAsyncResult result)
		{
			return this.jsonService.EndGetClientAccessToken(result);
		}

		public GetItemJsonResponse EndGetItem(IAsyncResult result)
		{
			return this.jsonService.EndGetItem(result);
		}

		public GetMailTipsJsonResponse EndGetMailTips(IAsyncResult result)
		{
			return this.jsonService.EndGetMailTips(result);
		}

		public GetMessageTrackingReportJsonResponse EndGetMessageTrackingReport(IAsyncResult result)
		{
			return this.jsonService.EndGetMessageTrackingReport(result);
		}

		public GetPasswordExpirationDateJsonResponse EndGetPasswordExpirationDate(IAsyncResult result)
		{
			return this.jsonService.EndGetPasswordExpirationDate(result);
		}

		public GetPersonaJsonResponse EndGetPersona(IAsyncResult result)
		{
			return this.jsonService.EndGetPersona(result);
		}

		public GetPhoneCallInformationJsonResponse EndGetPhoneCallInformation(IAsyncResult result)
		{
			return this.jsonService.EndGetPhoneCallInformation(result);
		}

		public GetRemindersJsonResponse EndGetReminders(IAsyncResult result)
		{
			return this.jsonService.EndGetReminders(result);
		}

		public PerformReminderActionJsonResponse EndPerformReminderAction(IAsyncResult result)
		{
			return this.jsonService.EndPerformReminderAction(result);
		}

		public GetRoomListsJsonResponse EndGetRoomLists(IAsyncResult result)
		{
			return this.jsonService.EndGetRoomLists(result);
		}

		public GetRoomsJsonResponse EndGetRooms(IAsyncResult result)
		{
			return this.jsonService.EndGetRooms(result);
		}

		public GetSearchableMailboxesJsonResponse EndGetSearchableMailboxes(IAsyncResult result)
		{
			return this.jsonService.EndGetSearchableMailboxes(result);
		}

		public GetServerTimeZonesJsonResponse EndGetServerTimeZones(IAsyncResult result)
		{
			return this.jsonService.EndGetServerTimeZones(result);
		}

		public GetServiceConfigurationJsonResponse EndGetServiceConfiguration(IAsyncResult result)
		{
			return this.jsonService.EndGetServiceConfiguration(result);
		}

		public GetSharingFolderJsonResponse EndGetSharingFolder(IAsyncResult result)
		{
			return this.jsonService.EndGetSharingFolder(result);
		}

		public GetSharingMetadataJsonResponse EndGetSharingMetadata(IAsyncResult result)
		{
			return this.jsonService.EndGetSharingMetadata(result);
		}

		public GetUserAvailabilityJsonResponse EndGetUserAvailability(IAsyncResult result)
		{
			return this.jsonService.EndGetUserAvailability(result);
		}

		public GetUserConfigurationJsonResponse EndGetUserConfiguration(IAsyncResult result)
		{
			return this.jsonService.EndGetUserConfiguration(result);
		}

		public GetUserOofSettingsJsonResponse EndGetUserOofSettings(IAsyncResult result)
		{
			return this.jsonService.EndGetUserOofSettings(result);
		}

		public GetUserRetentionPolicyTagsJsonResponse EndGetUserRetentionPolicyTags(IAsyncResult result)
		{
			return this.jsonService.EndGetUserRetentionPolicyTags(result);
		}

		public MarkAllItemsAsReadJsonResponse EndMarkAllItemsAsRead(IAsyncResult result)
		{
			return this.jsonService.EndMarkAllItemsAsRead(result);
		}

		public MarkAsJunkJsonResponse EndMarkAsJunk(IAsyncResult result)
		{
			return this.jsonService.EndMarkAsJunk(result);
		}

		public MoveFolderJsonResponse EndMoveFolder(IAsyncResult result)
		{
			return this.jsonService.EndMoveFolder(result);
		}

		public MoveItemJsonResponse EndMoveItem(IAsyncResult result)
		{
			return this.jsonService.EndMoveItem(result);
		}

		public PlayOnPhoneJsonResponse EndPlayOnPhone(IAsyncResult result)
		{
			return this.jsonService.EndPlayOnPhone(result);
		}

		public ProvisionJsonResponse EndProvision(IAsyncResult result)
		{
			return this.jsonService.EndProvision(result);
		}

		public DeprovisionJsonResponse EndDeprovision(IAsyncResult result)
		{
			return this.jsonService.EndDeprovision(result);
		}

		public RefreshSharingFolderJsonResponse EndRefreshSharingFolder(IAsyncResult result)
		{
			return this.jsonService.EndRefreshSharingFolder(result);
		}

		public RemoveContactFromImListJsonResponse EndRemoveContactFromImList(IAsyncResult result)
		{
			return this.jsonService.EndRemoveContactFromImList(result);
		}

		public RemoveDelegateJsonResponse EndRemoveDelegate(IAsyncResult result)
		{
			return this.jsonService.EndRemoveDelegate(result);
		}

		public RemoveDistributionGroupFromImListJsonResponse EndRemoveDistributionGroupFromImList(IAsyncResult result)
		{
			return this.jsonService.EndRemoveDistributionGroupFromImList(result);
		}

		public RemoveImContactFromGroupJsonResponse EndRemoveImContactFromGroup(IAsyncResult result)
		{
			return this.jsonService.EndRemoveImContactFromGroup(result);
		}

		public RemoveImGroupJsonResponse EndRemoveImGroup(IAsyncResult result)
		{
			return this.jsonService.EndRemoveImGroup(result);
		}

		public ResolveNamesJsonResponse EndResolveNames(IAsyncResult result)
		{
			return this.jsonService.EndResolveNames(result);
		}

		public SearchMailboxesJsonResponse EndSearchMailboxes(IAsyncResult result)
		{
			return this.jsonService.EndSearchMailboxes(result);
		}

		public SendItemJsonResponse EndSendItem(IAsyncResult result)
		{
			return this.jsonService.EndSendItem(result);
		}

		public SetHoldOnMailboxesJsonResponse EndSetHoldOnMailboxes(IAsyncResult result)
		{
			return this.jsonService.EndSetHoldOnMailboxes(result);
		}

		public SetImGroupJsonResponse EndSetImGroup(IAsyncResult result)
		{
			return this.jsonService.EndSetImGroup(result);
		}

		public SetImListMigrationCompletedJsonResponse EndSetImListMigrationCompleted(IAsyncResult result)
		{
			return this.jsonService.EndSetImListMigrationCompleted(result);
		}

		public SetUserOofSettingsJsonResponse EndSetUserOofSettings(IAsyncResult result)
		{
			return this.jsonService.EndSetUserOofSettings(result);
		}

		public SubscribeJsonResponse EndSubscribe(IAsyncResult result)
		{
			return this.jsonService.EndSubscribe(result);
		}

		public SyncFolderHierarchyJsonResponse EndSyncFolderHierarchy(IAsyncResult result)
		{
			return this.jsonService.EndSyncFolderHierarchy(result);
		}

		public SyncFolderItemsJsonResponse EndSyncFolderItems(IAsyncResult result)
		{
			return this.jsonService.EndSyncFolderItems(result);
		}

		public SyncConversationJsonResponse EndSyncConversation(IAsyncResult result)
		{
			return this.jsonService.EndSyncConversation(result);
		}

		public UnsubscribeJsonResponse EndUnsubscribe(IAsyncResult result)
		{
			return this.jsonService.EndUnsubscribe(result);
		}

		public UpdateDelegateJsonResponse EndUpdateDelegate(IAsyncResult result)
		{
			return this.jsonService.EndUpdateDelegate(result);
		}

		public UpdateFolderJsonResponse EndUpdateFolder(IAsyncResult result)
		{
			return this.jsonService.EndUpdateFolder(result);
		}

		public UpdateInboxRulesJsonResponse EndUpdateInboxRules(IAsyncResult result)
		{
			return this.jsonService.EndUpdateInboxRules(result);
		}

		public UpdateItemJsonResponse EndUpdateItem(IAsyncResult result)
		{
			return this.jsonService.EndUpdateItem(result);
		}

		public UpdateUserConfigurationJsonResponse EndUpdateUserConfiguration(IAsyncResult result)
		{
			return this.jsonService.EndUpdateUserConfiguration(result);
		}

		public UploadItemsJsonResponse EndUploadItems(IAsyncResult result)
		{
			return this.jsonService.EndUploadItems(result);
		}

		public LogPushNotificationDataJsonResponse EndLogPushNotificationData(IAsyncResult result)
		{
			return this.jsonService.EndLogPushNotificationData(result);
		}

		public GetUserUnifiedGroupsJsonResponse EndGetUserUnifiedGroups(IAsyncResult result)
		{
			return this.jsonService.EndGetUserUnifiedGroups(result);
		}

		public GetClutterStateJsonResponse EndGetClutterState(IAsyncResult result)
		{
			return this.jsonService.EndGetClutterState(result);
		}

		public SetClutterStateJsonResponse EndSetClutterState(IAsyncResult result)
		{
			return this.jsonService.EndSetClutterState(result);
		}

		public IAsyncResult BeginGetUserPhoto(string email, UserPhotoSize size, bool isPreview, bool fallbackToClearImage, AsyncCallback callback, object state)
		{
			return this.jsonService.BeginGetUserPhoto(email, size, isPreview, fallbackToClearImage, callback, state);
		}

		public Stream EndGetUserPhoto(IAsyncResult result)
		{
			return this.jsonService.EndGetUserPhoto(result);
		}

		public IAsyncResult BeginGetPeopleICommunicateWith(AsyncCallback callback, object state)
		{
			return this.jsonService.BeginGetPeopleICommunicateWith(callback, state);
		}

		public Stream EndGetPeopleICommunicateWith(IAsyncResult result)
		{
			return this.jsonService.EndGetPeopleICommunicateWith(result);
		}

		public IAsyncResult BeginGetTimeZoneOffsets(GetTimeZoneOffsetsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetTimeZoneOffsets(request, asyncCallback, asyncState);
		}

		public GetTimeZoneOffsetsJsonResponse EndGetTimeZoneOffsets(IAsyncResult result)
		{
			return this.jsonService.EndGetTimeZoneOffsets(result);
		}

		public ContactFolderResponse CreateContactFolder(BaseFolderId parentFolderId, string displayName, int priority)
		{
			return new CreateContactFolder(CallContext.Current, parentFolderId, displayName, priority).Execute();
		}

		public bool DeleteContactFolder(FolderId folderId)
		{
			return new DeleteContactFolder(CallContext.Current, folderId).Execute();
		}

		public ContactFolderResponse MoveContactFolder(FolderId folderId, int priority)
		{
			return new MoveContactFolder(CallContext.Current, folderId, priority).Execute();
		}

		public bool SetLayoutSettings(LayoutSettingsType layoutSettings)
		{
			return new SetLayoutSettings(CallContext.Current, layoutSettings).Execute();
		}

		public InlineExploreSpResultListType GetInlineExploreSpContent(string query, string targetUrl)
		{
			UserContext userContext = OWAService.GetUserContext();
			if (!userContext.FeaturesManager.ServerSettings.InlineExploreUI.Enabled)
			{
				throw new OwaInvalidRequestException("Service not enabled");
			}
			return new GetInlineExploreSpContentCommand(CallContext.Current, query, targetUrl).Execute();
		}

		public SuiteStorageJsonResponse ProcessSuiteStorage(SuiteStorageJsonRequest request)
		{
			return this.jsonService.ProcessSuiteStorage(request);
		}

		public IAsyncResult BeginGetWeatherForecast(GetWeatherForecastJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetWeatherForecast(request, asyncCallback, asyncState);
		}

		public GetWeatherForecastJsonResponse EndGetWeatherForecast(IAsyncResult result)
		{
			return this.jsonService.EndGetWeatherForecast(result);
		}

		public IAsyncResult BeginFindWeatherLocations(FindWeatherLocationsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginFindWeatherLocations(request, asyncCallback, asyncState);
		}

		public FindWeatherLocationsJsonResponse EndFindWeatherLocations(IAsyncResult result)
		{
			return this.jsonService.EndFindWeatherLocations(result);
		}

		public async Task<GetLinkPreviewResponse> GetLinkPreview(GetLinkPreviewRequest getLinkPreviewRequest)
		{
			return await new GetLinkPreview(CallContext.Current, getLinkPreviewRequest).Execute();
		}

		public GetBingMapsPreviewResponse GetBingMapsPreview(GetBingMapsPreviewRequest getBingMapsPreviewRequest)
		{
			return new GetBingMapsPreview(CallContext.Current, getBingMapsPreviewRequest).Execute();
		}

		public PerformInstantSearchResponse PerformInstantSearch(PerformInstantSearchRequest request)
		{
			UserContext userContext = OWAService.GetUserContext();
			CallContext.Current.HttpContext.Response.Buffer = false;
			PerformInstantSearch performInstantSearch = new PerformInstantSearch(CallContext.Current, userContext.InstantSearchManager, userContext.InstantSearchNotificationHandler, request);
			OWAService.PreExecute("PerformInstantSearch");
			PerformInstantSearchResponse result;
			try
			{
				PerformInstantSearchResponse value = performInstantSearch.Execute().Value;
				result = value;
			}
			finally
			{
				OWAService.PostExecute();
			}
			return result;
		}

		public EnableAppDataResponse EnableApp(EnableAppDataRequest request)
		{
			return this.jsonService.EnableApp(request);
		}

		public DisableAppDataResponse DisableApp(DisableAppDataRequest request)
		{
			return this.jsonService.DisableApp(request);
		}

		public RemoveAppDataResponse RemoveApp(RemoveAppDataRequest request)
		{
			return this.jsonService.RemoveApp(request);
		}

		public GetCalendarNotificationResponse GetCalendarNotification()
		{
			return this.jsonService.GetCalendarNotification();
		}

		public OptionsResponseBase SetCalendarNotification(SetCalendarNotificationRequest request)
		{
			return this.jsonService.SetCalendarNotification(request);
		}

		public GetCalendarProcessingResponse GetCalendarProcessing()
		{
			return this.jsonService.GetCalendarProcessing();
		}

		public OptionsResponseBase SetCalendarProcessing(SetCalendarProcessingRequest request)
		{
			return this.jsonService.SetCalendarProcessing(request);
		}

		public GetCASMailboxResponse GetCASMailbox()
		{
			return this.jsonService.GetCASMailbox();
		}

		public GetCASMailboxResponse GetCASMailbox2(GetCASMailboxRequest request)
		{
			return this.jsonService.GetCASMailbox2(request);
		}

		public SetCASMailboxResponse SetCASMailbox(SetCASMailboxRequest request)
		{
			return this.jsonService.SetCASMailbox(request);
		}

		public GetConnectedAccountsResponse GetConnectedAccounts(GetConnectedAccountsRequest request)
		{
			return this.jsonService.GetConnectedAccounts(request);
		}

		public GetConnectSubscriptionResponse GetConnectSubscription(GetConnectSubscriptionRequest request)
		{
			return this.jsonService.GetConnectSubscription(request);
		}

		public NewConnectSubscriptionResponse NewConnectSubscription(NewConnectSubscriptionRequest request)
		{
			return this.jsonService.NewConnectSubscription(request);
		}

		public SetConnectSubscriptionResponse SetConnectSubscription(SetConnectSubscriptionRequest request)
		{
			return this.jsonService.SetConnectSubscription(request);
		}

		public RemoveConnectSubscriptionResponse RemoveConnectSubscription(RemoveConnectSubscriptionRequest request)
		{
			return this.jsonService.RemoveConnectSubscription(request);
		}

		public GetHotmailSubscriptionResponse GetHotmailSubscription(IdentityRequest request)
		{
			return this.jsonService.GetHotmailSubscription(request);
		}

		public OptionsResponseBase SetHotmailSubscription(SetHotmailSubscriptionRequest request)
		{
			return this.jsonService.SetHotmailSubscription(request);
		}

		public GetImapSubscriptionResponse GetImapSubscription(IdentityRequest request)
		{
			return this.jsonService.GetImapSubscription(request);
		}

		public NewImapSubscriptionResponse NewImapSubscription(NewImapSubscriptionRequest request)
		{
			return this.jsonService.NewImapSubscription(request);
		}

		public OptionsResponseBase SetImapSubscription(SetImapSubscriptionRequest request)
		{
			return this.jsonService.SetImapSubscription(request);
		}

		public ImportContactListResponse ImportContactList(ImportContactListRequest request)
		{
			return this.jsonService.ImportContactList(request);
		}

		public DisableInboxRuleResponse DisableInboxRule(DisableInboxRuleRequest request)
		{
			return this.jsonService.DisableInboxRule(request);
		}

		public EnableInboxRuleResponse EnableInboxRule(EnableInboxRuleRequest request)
		{
			return this.jsonService.EnableInboxRule(request);
		}

		public GetInboxRuleResponse GetInboxRule(GetInboxRuleRequest request)
		{
			return this.jsonService.GetInboxRule(request);
		}

		public NewInboxRuleResponse NewInboxRule(NewInboxRuleRequest request)
		{
			return this.jsonService.NewInboxRule(request);
		}

		public RemoveInboxRuleResponse RemoveInboxRule(RemoveInboxRuleRequest request)
		{
			return this.jsonService.RemoveInboxRule(request);
		}

		public SetInboxRuleResponse SetInboxRule(SetInboxRuleRequest request)
		{
			return this.jsonService.SetInboxRule(request);
		}

		public GetMailboxResponse GetMailboxByIdentity(IdentityRequest request)
		{
			return this.jsonService.GetMailboxByIdentity(request);
		}

		public OptionsResponseBase SetMailbox(SetMailboxRequest request)
		{
			return this.jsonService.SetMailbox(request);
		}

		public GetMailboxAutoReplyConfigurationResponse GetMailboxAutoReplyConfiguration()
		{
			return this.jsonService.GetMailboxAutoReplyConfiguration();
		}

		public OptionsResponseBase SetMailboxAutoReplyConfiguration(SetMailboxAutoReplyConfigurationRequest request)
		{
			return this.jsonService.SetMailboxAutoReplyConfiguration(request);
		}

		public GetMailboxCalendarConfigurationResponse GetMailboxCalendarConfiguration()
		{
			return this.jsonService.GetMailboxCalendarConfiguration();
		}

		public OptionsResponseBase SetMailboxCalendarConfiguration(SetMailboxCalendarConfigurationRequest request)
		{
			return this.jsonService.SetMailboxCalendarConfiguration(request);
		}

		public GetMailboxJunkEmailConfigurationResponse GetMailboxJunkEmailConfiguration()
		{
			return this.jsonService.GetMailboxJunkEmailConfiguration();
		}

		public OptionsResponseBase SetMailboxJunkEmailConfiguration(SetMailboxJunkEmailConfigurationRequest request)
		{
			return this.jsonService.SetMailboxJunkEmailConfiguration(request);
		}

		public GetMailboxMessageConfigurationResponse GetMailboxMessageConfiguration()
		{
			return this.jsonService.GetMailboxMessageConfiguration();
		}

		public OptionsResponseBase SetMailboxMessageConfiguration(SetMailboxMessageConfigurationRequest request)
		{
			return this.jsonService.SetMailboxMessageConfiguration(request);
		}

		public GetMailboxRegionalConfigurationResponse GetMailboxRegionalConfiguration(GetMailboxRegionalConfigurationRequest request)
		{
			return this.jsonService.GetMailboxRegionalConfiguration(request);
		}

		public SetMailboxRegionalConfigurationResponse SetMailboxRegionalConfiguration(SetMailboxRegionalConfigurationRequest request)
		{
			return this.jsonService.SetMailboxRegionalConfiguration(request);
		}

		public GetMessageCategoryResponse GetMessageCategory()
		{
			return this.jsonService.GetMessageCategory();
		}

		public GetMessageClassificationResponse GetMessageClassification()
		{
			return this.jsonService.GetMessageClassification();
		}

		public GetAccountInformationResponse GetAccountInformation(GetAccountInformationRequest request)
		{
			return this.jsonService.GetAccountInformation(request);
		}

		public GetSocialNetworksOAuthInfoResponse GetConnectToSocialNetworksOAuthInfo(GetSocialNetworksOAuthInfoRequest request)
		{
			return this.jsonService.GetConnectToSocialNetworksOAuthInfo(request);
		}

		public GetPopSubscriptionResponse GetPopSubscription(IdentityRequest request)
		{
			return this.jsonService.GetPopSubscription(request);
		}

		public NewPopSubscriptionResponse NewPopSubscription(NewPopSubscriptionRequest request)
		{
			return this.jsonService.NewPopSubscription(request);
		}

		public OptionsResponseBase SetPopSubscription(SetPopSubscriptionRequest request)
		{
			return this.jsonService.SetPopSubscription(request);
		}

		public OptionsResponseBase AddActiveRetentionPolicyTags(IdentityCollectionRequest request)
		{
			return this.jsonService.AddActiveRetentionPolicyTags(request);
		}

		public GetRetentionPolicyTagsResponse GetActiveRetentionPolicyTags(GetRetentionPolicyTagsRequest request)
		{
			return this.jsonService.GetActiveRetentionPolicyTags(request);
		}

		public GetRetentionPolicyTagsResponse GetAvailableRetentionPolicyTags(GetRetentionPolicyTagsRequest request)
		{
			return this.jsonService.GetAvailableRetentionPolicyTags(request);
		}

		public OptionsResponseBase RemoveActiveRetentionPolicyTags(IdentityCollectionRequest request)
		{
			return this.jsonService.RemoveActiveRetentionPolicyTags(request);
		}

		public GetSendAddressResponse GetSendAddress()
		{
			return this.jsonService.GetSendAddress();
		}

		public GetSubscriptionResponse GetSubscription()
		{
			return this.jsonService.GetSubscription();
		}

		public NewSubscriptionResponse NewSubscription(NewSubscriptionRequest request)
		{
			return this.jsonService.NewSubscription(request);
		}

		public OptionsResponseBase RemoveSubscription(IdentityRequest request)
		{
			return this.jsonService.RemoveSubscription(request);
		}

		public SetUserResponse SetUser(SetUserRequest request)
		{
			return this.jsonService.SetUser(request);
		}

		public EndInstantSearchSessionResponse EndInstantSearchSession(string sessionId)
		{
			UserContext userContext = OWAService.GetUserContext();
			EndInstantSearchSessionRequest endInstantSearchSessionRequest = new EndInstantSearchSessionRequest();
			endInstantSearchSessionRequest.SessionId = sessionId;
			EndInstantSearchSession endInstantSearchSession = new EndInstantSearchSession(CallContext.Current, endInstantSearchSessionRequest, userContext.InstantSearchManager);
			return endInstantSearchSession.Execute().Value;
		}

		public GetMobileDeviceStatisticsResponse GetMobileDeviceStatistics(GetMobileDeviceStatisticsRequest request)
		{
			return this.jsonService.GetMobileDeviceStatistics(request);
		}

		public RemoveMobileDeviceResponse RemoveMobileDevice(RemoveMobileDeviceRequest request)
		{
			return this.jsonService.RemoveMobileDevice(request);
		}

		public ClearMobileDeviceResponse ClearMobileDevice(ClearMobileDeviceRequest request)
		{
			return this.jsonService.ClearMobileDevice(request);
		}

		public ClearTextMessagingAccountResponse ClearTextMessagingAccount(ClearTextMessagingAccountRequest request)
		{
			return this.jsonService.ClearTextMessagingAccount(request);
		}

		public GetTextMessagingAccountResponse GetTextMessagingAccount(GetTextMessagingAccountRequest request)
		{
			return this.jsonService.GetTextMessagingAccount(request);
		}

		public SetTextMessagingAccountResponse SetTextMessagingAccount(SetTextMessagingAccountRequest request)
		{
			return this.jsonService.SetTextMessagingAccount(request);
		}

		public CompareTextMessagingVerificationCodeResponse CompareTextMessagingVerificationCode(CompareTextMessagingVerificationCodeRequest request)
		{
			return this.jsonService.CompareTextMessagingVerificationCode(request);
		}

		public SendTextMessagingVerificationCodeResponse SendTextMessagingVerificationCode(SendTextMessagingVerificationCodeRequest request)
		{
			return this.jsonService.SendTextMessagingVerificationCode(request);
		}

		private static UserContext GetUserContext()
		{
			return UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
		}

		private static bool GetGroupCreationEnabledFromOwaMailboxPolicy()
		{
			PolicyConfiguration policyConfiguration = OwaMailboxPolicyCache.GetPolicyConfiguration(CallContext.Current.AccessingADUser.OwaMailboxPolicy, CallContext.Current.AccessingADUser.OrganizationId);
			if (policyConfiguration != null)
			{
				return policyConfiguration.GroupCreationEnabled;
			}
			if (ExEnvironment.IsTest && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				return true;
			}
			throw new OwaInvalidOperationException("Invalid owa mailbox policy returned.");
		}

		private void RegisterOwaCallback(bool isSearchCall)
		{
			if (isSearchCall)
			{
				IUserContext userContext = OWAService.GetUserContext();
				if (userContext != null && userContext.NotificationManager != null)
				{
					userContext.NotificationManager.SubscribeToSearchNotification();
					CallContext.Current.OwaCallback = userContext.NotificationManager.SearchNotificationHandler;
					return;
				}
			}
			if (CallContext.Current != null)
			{
				CallContext.Current.OwaCallback = NoOpOwaCallback.Prototype;
			}
		}

		public bool SendLinkClickedSignalToSP(SendLinkClickedSignalToSPRequest sendLinkClickedRequest)
		{
			OWAService.GetUserContext();
			return new SendLinkClickedSignalToSP(CallContext.Current, sendLinkClickedRequest).Execute();
		}

		public ValidateAggregatedConfigurationResponse ValidateAggregatedConfiguration(ValidateAggregatedConfigurationRequest request)
		{
			return new ValidateAggregatedConfiguration(CallContext.Current).Execute();
		}

		public IAsyncResult BeginCancelCalendarEvent(CancelCalendarEventJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginCancelCalendarEvent(request, asyncCallback, asyncState);
		}

		public CancelCalendarEventJsonResponse EndCancelCalendarEvent(IAsyncResult result)
		{
			return this.jsonService.EndCancelCalendarEvent(result);
		}

		public CalendarActionFolderIdResponse EnableBirthdayCalendar()
		{
			return new EnableBirthdayCalendarCommand(CallContext.Current).Execute();
		}

		public CalendarActionResponse DisableBirthdayCalendar()
		{
			return new DisableBirthdayCalendarCommand(CallContext.Current).Execute();
		}

		public CalendarActionResponse RemoveBirthdayEvent(Microsoft.Exchange.Services.Core.Types.ItemId contactId)
		{
			return new RemoveBirthdayEventCommand(CallContext.Current, contactId).Execute();
		}

		public IAsyncResult BeginGetBirthdayCalendarView(GetBirthdayCalendarViewJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return this.jsonService.BeginGetBirthdayCalendarView(request, asyncCallback, asyncState);
		}

		public GetBirthdayCalendarViewJsonResponse EndGetBirthdayCalendarView(IAsyncResult result)
		{
			return this.jsonService.EndGetBirthdayCalendarView(result);
		}

		public GetAllowedOptionsResponse GetAllowedOptions(GetAllowedOptionsRequest request)
		{
			return this.jsonService.GetAllowedOptions(request);
		}

		private static void PreExecute(string action)
		{
			RequestDetailsLogger protocolLog = CallContext.Current.ProtocolLog;
			if (protocolLog != null)
			{
				IActivityScope activityScope = protocolLog.ActivityScope;
				if (activityScope != null)
				{
					if (string.IsNullOrEmpty(activityScope.Component))
					{
						activityScope.Component = OWAService.component;
					}
					if (string.IsNullOrEmpty(activityScope.Action))
					{
						activityScope.Action = action;
					}
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(protocolLog, ServiceLatencyMetadata.PreExecutionLatency, activityScope.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private static void PostExecute()
		{
			RequestDetailsLogger protocolLog = CallContext.Current.ProtocolLog;
			if (protocolLog != null)
			{
				IActivityScope activityScope = protocolLog.ActivityScope;
				if (activityScope != null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(protocolLog, ServiceLatencyMetadata.CoreExecutionLatency, activityScope.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private static readonly string component = WorkloadType.Owa.ToString();

		private readonly JsonService jsonService;

		private readonly Action<string, Type> registerType = delegate(string s, Type type)
		{
			OwsLogRegistry.Register(s, type, new Type[0]);
		};
	}
}
