using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Conversations;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal abstract class ExchangeServiceBase : DisposeTrackableBase, IExchangeService, IDisposable
	{
		public AddAggregatedAccountResponse AddAggregatedAccount(AddAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<AddAggregatedAccountResponse>(() => new AddAggregatedAccount(this.CallContext, request), executionOption);
		}

		public Task<AddAggregatedAccountResponse> AddAggregatedAccountAsync(AddAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<AddAggregatedAccountResponse>(() => new AddAggregatedAccount(this.CallContext, request), executionOption);
		}

		public IsOffice365DomainResponse IsOffice365Domain(IsOffice365DomainRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<IsOffice365DomainResponse>(() => new IsOffice365Domain(this.CallContext, request), executionOption);
		}

		public Task<IsOffice365DomainResponse> IsOffice365DomainAsync(IsOffice365DomainRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<IsOffice365DomainResponse>(() => new IsOffice365Domain(this.CallContext, request), executionOption);
		}

		public GetAggregatedAccountResponse GetAggregatedAccount(GetAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetAggregatedAccountResponse>(() => new GetAggregatedAccount(this.CallContext, request), executionOption);
		}

		public Task<GetAggregatedAccountResponse> GetAggregatedAccountAsync(GetAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetAggregatedAccountResponse>(() => new GetAggregatedAccount(this.CallContext, request), executionOption);
		}

		public RemoveAggregatedAccountResponse RemoveAggregatedAccount(RemoveAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<RemoveAggregatedAccountResponse>(() => new RemoveAggregatedAccount(this.CallContext, request), executionOption);
		}

		public Task<RemoveAggregatedAccountResponse> RemoveAggregatedAccountAsync(RemoveAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<RemoveAggregatedAccountResponse>(() => new RemoveAggregatedAccount(this.CallContext, request), executionOption);
		}

		public SetAggregatedAccountResponse SetAggregatedAccount(SetAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SetAggregatedAccountResponse>(() => new SetAggregatedAccount(this.CallContext, request), executionOption);
		}

		public Task<SetAggregatedAccountResponse> SetAggregatedAccountAsync(SetAggregatedAccountRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SetAggregatedAccountResponse>(() => new SetAggregatedAccount(this.CallContext, request), executionOption);
		}

		public CreateUnifiedMailboxResponse CreateUnifiedMailbox(CreateUnifiedMailboxRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CreateUnifiedMailboxResponse>(() => new CreateUnifiedMailbox(this.CallContext, request), executionOption);
		}

		public Task<CreateUnifiedMailboxResponse> CreateUnifiedMailboxAsync(CreateUnifiedMailboxRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CreateUnifiedMailboxResponse>(() => new CreateUnifiedMailbox(this.CallContext, request), executionOption);
		}

		public GetFolderResponse GetFolder(GetFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetFolderResponse>(() => new GetFolder(this.CallContext, request), executionOption);
		}

		public Task<GetFolderResponse> GetFolderAsync(GetFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetFolderResponse>(() => new GetFolder(this.CallContext, request), executionOption);
		}

		public FindFolderResponse FindFolder(FindFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<FindFolderResponse>(() => new FindFolder(this.CallContext, request), executionOption);
		}

		public Task<FindFolderResponse> FindFolderAsync(FindFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<FindFolderResponse>(() => new FindFolder(this.CallContext, request), executionOption);
		}

		public CreateFolderResponse CreateFolder(CreateFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CreateFolderResponse>(() => new CreateFolder(this.CallContext, request), executionOption);
		}

		public Task<CreateFolderResponse> CreateFolderAsync(CreateFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CreateFolderResponse>(() => new CreateFolder(this.CallContext, request), executionOption);
		}

		public DeleteFolderResponse DeleteFolder(DeleteFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<DeleteFolderResponse>(() => new DeleteFolder(this.CallContext, request), executionOption);
		}

		public Task<DeleteFolderResponse> DeleteFolderAsync(DeleteFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<DeleteFolderResponse>(() => new DeleteFolder(this.CallContext, request), executionOption);
		}

		public UpdateFolderResponse UpdateFolder(UpdateFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<UpdateFolderResponse>(() => new UpdateFolder(this.CallContext, request), executionOption);
		}

		public Task<UpdateFolderResponse> UpdateFolderAsync(UpdateFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<UpdateFolderResponse>(() => new UpdateFolder(this.CallContext, request), executionOption);
		}

		public CopyFolderResponse CopyFolder(CopyFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CopyFolderResponse>(() => new CopyFolder(this.CallContext, request), executionOption);
		}

		public Task<CopyFolderResponse> CopyFolderAsync(CopyFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CopyFolderResponse>(() => new CopyFolder(this.CallContext, request), executionOption);
		}

		public MoveFolderResponse MoveFolder(MoveFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<MoveFolderResponse>(() => new MoveFolder(this.CallContext, request), executionOption);
		}

		public Task<MoveFolderResponse> MoveFolderAsync(MoveFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<MoveFolderResponse>(() => new MoveFolder(this.CallContext, request), executionOption);
		}

		public GetFavoritesResponse GetFavoriteFolders(ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<GetFavoritesResponse>(() => new GetFavorites(this.CallContext), executionOption, true);
		}

		public Task<GetFavoritesResponse> GetFavoriteFoldersAsync(ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommandAsync<GetFavoritesResponse>(() => new GetFavorites(this.CallContext), executionOption, true);
		}

		public UpdateFavoriteFolderResponse UpdateFavoriteFolder(UpdateFavoriteFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<UpdateFavoriteFolderResponse>(() => new UpdateFavoriteFolder(this.CallContext, request), executionOption, true);
		}

		public Task<UpdateFavoriteFolderResponse> UpdateFavoriteFolderAsync(UpdateFavoriteFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommandAsync<UpdateFavoriteFolderResponse>(() => new UpdateFavoriteFolder(this.CallContext, request), executionOption, true);
		}

		public GetItemResponse GetItem(GetItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetItemResponse>(() => new GetItem(this.CallContext, request), executionOption);
		}

		public Task<GetItemResponse> GetItemAsync(GetItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetItemResponse>(() => new GetItem(this.CallContext, request), executionOption);
		}

		public FindItemResponse FindItem(FindItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<FindItemResponse>(() => new FindItem(this.CallContext, request), executionOption);
		}

		public Task<FindItemResponse> FindItemAsync(FindItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<FindItemResponse>(() => new FindItem(this.CallContext, request), executionOption);
		}

		public IDisposableResponse<CreateItemResponse> CreateItem(CreateItemRequest request, ExecutionOption executionOption = null)
		{
			DisposableResponse<CreateItemResponse> response = null;
			try
			{
				response = new DisposableResponse<CreateItemResponse>(new CreateItem(this.CallContext, request), null);
				response.Response = this.InvokeServiceCommand<CreateItemResponse>(() => (CreateItem)response.Command, executionOption);
			}
			catch (Exception)
			{
				if (response != null)
				{
					response.Dispose();
					response = null;
				}
				throw;
			}
			return response;
		}

		public Task<IDisposableResponse<CreateItemResponse>> CreateItemAsync(CreateItemRequest request, ExecutionOption executionOption = null)
		{
			return Task<IDisposableResponse<CreateItemResponse>>.Factory.StartNew(() => this.CreateItem(request, executionOption));
		}

		public DeleteItemResponse DeleteItem(DeleteItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<DeleteItemResponse>(() => new DeleteItem(this.CallContext, request), executionOption);
		}

		public Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<DeleteItemResponse>(() => new DeleteItem(this.CallContext, request), executionOption);
		}

		public CopyItemResponse CopyItem(CopyItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CopyItemResponse>(() => new CopyItem(this.CallContext, request), executionOption);
		}

		public Task<CopyItemResponse> CopyItemAsync(CopyItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CopyItemResponse>(() => new CopyItem(this.CallContext, request), executionOption);
		}

		public MoveItemResponse MoveItem(MoveItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<MoveItemResponse>(() => new MoveItem(this.CallContext, request), executionOption);
		}

		public Task<MoveItemResponse> MoveItemAsync(MoveItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<MoveItemResponse>(() => new MoveItem(this.CallContext, request), executionOption);
		}

		public SendItemResponse SendItem(SendItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SendItemResponse>(() => new SendItem(this.CallContext, request), executionOption);
		}

		public Task<SendItemResponse> SendItemAsync(SendItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SendItemResponse>(() => new SendItem(this.CallContext, request), executionOption);
		}

		public GetConversationItemsResponse GetConversationItems(GetConversationItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetConversationItemsResponse>(() => new GetModernConversationItems(this.CallContext, request), executionOption);
		}

		public Task<GetConversationItemsResponse> GetConversationItemsAsync(GetConversationItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetConversationItemsResponse>(() => new GetModernConversationItems(this.CallContext, request), executionOption);
		}

		public GetThreadedConversationItemsResponse GetThreadedConversationItems(GetThreadedConversationItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetThreadedConversationItemsResponse>(() => new GetThreadedConversationItems(this.CallContext, request), executionOption);
		}

		public Task<GetThreadedConversationItemsResponse> GetThreadedConversationItemsAsync(GetThreadedConversationItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetThreadedConversationItemsResponse>(() => new GetThreadedConversationItems(this.CallContext, request), executionOption);
		}

		public FindConversationResponseMessage FindConversation(FindConversationRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<FindConversationResponseMessage>(() => new FindConversation(this.CallContext, request), executionOption);
		}

		public Task<FindConversationResponseMessage> FindConversationAsync(FindConversationRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<FindConversationResponseMessage>(() => new FindConversation(this.CallContext, request), executionOption);
		}

		public SyncCalendarResponse SyncCalendar(SyncCalendarParameters request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<SyncCalendarResponse>(() => new SyncCalendar(this.CallContext, request), executionOption, false);
		}

		public Task<SyncCalendarResponse> SyncCalendarAsync(SyncCalendarParameters request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommandAsync<SyncCalendarResponse>(() => new SyncCalendar(this.CallContext, request), executionOption, false);
		}

		public SyncFolderHierarchyResponse SyncFolderHierarchy(SyncFolderHierarchyRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SyncFolderHierarchyResponse>(() => new SyncFolderHierarchy(this.CallContext, request), executionOption);
		}

		public Task<SyncFolderHierarchyResponse> SyncFolderHierarchyAsync(SyncFolderHierarchyRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SyncFolderHierarchyResponse>(() => new SyncFolderHierarchy(this.CallContext, request), executionOption);
		}

		public SyncFolderItemsResponse SyncFolderItems(SyncFolderItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SyncFolderItemsResponse>(() => new SyncFolderItems(this.CallContext, request), executionOption);
		}

		public Task<SyncFolderItemsResponse> SyncFolderItemsAsync(SyncFolderItemsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SyncFolderItemsResponse>(() => new SyncFolderItems(this.CallContext, request), executionOption);
		}

		public SyncConversationResponseMessage SyncConversation(SyncConversationRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SyncConversationResponseMessage>(() => new SyncConversation(this.CallContext, request), executionOption);
		}

		public Task<SyncConversationResponseMessage> SyncConversationAsync(SyncConversationRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SyncConversationResponseMessage>(() => new SyncConversation(this.CallContext, request), executionOption);
		}

		public GetPersonaModernGroupMembershipResponse GetPersonaModernGroupMembership(GetPersonaModernGroupMembershipRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<GetPersonaModernGroupMembershipResponse>(() => new GetPersonaModernGroupMembership(this.CallContext, request), executionOption, false);
		}

		public GetModernGroupResponse GetModernGroup(GetModernGroupRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<GetModernGroupResponse>(() => new GetModernGroup(this.CallContext, request), executionOption, false);
		}

		public Task<GetModernGroupResponse> GetModernGroupAsync(GetModernGroupRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommandAsync<GetModernGroupResponse>(() => new GetModernGroup(this.CallContext, request), executionOption, false);
		}

		public UpdateItemResponse UpdateItem(UpdateItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<UpdateItemResponse>(() => new UpdateItem(this.CallContext, request), executionOption);
		}

		public Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<UpdateItemResponse>(() => new UpdateItem(this.CallContext, request), executionOption);
		}

		public GetUserPhotoResponse GetUserPhoto(GetUserPhotoRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetUserPhotoResponse>(() => new GetUserPhoto(this.CallContext, request, NullTracer.Instance), executionOption);
		}

		public Task<GetUserPhotoResponse> GetUserPhotoAsync(GetUserPhotoRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetUserPhotoResponse>(() => new GetUserPhoto(this.CallContext, request, NullTracer.Instance), executionOption);
		}

		public GetPeopleICommunicateWithResponse GetPeopleICommunicateWith(GetPeopleICommunicateWithRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetPeopleICommunicateWithResponse>(() => new GetPeopleICommunicateWith(this.CallContext, request), executionOption);
		}

		public Task<GetPeopleICommunicateWithResponse> GetPeopleICommunicateWithAsync(GetPeopleICommunicateWithRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetPeopleICommunicateWithResponse>(() => new GetPeopleICommunicateWith(this.CallContext, request), executionOption);
		}

		public ResolveNamesResponse ResolveNames(ResolveNamesRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<ResolveNamesResponse>(() => new ResolveNames(this.CallContext, request), executionOption);
		}

		public Task<ResolveNamesResponse> ResolveNamesAsync(ResolveNamesRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<ResolveNamesResponse>(() => new ResolveNames(this.CallContext, request), executionOption);
		}

		public ApplyConversationActionResponse ApplyConversationAction(ApplyConversationActionRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<ApplyConversationActionResponse>(() => new ApplyConversationAction(this.CallContext, request), executionOption);
		}

		public Task<ApplyConversationActionResponse> ApplyConversationActionAsync(ApplyConversationActionRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<ApplyConversationActionResponse>(() => new ApplyConversationAction(this.CallContext, request), executionOption);
		}

		public GetCalendarEventResponse GetCalendarEvent(GetCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetCalendarEventResponse>(() => new GetCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<GetCalendarEventResponse> GetCalendarEventAsync(GetCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetCalendarEventResponse>(() => new GetCalendarEvent(this.CallContext, request), executionOption);
		}

		public GetCalendarViewResponse GetCalendarView(GetCalendarViewRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetCalendarViewResponse>(() => new GetCalendarView(this.CallContext, request), executionOption);
		}

		public Task<GetCalendarViewResponse> GetCalendarViewAsync(GetCalendarViewRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetCalendarViewResponse>(() => new GetCalendarView(this.CallContext, request), executionOption);
		}

		public CancelCalendarEventResponse CancelCalendarEvent(CancelCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CancelCalendarEventResponse>(() => new CancelCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<CancelCalendarEventResponse> CancelCalendarEventAsync(CancelCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CancelCalendarEventResponse>(() => new CancelCalendarEvent(this.CallContext, request), executionOption);
		}

		public CreateCalendarEventResponse CreateCalendarEvent(CreateCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<CreateCalendarEventResponse>(() => new CreateCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<CreateCalendarEventResponse> CreateCalendarEventAsync(CreateCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<CreateCalendarEventResponse>(() => new CreateCalendarEvent(this.CallContext, request), executionOption);
		}

		public RespondToCalendarEventResponse RespondToCalendarEvent(RespondToCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<RespondToCalendarEventResponse>(() => new RespondToCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<RespondToCalendarEventResponse> RespondToCalendarEventAsync(RespondToCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<RespondToCalendarEventResponse>(() => new RespondToCalendarEvent(this.CallContext, request), executionOption);
		}

		public ExpandCalendarEventResponse ExpandCalendarEvent(ExpandCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<ExpandCalendarEventResponse>(() => new ExpandCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<ExpandCalendarEventResponse> ExpandCalendarEventAsync(ExpandCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<ExpandCalendarEventResponse>(() => new ExpandCalendarEvent(this.CallContext, request), executionOption);
		}

		public RefreshGALContactsFolderResponse RefreshGALContactsFolder(RefreshGALContactsFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<RefreshGALContactsFolderResponse>(() => new RefreshGALContactsFolderCommand(this.CallContext, request), executionOption);
		}

		public Task<RefreshGALContactsFolderResponse> RefreshGALContactsFolderAsync(RefreshGALContactsFolderRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<RefreshGALContactsFolderResponse>(() => new RefreshGALContactsFolderCommand(this.CallContext, request), executionOption);
		}

		public UpdateCalendarEventResponse UpdateCalendarEvent(UpdateCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<UpdateCalendarEventResponse>(() => new UpdateCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<UpdateCalendarEventResponse> UpdateCalendarEventAsync(UpdateCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<UpdateCalendarEventResponse>(() => new UpdateCalendarEvent(this.CallContext, request), executionOption);
		}

		public DeleteCalendarEventResponse DeleteCalendarEvent(DeleteCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<DeleteCalendarEventResponse>(() => new DeleteCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<DeleteCalendarEventResponse> DeleteCalendarEventAsync(DeleteCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<DeleteCalendarEventResponse>(() => new DeleteCalendarEvent(this.CallContext, request), executionOption);
		}

		public ForwardCalendarEventResponse ForwardCalendarEvent(ForwardCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<ForwardCalendarEventResponse>(() => new ForwardCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<ForwardCalendarEventResponse> ForwardCalendarEventAsync(ForwardCalendarEventRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<ForwardCalendarEventResponse>(() => new ForwardCalendarEvent(this.CallContext, request), executionOption);
		}

		public Task<IDisposableResponse<GetAttachmentResponse>> GetAttachmentAsync(GetAttachmentRequest request, ExecutionOption executionOption = null)
		{
			return Task<IDisposableResponse<GetAttachmentResponse>>.Factory.StartNew(() => this.GetAttachment(request, executionOption));
		}

		public IDisposableResponse<GetAttachmentResponse> GetAttachment(GetAttachmentRequest request, ExecutionOption executionOption = null)
		{
			DisposableResponse<GetAttachmentResponse> response = null;
			try
			{
				response = new DisposableResponse<GetAttachmentResponse>(new GetAttachment(this.CallContext, request), null);
				response.Response = this.InvokeServiceCommand<GetAttachmentResponse>(() => (GetAttachment)response.Command, executionOption);
			}
			catch (Exception)
			{
				if (response != null)
				{
					response.Dispose();
					response = null;
				}
				throw;
			}
			return response;
		}

		public IDisposableResponse<CreateAttachmentResponse> CreateAttachment(CreateAttachmentRequest request, ExecutionOption executionOption = null)
		{
			DisposableResponse<CreateAttachmentResponse> response = null;
			try
			{
				response = new DisposableResponse<CreateAttachmentResponse>(new CreateAttachment(this.CallContext, request), null);
				response.Response = this.InvokeServiceCommand<CreateAttachmentResponse>(() => (CreateAttachment)response.Command, executionOption);
			}
			catch (Exception)
			{
				if (response != null)
				{
					response.Dispose();
					response = null;
				}
				throw;
			}
			return response;
		}

		public Task<IDisposableResponse<CreateAttachmentResponse>> CreateAttachmentAsync(CreateAttachmentRequest request, ExecutionOption executionOption = null)
		{
			return Task<IDisposableResponse<CreateAttachmentResponse>>.Factory.StartNew(() => this.CreateAttachment(request, executionOption));
		}

		public DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<DeleteAttachmentResponse>(() => new DeleteAttachment(this.CallContext, request), executionOption);
		}

		public Task<DeleteAttachmentResponse> DeleteAttachmentAsync(DeleteAttachmentRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<DeleteAttachmentResponse>(() => new DeleteAttachment(this.CallContext, request), executionOption);
		}

		public ProvisionResponse Provision(ProvisionRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<ProvisionResponse>(() => new Provision(this.CallContext, request), executionOption);
		}

		public Task<ProvisionResponse> ProvisionAsync(ProvisionRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<ProvisionResponse>(() => new Provision(this.CallContext, request), executionOption);
		}

		public FindPeopleResponseMessage FindPeople(FindPeopleRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<FindPeopleResponseMessage>(() => new FindPeople(this.CallContext, request), executionOption);
		}

		public Task<FindPeopleResponseMessage> FindPeopleAsync(FindPeopleRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<FindPeopleResponseMessage>(() => new FindPeople(this.CallContext, request), executionOption);
		}

		public SyncAutoCompleteRecipientsResponseMessage SyncAutoCompleteRecipients(SyncAutoCompleteRecipientsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SyncAutoCompleteRecipientsResponseMessage>(() => new SyncAutoCompleteRecipients(this.CallContext, request), executionOption);
		}

		public Task<SyncAutoCompleteRecipientsResponseMessage> SyncAutoCompleteRecipientsAsync(SyncAutoCompleteRecipientsRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SyncAutoCompleteRecipientsResponseMessage>(() => new SyncAutoCompleteRecipients(this.CallContext, request), executionOption);
		}

		public GetPersonaResponseMessage GetPersona(GetPersonaRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetPersonaResponseMessage>(() => new GetPersona(this.CallContext, request), executionOption);
		}

		public Task<GetPersonaResponseMessage> GetPersonaAsync(GetPersonaRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<GetPersonaResponseMessage>(() => new GetPersona(this.CallContext, request), executionOption);
		}

		public ConversationFeedLoader GetConversationFeedLoader(ExTimeZone timezone)
		{
			return new ConversationFeedLoader(this.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), timezone);
		}

		public Guid GetMailboxGuid()
		{
			HttpContext.Current = this.CallContext.HttpContext;
			return this.CallContext.SessionCache.GetMailboxIdentityMailboxSession().MailboxGuid;
		}

		public Guid GetConversationGuidFromEwsId(string ewsId)
		{
			ConversationId conversationId = IdConverter.EwsIdToConversationId(ewsId);
			return new Guid(conversationId.GetBytes());
		}

		public GetComplianceConfigurationResponseMessage GetComplianceConfiguration(GetComplianceConfigurationRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetComplianceConfigurationResponseMessage>(() => new GetComplianceConfiguration(this.CallContext, request), executionOption);
		}

		public Guid GetMailboxGuidFromEwsId(string ewsId)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
			return new Guid(idHeaderInformation.MailboxId.MailboxGuid);
		}

		public string GetEwsIdFromConversationGuid(Guid conversationGuid, Guid mailboxGuid)
		{
			ConversationId conversationId = ConversationId.Create(conversationGuid);
			return IdConverter.ConversationIdToEwsId(mailboxGuid, conversationId);
		}

		public void GetItemMidFidDateFromEwsId(string ewsId, out long mid, out long fid, out ExDateTime date)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
			StoreObjectId storeObjectId = idHeaderInformation.ToStoreObjectId();
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(new Guid(idHeaderInformation.MailboxId.MailboxGuid)));
			IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
			mid = idConverter.GetMidFromMessageId(storeObjectId);
			fid = idConverter.GetFidFromId(storeObjectId);
			OccurrenceStoreObjectId occurrenceStoreObjectId = storeObjectId as OccurrenceStoreObjectId;
			if (occurrenceStoreObjectId != null)
			{
				date = occurrenceStoreObjectId.OccurrenceId;
				return;
			}
			date = ExDateTime.MinValue;
		}

		public string GetEwsIdFromItemMidFidDate(long mid, long fid, ExDateTime date, Guid mailboxGuid)
		{
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(mailboxGuid));
			IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
			StoreObjectId storeObjectId = idConverter.CreateMessageId(fid, mid);
			if (date != ExDateTime.MinValue)
			{
				storeObjectId = new OccurrenceStoreObjectId(storeObjectId.ProviderLevelItemId, date);
			}
			return StoreId.StoreIdToEwsId(mailboxGuid, storeObjectId);
		}

		public bool GetFolderFidAndMailboxFromEwsId(string ewsId, out long fid, out Guid mailboxGuid)
		{
			bool result;
			try
			{
				IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
				if (!Guid.TryParse(idHeaderInformation.MailboxId.MailboxGuid, out mailboxGuid))
				{
					fid = 0L;
					result = false;
				}
				else
				{
					StoreObjectId storeObjectId = idHeaderInformation.ToStoreObjectId();
					MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(new Guid(idHeaderInformation.MailboxId.MailboxGuid)));
					IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
					fid = idConverter.GetFidFromId(storeObjectId);
					result = true;
				}
			}
			catch
			{
				mailboxGuid = Guid.Empty;
				fid = 0L;
				result = false;
			}
			return result;
		}

		public long GetFolderFidFromEwsId(string ewsId)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
			StoreObjectId storeObjectId = idHeaderInformation.ToStoreObjectId();
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(new Guid(idHeaderInformation.MailboxId.MailboxGuid)));
			IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
			return idConverter.GetFidFromId(storeObjectId);
		}

		public string GetEwsIdFromFolderFid(long fid, Guid mailboxGuid)
		{
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(mailboxGuid));
			IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
			StoreObjectId storeId = idConverter.CreateFolderId(fid);
			return StoreId.StoreIdToEwsId(mailboxGuid, storeId);
		}

		public string GetDistinguishedFolderIdFromEwsId(string ewsId)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
			StoreObjectId folderId = idHeaderInformation.ToStoreObjectId();
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(new Guid(idHeaderInformation.MailboxId.MailboxGuid)));
			if (EWSSettings.DistinguishedFolderIdNameDictionary == null)
			{
				EWSSettings.DistinguishedFolderIdNameDictionary = new DistinguishedFolderIdNameDictionary();
			}
			return EWSSettings.DistinguishedFolderIdNameDictionary.Get(folderId, mailboxSessionByMailboxId);
		}

		public Guid GetAttachmentGuidFromEwsId(string ewsId, long mid, long fid)
		{
			long num;
			long num2;
			ExDateTime exDateTime;
			this.GetItemMidFidDateFromEwsId(ewsId, out num, out num2, out exDateTime);
			if (mid != num || fid != num2)
			{
				throw new InvalidOperationException("Item id mismatch.");
			}
			List<AttachmentId> list = new List<AttachmentId>();
			ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.ItemOrAttachment, list);
			if (list.Count > 1)
			{
				throw new InvalidOperationException("Nested attachments not supported.");
			}
			byte[] array = list[0].ToByteArray();
			if (array.Length != 18)
			{
				throw new InvalidOperationException("Unexpected attachment key.");
			}
			byte[] array2 = new byte[16];
			Array.Copy(array, 2, array2, 0, 16);
			return new Guid(array2);
		}

		public string GetEwsIdFromAttachmentGuid(Guid attachmentGuid, long mid, long fid, Guid mailboxGuid)
		{
			MailboxId mailboxId = new MailboxId(mailboxGuid);
			MailboxSession mailboxSessionByMailboxId = this.CallContext.SessionCache.GetMailboxSessionByMailboxId(mailboxId);
			IdConverter idConverter = mailboxSessionByMailboxId.IdConverter;
			StoreId storeId = idConverter.CreateMessageId(fid, mid);
			byte[] array = attachmentGuid.ToByteArray();
			short num = (short)array.Length;
			byte[] array2 = new byte[(int)(2 + num)];
			int num2 = 0;
			num2 += ExBitConverter.Write(num, array2, num2);
			array.CopyTo(array2, num2);
			AttachmentId item = AttachmentId.Deserialize(array2);
			return IdConverter.GetConcatenatedId(storeId, mailboxId, new List<AttachmentId>
			{
				item
			}).Id;
		}

		public SubscribeToConversationChangesResponseMessage SubscribeToConversationChanges(SubscribeToConversationChangesRequest request, Action<ConversationNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SubscribeToConversationChangesResponseMessage>(() => new SubscribeToConversationChanges(this.CallContext, request, callback), executionOption);
		}

		public Task<SubscribeToConversationChangesResponseMessage> SubscribeToConversationChangesAsync(SubscribeToConversationChangesRequest request, Action<ConversationNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SubscribeToConversationChangesResponseMessage>(() => new SubscribeToConversationChanges(this.CallContext, request, callback), executionOption);
		}

		public void SetCallContextFromActionInfo(string actionQueueId, string protocol, string deviceType, string actionId, bool IsOutlookService)
		{
			this.CallContext.SetCallContextFromActionInfo(actionQueueId, protocol, deviceType, actionId, IsOutlookService);
		}

		public void DisableDupDetection()
		{
			this.CallContext.DisableDupDetection();
		}

		public bool TryGetResponse<T>(out T results)
		{
			return this.CallContext.TryGetResponse<T>(out results);
		}

		public void SetResponse<T>(T result, Exception exception)
		{
			this.CallContext.SetResponse<T>(result, exception);
		}

		public bool? GetIsDuplicatedAction()
		{
			return this.CallContext.IsDuplicatedAction;
		}

		public bool GetReturningSavedResult()
		{
			return this.CallContext.ReturningSavedResult;
		}

		public bool GetResultSaved()
		{
			return this.CallContext.ResultSaved;
		}

		public QuotedTextResult ParseQuotedText(string msg, bool reorderMsgs)
		{
			return QuotedText.ParseHtmlQuotedText(msg, reorderMsgs);
		}

		public IOutlookServiceStorage GetOutlookServiceStorage()
		{
			HttpContext.Current = this.CallContext.HttpContext;
			MailboxSession mailboxIdentityMailboxSession = this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			return OutlookServiceStorage.Create(mailboxIdentityMailboxSession);
		}

		public SubscribeToCalendarChangesResponseMessage SubscribeToCalendarChanges(SubscribeToCalendarChangesRequest request, Action<CalendarChangeNotificationType> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SubscribeToCalendarChangesResponseMessage>(() => new SubscribeToCalendarChanges(this.CallContext, request, callback), executionOption);
		}

		public Task<SubscribeToCalendarChangesResponseMessage> SubscribeToCalendarChangesAsync(SubscribeToCalendarChangesRequest request, Action<CalendarChangeNotificationType> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SubscribeToCalendarChangesResponseMessage>(() => new SubscribeToCalendarChanges(this.CallContext, request, callback), executionOption);
		}

		public GetCalendarFoldersResponse GetCalendarFolders(ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<GetCalendarFoldersResponse>(() => new GetCalendarFoldersCommand(this.CallContext), executionOption, false);
		}

		public IHtmlReader GetHtmlReader(string html)
		{
			return new HtmlReaderWrapper(html);
		}

		public PerformInstantSearchResponse PerformInstantSearch(PerformInstantSearchRequest request, Action<InstantSearchPayloadType> searchPayloadCallback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<PerformInstantSearchResponse>(() => new PerformInstantSearch(this.CallContext, searchPayloadCallback, request), executionOption);
		}

		public Task<PerformInstantSearchResponse> PerformInstantSearchAsync(PerformInstantSearchRequest request, Action<InstantSearchPayloadType> searchPayloadCallback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<PerformInstantSearchResponse>(() => new PerformInstantSearch(this.CallContext, searchPayloadCallback, request), executionOption);
		}

		public EndInstantSearchSessionResponse EndInstantSearchSession(string deviceId, string sessionId, ExecutionOption executionOption = null)
		{
			EndInstantSearchSessionRequest request = new EndInstantSearchSessionRequest();
			request.DeviceId = deviceId;
			request.SessionId = sessionId;
			return this.InvokeServiceCommand<EndInstantSearchSessionResponse>(() => new EndInstantSearchSession(this.CallContext, request), executionOption);
		}

		public Task<EndInstantSearchSessionResponse> EndInstantSearchSessionAsync(string deviceId, string sessionId, ExecutionOption executionOption = null)
		{
			EndInstantSearchSessionRequest request = new EndInstantSearchSessionRequest();
			request.DeviceId = deviceId;
			request.SessionId = sessionId;
			return this.InvokeServiceCommandAsync<EndInstantSearchSessionResponse>(() => new EndInstantSearchSession(this.CallContext, request), executionOption);
		}

		public string GetBodyWithQuotedText(string messageBody, List<string> messageIds, List<string> messageBodyColl)
		{
			return QuotedTextBuilder.GetBodyWithQuotedText(this.CallContext, messageBody, messageIds, messageBodyColl);
		}

		public MasterCategoryListActionResponse GetMasterCategoryList(GetMasterCategoryListRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeOwsServiceCommand<MasterCategoryListActionResponse>(() => new GetMasterCategoryListCommand(this.CallContext, request), executionOption, false);
		}

		public GetUserAvailabilityResponse GetUserAvailability(GetUserAvailabilityRequest request, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<GetUserAvailabilityResponse>(() => new GetUserAvailability(this.CallContext, request), executionOption);
		}

		public SubscribeToHierarchyChangesResponseMessage SubscribeToHierarchyChanges(SubscribeToHierarchyChangesRequest request, Action<HierarchyNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SubscribeToHierarchyChangesResponseMessage>(() => new SubscribeToHierarchyChanges(this.CallContext, request, callback), executionOption);
		}

		public Task<SubscribeToHierarchyChangesResponseMessage> SubscribeToHierarchyChangesAsync(SubscribeToHierarchyChangesRequest request, Action<HierarchyNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SubscribeToHierarchyChangesResponseMessage>(() => new SubscribeToHierarchyChanges(this.CallContext, request, callback), executionOption);
		}

		public SubscribeToMessageChangesResponseMessage SubscribeToMessageChanges(SubscribeToMessageChangesRequest request, Action<MessageNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommand<SubscribeToMessageChangesResponseMessage>(() => new SubscribeToMessageChanges(this.CallContext, request, callback), executionOption);
		}

		public Task<SubscribeToMessageChangesResponseMessage> SubscribeToMessageChangesAsync(SubscribeToMessageChangesRequest request, Action<MessageNotification> callback, ExecutionOption executionOption = null)
		{
			return this.InvokeServiceCommandAsync<SubscribeToMessageChangesResponseMessage>(() => new SubscribeToMessageChanges(this.CallContext, request, callback), executionOption);
		}

		public void SetRequestTimeZoneId(string timeZoneId)
		{
			if (!string.IsNullOrEmpty(timeZoneId))
			{
				ExTimeZone exTimeZone = null;
				ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneId, out exTimeZone);
				if (exTimeZone != null)
				{
					EWSSettings.RequestTimeZone = exTimeZone;
				}
			}
		}

		public ExDateTime GetOriginalStartDateFromEwsId(string ewsId)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsId, BasicTypes.Item, null);
			StoreObjectId storeObjectId = idHeaderInformation.ToStoreObjectId();
			OccurrenceStoreObjectId occurrenceStoreObjectId = storeObjectId as OccurrenceStoreObjectId;
			if (occurrenceStoreObjectId != null)
			{
				return occurrenceStoreObjectId.OccurrenceId;
			}
			return ExDateTime.MinValue;
		}

		public PatternedRecurrence ConvertToPatternedRecurrence(Recurrence value)
		{
			RecurrenceConverter recurrenceConverter = new RecurrenceConverter(value.CreatedExTimeZone);
			return recurrenceConverter.Convert(value);
		}

		protected CallContext CallContext { get; set; }

		protected IActivityScope ActivityScope { get; set; }

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeServiceBase>(this);
		}

		protected virtual TResponse InvokeServiceCommand<TResponse>(Func<ServiceCommandBase> commandCreator, ExecutionOption executionOption)
		{
			ExecutionOption executionOption2 = executionOption ?? ExecutionOption.Default;
			TResponse response = default(TResponse);
			this.CallWithExceptionHandling(executionOption2, delegate
			{
				HttpContext.Current = this.CallContext.HttpContext;
				ServiceCommandBase serviceCommandBase = commandCreator();
				if (serviceCommandBase.PreExecute())
				{
					for (int i = 0; i < serviceCommandBase.StepCount; i++)
					{
						this.CallContext.Budget.CheckOverBudget();
						TaskExecuteResult taskExecuteResult = serviceCommandBase.ExecuteStep();
						if (taskExecuteResult == TaskExecuteResult.ProcessingComplete)
						{
							break;
						}
					}
				}
				response = (TResponse)((object)serviceCommandBase.PostExecute());
			});
			ExchangeServiceHelper.CheckResponse(response as IExchangeWebMethodResponse, executionOption2);
			return response;
		}

		protected virtual Task<TResponse> InvokeServiceCommandAsync<TResponse>(Func<ServiceCommandBase> commandCreator, ExecutionOption executionOption)
		{
			return Task<TResponse>.Factory.StartNew(() => this.InvokeServiceCommand<TResponse>(commandCreator, executionOption));
		}

		protected virtual TResponse InvokeOwsServiceCommand<TResponse>(Func<ServiceCommand<TResponse>> commandCreator, ExecutionOption executionOption, bool throwOnNullResponse = false)
		{
			ExecutionOption executionOption2 = executionOption ?? ExecutionOption.Default;
			TResponse response = default(TResponse);
			this.CallWithExceptionHandling(executionOption2, delegate
			{
				ServiceCommand<TResponse> serviceCommand = commandCreator();
				HttpContext.Current = this.CallContext.HttpContext;
				this.CallContext.Budget.CheckOverBudget();
				response = serviceCommand.Execute();
			});
			if (response == null && throwOnNullResponse)
			{
				throw new ExchangeServiceResponseException(CoreResources.ExchangeServiceResponseErrorNoResponse);
			}
			return response;
		}

		protected virtual Task<TResponse> InvokeOwsServiceCommandAsync<TResponse>(Func<ServiceCommand<TResponse>> commandCreator, ExecutionOption executionOption, bool throwOnNullResponse = false)
		{
			return Task<TResponse>.Factory.StartNew(() => this.InvokeOwsServiceCommand<TResponse>(commandCreator, executionOption, throwOnNullResponse));
		}

		protected void CallWithExceptionHandling(ExecutionOption executionOption, Action action)
		{
			if (executionOption.WrapExecutionExceptions)
			{
				try
				{
					action();
					return;
				}
				catch (ServicePermanentException innerException)
				{
					throw new ExchangeServicePermanentException(innerException);
				}
				catch (StoragePermanentException innerException2)
				{
					throw new ExchangeServicePermanentException(innerException2);
				}
				catch (StorageTransientException innerException3)
				{
					throw new ExchangeServiceTransientException(innerException3);
				}
				catch (FaultException ex)
				{
					throw new ExchangeServicePermanentException(new LocalizedString(ex.Message), ex);
				}
			}
			action();
		}

		public static readonly string RenewTag = 3841U.ToString();
	}
}
