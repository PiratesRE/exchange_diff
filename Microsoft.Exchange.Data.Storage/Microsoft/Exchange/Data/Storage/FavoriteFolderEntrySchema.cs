using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FavoriteFolderEntrySchema : FolderTreeDataSchema
	{
		public new static FavoriteFolderEntrySchema Instance
		{
			get
			{
				if (FavoriteFolderEntrySchema.instance == null)
				{
					FavoriteFolderEntrySchema.instance = new FavoriteFolderEntrySchema();
				}
				return FavoriteFolderEntrySchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition StoreEntryId = InternalSchema.NavigationNodeStoreEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition NodeEntryId = InternalSchema.NavigationNodeEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition NavigationNodeRecordKey = InternalSchema.NavigationNodeRecordKey;

		[Autoload]
		public static readonly StorePropertyDefinition FolderName = InternalSchema.Subject;

		private static FavoriteFolderEntrySchema instance;
	}
}
