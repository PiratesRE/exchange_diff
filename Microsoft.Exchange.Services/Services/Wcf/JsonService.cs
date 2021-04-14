using System;
using System.IO;
using System.Net;
using System.Runtime.ExceptionServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Wcf
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[MessageInspectorBehavior]
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	public class JsonService : IJsonServiceContract, IO365SuiteServiceContract, IJsonStreamingServiceContract
	{
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

		public ExtensibilityContext GetExtensibilityContext(GetExtensibilityContextParameters request)
		{
			return new GetExtensibilityContext(CallContext.Current, request).Execute();
		}

		public bool AddBuddy(Buddy buddy)
		{
			return new AddBuddyCommand(CallContext.Current, buddy).Execute();
		}

		public GetBuddyListResponse GetBuddyList()
		{
			return new GetBuddyListCommand(CallContext.Current).Execute();
		}

		public IAsyncResult BeginFindPlaces(FindPlacesRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return new FindPlaces(CallContext.Current, request, asyncCallback, asyncState).Execute();
		}

		public Persona[] EndFindPlaces(IAsyncResult result)
		{
			FindPlacesAsyncResult findPlacesAsyncResult = result as FindPlacesAsyncResult;
			findPlacesAsyncResult.EndTimeoutDetection();
			if (findPlacesAsyncResult.Fault != null)
			{
				throw findPlacesAsyncResult.Fault;
			}
			return findPlacesAsyncResult.Data;
		}

		public DeletePlaceJsonResponse DeletePlace(DeletePlaceRequest request)
		{
			new DeletePlaceCommand(CallContext.Current, request).Execute();
			return new DeletePlaceJsonResponse();
		}

		public CalendarActionResponse AddEventToMyCalendar(AddEventToMyCalendarRequest request)
		{
			return new AddEventToMyCalendarCommand(CallContext.Current, request).Execute();
		}

		public bool AddTrustedSender(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new AddTrustedSender(CallContext.Current, itemId).Execute();
		}

		public GetPersonaModernGroupMembershipJsonResponse GetPersonaModernGroupMembership(GetPersonaModernGroupMembershipJsonRequest request)
		{
			return new GetPersonaModernGroupMembershipJsonResponse
			{
				Body = new GetPersonaModernGroupMembership(CallContext.Current, request.Body).Execute()
			};
		}

		public GetModernGroupJsonResponse GetModernGroup(GetModernGroupJsonRequest request)
		{
			return new GetModernGroupJsonResponse
			{
				Body = new GetModernGroup(CallContext.Current, request.Body).Execute()
			};
		}

		public GetModernGroupJsonResponse GetRecommendedModernGroup(GetModernGroupJsonRequest request)
		{
			return new GetModernGroupJsonResponse
			{
				Body = new GetRecommendedModernGroup(CallContext.Current, request.Body).Execute()
			};
		}

		public GetModernGroupsJsonResponse GetModernGroups()
		{
			return new GetModernGroupsJsonResponse
			{
				Body = new GetModernGroups(CallContext.Current).Execute()
			};
		}

		public SetModernGroupPinStateJsonResponse SetModernGroupPinState(string smtpAddress, bool isPinned)
		{
			return new SetModernGroupPinState(CallContext.Current, smtpAddress, isPinned).Execute();
		}

		public SetModernGroupMembershipJsonResponse SetModernGroupMembership(SetModernGroupMembershipJsonRequest request)
		{
			return new SetModernGroupMembershipJsonResponse
			{
				Body = new SetModernGroupMembership(CallContext.Current, request).Execute()
			};
		}

		public bool SetModernGroupSubscription()
		{
			return new SetModernGroupSubscription(CallContext.Current).Execute();
		}

		public GetModernGroupUnseenItemsJsonResponse GetModernGroupUnseenItems(GetModernGroupUnseenItemsJsonRequest request)
		{
			return new GetModernGroupUnseenItemsJsonResponse
			{
				Body = new GetModernGroupUnseenItems(CallContext.Current).Execute()
			};
		}

		public GetFavoritesResponse GetFavorites()
		{
			return new GetFavorites(CallContext.Current).Execute();
		}

		public UpdateFavoriteFolderResponse UpdateFavoriteFolder(UpdateFavoriteFolderRequest request)
		{
			return new UpdateFavoriteFolder(CallContext.Current, request).Execute();
		}

		public UpdateMasterCategoryListResponse UpdateMasterCategoryList(UpdateMasterCategoryListRequest request)
		{
			return new UpdateMasterCategoryList(CallContext.Current, request).Execute();
		}

		public MasterCategoryListActionResponse GetMasterCategoryList(GetMasterCategoryListRequest request)
		{
			return new GetMasterCategoryListCommand(CallContext.Current, request).Execute();
		}

		public GetTaskFoldersResponse GetTaskFolders()
		{
			return new GetTaskFoldersCommand(CallContext.Current).Execute();
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

		public IAsyncResult BeginGetModernConversationAttachments(GetModernConversationAttachmentsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetModernConversationAttachmentsResponse>(asyncCallback, asyncState);
		}

		public GetModernConversationAttachmentsJsonResponse EndGetModernConversationAttachments(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetModernConversationAttachmentsJsonResponse, GetModernConversationAttachmentsResponse>(result, (GetModernConversationAttachmentsResponse body) => new GetModernConversationAttachmentsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindTrendingConversation(FindTrendingConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<FindConversationResponseMessage>(asyncCallback, asyncState);
		}

		public FindConversationJsonResponse EndFindTrendingConversation(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<FindConversationJsonResponse, FindConversationResponseMessage>(result, (FindConversationResponseMessage body) => new FindConversationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPostModernGroupItem(PostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<PostModernGroupItemResponse>(asyncCallback, asyncState);
		}

		public PostModernGroupItemJsonResponse EndPostModernGroupItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<PostModernGroupItemJsonResponse, PostModernGroupItemResponse>(result, (PostModernGroupItemResponse body) => new PostModernGroupItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateAndPostModernGroupItem(UpdateAndPostModernGroupItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UpdateAndPostModernGroupItemResponse>(asyncCallback, asyncState);
		}

		public UpdateAndPostModernGroupItemJsonResponse EndUpdateAndPostModernGroupItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UpdateAndPostModernGroupItemJsonResponse, UpdateAndPostModernGroupItemResponse>(result, (UpdateAndPostModernGroupItemResponse body) => new UpdateAndPostModernGroupItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateResponseFromModernGroup(CreateResponseFromModernGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CreateResponseFromModernGroupResponse>(asyncCallback, asyncState);
		}

		public CreateResponseFromModernGroupJsonResponse EndCreateResponseFromModernGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CreateResponseFromModernGroupJsonResponse, CreateResponseFromModernGroupResponse>(result, (CreateResponseFromModernGroupResponse body) => new CreateResponseFromModernGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginExecuteEwsProxy(EwsProxyRequestParameters request, AsyncCallback asyncCallback, object asyncState)
		{
			return new ExecuteEwsProxy(CallContext.Current, request.Body, request.Token, request.ExtensionId, asyncCallback, asyncState).Execute();
		}

		public EwsProxyResponse EndExecuteEwsProxy(IAsyncResult result)
		{
			ServiceAsyncResult<EwsProxyResponse> serviceAsyncResult = result as ServiceAsyncResult<EwsProxyResponse>;
			return serviceAsyncResult.Data;
		}

		public SaveExtensionSettingsResponse SaveExtensionSettings(SaveExtensionSettingsParameters request)
		{
			return new SaveExtensionSettings(CallContext.Current, request.ExtensionId, request.ExtensionVersion, request.Settings).Execute();
		}

		public LoadExtensionCustomPropertiesResponse LoadExtensionCustomProperties(LoadExtensionCustomPropertiesParameters request)
		{
			return new LoadExtensionCustomProperties(CallContext.Current, request.ExtensionId, request.ItemId).Execute();
		}

		public SaveExtensionCustomPropertiesResponse SaveExtensionCustomProperties(SaveExtensionCustomPropertiesParameters request)
		{
			return new SaveExtensionCustomProperties(CallContext.Current, request.ExtensionId, request.ItemId, request.CustomProperties).Execute();
		}

		public Persona UpdatePersona(UpdatePersonaJsonRequest request)
		{
			return new UpdatePersonaCommand(CallContext.Current, request.Body).Execute();
		}

		public DeletePersonaJsonResponse DeletePersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, BaseFolderId deleteInFolder)
		{
			new DeletePersonaCommand(CallContext.Current, personaId, deleteInFolder).Execute();
			return new DeletePersonaJsonResponse();
		}

		public MaskAutoCompleteRecipientResponse MaskAutoCompleteRecipient(MaskAutoCompleteRecipientRequest request)
		{
			return new MaskAutoCompleteRecipientCommand(CallContext.Current, request).Execute();
		}

		public Persona CreatePersona(CreatePersonaJsonRequest request)
		{
			return new CreatePersonaCommand(CallContext.Current, request.Body).Execute();
		}

		public CreateModernGroupResponse CreateModernGroup(CreateModernGroupRequest request)
		{
			return new CreateModernGroupCommand(CallContext.Current, request).Execute();
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

		public GetPeopleIKnowGraphResponse GetPeopleIKnowGraphCommand(GetPeopleIKnowGraphRequest request)
		{
			return new GetPeopleIKnowGraphCommand(CallContext.Current, request).Execute();
		}

		public Microsoft.Exchange.Services.Core.Types.ItemId[] GetPersonaSuggestions(Microsoft.Exchange.Services.Core.Types.ItemId personaId)
		{
			return new GetPersonaSuggestionsCommand(CallContext.Current, personaId).Execute();
		}

		public Persona UnlinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId contactId)
		{
			return new UnlinkPersonaCommand(CallContext.Current, personaId, contactId).Execute();
		}

		public Persona AcceptPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId)
		{
			return new AcceptPersonaLinkSuggestionCommand(CallContext.Current, personaId, suggestedPersonaId).Execute();
		}

		public Persona LinkPersona(Microsoft.Exchange.Services.Core.Types.ItemId linkToPersonaId, Microsoft.Exchange.Services.Core.Types.ItemId personaIdToBeLinked)
		{
			return new LinkPersonaCommand(CallContext.Current, linkToPersonaId, personaIdToBeLinked).Execute();
		}

		public Persona RejectPersonaLinkSuggestion(Microsoft.Exchange.Services.Core.Types.ItemId personaId, Microsoft.Exchange.Services.Core.Types.ItemId suggestedPersonaId)
		{
			return new RejectPersonaLinkSuggestionCommand(CallContext.Current, personaId, suggestedPersonaId).Execute();
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
			return new GetCalendarFolderConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetUserAvailabilityInternalJsonResponse GetUserAvailabilityInternal(GetUserAvailabilityInternalJsonRequest request)
		{
			return new GetUserAvailabilityInternalCommand(CallContext.Current, request.Body).Execute();
		}

		public SyncCalendarResponse SyncCalendar(SyncCalendarParameters request)
		{
			SyncCalendar syncCalendar = new SyncCalendar(CallContext.Current, request);
			return syncCalendar.Execute();
		}

		public UpdateUserConfigurationResponse UpdateUserConfiguration(UpdateUserConfigurationRequest request)
		{
			UpdateUserConfiguration updateUserConfiguration = new UpdateUserConfiguration(CallContext.Current, request);
			updateUserConfiguration.Execute();
			return (UpdateUserConfigurationResponse)updateUserConfiguration.GetResponse();
		}

		public bool SendReadReceipt(Microsoft.Exchange.Services.Core.Types.ItemId itemId)
		{
			return new SendReadReceipt(CallContext.Current, itemId).Execute();
		}

		public SuiteStorageJsonResponse ProcessSuiteStorage(SuiteStorageJsonRequest request)
		{
			return new SuiteStorageJsonResponse
			{
				Body = new ProcessSuiteStorage(CallContext.Current, request.Body).Execute()
			};
		}

		public SuiteStorageJsonResponse ProcessO365SuiteStorage(SuiteStorageJsonRequest request)
		{
			return new SuiteStorageJsonResponse
			{
				Body = new ProcessSuiteStorage(CallContext.Current, request.Body).Execute()
			};
		}

		public IAsyncResult BeginGetWeatherForecast(GetWeatherForecastJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return new GetWeatherForecast(CallContext.Current, request.Body, asyncCallback, asyncState, new WeatherService(WeatherConfigurationCache.Instance)).Execute();
		}

		public GetWeatherForecastJsonResponse EndGetWeatherForecast(IAsyncResult result)
		{
			ServiceAsyncResult<Task<GetWeatherForecastResponse>> serviceAsyncResult = result as ServiceAsyncResult<Task<GetWeatherForecastResponse>>;
			if (serviceAsyncResult == null || serviceAsyncResult.Data == null)
			{
				throw new FaultException("IAsyncResult in EndGetWeatherForecast was null or not of the expected type.");
			}
			if (serviceAsyncResult.Data.IsFaulted)
			{
				throw new FaultException((serviceAsyncResult.Data.Exception != null) ? serviceAsyncResult.Data.Exception.InnerExceptions[0].Message : CoreResources.GetLocalizedString((CoreResources.IDs)2933471333U));
			}
			return new GetWeatherForecastJsonResponse
			{
				Body = serviceAsyncResult.Data.Result
			};
		}

		public IAsyncResult BeginFindWeatherLocations(FindWeatherLocationsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return new FindWeatherLocations(CallContext.Current, request.Body, asyncCallback, asyncState, new WeatherService(WeatherConfigurationCache.Instance)).Execute();
		}

		public FindWeatherLocationsJsonResponse EndFindWeatherLocations(IAsyncResult result)
		{
			ServiceAsyncResult<Task<FindWeatherLocationsResponse>> serviceAsyncResult = result as ServiceAsyncResult<Task<FindWeatherLocationsResponse>>;
			if (serviceAsyncResult == null || serviceAsyncResult.Data == null)
			{
				throw new FaultException("IAsyncResult in EndFindWeatherLocations was null or not of the expected type.");
			}
			if (serviceAsyncResult.Data.IsFaulted)
			{
				throw new FaultException((serviceAsyncResult.Data.Exception != null) ? serviceAsyncResult.Data.Exception.InnerExceptions[0].Message : CoreResources.GetLocalizedString(CoreResources.IDs.MessageCouldNotFindWeatherLocations));
			}
			return new FindWeatherLocationsJsonResponse
			{
				Body = serviceAsyncResult.Data.Result
			};
		}

		public DisableAppDataResponse DisableApp(DisableAppDataRequest request)
		{
			return new DisableAppCommand(CallContext.Current, request).Execute();
		}

		public EnableAppDataResponse EnableApp(EnableAppDataRequest request)
		{
			return new EnableAppCommand(CallContext.Current, request).Execute();
		}

		public RemoveAppDataResponse RemoveApp(RemoveAppDataRequest request)
		{
			return new RemoveAppCommand(CallContext.Current, request).Execute();
		}

		public GetCalendarNotificationResponse GetCalendarNotification()
		{
			return new GetCalendarNotificationCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetCalendarNotification(SetCalendarNotificationRequest request)
		{
			return new SetCalendarNotificationCommand(CallContext.Current, request).Execute();
		}

		public GetCalendarProcessingResponse GetCalendarProcessing()
		{
			return new GetCalendarProcessingCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetCalendarProcessing(SetCalendarProcessingRequest request)
		{
			return new SetCalendarProcessingCommand(CallContext.Current, request).Execute();
		}

		public GetCASMailboxResponse GetCASMailbox()
		{
			return new GetCASMailboxCommand(CallContext.Current, null).Execute();
		}

		public GetCASMailboxResponse GetCASMailbox2(GetCASMailboxRequest request)
		{
			return new GetCASMailboxCommand(CallContext.Current, request).Execute();
		}

		public SetCASMailboxResponse SetCASMailbox(SetCASMailboxRequest request)
		{
			return new SetCASMailboxCommand(CallContext.Current, request).Execute();
		}

		public GetConnectedAccountsResponse GetConnectedAccounts(GetConnectedAccountsRequest request)
		{
			return new GetConnectedAccountsCommand(CallContext.Current, request).Execute();
		}

		public GetConnectSubscriptionResponse GetConnectSubscription(GetConnectSubscriptionRequest request)
		{
			return new GetConnectSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public NewConnectSubscriptionResponse NewConnectSubscription(NewConnectSubscriptionRequest request)
		{
			return new NewConnectSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public RemoveConnectSubscriptionResponse RemoveConnectSubscription(RemoveConnectSubscriptionRequest request)
		{
			return new RemoveConnectSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public SetConnectSubscriptionResponse SetConnectSubscription(SetConnectSubscriptionRequest request)
		{
			return new SetConnectSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public GetHotmailSubscriptionResponse GetHotmailSubscription(IdentityRequest request)
		{
			return new GetHotmailSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase SetHotmailSubscription(SetHotmailSubscriptionRequest request)
		{
			return new SetHotmailSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public GetImapSubscriptionResponse GetImapSubscription(IdentityRequest request)
		{
			return new GetImapSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public NewImapSubscriptionResponse NewImapSubscription(NewImapSubscriptionRequest request)
		{
			return new NewImapSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase SetImapSubscription(SetImapSubscriptionRequest request)
		{
			return new SetImapSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public ImportContactListResponse ImportContactList(ImportContactListRequest request)
		{
			return new ImportContactListCommand(CallContext.Current, request).Execute();
		}

		public DisableInboxRuleResponse DisableInboxRule(DisableInboxRuleRequest request)
		{
			return new DisableInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public EnableInboxRuleResponse EnableInboxRule(EnableInboxRuleRequest request)
		{
			return new EnableInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public GetInboxRuleResponse GetInboxRule(GetInboxRuleRequest request)
		{
			return new GetInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public NewInboxRuleResponse NewInboxRule(NewInboxRuleRequest request)
		{
			return new NewInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public RemoveInboxRuleResponse RemoveInboxRule(RemoveInboxRuleRequest request)
		{
			return new RemoveInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public SetInboxRuleResponse SetInboxRule(SetInboxRuleRequest request)
		{
			return new SetInboxRuleCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxResponse GetMailboxByIdentity(IdentityRequest request)
		{
			return new GetMailboxByIdentityCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase SetMailbox(SetMailboxRequest request)
		{
			return new SetMailboxCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxAutoReplyConfigurationResponse GetMailboxAutoReplyConfiguration()
		{
			return new GetMailboxAutoReplyConfigurationCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetMailboxAutoReplyConfiguration(SetMailboxAutoReplyConfigurationRequest request)
		{
			return new SetMailboxAutoReplyConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxCalendarConfigurationResponse GetMailboxCalendarConfiguration()
		{
			return new GetMailboxCalendarConfigurationCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetMailboxCalendarConfiguration(SetMailboxCalendarConfigurationRequest request)
		{
			return new SetMailboxCalendarConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxJunkEmailConfigurationResponse GetMailboxJunkEmailConfiguration()
		{
			return new GetMailboxJunkEmailConfigurationCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetMailboxJunkEmailConfiguration(SetMailboxJunkEmailConfigurationRequest request)
		{
			return new SetMailboxJunkEmailConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxMessageConfigurationResponse GetMailboxMessageConfiguration()
		{
			return new GetMailboxMessageConfigurationCommand(CallContext.Current).Execute();
		}

		public OptionsResponseBase SetMailboxMessageConfiguration(SetMailboxMessageConfigurationRequest request)
		{
			return new SetMailboxMessageConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetMailboxRegionalConfigurationResponse GetMailboxRegionalConfiguration(GetMailboxRegionalConfigurationRequest request)
		{
			return new GetMailboxRegionalConfigurationCommand(CallContext.Current, request).Execute();
		}

		public SetMailboxRegionalConfigurationResponse SetMailboxRegionalConfiguration(SetMailboxRegionalConfigurationRequest request)
		{
			return new SetMailboxRegionalConfigurationCommand(CallContext.Current, request).Execute();
		}

		public GetMessageCategoryResponse GetMessageCategory()
		{
			return new GetMessageCategoryCommand(CallContext.Current).Execute();
		}

		public GetMessageClassificationResponse GetMessageClassification()
		{
			return new GetMessageClassificationCommand(CallContext.Current).Execute();
		}

		public GetAccountInformationResponse GetAccountInformation(GetAccountInformationRequest request)
		{
			return new GetAccountInformationCommand(CallContext.Current, request).Execute();
		}

		public SetUserResponse SetUser(SetUserRequest request)
		{
			return new SetUserCommand(CallContext.Current, request).Execute();
		}

		public GetSocialNetworksOAuthInfoResponse GetConnectToSocialNetworksOAuthInfo(GetSocialNetworksOAuthInfoRequest request)
		{
			return new GetConnectToSocialNetworksOAuthInfoCommand(CallContext.Current, request).Execute();
		}

		public GetPopSubscriptionResponse GetPopSubscription(IdentityRequest request)
		{
			return new GetPopSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public NewPopSubscriptionResponse NewPopSubscription(NewPopSubscriptionRequest request)
		{
			return new NewPopSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase SetPopSubscription(SetPopSubscriptionRequest request)
		{
			return new SetPopSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase AddActiveRetentionPolicyTags(IdentityCollectionRequest request)
		{
			return new AddActiveRetentionPolicyTagsCommand(CallContext.Current, request).Execute();
		}

		public GetRetentionPolicyTagsResponse GetActiveRetentionPolicyTags(GetRetentionPolicyTagsRequest request)
		{
			return new GetActiveRetentionPolicyTagsCommand(CallContext.Current, request).Execute();
		}

		public GetRetentionPolicyTagsResponse GetAvailableRetentionPolicyTags(GetRetentionPolicyTagsRequest request)
		{
			return new GetAvailableRetentionPolicyTagsCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase RemoveActiveRetentionPolicyTags(IdentityCollectionRequest request)
		{
			return new RemoveActiveRetentionPolicyTagsCommand(CallContext.Current, request).Execute();
		}

		public GetSendAddressResponse GetSendAddress()
		{
			return new GetSendAddressCommand(CallContext.Current).Execute();
		}

		public GetSubscriptionResponse GetSubscription()
		{
			return new GetSubscriptionCommand(CallContext.Current).Execute();
		}

		public NewSubscriptionResponse NewSubscription(NewSubscriptionRequest request)
		{
			return new NewSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public OptionsResponseBase RemoveSubscription(IdentityRequest request)
		{
			return new RemoveSubscriptionCommand(CallContext.Current, request).Execute();
		}

		public LikeItemResponse LikeItem(LikeItemRequest request)
		{
			return new LikeItemCommand(CallContext.Current, request).Execute();
		}

		public GetLikersResponseMessage GetLikers(GetLikersRequest request)
		{
			return new GetLikers(CallContext.Current, request).Execute();
		}

		public GetAggregatedAccountResponse GetAggregatedAccount(GetAggregatedAccountRequest request)
		{
			GetAggregatedAccount getAggregatedAccount = new GetAggregatedAccount(CallContext.Current, request);
			if (getAggregatedAccount.PreExecute())
			{
				for (int i = 0; i < getAggregatedAccount.StepCount; i++)
				{
					CallContext.Current.Budget.CheckOverBudget();
					TaskExecuteResult taskExecuteResult = getAggregatedAccount.ExecuteStep();
					if (taskExecuteResult == TaskExecuteResult.ProcessingComplete)
					{
						break;
					}
				}
				return (GetAggregatedAccountResponse)getAggregatedAccount.PostExecute();
			}
			return null;
		}

		public AddAggregatedAccountResponse AddAggregatedAccount(AddAggregatedAccountRequest request)
		{
			AddAggregatedAccount addAggregatedAccount = new AddAggregatedAccount(CallContext.Current, request);
			if (addAggregatedAccount.PreExecute())
			{
				for (int i = 0; i < addAggregatedAccount.StepCount; i++)
				{
					CallContext.Current.Budget.CheckOverBudget();
					TaskExecuteResult taskExecuteResult = addAggregatedAccount.ExecuteStep();
					if (taskExecuteResult == TaskExecuteResult.ProcessingComplete)
					{
						break;
					}
				}
				return (AddAggregatedAccountResponse)addAggregatedAccount.PostExecute();
			}
			return null;
		}

		public IAsyncResult BeginCancelCalendarEvent(CancelCalendarEventJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CancelCalendarEventResponse>(asyncCallback, asyncState);
		}

		public CancelCalendarEventJsonResponse EndCancelCalendarEvent(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CancelCalendarEventJsonResponse, CancelCalendarEventResponse>(result, (CancelCalendarEventResponse body) => new CancelCalendarEventJsonResponse
			{
				Body = body
			});
		}

		public GetMobileDeviceStatisticsResponse GetMobileDeviceStatistics(GetMobileDeviceStatisticsRequest request)
		{
			return new GetMobileDeviceStatisticsCommand(CallContext.Current, request).Execute();
		}

		public RemoveMobileDeviceResponse RemoveMobileDevice(RemoveMobileDeviceRequest request)
		{
			return new RemoveMobileDeviceCommand(CallContext.Current, request).Execute();
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

		public ClearMobileDeviceResponse ClearMobileDevice(ClearMobileDeviceRequest request)
		{
			return new ClearMobileDeviceCommand(CallContext.Current, request).Execute();
		}

		public ClearTextMessagingAccountResponse ClearTextMessagingAccount(ClearTextMessagingAccountRequest request)
		{
			return new ClearTextMessagingAccountCommand(CallContext.Current, request).Execute();
		}

		public GetTextMessagingAccountResponse GetTextMessagingAccount(GetTextMessagingAccountRequest request)
		{
			return new GetTextMessagingAccountCommand(CallContext.Current, request).Execute();
		}

		public SetTextMessagingAccountResponse SetTextMessagingAccount(SetTextMessagingAccountRequest request)
		{
			return new SetTextMessagingAccountCommand(CallContext.Current, request).Execute();
		}

		public CompareTextMessagingVerificationCodeResponse CompareTextMessagingVerificationCode(CompareTextMessagingVerificationCodeRequest request)
		{
			return new CompareTextMessagingVerificationCodeCommand(CallContext.Current, request).Execute();
		}

		public SendTextMessagingVerificationCodeResponse SendTextMessagingVerificationCode(SendTextMessagingVerificationCodeRequest request)
		{
			return new SendTextMessagingVerificationCodeCommand(CallContext.Current, request).Execute();
		}

		public GetAllowedOptionsResponse GetAllowedOptions(GetAllowedOptionsRequest request)
		{
			return new GetAllowedOptionsCommand(CallContext.Current).Execute();
		}

		public IAsyncResult BeginConvertId(ConvertIdJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<ConvertIdResponse>(asyncCallback, asyncState);
		}

		public ConvertIdJsonResponse EndConvertId(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<ConvertIdJsonResponse, ConvertIdResponse>(result, (ConvertIdResponse body) => new ConvertIdJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUploadItems(UploadItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public UploadItemsJsonResponse EndUploadItems(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginExportItems(ExportItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public ExportItemsJsonResponse EndExportItems(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetFolder(GetFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetFolderResponse>(asyncCallback, asyncState);
		}

		public GetFolderJsonResponse EndGetFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetFolderJsonResponse, GetFolderResponse>(result, (GetFolderResponse body) => new GetFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateFolder(CreateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CreateFolderResponse>(asyncCallback, asyncState);
		}

		public CreateFolderJsonResponse EndCreateFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CreateFolderJsonResponse, CreateFolderResponse>(result, (CreateFolderResponse body) => new CreateFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteFolder(DeleteFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DeleteFolderResponse>(asyncCallback, asyncState);
		}

		public DeleteFolderJsonResponse EndDeleteFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DeleteFolderJsonResponse, DeleteFolderResponse>(result, (DeleteFolderResponse body) => new DeleteFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginEmptyFolder(EmptyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<EmptyFolderResponse>(asyncCallback, asyncState);
		}

		public EmptyFolderJsonResponse EndEmptyFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<EmptyFolderJsonResponse, EmptyFolderResponse>(result, (EmptyFolderResponse body) => new EmptyFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateFolder(UpdateFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UpdateFolderResponse>(asyncCallback, asyncState);
		}

		public UpdateFolderJsonResponse EndUpdateFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UpdateFolderJsonResponse, UpdateFolderResponse>(result, (UpdateFolderResponse body) => new UpdateFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMoveFolder(MoveFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<MoveFolderResponse>(asyncCallback, asyncState);
		}

		public MoveFolderJsonResponse EndMoveFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<MoveFolderJsonResponse, MoveFolderResponse>(result, (MoveFolderResponse body) => new MoveFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCopyFolder(CopyFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CopyFolderResponse>(asyncCallback, asyncState);
		}

		public CopyFolderJsonResponse EndCopyFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CopyFolderJsonResponse, CopyFolderResponse>(result, (CopyFolderResponse body) => new CopyFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindItem(FindItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			JsonService.AdjustMaxRowsIfNeeded(request.Body.Paging);
			return request.Body.ValidateAndSubmit<FindItemResponse>(asyncCallback, asyncState);
		}

		public FindItemJsonResponse EndFindItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<FindItemJsonResponse, FindItemResponse>(result, (FindItemResponse body) => new FindItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindFolder(FindFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<FindFolderResponse>(asyncCallback, asyncState);
		}

		public FindFolderJsonResponse EndFindFolder(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<FindFolderJsonResponse, FindFolderResponse>(result, (FindFolderResponse body) => new FindFolderJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetItem(GetItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetItemResponse>(asyncCallback, asyncState);
		}

		public GetItemJsonResponse EndGetItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetItemJsonResponse, GetItemResponse>(result, (GetItemResponse body) => new GetItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateItem(CreateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CreateItemResponse>(asyncCallback, asyncState);
		}

		public CreateItemJsonResponse EndCreateItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CreateItemJsonResponse, CreateItemResponse>(result, (CreateItemResponse body) => new CreateItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteItem(DeleteItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DeleteItemResponse>(asyncCallback, asyncState);
		}

		public DeleteItemJsonResponse EndDeleteItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DeleteItemJsonResponse, DeleteItemResponse>(result, (DeleteItemResponse body) => new DeleteItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateItem(UpdateItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UpdateItemResponse>(asyncCallback, asyncState);
		}

		public UpdateItemJsonResponse EndUpdateItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UpdateItemJsonResponse, UpdateItemResponse>(result, (UpdateItemResponse body) => new UpdateItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSendItem(SendItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SendItemResponse>(asyncCallback, asyncState);
		}

		public SendItemJsonResponse EndSendItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SendItemJsonResponse, SendItemResponse>(result, (SendItemResponse body) => new SendItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMoveItem(MoveItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<MoveItemResponse>(asyncCallback, asyncState);
		}

		public MoveItemJsonResponse EndMoveItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<MoveItemJsonResponse, MoveItemResponse>(result, (MoveItemResponse body) => new MoveItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCopyItem(CopyItemJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CopyItemResponse>(asyncCallback, asyncState);
		}

		public CopyItemJsonResponse EndCopyItem(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CopyItemJsonResponse, CopyItemResponse>(result, (CopyItemResponse body) => new CopyItemJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateAttachment(CreateAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CreateAttachmentResponse>(asyncCallback, asyncState);
		}

		public CreateAttachmentJsonResponse EndCreateAttachment(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CreateAttachmentJsonResponse, CreateAttachmentResponse>(result, (CreateAttachmentResponse body) => new CreateAttachmentJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteAttachment(DeleteAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DeleteAttachmentResponse>(asyncCallback, asyncState);
		}

		public DeleteAttachmentJsonResponse EndDeleteAttachment(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DeleteAttachmentJsonResponse, DeleteAttachmentResponse>(result, (DeleteAttachmentResponse body) => new DeleteAttachmentJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetAttachment(GetAttachmentJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetAttachmentResponse>(asyncCallback, asyncState);
		}

		public GetAttachmentJsonResponse EndGetAttachment(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetAttachmentJsonResponse, GetAttachmentResponse>(result, (GetAttachmentResponse body) => new GetAttachmentJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetClientAccessTokenResponse>(asyncCallback, asyncState);
		}

		public GetClientAccessTokenJsonResponse EndGetClientAccessToken(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetClientAccessTokenJsonResponse, GetClientAccessTokenResponse>(result, (GetClientAccessTokenResponse body) => new GetClientAccessTokenJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginResolveNames(ResolveNamesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<ResolveNamesResponse>(asyncCallback, asyncState);
		}

		public ResolveNamesJsonResponse EndResolveNames(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<ResolveNamesJsonResponse, ResolveNamesResponse>(result, (ResolveNamesResponse body) => new ResolveNamesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginExpandDL(ExpandDLJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public ExpandDLJsonResponse EndExpandDL(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetServerTimeZonesResponse>(asyncCallback, asyncState);
		}

		public GetServerTimeZonesJsonResponse EndGetServerTimeZones(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetServerTimeZonesJsonResponse, GetServerTimeZonesResponse>(result, (GetServerTimeZonesResponse body) => new GetServerTimeZonesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginCreateManagedFolder(CreateManagedFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public CreateManagedFolderJsonResponse EndCreateManagedFolder(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginSubscribe(SubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SubscribeResponse>(asyncCallback, asyncState);
		}

		public SubscribeJsonResponse EndSubscribe(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SubscribeJsonResponse, SubscribeResponse>(result, (SubscribeResponse body) => new SubscribeJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUnsubscribe(UnsubscribeJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UnsubscribeResponse>(asyncCallback, asyncState);
		}

		public UnsubscribeJsonResponse EndUnsubscribe(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UnsubscribeJsonResponse, UnsubscribeResponse>(result, (UnsubscribeResponse body) => new UnsubscribeJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetEvents(GetEventsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetEventsResponse>(asyncCallback, asyncState);
		}

		public GetEventsJsonResponse EndGetEvents(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetEventsJsonResponse, GetEventsResponse>(result, (GetEventsResponse body) => new GetEventsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchyJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SyncFolderHierarchyResponse>(asyncCallback, asyncState);
		}

		public SyncFolderHierarchyJsonResponse EndSyncFolderHierarchy(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SyncFolderHierarchyJsonResponse, SyncFolderHierarchyResponse>(result, (SyncFolderHierarchyResponse body) => new SyncFolderHierarchyJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncFolderItems(SyncFolderItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SyncFolderItemsResponse>(asyncCallback, asyncState);
		}

		public SyncFolderItemsJsonResponse EndSyncFolderItems(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SyncFolderItemsJsonResponse, SyncFolderItemsResponse>(result, (SyncFolderItemsResponse body) => new SyncFolderItemsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetDelegate(GetDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetDelegateJsonResponse EndGetDelegate(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginAddDelegate(AddDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public AddDelegateJsonResponse EndAddDelegate(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginRemoveDelegate(RemoveDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public RemoveDelegateJsonResponse EndRemoveDelegate(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginUpdateDelegate(UpdateDelegateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public UpdateDelegateJsonResponse EndUpdateDelegate(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<CreateUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public CreateUserConfigurationJsonResponse EndCreateUserConfiguration(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<CreateUserConfigurationJsonResponse, CreateUserConfigurationResponse>(result, (CreateUserConfigurationResponse body) => new CreateUserConfigurationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DeleteUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public DeleteUserConfigurationJsonResponse EndDeleteUserConfiguration(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DeleteUserConfigurationJsonResponse, DeleteUserConfigurationResponse>(result, (DeleteUserConfigurationResponse body) => new DeleteUserConfigurationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserConfiguration(GetUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public GetUserConfigurationJsonResponse EndGetUserConfiguration(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetUserConfigurationJsonResponse, GetUserConfigurationResponse>(result, (GetUserConfigurationResponse body) => new GetUserConfigurationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UpdateUserConfigurationResponse>(asyncCallback, asyncState);
		}

		public UpdateUserConfigurationJsonResponse EndUpdateUserConfiguration(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UpdateUserConfigurationJsonResponse, UpdateUserConfigurationResponse>(result, (UpdateUserConfigurationResponse body) => new UpdateUserConfigurationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetServiceConfigurationJsonResponse EndGetServiceConfiguration(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetMailTipsResponseMessage>(asyncCallback, asyncState);
		}

		public GetMailTipsJsonResponse EndGetMailTips(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetMailTipsJsonResponse, GetMailTipsResponseMessage>(result, (GetMailTipsResponseMessage body) => new GetMailTipsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPlayOnPhone(PlayOnPhoneJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<PlayOnPhoneResponseMessage>(asyncCallback, asyncState);
		}

		public PlayOnPhoneJsonResponse EndPlayOnPhone(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<PlayOnPhoneJsonResponse, PlayOnPhoneResponseMessage>(result, (PlayOnPhoneResponseMessage body) => new PlayOnPhoneJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetPhoneCallInformationResponseMessage>(asyncCallback, asyncState);
		}

		public GetPhoneCallInformationJsonResponse EndGetPhoneCallInformation(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetPhoneCallInformationJsonResponse, GetPhoneCallInformationResponseMessage>(result, (GetPhoneCallInformationResponseMessage body) => new GetPhoneCallInformationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DisconnectPhoneCallResponseMessage>(asyncCallback, asyncState);
		}

		public DisconnectPhoneCallJsonResponse EndDisconnectPhoneCall(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DisconnectPhoneCallJsonResponse, DisconnectPhoneCallResponseMessage>(result, (DisconnectPhoneCallResponseMessage body) => new DisconnectPhoneCallJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilityJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetUserAvailabilityResponse>(asyncCallback, asyncState);
		}

		public GetUserAvailabilityJsonResponse EndGetUserAvailability(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetUserAvailabilityJsonResponse, GetUserAvailabilityResponse>(result, (GetUserAvailabilityResponse body) => new GetUserAvailabilityJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetUserOofSettingsJsonResponse EndGetUserOofSettings(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public SetUserOofSettingsJsonResponse EndSetUserOofSettings(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetSharingMetadata(GetSharingMetadataJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetSharingMetadataJsonResponse EndGetSharingMetadata(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public RefreshSharingFolderJsonResponse EndRefreshSharingFolder(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetSharingFolder(GetSharingFolderJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetSharingFolderJsonResponse EndGetSharingFolder(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetReminders(GetRemindersJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetRemindersResponse>(asyncCallback, asyncState);
		}

		public GetRemindersJsonResponse EndGetReminders(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetRemindersJsonResponse, GetRemindersResponse>(result, (GetRemindersResponse body) => new GetRemindersJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginPerformReminderAction(PerformReminderActionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<PerformReminderActionResponse>(asyncCallback, asyncState);
		}

		public PerformReminderActionJsonResponse EndPerformReminderAction(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<PerformReminderActionJsonResponse, PerformReminderActionResponse>(result, (PerformReminderActionResponse body) => new PerformReminderActionJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetRoomLists(GetRoomListsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetRoomListsResponse>(asyncCallback, asyncState);
		}

		public GetRoomListsJsonResponse EndGetRoomLists(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetRoomListsJsonResponse, GetRoomListsResponse>(result, (GetRoomListsResponse body) => new GetRoomListsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetRooms(GetRoomsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetRoomsJsonResponse EndGetRooms(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public FindMessageTrackingReportJsonResponse EndFindMessageTrackingReport(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetMessageTrackingReportJsonResponse EndGetMessageTrackingReport(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginFindConversation(FindConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			JsonService.AdjustMaxRowsIfNeeded(request.Body.Paging);
			return request.Body.ValidateAndSubmit<FindConversationResponseMessage>(asyncCallback, asyncState);
		}

		public FindConversationJsonResponse EndFindConversation(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<FindConversationJsonResponse, FindConversationResponseMessage>(result, (FindConversationResponseMessage body) => new FindConversationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncConversation(SyncConversationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SyncConversationResponseMessage>(asyncCallback, asyncState);
		}

		public SyncConversationJsonResponse EndSyncConversation(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SyncConversationJsonResponse, SyncConversationResponseMessage>(result, (SyncConversationResponseMessage body) => new SyncConversationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginApplyConversationAction(ApplyConversationActionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<ApplyConversationActionResponse>(asyncCallback, asyncState);
		}

		public ApplyConversationActionJsonResponse EndApplyConversationAction(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<ApplyConversationActionJsonResponse, ApplyConversationActionResponse>(result, (ApplyConversationActionResponse body) => new ApplyConversationActionJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginFindPeople(FindPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<FindPeopleResponseMessage>(asyncCallback, asyncState);
		}

		public FindPeopleJsonResponse EndFindPeople(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<FindPeopleJsonResponse, FindPeopleResponseMessage>(result, (FindPeopleResponseMessage body) => new FindPeopleJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncAutoCompleteRecipients(SyncAutoCompleteRecipientsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SyncAutoCompleteRecipientsResponseMessage>(asyncCallback, asyncState);
		}

		public SyncAutoCompleteRecipientsJsonResponse EndSyncAutoCompleteRecipients(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SyncAutoCompleteRecipientsJsonResponse, SyncAutoCompleteRecipientsResponseMessage>(result, (SyncAutoCompleteRecipientsResponseMessage body) => new SyncAutoCompleteRecipientsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSyncPeople(SyncPeopleJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SyncPeopleResponseMessage>(asyncCallback, asyncState);
		}

		public SyncPeopleJsonResponse EndSyncPeople(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SyncPeopleJsonResponse, SyncPeopleResponseMessage>(result, (SyncPeopleResponseMessage body) => new SyncPeopleJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPersona(GetPersonaJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetPersonaResponseMessage>(asyncCallback, asyncState);
		}

		public GetPersonaJsonResponse EndGetPersona(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetPersonaJsonResponse, GetPersonaResponseMessage>(result, (GetPersonaResponseMessage body) => new GetPersonaJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetInboxRules(GetInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetInboxRulesJsonResponse EndGetInboxRules(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public UpdateInboxRulesJsonResponse EndUpdateInboxRules(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginExecuteDiagnosticMethod(ExecuteDiagnosticMethodJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public ExecuteDiagnosticMethodJsonResponse EndExecuteDiagnosticMethod(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public FindMailboxStatisticsByKeywordsJsonResponse EndFindMailboxStatisticsByKeywords(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetSearchableMailboxesResponse>(asyncCallback, asyncState);
		}

		public GetSearchableMailboxesJsonResponse EndGetSearchableMailboxes(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetSearchableMailboxesJsonResponse, GetSearchableMailboxesResponse>(result, (GetSearchableMailboxesResponse body) => new GetSearchableMailboxesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSearchMailboxes(SearchMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SearchMailboxesResponse>(asyncCallback, asyncState);
		}

		public SearchMailboxesJsonResponse EndSearchMailboxes(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SearchMailboxesJsonResponse, SearchMailboxesResponse>(result, (SearchMailboxesResponse body) => new SearchMailboxesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetHoldOnMailboxesResponse>(asyncCallback, asyncState);
		}

		public GetHoldOnMailboxesJsonResponse EndGetHoldOnMailboxes(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetHoldOnMailboxesJsonResponse, GetHoldOnMailboxesResponse>(result, (GetHoldOnMailboxesResponse body) => new GetHoldOnMailboxesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SetHoldOnMailboxesResponse>(asyncCallback, asyncState);
		}

		public SetHoldOnMailboxesJsonResponse EndSetHoldOnMailboxes(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SetHoldOnMailboxesJsonResponse, SetHoldOnMailboxesResponse>(result, (SetHoldOnMailboxesResponse body) => new SetHoldOnMailboxesJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetPasswordExpirationDateResponse>(asyncCallback, asyncState);
		}

		public GetPasswordExpirationDateJsonResponse EndGetPasswordExpirationDate(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetPasswordExpirationDateJsonResponse, GetPasswordExpirationDateResponse>(result, (GetPasswordExpirationDateResponse body) => new GetPasswordExpirationDateJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<MarkAllItemsAsReadResponse>(asyncCallback, asyncState);
		}

		public MarkAllItemsAsReadJsonResponse EndMarkAllItemsAsRead(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<MarkAllItemsAsReadJsonResponse, MarkAllItemsAsReadResponse>(result, (MarkAllItemsAsReadResponse body) => new MarkAllItemsAsReadJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginMarkAsJunk(MarkAsJunkJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<MarkAsJunkResponse>(asyncCallback, asyncState);
		}

		public MarkAsJunkJsonResponse EndMarkAsJunk(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<MarkAsJunkJsonResponse, MarkAsJunkResponse>(result, (MarkAsJunkResponse body) => new MarkAsJunkJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<AddDistributionGroupToImListResponseMessage>(asyncCallback, asyncState);
		}

		public AddDistributionGroupToImListJsonResponse EndAddDistributionGroupToImList(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<AddDistributionGroupToImListJsonResponse, AddDistributionGroupToImListResponseMessage>(result, (AddDistributionGroupToImListResponseMessage body) => new AddDistributionGroupToImListJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddImContactToGroup(AddImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<AddImContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddImContactToGroupJsonResponse EndAddImContactToGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<AddImContactToGroupJsonResponse, AddImContactToGroupResponseMessage>(result, (AddImContactToGroupResponseMessage body) => new AddImContactToGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<RemoveImContactFromGroupResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveImContactFromGroupJsonResponse EndRemoveImContactFromGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<RemoveImContactFromGroupJsonResponse, RemoveImContactFromGroupResponseMessage>(result, (RemoveImContactFromGroupResponseMessage body) => new RemoveImContactFromGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddImGroup(AddImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<AddImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddImGroupJsonResponse EndAddImGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<AddImGroupJsonResponse, AddImGroupResponseMessage>(result, (AddImGroupResponseMessage body) => new AddImGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<AddNewImContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddNewImContactToGroupJsonResponse EndAddNewImContactToGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<AddNewImContactToGroupJsonResponse, AddNewImContactToGroupResponseMessage>(result, (AddNewImContactToGroupResponseMessage body) => new AddNewImContactToGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<AddNewTelUriContactToGroupResponseMessage>(asyncCallback, asyncState);
		}

		public AddNewTelUriContactToGroupJsonResponse EndAddNewTelUriContactToGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<AddNewTelUriContactToGroupJsonResponse, AddNewTelUriContactToGroupResponseMessage>(result, (AddNewTelUriContactToGroupResponseMessage body) => new AddNewTelUriContactToGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetImItemList(GetImItemListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetImItemListResponseMessage>(asyncCallback, asyncState);
		}

		public GetImItemListJsonResponse EndGetImItemList(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetImItemListJsonResponse, GetImItemListResponseMessage>(result, (GetImItemListResponseMessage body) => new GetImItemListJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetImItems(GetImItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetImItemsResponseMessage>(asyncCallback, asyncState);
		}

		public GetImItemsJsonResponse EndGetImItems(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetImItemsJsonResponse, GetImItemsResponseMessage>(result, (GetImItemsResponseMessage body) => new GetImItemsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<RemoveContactFromImListResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveContactFromImListJsonResponse EndRemoveContactFromImList(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<RemoveContactFromImListJsonResponse, RemoveContactFromImListResponseMessage>(result, (RemoveContactFromImListResponseMessage body) => new RemoveContactFromImListJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<RemoveDistributionGroupFromImListResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveDistributionGroupFromImListJsonResponse EndRemoveDistributionGroupFromImList(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<RemoveDistributionGroupFromImListJsonResponse, RemoveDistributionGroupFromImListResponseMessage>(result, (RemoveDistributionGroupFromImListResponseMessage body) => new RemoveDistributionGroupFromImListJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRemoveImGroup(RemoveImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<RemoveImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public RemoveImGroupJsonResponse EndRemoveImGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<RemoveImGroupJsonResponse, RemoveImGroupResponseMessage>(result, (RemoveImGroupResponseMessage body) => new RemoveImGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetImGroup(SetImGroupJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SetImGroupResponseMessage>(asyncCallback, asyncState);
		}

		public SetImGroupJsonResponse EndSetImGroup(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SetImGroupJsonResponse, SetImGroupResponseMessage>(result, (SetImGroupResponseMessage body) => new SetImGroupJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SetImListMigrationCompletedResponseMessage>(asyncCallback, asyncState);
		}

		public SetImListMigrationCompletedJsonResponse EndSetImListMigrationCompleted(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SetImListMigrationCompletedJsonResponse, SetImListMigrationCompletedResponseMessage>(result, (SetImListMigrationCompletedResponseMessage body) => new SetImListMigrationCompletedJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetConversationItems(GetConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetConversationItemsResponse>(asyncCallback, asyncState);
		}

		public GetConversationItemsJsonResponse EndGetConversationItems(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetConversationItemsJsonResponse, GetConversationItemsResponse>(result, (GetConversationItemsResponse body) => new GetConversationItemsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetThreadedConversationItems(GetThreadedConversationItemsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetThreadedConversationItemsResponse>(asyncCallback, asyncState);
		}

		public GetThreadedConversationItemsJsonResponse EndGetThreadedConversationItems(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetThreadedConversationItemsJsonResponse, GetThreadedConversationItemsResponse>(result, (GetThreadedConversationItemsResponse body) => new GetThreadedConversationItemsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetConversationItemsDiagnostics(GetConversationItemsDiagnosticsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetConversationItemsDiagnosticsResponse>(asyncCallback, asyncState);
		}

		public GetConversationItemsDiagnosticsJsonResponse EndGetConversationItemsDiagnostics(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetConversationItemsDiagnosticsJsonResponse, GetConversationItemsDiagnosticsResponse>(result, (GetConversationItemsDiagnosticsResponse body) => new GetConversationItemsDiagnosticsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetUserRetentionPolicyTagsResponse>(asyncCallback, asyncState);
		}

		public GetUserRetentionPolicyTagsJsonResponse EndGetUserRetentionPolicyTags(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetUserRetentionPolicyTagsJsonResponse, GetUserRetentionPolicyTagsResponse>(result, (GetUserRetentionPolicyTagsResponse body) => new GetUserRetentionPolicyTagsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginProvision(ProvisionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<ProvisionResponse>(asyncCallback, asyncState);
		}

		public ProvisionJsonResponse EndProvision(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<ProvisionJsonResponse, ProvisionResponse>(result, (ProvisionResponse body) => new ProvisionJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetTimeZoneOffsets(GetTimeZoneOffsetsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetTimeZoneOffsetsResponseMessage>(asyncCallback, asyncState);
		}

		public GetTimeZoneOffsetsJsonResponse EndGetTimeZoneOffsets(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetTimeZoneOffsetsJsonResponse, GetTimeZoneOffsetsResponseMessage>(result, (GetTimeZoneOffsetsResponseMessage body) => new GetTimeZoneOffsetsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginDeprovision(DeprovisionJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<DeprovisionResponse>(asyncCallback, asyncState);
		}

		public DeprovisionJsonResponse EndDeprovision(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<DeprovisionJsonResponse, DeprovisionResponse>(result, (DeprovisionResponse body) => new DeprovisionJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginLogPushNotificationData(LogPushNotificationDataJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<LogPushNotificationDataResponse>(asyncCallback, asyncState);
		}

		public LogPushNotificationDataJsonResponse EndLogPushNotificationData(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<LogPushNotificationDataJsonResponse, LogPushNotificationDataResponse>(result, (LogPushNotificationDataResponse body) => new LogPushNotificationDataJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserUnifiedGroups(GetUserUnifiedGroupsJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetUserUnifiedGroupsResponseMessage>(asyncCallback, asyncState);
		}

		public GetUserUnifiedGroupsJsonResponse EndGetUserUnifiedGroups(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetUserUnifiedGroupsJsonResponse, GetUserUnifiedGroupsResponseMessage>(result, (GetUserUnifiedGroupsResponseMessage body) => new GetUserUnifiedGroupsJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetBirthdayCalendarView(GetBirthdayCalendarViewJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetBirthdayCalendarViewResponseMessage>(asyncCallback, asyncState);
		}

		public GetBirthdayCalendarViewJsonResponse EndGetBirthdayCalendarView(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetBirthdayCalendarViewJsonResponse, GetBirthdayCalendarViewResponseMessage>(result, (GetBirthdayCalendarViewResponseMessage body) => new GetBirthdayCalendarViewJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetClutterState(GetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<GetClutterStateResponse>(asyncCallback, asyncState);
		}

		public GetClutterStateJsonResponse EndGetClutterState(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<GetClutterStateJsonResponse, GetClutterStateResponse>(result, (GetClutterStateResponse body) => new GetClutterStateJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginSetClutterState(SetClutterStateJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SetClutterStateResponse>(asyncCallback, asyncState);
		}

		public SetClutterStateJsonResponse EndSetClutterState(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SetClutterStateJsonResponse, SetClutterStateResponse>(result, (SetClutterStateResponse body) => new SetClutterStateJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginGetUserPhoto(string email, UserPhotoSize size, bool isPreview, bool fallbackToClearImage, AsyncCallback callback, object state)
		{
			return new GetUserPhotoRequest(CallContext.Current.CreateWebResponseContext(), email, size, isPreview, fallbackToClearImage).ValidateAndSubmit<GetUserPhotoResponse>(callback, state);
		}

		public Stream EndGetUserPhoto(IAsyncResult result)
		{
			ServiceAsyncResult<GetUserPhotoResponse> serviceAsyncResult = (ServiceAsyncResult<GetUserPhotoResponse>)result;
			if (serviceAsyncResult.Data != null && serviceAsyncResult.Data.ResponseMessages.Items != null && serviceAsyncResult.Data.ResponseMessages.Items.Length > 0)
			{
				return ((GetUserPhotoResponseMessage)serviceAsyncResult.Data.ResponseMessages.Items[0]).UserPhotoStream;
			}
			IOutgoingWebResponseContext outgoingWebResponseContext = CallContext.Current.CreateWebResponseContext();
			outgoingWebResponseContext.StatusCode = HttpStatusCode.InternalServerError;
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
			IOutgoingWebResponseContext outgoingWebResponseContext = CallContext.Current.CreateWebResponseContext();
			outgoingWebResponseContext.StatusCode = HttpStatusCode.InternalServerError;
			return new MemoryStream();
		}

		public IAsyncResult BeginSubscribeToPushNotification(SubscribeToPushNotificationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<SubscribeToPushNotificationResponse>(asyncCallback, asyncState);
		}

		public SubscribeToPushNotificationJsonResponse EndSubscribeToPushNotification(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<SubscribeToPushNotificationJsonResponse, SubscribeToPushNotificationResponse>(result, (SubscribeToPushNotificationResponse body) => new SubscribeToPushNotificationJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginRequestDeviceRegistrationChallenge(RequestDeviceRegistrationChallengeJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<RequestDeviceRegistrationChallengeResponseMessage>(asyncCallback, asyncState);
		}

		public RequestDeviceRegistrationChallengeJsonResponse EndRequestDeviceRegistrationChallenge(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<RequestDeviceRegistrationChallengeJsonResponse, RequestDeviceRegistrationChallengeResponseMessage>(result, (RequestDeviceRegistrationChallengeResponseMessage body) => new RequestDeviceRegistrationChallengeJsonResponse
			{
				Body = body
			});
		}

		public IAsyncResult BeginUnsubscribeToPushNotification(UnsubscribeToPushNotificationJsonRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return request.Body.ValidateAndSubmit<UnsubscribeToPushNotificationResponse>(asyncCallback, asyncState);
		}

		public UnsubscribeToPushNotificationJsonResponse EndUnsubscribeToPushNotification(IAsyncResult result)
		{
			return JsonService.CreateJsonResponse<UnsubscribeToPushNotificationJsonResponse, UnsubscribeToPushNotificationResponse>(result, (UnsubscribeToPushNotificationResponse body) => new UnsubscribeToPushNotificationJsonResponse
			{
				Body = body
			});
		}

		private static TJsonResponse CreateJsonResponse<TJsonResponse, TJsonResponseBody>(IAsyncResult result, Func<TJsonResponseBody, TJsonResponse> createJsonResponseCallback)
		{
			bool flag = false;
			if (CallContext.Current.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(CallContext.Current.AccessingPrincipal.LegacyDn))
			{
				flag = true;
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
			TJsonResponse result2;
			try
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Entering End web method for {0}", CallContext.Current.Description);
				ServiceAsyncResult<TJsonResponseBody> serviceAsyncResult = JsonService.GetServiceAsyncResult<TJsonResponseBody>(result);
				TJsonResponse tjsonResponse = createJsonResponseCallback(serviceAsyncResult.Data);
				PerformanceMonitor.UpdateTotalCompletedRequestsCount();
				result2 = tjsonResponse;
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

		private static ServiceAsyncResult<TJsonResponseBody> GetServiceAsyncResult<TJsonResponseBody>(IAsyncResult result)
		{
			ServiceAsyncResult<TJsonResponseBody> serviceAsyncResult = (ServiceAsyncResult<TJsonResponseBody>)result;
			Exception ex = serviceAsyncResult.CompletionState as Exception;
			if (ex != null)
			{
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
				if (!JsonService.IsServiceHandledException(ex2))
				{
					throw new InternalServerErrorException(ex2);
				}
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
				exceptionDispatchInfo.Throw();
			}
			return serviceAsyncResult;
		}

		private static bool IsServiceHandledException(Exception exception)
		{
			return exception is BailOutException || exception is FaultException;
		}

		private static void AdjustMaxRowsIfNeeded(BasePagingType paging)
		{
			if (paging != null && !(paging is CalendarPageView))
			{
				paging.MaxRows = Math.Min(paging.MaxRows, 200);
			}
		}
	}
}
