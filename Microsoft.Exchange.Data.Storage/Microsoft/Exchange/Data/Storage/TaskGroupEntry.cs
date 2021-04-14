using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupEntry : FolderTreeData
	{
		public TaskGroupEntry(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.InitializeNewTaskGroupEntry();
				return;
			}
			this.Initialize();
		}

		public static TaskGroupEntry Bind(MailboxSession session, StoreId storeId)
		{
			return TaskGroupEntry.Bind(session, storeId, null);
		}

		public static TaskGroupEntry Bind(MailboxSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			TaskGroupEntry taskGroupEntry = ItemBuilder.ItemBind<TaskGroupEntry>(session, storeId, TaskGroupEntrySchema.Instance, propsToReturn);
			taskGroupEntry.MailboxSession = session;
			return taskGroupEntry;
		}

		public static TaskGroupEntry Create(MailboxSession session, Folder taskFolder, TaskGroup parentGroup)
		{
			Util.ThrowOnNullArgument(parentGroup, "parentGroup");
			Util.ThrowOnNullArgument(taskFolder, "TaskFolder");
			return TaskGroupEntry.Create(session, taskFolder.Id.ObjectId, parentGroup.GroupClassId, parentGroup.GroupName);
		}

		public static TaskGroupEntry Create(MailboxSession session, StoreObjectId taskFolderId, TaskGroup parentGroup)
		{
			Util.ThrowOnNullArgument(parentGroup, "parentGroup");
			TaskGroupEntry taskGroupEntry = TaskGroupEntry.Create(session, taskFolderId, parentGroup.GroupClassId, parentGroup.GroupName);
			taskGroupEntry.parentGroup = parentGroup;
			return taskGroupEntry;
		}

		internal static TaskGroupEntry Create(MailboxSession session, StoreObjectId taskFolderId, Guid parentGroupClassId, string parentGroupName)
		{
			Util.ThrowOnNullArgument(taskFolderId, "taskFolderId");
			if (taskFolderId.ObjectType != StoreObjectType.TasksFolder && taskFolderId.ObjectType != StoreObjectType.SearchFolder)
			{
				throw new NotSupportedException("A task group entry can only be associated with a storeobject of type task folder.");
			}
			TaskGroupEntry taskGroupEntry = TaskGroupEntry.Create(session, parentGroupClassId, parentGroupName);
			taskGroupEntry.TaskFolderId = taskFolderId;
			taskGroupEntry.StoreEntryId = Microsoft.Exchange.Data.Storage.StoreEntryId.ToProviderStoreEntryId(session.MailboxOwner);
			return taskGroupEntry;
		}

		internal static TaskGroupEntry Create(MailboxSession session, Guid parentGroupClassId, string parentGroupName)
		{
			Util.ThrowOnNullArgument(session, "session");
			TaskGroupEntry taskGroupEntry = ItemBuilder.CreateNewItem<TaskGroupEntry>(session, session.GetDefaultFolderId(DefaultFolderType.CommonViews), ItemCreateInfo.TaskGroupEntryInfo, CreateMessageType.Associated);
			taskGroupEntry.MailboxSession = session;
			taskGroupEntry.ParentGroupClassId = parentGroupClassId;
			taskGroupEntry.ParentGroupName = parentGroupName;
			return taskGroupEntry;
		}

		internal static TaskGroupEntryInfo GetTaskGroupEntryInfoFromRow(IStorePropertyBag row)
		{
			VersionedId versionedId = (VersionedId)row.TryGetProperty(ItemSchema.Id);
			byte[] valueOrDefault = row.GetValueOrDefault<byte[]>(TaskGroupEntrySchema.NodeEntryId, null);
			byte[] valueOrDefault2 = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId, null);
			string valueOrDefault3 = row.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
			byte[] valueOrDefault4 = row.GetValueOrDefault<byte[]>(FolderTreeDataSchema.Ordinal, null);
			row.GetValueOrDefault<byte[]>(TaskGroupEntrySchema.StoreEntryId, null);
			ExDateTime valueOrDefault5 = row.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
			row.GetValueOrDefault<FolderTreeDataType>(FolderTreeDataSchema.Type, FolderTreeDataType.NormalFolder);
			FolderTreeDataFlags valueOrDefault6 = row.GetValueOrDefault<FolderTreeDataFlags>(FolderTreeDataSchema.FolderTreeDataFlags, FolderTreeDataFlags.None);
			Guid safeGuidFromByteArray = FolderTreeData.GetSafeGuidFromByteArray(valueOrDefault2);
			if (safeGuidFromByteArray.Equals(Guid.Empty))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>(0L, "Found TaskGroupEntry with invalid parent group class id. ArrayLength: {0}", (valueOrDefault2 == null) ? -1 : valueOrDefault2.Length);
				return null;
			}
			if (IdConverter.IsFolderId(valueOrDefault))
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(valueOrDefault);
				if ((valueOrDefault6 & FolderTreeDataFlags.IsDefaultStore) == FolderTreeDataFlags.IsDefaultStore)
				{
					return new TaskGroupEntryInfo(valueOrDefault3, versionedId, storeObjectId, safeGuidFromByteArray, valueOrDefault4, valueOrDefault5);
				}
				ExTraceGlobals.StorageTracer.TraceDebug<StoreObjectType, string, VersionedId>(0L, "Found TaskGroupEntry of type {0} referencing a non-task folder. ObjectType: {0}. TaskFfolderName: {1}. Id: {2}.", storeObjectId.ObjectType, valueOrDefault3, versionedId);
			}
			return null;
		}

		public VersionedId TaskGroupEntryId
		{
			get
			{
				this.CheckDisposed("TaskGroupEntryId::get");
				return base.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			}
		}

		public byte[] TaskFolderRecordKey
		{
			get
			{
				this.CheckDisposed("TaskRecordKey::get");
				return base.GetValueOrDefault<byte[]>(TaskGroupEntrySchema.NodeRecordKey);
			}
			set
			{
				this.CheckDisposed("TaskRecordKey::set");
				this[TaskGroupEntrySchema.NodeRecordKey] = value;
			}
		}

		public StoreObjectId TaskFolderId
		{
			get
			{
				this.CheckDisposed("TaskFolderId::get");
				return this.taskFolderObjectId;
			}
			set
			{
				this.CheckDisposed("TaskFolderId::set");
				this[TaskGroupEntrySchema.NodeEntryId] = value.ProviderLevelItemId;
			}
		}

		public byte[] StoreEntryId
		{
			get
			{
				this.CheckDisposed("StoreEntryId::get");
				return base.GetValueOrDefault<byte[]>(TaskGroupEntrySchema.StoreEntryId);
			}
			set
			{
				this.CheckDisposed("StoreEntryId::set");
				this[TaskGroupEntrySchema.StoreEntryId] = value;
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

		public string ParentGroupName
		{
			get
			{
				this.CheckDisposed("ParentGroupName::get");
				return base.GetValueOrDefault<string>(TaskGroupEntrySchema.ParentGroupName);
			}
			private set
			{
				this.CheckDisposed("ParentGroupName::set");
				this[TaskGroupEntrySchema.ParentGroupName] = value;
			}
		}

		public string FolderName
		{
			get
			{
				this.CheckDisposed("FolderName::get");
				return base.GetValueOrDefault<string>(ItemSchema.Subject);
			}
			set
			{
				this.CheckDisposed("FolderName::set");
				this[ItemSchema.Subject] = value;
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
				if (propertyDefinition == TaskGroupEntrySchema.NodeEntryId)
				{
					this.SetTaskFolderId(value as byte[]);
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
				return TaskGroupEntrySchema.Instance;
			}
		}

		public TaskGroupEntryInfo GetTaskGroupEntryInfo()
		{
			return TaskGroupEntry.GetTaskGroupEntryInfoFromRow(this);
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			if (base.IsNew)
			{
				if (Guid.Empty.Equals(this.ParentGroupClassId))
				{
					throw new NotSupportedException("A new Task group entry needs to have its ParentGroupClassId set.");
				}
				byte[] nodeBefore = null;
				if (this.parentGroup != null)
				{
					ReadOnlyCollection<TaskGroupEntryInfo> childTaskFolders = this.parentGroup.GetChildTaskFolders();
					if (childTaskFolders.Count > 0)
					{
						nodeBefore = childTaskFolders[childTaskFolders.Count - 1].Ordinal;
					}
				}
				else
				{
					bool flag;
					nodeBefore = FolderTreeData.GetOrdinalValueOfFirstMatchingNode(base.MailboxSession, TaskGroupEntry.FindLastTaskFolderOrdinalSortOrder, (IStorePropertyBag row) => TaskGroup.IsFolderTreeData(row) && TaskGroup.IsTaskSection(row) && TaskGroup.IsTaskFolderInGroup(row, this.ParentGroupClassId), TaskGroup.TaskInfoProperties, out flag);
				}
				base.SetNodeOrdinalInternal(nodeBefore, null);
			}
		}

		private void Initialize()
		{
			this.SetTaskFolderId(base.GetValueOrDefault<byte[]>(TaskGroupEntrySchema.NodeEntryId));
			this.parentGroupClassId = FolderTreeData.GetSafeGuidFromByteArray(base.GetValueOrDefault<byte[]>(FolderTreeDataSchema.ParentGroupClassId));
		}

		private void InitializeNewTaskGroupEntry()
		{
			this[StoreObjectSchema.ItemClass] = "IPM.Microsoft.WunderBar.Link";
			this[FolderTreeDataSchema.GroupSection] = FolderTreeDataSection.Tasks;
			this[FolderTreeDataSchema.ClassId] = TaskGroup.TaskSectionClassId.ToByteArray();
			this[FolderTreeDataSchema.Type] = FolderTreeDataType.NormalFolder;
			this[FolderTreeDataSchema.FolderTreeDataFlags] = FolderTreeDataFlags.IsDefaultStore;
		}

		private void SetTaskFolderId(byte[] entryId)
		{
			if (IdConverter.IsFolderId(entryId))
			{
				this.taskFolderObjectId = StoreObjectId.FromProviderSpecificIdOrNull(entryId);
			}
		}

		private static readonly SortBy[] FindLastTaskFolderOrdinalSortOrder = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.GroupSection, SortOrder.Ascending),
			new SortBy(FolderTreeDataSchema.ParentGroupClassId, SortOrder.Descending),
			new SortBy(FolderTreeDataSchema.Ordinal, SortOrder.Descending)
		};

		private StoreObjectId taskFolderObjectId;

		private Guid parentGroupClassId;

		private TaskGroup parentGroup;
	}
}
