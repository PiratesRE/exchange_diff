using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HierarchySyncMetadataItemSchema : ItemSchema
	{
		public new static HierarchySyncMetadataItemSchema Instance
		{
			get
			{
				if (HierarchySyncMetadataItemSchema.instance == null)
				{
					HierarchySyncMetadataItemSchema.instance = new HierarchySyncMetadataItemSchema();
				}
				return HierarchySyncMetadataItemSchema.instance;
			}
		}

		private static HierarchySyncMetadataItemSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition LastAttemptedSyncTime = InternalSchema.HierarchySyncLastAttemptedSyncTime;

		[Autoload]
		public static readonly StorePropertyDefinition LastFailedSyncTime = InternalSchema.HierarchySyncLastFailedSyncTime;

		[Autoload]
		public static readonly StorePropertyDefinition LastSuccessfulSyncTime = InternalSchema.HierarchySyncLastSuccessfulSyncTime;

		[Autoload]
		public static readonly StorePropertyDefinition FirstFailedSyncTimeAfterLastSuccess = InternalSchema.HierarchySyncFirstFailedSyncTimeAfterLastSuccess;

		[Autoload]
		public static readonly StorePropertyDefinition LastSyncFailure = InternalSchema.HierarchySyncLastSyncFailure;

		[Autoload]
		public static readonly StorePropertyDefinition NumberOfAttemptsAfterLastSuccess = InternalSchema.HierarchySyncNumberOfAttemptsAfterLastSuccess;

		[Autoload]
		public static readonly StorePropertyDefinition NumberOfBatchesExecuted = InternalSchema.HierarchySyncNumberOfBatchesExecuted;

		[Autoload]
		public static readonly StorePropertyDefinition NumberOfFoldersSynced = InternalSchema.HierarchySyncNumberOfFoldersSynced;

		[Autoload]
		public static readonly StorePropertyDefinition NumberOfFoldersToBeSynced = InternalSchema.HierarchySyncNumberOfFoldersToBeSynced;

		[Autoload]
		public static readonly StorePropertyDefinition BatchSize = InternalSchema.HierarchySyncBatchSize;
	}
}
