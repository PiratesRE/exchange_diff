using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.OutlookClassIds;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FavoriteFolderEntry : FolderTreeData
	{
		internal FavoriteFolderEntry(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this[StoreObjectSchema.ItemClass] = "IPM.Microsoft.WunderBar.Link";
			}
		}

		public static FavoriteFolderEntry Create(MailboxSession session, StoreObjectId folderId, FolderTreeDataType dataType)
		{
			return FavoriteFolderEntry.Create(session, folderId, dataType, FavoriteFolderType.Mail);
		}

		public static FavoriteFolderEntry Create(MailboxSession session, StoreObjectId folderId, FolderTreeDataType dataType, FavoriteFolderType favoriteFolderType)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(session, "folderId");
			EnumValidator.ThrowIfInvalid<FolderTreeDataType>(dataType, "dataType");
			EnumValidator.ThrowIfInvalid<FavoriteFolderType>(favoriteFolderType, "favoriteFolderType");
			FolderTreeDataSection groupSection = FavoriteFolderEntry.FavoritesGroupSectionByFolderType[favoriteFolderType];
			byte[] favoritesClassIdValue = FavoriteFolderEntry.FavoritesClassIdValueByFolderType[favoriteFolderType];
			FavoriteFolderEntry favoriteFolderEntry = ItemBuilder.CreateNewItem<FavoriteFolderEntry>(session, session.GetDefaultFolderId(DefaultFolderType.CommonViews), ItemCreateInfo.FavoriteFolderEntryInfo, CreateMessageType.Associated);
			byte[] favoritesParentGroupClassIdValue = (FavoriteFolderType.Contact == favoriteFolderType || FavoriteFolderType.Calendar == favoriteFolderType) ? NavigationNodeParentGroup.OtherFoldersClassId.AsBytes : null;
			FolderTreeDataFlags additionalFolderTreeDataFlags = FolderTreeDataFlags.None;
			if (folderId.IsLegacyPublicFolderType())
			{
				additionalFolderTreeDataFlags = FavoriteFolderEntry.PublicFolderTreeDataFlagsByFolderType[favoriteFolderType];
			}
			favoriteFolderEntry.SetPropertiesOfFavoriteFolderEntry(session, folderId, dataType, groupSection, favoritesClassIdValue, favoritesParentGroupClassIdValue, additionalFolderTreeDataFlags);
			return favoriteFolderEntry;
		}

		private void SetPropertiesOfFavoriteFolderEntry(MailboxSession session, StoreObjectId folderId, FolderTreeDataType dataType, FolderTreeDataSection groupSection, byte[] favoritesClassIdValue, byte[] favoritesParentGroupClassIdValue, FolderTreeDataFlags additionalFolderTreeDataFlags)
		{
			if (folderId.ObjectType != StoreObjectType.Folder && folderId.ObjectType != StoreObjectType.SearchFolder)
			{
				throw new NotSupportedException("Only folder and search folder types can be added to favorites.");
			}
			this[FolderTreeDataSchema.GroupSection] = groupSection;
			if (favoritesClassIdValue != null)
			{
				this[FolderTreeDataSchema.ClassId] = favoritesClassIdValue;
			}
			if (favoritesParentGroupClassIdValue != null)
			{
				this[FolderTreeDataSchema.ParentGroupClassId] = favoritesParentGroupClassIdValue;
			}
			base.MailboxSession = session;
			this.NodeEntryId = folderId.ProviderLevelItemId;
			this.StoreEntryId = Microsoft.Exchange.Data.Storage.StoreEntryId.ToProviderStoreEntryId(session.MailboxOwner, folderId.IsLegacyPublicFolderType());
			if (additionalFolderTreeDataFlags != FolderTreeDataFlags.None)
			{
				base.FolderTreeDataFlags |= additionalFolderTreeDataFlags;
			}
			if (folderId.IsLegacyPublicFolderType())
			{
				byte[] providerLevelItemId = folderId.ProviderLevelItemId;
				providerLevelItemId[0] = 239;
				providerLevelItemId[20] = 2;
				this.NavigationNodeRecordKey = providerLevelItemId;
			}
			base.FolderTreeDataType = dataType;
		}

		public static FavoriteFolderEntry Bind(MailboxSession session, StoreId storeId)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			FavoriteFolderEntry favoriteFolderEntry = ItemBuilder.ItemBind<FavoriteFolderEntry>(session, storeId, FavoriteFolderEntrySchema.Instance, null);
			favoriteFolderEntry.MailboxSession = session;
			return favoriteFolderEntry;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return FavoriteFolderEntrySchema.Instance;
			}
		}

		public byte[] NodeEntryId
		{
			get
			{
				this.CheckDisposed("NodeEntryId::get");
				return base.GetValueOrDefault<byte[]>(FavoriteFolderEntrySchema.NodeEntryId);
			}
			set
			{
				this.CheckDisposed("NodeEntryId::set");
				this[FavoriteFolderEntrySchema.NodeEntryId] = value;
			}
		}

		public byte[] StoreEntryId
		{
			get
			{
				this.CheckDisposed("StoreEntryId::get");
				return base.GetValueOrDefault<byte[]>(FavoriteFolderEntrySchema.StoreEntryId);
			}
			set
			{
				this.CheckDisposed("StoreEntryId::set");
				this[FavoriteFolderEntrySchema.StoreEntryId] = value;
			}
		}

		public byte[] NavigationNodeRecordKey
		{
			get
			{
				this.CheckDisposed("RecordKey::get");
				return base.GetValueOrDefault<byte[]>(FavoriteFolderEntrySchema.NavigationNodeRecordKey);
			}
			set
			{
				this.CheckDisposed("RecordKey::set");
				this[FavoriteFolderEntrySchema.NavigationNodeRecordKey] = value;
			}
		}

		public string FolderDisplayName
		{
			get
			{
				this.CheckDisposed("FolderDisplayName::get");
				return base.GetValueOrDefault<string>(FavoriteFolderEntrySchema.FolderName);
			}
			set
			{
				this.CheckDisposed("FolderDisplayName::set");
				this[FavoriteFolderEntrySchema.FolderName] = value;
			}
		}

		public override void SetNodeOrdinal(byte[] nodeBefore, byte[] nodeAfter)
		{
			this.CheckDisposed("SetNodeOrdinal");
			base.SetNodeOrdinalInternal(nodeBefore, nodeAfter);
		}

		private const int PublicFolderFavByte = 239;

		private const int PublicFolderFavByteIndex = 0;

		private const int PublicFolderFavoriteType = 2;

		private const int PublicFolderFavoriteTypeIndex = 20;

		private static readonly Dictionary<FavoriteFolderType, byte[]> FavoritesClassIdValueByFolderType = new Dictionary<FavoriteFolderType, byte[]>
		{
			{
				FavoriteFolderType.Mail,
				NavigationNode.MailFolderFavoriteClassId.AsBytes
			},
			{
				FavoriteFolderType.Calendar,
				NavigationNode.CalendarFolderFavoriteClassId.AsBytes
			},
			{
				FavoriteFolderType.Contact,
				NavigationNode.ContactFolderFavoriteClassId.AsBytes
			}
		};

		private static readonly Dictionary<FavoriteFolderType, FolderTreeDataSection> FavoritesGroupSectionByFolderType = new Dictionary<FavoriteFolderType, FolderTreeDataSection>
		{
			{
				FavoriteFolderType.Mail,
				FolderTreeDataSection.First
			},
			{
				FavoriteFolderType.Calendar,
				FolderTreeDataSection.Calendar
			},
			{
				FavoriteFolderType.Contact,
				FolderTreeDataSection.Contacts
			}
		};

		private static readonly Dictionary<FavoriteFolderType, FolderTreeDataFlags> PublicFolderTreeDataFlagsByFolderType = new Dictionary<FavoriteFolderType, FolderTreeDataFlags>
		{
			{
				FavoriteFolderType.Mail,
				FolderTreeDataFlags.PublicFolder | FolderTreeDataFlags.PublicFolderFavorite | FolderTreeDataFlags.IpfNote
			},
			{
				FavoriteFolderType.Calendar,
				FolderTreeDataFlags.PublicFolder | FolderTreeDataFlags.PublicFolderFavorite | FolderTreeDataFlags.SharedOut
			},
			{
				FavoriteFolderType.Contact,
				FolderTreeDataFlags.PublicFolder | FolderTreeDataFlags.PublicFolderFavorite | FolderTreeDataFlags.SharedOut
			}
		};
	}
}
