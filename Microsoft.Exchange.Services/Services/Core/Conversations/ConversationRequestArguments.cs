using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Conversations
{
	internal sealed class ConversationRequestArguments
	{
		public ConversationRequestArguments(int maxItemsToReturn, bool returnSubmittedItems, ConversationNodeSortOrder sortOrder)
		{
			this.SortOrder = ConversationRequestArguments.ConvertEwstoXsoSortOrder(sortOrder);
			this.ReturnSubmittedItems = returnSubmittedItems;
			this.MaxItemsToReturn = maxItemsToReturn;
		}

		public int MaxItemsToReturn { get; private set; }

		public bool ReturnSubmittedItems { get; private set; }

		public ConversationTreeSortOrder SortOrder { get; private set; }

		public bool IsNewestOnTop
		{
			get
			{
				return this.SortOrder == ConversationTreeSortOrder.ChronologicalDescending || this.SortOrder == ConversationTreeSortOrder.DeepTraversalDescending || this.SortOrder == ConversationTreeSortOrder.TraversalChronologicalDescending;
			}
		}

		private static ConversationTreeSortOrder ConvertEwstoXsoSortOrder(ConversationNodeSortOrder sortOrder)
		{
			if (sortOrder != ConversationNodeSortOrder.DateOrderDescending)
			{
				return ConversationTreeSortOrder.TraversalChronologicalAscending;
			}
			return ConversationTreeSortOrder.TraversalChronologicalDescending;
		}
	}
}
