using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ServiceContract]
	public interface IJsonServiceContract
	{
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginConvertId(ConvertIdJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ConvertIdJsonResponse EndConvertId(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUploadItems(UploadItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UploadItemsJsonResponse EndUploadItems(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginExportItems(ExportItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ExportItemsJsonResponse EndExportItems(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginGetFolder(GetFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetFolderJsonResponse EndGetFolder(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginCreateFolder(CreateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateFolderJsonResponse EndCreateFolder(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeleteFolder(DeleteFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DeleteFolderJsonResponse EndDeleteFolder(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginEmptyFolder(EmptyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		EmptyFolderJsonResponse EndEmptyFolder(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginUpdateFolder(UpdateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateFolderJsonResponse EndUpdateFolder(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginMoveFolder(MoveFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		MoveFolderJsonResponse EndMoveFolder(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginCopyFolder(CopyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CopyFolderJsonResponse EndCopyFolder(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFindItem(FindItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindItemJsonResponse EndFindItem(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFindFolder(FindFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindFolderJsonResponse EndFindFolder(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginGetItem(GetItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetItemJsonResponse EndGetItem(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginPostModernGroupItem(PostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		PostModernGroupItemJsonResponse EndPostModernGroupItem(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginUpdateAndPostModernGroupItem(UpdateAndPostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateAndPostModernGroupItemJsonResponse EndUpdateAndPostModernGroupItem(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginCreateResponseFromModernGroup(CreateResponseFromModernGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateResponseFromModernGroupJsonResponse EndCreateResponseFromModernGroup(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginCreateItem(CreateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateItemJsonResponse EndCreateItem(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = true)]
		IAsyncResult BeginDeleteItem(DeleteItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DeleteItemJsonResponse EndDeleteItem(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUpdateItem(UpdateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateItemJsonResponse EndUpdateItem(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSendItem(SendItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SendItemJsonResponse EndSendItem(IAsyncResult result);

		[OfflineClient(Queued = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginMoveItem(MoveItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		MoveItemJsonResponse EndMoveItem(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginCopyItem(CopyItemJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CopyItemJsonResponse EndCopyItem(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateAttachment(CreateAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateAttachmentJsonResponse EndCreateAttachment(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginDeleteAttachment(DeleteAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DeleteAttachmentJsonResponse EndDeleteAttachment(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginGetAttachment(GetAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetAttachmentJsonResponse EndGetAttachment(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetClientAccessTokenJsonResponse EndGetClientAccessToken(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginResolveNames(ResolveNamesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ResolveNamesJsonResponse EndResolveNames(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginExpandDL(ExpandDLJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ExpandDLJsonResponse EndExpandDL(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetServerTimeZonesJsonResponse EndGetServerTimeZones(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateManagedFolder(CreateManagedFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateManagedFolderJsonResponse EndCreateManagedFolder(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSubscribe(SubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SubscribeJsonResponse EndSubscribe(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginUnsubscribe(UnsubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UnsubscribeJsonResponse EndUnsubscribe(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetEvents(GetEventsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetEventsJsonResponse EndGetEvents(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchyJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SyncFolderHierarchyJsonResponse EndSyncFolderHierarchy(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSyncFolderItems(SyncFolderItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SyncFolderItemsJsonResponse EndSyncFolderItems(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDelegate(GetDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetDelegateJsonResponse EndGetDelegate(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginAddDelegate(AddDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddDelegateJsonResponse EndAddDelegate(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginRemoveDelegate(RemoveDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RemoveDelegateJsonResponse EndRemoveDelegate(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginUpdateDelegate(UpdateDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateDelegateJsonResponse EndUpdateDelegate(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CreateUserConfigurationJsonResponse EndCreateUserConfiguration(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DeleteUserConfigurationJsonResponse EndDeleteUserConfiguration(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginGetUserConfiguration(GetUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetUserConfigurationJsonResponse EndGetUserConfiguration(IAsyncResult result);

		[OfflineClient(Queued = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateUserConfigurationJsonResponse EndUpdateUserConfiguration(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetServiceConfigurationJsonResponse EndGetServiceConfiguration(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginGetMailTips(GetMailTipsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetMailTipsJsonResponse EndGetMailTips(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginPlayOnPhone(PlayOnPhoneJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		PlayOnPhoneJsonResponse EndPlayOnPhone(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetPhoneCallInformationJsonResponse EndGetPhoneCallInformation(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DisconnectPhoneCallJsonResponse EndDisconnectPhoneCall(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetUserAvailability(GetUserAvailabilityJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetUserAvailabilityJsonResponse EndGetUserAvailability(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetUserOofSettingsJsonResponse EndGetUserOofSettings(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SetUserOofSettingsJsonResponse EndSetUserOofSettings(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetSharingMetadata(GetSharingMetadataJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetSharingMetadataJsonResponse EndGetSharingMetadata(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RefreshSharingFolderJsonResponse EndRefreshSharingFolder(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetSharingFolder(GetSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetSharingFolderJsonResponse EndGetSharingFolder(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetReminders(GetRemindersJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetRemindersJsonResponse EndGetReminders(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = true)]
		IAsyncResult BeginPerformReminderAction(PerformReminderActionJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		PerformReminderActionJsonResponse EndPerformReminderAction(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetRoomLists(GetRoomListsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetRoomListsJsonResponse EndGetRoomLists(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetRooms(GetRoomsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetRoomsJsonResponse EndGetRooms(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindMessageTrackingReportJsonResponse EndFindMessageTrackingReport(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetMessageTrackingReportJsonResponse EndGetMessageTrackingReport(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginFindConversation(FindConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindConversationJsonResponse EndFindConversation(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginSyncConversation(SyncConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SyncConversationJsonResponse EndSyncConversation(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = true)]
		IAsyncResult BeginApplyConversationAction(ApplyConversationActionJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ApplyConversationActionJsonResponse EndApplyConversationAction(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetInboxRules(GetInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetInboxRulesJsonResponse EndGetInboxRules(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginFindPeople(FindPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindPeopleJsonResponse EndFindPeople(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSyncAutoCompleteRecipients(SyncAutoCompleteRecipientsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SyncAutoCompleteRecipientsJsonResponse EndSyncAutoCompleteRecipients(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSyncPeople(SyncPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SyncPeopleJsonResponse EndSyncPeople(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPersona(GetPersonaJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetPersonaJsonResponse EndGetPersona(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		UpdateInboxRulesJsonResponse EndUpdateInboxRules(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecuteDiagnosticMethod(ExecuteDiagnosticMethodJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ExecuteDiagnosticMethodJsonResponse EndExecuteDiagnosticMethod(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindMailboxStatisticsByKeywordsJsonResponse EndFindMailboxStatisticsByKeywords(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetSearchableMailboxesJsonResponse EndGetSearchableMailboxes(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSearchMailboxes(SearchMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SearchMailboxesJsonResponse EndSearchMailboxes(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetHoldOnMailboxesJsonResponse EndGetHoldOnMailboxes(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SetHoldOnMailboxesJsonResponse EndSetHoldOnMailboxes(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetPasswordExpirationDateJsonResponse EndGetPasswordExpirationDate(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		MarkAllItemsAsReadJsonResponse EndMarkAllItemsAsRead(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = true)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginMarkAsJunk(MarkAsJunkJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		MarkAsJunkJsonResponse EndMarkAsJunk(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddDistributionGroupToImListJsonResponse EndAddDistributionGroupToImList(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginAddImContactToGroup(AddImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddImContactToGroupJsonResponse EndAddImContactToGroup(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RemoveImContactFromGroupJsonResponse EndRemoveImContactFromGroup(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginAddImGroup(AddImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddImGroupJsonResponse EndAddImGroup(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddNewImContactToGroupJsonResponse EndAddNewImContactToGroup(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		AddNewTelUriContactToGroupJsonResponse EndAddNewTelUriContactToGroup(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetImItemList(GetImItemListJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetImItemListJsonResponse EndGetImItemList(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetImItems(GetImItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetImItemsJsonResponse EndGetImItems(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RemoveContactFromImListJsonResponse EndRemoveContactFromImList(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RemoveDistributionGroupFromImListJsonResponse EndRemoveDistributionGroupFromImList(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginRemoveImGroup(RemoveImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		RemoveImGroupJsonResponse EndRemoveImGroup(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginSetImGroup(SetImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SetImGroupJsonResponse EndSetImGroup(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SetImListMigrationCompletedJsonResponse EndSetImListMigrationCompleted(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetConversationItemsDiagnostics(GetConversationItemsDiagnosticsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetConversationItemsDiagnosticsJsonResponse EndGetConversationItemsDiagnostics(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetConversationItems(GetConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetConversationItemsJsonResponse EndGetConversationItems(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetThreadedConversationItems(GetThreadedConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetThreadedConversationItemsJsonResponse EndGetThreadedConversationItems(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetModernConversationAttachments(GetModernConversationAttachmentsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetModernConversationAttachmentsJsonResponse EndGetModernConversationAttachments(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetUserRetentionPolicyTagsJsonResponse EndGetUserRetentionPolicyTags(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		AddSharedCalendarResponse AddSharedCalendar(AddSharedCalendarRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		ClearMobileDeviceResponse ClearMobileDevice(ClearMobileDeviceRequest request);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		CalendarActionFolderIdResponse SubscribeInternalCalendar(SubscribeInternalCalendarRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		CalendarActionFolderIdResponse SubscribeInternetCalendar(SubscribeInternetCalendarRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		GetCalendarSharingRecipientInfoResponse GetCalendarSharingRecipientInfo(GetCalendarSharingRecipientInfoRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetCalendarSharingPermissionsResponse GetCalendarSharingPermissions(GetCalendarSharingPermissionsRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		CalendarActionResponse SetCalendarSharingPermissions(SetCalendarSharingPermissionsRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		SetCalendarPublishingResponse SetCalendarPublishing(SetCalendarPublishingRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		CalendarShareInviteResponse SendCalendarSharingInvite(CalendarShareInviteRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		ExtensibilityContext GetExtensibilityContext(GetExtensibilityContextParameters request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		bool AddBuddy(Buddy buddy);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		GetBuddyListResponse GetBuddyList();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginFindTrendingConversation(FindTrendingConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindConversationJsonResponse EndFindTrendingConversation(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginFindPlaces(FindPlacesRequest request, AsyncCallback asyncCallback, object asyncState);

		Persona[] EndFindPlaces(IAsyncResult result);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = true)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		DeletePlaceJsonResponse DeletePlace(DeletePlaceRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		CalendarActionResponse AddEventToMyCalendar(AddEventToMyCalendarRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = true)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		bool AddTrustedSender(Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetFavoritesResponse GetFavorites();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetPersonaModernGroupMembershipJsonResponse GetPersonaModernGroupMembership(GetPersonaModernGroupMembershipJsonRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetModernGroupsJsonResponse GetModernGroups();

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		GetModernGroupJsonResponse GetModernGroup(GetModernGroupJsonRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		GetModernGroupJsonResponse GetRecommendedModernGroup(GetModernGroupJsonRequest request);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		SetModernGroupPinStateJsonResponse SetModernGroupPinState(string smtpAddress, bool isPinned);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		SetModernGroupMembershipJsonResponse SetModernGroupMembership(SetModernGroupMembershipJsonRequest request);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		bool SetModernGroupSubscription();

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		GetModernGroupUnseenItemsJsonResponse GetModernGroupUnseenItems(GetModernGroupUnseenItemsJsonRequest request);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		UpdateFavoriteFolderResponse UpdateFavoriteFolder(UpdateFavoriteFolderRequest request);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		UpdateMasterCategoryListResponse UpdateMasterCategoryList(UpdateMasterCategoryListRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		MasterCategoryListActionResponse GetMasterCategoryList(GetMasterCategoryListRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetTaskFoldersResponse GetTaskFolders();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		TaskFolderActionFolderIdResponse CreateTaskFolder(string newTaskFolderName, string parentGroupGuid);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		TaskFolderActionFolderIdResponse RenameTaskFolder(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newTaskFolderName);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		TaskFolderActionResponse DeleteTaskFolder(Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecuteEwsProxy(EwsProxyRequestParameters request, AsyncCallback asyncCallback, object asyncState);

		EwsProxyResponse EndExecuteEwsProxy(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		SaveExtensionSettingsResponse SaveExtensionSettings(SaveExtensionSettingsParameters request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		LoadExtensionCustomPropertiesResponse LoadExtensionCustomProperties(LoadExtensionCustomPropertiesParameters request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		SaveExtensionCustomPropertiesResponse SaveExtensionCustomProperties(SaveExtensionCustomPropertiesParameters request);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		Persona UpdatePersona(UpdatePersonaJsonRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		DeletePersonaJsonResponse DeletePersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, BaseFolderId folderId);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		MaskAutoCompleteRecipientResponse MaskAutoCompleteRecipient(MaskAutoCompleteRecipientRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		Persona CreatePersona(CreatePersonaJsonRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		CreateModernGroupResponse CreateModernGroup(CreateModernGroupRequest request);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		UpdateModernGroupResponse UpdateModernGroup(UpdateModernGroupRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		RemoveModernGroupResponse RemoveModernGroup(RemoveModernGroupRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		ModernGroupMembershipRequestMessageDetailsResponse ModernGroupMembershipRequestMessageDetails(ModernGroupMembershipRequestMessageDetailsRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		ValidateModernGroupAliasResponse ValidateModernGroupAlias(ValidateModernGroupAliasRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetModernGroupDomainResponse GetModernGroupDomain();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		GetPeopleIKnowGraphResponse GetPeopleIKnowGraphCommand(GetPeopleIKnowGraphRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		Microsoft.Exchange.Services.Core.Types.ItemId[] GetPersonaSuggestions(Microsoft.Exchange.Services.Core.Types.ItemId personaId);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		Persona UnlinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId contactId);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		Persona AcceptPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId linkToPersonaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		Persona LinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId linkToPersonaId, Microsoft.Exchange.Services.Core.Types.ItemId personaIdToBeLinked);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		Persona RejectPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		SyncCalendarResponse SyncCalendar(SyncCalendarParameters request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		CalendarActionGroupIdResponse CreateCalendarGroup(string newGroupName);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		CalendarActionGroupIdResponse RenameCalendarGroup(Microsoft.Exchange.Services.Core.Types.ItemId groupId, string newGroupName);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		CalendarActionResponse DeleteCalendarGroup(string groupId);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		CalendarActionFolderIdResponse CreateCalendar(string newCalendarName, string parentGroupGuid, string emailAddress);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		CalendarActionFolderIdResponse RenameCalendar(Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newCalendarName);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		CalendarActionResponse DeleteCalendar(Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		CalendarActionItemIdResponse SetCalendarColor(Microsoft.Exchange.Services.Core.Types.ItemId itemId, CalendarColor calendarColor);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		CalendarActionResponse MoveCalendar(FolderId calendarToMove, string parentGroupId, FolderId calendarBefore);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		CalendarActionResponse SetCalendarGroupOrder(string groupToPosition, string beforeGroup);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		GetCalendarFoldersResponse GetCalendarFolders();

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		GetCalendarFolderConfigurationResponse GetCalendarFolderConfiguration(GetCalendarFolderConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		EnableAppDataResponse EnableApp(EnableAppDataRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		DisableAppDataResponse DisableApp(DisableAppDataRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		RemoveAppDataResponse RemoveApp(RemoveAppDataRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		GetCalendarNotificationResponse GetCalendarNotification();

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		OptionsResponseBase SetCalendarNotification(SetCalendarNotificationRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetCalendarProcessingResponse GetCalendarProcessing();

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetCalendarProcessing(SetCalendarProcessingRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetCASMailboxResponse GetCASMailbox();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		GetCASMailboxResponse GetCASMailbox2(GetCASMailboxRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMobileDeviceStatisticsResponse GetMobileDeviceStatistics(GetMobileDeviceStatisticsRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SetCASMailboxResponse SetCASMailbox(SetCASMailboxRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		GetConnectedAccountsResponse GetConnectedAccounts(GetConnectedAccountsRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		GetConnectSubscriptionResponse GetConnectSubscription(GetConnectSubscriptionRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		NewConnectSubscriptionResponse NewConnectSubscription(NewConnectSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		RemoveConnectSubscriptionResponse RemoveConnectSubscription(RemoveConnectSubscriptionRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		SetConnectSubscriptionResponse SetConnectSubscription(SetConnectSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetHotmailSubscriptionResponse GetHotmailSubscription(IdentityRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		OptionsResponseBase SetHotmailSubscription(SetHotmailSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetImapSubscriptionResponse GetImapSubscription(IdentityRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		ClearTextMessagingAccountResponse ClearTextMessagingAccount(ClearTextMessagingAccountRequest request);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetTextMessagingAccountResponse GetTextMessagingAccount(GetTextMessagingAccountRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SetTextMessagingAccountResponse SetTextMessagingAccount(SetTextMessagingAccountRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		CompareTextMessagingVerificationCodeResponse CompareTextMessagingVerificationCode(CompareTextMessagingVerificationCodeRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SendTextMessagingVerificationCodeResponse SendTextMessagingVerificationCode(SendTextMessagingVerificationCodeRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		NewImapSubscriptionResponse NewImapSubscription(NewImapSubscriptionRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetImapSubscription(SetImapSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		ImportContactListResponse ImportContactList(ImportContactListRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		DisableInboxRuleResponse DisableInboxRule(DisableInboxRuleRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		EnableInboxRuleResponse EnableInboxRule(EnableInboxRuleRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetInboxRuleResponse GetInboxRule(GetInboxRuleRequest request);

		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		NewInboxRuleResponse NewInboxRule(NewInboxRuleRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		RemoveInboxRuleResponse RemoveInboxRule(RemoveInboxRuleRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SetInboxRuleResponse SetInboxRule(SetInboxRuleRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetMailboxResponse GetMailboxByIdentity(IdentityRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetMailbox(SetMailboxRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HttpHeaders)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMailboxAutoReplyConfigurationResponse GetMailboxAutoReplyConfiguration();

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetMailboxAutoReplyConfiguration(SetMailboxAutoReplyConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HttpHeaders)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMailboxCalendarConfigurationResponse GetMailboxCalendarConfiguration();

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetMailboxCalendarConfiguration(SetMailboxCalendarConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMailboxJunkEmailConfigurationResponse GetMailboxJunkEmailConfiguration();

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		OptionsResponseBase SetMailboxJunkEmailConfiguration(SetMailboxJunkEmailConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMailboxRegionalConfigurationResponse GetMailboxRegionalConfiguration(GetMailboxRegionalConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SetMailboxRegionalConfigurationResponse SetMailboxRegionalConfiguration(SetMailboxRegionalConfigurationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetMailboxMessageConfigurationResponse GetMailboxMessageConfiguration();

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		OptionsResponseBase SetMailboxMessageConfiguration(SetMailboxMessageConfigurationRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		GetMessageCategoryResponse GetMessageCategory();

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		GetMessageClassificationResponse GetMessageClassification();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		GetAccountInformationResponse GetAccountInformation(GetAccountInformationRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetSocialNetworksOAuthInfoResponse GetConnectToSocialNetworksOAuthInfo(GetSocialNetworksOAuthInfoRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetPopSubscriptionResponse GetPopSubscription(IdentityRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		NewPopSubscriptionResponse NewPopSubscription(NewPopSubscriptionRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		OptionsResponseBase SetPopSubscription(SetPopSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		OptionsResponseBase AddActiveRetentionPolicyTags(IdentityCollectionRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		GetRetentionPolicyTagsResponse GetActiveRetentionPolicyTags(GetRetentionPolicyTagsRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		GetRetentionPolicyTagsResponse GetAvailableRetentionPolicyTags(GetRetentionPolicyTagsRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		OptionsResponseBase RemoveActiveRetentionPolicyTags(IdentityCollectionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		GetSendAddressResponse GetSendAddress();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HttpHeaders)]
		[OperationContract]
		GetSubscriptionResponse GetSubscription();

		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		NewSubscriptionResponse NewSubscription(NewSubscriptionRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		OptionsResponseBase RemoveSubscription(IdentityRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		SetUserResponse SetUser(SetUserRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		RemoveMobileDeviceResponse RemoveMobileDevice(RemoveMobileDeviceRequest request);

		[OperationContract]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.HeaderBodyFormat)]
		GetUserAvailabilityInternalJsonResponse GetUserAvailabilityInternal(GetUserAvailabilityInternalJsonRequest request);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginProvision(ProvisionJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		ProvisionJsonResponse EndProvision(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetTimeZoneOffsets(GetTimeZoneOffsetsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetTimeZoneOffsetsJsonResponse EndGetTimeZoneOffsets(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginDeprovision(DeprovisionJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		DeprovisionJsonResponse EndDeprovision(IAsyncResult result);

		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		bool SendReadReceipt(Microsoft.Exchange.Services.Core.Types.ItemId itemId);

		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		SuiteStorageJsonResponse ProcessSuiteStorage(SuiteStorageJsonRequest request);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetWeatherForecast(GetWeatherForecastJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetWeatherForecastJsonResponse EndGetWeatherForecast(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFindWeatherLocations(FindWeatherLocationsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		FindWeatherLocationsJsonResponse EndFindWeatherLocations(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[OfflineClient(Queued = false)]
		IAsyncResult BeginLogPushNotificationData(LogPushNotificationDataJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		LogPushNotificationDataJsonResponse EndLogPushNotificationData(IAsyncResult result);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		LikeItemResponse LikeItem(LikeItemRequest request);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		GetLikersResponseMessage GetLikers(GetLikersRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		[OperationContract]
		GetAggregatedAccountResponse GetAggregatedAccount(GetAggregatedAccountRequest request);

		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		AddAggregatedAccountResponse AddAggregatedAccount(AddAggregatedAccountRequest request);

		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginRequestDeviceRegistrationChallenge(RequestDeviceRegistrationChallengeJsonRequest deviceRegistrationChallengeRequest, AsyncCallback asyncCallback, object asyncState);

		RequestDeviceRegistrationChallengeJsonResponse EndRequestDeviceRegistrationChallenge(IAsyncResult result);

		[OfflineClient(Queued = true)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginCancelCalendarEvent(CancelCalendarEventJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		CancelCalendarEventJsonResponse EndCancelCalendarEvent(IAsyncResult result);

		[Deprecated(ExchangeVersionType.V2_4)]
		[OperationContract]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		CalendarActionFolderIdResponse EnableBirthdayCalendar();

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[JsonRequestFormat(Format = JsonRequestFormat.None)]
		[OperationContract]
		CalendarActionResponse DisableBirthdayCalendar();

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract]
		CalendarActionResponse RemoveBirthdayEvent(Microsoft.Exchange.Services.Core.Types.ItemId contactId);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetBirthdayCalendarView(GetBirthdayCalendarViewJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetBirthdayCalendarViewJsonResponse EndGetBirthdayCalendarView(IAsyncResult result);

		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetUserUnifiedGroups(GetUserUnifiedGroupsJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		[JsonRequestFormat(Format = JsonRequestFormat.HttpHeaders)]
		[OfflineClient(Queued = false)]
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		GetAllowedOptionsResponse GetAllowedOptions(GetAllowedOptionsRequest request);

		GetUserUnifiedGroupsJsonResponse EndGetUserUnifiedGroups(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[OperationContract(AsyncPattern = true)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetClutterState(GetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		GetClutterStateJsonResponse EndGetClutterState(IAsyncResult result);

		[OfflineClient(Queued = false)]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSetClutterState(SetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState);

		SetClutterStateJsonResponse EndSetClutterState(IAsyncResult result);
	}
}
