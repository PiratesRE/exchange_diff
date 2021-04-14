using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderTree : ICustomSerializableBuilder, ICustomSerializable, IEnumerable
	{
		public FolderTree()
		{
			this.folderTree = new Dictionary<ISyncItemId, FolderTree.FolderInfo>();
		}

		public ushort TypeId
		{
			get
			{
				return FolderTree.typeId;
			}
			set
			{
				FolderTree.typeId = value;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public static void BuildFolderTree(MailboxSession mailboxSession, SyncState syncState)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			SharingSubscriptionData[] array = null;
			using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(mailboxSession))
			{
				array = sharingSubscriptionManager.GetAll();
			}
			using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId))
			{
				FolderTree folderTree;
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, FolderTree.fetchProperties))
				{
					folderTree = new FolderTree();
					object[][] rows;
					do
					{
						rows = queryResult.GetRows(10000);
						for (int i = 0; i < rows.Length; i++)
						{
							MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(((VersionedId)rows[i][0]).ObjectId);
							MailboxSyncItemId mailboxSyncItemId2 = MailboxSyncItemId.CreateForNewItem((StoreObjectId)rows[i][1]);
							folderTree.AddFolder(mailboxSyncItemId);
							object obj = rows[i][3];
							int num = (obj is PropertyError) ? 0 : ((int)obj);
							if ((num & 1073741824) != 0)
							{
								for (int j = 0; j < array.Length; j++)
								{
									if (array[j].LocalFolderId.Equals(mailboxSyncItemId.NativeId))
									{
										folderTree.SetPermissions(mailboxSyncItemId, SyncPermissions.Readonly);
										folderTree.SetOwner(mailboxSyncItemId, array[j].SharerIdentity);
										break;
									}
								}
							}
							if (!defaultFolderId.Equals(mailboxSyncItemId2.NativeId))
							{
								folderTree.AddFolder(mailboxSyncItemId2);
								folderTree.LinkChildToParent(mailboxSyncItemId2, mailboxSyncItemId);
							}
							if ((bool)rows[i][2])
							{
								folderTree.SetHidden(mailboxSyncItemId, true);
							}
						}
					}
					while (rows.Length != 0);
				}
				syncState[CustomStateDatumType.FullFolderTree] = folderTree;
				syncState[CustomStateDatumType.RecoveryFullFolderTree] = syncState[CustomStateDatumType.FullFolderTree];
			}
		}

		public bool AddFolder(ISyncItemId folderId)
		{
			if (this.folderTree.ContainsKey(folderId))
			{
				return false;
			}
			this.folderTree[folderId] = new FolderTree.FolderInfo();
			this.isDirty = true;
			return true;
		}

		public bool Contains(ISyncItemId folderId)
		{
			return this.folderTree.ContainsKey(folderId);
		}

		public List<ISyncItemId> GetChildren(ISyncItemId folderId)
		{
			return new List<ISyncItemId>(this.GetFolderInfo(folderId).Children);
		}

		public string GetOwner(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).Owner;
		}

		public ISyncItemId GetParentId(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).ParentId;
		}

		public SyncPermissions GetPermissions(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).Permissions;
		}

		public bool IsHidden(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).Hidden;
		}

		public bool IsHiddenDueToParent(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).HiddenDueToParent;
		}

		public bool IsSharedFolder(ISyncItemId folderId)
		{
			return this.GetFolderInfo(folderId).IsSharedFolder;
		}

		public void LinkChildToParent(ISyncItemId parentId, ISyncItemId childId)
		{
			this.LinkChildToParent(parentId, childId, null);
		}

		public void LinkChildToParent(ISyncItemId parentId, ISyncItemId childId, HashSet<ISyncItemId> visibilityChangedList)
		{
			this.GetFolderInfo(parentId).AddChild(childId, this, visibilityChangedList);
			this.GetFolderInfo(childId).ParentId = parentId;
			this.isDirty = true;
		}

		public void RemoveFolder(ISyncItemId folderId)
		{
			ISyncItemId parentId = this.GetFolderInfo(folderId).ParentId;
			if (parentId != null)
			{
				this.UnlinkChild(parentId, folderId);
			}
			this.isDirty |= this.folderTree.Remove(folderId);
		}

		public void RemoveFolderAndChildren(ISyncItemId folderId, FolderIdMapping folderIdMapping)
		{
			this.RemoveFolderAndChildren(folderId, folderIdMapping, 0, this.folderTree.Keys.Count);
		}

		public void SetHidden(ISyncItemId folderId, bool newValue)
		{
			this.SetHidden(folderId, newValue, null);
		}

		public void SetHidden(ISyncItemId folderId, bool newValue, HashSet<ISyncItemId> visibilityChangedList)
		{
			this.isDirty |= this.GetFolderInfo(folderId).SetHidden(newValue, this, visibilityChangedList);
		}

		public void SetHiddenDueToParent(ISyncItemId folderId, bool newValue, HashSet<ISyncItemId> visibilityChangedList)
		{
			this.isDirty |= this.GetFolderInfo(folderId).SetHiddenDueToParent(newValue, this, visibilityChangedList);
		}

		public void SetOwner(ISyncItemId folderId, string owner)
		{
			FolderTree.FolderInfo folderInfo = this.GetFolderInfo(folderId);
			if (!string.Equals(folderInfo.Owner, owner))
			{
				folderInfo.Owner = owner;
				this.isDirty = true;
			}
		}

		public void SetPermissions(ISyncItemId folderId, SyncPermissions permissions)
		{
			FolderTree.FolderInfo folderInfo = this.GetFolderInfo(folderId);
			if (folderInfo.Permissions != permissions)
			{
				folderInfo.Permissions = permissions;
				this.isDirty = true;
			}
		}

		public void TraceTree()
		{
			AirSyncDiagnostics.TraceError(ExTraceGlobals.AlgorithmTracer, this, "Inconsistency found in FolderTree.  Dumping contents of tree:");
			foreach (KeyValuePair<ISyncItemId, FolderTree.FolderInfo> keyValuePair in this.folderTree)
			{
				FolderTree.FolderInfo value = keyValuePair.Value;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < value.Children.Count; i++)
				{
					stringBuilder.Append(value.Children[i]);
					if (i + 1 < value.Children.Count)
					{
						stringBuilder.Append(", ");
					}
				}
				AirSyncDiagnostics.TraceError(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncId: {0}, ParentSyncId: {1}, Hidden: {2}, HiddenDueToParent: {3}, Children: {4}", new object[]
				{
					keyValuePair.Key,
					value.ParentId,
					value.Hidden,
					value.HiddenDueToParent,
					stringBuilder
				});
			}
		}

		public void UnlinkChild(ISyncItemId parentId, ISyncItemId childId)
		{
			FolderTree.FolderInfo folderInfo = this.GetFolderInfo(parentId);
			FolderTree.FolderInfo folderInfo2 = this.GetFolderInfo(childId);
			if (!parentId.Equals(folderInfo2.ParentId) || !folderInfo.Children.Contains(childId))
			{
				AirSyncDiagnostics.TraceError<ISyncItemId, ISyncItemId, ISyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "Tried to unlink a child folder from a folder that was not its parent.  ChildSyncId: {0}, child's ParentSyncId: {1}, ParentSyncId passed in: {2}", childId, folderInfo2.ParentId, parentId);
				this.TraceTree();
				return;
			}
			if (folderInfo.Children.Remove(childId))
			{
				this.GetFolderInfo(childId).ParentId = null;
				this.isDirty = true;
			}
		}

		public ICustomSerializable BuildObject()
		{
			return new FolderTree();
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderTree.FolderInfo> genericDictionaryData = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderTree.FolderInfo>();
			genericDictionaryData.DeserializeData(reader, componentDataPool);
			this.folderTree = genericDictionaryData.Data;
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderTree.FolderInfo>(this.folderTree).SerializeData(writer, componentDataPool);
		}

		public IEnumerator GetEnumerator()
		{
			return this.folderTree.Keys.GetEnumerator();
		}

		private FolderTree.FolderInfo GetFolderInfo(ISyncItemId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (!this.folderTree.ContainsKey(folderId))
			{
				this.TraceTree();
				AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, null, false);
				if (Command.CurrentCommand.ProtocolLogger != null)
				{
					string text = string.Format("{0}InvalidFolderIdException", Command.CurrentCommand.Request.CommandType);
					Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, text);
					ex.ErrorStringForProtocolLogger = text;
				}
				else
				{
					ex.ErrorStringForProtocolLogger = "BadFolderIdInFolderTree";
				}
				throw ex;
			}
			return this.folderTree[folderId];
		}

		private bool RemoveFolderAndChildren(ISyncItemId folderId, FolderIdMapping folderIdMapping, int foldersSeen, int numberOfFoldersInTree)
		{
			FolderTree.FolderInfo folderInfo = this.GetFolderInfo(folderId);
			foldersSeen++;
			if (foldersSeen > numberOfFoldersInTree)
			{
				AirSyncDiagnostics.TraceError<int>(ExTraceGlobals.AlgorithmTracer, this, "Error: Loop detected in folder tree.  NumberOfFoldersInTree: {0}", numberOfFoldersInTree);
				this.TraceTree();
				return false;
			}
			int num = 0;
			while (folderInfo.Children.Count > num)
			{
				ISyncItemId syncItemId = folderInfo.Children[num];
				if (this.folderTree.ContainsKey(syncItemId))
				{
					if (!this.RemoveFolderAndChildren(syncItemId, folderIdMapping, foldersSeen, numberOfFoldersInTree))
					{
						return false;
					}
				}
				else
				{
					num++;
					MailboxSyncItemId mailboxSyncItemId = syncItemId as MailboxSyncItemId;
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.AlgorithmTracer, this, "FolderTree.RemoveFolderAndChildren could not find child folder: {0} to remove.", (mailboxSyncItemId == null) ? "<folder is not a MailboxSyncItemId>" : mailboxSyncItemId.ToString());
				}
			}
			if (folderIdMapping.Contains(folderId))
			{
				folderIdMapping.Delete(new ISyncItemId[]
				{
					folderId
				});
			}
			this.RemoveFolder(folderId);
			return true;
		}

		private static PropertyDefinition[] fetchProperties = new PropertyDefinition[]
		{
			MessageItemSchema.FolderId,
			StoreObjectSchema.ParentItemId,
			FolderSchema.IsHidden,
			FolderSchema.ExtendedFolderFlags
		};

		private static ushort typeId;

		private Dictionary<ISyncItemId, FolderTree.FolderInfo> folderTree;

		private bool isDirty;

		private enum FetchPropertiesEnum
		{
			Id,
			ParentId,
			IsHidden,
			ExtendedFolderFlags
		}

		private sealed class FolderInfo : ICustomSerializableBuilder, ICustomSerializable
		{
			public FolderInfo()
			{
				this.Children = new List<ISyncItemId>(0);
			}

			public ISyncItemId ParentId { get; set; }

			public List<ISyncItemId> Children { get; private set; }

			public bool Hidden
			{
				get
				{
					return this.hidden || this.HiddenDueToParent;
				}
			}

			public bool HiddenDueToParent { get; set; }

			public string Owner { get; set; }

			public SyncPermissions Permissions { get; set; }

			public bool IsSharedFolder
			{
				get
				{
					return !string.IsNullOrEmpty(this.Owner);
				}
			}

			public ushort TypeId
			{
				get
				{
					return FolderTree.FolderInfo.typeId;
				}
				set
				{
					FolderTree.FolderInfo.typeId = value;
				}
			}

			public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
			{
				DerivedData<ISyncItemId> derivedData = new DerivedData<ISyncItemId>();
				derivedData.DeserializeData(reader, componentDataPool);
				this.ParentId = derivedData.Data;
				GenericListData<DerivedData<ISyncItemId>, ISyncItemId> genericListData = new GenericListData<DerivedData<ISyncItemId>, ISyncItemId>();
				genericListData.DeserializeData(reader, componentDataPool);
				this.Children = genericListData.Data;
				BooleanData booleanDataInstance = componentDataPool.GetBooleanDataInstance();
				booleanDataInstance.DeserializeData(reader, componentDataPool);
				this.hidden = booleanDataInstance.Data;
				booleanDataInstance.DeserializeData(reader, componentDataPool);
				this.HiddenDueToParent = booleanDataInstance.Data;
				Int32Data int32DataInstance = componentDataPool.GetInt32DataInstance();
				int32DataInstance.DeserializeData(reader, componentDataPool);
				this.Permissions = (SyncPermissions)int32DataInstance.Data;
				StringData stringDataInstance = componentDataPool.GetStringDataInstance();
				stringDataInstance.DeserializeData(reader, componentDataPool);
				this.Owner = stringDataInstance.Data;
			}

			public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
			{
				DerivedData<ISyncItemId> derivedData = new DerivedData<ISyncItemId>(this.ParentId);
				derivedData.SerializeData(writer, componentDataPool);
				GenericListData<DerivedData<ISyncItemId>, ISyncItemId> genericListData = new GenericListData<DerivedData<ISyncItemId>, ISyncItemId>(this.Children);
				genericListData.SerializeData(writer, componentDataPool);
				componentDataPool.GetBooleanDataInstance().Bind(this.hidden).SerializeData(writer, componentDataPool);
				componentDataPool.GetBooleanDataInstance().Bind(this.HiddenDueToParent).SerializeData(writer, componentDataPool);
				componentDataPool.GetInt32DataInstance().Bind((int)this.Permissions).SerializeData(writer, componentDataPool);
				componentDataPool.GetStringDataInstance().Bind(this.Owner).SerializeData(writer, componentDataPool);
			}

			public ICustomSerializable BuildObject()
			{
				return new FolderTree.FolderInfo();
			}

			public void AddChild(ISyncItemId childId, FolderTree fullFolderTree, HashSet<ISyncItemId> visibilityChangedList)
			{
				this.Children.Add(childId);
				fullFolderTree.SetHiddenDueToParent(childId, this.Hidden, visibilityChangedList);
			}

			public bool SetHidden(bool newValue, FolderTree fullFolderTree, HashSet<ISyncItemId> visibilityChangedList)
			{
				bool result = false;
				if (this.ParentId == null && this.HiddenDueToParent)
				{
					this.HiddenDueToParent = false;
					result = true;
				}
				if (this.hidden != newValue)
				{
					this.hidden = newValue;
					foreach (ISyncItemId syncItemId in this.Children)
					{
						if (visibilityChangedList != null)
						{
							visibilityChangedList.Add(syncItemId);
						}
						fullFolderTree.SetHiddenDueToParent(syncItemId, newValue || this.HiddenDueToParent, visibilityChangedList);
					}
					result = true;
				}
				return result;
			}

			public bool SetHiddenDueToParent(bool newValue, FolderTree fullFolderTree, HashSet<ISyncItemId> visibilityChangedList)
			{
				if (this.HiddenDueToParent != newValue)
				{
					this.HiddenDueToParent = newValue;
					if (!this.hidden)
					{
						foreach (ISyncItemId syncItemId in this.Children)
						{
							if (visibilityChangedList != null)
							{
								visibilityChangedList.Add(syncItemId);
							}
							fullFolderTree.SetHiddenDueToParent(syncItemId, newValue, visibilityChangedList);
						}
					}
					return true;
				}
				return false;
			}

			private static ushort typeId;

			private bool hidden;
		}
	}
}
