using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal static class MapiPropertyDefinitions
	{
		public static readonly MapiPropertyDefinition AddressBookEntryId = new MapiPropertyDefinition("AddressBookEntryId", typeof(MapiEntryId), PropTag.AddressBookEntryId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition AdminDisplayName = new MapiPropertyDefinition("AdminDisplayName", typeof(string), PropTag.AdminDisplayName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition AgeLimit = new MapiPropertyDefinition("AgeLimit", typeof(EnhancedTimeSpan?), PropTag.OverallAgeLimit, MapiPropertyDefinitionFlags.PersistDefaultValue, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableEnhancedTimeSpanFromDays), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableEnhancedTimeSpanIntoDays), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition AssociatedItemCount = new MapiPropertyDefinition("AssociatedItemCount", typeof(uint?), PropTag.AssocContentCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ContactCount = new MapiPropertyDefinition("ContactCount", typeof(uint?), PropTag.NamedPropertiesCountQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CreationTime = new MapiPropertyDefinition("CreationTime", typeof(DateTime?), PropTag.CreationTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DeletedItemCount = new MapiPropertyDefinition("DeletedItemCount", typeof(uint?), PropTag.DeletedMsgCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DisablePeruserRead = new MapiPropertyDefinition("DisablePeruserRead", typeof(bool), PropTag.DisablePeruserRead, MapiPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DisconnectDate = new MapiPropertyDefinition("DisconnectDate", typeof(DateTime?), PropTag.DateDiscoveredAbsentInDS, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DisplayName = new MapiPropertyDefinition("DisplayName", typeof(string), PropTag.DisplayName, MapiPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		});

		public static readonly MapiPropertyDefinition EntryId = new MapiPropertyDefinition("EntryId", typeof(MapiEntryId), PropTag.EntryId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ExpiryTime = new MapiPropertyDefinition("ExpiryTime", typeof(DateTime?), PropTag.ExpiryTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderPath = new MapiPropertyDefinition("FolderPath", typeof(string), PropTag.FolderPathName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition HasSubFolders = new MapiPropertyDefinition("HasSubFolders", typeof(bool), PropTag.SubFolders, MapiPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition IsDeletePending = new MapiPropertyDefinition("IsDeletePending", typeof(bool), PropTag.BeingDeleted, MapiPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition IssueWarningQuotaValue = new MapiPropertyDefinition("IssueWarningQuotaValue", typeof(Unlimited<ByteQuantifiedSize>?), PropTag.PfStorageQuota, MapiPropertyDefinitionFlags.PersistDefaultValue, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableUnlimitedByteQuantifiedSizeFromKilobytes), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableUnlimitedByteQuantifiedSizeIntoKilobytes), new PropertyDefinitionConstraint[]
		{
			new RangedNullableUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ItemCount = new MapiPropertyDefinition("ItemCount", typeof(uint?), PropTag.ContentCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastAccessTime = new MapiPropertyDefinition("LastAccessTime", typeof(DateTime?), PropTag.LastAccessTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastLoggedOnUserAccount = new MapiPropertyDefinition("LastLoggedOnUserAccount", typeof(string), PropTag.NTUserName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastLogoffTime = new MapiPropertyDefinition("LastLogoffTime", typeof(DateTime?), PropTag.LastLogoffTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastLogonTime = new MapiPropertyDefinition("LastLogonTime", typeof(DateTime?), PropTag.LastLogonTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastModificationTime = new MapiPropertyDefinition("LastModificationTime", typeof(DateTime?), PropTag.LastModificationTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LegacyDN = new MapiPropertyDefinition("LegacyDN", typeof(string), PropTag.EmailAddress, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LocalReplicaAgeLimit = new MapiPropertyDefinition("LocalReplicaAgeLimit", typeof(EnhancedTimeSpan?), PropTag.PfMsgAgeLimit, MapiPropertyDefinitionFlags.PersistDefaultValue, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableEnhancedTimeSpanFromDays), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableEnhancedTimeSpanIntoDays), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyStrictConstraint(),
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		});

		public static readonly MapiPropertyDefinition MailEnabled = new MapiPropertyDefinition("MailEnabled", typeof(bool), PropTag.PfProxyRequired, MapiPropertyDefinitionFlags.None, false, null, new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableValueToBool), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MailboxGuid = new MapiPropertyDefinition("MailboxGuid", typeof(Guid), PropTag.UserGuid, MapiPropertyDefinitionFlags.ReadOnly, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static readonly MapiPropertyDefinition MailboxMiscFlags = new MapiPropertyDefinition("MailboxMiscFlags", typeof(int?), PropTag.MailboxMiscFlags, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MaxItemSizeValue = new MapiPropertyDefinition("MaxItemSizeValue", typeof(Unlimited<ByteQuantifiedSize>?), PropTag.PfMsgSizeLimit, MapiPropertyDefinitionFlags.None, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableUnlimitedByteQuantifiedSizeFromKilobytes), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableUnlimitedByteQuantifiedSizeIntoKilobytes), new PropertyDefinitionConstraint[]
		{
			new RangedNullableUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition Name = new MapiPropertyDefinition("Name", typeof(string), PropTag.DisplayName, MapiPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ContainingNonWhitespaceConstraint(),
			new NotNullOrEmptyConstraint(),
			new CharacterConstraint(new char[]
			{
				'\\'
			}, false)
		});

		public static readonly MapiPropertyDefinition ObjectClass = new MapiPropertyDefinition("ObjectClass", typeof(ObjectClass), PropTag.ObjectClassFlags, MapiPropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.Mapi.ObjectClass.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition OwnerCount = new MapiPropertyDefinition("OwnerCount", typeof(uint?), PropTag.FoldersCountReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ProhibitPostQuotaValue = new MapiPropertyDefinition("ProhibitPostQuotaValue", typeof(Unlimited<ByteQuantifiedSize>?), PropTag.PfOverHardQuotaLimit, MapiPropertyDefinitionFlags.None, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableUnlimitedByteQuantifiedSizeFromKilobytes), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableUnlimitedByteQuantifiedSizeIntoKilobytes), new PropertyDefinitionConstraint[]
		{
			new RangedNullableUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ProxyGuid = new MapiPropertyDefinition("ProxyGuid", typeof(Guid?), PropTag.PfProxy, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PublicFolderQuotaStyle = new MapiPropertyDefinition("PublicFolderQuotaStyle", typeof(PublicFolderQuotaStyle), PropTag.PfQuotaStyle, MapiPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Mapi.PublicFolderQuotaStyle.UseMdbDefault, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PublishInAddressBook = new MapiPropertyDefinition("PublishInAddressBook", typeof(bool), PropTag.PublishInAddressBook, MapiPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ReplicasValue = new MapiPropertyDefinition("ReplicasValue", typeof(string), PropTag.ReplicaList, MapiPropertyDefinitionFlags.MultiValued, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractMultiAnsiStringsFromBytes), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackMultiAnsiStringsIntoBytes), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ReplicationScheduleValue = new MapiPropertyDefinition("ReplicationScheduleValue", typeof(Schedule), PropTag.ReplicationSchedule, MapiPropertyDefinitionFlags.PersistDefaultValue, Schedule.Never, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ReplicationStyle = new MapiPropertyDefinition("ReplicationStyle", typeof(ReplicationStyle), PropTag.ReplicationStyle, MapiPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Mapi.ReplicationStyle.Never, Microsoft.Exchange.Data.Mapi.ReplicationStyle.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition RetainDeletedItemsFor = new MapiPropertyDefinition("RetainDeletedItemsFor", typeof(EnhancedTimeSpan?), PropTag.RetentionAgeLimit, MapiPropertyDefinitionFlags.None, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractNullableEnhancedTimeSpanFromSeconds), new MapiPropValuePackerDelegate(CustomizedMapiPropValueConvertor.PackNullableEnhancedTimeSpanIntoSeconds), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition StorageLimitStatus = new MapiPropertyDefinition("StorageLimitStatus", typeof(StorageLimitStatus?), PropTag.StorageLimitInformation, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TotalAssociatedItemSize = new MapiPropertyDefinition("TotalAssociatedItemSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.AssocMessageSizeExtended, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromBytes), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TotalDeletedItemSize = new MapiPropertyDefinition("TotalDeletedItemSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.DeletedMessageSizeExtended, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromBytes), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TotalItemSize = new MapiPropertyDefinition("TotalItemSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.MessageSizeExtended, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromBytes), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderType = new MapiPropertyDefinition("FolderType", typeof(string), PropTag.ContainerClass, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition HasRules = new MapiPropertyDefinition("HasRules", typeof(bool), PropTag.HasRules, MapiPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition HasModerator = new MapiPropertyDefinition("HasModerator", typeof(bool), PropTag.HasModerator, MapiPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition IsQuarantined = new MapiPropertyDefinition("IsQuarantined", typeof(bool), PropTag.MailboxQuarantined, MapiPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition QuarantineDescription = new MapiPropertyDefinition("QuarantineDescription", typeof(string), PropTag.MailboxQuarantineDescription, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition QuarantineLastCrash = new MapiPropertyDefinition("QuarantineLastCrash", typeof(DateTime?), PropTag.MailboxQuarantineLastCrash, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition QuarantineEnd = new MapiPropertyDefinition("QuarantineEnd", typeof(DateTime?), PropTag.MailboxQuarantineEnd, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastUserModificationTime = new MapiPropertyDefinition("LastUserModificationTime", typeof(DateTime?), PropTag.LastUserModificationTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastUserAccessTime = new MapiPropertyDefinition("LastUserAccessTime", typeof(DateTime?), PropTag.LastUserAccessTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PersistableTenantPartitionHint = new MapiPropertyDefinition("PersistableTenantPartitionHint", typeof(byte[]), PropTag.PersistableTenantPartitionHint, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MailboxMessagesPerFolderCountWarningQuota = new MapiPropertyDefinition("MailboxMessagesPerFolderCountWarningQuota", typeof(int?), PropTag.MailboxMessagesPerFolderCountWarningQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MailboxMessagesPerFolderCountReceiveQuota = new MapiPropertyDefinition("MailboxMessagesPerFolderCountReceiveQuota", typeof(int?), PropTag.MailboxMessagesPerFolderCountReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DumpsterMessagesPerFolderCountWarningQuota = new MapiPropertyDefinition("DumpsterMessagesPerFolderCountWarningQuota", typeof(int?), PropTag.DumpsterMessagesPerFolderCountWarningQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DumpsterMessagesPerFolderCountReceiveQuota = new MapiPropertyDefinition("DumpsterMessagesPerFolderCountReceiveQuota", typeof(int?), PropTag.DumpsterMessagesPerFolderCountReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderHierarchyChildrenCountWarningQuota = new MapiPropertyDefinition("FolderHierarchyChildrenCountWarningQuota", typeof(int?), PropTag.FolderHierarchyChildrenCountWarningQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderHierarchyChildrenCountReceiveQuota = new MapiPropertyDefinition("FolderHierarchyChildrenCountReceiveQuota", typeof(int?), PropTag.FolderHierarchyChildrenCountReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderHierarchyDepthWarningQuota = new MapiPropertyDefinition("FolderHierarchyDepthWarningQuota", typeof(int?), PropTag.FolderHierarchyDepthWarningQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderHierarchyDepthReceiveQuota = new MapiPropertyDefinition("FolderHierarchyDepthReceiveQuota", typeof(int?), PropTag.FolderHierarchyDepthReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FoldersCountWarningQuota = new MapiPropertyDefinition("FoldersCountWarningQuota", typeof(int?), PropTag.FoldersCountWarningQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FoldersCountReceiveQuota = new MapiPropertyDefinition("FoldersCountReceiveQuota", typeof(int?), PropTag.FoldersCountReceiveQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition NamedPropertiesCountQuota = new MapiPropertyDefinition("NamedPropertiesCountQuota", typeof(int?), PropTag.NamedPropertiesCountQuota, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MessageTableTotalSize = new MapiPropertyDefinition("MessageTableTotalSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.MessageTableTotalPages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MessageTableAvailableSize = new MapiPropertyDefinition("MessageTableAvailableSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.MessageTableAvailablePages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition AttachmentTableTotalSize = new MapiPropertyDefinition("AttachmentTableTotalSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.AttachmentTableTotalPages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition AttachmentTableAvailableSize = new MapiPropertyDefinition("AttachmentTableAvailableSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.AttachmentTableAvailablePages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition OtherTablesTotalSize = new MapiPropertyDefinition("OtherTablesTotalSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.OtherTablesTotalPages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition OtherTablesAvailableSize = new MapiPropertyDefinition("OtherTablesAvailableSize", typeof(Unlimited<ByteQuantifiedSize>), PropTag.OtherTablesAvailablePages, MapiPropertyDefinitionFlags.ReadOnly, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractUnlimitedByteQuantifiedSizeFromPages), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
