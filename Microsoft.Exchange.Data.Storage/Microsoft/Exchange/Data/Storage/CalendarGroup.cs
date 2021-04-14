using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroup : FolderTreeData, ICalendarGroup, IFolderTreeData, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal static bool IsFolderTreeData(IStorePropertyBag row)
		{
			string valueOrDefault = row.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			return ObjectClass.IsFolderTreeData(valueOrDefault);
		}

		internal static bool IsCalendarSection(IStorePropertyBag row)
		{
			FolderTreeDataSection valueOrDefault = row.GetValueOrDefault<FolderTreeDataSection>(FolderTreeDataSchema.GroupSection, FolderTreeDataSection.None);
			return valueOrDefault == FolderTreeDataSection.Calendar;
		}

		internal static bool IsCalendarGroupEntry(IStorePropertyBag row)
		{
			FolderTreeDataType valueOrDefault = row.GetValueOrDefault<FolderTreeDataType>(FolderTreeDataSchema.Type, FolderTreeDataType.Undefined);
			return valueOrDefault == FolderTreeDataType.NormalFolder || valueOrDefault == FolderTreeDataType.SharedFolder;
		}

		internal static bool IsCalendarGroup(IStorePropertyBag row)
		{
			FolderTreeDataType valueOrDefault = row.GetValueOrDefault<FolderTreeDataType>(FolderTreeDataSchema.Type, FolderTreeDataType.Undefined);
			return valueOrDefault == FolderTreeDataType.Header;
		}

		internal static bool IsCalendarGroup(IStorePropertyBag row, Guid groupId)
		{
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(CalendarGroupSchema.GroupClassId, null);
			return CalendarGroup.IsCalendarGroup(row) && Util.CompareByteArray(groupId.ToByteArray(), valueOrDefault);
		}

		internal static bool IsCalendarGroupEntryForCalendar(IStorePropertyBag row, StoreObjectId calendarId)
		{
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(CalendarGroupEntrySchema.NodeEntryId, null);
			return CalendarGroup.IsCalendarGroupEntry(row) && Util.CompareByteArray(calendarId.ProviderLevelItemId, valueOrDefault);
		}

		internal static bool IsCalendarInGroup(IStorePropertyBag row, Guid groupClassId)
		{
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId, null);
			return CalendarGroup.IsCalendarGroupEntry(row) && Util.CompareByteArray(groupClassId.ToByteArray(), valueOrDefault);
		}

		public static CalendarGroup Bind(MailboxSession session, CalendarGroupType defaultGroupType)
		{
			EnumValidator.ThrowIfInvalid<CalendarGroupType>(defaultGroupType, new CalendarGroupType[]
			{
				CalendarGroupType.MyCalendars,
				CalendarGroupType.OtherCalendars,
				CalendarGroupType.PeoplesCalendars
			});
			return CalendarGroup.Bind(session, CalendarGroup.GetGroupGuidFromType(defaultGroupType));
		}

		public static CalendarGroup Bind(MailboxSession session, Guid groupClassId)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(groupClassId, "groupClassId");
			if (groupClassId.Equals(Guid.Empty))
			{
				throw new ArgumentException("Invalid GroupClassId", "groupClassId");
			}
			CalendarGroup calendarGroup = null;
			bool flag = true;
			bool flag2 = false;
			CalendarGroup result;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.CommonViews))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, CalendarGroup.CalendarGroupViewSortOrder, CalendarGroup.CalendarInfoProperties))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, CalendarGroup.CalendarSectionFilter);
					while (flag)
					{
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags.Length == 0)
						{
							break;
						}
						for (int i = 0; i < propertyBags.Length; i++)
						{
							IStorePropertyBag storePropertyBag = propertyBags[i];
							if (!CalendarGroup.IsCalendarSection(storePropertyBag))
							{
								flag = false;
								break;
							}
							if (CalendarGroup.IsFolderTreeData(storePropertyBag))
							{
								if (!flag2 && CalendarGroup.IsCalendarGroup(storePropertyBag, groupClassId))
								{
									flag2 = true;
									VersionedId storeId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
									calendarGroup = CalendarGroup.Bind(session, storeId, null);
								}
								if (flag2)
								{
									calendarGroup.LoadChildNodesCollection(propertyBags, i);
									break;
								}
							}
						}
					}
					if (flag2)
					{
						result = calendarGroup;
					}
					else if (FolderTreeData.MyFoldersClassId.Equals(groupClassId))
					{
						result = CalendarGroup.CreateMyCalendarsGroup(session);
					}
					else
					{
						if (!FolderTreeData.OtherFoldersClassId.Equals(groupClassId))
						{
							throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
						}
						result = CalendarGroup.InternalCreateGroup(session, CalendarGroupType.OtherCalendars);
					}
				}
			}
			return result;
		}

		public static CalendarGroup Bind(MailboxSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn = null)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			CalendarGroup calendarGroup = ItemBuilder.ItemBind<CalendarGroup>(session, storeId, CalendarGroupSchema.Instance, propsToReturn);
			calendarGroup.MailboxSession = session;
			return calendarGroup;
		}

		public static CalendarGroup Create(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			CalendarGroup calendarGroup = ItemBuilder.CreateNewItem<CalendarGroup>(session, session.GetDefaultFolderId(DefaultFolderType.CommonViews), ItemCreateInfo.CalendarGroupInfo, CreateMessageType.Associated);
			calendarGroup.MailboxSession = session;
			return calendarGroup;
		}

		public static CalendarGroupInfoList GetCalendarGroupsView(MailboxSession session)
		{
			bool flag = false;
			bool flag2 = true;
			Dictionary<Guid, CalendarGroupInfo> guidToGroupMapping = new Dictionary<Guid, CalendarGroupInfo>();
			Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo> dictionary = new Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo>();
			List<FolderTreeDataInfo> duplicateNodes = new List<FolderTreeDataInfo>();
			Dictionary<CalendarGroupType, CalendarGroupInfo> defaultGroups = new Dictionary<CalendarGroupType, CalendarGroupInfo>();
			CalendarGroupInfoList calendarGroupInfoList = new CalendarGroupInfoList(duplicateNodes, defaultGroups, dictionary);
			using (Folder folder = Folder.Bind(session, DefaultFolderType.CommonViews))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, CalendarGroup.CalendarGroupViewSortOrder, CalendarGroup.CalendarInfoProperties))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, CalendarGroup.CalendarSectionFilter);
					while (flag2)
					{
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags.Length == 0)
						{
							break;
						}
						foreach (IStorePropertyBag storePropertyBag in propertyBags)
						{
							if (!CalendarGroup.IsCalendarSection(storePropertyBag))
							{
								flag2 = false;
								break;
							}
							if (CalendarGroup.IsFolderTreeData(storePropertyBag))
							{
								if (CalendarGroup.IsCalendarGroup(storePropertyBag))
								{
									if (flag)
									{
										ExTraceGlobals.StorageTracer.TraceDebug<VersionedId, string>(0L, "Unexpected processing calendar group out of order. ItemId: {0}, Subject: {1}", (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id), storePropertyBag.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty));
									}
									else
									{
										CalendarGroup.AddGroupToList(storePropertyBag, guidToGroupMapping, calendarGroupInfoList);
									}
								}
								else if (CalendarGroup.IsCalendarGroupEntry(storePropertyBag))
								{
									flag = true;
									CalendarGroup.AddCalendarToList(storePropertyBag, guidToGroupMapping, dictionary, calendarGroupInfoList);
								}
							}
						}
					}
				}
			}
			return calendarGroupInfoList;
		}

		private static void AddCalendarToList(IStorePropertyBag row, Dictionary<Guid, CalendarGroupInfo> guidToGroupMapping, Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo> calendarIdToGroupEntryMapping, CalendarGroupInfoList calendarGroups)
		{
			CalendarGroupEntryInfo calendarGroupEntryInfoFromRow = CalendarGroupEntry.GetCalendarGroupEntryInfoFromRow(row);
			if (calendarGroupEntryInfoFromRow == null)
			{
				return;
			}
			CalendarGroupInfo calendarGroupInfo;
			if (!guidToGroupMapping.TryGetValue(calendarGroupEntryInfoFromRow.ParentGroupClassId, out calendarGroupInfo))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, Guid, VersionedId>(0L, "Found a calendar group entry with an invalid parent id. CalendarName: {0}, ParentId: {1}, ItemId: {2}", calendarGroupEntryInfoFromRow.CalendarName, calendarGroupEntryInfoFromRow.ParentGroupClassId, calendarGroupEntryInfoFromRow.Id);
				return;
			}
			LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo = calendarGroupEntryInfoFromRow as LocalCalendarGroupEntryInfo;
			if (calendarGroupEntryInfoFromRow != null && calendarGroupEntryInfoFromRow.CalendarId != null && calendarGroupEntryInfoFromRow.CalendarId.IsPublicFolderType())
			{
				foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in calendarGroupInfo.Calendars)
				{
					if (calendarGroupEntryInfo.CalendarId != null && calendarGroupEntryInfo.CalendarId.Equals(calendarGroupEntryInfoFromRow.CalendarId))
					{
						return;
					}
				}
			}
			if (localCalendarGroupEntryInfo != null)
			{
				LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo2;
				if (calendarIdToGroupEntryMapping.TryGetValue(localCalendarGroupEntryInfo.CalendarId, out localCalendarGroupEntryInfo2))
				{
					if (localCalendarGroupEntryInfo2.LastModifiedTime.CompareTo(localCalendarGroupEntryInfo.LastModifiedTime) > 0)
					{
						calendarGroups.DuplicateNodes.Add(localCalendarGroupEntryInfo);
						return;
					}
					calendarGroups.DuplicateNodes.Add(localCalendarGroupEntryInfo2);
					guidToGroupMapping[localCalendarGroupEntryInfo2.ParentGroupClassId].Calendars.Remove(localCalendarGroupEntryInfo2);
					calendarIdToGroupEntryMapping[localCalendarGroupEntryInfo.CalendarId] = localCalendarGroupEntryInfo;
				}
				else
				{
					calendarIdToGroupEntryMapping.Add(localCalendarGroupEntryInfo.CalendarId, localCalendarGroupEntryInfo);
				}
			}
			calendarGroupInfo.Calendars.Add(calendarGroupEntryInfoFromRow);
		}

		private static void AddGroupToList(IStorePropertyBag row, Dictionary<Guid, CalendarGroupInfo> guidToGroupMapping, CalendarGroupInfoList calendarGroups)
		{
			CalendarGroupInfo calendarGroupInfoFromRow = CalendarGroup.GetCalendarGroupInfoFromRow(row);
			if (calendarGroupInfoFromRow == null)
			{
				return;
			}
			CalendarGroupInfo calendarGroupInfo;
			if (guidToGroupMapping.TryGetValue(calendarGroupInfoFromRow.GroupClassId, out calendarGroupInfo))
			{
				if (calendarGroupInfo.LastModifiedTime.CompareTo(calendarGroupInfoFromRow.LastModifiedTime) > 0)
				{
					calendarGroups.DuplicateNodes.Add(calendarGroupInfoFromRow);
					return;
				}
				guidToGroupMapping[calendarGroupInfoFromRow.GroupClassId] = calendarGroupInfoFromRow;
				calendarGroups.DuplicateNodes.Add(calendarGroupInfo);
				calendarGroups.Remove(calendarGroupInfo);
				if (calendarGroups.DefaultGroups.ContainsKey(calendarGroupInfoFromRow.GroupType))
				{
					calendarGroups.DefaultGroups[calendarGroupInfoFromRow.GroupType] = calendarGroupInfoFromRow;
				}
			}
			else
			{
				guidToGroupMapping.Add(calendarGroupInfoFromRow.GroupClassId, calendarGroupInfoFromRow);
				if (calendarGroupInfoFromRow.GroupType != CalendarGroupType.Normal)
				{
					calendarGroups.DefaultGroups.Add(calendarGroupInfoFromRow.GroupType, calendarGroupInfoFromRow);
				}
			}
			calendarGroups.Add(calendarGroupInfoFromRow);
		}

		private static CalendarGroup CreateMyCalendarsGroup(MailboxSession session)
		{
			CalendarGroup calendarGroup = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				calendarGroup = CalendarGroup.InternalCreateGroup(session, CalendarGroupType.MyCalendars);
				disposeGuard.Add<CalendarGroup>(calendarGroup);
				using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(session, session.GetDefaultFolderId(DefaultFolderType.Calendar), FolderTreeData.MyFoldersClassId, calendarGroup.GroupName))
				{
					calendarGroupEntry.CalendarName = ClientStrings.Calendar.ToString(session.InternalCulture);
					ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
					if (conflictResolutionResult.SaveStatus != SaveResult.Success)
					{
						ExTraceGlobals.StorageTracer.TraceWarning<SmtpAddress>(0L, "Unable to associate default calendar with the MyCalendars group for user: {0}. Attempting to delete default calendars group.", session.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
						AggregateOperationResult aggregateOperationResult = session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							calendarGroup.Id
						});
						if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
						{
							ExTraceGlobals.StorageTracer.TraceWarning<SmtpAddress>(0L, "Unable to delete default calendar group after failing to add the default calendar to it. User: {0}", session.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
						}
						throw new DefaultCalendarNodeCreationException();
					}
				}
				disposeGuard.Success();
			}
			return calendarGroup;
		}

		private static CalendarGroup InternalCreateGroup(MailboxSession session, CalendarGroupType groupType)
		{
			CalendarGroup calendarGroup;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				calendarGroup = CalendarGroup.CreateDefaultGroup(groupType, session);
				disposeGuard.Add<CalendarGroup>(calendarGroup);
				ConflictResolutionResult conflictResolutionResult = calendarGroup.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success)
				{
					ExTraceGlobals.StorageTracer.TraceWarning<CalendarGroupType, SmtpAddress>(0L, "Unable to create group of type {0} for user: {1}", groupType, session.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
					throw new DefaultCalendarGroupCreationException(groupType.ToString());
				}
				calendarGroup.Load();
				disposeGuard.Success();
			}
			return calendarGroup;
		}

		private static CalendarGroup CreateDefaultGroup(CalendarGroupType groupType, MailboxSession session)
		{
			CalendarGroup calendarGroup = CalendarGroup.Create(session);
			Guid groupGuidFromType = CalendarGroup.GetGroupGuidFromType(groupType);
			string groupName = string.Empty;
			switch (groupType)
			{
			case CalendarGroupType.MyCalendars:
				groupName = ClientStrings.MyCalendars.ToString(session.InternalPreferedCulture);
				break;
			case CalendarGroupType.OtherCalendars:
				groupName = ClientStrings.OtherCalendars.ToString(session.InternalPreferedCulture);
				break;
			case CalendarGroupType.PeoplesCalendars:
				groupName = ClientStrings.PeoplesCalendars.ToString(session.InternalPreferedCulture);
				break;
			}
			calendarGroup.GroupClassId = groupGuidFromType;
			calendarGroup.GroupName = groupName;
			return calendarGroup;
		}

		private static Guid GetGroupGuidFromType(CalendarGroupType groupType)
		{
			switch (groupType)
			{
			case CalendarGroupType.MyCalendars:
				return FolderTreeData.MyFoldersClassId;
			case CalendarGroupType.OtherCalendars:
				return FolderTreeData.OtherFoldersClassId;
			case CalendarGroupType.PeoplesCalendars:
				return FolderTreeData.PeoplesFoldersClassId;
			default:
				return Guid.Empty;
			}
		}

		private static CalendarGroupType GetGroupTypeFromGuid(Guid groupClassId)
		{
			if (groupClassId.Equals(FolderTreeData.MyFoldersClassId))
			{
				return CalendarGroupType.MyCalendars;
			}
			if (groupClassId.Equals(FolderTreeData.OtherFoldersClassId))
			{
				return CalendarGroupType.OtherCalendars;
			}
			if (groupClassId.Equals(FolderTreeData.PeoplesFoldersClassId))
			{
				return CalendarGroupType.PeoplesCalendars;
			}
			return CalendarGroupType.Normal;
		}

		private static CalendarGroupInfo GetCalendarGroupInfoFromRow(IStorePropertyBag row)
		{
			VersionedId id = (VersionedId)row.TryGetProperty(ItemSchema.Id);
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(CalendarGroupSchema.GroupClassId, null);
			string valueOrDefault2 = row.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
			byte[] valueOrDefault3 = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.Ordinal, null);
			ExDateTime valueOrDefault4 = row.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
			Guid safeGuidFromByteArray = FolderTreeData.GetSafeGuidFromByteArray(valueOrDefault);
			if (safeGuidFromByteArray.Equals(Guid.Empty))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "Found calendar group with invalid group class id. ArrayLength: {0}", (valueOrDefault == null) ? -1 : valueOrDefault.Length);
				return null;
			}
			return new CalendarGroupInfo(valueOrDefault2, id, safeGuidFromByteArray, CalendarGroup.GetGroupTypeFromGuid(safeGuidFromByteArray), valueOrDefault3, valueOrDefault4);
		}

		internal CalendarGroup(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.InitializeNewGroup();
				return;
			}
			this.groupClassId = FolderTreeData.GetSafeGuidFromByteArray(base.GetValueOrDefault<byte[]>(CalendarGroupSchema.GroupClassId));
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
				if (propertyDefinition == CalendarGroupSchema.GroupClassId)
				{
					byte[] guid = value as byte[];
					this.groupClassId = FolderTreeData.GetSafeGuidFromByteArray(guid);
				}
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarGroupSchema.Instance;
			}
		}

		public Guid GroupClassId
		{
			get
			{
				this.CheckDisposed("GroupClassId::get");
				return this.groupClassId;
			}
			private set
			{
				this.CheckDisposed("GroupClassId::set");
				this[CalendarGroupSchema.GroupClassId] = value.ToByteArray();
			}
		}

		public string GroupName
		{
			get
			{
				this.CheckDisposed("GroupName::get");
				return base.GetValueOrDefault<string>(ItemSchema.Subject);
			}
			set
			{
				this.CheckDisposed("GroupName::set");
				this[ItemSchema.Subject] = value;
			}
		}

		public CalendarGroupType GroupType
		{
			get
			{
				this.CheckDisposed("GroupType::get");
				return CalendarGroup.GetGroupTypeFromGuid(this.GroupClassId);
			}
		}

		public ReadOnlyCollection<CalendarGroupEntryInfo> GetChildCalendars()
		{
			this.CheckDisposed("GetChildCalendars");
			this.LoadChildNodesCollection();
			return new ReadOnlyCollection<CalendarGroupEntryInfo>(this.children);
		}

		public CalendarGroupInfo GetCalendarGroupInfo()
		{
			return CalendarGroup.GetCalendarGroupInfoFromRow(this);
		}

		public CalendarGroupEntryInfo FindSharedGSCalendaryEntry(string sharerLegacyDN)
		{
			this.CheckDisposed("FindSharedGSCalendaryEntry");
			Util.ThrowOnNullArgument("sharerLegacyDN", sharerLegacyDN);
			return this.children.FirstOrDefault(delegate(CalendarGroupEntryInfo folder)
			{
				LinkedCalendarGroupEntryInfo linkedCalendarGroupEntryInfo = folder as LinkedCalendarGroupEntryInfo;
				return linkedCalendarGroupEntryInfo != null && linkedCalendarGroupEntryInfo.IsGeneralScheduleCalendar && string.Equals(linkedCalendarGroupEntryInfo.CalendarOwner, sharerLegacyDN, StringComparison.OrdinalIgnoreCase);
			});
		}

		public CalendarGroupEntryInfo FindSharedCalendaryEntry(StoreObjectId folderId)
		{
			this.CheckDisposed("FindSharedCalendaryEntry");
			Util.ThrowOnNullArgument(folderId, "folderId");
			return this.children.FirstOrDefault((CalendarGroupEntryInfo folder) => folder.CalendarId != null && folder.CalendarId.Equals(folderId));
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			if (base.IsNew)
			{
				bool flag;
				byte[] nodeBefore = FolderTreeData.GetOrdinalValueOfFirstMatchingNode(base.MailboxSession, CalendarGroup.FindLastGroupOrdinalSortOrder, (IStorePropertyBag row) => CalendarGroup.IsFolderTreeData(row) && CalendarGroup.IsCalendarSection(row) && CalendarGroup.IsCalendarGroup(row), CalendarGroup.CalendarInfoProperties, out flag);
				if (flag && !FolderTreeData.MyFoldersClassId.Equals(this.GroupClassId))
				{
					using (CalendarGroup calendarGroup = CalendarGroup.CreateMyCalendarsGroup(base.MailboxSession))
					{
						nodeBefore = calendarGroup.NodeOrdinal;
					}
				}
				base.SetNodeOrdinalInternal(nodeBefore, null);
			}
		}

		private void InitializeNewGroup()
		{
			this[StoreObjectSchema.ItemClass] = "IPM.Microsoft.WunderBar.Link";
			this[FolderTreeDataSchema.Type] = FolderTreeDataType.Header;
			this[FolderTreeDataSchema.FolderTreeDataFlags] = 0;
			this[FolderTreeDataSchema.GroupSection] = FolderTreeDataSection.Calendar;
			this[CalendarGroupSchema.GroupClassId] = Guid.NewGuid().ToByteArray();
			this[FolderTreeDataSchema.ClassId] = CalendarGroup.CalendarSectionClassId.ToByteArray();
		}

		private void LoadChildNodesCollection()
		{
			if (base.IsNew || this.hasLoadedCalendarsCollection)
			{
				return;
			}
			using (Folder folder = Folder.Bind(base.MailboxSession, DefaultFolderType.CommonViews))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, CalendarGroup.CalendarGroupViewSortOrder, CalendarGroup.CalendarInfoProperties))
				{
					for (;;)
					{
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags.Length == 0)
						{
							break;
						}
						this.LoadChildNodesCollection(propertyBags, 0);
					}
				}
			}
		}

		private void LoadChildNodesCollection(IStorePropertyBag[] rows, int startIndex)
		{
			for (int i = startIndex; i < rows.Length; i++)
			{
				IStorePropertyBag storePropertyBag = rows[i];
				if (CalendarGroup.IsCalendarSection(storePropertyBag) && CalendarGroup.IsFolderTreeData(storePropertyBag) && CalendarGroup.IsCalendarGroupEntry(storePropertyBag))
				{
					byte[] valueOrDefault = storePropertyBag.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId, null);
					if (valueOrDefault == null || valueOrDefault.Length != 16)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "Found CalendarGroupEntry with invalid parent group id. ArrayLength: {0}", (valueOrDefault == null) ? -1 : valueOrDefault.Length);
					}
					else
					{
						Guid g = new Guid(valueOrDefault);
						if (this.groupClassId.Equals(g))
						{
							CalendarGroupEntryInfo calendarGroupEntryInfoFromRow = CalendarGroupEntry.GetCalendarGroupEntryInfoFromRow(storePropertyBag);
							if (calendarGroupEntryInfoFromRow != null)
							{
								this.children.Add(calendarGroupEntryInfoFromRow);
							}
						}
					}
				}
			}
			this.hasLoadedCalendarsCollection = true;
		}

		internal static readonly Guid CalendarSectionClassId = new Guid("{00067802-0000-0000-c000-000000000046}");

		private static readonly QueryFilter CalendarSectionFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderTreeDataSchema.GroupSection, FolderTreeDataSection.Calendar);

		private static readonly SortBy[] CalendarGroupViewSortOrder = new SortBy[]
		{
			new SortBy(FolderTreeDataSchema.GroupSection, SortOrder.Ascending),
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.ParentGroupClassId, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.Ordinal, SortOrder.Ascending)
		};

		private static readonly SortBy[] FindLastGroupOrdinalSortOrder = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.GroupSection, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.Type, SortOrder.Descending),
			new SortBy(FolderTreeDataSchema.Ordinal, SortOrder.Descending)
		};

		internal static readonly PropertyDefinition[] CalendarInfoProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ItemClass,
			ItemSchema.Subject,
			FolderTreeDataSchema.GroupSection,
			CalendarGroupSchema.GroupClassId,
			FolderTreeDataSchema.Type,
			FolderTreeDataSchema.Ordinal,
			FolderTreeDataSchema.ParentGroupClassId,
			CalendarGroupEntrySchema.NodeEntryId,
			CalendarGroupEntrySchema.CalendarColor,
			CalendarGroupEntrySchema.SharerAddressBookEntryId,
			CalendarGroupEntrySchema.StoreEntryId,
			FolderTreeDataSchema.FolderTreeDataFlags
		};

		private readonly List<CalendarGroupEntryInfo> children = new List<CalendarGroupEntryInfo>();

		private Guid groupClassId;

		private bool hasLoadedCalendarsCollection;
	}
}
