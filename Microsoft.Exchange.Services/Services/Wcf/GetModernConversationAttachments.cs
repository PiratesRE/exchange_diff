using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Conversations.Repositories;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetModernConversationAttachments : MultiStepServiceCommand<GetModernConversationAttachmentsRequest, ModernConversationAttachmentsResponseType>
	{
		public GetModernConversationAttachments(CallContext callContext, GetModernConversationAttachmentsRequest request) : base(callContext, request)
		{
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.Conversations.Length;
			}
		}

		private static PropertyDefinition[] MandatoryPropertiesToLoad
		{
			get
			{
				if (GetModernConversationAttachments.mandatoryPropertiesToLoad == null)
				{
					GetModernConversationAttachments.mandatoryPropertiesToLoad = new List<PropertyDefinition>(ConversationLoaderHelper.MandatoryConversationPropertiesToLoad)
					{
						MessageItemSchema.ReplyToBlobExists,
						MessageItemSchema.ReplyToNamesExists
					}.ToArray();
				}
				return GetModernConversationAttachments.mandatoryPropertiesToLoad;
			}
		}

		private static PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				if (GetModernConversationAttachments.propertiesToLoad == null)
				{
					List<PropertyDefinition> list = new List<PropertyDefinition>(GetModernConversationAttachments.MandatoryPropertiesToLoad);
					list.AddRange(ConversationLoaderHelper.NonMandatoryPropertiesToLoad);
					list.AddRange(ConversationLoaderHelper.InReplyToPropertiesToLoad);
					GetModernConversationAttachments.propertiesToLoad = list.ToArray();
				}
				return GetModernConversationAttachments.propertiesToLoad;
			}
		}

		private ItemResponseShape GetResponseShape()
		{
			return new ItemResponseShape
			{
				AdditionalProperties = new List<PropertyPath>
				{
					new PropertyUri(PropertyUriEnum.Attachments),
					new PropertyUri(PropertyUriEnum.From)
				}.ToArray(),
				ClientSupportsIrm = base.Request.ClientSupportsIrm
			};
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.itemResponseShape = this.GetResponseShape();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetModernConversationAttachmentsResponse getModernConversationAttachmentsResponse = new GetModernConversationAttachmentsResponse();
			getModernConversationAttachmentsResponse.BuildForResults<ModernConversationAttachmentsResponseType>(base.Results);
			return getModernConversationAttachmentsResponse;
		}

		internal ConversationRequestType CurrentConversationRequest
		{
			get
			{
				return base.Request.Conversations[base.CurrentStep];
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

		protected override IParticipantResolver ConstructParticipantResolver()
		{
			return Microsoft.Exchange.Services.Core.Types.ParticipantResolver.Create(base.CallContext, this.ItemResponseShape.MaximumRecipientsToReturn);
		}

		internal override ServiceResult<ModernConversationAttachmentsResponseType> Execute()
		{
			ServiceResult<ModernConversationAttachmentsResponseType> serviceResult = null;
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				ConversationRequestType currentConversationRequest = this.CurrentConversationRequest;
				IdAndSession sessionFromConversationId = XsoConversationRepositoryExtensions.GetSessionFromConversationId(this.IdConverter, currentConversationRequest.ConversationId, MailboxSearchLocation.PrimaryOnly);
				ConversationId conversationId = sessionFromConversationId.Id as ConversationId;
				MailboxSession mailboxSession = (MailboxSession)sessionFromConversationId.Session;
				serviceResult = this.ExtractAttachmentsFromConversation(conversationId, currentConversationRequest.SyncState, mailboxSession);
			}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			return serviceResult;
		}

		private ServiceResult<ModernConversationAttachmentsResponseType> ExtractAttachmentsFromConversation(ConversationId conversationId, byte[] currentSyncState, MailboxSession mailboxSession)
		{
			ServiceResult<ModernConversationAttachmentsResponseType> result;
			try
			{
				XsoConversationRepository<Conversation> xsoConversationRepository = new XsoConversationRepository<Conversation>(this.ItemResponseShape, GetModernConversationAttachments.PropertiesToLoad, base.IdConverter, new ConversationFactory(mailboxSession), base.ParticipantResolver);
				ICoreConversation coreConversation = xsoConversationRepository.Load(conversationId, mailboxSession, base.Request.FoldersToIgnore, true, false, new PropertyDefinition[0]);
				if (coreConversation == null || coreConversation.ConversationTree == null || coreConversation.ConversationTree.Count == 0)
				{
					throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
				}
				EWSSettings.CurrentConversation = coreConversation;
				List<IConversationTreeNode> onlyNodesWithAttachments = this.GetOnlyNodesWithAttachments(XsoConversationRepositoryExtensions.GetTreeNodes(coreConversation, currentSyncState));
				if (onlyNodesWithAttachments.Count > 0)
				{
					coreConversation.OnBeforeItemLoad += this.ConversationOnBeforeItemLoad;
					this.LoadItemParts(coreConversation, onlyNodesWithAttachments, mailboxSession, xsoConversationRepository);
				}
				List<IConversationTreeNode> onlyNodesWithAttachments2 = this.GetOnlyNodesWithAttachments(coreConversation.ConversationTree.ToList<IConversationTreeNode>());
				List<ItemType> list;
				if (onlyNodesWithAttachments2.Count > 0)
				{
					list = this.BuildAttachmentItems(coreConversation, mailboxSession, onlyNodesWithAttachments, onlyNodesWithAttachments2);
				}
				else
				{
					list = new List<ItemType>(0);
				}
				ModernConversationAttachmentsResponseType value = new ModernConversationAttachmentsResponseType
				{
					ConversationId = this.CurrentConversationRequest.ConversationId,
					SyncState = coreConversation.SerializedTreeState,
					ItemsWithAttachments = list.ToArray()
				};
				result = new ServiceResult<ModernConversationAttachmentsResponseType>(value);
			}
			finally
			{
				EWSSettings.CurrentConversation = null;
			}
			return result;
		}

		private void LoadItemParts(ICoreConversation conversation, List<IConversationTreeNode> relevantNodes, MailboxSession mailboxSession, XsoConversationRepository<Conversation> conversationLoader)
		{
			List<StoreObjectId> listStoreObjectIds = XsoConversationRepositoryExtensions.GetListStoreObjectIds(relevantNodes);
			this.PrefetchItems(mailboxSession, listStoreObjectIds);
			conversationLoader.LoadItemParts(conversation, relevantNodes, true);
		}

		private void PrefetchItems(MailboxSession mailboxSession, List<StoreObjectId> itemIds)
		{
			mailboxSession.PrereadMessages(itemIds.ToArray());
		}

		private List<IConversationTreeNode> GetOnlyNodesWithAttachments(IEnumerable<IConversationTreeNode> relevantNodes)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>();
			foreach (IConversationTreeNode conversationTreeNode in relevantNodes)
			{
				if (conversationTreeNode.HasAttachments)
				{
					list.Add(conversationTreeNode);
				}
			}
			return list;
		}

		private List<ItemType> BuildAttachmentItems(ICoreConversation conversation, MailboxSession mailboxSession, List<IConversationTreeNode> nodesToLoad, List<IConversationTreeNode> nodesWithAttachments)
		{
			List<ItemType> list = new List<ItemType>();
			HashSet<IConversationTreeNode> hashSet = new HashSet<IConversationTreeNode>(nodesToLoad);
			base.ParticipantResolver.LoadAdDataIfNeeded(conversation.AllParticipants(nodesToLoad));
			ConversationTreeNodeBase.SortByDate(nodesWithAttachments);
			foreach (IConversationTreeNode conversationTreeNode in nodesWithAttachments)
			{
				list.AddRange(this.CreateItemsFromTreeNode(conversationTreeNode, conversation, mailboxSession, hashSet.Contains(conversationTreeNode)));
			}
			return list;
		}

		public ItemType[] CreateItemsFromTreeNode(IConversationTreeNode treeNode, ICoreConversation conversation, MailboxSession mailboxSession, bool getItemPart)
		{
			List<ItemType> list = new List<ItemType>(treeNode.StorePropertyBags.Count);
			foreach (IStorePropertyBag storePropertyBag in treeNode.StorePropertyBags)
			{
				VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
				StoreObjectId objectId = versionedId.ObjectId;
				IdAndSession itemIdAndSession = new IdAndSession(objectId, mailboxSession);
				ItemType itemType = ItemType.CreateFromStoreObjectType(objectId.ObjectType);
				itemType.ItemClass = storePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
				itemType.ItemId = IdConverter.GetItemIdFromStoreId(objectId, new MailboxId(mailboxSession));
				if (getItemPart)
				{
					ItemPart itemPart = conversation.GetItemPart(objectId);
					itemType.PropertyBag[ItemSchema.DateTimeSent] = ConversationDataConverter.GetDatetimeProperty(itemPart, ItemSchema.SentTime);
					itemType.PropertyBag[MessageSchema.From] = base.ParticipantResolver.ResolveToSingleRecipientType(itemPart.StorePropertyBag.GetValueOrDefault<IParticipant>(ItemSchema.From, null));
					itemType.HasAttachments = new bool?(itemPart.StorePropertyBag.GetValueOrDefault<bool>(ItemSchema.HasAttachment, false));
					itemType.Attachments = ConversationDataConverter.GetAttachments(itemPart, itemIdAndSession);
				}
				else
				{
					itemType.HasAttachments = new bool?(true);
				}
				if (itemType is MessageType)
				{
					list.Add(itemType);
				}
			}
			return list.ToArray();
		}

		private void ConversationOnBeforeItemLoad(object sender, LoadItemEventArgs eventArgs)
		{
			eventArgs.HtmlStreamOptionCallback = null;
			eventArgs.MessagePropertyDefinitions = new List<PropertyDefinition>
			{
				MessageItemSchema.ReplyToBlob,
				MessageItemSchema.ReplyToNames
			}.ToArray<PropertyDefinition>();
			eventArgs.OpportunisticLoadPropertyDefinitions = null;
		}

		private static PropertyDefinition[] propertiesToLoad;

		private static PropertyDefinition[] mandatoryPropertiesToLoad;

		private ItemResponseShape itemResponseShape;
	}
}
