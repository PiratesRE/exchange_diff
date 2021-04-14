using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreFolderSchema : CoreObjectSchema
	{
		private CoreFolderSchema()
		{
		}

		public new static CoreFolderSchema Instance
		{
			get
			{
				if (CoreFolderSchema.instance == null)
				{
					CoreFolderSchema.instance = new CoreFolderSchema();
				}
				return CoreFolderSchema.instance;
			}
		}

		private static CoreFolderSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition AssociatedItemCount = InternalSchema.AssociatedItemCount;

		[Autoload]
		public static readonly StorePropertyDefinition ChildCount = InternalSchema.ChildCount;

		[Autoload]
		public static readonly StorePropertyDefinition Description = InternalSchema.Description;

		[ConditionallyRequired(CustomConstraintDelegateEnum.IsNotConfigurationFolder)]
		[ConditionalStringLengthConstraint(1, 256, CustomConstraintDelegateEnum.IsNotConfigurationFolder)]
		[Autoload]
		[ConditionallyReadOnly(CustomConstraintDelegateEnum.DoesFolderHaveFixedDisplayName)]
		public static readonly StorePropertyDefinition DisplayName = InternalSchema.DisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition HasRules = InternalSchema.HasRules;

		[Autoload]
		public static readonly StorePropertyDefinition Id = InternalSchema.FolderId;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedUrl = InternalSchema.LinkedUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedSiteUrl = InternalSchema.LinkedSiteUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedListId = InternalSchema.LinkedListId;

		[Autoload]
		public static readonly StorePropertyDefinition ItemCount = InternalSchema.ItemCount;

		[Autoload]
		public static readonly StorePropertyDefinition UnreadCount = InternalSchema.UnreadCount;

		public static readonly PropertyTagPropertyDefinition MergeMidsetDeleted = InternalSchema.MergeMidsetDeleted;

		[Autoload]
		public static readonly StorePropertyDefinition ReplicaList = InternalSchema.ReplicaList;

		public static readonly StorePropertyDefinition LastMovedTimeStamp = InternalSchema.LastMovedTimeStamp;

		[Autoload]
		public static readonly StorePropertyDefinition DeletedItemsEntryId = InternalSchema.DeletedItemsEntryId;

		internal static readonly NativeStorePropertyDefinition MapiAclTable = InternalSchema.MapiAclTable;

		internal static readonly NativeStorePropertyDefinition MapiRulesTable = InternalSchema.MapiRulesTable;

		[Autoload]
		[FixedValueOnly(true)]
		internal static readonly NativeStorePropertyDefinition PermissionChangeBlocked = InternalSchema.PermissionChangeBlocked;

		public static readonly NativeStorePropertyDefinition RawSecurityDescriptor = InternalSchema.RawSecurityDescriptor;

		public static readonly NativeStorePropertyDefinition RawFreeBusySecurityDescriptor = InternalSchema.RawFreeBusySecurityDescriptor;

		public static readonly NativeStorePropertyDefinition AclTableAndSecurityDescriptor = InternalSchema.AclTableAndSecurityDescriptor;

		public static readonly StorePropertyDefinition SecurityDescriptor = InternalSchema.SecurityDescriptor;

		public static readonly StorePropertyDefinition FreeBusySecurityDescriptor = InternalSchema.FreeBusySecurityDescriptor;

		[Autoload]
		internal static readonly NativeStorePropertyDefinition RecentBindingHistory = InternalSchema.RecentBindingHistory;
	}
}
