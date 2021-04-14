using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderTreeDataSchema : MessageItemSchema
	{
		public new static FolderTreeDataSchema Instance
		{
			get
			{
				if (FolderTreeDataSchema.instance == null)
				{
					FolderTreeDataSchema.instance = new FolderTreeDataSchema();
				}
				return FolderTreeDataSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition Type = InternalSchema.NavigationNodeType;

		[Autoload]
		public static readonly StorePropertyDefinition OutlookTagId = InternalSchema.NavigationNodeOutlookTagId;

		[Autoload]
		public static readonly StorePropertyDefinition Ordinal = InternalSchema.NavigationNodeOrdinal;

		[Autoload]
		public static readonly StorePropertyDefinition ClassId = InternalSchema.NavigationNodeClassId;

		[Autoload]
		public static readonly StorePropertyDefinition GroupSection = InternalSchema.NavigationNodeGroupSection;

		[Autoload]
		public static readonly StorePropertyDefinition ParentGroupClassId = InternalSchema.NavigationNodeParentGroupClassId;

		[Autoload]
		public static readonly StorePropertyDefinition FolderTreeDataFlags = InternalSchema.NavigationNodeFlags;

		private static FolderTreeDataSchema instance;
	}
}
