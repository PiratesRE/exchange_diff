using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeNode : ConversationTreeNodeBase
	{
		internal ConversationTreeNode(PropertyDefinition indexPropertyDefinition, List<IStorePropertyBag> storePropertyBags, IConversationTreeNodeSorter conversationTreeNodeSorter) : base(conversationTreeNodeSorter)
		{
			ArgumentValidator.ThrowIfNull("storePropertyBags", storePropertyBags);
			this.indexPropertyDefinition = indexPropertyDefinition;
			this.storePropertyBags = storePropertyBags;
		}

		public override List<IStorePropertyBag> StorePropertyBags
		{
			get
			{
				return this.storePropertyBags;
			}
		}

		public override bool UpdatePropertyBag(StoreObjectId itemId, IStorePropertyBag bag)
		{
			for (int i = 0; i < this.storePropertyBags.Count; i++)
			{
				if (ConversationTreeNode.RetrieveStoreObjectId(this.storePropertyBags[i]).Equals(itemId))
				{
					this.storePropertyBags[i] = bag;
					return true;
				}
			}
			return false;
		}

		public static StoreObjectId RetrieveStoreObjectId(IStorePropertyBag bag)
		{
			return ((VersionedId)bag.TryGetProperty(ItemSchema.Id)).ObjectId;
		}

		public override bool HasAttachments
		{
			get
			{
				return this.AnyPropertyValueIsTrue(ItemSchema.HasAttachment);
			}
		}

		public override ConversationId ConversationThreadId
		{
			get
			{
				return this.GetValueOrDefault<ConversationId>(ItemSchema.ConversationFamilyId, null);
			}
		}

		public override bool HasBeenSubmitted
		{
			get
			{
				return this.AnyPropertyValueIsTrue(MessageItemSchema.HasBeenSubmitted);
			}
		}

		public override bool IsSpecificMessageReplyStamped
		{
			get
			{
				return this.AnyPropertyValueIsTrue(MessageItemSchema.IsSpecificMessageReplyStamped);
			}
		}

		public override bool IsSpecificMessageReply
		{
			get
			{
				return this.AnyPropertyValueIsTrue(MessageItemSchema.IsSpecificMessageReply);
			}
		}

		public override ConversationId ConversationId
		{
			get
			{
				byte[] valueOrDefault = this.GetValueOrDefault<byte[]>(ItemSchema.ConversationIndex, null);
				if (valueOrDefault != null)
				{
					return ConversationIndex.RetrieveConversationId(valueOrDefault);
				}
				return null;
			}
		}

		public override bool HasData
		{
			get
			{
				return this.StorePropertyBags.Count > 0;
			}
		}

		public override byte[] Index
		{
			get
			{
				if (this.index == null)
				{
					this.index = this.GetValueOrDefault<byte[]>(this.indexPropertyDefinition, null);
				}
				return this.index;
			}
		}

		public override StoreObjectId MainStoreObjectId
		{
			get
			{
				List<StoreObjectId> list = this.ToListStoreObjectId();
				if (list.Count > 0)
				{
					return list[0];
				}
				return null;
			}
		}

		public override ExDateTime? ReceivedTime
		{
			get
			{
				return this.GetValueOrDefault<ExDateTime?>(ItemSchema.ReceivedTime, null);
			}
		}

		public override string ItemClass
		{
			get
			{
				return this.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			}
		}

		public override List<StoreObjectId> ToListStoreObjectId()
		{
			if (this.ids == null)
			{
				this.ids = new List<StoreObjectId>();
				if (this.HasData)
				{
					foreach (IStorePropertyBag bag in this.StorePropertyBags)
					{
						this.ids.Add(ConversationTreeNode.RetrieveStoreObjectId(bag));
					}
				}
			}
			return this.ids;
		}

		public override T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue = default(T))
		{
			return this.GetValueOrDefault<T>(this.MainStoreObjectId, propertyDefinition, defaultValue);
		}

		public override T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T))
		{
			IStorePropertyBag storePropertyBag;
			if (this.TryGetPropertyBag(itemId, out storePropertyBag))
			{
				return storePropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}
			return defaultValue;
		}

		public override bool TryGetPropertyBag(StoreObjectId itemId, out IStorePropertyBag bag)
		{
			bag = null;
			if (!this.HasData || itemId == null)
			{
				return false;
			}
			foreach (IStorePropertyBag storePropertyBag in this.StorePropertyBags)
			{
				if (itemId.Equals(ConversationTreeNode.RetrieveStoreObjectId(storePropertyBag)))
				{
					bag = storePropertyBag;
					return true;
				}
			}
			return false;
		}

		public override IStorePropertyBag MainPropertyBag
		{
			get
			{
				IStorePropertyBag result;
				if (this.TryGetPropertyBag(this.MainStoreObjectId, out result))
				{
					return result;
				}
				return null;
			}
		}

		private bool AnyPropertyValueIsTrue(PropertyDefinition propertyDefinition)
		{
			if (!this.HasData)
			{
				return false;
			}
			foreach (IStorePropertyBag storePropertyBag in this.StorePropertyBags)
			{
				if (storePropertyBag.GetValueOrDefault<bool>(propertyDefinition, false))
				{
					return true;
				}
			}
			return false;
		}

		private readonly PropertyDefinition indexPropertyDefinition;

		private readonly List<IStorePropertyBag> storePropertyBags;

		private List<StoreObjectId> ids;

		private byte[] index;
	}
}
