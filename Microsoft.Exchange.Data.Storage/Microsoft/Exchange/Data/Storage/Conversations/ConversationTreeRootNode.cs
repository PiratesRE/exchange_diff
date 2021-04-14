using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeRootNode : ConversationTreeNodeBase
	{
		internal ConversationTreeRootNode(IConversationTreeNodeSorter conversationTreeNodeSorter) : base(conversationTreeNodeSorter)
		{
		}

		public override bool HasData
		{
			get
			{
				return false;
			}
		}

		public override List<IStorePropertyBag> StorePropertyBags
		{
			get
			{
				return new List<IStorePropertyBag>();
			}
		}

		public override bool UpdatePropertyBag(StoreObjectId itemId, IStorePropertyBag bag)
		{
			return false;
		}

		public override bool HasAttachments
		{
			get
			{
				return false;
			}
		}

		public override bool HasBeenSubmitted
		{
			get
			{
				return false;
			}
		}

		public override bool IsSpecificMessageReplyStamped
		{
			get
			{
				return false;
			}
		}

		public override bool IsSpecificMessageReply
		{
			get
			{
				return false;
			}
		}

		public override ConversationId ConversationId
		{
			get
			{
				return null;
			}
		}

		public override ConversationId ConversationThreadId
		{
			get
			{
				return null;
			}
		}

		public override byte[] Index
		{
			get
			{
				return null;
			}
		}

		public override StoreObjectId MainStoreObjectId
		{
			get
			{
				return null;
			}
		}

		public override ExDateTime? ReceivedTime
		{
			get
			{
				return null;
			}
		}

		public override string ItemClass
		{
			get
			{
				return string.Empty;
			}
		}

		public override List<StoreObjectId> ToListStoreObjectId()
		{
			return new List<StoreObjectId>();
		}

		public override T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue = default(T))
		{
			return defaultValue;
		}

		public override T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T))
		{
			return defaultValue;
		}

		public override bool TryGetPropertyBag(StoreObjectId itemId, out IStorePropertyBag bag)
		{
			bag = null;
			return false;
		}

		public override IStorePropertyBag MainPropertyBag
		{
			get
			{
				return null;
			}
		}
	}
}
