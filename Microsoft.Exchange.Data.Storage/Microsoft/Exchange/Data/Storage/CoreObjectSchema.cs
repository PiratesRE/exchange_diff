using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CoreObjectSchema : Schema
	{
		protected CoreObjectSchema()
		{
		}

		public new static CoreObjectSchema Instance
		{
			get
			{
				if (CoreObjectSchema.instance == null)
				{
					CoreObjectSchema.instance = new CoreObjectSchema();
				}
				return CoreObjectSchema.instance;
			}
		}

		private static CoreObjectSchema instance = null;

		public static readonly StorePropertyDefinition ChangeKey = InternalSchema.ChangeKey;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition CreationTime = InternalSchema.CreationTime;

		public static readonly StorePropertyDefinition DeletedOnTime = InternalSchema.DeletedOnTime;

		[Autoload]
		public static readonly StorePropertyDefinition EntryId = InternalSchema.EntryId;

		public static readonly StorePropertyDefinition LastModifiedTime = InternalSchema.LastModifiedTime;

		[Autoload]
		public static readonly StorePropertyDefinition ParentEntryId = InternalSchema.ParentEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition ContentClass = InternalSchema.ContentClass;

		[Autoload]
		public static readonly StorePropertyDefinition ParentFid = InternalSchema.ParentFid;

		internal static readonly StorePropertyDefinition ParentSourceKey = InternalSchema.ParentSourceKey;

		internal static readonly StorePropertyDefinition PredecessorChangeList = InternalSchema.PredecessorChangeList;

		internal static readonly StorePropertyDefinition SourceKey = InternalSchema.SourceKey;

		public static readonly StorePropertyDefinition[] AllPropertiesOnStore = InternalSchema.ContentConversionProperties;
	}
}
