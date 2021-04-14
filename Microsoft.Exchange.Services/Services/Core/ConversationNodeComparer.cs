using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ConversationNodeComparer : IComparer<ConversationNode>
	{
		public ConversationNodeComparer(ConversationNodeSortOrder sortOrder)
		{
			this.sortOrder = sortOrder;
		}

		public int Compare(ConversationNode x, ConversationNode y)
		{
			string value = null;
			string value2 = null;
			switch (this.sortOrder)
			{
			case ConversationNodeSortOrder.DateOrderAscending:
				value = x.Items.OrderBy((ItemType i) => i.DateTimeReceived, ConversationHelper.DateTimeStringComparer).FirstOrDefault<ItemType>().DateTimeReceived;
				value2 = y.Items.OrderBy((ItemType i) => i.DateTimeReceived, ConversationHelper.DateTimeStringComparer).FirstOrDefault<ItemType>().DateTimeReceived;
				break;
			case ConversationNodeSortOrder.DateOrderDescending:
				value2 = x.Items.OrderBy((ItemType i) => i.DateTimeReceived, ConversationHelper.DateTimeStringComparer).LastOrDefault<ItemType>().DateTimeReceived;
				value = y.Items.OrderBy((ItemType i) => i.DateTimeReceived, ConversationHelper.DateTimeStringComparer).LastOrDefault<ItemType>().DateTimeReceived;
				break;
			}
			if (string.IsNullOrEmpty(value))
			{
				value = DateTime.MinValue.ToString();
			}
			if (string.IsNullOrEmpty(value2))
			{
				value2 = DateTime.MinValue.ToString();
			}
			return Convert.ToDateTime(value).CompareTo(Convert.ToDateTime(value2));
		}

		private ConversationNodeSortOrder sortOrder;
	}
}
