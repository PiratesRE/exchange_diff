using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeNodeFactory
	{
		public ConversationTreeNodeFactory() : this(ConversationTreeNodeFactory.DefaultTreeNodeIndexPropertyDefinition)
		{
		}

		public ConversationTreeNodeFactory(PropertyDefinition indexPropertyDefinition)
		{
			this.indexPropertyDefinition = indexPropertyDefinition;
		}

		public ConversationTreeNode CreateInstance(List<IStorePropertyBag> storePropertyBags)
		{
			return new ConversationTreeNode(this.indexPropertyDefinition, storePropertyBags, TraversalChronologicalNodeSorter.Instance);
		}

		public ConversationTreeRootNode CreateRootNode()
		{
			return new ConversationTreeRootNode(TraversalChronologicalNodeSorter.Instance);
		}

		public static PropertyDefinition DefaultTreeNodeIndexPropertyDefinition = ItemSchema.ConversationIndex;

		public static PropertyDefinition ConversationFamilyTreeNodeIndexPropertyDefinition = ItemSchema.ConversationFamilyIndex;

		private readonly PropertyDefinition indexPropertyDefinition;
	}
}
