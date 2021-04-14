using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroupEntry : FolderTreeData, ICalendarGroupEntry, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public static CalendarGroupEntry BindFromCalendarFolder(MailboxSession session, StoreObjectId calendarFolderObjectId)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(calendarFolderObjectId, "calendarFolderObjectId");
			if (calendarFolderObjectId.ObjectType != StoreObjectType.CalendarFolder && calendarFolderObjectId.ObjectType != StoreObjectType.Folder)
			{
				throw new ArgumentException("StoreObject is not a calendar folder.", "calendarFolderObjectId");
			}
			VersionedId groupEntryIdFromFolderId = CalendarGroupEntry.GetGroupEntryIdFromFolderId(session, calendarFolderObjectId);
			if (groupEntryIdFromFolderId == null)
			{
				if (calendarFolderObjectId.Equals(session.GetDefaultFolderId(DefaultFolderType.Calendar)))
				{
					using (CalendarGroup calendarGroup = CalendarGroup.Bind(session, FolderTreeData.MyFoldersClassId))
					{
						ReadOnlyCollection<CalendarGroupEntryInfo> childCalendars = calendarGroup.GetChildCalendars();
						foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in childCalendars)
						{
							LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo = calendarGroupEntryInfo as LocalCalendarGroupEntryInfo;
							if (localCalendarGroupEntryInfo != null && calendarFolderObjectId.Equals(localCalendarGroupEntryInfo.CalendarId))
							{
								return CalendarGroupEntry.Bind(session, calendarGroupEntryInfo.Id, null);
							}
						}
					}
				}
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
			}
			return CalendarGroupEntry.Bind(session, groupEntryIdFromFolderId, null);
		}

		public static VersionedId GetGroupEntryIdFromFolderId(MailboxSession session, StoreObjectId calendarFolderObjectId)
		{
			return FolderTreeData.FindFirstRowMatchingFilter(session, CalendarGroup.CalendarInfoProperties, (IStorePropertyBag row) => CalendarGroup.IsFolderTreeData(row) && CalendarGroup.IsCalendarSection(row) && CalendarGroup.IsCalendarGroupEntryForCalendar(row, calendarFolderObjectId));
		}

		public static CalendarGroupEntry Bind(MailboxSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			CalendarGroupEntry calendarGroupEntry = ItemBuilder.ItemBind<CalendarGroupEntry>(session, storeId, CalendarGroupEntrySchema.Instance, propsToReturn);
			calendarGroupEntry.MailboxSession = session;
			return calendarGroupEntry;
		}

		public static CalendarGroupEntry Create(MailboxSession session, string legacyDistinguishedName, CalendarGroup parentGroup)
		{
			Util.ThrowOnNullOrEmptyArgument(legacyDistinguishedName, "legacyDistinguishedName");
			CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(session, parentGroup.GroupClassId, parentGroup.GroupName);
			calendarGroupEntry[FolderTreeDataSchema.Type] = FolderTreeDataType.SharedFolder;
			calendarGroupEntry[FolderTreeDataSchema.FolderTreeDataFlags] = 0;
			calendarGroupEntry.SharerAddressBookEntryId = AddressBookEntryId.MakeAddressBookEntryID(legacyDistinguishedName, false);
			calendarGroupEntry.UserAddressBookStoreEntryId = Microsoft.Exchange.Data.Storage.StoreEntryId.ToProviderStoreEntryId(session.MailboxOwner);
			return calendarGroupEntry;
		}

		public static CalendarGroupEntry Create(MailboxSession session, CalendarFolder calendarFolder, CalendarGroup parentGroup)
		{
			Util.ThrowOnNullArgument(parentGroup, "parentGroup");
			Util.ThrowOnNullArgument(calendarFolder, "calendarFolder");
			CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(session, calendarFolder.Id.ObjectId, parentGroup.GroupClassId, parentGroup.GroupName);
			FolderTreeDataFlags folderTreeDataFlags = CalendarGroupEntry.ReadSharingFlags(calendarFolder);
			bool flag = !session.MailboxGuid.Equals(((MailboxSession)calendarFolder.Session).MailboxGuid);
			if (flag)
			{
				calendarGroupEntry[FolderTreeDataSchema.Type] = FolderTreeDataType.SharedFolder;
				calendarGroupEntry.StoreEntryId = Microsoft.Exchange.Data.Storage.StoreEntryId.ToProviderStoreEntryId(((MailboxSession)calendarFolder.Session).MailboxOwner);
				calendarGroupEntry.CalendarRecordKey = (byte[])calendarFolder.TryGetProperty(StoreObjectSchema.RecordKey);
			}
			else
			{
				folderTreeDataFlags |= FolderTreeDataFlags.IsDefaultStore;
			}
			calendarGroupEntry[FolderTreeDataSchema.FolderTreeDataFlags] = folderTreeDataFlags;
			return calendarGroupEntry;
		}

		public static CalendarGroupEntry Create(MailboxSession session, StoreObjectId calendarFolderId, CalendarGroup parentGroup)
		{
			Util.ThrowOnNullArgument(parentGroup, "parentGroup");
			CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(session, calendarFolderId, parentGroup.GroupClassId, parentGroup.GroupName);
			calendarGroupEntry.parentGroup = parentGroup;
			return calendarGroupEntry;
		}

		internal static CalendarGroupEntry Create(MailboxSession session, StoreObjectId calendarFolderId, Guid parentGroupClassId, string parentGroupName)
		{
			Util.ThrowOnNullArgument(calendarFolderId, "calendarFolderId");
			if (calendarFolderId.ObjectType != StoreObjectType.CalendarFolder)
			{
				throw new NotSupportedException("A calendar group entry can only be associated with a storeobject of type calendar folder.");
			}
			CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(session, parentGroupClassId, parentGroupName);
			calendarGroupEntry.CalendarId = calendarFolderId;
			calendarGroupEntry.StoreEntryId = Microsoft.Exchange.Data.Storage.StoreEntryId.ToProviderStoreEntryId(session.MailboxOwner);
			return calendarGroupEntry;
		}

		internal static CalendarGroupEntry Create(MailboxSession session, Guid parentGroupClassId, string parentGroupName)
		{
			Util.ThrowOnNullArgument(session, "session");
			CalendarGroupEntry calendarGroupEntry = ItemBuilder.CreateNewItem<CalendarGroupEntry>(session, session.GetDefaultFolderId(DefaultFolderType.CommonViews), ItemCreateInfo.CalendarGroupEntryInfo, CreateMessageType.Associated);
			calendarGroupEntry.MailboxSession = session;
			calendarGroupEntry.ParentGroupClassId = parentGroupClassId;
			calendarGroupEntry.ParentGroupName = parentGroupName;
			return calendarGroupEntry;
		}

		internal static CalendarGroupEntryInfo GetCalendarGroupEntryInfoFromRow(IStorePropertyBag row)
		{
			VersionedId versionedId = (VersionedId)row.TryGetProperty(ItemSchema.Id);
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.NodeEntryId, null);
			byte[] valueOrDefault2 = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId, null);
			string valueOrDefault3 = row.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
			LegacyCalendarColor valueOrDefault4 = row.GetValueOrDefault<LegacyCalendarColor>(CalendarGroupEntrySchema.CalendarColor, LegacyCalendarColor.Auto);
			byte[] valueOrDefault5 = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.Ordinal, null);
			byte[] valueOrDefault6 = row.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.SharerAddressBookEntryId, null);
			byte[] valueOrDefault7 = row.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.StoreEntryId, null);
			ExDateTime valueOrDefault8 = row.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
			FolderTreeDataType valueOrDefault9 = row.GetValueOrDefault<FolderTreeDataType>(FolderTreeDataSchema.Type, FolderTreeDataType.NormalFolder);
			FolderTreeDataFlags valueOrDefault10 = row.GetValueOrDefault<FolderTreeDataFlags>(FolderTreeDataSchema.FolderTreeDataFlags, FolderTreeDataFlags.None);
			Guid safeGuidFromByteArray = FolderTreeData.GetSafeGuidFromByteArray(valueOrDefault2);
			if (safeGuidFromByteArray.Equals(Guid.Empty))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "Found CalendarGroupEntry with invalid parent group class id. ArrayLength: {0}", (valueOrDefault2 == null) ? -1 : valueOrDefault2.Length);
				return null;
			}
			if (valueOrDefault9 != FolderTreeDataType.SharedFolder)
			{
				if (IdConverter.IsFolderId(valueOrDefault))
				{
					StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(valueOrDefault);
					if ((valueOrDefault10 & FolderTreeDataFlags.IsDefaultStore) == FolderTreeDataFlags.IsDefaultStore)
					{
						return new LocalCalendarGroupEntryInfo(valueOrDefault3, versionedId, valueOrDefault4, storeObjectId, valueOrDefault5, safeGuidFromByteArray, (valueOrDefault10 & FolderTreeDataFlags.ICalFolder) == FolderTreeDataFlags.ICalFolder || (valueOrDefault10 & FolderTreeDataFlags.SharedIn) == FolderTreeDataFlags.SharedIn, valueOrDefault8);
					}
					if ((valueOrDefault10 & FolderTreeDataFlags.PublicFolder) == FolderTreeDataFlags.PublicFolder)
					{
						string calendarOwner = Microsoft.Exchange.Data.Storage.StoreEntryId.TryParseStoreEntryIdMailboxDN(valueOrDefault7);
						storeObjectId = StoreObjectId.FromLegacyFavoritePublicFolderId(storeObjectId);
						return new LinkedCalendarGroupEntryInfo(valueOrDefault3, versionedId, valueOrDefault4, storeObjectId, calendarOwner, safeGuidFromByteArray, valueOrDefault5, false, true, valueOrDefault8);
					}
					ExTraceGlobals.StorageTracer.TraceDebug<StoreObjectType, string, VersionedId>(0L, "Found CalendarGroupEntry of type {0} referencing a non-calendar folder. ObjectType: {0}. CalendarName: {1}. Id: {2}.", storeObjectId.ObjectType, valueOrDefault3, versionedId);
				}
				return null;
			}
			bool flag = true;
			Eidt eidt;
			string text;
			if (!AddressBookEntryId.IsAddressBookEntryId(valueOrDefault6, out eidt, out text))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "AddressBookEntryId is missing, not primary calendar {0}", valueOrDefault3);
				if (valueOrDefault7 == null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "StoreEntryId is missing for calendar: {0} - invalid entry, skipping.", valueOrDefault3);
					return null;
				}
				text = Microsoft.Exchange.Data.Storage.StoreEntryId.TryParseStoreEntryIdMailboxDN(valueOrDefault7);
				flag = false;
			}
			if (text == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "Unable to determine owner of shared calendar: {0}. Skipping.", valueOrDefault3);
				return null;
			}
			StoreObjectId storeObjectId2 = IdConverter.IsFolderId(valueOrDefault) ? StoreObjectId.FromProviderSpecificId(valueOrDefault) : null;
			if (!flag && storeObjectId2 == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "Secondary shared calendar without a folder id encountered {0}. Skipping.", valueOrDefault3);
				return null;
			}
			return new LinkedCalendarGroupEntryInfo(valueOrDefault3, versionedId, valueOrDefault4, storeObjectId2, text, safeGuidFromByteArray, valueOrDefault5, flag, false, valueOrDefault8);
		}

		public CalendarGroupEntry(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.InitializeNewCalendarGroupEntry();
				return;
			}
			this.Initialize();
		}

		public CalendarGroupEntryInfo GetCalendarGroupEntryInfo()
		{
			return CalendarGroupEntry.GetCalendarGroupEntryInfoFromRow(this);
		}

		private static FolderTreeDataFlags ReadSharingFlags(CalendarFolder calendarFolder)
		{
			ExtendedFolderFlags valueOrDefault = calendarFolder.GetValueOrDefault<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
			FolderTreeDataFlags folderTreeDataFlags = FolderTreeDataFlags.None;
			foreach (ExtendedFolderFlags extendedFolderFlags in CalendarGroupEntry.mapExtendedFolderToSharingFlag.Keys)
			{
				if ((valueOrDefault & extendedFolderFlags) == extendedFolderFlags)
				{
					folderTreeDataFlags |= CalendarGroupEntry.mapExtendedFolderToSharingFlag[extendedFolderFlags];
				}
			}
			return folderTreeDataFlags;
		}

		private void InitializeNewCalendarGroupEntry()
		{
			this[StoreObjectSchema.ItemClass] = "IPM.Microsoft.WunderBar.Link";
			this[FolderTreeDataSchema.GroupSection] = FolderTreeDataSection.Calendar;
			this[FolderTreeDataSchema.ClassId] = CalendarGroup.CalendarSectionClassId.ToByteArray();
			this[FolderTreeDataSchema.Type] = FolderTreeDataType.NormalFolder;
			this[FolderTreeDataSchema.FolderTreeDataFlags] = FolderTreeDataFlags.IsDefaultStore;
			this[CalendarGroupEntrySchema.CalendarColor] = LegacyCalendarColor.Auto;
		}

		private void Initialize()
		{
			this.SetCalendarId(base.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.NodeEntryId));
			this.parentGroupClassId = FolderTreeData.GetSafeGuidFromByteArray(base.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId));
		}

		private void SetCalendarId(byte[] entryId)
		{
			if (IdConverter.IsFolderId(entryId))
			{
				this.calendarObjectId = StoreObjectId.FromProviderSpecificIdOrNull(entryId);
			}
		}

		public override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				this.CheckDisposed("this::get");
				return base[propertyDefinition];
			}
			set
			{
				this.CheckDisposed("this::set");
				base[propertyDefinition] = value;
				if (propertyDefinition == CalendarGroupEntrySchema.NodeEntryId)
				{
					this.SetCalendarId(value as byte[]);
					return;
				}
				if (propertyDefinition == FolderTreeDataSchema.ParentGroupClassId)
				{
					this.parentGroupClassId = FolderTreeData.GetSafeGuidFromByteArray(value as byte[]);
				}
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarGroupEntrySchema.Instance;
			}
		}

		public byte[] StoreEntryId
		{
			get
			{
				this.CheckDisposed("StoreEntryId::get");
				return base.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.StoreEntryId);
			}
			set
			{
				this.CheckDisposed("StoreEntryId::set");
				this[CalendarGroupEntrySchema.StoreEntryId] = value;
			}
		}

		public string CalendarName
		{
			get
			{
				this.CheckDisposed("CalendarName::get");
				return base.GetValueOrDefault<string>(CalendarGroupEntrySchema.CalendarName);
			}
			set
			{
				this.CheckDisposed("CalendarName::set");
				this[CalendarGroupEntrySchema.CalendarName] = value;
			}
		}

		public string ParentGroupName
		{
			get
			{
				this.CheckDisposed("ParentGroupName::get");
				return base.GetValueOrDefault<string>(CalendarGroupEntrySchema.ParentGroupName);
			}
			private set
			{
				this.CheckDisposed("ParentGroupName::set");
				this[CalendarGroupEntrySchema.ParentGroupName] = value;
			}
		}

		public Guid ParentGroupClassId
		{
			get
			{
				this.CheckDisposed("ParentGroupClassId::get");
				return this.parentGroupClassId;
			}
			set
			{
				this.CheckDisposed("ParentGroupClassId::set");
				this[FolderTreeDataSchema.ParentGroupClassId] = value.ToByteArray();
			}
		}

		public LegacyCalendarColor LegacyCalendarColor
		{
			get
			{
				this.CheckDisposed("LegacyCalendarColor::get");
				return base.GetValueOrDefault<LegacyCalendarColor>(CalendarGroupEntrySchema.CalendarColor, LegacyCalendarColor.Auto);
			}
			set
			{
				this.CheckDisposed("LegacyCalendarColor::set");
				EnumValidator.ThrowIfInvalid<LegacyCalendarColor>(value, "LegacyCalendarColor");
				this[CalendarGroupEntrySchema.CalendarColor] = value;
			}
		}

		public CalendarColor CalendarColor
		{
			get
			{
				this.CheckDisposed("CalendarColor::get");
				return LegacyCalendarColorConverter.FromLegacyCalendarColor(this.LegacyCalendarColor);
			}
			set
			{
				this.CheckDisposed("CalendarColor::set");
				EnumValidator.ThrowIfInvalid<CalendarColor>(value, "CalendarColor");
				this.LegacyCalendarColor = LegacyCalendarColorConverter.ToLegacyCalendarColor(value);
			}
		}

		public StoreObjectId CalendarId
		{
			get
			{
				this.CheckDisposed("CalendarId::get");
				return this.calendarObjectId;
			}
			set
			{
				this.CheckDisposed("CalendarId::set");
				this[CalendarGroupEntrySchema.NodeEntryId] = value.ProviderLevelItemId;
			}
		}

		public VersionedId CalendarGroupEntryId
		{
			get
			{
				this.CheckDisposed("CalendarGroupEntryId::get");
				return base.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			}
		}

		public byte[] CalendarRecordKey
		{
			get
			{
				this.CheckDisposed("CalendarRecordKey::get");
				return base.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.NodeRecordKey);
			}
			set
			{
				this.CheckDisposed("CalendarRecordKey::set");
				this[CalendarGroupEntrySchema.NodeRecordKey] = value;
			}
		}

		public bool IsLocalMailboxCalendar
		{
			get
			{
				return this.CalendarId != null && base.FolderTreeDataType == FolderTreeDataType.NormalFolder && (base.FolderTreeDataFlags & FolderTreeDataFlags.IsDefaultStore) == FolderTreeDataFlags.IsDefaultStore;
			}
		}

		public byte[] SharerAddressBookEntryId
		{
			get
			{
				this.CheckDisposed("SharerAddressBookEntryId::get");
				return base.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.SharerAddressBookEntryId);
			}
			set
			{
				this.CheckDisposed("SharerAddressBookEntryId::set");
				this[CalendarGroupEntrySchema.SharerAddressBookEntryId] = value;
			}
		}

		public byte[] UserAddressBookStoreEntryId
		{
			get
			{
				this.CheckDisposed("UserAddressBookStoreEntryId::get");
				return base.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.UserAddressBookStoreEntryId);
			}
			set
			{
				this.CheckDisposed("UserAddressBookStoreEntryId::set");
				this[CalendarGroupEntrySchema.UserAddressBookStoreEntryId] = value;
			}
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			if (base.IsNew)
			{
				if (Guid.Empty.Equals(this.ParentGroupClassId))
				{
					throw new NotSupportedException("A new calendar group entry needs to have its ParentGroupClassId set.");
				}
				byte[] nodeBefore = null;
				if (this.parentGroup != null)
				{
					ReadOnlyCollection<CalendarGroupEntryInfo> childCalendars = this.parentGroup.GetChildCalendars();
					if (childCalendars.Count > 0)
					{
						nodeBefore = childCalendars[childCalendars.Count - 1].Ordinal;
					}
				}
				else
				{
					bool flag;
					nodeBefore = FolderTreeData.GetOrdinalValueOfFirstMatchingNode(base.MailboxSession, CalendarGroupEntry.FindLastCalendarOrdinalSortOrder, (IStorePropertyBag row) => CalendarGroup.IsFolderTreeData(row) && CalendarGroup.IsCalendarSection(row) && CalendarGroup.IsCalendarInGroup(row, this.ParentGroupClassId), CalendarGroup.CalendarInfoProperties, out flag);
				}
				base.SetNodeOrdinalInternal(nodeBefore, null);
			}
		}

		private static readonly SortBy[] FindLastCalendarOrdinalSortOrder = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.GroupSection, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.ParentGroupClassId, SortOrder.Descending),
			new SortBy(FolderTreeDataSchema.Ordinal, SortOrder.Descending)
		};

		private static readonly Dictionary<ExtendedFolderFlags, FolderTreeDataFlags> mapExtendedFolderToSharingFlag = new Dictionary<ExtendedFolderFlags, FolderTreeDataFlags>
		{
			{
				ExtendedFolderFlags.IsSharepointFolder,
				FolderTreeDataFlags.SharepointFolder
			},
			{
				ExtendedFolderFlags.SharedIn,
				FolderTreeDataFlags.SharedIn
			},
			{
				ExtendedFolderFlags.SharedOut,
				FolderTreeDataFlags.SharedOut
			},
			{
				ExtendedFolderFlags.SharedViaExchange,
				FolderTreeDataFlags.SharedOut
			},
			{
				ExtendedFolderFlags.PersonalShare,
				FolderTreeDataFlags.PersonFolder
			},
			{
				ExtendedFolderFlags.ICalFolder,
				FolderTreeDataFlags.ICalFolder
			}
		};

		private StoreObjectId calendarObjectId;

		private Guid parentGroupClassId;

		private CalendarGroup parentGroup;
	}
}
