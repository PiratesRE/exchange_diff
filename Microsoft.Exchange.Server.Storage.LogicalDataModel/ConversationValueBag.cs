using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationValueBag : IColumnValueBag
	{
		public ConversationValueBag(ConversationItem conversationItem, ExchangeId folderId, ConversationMembers conversationMembers)
		{
			this.conversationItem = conversationItem;
			this.folderId = folderId;
			this.conversationMembers = conversationMembers;
		}

		public ConversationValueBag(ConversationItem conversationItem, ICollection<FidMid> originalMembersFilterList, ICollection<FidMid> membersFilterList, ConversationMembers conversationMembers)
		{
			this.conversationItem = conversationItem;
			this.originalMembersFilterList = originalMembersFilterList;
			this.membersFilterList = membersFilterList;
			this.conversationMembers = conversationMembers;
		}

		public object GetColumnValue(Context context, Column column)
		{
			object result;
			if (column is PropertyColumn && ConversationMembers.IsAggregateProperty(((PropertyColumn)column).StorePropTag))
			{
				StorePropTag storePropTag = ((PropertyColumn)column).StorePropTag;
				result = this.GetAggregateProperty(context, storePropTag);
			}
			else
			{
				result = this.conversationItem.GetColumnValue(context, column);
			}
			return result;
		}

		public object GetOriginalColumnValue(Context context, Column column)
		{
			object result;
			if (column is PropertyColumn && ConversationMembers.IsAggregateProperty(((PropertyColumn)column).StorePropTag))
			{
				StorePropTag storePropTag = ((PropertyColumn)column).StorePropTag;
				result = this.GetOriginalAggregateProperty(context, storePropTag);
			}
			else
			{
				result = this.conversationItem.GetOriginalColumnValue(context, column);
			}
			return result;
		}

		public bool IsColumnChanged(Context context, Column column)
		{
			return 0 != ValueHelper.ValuesCompare(this.GetColumnValue(context, column), this.GetOriginalColumnValue(context, column));
		}

		public void SetInstanceNumber(Context context, object instanceNumber)
		{
			this.conversationItem.SetInstanceNumber(context, instanceNumber);
		}

		private object GetAggregateProperty(Context context, StorePropTag proptag)
		{
			object aggregateProperty;
			if (this.cachedAggregateProperties == null || !this.cachedAggregateProperties.TryGetValue(proptag, out aggregateProperty))
			{
				if (this.folderId.IsValid)
				{
					aggregateProperty = this.conversationMembers.GetAggregateProperty(context, proptag, this.folderId, false);
				}
				else
				{
					aggregateProperty = this.conversationMembers.GetAggregateProperty(context, proptag, this.membersFilterList, false);
				}
				if (this.cachedAggregateProperties == null)
				{
					this.cachedAggregateProperties = new Dictionary<StorePropTag, object>(30);
				}
				this.cachedAggregateProperties.Add(proptag, aggregateProperty);
			}
			return aggregateProperty;
		}

		private object GetOriginalAggregateProperty(Context context, StorePropTag proptag)
		{
			object aggregateProperty;
			if (this.cachedOriginalAggregateProperties == null || !this.cachedOriginalAggregateProperties.TryGetValue(proptag, out aggregateProperty))
			{
				if (this.folderId.IsValid)
				{
					aggregateProperty = this.conversationMembers.GetAggregateProperty(context, proptag, this.folderId, true);
				}
				else
				{
					aggregateProperty = this.conversationMembers.GetAggregateProperty(context, proptag, this.originalMembersFilterList, true);
				}
				if (this.cachedOriginalAggregateProperties == null)
				{
					this.cachedOriginalAggregateProperties = new Dictionary<StorePropTag, object>(30);
				}
				this.cachedOriginalAggregateProperties.Add(proptag, aggregateProperty);
			}
			return aggregateProperty;
		}

		private readonly ConversationItem conversationItem;

		private readonly ExchangeId folderId;

		private readonly ICollection<FidMid> originalMembersFilterList;

		private readonly ICollection<FidMid> membersFilterList;

		private readonly ConversationMembers conversationMembers;

		private Dictionary<StorePropTag, object> cachedAggregateProperties;

		private Dictionary<StorePropTag, object> cachedOriginalAggregateProperties;
	}
}
