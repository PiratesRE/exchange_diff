using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Conversations.Repositories;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetConversationItems : MultiStepServiceCommand<GetConversationItemsRequest, Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>
	{
		public GetConversationItems(CallContext callContext, GetConversationItemsRequest request) : base(callContext, request)
		{
			this.foldersToIgnore = base.Request.FoldersToIgnore;
			this.conversations = base.Request.Conversations;
			this.sortOrder = base.Request.SortOrder;
			this.maxItemsToReturn = base.Request.MaxItemsToReturn;
			this.mailboxScope = base.Request.MailboxScope;
		}

		private static PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				if (GetConversationItems.propertiesToLoad == null)
				{
					List<PropertyDefinition> list = new List<PropertyDefinition>(ConversationLoaderHelper.MandatoryConversationPropertiesToLoad);
					list.AddRange(ConversationLoaderHelper.NonMandatoryPropertiesToLoad);
					GetConversationItems.propertiesToLoad = list.ToArray();
				}
				return GetConversationItems.propertiesToLoad;
			}
		}

		private static PropertyDefinition[] InferenceEnabledPropertiesToLoad
		{
			get
			{
				if (GetConversationItems.inferenceEnabledPropertiesToLoad == null)
				{
					GetConversationItems.inferenceEnabledPropertiesToLoad = ConversationLoaderHelper.CalculateInferenceEnabledPropertiesToLoad(GetConversationItems.PropertiesToLoad);
				}
				return GetConversationItems.inferenceEnabledPropertiesToLoad;
			}
		}

		internal override int StepCount
		{
			get
			{
				return this.conversations.Length;
			}
		}

		internal Microsoft.Exchange.Services.Core.Types.ConversationRequestType CurrentConversationRequest
		{
			get
			{
				return this.conversations[base.CurrentStep];
			}
		}

		private ItemResponseShape ItemResponseShape
		{
			get
			{
				if (this.itemResponseShape == null)
				{
					this.itemResponseShape = this.GetResponseShape();
				}
				return this.itemResponseShape;
			}
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			if (this.maxItemsToReturn < 0 || this.maxItemsToReturn > 100)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.InvalidMaxItemsToReturn);
			}
			this.conversationStatisticsLogger = new ConversationStatisticsLogger(base.CallContext.ProtocolLog);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetConversationItemsResponse getConversationItemsResponse = new GetConversationItemsResponse();
			getConversationItemsResponse.BuildForResults<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(base.Results);
			return getConversationItemsResponse;
		}

		internal override ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> Execute()
		{
			ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> serviceResult = null;
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				Microsoft.Exchange.Services.Core.Types.ConversationRequestType currentConversationRequest = this.CurrentConversationRequest;
				this.conversationStatisticsLogger.Log(currentConversationRequest);
				IdAndSession sessionFromConversationId = this.GetSessionFromConversationId(currentConversationRequest.ConversationId);
				ConversationId conversationId = sessionFromConversationId.Id as ConversationId;
				MailboxSession mailboxSession = (MailboxSession)sessionFromConversationId.Session;
				ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> serviceResult;
				if (!this.Request.MailboxScopeSpecified || this.Request.MailboxScope != MailboxSearchLocation.All)
				{
					serviceResult = this.LoadAndBuildConversation(conversationId, currentConversationRequest.SyncState, mailboxSession);
					return;
				}
				if (mailboxSession.MailboxOwner.GetArchiveMailbox() == null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorArchiveMailboxNotEnabled);
				}
				GetConversationItems.ExecuteArchiveGetConversationItemsDelegate executeArchiveGetConversationItemsDelegate = new GetConversationItems.ExecuteArchiveGetConversationItemsDelegate(this.LoadAndBuildArchiveConversation);
				IAsyncResult result = executeArchiveGetConversationItemsDelegate.BeginInvoke(conversationId, currentConversationRequest.SyncState, mailboxSession.MailboxOwner, null, null);
				serviceResult = this.LoadAndBuildConversation(conversationId, currentConversationRequest.SyncState, mailboxSession);
				ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> serviceResult2 = executeArchiveGetConversationItemsDelegate.EndInvoke(result);
				if (serviceResult != null && serviceResult.Value != null && serviceResult2 != null && serviceResult2.Value != null)
				{
					ConversationNode[] conversationNodes = serviceResult.Value.ConversationNodes;
					ConversationNode[] conversationNodes2 = serviceResult2.Value.ConversationNodes;
					serviceResult = new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType
					{
						ConversationId = serviceResult.Value.ConversationId,
						ConversationNodes = ConversationNodeFactory.MergeConversationNodes(conversationNodes, conversationNodes2, this.sortOrder, this.maxItemsToReturn),
						SyncState = serviceResult.Value.SyncState
					});
					return;
				}
				if (serviceResult != null && serviceResult.Value != null)
				{
					serviceResult = serviceResult;
					return;
				}
				serviceResult = serviceResult2;
			}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			return serviceResult;
		}

		internal static bool CanOptimizeAsSingleNodeTree(ICoreConversation conversation, byte[] currentSyncState, ItemResponseShape itemResponseShape, bool returnSubmittedItems)
		{
			return (itemResponseShape.UseSafeHtml || !string.IsNullOrEmpty(itemResponseShape.CssScopeClassName)) && (!conversation.ConversationTree.RootMessageNode.HasBeenSubmitted || returnSubmittedItems) && conversation.ConversationTree.Count == 1 && (currentSyncState == null || currentSyncState.Length == 0);
		}

		private static PropertyDefinition[] GetPropertiesForConversationLoad(ItemResponseShape itemResponseShape)
		{
			if (!itemResponseShape.InferenceEnabled)
			{
				return GetConversationItems.PropertiesToLoad;
			}
			return GetConversationItems.InferenceEnabledPropertiesToLoad;
		}

		private static ICollection<IConversationTreeNode> CalculateChangedTreeNodes(IConversationTree conversationTree, List<StoreObjectId> changedItems)
		{
			HashSet<IConversationTreeNode> hashSet = new HashSet<IConversationTreeNode>();
			foreach (StoreObjectId storeObjectId in changedItems)
			{
				IConversationTreeNode item;
				if (conversationTree.TryGetConversationTreeNode(storeObjectId, out item))
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		private static List<StoreObjectId> CalculateItemChanges(ICoreConversation conversation, byte[] syncState)
		{
			KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> keyValuePair = conversation.CalculateChanges(syncState);
			List<StoreObjectId> list = new List<StoreObjectId>();
			list.AddRange(keyValuePair.Key);
			list.AddRange(keyValuePair.Value);
			return list;
		}

		private ItemResponseShape GetResponseShape()
		{
			return Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(base.Request.ShapeName, base.Request.ItemShape, base.CallContext.FeaturesManager);
		}

		protected override IParticipantResolver ConstructParticipantResolver()
		{
			return Microsoft.Exchange.Services.Core.Types.ParticipantResolver.Create(base.CallContext, this.ItemResponseShape.MaximumRecipientsToReturn);
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> LoadAndBuildConversation(ConversationId conversationId, byte[] currentSyncState, MailboxSession mailboxSession)
		{
			ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> result;
			try
			{
				this.conversationLoader = new XsoConversationRepository<Conversation>(this.ItemResponseShape, GetConversationItems.GetPropertiesForConversationLoad(this.ItemResponseShape), base.IdConverter, new ConversationFactory(mailboxSession), base.CallContext, base.ParticipantResolver);
				Conversation conversation = this.conversationLoader.Load(conversationId, mailboxSession, this.foldersToIgnore, true, true, new PropertyDefinition[0]);
				if (conversation == null)
				{
					throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
				}
				conversation.TrimToNewest(this.maxItemsToReturn);
				EWSSettings.CurrentConversation = conversation;
				ConversationNode[] conversationNodes;
				if (GetConversationItems.CanOptimizeAsSingleNodeTree(conversation, currentSyncState, this.ItemResponseShape, base.Request.ReturnSubmittedItems))
				{
					ConversationNodeFactory conversationNodeFactory = this.CreateConversationNodeFactory(mailboxSession, conversation);
					ConversationNode conversationNode = conversationNodeFactory.BuildConversationNodeFromSingleNodeTree();
					conversationNodes = new ConversationNode[]
					{
						conversationNode
					};
				}
				else
				{
					conversationNodes = this.BuildConversationNodes(conversation, currentSyncState, mailboxSession);
				}
				this.conversationStatisticsLogger.Log(conversation.ConversationStatistics);
				result = new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType
				{
					ConversationId = this.CurrentConversationRequest.ConversationId,
					ConversationNodes = conversationNodes,
					SyncState = conversation.SerializedTreeState,
					TotalConversationNodesCount = conversation.ConversationTree.Count
				});
			}
			finally
			{
				EWSSettings.CurrentConversation = null;
			}
			return result;
		}

		private ConversationNode[] BuildConversationNodes(Conversation conversation, byte[] syncState, MailboxSession mailboxSession)
		{
			IConversationTree conversationTree = conversation.ConversationTree;
			if (conversationTree == null || conversationTree.Count == 0)
			{
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
			}
			conversationTree.Sort((ConversationTreeSortOrder)this.sortOrder);
			Func<StoreObjectId, bool> returnOnlyMandatoryProperties;
			if (syncState != null && syncState.Length > 0)
			{
				List<StoreObjectId> itemsToRender = GetConversationItems.CalculateItemChanges(conversation, syncState);
				if (itemsToRender.Count > 0)
				{
					this.PrefetchItems(mailboxSession, itemsToRender);
				}
				ICollection<IConversationTreeNode> collection = GetConversationItems.CalculateChangedTreeNodes(conversationTree, itemsToRender);
				this.conversationLoader.LoadItemParts(conversation, collection, false);
				base.ParticipantResolver.LoadAdDataIfNeeded(conversation.AllParticipants(collection));
				returnOnlyMandatoryProperties = ((StoreObjectId objectId) => !itemsToRender.Contains(objectId));
			}
			else
			{
				this.PrefetchItems(mailboxSession, conversation.GetMessageIdsForPreread());
				this.conversationLoader.LoadItemParts(conversation, conversationTree, false);
				base.ParticipantResolver.LoadAdDataIfNeeded(conversation.AllParticipants(null));
				returnOnlyMandatoryProperties = ((StoreObjectId objectId) => false);
			}
			List<ConversationNode> list = this.BuildConversationNodes(mailboxSession, conversation, returnOnlyMandatoryProperties);
			if (list != null)
			{
				return list.ToArray();
			}
			return null;
		}

		private List<ConversationNode> BuildConversationNodes(MailboxSession mailboxSession, ICoreConversation conversation, Func<StoreObjectId, bool> returnOnlyMandatoryProperties)
		{
			List<ConversationNode> list = new List<ConversationNode>();
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(4043713853U, conversation.Topic);
			ConversationNodeFactory conversationNodeFactory = this.CreateConversationNodeFactory(mailboxSession, conversation);
			foreach (IConversationTreeNode treeNode in conversation.ConversationTree)
			{
				if (list.Count >= this.maxItemsToReturn)
				{
					break;
				}
				ConversationNode conversationNode = conversationNodeFactory.CreateInstance(treeNode, returnOnlyMandatoryProperties);
				if (conversationNode.ItemCount != 0)
				{
					list.Add(conversationNode);
				}
			}
			return list;
		}

		private ConversationNodeFactory CreateConversationNodeFactory(MailboxSession mailboxSession, ICoreConversation conversation)
		{
			return new ConversationNodeFactory(mailboxSession, conversation, base.ParticipantResolver, this.ItemResponseShape, base.Request.ReturnSubmittedItems, ConversationLoaderHelper.MandatoryConversationPropertiesToLoad, GetConversationItems.GetPropertiesForConversationLoad(this.ItemResponseShape), this.conversationLoader.PropertiesLoaded, this.conversationLoader.PropertiesLoadedPerItem, !string.IsNullOrEmpty(base.Request.ShapeName));
		}

		private void PrefetchItems(MailboxSession mailboxSession, List<StoreObjectId> itemIds)
		{
			mailboxSession.PrereadMessages(itemIds.ToArray());
		}

		private IdAndSession GetSessionFromConversationId(BaseItemId conversationId)
		{
			IdAndSession idAndSession = base.IdConverter.ConvertConversationIdToIdAndSession(conversationId, this.mailboxScope == MailboxSearchLocation.ArchiveOnly);
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationSupportedOnlyForMailboxSession);
			}
			return idAndSession;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> LoadAndBuildArchiveConversation(ConversationId conversationId, byte[] currentSyncState, IExchangePrincipal primaryPrincipal)
		{
			ExchangeServiceBinding archiveServiceBinding = EwsClientHelper.GetArchiveServiceBinding(base.CallContext.EffectiveCaller, primaryPrincipal);
			if (archiveServiceBinding != null)
			{
				return this.LoadAndBuildRemoteConversation(archiveServiceBinding);
			}
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new ServiceError((CoreResources.IDs)3156121664U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorArchiveMailboxServiceDiscoveryFailed, 0, ExchangeVersion.Exchange2012));
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> LoadAndBuildRemoteConversation(ExchangeServiceBinding serviceBinding)
		{
			GetConversationItemsRequest source = new GetConversationItemsRequest
			{
				Conversations = new Microsoft.Exchange.Services.Core.Types.ConversationRequestType[]
				{
					this.CurrentConversationRequest
				},
				FoldersToIgnore = base.Request.FoldersToIgnore,
				ItemShape = this.GetResponseShape(),
				MaxItemsToReturn = base.Request.MaxItemsToReturn,
				SortOrder = base.Request.SortOrder,
				MailboxScope = MailboxSearchLocation.ArchiveOnly
			};
			GetConversationItemsType getConversationItemsType = EwsClientHelper.Convert<GetConversationItemsRequest, GetConversationItemsType>(source);
			Exception ex = null;
			GetConversationItemsResponseType getConversationItemsResponseType = null;
			bool flag = EwsClientHelper.ExecuteEwsCall(delegate
			{
				getConversationItemsResponseType = serviceBinding.GetConversationItems(getConversationItemsType);
			}, out ex);
			if (!flag || getConversationItemsResponseType.ResponseMessages == null || getConversationItemsResponseType.ResponseMessages.Items == null || getConversationItemsResponseType.ResponseMessages.Items.Length <= 0)
			{
				return new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new ServiceError((CoreResources.IDs)3668888236U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012));
			}
			GetConversationItemsResponseMessageType source2 = (GetConversationItemsResponseMessageType)getConversationItemsResponseType.ResponseMessages.Items[0];
			GetConversationItemsResponseMessage getConversationItemsResponseMessage = EwsClientHelper.Convert<GetConversationItemsResponseMessageType, GetConversationItemsResponseMessage>(source2);
			if (getConversationItemsResponseMessage.ResponseClass == ResponseClass.Success)
			{
				return new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType
				{
					ConversationId = getConversationItemsResponseMessage.Conversation.ConversationId,
					ConversationNodes = getConversationItemsResponseMessage.Conversation.ConversationNodes,
					SyncState = getConversationItemsResponseMessage.Conversation.SyncState
				});
			}
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType>(new ServiceError((CoreResources.IDs)3668888236U, getConversationItemsResponseMessage.ResponseCode, getConversationItemsResponseMessage.DescriptiveLinkKey, ExchangeVersion.Exchange2012));
		}

		private const int MaxItemsToReturnLimit = 100;

		private static PropertyDefinition[] propertiesToLoad;

		private static PropertyDefinition[] inferenceEnabledPropertiesToLoad;

		private readonly int maxItemsToReturn;

		private ItemResponseShape itemResponseShape;

		private Microsoft.Exchange.Services.Core.Types.ConversationRequestType[] conversations;

		private BaseFolderId[] foldersToIgnore;

		private XsoConversationRepository<Conversation> conversationLoader;

		private Microsoft.Exchange.Services.Core.Types.ConversationNodeSortOrder sortOrder;

		private MailboxSearchLocation mailboxScope;

		private ConversationStatisticsLogger conversationStatisticsLogger;

		private delegate ServiceResult<Microsoft.Exchange.Services.Core.Types.Conversations.ConversationResponseType> ExecuteArchiveGetConversationItemsDelegate(ConversationId conversationId, byte[] currentSyncState, IExchangePrincipal primaryPrincipal);
	}
}
