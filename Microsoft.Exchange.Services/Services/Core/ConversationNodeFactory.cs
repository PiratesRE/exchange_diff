using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ConversationNodeFactory : ConversationNodeFactoryBase<ConversationNode>
	{
		public ConversationNodeFactory(MailboxSession mailboxSession, ICoreConversation conversation, IParticipantResolver participantsResolver, ItemResponseShape itemResponse, bool createdNodeFromSubmittedItems, ICollection<PropertyDefinition> mandatoryPropertiesToLoad, ICollection<PropertyDefinition> conversationPropertiesToLoad, HashSet<PropertyDefinition> propertiesLoaded, Dictionary<StoreObjectId, HashSet<PropertyDefinition>> propertiesLoadedPerItem, bool isOwaCall) : base(mailboxSession, conversation, participantsResolver, itemResponse, mandatoryPropertiesToLoad, conversationPropertiesToLoad, propertiesLoaded, propertiesLoadedPerItem, isOwaCall)
		{
			this.createdNodeFromSubmittedItems = createdNodeFromSubmittedItems;
		}

		private bool ShouldPopulateItem(IConversationTreeNode node)
		{
			return this.createdNodeFromSubmittedItems || !node.HasBeenSubmitted;
		}

		protected override void PopulateConversationNodeComplexProperties(ConversationNode conversationNode, IConversationTreeNode treeNode, Func<StoreObjectId, bool> returnOnlyMandatoryProperties)
		{
			if (this.ShouldPopulateItem(treeNode))
			{
				base.PopulateConversationNodeComplexProperties(conversationNode, treeNode, returnOnlyMandatoryProperties);
			}
		}

		protected override ConversationNode CreateEmptyInstance()
		{
			return new ConversationNode();
		}

		public static ConversationNode[] MergeConversationNodes(ConversationNode[] conversationNodesX, ConversationNode[] conversationNodesY, ConversationNodeSortOrder sortOrder, int maxItemsToReturn)
		{
			IEnumerable<ConversationNode> enumerable;
			if (conversationNodesY != null)
			{
				if (conversationNodesX == null)
				{
					enumerable = conversationNodesY;
				}
				else
				{
					enumerable = conversationNodesX.Union(conversationNodesY, ConversationHelper.ConversationNodeEqualityComparer).OrderBy((ConversationNode x) => x, new ConversationNodeComparer(sortOrder)).Take(maxItemsToReturn);
					ConversationNode[] array = enumerable.ToArray<ConversationNode>();
					for (int i = 0; i < array.Length; i++)
					{
						if (string.IsNullOrEmpty(array[i].ParentInternetMessageId) && i + 1 < array.Length)
						{
							array[i].ParentInternetMessageId = array[i + 1].ParentInternetMessageId;
						}
					}
					enumerable = array;
				}
			}
			else
			{
				enumerable = conversationNodesX;
			}
			if (enumerable != null)
			{
				return enumerable.ToArray<ConversationNode>();
			}
			return null;
		}

		private readonly bool createdNodeFromSubmittedItems;
	}
}
