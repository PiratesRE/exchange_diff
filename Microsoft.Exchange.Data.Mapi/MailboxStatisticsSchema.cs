using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal sealed class MailboxStatisticsSchema : MapiObjectSchema
	{
		public static readonly MapiPropertyDefinition AssociatedItemCount = MapiPropertyDefinitions.AssociatedItemCount;

		public static readonly MapiPropertyDefinition DeletedItemCount = MapiPropertyDefinitions.DeletedItemCount;

		public static readonly MapiPropertyDefinition DisconnectDate = MapiPropertyDefinitions.DisconnectDate;

		public static readonly MapiPropertyDefinition DisplayName = MapiPropertyDefinitions.DisplayName;

		public static readonly MapiPropertyDefinition ItemCount = MapiPropertyDefinitions.ItemCount;

		public static readonly MapiPropertyDefinition LastLoggedOnUserAccount = MapiPropertyDefinitions.LastLoggedOnUserAccount;

		public static readonly MapiPropertyDefinition LastLogoffTime = MapiPropertyDefinitions.LastLogoffTime;

		public static readonly MapiPropertyDefinition LastLogonTime = MapiPropertyDefinitions.LastLogonTime;

		public static readonly MapiPropertyDefinition LegacyDN = MapiPropertyDefinitions.LegacyDN;

		public static readonly MapiPropertyDefinition MailboxGuid = MapiPropertyDefinitions.MailboxGuid;

		internal static readonly MapiPropertyDefinition MailboxMiscFlags = MapiPropertyDefinitions.MailboxMiscFlags;

		public static readonly MapiPropertyDefinition ObjectClass = MapiPropertyDefinitions.ObjectClass;

		public static readonly MapiPropertyDefinition StorageLimitStatus = MapiPropertyDefinitions.StorageLimitStatus;

		public static readonly MapiPropertyDefinition TotalDeletedItemSize = MapiPropertyDefinitions.TotalDeletedItemSize;

		public static readonly MapiPropertyDefinition TotalItemSize = MapiPropertyDefinitions.TotalItemSize;

		public static readonly MapiPropertyDefinition IsQuarantined = MapiPropertyDefinitions.IsQuarantined;

		public static readonly MapiPropertyDefinition QuarantineDescription = MapiPropertyDefinitions.QuarantineDescription;

		public static readonly MapiPropertyDefinition QuarantineLastCrash = MapiPropertyDefinitions.QuarantineLastCrash;

		public static readonly MapiPropertyDefinition QuarantineEnd = MapiPropertyDefinitions.QuarantineEnd;

		public static readonly MapiPropertyDefinition MailboxRootEntryId = new MapiPropertyDefinition("MailboxRootEntryId", typeof(long?), PropTag.MsgHeaderFid, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition StoreMailboxType = new MapiPropertyDefinition("MailboxType", typeof(StoreMailboxType), PropTag.MailboxType, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CurrentSchemaVersion = new MapiPropertyDefinition("CurrentSchemaVersion", typeof(int?), PropTag.MailboxDatabaseVersion, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PersistableTenantPartitionHint = MapiPropertyDefinitions.PersistableTenantPartitionHint;

		public static readonly MapiPropertyDefinition DatabaseName = new MapiPropertyDefinition("DatabaseName", typeof(string), PropTag.Null, MapiPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MailboxMessagesPerFolderCountWarningQuota = MapiPropertyDefinitions.MailboxMessagesPerFolderCountWarningQuota;

		public static readonly MapiPropertyDefinition MailboxMessagesPerFolderCountReceiveQuota = MapiPropertyDefinitions.MailboxMessagesPerFolderCountReceiveQuota;

		public static readonly MapiPropertyDefinition DumpsterMessagesPerFolderCountWarningQuota = MapiPropertyDefinitions.DumpsterMessagesPerFolderCountWarningQuota;

		public static readonly MapiPropertyDefinition DumpsterMessagesPerFolderCountReceiveQuota = MapiPropertyDefinitions.DumpsterMessagesPerFolderCountReceiveQuota;

		public static readonly MapiPropertyDefinition FolderHierarchyChildrenCountWarningQuota = MapiPropertyDefinitions.FolderHierarchyChildrenCountWarningQuota;

		public static readonly MapiPropertyDefinition FolderHierarchyChildrenCountReceiveQuota = MapiPropertyDefinitions.FolderHierarchyChildrenCountReceiveQuota;

		public static readonly MapiPropertyDefinition FolderHierarchyDepthWarningQuota = MapiPropertyDefinitions.FolderHierarchyDepthWarningQuota;

		public static readonly MapiPropertyDefinition FolderHierarchyDepthReceiveQuota = MapiPropertyDefinitions.FolderHierarchyDepthReceiveQuota;

		public static readonly MapiPropertyDefinition FoldersCountWarningQuota = MapiPropertyDefinitions.FoldersCountWarningQuota;

		public static readonly MapiPropertyDefinition FoldersCountReceiveQuota = MapiPropertyDefinitions.FoldersCountReceiveQuota;

		public static readonly MapiPropertyDefinition NamedPropertiesCountQuota = MapiPropertyDefinitions.NamedPropertiesCountQuota;

		public static readonly MapiPropertyDefinition MessageTableTotalSize = MapiPropertyDefinitions.MessageTableTotalSize;

		public static readonly MapiPropertyDefinition MessageTableAvailableSize = MapiPropertyDefinitions.MessageTableAvailableSize;

		public static readonly MapiPropertyDefinition AttachmentTableTotalSize = MapiPropertyDefinitions.AttachmentTableTotalSize;

		public static readonly MapiPropertyDefinition AttachmentTableAvailableSize = MapiPropertyDefinitions.AttachmentTableAvailableSize;

		public static readonly MapiPropertyDefinition OtherTablesTotalSize = MapiPropertyDefinitions.OtherTablesTotalSize;

		public static readonly MapiPropertyDefinition OtherTablesAvailableSize = MapiPropertyDefinitions.OtherTablesAvailableSize;
	}
}
