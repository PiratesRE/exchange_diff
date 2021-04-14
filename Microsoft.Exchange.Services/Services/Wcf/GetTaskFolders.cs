using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetTaskFolders : TaskFolderActionBase<GetTaskFoldersResponse>
	{
		public GetTaskFolders(MailboxSession session) : base(session)
		{
		}

		public override GetTaskFoldersResponse Execute()
		{
			this.taskFolders = this.FindTaskFolders();
			TaskGroupInfoList taskGroupsView = TaskGroup.GetTaskGroupsView(base.MailboxSession);
			if (!taskGroupsView.DefaultGroups.ContainsKey(TaskGroupType.MyTasks))
			{
				this.CreateDefaultTaskGroup(taskGroupsView);
			}
			this.FixGroupAssociations(taskGroupsView);
			if (taskGroupsView.DuplicateNodes.Count > 0)
			{
				this.RemoveDuplicateEntries(taskGroupsView.DuplicateNodes);
			}
			List<TaskGroup> list = this.ProcessGroupInformation(taskGroupsView);
			return new GetTaskFoldersResponse
			{
				TaskGroups = list.ToArray()
			};
		}

		private List<TaskGroup> ProcessGroupInformation(TaskGroupInfoList groups)
		{
			List<TaskGroup> list = new List<TaskGroup>(groups.Count);
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.ToDoSearch);
			StoreObjectId defaultFolderId2 = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks);
			foreach (TaskGroupInfo taskGroupInfo in groups)
			{
				TaskGroup taskGroup = new TaskGroup
				{
					ItemId = IdConverter.ConvertStoreItemIdToItemId(taskGroupInfo.Id, base.MailboxSession),
					GroupId = taskGroupInfo.GroupClassId.ToString(),
					GroupName = taskGroupInfo.GroupName,
					GroupType = taskGroupInfo.GroupType
				};
				if (taskGroupInfo.TaskFolders.Count > 0)
				{
					List<TaskFolderEntry> list2 = new List<TaskFolderEntry>(taskGroupInfo.TaskFolders.Count);
					foreach (TaskGroupEntryInfo taskGroupEntryInfo in taskGroupInfo.TaskFolders)
					{
						TaskFolderEntryType taskFolderEntryType = TaskFolderEntryType.Normal;
						if (defaultFolderId.Equals(taskGroupEntryInfo.TaskFolderId))
						{
							taskFolderEntryType = TaskFolderEntryType.ToDoSearch;
						}
						else if (defaultFolderId2.Equals(taskGroupEntryInfo.TaskFolderId))
						{
							taskFolderEntryType = TaskFolderEntryType.Tasks;
						}
						TaskFolderEntry item = new TaskFolderEntry
						{
							TaskFolderId = IdConverter.ConvertStoreFolderIdToFolderId(taskGroupEntryInfo.TaskFolderId, base.MailboxSession),
							FolderName = taskGroupEntryInfo.FolderName,
							ParentGroupId = taskGroupEntryInfo.ParentGroupClassId.ToString(),
							ItemId = IdConverter.ConvertStoreItemIdToItemId(taskGroupEntryInfo.Id, base.MailboxSession),
							TaskFolderEntryType = taskFolderEntryType
						};
						list2.Add(item);
					}
					taskGroup.TaskFolders = list2.ToArray();
				}
				list.Add(taskGroup);
			}
			return list;
		}

		private Dictionary<StoreObjectId, TasksFolderType> FindTaskFolders()
		{
			Dictionary<StoreObjectId, TasksFolderType> dictionary = new Dictionary<StoreObjectId, TasksFolderType>();
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			using (Folder folder = Folder.Bind(base.MailboxSession, DefaultFolderType.Root))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, GetTaskFolders.folderProperties))
				{
					for (;;)
					{
						bool flag = false;
						int num = -1;
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags.Length == 0)
						{
							break;
						}
						foreach (IStorePropertyBag storePropertyBag in propertyBags)
						{
							VersionedId versionedId = storePropertyBag.TryGetProperty(FolderSchema.Id) as VersionedId;
							int num2 = storePropertyBag.TryGetValueOrDefault(FolderSchema.FolderHierarchyDepth, -1);
							if (num2 == -1)
							{
								ExTraceGlobals.GetTaskFoldersCallTracer.TraceError<VersionedId>((long)this.GetHashCode(), "Folder without folder depth set. FolderId: {0}", versionedId);
							}
							else if (defaultFolderId.Equals(versionedId.ObjectId))
							{
								flag = true;
								num = num2;
								ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Found Deleted Items at folder depth: {0}. Ignoring folders until FolderDepth is the same again.", num2);
							}
							else
							{
								if (flag)
								{
									if (num2 > num)
									{
										ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Ignoring task folder found under Deleted Items - FolderId: {0}", versionedId);
										goto IL_1DC;
									}
									flag = false;
									ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Folder depth is the same or smaller than Deleted Items. Stop ignoring folders. FolderDepth: {0}", num2);
								}
								if (!storePropertyBag.TryGetValueOrDefault(FolderSchema.IsHidden, false))
								{
									string containerClass = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ContainerClass, string.Empty);
									if (ObjectClass.IsTaskFolder(containerClass))
									{
										StoreObjectId storeObjectId = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ParentItemId, null);
										if (storeObjectId == null)
										{
											ExTraceGlobals.GetTaskFoldersCallTracer.TraceError<VersionedId>((long)this.GetHashCode(), "Encountered folder without parent folder set. FolderId: {1}", versionedId);
										}
										else
										{
											string text = storePropertyBag.TryGetValueOrDefault(FolderSchema.DisplayName, string.Empty);
											if (string.IsNullOrEmpty(text))
											{
												ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Task folder with empty name encountered. Skipping. FolderId: {0}", versionedId);
											}
											else
											{
												EffectiveRights effectiveRights = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.EffectiveRights, EffectiveRights.None);
												dictionary.Add(StoreId.GetStoreObjectId(versionedId), this.GetTasksFolderType(versionedId, storeObjectId, text, effectiveRights));
											}
										}
									}
								}
							}
							IL_1DC:;
						}
					}
				}
			}
			return dictionary;
		}

		private TasksFolderType GetTasksFolderType(StoreId folderId, StoreObjectId parentFolderId, string folderName, EffectiveRights effectiveRights)
		{
			return new TasksFolderType
			{
				FolderId = IdConverter.ConvertStoreFolderIdToFolderId(folderId, base.MailboxSession),
				ParentFolderId = IdConverter.ConvertStoreFolderIdToFolderId(parentFolderId, base.MailboxSession),
				DisplayName = folderName,
				FolderClass = "IPF.Task",
				EffectiveRights = EffectiveRightsProperty.GetFromEffectiveRights(effectiveRights, base.MailboxSession)
			};
		}

		private void CreateDefaultTaskGroup(TaskGroupInfoList groups)
		{
			Dictionary<StoreObjectId, TaskGroupEntryInfo> taskFolderMapping = groups.TaskFolderMapping;
			TaskGroupInfo taskGroupInfo;
			using (TaskGroup taskGroup = TaskGroup.BindDefaultGroup(base.MailboxSession))
			{
				ReadOnlyCollection<TaskGroupEntryInfo> childTaskFolders = taskGroup.GetChildTaskFolders();
				taskGroupInfo = taskGroup.GetTaskGroupInfo();
				foreach (TaskGroupEntryInfo taskGroupEntryInfo in childTaskFolders)
				{
					taskGroupInfo.TaskFolders.Add(taskGroupEntryInfo);
					TaskGroupEntryInfo item;
					if (taskFolderMapping.TryGetValue(taskGroupEntryInfo.TaskFolderId, out item))
					{
						groups.DuplicateNodes.Add(item);
						taskFolderMapping[taskGroupEntryInfo.TaskFolderId] = taskGroupEntryInfo;
					}
					else
					{
						taskFolderMapping.Add(taskGroupEntryInfo.TaskFolderId, taskGroupEntryInfo);
					}
				}
			}
			groups.Add(taskGroupInfo);
			groups.DefaultGroups.Add(TaskGroupType.MyTasks, taskGroupInfo);
		}

		private void FixGroupAssociations(TaskGroupInfoList groups)
		{
			List<KeyValuePair<StoreObjectId, TasksFolderType>> list = new List<KeyValuePair<StoreObjectId, TasksFolderType>>();
			foreach (KeyValuePair<StoreObjectId, TasksFolderType> item in this.taskFolders)
			{
				if (!groups.TaskFolderMapping.ContainsKey(item.Key))
				{
					list.Add(item);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			TaskGroupInfo taskGroupInfo = groups.DefaultGroups[TaskGroupType.MyTasks];
			using (TaskGroup taskGroup = TaskGroup.BindDefaultGroup(base.MailboxSession))
			{
				foreach (KeyValuePair<StoreObjectId, TasksFolderType> keyValuePair in list)
				{
					StoreObjectId key = keyValuePair.Key;
					TasksFolderType value = keyValuePair.Value;
					using (TaskGroupEntry taskGroupEntry = TaskGroupEntry.Create(base.MailboxSession, key, taskGroup))
					{
						taskGroupEntry.FolderName = value.DisplayName;
						taskGroupEntry.TaskFolderRecordKey = key.ProviderLevelItemId;
						ConflictResolutionResult conflictResolutionResult = taskGroupEntry.Save(SaveMode.NoConflictResolution);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							ExTraceGlobals.GetTaskFoldersCallTracer.TraceError<string, StoreObjectId>((long)this.GetHashCode(), "Unable to associate task folder with MyTasks group. FolderName: {0}, FolderId: {1}.", value.DisplayName, key);
						}
						else
						{
							taskGroupEntry.Load();
							taskGroupInfo.TaskFolders.Add(taskGroupEntry.GetTaskGroupEntryInfo());
						}
					}
				}
			}
		}

		private void RemoveDuplicateEntries(IList<FolderTreeDataInfo> duplicates)
		{
			List<StoreId> list = new List<StoreId>();
			foreach (FolderTreeDataInfo folderTreeDataInfo in duplicates)
			{
				list.Add(folderTreeDataInfo.Id);
				TaskGroupEntryInfo taskGroupEntryInfo = folderTreeDataInfo as TaskGroupEntryInfo;
				TaskGroupInfo taskGroupInfo = folderTreeDataInfo as TaskGroupInfo;
				if (taskGroupEntryInfo != null)
				{
					ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<string, VersionedId, Guid>((long)this.GetHashCode(), "Removing duplicate TaskFolderEntry entry. FolderName: {0}, StoreId: {1}, ParentGroupId: {2}. ", taskGroupEntryInfo.FolderName ?? string.Empty, taskGroupEntryInfo.Id, taskGroupEntryInfo.ParentGroupClassId);
				}
				if (taskGroupInfo != null)
				{
					ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug<string, VersionedId, Guid>((long)this.GetHashCode(), "Removing duplicate TaskGroup entry. GroupName: {0}, StoreId: {1}, GroupId: {2}.", taskGroupInfo.GroupName, taskGroupInfo.Id, taskGroupInfo.GroupClassId);
				}
			}
			if (list.Count > 0)
			{
				AggregateOperationResult aggregateOperationResult = base.MailboxSession.Delete(DeleteItemFlags.SoftDelete, list.ToArray());
				if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
				{
					ExTraceGlobals.GetTaskFoldersCallTracer.TraceDebug((long)this.GetHashCode(), "Unable to delete duplicate FolderTreeData entries.");
				}
			}
		}

		private static readonly PropertyDefinition[] folderProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			StoreObjectSchema.ContainerClass,
			FolderSchema.IsHidden,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.EffectiveRights,
			FolderSchema.FolderHierarchyDepth
		};

		private Dictionary<StoreObjectId, TasksFolderType> taskFolders;
	}
}
