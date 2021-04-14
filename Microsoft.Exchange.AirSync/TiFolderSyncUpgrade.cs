using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class TiFolderSyncUpgrade
	{
		public MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
			set
			{
				this.mailboxSession = value;
			}
		}

		public SyncStateStorage SyncStateStorage
		{
			get
			{
				return this.syncStateStorage;
			}
			set
			{
				this.syncStateStorage = value;
			}
		}

		public void UpdateLastFolderId(int folderId)
		{
			FolderIdMappingSyncStateInfo syncStateInfo = new FolderIdMappingSyncStateInfo();
			this.folderIdMappingSyncState = this.syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
			FolderIdMapping folderIdMapping = (FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping];
			folderIdMapping.IncreaseCounterTo((long)folderId);
			this.folderIdMappingSyncState.Commit();
		}

		public Dictionary<string, StoreObjectId> Upgrade(Dictionary<string, FolderNode> folders, string synckeyIn, DeviceIdentity deviceIdentityIn, uint version, out Dictionary<string, StoreObjectType> contentTypeTable)
		{
			if (synckeyIn == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.TiUpgradeTracer, this, "Input to TiFolderSyncUpgrade.Upgrade is null: field synckeyIn");
				throw new ArgumentNullException("synckeyIn");
			}
			if (deviceIdentityIn == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.TiUpgradeTracer, this, "Input to TiFolderSyncUpgrade.Upgrade is null: field deviceIdentityIn");
				throw new ArgumentNullException("deviceIdentityIn");
			}
			this.deviceIdentity = deviceIdentityIn;
			this.folderIdMappingSyncState = null;
			Dictionary<string, StoreObjectId> dictionary = new Dictionary<string, StoreObjectId>();
			try
			{
				this.LoadE12SyncState();
				this.folderHierarchySync = this.syncState.GetFolderHierarchySync();
				FolderSyncUpgrade folderSyncUpgrade = new FolderSyncUpgrade(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root));
				dictionary = folderSyncUpgrade.Upgrade(this.folderHierarchySync, this.syncState, folders, out contentTypeTable);
				string text = synckeyIn;
				int num = text.LastIndexOf("}");
				if (num >= 0)
				{
					text = text.Substring(num + 1, text.Length - num - 1);
				}
				if (text.Length > 0)
				{
					int data = int.Parse(text, CultureInfo.InvariantCulture);
					this.syncState[CustomStateDatumType.SyncKey] = new Int32Data(data);
				}
				this.UpdateMapping(dictionary);
				AirSyncSyncStateTypeFactory.EnsureSyncStateTypesRegistered();
				this.syncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data((int)version);
				this.folderIdMappingSyncState.Commit();
				if (folders != null)
				{
					this.syncState.Commit();
				}
				contentTypeTable["EmailSyncFile"] = StoreObjectType.Folder;
				contentTypeTable["ContactsSyncFile"] = StoreObjectType.ContactsFolder;
				contentTypeTable["CalendarSyncFile"] = StoreObjectType.CalendarFolder;
				contentTypeTable["TasksSyncFile"] = StoreObjectType.TasksFolder;
			}
			finally
			{
				if (this.syncState != null)
				{
					this.syncState.Dispose();
				}
			}
			return dictionary;
		}

		public void Close()
		{
			if (this.mailboxSession != null)
			{
				this.mailboxSession = null;
			}
			if (this.syncStateStorage != null)
			{
				this.syncStateStorage.Dispose();
				this.syncStateStorage = null;
			}
		}

		private void LoadE12SyncState()
		{
			FolderIdMappingSyncStateInfo folderIdMappingSyncStateInfo = new FolderIdMappingSyncStateInfo();
			this.syncStateStorage = SyncStateStorage.Create(this.mailboxSession, this.deviceIdentity, StateStorageFeatures.ContentState, true, null);
			this.syncStateStorage.DeleteAllSyncStates();
			this.folderIdMappingSyncState = this.syncStateStorage.GetCustomSyncState(folderIdMappingSyncStateInfo, new PropertyDefinition[0]);
			if (this.folderIdMappingSyncState == null || this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] == null)
			{
				this.syncStateStorage.DeleteFolderHierarchySyncState();
				this.folderIdMappingSyncState = this.syncStateStorage.CreateCustomSyncState(folderIdMappingSyncStateInfo);
				this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] = new FolderIdMapping();
			}
			else
			{
				this.syncStateStorage.DeleteFolderHierarchySyncState();
			}
			this.folderIdMappingSyncState[SyncStateProp.Version] = new NullableData<Int32Data, int>(new int?(2));
			folderIdMappingSyncStateInfo.HandleSyncStateVersioning(this.folderIdMappingSyncState);
			this.syncState = this.syncStateStorage.CreateFolderHierarchySyncState();
			this.syncState.CustomVersion = new int?(2);
		}

		private void UpdateMapping(Dictionary<string, StoreObjectId> mapping)
		{
			FolderIdMapping folderIdMapping = (FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping];
			Dictionary<StoreObjectId, FolderStateEntry> data = ((GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderStateEntry>)this.syncState[SyncStateProp.ClientState]).Data;
			foreach (string text in mapping.Keys)
			{
				StoreObjectId storeObjectId = mapping[text];
				folderIdMapping.Add(MailboxSyncItemId.CreateForNewItem(storeObjectId), text);
				if (data.ContainsKey(storeObjectId))
				{
					FolderStateEntry folderStateEntry = data[storeObjectId];
					if (folderStateEntry.ChangeKey.Length == 1 && folderStateEntry.ChangeKey[0] == 1)
					{
						folderStateEntry.ChangeTrackingHash = "blah".GetHashCode();
					}
					else
					{
						folderStateEntry.ChangeTrackingHash = FolderCommand.ComputeChangeTrackingHash(this.mailboxSession, mapping[text], null);
					}
				}
			}
			folderIdMapping.CommitChanges();
			StoreObjectId defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			mapping.Add("EmailSyncFile", defaultFolderId);
			defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts);
			mapping.Add("ContactsSyncFile", defaultFolderId);
			defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			mapping.Add("CalendarSyncFile", defaultFolderId);
			defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks);
			mapping.Add("TasksSyncFile", defaultFolderId);
		}

		private MailboxSession mailboxSession;

		private SyncStateStorage syncStateStorage;

		private FolderHierarchySyncState syncState;

		private FolderHierarchySync folderHierarchySync;

		private CustomSyncState folderIdMappingSyncState;

		private DeviceIdentity deviceIdentity;
	}
}
