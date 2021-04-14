using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ShortcutMessageSchema : MessageItemSchema
	{
		public new static ShortcutMessageSchema Instance
		{
			get
			{
				if (ShortcutMessageSchema.instance == null)
				{
					ShortcutMessageSchema.instance = new ShortcutMessageSchema();
				}
				return ShortcutMessageSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition FavPublicSourceKey = InternalSchema.FavPublicSourceKey;

		[Autoload]
		public static readonly StorePropertyDefinition FavoriteDisplayAlias = InternalSchema.FavoriteDisplayAlias;

		[Autoload]
		public static readonly StorePropertyDefinition FavoriteDisplayName = InternalSchema.FavoriteDisplayName;

		private static ShortcutMessageSchema instance;
	}
}
