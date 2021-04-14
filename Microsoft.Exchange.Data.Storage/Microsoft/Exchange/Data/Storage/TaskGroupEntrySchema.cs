using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupEntrySchema : FolderTreeDataSchema
	{
		public new static TaskGroupEntrySchema Instance
		{
			get
			{
				if (TaskGroupEntrySchema.instance == null)
				{
					TaskGroupEntrySchema.instance = new TaskGroupEntrySchema();
				}
				return TaskGroupEntrySchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition StoreEntryId = InternalSchema.NavigationNodeStoreEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition NodeRecordKey = InternalSchema.NavigationNodeRecordKey;

		[Autoload]
		public static readonly StorePropertyDefinition NodeEntryId = InternalSchema.NavigationNodeEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition ParentGroupName = InternalSchema.NavigationNodeGroupName;

		private static TaskGroupEntrySchema instance;
	}
}
