using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ModernConversationNodeFactory : ConversationNodeFactoryBase<ConversationNode>, IModernConversationNodeFactory
	{
		public ModernConversationNodeFactory(MailboxSession mailboxSession, ICoreConversation conversation, IParticipantResolver participantResolver, ItemResponseShape itemResponse, ICollection<PropertyDefinition> mandatoryPropertiesToLoad, ICollection<PropertyDefinition> conversationPropertiesToLoad, HashSet<PropertyDefinition> propertiesLoaded, Dictionary<StoreObjectId, HashSet<PropertyDefinition>> propertiesLoadedPerItem, IEnumerable<IConversationTreeNode> itemsToBeFullyLoaded, bool isOwaCall) : base(mailboxSession, conversation, participantResolver, itemResponse, mandatoryPropertiesToLoad, conversationPropertiesToLoad, propertiesLoaded, propertiesLoadedPerItem, isOwaCall)
		{
			this.itemsToBeFullyLoaded = itemsToBeFullyLoaded;
		}

		public ConversationNode CreateInstance(IConversationTreeNode treeNode)
		{
			return base.CreateInstance(treeNode, new Func<StoreObjectId, bool>(this.ShouldReturnOnlyMandatoryProperties));
		}

		public bool TryLoadRelatedItemInfo(IConversationTreeNode treeNode, out IRelatedItemInfo relatedItem)
		{
			relatedItem = null;
			return treeNode.HasData && this.TryLoadRelatedItemInfo(treeNode.MainPropertyBag, out relatedItem);
		}

		public bool TryLoadRelatedItemInfo(IStorePropertyBag storePropertyBag, out IRelatedItemInfo relatedItem)
		{
			VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
			StoreObjectId objectId = versionedId.ObjectId;
			ItemType itemType = null;
			relatedItem = null;
			if (base.TryLoadServiceItemType(objectId, storePropertyBag, false, out itemType))
			{
				relatedItem = (itemType as IRelatedItemInfo);
				return relatedItem != null;
			}
			return false;
		}

		protected override ConversationNode CreateEmptyInstance()
		{
			return new ConversationNode();
		}

		private bool ShouldReturnOnlyMandatoryProperties(StoreObjectId id)
		{
			return !this.itemsToBeFullyLoaded.Any((IConversationTreeNode node) => node.IsPartOf(id));
		}

		private readonly IEnumerable<IConversationTreeNode> itemsToBeFullyLoaded;
	}
}
