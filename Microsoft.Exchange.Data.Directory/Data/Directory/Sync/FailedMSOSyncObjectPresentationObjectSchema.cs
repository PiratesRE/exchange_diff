using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class FailedMSOSyncObjectPresentationObjectSchema : ObjectSchema
	{
		public static readonly PropertyDefinition ObjectId = FailedMSOSyncObjectSchema.ObjectId;

		public static readonly PropertyDefinition ContextId = FailedMSOSyncObjectSchema.ContextId;

		public static readonly PropertyDefinition ExternalDirectoryObjectClass = FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass;

		public static readonly PropertyDefinition SyncObjectId = FailedMSOSyncObjectSchema.SyncObjectId;

		public static readonly PropertyDefinition DivergenceTimestamp = FailedMSOSyncObjectSchema.DivergenceTimestamp;

		public static readonly PropertyDefinition DivergenceCount = FailedMSOSyncObjectSchema.DivergenceCount;

		public static readonly PropertyDefinition IsTemporary = FailedMSOSyncObjectSchema.IsTemporary;

		public static readonly PropertyDefinition IsIncrementalOnly = FailedMSOSyncObjectSchema.IsIncrementalOnly;

		public static readonly PropertyDefinition IsLinkRelated = FailedMSOSyncObjectSchema.IsLinkRelated;

		public static readonly PropertyDefinition IsIgnoredInHaltCondition = FailedMSOSyncObjectSchema.IsIgnoredInHaltCondition;

		public static readonly PropertyDefinition IsTenantWideDivergence = FailedMSOSyncObjectSchema.IsTenantWideDivergence;

		public static readonly PropertyDefinition IsRetriable = FailedMSOSyncObjectSchema.IsRetriable;

		public static readonly PropertyDefinition IsValidationDivergence = FailedMSOSyncObjectSchema.IsValidationDivergence;

		public static readonly PropertyDefinition Errors = FailedMSOSyncObjectSchema.Errors;

		public static readonly PropertyDefinition Comment = FailedMSOSyncObjectSchema.Comment;

		public static readonly PropertyDefinition WhenChanged = ADObjectSchema.WhenChanged;

		public static readonly PropertyDefinition WhenChangedUTC = ADObjectSchema.WhenChangedUTC;
	}
}
