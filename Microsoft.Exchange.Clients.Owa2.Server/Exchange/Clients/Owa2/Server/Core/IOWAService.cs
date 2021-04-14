using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ServiceContract]
	public interface IOWAService : IJsonServiceContract
	{
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		PeopleFilter[] GetPeopleFilters();

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		SubscriptionResponseData[] SubscribeToNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonRequestWrapperType(typeof(UnsubscribeToNotificationRequest))]
		[OfflineClient(Queued = false)]
		bool UnsubscribeToNotification(SubscriptionData[] subscriptionData);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		SubscriptionResponseData[] SubscribeToGroupNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonRequestWrapperType(typeof(UnsubscribeToNotificationRequest))]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool UnsubscribeToGroupNotification(SubscriptionData[] subscriptionData);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		SubscriptionResponseData[] SubscribeToGroupUnseenNotification(NotificationSubscribeJsonRequest request, SubscriptionData[] subscriptionData);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonRequestWrapperType(typeof(UnsubscribeToNotificationRequest))]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool UnsubscribeToGroupUnseenNotification(SubscriptionData[] subscriptionData);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		bool AddSharedFolders(string displayName, string primarySMTPAddress);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool RemoveSharedFolders(string primarySMTPAddress);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		OwaOtherMailboxConfiguration GetOtherMailboxConfiguration();

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		NavBarData GetBposNavBarData();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		NavBarData GetBposShellInfoNavBarData();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetModernAttachmentsResponse GetModernAttachments(GetModernAttachmentsRequest request);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		CreateUnifiedGroupResponse CreateUnifiedGroup(CreateUnifiedGroupRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		AddMembersToUnifiedGroupResponse AddMembersToUnifiedGroup(AddMembersToUnifiedGroupRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetPersonaNotesResponse GetNotesForPersona(GetNotesForPersonaRequest getNotesForPersonaRequest);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		GetPersonaOrganizationHierarchyResponse GetOrganizationHierarchyForPersona(GetOrganizationHierarchyForPersonaRequest getOrganizationHierarchyForPersonaRequest);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		GetPersonaOrganizationHierarchyResponse GetPersonaOrganizationHierarchy(string galObjectGuid);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetGroupResponse GetGroup(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string adObjectId, EmailAddressWrapper emailAddress, IndexedPageView paging, GetGroupResultSet resultSet);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetGroupResponse GetGroupInfo(GetGroupInfoRequest getGroupInfoRequest);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		GetPersonaNotesResponse GetPersonaNotes(string personaId, int maxBytesToFetch);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		OwaUserConfiguration GetOwaUserConfiguration();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		Alert[] GetSystemAlerts();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		bool SetNotificationSettings(NotificationSettingsJsonRequest settings);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		ScopeFlightsSetting[] GetFlightsSettings();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		ComplianceConfiguration GetComplianceConfiguration();

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		AttachmentDataProvider AddAttachmentDataProvider(AttachmentDataProvider attachmentDataProvider);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[Obsolete]
		AttachmentDataProvider[] GetAttachmentDataProviders();

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		AttachmentDataProvider[] GetAllAttachmentDataProviders(GetAttachmentDataProvidersRequest request);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		AttachmentDataProviderType GetAttachmentDataProviderTypes();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderItems(GetAttachmentDataProviderItemsRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderRecentItems();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetAttachmentDataProviderItemsResponse GetAttachmentDataProviderGroups();

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		string CreateReferenceAttachmentFromLocalFile(CreateReferenceAttachmentFromLocalFileRequest requestObject);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonRequestWrapperType(typeof(CreateAttachmentFromAttachmentDataProviderRequest))]
		[OfflineClient(Queued = false)]
		string CreateAttachmentFromAttachmentDataProvider(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string attachmentDataProviderId, string location, string attachmentId, string subscriptionId, string channelId, string dataProviderParentItemId, string providerEndpointUrl, string cancellationId = null);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool CancelAttachment(string cancellationId);

		[JsonRequestWrapperType(typeof(CreateReferenceAttachmentFromAttachmentDataProviderRequest))]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		CreateAttachmentResponse CreateReferenceAttachmentFromAttachmentDataProvider(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string attachmentDataProviderId, string location, string attachmentId, string dataProviderParentItemId, string providerEndpointUrl, string cancellationId = null);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		string GetAttachmentDataProviderUploadFolderName();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		TargetFolderMruConfiguration GetFolderMruConfiguration();

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool SetFolderMruConfiguration(TargetFolderMruConfiguration folderMruConfiguration);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		UcwaUserConfiguration GetUcwaUserConfiguration(string sipUri);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		OnlineMeetingType CreateOnlineMeeting(string sipUri, Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		OnlineMeetingType CreateMeetNow(string sipUri, string subject);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		string GetWacIframeUrl(string attachmentId);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		string GetWacIframeUrlForOneDrive(GetWacIframeUrlForOneDriveRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		WacAttachmentType GetWacAttachmentInfo(string attachmentId, bool isEdit, string draftId);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		string CreateResendDraft(string ndrMessageId, string draftsFolderId);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		CreateAttachmentJsonResponse CreateAttachmentFromLocalFile(CreateAttachmentJsonRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		CreateAttachmentJsonResponse CreateAttachmentFromForm();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetFlowConversationResponse GetFlowConversation(BaseFolderId folderId, int conversationCount);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		FindFlowConversationItemResponse FindFlowConversationItem(BaseFolderId folderId, string flowConversationId, int itemCount);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		string UploadAndShareAttachmentFromForm();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		string UpdateAttachmentPermissions(UpdateAttachmentPermissionsRequest permissionsRequest);

		[OfflineClient(Queued = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[JsonRequestWrapperType(typeof(LogDatapointRequest))]
		bool LogDatapoint(Datapoint[] datapoints);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		PerformInstantSearchResponse PerformInstantSearch(PerformInstantSearchRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		EndInstantSearchSessionResponse EndInstantSearchSession(string sessionId);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool ConnectedAccountsNotification(bool isOWALogon);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		UploadPhotoResponse UploadPhoto(UploadPhotoRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		UploadPhotoResponse UploadPhotoFromForm();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		int VerifyCert(string certRawData);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[Obsolete]
		GetCertsResponse GetCerts(GetCertsRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetCertsResponse GetEncryptionCerts(GetCertsRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		GetCertsInfoResponse GetCertsInfo(string certRawData, bool isSend);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		string GetMime(Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		int InstantMessageSignIn(bool signedInManually);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int InstantMessageSignOut(bool reserved);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int SendChatMessage(ChatMessage message);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool TerminateChatSession(int chatSessionId);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		int AcceptChatSession(int chatSessionId);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool AcceptBuddy(InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool AddImBuddy(InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool DeclineBuddy(InstantMessageBuddy instantMessageBuddy);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool RemoveBuddy(InstantMessageBuddy instantMessageBuddy, Microsoft.Exchange.Services.Core.Types.ItemId contactId);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool AddFavorite(InstantMessageBuddy instantMessageBuddy);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		bool RemoveFavorite(Microsoft.Exchange.Services.Core.Types.ItemId personaId);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool NotifyAppWipe(DataWipeReason wipeReason);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool NotifyTyping(int chatSessionId, bool typingCancelled);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int SetPresence(InstantMessagePresence presenceSetting);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		int ResetPresence();

		[JsonRequestWrapperType(typeof(GetPresenceRequest))]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int GetPresence(string[] sipUris);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestWrapperType(typeof(GetPresenceRequest))]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int SubscribeForPresenceUpdates(string[] sipUris);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		int UnsubscribeFromPresenceUpdates(string sipUri);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[JsonRequestWrapperType(typeof(GetInstantMessageProxySettingsRequest))]
		ProxySettings[] GetInstantMessageProxySettings(string[] userPrincipalNames);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		ThemeSelectionInfoType GetThemes();

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		bool SetTheme(string theme);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		SetUserThemeResponse SetUserTheme(SetUserThemeRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		TimeZoneConfiguration GetTimeZone(bool needTimeZoneList);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool SetTimeZone(string timezone);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		bool SetUserLocale(string userLocale, bool localizeFolders);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HttpHeaders)]
		UserOofSettingsType GetOwaUserOofSettings();

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		bool SetOwaUserOofSettings(UserOofSettingsType userOofSettings);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		EmailSignatureConfiguration GetOwaUserEmailSignature();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool SetOwaUserEmailSignature(EmailSignatureConfiguration userEmailSignature);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		int GetDaysUntilPasswordExpiration();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		EwsRoomType[] GetRoomsInternal(string roomList);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		OptionSummary GetOptionSummary();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetWellKnownShapesResponse GetWellKnownShapes();

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		IAsyncResult BeginSubscribeToPushNotification(SubscribeToPushNotificationJsonRequest pushNotificationSubscription, AsyncCallback asyncCallback, object asyncState);

		SubscribeToPushNotificationJsonResponse EndSubscribeToPushNotification(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnsubscribeToPushNotification(UnsubscribeToPushNotificationJsonRequest pushNotificationSubscription, AsyncCallback asyncCallback, object asyncState);

		UnsubscribeToPushNotificationJsonResponse EndUnsubscribeToPushNotification(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		ContactFolderResponse CreateContactFolder(BaseFolderId parentFolderId, string displayName, int priority);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		bool DeleteContactFolder(FolderId folderId);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		ContactFolderResponse MoveContactFolder(FolderId folderId, int priority);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		bool SetLayoutSettings(LayoutSettingsType layoutSettings);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		Task<GetLinkPreviewResponse> GetLinkPreview(GetLinkPreviewRequest getLinkPreviewRequest);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetBingMapsPreviewResponse GetBingMapsPreview(GetBingMapsPreviewRequest getBingMapsPreviewRequest);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		InlineExploreSpResultListType GetInlineExploreSpContent(string query, string targetUrl);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		int PingOwa();

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDlpPolicyTips(GetDlpPolicyTipsRequest request, AsyncCallback asyncCallback, object asyncState);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		string CreateAttachmentFromUri(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string uri, string name, string subscriptionId);

		GetDlpPolicyTipsResponse EndGetDlpPolicyTips(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		string SanitizeHtml(string input);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginSynchronizeWacAttachment(SynchronizeWacAttachmentRequest request, AsyncCallback asyncCallback, object asyncState);

		SynchronizeWacAttachmentResponse EndSynchronizeWacAttachment(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		IAsyncResult BeginPublishO365Notification(O365Notification notification, AsyncCallback asyncCallback, object asyncState);

		bool EndPublishO365Notification(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OperationContract]
		[OfflineClient(Queued = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool SendLinkClickedSignalToSP(SendLinkClickedSignalToSPRequest sendLinkClickedRequest);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		ValidateAggregatedConfigurationResponse ValidateAggregatedConfiguration(ValidateAggregatedConfigurationRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		FindMembersInUnifiedGroupResponse FindMembersInUnifiedGroup(FindMembersInUnifiedGroupRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		GetRegionalConfigurationResponse GetRegionalConfiguration(GetRegionalConfigurationRequest request);
	}
}
