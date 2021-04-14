using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSchema : StoreObjectSchema
	{
		protected FolderSchema()
		{
			base.AddDependencies(new Schema[]
			{
				CoreFolderSchema.Instance
			});
		}

		public new static FolderSchema Instance
		{
			get
			{
				if (FolderSchema.instance == null)
				{
					FolderSchema.instance = new FolderSchema();
				}
				return FolderSchema.instance;
			}
		}

		internal virtual void CoreObjectUpdate(CoreFolder coreFolder)
		{
			Folder.CoreObjectUpdateRetentionProperties(coreFolder);
		}

		[Autoload]
		public new static readonly StorePropertyDefinition DisplayName = CoreFolderSchema.DisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition Id = CoreFolderSchema.Id;

		[Autoload]
		public static readonly StorePropertyDefinition CorrelationId = InternalSchema.CorrelationId;

		[Autoload]
		internal static readonly StorePropertyDefinition SystemFolderFlags = InternalSchema.SystemFolderFlags;

		[Autoload]
		public static readonly StorePropertyDefinition HasRules = CoreFolderSchema.HasRules;

		[Autoload]
		public static readonly StorePropertyDefinition DeletedCountTotal = InternalSchema.DeletedCountTotal;

		[Autoload]
		public static readonly StorePropertyDefinition DeletedMsgCount = InternalSchema.DeletedMsgCount;

		[Autoload]
		public static readonly StorePropertyDefinition DeletedAssocMsgCount = InternalSchema.DeletedAssocMsgCount;

		[Autoload]
		public static readonly StorePropertyDefinition Description = CoreFolderSchema.Description;

		[Autoload]
		public static readonly StorePropertyDefinition ExtendedFolderFlags = InternalSchema.ExtendedFolderFlags;

		[Autoload]
		public static readonly StorePropertyDefinition HasChildren = InternalSchema.HasChildren;

		[Autoload]
		public static readonly StorePropertyDefinition ItemCount = CoreFolderSchema.ItemCount;

		[Autoload]
		public static readonly StorePropertyDefinition AssociatedItemCount = CoreFolderSchema.AssociatedItemCount;

		[Autoload]
		public static readonly StorePropertyDefinition EhaMigrationMessageCount = InternalSchema.EHAMigrationMessageCount;

		public static readonly StorePropertyDefinition SearchFolderItemCount = InternalSchema.SearchFolderItemCount;

		[Autoload]
		public static readonly StorePropertyDefinition MapiFolderType = InternalSchema.MapiFolderType;

		[Autoload]
		public static readonly StorePropertyDefinition ChildCount = CoreFolderSchema.ChildCount;

		[Autoload]
		public static readonly StorePropertyDefinition IsOutlookSearchFolder = InternalSchema.IsOutlookSearchFolder;

		[Autoload]
		public static readonly StorePropertyDefinition UnreadCount = InternalSchema.UnreadCount;

		public static readonly StorePropertyDefinition AccessRights = InternalSchema.AccessRights;

		[Autoload]
		public static readonly StorePropertyDefinition AdminFolderFlags = InternalSchema.AdminFolderFlags;

		public static readonly StorePropertyDefinition ELCFolderComment = InternalSchema.ELCFolderComment;

		[Autoload]
		public static readonly StorePropertyDefinition ELCPolicyIds = InternalSchema.ELCPolicyIds;

		public static readonly StorePropertyDefinition RetentionDate = InternalSchema.RetentionDate;

		public static readonly StorePropertyDefinition FolderQuota = InternalSchema.FolderQuota;

		public static readonly StorePropertyDefinition FolderSize = InternalSchema.FolderSize;

		public static readonly StorePropertyDefinition RetentionAgeLimit = InternalSchema.RetentionAgeLimit;

		public static readonly StorePropertyDefinition OverallAgeLimit = InternalSchema.OverallAgeLimit;

		public static readonly GuidNamePropertyDefinition LastMovedTimeStamp = InternalSchema.LastMovedTimeStamp;

		public static readonly StorePropertyDefinition PfOverHardQuotaLimit = InternalSchema.PfOverHardQuotaLimit;

		public static readonly StorePropertyDefinition PfStorageQuota = InternalSchema.PfStorageQuota;

		public static readonly StorePropertyDefinition PfMsgSizeLimit = InternalSchema.PfMsgSizeLimit;

		public static readonly StorePropertyDefinition PfQuotaStyle = InternalSchema.PfQuotaStyle;

		public static readonly StorePropertyDefinition DisablePerUserRead = InternalSchema.DisablePerUserRead;

		public static readonly StorePropertyDefinition LocalCommitTimeMax = InternalSchema.LocalCommitTimeMax;

		public static readonly StorePropertyDefinition EformsLocaleId = InternalSchema.EformsLocaleId;

		public static readonly StorePropertyDefinition DefaultFoldersLocaleId = InternalSchema.DefaultFoldersLocaleId;

		public static readonly StorePropertyDefinition HiddenFromAddressListsEnabled = InternalSchema.PublishInAddressBook;

		public static readonly PropertyTagPropertyDefinition FolderRulesSize = InternalSchema.FolderRulesSize;

		public static readonly StorePropertyDefinition PopImapConversionVersion = InternalSchema.PopImapConversionVersion;

		public static readonly StorePropertyDefinition ImapLastSeenArticleId = InternalSchema.ImapLastSeenArticleId;

		public static readonly StorePropertyDefinition ImapLastUidFixTime = InternalSchema.ImapLastUidFixTime;

		[Autoload]
		public static readonly StorePropertyDefinition IsHidden = InternalSchema.IsHidden;

		public static readonly StorePropertyDefinition NextArticleId = InternalSchema.NextArticleId;

		public static readonly StorePropertyDefinition FolderHierarchyDepth = InternalSchema.FolderHierarchyDepth;

		public static readonly StorePropertyDefinition UrlName = InternalSchema.UrlName;

		public static readonly StorePropertyDefinition FolderPathName = InternalSchema.FolderPathName;

		public static readonly StorePropertyDefinition ExtendedSize = InternalSchema.ExtendedSize;

		public static readonly StorePropertyDefinition SyncFolderSourceKey = InternalSchema.SyncFolderSourceKey;

		internal static readonly StorePropertyDefinition AssociatedSearchFolderLastUsedTime = InternalSchema.AssociatedSearchFolderLastUsedTime;

		public static readonly StorePropertyDefinition FolderHomePageUrl = InternalSchema.FolderHomePageUrl;

		public static readonly StorePropertyDefinition ElcRootFolderEntryId = InternalSchema.ElcRootFolderEntryId;

		public static readonly StorePropertyDefinition RetentionTagEntryId = InternalSchema.RetentionTagEntryId;

		public static readonly StorePropertyDefinition PublicFolderDumpsterHolderEntryId = InternalSchema.PublicFolderDumpsterHolderEntryId;

		public static readonly StorePropertyDefinition ElcFolderLocalizedName = InternalSchema.ElcFolderLocalizedName;

		public static readonly StorePropertyDefinition OutOfOfficeHistory = InternalSchema.OutOfOfficeHistory;

		private static FolderSchema instance = null;

		public static readonly PropertyTagPropertyDefinition SearchFolderMessageCount = InternalSchema.SearchFolderItemCount;

		public static readonly StorePropertyDefinition AggregationSyncProgress = InternalSchema.AggregationSyncProgress;

		public static readonly StorePropertyDefinition SecurityDescriptor = InternalSchema.SecurityDescriptor;

		public static readonly PropertyDefinition OutlookSearchFolderClsId = InternalSchema.OutlookSearchFolderClsId;

		public static readonly PropertyDefinition SearchFolderAllowAgeout = InternalSchema.SearchFolderAllowAgeout;

		public static readonly PropertyDefinition IPMFolder = InternalSchema.IPMFolder;

		public static readonly PropertyDefinition SearchFolderAgeOutTimeout = InternalSchema.SearchFolderAgeOutTimeout;

		public static readonly PropertyDefinition AssociatedSearchFolderId = InternalSchema.AssociatedSearchFolderId;

		public static readonly StorePropertyDefinition OwnerLogonUserConfigurationCache = InternalSchema.OwnerLogonUserConfigurationCache;

		public static readonly StorePropertyDefinition FolderFlags = InternalSchema.FolderFlags;

		[Autoload]
		public static readonly NativeStorePropertyDefinition PermissionChangeBlocked = CoreFolderSchema.PermissionChangeBlocked;

		public static readonly PropertyDefinition OwaViewStateSortColumn = InternalSchema.OwaViewStateSortColumn;

		public static readonly PropertyDefinition OwaViewStateSortOrder = InternalSchema.OwaViewStateSortOrder;

		public static readonly PropertyDefinition ReplicaList = CoreFolderSchema.ReplicaList;

		public static readonly PropertyDefinition ReplicaListBinary = InternalSchema.ReplicaListBinary;

		public static readonly PropertyDefinition ResolveMethod = InternalSchema.ResolveMethod;

		[Autoload]
		public static readonly PropertyDefinition RecentBindingHistory = CoreFolderSchema.RecentBindingHistory;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedUrl = InternalSchema.LinkedUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedId = InternalSchema.LinkedId;

		[Autoload]
		public static readonly StorePropertyDefinition SharePointChangeToken = InternalSchema.SharePointChangeToken;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedSiteUrl = InternalSchema.LinkedSiteUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedListId = InternalSchema.LinkedListId;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedSiteAuthorityUrl = InternalSchema.LinkedSiteAuthorityUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedLastFullSyncTime = InternalSchema.LinkedLastFullSyncTime;

		[Autoload]
		public static readonly StorePropertyDefinition IsDocumentLibraryFolder = InternalSchema.IsDocumentLibraryFolder;

		public static readonly StorePropertyDefinition MailEnabled = InternalSchema.MailEnabled;

		public static readonly StorePropertyDefinition ProxyGuid = InternalSchema.ProxyGuid;

		[Autoload]
		public static readonly GuidNamePropertyDefinition SubscriptionLastSuccessfulSyncTime = InternalSchema.SubscriptionLastSuccessfulSyncTime;

		[Autoload]
		public static readonly GuidIdPropertyDefinition SubscriptionLastAttemptedSyncTime = InternalSchema.SharingLastSync;

		[Autoload]
		public static readonly PropertyDefinition ConversationTopicHashEntries = InternalSchema.ConversationTopicHashEntries;

		public static readonly PropertyDefinition SearchBacklinkNames = InternalSchema.SearchBacklinkNames;

		public static readonly PropertyDefinition PeopleHubSortGroupPriority = InternalSchema.PeopleHubSortGroupPriority;

		public static readonly PropertyDefinition PeopleHubSortGroupPriorityVersion = InternalSchema.PeopleHubSortGroupPriorityVersion;

		public static readonly GuidNamePropertyDefinition IsPeopleConnectSyncFolder = InternalSchema.IsPeopleConnectSyncFolder;

		public static readonly PropertyDefinition PartOfContentIndexing = InternalSchema.PartOfContentIndexing;

		public static readonly PropertyDefinition ContentAggregationFlags = InternalSchema.ContentAggregationFlags;

		public static readonly PropertyDefinition PeopleIKnowEmailAddressCollection = InternalSchema.PeopleIKnowEmailAddressCollection;

		public static readonly PropertyDefinition PeopleIKnowEmailAddressRelevanceScoreCollection = InternalSchema.PeopleIKnowEmailAddressRelevanceScoreCollection;

		public static readonly PropertyDefinition GalContactsFolderState = InternalSchema.GalContactsFolderState;

		[Autoload]
		public static readonly PropertyDefinition UnClutteredViewFolderEntryId = InternalSchema.UnClutteredViewFolderEntryId;

		[Autoload]
		public static readonly PropertyDefinition ClutteredViewFolderEntryId = InternalSchema.ClutteredViewFolderEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarFolderVersion = InternalSchema.CalendarFolderVersion;

		[Autoload]
		public static readonly StorePropertyDefinition WorkingSetSourcePartitionInternal = InternalSchema.WorkingSetSourcePartitionInternal;

		[Autoload]
		public static readonly StorePropertyDefinition OfficeGraphLocation = InternalSchema.OfficeGraphLocation;
	}
}
