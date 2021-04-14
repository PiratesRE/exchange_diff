using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal interface IExchangeService : IDisposable
	{
		AddAggregatedAccountResponse AddAggregatedAccount(AddAggregatedAccountRequest request, ExecutionOption executionOption = null);

		Task<AddAggregatedAccountResponse> AddAggregatedAccountAsync(AddAggregatedAccountRequest request, ExecutionOption executionOption = null);

		IsOffice365DomainResponse IsOffice365Domain(IsOffice365DomainRequest request, ExecutionOption executionOption = null);

		Task<IsOffice365DomainResponse> IsOffice365DomainAsync(IsOffice365DomainRequest request, ExecutionOption executionOption = null);

		GetAggregatedAccountResponse GetAggregatedAccount(GetAggregatedAccountRequest request, ExecutionOption executionOption = null);

		Task<GetAggregatedAccountResponse> GetAggregatedAccountAsync(GetAggregatedAccountRequest request, ExecutionOption executionOption = null);

		RemoveAggregatedAccountResponse RemoveAggregatedAccount(RemoveAggregatedAccountRequest request, ExecutionOption executionOption = null);

		Task<RemoveAggregatedAccountResponse> RemoveAggregatedAccountAsync(RemoveAggregatedAccountRequest request, ExecutionOption executionOption = null);

		SetAggregatedAccountResponse SetAggregatedAccount(SetAggregatedAccountRequest request, ExecutionOption executionOption = null);

		Task<SetAggregatedAccountResponse> SetAggregatedAccountAsync(SetAggregatedAccountRequest request, ExecutionOption executionOption = null);

		CreateUnifiedMailboxResponse CreateUnifiedMailbox(CreateUnifiedMailboxRequest request, ExecutionOption executionOption = null);

		Task<CreateUnifiedMailboxResponse> CreateUnifiedMailboxAsync(CreateUnifiedMailboxRequest request, ExecutionOption executionOption = null);

		GetFolderResponse GetFolder(GetFolderRequest request, ExecutionOption executionOption = null);

		Task<GetFolderResponse> GetFolderAsync(GetFolderRequest request, ExecutionOption executionOption = null);

		FindFolderResponse FindFolder(FindFolderRequest request, ExecutionOption executionOption = null);

		Task<FindFolderResponse> FindFolderAsync(FindFolderRequest request, ExecutionOption executionOption = null);

		CreateFolderResponse CreateFolder(CreateFolderRequest request, ExecutionOption executionOption = null);

		Task<CreateFolderResponse> CreateFolderAsync(CreateFolderRequest request, ExecutionOption executionOption = null);

		DeleteFolderResponse DeleteFolder(DeleteFolderRequest request, ExecutionOption executionOption = null);

		Task<DeleteFolderResponse> DeleteFolderAsync(DeleteFolderRequest request, ExecutionOption executionOption = null);

		UpdateFolderResponse UpdateFolder(UpdateFolderRequest request, ExecutionOption executionOption = null);

		Task<UpdateFolderResponse> UpdateFolderAsync(UpdateFolderRequest request, ExecutionOption executionOption = null);

		CopyFolderResponse CopyFolder(CopyFolderRequest request, ExecutionOption executionOption = null);

		Task<CopyFolderResponse> CopyFolderAsync(CopyFolderRequest request, ExecutionOption executionOption = null);

		MoveFolderResponse MoveFolder(MoveFolderRequest request, ExecutionOption executionOption = null);

		Task<MoveFolderResponse> MoveFolderAsync(MoveFolderRequest request, ExecutionOption executionOption = null);

		GetFavoritesResponse GetFavoriteFolders(ExecutionOption executionOption = null);

		Task<GetFavoritesResponse> GetFavoriteFoldersAsync(ExecutionOption executionOption = null);

		UpdateFavoriteFolderResponse UpdateFavoriteFolder(UpdateFavoriteFolderRequest request, ExecutionOption executionOption = null);

		Task<UpdateFavoriteFolderResponse> UpdateFavoriteFolderAsync(UpdateFavoriteFolderRequest request, ExecutionOption executionOption = null);

		GetItemResponse GetItem(GetItemRequest request, ExecutionOption executionOption = null);

		Task<GetItemResponse> GetItemAsync(GetItemRequest request, ExecutionOption executionOption = null);

		FindItemResponse FindItem(FindItemRequest request, ExecutionOption executionOption = null);

		Task<FindItemResponse> FindItemAsync(FindItemRequest request, ExecutionOption executionOption = null);

		IDisposableResponse<CreateItemResponse> CreateItem(CreateItemRequest request, ExecutionOption executionOption = null);

		Task<IDisposableResponse<CreateItemResponse>> CreateItemAsync(CreateItemRequest request, ExecutionOption executionOption = null);

		DeleteItemResponse DeleteItem(DeleteItemRequest request, ExecutionOption executionOption = null);

		Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, ExecutionOption executionOption = null);

		CopyItemResponse CopyItem(CopyItemRequest request, ExecutionOption executionOption = null);

		Task<CopyItemResponse> CopyItemAsync(CopyItemRequest request, ExecutionOption executionOption = null);

		MoveItemResponse MoveItem(MoveItemRequest request, ExecutionOption executionOption = null);

		Task<MoveItemResponse> MoveItemAsync(MoveItemRequest request, ExecutionOption executionOption = null);

		SendItemResponse SendItem(SendItemRequest request, ExecutionOption executionOption = null);

		Task<SendItemResponse> SendItemAsync(SendItemRequest request, ExecutionOption executionOption = null);

		GetConversationItemsResponse GetConversationItems(GetConversationItemsRequest request, ExecutionOption executionOption = null);

		Task<GetThreadedConversationItemsResponse> GetThreadedConversationItemsAsync(GetThreadedConversationItemsRequest request, ExecutionOption executionOption = null);

		GetThreadedConversationItemsResponse GetThreadedConversationItems(GetThreadedConversationItemsRequest request, ExecutionOption executionOption = null);

		Task<GetConversationItemsResponse> GetConversationItemsAsync(GetConversationItemsRequest request, ExecutionOption executionOption = null);

		FindConversationResponseMessage FindConversation(FindConversationRequest request, ExecutionOption executionOption = null);

		Task<FindConversationResponseMessage> FindConversationAsync(FindConversationRequest request, ExecutionOption executionOption = null);

		SyncFolderHierarchyResponse SyncFolderHierarchy(SyncFolderHierarchyRequest request, ExecutionOption executionOption = null);

		Task<SyncFolderHierarchyResponse> SyncFolderHierarchyAsync(SyncFolderHierarchyRequest request, ExecutionOption executionOption = null);

		SyncFolderItemsResponse SyncFolderItems(SyncFolderItemsRequest request, ExecutionOption executionOption = null);

		Task<SyncFolderItemsResponse> SyncFolderItemsAsync(SyncFolderItemsRequest request, ExecutionOption executionOption = null);

		SyncCalendarResponse SyncCalendar(SyncCalendarParameters request, ExecutionOption executionOption = null);

		Task<SyncCalendarResponse> SyncCalendarAsync(SyncCalendarParameters request, ExecutionOption executionOption = null);

		SyncConversationResponseMessage SyncConversation(SyncConversationRequest request, ExecutionOption executionOption = null);

		Task<SyncConversationResponseMessage> SyncConversationAsync(SyncConversationRequest request, ExecutionOption executionOption = null);

		GetModernGroupResponse GetModernGroup(GetModernGroupRequest request, ExecutionOption executionOption = null);

		Task<GetModernGroupResponse> GetModernGroupAsync(GetModernGroupRequest request, ExecutionOption executionOption = null);

		UpdateItemResponse UpdateItem(UpdateItemRequest request, ExecutionOption executionOption = null);

		Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, ExecutionOption executionOption = null);

		GetUserPhotoResponse GetUserPhoto(GetUserPhotoRequest request, ExecutionOption executionOption = null);

		Task<GetUserPhotoResponse> GetUserPhotoAsync(GetUserPhotoRequest request, ExecutionOption executionOption = null);

		GetPeopleICommunicateWithResponse GetPeopleICommunicateWith(GetPeopleICommunicateWithRequest request, ExecutionOption executionOption = null);

		Task<GetPeopleICommunicateWithResponse> GetPeopleICommunicateWithAsync(GetPeopleICommunicateWithRequest request, ExecutionOption executionOption = null);

		ResolveNamesResponse ResolveNames(ResolveNamesRequest request, ExecutionOption executionOption = null);

		Task<ResolveNamesResponse> ResolveNamesAsync(ResolveNamesRequest request, ExecutionOption executionOption = null);

		ApplyConversationActionResponse ApplyConversationAction(ApplyConversationActionRequest request, ExecutionOption executionOption = null);

		Task<ApplyConversationActionResponse> ApplyConversationActionAsync(ApplyConversationActionRequest request, ExecutionOption executionOption = null);

		GetCalendarEventResponse GetCalendarEvent(GetCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<GetCalendarEventResponse> GetCalendarEventAsync(GetCalendarEventRequest request, ExecutionOption executionOption = null);

		GetCalendarViewResponse GetCalendarView(GetCalendarViewRequest request, ExecutionOption executionOption = null);

		Task<GetCalendarViewResponse> GetCalendarViewAsync(GetCalendarViewRequest request, ExecutionOption executionOption = null);

		CancelCalendarEventResponse CancelCalendarEvent(CancelCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<CancelCalendarEventResponse> CancelCalendarEventAsync(CancelCalendarEventRequest request, ExecutionOption executionOption = null);

		CreateCalendarEventResponse CreateCalendarEvent(CreateCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<CreateCalendarEventResponse> CreateCalendarEventAsync(CreateCalendarEventRequest request, ExecutionOption executionOption = null);

		RespondToCalendarEventResponse RespondToCalendarEvent(RespondToCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<RespondToCalendarEventResponse> RespondToCalendarEventAsync(RespondToCalendarEventRequest request, ExecutionOption executionOption = null);

		ExpandCalendarEventResponse ExpandCalendarEvent(ExpandCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<ExpandCalendarEventResponse> ExpandCalendarEventAsync(ExpandCalendarEventRequest request, ExecutionOption executionOption = null);

		RefreshGALContactsFolderResponse RefreshGALContactsFolder(RefreshGALContactsFolderRequest request, ExecutionOption executionOption = null);

		Task<RefreshGALContactsFolderResponse> RefreshGALContactsFolderAsync(RefreshGALContactsFolderRequest request, ExecutionOption executionOption = null);

		UpdateCalendarEventResponse UpdateCalendarEvent(UpdateCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<UpdateCalendarEventResponse> UpdateCalendarEventAsync(UpdateCalendarEventRequest request, ExecutionOption executionOption = null);

		DeleteCalendarEventResponse DeleteCalendarEvent(DeleteCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<DeleteCalendarEventResponse> DeleteCalendarEventAsync(DeleteCalendarEventRequest request, ExecutionOption executionOption = null);

		ForwardCalendarEventResponse ForwardCalendarEvent(ForwardCalendarEventRequest request, ExecutionOption executionOption = null);

		Task<ForwardCalendarEventResponse> ForwardCalendarEventAsync(ForwardCalendarEventRequest request, ExecutionOption executionOption = null);

		IDisposableResponse<GetAttachmentResponse> GetAttachment(GetAttachmentRequest request, ExecutionOption executionOption = null);

		Task<IDisposableResponse<GetAttachmentResponse>> GetAttachmentAsync(GetAttachmentRequest request, ExecutionOption executionOption = null);

		IDisposableResponse<CreateAttachmentResponse> CreateAttachment(CreateAttachmentRequest request, ExecutionOption executionOption = null);

		Task<IDisposableResponse<CreateAttachmentResponse>> CreateAttachmentAsync(CreateAttachmentRequest request, ExecutionOption executionOption = null);

		DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request, ExecutionOption executionOption = null);

		Task<DeleteAttachmentResponse> DeleteAttachmentAsync(DeleteAttachmentRequest request, ExecutionOption executionOption = null);

		ProvisionResponse Provision(ProvisionRequest request, ExecutionOption executionOption = null);

		Task<ProvisionResponse> ProvisionAsync(ProvisionRequest request, ExecutionOption executionOption = null);

		FindPeopleResponseMessage FindPeople(FindPeopleRequest request, ExecutionOption executionOption = null);

		Task<FindPeopleResponseMessage> FindPeopleAsync(FindPeopleRequest request, ExecutionOption executionOption = null);

		SyncAutoCompleteRecipientsResponseMessage SyncAutoCompleteRecipients(SyncAutoCompleteRecipientsRequest request, ExecutionOption executionOption = null);

		Task<SyncAutoCompleteRecipientsResponseMessage> SyncAutoCompleteRecipientsAsync(SyncAutoCompleteRecipientsRequest request, ExecutionOption executionOption = null);

		GetPersonaResponseMessage GetPersona(GetPersonaRequest request, ExecutionOption executionOption = null);

		Task<GetPersonaResponseMessage> GetPersonaAsync(GetPersonaRequest request, ExecutionOption executionOption = null);

		ConversationFeedLoader GetConversationFeedLoader(ExTimeZone timezone);

		Guid GetMailboxGuid();

		Guid GetConversationGuidFromEwsId(string ewsId);

		GetComplianceConfigurationResponseMessage GetComplianceConfiguration(GetComplianceConfigurationRequest request, ExecutionOption executionOption = null);

		Guid GetMailboxGuidFromEwsId(string ewsId);

		string GetEwsIdFromConversationGuid(Guid conversationGuid, Guid mailboxGuid);

		void GetItemMidFidDateFromEwsId(string ewsId, out long mid, out long fid, out ExDateTime date);

		string GetEwsIdFromItemMidFidDate(long mid, long fid, ExDateTime date, Guid mailboxGuid);

		bool GetFolderFidAndMailboxFromEwsId(string ewsId, out long fid, out Guid mailboxGuid);

		long GetFolderFidFromEwsId(string ewsId);

		string GetEwsIdFromFolderFid(long fid, Guid mailboxGuid);

		string GetDistinguishedFolderIdFromEwsId(string ewsId);

		Guid GetAttachmentGuidFromEwsId(string ewsId, long mid, long fid);

		string GetEwsIdFromAttachmentGuid(Guid attachmentGuid, long mid, long fid, Guid mailboxGuid);

		SubscribeToConversationChangesResponseMessage SubscribeToConversationChanges(SubscribeToConversationChangesRequest request, Action<ConversationNotification> callback, ExecutionOption executionOption = null);

		Task<SubscribeToConversationChangesResponseMessage> SubscribeToConversationChangesAsync(SubscribeToConversationChangesRequest request, Action<ConversationNotification> callback, ExecutionOption executionOption = null);

		void SetCallContextFromActionInfo(string actionQueueId, string protocol, string deviceType, string actionId, bool IsOutlookService);

		void DisableDupDetection();

		bool TryGetResponse<T>(out T results);

		void SetResponse<T>(T result, Exception exception);

		bool? GetIsDuplicatedAction();

		bool GetReturningSavedResult();

		bool GetResultSaved();

		QuotedTextResult ParseQuotedText(string msg, bool reorderMsgs);

		IOutlookServiceStorage GetOutlookServiceStorage();

		SubscribeToCalendarChangesResponseMessage SubscribeToCalendarChanges(SubscribeToCalendarChangesRequest request, Action<CalendarChangeNotificationType> callback, ExecutionOption executionOption = null);

		Task<SubscribeToCalendarChangesResponseMessage> SubscribeToCalendarChangesAsync(SubscribeToCalendarChangesRequest request, Action<CalendarChangeNotificationType> callback, ExecutionOption executionOption = null);

		GetCalendarFoldersResponse GetCalendarFolders(ExecutionOption executionOption = null);

		IHtmlReader GetHtmlReader(string html);

		PerformInstantSearchResponse PerformInstantSearch(PerformInstantSearchRequest request, Action<InstantSearchPayloadType> searchPayloadCallback, ExecutionOption executionOption = null);

		Task<PerformInstantSearchResponse> PerformInstantSearchAsync(PerformInstantSearchRequest request, Action<InstantSearchPayloadType> searchPayloadCallback, ExecutionOption executionOption = null);

		EndInstantSearchSessionResponse EndInstantSearchSession(string deviceId, string sessionId, ExecutionOption executionOption = null);

		Task<EndInstantSearchSessionResponse> EndInstantSearchSessionAsync(string deviceId, string sessionId, ExecutionOption executionOption = null);

		string GetBodyWithQuotedText(string messageBody, List<string> messageIds, List<string> messageBodyColl);

		MasterCategoryListActionResponse GetMasterCategoryList(GetMasterCategoryListRequest request, ExecutionOption executionOption = null);

		GetUserAvailabilityResponse GetUserAvailability(GetUserAvailabilityRequest request, ExecutionOption executionOption = null);

		SubscribeToHierarchyChangesResponseMessage SubscribeToHierarchyChanges(SubscribeToHierarchyChangesRequest request, Action<HierarchyNotification> callback, ExecutionOption executionOption = null);

		Task<SubscribeToHierarchyChangesResponseMessage> SubscribeToHierarchyChangesAsync(SubscribeToHierarchyChangesRequest request, Action<HierarchyNotification> callback, ExecutionOption executionOption = null);

		SubscribeToMessageChangesResponseMessage SubscribeToMessageChanges(SubscribeToMessageChangesRequest request, Action<MessageNotification> callback, ExecutionOption executionOption = null);

		Task<SubscribeToMessageChangesResponseMessage> SubscribeToMessageChangesAsync(SubscribeToMessageChangesRequest request, Action<MessageNotification> callback, ExecutionOption executionOption = null);

		void SetRequestTimeZoneId(string timeZoneId);

		ExDateTime GetOriginalStartDateFromEwsId(string ewsId);

		PatternedRecurrence ConvertToPatternedRecurrence(Recurrence value);
	}
}
