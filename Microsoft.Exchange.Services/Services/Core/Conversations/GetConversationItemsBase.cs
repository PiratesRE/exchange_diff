using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders;
using Microsoft.Exchange.Services.Core.Conversations.Repositories;
using Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Conversations
{
	internal abstract class GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType> : MultiStepServiceCommand<TRequestType, TSingleItemType> where TConversationType : ICoreConversation where TRequestType : GetConversationItemsRequest where TSingleItemType : new()
	{
		protected GetConversationItemsBase(CallContext callContext, TRequestType request) : base(callContext, request)
		{
		}

		private static PropertyDefinition[] MandatoryPropertiesToLoad
		{
			get
			{
				if (GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.mandatoryPropertiesToLoad == null)
				{
					List<PropertyDefinition> list = new List<PropertyDefinition>(ConversationLoaderHelper.MandatoryConversationPropertiesToLoad);
					list.AddRange(ConversationLoaderHelper.ModernConversationMandatoryPropertiesToLoad);
					GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.mandatoryPropertiesToLoad = list.ToArray();
				}
				return GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.mandatoryPropertiesToLoad;
			}
		}

		protected override IParticipantResolver ConstructParticipantResolver()
		{
			return Microsoft.Exchange.Services.Core.Types.ParticipantResolver.Create(base.CallContext, this.ItemResponseShape.MaximumRecipientsToReturn);
		}

		internal override int StepCount
		{
			get
			{
				TRequestType request = base.Request;
				return request.Conversations.Length;
			}
		}

		protected virtual ServiceResult<TSingleItemType>[] InternalResults
		{
			get
			{
				return base.Results;
			}
		}

		protected virtual PropertyDefinition[] AdditionalRequestedProperties
		{
			get
			{
				return new PropertyDefinition[0];
			}
		}

		protected abstract int MaxItemsToReturn { get; }

		private ItemResponseShape GetResponseShape()
		{
			IResponseShapeResolver responseShapeResolver = Global.ResponseShapeResolver;
			TRequestType request = base.Request;
			string shapeName = request.ShapeName;
			TRequestType request2 = base.Request;
			return responseShapeResolver.GetResponseShape<ItemResponseShape>(shapeName, request2.ItemShape, base.CallContext.FeaturesManager);
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.conversationStatisticsLogger = new ConversationStatisticsLogger(base.CallContext.ProtocolLog);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return this.GetResponseInternal();
		}

		internal ConversationRequestType CurrentConversationRequest
		{
			get
			{
				TRequestType request = base.Request;
				return request.Conversations[base.CurrentStep];
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

		private static PropertyDefinition[] GetPropertiesForConversationLoad(ItemResponseShape itemResponseShape)
		{
			PropertyDefinition[] propertiesToLoad = GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.GetPropertiesToLoad(itemResponseShape);
			if (!itemResponseShape.InferenceEnabled)
			{
				return propertiesToLoad;
			}
			return ConversationLoaderHelper.CalculateInferenceEnabledPropertiesToLoad(propertiesToLoad);
		}

		private static PropertyDefinition[] GetPropertiesToLoad(ItemResponseShape itemResponseShape)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.MandatoryPropertiesToLoad);
			list.AddRange(ConversationLoaderHelper.NonMandatoryPropertiesToLoad);
			list.AddRange(ConversationLoaderHelper.InReplyToPropertiesToLoad);
			if (itemResponseShape.AdditionalProperties != null && itemResponseShape.AdditionalProperties.Any<PropertyPath>())
			{
				foreach (PropertyInformation propertyInformation in ConversationLoaderHelper.ModernConversationOptionalPropertiesToLoad)
				{
					if (itemResponseShape.AdditionalProperties.Contains(propertyInformation.PropertyPath))
					{
						list.AddRange(propertyInformation.GetPropertyDefinitions(null));
					}
				}
			}
			return list.ToArray();
		}

		internal override ServiceResult<TSingleItemType> Execute()
		{
			int maxItemsToReturn = this.MaxItemsToReturn;
			TRequestType request = base.Request;
			bool returnSubmittedItems = request.ReturnSubmittedItems;
			TRequestType request2 = base.Request;
			ConversationRequestArguments conversationRequestArguments = new ConversationRequestArguments(maxItemsToReturn, returnSubmittedItems, request2.SortOrder);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug((long)this.GetHashCode(), "GetConversationItemsBase.Execute: Requesting conversation. ConversationId: {0}, MaxItemsToReturn: {1}, ReturnSubmittedItems: {2}, SortOrder: {3}", new object[]
			{
				this.CurrentConversationRequest.ConversationId,
				conversationRequestArguments.MaxItemsToReturn,
				conversationRequestArguments.ReturnSubmittedItems,
				conversationRequestArguments.SortOrder
			});
			ConversationRequestType currentConversationRequest = this.CurrentConversationRequest;
			byte[] syncState = currentConversationRequest.SyncState;
			IdAndSession sessionFromConversationId = XsoConversationRepositoryExtensions.GetSessionFromConversationId(base.IdConverter, currentConversationRequest.ConversationId, MailboxSearchLocation.PrimaryOnly);
			ConversationId conversationId = sessionFromConversationId.Id as ConversationId;
			MailboxSession mailboxSession = (MailboxSession)sessionFromConversationId.Session;
			XsoConversationRepository<TConversationType> xsoConversationRepository = new XsoConversationRepository<TConversationType>(this.ItemResponseShape, GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.GetPropertiesForConversationLoad(this.ItemResponseShape), base.IdConverter, this.CreateConversationFactory(mailboxSession), base.CallContext, base.ParticipantResolver);
			bool useFolderIdsAsExclusionList = !mailboxSession.IsGroupMailbox();
			BaseFolderId[] array;
			if (!mailboxSession.IsGroupMailbox())
			{
				TRequestType request3 = base.Request;
				array = request3.FoldersToIgnore;
			}
			else
			{
				array = this.GetGroupFoldersToLoadFrom(mailboxSession);
			}
			BaseFolderId[] folderIds = array;
			TConversationType tconversationType = xsoConversationRepository.Load(conversationId, mailboxSession, folderIds, useFolderIdsAsExclusionList, true, this.AdditionalRequestedProperties);
			if (tconversationType == null || tconversationType.ConversationTree == null || tconversationType.ConversationTree.Count == 0)
			{
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
			}
			ConversationNodeLoadingList conversationNodeLoadingList = this.CalculateNodesToLoadFromXso(tconversationType, syncState, conversationRequestArguments);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "GetConversationItemsBase.Execute: LoadingList calculated. Items to be loaded: {0}, Items to be ignored: {1}, Items to be skipped: {2}", conversationNodeLoadingList.ToBeLoaded.Count<IConversationTreeNode>(), conversationNodeLoadingList.ToBeIgnored.Count<IConversationTreeNode>(), conversationNodeLoadingList.NotToBeLoaded.Count<IConversationTreeNode>());
			this.LoadConversationFromXso(syncState, mailboxSession, conversationNodeLoadingList, tconversationType, xsoConversationRepository);
			TSingleItemType value = this.ConvertXsoConversationToEwsConversation(mailboxSession, xsoConversationRepository, tconversationType, conversationNodeLoadingList, conversationRequestArguments);
			this.conversationStatisticsLogger.Log(currentConversationRequest);
			this.conversationStatisticsLogger.Log(tconversationType.ConversationStatistics);
			return new ServiceResult<TSingleItemType>(value);
		}

		protected abstract IExchangeWebMethodResponse GetResponseInternal();

		protected abstract ICoreConversationFactory<TConversationType> CreateConversationFactory(IMailboxSession mailboxSession);

		protected abstract ConversationNodeLoadingListBuilderBase CreateConversationNodeLoadingListBuilder(TConversationType conversation, ConversationRequestArguments requestArguments, List<IConversationTreeNode> nonSyncedNodes);

		protected abstract ConversationResponseBuilderBase<TSingleItemType> CreateBuilder(IMailboxSession mailboxSession, TConversationType conversation, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments, ModernConversationNodeFactory conversationNodeFactory);

		private ConversationNodeLoadingList CalculateNodesToLoadFromXso(TConversationType conversation, byte[] currentSyncState, ConversationRequestArguments requestArguments)
		{
			List<IConversationTreeNode> treeNodes = XsoConversationRepositoryExtensions.GetTreeNodes(conversation, currentSyncState);
			ConversationNodeLoadingListBuilderBase conversationNodeLoadingListBuilderBase = this.CreateConversationNodeLoadingListBuilder(conversation, requestArguments, treeNodes);
			return conversationNodeLoadingListBuilderBase.Build();
		}

		private void LoadConversationFromXso(byte[] currentSyncState, MailboxSession mailboxSession, ConversationNodeLoadingList loadingList, ICoreConversation conversation, IConversationRepository<TConversationType> conversationRepository)
		{
			bool flag = XsoConversationRepositoryExtensions.IsValidSyncState(currentSyncState);
			HashSet<IConversationTreeNode> hashSet = new HashSet<IConversationTreeNode>(loadingList.ToBeLoaded);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "GetConversationItemsBase.LoadConversationFromXso: Loading nodes from XSO. Nodes to load: {0}, Has sync state: {1}", hashSet.Count, flag);
			conversationRepository.PrefetchAndLoadItemParts(mailboxSession, conversation, hashSet, flag);
		}

		private TSingleItemType ConvertXsoConversationToEwsConversation(MailboxSession mailboxSession, XsoConversationRepository<TConversationType> conversationRepository, TConversationType conversation, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments)
		{
			ModernConversationNodeFactory conversationNodeFactory = this.CreateConversationNodeFactory(mailboxSession, conversationRepository, conversation, new HashSet<IConversationTreeNode>(loadingList.ToBeLoaded, ConversationTreeNodeBase.EqualityComparer));
			ConversationResponseBuilderBase<TSingleItemType> conversationResponseBuilderBase = this.CreateBuilder(mailboxSession, conversation, loadingList, requestArguments, conversationNodeFactory);
			return conversationResponseBuilderBase.Build();
		}

		private BaseFolderId[] GetGroupFoldersToLoadFrom(MailboxSession mailboxSession)
		{
			DistinguishedFolderId distinguishedFolderId = new DistinguishedFolderId
			{
				Id = DistinguishedFolderIdName.inbox,
				Mailbox = new EmailAddressWrapper
				{
					EmailAddress = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
					RoutingType = "SMTP"
				}
			};
			return new BaseFolderId[]
			{
				distinguishedFolderId
			};
		}

		private ModernConversationNodeFactory CreateConversationNodeFactory(MailboxSession mailboxSession, XsoConversationRepository<TConversationType> conversationRepository, ICoreConversation conversation, HashSet<IConversationTreeNode> itemsToBeFullyLoaded)
		{
			IParticipantResolver participantResolver = base.ParticipantResolver;
			ItemResponseShape itemResponse = this.ItemResponseShape;
			ICollection<PropertyDefinition> collection = GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.MandatoryPropertiesToLoad;
			ICollection<PropertyDefinition> propertiesForConversationLoad = GetConversationItemsBase<TConversationType, TRequestType, TSingleItemType>.GetPropertiesForConversationLoad(this.ItemResponseShape);
			HashSet<PropertyDefinition> propertiesLoaded = conversationRepository.PropertiesLoaded;
			Dictionary<StoreObjectId, HashSet<PropertyDefinition>> propertiesLoadedPerItem = conversationRepository.PropertiesLoadedPerItem;
			TRequestType request = base.Request;
			return new ModernConversationNodeFactory(mailboxSession, conversation, participantResolver, itemResponse, collection, propertiesForConversationLoad, propertiesLoaded, propertiesLoadedPerItem, itemsToBeFullyLoaded, !string.IsNullOrEmpty(request.ShapeName));
		}

		private static PropertyDefinition[] mandatoryPropertiesToLoad;

		private ItemResponseShape itemResponseShape;

		private ConversationStatisticsLogger conversationStatisticsLogger;
	}
}
