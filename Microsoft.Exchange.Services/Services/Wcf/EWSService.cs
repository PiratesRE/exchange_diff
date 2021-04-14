using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageInspectorBehavior]
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class EWSService : IEWSContract, IEWSStreamingContract
	{
		public IAsyncResult BeginConvertId(ConvertIdSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ConvertIdResponse>(asyncCallback, asyncState);
		}

		public ConvertIdSoapResponse EndConvertId(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ConvertIdSoapResponse, ConvertIdResponse>(result, (ConvertIdResponse body) => new ConvertIdSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUploadItems(UploadItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UploadItemsResponse>(asyncCallback, asyncState);
		}

		public UploadItemsSoapResponse EndUploadItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UploadItemsSoapResponse, UploadItemsResponse>(result, (UploadItemsResponse body) => new UploadItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginExportItems(ExportItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ExportItemsResponse>(asyncCallback, asyncState);
		}

		public ExportItemsSoapResponse EndExportItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ExportItemsSoapResponse, ExportItemsResponse>(result, (ExportItemsResponse body) => new ExportItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetFolder(GetFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetFolderResponse>(asyncCallback, asyncState);
		}

		public GetFolderSoapResponse EndGetFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetFolderSoapResponse, GetFolderResponse>(result, (GetFolderResponse body) => new GetFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateFolder(CreateFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateFolderResponse>(asyncCallback, asyncState);
		}

		public CreateFolderSoapResponse EndCreateFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateFolderSoapResponse, CreateFolderResponse>(result, (CreateFolderResponse body) => new CreateFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateUnifiedMailbox(CreateUnifiedMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateUnifiedMailboxResponse>(asyncCallback, asyncState);
		}

		public CreateUnifiedMailboxSoapResponse EndCreateUnifiedMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateUnifiedMailboxSoapResponse, CreateUnifiedMailboxResponse>(result, (CreateUnifiedMailboxResponse body) => new CreateUnifiedMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteFolder(DeleteFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DeleteFolderResponse>(asyncCallback, asyncState);
		}

		public DeleteFolderSoapResponse EndDeleteFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DeleteFolderSoapResponse, DeleteFolderResponse>(result, (DeleteFolderResponse body) => new DeleteFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginEmptyFolder(EmptyFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<EmptyFolderResponse>(asyncCallback, asyncState);
		}

		public EmptyFolderSoapResponse EndEmptyFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<EmptyFolderSoapResponse, EmptyFolderResponse>(result, (EmptyFolderResponse body) => new EmptyFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateFolder(UpdateFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateFolderResponse>(asyncCallback, asyncState);
		}

		public UpdateFolderSoapResponse EndUpdateFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateFolderSoapResponse, UpdateFolderResponse>(result, (UpdateFolderResponse body) => new UpdateFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMoveFolder(MoveFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<MoveFolderResponse>(asyncCallback, asyncState);
		}

		public MoveFolderSoapResponse EndMoveFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<MoveFolderSoapResponse, MoveFolderResponse>(result, (MoveFolderResponse body) => new MoveFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCopyFolder(CopyFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CopyFolderResponse>(asyncCallback, asyncState);
		}

		public CopyFolderSoapResponse EndCopyFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CopyFolderSoapResponse, CopyFolderResponse>(result, (CopyFolderResponse body) => new CopyFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindItem(FindItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindItemResponse>(asyncCallback, asyncState);
		}

		public FindItemSoapResponse EndFindItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindItemSoapResponse, FindItemResponse>(result, (FindItemResponse body) => new FindItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindFolder(FindFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindFolderResponse>(asyncCallback, asyncState);
		}

		public FindFolderSoapResponse EndFindFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindFolderSoapResponse, FindFolderResponse>(result, (FindFolderResponse body) => new FindFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetItem(GetItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetItemResponse>(asyncCallback, asyncState);
		}

		public GetItemSoapResponse EndGetItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetItemSoapResponse, GetItemResponse>(result, (GetItemResponse body) => new GetItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateFolderPath(CreateFolderPathSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateFolderPathResponse>(asyncCallback, asyncState);
		}

		public CreateFolderPathSoapResponse EndCreateFolderPath(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateFolderPathSoapResponse, CreateFolderPathResponse>(result, (CreateFolderPathResponse body) => new CreateFolderPathSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateItem(CreateItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateItemResponse>(asyncCallback, asyncState);
		}

		public CreateItemSoapResponse EndCreateItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateItemSoapResponse, CreateItemResponse>(result, (CreateItemResponse body) => new CreateItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteItem(DeleteItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DeleteItemResponse>(asyncCallback, asyncState);
		}

		public DeleteItemSoapResponse EndDeleteItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DeleteItemSoapResponse, DeleteItemResponse>(result, (DeleteItemResponse body) => new DeleteItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateItem(UpdateItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateItemResponse>(asyncCallback, asyncState);
		}

		public UpdateItemSoapResponse EndUpdateItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateItemSoapResponse, UpdateItemResponse>(result, (UpdateItemResponse body) => new UpdateItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateItemInRecoverableItems(UpdateItemInRecoverableItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateItemInRecoverableItemsResponse>(asyncCallback, asyncState);
		}

		public UpdateItemInRecoverableItemsSoapResponse EndUpdateItemInRecoverableItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateItemInRecoverableItemsSoapResponse, UpdateItemInRecoverableItemsResponse>(result, (UpdateItemInRecoverableItemsResponse body) => new UpdateItemInRecoverableItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSendItem(SendItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SendItemResponse>(asyncCallback, asyncState);
		}

		public SendItemSoapResponse EndSendItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SendItemSoapResponse, SendItemResponse>(result, (SendItemResponse body) => new SendItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMoveItem(MoveItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<MoveItemResponse>(asyncCallback, asyncState);
		}

		public MoveItemSoapResponse EndMoveItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<MoveItemSoapResponse, MoveItemResponse>(result, (MoveItemResponse body) => new MoveItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCopyItem(CopyItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CopyItemResponse>(asyncCallback, asyncState);
		}

		public CopyItemSoapResponse EndCopyItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CopyItemSoapResponse, CopyItemResponse>(result, (CopyItemResponse body) => new CopyItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginArchiveItem(ArchiveItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ArchiveItemResponse>(asyncCallback, asyncState);
		}

		public ArchiveItemSoapResponse EndArchiveItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ArchiveItemSoapResponse, ArchiveItemResponse>(result, (ArchiveItemResponse body) => new ArchiveItemSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateAttachment(CreateAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateAttachmentResponse>(asyncCallback, asyncState);
		}

		public CreateAttachmentSoapResponse EndCreateAttachment(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateAttachmentSoapResponse, CreateAttachmentResponse>(result, (CreateAttachmentResponse body) => new CreateAttachmentSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteAttachment(DeleteAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DeleteAttachmentResponse>(asyncCallback, asyncState);
		}

		public DeleteAttachmentSoapResponse EndDeleteAttachment(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DeleteAttachmentSoapResponse, DeleteAttachmentResponse>(result, (DeleteAttachmentResponse body) => new DeleteAttachmentSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetAttachment(GetAttachmentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetAttachmentResponse>(asyncCallback, asyncState);
		}

		public GetAttachmentSoapResponse EndGetAttachment(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetAttachmentSoapResponse, GetAttachmentResponse>(result, (GetAttachmentResponse body) => new GetAttachmentSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetClientAccessTokenResponse>(asyncCallback, asyncState);
		}

		public GetClientAccessTokenSoapResponse EndGetClientAccessToken(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetClientAccessTokenSoapResponse, GetClientAccessTokenResponse>(result, (GetClientAccessTokenResponse body) => new GetClientAccessTokenSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginResolveNames(ResolveNamesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ResolveNamesResponse>(asyncCallback, asyncState);
		}

		public ResolveNamesSoapResponse EndResolveNames(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ResolveNamesSoapResponse, ResolveNamesResponse>(result, (ResolveNamesResponse body) => new ResolveNamesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginExpandDL(ExpandDLSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ExpandDLResponse>(asyncCallback, asyncState);
		}

		public ExpandDLSoapResponse EndExpandDL(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ExpandDLSoapResponse, ExpandDLResponse>(result, (ExpandDLResponse body) => new ExpandDLSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetServerTimeZonesResponse>(asyncCallback, asyncState);
		}

		public GetServerTimeZonesSoapResponse EndGetServerTimeZones(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetServerTimeZonesSoapResponse, GetServerTimeZonesResponse>(result, (GetServerTimeZonesResponse body) => new GetServerTimeZonesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateManagedFolder(CreateManagedFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateManagedFolderResponse>(asyncCallback, asyncState);
		}

		public CreateManagedFolderSoapResponse EndCreateManagedFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateManagedFolderSoapResponse, CreateManagedFolderResponse>(result, (CreateManagedFolderResponse body) => new CreateManagedFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSubscribe(SubscribeSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SubscribeResponse>(asyncCallback, asyncState);
		}

		public SubscribeSoapResponse EndSubscribe(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SubscribeSoapResponse, SubscribeResponse>(result, (SubscribeResponse body) => new SubscribeSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUnsubscribe(UnsubscribeSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UnsubscribeResponse>(asyncCallback, asyncState);
		}

		public UnsubscribeSoapResponse EndUnsubscribe(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UnsubscribeSoapResponse, UnsubscribeResponse>(result, (UnsubscribeResponse body) => new UnsubscribeSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetEvents(GetEventsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetEventsResponse>(asyncCallback, asyncState);
		}

		public GetEventsSoapResponse EndGetEvents(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetEventsSoapResponse, GetEventsResponse>(result, (GetEventsResponse body) => new GetEventsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClientExtension(GetClientExtensionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetClientExtensionResponse>(asyncCallback, asyncState);
		}

		public GetClientExtensionSoapResponse EndGetClientExtension(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetClientExtensionSoapResponse, GetClientExtensionResponse>(result, (GetClientExtensionResponse body) => new GetClientExtensionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetClientExtension(SetClientExtensionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetClientExtensionResponse>(asyncCallback, asyncState);
		}

		public SetClientExtensionSoapResponse EndSetClientExtension(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetClientExtensionSoapResponse, SetClientExtensionResponse>(result, (SetClientExtensionResponse body) => new SetClientExtensionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetEncryptionConfiguration(GetEncryptionConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetEncryptionConfigurationResponse>(asyncCallback, asyncState);
		}

		public GetEncryptionConfigurationSoapResponse EndGetEncryptionConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetEncryptionConfigurationSoapResponse, GetEncryptionConfigurationResponse>(result, (GetEncryptionConfigurationResponse body) => new GetEncryptionConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetEncryptionConfiguration(SetEncryptionConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetEncryptionConfigurationResponse>(asyncCallback, asyncState);
		}

		public SetEncryptionConfigurationSoapResponse EndSetEncryptionConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetEncryptionConfigurationSoapResponse, SetEncryptionConfigurationResponse>(result, (SetEncryptionConfigurationResponse body) => new SetEncryptionConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetAppManifests(GetAppManifestsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetAppManifestsResponse>(asyncCallback, asyncState);
		}

		public GetAppManifestsSoapResponse EndGetAppManifests(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetAppManifestsSoapResponse, GetAppManifestsResponse>(result, (GetAppManifestsResponse body) => new GetAppManifestsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginInstallApp(InstallAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<InstallAppResponse>(asyncCallback, asyncState);
		}

		public InstallAppSoapResponse EndInstallApp(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<InstallAppSoapResponse, InstallAppResponse>(result, (InstallAppResponse body) => new InstallAppSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUninstallApp(UninstallAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UninstallAppResponse>(asyncCallback, asyncState);
		}

		public UninstallAppSoapResponse EndUninstallApp(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UninstallAppSoapResponse, UninstallAppResponse>(result, (UninstallAppResponse body) => new UninstallAppSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDisableApp(DisableAppSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DisableAppResponse>(asyncCallback, asyncState);
		}

		public DisableAppSoapResponse EndDisableApp(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DisableAppSoapResponse, DisableAppResponse>(result, (DisableAppResponse body) => new DisableAppSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetAppMarketplaceUrl(GetAppMarketplaceUrlSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetAppMarketplaceUrlResponse>(asyncCallback, asyncState);
		}

		public GetAppMarketplaceUrlSoapResponse EndGetAppMarketplaceUrl(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetAppMarketplaceUrlSoapResponse, GetAppMarketplaceUrlResponse>(result, (GetAppMarketplaceUrlResponse body) => new GetAppMarketplaceUrlSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddAggregatedAccount(AddAggregatedAccountSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddAggregatedAccountResponse>(asyncCallback, asyncState);
		}

		public AddAggregatedAccountSoapResponse EndAddAggregatedAccount(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddAggregatedAccountSoapResponse, AddAggregatedAccountResponse>(result, (AddAggregatedAccountResponse body) => new AddAggregatedAccountSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetStreamingEvents(GetStreamingEventsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetStreamingEventsResponse>(asyncCallback, asyncState);
		}

		public GetStreamingEventsSoapResponse EndGetStreamingEvents(IAsyncResult result)
		{
			ServiceAsyncResult<GetStreamingEventsResponse> serviceAsyncResult = EWSService.GetServiceAsyncResult<GetStreamingEventsResponse>(result);
			GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
			getStreamingEventsSoapResponse.Body = serviceAsyncResult.Data;
			PerformanceMonitor.UpdateTotalCompletedRequestsCount();
			return getStreamingEventsSoapResponse;
		}

		public IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SyncFolderHierarchyResponse>(asyncCallback, asyncState);
		}

		public SyncFolderHierarchySoapResponse EndSyncFolderHierarchy(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SyncFolderHierarchySoapResponse, SyncFolderHierarchyResponse>(result, (SyncFolderHierarchyResponse body) => new SyncFolderHierarchySoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncFolderItems(SyncFolderItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SyncFolderItemsResponse>(asyncCallback, asyncState);
		}

		public SyncFolderItemsSoapResponse EndSyncFolderItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SyncFolderItemsSoapResponse, SyncFolderItemsResponse>(result, (SyncFolderItemsResponse body) => new SyncFolderItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetDelegate(GetDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetDelegateResponseMessage>(asyncCallback, asyncState);
		}

		public GetDelegateSoapResponse EndGetDelegate(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetDelegateSoapResponse, GetDelegateResponseMessage>(result, (GetDelegateResponseMessage body) => new GetDelegateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddDelegate(AddDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddDelegateResponseMessage>(asyncCallback, asyncState);
		}

		public AddDelegateSoapResponse EndAddDelegate(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddDelegateSoapResponse, AddDelegateResponseMessage>(result, (AddDelegateResponseMessage body) => new AddDelegateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveDelegate(RemoveDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RemoveDelegateResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveDelegateSoapResponse EndRemoveDelegate(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RemoveDelegateSoapResponse, RemoveDelegateResponseMessage>(result, (RemoveDelegateResponseMessage body) => new RemoveDelegateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateDelegate(UpdateDelegateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateDelegateResponseMessage>(asyncCallback, asyncState);
		}

		public UpdateDelegateSoapResponse EndUpdateDelegate(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateDelegateSoapResponse, UpdateDelegateResponseMessage>(result, (UpdateDelegateResponseMessage body) => new UpdateDelegateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public CreateUserConfigurationSoapResponse EndCreateUserConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateUserConfigurationSoapResponse, CreateUserConfigurationResponse>(result, (CreateUserConfigurationResponse body) => new CreateUserConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DeleteUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public DeleteUserConfigurationSoapResponse EndDeleteUserConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DeleteUserConfigurationSoapResponse, DeleteUserConfigurationResponse>(result, (DeleteUserConfigurationResponse body) => new DeleteUserConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserConfiguration(GetUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public GetUserConfigurationSoapResponse EndGetUserConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserConfigurationSoapResponse, GetUserConfigurationResponse>(result, (GetUserConfigurationResponse body) => new GetUserConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public UpdateUserConfigurationSoapResponse EndUpdateUserConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateUserConfigurationSoapResponse, UpdateUserConfigurationResponse>(result, (UpdateUserConfigurationResponse body) => new UpdateUserConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			ExTraceGlobals.GetOrganizationConfigurationCallTracer.TraceDebug((long)this.GetHashCode(), "WcfService.GetServiceConfiguration called");
			return soapRequest.Body.Submit<GetServiceConfigurationResponseMessage>(asyncCallback, asyncState);
		}

		public GetServiceConfigurationSoapResponse EndGetServiceConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetServiceConfigurationSoapResponse, GetServiceConfigurationResponseMessage>(result, (GetServiceConfigurationResponseMessage body) => new GetServiceConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "WcfService.GetMailTips called");
			return soapRequest.Body.Submit<GetMailTipsResponseMessage>(asyncCallback, asyncState);
		}

		public GetMailTipsSoapResponse EndGetMailTips(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetMailTipsSoapResponse, GetMailTipsResponseMessage>(result, (GetMailTipsResponseMessage body) => new GetMailTipsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPlayOnPhone(PlayOnPhoneSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<PlayOnPhoneResponseMessage>(asyncCallback, asyncState);
		}

		public PlayOnPhoneSoapResponse EndPlayOnPhone(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<PlayOnPhoneSoapResponse, PlayOnPhoneResponseMessage>(result, (PlayOnPhoneResponseMessage body) => new PlayOnPhoneSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetPhoneCallInformationResponseMessage>(asyncCallback, asyncState);
		}

		public GetPhoneCallInformationSoapResponse EndGetPhoneCallInformation(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetPhoneCallInformationSoapResponse, GetPhoneCallInformationResponseMessage>(result, (GetPhoneCallInformationResponseMessage body) => new GetPhoneCallInformationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DisconnectPhoneCallResponseMessage>(asyncCallback, asyncState);
		}

		public DisconnectPhoneCallSoapResponse EndDisconnectPhoneCall(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DisconnectPhoneCallSoapResponse, DisconnectPhoneCallResponseMessage>(result, (DisconnectPhoneCallResponseMessage body) => new DisconnectPhoneCallSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMPrompt(GetUMPromptSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMPromptResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMPromptSoapResponse EndGetUMPrompt(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMPromptSoapResponse, GetUMPromptResponseMessage>(result, (GetUMPromptResponseMessage body) => new GetUMPromptSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMPromptNames(GetUMPromptNamesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMPromptNamesResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMPromptNamesSoapResponse EndGetUMPromptNames(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMPromptNamesSoapResponse, GetUMPromptNamesResponseMessage>(result, (GetUMPromptNamesResponseMessage body) => new GetUMPromptNamesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteUMPrompts(DeleteUMPromptsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<DeleteUMPromptsResponseMessage>(asyncCallback, asyncState);
		}

		public DeleteUMPromptsSoapResponse EndDeleteUMPrompts(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<DeleteUMPromptsSoapResponse, DeleteUMPromptsResponseMessage>(result, (DeleteUMPromptsResponseMessage body) => new DeleteUMPromptsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateUMPrompt(CreateUMPromptSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateUMPromptResponseMessage>(asyncCallback, asyncState);
		}

		public CreateUMPromptSoapResponse EndCreateUMPrompt(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateUMPromptSoapResponse, CreateUMPromptResponseMessage>(result, (CreateUMPromptResponseMessage body) => new CreateUMPromptSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilitySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			object obj;
			if (EwsOperationContextBase.Current.IncomingMessageProperties.TryGetValue("DefaultFreeBusyAccessOnly", out obj) && obj is bool)
			{
				soapRequest.Body.DefaultFreeBusyAccessOnly = (bool)obj;
			}
			return soapRequest.Body.Submit<GetUserAvailabilityResponse>(asyncCallback, asyncState);
		}

		public GetUserAvailabilitySoapResponse EndGetUserAvailability(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserAvailabilitySoapResponse, GetUserAvailabilityResponse>(result, (GetUserAvailabilityResponse body) => new GetUserAvailabilitySoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUserOofSettingsResponse>(asyncCallback, asyncState);
		}

		public GetUserOofSettingsSoapResponse EndGetUserOofSettings(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserOofSettingsSoapResponse, GetUserOofSettingsResponse>(result, (GetUserOofSettingsResponse body) => new GetUserOofSettingsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetUserOofSettingsResponse>(asyncCallback, asyncState);
		}

		public SetUserOofSettingsSoapResponse EndSetUserOofSettings(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetUserOofSettingsSoapResponse, SetUserOofSettingsResponse>(result, (SetUserOofSettingsResponse body) => new SetUserOofSettingsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetSharingMetadata(GetSharingMetadataSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetSharingMetadataResponseMessage>(asyncCallback, asyncState);
		}

		public GetSharingMetadataSoapResponse EndGetSharingMetadata(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetSharingMetadataSoapResponse, GetSharingMetadataResponseMessage>(result, (GetSharingMetadataResponseMessage body) => new GetSharingMetadataSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RefreshSharingFolderResponseMessage>(asyncCallback, asyncState);
		}

		public RefreshSharingFolderSoapResponse EndRefreshSharingFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RefreshSharingFolderSoapResponse, RefreshSharingFolderResponseMessage>(result, (RefreshSharingFolderResponseMessage body) => new RefreshSharingFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetSharingFolder(GetSharingFolderSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetSharingFolderResponseMessage>(asyncCallback, asyncState);
		}

		public GetSharingFolderSoapResponse EndGetSharingFolder(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetSharingFolderSoapResponse, GetSharingFolderResponseMessage>(result, (GetSharingFolderResponseMessage body) => new GetSharingFolderSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetTeamMailbox(SetTeamMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetTeamMailboxResponseMessage>(asyncCallback, asyncState);
		}

		public SetTeamMailboxSoapResponse EndSetTeamMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetTeamMailboxSoapResponse, SetTeamMailboxResponseMessage>(result, (SetTeamMailboxResponseMessage body) => new SetTeamMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUnpinTeamMailbox(UnpinTeamMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UnpinTeamMailboxResponseMessage>(asyncCallback, asyncState);
		}

		public UnpinTeamMailboxSoapResponse EndUnpinTeamMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UnpinTeamMailboxSoapResponse, UnpinTeamMailboxResponseMessage>(result, (UnpinTeamMailboxResponseMessage body) => new UnpinTeamMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetRoomLists(GetRoomListsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetRoomListsResponse>(asyncCallback, asyncState);
		}

		public GetRoomListsSoapResponse EndGetRoomLists(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetRoomListsSoapResponse, GetRoomListsResponse>(result, (GetRoomListsResponse body) => new GetRoomListsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetRooms(GetRoomsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetRoomsResponse>(asyncCallback, asyncState);
		}

		public GetRoomsSoapResponse EndGetRooms(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetRoomsSoapResponse, GetRoomsResponse>(result, (GetRoomsResponse body) => new GetRoomsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetReminders(GetRemindersSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetRemindersResponse>(asyncCallback, asyncState);
		}

		public GetRemindersSoapResponse EndGetReminders(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetRemindersSoapResponse, GetRemindersResponse>(result, (GetRemindersResponse body) => new GetRemindersSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPerformReminderAction(PerformReminderActionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<PerformReminderActionResponse>(asyncCallback, asyncState);
		}

		public PerformReminderActionSoapResponse EndPerformReminderAction(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<PerformReminderActionSoapResponse, PerformReminderActionResponse>(result, (PerformReminderActionResponse body) => new PerformReminderActionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindMessageTrackingReportResponseMessage>(asyncCallback, asyncState);
		}

		public FindMessageTrackingReportSoapResponse EndFindMessageTrackingReport(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindMessageTrackingReportSoapResponse, FindMessageTrackingReportResponseMessage>(result, (FindMessageTrackingReportResponseMessage body) => new FindMessageTrackingReportSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetMessageTrackingReportResponseMessage>(asyncCallback, asyncState);
		}

		public GetMessageTrackingReportSoapResponse EndGetMessageTrackingReport(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetMessageTrackingReportSoapResponse, GetMessageTrackingReportResponseMessage>(result, (GetMessageTrackingReportResponseMessage body) => new GetMessageTrackingReportSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindConversation(FindConversationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindConversationResponseMessage>(asyncCallback, asyncState);
		}

		public FindConversationSoapResponse EndFindConversation(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindConversationSoapResponse, FindConversationResponseMessage>(result, (FindConversationResponseMessage body) => new FindConversationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindPeople(FindPeopleSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindPeopleResponseMessage>(asyncCallback, asyncState);
		}

		public FindPeopleSoapResponse EndFindPeople(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindPeopleSoapResponse, FindPeopleResponseMessage>(result, (FindPeopleResponseMessage body) => new FindPeopleSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPersona(GetPersonaSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetPersonaResponseMessage>(asyncCallback, asyncState);
		}

		public GetPersonaSoapResponse EndGetPersona(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetPersonaSoapResponse, GetPersonaResponseMessage>(result, (GetPersonaResponseMessage body) => new GetPersonaSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginApplyConversationAction(ApplyConversationActionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ApplyConversationActionResponse>(asyncCallback, asyncState);
		}

		public ApplyConversationActionSoapResponse EndApplyConversationAction(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ApplyConversationActionSoapResponse, ApplyConversationActionResponse>(result, (ApplyConversationActionResponse body) => new ApplyConversationActionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetInboxRules(GetInboxRulesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetInboxRulesResponse>(asyncCallback, asyncState);
		}

		public GetInboxRulesSoapResponse EndGetInboxRules(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetInboxRulesSoapResponse, GetInboxRulesResponse>(result, (GetInboxRulesResponse body) => new GetInboxRulesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateInboxRulesResponse>(asyncCallback, asyncState);
		}

		public UpdateInboxRulesSoapResponse EndUpdateInboxRules(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateInboxRulesSoapResponse, UpdateInboxRulesResponse>(result, (UpdateInboxRulesResponse body) => new UpdateInboxRulesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<MarkAllItemsAsReadResponse>(asyncCallback, asyncState);
		}

		public MarkAllItemsAsReadSoapResponse EndMarkAllItemsAsRead(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<MarkAllItemsAsReadSoapResponse, MarkAllItemsAsReadResponse>(result, (MarkAllItemsAsReadResponse body) => new MarkAllItemsAsReadSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMarkAsJunk(MarkAsJunkSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<MarkAsJunkResponse>(asyncCallback, asyncState);
		}

		public MarkAsJunkSoapResponse EndMarkAsJunk(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<MarkAsJunkSoapResponse, MarkAsJunkResponse>(result, (MarkAsJunkResponse body) => new MarkAsJunkSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetConversationItems(GetConversationItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetConversationItemsResponse>(asyncCallback, asyncState);
		}

		public GetConversationItemsSoapResponse EndGetConversationItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetConversationItemsSoapResponse, GetConversationItemsResponse>(result, (GetConversationItemsResponse body) => new GetConversationItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginExecuteDiagnosticMethod(ExecuteDiagnosticMethodSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ExecuteDiagnosticMethodResponse>(asyncCallback, asyncState);
		}

		public ExecuteDiagnosticMethodSoapResponse EndExecuteDiagnosticMethod(IAsyncResult result)
		{
			ServiceAsyncResult<ExecuteDiagnosticMethodResponse> serviceAsyncResult = EWSService.GetServiceAsyncResult<ExecuteDiagnosticMethodResponse>(result);
			ExecuteDiagnosticMethodSoapResponse executeDiagnosticMethodSoapResponse = new ExecuteDiagnosticMethodSoapResponse();
			executeDiagnosticMethodSoapResponse.Body = serviceAsyncResult.Data;
			PerformanceMonitor.UpdateTotalCompletedRequestsCount();
			return executeDiagnosticMethodSoapResponse;
		}

		public IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<FindMailboxStatisticsByKeywordsResponse>(asyncCallback, asyncState);
		}

		public FindMailboxStatisticsByKeywordsSoapResponse EndFindMailboxStatisticsByKeywords(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<FindMailboxStatisticsByKeywordsSoapResponse, FindMailboxStatisticsByKeywordsResponse>(result, (FindMailboxStatisticsByKeywordsResponse body) => new FindMailboxStatisticsByKeywordsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetSearchableMailboxesResponse>(asyncCallback, asyncState);
		}

		public GetSearchableMailboxesSoapResponse EndGetSearchableMailboxes(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetSearchableMailboxesSoapResponse, GetSearchableMailboxesResponse>(result, (GetSearchableMailboxesResponse body) => new GetSearchableMailboxesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSearchMailboxes(SearchMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SearchMailboxesResponse>(asyncCallback, asyncState);
		}

		public SearchMailboxesSoapResponse EndSearchMailboxes(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SearchMailboxesSoapResponse, SearchMailboxesResponse>(result, (SearchMailboxesResponse body) => new SearchMailboxesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetDiscoverySearchConfiguration(GetDiscoverySearchConfigurationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetDiscoverySearchConfigurationResponse>(asyncCallback, asyncState);
		}

		public GetDiscoverySearchConfigurationSoapResponse EndGetDiscoverySearchConfiguration(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetDiscoverySearchConfigurationSoapResponse, GetDiscoverySearchConfigurationResponse>(result, (GetDiscoverySearchConfigurationResponse body) => new GetDiscoverySearchConfigurationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetHoldOnMailboxesResponse>(asyncCallback, asyncState);
		}

		public GetHoldOnMailboxesSoapResponse EndGetHoldOnMailboxes(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetHoldOnMailboxesSoapResponse, GetHoldOnMailboxesResponse>(result, (GetHoldOnMailboxesResponse body) => new GetHoldOnMailboxesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetHoldOnMailboxesResponse>(asyncCallback, asyncState);
		}

		public SetHoldOnMailboxesSoapResponse EndSetHoldOnMailboxes(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetHoldOnMailboxesSoapResponse, SetHoldOnMailboxesResponse>(result, (SetHoldOnMailboxesResponse body) => new SetHoldOnMailboxesSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetNonIndexableItemStatistics(GetNonIndexableItemStatisticsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetNonIndexableItemStatisticsResponse>(asyncCallback, asyncState);
		}

		public GetNonIndexableItemStatisticsSoapResponse EndGetNonIndexableItemStatistics(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetNonIndexableItemStatisticsSoapResponse, GetNonIndexableItemStatisticsResponse>(result, (GetNonIndexableItemStatisticsResponse body) => new GetNonIndexableItemStatisticsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetNonIndexableItemDetails(GetNonIndexableItemDetailsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetNonIndexableItemDetailsResponse>(asyncCallback, asyncState);
		}

		public GetNonIndexableItemDetailsSoapResponse EndGetNonIndexableItemDetails(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetNonIndexableItemDetailsSoapResponse, GetNonIndexableItemDetailsResponse>(result, (GetNonIndexableItemDetailsResponse body) => new GetNonIndexableItemDetailsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetPasswordExpirationDateResponse>(asyncCallback, asyncState);
		}

		public GetPasswordExpirationDateSoapResponse EndGetPasswordExpirationDate(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetPasswordExpirationDateSoapResponse, GetPasswordExpirationDateResponse>(result, (GetPasswordExpirationDateResponse body) => new GetPasswordExpirationDateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddDistributionGroupToImListResponseMessage>(asyncCallback, asyncState);
		}

		public AddDistributionGroupToImListSoapResponse EndAddDistributionGroupToImList(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddDistributionGroupToImListSoapResponse, AddDistributionGroupToImListResponseMessage>(result, (AddDistributionGroupToImListResponseMessage body) => new AddDistributionGroupToImListSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddImContactToGroup(AddImContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddImContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddImContactToGroupSoapResponse EndAddImContactToGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddImContactToGroupSoapResponse, AddImContactToGroupResponseMessage>(result, (AddImContactToGroupResponseMessage body) => new AddImContactToGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RemoveImContactFromGroupResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveImContactFromGroupSoapResponse EndRemoveImContactFromGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RemoveImContactFromGroupSoapResponse, RemoveImContactFromGroupResponseMessage>(result, (RemoveImContactFromGroupResponseMessage body) => new RemoveImContactFromGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddImGroup(AddImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddImGroupSoapResponse EndAddImGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddImGroupSoapResponse, AddImGroupResponseMessage>(result, (AddImGroupResponseMessage body) => new AddImGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddNewImContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddNewImContactToGroupSoapResponse EndAddNewImContactToGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddNewImContactToGroupSoapResponse, AddNewImContactToGroupResponseMessage>(result, (AddNewImContactToGroupResponseMessage body) => new AddNewImContactToGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<AddNewTelUriContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddNewTelUriContactToGroupSoapResponse EndAddNewTelUriContactToGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<AddNewTelUriContactToGroupSoapResponse, AddNewTelUriContactToGroupResponseMessage>(result, (AddNewTelUriContactToGroupResponseMessage body) => new AddNewTelUriContactToGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetImItemList(GetImItemListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetImItemListResponseMessage>(asyncCallback, asyncState);
		}

		public GetImItemListSoapResponse EndGetImItemList(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetImItemListSoapResponse, GetImItemListResponseMessage>(result, (GetImItemListResponseMessage body) => new GetImItemListSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetImItems(GetImItemsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetImItemsResponseMessage>(asyncCallback, asyncState);
		}

		public GetImItemsSoapResponse EndGetImItems(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetImItemsSoapResponse, GetImItemsResponseMessage>(result, (GetImItemsResponseMessage body) => new GetImItemsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RemoveContactFromImListResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveContactFromImListSoapResponse EndRemoveContactFromImList(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RemoveContactFromImListSoapResponse, RemoveContactFromImListResponseMessage>(result, (RemoveContactFromImListResponseMessage body) => new RemoveContactFromImListSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RemoveDistributionGroupFromImListResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveDistributionGroupFromImListSoapResponse EndRemoveDistributionGroupFromImList(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RemoveDistributionGroupFromImListSoapResponse, RemoveDistributionGroupFromImListResponseMessage>(result, (RemoveDistributionGroupFromImListResponseMessage body) => new RemoveDistributionGroupFromImListSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveImGroup(RemoveImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<RemoveImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveImGroupSoapResponse EndRemoveImGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<RemoveImGroupSoapResponse, RemoveImGroupResponseMessage>(result, (RemoveImGroupResponseMessage body) => new RemoveImGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetImGroup(SetImGroupSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public SetImGroupSoapResponse EndSetImGroup(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetImGroupSoapResponse, SetImGroupResponseMessage>(result, (SetImGroupResponseMessage body) => new SetImGroupSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetImListMigrationCompletedResponseMessage>(asyncCallback, asyncState);
		}

		public SetImListMigrationCompletedSoapResponse EndSetImListMigrationCompleted(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetImListMigrationCompletedSoapResponse, SetImListMigrationCompletedResponseMessage>(result, (SetImListMigrationCompletedResponseMessage body) => new SetImListMigrationCompletedSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUserRetentionPolicyTagsResponse>(asyncCallback, asyncState);
		}

		public GetUserRetentionPolicyTagsSoapResponse EndGetUserRetentionPolicyTags(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserRetentionPolicyTagsSoapResponse, GetUserRetentionPolicyTagsResponse>(result, (GetUserRetentionPolicyTagsResponse body) => new GetUserRetentionPolicyTagsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginStartFindInGALSpeechRecognition(StartFindInGALSpeechRecognitionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<StartFindInGALSpeechRecognitionResponseMessage>(asyncCallback, asyncState);
		}

		public StartFindInGALSpeechRecognitionSoapResponse EndStartFindInGALSpeechRecognition(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<StartFindInGALSpeechRecognitionSoapResponse, StartFindInGALSpeechRecognitionResponseMessage>(result, (StartFindInGALSpeechRecognitionResponseMessage body) => new StartFindInGALSpeechRecognitionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCompleteFindInGALSpeechRecognition(CompleteFindInGALSpeechRecognitionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CompleteFindInGALSpeechRecognitionResponseMessage>(asyncCallback, asyncState);
		}

		public CompleteFindInGALSpeechRecognitionSoapResponse EndCompleteFindInGALSpeechRecognition(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CompleteFindInGALSpeechRecognitionSoapResponse, CompleteFindInGALSpeechRecognitionResponseMessage>(result, (CompleteFindInGALSpeechRecognitionResponseMessage body) => new CompleteFindInGALSpeechRecognitionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserPhotoData(GetUserPhotoSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUserPhotoResponseMessage>(asyncCallback, asyncState);
		}

		public GetUserPhotoSoapResponse EndGetUserPhotoData(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserPhotoSoapResponse, GetUserPhotoResponseMessage>(result, (GetUserPhotoResponseMessage body) => new GetUserPhotoSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClientIntent(GetClientIntentSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetClientIntentResponseMessage>(asyncCallback, asyncState);
		}

		public GetClientIntentSoapResponse EndGetClientIntent(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetClientIntentSoapResponse, GetClientIntentResponseMessage>(result, (GetClientIntentResponseMessage body) => new GetClientIntentSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPerformInstantSearch(PerformInstantSearchSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<PerformInstantSearchResponse>(asyncCallback, asyncState);
		}

		public PerformInstantSearchSoapResponse EndPerformInstantSearch(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<PerformInstantSearchSoapResponse, PerformInstantSearchResponse>(result, (PerformInstantSearchResponse body) => new PerformInstantSearchSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginEndInstantSearchSession(EndInstantSearchSessionSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<EndInstantSearchSessionResponse>(asyncCallback, asyncState);
		}

		public EndInstantSearchSessionSoapResponse EndEndInstantSearchSession(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<EndInstantSearchSessionSoapResponse, EndInstantSearchSessionResponse>(result, (EndInstantSearchSessionResponse body) => new EndInstantSearchSessionSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserUnifiedGroups(GetUserUnifiedGroupsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUserUnifiedGroupsResponseMessage>(asyncCallback, asyncState);
		}

		public GetUserUnifiedGroupsSoapResponse EndGetUserUnifiedGroups(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUserUnifiedGroupsSoapResponse, GetUserUnifiedGroupsResponseMessage>(result, (GetUserUnifiedGroupsResponseMessage body) => new GetUserUnifiedGroupsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClutterState(GetClutterStateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetClutterStateResponse>(asyncCallback, asyncState);
		}

		public GetClutterStateSoapResponse EndGetClutterState(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetClutterStateSoapResponse, GetClutterStateResponse>(result, (GetClutterStateResponse body) => new GetClutterStateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetClutterState(SetClutterStateSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SetClutterStateResponse>(asyncCallback, asyncState);
		}

		public SetClutterStateSoapResponse EndSetClutterState(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SetClutterStateSoapResponse, SetClutterStateResponse>(result, (SetClutterStateResponse body) => new SetClutterStateSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserPhoto(string email, UserPhotoSize size, AsyncCallback callback, object state)
		{
			return new GetUserPhotoRequest(CallContext.Current.CreateWebResponseContext(), email, size, false, false).ValidateAndSubmit<GetUserPhotoResponse>(callback, state);
		}

		public Stream EndGetUserPhoto(IAsyncResult result)
		{
			ServiceAsyncResult<GetUserPhotoResponse> serviceAsyncResult = (ServiceAsyncResult<GetUserPhotoResponse>)result;
			if (serviceAsyncResult.Data != null && serviceAsyncResult.Data.ResponseMessages.Items != null && serviceAsyncResult.Data.ResponseMessages.Items.Length > 0)
			{
				return ((GetUserPhotoResponseMessage)serviceAsyncResult.Data.ResponseMessages.Items[0]).UserPhotoStream;
			}
			WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
			return new MemoryStream();
		}

		public IAsyncResult BeginGetPeopleICommunicateWith(AsyncCallback callback, object state)
		{
			return new GetPeopleICommunicateWithRequest(CallContext.Current.CreateWebResponseContext()).ValidateAndSubmit<GetPeopleICommunicateWithResponse>(callback, state);
		}

		public Stream EndGetPeopleICommunicateWith(IAsyncResult result)
		{
			ServiceAsyncResult<GetPeopleICommunicateWithResponse> serviceAsyncResult = (ServiceAsyncResult<GetPeopleICommunicateWithResponse>)result;
			if (serviceAsyncResult.Data != null && serviceAsyncResult.Data.ResponseMessages.Items != null && serviceAsyncResult.Data.ResponseMessages.Items.Length > 0)
			{
				return ((GetPeopleICommunicateWithResponseMessage)serviceAsyncResult.Data.ResponseMessages.Items[0]).Stream;
			}
			ExTraceGlobals.PeopleICommunicateWithTracer.TraceError<string>(0L, "Error in EndGetPeopleICommunicateWith - no results.", CallContext.Current.Description);
			WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
			return new MemoryStream();
		}

		public IAsyncResult BeginCreateUMCallDataRecord(CreateUMCallDataRecordSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<CreateUMCallDataRecordResponseMessage>(asyncCallback, asyncState);
		}

		public CreateUMCallDataRecordSoapResponse EndCreateUMCallDataRecord(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<CreateUMCallDataRecordSoapResponse, CreateUMCallDataRecordResponseMessage>(result, (CreateUMCallDataRecordResponseMessage body) => new CreateUMCallDataRecordSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMCallDataRecords(GetUMCallDataRecordsSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMCallDataRecordsResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMCallDataRecordsSoapResponse EndGetUMCallDataRecords(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMCallDataRecordsSoapResponse, GetUMCallDataRecordsResponseMessage>(result, (GetUMCallDataRecordsResponseMessage body) => new GetUMCallDataRecordsSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMCallSummary(GetUMCallSummarySoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMCallSummaryResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMCallSummarySoapResponse EndGetUMCallSummary(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMCallSummarySoapResponse, GetUMCallSummaryResponseMessage>(result, (GetUMCallSummaryResponseMessage body) => new GetUMCallSummarySoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginInitUMMailbox(InitUMMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<InitUMMailboxResponseMessage>(asyncCallback, asyncState);
		}

		public InitUMMailboxSoapResponse EndInitUMMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<InitUMMailboxSoapResponse, InitUMMailboxResponseMessage>(result, (InitUMMailboxResponseMessage body) => new InitUMMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginResetUMMailbox(ResetUMMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ResetUMMailboxResponseMessage>(asyncCallback, asyncState);
		}

		public ResetUMMailboxSoapResponse EndResetUMMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ResetUMMailboxSoapResponse, ResetUMMailboxResponseMessage>(result, (ResetUMMailboxResponseMessage body) => new ResetUMMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginValidateUMPin(ValidateUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<ValidateUMPinResponseMessage>(asyncCallback, asyncState);
		}

		public ValidateUMPinSoapResponse EndValidateUMPin(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<ValidateUMPinSoapResponse, ValidateUMPinResponseMessage>(result, (ValidateUMPinResponseMessage body) => new ValidateUMPinSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSaveUMPin(SaveUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<SaveUMPinResponseMessage>(asyncCallback, asyncState);
		}

		public SaveUMPinSoapResponse EndSaveUMPin(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<SaveUMPinSoapResponse, SaveUMPinResponseMessage>(result, (SaveUMPinResponseMessage body) => new SaveUMPinSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMPin(GetUMPinSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMPinResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMPinSoapResponse EndGetUMPin(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMPinSoapResponse, GetUMPinResponseMessage>(result, (GetUMPinResponseMessage body) => new GetUMPinSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUMSubscriberCallAnsweringData(GetUMSubscriberCallAnsweringDataSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<GetUMSubscriberCallAnsweringDataResponseMessage>(asyncCallback, asyncState);
		}

		public GetUMSubscriberCallAnsweringDataSoapResponse EndGetUMSubscriberCallAnsweringData(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<GetUMSubscriberCallAnsweringDataSoapResponse, GetUMSubscriberCallAnsweringDataResponseMessage>(result, (GetUMSubscriberCallAnsweringDataResponseMessage body) => new GetUMSubscriberCallAnsweringDataSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateMailboxAssociation(UpdateMailboxAssociationSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<UpdateMailboxAssociationResponse>(asyncCallback, asyncState);
		}

		public UpdateMailboxAssociationSoapResponse EndUpdateMailboxAssociation(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateMailboxAssociationSoapResponse, UpdateMailboxAssociationResponse>(result, (UpdateMailboxAssociationResponse body) => new UpdateMailboxAssociationSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateGroupMailbox(UpdateGroupMailboxSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.ValidateAndSubmit<UpdateGroupMailboxResponse>(asyncCallback, asyncState);
		}

		public UpdateGroupMailboxSoapResponse EndUpdateGroupMailbox(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<UpdateGroupMailboxSoapResponse, UpdateGroupMailboxResponse>(result, (UpdateGroupMailboxResponse body) => new UpdateGroupMailboxSoapResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPostModernGroupItem(PostModernGroupItemSoapRequest soapRequest, AsyncCallback asyncCallback, object asyncState)
		{
			return soapRequest.Body.Submit<PostModernGroupItemResponse>(asyncCallback, asyncState);
		}

		public PostModernGroupItemSoapResponse EndPostModernGroupItem(IAsyncResult result)
		{
			return EWSService.CreateSoapResponse<PostModernGroupItemSoapResponse, PostModernGroupItemResponse>(result, (PostModernGroupItemResponse body) => new PostModernGroupItemSoapResponse
			{
				Body = body
			});
		}

		private static TSoapResponse CreateSoapResponse<TSoapResponse, TSoapResponseBody>(IAsyncResult result, Func<TSoapResponseBody, TSoapResponse> createSoapResponseCallback)
		{
			bool flag = false;
			if (CallContext.Current.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(CallContext.Current.AccessingPrincipal.LegacyDn))
			{
				flag = true;
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
			TSoapResponse result2;
			try
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Entering End web method for {0}", CallContext.Current.Description);
				ServiceAsyncResult<TSoapResponseBody> serviceAsyncResult = EWSService.GetServiceAsyncResult<TSoapResponseBody>(result);
				TSoapResponse tsoapResponse = createSoapResponseCallback(serviceAsyncResult.Data);
				PerformanceMonitor.UpdateTotalCompletedRequestsCount();
				result2 = tsoapResponse;
			}
			finally
			{
				if (flag)
				{
					BaseTrace.CurrentThreadSettings.DisableTracing();
				}
			}
			return result2;
		}

		private static ServiceAsyncResult<TSoapResponseBody> GetServiceAsyncResult<TSoapResponseBody>(IAsyncResult result)
		{
			ServiceAsyncResult<TSoapResponseBody> serviceAsyncResult = (ServiceAsyncResult<TSoapResponseBody>)result;
			Exception ex = serviceAsyncResult.CompletionState as Exception;
			if (ex == null)
			{
				return serviceAsyncResult;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceError<Exception>(0L, "Request failed with: {0}", ex);
			Exception ex2 = ex;
			if (ex is GrayException)
			{
				ex2 = ex.InnerException;
			}
			LocalizedException ex3 = ex2 as LocalizedException;
			if (ex3 != null)
			{
				throw FaultExceptionUtilities.CreateFault(ex3, FaultParty.Receiver);
			}
			if (EWSService.IsServiceHandledException(ex2))
			{
				throw ex2;
			}
			throw new InternalServerErrorException(ex2);
		}

		private static bool IsServiceHandledException(Exception exception)
		{
			return exception is BailOutException || exception is FaultException;
		}
	}
}
