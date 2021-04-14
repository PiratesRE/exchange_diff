using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StatusLogStorageSchema
	{
		public static StoreObjectId FindChildFolderByName(Folder folder, string name)
		{
			return folder.FindChildFolderByName(name);
		}

		public const string StatusLogItemClass = "IPM.Configuration.MailboxDiscoverySearch.StatusLog";

		public static readonly StorePropertyDefinition NameProperty = InternalSchema.Subject;

		public static readonly StorePropertyDefinition ItemClassProperty = InternalSchema.ItemClass;

		public static readonly StorePropertyDefinition ItemIdProperty = InternalSchema.ItemId;

		public static readonly GuidNamePropertyDefinition OperationStatusProperty = GuidNamePropertyDefinition.InternalCreate("OperationStatus", typeof(int), PropType.Int, MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "OperationStatus", PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.AllowCompatibleType, false, PropertyDefinitionConstraint.None);

		public static readonly GuidNamePropertyDefinition ExportSettingsProperty = GuidNamePropertyDefinition.InternalCreate("ExportSettings", typeof(byte[]), PropType.Binary, MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ExportSettings", PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, PropertyDefinitionConstraint.None);
	}
}
