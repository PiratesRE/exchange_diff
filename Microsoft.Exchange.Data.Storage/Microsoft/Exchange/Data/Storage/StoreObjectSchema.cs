using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StoreObjectSchema : Schema
	{
		protected StoreObjectSchema()
		{
			base.AddDependencies(new Schema[]
			{
				CoreObjectSchema.Instance
			});
		}

		public new static StoreObjectSchema Instance
		{
			get
			{
				if (StoreObjectSchema.instance == null)
				{
					StoreObjectSchema.instance = new StoreObjectSchema();
				}
				return StoreObjectSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition ChangeKey = CoreObjectSchema.ChangeKey;

		[Autoload]
		public static readonly StorePropertyDefinition ContainerClass = InternalSchema.ContainerClass;

		[Autoload]
		public static readonly StorePropertyDefinition CreationTime = CoreObjectSchema.CreationTime;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition DisplayName = InternalSchema.DisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition EntryId = CoreObjectSchema.EntryId;

		[Autoload]
		public static readonly StorePropertyDefinition ItemClass = CoreItemSchema.ItemClass;

		[Autoload]
		public static readonly StorePropertyDefinition ContentClass = CoreObjectSchema.ContentClass;

		[Autoload]
		public static readonly StorePropertyDefinition IsRestricted = InternalSchema.IsRestricted;

		[Autoload]
		public static readonly StorePropertyDefinition LastModifiedTime = InternalSchema.LastModifiedTime;

		[Autoload]
		public static readonly StorePropertyDefinition ParentEntryId = CoreObjectSchema.ParentEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition SearchKey = InternalSchema.SearchKey;

		[Autoload]
		public static readonly StorePropertyDefinition ParentItemId = InternalSchema.ParentItemId;

		[Autoload]
		public static readonly StorePropertyDefinition RecordKey = InternalSchema.RecordKey;

		public static readonly StorePropertyDefinition MapiStoreEntryId = InternalSchema.StoreEntryId;

		internal static readonly StorePropertyDefinition SourceKey = CoreObjectSchema.SourceKey;

		internal static readonly StorePropertyDefinition ParentSourceKey = CoreObjectSchema.ParentSourceKey;

		internal static readonly StorePropertyDefinition PredecessorChangeList = CoreObjectSchema.PredecessorChangeList;

		public static readonly StorePropertyDefinition[] ContentConversionProperties = CoreObjectSchema.AllPropertiesOnStore;

		public static readonly StorePropertyDefinition EffectiveRights = InternalSchema.EffectiveRights;

		public static readonly StorePropertyDefinition DeletedOnTime = CoreObjectSchema.DeletedOnTime;

		public static readonly StorePropertyDefinition IsSoftDeleted = InternalSchema.IsSoftDeleted;

		public static readonly StorePropertyDefinition PolicyTag = InternalSchema.PolicyTag;

		public static readonly StorePropertyDefinition ExplicitPolicyTag = InternalSchema.ExplicitPolicyTag;

		public static readonly StorePropertyDefinition ArchiveTag = InternalSchema.ArchiveTag;

		public static readonly StorePropertyDefinition ExplicitArchiveTag = InternalSchema.ExplicitArchiveTag;

		public static readonly StorePropertyDefinition RetentionPeriod = InternalSchema.RetentionPeriod;

		public static readonly StorePropertyDefinition ArchivePeriod = InternalSchema.ArchivePeriod;

		public static readonly StorePropertyDefinition PhoneticDisplayName = InternalSchema.PhoneticDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition RetentionFlags = InternalSchema.RetentionFlags;

		private static StoreObjectSchema instance = null;
	}
}
