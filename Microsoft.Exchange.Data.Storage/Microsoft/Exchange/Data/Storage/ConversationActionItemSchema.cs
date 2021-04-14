using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationActionItemSchema : ItemSchema
	{
		public new static ConversationActionItemSchema Instance
		{
			get
			{
				if (ConversationActionItemSchema.instance == null)
				{
					ConversationActionItemSchema.instance = new ConversationActionItemSchema();
				}
				return ConversationActionItemSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionMoveFolderId = InternalSchema.ConversationActionMoveFolderId;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionMoveStoreId = InternalSchema.ConversationActionMoveStoreId;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionPolicyTag = InternalSchema.ConversationActionPolicyTag;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionMaxDeliveryTime = InternalSchema.ConversationActionMaxDeliveryTime;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionLastMoveFolderId = InternalSchema.ConversationActionLastMoveFolderId;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionLastCategorySet = InternalSchema.ConversationActionLastCategorySet;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationActionVersion = InternalSchema.ConversationActionVersion;

		private static ConversationActionItemSchema instance = null;
	}
}
