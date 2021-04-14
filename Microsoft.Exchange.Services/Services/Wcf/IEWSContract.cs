using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Services.Wcf
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public interface IEWSContract
	{
		[XmlSerializerFormat]
		[OperationContract(Action = "*", ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginConvertId(ConvertIdSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ConvertIdSoapResponse EndConvertId(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateUnifiedMailbox(CreateUnifiedMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateUnifiedMailboxSoapResponse EndCreateUnifiedMailbox(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginUploadItems(UploadItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UploadItemsSoapResponse EndUploadItems(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginExportItems(ExportItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ExportItemsSoapResponse EndExportItems(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetFolder(GetFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetFolderSoapResponse EndGetFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCreateFolder(CreateFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateFolderSoapResponse EndCreateFolder(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginDeleteFolder(DeleteFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DeleteFolderSoapResponse EndDeleteFolder(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginEmptyFolder(EmptyFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		EmptyFolderSoapResponse EndEmptyFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateFolder(UpdateFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateFolderSoapResponse EndUpdateFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginMoveFolder(MoveFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		MoveFolderSoapResponse EndMoveFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCopyFolder(CopyFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CopyFolderSoapResponse EndCopyFolder(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateFolderPath(CreateFolderPathSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateFolderPathSoapResponse EndCreateFolderPath(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginFindItem(FindItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		FindItemSoapResponse EndFindItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginFindFolder(FindFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		FindFolderSoapResponse EndFindFolder(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetItem(GetItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetItemSoapResponse EndGetItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCreateItem(CreateItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateItemSoapResponse EndCreateItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginDeleteItem(DeleteItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DeleteItemSoapResponse EndDeleteItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateItem(UpdateItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateItemSoapResponse EndUpdateItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateItemInRecoverableItems(UpdateItemInRecoverableItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateItemInRecoverableItemsSoapResponse EndUpdateItemInRecoverableItems(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSendItem(SendItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SendItemSoapResponse EndSendItem(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginMoveItem(MoveItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		MoveItemSoapResponse EndMoveItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCopyItem(CopyItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CopyItemSoapResponse EndCopyItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginArchiveItem(ArchiveItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ArchiveItemSoapResponse EndArchiveItem(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCreateAttachment(CreateAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateAttachmentSoapResponse EndCreateAttachment(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginDeleteAttachment(DeleteAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DeleteAttachmentSoapResponse EndDeleteAttachment(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetAttachment(GetAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetAttachmentSoapResponse EndGetAttachment(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetClientAccessTokenSoapResponse EndGetClientAccessToken(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginResolveNames(ResolveNamesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ResolveNamesSoapResponse EndResolveNames(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginExpandDL(ExpandDLSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ExpandDLSoapResponse EndExpandDL(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetServerTimeZonesSoapResponse EndGetServerTimeZones(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateManagedFolder(CreateManagedFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateManagedFolderSoapResponse EndCreateManagedFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSubscribe(SubscribeSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SubscribeSoapResponse EndSubscribe(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUnsubscribe(UnsubscribeSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UnsubscribeSoapResponse EndUnsubscribe(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetEvents(GetEventsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetEventsSoapResponse EndGetEvents(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetStreamingEvents(GetStreamingEventsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetStreamingEventsSoapResponse EndGetStreamingEvents(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SyncFolderHierarchySoapResponse EndSyncFolderHierarchy(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginSyncFolderItems(SyncFolderItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SyncFolderItemsSoapResponse EndSyncFolderItems(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetDelegate(GetDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetDelegateSoapResponse EndGetDelegate(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginAddDelegate(AddDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddDelegateSoapResponse EndAddDelegate(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRemoveDelegate(RemoveDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RemoveDelegateSoapResponse EndRemoveDelegate(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateDelegate(UpdateDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateDelegateSoapResponse EndUpdateDelegate(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateUserConfigurationSoapResponse EndCreateUserConfiguration(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DeleteUserConfigurationSoapResponse EndDeleteUserConfiguration(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUserConfiguration(GetUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUserConfigurationSoapResponse EndGetUserConfiguration(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateUserConfigurationSoapResponse EndUpdateUserConfiguration(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetServiceConfigurationSoapResponse EndGetServiceConfiguration(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetMailTips(GetMailTipsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetMailTipsSoapResponse EndGetMailTips(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginPlayOnPhone(PlayOnPhoneSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		PlayOnPhoneSoapResponse EndPlayOnPhone(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetPhoneCallInformationSoapResponse EndGetPhoneCallInformation(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DisconnectPhoneCallSoapResponse EndDisconnectPhoneCall(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateUMPrompt(CreateUMPromptSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateUMPromptSoapResponse EndCreateUMPrompt(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginDeleteUMPrompts(DeleteUMPromptsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DeleteUMPromptsSoapResponse EndDeleteUMPrompts(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetUMPrompt(GetUMPromptSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMPromptSoapResponse EndGetUMPrompt(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUMPromptNames(GetUMPromptNamesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMPromptNamesSoapResponse EndGetUMPromptNames(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetUserAvailability(GetUserAvailabilitySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUserAvailabilitySoapResponse EndGetUserAvailability(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUserOofSettingsSoapResponse EndGetUserOofSettings(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetUserOofSettingsSoapResponse EndSetUserOofSettings(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetSharingMetadata(GetSharingMetadataSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetSharingMetadataSoapResponse EndGetSharingMetadata(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RefreshSharingFolderSoapResponse EndRefreshSharingFolder(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetSharingFolder(GetSharingFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetSharingFolderSoapResponse EndGetSharingFolder(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginSetTeamMailbox(SetTeamMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetTeamMailboxSoapResponse EndSetTeamMailbox(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUnpinTeamMailbox(UnpinTeamMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UnpinTeamMailboxSoapResponse EndUnpinTeamMailbox(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetRoomLists(GetRoomListsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetRoomListsSoapResponse EndGetRoomLists(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetRooms(GetRoomsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetRoomsSoapResponse EndGetRooms(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetReminders(GetRemindersSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetRemindersSoapResponse EndGetReminders(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginPerformReminderAction(PerformReminderActionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		PerformReminderActionSoapResponse EndPerformReminderAction(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		FindMessageTrackingReportSoapResponse EndFindMessageTrackingReport(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetMessageTrackingReportSoapResponse EndGetMessageTrackingReport(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginFindConversation(FindConversationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		FindConversationSoapResponse EndFindConversation(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginFindPeople(FindPeopleSoapRequest request, AsyncCallback asyncCallback, object asyncState);

		FindPeopleSoapResponse EndFindPeople(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetPersona(GetPersonaSoapRequest request, AsyncCallback asyncCallback, object asyncState);

		GetPersonaSoapResponse EndGetPersona(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginApplyConversationAction(ApplyConversationActionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ApplyConversationActionSoapResponse EndApplyConversationAction(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetInboxRules(GetInboxRulesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetInboxRulesSoapResponse EndGetInboxRules(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateInboxRulesSoapResponse EndUpdateInboxRules(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		MarkAllItemsAsReadSoapResponse EndMarkAllItemsAsRead(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginMarkAsJunk(MarkAsJunkSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		MarkAsJunkSoapResponse EndMarkAsJunk(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginExecuteDiagnosticMethod(ExecuteDiagnosticMethodSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ExecuteDiagnosticMethodSoapResponse EndExecuteDiagnosticMethod(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		FindMailboxStatisticsByKeywordsSoapResponse EndFindMailboxStatisticsByKeywords(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetSearchableMailboxesSoapResponse EndGetSearchableMailboxes(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSearchMailboxes(SearchMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SearchMailboxesSoapResponse EndSearchMailboxes(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetDiscoverySearchConfiguration(GetDiscoverySearchConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetDiscoverySearchConfigurationSoapResponse EndGetDiscoverySearchConfiguration(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetHoldOnMailboxesSoapResponse EndGetHoldOnMailboxes(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetHoldOnMailboxesSoapResponse EndSetHoldOnMailboxes(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetNonIndexableItemStatistics(GetNonIndexableItemStatisticsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetNonIndexableItemStatisticsSoapResponse EndGetNonIndexableItemStatistics(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetNonIndexableItemDetails(GetNonIndexableItemDetailsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetNonIndexableItemDetailsSoapResponse EndGetNonIndexableItemDetails(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetPasswordExpirationDateSoapResponse EndGetPasswordExpirationDate(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetClientExtension(GetClientExtensionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetClientExtensionSoapResponse EndGetClientExtension(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginSetClientExtension(SetClientExtensionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetClientExtensionSoapResponse EndSetClientExtension(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetEncryptionConfiguration(GetEncryptionConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetEncryptionConfigurationSoapResponse EndGetEncryptionConfiguration(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSetEncryptionConfiguration(SetEncryptionConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetEncryptionConfigurationSoapResponse EndSetEncryptionConfiguration(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetAppManifests(GetAppManifestsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetAppManifestsSoapResponse EndGetAppManifests(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginInstallApp(InstallAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		InstallAppSoapResponse EndInstallApp(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginUninstallApp(UninstallAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UninstallAppSoapResponse EndUninstallApp(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginDisableApp(DisableAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		DisableAppSoapResponse EndDisableApp(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetAppMarketplaceUrl(GetAppMarketplaceUrlSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetAppMarketplaceUrlSoapResponse EndGetAppMarketplaceUrl(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginAddAggregatedAccount(AddAggregatedAccountSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddAggregatedAccountSoapResponse EndAddAggregatedAccount(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddDistributionGroupToImListSoapResponse EndAddDistributionGroupToImList(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginAddImContactToGroup(AddImContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddImContactToGroupSoapResponse EndAddImContactToGroup(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RemoveImContactFromGroupSoapResponse EndRemoveImContactFromGroup(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginAddImGroup(AddImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddImGroupSoapResponse EndAddImGroup(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddNewImContactToGroupSoapResponse EndAddNewImContactToGroup(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		AddNewTelUriContactToGroupSoapResponse EndAddNewTelUriContactToGroup(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetImItemList(GetImItemListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetImItemListSoapResponse EndGetImItemList(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetImItems(GetImItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetImItemsSoapResponse EndGetImItems(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RemoveContactFromImListSoapResponse EndRemoveContactFromImList(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RemoveDistributionGroupFromImListSoapResponse EndRemoveDistributionGroupFromImList(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginRemoveImGroup(RemoveImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		RemoveImGroupSoapResponse EndRemoveImGroup(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSetImGroup(SetImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetImGroupSoapResponse EndSetImGroup(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SetImListMigrationCompletedSoapResponse EndSetImListMigrationCompleted(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetConversationItems(GetConversationItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetConversationItemsSoapResponse EndGetConversationItems(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUserRetentionPolicyTagsSoapResponse EndGetUserRetentionPolicyTags(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginStartFindInGALSpeechRecognition(StartFindInGALSpeechRecognitionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		StartFindInGALSpeechRecognitionSoapResponse EndStartFindInGALSpeechRecognition(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginCompleteFindInGALSpeechRecognition(CompleteFindInGALSpeechRecognitionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CompleteFindInGALSpeechRecognitionSoapResponse EndCompleteFindInGALSpeechRecognition(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginCreateUMCallDataRecord(CreateUMCallDataRecordSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		CreateUMCallDataRecordSoapResponse EndCreateUMCallDataRecord(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUMCallDataRecords(GetUMCallDataRecordsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMCallDataRecordsSoapResponse EndGetUMCallDataRecords(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true)]
		IAsyncResult BeginGetUMCallSummary(GetUMCallSummarySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMCallSummarySoapResponse EndGetUMCallSummary(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetUserPhoto")]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUserPhotoData(GetUserPhotoSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUserPhotoSoapResponse EndGetUserPhotoData(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "InitUMMailbox")]
		IAsyncResult BeginInitUMMailbox(InitUMMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		InitUMMailboxSoapResponse EndInitUMMailbox(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "ResetUMMailbox")]
		[XmlSerializerFormat]
		IAsyncResult BeginResetUMMailbox(ResetUMMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ResetUMMailboxSoapResponse EndResetUMMailbox(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "ValidateUMPin")]
		IAsyncResult BeginValidateUMPin(ValidateUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		ValidateUMPinSoapResponse EndValidateUMPin(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "SaveUMPin")]
		IAsyncResult BeginSaveUMPin(SaveUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		SaveUMPinSoapResponse EndSaveUMPin(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetUMPin")]
		[XmlSerializerFormat]
		IAsyncResult BeginGetUMPin(GetUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMPinSoapResponse EndGetUMPin(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetClientIntent")]
		IAsyncResult BeginGetClientIntent(GetClientIntentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetClientIntentSoapResponse EndGetClientIntent(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetUMSubscriberCallAnsweringData")]
		IAsyncResult BeginGetUMSubscriberCallAnsweringData(GetUMSubscriberCallAnsweringDataSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		GetUMSubscriberCallAnsweringDataSoapResponse EndGetUMSubscriberCallAnsweringData(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "UpdateMailboxAssociation")]
		[XmlSerializerFormat]
		IAsyncResult BeginUpdateMailboxAssociation(UpdateMailboxAssociationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateMailboxAssociationSoapResponse EndUpdateMailboxAssociation(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "UpdateGroupMailbox")]
		IAsyncResult BeginUpdateGroupMailbox(UpdateGroupMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		UpdateGroupMailboxSoapResponse EndUpdateGroupMailbox(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "PostModernGroupItem")]
		IAsyncResult BeginPostModernGroupItem(PostModernGroupItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		PostModernGroupItemSoapResponse EndPostModernGroupItem(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "PerformInstantSearch")]
		[XmlSerializerFormat]
		IAsyncResult BeginPerformInstantSearch(PerformInstantSearchSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		PerformInstantSearchSoapResponse EndPerformInstantSearch(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "EndInstantSearchSession")]
		IAsyncResult BeginEndInstantSearchSession(EndInstantSearchSessionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState);

		EndInstantSearchSessionSoapResponse EndEndInstantSearchSession(IAsyncResult result);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetUserUnifiedGroups")]
		IAsyncResult BeginGetUserUnifiedGroups(GetUserUnifiedGroupsSoapRequest request, AsyncCallback asyncCallback, object asyncState);

		GetUserUnifiedGroupsSoapResponse EndGetUserUnifiedGroups(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "GetClutterState")]
		[XmlSerializerFormat]
		IAsyncResult BeginGetClutterState(GetClutterStateSoapRequest request, AsyncCallback asyncCallback, object asyncState);

		GetClutterStateSoapResponse EndGetClutterState(IAsyncResult result);

		[OperationContract(ReplyAction = "*", AsyncPattern = true, Name = "SetClutterState")]
		[XmlSerializerFormat]
		IAsyncResult BeginSetClutterState(SetClutterStateSoapRequest request, AsyncCallback asyncCallback, object asyncState);

		SetClutterStateSoapResponse EndSetClutterState(IAsyncResult result);
	}
}
